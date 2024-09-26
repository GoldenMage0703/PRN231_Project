using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO
{
    public class CreateCourseWithQuestionsDTO
    {
        public CreateCourseDTO Course { get; set; }
        public List<CreateQuestionDTO> Questions { get; set; }
    }

}
