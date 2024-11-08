using Lib.DTO;
using Lib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers.Bill
{
	[Route("api/[controller]")]
	[ApiController]
	public class BillController : ControllerBase
	{
		private readonly PRN231_ProjectContext _context;

		public BillController(PRN231_ProjectContext context)
		{
			_context = context;
		}

		[HttpGet("GetAllBill")]
		public async Task<ActionResult> GetBills()
		{
			var respone  = await _context.Bills
                .Select(b => new
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    TotalPayment = b.TotalPayment,
                    BillDetail = b.Courses.Select(x => new
                    {
                        CourseName = x.CourseName,
						Price = x.Price
                    }).ToList()
                }).ToListAsync();
            return Ok(respone);
		}


        [HttpGet("GetUserBill")]
        public async Task<ActionResult> GetBills(int userID)
        {
            var respone = await _context.Bills.Where(bill=>bill.UserId==userID)
                .Select(b => new
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    TotalPayment = b.TotalPayment,
                    BillDetail = b.Courses.Select(x => new
                    {
                        CourseName = x.CourseName,
                        Price = x.Price
                    }).ToList()
                }).ToListAsync();
            return Ok(respone);
        }
    }
}
