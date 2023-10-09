using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IBTODeliveryOrderHistoryRepository : IRepositoryBase<BTODeliveryOrderHistory>
    {
        Task<long> CreateBTODeliveryOrderHistory(BTODeliveryOrderHistory bTODeliveryOrderHistory);
        Task<PagedList<BTODeliveryOrderHistory>> GetAllBtoHistoryDetails(PagingParameter pagingParameter, SearchParammes searchParamess);
        Task<PagedList<BTODeliveryOrderHistory>> GetAllReturnDeliveryOrder([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParams);
        Task<BTODeliveryOrderHistory> GetBtoHistoryDetailsById(int id);

        Task<IEnumerable<BTODeliveryOrderHistory>> GetBtoHistoryDetailsByBtoNo(string btoNumber, string uniqueId);
        Task<string> GetBTONumberCount(string btoNumber);

    }
}
