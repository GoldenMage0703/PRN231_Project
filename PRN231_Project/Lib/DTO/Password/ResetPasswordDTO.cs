using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO.Password
{
	public class ResetPasswordDTO
	{
		public int UserId { get; set; }
		public string Token { get; set; }
		public string NewPassword { get; set; }
	}
}
