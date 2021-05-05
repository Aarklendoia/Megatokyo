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
                    .ConstructUsing(stripOutputDTO => new StripDomain(stripOutputDTO.Category, stripOutputDTO.Number, stripOutputDTO.Title, stripOutputDTO.Url, stripOutputDTO.PublishDate))
                    .ReverseMap();

            CreateMap<ChapterOutputDTO, ChapterDomain>()
                    .ConstructUsing(chapterOutputDTO => new ChapterDomain(chapterOutputDTO.Number, chapterOutputDTO.Title, chapterOutputDTO.Category))
                    .ReverseMap();

            CreateMap<RantOutputDTO, RantDomain>()
                .ConstructUsing(rantOutputDTO => new RantDomain(rantOutputDTO.Title, rantOutputDTO.Number, rantOutputDTO.Author, rantOutputDTO.Url, rantOutputDTO.PublishDate, rantOutputDTO.Content))
                .ReverseMap();
        }
    }
}
