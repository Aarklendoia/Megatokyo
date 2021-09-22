using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Server.DTO.v1;

namespace Megatokyo.Server.Mapping
{
    public class DTOMappingProfile : Profile
    {
        public DTOMappingProfile()
        {
            CreateMap<StripOutputDTO, Strip>()
                    .ConstructUsing(stripOutputDTO => new Strip(stripOutputDTO.Category, stripOutputDTO.Number, stripOutputDTO.Title, stripOutputDTO.Url, stripOutputDTO.PublishDate))
                    .ReverseMap();

            CreateMap<ChapterOutputDTO, Chapter>()
                    .ConstructUsing(chapterOutputDTO => new Chapter(chapterOutputDTO.Number, chapterOutputDTO.Title, chapterOutputDTO.Category))
                    .ReverseMap();

            CreateMap<RantOutputDTO, Rant>()
                .ConstructUsing(rantOutputDTO => new Rant(rantOutputDTO.Title, rantOutputDTO.Number, rantOutputDTO.Author, rantOutputDTO.Url, rantOutputDTO.PublishDate, rantOutputDTO.Content))
                .ReverseMap();
        }
    }
}
