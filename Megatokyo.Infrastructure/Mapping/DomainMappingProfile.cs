using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;

namespace Megatokyo.Infrastructure.Mapping
{
    public class DomainMappingProfile : Profile
    {
        public DomainMappingProfile()
        {
            CreateMap<StripEntity, StripDomain>()
                .ConstructUsing(stripEntity => new StripDomain(stripEntity.Category, stripEntity.Number, stripEntity.Title, stripEntity.Url, stripEntity.PublishDate))
                .ReverseMap();

            CreateMap<ChapterEntity, ChapterDomain>()
                .ConstructUsing(chapterEntity => new ChapterDomain(chapterEntity.Number, chapterEntity.Title, chapterEntity.Category))
                .ReverseMap();

            CreateMap<RantEntity, RantDomain>()
                .ConstructUsing(rantEntity => new RantDomain(rantEntity.Title, rantEntity.Number, rantEntity.Author, rantEntity.Url, rantEntity.PublishDate, rantEntity.Content))
                .ReverseMap();

            CreateMap<CheckingEntity, CheckingDomain>()
                .ConstructUsing(checkingEntity => new CheckingDomain(checkingEntity.LastCheck, checkingEntity.LastRantNumber, checkingEntity.LastStripNumber))
                .ReverseMap();
        }
    }
}
