using Lib.DTO;
using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers.Courses
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {

        private readonly IRepository<Course> _course;
        private readonly IRepository<Question> _question;
        private readonly IRepository<Option> _option;
        private readonly IRepository<Category> _category;
        public CourseController(IRepository<Course> course, IRepository<Question> question, IRepository<Option> option, IRepository<Category> category)
        {
            _course = course;
            _question = question;
            _option = option;
            _category = category;
        }

        [HttpGet("GetAllCourse")]
        public async Task<IActionResult> Get()
        {
            var courseList = await _course.GetAllAsync();
            return Ok(courseList);
        }

        [HttpPut("CreateCourse")]
        public async Task<IActionResult> Put(CreateCourseWithQuestionsDTO createCourseWithQuestionsDTO)
        {
            try
            {
                var createCourseDTO = createCourseWithQuestionsDTO.Course;
                var createQuestionDTO = createCourseWithQuestionsDTO.Questions;

                await _course.AddAsync(new Course
                {
                    CourseName = createCourseDTO.CourseName,
                    Publish = createCourseDTO.Publish,
                    TotalJoined = 0,
                    CreatedBy = 1,
                    CreatedAt = DateTime.Now,
                    Image = createCourseDTO.Image,
                    Category = createCourseDTO.Category,
                });

                var createCourse = await _course.GetLastAsync(x=>x.Id);

                foreach (CreateQuestionDTO question in createQuestionDTO)
                {
                    await _question.AddAsync(new Question
                    {
                        Course = createCourse.Id,
                        QuestionText = question.QuestionText,
                    });

                    foreach (var item in question.Options)
                    {
                        await _option.AddAsync(new Option
                        {
                            OptionText = item.OptionText,
                            Question = await _question.GetLastAsync(x=>x.Id),
                        });
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
