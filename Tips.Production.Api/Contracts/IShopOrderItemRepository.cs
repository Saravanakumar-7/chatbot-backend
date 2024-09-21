using Tips.Production.Api.Entities;
using Tips.Production.Api.Repository;

namespace Tips.Production.Api.Contracts
{
    public interface IShopOrderItemRepository
    {
        Task<IEnumerable<ShopOrderItem>> GetAllShopOrderItems();
        Task<ShopOrderItem> GetShopOrderItemById(int id);
        Task<decimal?> GetNotShortCloseQty(string fgItemNumber,string saItemNumber,string projectNumber,string salesOrderNumber);
        Task<long> CreateShopOrderItem(ShopOrderItem shopOrderItem);
        Task<string> UpdateShopOrderItem(ShopOrderItem shopOrderItem);
        Task<int?> GetShopOrderItemShortCloseCount(int shopOrderId);
        public void SaveAsync();
    }
}
