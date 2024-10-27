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
            CreateMap<AddTutorModel, Tutor>()
                .ForMember(t => t.Id, cfg => cfg.Ignore())
                .ForMember(t => t.UserId, cfg => cfg.MapFrom(at => at.UserId));
            CreateMap<Tutor, TutorDTO>()
                .ForMember(td => td.Id, cfg => cfg.MapFrom(t => t.Id))
                .ForMember(td => td.User, cfg => cfg.MapFrom(t => t.User));
        }
    }
}

