using System;
using System.Collections.Generic;

namespace Lib.Models
{
    public partial class Course
    {
        public Course()
        {
            CourseAttempts = new HashSet<CourseAttempt>();
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }
        public string CourseName { get; set; } = null!;
        public bool Publish { get; set; }
        public int TotalJoined { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[]? Image { get; set; }
        public int Category { get; set; }
        public decimal Price { get; set; }

        public virtual Category CategoryNavigation { get; set; } = null!;
        public virtual User CreatedByNavigation { get; set; } = null!;
        public virtual ICollection<CourseAttempt> CourseAttempts { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
