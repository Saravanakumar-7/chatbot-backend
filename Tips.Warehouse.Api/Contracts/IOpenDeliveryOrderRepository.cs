using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{   
    public interface IOpenDeliveryOrderRepository : IRepositoryBase<OpenDeliveryOrder>
    {
        Task<PagedList<OpenDeliveryOrder>> GetAllOpenDeliveryOrders(PagingParameter pagingParameter, SearchParams searchParams);
        Task<string> GenerateODONumber();
        Task<OpenDeliveryOrder> GetOpenDeliveryOrderById(int id);
        Task<int?> CreateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder);
        Task<string> UpdateOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder);
        Task<int?> GetODONumberAutoIncrementCount(DateTime date);

        Task<string> DeleteOpenDeliveryOrder(OpenDeliveryOrder openDeliveryOrder);

        Task<IEnumerable<OpenDeliveryOrderIdNameList>> GetAllOpenDeliveryOrderIdNameList();

        Task<IEnumerable<OpenDeliveryOrder>> GetAllOpenDeliveryOrderWithItems(OpenDeliveryOrderSearchDto OpenDeliveryOrderSearch);
        Task<IEnumerable<OpenDeliveryOrder>> SearchOpenDeliveryOrder([FromQuery] SearchParames searchParames);
        Task<IEnumerable<OpenDeliveryOrder>> SearchOpenDeliveryOrderDate([FromQuery] SearchsDateParms searchsDateParms);


    }

}
