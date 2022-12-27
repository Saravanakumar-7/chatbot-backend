using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IDeliveryOrderItemsRepository : IRepositoryBase<DeliveryOrderItems>
    {
        Task<PagedList<DeliveryOrderItems>> GetAllDeliveryOrderItems(PagingParameter pagingParameter);
        Task<DeliveryOrderItems> GetDeliveryOrderItemsById(int id);
        //Task<DeliveryOrderItems> GetDeliveryOrderByPONumber(string PONumber);
        Task<IEnumerable<DeliveryOrderItems>> GetAllActiveDeliveryOrderItems();
        Task<long> CreateDeliveryOrderItems(DeliveryOrderItems deliveryOrderItems);
        Task<string> UpdateDeliveryOrderItems(DeliveryOrderItems deliveryOrderItems);
        Task<string> DeleteDeliveryOrderItems(DeliveryOrderItems deliveryOrderItems);
        //Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllActiveBTODeliveryOrderNameList();
    }
}
