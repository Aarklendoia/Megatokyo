using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;

namespace Megatokyo.Infrastructure.Mapping
{
    public class DomainMappingProfile : Profile
    {
        public DomainMappingProfile()
        {
            CreateMap<StripEntity, Strip>()
                .ConstructUsing(stripEntity => new Strip(stripEntity.Category, stripEntity.Number, stripEntity.Title, stripEntity.Url, stripEntity.PublishDate))
                .ReverseMap();

            CreateMap<ChapterEntity, Chapter>()
                .ConstructUsing(chapterEntity => new Chapter(chapterEntity.Number, chapterEntity.Title, chapterEntity.Category))
                .ReverseMap();

            CreateMap<RantEntity, Rant>()
                .ConstructUsing(rantEntity => new Rant(rantEntity.Title, rantEntity.Number, rantEntity.Author, rantEntity.Url, rantEntity.PublishDate, rantEntity.Content))
                .ReverseMap();

            CreateMap<CheckingEntity, Checking>()
                .ConstructUsing(checkingEntity => new Checking(checkingEntity.LastCheck, checkingEntity.LastRantNumber, checkingEntity.LastStripNumber))
                .ReverseMap();
        }
    }
}
