using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearnIT.WebUI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private IUsersService _usersService;
        private IConfiguration _configuration;

        public UserLoginController(IUsersService usersService, IConfiguration configuration)
        {
            _usersService = usersService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("/login")]
        public async Task<IActionResult> Login(UserLoginModel login)
        {
            IActionResult response = Unauthorized();
            UserDTO? user = await AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(tokenString);
            }

            return response;
        }

        private async Task<UserDTO?> AuthenticateUser(UserLoginModel loginModel)
        {
            UserDTO? user = await _usersService.GetUserByLoginAsync(loginModel);
            if (user != null)
                return user;
            return null;
        }

        private string GenerateJSONWebToken(UserDTO user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            IList<Claim> claims = [];
            string tutorIdClaim = string.Empty;
            if (user.TutorId.HasValue)
                tutorIdClaim = user.TutorId.Value.ToString();

            claims.Add(new Claim("Id", user.Id.ToString()));
            claims.Add(new Claim("TutorId", tutorIdClaim));
            if (user.TutorId.HasValue)
                claims.Add(new Claim(ClaimTypes.Role, "Tutor"));
            else claims.Add(new Claim(ClaimTypes.Role, "User"));

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Issuer"],
              claims.ToArray(),
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
