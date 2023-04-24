using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IBTODeliveryOrderItemsRepository : IRepositoryBase<BTODeliveryOrderItems>
    {
        //Task<PagedList<bTODeliveryOrderItems>> GetAllBTODeliveryOrderItems(PagingParameter pagingParameter);
        //Task<bTODeliveryOrderItems> GetBTODeliveryOrderItemById(int id);
        Task<BTODeliveryOrderItems> UpdateBtoDelieveryOrderBalanceQty(string itemNumber, string BtoDeliveryNumber, decimal Qty);

        Task<BTODeliveryOrderItems> GetBtoDelieveryOrderItemDetails(int btoDeliveryOrderPartsId);


        //Task<IEnumerable<bTODeliveryOrderItems>> GetAllActiveBTODeliveryOrderItems();
        //Task<long> CreateBTODeliveryOrderItem(bTODeliveryOrderItems bTODeliveryOrderItems);
        //Task<string> UpdateBTODeliveryOrderItem(bTODeliveryOrderItems bTODeliveryOrderItems);
        //Task<string> DeleteBTODeliveryOrderItem(bTODeliveryOrderItems bTODeliveryOrderItems);

    }
}
