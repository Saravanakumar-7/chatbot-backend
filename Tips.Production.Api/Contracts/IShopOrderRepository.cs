using Entities.DTOs;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Contracts
{
    public interface IShopOrderRepository : IRepositoryBase<ShopOrder>
    {
        Task<IEnumerable<ShopOrder>> GetAllShopOrders();
        Task<ShopOrder> GetShopOrderById(int id);
        Task<long> CreateShopOrder(ShopOrder shopOrder);
        Task<string> UpdateShopOrder(ShopOrder shopOrder);
        
        Task<ShopOrder> GetShopOrderBySalesOrderNo(string salesOrderNo);
        Task<ShopOrder> GetShopOrderByShopOrderNo(string shopOrderNo);

        Task<IEnumerable<ShopOrder>> GetAllOpenShopOrders();

    }
}
