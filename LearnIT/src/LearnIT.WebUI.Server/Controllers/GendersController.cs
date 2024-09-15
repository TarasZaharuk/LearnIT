using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LearnIT.WebUI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GendersController : ControllerBase
    {
        private readonly IGendersRepository _gendersRepository;

        public GendersController(IGendersRepository gendersRepository)
        {
            _gendersRepository = gendersRepository;
        }

        [HttpGet("/genders")]
        public async Task<List<Gender>> GetGenders()
        {
            return await _gendersRepository.GetAllAsync();
        }
    }
}
