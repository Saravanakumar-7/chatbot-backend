
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.Purchase.Api.Contracts;
using System.Security.Claims;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.DTOs;
using System.Reflection;
using Tips.Purchase.Api.Entities.Enums;
using System.Collections.Generic;

namespace Tips.Purchase.Api.Repository
{
    public class PurchaseRequisitionRepository : RepositoryBase<PurchaseRequisition>, IPurchaseRequisitionRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PurchaseRequisitionRepository(TipsPurchaseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsPurchaseDbContext= repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<IEnumerable<GetDownloadUrlDto>> GetDownloadUrlDetails(string prNumber, string prItemNumber)
        {
            var PRid = await _tipsPurchaseDbContext.PurchaseRequisitions.Where(x => x.PrNumber == prNumber).Select(x=>x.Id).FirstOrDefaultAsync();
            var FileIds = await _tipsPurchaseDbContext.PrItems.Where(x => x.PurchaseRequistionId == PRid && x.ItemNumber == prItemNumber).Select(x => x.PRFileIds).FirstOrDefaultAsync();
            if (FileIds != null) {
                string[]? ids = FileIds.Split(',');
                List<GetDownloadUrlDto> getDownloadDetails = new List<GetDownloadUrlDto>();
                for (int i = 0; i < ids.Count(); i++)
                {
                    GetDownloadUrlDto getDownloadDetails_1 = await _tipsPurchaseDbContext.PRItemsDocumentUploads
                                    .Where(b => b.Id == Convert.ToInt32(ids[i]))
                                    .Select(x => new GetDownloadUrlDto()
                                    {
                                        Id = x.Id,
                                        FileName = x.FileName,
                                        FileExtension = x.FileExtension,
                                        FilePath = x.FilePath
                                    })
                                  .FirstOrDefaultAsync();

                    getDownloadDetails.Add(getDownloadDetails_1);
                }
                return getDownloadDetails;
            }
            else
            {
                return null;
            }
        }
        public async Task<long> CreatePurchaseRequisition(PurchaseRequisition purchaseRequisitions)
        {
            var date = DateTime.Now;
            purchaseRequisitions.CreatedBy = _createdBy;
            purchaseRequisitions.CreatedOn = date.Date;
           // Guid purchaseRequisitionsNumber = Guid.NewGuid();
           // purchaseRequisitions.PRNumber = "PR-" + purchaseRequisitionsNumber.ToString();
            purchaseRequisitions.Unit = _unitname;
            purchaseRequisitions.RevisionNumber = 1;
            var result = await Create(purchaseRequisitions);
            return result.Id;
        }
        public async Task<int?> GetPRNumberAutoIncrementCount(DateTime date)
        {
            var getPRNumberAutoIncrementCount = _tipsPurchaseDbContext.PurchaseRequisitions.Where(x => x.CreatedOn == date.Date).Count();

            return getPRNumberAutoIncrementCount;
        }
        public async Task<string> GeneratePRNumberAvision()
        {
            using var transaction = await _tipsPurchaseDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var rfqNumberEntity = await _tipsPurchaseDbContext.PRNumbers.SingleAsync();
                rfqNumberEntity.CurrentValue += 1;
                _tipsPurchaseDbContext.Update(rfqNumberEntity);
                await _tipsPurchaseDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                //int currentYear = DateTime.Now.Year % 100; // Get the last two digits of the current year
                //int nextYear = (DateTime.Now.Year + 1) % 100; // Get the last two digits of the next year

                DateTime currentDate = DateTime.Now;
                DateTime financeYearStart;

                if (currentDate.Month >= 4) // Check if the current date is after or equal to April
                {
                    financeYearStart = new DateTime(currentDate.Year, 4, 1);
                }
                else
                {
                    financeYearStart = new DateTime(currentDate.Year - 1, 4, 1);
                }

                int currentYear = financeYearStart.Year % 100; // Get the last two digits of the current finance year
                int nextYear = (financeYearStart.Year + 1) % 100; // Get the last two digits of the next finance year

                return $"ASPL|PR|{currentYear:D2}-{nextYear:D2}|{rfqNumberEntity.CurrentValue:D3}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<string> GeneratePRNumber()
        {
            using var transaction = await _tipsPurchaseDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var prNumberEntity = await _tipsPurchaseDbContext.PRNumbers.SingleAsync();
                prNumberEntity.CurrentValue += 1;
                _tipsPurchaseDbContext.Update(prNumberEntity);
                await _tipsPurchaseDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"PR-{prNumberEntity.CurrentValue:D5}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }


        public async Task<IEnumerable<PurchaseRequisition>> SearchPurchaseRequisition([FromQuery] SearchParamess searchParammes)
        {
            using (var context = _tipsPurchaseDbContext)
            {
                var query = _tipsPurchaseDbContext.PurchaseRequisitions.Include("PrItemsDtoList");
                if (!string.IsNullOrEmpty(searchParammes.SearchValue))
                {
                    query = query.Where(po => po.PrNumber.Contains(searchParammes.SearchValue)
                    || po.PrDate.ToString().Contains(searchParammes.SearchValue)
                    //|| po.RevisionNumber.Equals(int.Parse(searchParammes.SearchValue))
                    || po.ProcurementType.Contains(searchParammes.SearchValue)
                    || po.ShippingMode.Contains(searchParammes.SearchValue)
                    || po.PaymentTerms.Contains(searchParammes.SearchValue)
                    || po.DeliveryTerms.Contains(searchParammes.SearchValue)
                    || po.PrItemsDtoList.Any(s => s.ItemNumber.Contains(searchParammes.SearchValue) ||
                    s.Description.Contains(searchParammes.SearchValue)
                    || s.MftrItemNumber.Contains(searchParammes.SearchValue)))
                         .Include(itm => itm.PrItemsDtoList)
                        .ThenInclude(pr => pr.prAddprojectsDtoList)
                         .Include(itm => itm.PrItemsDtoList)
                        .ThenInclude(pr => pr.prAddDeliverySchedulesDtoList)
                          .Include(itm => itm.PrItemsDtoList)
                        .ThenInclude(pr => pr.prSpecialInstructionsDtoList);
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<PurchaseRequisition>> GetAllPurchaseRequisitionWithItems(PurchaseRequisitionSearchDto purchaseRequisitionSearch)
        {
            using (var context = _tipsPurchaseDbContext)
            {
                var query = _tipsPurchaseDbContext.PurchaseRequisitions.Include("PrItemsDtoList");
                if (purchaseRequisitionSearch != null || (purchaseRequisitionSearch.PrNumber.Any())
               && purchaseRequisitionSearch.ProcurementType.Any() && purchaseRequisitionSearch.ShippingMode.Any() && purchaseRequisitionSearch.PRStatus.Any())
                {
                    query = query.Where
                    (po => (purchaseRequisitionSearch.PrNumber.Any() ? purchaseRequisitionSearch.PrNumber.Contains(po.PrNumber) : true)
                   && (purchaseRequisitionSearch.ProcurementType.Any() ? purchaseRequisitionSearch.ProcurementType.Contains(po.ProcurementType) : true)
                   && (purchaseRequisitionSearch.ShippingMode.Any() ? purchaseRequisitionSearch.ShippingMode.Contains(po.ShippingMode) : true)
                   && (purchaseRequisitionSearch.PRStatus.Any() ? purchaseRequisitionSearch.PRStatus.Contains(po.Status) : true))
                    .Include(itm => itm.PrItemsDtoList)
                    .ThenInclude(pr => pr.prAddprojectsDtoList)
                    .Include(itm => itm.PrItemsDtoList)
                    .ThenInclude(pr => pr.prAddDeliverySchedulesDtoList)
                    .Include(itm => itm.PrItemsDtoList)
                     .ThenInclude(pr => pr.prSpecialInstructionsDtoList);
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<PurchaseRequistionRevNoListDto>> GetAllRevisionNumberListByPRNumber(string prNumber)
        {
            IEnumerable<PurchaseRequistionRevNoListDto> revNoListbyPRNumber = await _tipsPurchaseDbContext.PurchaseRequisitions
            .Where(x => x.PrNumber == prNumber).Select(x => new PurchaseRequistionRevNoListDto()
            {
                RevisionNumber = x.RevisionNumber,
            }).ToListAsync();
            return revNoListbyPRNumber;
        }

        public async Task<PurchaseRequisition> GetPurchaseRequisitionByPRNoAndRevNo(string prNumber, int revisionNumber)
        {
            var purchaseRequisitionDetail = await _tipsPurchaseDbContext.PurchaseRequisitions
            .Where(x => x.PrNumber == prNumber && x.RevisionNumber == revisionNumber && x.IsDeleted == false)
            .Include(o => o.PrFiles)
            .Include(t => t.PrItemsDtoList)
            .ThenInclude(x => x.prAddprojectsDtoList)
            .Include(m => m.PrItemsDtoList)
            .ThenInclude(i => i.prAddDeliverySchedulesDtoList)
            .Include(itm => itm.PrItemsDtoList)
             .ThenInclude(pr => pr.prSpecialInstructionsDtoList)
            .FirstOrDefaultAsync();

            return purchaseRequisitionDetail;
        }

        public async Task<IEnumerable<PurchaseRequisition>> SearchPurchaseRequisitionDate([FromQuery] SearchDatesParams searchDatesParams)
        {
            var purchaseRequsisitionDetails = _tipsPurchaseDbContext.PurchaseRequisitions
            .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
            inv.CreatedOn <= searchDatesParams.SearchToDate
            )))
            .Include(itm => itm.PrItemsDtoList)
            .ThenInclude(pr => pr.prAddprojectsDtoList)
             .Include(itm => itm.PrItemsDtoList)
             .ThenInclude(pr => pr.prAddDeliverySchedulesDtoList)
             .Include(itm => itm.PrItemsDtoList)
              .ThenInclude(pr => pr.prSpecialInstructionsDtoList)
            .ToList();

            return purchaseRequsisitionDetails;
        }

        public async Task<PurchaseRequisition> ChangePurchaseRequisitionVersion(PurchaseRequisition purchaseRequisition)
        {
            //purchaseRequisition.CreatedBy = "Admin";
            //purchaseRequisition.CreatedOn = DateTime.Now;
            //purchaseRequisition.Unit = "Bangalore";
            //var getOldRevisionNumber = _tipsPurchaseDbContext.PurchaseRequisitions
            //    .Where(x => x.PrNumber == purchaseRequisition.PrNumber)
            //    .OrderByDescending(x => x.Id)
            //    .Select(x => x.RevisionNumber)
            //    .FirstOrDefault();
          
            //var increaseVersionNumber = 1;
            //var convertversionnumber = (increaseVersionNumber);
            //var version = getOldRevisionNumber + convertversionnumber;
            //purchaseRequisition.RevisionNumber = (version);
            //var result = await Create(purchaseRequisition);
            //return result;

            var getOldPRDetails = _tipsPurchaseDbContext.PurchaseRequisitions
             .Where(x => x.PrNumber == purchaseRequisition.PrNumber && x.IsModified == false)
             .FirstOrDefault();

            if (getOldPRDetails != null)
            {
                getOldPRDetails.IsModified = true;
                getOldPRDetails.LastModifiedBy = _createdBy;
                getOldPRDetails.LastModifiedOn = DateTime.Now;
                Update(getOldPRDetails);
            }
            purchaseRequisition.CreatedBy = _createdBy;
            purchaseRequisition.CreatedOn = DateTime.Now;
            // purchaseRequisition.LastModifiedBy = _createdBy;
            // purchaseRequisition.LastModifiedOn = 
            var getOldRevisionNumber = _tipsPurchaseDbContext.PurchaseRequisitions
            .Where(x => x.PrNumber == purchaseRequisition.PrNumber)
            .OrderByDescending(x => x.Id)
            .Select(x => x.RevisionNumber)
            .FirstOrDefault();

            purchaseRequisition.RevisionNumber = (getOldRevisionNumber + 1);
            var result = await Create(purchaseRequisition);
            return result;


        }

        public  async Task<string> DeletePurchaseRequisition(PurchaseRequisition purchaseRequisitions)
        {
            Delete(purchaseRequisitions);
            string result = $"PurchaseRequistion details of {purchaseRequisitions.Id} is deleted successfully!";
            return result;
        }

        //public async Task<PagedList<PurchaseRequisition>> GetAllActivePurchaseRequisitions([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        //{
        //    var activePurchaseRequsitionDetails = FindAll()
        //       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PrNumber.Contains(searchParams.SearchValue)
        //       || inv.RevisionNumber.Equals(int.Parse(searchParams.SearchValue))
        //       || inv.PrDate.Equals(int.Parse(searchParams.SearchValue)))))
        //                        .Include(o => o.PrFiles)
        //                        .Include(t => t.PrItemsDtoList)
        //                        .ThenInclude(x => x.prAddprojectsDtoList)
        //                        .Include(m => m.PrItemsDtoList)
        //                        .ThenInclude(i => i.prAddDeliverySchedulesDtoList);
        //    return PagedList<PurchaseRequisition>.ToPagedList(activePurchaseRequsitionDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        //}
        public async Task<IEnumerable<PurchaseRequisition>> GetAllActivePurchaseRequisitions()
        {
            var activePurchaseRequsitionDetails = FindAll()
                                .Where(x=>x.Status != Status.Closed)
                                .Include(o => o.PrFiles)
                                .Include(t => t.PrItemsDtoList)
                                .ThenInclude(x => x.prAddprojectsDtoList)
                                .Include(m => m.PrItemsDtoList)
                                .ThenInclude(i => i.prAddDeliverySchedulesDtoList)
                                .Include(itm => itm.PrItemsDtoList)
                                .ThenInclude(pr => pr.prSpecialInstructionsDtoList);

            return activePurchaseRequsitionDetails;
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

        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPurchaseRequisitionNameList()

        {
            IEnumerable<PurchaseRequisitionIdNameListDto> prNumberList = await _tipsPurchaseDbContext.PurchaseRequisitions
                                .Select(x => new PurchaseRequisitionIdNameListDto()
                                {
                                    Id = x.Id,
                                    PrNumber = x.PrNumber,
                                })
                              .ToListAsync();

            return prNumberList;
        }

        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalINameList()
        {
            //IEnumerable<PurchaseRequisitionIdNameListDto> pendingPRApprovalINameList = await _tipsPurchaseDbContext.PurchaseRequisitions
            //                .Where(x => x.PrApprovalI == false).Select(x => new PurchaseRequisitionIdNameListDto()
            //                {
            //                    Id = x.Id,
            //                    PrNumber = x.PrNumber,
            //                }).ToListAsync();
            IEnumerable<PurchaseRequisitionIdNameListDto> pendingPRApprovalINameList = await _tipsPurchaseDbContext.PurchaseRequisitions
           .Where(x => x.PrApprovalI == false && x.IsModified == false)
           .GroupBy(x => x.PrNumber)
           .Select(pr => new PurchaseRequisitionIdNameListDto()
           {
               Id = pr.OrderByDescending(x => x.RevisionNumber).FirstOrDefault().Id,
               PrNumber = pr.Key
           })
           .ToListAsync();

            return pendingPRApprovalINameList;
        }
        public async Task<PagedList<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            IQueryable<PurchaseRequisitionIdNameListDto> pendingPRApprovalIList =  _tipsPurchaseDbContext.PurchaseRequisitions
           .Where(x => x.PrApprovalI == false && x.IsDeleted == false && x.IsModified == false && x.PrStatus != PrStatus.ShortClosed).OrderByDescending(x=>x.Id)
           .Select(pr => new PurchaseRequisitionIdNameListDto()
           {
               Id = pr.Id,
               PrNumber = pr.PrNumber,
               PrDate = pr.PrDate,
               RevisionNumber = pr.RevisionNumber,
               ProcurementType = pr.ProcurementType,
               Purpose = pr.Purpose,
               DeliveryTerms = pr.DeliveryTerms,
               PrApprovalI = pr.PrApprovalI,
               PrApprovedIDate = pr.PrApprovedIDate,
               PrApprovedIBy = pr.PrApprovedIBy,
               PrApprovalII = pr.PrApprovalII,
               PrApprovedIIDate = pr.PrApprovedIIDate,
               PrApprovedIIBy = pr.PrApprovedIIBy,
               PaymentTerms = pr.PaymentTerms,
               ShippingMode = pr.ShippingMode,
               RetentionPeriod = pr.RetentionPeriod,
               SpecialTermsConditions = pr.SpecialTermsConditions,
               Unit = pr.Unit,
               CreatedBy = pr.CreatedBy,
               CreatedOn = pr.CreatedOn,
               LastModifiedBy = pr.LastModifiedBy,
               LastModifiedOn = pr.LastModifiedOn,

           });
            if (searchParams != null && !string.IsNullOrEmpty(searchParams.SearchValue))
            {
                string searchValue = searchParams.SearchValue.ToLower();

                pendingPRApprovalIList = pendingPRApprovalIList
                    .Where(item =>
                        item.PrNumber.ToLower().Contains(searchValue) ||
                        item.Purpose.ToLower().Contains(searchValue) ||
                        item.DeliveryTerms.ToLower().Contains(searchValue) ||
                        item.ShippingMode.ToLower().Contains(searchValue) ||
                        item.PrApprovedIBy.ToLower().Contains(searchValue) ||
                        item.PrApprovedIIBy.ToLower().Contains(searchValue) ||
                        item.ProcurementType.ToLower().Contains(searchValue)
                    ).OrderByDescending(x=>x.Id);
            }

            int totalCount = await pendingPRApprovalIList.CountAsync();

            var result = await pendingPRApprovalIList
                .Skip((pagingParameter.PageNumber - 1) * pagingParameter.PageSize)
                .Take(pagingParameter.PageSize)
                .ToListAsync();

            return PagedList<PurchaseRequisitionIdNameListDto>.ToPagedList(pendingPRApprovalIList, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<PurchaseRequisitionIdNameListDto>> GetAllLastestPendingPRApprovalIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            IQueryable<PurchaseRequisitionIdNameListDto> lastestPendingPRApprovalIList = _tipsPurchaseDbContext.PurchaseRequisitions
               .Where(x => x.PrApprovalI == false && x.IsDeleted == false && x.IsModified == false && x.PrStatus != PrStatus.ShortClosed)               
               .OrderByDescending(x => x.Id)
               .Select(pr => new PurchaseRequisitionIdNameListDto()
               {
                   Id = pr.Id,
                   PrNumber = pr.PrNumber,
                   PrDate = pr.PrDate,
                   RevisionNumber = pr.RevisionNumber,
                   ProcurementType = pr.ProcurementType,
                   Purpose = pr.Purpose,
                   DeliveryTerms = pr.DeliveryTerms,
                   PrApprovalI = pr.PrApprovalI,
                   PrApprovedIDate = pr.PrApprovedIDate,
                   PrApprovedIBy = pr.PrApprovedIBy,
                   PrApprovalII = pr.PrApprovalII,
                   PrApprovedIIDate = pr.PrApprovedIIDate,
                   PrApprovedIIBy = pr.PrApprovedIIBy,
                   PaymentTerms = pr.PaymentTerms,
                   ShippingMode = pr.ShippingMode,
                   RetentionPeriod = pr.RetentionPeriod,
                   SpecialTermsConditions = pr.SpecialTermsConditions,
                   Unit = pr.Unit,
                   CreatedBy = pr.CreatedBy,
                   CreatedOn = pr.CreatedOn,
                   LastModifiedBy = pr.LastModifiedBy,
                   LastModifiedOn = pr.LastModifiedOn,
               })
               .GroupBy(x => x.PrNumber)
               .Select(group => group.OrderByDescending(x => x.RevisionNumber).First());

            if (searchParams != null && !string.IsNullOrEmpty(searchParams.SearchValue))
            {
                string searchValue = searchParams.SearchValue.ToLower();

                lastestPendingPRApprovalIList = lastestPendingPRApprovalIList
                    .Where(item =>
                        item.PrNumber.ToLower().Contains(searchValue) ||
                        item.Purpose.ToLower().Contains(searchValue) ||
                        item.DeliveryTerms.ToLower().Contains(searchValue) ||
                        item.ShippingMode.ToLower().Contains(searchValue) ||
                        item.PrApprovedIBy.ToLower().Contains(searchValue) ||
                        item.PrApprovedIIBy.ToLower().Contains(searchValue) ||
                        item.ProcurementType.ToLower().Contains(searchValue)
                    ).OrderByDescending(x => x.Id);
            }

            int totalCount = await lastestPendingPRApprovalIList.CountAsync();

            var result = await lastestPendingPRApprovalIList
                .Skip((pagingParameter.PageNumber - 1) * pagingParameter.PageSize)
                .Take(pagingParameter.PageSize)
                .ToListAsync();

            return new PagedList<PurchaseRequisitionIdNameListDto>(result, totalCount, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<PagedList<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalIIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            IQueryable<PurchaseRequisitionIdNameListDto> pendingPRApprovalIIList =  _tipsPurchaseDbContext.PurchaseRequisitions
           .Where(x => x.PrApprovalI == true && x.PrApprovalII == false && x.IsDeleted == false && x.IsModified == false && x.PrStatus != PrStatus.ShortClosed).OrderByDescending(x=>x.Id)
           .Select(pr => new PurchaseRequisitionIdNameListDto()
           {
               Id = pr.Id,
               PrNumber = pr.PrNumber,
               PrDate = pr.PrDate,
               RevisionNumber = pr.RevisionNumber,
               ProcurementType = pr.ProcurementType,
               Purpose = pr.Purpose,
               DeliveryTerms = pr.DeliveryTerms,
               PrApprovalI = pr.PrApprovalI,
               PrApprovedIDate = pr.PrApprovedIDate,
               PrApprovedIBy = pr.PrApprovedIBy,
               PrApprovalII = pr.PrApprovalII,
               PrApprovedIIDate = pr.PrApprovedIIDate,
               PrApprovedIIBy = pr.PrApprovedIIBy,
               PaymentTerms = pr.PaymentTerms,
               ShippingMode = pr.ShippingMode,
               RetentionPeriod = pr.RetentionPeriod,
               SpecialTermsConditions = pr.SpecialTermsConditions,
               Unit = pr.Unit,
               CreatedBy = pr.CreatedBy,
               CreatedOn = pr.CreatedOn,
               LastModifiedBy = pr.LastModifiedBy,
               LastModifiedOn = pr.LastModifiedOn,

           });
            if (searchParams != null && !string.IsNullOrEmpty(searchParams.SearchValue))
            {
                string searchValue = searchParams.SearchValue.ToLower();

                pendingPRApprovalIIList = pendingPRApprovalIIList
                    .Where(item =>
                        item.PrNumber.ToLower().Contains(searchValue) ||
                        item.Purpose.ToLower().Contains(searchValue) ||
                        item.DeliveryTerms.ToLower().Contains(searchValue) ||
                        item.ShippingMode.ToLower().Contains(searchValue) ||
                        item.PrApprovedIBy.ToLower().Contains(searchValue) ||
                        item.PrApprovedIIBy.ToLower().Contains(searchValue) ||
                        item.ProcurementType.ToLower().Contains(searchValue)
                    ).OrderByDescending(x=>x.Id);
            }

            int totalCount = await pendingPRApprovalIIList.CountAsync();

            var result = await pendingPRApprovalIIList
                .Skip((pagingParameter.PageNumber - 1) * pagingParameter.PageSize)
                .Take(pagingParameter.PageSize)
                .ToListAsync();

            return PagedList<PurchaseRequisitionIdNameListDto>.ToPagedList(pendingPRApprovalIIList, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<PurchaseRequisitionIdNameListDto>> GetAllLastestPendingPRApprovalIIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            IQueryable<PurchaseRequisitionIdNameListDto> pendingPRApprovalIIList = _tipsPurchaseDbContext.PurchaseRequisitions
               .Where(x => x.PrApprovalI == true && x.PrApprovalII == false && x.IsDeleted == false && x.IsModified == false && x.PrStatus != PrStatus.ShortClosed)                
               .OrderByDescending(x => x.Id)
               .Select(pr => new PurchaseRequisitionIdNameListDto()
               {
                   Id = pr.Id,
                   PrNumber = pr.PrNumber,
                   PrDate = pr.PrDate,
                   RevisionNumber = pr.RevisionNumber,
                   ProcurementType = pr.ProcurementType,
                   Purpose = pr.Purpose,
                   DeliveryTerms = pr.DeliveryTerms,
                   PrApprovalI = pr.PrApprovalI,
                   PrApprovedIDate = pr.PrApprovedIDate,
                   PrApprovedIBy = pr.PrApprovedIBy,
                   PrApprovalII = pr.PrApprovalII,
                   PrApprovedIIDate = pr.PrApprovedIIDate,
                   PrApprovedIIBy = pr.PrApprovedIIBy,
                   PaymentTerms = pr.PaymentTerms,
                   ShippingMode = pr.ShippingMode,
                   RetentionPeriod = pr.RetentionPeriod,
                   SpecialTermsConditions = pr.SpecialTermsConditions,
                   Unit = pr.Unit,
                   CreatedBy = pr.CreatedBy,
                   CreatedOn = pr.CreatedOn,
                   LastModifiedBy = pr.LastModifiedBy,
                   LastModifiedOn = pr.LastModifiedOn,
               })
               .GroupBy(x => x.PrNumber)
               .Select(group => group.OrderByDescending(x => x.RevisionNumber).First());

            if (searchParams != null && !string.IsNullOrEmpty(searchParams.SearchValue))
            {
                string searchValue = searchParams.SearchValue.ToLower();

                pendingPRApprovalIIList = pendingPRApprovalIIList
                    .Where(item =>
                        item.PrNumber.ToLower().Contains(searchValue) ||
                        item.Purpose.ToLower().Contains(searchValue) ||
                        item.DeliveryTerms.ToLower().Contains(searchValue) ||
                        item.ShippingMode.ToLower().Contains(searchValue) ||
                        item.PrApprovedIBy.ToLower().Contains(searchValue) ||
                        item.PrApprovedIIBy.ToLower().Contains(searchValue) ||
                        item.ProcurementType.ToLower().Contains(searchValue)
                    ).OrderByDescending(x => x.Id);
            }

            int totalCount = await pendingPRApprovalIIList.CountAsync();

            var result = await pendingPRApprovalIIList
                .Skip((pagingParameter.PageNumber - 1) * pagingParameter.PageSize)
                .Take(pagingParameter.PageSize)
                .ToListAsync();

            return new PagedList<PurchaseRequisitionIdNameListDto>(result, totalCount, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<IEnumerable<PurchaseRequisitionIdNameListDto>> GetAllPendingPRApprovalIINameList()
        {
            //IEnumerable<PurchaseRequisitionIdNameListDto> pendingPRApprovalIINameList = await _tipsPurchaseDbContext.PurchaseRequisitions
            //                .Where(x => x.PrApprovalII == false).Select(x => new PurchaseRequisitionIdNameListDto()
            //                {
            //                    Id = x.Id,
            //                    PrNumber = x.PrNumber,
            //                }).ToListAsync();
            IEnumerable<PurchaseRequisitionIdNameListDto> pendingPRApprovalIINameList = await _tipsPurchaseDbContext.PurchaseRequisitions
            .Where(x => x.PrApprovalI == true  && x.PrApprovalII == false && x.IsDeleted == false && x.IsModified == false)
            .GroupBy(x => x.PrNumber)
            .Select(pr => new PurchaseRequisitionIdNameListDto()
            {
                Id = pr.OrderByDescending(x => x.RevisionNumber).FirstOrDefault().Id,
                PrNumber = pr.Key
            })
            .ToListAsync();

            return pendingPRApprovalIINameList;
        }

        public async Task<PagedList<PurchaseRequisition>> GetAllPurchaseRequisitions([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            int? searchrev;
            DateTime? searchDate;
            try
            {
                searchrev = int.Parse(searchParams.SearchValue);
            }
            catch
            {
                searchrev = null;
            }
            try
            {
                searchDate = DateTime.Parse(searchParams.SearchValue);
            }
            catch
            {
                searchDate = null;
            }
            var purchaseRequistionDetails = FindAll().OrderByDescending(on => on.Id)
            .Where(inv => (string.IsNullOrWhiteSpace(searchParams.SearchValue)
              || inv.PrNumber.Contains(searchParams.SearchValue)
              || inv.RevisionNumber.Equals(searchrev)
              || inv.PrDate.Equals(searchDate)))
            .Include(f => f.PrFiles)
            .Include(t => t.PrItemsDtoList)
                                  .ThenInclude(x => x.prAddprojectsDtoList)
                                  .Include(m => m.PrItemsDtoList)
                                  .ThenInclude(i => i.prAddDeliverySchedulesDtoList)
                                  .Include(itm => itm.PrItemsDtoList)
                                  .ThenInclude(pr => pr.prSpecialInstructionsDtoList);

            return PagedList<PurchaseRequisition>.ToPagedList(purchaseRequistionDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
            //var purchaseRequistionDetails = FindAll().OrderByDescending(on => on.Id)

            //    .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.PrNumber.Contains(searchParams.SearchValue)
            //   || inv.RevisionNumber.Equals(int.Parse(searchParams.SearchValue))
            //   || inv.PrDate.Equals(int.Parse(searchParams.SearchValue))))).Include(f => f.PrFiles)
            //                      .Include(t => t.PrItemsDtoList)
            //                      .ThenInclude(x => x.prAddprojectsDtoList)
            //                      .Include(m => m.PrItemsDtoList)
            //                      .ThenInclude(i => i.prAddDeliverySchedulesDtoList)
            //                      .Include(itm => itm.PrItemsDtoList)
            //                      .ThenInclude(pr => pr.prSpecialInstructionsDtoList);

            //return PagedList<PurchaseRequisition>.ToPagedList(purchaseRequistionDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<PurchaseRequisition>> GetAllLastestPurchaseRequisitions([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            int? searchrev;
            DateTime? searchDate;
            try
            {
                searchrev = int.Parse(searchParams.SearchValue);
            }
            catch
            {
                searchrev = null;
            }
            try
            {
                searchDate = DateTime.Parse(searchParams.SearchValue);
            }
            catch
            {
                searchDate = null;
            }

            var purchaseRequisitionDetails = FindAll()
                .OrderByDescending(on => on.Id)
                .Where(inv =>
                    (string.IsNullOrWhiteSpace(searchParams.SearchValue)
                    || inv.PrNumber.Contains(searchParams.SearchValue)
                    || (searchrev.HasValue && inv.RevisionNumber == searchrev)
                    || (searchDate.HasValue && inv.PrDate == searchDate))
                )
                
                .Include(f => f.PrFiles)
                .Include(t => t.PrItemsDtoList)
                    .ThenInclude(x => x.prAddprojectsDtoList)
                .Include(m => m.PrItemsDtoList)
                    .ThenInclude(i => i.prAddDeliverySchedulesDtoList)
                .Include(itm => itm.PrItemsDtoList)
                    .ThenInclude(pr => pr.prSpecialInstructionsDtoList)
                    .GroupBy(inv => inv.PrNumber)
                .Select(group => group.OrderByDescending(inv => inv.RevisionNumber).First());

            return PagedList<PurchaseRequisition>.ToPagedList(purchaseRequisitionDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PurchaseRequisition> GetPurchaseRequisitionById(int id)
        {
            var purchaseRequistionDetailById = await _tipsPurchaseDbContext.PurchaseRequisitions.Where(x => x.Id == id)
                                 .Include(o => o.PrFiles)
                                 //.Include(itm => itm.PrItemsDtoList)
                                 //.ThenInclude(x => x.Upload)
                                 .Include(t => t.PrItemsDtoList)
                                 .ThenInclude(x => x.prAddprojectsDtoList)
                                 .Include(m => m.PrItemsDtoList)
                                 .ThenInclude(i => i.prAddDeliverySchedulesDtoList)
                                 .Include(itm => itm.PrItemsDtoList)
                                 .ThenInclude(pr => pr.prSpecialInstructionsDtoList)

                                 .FirstOrDefaultAsync();

            return purchaseRequistionDetailById;
        }

        public async Task<PurchaseRequisition> GetPurchaseRequisitionByPRNumber(string prNumber, int RevNo)
        {
            var purchaseRequistionByPRNumber = await _tipsPurchaseDbContext.PurchaseRequisitions.
                Where(x => x.PrNumber == prNumber && x.RevisionNumber == RevNo)
                                  .Include(o => o.PrFiles)
                                  .Include(t => t.PrItemsDtoList)
                                .ThenInclude(x => x.prAddprojectsDtoList)
                                .Include(m => m.PrItemsDtoList)
                                .ThenInclude(i => i.prAddDeliverySchedulesDtoList)
                                .Include(itm => itm.PrItemsDtoList)
                                .ThenInclude(pr => pr.prSpecialInstructionsDtoList)
                                .FirstOrDefaultAsync();

            return purchaseRequistionByPRNumber;
        }

        public async Task<string> UpdatePurchaseRequisition(PurchaseRequisition purchaseRequisitions)
        {
            purchaseRequisitions.LastModifiedBy = _createdBy;
            purchaseRequisitions.LastModifiedOn = DateTime.Now;
            Update(purchaseRequisitions);
            string result = $"PurchaseRequisitions of Detail {purchaseRequisitions.Id} is updated successfully!";
            return result;
        }
        public async Task<string> UpdatePurchaseRequisition_ForApproval(PurchaseRequisition purchaseRequisitions)
        {
           // purchaseRequisitions.LastModifiedBy = _createdBy;
           // purchaseRequisitions.LastModifiedOn = DateTime.Now;
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
    public class PRItemsUploadDocumentRepository : RepositoryBase<PRItemsDocumentUpload>, IPRItemsDocumentUploadRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PRItemsUploadDocumentRepository(TipsPurchaseDbContext tipsPurchaseDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsPurchaseDbContext)
        {
            _tipsPurchaseDbContext = tipsPurchaseDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateUploadDocumentPO(PRItemsDocumentUpload documentUpload)
        {
            documentUpload.CreatedBy = _createdBy;
            documentUpload.CreatedOn = DateTime.Now;

            var result = await Create(documentUpload);
            return result.Id;
        }
        public async Task<PRItemsDocumentUpload> GetUploadDocById(int id)
        {
            var uploadDocFileNameById = await _tipsPurchaseDbContext.PRItemsDocumentUploads
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            return uploadDocFileNameById;
        }
        public async Task<PRItemsDocumentUpload> GetUploadDocByFileName(string fileName)
        {
            var uploadDocFileNameById = await _tipsPurchaseDbContext.PRItemsDocumentUploads
                .Where(x => x.FileName == fileName)
                .FirstOrDefaultAsync();

            return uploadDocFileNameById;
        }
        public async Task<string> UpdateUploadDoc(PRItemsDocumentUpload prItemsDocumentUpload)
        {
            Update(prItemsDocumentUpload);
            string result = $"PRItemsDocumentUpload of Detail {prItemsDocumentUpload.Id} is updated successfully!";
            return result;
        }
        public async Task<string> DeleteUploadFile(PRItemsDocumentUpload documentUpload)
        {
            Delete(documentUpload);
            string result = $"DocumentUpload details of {documentUpload.Id} is deleted successfully!";
            return result;
        }
    }
    public class PurchaseRequisitionItemRepository : RepositoryBase<PrItem>, IPrItemsRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContexts;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PurchaseRequisitionItemRepository(TipsPurchaseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsPurchaseDbContexts = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public Task<IEnumerable<PrItem>> GetAllPrItems()
        {
            throw new NotImplementedException();
        }

        public async Task<PrItem> GetPrItemByPRNo(string prNo,string pritem)
        {
            var prId = await _tipsPurchaseDbContexts.PurchaseRequisitions
                .Where(x => x.PrNumber == prNo)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (prId != 0) 
            {
                var prItems = await _tipsPurchaseDbContexts.PrItems
                    .Where(x => x.PurchaseRequistionId == prId && x.ItemNumber == pritem)
                    .FirstOrDefaultAsync();

                return prItems;
            }
            else
            {
                return null;
            }
        }

        public async Task<PrStatus> GetPrItemClosedStatusCount(string prNo)
        {
            var prId = await _tipsPurchaseDbContexts.PurchaseRequisitions
               .Where(x => x.PrNumber == prNo)
               .Select(x => x.Id)
               .FirstOrDefaultAsync();
            
                PrStatus status=PrStatus.Open;
                var prCount=_tipsPurchaseDbContexts.PrItems.Where(x => x.PurchaseRequistionId == prId).Count();
                var prStatusCount =_tipsPurchaseDbContexts.PrItems.Where(x => x.PurchaseRequistionId == prId && x.PrStatus != PrStatus.Closed).Count();
                if (prStatusCount == 0) status = PrStatus.Closed;
                else if (prCount > prStatusCount) status = PrStatus.PartiallyClosed;
                return status;
                        
        }
        public Task<IEnumerable<PrItem>> GetAllActivePrItems()
        {
            throw new NotImplementedException();
        }

        public Task<int?> CreatePrItem(PrItem prItem)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UpdatePrItem(PrItem prItem)
        {
            Update(prItem);
            string result = $"PrItem of Detail {prItem.Id} is updated successfully!";
            return result;
        }

        public Task<string> DeletePrItem(PrItem prItem)
        {
            throw new NotImplementedException();
        }
        public async Task<PrItem> ClosePrItemSatusByPrItemId(int prItemId)
        {
            var prItemDetailByPrItemId = await _tipsPurchaseDbContext.PrItems.Where(x => x.Id == prItemId)

                                .FirstOrDefaultAsync();

            return prItemDetailByPrItemId;
        }
        public async Task<int?> GetPrItemOpenStatusCount(int prId)
        {
            var prItemStatusCount = _tipsPurchaseDbContext.PrItems
                                        .Where(x => x.PurchaseRequistionId == prId && x.PrStatus == PrStatus.Open).Count();

            return prItemStatusCount;
        }

        public Task<PrItem> GetPrItemById(int id)
        {
            throw new NotImplementedException();
        }
    }

}
