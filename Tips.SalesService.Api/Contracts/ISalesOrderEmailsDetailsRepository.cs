using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderEmailsDetailsRepository : IRepositoryBase<SalesOrderEmailsDetails>
    {
        Task<int> CreateSalesOrderEmailsDetails(SalesOrderEmailsDetails salesOrderEmailsDetails);
    }
}
