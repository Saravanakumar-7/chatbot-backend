using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected TipsMasterDbContext TipsMasterDbContext { get; set; }
        public RepositoryBase(TipsMasterDbContext repositoryContext)
        {
            TipsMasterDbContext = repositoryContext;
        }
        public IQueryable<T> FindAll() => TipsMasterDbContext.Set<T>().AsNoTracking();
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
            TipsMasterDbContext.Set<T>().Where(expression).AsNoTracking();
        public void Create(T entity) => TipsMasterDbContext.Set<T>().Add(entity);
        public void Update(T entity) => TipsMasterDbContext.Set<T>().Update(entity);
        public void Delete(T entity) => TipsMasterDbContext.Set<T>().Remove(entity);
    }
}