using AutoMapper;

using DomainEntities = Domain.Entities;
using MongoModels = Infrastructure.Persistence.Mongo.Models;
using SqlModels = Infrastructure.Persistence.Sql.Models;

namespace Infrastructure.Mapping
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            // Domain Claim <-> MongoDB Claim
            CreateMap<DomainEntities.Claim, MongoModels.Claim>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (MongoModels.ClaimType)(int)src.Type))
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (DomainEntities.ClaimType)(int)src.Type));

            // Domain Cover <-> MongoDB Cover
            CreateMap<DomainEntities.Cover, MongoModels.Cover>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (MongoModels.CoverType)(int)src.Type))
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (DomainEntities.CoverType)(int)src.Type));

            // Domain ClaimAudit <-> SQL ClaimAudit
            CreateMap<DomainEntities.ClaimAudit, SqlModels.ClaimAudit>()
                .ReverseMap();

            // Domain CoverAudit <-> SQL CoverAudit
            CreateMap<DomainEntities.CoverAudit, SqlModels.CoverAudit>()
                .ReverseMap();
        }
    }
}
