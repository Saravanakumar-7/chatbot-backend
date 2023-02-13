using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IBTODeliveryOrderHistoryRepository : IRepositoryBase<BTODeliveryOrderHistory>
    {
        Task<long> CreateBTODeliveryOrderHistory(BTODeliveryOrderHistory bTODeliveryOrderHistory);
        Task<PagedList<BTODeliveryOrderHistory>> GetAllBtoHistoryDetails(PagingParameter pagingParameter);

        Task<BTODeliveryOrderHistory> GetBtoHistoryDetailsById(int id);

        Task<IEnumerable<BTODeliveryOrderHistory>> GetBtoHistoryDetailsByBtoNo(string btoNumber, string uniqueId);

    }
}
