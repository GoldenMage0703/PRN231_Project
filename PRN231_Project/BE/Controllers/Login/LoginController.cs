using Lib.DTO;
using Lib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BE.Controllers.Login
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
        private IConfiguration _config;
		private readonly PRN231_ProjectContext _context;
		public LoginController(IConfiguration configuration, PRN231_ProjectContext context)
        {
            _config = configuration;
			_context = context;
		}
        private Lib.Models.User AuthenticateUser(UserLoginDTO userLogin)
        {
			var user = _context.Users.FirstOrDefault(u => u.Username == userLogin.Username && u.Password == userLogin.Password);
			if (user != null)
			{
				return user;
			}
			return null;
		}
        private string GenerateToken(UserLoginDTO user, string role)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Username),
				new Claim(ClaimTypes.Role, role) 
			};
			var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"],
				claims,			
                expires: DateTime.Now.AddMinutes(30),
				signingCredentials : credentials);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(UserLoginDTO userLogin)
        {
            IActionResult response = Unauthorized("Invalid username or password.");
			var user = AuthenticateUser(userLogin);
            if (user != null)
            {
				string role;
				if (userLogin.Username == "admin")
				{
					role = "Admin";
				}
				else if (userLogin.Username == "teacher") 
				{
					role = "Teacher";
				}
				else
				{
					role = "User";
				}
				var token = GenerateToken(userLogin, role);
				response = Ok(new { token = token });
			}
            return response;
        }

		[Authorize(Roles = "Admin")]
		[HttpGet]
		[Route("Admin")]
		public IActionResult Admin()
		{
			return Ok("For only admin");
		}

		[Authorize(Roles = "Teacher")]
		[HttpGet]
		[Route("Teacher")]
		public IActionResult Teacher()
		{
			return Ok("For teachers.");
		}

		[Authorize(Roles = "User")]
		[HttpGet]
		[Route("User")]
		public IActionResult User()
		{
			return Ok("Users.");
		}

		[Authorize]
        [HttpGet]
        [Route("GetData")]
        public string GetData()
        {
            return "Authenticated with JWT";
        }

		[Authorize]
		[HttpPost]
		[Route("GetData")]
		public string AddUser(Lib.Models.User user)
		{
			return "User added with username" + user.Username;
		}
	}
}
