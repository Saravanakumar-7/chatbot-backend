
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
            var date = DateTime.Now;
            purchaseRequisitions.CreatedBy = "Admin";
            purchaseRequisitions.CreatedOn = date.Date;
           // Guid purchaseRequisitionsNumber = Guid.NewGuid();
           // purchaseRequisitions.PRNumber = "PR-" + purchaseRequisitionsNumber.ToString();
            purchaseRequisitions.Unit = "Bangalore";
            purchaseRequisitions.RevisionNumber = 1;
            var result = await Create(purchaseRequisitions);
            return result.Id;
        }
        public async Task<int?> GetPRNumberAutoIncrementCount(DateTime date)
        {
            var getPRNumberAutoIncrementCount = _tipsPurchaseDbContext.PurchaseRequisitions.Where(x => x.CreatedOn == date.Date).Count();

            return getPRNumberAutoIncrementCount;
        }

        public async Task<PurchaseRequisition> ChangePurchaseRequisitionVersion(PurchaseRequisition purchaseRequisition)
        {
            purchaseRequisition.CreatedBy = "Admin";
            purchaseRequisition.CreatedOn = DateTime.Now;
            purchaseRequisition.Unit = "Bangalore";
            var getOldRevisionNumber = _tipsPurchaseDbContext.PurchaseRequisitions
                .Where(x => x.PrNumber == purchaseRequisition.PrNumber)
                .OrderByDescending(x => x.Id)
                .Select(x => x.RevisionNumber)
                .FirstOrDefault();
          
            var increaseVersionNumber = 1;
            var convertversionnumber = (increaseVersionNumber);
            var version = getOldRevisionNumber + convertversionnumber;
            purchaseRequisition.RevisionNumber = (version);
            var result = await Create(purchaseRequisition);
            return result;
        }

        public  async Task<string> DeletePurchaseRequisition(PurchaseRequisition purchaseRequisitions)
        {
            Delete(purchaseRequisitions);
            string result = $"PurchaseRequistion details of {purchaseRequisitions.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PurchaseRequisition>> GetAllActivePurchaseRequisitions()
        {
            var activePurchaseRequistionDetails = await FindAll().ToListAsync();
            return activePurchaseRequistionDetails;
        }

        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllActivePurchaseRequisitionNameList()
        {
            IEnumerable<PurchaseRequisitionIdNameListDto> activePRNamelist = await _tipsPurchaseDbContext.PurchaseRequisitions
                                .Select(x => new PurchaseRequisitionIdNameListDto()
                                {
                                    Id = x.Id,
                                    PrNumber = x.PrNumber,
                                })
                              .ToListAsync();

            return activePRNamelist;
        }

        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalINameList()
        {
            IEnumerable<PurchaseRequisitionIdNameListDto> pendingPRApprovalINameList = await _tipsPurchaseDbContext.PurchaseRequisitions
                            .Where(x => x.PrApprovalI == false).Select(x => new PurchaseRequisitionIdNameListDto()
                            {
                                Id = x.Id,
                                PrNumber = x.PrNumber,
                            }).ToListAsync();


            return pendingPRApprovalINameList;
        }

        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalIINameList()
        {
            IEnumerable<PurchaseRequisitionIdNameListDto> pendingPRApprovalIINameList = await _tipsPurchaseDbContext.PurchaseRequisitions
                            .Where(x => x.PrApprovalII == false).Select(x => new PurchaseRequisitionIdNameListDto()
                            {
                                Id = x.Id,
                                PrNumber = x.PrNumber,
                            }).ToListAsync();


            return pendingPRApprovalIINameList;
        }

        public async Task<PagedList<PurchaseRequisition>> GetAllPurchaseRequisitions(PagingParameter pagingParameter)
        {
  
            var purchaseRequistionDetails = PagedList<PurchaseRequisition>.ToPagedList(FindAll()
                                .Include(t => t.PrItemList)
                                .ThenInclude(x => x.PrAddprojects)
                                .Include(m => m.PrItemList)
                                .ThenInclude(i => i.PrAddDeliverySchedules)
               .OrderByDescending(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return purchaseRequistionDetails;
        }

        public async Task<PurchaseRequisition> GetPurchaseRequisitionById(int id)
        {
            var purchaseRequistionDetailById = await _tipsPurchaseDbContext.PurchaseRequisitions.Where(x => x.Id == id)
                                 .Include(t => t.PrItemList)
                                 .ThenInclude(x => x.PrAddprojects)
                                .Include(m => m.PrItemList)
                                .ThenInclude(i => i.PrAddDeliverySchedules)
                                .FirstOrDefaultAsync();

            return purchaseRequistionDetailById;
        }

        public async Task<PurchaseRequisition> GetPurchaseRequisitionByPRNumber(string prNumber)
        {
            var purchaseRequistionByPRNumber = await _tipsPurchaseDbContext.PurchaseRequisitions.Where(x => x.PrNumber == prNumber)
                                 .Include(t => t.PrItemList)
                                 .ThenInclude(x => x.PrAddprojects)
                                .Include(m => m.PrItemList)
                                .ThenInclude(i => i.PrAddDeliverySchedules)
                                .FirstOrDefaultAsync();

            return purchaseRequistionByPRNumber;
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
