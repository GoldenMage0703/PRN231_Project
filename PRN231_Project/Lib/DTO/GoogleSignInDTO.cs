using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO
{
	public class GoogleSignInDTO
	{
		public string GoogleIdToken { get; set; } // Mã token ID từ Google
		public string Email { get; set; } 
		public string DisplayName { get; set; } 
	}
}
