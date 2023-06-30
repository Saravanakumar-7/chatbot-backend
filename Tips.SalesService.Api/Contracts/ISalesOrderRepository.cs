using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderRepository : IRepositoryBase<SalesOrder>
    {
        Task<PagedList<SalesOrder>> GetAllSalesOrder(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<SalesOrder> GetSalesOrderById(int id);
        Task<PagedList<SalesOrder>> GetAllActiveSalesOrder(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<long> CreateSalesOrder(SalesOrder salesOrder);
        Task<string> UpdateSalesOrder(SalesOrder salesOrder);
        Task<int?> GetSONumberAutoIncrementCount(DateTime date);
        Task<IEnumerable<ListofSalesOrderDetails>> GetSalesOrderDetailsByCustomerId(string Customerid);
        Task<string> DeleteSalesOrder(SalesOrder salesOrder);

        Task<IEnumerable<SalesOrder>> SearchSalesOrder([FromQuery] SearchParammes searchParammes);

        Task<IEnumerable<SalesOrder>> SearchSalesOrderDate([FromQuery] SearchDateParam searchDateParam);

        Task<IEnumerable<SalesOrderIdNameListDto>> GetAllSalesOrderIdNameList();

        //Task<string> UpdateSOBasedOnCreatingDO();

        //Task<string> UpdateSOBasedOnCreatingShopOrder();
        Task<List<ProjectSODetailDto>> GetProjectDetailsByItemNo(string itemNumber);
        Task<List<SalesOrderQtyDto>> GetSalesOrderQtyDetailsByItemNo(string itemNumber,string projectNo);
        Task<IEnumerable<SalesOrderIdNameListDto>> GetAllActiveSalesOrderNameList();
        Task<string> GenerateSONumber();
        Task<object> GetSalesOrderTotalBySalesOrderId(int salesOrderId);
        Task<IEnumerable<SalesOrder>> GetAllSalesOrderWithItems(SalesOrderSearchDto salesOrderSearch);
        Task<SalesOrder> GetSalesOrderDetailsBySONumber(string salesOrderNumber);
    }
}
