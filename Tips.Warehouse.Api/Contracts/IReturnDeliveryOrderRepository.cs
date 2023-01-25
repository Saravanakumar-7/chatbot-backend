using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IReturnDeliveryOrderRepository : IRepositoryBase<ReturnDeliveryOrder>
    {
        Task<PagedList<ReturnDeliveryOrder>> GetAllReturnDeliveryOrders(PagingParameter pagingParameter);

        Task<ReturnDeliveryOrder> GetReturnDeliveryOrderById(int id);
        Task<int?> CreateReturnDeliveryOrder(ReturnDeliveryOrder returnDeliveryOrder);
        Task<string> UpdateReturnDeliveryOrder(ReturnDeliveryOrder returnDeliveryOrder);
        Task<string> DeleteReturnDeliveryOrder(ReturnDeliveryOrder returnDeliveryOrder);

    }
}
