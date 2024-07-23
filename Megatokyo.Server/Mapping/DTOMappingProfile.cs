using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Server.Dto.v1;

namespace Megatokyo.Server.Mapping
{
    public class DTOMappingProfile : Profile
    {
        public DTOMappingProfile()
        {
            CreateMap<Strip, StripOutputDto>()
                    .ConstructUsing(dest => new StripOutputDto()
                    {
                        Category = dest.Category,
                        Number = dest.Number,
                        PublishDate = dest.PublishDate,
                        Title = dest.Title,
                        Url = dest.Url
                    });

            CreateMap<Chapter, ChapterOutputDto>()
                    .ConstructUsing(dest => new ChapterOutputDto()
                    {
                        Category = dest.Category,
                        Number = dest.Number,
                        Title = dest.Title
                    });

            CreateMap<Rant, RantOutputDto>()
                .ConstructUsing(dest => new RantOutputDto()
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
