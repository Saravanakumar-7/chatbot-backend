
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IEnumerable<PurchaseRequisition>> SearchPurchaseRequisition([FromQuery] SearchParamess searchParammes)
        {
            using (var context = _tipsPurchaseDbContext)
            {
                var query = _tipsPurchaseDbContext.PurchaseRequisitions.Include("PrItems");
                if (!string.IsNullOrEmpty(searchParammes.SearchValue))
                {
                    query = query.Where(po => po.PrNumber.Contains(searchParammes.SearchValue)
                    || po.PrDate.ToString().Contains(searchParammes.SearchValue)
                   //|| po.RevisionNumber.Equals(int.Parse(searchParammes.SearchValue))
                                   || po.ProcurementType.Contains(searchParammes.SearchValue)
                    || po.ShippingMode.Contains(searchParammes.SearchValue)
                    || po.PaymentTerms.Contains(searchParammes.SearchValue)
                    || po.DeliveryTerms.Contains(searchParammes.SearchValue)
                    || po.PrItemList.Any(s => s.ItemNumber.Contains(searchParammes.SearchValue) ||
                    s.Description.Contains(searchParammes.SearchValue)
                    || s.MftrItemNumber.Contains(searchParammes.SearchValue)));
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<PurchaseRequisition>> GetAllPurchaseRequisitionWithItems(PurchaseRequisitionSearchDto purchaseRequisitionSearch)
        {
            using (var context = _tipsPurchaseDbContext)
            {
                var query = _tipsPurchaseDbContext.PurchaseRequisitions.Include("PrItemList");
                if (purchaseRequisitionSearch != null || (purchaseRequisitionSearch.PrNumber.Any())
               && purchaseRequisitionSearch.ProcurementType.Any() && purchaseRequisitionSearch.ItemNumber.Any() && purchaseRequisitionSearch.PRStatus.Any())
                {
                    query = query.Where
                    (po => (purchaseRequisitionSearch.PrNumber.Any() ? purchaseRequisitionSearch.PrNumber.Contains(po.PrNumber) : true)
                   && (purchaseRequisitionSearch.ProcurementType.Any() ? purchaseRequisitionSearch.ProcurementType.Contains(po.ProcurementType) : true)
                   //&& (purchaseRequisitionSearch.ItemNumber.Any() ? purchaseRequisitionSearch.ItemNumber.Contains(po.PrItemList.)) : true)
                   && (purchaseRequisitionSearch.PRStatus.Any() ? purchaseRequisitionSearch.PRStatus.Contains(po.Status) : true));
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<PurchaseRequisition>> SearchPurchaseRequisitionDate([FromQuery] SearchDatesParams searchDatesParams)
        {
            var purchaseRequsisitionDetails = _tipsPurchaseDbContext.PurchaseRequisitions
            .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
            inv.CreatedOn <= searchDatesParams.SearchToDate
            )))
            .Include(itm => itm.PrItemList)
            .ToList();
            return purchaseRequsisitionDetails;
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

        public async Task<PagedList<PurchaseRequisition>> GetAllActivePurchaseRequisitions([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            var activePurchaseRequsitionDetails = FindAll()
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PrNumber.Contains(searchParams.SearchValue)
               || inv.RevisionNumber.Equals(int.Parse(searchParams.SearchValue))
               || inv.PrDate.Equals(int.Parse(searchParams.SearchValue)))))
                                .Include(o => o.PrFiles)
                                .Include(t => t.PrItemList)
                                .ThenInclude(x => x.PrAddprojects)
                                .Include(m => m.PrItemList)
                                .ThenInclude(i => i.PrAddDeliverySchedules);
            return PagedList<PurchaseRequisition>.ToPagedList(activePurchaseRequsitionDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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

        public async Task<PagedList<PurchaseRequisition>> GetAllPurchaseRequisitions([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {

            var purchaseRequistionDetails = FindAll().OrderByDescending(on => on.Id)

                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PrNumber.Contains(searchParams.SearchValue)
               || inv.RevisionNumber.Equals(int.Parse(searchParams.SearchValue))
               || inv.PrDate.Equals(int.Parse(searchParams.SearchValue))))).Include(f => f.PrFiles)
                                  .Include(t => t.PrItemList)
                                  .ThenInclude(x => x.PrAddprojects)
                                  .Include(m => m.PrItemList)
                                  .ThenInclude(i => i.PrAddDeliverySchedules);
            return PagedList<PurchaseRequisition>.ToPagedList(purchaseRequistionDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<PurchaseRequisition> GetPurchaseRequisitionById(int id)
        {
            var purchaseRequistionDetailById = await _tipsPurchaseDbContext.PurchaseRequisitions.Where(x => x.Id == id)
                                .Include(o => o.PrFiles)
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
                                  .Include(o => o.PrFiles)
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
        public async Task<IEnumerable<GetPRDownloadUrlDto>> GetDownloadUrlDetail(string prNumber)
        { 
            IEnumerable<GetPRDownloadUrlDto> getDownloadDetails = await _tipsPurchaseDbContext.DocumentUploads
                                .Where(b => b.ParentNumber == prNumber)
                                .Select(x => new GetPRDownloadUrlDto()
                                {
                                    Id = x.Id,
                                    FileName = x.FileName,
                                    FileExtension = x.FileExtension,
                                    FilePath = x.FilePath
                                })
                              .ToListAsync();

            return getDownloadDetails;
        }

    }

    public class PRUploadDocumentRepository : RepositoryBase<DocumentUpload>, IDocumentUploadRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        public PRUploadDocumentRepository(TipsPurchaseDbContext tipsPurchaseDbContext) : base(tipsPurchaseDbContext)
        {
            _tipsPurchaseDbContext = tipsPurchaseDbContext;
        }

        public async Task<int?> CreateUploadDocumentPO(DocumentUpload documentUpload)
        {
            documentUpload.CreatedBy = "Admin";
            documentUpload.CreatedOn = DateTime.Now;
            documentUpload.LastModifiedBy = "Admin";
            documentUpload.LastModifiedOn = DateTime.Now;

            var result = await Create(documentUpload);
            return result.Id;
        }
        public async Task<DocumentUpload> GetUploadDocById(int id)
        {
            var uploadDocFileNameById = await _tipsPurchaseDbContext.DocumentUploads
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            return uploadDocFileNameById;
        }
        public async Task<string> DeleteUploadFile(DocumentUpload documentUpload)
        {
            Delete(documentUpload);
            string result = $"DocumentUpload details of {documentUpload.Id} is deleted successfully!";
            return result;
        }
    }

}
