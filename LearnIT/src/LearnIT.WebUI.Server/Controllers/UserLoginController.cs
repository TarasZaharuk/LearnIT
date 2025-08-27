using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Services;
using LearnIT.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Domain;
using LearnIT.Domain.Entities;


namespace LearnIT.WebUI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly ITokenService _tokenService;

        public UserLoginController(IUsersService usersService, ITokenService tokenService)
        {
            _usersService = usersService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("/login")]
        public async Task<IActionResult> Login(UserLoginModel login)
        {
            UserDTO? user = await AuthenticateUser(login);

            if (user == null)
                return Unauthorized();
            
            string tokenString = _tokenService.GenerateAuthenticationToken(user);
            return Ok(tokenString);
        }

        [Authorize(Roles = "User")]
        [HttpGet("/token/renewed")]
        public async Task<IActionResult> GetRenewedToken()
        {
            string? tutorIdClaim = User.FindFirst("Id")?.Value;

            int.TryParse(tutorIdClaim, out int userId);
            UserDTO? user = await _usersService.GetByIdAsync(userId);
            if (user == null)
                return Unauthorized();
            string tokenString = _tokenService.GenerateAuthenticationToken(user);
            return Ok(tokenString);
        }

        private async Task<UserDTO?> AuthenticateUser(UserLoginModel loginModel)
        {
            UserDTO? user = await _usersService.GetUserByLoginAsync(loginModel);
            if (user != null)
                return user;
            return null;
        }
    }
}
