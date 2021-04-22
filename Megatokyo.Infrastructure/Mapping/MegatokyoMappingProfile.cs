using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;

namespace Megatokyo.Infrastructure.Mapping
{
    public class MegatokyoMappingProfile : Profile
    {
        public MegatokyoMappingProfile()
        {
            CreateMap<StripEntity, StripDomain>()
                .ConstructUsing((stripEntity, ctx) => new StripDomain(ctx.Mapper.Map<ChapterDomain>(stripEntity), stripEntity.Number, stripEntity.Title, stripEntity.Url, stripEntity.DateTime))
                .ReverseMap();

            CreateMap<ChapterEntity, ChapterDomain>()
                .ConstructUsing(chapterEntity => new ChapterDomain(chapterEntity.Number, chapterEntity.Title, chapterEntity.Category))
                .ReverseMap();

            CreateMap<RantEntity, RantDomain>()
                .ConstructUsing(rantEntity => new RantDomain(rantEntity.Title, rantEntity.Number, rantEntity.Author, rantEntity.Url, rantEntity.TimeStamp, rantEntity.Content))
                .ReverseMap();

            CreateMap<CheckingEntity, CheckingDomain>()
                .ConstructUsing(checkingEntity => new CheckingDomain(checkingEntity.LastCheck, checkingEntity.LastRantNumber, checkingEntity.LastStripNumber))
                .ReverseMap();
        }
    }
}
