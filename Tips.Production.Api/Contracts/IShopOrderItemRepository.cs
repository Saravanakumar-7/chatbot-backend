using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IShopOrderItemRepository
    {
        Task<IEnumerable<ShopOrder>> GetAllShopOrderItems();
        Task<ShopOrder> GetShopOrderItemById(int id);
        Task<long> CreateShopOrderItem(ShopOrder shopOrderItem);
        Task<string> UpdateShopOrderItem(ShopOrder shopOrderItem);
    }
}
