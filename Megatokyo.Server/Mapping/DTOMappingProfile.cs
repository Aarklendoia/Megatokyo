using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Server.DTO.v1;

namespace Megatokyo.Server.Mapping
{
    public class DTOMappingProfile : Profile
    {
        public DTOMappingProfile()
        {
            CreateMap<StripOutputDTO, StripDomain>()
                    .ConstructUsing(stripEntity => new StripDomain(stripEntity.Category, stripEntity.Number, stripEntity.Title, stripEntity.Url, stripEntity.PublishDate))
                    .ReverseMap();

            CreateMap<ChapterOutputDTO, ChapterDomain>()
                    .ConstructUsing(chapterEntity => new ChapterDomain(chapterEntity.Number, chapterEntity.Title, chapterEntity.Category))
                    .ReverseMap();

            CreateMap<RantOutputDTO, RantDomain>()
                .ConstructUsing(rantEntity => new RantDomain(rantEntity.Title, rantEntity.Number, rantEntity.Author, rantEntity.Url, rantEntity.PublishDate, rantEntity.Content))
                .ReverseMap();
        }
    }
}
