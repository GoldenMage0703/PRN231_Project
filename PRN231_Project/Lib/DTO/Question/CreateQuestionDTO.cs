using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.DTO.Options;

namespace Lib.DTO.Question
{
    public class CreateQuestionDTO
    {
        
        public string QuestionText { get; set; } = null!;
        public ICollection<CreateOptionDTO>? Options { get; set; }
    }
}
