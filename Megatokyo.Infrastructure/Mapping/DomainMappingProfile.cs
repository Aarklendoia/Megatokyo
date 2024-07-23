using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;

namespace Megatokyo.Infrastructure.Mapping
{
    public class DomainMappingProfile : Profile
    {
        public DomainMappingProfile()
        {
            CreateMap<Strip, StripEntity>()
                .ForMember(stripEntity => stripEntity.Id, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Chapter, ChapterEntity>()
                .ForMember(chapterEntity => chapterEntity.Id, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Rant, RantEntity>()
                .ForMember(rantEntity => rantEntity.Id, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Checking, CheckingEntity>()
                .ForMember(checkingEntity => checkingEntity.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
