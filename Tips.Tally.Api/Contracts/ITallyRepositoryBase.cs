using System.Linq.Expressions;

namespace Tips.Tally.Api.Contracts
{
    public interface ITallyRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        Task<T> Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
