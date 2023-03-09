using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderRepository : IRepositoryBase<SalesOrder>
    {
        Task<PagedList<SalesOrder>> GetAllSalesOrder(PagingParameter pagingParameter);
        Task<SalesOrder> GetSalesOrderById(int id);
        Task<IEnumerable<SalesOrder>> GetAllActiveSalesOrder();
        Task<long> CreateSalesOrder(SalesOrder salesOrder);
        Task<string> UpdateSalesOrder(SalesOrder salesOrder);
        Task<int?> GetSONumberAutoIncrementCount(DateTime date);
        Task<IEnumerable<ListofSalesOrderDetails>> GetSalesOrderDetailsByCustomerId(string Customerid);
        Task<string> DeleteSalesOrder(SalesOrder salesOrder);

        //Task<string> UpdateSOBasedOnCreatingDO();

        //Task<string> UpdateSOBasedOnCreatingShopOrder();
        Task<List<ProjectSODetailDto>> GetProjectDetailsByItemNo(string itemNumber);
        Task<List<SalesOrderQtyDto>> GetSalesOrderQtyDetailsByItemNo(string itemNumber,string projectNo);

    }
}
