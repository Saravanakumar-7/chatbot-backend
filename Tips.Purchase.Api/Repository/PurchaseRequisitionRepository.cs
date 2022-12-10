
using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;

using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Repository
{
    public class PurchaseRequisitionRepository : RepositoryBase<PurchaseRequisition>, IPurchaseRequisitionRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        public PurchaseRequisitionRepository(TipsPurchaseDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsPurchaseDbContext= repositoryContext;
        }

        public async Task<long> CreatePurchaseRequisition(PurchaseRequisition purchaseRequisitions)
        {
            purchaseRequisitions.CreatedBy = "Admin";
            purchaseRequisitions.CreatedOn = DateTime.Now;
            purchaseRequisitions.LastModifiedBy = "Admin";
            purchaseRequisitions.LastModifiedOn= DateTime.Now;
            var result = await Create(purchaseRequisitions);
            return result.Id;
        }

        public  async Task<string> DeletePurchaseRequisition(PurchaseRequisition purchaseRequisitions)
        {
            Delete(purchaseRequisitions);
            string result = $"PurchaseRequistion details of {purchaseRequisitions.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PurchaseRequisition>> GetAllActivePurchaseRequisition()
        {
            var purchaseRequistionDetails = await FindAll().ToListAsync();
            return purchaseRequistionDetails;
        }

        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllActivePurchaseRequisitionNameList()
        {
            IEnumerable<PurchaseRequisitionIdNameListDto> purchaseRequistionDetails = await _tipsPurchaseDbContext.PurchaseRequisition
                                .Select(x => new PurchaseRequisitionIdNameListDto()
                                {
                                    Id = x.Id,
                                    PRNumber = x.PRNumber,
                                })
                              .ToListAsync();

            return purchaseRequistionDetails;
        }

        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPurchaseRequisitionApprovalINameList()
        {
            IEnumerable<PurchaseRequisitionIdNameListDto> purchaseRequistionDetails = await _tipsPurchaseDbContext.PurchaseRequisition
                            .Where(x => x.PRApprovalI == false).Select(x => new PurchaseRequisitionIdNameListDto()
                            {
                                Id = x.Id,
                                PRNumber = x.PRNumber,
                            }).ToListAsync();


            return purchaseRequistionDetails;
        }

        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPurchaseRequisitionApprovalIINameList()
        {
            IEnumerable<PurchaseRequisitionIdNameListDto> purchaseRequistionDetails = await _tipsPurchaseDbContext.PurchaseRequisition
                            .Where(x => x.PRApprovalII == false).Select(x => new PurchaseRequisitionIdNameListDto()
                            {
                                Id = x.Id,
                                PRNumber = x.PRNumber,
                            }).ToListAsync();


            return purchaseRequistionDetails;
        }

        public async Task<PagedList<PurchaseRequisition>> GetAllPurchaseRequisition(PagingParameter pagingParameter)
        {
  
            var purchaseRequistionDetails = PagedList<PurchaseRequisition>.ToPagedList(FindAll()
                                .Include(t => t.prItems)
                                .ThenInclude(x => x.prAddprojects)
                                .Include(m => m.prItems)
                                .ThenInclude(i => i.prAddDeliverySchedules)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return purchaseRequistionDetails;
        }

        public async Task<PurchaseRequisition> GetPurchaseRequisitionById(int id)
        {
            var purchaseRequistionDetails = await _tipsPurchaseDbContext.PurchaseRequisition.Where(x => x.Id == id)
                                 .Include(t => t.prItems)
                                 .ThenInclude(x => x.prAddprojects)
                                .Include(m => m.prItems)
                                .ThenInclude(i => i.prAddDeliverySchedules)
                                .FirstOrDefaultAsync();

            return purchaseRequistionDetails;
        }

        public async Task<PurchaseRequisition> GetPurchaseRequisitionByPRNumber(string PRNumber)
        {
            var purchaseRequistionDetails = await _tipsPurchaseDbContext.PurchaseRequisition.Where(x => x.PRNumber == PRNumber)
                                 .Include(t => t.prItems)
                                 .ThenInclude(x => x.prAddprojects)
                                .Include(m => m.prItems)
                                .ThenInclude(i => i.prAddDeliverySchedules)
                                .FirstOrDefaultAsync();

            return purchaseRequistionDetails;
        }

        public async Task<string> UpdatePurchaseRequisition(PurchaseRequisition purchaseRequisitions)
        {
            purchaseRequisitions.LastModifiedBy = "Admin";
            purchaseRequisitions.LastModifiedOn = DateTime.Now;
            Update(purchaseRequisitions);
            string result = $"PurchaseRequisition of Detail {purchaseRequisitions.Id} is updated successfully!";
            return result;
        }
        
    }
}
