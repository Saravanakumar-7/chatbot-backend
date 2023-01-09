using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IBTODeliveryOrderItemsRepository : IRepositoryBase<BTODeliveryOrderItems>
    {
        Task<PagedList<BTODeliveryOrderItems>> GetAllBTODeliveryOrderItems(PagingParameter pagingParameter);
        Task<BTODeliveryOrderItems> GetBTODeliveryOrderItemById(int id);
        
        Task<IEnumerable<BTODeliveryOrderItems>> GetAllActiveBTODeliveryOrderItems();
        Task<long> CreateBTODeliveryOrderItem(BTODeliveryOrderItems bTODeliveryOrderItems);
        Task<string> UpdateBTODeliveryOrderItem(BTODeliveryOrderItems bTODeliveryOrderItems);
        Task<string> DeleteBTODeliveryOrderItem(BTODeliveryOrderItems bTODeliveryOrderItems);
        
    }
}
