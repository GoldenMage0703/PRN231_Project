using Lib.DTO;
using Lib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lib.DTO.Authenticate;

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
        private string GenerateToken(Lib.Models.User user)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("username", user.Username.Trim()), // Trimmed username
                new Claim("id", user.Id.ToString()), // User ID
                new Claim("displayName", user.DisplayName.Trim()), // Trimmed display name
                new Claim(JwtRegisteredClaimNames.Email, user.Email.Trim()), // Trimmed email
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Role as a string
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

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
				var token = GenerateToken(user);
				response = Ok(new { token = token });
			}
            return response;
        }

        [Authorize]
        [HttpGet("user")]
        public IActionResult GetUserData()
        {
            // Get the claims from the user identity
            var usernameClaim = User.FindFirst("username");
            var userIdClaim = User.FindFirst("id"); // Custom claim for user ID
            var displayNameClaim = User.FindFirst("displayName"); // Custom claim for display name
            var emailClaim = User.FindFirst(JwtRegisteredClaimNames.Email); // Email claim
            var roleClaim = User.FindFirst(ClaimTypes.Role); // Role claim

            if (usernameClaim == null)
            {
                return Unauthorized("Invalid token.");
            }

            // Find the user in the database using the username
            var user = _context.Users.FirstOrDefault(u => u.Username == usernameClaim.Value);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Create a user data response object
            var userData = new
            {
                Id = userIdClaim?.Value, // Get user ID from claims
                Username = user.Username,
                DisplayName = displayNameClaim?.Value, // Get display name from claims
                Email = emailClaim?.Value, // Get email from claims
                Role = roleClaim?.Value // Get role from claims
            };

            return Ok(userData); // Return the user data
        }

    }
}
