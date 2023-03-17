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
        Task<PagedList<PurchaseOrder>> GetAllPurchaseOrders(PagingParameter pagingParameter,SearchParamess searchParamess);
        Task<PurchaseOrder> GetPurchaseOrderById(int id);
        Task<PurchaseOrder> GetPurchaseOrderByPONumber(string poNumber);

        Task<IEnumerable<GetDownloadUrlDto>> GetDownloadUrlDetails(string poNumber);

        Task<PagedList<PurchaseOrder>> GetAllActivePurchaseOrders(PagingParameter pagingParameter, SearchParamess searchParamess);
        Task<long> CreatePurchaseOrder(PurchaseOrder purchaseOrder);

        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPONumberListByVendorId(string vendorId);

        Task<string> UpdatePurchaseOrder(PurchaseOrder purchaseOrder);
        Task<string> DeletePurchaseOrder(PurchaseOrder purchaseOrder);
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllActivePurchaseOrderNameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalINameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIINameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPONumberListByVendorName(string vendorName);
        Task<IEnumerable<PurchaseOrderItemNoListDto>> GetAllPOItemNumberListByPoNumber(string poNumber);
        Task<int?> GetPONumberAutoIncrementCount(DateTime date);
        Task<PurchaseOrder> ChangePurchaseOrderVersion(PurchaseOrder purchaseOrder);



    }
}
