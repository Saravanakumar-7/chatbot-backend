using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IDeliveryOrderRepository : IRepositoryBase<DeliveryOrder>
    {
        Task<PagedList<DeliveryOrder>> GetAllDeliveryOrders(PagingParameter pagingParameter);
        Task<DeliveryOrder> GetDeliveryOrderById(int id);
        Task<int?> GetDONumberAutoIncrementCount(DateTime date);
        Task<IEnumerable<DeliveryOrder>> GetAllActiveDeliveryOrders();
        Task<long> CreateDeliveryOrder(DeliveryOrder deliveryOrder);
        Task<string> UpdateDeliveryOrder(DeliveryOrder deliveryOrder);
        Task<string> DeleteDeliveryOrder(DeliveryOrder deliveryOrder);
        
    }
}
