using Microsoft.AspNetCore.Mvc;
using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Services;
using Shared;

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

        [HttpPost("/user")]
        public async Task<IActionResult> AddUser(AddUserModel addedUser)
        {
            //
            string messege = await _usersService.AddAsync(addedUser);
            return Ok(messege);
        }

        [HttpDelete("/user/{id}")]
        public async Task DeleteTutors(int id)
        {
            await _usersService.DeleteByIdAsync(id);
        }
    }
}