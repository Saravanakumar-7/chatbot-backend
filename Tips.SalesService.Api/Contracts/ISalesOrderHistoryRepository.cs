using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderHistoryRepository : IRepositoryBase<SalesOrderHistory>
    {
        Task<SalesOrderHistory> CreateSalesOrderHistory(SalesOrderHistory salesOrderHistory);
        Task<List<SalesOrderHistory>> GetSalesOrderHistoryBySONoAndItemNumberifShortCLosed(string SOnumber, string Itemnumber);
    }
}
