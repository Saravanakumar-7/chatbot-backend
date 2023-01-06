using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;

using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Repository
{
    public class PurchaseOrderRepository : RepositoryBase<PurchaseOrder>, IPurchaseOrderRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        public PurchaseOrderRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
        }

        public async Task<long> CreatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            purchaseOrder.CreatedBy = "Admin";
            purchaseOrder.CreatedOn = DateTime.Now;
            purchaseOrder.Unit = "Bangalore";
            var result = await Create(purchaseOrder);
            return result.Id;
        }

        public async Task<string> DeletePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            Delete(purchaseOrder);
            string result = $"PurchaseOrder details of {purchaseOrder.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllActivePurchaseOrder()
        {
            var allActivePurchaseOrder = await FindAll().ToListAsync();
            return allActivePurchaseOrder;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllActivePurchaseOrderNameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> allActivePurchaseOrderNameList = await _tipsPurchaseDbContext.PurchaseOrders
                                .Select(x => new PurchaseOrderIdNameListDto()
                                {
                                    Id = x.Id,
                                    PONumber = x.PONumber,
                                })
                              .ToListAsync();

            return allActivePurchaseOrderNameList;
        }
        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPurchaseOrderApprovalINameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> allPendingPOApprovalINameList = await _tipsPurchaseDbContext.PurchaseOrders
                            .Where(x => x.POApprovalI == false).Select(x => new PurchaseOrderIdNameListDto()
                            {
                                Id = x.Id,
                                PONumber = x.PONumber,
                            }).ToListAsync();


            return allPendingPOApprovalINameList;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPurchaseOrderApprovalIINameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> allPendingPOApprovalIINameList = await _tipsPurchaseDbContext.PurchaseOrders
                            .Where(x => x.POApprovalII == false).Select(x => new PurchaseOrderIdNameListDto()
                            {
                                Id = x.Id,
                                PONumber = x.PONumber,
                            }).ToListAsync();


            return allPendingPOApprovalIINameList;
        }

        public async Task<PagedList<PurchaseOrder>> GetAllPurchaseOrder(PagingParameter pagingParameter)
        {

            var getAllPurchaseOrderDetails = PagedList<PurchaseOrder>.ToPagedList(FindAll()
                                .Include(t => t.POItemList)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItemList)
                                .ThenInclude(i => i.POAddDeliverySchedules)

               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllPurchaseOrderDetails;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderById(int id)
        {
            var purchaseOrderbyId = await _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.Id == id)
                                .Include(t => t.POItemList)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItemList)
                                .ThenInclude(i => i.POAddDeliverySchedules)

                                .FirstOrDefaultAsync();


            return purchaseOrderbyId;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByPONumber(string PONumber)
        {
            var purchaseOrderDetailsbyPONumber = await _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.PONumber == PONumber)
                                .Include(t => t.POItemList)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItemList)
                                .ThenInclude(i => i.POAddDeliverySchedules)

                                .FirstOrDefaultAsync();


            return purchaseOrderDetailsbyPONumber;
        }

        public async Task<string> UpdatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            purchaseOrder.LastModifiedBy = "Admin";
            purchaseOrder.LastModifiedOn = DateTime.Now;
            Update(purchaseOrder);
            string result = $"PurchaseOrder of Detail {purchaseOrder.Id} is updated successfully!";
            return result;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPoNumberListByVendorName(string vendorName)
        {
            IEnumerable<PurchaseOrderIdNameListDto> getAllPONumberListByVendorName = await _tipsPurchaseDbContext.PurchaseOrders
                           .Where(x => x.VendorName == vendorName).Select(x => new PurchaseOrderIdNameListDto()
                           {
                               Id = x.Id,
                               PONumber = x.PONumber,
                           }).ToListAsync();


            return getAllPONumberListByVendorName;
        }

        public async Task<IEnumerable<PurchaseOrderItemNoListDto>> GetAllPoItemNumberListByPoNumber(string poNumber)
        {
            int getAllPOItemNoByPONumber = await _tipsPurchaseDbContext.PurchaseOrders
                .Where(s => s.PONumber == poNumber).Select(x => x.Id).FirstOrDefaultAsync();

            IEnumerable<PurchaseOrderItemNoListDto> purchaseOrderItemNoLists = await _tipsPurchaseDbContext.PoItems
                          .Where(x => x.PurchaseOrderId == getAllPOItemNoByPONumber)
                          .Select(r => new PurchaseOrderItemNoListDto()
                          {
                              Id = r.Id,
                              ItemNumber = r.ItemNumber
                          }).ToListAsync();

            return purchaseOrderItemNoLists;
        }

    }
}
