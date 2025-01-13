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
        Task<IEnumerable<ListofSalesOrderDetails>> GetSalesOrderNoDetailsByCustomerId(string Customerid);
        Task<string> DeleteSalesOrder(SalesOrder salesOrder);
        Task<string> UpdateSalesOrderShortClose(SalesOrder salesOrder);
        Task<IEnumerable<SalesOrder>> SearchSalesOrder([FromQuery] SearchParammes searchParammes);
        Task<List<SalesOrderforKeusDto>> GetAllSalesOrderforKeus([FromQuery] string? SearchTerm, [FromQuery] int Offset, [FromQuery] int Limit);
        Task<IEnumerable<SalesOrder>> SearchSalesOrderDate([FromQuery] SearchDateParam searchDateParam);
        Task<int> GetAllSalesOrderCountforKeus(string? SearchTerm);
        Task<PagedList<SalesOrder>> GetAllSalesOrderForecast(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<PagedList<SalesOrder>> GetAllSalesOrderRfq(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<IEnumerable<SalesOrderIdNameListDto>> GetAllSalesOrderIdNameList();
        Task<List<string>> GetAllSalesOrderCustomerNames(List<string> SalesOrders);
        Task<List<ProjectSODetailDto>> GetProjectDetailsByItemNo(string itemNumber, string projectType);
        //Task<List<ProjectSODetailDto>> GetProjectDetailsByItemNo(string itemNumber);

        Task<List<ProjectSOSADetailDto>> GetProjectDetailsBySAItemNo(string fgItemNumbers);
        Task<List<SalesOrderQtyDto>> GetSalesOrderQtyDetailsByItemNo(string itemNumber,string projectNo);
        Task<IEnumerable<SalesOrderIdNameListDto>> GetAllActiveSalesOrderNameList();
        Task<string> GenerateSONumber();
        Task<object> GetSalesOrderTotalBySalesOrderId(int salesOrderId);
        Task<IEnumerable<SalesOrder>> GetAllSalesOrderWithItems(SalesOrderSearchDto salesOrderSearch);
        Task<SalesOrder> GetSalesOrderDetailsBySONumber(string salesOrderNumber);
        Task<decimal> GetOpenSalesOrderQuantityByItemNumber(string salesOrderNumber);
        Task<List<SalesOrderQtyForSADto>> GetSASalesOrderQtyDetailsByItemNo(string fgItemNumber, string projectNo, decimal BomQty);
        Task<string> GenerateSONumberForAvision();
        Task<SalesOrderId_SP> GetSalesOrderDetialsById_SP(int id);
        Task<PagedList<SalesOrderSPReport>> GetSalesOrderSPReport(PagingParameter pagingParameter);
        Task<IEnumerable<RecievableCustomer>> GetRecievableCustomersWithCustomerID(string CustomerId);
        Task<IEnumerable<RecievableCustomer>> GetReceivableReportsForMultiCustomerID(string CustomerId);
        Task<IEnumerable<SalesOrderSPReport>> GetSalesOrderSPReportWithParam(string CustomerName, string SalesOrderNumber, string KPN);
        Task<IEnumerable<SalesOrderQtyDetailsDto>> GetSalesOrderQtySPReportWithParam(string itemNo, decimal bomQty);
        Task<List<SalesOrderDashboardSPReport>> GetSalesOrderDashboardSPReportWithParam(string Bucket_Id);
        Task<List<TransactionDashboardSPReport>> GetTransactionDashboardSPReportWithParam();
        Task<List<TransactionDashboardSPReport_bucketId1>> GetTransactionDashboardSPReportWithParam_bucketId1();
        Task<List<TransactionDashboardSPReport_bucketId2>> GetTransactionDashboardSPReportWithParam_bucketId2();
        Task<List<TransactionDashboardSPReport_bucketId3>> GetTransactionDashboardSPReportWithParam_bucketId3();
        Task<List<TransactionDashboardSPReport_bucketId5>> GetTransactionDashboardSPReportWithParam_bucketId5();

        Task<List<FinancialYearDashboardSPReport>> GetFinancialYearDashboardSPReportWithParam(string Bucket_Id);
        Task<IEnumerable<SalesOrderSPReport>> GetSalesOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<SOSummarySPReport>> GetSOSummarySPReportWithParam(string CustomerId, string SalesOrderNumber, string KPN);
        Task<IEnumerable<SOSummarySPReport>> GetSOSummarySPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<SOMonthlyConsumptionSPReport>> GetSOMonthlyConsumptionSPReportWithParam(string CustomerId);
        Task<IEnumerable<CustomerWiseTransactionSPReport>> GetCustomerWiseTransactionSPReportWithParam(string CustomerId);
        Task<IEnumerable<SOMonthlyConsumptionSPReport>> GetSOMonthlyConsumptionSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<FGSalesOrderSPReport>> GetFGSalesOrderSPReportWithParam(string? SalesOrderNumber, string? ProjectNumber);
        Task<IEnumerable<RfqSalesOrderSPReport>> GetRfqSalesOrderSPReportWithParam(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus,string CustomerId);
        Task<IEnumerable<RfqSalesOrderRoomWiseSPReport>> GetRfqSalesOrderRoomWiseSPReportWithParam(string CustomerName, string SalesOrderNumber, string KPN, string CustomerId);
        Task<IEnumerable<RfqSalesOrderRoomWiseSPReport>> GetRfqSalesOrderRoomWiseSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<RfqSalesOrderSPReportForTrans>> GetRfqSalesOrderSPReportWithParamForTrans(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus,string ProjectNumber);
        Task<IEnumerable<RfqSalesOrderSPReportForTrans>> GetRfqSalesOrderSPReportWithParamForAvision(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus);
        Task<IEnumerable<RfqSalesOrderSPReportForTrans>> GetRfqSalesOrderSPReportWithDateForTransAvision(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<RfqSalesOrderSPReport>> GetRfqSalesOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<ForecastSalesOrderSPReport>> GetForecastSalesOrderSPReportWithParam(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus, string CustomerId);
        Task<IEnumerable<ForecastSalesOrderSPReportForTrans>> GetForecastSalesOrderSPReportWithParamForTrans(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus, string ProjectNumber);
        Task<IEnumerable<ForecastSalesOrderSPReportForTrans>> GetForecastSalesOrderSPReportWithParamForAvision(string CustomerName, string SalesOrderNumber, string KPN, string SOStatus);
        Task<IEnumerable<ForecastSalesOrderSPReport>> GetForecastSalesOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<ForecastSalesOrderSPReportForTrans>> GetForecastSalesOrderSPReportWithDateForTransAvision(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<SalesOrderFGItemNumberDto>> GetAllSalesOrderFGItemNoListByProjectNo(string projectNo);
    }
}
