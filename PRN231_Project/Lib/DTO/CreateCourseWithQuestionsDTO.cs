using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.DTO.Question;

namespace Lib.DTO
{
    public class CreateCourseWithQuestionsDTO
    {
        public CreateCourseDTO Course { get; set; }
        public List<CreateQuestionDTO> Questions { get; set; }
    }

}
