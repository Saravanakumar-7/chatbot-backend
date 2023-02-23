using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IDeliveryOrderItemsRepository : IRepositoryBase<DeliveryOrderItems>
    {
        Task<PagedList<DeliveryOrderItems>> GetAllDeliveryOrderItems(PagingParameter pagingParameter);
        Task<DeliveryOrderItems> GetDeliveryOrderItemById(int id);
       
        Task<IEnumerable<DeliveryOrderItems>> GetAllActiveDeliveryOrderItems();
        Task<long> CreateDeliveryOrderItem(DeliveryOrderItems deliveryOrderItems);
        Task<string> UpdateDeliveryOrderItem(DeliveryOrderItems deliveryOrderItems);
        Task<string> DeleteDeliveryOrderItem(DeliveryOrderItems deliveryOrderItems);
        
    }
}
