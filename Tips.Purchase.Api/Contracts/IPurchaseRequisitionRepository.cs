using Entities.Helper;
using Entities;
using Tips.Purchase.Api.Entities;
using Contracts;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPurchaseRequisitionRepository : IRepositoryBase<PurchaseRequisition>
    {
        Task<PagedList<PurchaseRequisition>> GetAllPurchaseRequisitions(PagingParameter pagingParameter);
        Task<PurchaseRequisition> GetPurchaseRequisitionById(int id);
        Task<PurchaseRequisition> GetPurchaseRequisitionByPRNumber(string prNumber);

        Task<IEnumerable<GetDownloadUrlDto>> GetDownloadUrlDetails(string prNumber);
        Task<IEnumerable<PurchaseRequisition>> GetAllActivePurchaseRequisitions();
        Task<long> CreatePurchaseRequisition(PurchaseRequisition purchaseRequisition);
        Task<PurchaseRequisition> ChangePurchaseRequisitionVersion(PurchaseRequisition purchaseRequisition);
        Task<string> UpdatePurchaseRequisition(PurchaseRequisition purchaseRequisition);
        Task<string> DeletePurchaseRequisition(PurchaseRequisition purchaseRequisition);
        Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllActivePurchaseRequisitionNameList();
        Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalINameList();
        Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalIINameList();
        Task<int?> GetPRNumberAutoIncrementCount(DateTime date);


    }
}
