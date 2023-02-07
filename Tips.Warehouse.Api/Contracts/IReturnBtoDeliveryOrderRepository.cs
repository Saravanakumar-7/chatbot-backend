using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IReturnBtoDeliveryOrderRepository : IRepositoryBase<ReturnBtoDeliveryOrder>
    {
        Task<PagedList<ReturnBtoDeliveryOrder>> GetAllReturnBtoDeliveryOrders(PagingParameter pagingParameter);

        Task<ReturnBtoDeliveryOrder> GetReturnBtoDeliveryOrderById(int id);
        Task<int?> CreateReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder);
        Task<string> UpdateReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder);
        Task<string> DeleteReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder);

    }
}
