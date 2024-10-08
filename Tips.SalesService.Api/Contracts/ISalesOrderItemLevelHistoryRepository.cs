using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderItemLevelHistoryRepository : IRepositoryBase<SalesOrderItemLevelHistory>
    {
        Task<SalesOrderItemLevelHistory> CreateSalesOrderItemLevelHistory(SalesOrderItemLevelHistory salesOrderItemLevelHistory);
        Task<string> UpdateSalesOrderItemLevelHistory(SalesOrderItemLevelHistory salesOrderItemLevelHistory);
    }
}
