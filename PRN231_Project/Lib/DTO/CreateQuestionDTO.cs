using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO
{
    public class CreateQuestionDTO
    {
        public int Course { get; set; }
        public string QuestionText { get; set; } = null!;
     public ICollection<CreateOptionDTO> Options { get; set; }
    }
}
