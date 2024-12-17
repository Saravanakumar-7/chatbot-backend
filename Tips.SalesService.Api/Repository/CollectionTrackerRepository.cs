using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Repository
{
    public class CollectionTrackerRepository : RepositoryBase<CollectionTracker>, ICollectionTrackerRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public CollectionTrackerRepository(TipsSalesServiceDbContext tipsSalesServiceDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsSalesServiceDbContext)
        {
            _tipsSalesServiceDbContext = tipsSalesServiceDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateCollectionTracker(CollectionTracker collectionTracker)
        {
            collectionTracker.CreatedBy = _createdBy;
            collectionTracker.CreatedOn = DateTime.Now;
            collectionTracker.Unit = _unitname;
            var result = await Create(collectionTracker);
            return result.Id;
        }

        public async Task<IEnumerable<CollectionTracker>> GetAllCollectionTrackerWithItems(CollectionTrackerSearchDto collectionTrackerSearch)
        {
            using (var context = _tipsSalesServiceDbContext)
            {
                var query = _tipsSalesServiceDbContext.CollectionTrackers.AsQueryable();
                if (collectionTrackerSearch != null || (collectionTrackerSearch.CustomerId.Any())
                 && collectionTrackerSearch.CustomerName.Any() && collectionTrackerSearch.Remarks.Any())

                {
                    query = query.Where
                    (inv => (collectionTrackerSearch.CustomerId.Any() ? collectionTrackerSearch.CustomerId.Contains(inv.CustomerId) : true)
                   && (collectionTrackerSearch.CustomerName.Any() ? collectionTrackerSearch.CustomerName.Contains(inv.CustomerName) : true)
                   && (collectionTrackerSearch.Remarks.Any() ? collectionTrackerSearch.Remarks.Contains(inv.Remarks) : true))
                    .Include(x=>x.SOBreakDown);
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<CollectionTracker>> SearchCollectionTrackerDate([FromQuery] SearchDateParam searchDateParam)
        {
            var collectionTrackerDetails = _tipsSalesServiceDbContext.CollectionTrackers
            .Where(inv => ((inv.CreatedOn >= searchDateParam.SearchFromDate &&
            inv.CreatedOn <= searchDateParam.SearchToDate
            )))
            .Include(x => x.SOBreakDown)
            .ToList();
            return collectionTrackerDetails;
        }

        public async Task<IEnumerable<CollectionTracker>> SearchCollectionTracker([FromQuery] SearchParammes searchParammes)
        {
            using (var context = _tipsSalesServiceDbContext)
            {
                var query = _tipsSalesServiceDbContext.CollectionTrackers.AsQueryable();
                if (!string.IsNullOrEmpty(searchParammes.SearchValue))
                {
                    query = query.Where(inv => inv.CustomerId.Contains(searchParammes.SearchValue)
                    || inv.CustomerName.Contains(searchParammes.SearchValue)
                    || inv.Remarks.Contains(searchParammes.SearchValue))
                        .Include(x => x.SOBreakDown);
                }
                return query.ToList();
            }
        }

        public async Task<string> DeleteCollectionTracker(CollectionTracker collectionTracker)
        {
            Delete(collectionTracker);
            string result = $"CollectionTracker details of {collectionTracker.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<CollectionTracker>> GetAllCollectionTrackers(PagingParameter pagingParameter, SearchParammes searchParammes)
        {
            var collectionTrackerDetails = FindAll().OrderByDescending(x => x.Id)
                .Where(col => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || col.CustomerName.Contains(searchParammes.SearchValue) ||
                 col.CustomerId.Contains(searchParammes.SearchValue))));

            return PagedList<CollectionTracker>.ToPagedList(collectionTrackerDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<CollectionTracker> GetCollectionTrackerById(int id)
        {
            var collectionTrackerDetailsById = await FindByCondition(x => x.Id == id)
                .Include(c=>c.SOBreakDown)
                .FirstOrDefaultAsync();
            return collectionTrackerDetailsById;
        }
        public async Task<IEnumerable<CollectionTrackerSPReport>> GetCollectionTrackerSPReportWithParam(string CustomerId)
        {
            var result = _tipsSalesServiceDbContext
            .Set<CollectionTrackerSPReport>()
            .FromSqlInterpolated($"CALL collectiontrackers_report_withparameters({CustomerId})")
            .ToList();

            return result;

        }

        public async Task<IEnumerable<CollectionTrackerSPReport>> GetCollectionTrackerSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<CollectionTrackerSPReport>()
                        .FromSqlInterpolated($"CALL collectiontrackers_report_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<CollectionTrackerByCustomerIdSPReport>> GetCollectionTrackerByCustomerIdSPReportWithParam(string CustomerId)
        {
            var result = _tipsSalesServiceDbContext
            .Set<CollectionTrackerByCustomerIdSPReport>()
            .FromSqlInterpolated($"CALL customerdataby_customerid({CustomerId})")
            .ToList();

            return result;

        }

        public async Task<IEnumerable<CollectionTrackerByCustomerIdSPReport>> GetCollectionTrackerByCustomerIdSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<CollectionTrackerByCustomerIdSPReport>()
                        .FromSqlInterpolated($"CALL cutomerdataby_cutomerid_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<CollectionTrackerBySalesOrderNoSPReport>> GetCollectionTrackerBySalesOrderNoSPReportWithParam(string CustomerId)
        {
            var result = _tipsSalesServiceDbContext
            .Set<CollectionTrackerBySalesOrderNoSPReport>()
            .FromSqlInterpolated($"CALL cutomerdataby_salesorderno_withparameter({CustomerId})")
            .ToList();

            return result;

        }

        public async Task<IEnumerable<CollectionTrackerBySalesOrderNoSPReport>> GetCollectionTrackerBySalesOrderNoSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<CollectionTrackerBySalesOrderNoSPReport>()
                        .FromSqlInterpolated($"CALL cutomerdataby_salesorderno_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<CollectionTrackerWithCustomerWiseSPReport>> GetCollectionTrackerWithCustomerWiseSPReportWithParam(string CustomerId)
        {
            var result = _tipsSalesServiceDbContext
            .Set<CollectionTrackerWithCustomerWiseSPReport>()
            .FromSqlInterpolated($"CALL collectiontrackers_withcustomerwise_withparameters({CustomerId})")
            .ToList();

            return result;

        }

        public async Task<IEnumerable<CollectionTrackerWithCustomerWiseSPReport>> GetCollectionTrackerWithCustomerWiseSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<CollectionTrackerWithCustomerWiseSPReport>()
                        .FromSqlInterpolated($"CALL collectiontrackers_withcustomerwise_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<CollectionTrackerWithSalesOrderNoWiseSPReport>> GetCollectionTrackerWithSalesOrderNoWiseSPReportWithParam(string salesOrderNumber)
        {
            var result = _tipsSalesServiceDbContext
            .Set<CollectionTrackerWithSalesOrderNoWiseSPReport>()
            .FromSqlInterpolated($"CALL collectiontrackers_withsalesorderno_withparameters({salesOrderNumber})")
            .ToList();

            return result;

        }

        public async Task<IEnumerable<CollectionTrackerWithSalesOrderNoWiseSPReport>> GetCollectionTrackerWithSalesOrderNoWiseSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsSalesServiceDbContext.Set<CollectionTrackerWithSalesOrderNoWiseSPReport>()
                        .FromSqlInterpolated($"CALL collectiontrackers_withsalesorderno_withdate({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<List<OpenSalesOrderDetailsDto>> GetOpenSODetailsByCustomerId(string customerId)
        {
            var salesOrderTotalValue = _tipsSalesServiceDbContext.SalesOrders.Where(x => x.CustomerId == customerId).Sum(s => s.Total);

            var soBreakDownDetails = _tipsSalesServiceDbContext.SOBreakDowns.Where(x => x.CustomerId == customerId).Select(x => x.AmountAgainstSO).Count();
            if (soBreakDownDetails != 0)
            {
                var SODetails = from e in _tipsSalesServiceDbContext.SalesOrders
                                where e.CustomerId == customerId
                                join d in _tipsSalesServiceDbContext.SOBreakDowns on e.SalesOrderNumber equals d.SalesOrderNumber into dept
                                from SOBreakDown in dept.DefaultIfEmpty()
                                group new { e, SOBreakDown } by new { e.Id, e.SalesOrderNumber, e.Total ,e.TypeOfSolution} into g
                                select new OpenSalesOrderDetailsDto
                                {
                                    SalesOrderId = g.Key.Id,
                                    SalesOrderNo = g.Key.SalesOrderNumber,
                                    TypeOfSolution = g.Key.TypeOfSolution,
                                    TotalValue = g.Key.Total,
                                    PendingValue = g.Key.Total - g.Sum(x => x.SOBreakDown.AmountAgainstSO),
                                    AmountRecieved = g.Sum(x => x.SOBreakDown.AmountAgainstSO)
                                };

                var soData = SODetails.ToList();

                return soData;


            }
            else
            {
                var SODetails = from e in _tipsSalesServiceDbContext.SalesOrders
                                where e.CustomerId == customerId
                                join d in _tipsSalesServiceDbContext.SOBreakDowns on e.CustomerId equals d.CustomerId into dept
                                from SOBreakDown in dept.DefaultIfEmpty()
                                select new OpenSalesOrderDetailsDto
                                {
                                    SalesOrderId = e.Id,
                                    SalesOrderNo = e.SalesOrderNumber,
                                    TypeOfSolution = e.TypeOfSolution,
                                    TotalValue = e.Total,
                                    PendingValue = e.Total,
                                    AmountRecieved = 0
                                };

            var soData = SODetails.ToList();

            return soData; 
            }
        }

        public async Task<List<OpenSalesOrderDetailsForKeusDto>> GetOpenSODetailsByCustomerIdForKeus(string salesOrderNumber)
        {

            var soBreakDownDetails = _tipsSalesServiceDbContext.SOBreakDowns.Where(x => x.SalesOrderNumber == salesOrderNumber).Count();
            if (soBreakDownDetails != 0)
            {
                var SODetails = from e in _tipsSalesServiceDbContext.SalesOrders
                                where e.SalesOrderNumber == salesOrderNumber
                                join d in _tipsSalesServiceDbContext.SOBreakDowns on e.SalesOrderNumber equals d.SalesOrderNumber into dept
                                from SOBreakDown in dept.DefaultIfEmpty()
                                group new { e, SOBreakDown } by new { e.Id, e.SalesOrderNumber, e.Total, e.TypeOfSolution } into g
                                select new OpenSalesOrderDetailsForKeusDto
                                {
                                    SalesOrderId = g.Key.Id,
                                    SalesOrderNo = g.Key.SalesOrderNumber,
                                    TypeOfSolution = g.Key.TypeOfSolution,
                                    TotalValue = g.Key.Total,
                                    PendingValue = g.Key.Total - g.Sum(x => x.SOBreakDown.AmountAgainstSO),
                                    AmountRecieved = g.Sum(x => x.SOBreakDown.AmountAgainstSO)
                                };

                var soData = SODetails.ToList();

                return soData;


            }
            else
            {
                var SODetails = from e in _tipsSalesServiceDbContext.SalesOrders
                                where e.SalesOrderNumber == salesOrderNumber
                                join d in _tipsSalesServiceDbContext.SOBreakDowns on e.SalesOrderNumber equals d.SalesOrderNumber into dept
                                from SOBreakDown in dept.DefaultIfEmpty()
                                select new OpenSalesOrderDetailsForKeusDto
                                {
                                    SalesOrderId = e.Id,
                                    SalesOrderNo = e.SalesOrderNumber,
                                    TypeOfSolution = e.TypeOfSolution,
                                    TotalValue = e.Total,
                                    PendingValue = e.Total,
                                    AmountRecieved = 0
                                };

                var soData = SODetails.ToList();

                return soData;
            }
        }

        public async Task<CollectionTrackerDetailsDto> GetSOCollectionTrackerByCustomerId(string customerId)
        {
            var salesOrderTotalValue = _tipsSalesServiceDbContext.SalesOrders
                    .Where(x => x.CustomerId == customerId)
                        .Sum(s => s.Total);

            var collectionDetails = _tipsSalesServiceDbContext.CollectionTrackers
                .Where(x => x.CustomerId == customerId)
                .Select(x => x.AlreadyRecieved)
                .Count();

            if (collectionDetails != 0)
            {
                var amountRecieved = _tipsSalesServiceDbContext.CollectionTrackers
                    .Where(x => x.CustomerId == customerId)
                    .Sum(x => x.AmountRecieved);

                //var alreadyRecieved = _tipsSalesServiceDbContext.CollectionTrackers
                //    .Where(x => x.CustomerId == customerId)
                //    .Sum(x => x.AlreadyRecieved);

                //var alreadyRecievedData = Convert.ToInt32(alreadyRecieved) + Convert.ToInt32(amountRecieved);

                var collectiveTrackerDetails = new CollectionTrackerDetailsDto()
                {
                    TotalSumOfSOAmount = salesOrderTotalValue,
                    AlreadyRecieved = Convert.ToDecimal(amountRecieved)
                };

                return collectiveTrackerDetails;
            }
            else
            {
                var alreadyRecieved = 0;

                var collectiveTrackerDetails = new CollectionTrackerDetailsDto()
                {
                    TotalSumOfSOAmount = salesOrderTotalValue,
                    AlreadyRecieved = alreadyRecieved
                };

                return collectiveTrackerDetails;
            }
        }

                //var salesOrderTotalValue = _tipsSalesServiceDbContext.SalesOrders.Where(x => x.CustomerId == customerId).Sum(s => s.Total);

                //var collectionDetails = _tipsSalesServiceDbContext.CollectionTrackers.Where(x => x.CustomerId == customerId).Select(x=>x.AlreadyRecieved).Count();
                //if (collectionDetails != 0)
                //{
                //    var amountRecieved = _tipsSalesServiceDbContext.CollectionTrackers.Where(x => x.CustomerId == customerId).Sum(x=>x.AmountRecieved);
                //    var alreadyRecieved = _tipsSalesServiceDbContext.CollectionTrackers.Where(x => x.CustomerId == customerId).Sum(x => x.AlreadyRecieved);

                //    var alreadyRecievedData = Convert.ToInt32(alreadyRecieved) + Convert.ToInt32(amountRecieved);

                //    var collectiveTrackerDetails = await _tipsSalesServiceDbContext.SalesOrders
                //                    .Select(s => new CollectionTrackerDetailsDto()
                //                    {
                //                        TotalSumOfSOAmount = salesOrderTotalValue,
                //                        AlreadyRecieved = Convert.ToDecimal(alreadyRecievedData)

                //                    }).Distinct().FirstOrDefaultAsync();

                //    return collectiveTrackerDetails;
                //}
                //else
                //{
                //    var alreadyRecieved = 0;
                //    var collectiveTrackerDetails = await _tipsSalesServiceDbContext.SalesOrders
                //                   .Select(s => new CollectionTrackerDetailsDto()
                //                   {
                //                       TotalSumOfSOAmount = salesOrderTotalValue,
                //                       AlreadyRecieved = alreadyRecieved

                //                   }).Distinct().FirstOrDefaultAsync();

                //    return collectiveTrackerDetails;
                //}

            

        public async Task<string> UpdateCollectionTracker(CollectionTracker collectionTracker, List<SOBreakDown> sOBreakDowns)
        {
            collectionTracker.LastModifiedBy = _createdBy;
            collectionTracker.LastModifiedOn = DateTime.Now;
            Update(collectionTracker);
            _tipsSalesServiceDbContext.SOBreakDowns.RemoveRange(sOBreakDowns);
            string result = $"CollectionTracker details of {collectionTracker.Id} is updated successfully!";
            return result;
        }
    }
}

