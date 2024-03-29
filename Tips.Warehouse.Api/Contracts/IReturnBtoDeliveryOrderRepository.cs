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
        Task<IEnumerable<ReturnDOSPReport>> ReturnDOSPReportWithParam(string DoNumber, string CustomerName,string CustomerAliasName, string LeadId, string SalesOrderNumber, string Location, string Warehouse, string ProductType, string TypeOfSolution, string KPN, string MPN);
    }
}
