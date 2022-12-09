using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface ISAShopOrderRepository : IRepositoryBase<SAShopOrder>
    {
        Task<IEnumerable<SAShopOrder>> GetAllSAShopOrders();
        Task<SAShopOrder> GetSAShopOrderById(int id);
        Task<long> CreateSAShopOrder(SAShopOrder SAshopOrder);
        Task<string> UpdateSAShopOrder(SAShopOrder SAshopOrder);

        Task<SAShopOrder> GetSAShopOrderBySalesOrderNo(string salesOrderNo);
        Task<SAShopOrder> GetSAShopOrderShopOrderNo(string SAshopOrderNo);

        Task<IEnumerable<SAShopOrder>> GetAllOpenSAShopOrders();

    }
    
}
