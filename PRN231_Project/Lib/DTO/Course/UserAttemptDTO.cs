using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO.Course
{
	public class UserAttemptDTO
	{
		public int UserId { get; set; }
		public string UserName { get; set; }
		public int CourseId { get; set; }
		public string CourseName { get; set; }
		public int Status { get; set; }
	}

}
