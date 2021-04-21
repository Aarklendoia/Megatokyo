using AutoMapper;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megatokyo.Infrastructure.Mapping
{
    public class MegatokyoMappingProfile : Profile
    {
        public MegatokyoMappingProfile()
        {
            CreateMap<StripEntity, StripDomain>()
                .ConstructUsing((stripEntity, ctx) => new StripDomain(
                ctx.Mapper.Map<ChapterDomain>(stripEntity),
                 stripEntity.Number,
                 stripEntity.Title,
                 stripEntity.Url,
                 stripEntity.DateTime
                ))
                .ReverseMap();

            CreateMap<ChapterEntity, ChapterDomain>()
                .ConstructUsing(chapterEntity => new ChapterDomain(chapterEntity.Id, chapterEntity.Number, chapterEntity.Title, chapterEntity.Category))
                .ReverseMap();
        }
    }
}
