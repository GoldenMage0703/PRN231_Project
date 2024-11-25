using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO
{
	public class UserRegisterDTO
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string DisplayName { get; set; }
		public string Email { get; set; }
		// 1: Admin, 2: Teacher, 3: User
		public int Role { get; set; }
	}
}

