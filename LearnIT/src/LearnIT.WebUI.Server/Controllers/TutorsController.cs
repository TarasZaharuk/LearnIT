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

        [HttpGet("/tutors/{id}/logo")]
        public async Task<IActionResult> GetTutorLogoAsync(int id)
        {
            byte[] logo = await _tutorsService.GetLogoAsync(id);
            return File(logo,"image/jpeg");
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

        [HttpPost("/tutors/{id}/logo")]
        public async Task<IActionResult> AddLogo(IFormFile formFile,int id)
        {
            byte[] logo = [];
            if (formFile.ContentType != "image/jpeg")
                return BadRequest(new BadImageFormatException());

            using var stream = new MemoryStream();
            await formFile.CopyToAsync(stream);
            logo = stream.ToArray();
            

            await _tutorsService.SetLogoAsync(id, logo);
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
