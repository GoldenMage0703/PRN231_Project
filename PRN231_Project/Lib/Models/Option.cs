using System;
using System.Collections.Generic;

namespace Lib.Models
{
    public partial class Option
    {
        public Option()
        {
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string OptionText { get; set; } = null!;

        public virtual Question Question { get; set; } = null!;

        public virtual ICollection<Question> Questions { get; set; }
    }
}
