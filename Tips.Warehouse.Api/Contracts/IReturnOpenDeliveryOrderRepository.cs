using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;

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
        Task<IEnumerable<ReturnOpenDeliveryOrderSPResport>> ReturnOpenDeliveryOrderSPReportWithParam(string? ODONumber, string? CustomerName, string? CustomerAliasName, string? LeadId, string? IssuedTo, string? KPN, string? MPN, string? Warehouse, string? Location, string? ODOType);
    }
}
