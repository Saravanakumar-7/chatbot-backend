using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IReturnOpenDeliveryOrderRepository : IRepositoryBase<ReturnOpenDeliveryOrder>
    {
        Task<PagedList<ReturnOpenDeliveryOrder>> GetAllReturnOpenDeliveryOrderDetails(PagingParameter pagingParameter, SearchParams searchParams);
        Task<ReturnOpenDeliveryOrder> GetReturnOpenDeliveryOrderById(int id);
        Task<int?> CreateReturnOpenDeliveryOrder(ReturnOpenDeliveryOrder returnOpenDeliveryOrder);
        Task<string> UpdateReturnOpenDeliveryOrder(ReturnOpenDeliveryOrder returnOpenDeliveryOrder);
        Task<string> DeleteReturnOpenDeliveryOrder(ReturnOpenDeliveryOrder returnOpenDeliveryOrder);
        Task<int?> GetReturnOpenDeliveryOrderByODONo(string odoNumber);
        Task<PagedList<ReturnOpenDeliveryOrderSPResport>> GetReturnOpenDeliveryOrderSPResport(PagingParameter pagingParameter);
        Task<IEnumerable<ReturnOpenDeliveryOrderSPResport>> ReturnOpenDeliveryOrderSPReportDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<ReturnODONumberListDto>> GetReturnOpenDeliveryOrderNumberList();
        Task<IEnumerable<ReturnOpenDeliveryOrderSPResport>> ReturnOpenDeliveryOrderSPReportWithParam(string? ODONumber, string? CustomerName, string? CustomerAliasName, string? LeadId, string? IssuedTo, string? KPN, string? MPN, string? Warehouse, string? Location, string? ODOType);
        Task<IEnumerable<ReturnOpenDeliveryOrderSPResport>> ReturnOpenDeliveryOrderSPReportWithParamForTrans(string? ODONumber, string? CustomerName, string? CustomerAliasName, string? LeadId, string? IssuedTo, string? Location, string? Warehouse, string? KPN, string? MPN, string? ODOType, string? ProjectNumber);
    }
}
