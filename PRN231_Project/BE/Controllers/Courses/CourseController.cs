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
        public CourseController(IRepository<Course> course)
        {
            _course = course;
        }
        [HttpGet ("GetAllCourse")]
        public async Task<IActionResult> Get() {
            var courseList = await _course.GetAllAsync();
            return  Ok(courseList);
        }

        //[HttpPut ("CreateCourse")]
        //public async Task<IActionResult> Put()
        //{

        //}





    }
}
