using AutoMapper;
using LearnIT.Application.DTOs;
using Shared;
using LearnIT.Domain.Entities;

namespace LearnIT.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(u => u.Gender, cfg => cfg.MapFrom(u => u.Gender.Name));
            CreateMap<AddUserModel, User>()
                .ForMember(u => u.GenderId, cfg => cfg.MapFrom(u => u.GenderId))
                .ForMember(u => u.Id, cfg => cfg.Ignore());
        }

    }
}
