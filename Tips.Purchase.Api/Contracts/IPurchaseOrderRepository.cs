using Entities.Helper;
using Entities;
using Tips.Purchase.Api.Entities;
using Entities.DTOs;
using Contracts;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities.Dto;
using Microsoft.AspNetCore.Mvc;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPurchaseOrderRepository : IRepositoryBase<PurchaseOrder>
    {
        Task<PagedList<PurchaseOrder>> GetAllPurchaseOrders(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<PagedList<PurchaseOrder>> GetAllLastestPurchaseOrders(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PurchaseOrder> GetPurchaseOrderById(int id);
        Task<PurchaseOrder> GetPurchaseOrderByPONumber(string poNumber);
        Task<List<GetDownloadUrlDto>> GetDownloadUrlPoDetails(string FileIds);
        Task<IEnumerable<GetDownloadUrlDto>> GetDownloadUrlDetails(string poNumber);
        Task<PurchaseOrder> GetLastestPurchaseOrderByPONumber(string poNumber);
        Task<IEnumerable<PurchaseOrder>> GetAllActivePurchaseOrders();
        Task<IEnumerable<PRNoandQtyListDto>> GetPRNumberandQtyListByItemNumber(string itemNumber);
        Task<string> GeneratePONumber();
        Task<long> CreatePurchaseOrder(PurchaseOrder purchaseOrder);

        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPONumberListByVendorId(string vendorId);
        Task<PurchaseOrder> GetPurchaseOrderByPONoAndRevNo(string poNumber, int revisionNumber);
        Task<string> UpdatePurchaseOrder(PurchaseOrder purchaseOrder);
        Task<string> UpdatePurchaseOrder_ForApproval(PurchaseOrder purchaseOrder);
        Task<string> DeletePurchaseOrder(PurchaseOrder purchaseOrder);
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllActivePurchaseOrderNameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPurchaseOrderNameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalINameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIINameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPONumberListByVendorName(string vendorName);
        Task<IEnumerable<PurchaseOrderItemNoListDto>> GetAllPOItemNumberListByPoNumber(string poNumber);
        Task<int?> GetPONumberAutoIncrementCount(DateTime date);
        Task<PurchaseOrder> ChangePurchaseOrderVersion(PurchaseOrder purchaseOrder);
        Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrderWithItems(PurchaseOrderSearchDto purchaseOrderSearch, PoVersion poVersion);
        Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrder([FromQuery] SearchParamess searchParammes, PoVersion poVersion);
        Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrderDate([FromQuery] SearchDatesParams searchDatesParams, PoVersion poVersion);
        Task<IEnumerable<PurchaseOrderRevNoListDto>> GetAllRevisionNumberListByPoNumber(string poNumber);

        Task<decimal> GetOpenPoQuantityByItemNumber(string itemNumber);
        Task<string> GeneratePONumberForAvision();
        Task<PagedList<PurchaseOrderSPReport>> GetPurchaseOrderSPResport(PagingParameter pagingParameter);
        Task<IEnumerable<PurchaseOrderSPReport>> GetPurchaseOrderSPReportWithParam(string VendorName, string PONumber, string itemNumber);
        Task<IEnumerable<PurchaseOrderSPReport>> GetPurchaseOrderSPReportWithParamForTrans(string VendorName, string PONumber, string itemNumber, string ProjectNumber);
        Task<IEnumerable<PurchaseOrderApprovalSPReport>> GetPurchaseOrderApprovalSPReportWithParam(string VendorName, string PONumber, string itemNumber,
                                                                                                     string RecordType, string Postatus, string Approval);
        Task<IEnumerable<PurchaseOrderSPReportForTrans>> GetPurchaseOrderApprovalSPReportWithParamForTrans(string VendorName, string PONumber, string itemNumber,
                                                                                                     string RecordType, string Postatus, string Approval, string ProjectNumber);
        Task<IEnumerable<PurchaseOrderSPReportForAvision>> GetPurchaseOrderApprovalSPReportWithParamForAvision(string VendorName, string PONumber, string itemNumber,
                                                                                                    string RecordType, string Postatus, string Approval, string ProjectNumber);
        Task<IEnumerable<PurchaseOrderSPReportForAvision>> GetPurchaseOrderApprovalSPReportWithDateForAvision(DateTime? FromDate, DateTime? ToDate, string RecordType
                                                                                                                , string Approval);
        Task<IEnumerable<PurchaseOrderSPReport>> GetPurchaseOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<PurchaseOrderSPReportForTrans>> GetPurchaseOrderApprovalSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate, string RecordType
                                                                                                                  , string Approval);
        Task<IEnumerable<PurchaseOrderApprovalSPReport>> GetPurchaseOrderApprovalSPReportWithDate(DateTime? FromDate, DateTime? ToDate, string RecordType
                                                                                                                 , string Approval);
        Task<PagedList<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<PurchaseOrder>> GetAllLastestPendingPOApprovalIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<PurchaseOrder>> GetAllLastestPendingPOApprovalIIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<Tras_POSPReport>> Get_Tras_PurchaseOrderSPResport(PagingParameter pagingParameter);
        Task<PagedList<Tras_PO_ConfirmationDate>> Get_Tras_POReport_ConfirmationDate(PagingParameter pagingParameter);
        Task<IEnumerable<Tras_POSPReport>> Get_Tras_PurchaseOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<Tras_POSPReport>> Get_Tras_PurchaseOrderSPReportWithParam(string VendorName, string PONumber, string PartNumber);
        Task<PurchaseOrder> GetPurchaseOrderItemsByPONumber(string poNumber);
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPoNumberListByVendorIdForAvision(string vendorId);
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllServicePoNumberListByVendorId(string vendorId);
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllNonServicePoNumberListByVendorId(string vendorId);
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllServicePoNumberListByVendorIdForAvision(string vendorId);
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllNonServicePoNumberListByVendorIdForAvision(string vendorId);
        Task<PagedList<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIIIListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams);
        Task<PagedList<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIVListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams);
        Task<PagedList<PurchaseOrder>> GetAllLastestPendingPOApprovalIIIListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams);
        Task<PagedList<PurchaseOrder>> GetAllLastestPendingPOApprovalIVListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams);
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllLatestRevNoPurchaseOrderNameList();
        Task<IEnumerable<poconfirmation_report_Dto>> GetPoConfirmationSPReportwithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<podeliveryschedule_report_Dto>> GetPoDeliveryScheduleSPReportwithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<poproject_report_Dto>> GetPoProjectSPReportwithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<poconfirmation_report_Dto>> GetPoConfirmationSPReportwithParam(string? ItemNumber, string? PONumber, string? VendorName, string? POStatus
                                                                                                       , string? Approval, string? RecordType);
        Task<IEnumerable<podeliveryschedule_report_Dto>> GetPoDeliverySchedulewithParam(string? ItemNumber, string? PONumber, string? VendorName, string? POStatus
                                                                                                      , string? Approval, string? RecordType);
        Task<IEnumerable<poproject_report_Dto>> GetPoProjectSPReportwithParam(string? ItemNumber, string? PONumber, string? VendorName, string? POStatus
                                                                                                       , string? Approval, string? ProjectNumber, string? RecordType);
      
    }
}
