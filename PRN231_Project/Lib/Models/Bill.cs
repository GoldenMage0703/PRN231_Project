using System;
using System.Collections.Generic;

namespace Lib.Models
{
    public partial class Bill
    {
        public Bill()
        {
            Courses = new HashSet<Course>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalPayment { get; set; }

        public virtual User User { get; set; } = null!;

        public virtual ICollection<Course> Courses { get; set; }
    }
}
