using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Thêm using cho ILogger
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lib.Models;
using BE.DTOs;

namespace BE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly PRN231_ProjectContext _context;
		private readonly IConfiguration _configuration;
		private readonly ILogger<LoginController> _logger; // Thêm ILogger

		public LoginController(PRN231_ProjectContext context, IConfiguration configuration, ILogger<LoginController> logger)
		{
			_context = context;
			_configuration = configuration;
			_logger = logger; // Khởi tạo logger
		}

		[HttpPost]
		public async Task<IActionResult> Login(UserLoginDTO request)
		{
			var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);

			if (user == null)
			{
				_logger.LogWarning("Login attempt failed for email: {Email}", request.Email);
				return Unauthorized("Invalid email or password.");
			}

			if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
			{
				_logger.LogWarning("Invalid password attempt for email: {Email}", request.Email);
				return Unauthorized("Invalid email or password.");
			}

			// Generate JWT Token
			var token = CreateJwtToken(user);

			_logger.LogInformation("User {Username} logged in successfully.", user.Username); // Ghi log khi đăng nhập thành công

			return Ok(new
			{
				token,
				userId = user.Id,
				displayName = user.DisplayName,
				role = GetUserRole(user.Role)
			});
		}

		private string CreateJwtToken(User user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Role, GetUserRole(user.Role))
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:a_very_long_and_random_string_12345"])); // Sửa đây
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:abc"],
				audience: _configuration["Jwt:abc"],
				claims: claims,
				expires: DateTime.Now.AddHours(1),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		// Assign role based on the Role ID from the user table
		private string GetUserRole(int role)
		{
			return role switch
			{
				1 => "Admin",  // Admin
				2 => "Teacher", // Teacher
				3 => "User",    // User
				_ => "User"     // Default to User
			};
		}

		// Phương thức thêm người dùng mới với mật khẩu đã mã hóa
		//[HttpPost("register")]
		//public async Task<IActionResult> Register(UserRegisterDTO request)
		//{
		//	var existingUser = _context.Users.SingleOrDefault(u => u.Email == request.Email);
		//	if (existingUser != null)
		//	{
		//		return BadRequest("Email is already registered.");
		//	}

		//	var newUser = new User
		//	{
		//		DisplayName = request.DisplayName,
		//		Email = request.Email,
		//		Username = request.Username,
		//		Password = BCrypt.Net.BCrypt.HashPassword(request.Password), // Mã hóa mật khẩu
		//		Role = 3, // Giả sử người dùng mới mặc định là User
		//		Created = DateTime.Now
		//	};

		//	_context.Users.Add(newUser);
		//	await _context.SaveChangesAsync();

		//	return Ok("User registered successfully.");
		//}
	}
}
