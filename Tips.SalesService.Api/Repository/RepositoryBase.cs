
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected TipsSalesServiceDbContext TipsSalesServiceDbContext { get; set; }
        public RepositoryBase(TipsSalesServiceDbContext repositoryContext)
        {
            TipsSalesServiceDbContext = repositoryContext;
        }
        public  IQueryable<T> FindAll()
        {

            var result =  TipsSalesServiceDbContext.Set<T>().AsNoTracking();
            return result;
        }
        public  IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            
            var result =  TipsSalesServiceDbContext.Set<T>().Where(expression).AsNoTracking();
            return result;
        }
        public async Task<T> Create(T entity) {
            
            var result = await TipsSalesServiceDbContext.Set<T>().AddAsync(entity);
            
            return result.Entity;
        }
        public void Update(T entity)
        {
            
            TipsSalesServiceDbContext.Set<T>().Update(entity);
            
        }
        public void Delete(T entity)
        {
            
            TipsSalesServiceDbContext.Set<T>().Remove(entity);
            
        }
    }
}