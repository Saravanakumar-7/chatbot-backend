using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IReturnBtoDeliveryOrderRepository : IRepositoryBase<ReturnBtoDeliveryOrder>
    {
        Task<PagedList<ReturnBtoDeliveryOrder>> GetAllReturnBtoDeliveryOrderDetails(PagingParameter pagingParameter, SearchParams searchParams);
        Task<ReturnBtoDeliveryOrder> GetReturnBtoDeliveryOrderById(int id);
        Task<int?> CreateReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder);
        Task<string> UpdateReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder);
        Task<string> DeleteReturnBtoDeliveryOrder(ReturnBtoDeliveryOrder returnBtoDeliveryOrder);
        Task<int?> GetReturnBtoDeliveryOrderByBtoNo(string BTONumber);
        Task<PagedList<ReturnDOSPReport>> ReturnDeliveryOrderSPReport(PagingParameter pagingParameter);
        Task<IEnumerable<ReturnDOSPReport>> ReturnDeliveryOrderSPReportDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<ReturnBTONumberListDto>> GetReturnBtoDeliveryOrderNumberList();
        Task<string> GetReturnBtoDeliveryOrderNoByReturnBtoNo(string returnBTONumber);
        Task<IEnumerable<ReturnDOSPReport>> ReturnDOSPReportWithParam(string? ReturnBTONumber, string? CustomerName, string? CustomerAliasName, string? Customerleadid, string? SalesOrderNumber, string? ProductType, string? TypeOfSolution, string? Warehouse, string? Location, string? KPN, string? MPN);
        Task<IEnumerable<ReturnDOSPReportForTras>> ReturnDOSPReportWithParamForTrans(string? ReturnBTONumber, string? CustomerName, string? CustomerAliasName, string? CustomerLeadid, string? SalesOrderNumber, string? ProductType, string? TypeOfSolution, string? Warehouse, string? Location, string? KPN, string? MPN, string? ProjectNumber);
        Task<IEnumerable<SerialNoDetailDto>> GetReturnBTOSerialNoDetailsByFGItemNoAndBTONo(string itemNumber, string doNumber);
    }
}
