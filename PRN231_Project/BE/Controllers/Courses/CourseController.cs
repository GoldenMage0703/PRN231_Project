using Lib.DTO;
using Lib.DTO.Course;
using Lib.DTO.Question;
using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers.Courses
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        protected readonly PRN231_ProjectContext _context;
        private readonly IRepository<Course> _course;
        private readonly IRepository<Question> _question;
        private readonly IRepository<Option> _option;
        private readonly IRepository<Category> _category;
        private readonly IRepository<CourseAttempt> _courseAttempt;
        public CourseController(IRepository<Course> course, IRepository<CourseAttempt> courseAttempt, IRepository<Question> question, IRepository<Option> option, IRepository<Category> category, PRN231_ProjectContext context)
        {
            _courseAttempt = courseAttempt;
            _course = course;
            _question = question;
            _option = option;
            _category = category;
            _context = context;
        }

        [HttpGet("GetAllCourse")]
        public async Task<IActionResult> Get()
        {
            var courseList = await _course.GetAllAsync();
            return Ok(courseList);
        }

        [HttpGet("GetAllCourseBrowse")]
        public async Task<IActionResult> GetAllCourse(int userId)
        {
            // Step 1: Get the list of courses the user has attempted
            var attemptedCourseIds = await _context.CourseAttempts
                .Where(x => x.UserId == userId)
                .Select(x => x.CourseId)  // Get only the CourseIds
                .ToListAsync();

            // Step 2: Get all courses excluding the ones the user has already attempted
            var courseList = await _context.Courses
                .Include(c => c.CategoryNavigation) // Load related Category
                .Where(c => !attemptedCourseIds.Contains(c.Id)) // Exclude attempted courses
                .Select(c => new GetCourseDTO
                {
                    Id = c.Id,
                    CourseName = c.CourseName,
                    Publish = c.Publish,
                    TotalJoined = c.TotalJoined,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    Image = c.Image,
                    Price = c.Price,
                    CategoryName = c.CategoryNavigation.CategoryName // Map category name
                }).ToListAsync();

            // If no courses are found, return NotFound
            if (courseList == null || !courseList.Any())
            {
                return NotFound();
            }

            return Ok(courseList);
        }

        [HttpGet("GetMyManageCourse")]
        public async Task<IActionResult> Detail(int userId) {
            var courseList = await _context.Courses
               .Include(c => c.CategoryNavigation).Where(x=>x.CreatedBy == userId) // Load related Category
               .Select(c => new GetCourseDTO
               {
                   Id = c.Id,
                   CourseName = c.CourseName,
                   Publish = c.Publish,
                   TotalJoined = c.TotalJoined,
                   CreatedBy = c.CreatedBy,
                   CreatedAt = c.CreatedAt,
                   Image = c.Image,
                   Price = c.Price,
                   CategoryName = c.CategoryNavigation.CategoryName // Map category name
               }).ToListAsync();
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

                var createCourse = await _course.GetLastAsync(x => x.Id);

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
                            Question = await _question.GetLastAsync(x => x.Id),
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

        [HttpGet("GetCourseByID")]
        public async Task<IActionResult> GetCourseByID(int id)
        {
            var courseList = await _course.GetByIdAsync(id);
            return Ok(courseList);
        }

        [HttpGet("GetMyAttemptCourse")]
        public async Task<IActionResult> GetMyAttemptCourse(int userId)
        {
            var response = await _context.CourseAttempts
                .Where(x => x.UserId == userId)
                .Select(c => new GetCourseDTO
                {
                    Id = c.CourseId,
                    CourseName = c.Course.CourseName,
                    Publish = c.Course.Publish,
                    TotalJoined = c.Course.TotalJoined,
                    CreatedBy = c.Course.CreatedBy,
                    CreatedAt = c.Course.CreatedAt,
                    Image = c.Course.Image,
                    Price = c.Course.Price,
                    CategoryName = c.Course.CategoryNavigation.CategoryName // Map category name
                })
                .ToListAsync(); // Fetch all data first

            // Now apply DistinctBy on the client side
            var distinctResponse = response.DistinctBy(x => x.Id).ToList();

            return Ok(distinctResponse);
        }


        [HttpGet("GetCourseByNameBrowse")]
        public async Task<IActionResult> GetCourseByIDBrowse(string name)
        {
            // Fetch the course by ID and include the related Questions and Category
            var course = await _context.Courses
                .Include(c => c.Questions) // Include the Questions related to the course
                .Include(c => c.CategoryNavigation)  // Include the Category related to the course
                .Where(c => c.CourseName.Contains(name))
                .Select(c => new GetCourseDTO
                {
                    Id = c.Id,
                    CourseName = c.CourseName,
                    Publish = c.Publish,
                    TotalJoined = c.TotalJoined,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt,
                    Image = c.Image,
                    CategoryName = c.CategoryNavigation.CategoryName,
                })
                .FirstOrDefaultAsync();

            // If the course is not found, return NotFound
            if (course == null)
            {
                return NotFound();
            }

            // Return the course with category name and questions
            return Ok(course);
        }

    }
}
