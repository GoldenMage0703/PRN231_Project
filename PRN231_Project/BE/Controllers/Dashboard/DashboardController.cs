using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lib.Models;
using Lib.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit.Tnef;

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

        [HttpGet("CourseSumary")]
        public async Task<IActionResult> GetCourseSumary(int courseID)
        {
           int totalAttempts = await _context.CourseAttempts.Where(x=>x.CourseId==courseID).CountAsync();
           decimal? totalRevenue = _context.Courses.FirstOrDefault(x=>x.Id == courseID).Price * totalAttempts;
            int totalQuestion = _context.Questions.Where(x=>x.Course==courseID).Count();
            var attemptsPerMonth = _context.CourseAttempts.Where(x=>x.CourseId==courseID)
     .GroupBy(x => new { Year = x.AttemptDate.Year, Month = x.AttemptDate.Month })
     .Select(g => new
     {
         Year = g.Key.Year,
         Month = g.Key.Month,
         NumberOfAttempts = g.Count()
     })
     .OrderBy(x => x.Year)
     .ThenBy(x => x.Month)
     .ToList();
            var respone = new
            {
                totalAttempts,
                totalRevenue,
                totalQuestion,
                attemptsPerMonth

            };
            return Ok(respone);
        }

    }
}
