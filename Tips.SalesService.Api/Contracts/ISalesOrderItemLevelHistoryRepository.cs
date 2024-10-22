using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderItemLevelHistoryRepository : IRepositoryBase<SalesOrderItemLevelHistory>
    {
        Task<SalesOrderItemLevelHistory> CreateSalesOrderItemLevelHistory(SalesOrderItemLevelHistory salesOrderItemLevelHistory);
        Task<string> UpdateSalesOrderItemLevelHistory(SalesOrderItemLevelHistory salesOrderItemLevelHistory);
        Task<SalesOrderItemLevelHistory> GetSalesOrderItemLevelHistoryBySalesOrderItemIdAndRevNo(int soItemid, int? revNo);
        Task<SalesOrderItemLevelHistory> GetShortCloseSalesOrderItemLevelHistoryBySalesOrderItemIdAndRevNo(int soItemid, int? revNo);
        Task<int> GetShortCloseSalesOrderItemLevelHistoryIdBySalesOrderItemIdAndRevNo(int soItemid, int? revNo);
        Task<int> GetSalesOrderItemLevelHistoryIdBySalesOrderItemIdAndRevNo(int soItemid, int? revNo);
    }
}
