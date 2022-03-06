using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Server.DTO.v1;

namespace Megatokyo.Server.Mapping
{
    public class DTOMappingProfile : Profile
    {
        public DTOMappingProfile()
        {
            CreateMap<Strip, StripOutputDTO>()
                    .ConstructUsing(dest => new StripOutputDTO()
                    {
                        Category = dest.Category,
                        Number = dest.Number,
                        PublishDate = dest.PublishDate,
                        Title = dest.Title,
                        Url = dest.Url
                    });

            CreateMap<Chapter, ChapterOutputDTO>()
                    .ConstructUsing(dest => new ChapterOutputDTO()
                    {
                        Category = dest.Category,
                        Number = dest.Number,
                        Title = dest.Title
                    });

            CreateMap<Rant, RantOutputDTO>()
                .ConstructUsing(dest => new RantOutputDTO()
                {
                    Author = dest.Author,
                    Content = dest.Content,
                    Number = dest.Number,
                    PublishDate = dest.PublishDate,
                    Title = dest.Title,
                    Url = dest.Url
                });
        }
    }
}
