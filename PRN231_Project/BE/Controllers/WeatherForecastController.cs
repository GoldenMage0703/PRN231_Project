using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly PRN231_ProjectContext context;
        private readonly IRepository<Lib.Models.User> _userRepository;
        private readonly IRepository<Question> _question;


        public WeatherForecastController(PRN231_ProjectContext context,
            IRepository<Lib.Models.User> user,
            IRepository<Question> question)
        {
            _userRepository = user;
           
            this.context = context;
            _question = question;   
        }

      

        [HttpGet("Test1")]
        public async Task<IActionResult> get()
        {
            
            return Ok(await (_userRepository.GetAllAsync()));
        }
    }
}
