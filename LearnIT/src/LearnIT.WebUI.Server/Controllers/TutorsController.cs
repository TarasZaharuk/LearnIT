using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Services;
using LearnIT.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearnIT.WebUI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TutorsController : ControllerBase
    {
        private ITutorsService _tutorsService;

        public TutorsController(ITutorsService tutorsService)
        {
            _tutorsService = tutorsService;
        }

        [HttpGet("/tutors")]
        public async Task<List<TutorDTO>> GetTutorsAsync()
        {
            return await _tutorsService.GetAsync();
        }

        [HttpPost("/tutor")]
        public async Task AddTutor(AddTutorModel addTutor)
        {
            await _tutorsService.AddAsync(addTutor);
        }

        [HttpPost("/tutor/skills")]
        public async Task AddSkills(AddTutorSkillsModel addTutorSkills)
        {
            await _tutorsService.AddSkillsAsync(addTutorSkills);
        }

        [HttpDelete("/tutor/{id}")]
        public async Task DeleteTutor(int id)
        {
            await _tutorsService.DeleteByIdAsync(id);
        }

        [HttpDelete("/tutors")]
        public async Task DeleteTutors()
        {
            await _tutorsService.DeleteAllAsync();
        }
    }
}
