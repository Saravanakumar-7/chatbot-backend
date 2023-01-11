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
            Guid purchaseOrderNumber = Guid.NewGuid();
            purchaseOrder.PONumber = "PO-" + purchaseOrderNumber.ToString();
            purchaseOrder.Unit = "Banglore";
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
            var AllActivepurchaseOrder = await FindAll().ToListAsync();
            return AllActivepurchaseOrder;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllActivePurchaseOrderNameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> AllActivePurchaseOrderName = await _tipsPurchaseDbContext.PurchaseOrders
                                .Select(x => new PurchaseOrderIdNameListDto()
                                {
                                    Id = x.Id,
                                    PONumber = x.PONumber,
                                })
                              .ToListAsync();

            return AllActivePurchaseOrderName;
        }
        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPurchaseOrderApprovalINameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> AllPendingPOApprovalIName = await _tipsPurchaseDbContext.PurchaseOrders
                            .Where(x => x.POApprovalI == false).Select(x => new PurchaseOrderIdNameListDto()
                            {
                                Id = x.Id,
                                PONumber = x.PONumber,
                            }).ToListAsync();


            return AllPendingPOApprovalIName;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPurchaseOrderApprovalIINameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> AllPendingPOApprovalIIName = await _tipsPurchaseDbContext.PurchaseOrders
                            .Where(x => x.POApprovalII == false).Select(x => new PurchaseOrderIdNameListDto()
                            {
                                Id = x.Id,
                                PONumber = x.PONumber,
                            }).ToListAsync();


            return AllPendingPOApprovalIIName;
        }

        public async Task<PagedList<PurchaseOrder>> GetAllPurchaseOrder(PagingParameter pagingParameter)
        {

            var GetallpurchaseOrderDetails = PagedList<PurchaseOrder>.ToPagedList(FindAll()
                                .Include(t => t.POItemList)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItemList)
                                .ThenInclude(i => i.POAddDeliverySchedules)

               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return GetallpurchaseOrderDetails;
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
            var purchaseOrderbyPONumber = await _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.PONumber == PONumber)
                                .Include(t => t.POItemList)
                                .ThenInclude(x => x.POAddprojects)
                                .Include(m => m.POItemList)
                                .ThenInclude(i => i.POAddDeliverySchedules)

                                .FirstOrDefaultAsync();


            return purchaseOrderbyPONumber;
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
            IEnumerable<PurchaseOrderIdNameListDto> AllPObyVendorName = await _tipsPurchaseDbContext.PurchaseOrders
                           .Where(x => x.VendorName == vendorName).Select(x => new PurchaseOrderIdNameListDto()
                           {
                               Id = x.Id,
                               PONumber = x.PONumber,
                           }).ToListAsync();


            return AllPObyVendorName;
        }

        public async Task<IEnumerable<PurchaseOrderItemNoListDto>> GetAllPoItemNumberListByPoNumber(string poNumber)
        {
            int AllPOItemNobyPONumber = await _tipsPurchaseDbContext.PurchaseOrders
                .Where(s => s.PONumber == poNumber).Select(x => x.Id).FirstOrDefaultAsync();

            IEnumerable<PurchaseOrderItemNoListDto> itemDetails = await _tipsPurchaseDbContext.PoItems
                          .Where(x => x.PurchaseOrderId == AllPOItemNobyPONumber)
                          .Select(r => new PurchaseOrderItemNoListDto()
                          {
                              Id = r.Id,
                              ItemNumber = r.ItemNumber
                          }).ToListAsync();

            return itemDetails;
        }

    }
}
