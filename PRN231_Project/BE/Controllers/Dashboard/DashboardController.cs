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
	public class DashboardController : ControllerBase
	{
		private readonly PRN231_ProjectContext _context;

		public DashboardController(PRN231_ProjectContext context)
		{
			_context = context;
		}


		[HttpGet]
		[Route("GetUser")]
		public async Task<ActionResult<IEnumerable<UserRegisterDTO>>> GetUsers()
		{
			// Truy xuất tất cả người dùng từ cơ sở dữ liệu
			var users = await _context.Users
				.Select(user => new UserDTO
				{
					Username = user.Username,
					Email = user.Email,
					DisplayName = user.DisplayName,
					Role = user.Role
				}).ToListAsync();

			// Trả về danh sách người dùng dưới dạng JSON
			return Ok(users);
		}

		private bool UserExists(int id)
		{
			return _context.Users.Any(e => e.Id == id);
		}
		[HttpPost]
		[Route("CreateNewUser")]
		public async Task<ActionResult<UserDTO>> CreateUser(UserRegisterDTO userDTO)
		{
			var user = new Lib.Models.User
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
		[HttpDelete]
		[Route("DeleteUser")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var user = await _context.Users.FindAsync(id);
			if(user == null)
			{
				return NotFound(new {message = "User not Found"});
			}
			_context.Users.Remove(user);
			await _context.SaveChangesAsync();
			return Ok(new {message = "Delete successfully"});
		}

		[HttpPut]
		[Route("UpdateUser")]
		public async Task<IActionResult> UpdateUser(int id, UserRegisterDTO userDTO)
		{
			// Tìm kiếm người dùng theo id
			var user = await _context.Users.FindAsync(id);

			if (user == null)
			{
				return NotFound("User not found.");
			}

			user.Username = userDTO.Username;
			user.Password = userDTO.Password; 
			user.Email = userDTO.Email;
			user.DisplayName = userDTO.DisplayName;
			user.Role = userDTO.Role;

			_context.Entry(user).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!UserExists(id))
				{
					return NotFound("User not found.");
				}
				else
				{
					throw;
				}
			}

			return Ok(new { message = "User updated successfully." });
		}

		[HttpGet("TotalRevenue")]
		public async Task<ActionResult<decimal>> GetTotalRevenue()
		{
			// Tính tổng tiền đã thu được từ hóa đơn
			var totalRevenue = await _context.Bills.SumAsync(b => b.TotalPayment);
			return Ok(totalRevenue);
		}

		[HttpGet("TotalRegistrations")]
		public async Task<IActionResult> GetTotalRegistrations()
		{
			var totalRegistrations = await _context.CourseAttempts
				.CountAsync(); // Đếm số lượng bản ghi trong CourseAttempt

			return Ok(new { TotalRegistrations = totalRegistrations });
		}
	}
}
