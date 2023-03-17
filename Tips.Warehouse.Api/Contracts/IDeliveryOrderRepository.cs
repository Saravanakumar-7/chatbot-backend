using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IDeliveryOrderRepository : IRepositoryBase<DeliveryOrder>
    {
        Task<PagedList<DeliveryOrder>> GetAllDeliveryOrders(PagingParameter pagingParameter, SearchParams searchParams);
        Task<DeliveryOrder> GetDeliveryOrderById(int id);
        Task<int?> GetDONumberAutoIncrementCount(DateTime date);
        Task<PagedList<DeliveryOrder>> GetAllActiveDeliveryOrders(PagingParameter pagingParameter, SearchParams searchParams);
        Task<long> CreateDeliveryOrder(DeliveryOrder deliveryOrder);
        Task<string> UpdateDeliveryOrder(DeliveryOrder deliveryOrder);
        Task<string> DeleteDeliveryOrder(DeliveryOrder deliveryOrder);
        
    }
}
