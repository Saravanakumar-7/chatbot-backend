using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IReturnDeliveryOrderItemsRepository 
    {
        Task<PagedList<ReturnDeliveryOrderItems>> GetAllReturnDeliveryOrdersItems(PagingParameter pagingParameter);

        Task<ReturnDeliveryOrderItems> GetReturnDeliveryOrderItemsById(int id);
        Task<int?> CreateReturnDeliveryOrderItems(ReturnDeliveryOrderItems returnDeliveryOrderItems);
        Task<string> UpdateReturnDeliveryOrderItems(ReturnDeliveryOrderItems returnDeliveryOrderItems);
        Task<string> DeleteReturnDeliveryOrderItems(ReturnDeliveryOrderItems returnDeliveryOrderItems);
    }
}
