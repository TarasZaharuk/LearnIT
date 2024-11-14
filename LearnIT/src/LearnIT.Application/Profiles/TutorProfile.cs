using AutoMapper;
using LearnIT.Application.DTOs;
using LearnIT.Application.Models;
using LearnIT.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace LearnIT.Application.Profiles
{
    public class TutorProfile : Profile
    {
        public TutorProfile()
        {
            CreateMap<string, TutorSkill>()
                .ForMember(ts => ts.SkillName, cfg => cfg.MapFrom(s => s));
            CreateMap<TutorSkill, SkillDTO>()
                .ForMember(sd => sd.SkillName, cfg => cfg.MapFrom(s => s.SkillName))
                .ForMember(sd => sd.Id, cfg => cfg.MapFrom(s => s.Id));

            CreateMap<AddTutorModel, Tutor>()
                .ForMember(t => t.Id, cfg => cfg.Ignore())
                .ForMember(t => t.UserId, cfg => cfg.MapFrom(at => at.UserId));
            CreateMap<Tutor, TutorDTO>()
                .ForMember(td => td.Id, cfg => cfg.MapFrom(t => t.Id))
                .ForMember(td => td.User, cfg => cfg.MapFrom(t => t.User))
                .ForMember(td => td.Skills, cfg => cfg.MapFrom(t => t.Skills));
        }
    }
}

