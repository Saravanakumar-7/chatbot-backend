using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IOpenDeliveryOrderPartsRepository: IRepositoryBase<OpenDeliveryOrderParts>
    {
        //Task<OpenDeliveryOrderParts> UpdateOpenDelieveryOrderBalanceQty(string itemNumber, string odoNumber, decimal Qty);

        Task<OpenDeliveryOrderParts> GetOpenDelieveryOrderPartDetails(int odoDeliveryOrderPartsId);
    }
}
