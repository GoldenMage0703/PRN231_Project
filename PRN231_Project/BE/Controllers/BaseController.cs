using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly IRepository<Question> _question;

        public BaseController(IRepository<Question> question)
        {
            _question = question;
        }

        // Common functionality for all controllers
        protected IActionResult HandleException(Exception ex)
        {
            // Log the exception if needed (using ILogger, for example)
            // Return a standardized error response
            return StatusCode(500, new { message = "An error occurred. Please try again later.", details = ex.Message });
        }

        protected IActionResult OkResponse<T>(T result)
        {
            return Ok(new { success = true, data = result });
        }

        protected IActionResult ErrorResponse(string errorMessage)
        {
            return BadRequest(new { success = false, message = errorMessage });
        }
    }

}
