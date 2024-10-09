using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lib.Models;
using Lib.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly PRN231_ProjectContext _context;

		public UserController(PRN231_ProjectContext context)
		{
			_context = context;
		}
		[HttpPost]
		public async Task<ActionResult<UserDTO>> CreateUser(UserDTO userDTO)
		{
			// Ánh xạ UserDTO thành thực thể User
			var user = new User
			{
				Username = userDTO.Username,
				Password = userDTO.Password,
				Email = userDTO.Email,
				DisplayName = userDTO.DisplayName,
				Role = userDTO.Role
			};
			if (_context.Users.Any(u => u.Username == user.Username))
			{
				return Conflict("User already exists.");
			}

			_context.Users.Add(user);
			await _context.SaveChangesAsync(); // Đảm bảo gọi await để lưu thay đổi

			return CreatedAtAction(nameof(CreateUser), new { id = user.Id }, user);
		}
		
	}
}
