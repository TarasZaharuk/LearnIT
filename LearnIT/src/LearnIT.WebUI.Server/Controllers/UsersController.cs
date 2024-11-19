using Microsoft.AspNetCore.Mvc;
using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Services;
using Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace LearnIT.WebUI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private IUsersService _usersService;
        private IConfiguration _configuration;

        public UsersController(IUsersService usersService, IConfiguration configuration)
        {
            _usersService = usersService;
            _configuration = configuration;
        }

        [HttpGet("/users")]
        public async Task<List<UserDTO>> GetUsersAsync()
        {
            return await _usersService.GetAsync();
        }

        [HttpGet("/users/{id}")]
        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            return await _usersService.GetByIdAsync(id);
        }

        [HttpPost("/user")]
        public async Task<IActionResult> AddUser(AddUserModel user)
        {
            int userId = await _usersService.AddAsync(user);
            UserDTO? userDTO = await _usersService.GetByIdAsync(userId);
            if(userDTO == null)
                return NotFound();
            string token = GenerateJSONWebToken(userDTO);
            return Ok(token);
        }

        [HttpDelete("/user/{id}")]
        public async Task DeleteTutors(int id)
        {
            await _usersService.DeleteByIdAsync(id);
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
