using Google.Apis.Auth;
using Lib.DTO;
using Lib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BE.Controllers.Login
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginGoogleController : ControllerBase
	{
		private readonly PRN231_ProjectContext _context;
		private readonly IConfiguration _config; // Đảm bảo bạn có IConfiguration để lấy ClientId

		public LoginGoogleController(PRN231_ProjectContext context, IConfiguration config)
		{
			_context = context;
			_config = config;
		}

		private UserRegisterDTO ConvertToUserDTO(Lib.Models.User user)
		{
			return new UserRegisterDTO
            {
				Username = user.Username,
				Password = user.Password,
				Email = user.Email,
				DisplayName = user.DisplayName,
				Role = user.Role // Chuyển đổi thuộc tính Role nếu cần
			};
		}
		private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken)
		{
			var settings = new GoogleJsonWebSignature.ValidationSettings()
			{
				Audience = new[] { _config["Google:ClientId"] } // Địa chỉ ClientId từ Google
			};

			return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
		}
		private string GenerateRandomPassword(int length = 8)
		{
			const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			using (var rng = new RNGCryptoServiceProvider())
			{
				var res = new char[length];
				var uint32Buffer = new byte[4];

				for (int i = 0; i < length; i++)
				{
					rng.GetBytes(uint32Buffer);
					int index = (int)(BitConverter.ToUInt32(uint32Buffer, 0) % (uint)valid.Length); // Chuyển đổi thành int
					res[i] = valid[index];
				}

				return new string(res);
			}
		}

		private string GenerateVerificationCode(int length = 6)
		{
			Random random = new Random();
			int minValue = (int)Math.Pow(10, length - 1);
			int maxValue = (int)Math.Pow(10, length) - 1; // Đảm bảo giá trị tối đa là int

			return random.Next(minValue, maxValue + 1).ToString(); // Cộng thêm 1 để bao gồm giá trị tối đa
		}


		private async Task SendVerificationCodeAsync(string email, string verificationCode)
		{
			var smtpServer = _config["EmailSettings:SmtpServer"];
			var smtpPort = int.Parse(_config["EmailSettings:Port"]);
			var senderEmail = _config["EmailSettings:SenderEmail"];
			var senderPassword = _config["EmailSettings:SenderPassword"];

			using (var client = new SmtpClient(smtpServer, smtpPort))
			{
				client.Credentials = new NetworkCredential(senderEmail, senderPassword);
				client.EnableSsl = true;

				var mailMessage = new MailMessage
				{
					From = new MailAddress(senderEmail),
					Subject = "Your Verification Code",
					Body = $"Your verification code is: {verificationCode}",
					IsBodyHtml = false,
				};

				mailMessage.To.Add(email);

				try
				{
					await client.SendMailAsync(mailMessage);
				}
				catch (Exception ex)
				{
					// Xử lý ngoại lệ khi gửi email thất bại
					Console.WriteLine($"Error sending email: {ex.Message}");
				}
			}
		}

		private string GenerateToken(Lib.Models.User user)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Username),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Role, user.Role.ToString())
			};

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		[HttpPost("GoogleSignIn")]
		public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInDTO googleSignInDTO)
		{
			var payload = ValidateGoogleToken(googleSignInDTO.GoogleIdToken);
			if (payload == null)
			{
				return Unauthorized("Invalid Google token.");
			}

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == googleSignInDTO.Email);

			if (user == null)
			{
				user = new Lib.Models.User
				{
					Username = googleSignInDTO.DisplayName,
					Email = googleSignInDTO.Email,
					Password = GenerateRandomPassword(),
					DisplayName = googleSignInDTO.DisplayName,
					Role = 3
				};

				await _context.Users.AddAsync(user);
				await _context.SaveChangesAsync();

				// Gửi mã xác thực
				string verificationCode = GenerateVerificationCode();
				await SendVerificationCodeAsync(googleSignInDTO.Email, verificationCode);
			}

			// Tạo và trả về JWT token
			var token = GenerateToken(user);
			var userDTO = ConvertToUserDTO(user);
			return Ok(new { token, user = userDTO });
		}


	}
}
