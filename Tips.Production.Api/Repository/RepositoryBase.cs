using Tips.Production.Api.Contracts;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Entities;


namespace Tips.Production.Api.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected TipsProductionDbContext _tipsProductionDbContext { get; set; }
        protected AdvitaTipsProductionDbContext _advitaTipsProductionDbContext { get; set; }

        public RepositoryBase(TipsProductionDbContext repositoryContext, AdvitaTipsProductionDbContext repositoryContext_1)
        {
            _tipsProductionDbContext = repositoryContext;
            _advitaTipsProductionDbContext = repositoryContext_1;
        }
        
        public IQueryable<T> FindAll()
        {

            var result = _tipsProductionDbContext.Set<T>().AsNoTracking();
            return result;
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {

            var result = _tipsProductionDbContext.Set<T>().Where(expression).AsNoTracking();
            return result;
        }
        public async Task<T> Create(T entity)
        {

            var result = await _tipsProductionDbContext.Set<T>().AddAsync(entity);

            return result.Entity;
        }       
        public async Task<T> CreateAdvita(T entity)
        {

            var result = await _advitaTipsProductionDbContext.Set<T>().AddAsync(entity);

            return result.Entity;
        }
        public void Update(T entity)
        {

            _tipsProductionDbContext.Set<T>().Update(entity);

        }
        public void Delete(T entity)
        {

            _tipsProductionDbContext.Set<T>().Remove(entity);

        }
        public void SaveAdvitaAsync()
        {
            _advitaTipsProductionDbContext.SaveChanges();
        }
        public void SaveAsync()
        {
            _tipsProductionDbContext.SaveChanges();
        }
    }
}
