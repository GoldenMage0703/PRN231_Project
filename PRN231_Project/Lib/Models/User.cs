using System;
using System.Collections.Generic;

namespace Lib.Models
{
    public partial class User
    {
        public User()
        {
            Bills = new HashSet<Bill>();
            CourseAttempts = new HashSet<CourseAttempt>();
            Courses = new HashSet<Course>();
        }

        public int Id { get; set; }
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
		//public string PasswordHash { get; set; } = null!; // thêm PasswordHash
		public int Role { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<CourseAttempt> CourseAttempts { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
    }
}
