using AutoMapper;
using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Application.Interfaces.Services;
using LearnIT.Domain.Entities;

namespace LearnIT.Application.Services
{
    public class ConsultationsService : IConsultationsService
    {
        private readonly IConsultationsRepository _consultationsRepository;
        private readonly IMapper _mapper;

        public ConsultationsService(IConsultationsRepository consultationsRepository, IMapper mapper)
        {
            _consultationsRepository = consultationsRepository;
            _mapper = mapper;
        }

        public async Task<int> AddAsync(AddConsultationModel consultation)
        {
            Consultation consultationEntity = _mapper.Map<AddConsultationModel, Consultation>(consultation);
            int consultationId = await _consultationsRepository.AddAsync(consultationEntity);
            return consultationId;
        }

        public async Task AddAsync(List<AddConsultationModel> consultations)
        {
            List<Consultation> consultationEntities = _mapper.Map<List<AddConsultationModel>, List<Consultation>>(consultations);
            await _consultationsRepository.AddAsync(consultationEntities);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _consultationsRepository.DeleteAsync(id);
        }

        public async Task DeleteAllAsync()
        {
            await _consultationsRepository.DeleteAllAsync();
        }

        public async Task<List<ConsultationDTO>> GetAllAsync()
        {
            List<Consultation> consultations = await _consultationsRepository.GetAllAsync();
            List<ConsultationDTO> consultationDTOs = _mapper.Map<List<Consultation>, List<ConsultationDTO>>(consultations);
            return consultationDTOs;
        }

        public async Task<ConsultationDTO?> GetByIdAsync(int id)
        {
            Consultation? consultation = await _consultationsRepository.GetByIdAsync(id);
            if (consultation == null)
                return null;

            ConsultationDTO consultationDTO = _mapper.Map<Consultation, ConsultationDTO>(consultation);
            return consultationDTO;
        }

        public async Task<List<ConsultationDTO>> GetByUserIdAsync(int userId)
        {
            List<Consultation> consultations = await _consultationsRepository.GetByUserIdAsync(userId);
            List<ConsultationDTO> consultationDTOs = _mapper.Map<List<Consultation>, List<ConsultationDTO>>(consultations);
            return consultationDTOs;
        }

        public async Task<List<ConsultationDTO>> GetByTutorIdAsync(int tutorId)
        {
            List<Consultation> consultations = await _consultationsRepository.GetByTutorIdAsync(tutorId);
            List<ConsultationDTO> consultationDTOs = _mapper.Map<List<Consultation>, List<ConsultationDTO>>(consultations);
            return consultationDTOs;
        }

        public async Task<List<ConsultationDTO>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            List<Consultation> consultations = await _consultationsRepository.GetByDateRangeAsync(startDate, endDate);
            List<ConsultationDTO> consultationDTOs = _mapper.Map<List<Consultation>, List<ConsultationDTO>>(consultations);
            return consultationDTOs;
        }

        public async Task UpdateAsync(UpdateConsultationModel consultation)
        {
            Consultation? existingConsultation = await _consultationsRepository.GetByIdAsync(consultation.Id);
            if (existingConsultation == null)
                return;

            _mapper.Map(consultation, existingConsultation);
            await _consultationsRepository.UpdateAsync(existingConsultation);
        }
    }
}
