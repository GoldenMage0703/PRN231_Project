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

		[HttpGet]
		public async Task<ActionResult<IEnumerable<BillDTO>>> GetBills()
		{
			return await _context.Bills
				.Select(b => new BillDTO
				{
					Id = b.Id,
					UserId = b.UserId,
					TotalPayment = b.TotalPayment
				}).ToListAsync();
		}

		[HttpPost]
		public async Task<ActionResult<BillDTO>> CreateBill(BillDTO billDTO)
		{
			var bill = new Lib.Models.Bill
			{
				UserId = billDTO.UserId,
				TotalPayment = billDTO.TotalPayment
			};

			_context.Bills.Add(bill);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetBills), new { id = bill.Id }, billDTO);
		}
	}
}
