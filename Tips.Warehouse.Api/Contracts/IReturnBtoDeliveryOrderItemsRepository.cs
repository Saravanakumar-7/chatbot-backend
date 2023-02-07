using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IReturnBtoDeliveryOrderItemsRepository 
    {
        Task<PagedList<ReturnBtoDeliveryOrderItems>> GetAllReturnBtoDeliveryOrdersItems(PagingParameter pagingParameter);

        Task<ReturnBtoDeliveryOrderItems> GetReturnBtoDeliveryOrderItemsById(int id);
        Task<int?> CreateReturnBtoDeliveryOrderItems(ReturnBtoDeliveryOrderItems returnBtoDeliveryOrderItems);
        Task<string> UpdateReturnBtoDeliveryOrderItems(ReturnBtoDeliveryOrderItems returnBtoDeliveryOrderItems);
        Task<string> DeleteReturnBtoDeliveryOrderItems(ReturnBtoDeliveryOrderItems returnBtoDeliveryOrderItems);
    }
}
