using Entities;
using Entities.Helper;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ILocationTransferRepository : IRepositoryBase<LocationTransfer>
    {
        Task<int?> CreateLocationTransfer(LocationTransfer locationTransfer);
        Task<string> UpdateLocationTransfer(LocationTransfer locationTransfer);
        Task<string> DeleteLocationTransfer(LocationTransfer locationTransfer);
        Task<PagedList<LocationTransfer>> GetAllLocationTransfer(PagingParameter pagingParameter);
        Task<LocationTransfer> GetLocationTransferById(int id);
    }
}