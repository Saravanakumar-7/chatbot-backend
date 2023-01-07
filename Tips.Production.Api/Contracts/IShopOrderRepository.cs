using Entities;
using Entities.DTOs;
using Entities.Helper;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Contracts
{
    public interface IShopOrderRepository 
    {
        Task<PagedList<ShopOrder>> GetAllShopOrders(PagingParameter pagingParameter);
        Task<ShopOrder> GetShopOrderById(int id);
        Task<int?> CreateShopOrder(ShopOrder shopOrder);
        Task<string> UpdateShopOrder(ShopOrder shopOrder);
        
        Task<ShopOrder> GetShopOrderBySalesOrderNo(string salesOrderNo);
        Task<ShopOrder> GetShopOrderByShopOrderNo(string shopOrderNo);

        Task<IEnumerable<ShopOrder>> GetAllOpenShopOrders();
        public void SaveAsync();
    }
}
