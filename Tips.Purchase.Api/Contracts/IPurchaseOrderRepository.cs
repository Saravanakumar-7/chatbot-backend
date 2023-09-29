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
        Task<PagedList<PurchaseOrder>> GetAllPurchaseOrders(PagingParameter pagingParameter,SearchParamess searchParamess);
        Task<PurchaseOrder> GetPurchaseOrderById(int id);
        Task<PurchaseOrder> GetPurchaseOrderByPONumber(string poNumber);

        Task<IEnumerable<GetDownloadUrlDto>> GetDownloadUrlDetails(string poNumber);

        Task<IEnumerable<PurchaseOrder>> GetAllActivePurchaseOrders();
        Task<IEnumerable<PRNoandQtyListDto>> GetPRNumberandQtyListByItemNumber(string itemNumber);
        Task<string> GeneratePONumber();
        Task<long> CreatePurchaseOrder(PurchaseOrder purchaseOrder);

        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPONumberListByVendorId(string vendorId);
        Task<PurchaseOrder> GetPurchaseOrderByPONoAndRevNo(string poNumber, int revisionNumber);
        Task<string> UpdatePurchaseOrder(PurchaseOrder purchaseOrder);
        Task<string> DeletePurchaseOrder(PurchaseOrder purchaseOrder);
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllActivePurchaseOrderNameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPurchaseOrderNameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalINameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPOApprovalIINameList();
        Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPONumberListByVendorName(string vendorName);
        Task<IEnumerable<PurchaseOrderItemNoListDto>> GetAllPOItemNumberListByPoNumber(string poNumber);
        Task<int?> GetPONumberAutoIncrementCount(DateTime date);
        Task<PurchaseOrder> ChangePurchaseOrderVersion(PurchaseOrder purchaseOrder);
        Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrderWithItems(PurchaseOrderSearchDto purchaseOrderSearch);
        Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrder([FromQuery] SearchParamess searchParammes);
        Task<IEnumerable<PurchaseOrder>> SearchPurchaseOrderDate([FromQuery] SearchDatesParams searchDatesParams);
        Task<IEnumerable<PurchaseOrderRevNoListDto>> GetAllRevisionNumberListByPoNumber(string poNumber);
        Task<PurchaseRequisition> GetPrDetailsByPrNumber(string prNumber);

        Task<decimal> GetOpenPoQuantityByItemNumber(string itemNumber);
        Task<string> GeneratePONumberForAvision();
    }
}
