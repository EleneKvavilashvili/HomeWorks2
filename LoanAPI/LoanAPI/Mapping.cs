using AutoMapper;
using LoanAPI.DTOs;
using LoanAPI.Models;

namespace LoanAPI
{
    public class Mapping: Profile
    {
        public Mapping()
        {
            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.IsBlocked, opt => opt.MapFrom(src => false));
            CreateMap<UpdateDTO, User>();
        }
    }
}
