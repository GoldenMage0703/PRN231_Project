using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO
{
	public class UserDTO
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string DisplayName { get; set; }
		public int Role { get; set; }
		public int Status { get; set; }
		public DateTime Created { get; set; }

    }
}
