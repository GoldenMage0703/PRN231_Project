using Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Lib.DTO;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers.User
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

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            // Truy xuất tất cả người dùng từ cơ sở dữ liệu
            var users = await _context.Users
                .Select(user => new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    Role = user.Role,
                    Status = user.Status,
                    Created = user.Created
                }).ToListAsync();

            // Trả về danh sách người dùng dưới dạng JSON
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            // Truy xuất người dùng từ cơ sở dữ liệu theo ID
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(user => new UserDTO
                {
                    Username = user.Username,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    Role = user.Role,
                    Status = user.Status
                })
                .FirstOrDefaultAsync();

            // Nếu không tìm thấy người dùng, trả về NotFound
            if (user == null)
            {
                return NotFound();
            }

            // Trả về thông tin người dùng dưới dạng JSON
            return Ok(user);
        }

        [HttpPut("update/{id}")]
		public async Task<IActionResult> UpdateUserProfile(int id, EditProfileDTO editProfileDTO)
		{
			var user = await _context.Users.FindAsync(id);

			if (user == null)
			{
				return NotFound("User not found.");
			}

			user.Username = editProfileDTO.Username;
			user.Password = editProfileDTO.Password;
			user.Email = editProfileDTO.Email;
			user.DisplayName = editProfileDTO.DisplayName;
			user.Role = editProfileDTO.Role;
            user.Status = editProfileDTO.Status;

            _context.Users.Update(user);
			await _context.SaveChangesAsync();

			return Ok("Profile updated successfully.");
		}

        [HttpPut("updateAdmin/{id}")]
        public async Task<IActionResult> UpdateUserProfileAdmin(int id, EditProfileDTO editProfileDTO)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Role = editProfileDTO.Role;
            user.Status = editProfileDTO.Status;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Profile updated successfully.");
        }

    }
}
