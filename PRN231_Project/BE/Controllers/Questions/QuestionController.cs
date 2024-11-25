using Lib.DTO.Options;
using Lib.DTO.Question;
using Lib.File_Utils;
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
                    OptionText = o.OptionText,
                    IsCorrect = o.IsCorrect,
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
            var questionToAdd = new Question
            {
                Course = courseID,
                QuestionText = question.QuestionText,
            };
            await _question.AddAsync(questionToAdd);
            var IdToAdd = await _question.GetLastAsync(x => x.Id);
            var listOption = (ICollection<Option>)question.Options.Select(x => new Option
            {
                QuestionId = IdToAdd.Id,
                IsCorrect = x.isCorrect,
                OptionText = x.OptionText,
            }).ToList();
            await _option.AddRangeAsync(listOption);
            return Ok();
        }

        [HttpDelete("DeleteQuestion")]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            var ques = (Question)await _question.GetByIdIncludeAsync(x => x.Options, x => x.Id == questionId);

            await _option.DeleteRangeAsync(ques.Options);
            await _question.DeleteAsync(questionId);
            return Ok(ques);
        }

        [HttpPost("DownloadInportQuestionTemplate")]
        public async Task<IActionResult> Download()
        {
            var fileBytes = ExcelGeneratorUtil.GenerateExcelFile();
            var fileName = "Questions.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(fileBytes, contentType, fileName);

        }
        [HttpPost("ImportListQuestion")]
        public async Task<IActionResult> ImportListQuestion(IFormFile file, int courseId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not selected or empty.");
            }

            List<CreateQuestionDTO> questions;
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();

                    // Use the ExcelReaderUtil to parse the file
                    questions = ExcelReaderUtil.ReadQuestionsFromExcel(fileBytes);
                }

                var questionEntities = questions.Select(q => new Question
                {
                    Course = courseId,
                    QuestionText = q.QuestionText,
                }).ToList();

                await _question.AddRangeAsync(questionEntities);
            

                var optionEntities = new List<Option>();

                foreach (var questionEntity in questionEntities)
                {
                    var question = questions.First(q => q.QuestionText == questionEntity.QuestionText);
                    var options = question.Options.Select(o => new Option
                    {
                        QuestionId = questionEntity.Id,
                        IsCorrect = o.isCorrect,
                        OptionText = o.OptionText,
                    });

                    optionEntities.AddRange(options);
                }

                await _option.AddRangeAsync(optionEntities);
            }
            catch (Exception e)
            {
                return BadRequest("File is wrong with format    .");
            }
            // Read and parse the uploaded file
            
           

            return Ok(questions);
        }

    }

}


