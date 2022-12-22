using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastCustomGroupRepository : IRepositoryBase<ForeCastCustomGroup>
    {
        Task<PagedList<ForeCastCustomGroup>> GetAllForeCastCustomGroup(PagingParameter pagingParameter);
        Task<ForeCastCustomGroup> GetForeCastCustomGroupById(int id);
        Task<int?> CreateForeCastCustomGroup(ForeCastCustomGroup foreCastCustomGroup);
        Task<string> UpdateForeCastCustomGroup(ForeCastCustomGroup foreCastCustomGroup);
        Task<string> DeleteForeCastCustomGroup(ForeCastCustomGroup foreCastCustomGroup);
    }
}
