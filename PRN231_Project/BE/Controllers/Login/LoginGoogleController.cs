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

		[HttpPost]
		[Route("GoogleSignIn")]
		public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInDTO googleSignInDTO)
		{
			if (string.IsNullOrEmpty(googleSignInDTO.GoogleIdToken))
			{
				return BadRequest("GoogleIdToken is required.");
			}

			GoogleJsonWebSignature.Payload payload;
			try
			{
				payload = await ValidateGoogleToken(googleSignInDTO.GoogleIdToken);
			}
			catch (Exception ex)
			{
				return BadRequest($"Error validating Google token: {ex.Message}");
			}

			if (payload == null)
			{
				return Unauthorized("Invalid Google token.");
			}

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

			if (user == null)
			{
				// Tạo tài khoản mới nếu chưa tồn tại
				user = new Lib.Models.User
				{
					Username = payload.Name,
					Email = payload.Email,
					DisplayName = payload.Name,
					Role = 3
				};

				await _context.Users.AddAsync(user);
				await _context.SaveChangesAsync();
			}

			// Tạo và trả về JWT token
			var token = GenerateToken(user);
			return Ok(new { token, user });
		}

		private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken)
		{
			var settings = new GoogleJsonWebSignature.ValidationSettings()
			{
				Audience = new[] { _config["Google:ClientId"] }
			};

			return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
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
	}
}
