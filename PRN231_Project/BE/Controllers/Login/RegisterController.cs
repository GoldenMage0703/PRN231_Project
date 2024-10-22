using Lib.DTO;
using Lib.Models;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers.Login
{
	[Route("api/[controller]")]
	[ApiController]
	public class RegisterController : ControllerBase
	{
		private readonly PRN231_ProjectContext _context;

		public RegisterController(PRN231_ProjectContext context)
		{
			_context = context;
		}

		[HttpPost]
		public IActionResult Register(UserRegisterDTO userRegister)
		{
			// Tạo người dùng mới từ DTO
			var user = new User
			{
				Username = userRegister.Username,
				Password = userRegister.Password,  // Lưu mật khẩu trực tiếp
				DisplayName = userRegister.DisplayName,
				Email = userRegister.Email,

                Role = 3
			};

			// Lưu người dùng vào cơ sở dữ liệu
			_context.Users.Add(user);
			_context.SaveChanges();

			return Ok("User registered successfully.");
		}
	}
}
