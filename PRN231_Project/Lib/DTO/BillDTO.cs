using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO
{
	public class BillDTO
	{
		public int Id { get; set; } 
		public int UserId { get; set; }
		public decimal TotalPayment { get; set; } 
	}
}
