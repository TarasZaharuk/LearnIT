using Microsoft.AspNetCore.Mvc;
using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Services;
using LearnIT.Application.Models;

namespace LearnIT.WebUI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet("/users")]
        public async Task<List<UserDTO>> GetUsersAsync()
        {
            return await _usersService.GetAsync();
        }

        [HttpPost("/user")]
        public async Task AddUser(AddUserModel user)
        {
            await _usersService.AddAsync(user);
        }

        [HttpDelete("/user/{id}")]
        public async Task DeleteTutors(int id)
        {
            await _usersService.DeleteByIdAsync(id);
        }
    }
}
