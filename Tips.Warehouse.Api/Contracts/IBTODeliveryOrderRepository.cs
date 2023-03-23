using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IBTODeliveryOrderRepository : IRepositoryBase<BTODeliveryOrder>
    {
        Task<PagedList<BTODeliveryOrder>> GetAllBTODeliveryOrders(PagingParameter pagingParameter, SearchParams searchParams);
        Task<BTODeliveryOrder> GetBTODeliveryOrderById(int id);

        Task<IEnumerable<ListOfBtoNumberDetails>> GetBtoNumberListByCustomerId(string customerLeadId);
        Task<BTODeliveryOrder> GetBtoDetailsByBtoNo(string BTONumber);

        Task<PagedList<BTODeliveryOrder>> GetAllActiveBTODeliveryOrders(PagingParameter pagingParameter, SearchParams searchParams);
        Task<long> CreateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        Task<int?> GetBTONumberAutoIncrementCount(DateTime date);
        Task<IEnumerable<ListofBtoDeliveryOrderDetails>> GetBtoDeliveryOrderNumberList();
        Task<string> UpdateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        Task<string> DeleteBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        Task<IEnumerable<BtoIDNameList>> GetAllBTOIdNameIdNameList();


    }
}
