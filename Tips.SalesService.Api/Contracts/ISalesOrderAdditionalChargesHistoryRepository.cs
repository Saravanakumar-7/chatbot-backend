using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderAdditionalChargesHistoryRepository : IRepositoryBase<SalesOrderAdditionalChargesHistory>
    {
        Task<SalesOrderAdditionalChargesHistory> CreateSalesOrderAdditionalChargesHistory(SalesOrderAdditionalChargesHistory salesOrderAdditionalChargesHistory);

    }
}
