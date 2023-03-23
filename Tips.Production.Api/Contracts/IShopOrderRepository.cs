using Entities;
using Entities.DTOs;
using Entities.Helper;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Contracts
{
    public interface IShopOrderRepository 
    {
 
        Task<PagedList<ShopOrder>> GetAllShopOrders(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<ShopOrder> GetShopOrderById(int id);
        Task<int?> CreateShopOrder(ShopOrder shopOrder);
        Task<string> UpdateShopOrder(ShopOrder shopOrder);
        Task<ShopOrder> GetShopOrderDetailsByShopOrderNo(string shopOrderNo);
        Task<ShopOrder> GetShopOrderBySalesOrderNo(string salesOrderNo);
        Task<ShopOrder> GetShopOrderByShopOrderNo(string shopOrderNo);
        Task<IEnumerable<ListOfShopOrderDto>> GetShopOrderByItemType(string itemType);
        Task<IEnumerable<ListOfShopOrderDto>> GetShopOrderByFGNo(string fGNumber);
        Task<IEnumerable<ListOfShopOrderDto>> GetShopOrderByFGNoAndSANo(string fGNumber, string sANumber);
        Task<IEnumerable<ShopOrder>> GetAllOpenShopOrders();
        public void SaveAsync();
    }
}
