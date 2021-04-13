using Megatokyo.Server.Database.Contract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Megatokyo.Server.Database.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected MegatokyoDbContext RepositoryContext { get; set; }

        public virtual async Task<IEnumerable<T>> FindAllAsync()
        {
            return await RepositoryContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await RepositoryContext.Set<T>().Where(expression).ToListAsync();
        }

        public void Create(T entity)
        {
            RepositoryContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            RepositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            RepositoryContext.Set<T>().Remove(entity);
        }

        public async Task SaveAsync()
        {
            await RepositoryContext.SaveChangesAsync();
        }

        public async Task<T> LatestAsync(Expression<Func<T, DateTime>> expression)
        {
            return await RepositoryContext.Set<T>().OrderByDescending(expression).FirstAsync();
        }
    }
}