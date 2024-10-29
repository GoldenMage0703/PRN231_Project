using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO
{
	public class EditProfileDTO
	{
		public string? Username { get; set; }
		public string? Email { get; set; }
		public string? DisplayName { get; set; }
		public string? Password { get; set; }
	}
}
