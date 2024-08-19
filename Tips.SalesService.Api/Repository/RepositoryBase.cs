
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext { get; set; }
        public RepositoryBase(TipsSalesServiceDbContext repositoryContext)
        {
            _tipsSalesServiceDbContext = repositoryContext;
        }
         
        public  IQueryable<T> FindAll()
        {

            var result = _tipsSalesServiceDbContext.Set<T>().AsNoTracking();
            return result;
        }
        public  IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            
            var result = _tipsSalesServiceDbContext.Set<T>().Where(expression).AsNoTracking();
            return result;
        }
        public async Task<T> Create(T entity) {
            
            var result = await _tipsSalesServiceDbContext.Set<T>().AddAsync(entity);
            
            return result.Entity;
        }
        public void Update(T entity)
        {

            _tipsSalesServiceDbContext.Set<T>().Update(entity);
            
        }
        public void Delete(T entity)
        {

            _tipsSalesServiceDbContext.Set<T>().Remove(entity);
            
        }
        public void SaveAsync()
        {
            _tipsSalesServiceDbContext.SaveChangesAsync();
        }
    }
}