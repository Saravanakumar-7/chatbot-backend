using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Misc;
using System.Security.Claims;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Repository
{
    public class POCollectionTrackerForAviRepository : RepositoryBase<POCollectionTrackerForAvi>, IPOCollectionTrackerForAviRepository
    {
        private TipsPurchaseDbContext _tipsPurchaseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public POCollectionTrackerForAviRepository(TipsPurchaseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsPurchaseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreatePOCollectionTrackerForAvi(POCollectionTrackerForAvi pocollectionTracker)
        {
            pocollectionTracker.CreatedBy = _createdBy;
            pocollectionTracker.CreatedOn = DateTime.Now;
            pocollectionTracker.Unit = _unitname;
            var result = await Create(pocollectionTracker);
            return result.Id;
        }

        public async Task<IEnumerable<POCollectionTrackerForAvi>> GetAllPOCollectionTrackerForAviWithItems(POCollectionTrackerForAviSearchDto poCollectionTrackerSearch)
        {
            using (var context = _tipsPurchaseDbContext)
            {
                var query = _tipsPurchaseDbContext.POCollectionTrackersForAvi.AsQueryable();
                if (poCollectionTrackerSearch != null || (poCollectionTrackerSearch.VendorId.Any())
                 && poCollectionTrackerSearch.VendorName.Any() && poCollectionTrackerSearch.Remarks.Any())

                {
                    query = query.Where
                    (inv => (poCollectionTrackerSearch.VendorId.Any() ? poCollectionTrackerSearch.VendorId.Contains(inv.VendorId) : true)
                   && (poCollectionTrackerSearch.VendorName.Any() ? poCollectionTrackerSearch.VendorName.Contains(inv.VendorName) : true)
                   && (poCollectionTrackerSearch.Remarks.Any() ? poCollectionTrackerSearch.Remarks.Contains(inv.Remarks) : true))
                    .Include(x => x.POBreakDownForAvi);
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<POCollectionTrackerForAvi>> SearchPOCollectionTrackerForAviDate([FromQuery] SearchDatesParams searchDatesParams)
        {
            var poCollectionTrackerDetails = _tipsPurchaseDbContext.POCollectionTrackersForAvi
            .Where(inv => ((inv.CreatedOn >= searchDatesParams.SearchFromDate &&
            inv.CreatedOn <= searchDatesParams.SearchToDate
            )))
            .Include(x => x.POBreakDownForAvi)
            .ToList();
            return poCollectionTrackerDetails;
        }

        public async Task<IEnumerable<POCollectionTrackerForAvi>> SearchPOCollectionTrackerForAvi([FromQuery] SearchParamess searchParamess)
        {
            using (var context = _tipsPurchaseDbContext)
            {
                var query = _tipsPurchaseDbContext.POCollectionTrackersForAvi.AsQueryable();
                if (!string.IsNullOrEmpty(searchParamess.SearchValue))
                {
                    query = query.Where(inv => inv.VendorId.Contains(searchParamess.SearchValue)
                    || inv.VendorName.Contains(searchParamess.SearchValue)
                    || inv.Remarks.Contains(searchParamess.SearchValue))
                        .Include(x => x.POBreakDownForAvi);
                }
                return query.ToList();
            }
        }

        public async Task<string> DeletePOCollectionTrackerForAvi(POCollectionTrackerForAvi pocollectionTracker)
        {
            Delete(pocollectionTracker);
            string result = $"POCollectionTracker details of {pocollectionTracker.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<POCollectionTrackerForAvi>> GetAllPOCollectionTrackersForAvi(PagingParameter pagingParameter, SearchParamess searchParamess)
        {
            var pocollectionTrackerDetails = FindAll().OrderByDescending(x => x.Id)
                 .Where(col => ((string.IsNullOrWhiteSpace(searchParamess.SearchValue) || col.VendorName.Contains(searchParamess.SearchValue) ||
                  col.VendorId.Contains(searchParamess.SearchValue))));

            return PagedList<POCollectionTrackerForAvi>.ToPagedList(pocollectionTrackerDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<List<OpenPurchaseOrderForAviDetailsDto>> GetOpenPOForAviDetailsByVendorId(string vendorId)
        {

            var purchaseOrderTotalValue = _tipsPurchaseDbContext.PurchaseOrders.Where(x => x.VendorNumber == vendorId).Sum(s => s.TotalAmount);

            var poBreakDownDetails = _tipsPurchaseDbContext.POBreakDownsForAvi.Where(x => x.VendorId == vendorId).Select(x => x.AmountAgainstPO).Count();
            var maxRevisions = from e in _tipsPurchaseDbContext.PurchaseOrders
                               group e by e.PONumber into g
                               select new
                               {
                                   PONumber = g.Key,
                                   MaxRevisionNumber = g.Max(x => x.RevisionNumber)
                               };
            if (poBreakDownDetails != 0)
            {
                var PODetails = from e in _tipsPurchaseDbContext.PurchaseOrders
                                join maxRev in maxRevisions on new { e.PONumber, e.RevisionNumber } equals new { PONumber = maxRev.PONumber, RevisionNumber = maxRev.MaxRevisionNumber }
                                where e.VendorNumber == vendorId
                                join d in _tipsPurchaseDbContext.POBreakDownsForAvi on e.PONumber equals d.PONumber into dept
                                from POBreakDownForAvi in dept.DefaultIfEmpty()
                                group new { e, POBreakDownForAvi } by new { e.Id, e.PONumber, e.TotalAmount } into g
                                select new OpenPurchaseOrderForAviDetailsDto
                                {
                                    PurchaseOrderId = g.Key.Id,
                                    PONumber = g.Key.PONumber,
                                    TotalValue = g.Key.TotalAmount,
                                    PendingValue = g.Key.TotalAmount - g.Sum(x => x.POBreakDownForAvi.AmountAgainstPO),
                                    AmountRecieved = g.Sum(x => x.POBreakDownForAvi.AmountAgainstPO)
                                };

                var poData = PODetails.ToList();

                return poData;


            }
            else
            {
                var PODetails = from e in _tipsPurchaseDbContext.PurchaseOrders
                                join maxRev in maxRevisions on new { e.PONumber, e.RevisionNumber } equals new { PONumber = maxRev.PONumber, RevisionNumber = maxRev.MaxRevisionNumber }
                                where e.VendorNumber == vendorId
                                join d in _tipsPurchaseDbContext.POBreakDownsForAvi on e.VendorId equals d.VendorId into dept
                                from POBreakDownsForAvi in dept.DefaultIfEmpty()
                                select new OpenPurchaseOrderForAviDetailsDto
                                {
                                    PurchaseOrderId = e.Id,
                                    PONumber = e.PONumber,
                                    TotalValue = e.TotalAmount,
                                    PendingValue = e.TotalAmount,
                                    AmountRecieved = 0
                                };

                var poData = PODetails.ToList();

                return poData;
            }
        }

        public async Task<POCollectionTrackerForAvi> GetPOCollectionTrackerForAviById(int id)
        {
            var pocollectionTrackerDetailsById = await FindByCondition(x => x.Id == id)
               .Include(c => c.POBreakDownForAvi)
               .FirstOrDefaultAsync();
            return pocollectionTrackerDetailsById;
        }
        public async Task<IEnumerable<PayableSPReport>> GetPayableSPReportWithParam(string PONumber, string VendorName, string ProjectNumber)
        {

            var result = _tipsPurchaseDbContext
            .Set<PayableSPReport>()
            .FromSqlInterpolated($"CALL Payable_Report_with_Parameter({PONumber},{VendorName},{ProjectNumber})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<PayableSPReport>> GetPayableSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsPurchaseDbContext.Set<PayableSPReport>()
                        .FromSqlInterpolated($"CALL Payable_Report_with_Date({FromDate},{ToDate})")
                        .ToList();

            return results;
        }
        public async Task<POCollectionTrackerForAviDetailsDto> GetPOCollectionTrackerForAviByVendorId(string vendorId)
        {
            //var purchaseOrderTotalValue = _tipsPurchaseDbContext.PurchaseOrders
            //         .Where(x => x.VendorNumber == vendorId)
            //             .Sum(s => s.TotalAmount);

            var purchaseOrderTotalValue = _tipsPurchaseDbContext.PurchaseOrders
                        .Where(x => x.VendorNumber == vendorId)
                        .OrderByDescending(x => x.CreatedOn)
                        .Sum(x => x.TotalAmount);

            var pocollectionDetails = _tipsPurchaseDbContext.POCollectionTrackersForAvi
                .Where(x => x.VendorId == vendorId)
                .Select(x => x.AlreadyRecieved)
                .Count();

            if (pocollectionDetails != 0)
            {
                var amountRecieved = _tipsPurchaseDbContext.POCollectionTrackersForAvi
                    .Where(x => x.VendorId == vendorId)
                    .Sum(x => x.AmountRecieved);


                var pocollectiveTrackerDetails = new POCollectionTrackerForAviDetailsDto()
                {
                    TotalSumOfPOAmount = purchaseOrderTotalValue,
                    AlreadyRecieved = Convert.ToDecimal(amountRecieved)
                };

                return pocollectiveTrackerDetails;
            }
            else
            {
                var alreadyRecieved = 0;

                var pocollectiveTrackerDetails = new POCollectionTrackerForAviDetailsDto()
                {
                    TotalSumOfPOAmount = purchaseOrderTotalValue,
                    AlreadyRecieved = alreadyRecieved
                };

                return pocollectiveTrackerDetails;
            }
        }

        public async Task<string> UpdatePOCollectionTrackerForAvi(POCollectionTrackerForAvi pocollectionTracker)
        {
            pocollectionTracker.LastModifiedBy = _createdBy;
            pocollectionTracker.LastModifiedOn = DateTime.Now;
            Update(pocollectionTracker);
            string result = $"POCollectionTracker details of {pocollectionTracker.Id} is updated successfully!";
            return result;
        }

    }
}