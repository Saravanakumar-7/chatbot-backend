using System.Linq.Expressions;

namespace Tips.Production.Api.Contracts
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        Task<T> Create(T entity);
        void Update(T entity);
        void Delete(T entity);

        public void SaveAsync();

    }
}
