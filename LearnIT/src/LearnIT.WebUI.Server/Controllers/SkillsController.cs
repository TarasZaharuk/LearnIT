﻿using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LearnIT.WebUI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SkillsController : ControllerBase
    {

        private readonly ISkillsRepository _skillsRepository;

        public SkillsController(ISkillsRepository skillsRepository)
        {
            _skillsRepository = skillsRepository;
        }

        [HttpGet("/skills")]
        public async Task<List<GeneralSkill>> GetSkillsAsync()
        {
            return await _skillsRepository.GetAllAsync();
        }

        [HttpPost("/skill")]
        public async Task AddSkill(GeneralSkill skill)
        {
            await _skillsRepository.AddAsync(skill);
        }
    }
}
