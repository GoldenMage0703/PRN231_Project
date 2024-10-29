using Lib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Lib.DTO;

namespace BE.Controllers.User
{
	[Route("api/[controller]")]
	[ApiController]
	public class EditProfileController : ControllerBase
	{
		private readonly PRN231_ProjectContext _context;

		public EditProfileController(PRN231_ProjectContext context)
		{
			_context = context;
		}

		[HttpPut("{id}")]
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

			_context.Users.Update(user);
			await _context.SaveChangesAsync();

			return Ok("Profile updated successfully.");
		}
	}
}
