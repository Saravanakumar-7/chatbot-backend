using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;

namespace Tips.Grin.Api.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private TipsGrinDbContext _tipsGrinDbContext { get; set; }
        public RepositoryBase(TipsGrinDbContext repositoryContext)
        {
            _tipsGrinDbContext = repositoryContext;
        }
        public async Task<T> Create(T entity)
        {
            var result = await _tipsGrinDbContext.Set<T>().AddAsync(entity);

            return result.Entity;
        }

        public void Delete(T entity)
        {
            _tipsGrinDbContext.Set<T>().Remove(entity);
        }

        public IQueryable<T> FindAll()
        {
            var result = _tipsGrinDbContext.Set<T>().AsNoTracking();
            return result;
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            var result = _tipsGrinDbContext.Set<T>().Where(expression).AsNoTracking();
            return result;
        }

        public void Update(T entity)
        {
            _tipsGrinDbContext.Set<T>().Update(entity);
        }

        public void SaveAsync()
        {
            _tipsGrinDbContext.SaveChanges();
        }
    }
}
