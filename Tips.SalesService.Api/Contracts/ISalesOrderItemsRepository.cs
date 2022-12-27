using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderItemsRepository : IRepositoryBase<SalesOrderItems>
    {
        Task<PagedList<SalesOrderItems>> GetAllSalesOrderItems(PagingParameter pagingParameter);
        Task<SalesOrderItems> GetSalesOrderItemsById(int id);
        Task<IEnumerable<SalesOrderItems>> GetAllActiveSalesOrderItems();
        Task<long> CreateSalesOrderItems(SalesOrderItems salesOrderItems);
        Task<string> UpdateSalesOrderItems(SalesOrderItems salesOrderItems);
        Task<string> DeleteSalesOrderItems(SalesOrderItems salesOrderItems);
    }
}
