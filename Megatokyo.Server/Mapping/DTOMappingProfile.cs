using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Server.DTO.v1;

namespace Megatokyo.Server.Mapping
{
    /// <summary>
    /// MApping for the DTO.
    /// </summary>
    public class DTOMappingProfile : Profile
    {
        /// <summary>
        /// Create a new instance of the mapping profile.
        /// </summary>
        public DTOMappingProfile()
        {
            CreateMap<StripOutputDTO, StripDomain>()
                    .ConstructUsing((stripEntity, ctx) => new StripDomain(ctx.Mapper.Map<ChapterDomain>(stripEntity), stripEntity.Number, stripEntity.Title, stripEntity.Url, stripEntity.Timestamp))
                    .ReverseMap();

            CreateMap<ChapterOutputDTO, ChapterDomain>()
                    .ConstructUsing(chapterEntity => new ChapterDomain(chapterEntity.Number, chapterEntity.Title, chapterEntity.Category))
                    .ReverseMap();

            CreateMap<RantOutputDTO, RantDomain>()
                .ConstructUsing(rantEntity => new RantDomain(rantEntity.Title, rantEntity.Number, rantEntity.Author, rantEntity.Url, rantEntity.Timestamp, rantEntity.Content))
                .ReverseMap();
        }
    }
}
