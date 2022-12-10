using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IBTODeliveryOrderRepository : IRepositoryBase<BTODeliveryOrder>
    {
        Task<PagedList<BTODeliveryOrder>> GetAllBTODeliveryOrder(PagingParameter pagingParameter,string BTONumber);
        Task<BTODeliveryOrder> GetBTODeliveryOrderById(int id, string BTONumber);
        //Task<BTODeliveryOrder> GetBTODeliveryOrderByPONumber(string PONumber);
        Task<IEnumerable<BTODeliveryOrder>> GetAllActiveBTODeliveryOrder();
        Task<long> CreateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        Task<string> UpdateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder, string BTONumber);
        Task<string> DeleteBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        //Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllActiveBTODeliveryOrderNameList();
    }
}
