using Entities.Helper;
using Entities;
using Tips.Purchase.Api.Entities;
using Entities.DTOs;
using Contracts;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities.Dto;

namespace Tips.Purchase.Api.Contracts
{
    public interface IPurchaseOrderRepository : IRepositoryBase<PurchaseOrder>
    {
        Task<PagedList<PurchaseOrder>> GetAllPurchaseOrder(PagingParameter pagingParameter);
        Task<PurchaseOrder> GetPurchaseOrderById(int id);
        Task<PurchaseOrder> GetPurchaseOrderByPONumber(string PONumber);
        Task<IEnumerable<PurchaseOrder>> GetAllActivePurchaseOrder();
        Task<long> CreatePurchaseOrder(PurchaseOrder purchaseOrder);
        Task<string> UpdatePurchaseOrder(PurchaseOrder purchaseOrder);
        Task<string> DeletePurchaseOrder(PurchaseOrder purchaseOrder);
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllActivePurchaseOrderNameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPurchaseOrderApprovalINameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPurchaseOrderApprovalIINameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPoNumberListByVendorName(string vendorName);
        Task<IEnumerable<PurchaseOrderItemNoListDto>> GetAllPoItemNumberListByPoNumber(string poNumber);
    }
}
