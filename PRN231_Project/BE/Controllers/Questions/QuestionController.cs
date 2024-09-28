using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers.Questions
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IRepository<Question> _question;

        public QuestionController(IRepository<Question> question)
        {
            _question = question;
        }

        [HttpGet("GetQuestionByCourse")]
        public async Task<IActionResult> Get(int courseID) {
        
      var list= await _question.FindIncludeAsync(x=>x.Options,x=>x.Course == courseID);
            //var list = await _question.GetAllAsync();
            return Ok(list);
        }

    }
}
