using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IReturnOpenDeliveryOrderRepository : IRepositoryBase<ReturnOpenDeliveryOrder>
    {
        Task<PagedList<ReturnOpenDeliveryOrder>> GetAllReturnOpenDeliveryOrderDetails(PagingParameter pagingParameter, SearchParams searchParams);
        Task<ReturnOpenDeliveryOrder> GetReturnOpenDeliveryOrderById(int id);
        Task<int?> CreateReturnOpenDeliveryOrder(ReturnOpenDeliveryOrder returnOpenDeliveryOrder);
        Task<string> UpdateReturnOpenDeliveryOrder(ReturnOpenDeliveryOrder returnOpenDeliveryOrder);
        Task<string> DeleteReturnOpenDeliveryOrder(ReturnOpenDeliveryOrder returnOpenDeliveryOrder);
        Task<int?> GetReturnOpenDeliveryOrderByBtoNo(string BTONumber);

    }
}
