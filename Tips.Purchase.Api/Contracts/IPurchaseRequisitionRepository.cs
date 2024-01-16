using Entities.Helper;
using Entities;
using Tips.Purchase.Api.Entities;
using Contracts;
using Tips.Purchase.Api.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

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
        Task<PagedList<PurchaseRequisitionIdNameListDto>> GetAllLastestPendingPRApprovalIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalIIList(PagingParameter pagingParameter, SearchParamess searchParams);
        Task<PagedList<PurchaseRequisitionIdNameListDto>> GetAllLastestPendingPRApprovalIIList(PagingParameter pagingParameter, SearchParamess searchParams);
    }
}
