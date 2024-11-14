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
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TutorsService(ITutorsRepository tutorsRepository, IMapper mapper, IConfiguration configuration)
        {
            _tutorsRepository = tutorsRepository;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<int> AddAsync(AddTutorModel addedTutor)
        {
            Tutor tutor = _mapper.Map<AddTutorModel, Tutor>(addedTutor);
            tutor.EntityState = GetEntityState(tutor);
            int tutorId = await _tutorsRepository.AddAsync(tutor);
            return tutorId;
        }

        public async Task UpdateSkillsAsync(AddTutorSkillsModel addTutorSkills)
        {
            var tutor = await _tutorsRepository.GetByIdAsync(addTutorSkills.TutorId);
            if (tutor == null)
                return;
            tutor.Skills = addTutorSkills.Skills.Select(s => new TutorSkill { SkillName = s }).ToList();
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
            List<Tutor> tutors = await _tutorsRepository.GetActiveAsync(tutorsFilter);
            List<TutorDTO> tutorDTOs = _mapper.Map<List<Tutor>, List<TutorDTO>>(tutors);
            string serverAddress = _configuration["BaseServerAddress"] ?? throw new ConfigurationErrorsException("BaseServerAddress is null");
            foreach (var tutor in tutorDTOs)
                tutor.LogoUrl = $"{serverAddress}/tutors/{tutor.Id}/logo";

            return tutorDTOs;
        }

        public async Task SetLogoAsync(int tutorId, byte[] logo)
        {
            Tutor tutor = await _tutorsRepository.GetByIdAsync(tutorId) ?? throw new Exception("Invalid 'TutorId'");
            tutor.Logo = logo;
            await _tutorsRepository.UpdateAsync(tutor);
        }

        public async Task<byte[]> GetLogoAsync(int tutorId)
        {
            Tutor tutor = await _tutorsRepository.GetByIdAsync(tutorId) ?? throw new Exception("Invalid 'TutorId'");
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

        private EntityState GetEntityState(Tutor tutor)
        {
            bool isDraft = string.IsNullOrEmpty(tutor.JobTitle) || string.IsNullOrEmpty(tutor.SummaryOfQualification);
            if (isDraft)
                return EntityState.Draft;

            return EntityState.Active;
        }

        public async Task UpdateGeneralInfoAsync(UpdateTutorGeneralInfoModel tutor)
        {
            Tutor? updatedTutor = await _tutorsRepository.GetByIdAsync(tutor.TutorId);
            if (updatedTutor == null)
                return;

            updatedTutor.WagePerHour = tutor.WagePerHour;
            updatedTutor.LinkedInUrl = tutor.LinkedInUrl;
            updatedTutor.GitHubUrl = tutor.GitHubUrl;
            updatedTutor.JobTitle = tutor.JobTitle;
            updatedTutor.SummaryOfQualification = tutor.SummaryOfQualification;
            updatedTutor.EntityState = GetEntityState(updatedTutor);

            await _tutorsRepository.UpdateAsync(updatedTutor);
        }

        public async Task<TutorDTO?> GetByIdAsync(int id)
        {
            Tutor? tutor = await _tutorsRepository.GetByIdAsync(id);
            if (tutor == null)
                return null;
            TutorDTO tutorDTO = _mapper.Map<Tutor,TutorDTO>(tutor);
            string serverAddress = _configuration["BaseServerAddress"] ?? throw new ConfigurationErrorsException("BaseServerAddress is null");
            tutorDTO.LogoUrl = $"{serverAddress}/tutors/{tutor.Id}/logo";
            return tutorDTO;
        }
    }
}
