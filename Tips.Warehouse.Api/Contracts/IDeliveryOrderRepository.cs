using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Macs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IDeliveryOrderRepository : IRepositoryBase<DeliveryOrder>
    {
        Task<PagedList<DeliveryOrder>> GetAllDeliveryOrders(PagingParameter pagingParameter, SearchParams searchParams);
        Task<DeliveryOrder> GetDeliveryOrderById(int id);
        Task<int?> GetDONumberAutoIncrementCount(DateTime date);
        Task<PagedList<DeliveryOrder>> GetAllActiveDeliveryOrders(PagingParameter pagingParameter, SearchParams searchParams);
        Task<long> CreateDeliveryOrder(DeliveryOrder deliveryOrder);
        Task<string> UpdateDeliveryOrder(DeliveryOrder deliveryOrder);
        Task<string> DeleteDeliveryOrder(DeliveryOrder deliveryOrder); 
        Task<IEnumerable<DeliveryOrder>> GetAllDeliveryOrderWithItems(DeliveryOrderSearchDto DeliveryOrderSearch);
        Task<IEnumerable<DeliveryOrder>> SearchDeliveryOrder([FromQuery] SearchParames searchParames);
        Task<IEnumerable<DeliveryOrder>> SearchDeliveryOrderDate([FromQuery] SearchsDateParms searchsDateParms);
        Task<IEnumerable<DeliveryOrderIdNameList>> GetAllDeliveryOrderIdNameList();

    }
}
