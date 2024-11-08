using Lib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BE.Controllers.CoursesAttempt
{
	[Route("api/[controller]")]
	[ApiController]
	public class CoursesAttemptController : ControllerBase
	{
		private readonly PRN231_ProjectContext _context;

		public CoursesAttemptController(PRN231_ProjectContext context)
		{
			_context = context;
		}

		[HttpGet]
		[Route("GetUserAttemptByCourseId/{courseId}")]
		public async Task<IActionResult> GetUserAttemptByCourseId(int courseId)
		{
			var userAttempt = await _context.CourseAttempts
				.Where(ua => ua.CourseId == courseId)
				.Select(ua => new
				{
					ua.UserId,
					ua.Status
				})
				.FirstOrDefaultAsync();

			if (userAttempt == null)
			{
				return NotFound("Không tìm thấy thông tin tham gia của người dùng cho khóa học này.");
			}

			var userIdCookieOptions = new CookieOptions
			{
				Expires = DateTime.Now.AddHours(1), // Hạn sử dụng 1 giờ
				HttpOnly = true,
				Secure = true
			};
			var statusCookieOptions = new CookieOptions
			{
				Expires = DateTime.Now.AddHours(1),
				HttpOnly = true,
				Secure = true
			};

			Response.Cookies.Append("UserId", userAttempt.UserId.ToString(), userIdCookieOptions);
			Response.Cookies.Append("Status", userAttempt.Status.ToString(), statusCookieOptions);

			return Ok(new { Message = "Đã lưu thông tin vào cookie." });
		}

		[HttpGet]
		[Route("GetUserAttemptFromCookies")]
		public IActionResult GetUserAttemptFromCookies()
		{
			var userId = Request.Cookies["UserId"];
			var status = Request.Cookies["Status"];

			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(status))
			{
				return NotFound("Không tìm thấy thông tin trong cookie.");
			}

			return Ok(new { UserId = userId, Status = status });
		}

		[HttpPut]
		[Route("UpdateUserAttemptStatus/{courseId}/{userId}")]
		public async Task<IActionResult> UpdateUserAttemptStatus(int courseId, int userId, [FromBody] int newStatus)
		{
			var userAttempt = await _context.CourseAttempts
				.FirstOrDefaultAsync(ua => ua.CourseId == courseId && ua.UserId == userId);

			if (userAttempt == null)
			{
				return NotFound("Không tìm thấy thông tin tham gia của người dùng cho khóa học này.");
			}

			userAttempt.Status = newStatus;
			await _context.SaveChangesAsync();

			// Cập nhật Status trong cookie
			var statusCookieOptions = new CookieOptions
			{
				Expires = DateTime.Now.AddHours(1),
				HttpOnly = true,
				Secure = true
			};
			Response.Cookies.Append("Status", newStatus.ToString(), statusCookieOptions);

			return Ok("Trạng thái tham gia đã được cập nhật thành công.");
		}
	}
}