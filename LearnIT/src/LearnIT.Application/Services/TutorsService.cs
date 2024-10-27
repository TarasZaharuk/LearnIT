using AutoMapper;
using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Application.Interfaces.Services;
using LearnIT.Application.Models;
using LearnIT.Domain.Entities;
using System.Configuration;
using Shared;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace LearnIT.Application.Services
{
    public class TutorsService : ITutorsService
    {
        private readonly ITutorsRepository _tutorsRepository;
        private readonly ISkillsRepository _skillsRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TutorsService(ITutorsRepository tutorsRepository, ISkillsRepository skillsRepository, IMapper mapper, IConfiguration configuration)
        {
            _tutorsRepository = tutorsRepository;
            _skillsRepository = skillsRepository;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task AddAsync(AddTutorModel addedTutor)
        {
            Tutor tutor = _mapper.Map<AddTutorModel, Tutor>(addedTutor);
            List<Skill> skills = await _skillsRepository.GetByIdsAsync(addedTutor.SkillsIds);
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
        public async Task<List<TutorDTO>> GetAsync(TutorsFilterModel tutorsFilter)
        {
            List<Tutor> tutors = await _tutorsRepository.GetAsync(tutorsFilter);
            List<TutorDTO> tutorDTOs = _mapper.Map<List<Tutor>, List<TutorDTO>>(tutors);
            string serverAddress = _configuration["BaseServerAddress"] ?? throw new ConfigurationErrorsException("BaseServerAddress is null");
            foreach (var tutor in tutorDTOs)
                tutor.LogoUrl = $"{serverAddress}/tutors/{tutor.Id}/logo";

            return tutorDTOs;
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
            if (tutor.Logo != null)
                return tutor.Logo;

            string? defultTutorLogoName = _configuration["DefultTutorLogoName"];
            if (string.IsNullOrEmpty(defultTutorLogoName))
                throw new ConfigurationErrorsException("DefultTutorLogoName is null or empty");
            string? basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (basePath == null)
                throw new Exception("ExecutingAssembly location is null");
            string? defultTutorLogoPath = Directory.GetFiles(basePath,defultTutorLogoName,SearchOption.AllDirectories).FirstOrDefault();
            if (string.IsNullOrEmpty(defultTutorLogoPath))
                throw new FileNotFoundException("defultTutorLogo not found");

            byte[] defultTutorLogo = await File.ReadAllBytesAsync(defultTutorLogoPath);

            return defultTutorLogo;
        }
    }
}
