using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Tips.SalesService.Api.Contracts
{
    public interface ISalesOrderItemsRepository : IRepositoryBase<SalesOrderItems>
    { 
        Task<IEnumerable<GetSalesOrderDetailsDto>> getSalesOrderDetailByProjectNoandItemNo(string ItemNo, string ProjectNo);
        Task<IEnumerable<SalesOrderItems>> SearchSalesOrderItem([FromQuery] SearchParammes searchParammes);
        Task<IEnumerable<SalesOrderItems>>  UpdateShopOrderBySalesOrderNoandItemNo(string salesOrderNumber, string itemNumber, string projectNumber);
        Task<IEnumerable<SalesOrderItems>> GetSalesOrderItemDetailsByIdandItemNo(string ItemNumber, int SalesOrderId);
        Task<List<SalesOrderFGandBalanceQty>> GetAllSalesOrderFGOrTGItemDetails();
        Task<string> UpdateSalesOrderItem(SalesOrderItems salesOrderItems);
        Task<IEnumerable<ListOfProjectNoDto>> GetprojectNoByItemNo(string itemNo);
        Task<SalesOrderItems> GetSOItemDetailById(int soItemId);
        Task<int?> GetSOItemOpenStatusCount(int soId);
        Task<SalesOrderItems> GetSalesOrderItemDetailsById(int soItemId); 
    }
}
