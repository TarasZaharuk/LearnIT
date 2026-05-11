using AutoMapper;
using LearnIT.Application.DTOs;
using LearnIT.Domain.Entities;

namespace LearnIT.Application.Profiles
{
    public class ConsultationProfile : Profile
    {
        public ConsultationProfile()
        {
            CreateMap<AddConsultationModel, Consultation>();
            CreateMap<UpdateConsultationModel, Consultation>();
            CreateMap<Consultation, ConsultationDTO>();
        }
    }
}
