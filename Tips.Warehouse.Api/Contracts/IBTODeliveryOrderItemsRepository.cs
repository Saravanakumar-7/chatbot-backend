using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IBTODeliveryOrderItemsRepository : IRepositoryBase<BTODeliveryOrderItems>
    {
        Task<PagedList<BTODeliveryOrderItems>> GetAllBTODeliveryOrderItems(PagingParameter pagingParameter);
        Task<BTODeliveryOrderItems> GetBTOBTODeliveryOrderItemsById(int id);
        
        Task<IEnumerable<BTODeliveryOrderItems>> GetAllActiveBTODeliveryOrderItems();
        Task<long> CreateBTODeliveryOrderItems(BTODeliveryOrderItems bTODeliveryOrderItems);
        Task<string> UpdateBTODeliveryOrderItems(BTODeliveryOrderItems bTODeliveryOrderItems);
        Task<string> DeleteBTODeliveryOrderItems(BTODeliveryOrderItems bTODeliveryOrderItems);
        
    }
}
