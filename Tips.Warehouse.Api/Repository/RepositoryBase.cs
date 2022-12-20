using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;

using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {    
        protected TipsWarehouseDbContext _tipsWarehouseDbContext { get; set; }
        public RepositoryBase(TipsWarehouseDbContext repositoryContext)
        {
            _tipsWarehouseDbContext = repositoryContext;
        }
        public IQueryable<T> FindAll()
        {

            var result = _tipsWarehouseDbContext.Set<T>().AsNoTracking();
            return result;
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {

            var result = _tipsWarehouseDbContext.Set<T>().Where(expression).AsNoTracking();
            return result;
        }
        public async Task<T> Create(T entity)
        {

            var result = await _tipsWarehouseDbContext.Set<T>().AddAsync(entity);

            return result.Entity;
        }
        public void Update(T entity)
        {

            _tipsWarehouseDbContext.Set<T>().Update(entity);

        }
        public void Delete(T entity)
        {

            _tipsWarehouseDbContext.Set<T>().Remove(entity);

        }
        public void SaveAsync()
        {
            _tipsWarehouseDbContext.SaveChanges();
        }
    }

}
