using Lib.DTO.Options;
using Lib.DTO.Question;
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
        private readonly IRepository<Option> _option;

        public QuestionController(IRepository<Question> question, IRepository<Option> option)
        {
            _question = question;
            _option = option;
        }

        [HttpGet("GetQuestionByCourse")]
        public async Task<IActionResult> Get(int courseID)
        {
            var questions = await _question.FindIncludeAsync(x => x.Options, x => x.Course == courseID);
            var questionDtos = questions.Select(q => new QuestionDTO
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                Options = q.Options.Select(o => new OptionDTO
                {
                    Id = o.Id,
                    OptionText = o.OptionText
                }).ToList()
            }).ToList();

            return Ok(questionDtos);
        }

        [HttpGet("GetQuestionByID")]
        public async Task<IActionResult> GetQues(int id)
        {

            var list = await _question.GetByIdIncludeAsync(x => x.Options, x => x.Id == id);
            //var list = await _question.GetAllAsync();
            return Ok(list);
        }
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateQuestionWithOptions(int id, [FromBody] UpdateQuestionDTO questionUpdate)
        {
            // Retrieve the existing question including its options
            var questionList = await _question.GetByIdIncludeAsync(x => x.Options, x => x.Id == id);
            questionList.QuestionText = questionUpdate.QuestionText;
            var optionListUpdate = questionUpdate.Options.Select(x=> new
            {
                x.id,
                x.OptionText
            }).ToDictionary(opt => opt.id, opt => opt.OptionText);
            var optionListPrevious = questionList.Options.Select(x => new
            {
                x.Id,
                x
            }).ToDictionary(opt => opt.Id, opt => opt.x);
            ICollection<Option> itemNotMapping =optionListPrevious.
                Where(x => optionListUpdate.ContainsKey(x.Key) == false).
                Select(x=>x.Value).ToList();
            await _option.DeleteRangeAsync(itemNotMapping);


            foreach (var option in questionUpdate.Options)
            {
                if (option.id == 0)
                {
                  await  _option.AddAsync(new Option {
                       QuestionId = questionList.Id,
                       OptionText = option.OptionText,
                       IsCorrect = option.isCorrect          
                    });
                    continue;
                }
                if (optionListPrevious.ContainsKey(option.id))
                {
                    var prevOption =  await _option.GetByIdAsync(option.id);
                    prevOption.OptionText = option.OptionText;
                    prevOption.IsCorrect = option.isCorrect;
                  await  _option.UpdateAsync(prevOption);
                }
            }

            await _question.UpdateAsync(questionList);

            return Ok(questionList);
        }

        [HttpPost("CreateQuestion")]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionDTO question,int courseID)
        {
           await _question.AddAsync(new Question
            {
                Course = courseID,
                QuestionText = question.QuestionText,
            });
            var questionId =await _question.GetLastAsync(x => x.Id);
            var listOption = (ICollection<Option>)question.Options.Select(x => new Option
            {
                QuestionId = _question.GetLastAsync(x => x.Id).Id,
                IsCorrect = x.isCorrect,
                OptionText = x.OptionText,
            }).ToList();
            _option.AddRangeAsync(listOption);
            return Ok();
        }
        }

    }


