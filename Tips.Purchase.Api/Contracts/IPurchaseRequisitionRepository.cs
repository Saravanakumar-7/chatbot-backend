using Entities.Helper;
using Entities;
using Tips.Purchase.Api.Entities;
using Contracts;
using Tips.Purchase.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPurchaseRequisitionRepository : IRepositoryBase<PurchaseRequisition>
    {
        Task<PagedList<PurchaseRequisition>> GetAllPurchaseRequisitions(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<PagedList<PurchaseRequisition>> GetAllLastestPurchaseRequisitions(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PurchaseRequisition> GetPurchaseRequisitionById(int id);
        Task<PurchaseRequisition> GetPurchaseRequisitionByPRNumber(string prNumber, int RevNo);
        Task<IEnumerable<PurchaseRequisition>> GetAllPurchaseRequisitionWithItems(PurchaseRequisitionSearchDto purchaseRequisitionSearch);
        Task<IEnumerable<PurchaseRequisition>> SearchPurchaseRequisition([FromQuery] SearchParamess searchParammes);
        Task<IEnumerable<PurchaseRequisition>> SearchPurchaseRequisitionDate([FromQuery] SearchDatesParams searchDatesParams);
        Task<string> GeneratePRNumber();
        Task<IEnumerable<GetPRDownloadUrlDto>> GetDownloadUrlDetail(string prNumber);
        Task<IEnumerable<PurchaseRequisition>> GetAllActivePurchaseRequisitions();
        Task<long> CreatePurchaseRequisition(PurchaseRequisition purchaseRequisition);
        Task<PurchaseRequisition> ChangePurchaseRequisitionVersion(PurchaseRequisition purchaseRequisition);
        Task<string> UpdatePurchaseRequisition(PurchaseRequisition purchaseRequisition);
        Task<string> UpdatePurchaseRequisition_ForApproval(PurchaseRequisition purchaseRequisitions);
        Task<string> DeletePurchaseRequisition(PurchaseRequisition purchaseRequisition);
        Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllActivePurchaseRequisitionNameList();
        Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPurchaseRequisitionNameList();
        Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalINameList();
        Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalIINameList();
        Task<int?> GetPRNumberAutoIncrementCount(DateTime date);
        Task<IEnumerable<PurchaseRequistionRevNoListDto>> GetAllRevisionNumberListByPRNumber(string prNumber);
        Task<IEnumerable<GetDownloadUrlDto>> GetDownloadUrlDetails(string prNumber, string prItemNumber);
        Task<PurchaseRequisition> GetPurchaseRequisitionByPRNoAndRevNo(string prNumber, int revisionNumber);
        Task<string> GeneratePRNumberAvision();
        Task<PagedList<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalIList(PagingParameter pagingParameter ,SearchParamess searchParams);
        Task<PagedList<PurchaseRequisition>> GetAllLastestPendingPRApprovalIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalIIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<PurchaseRequisition>> GetAllLastestPendingPRApprovalIIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<List<GetDownloadUrlDto>> GetDownloadUrlPrItemsDetails(string FileIds);
        Task<PurchaseRequisition> GetPurchaseRequisitionByPRNo(string prNumber);
        Task<PagedList<PurchaseRequisitionSPReportForTrans>> GetPurchaseRequisitionsSPReportForTrans(PagingParameter pagingParameter);
        Task<PagedList<PurchaseRequisitionSPReportForAvision>> GetPurchaseRequisitionsSPReportForAvi(PagingParameter pagingParameter);
        Task<IEnumerable<PurchaseRequisitionSPReport>> GetPurchaseRequisitionsSPReportWithParam(string PrNumber, string ProcurementType, string ShippingMode, string PrStatus);
        Task<IEnumerable<PurchaseRequisitionSPReportForTrans>> GetPurchaseRequisitionsSPReportWithParamForTrans(string PrNumber, string ProcurementType, string PrStatus, string ProjectNumber);
        Task<IEnumerable<PurchaseRequisitionSPReportForAvision>> GetPurchaseRequisitionsSPReportWithParamForAvi(string PrNumber, string ProcurementType, string PrStatus);
        Task<IEnumerable<PurchaseRequisitionSPReport>> GetPurchaseRequisitionsSPReportWithDate(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<PurchaseRequisitionSPReportForTrans>> GetPurchaseRequisitionsSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate);
        Task<IEnumerable<PurchaseRequisitionSPReportForAvision>> GetPurchaseRequisitionsSPReportWithDateForAvi(DateTime? FromDate, DateTime? ToDate);
        Task<PurchaseRequisition> GetPrDetailsByPrNumber(string prNumber);
    }
}
