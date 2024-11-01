using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO.Password
{
	public class PasswordResetRequest
	{
		public int Id { get; set; }
		public string UserEmail { get; set; }
		public string ResetToken { get; set; }
		public DateTime Expiration { get; set; } // Có thể thêm thời gian hết hạn cho token
	}

}
