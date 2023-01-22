using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IBTODeliveryOrderRepository : IRepositoryBase<BTODeliveryOrder>
    {
        Task<PagedList<BTODeliveryOrder>> GetAllBTODeliveryOrders(PagingParameter pagingParameter);
        Task<BTODeliveryOrder> GetBTODeliveryOrderById(int id);

        Task<IEnumerable<BTODeliveryOrder>> GetAllActiveBTODeliveryOrders();
        Task<long> CreateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        Task<int?> GetBTONumberAutoIncrementCount(DateTime date);
        Task<IEnumerable<ListofBtoDeliveryOrderDetails>> GetDeliveryOrderdetailsByProjectNo(string ProjectNo);
        Task<string> UpdateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        Task<string> DeleteBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        
    }
}
