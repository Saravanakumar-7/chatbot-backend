using Entities;
using Entities.Helper;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface ILocationTransferRepository : IRepositoryBase<LocationTransfer>
    {
        Task<int?> CreateLocationTransfer(LocationTransfer locationTransfer);
        Task<string> UpdateLocationTransfer(LocationTransfer locationTransfer);
        Task<string> DeleteLocationTransfer(LocationTransfer locationTransfer);
        Task<PagedList<LocationTransfer>> GetAllLocationTransfer(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<LocationTransfer> GetLocationTransferById(int id);

        Task<IEnumerable<LocationTransferIdNameList>> GetAllLocationTransferIdNameList();

    }
}