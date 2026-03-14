using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
        public  IQueryable<T> FindAll()
        { 
            var result =  TipsMasterDbContext.Set<T>().AsNoTracking();
            return result;
        }
        public  IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            var result =  TipsMasterDbContext.Set<T>().Where(expression).AsNoTracking();
            return result;
        }
        public async Task<T> Create(T entity)
        {     
            var result = await TipsMasterDbContext.Set<T>().AddAsync(entity);     
            return result.Entity;
        }
        public void Update(T entity)
        {
            
            TipsMasterDbContext.Set<T>().Update(entity);
            
        }
        public void Delete(T entity)
        {
            
            TipsMasterDbContext.Set<T>().Remove(entity);
            
        }
    }
}