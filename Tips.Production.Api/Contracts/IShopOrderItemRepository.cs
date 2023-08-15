using Tips.Production.Api.Entities;
using Tips.Production.Api.Repository;

namespace Tips.Production.Api.Contracts
{
    public interface IShopOrderItemRepository
    {
        Task<IEnumerable<ShopOrderItem>> GetAllShopOrderItems();
        Task<ShopOrderItem> GetShopOrderItemById(int id);
        Task<long> CreateShopOrderItem(ShopOrderItem shopOrderItem);
        Task<string> UpdateShopOrderItem(ShopOrderItem shopOrderItem);
        public void SaveAsync();
    }
}
