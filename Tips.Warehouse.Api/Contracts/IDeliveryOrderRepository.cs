using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IDeliveryOrderRepository : IRepositoryBase<DeliveryOrder>
    {
        Task<PagedList<DeliveryOrder>> GetAllDeliveryOrder(PagingParameter pagingParameter, string DeliveryOrderNumber);
        Task<DeliveryOrder> GetDeliveryOrderById(int id, string DeliveryOrderNumber);
        //Task<DeliveryOrder> GetDeliveryOrderByPONumber(string PONumber);
        Task<IEnumerable<DeliveryOrder>> GetAllActiveDeliveryOrder();
        Task<long> CreateDeliveryOrder(DeliveryOrder deliveryOrder);
        Task<string> UpdateDeliveryOrder(DeliveryOrder deliveryOrder, string DeliveryOrderNumber);
        Task<string> DeleteDeliveryOrder(DeliveryOrder deliveryOrder);
        //Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllActiveBTODeliveryOrderNameList();
    }
}
