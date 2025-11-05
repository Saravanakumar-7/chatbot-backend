using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tips.Tally.Api.Contracts;
using Tips.Tally.Api.Entities;

namespace Tips.Tally.Api.Repository
{
    public abstract class TallyRepositoryBase<T> : ITallyRepositoryBase<T> where T : class
    {
        protected TipsTallyDbContext TipsTallyDbContext { get; set; }
        public TallyRepositoryBase(TipsTallyDbContext repositoryContext)
        {
            TipsTallyDbContext = repositoryContext;
        }
        public IQueryable<T> FindAll()
        {

            var result = TipsTallyDbContext.Set<T>().AsNoTracking();
            return result;
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {

            var result = TipsTallyDbContext.Set<T>().Where(expression).AsNoTracking();
            return result;
        }
        public async Task<T> Create(T entity)
        {

            var result = await TipsTallyDbContext.Set<T>().AddAsync(entity);

            return result.Entity;
        }
        public void Update(T entity)
        {

            TipsTallyDbContext.Set<T>().Update(entity);

        }
        public void Delete(T entity)
        {

            TipsTallyDbContext.Set<T>().Remove(entity);

        }
    }
}
