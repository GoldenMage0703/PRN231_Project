using Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BE.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RegisterController : ControllerBase
	{
		private readonly PRN231_ProjectContext _context;
		private readonly ILogger<RegisterController> _logger; // Thêm ILogger

		public RegisterController(PRN231_ProjectContext context, ILogger<RegisterController> logger)
		{
			_context = context;
			_logger = logger; // Khởi tạo logger
		}

		[HttpPost]
		public async Task<IActionResult> Register(UserRegisterDTO request)
		{
			// Kiểm tra email đã tồn tại
			if (_context.Users.Any(u => u.Email == request.Email))
			{
				_logger.LogWarning("Attempt to register with existing email: {Email}", request.Email);
				return BadRequest("Email already exists.");
			}

			// Băm mật khẩu
			var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

			var newUser = new User
			{
				DisplayName = request.DisplayName,
				Email = request.Email,
				Username = request.Username,
				Password = hashedPassword, // Lưu mật khẩu đã mã hóa
				Role = 3, //User
				Created = DateTime.Now
			};

			_context.Users.Add(newUser);
			await _context.SaveChangesAsync();

			_logger.LogInformation("New user registered with email: {Email}", request.Email);

			return Ok(new
			{
				message = "User registered successfully.",
				userId = newUser.Id,
				displayName = newUser.DisplayName,
				role = GetUserRole(newUser.Role)
			});
		}
		private string GetUserRole(int role)
		{
			return role switch
			{
				1 => "Admin",   // Admin
				2 => "Teacher", // Teacher
				3 => "User",    // User
				_ => "User"     // Default to User
			};
		}
	}
}
