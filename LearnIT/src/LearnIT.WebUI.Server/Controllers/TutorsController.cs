using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Services;
using LearnIT.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shared;
using System.Buffers.Text;
using System;

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

        [HttpGet("/tutors/{id}/logo")]
        public async Task<IActionResult> GetTutorLogoAsync(int id)
        {
            byte[] logo = await _tutorsService.GetLogoAsync(id);
            return File(logo, "image/jpeg");
        }

        [Authorize(Roles = "Tutor")]
        [HttpGet("/tutors/{id}")]
        public async Task<IActionResult> GetTutorById(int id)
        {
            if (!IsValidToken())
                return Unauthorized("Invalid token or missing ID.");
            if (!IsUserHasAccess(id))
                return Forbid("You are not authorized to access this tutor's data.");

            TutorDTO? tutor = await _tutorsService.GetByIdAsync(id);
            return Ok(tutor);
        }

        [Authorize(Roles = "User")]
        [HttpPost("/tutor")]
        public async Task<IActionResult> AddTutor(AddTutorModel addTutor)
        {
            int tutorId = await _tutorsService.AddAsync(addTutor);
            return Ok(tutorId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/tutors")]
        public async Task<IActionResult> UpdateTutorGeneralInfo(UpdateTutorGeneralInfoModel updatedTutor)
        {
            await _tutorsService.UpdateGeneralInfoAsync(updatedTutor);
            return Ok();
        }

        [Authorize(Roles = "Tutor")]
        [HttpPost("/tutor/skills")]
        public async Task<IActionResult> AddSkills(AddTutorSkillsModel addTutorSkills)
        {
            if (!IsValidToken())
                return Unauthorized("Invalid token or missing ID.");
            if (!IsUserHasAccess(addTutorSkills.TutorId))
                return Forbid("You are not authorized to access this tutor's data.");

            await _tutorsService.UpdateSkillsAsync(addTutorSkills);

            return Ok();
        }

        [Authorize(Roles = "Tutor")]
        [HttpPost("/tutors/{id}/logo")]
        public async Task<IActionResult> AddLogo(int id, byte[] file)
        {
            if (!IsValidToken())
                return Unauthorized("Invalid token or missing ID.");
            if (!IsUserHasAccess(id))
                return Forbid("You are not authorized to access this tutor's data.");
            await _tutorsService.SetLogoAsync(id, file);
            return Ok();
        }

        [Authorize(Roles = "Tutor")]
        [HttpDelete("/tutors/{id}")]
        public async Task<IActionResult> DeleteTutor(int id)
        {
            if (!IsValidToken())
                return Unauthorized("Invalid token or missing ID.");
            if(!IsUserHasAccess(id))
                return Forbid("You are not authorized to access this tutor's data.");
            await _tutorsService.DeleteByIdAsync(id);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("/tutors")]
        public async Task DeleteTutors()
        {
            await _tutorsService.DeleteAllAsync();
        }

        private bool IsValidToken()
        {
            int? tutorId = GetTokenTutorId();
            if (tutorId != null)
                return true;

            return false;
        }

        private bool IsUserHasAccess(int id)
        {
            if (!IsValidToken())
                return false;
            int? tutorId = GetTokenTutorId();

            if (tutorId != null && tutorId == id)
                return true;

            return false;
        }

        private int? GetTokenTutorId()
        {
            string? tutorIdClaim = User.FindFirst("TutorId")?.Value;

            if (!string.IsNullOrEmpty(tutorIdClaim) && int.TryParse(tutorIdClaim, out int tutorId))
                return tutorId;

            return null;
        }
    }
}
