using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LearnIT.WebUI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsultationsController : ControllerBase
    {
        private readonly IConsultationsService _consultationsService;

        public ConsultationsController(IConsultationsService consultationsService)
        {
            _consultationsService = consultationsService;
        }

        //todo: Update to Authorize and check if user has access
        [HttpGet]
        public async Task<List<ConsultationDTO>> GetAllConsultationsAsync()
        {
            return await _consultationsService.GetAllAsync();
        }

        //todo: Update to Authorize and check if user has access
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConsultationByIdAsync(int id)
        {
            ConsultationDTO? consultation = await _consultationsService.GetByIdAsync(id);
            if (consultation == null)
                return NotFound("Consultation not found");

            return Ok(consultation);
        }

        //todo: Update to Authorize and check if user has access
        [HttpGet("tutor/{tutorId}")]
        public async Task<List<ConsultationDTO>> GetConsultationsByTutorIdAsync(int tutorId)
        {
            return await _consultationsService.GetByTutorIdAsync(tutorId);
        }

        [HttpGet("user/{userId}")]
        public async Task<List<ConsultationDTO>> GetConsultationsByUserIdAsync(int userId)
        {
            return await _consultationsService.GetByUserIdAsync(userId);
        }

        //todo: Update to Authorize and check if user has access
        [HttpGet("date-range")]
        public async Task<List<ConsultationDTO>> GetConsultationsByDateRangeAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            return await _consultationsService.GetByDateRangeAsync(startDate, endDate);
        }

        [Authorize(Roles = "Tutor")]
        [HttpPost]
        public async Task<IActionResult> AddConsultationAsync(AddConsultationModel consultation)
        {
            IActionResult? tokenAccessError = GetTokenAccessError(consultation.TutorId);
            if (tokenAccessError != null) return tokenAccessError;

            int consultationId = await _consultationsService.AddAsync(consultation);
            return Ok(consultationId);
        }

        [Authorize(Roles = "Tutor")]
        [HttpPost("batch")]
        public async Task<IActionResult> AddConsultationsAsync(List<AddConsultationModel> consultations)
        {
            if (!consultations.Any())
                return BadRequest("Consultations list is empty");

            int firstTutorId = consultations.First().TutorId;
            if (!consultations.All(c => c.TutorId == firstTutorId))
                return BadRequest("All consultations must have the same tutor");

            IActionResult? tokenAccessError = GetTokenAccessError(firstTutorId);
            if (tokenAccessError != null) return tokenAccessError;

            await _consultationsService.AddAsync(consultations);
            return Ok();
        }

        [Authorize(Roles = "Tutor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConsultationAsync(int id, UpdateConsultationModel consultation)
        {
            if (id != consultation.Id)
                return BadRequest("ID mismatch");

            IActionResult? tokenAccessError = GetTokenAccessError(consultation.TutorId);
            if (tokenAccessError != null) return tokenAccessError;

            await _consultationsService.UpdateAsync(consultation);
            return Ok();
        }

        [Authorize(Roles = "Tutor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsultationAsync(int id)
        {
            ConsultationDTO? consultation = await _consultationsService.GetByIdAsync(id);
            if (consultation == null)
                return NotFound("Consultation not found");

            IActionResult? tokenAccessError = GetTokenAccessError(consultation.TutorId);
            if (tokenAccessError != null) return tokenAccessError;

            await _consultationsService.DeleteByIdAsync(id);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task DeleteAllConsultationsAsync()
        {
            await _consultationsService.DeleteAllAsync();
        }

        private IActionResult? GetTokenAccessError(int tutorId)
        {
            if (!IsTokenHasTutorId()) return Unauthorized("Token has no TutorId");
            if (!IsUserHasAccess(tutorId)) return Forbid("You are not authorized to access this tutor's consultations.");

            return null;
        }

        private bool IsTokenHasTutorId()
        {
            int? tutorId = GetTutorIdFromToken();
            return tutorId != null;
        }

        private bool IsUserHasAccess(int tutorId)
        {
            int? tokenTutorId = GetTutorIdFromToken();

            if (tokenTutorId != null && tokenTutorId == tutorId)
                return true;

            return false;
        }

        private int? GetTutorIdFromToken()
        {
            string? tutorIdClaim = User.FindFirst("TutorId")?.Value;

            if (int.TryParse(tutorIdClaim, out int tutorId))
                return tutorId;

            return null;
        }
    }
}
