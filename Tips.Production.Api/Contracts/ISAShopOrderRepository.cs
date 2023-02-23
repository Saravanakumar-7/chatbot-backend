using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Contracts
{
    public interface ISAShopOrderRepository : IRepositoryBase<SAShopOrder>
    {
        Task<IEnumerable<SAShopOrder>> GetAllSAShopOrders();
        Task<SAShopOrder> GetSAShopOrderById(int id);
        Task<long> CreateSAShopOrder(SAShopOrder sAShopOrder);
        Task<string> UpdateSAShopOrder(SAShopOrder sAShopOrder);
        Task<SAShopOrder> GetSAShopOrderBySalesOrderNo(string salesOrderNo);
        Task<SAShopOrder> GetSAShopOrderBySAShopOrderNo(string sAShopOrderNo);
        Task<IEnumerable<SAShopOrder>> GetAllOpenSAShopOrders();

    }
    
}
