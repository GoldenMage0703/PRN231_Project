using Lib.DTO.Password;
using Lib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using System.Security.Cryptography;

namespace BE.Controllers.Login
{
	[Route("api/[controller]")]
	[ApiController]
	public class ForgotPasswordController : ControllerBase
	{
		private readonly PRN231_ProjectContext _context;
		private readonly Lib.DTO.Password.IEmailService _emailService;
		public ForgotPasswordController(Lib.DTO.Password.IEmailService emailService, PRN231_ProjectContext context)
		{
			_emailService = emailService;
			_context = context;
			
		}

		[HttpPost("ForgotPassword")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDTO.Email);
			if (user == null)
			{
				return NotFound("Email không tồn tại.");
			}

			var resetToken = GenerateResetToken();
			// Lưu reset token vào database hoặc cache với thời gian giới hạn
			// Để minh họa, ở đây ta không lưu token.

			string resetUrl = $"http://localhost:5001/api/ForgotPassword/reset-password?token={resetToken}";


			await _emailService.SendEmailAsync(
				forgotPasswordDTO.Email,
				"Yêu cầu đặt lại mật khẩu",
				$"Nhấn vào đây để đặt lại mật khẩu của bạn: {resetUrl}"
			);

			return Ok("Liên kết đặt lại mật khẩu đã được gửi tới email của bạn.");
		}

		private string GenerateResetToken()
		{
			var token = new byte[32];
			using (var random = RandomNumberGenerator.Create())
			{
				random.GetBytes(token);
				return Convert.ToBase64String(token);
			}
		}
	}

}
