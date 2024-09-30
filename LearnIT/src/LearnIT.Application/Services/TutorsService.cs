using AutoMapper;
using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Application.Interfaces.Services;
using LearnIT.Application.Models;
using LearnIT.Domain.Entities;

namespace LearnIT.Application.Services
{
    public class TutorsService : ITutorsService
    {
        private readonly ITutorsRepository _tutorsRepository;
        private readonly ISkillsRepository _skillsRepository;
        private readonly IMapper _mapper;
        public TutorsService(ITutorsRepository tutorsRepository, ISkillsRepository skillsRepository, IMapper mapper)
        {
            _tutorsRepository = tutorsRepository;
            _skillsRepository = skillsRepository;
            _mapper = mapper;
        }
        public async Task AddAsync(AddTutorModel addedTutor)
        {
            Tutor tutor = _mapper.Map<AddTutorModel, Tutor>(addedTutor);
            List<Skill> skills = await _skillsRepository.GetByIdsAsync(addedTutor.SkillsIds);
            if (skills.Any())
                tutor.Skills = skills;
            await _tutorsRepository.AddAsync(tutor);
        }

        public async Task AddSkillsAsync(AddTutorSkillsModel addTutorSkills)
        {
            var tutor = await _tutorsRepository.GetByIdAsync(addTutorSkills.TutorId);
            if (tutor == null)
                return;

            var skills = await _skillsRepository.GetByIdsAsync(addTutorSkills.SkillIds);
            if (!skills.Any())
                return;

            skills.Where(sk => !tutor.Skills.Contains(sk)).ToList().ForEach(tutor.Skills.Add);
            await _tutorsRepository.UpdateAsync(tutor);
        }

        public async Task DeleteAllAsync()
        {
            await _tutorsRepository.DeleteAllAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _tutorsRepository.DeleteAsync(id);
        }

        public async Task<List<TutorDTO>> GetAsync()
        {
            List<Tutor> tutors = await _tutorsRepository.GetAllAsync();
            return _mapper.Map<List<Tutor>, List<TutorDTO>>(tutors);
        }

        public async Task SetLogoAsync(int tutorId, byte[] logo)
        {
            Tutor tutor = await _tutorsRepository.GetByIdAsync(tutorId) ?? throw new Exception("Invalid 'Id'");
            tutor.Logo = logo;
            await _tutorsRepository.UpdateAsync(tutor);
        }

        public async Task<byte[]> GetLogoAsync(int tutorId)
        {
            Tutor tutor = await _tutorsRepository.GetByIdAsync(tutorId) ?? throw new Exception("Invalid 'Id'");
            return tutor.Logo;
        }
    }
}
