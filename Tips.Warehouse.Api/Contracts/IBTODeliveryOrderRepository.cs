using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IBTODeliveryOrderRepository : IRepositoryBase<BTODeliveryOrder>
    {
        Task<PagedList<BTODeliveryOrder>> GetAllBTODeliveryOrder(PagingParameter pagingParameter);
        Task<BTODeliveryOrder> GetBTODeliveryOrderById(int id);

        Task<IEnumerable<BTODeliveryOrder>> GetAllActiveBTODeliveryOrder();
        Task<long> CreateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        Task<string> UpdateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        Task<string> DeleteBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        
    }
}
