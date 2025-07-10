using Microsoft.AspNetCore.Mvc;
using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Services;
using Shared;
using Shared.AddUserResponse;

namespace LearnIT.WebUI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
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

        [HttpGet("/users/{id}email/state")]
        public async Task<bool> IsEmailConfirmedAsync(int id)
        {
            return await _usersService.IsEmailConfirmed(id);
        }

        [HttpGet("/users/{id}email")]
        public async Task<IActionResult> GetUserEmailByIdAsync(int id)
        {
            string? email = await _usersService.GetEmailByIdAsync(id);
            return Ok(email);
        }

        [HttpPost("/user")]
        public async Task<IActionResult> AddUser(AddUserModel addedUser)
        {
            AddUserResponse addUserResponse = await _usersService.AddAsync(addedUser);
            if (addUserResponse.Issue is AddingUserIssue.None)
                return Ok(addUserResponse);
            else return BadRequest(addUserResponse);
        }

        [HttpDelete("/user/{id}")]
        public async Task DeleteTutors(int id)
        {
            await _usersService.DeleteByIdAsync(id);
        }
    }
}