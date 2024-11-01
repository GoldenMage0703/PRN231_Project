using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lib.Models
{
    public partial class Option
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string OptionText { get; set; } = null!;
        public bool IsCorrect { get; set; }
        [JsonIgnore]
        public virtual Question Question { get; set; } = null!;
    }
}
