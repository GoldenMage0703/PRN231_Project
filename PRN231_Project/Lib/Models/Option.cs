using System;
using System.Collections.Generic;

namespace Lib.Models
{
    public partial class Option
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string OptionText { get; set; } = null!;
        public bool IsCorrect { get; set; }

        public virtual Question Question { get; set; } = null!;
    }
}
