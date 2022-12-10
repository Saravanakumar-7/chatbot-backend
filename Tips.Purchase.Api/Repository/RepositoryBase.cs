using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;
using Tips.Purchase.Api.Entities;

namespace Tips.Purchase.Api.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {    
        protected TipsPurchaseDbContext _tipsPurchaseDbContext { get; set; }
        public RepositoryBase(TipsPurchaseDbContext repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
        }
        public IQueryable<T> FindAll()
        {

            var result = _tipsPurchaseDbContext.Set<T>().AsNoTracking();
            return result;
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {

            var result = _tipsPurchaseDbContext.Set<T>().Where(expression).AsNoTracking();
            return result;
        }
        public async Task<T> Create(T entity)
        {

            var result = await _tipsPurchaseDbContext.Set<T>().AddAsync(entity);

            return result.Entity;
        }
        public void Update(T entity)
        {

            _tipsPurchaseDbContext.Set<T>().Update(entity);

        }
        public void Delete(T entity)
        {

            _tipsPurchaseDbContext.Set<T>().Remove(entity);

        }
        public void SaveAsync()
        {
            _tipsPurchaseDbContext.SaveChanges();
        }
    }

}
