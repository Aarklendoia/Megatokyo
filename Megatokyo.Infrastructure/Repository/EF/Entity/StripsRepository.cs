using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Megatokyo.Infrastructure.Repository.EF.Entity;
using Megatokyo.Domain;
using Megatokyo.Infrastructure.Repository.EF;
using System.Linq.Expressions;
using System;

namespace Megatokyo.Infrastructure.EF.Repository
{
    public class StripsRepository
    {
        private readonly APIContext _Context;
        public DbSet<StripEntity> DbSet { get; }

        public StripsRepository(APIContext dataContext)
        {
            _Context = dataContext;
            DbSet = _Context.Set<StripEntity>();
        }

        public async Task<IEnumerable<StripDomain>> FindAllAsync()
        {
            IEnumerable<StripEntity> strips = await DbSet.Include(strip => strip.Chapter).ToListAsync();
            return strips.Select(strip => new StripDomain(
                new ChapterDomain(strip.Chapter.Id, strip.Chapter.Number, strip.Chapter.Title, strip.Chapter.Category),
                strip.Number,
                strip.Title,
                strip.Url,
                strip.DateTime
                )
            );
        }

        public async Task<IEnumerable<StripDomain>> FindByConditionAsync(Expression<Func<StripEntity, bool>> expression)
        {
            IEnumerable<StripEntity> strips = await DbSet.Where(expression).Include(strip => strip.Chapter).ToListAsync();
            return strips.Select(strip => new StripDomain(
                new ChapterDomain(strip.Chapter.Id, strip.Chapter.Number, strip.Chapter.Title, strip.Chapter.Category),
                strip.Number,
                strip.Title,
                strip.Url,
                strip.DateTime
                )
            );
        }
    }
}