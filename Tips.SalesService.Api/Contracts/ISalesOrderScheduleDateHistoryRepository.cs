using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderScheduleDateHistoryRepository : IRepositoryBase<SalesOrderScheduleDateHistory>
    {
        Task<SalesOrderScheduleDateHistory> CreateSalesOrderScheduleDateHistory(SalesOrderScheduleDateHistory salesOrderScheduleDateHistory);
        Task<string> UpdateSalesOrderScheduleDateHistory(SalesOrderScheduleDateHistory salesOrderScheduleDateHistory);
    }
}
