using AutoMapper;
using Megatokyo.Client.Core.DTO;
using Megatokyo.Client.Core.Models;
using System;

namespace Megatokyo.Client.Core.Mappings
{
    internal class RantMappingProfile : Profile
    {
        RantMappingProfile()
        {
            CreateMap<RantDTO, Rant>()
                .ConvertUsing(dest => new Rant()
                {
                    Author = dest.Author,
                    Content = dest.Content,
                    Number = dest.Number,
                    PublishDate = dest.PublishDate,
                    Title = dest.Title,
                    Url = new Uri(dest.Url)
                });
        }
    }
}
