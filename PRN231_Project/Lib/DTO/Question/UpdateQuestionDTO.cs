using Lib.DTO.Options;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO.Question
{
    public class UpdateQuestionDTO
    {
        public int Id { get; set; }
        
        public string QuestionText { get; set; } = null!;
        public ICollection<UpdateOptionDTO> Options { get; set; }
    }
}
