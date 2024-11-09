using Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    ua.Status,
                    User = _context.Users
                        .Where(u => u.Id == ua.UserId)
                        .Select(u => new
                        {
                            u.Email,
                            u.DisplayName,
                            u.Role
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (userAttempt == null)
            {
                return NotFound("Không tìm thấy thông tin tham gia của người dùng cho khóa học này.");
            }

            return Ok(new
            {
                UserId = userAttempt.UserId,
                Status = userAttempt.Status,
                User = userAttempt.User
            });
        }

        [HttpPut]
        [Route("UpdateUserAttemptStatus/{courseId}/{userId}")]
        public async Task<IActionResult> UpdateUserAttemptStatus(int courseId, int userId, [FromBody] UpdateStatusRequest request)
        {
            var userAttempt = await _context.CourseAttempts
                .FirstOrDefaultAsync(ua => ua.CourseId == courseId && ua.UserId == userId);

            if (userAttempt == null)
            {
                return NotFound("Không tìm thấy thông tin tham gia của người dùng cho khóa học này.");
            }

            userAttempt.Status = request.Status;
            await _context.SaveChangesAsync();

            return Ok("Trạng thái tham gia đã được cập nhật thành công.");
        }
    }

    public class UpdateStatusRequest
    {
        public int Status { get; set; }
    }
}
