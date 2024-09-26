using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastCustomGroupRepository : IRepositoryBase<ForeCastCustomGroup>
    {
        Task<PagedList<ForeCastCustomGroup>> GetAllForeCastCustomGroup(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<ForeCastCustomGroup> GetForeCastCustomGroupById(int id);
        Task<int?> CreateForeCastCustomGroup(ForeCastCustomGroup foreCastCustomGroup);
        Task<string> UpdateForeCastCustomGroup(ForeCastCustomGroup foreCastCustomGroup);
        Task<string> DeleteForeCastCustomGroup(ForeCastCustomGroup foreCastCustomGroup);

        Task<IEnumerable<ListOfForecastCustomGroupDto>> GetAllForecastCustomGroupList();
    }
}
