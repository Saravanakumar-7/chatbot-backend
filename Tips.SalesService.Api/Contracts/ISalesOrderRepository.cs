using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderRepository : IRepositoryBase<SalesOrder>
    {
        Task<PagedList<SalesOrder>> GetAllSalesOrder(PagingParameter pagingParameter);
        Task<SalesOrder> GetSalesOrderById(int id);
        Task<IEnumerable<SalesOrder>> GetAllActiveSalesOrder();
        Task<long> CreateSalesOrder(SalesOrder salesOrder);
        Task<string> UpdateSalesOrder(SalesOrder salesOrder);
        Task<string> DeleteSalesOrder(SalesOrder salesOrder);
        //Task<string> UpdateSOBasedOnCreatingDO();

        //Task<string> UpdateSOBasedOnCreatingShopOrder();

        


    }
}
