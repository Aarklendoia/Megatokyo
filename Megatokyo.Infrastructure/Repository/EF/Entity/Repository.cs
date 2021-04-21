using Megatokyo.Infrastructure.Repository.EF;
using Megatokyo.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Megatokyo.Infrastructure.EF.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected APIContext RepositoryContext { get; set; }

        public virtual async Task<IEnumerable<T>> FindAllAsync()
        {
            return await RepositoryContext.Set<T>().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
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