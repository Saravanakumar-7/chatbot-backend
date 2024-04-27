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
        Task<PurchaseRequisition> GetPrDetailsByPrNumber(string prNumber);

        Task<decimal> GetOpenPoQuantityByItemNumber(string itemNumber);
        Task<string> GeneratePONumberForAvision();
        Task<PagedList<PurchaseOrderSPReport>> GetPurchaseOrderSPResport(PagingParameter pagingParameter);
        Task<IEnumerable<PurchaseOrderSPReport>> GetPurchaseOrderSPReportWithParam(string VendorName, string PONumber, string itemNumber);
        Task<IEnumerable<PurchaseOrderSPReport>> GetPurchaseOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<PagedList<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<PurchaseOrder>> GetAllLastestPendingPOApprovalIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<PurchaseOrder>> GetAllLastestPendingPOApprovalIIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<Tras_POSPReport>> Get_Tras_PurchaseOrderSPResport(PagingParameter pagingParameter);
        Task<PagedList<Tras_PO_ConfirmationDate>> Get_Tras_POReport_ConfirmationDate(PagingParameter pagingParameter);
        Task<IEnumerable<Tras_POSPReport>> Get_Tras_PurchaseOrderSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<PurchaseOrder_ReportDto> GetPurchaseOrderReportswithDate(DateTime? FromDate, DateTime? ToDate);
        Task<PurchaseOrder_ReportDto> GetPurchaseOrderReportswithPara(string? ItemNumber, string? PONumber, string? VendorName, int? POStatus);
        Task<IEnumerable<Tras_POSPReport>> Get_Tras_PurchaseOrderSPReportWithParam(string VendorName, string PONumber, string PartNumber);
        Task<PurchaseOrder> GetPurchaseOrderItemsByPONumber(string poNumber);
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPoNumberListByVendorIdForAvision(string vendorId);
        Task<PagedList<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIIIListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams);
        Task<PagedList<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIVListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams);
        Task<PagedList<PurchaseOrder>> GetAllLastestPendingPOApprovalIIIListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams);
        Task<PagedList<PurchaseOrder>> GetAllLastestPendingPOApprovalIVListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams);
    }
}
