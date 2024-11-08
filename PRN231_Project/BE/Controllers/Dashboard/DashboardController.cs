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


        [HttpGet("Summary")]
        public async Task<IActionResult> GetSummary()
        {
            // Calculate total users
            var totalUsers = await _context.Users.CountAsync();

            // Calculate total quizzes
            var totalQuizzes = await _context.Courses.CountAsync();

            // Calculate total purchased quizzes (course attempts)
            var purchasedQuizzes = await _context.CourseAttempts.CountAsync();

            // Calculate total revenue
            var totalRevenue = await _context.Bills.SumAsync(b => b.TotalPayment);

            // Return the results as a combined summary object
            var summary = new
            {
                totalUsers = totalUsers,
                totalQuizzes = totalQuizzes,
                purchasedQuizzes = purchasedQuizzes,
                totalRevenue = totalRevenue
            };

            return Ok(summary);
        }

    }
}
