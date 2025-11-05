using Contracts;
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
    }
}
