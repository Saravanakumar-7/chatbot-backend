using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IDeliveryOrderRepository : IRepositoryBase<DeliveryOrder>
    {
        Task<PagedList<DeliveryOrder>> GetAllDeliveryOrder(PagingParameter pagingParameter);
        Task<DeliveryOrder> GetDeliveryOrderById(int id);
        
        Task<IEnumerable<DeliveryOrder>> GetAllActiveDeliveryOrder();
        Task<long> CreateDeliveryOrder(DeliveryOrder deliveryOrder);
        Task<string> UpdateDeliveryOrder(DeliveryOrder deliveryOrder);
        Task<string> DeleteDeliveryOrder(DeliveryOrder deliveryOrder);
        
    }
}
