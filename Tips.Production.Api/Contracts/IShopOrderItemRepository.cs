using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface IShopOrderItemRepository
    {
        Task<IEnumerable<ShopOrder>> GetAllShopOrders();
        Task<ShopOrder> GetShopOrderById(int id);
        Task<long> CreateShopOrder(ShopOrder shopOrder);
        Task<string> UpdateShopOrder(ShopOrder shopOrder);
    }
}
