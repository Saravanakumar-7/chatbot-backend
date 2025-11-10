using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Tally.Api.Contracts;
using Tips.Tally.Api.Entities;
using Tips.Tally.Api.Entities.DTOs;

namespace Tips.Tally.Api.Repository
{
    public class TallyRepository : TallyRepositoryBase<TallyVendorMasterSpReport>, ITallyRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public TallyRepository(TipsTallyDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<IEnumerable<TallyVendorMasterSpReport>> GetTallyVendorMasterSpReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = TipsTallyDbContext.Set<TallyVendorMasterSpReport>()
                        .FromSqlInterpolated($"CALL Tally_Vendor_Master({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<TallyCurrencyMasterSPReport>> GetTallyCurrencyMastertSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = TipsTallyDbContext.Set<TallyCurrencyMasterSPReport>()
                        .FromSqlInterpolated($"CALL Tally_Currency_Master({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<TallyCustomerMasterSpReport>> GetTallyCustomerMastertSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = TipsTallyDbContext.Set<TallyCustomerMasterSpReport>()
                        .FromSqlInterpolated($"CALL Tally_Customer_Master({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<TallyPurchaseOrderSpReport>> GetTallyPurchaseOrderSpReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = TipsTallyDbContext.Set<TallyPurchaseOrderSpReport>()
                        .FromSqlInterpolated($"CALL Tally_PurchaseOrder({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<TallyStockItemSPReport>> GetTallyStockItemSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = TipsTallyDbContext.Set<TallyStockItemSPReport>()
                        .FromSqlInterpolated($"CALL Tally_stock_Item({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<TallybtodeliveryorderSpReport>> GetTallybtodeliveryorderSpReportWIthDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = TipsTallyDbContext.Set<TallybtodeliveryorderSpReport>()
                        .FromSqlInterpolated($"CALL Tally_BTODeliveryOrder({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<TallyFGWIPMaterialIssueSpReport>> GetTallyFGWIPMaterialIssueSpReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = TipsTallyDbContext.Set<TallyFGWIPMaterialIssueSpReport>()
                        .FromSqlInterpolated($"CALL Tally_FG_WIP_MaterialIssue({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<TallyGrinSpReport>> GetTallyGrinSpReportSpReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = TipsTallyDbContext.Set<TallyGrinSpReport>()
                        .FromSqlInterpolated($"CALL Tally_GRIN({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
        public async Task<IEnumerable<TallySalesOrderSpReport>> GetTallySalesOrderSpReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = TipsTallyDbContext.Set<TallySalesOrderSpReport>()
                        .FromSqlInterpolated($"CALL Tally_SalesOrder({FromDate},{ToDate})")
                        .ToList();

            return results;

        }
    }
}
