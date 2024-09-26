using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IReturnOpenDeliveryOrderPartsRepository
    {
        Task<PagedList<ReturnOpenDeliveryOrderParts>> GetAllReturnOpenDeliveryOrderParts(PagingParameter pagingParameter);
        Task<ReturnOpenDeliveryOrderParts> GetReturnOpenDeliveryOrderPartsById(int id);
        Task<int?> CreateReturnOpenDeliveryOrderParts(ReturnOpenDeliveryOrderParts returnOpenDeliveryOrderParts);
        Task<string> UpdateReturnOpenDeliveryOrderParts(ReturnOpenDeliveryOrderParts returnOpenDeliveryOrderParts);
        Task<string> DeleteReturnOpenDeliveryOrderParts(ReturnOpenDeliveryOrderParts returnOpenDeliveryOrderParts);
    }
}
