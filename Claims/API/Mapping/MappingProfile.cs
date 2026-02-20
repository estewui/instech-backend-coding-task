using AutoMapper;
using API.Contracts.Requests;
using API.Contracts.Responses;
using Domain.Entities;

namespace API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Claim mappings
            CreateMap<CreateClaimRequest, Claim>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID will be set by service/repository
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (Domain.Entities.ClaimType)(int)src.Type));

            CreateMap<Claim, ClaimResponse>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (API.Contracts.Types.ClaimType)(int)src.Type));

            // Cover mappings
            CreateMap<CreateCoverRequest, Cover>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID will be set by service/repository
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (Domain.Entities.CoverType)(int)src.Type));

            CreateMap<Cover, CoverResponse>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (API.Contracts.Types.CoverType)(int)src.Type));
        }
    }
}
