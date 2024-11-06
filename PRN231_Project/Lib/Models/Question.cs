using System;
using System.Collections.Generic;

namespace Lib.Models
{
    public partial class Question
    {
        public Question()
        {
            Options = new HashSet<Option>();
        }

        public int Id { get; set; }
        public int Course { get; set; }
        public string QuestionText { get; set; } = null!;

        public virtual Course CourseNavigation { get; set; } = null!;
        public virtual ICollection<Option> Options { get; set; }
    }
}
