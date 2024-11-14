using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Services;
using LearnIT.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Shared;

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
        public async Task<List<TutorDTO>> GetTutorsAsync([FromQuery]TutorsFilterModel tutorsFilter)
        {
            return await _tutorsService.GetAsync(tutorsFilter);
        }

        [HttpGet("/tutors/{id}")]
        public async Task<TutorDTO?> GetTutorById(int id)
        {
            return await _tutorsService.GetByIdAsync(id);
        }

        [HttpGet("/tutors/{id}/logo")]
        public async Task<IActionResult> GetTutorLogoAsync(int id)
        {
            byte[] logo = await _tutorsService.GetLogoAsync(id);
            return File(logo,"image/jpeg");
        }

        [HttpPost("/tutor")]
        public async Task<IActionResult> AddTutor(AddTutorModel addTutor)
        {
            int tutorId = await _tutorsService.AddAsync(addTutor);
            return Ok(tutorId);
        }

        [HttpPut("/tutors")]
        public async Task<IActionResult> UpdateTutorGeneralInfo(UpdateTutorGeneralInfoModel updatedTutor)
        {
            await _tutorsService.UpdateGeneralInfoAsync(updatedTutor);
            return Ok();
        }

        [HttpPost("/tutor/skills")]
        public async Task AddSkills(AddTutorSkillsModel addTutorSkills)
        {
            await _tutorsService.UpdateSkillsAsync(addTutorSkills);
        }

        [HttpPost("/tutors/{id}/logo")]
        public async Task<IActionResult> AddLogo(int id, byte[] file)
        {            
            await _tutorsService.SetLogoAsync(id, file);
            return Ok();
        }

        [HttpDelete("/tutors/{id}")]
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
