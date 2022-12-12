using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;

using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
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
            purchaseOrder.LastModifiedBy = "Admin";
            purchaseOrder.LastModifiedOn=DateTime.Now;
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
            var purchaseOrderDetails = await FindAll().ToListAsync();
            return purchaseOrderDetails;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllActivePurchaseOrderNameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> purchaseOrderDetails = await _tipsPurchaseDbContext.PurchaseOrders
                                .Select(x => new PurchaseOrderIdNameListDto()
                                {
                                    Id = x.Id,
                                    PONumber = x.PONumber,
                                })
                              .ToListAsync();

            return purchaseOrderDetails;
        }
        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPurchaseOrderApprovalINameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> purchaseOrderDetails = await _tipsPurchaseDbContext.PurchaseOrders
                            .Where(x=>x.POApprovalI==false).Select(x => new PurchaseOrderIdNameListDto()
                            {
                               Id = x.Id,
                               PONumber = x.PONumber,
                            }).ToListAsync();
                              

            return purchaseOrderDetails;
        }

        public async Task<IEnumerable<PurchaseOrderIdNameListDto>> GetAllPendingPurchaseOrderApprovalIINameList()
        {
            IEnumerable<PurchaseOrderIdNameListDto> purchaseOrderDetails = await _tipsPurchaseDbContext.PurchaseOrders
                            .Where(x => x.POApprovalII == false).Select(x => new PurchaseOrderIdNameListDto()
                            {
                                Id = x.Id,
                                PONumber = x.PONumber,
                            }).ToListAsync();


            return purchaseOrderDetails;
        }

        public async Task<PagedList<PurchaseOrder>> GetAllPurchaseOrder(PagingParameter pagingParameter)
        {

            var purchaseOrderDetails = PagedList<PurchaseOrder>.ToPagedList(FindAll()
                                .Include(t => t.poItems)
                                .ThenInclude(x => x.poAddprojects)
                                .Include(m => m.poItems)
                                .ThenInclude(i => i.poAddDeliverySchedules)
                                
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return purchaseOrderDetails;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderById(int id)
        {
            var purchaseOrderDetails = await _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.Id == id)
                                .Include(t => t.poItems)
                                .ThenInclude(x => x.poAddprojects)
                                .Include(m => m.poItems)
                                .ThenInclude(i => i.poAddDeliverySchedules)
                                
                                .FirstOrDefaultAsync();
                                

            return purchaseOrderDetails;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByPONumber(string PONumber)
        {
            var purchaseOrderDetails = await _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.PONumber == PONumber)
                                .Include(t => t.poItems)
                                .ThenInclude(x => x.poAddprojects)
                                .Include(m => m.poItems)
                                .ThenInclude(i => i.poAddDeliverySchedules)

                                .FirstOrDefaultAsync();


            return purchaseOrderDetails;
        }

        public async Task<string> UpdatePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            purchaseOrder.LastModifiedBy = "Admin";
            purchaseOrder.LastModifiedOn = DateTime.Now;
            Update(purchaseOrder);
            string result = $"PurchaseOrder of Detail {purchaseOrder.Id} is updated successfully!";
            return result;
        }
    }       
}
