using Entities.Helper;
using Entities;
using Tips.Purchase.Api.Entities;
using Contracts;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPurchaseRequisitionRepository : IRepositoryBase<PurchaseRequisition>
    {
        Task<PagedList<PurchaseRequisition>> GetAllPurchaseRequisition(PagingParameter pagingParameter);
        Task<PurchaseRequisition> GetPurchaseRequisitionById(int id);
        Task<PurchaseRequisition> GetPurchaseRequisitionByPRNumber(string PRNumber);
        Task<IEnumerable<PurchaseRequisition>> GetAllActivePurchaseRequisition();
        Task<long> CreatePurchaseRequisition(PurchaseRequisition purchaseRequisition);
        Task<string> UpdatePurchaseRequisition(PurchaseRequisition purchaseRequisition);
        Task<string> DeletePurchaseRequisition(PurchaseRequisition purchaseRequisition);
        Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllActivePurchaseRequisitionNameList();
        Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPurchaseRequisitionApprovalINameList();
        Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPurchaseRequisitionApprovalIINameList();

    }
}
