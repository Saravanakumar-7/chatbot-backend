using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IOpenDeliveryOrderHistoryRepository : IRepositoryBase<OpenDeliveryOrderHistory>
    {
        Task<long> CreateOpenDeliveryOrderHistory(OpenDeliveryOrderHistory openDeliveryOrderHistory);
        Task<PagedList<OpenDeliveryOrderHistory>> GetAllOpenDeliveryOrderHistoryDetails(PagingParameter pagingParameter);

        Task<OpenDeliveryOrderHistory> GetOpenDeliveryOrderHistoryDetailsById(int id);

        Task<IEnumerable<OpenDeliveryOrderHistory>> GetOpenDeliveryOrderHistoryDetailsByBtoNo(string odoNumber, string uniqueId);
    }
}
