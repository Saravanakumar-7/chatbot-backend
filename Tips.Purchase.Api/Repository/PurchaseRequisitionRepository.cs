
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
            var AllActivepurchaseRequistions = await FindAll().ToListAsync();
            return AllActivepurchaseRequistions;
        }

        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllActivePurchaseRequisitionNameList()
        {
            IEnumerable<PurchaseRequisitionIdNameListDto> AllActivepurchaseRequistionsName = await _tipsPurchaseDbContext.PurchaseRequisitions
                                .Select(x => new PurchaseRequisitionIdNameListDto()
                                {
                                    Id = x.Id,
                                    PRNumber = x.PRNumber,
                                })
                              .ToListAsync();

            return AllActivepurchaseRequistionsName;
        }

        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPurchaseRequisitionApprovalINameList()
        {
            IEnumerable<PurchaseRequisitionIdNameListDto> AllPendingPRApprovalIName = await _tipsPurchaseDbContext.PurchaseRequisitions
                            .Where(x => x.PRApprovalI == false).Select(x => new PurchaseRequisitionIdNameListDto()
                            {
                                Id = x.Id,
                                PRNumber = x.PRNumber,
                            }).ToListAsync();


            return AllPendingPRApprovalIName;
        }

        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPurchaseRequisitionApprovalIINameList()
        {
            IEnumerable<PurchaseRequisitionIdNameListDto> AllPendingPRApprovalIIName = await _tipsPurchaseDbContext.PurchaseRequisitions
                            .Where(x => x.PRApprovalII == false).Select(x => new PurchaseRequisitionIdNameListDto()
                            {
                                Id = x.Id,
                                PRNumber = x.PRNumber,
                            }).ToListAsync();


            return AllPendingPRApprovalIIName;
        }

        public async Task<PagedList<PurchaseRequisition>> GetAllPurchaseRequisition(PagingParameter pagingParameter)
        {
  
            var GetallPurchaseRequistionDetails = PagedList<PurchaseRequisition>.ToPagedList(FindAll()
                                .Include(t => t.PrItemList)
                                .ThenInclude(x => x.PrAddprojects)
                                .Include(m => m.PrItemList)
                                .ThenInclude(i => i.PrAddDeliverySchedules)
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return GetallPurchaseRequistionDetails;
        }

        public async Task<PurchaseRequisition> GetPurchaseRequisitionById(int id)
        {
            var PurchaseRequistionbyId = await _tipsPurchaseDbContext.PurchaseRequisitions.Where(x => x.Id == id)
                                 .Include(t => t.PrItemList)
                                 .ThenInclude(x => x.PrAddprojects)
                                .Include(m => m.PrItemList)
                                .ThenInclude(i => i.PrAddDeliverySchedules)
                                .FirstOrDefaultAsync();

            return PurchaseRequistionbyId;
        }

        public async Task<PurchaseRequisition> GetPurchaseRequisitionByPRNumber(string PRNumber)
        {
            var PurchaseRequistionByPRNumber = await _tipsPurchaseDbContext.PurchaseRequisitions.Where(x => x.PRNumber == PRNumber)
                                 .Include(t => t.PrItemList)
                                 .ThenInclude(x => x.PrAddprojects)
                                .Include(m => m.PrItemList)
                                .ThenInclude(i => i.PrAddDeliverySchedules)
                                .FirstOrDefaultAsync();

            return PurchaseRequistionByPRNumber;
        }

        public async Task<string> UpdatePurchaseRequisition(PurchaseRequisition purchaseRequisitions)
        {
            purchaseRequisitions.LastModifiedBy = "Admin";
            purchaseRequisitions.LastModifiedOn = DateTime.Now;
            Update(purchaseRequisitions);
            string result = $"PurchaseRequisitions of Detail {purchaseRequisitions.Id} is updated successfully!";
            return result;
        }
        
    }
}
