using System;
using System.Collections.Generic;

namespace Lib.Models
{
    public partial class CourseAttempt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime AttemptDate { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
