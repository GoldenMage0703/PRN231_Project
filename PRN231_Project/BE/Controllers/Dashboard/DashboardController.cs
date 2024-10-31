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
