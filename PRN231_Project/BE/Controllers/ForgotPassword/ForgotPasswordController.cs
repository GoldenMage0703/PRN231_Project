using Lib.DTO.Password;
using Lib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NETCore.MailKit.Core;
using System.Security.Cryptography;
using System.Text;

namespace BE.Controllers.Login
{
	[Route("api/[controller]")]
	[ApiController]
	public class ForgotPasswordController : ControllerBase
	{
		private readonly PRN231_ProjectContext _context;
		private readonly IMemoryCache _cache;
		private readonly Lib.DTO.Password.IEmailService _emailService;
		public ForgotPasswordController(Lib.DTO.Password.IEmailService emailService, PRN231_ProjectContext context, IMemoryCache cache)
		{
			_emailService = emailService;
			_context = context;
			_cache = cache;
		}

		[HttpPost]
		[Route("ForgotPassword")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDTO.Email);
			if (user == null)
			{
				return NotFound("Email không tồn tại.");
			}

			var resetToken = GenerateResetToken(user.Id);
			string cacheKey = $"ResetToken_{user.Id}";
			_cache.Set(cacheKey, user.Id, TimeSpan.FromHours(1));

			string resetUrl = GenerateResetUrl(resetToken);

			if (string.IsNullOrWhiteSpace(forgotPasswordDTO.Email) || !IsValidEmail(forgotPasswordDTO.Email))
			{
				return BadRequest("Địa chỉ email không hợp lệ.");
			}

			await _emailService.SendEmailAsync(
				forgotPasswordDTO.Email,
				"Yêu cầu đặt lại mật khẩu",
				$"Nhấn vào đây để đặt lại mật khẩu của bạn: {resetUrl}"
			);

			return Ok("Liên kết đặt lại mật khẩu đã được gửi tới email của bạn.");
		}
		private string GenerateResetUrl(string token)
		{
			return $"http://localhost:5001/api/ForgotPassword/reset-password?token={token}";
		}

		[HttpPost]
		[Route("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
		{
			var decoded = DecodeResetToken(resetPasswordDTO.Token);
			if (decoded == null)
			{
				return BadRequest("Token không hợp lệ hoặc đã hết hạn.");
			}

			var userId = decoded.Value.userId;
			var user = await _context.Users.FindAsync(userId);
			if (user == null)
			{
				return NotFound("Người dùng không tồn tại.");
			}

			// Cập nhật mật khẩu
			user.Password = resetPasswordDTO.NewPassword; // Đảm bảo mã hóa mật khẩu trước khi lưu
			await _context.SaveChangesAsync();

			return Ok("Mật khẩu đã được đặt lại thành công.");
		}


		private bool IsValidEmail(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}
		private string GenerateResetToken(int userId)
		{
			var expiry = DateTime.UtcNow.AddMinutes(10);
			var token = $"{userId}:{expiry.ToBinary()}"; // Tạo token với thông tin userId và thời gian hết hạn
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(token)); // Mã hóa token
		}

		private (int userId, DateTime expiry)? DecodeResetToken(string token)
		{
			try
			{
				var decodedBytes = Convert.FromBase64String(token);
				var decodedToken = Encoding.UTF8.GetString(decodedBytes).Split(':');
				var userId = int.Parse(decodedToken[0]);
				var expiry = DateTime.FromBinary(long.Parse(decodedToken[1]));

				if (expiry < DateTime.UtcNow)
				{
					return null; // Token đã hết hạn
				}

				return (userId, expiry);
			}
			catch
			{
				return null; // Lỗi khi giải mã
			}
		}
	}

}
