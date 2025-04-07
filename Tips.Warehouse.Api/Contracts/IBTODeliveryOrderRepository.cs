using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IBTODeliveryOrderRepository : IRepositoryBase<BTODeliveryOrder>
    {
        Task<PagedList<BTODeliveryOrder>> GetAllBTODeliveryOrders(PagingParameter pagingParameter, SearchParams searchParams);
        Task<BTODeliveryOrder> GetBTODeliveryOrderById(int id);
        Task<string> GenerateBTONumber();
        Task<IEnumerable<ListOfBtoNumberDetails>> GetBtoNumberListByCustomerId(string customerLeadId);
        Task<BTODeliveryOrder> GetBtoDetailsByBtoNo(string BTONumber);
        Task<string> UpdateBTODeliveryOrderFromReturnDO(BTODeliveryOrder bTODeliveryOrder);
        Task<PagedList<BTODeliveryOrder>> GetAllActiveBTODeliveryOrders(PagingParameter pagingParameter, SearchParams searchParams);
        Task<long> CreateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        Task<IEnumerable<ListOfBtoNumberDetails>> GetBtoNumberListBySalesOrderId(int salesOrderId);
        Task<int?> GetBTONumberAutoIncrementCount(DateTime date);
        Task<IEnumerable<ListofBtoDeliveryOrderDetails>> GetBtoDeliveryOrderNumberList();
        Task<string> UpdateBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        Task<string> DeleteBTODeliveryOrder(BTODeliveryOrder bTODeliveryOrder);
        Task<IEnumerable<BtoIDNameList>> GetAllBTOIdNameIdNameList();
        Task<IEnumerable<BTODeliveryOrder>> GetAllBTODeliveryOrderWithItems(BTODeliveryOrderSearchDto bTODeliveryOrderSearch);
        Task<IEnumerable<BTODeliveryOrder>> SearchBTODeliveryOrder([FromQuery] SearchParames searchParames);
        Task<IEnumerable<BTODeliveryOrder>> SearchBTODeliveryOrderDate([FromQuery] SearchsDateParms searchsDateParms);
        Task<string> GenerateBTONumberAvision();
        Task<IEnumerable<DailyDOReport>> GetDailyDeliveryOrderReports(string LeadId, string SONumber, string DOnumber, string DispatchKPN);
        Task<PagedList<DeliveryOrderSPReport>> DeliveryOrderSPReport(PagingParameter pagingParameter);
        Task<IEnumerable<DeliveryOrderSPReport>> DeliveryOrderSPReportdate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<DeliveryOrderSPReportForTrans>> DeliveryOrderSPReportdateForTrans(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<DOSPReportForTrans>> DOSPReportdateForTrans(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<DeliveryOrderSPReport>> GetDeliveryOrderSPReportsWithParam(string DONumber, string CustomerName, string CustomerAliasName,
                                                                                                    string CustomerID, string SalesOrderNumber, string ProductType,
                                                                                                    string Warehouse, string Location, string KPN, string MPN, string ProjectNumber);
        Task<IEnumerable<DeliveryOrderSPReportForTrans>> GetDeliveryOrderSPReportsWithParamForTrans(string DONumber, string CustomerName, string SalesOrderNumber, string ProductType,
                                                                                                    string Warehouse, string Location, string ItemNumber, string MPN, string ProjectNumber);
        Task<IEnumerable<DOSPReportForTrans>> GetDOSPReportsWithParamForTrans(string DONumber, string CustomerName, string SalesOrderNumber, string ProductType,
                                                                                                    string Warehouse, string Location, string ItemNumber, string MPN, string ProjectNumber);
        Task<IEnumerable<DailyDOReport>> GetDailyDeliveryOrderReports();
        Task<SalesOrderNoandIdDto> GetAllSalesOrderNoAndIdByBTONo(string btoNumber);
        Task<IEnumerable<DoLotNumberListDto>> GetDOLotNumberListByBTONoAndItemNo(string btoNumber, string itemNumber);
        Task<BTODeliveryOrder> GetBTODeliveryOrderByIdExcludingClosed(int id);
        Task<IEnumerable<ListOfBtoNumberDetails>> GetBtoNumberListByCustomerIdExcludingClosed(string customerLeadId);
        Task<List<DoConsumpDto>> GetDoConsumpDetailsByBTONumberList(List<string> btoNumber);
    }
}
