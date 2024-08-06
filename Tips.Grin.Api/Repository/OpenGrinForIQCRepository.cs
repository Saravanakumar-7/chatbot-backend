using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Repository
{
    public class OpenGrinForIQCRepository : RepositoryBase<OpenGrinForIQC>, IOpenGrinForIQCRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OpenGrinForIQCRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<PagedList<OpenGrinForIQC>> GetAllOpenGrinForIQCDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var openGrinForIQCDetails = FindAll()
                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || (inv.OpenGrinNumber != null && inv.OpenGrinNumber.Contains(searchParams.SearchValue)) ||
                   (inv.Id != null && inv.Id.ToString().Contains(searchParams.SearchValue))
                ))).OrderByDescending(x => x.Id);


            return PagedList<OpenGrinForIQC>.ToPagedList(openGrinForIQCDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

        }
        public async Task<IEnumerable<OpenGrinForIQCSPReport>> GetOpenGrinForIQCSPReportWithParam(string? openGrinForIQCNumber, string? itemNumber)
        {
            var result = _tipsGrinDbContext
            .Set<OpenGrinForIQCSPReport>()
            .FromSqlInterpolated($"CALL Opengrinforiqc_with_parameter({openGrinForIQCNumber},{itemNumber})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<OpenGrinForIQCSPReport>> GetOpenGrinForIQCSPReportWithParamForTrans(string? openGrinForIQCNumber, string? itemNumber, string? projectNumber)
        {
            var result = _tipsGrinDbContext
            .Set<OpenGrinForIQCSPReport>()
            .FromSqlInterpolated($"CALL Opengrinforiqc_with_parameter_tras({openGrinForIQCNumber},{itemNumber},{projectNumber})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<OpenGrinForIQCSPReport>> GetOpenGrinForIQCSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsGrinDbContext.Set<OpenGrinForIQCSPReport>()
                      .FromSqlInterpolated($"CALL Opengrinforiqc_with_date({FromDate},{ToDate})")
                      .ToList();

            return results;
        }
        public async Task<OpenGrinForIQC> GetOpenGrinForIQCDetailsbyId(int id)
        {
            var openForGrinIQCDetails= await _tipsGrinDbContext.OpenGrinForIQCs.Where(x => x.Id == id)
                                .Include(x => x.OpenGrinForIQCItems)
                                .FirstOrDefaultAsync();
            return openForGrinIQCDetails;
        }

        public async Task<int?> CreateOpenGrinForIQC(OpenGrinForIQC openGrinForIQC)
        {
            var date = DateTime.Now;
            openGrinForIQC.CreatedBy = _createdBy;
            openGrinForIQC.CreatedOn = date.Date;
            openGrinForIQC.Unit = _unitname;

            var result = await Create(openGrinForIQC);
            return result.Id;
        }
        public async Task<OpenGrinForIQC> GetOpenGrinForIQCDetailsbyOpenGrinNo(string openGrinNumber)
        {
            var iQCDetail = await _tipsGrinDbContext.OpenGrinForIQCs
                .Where(x => x.OpenGrinNumber == openGrinNumber)
               .Include(t => t.OpenGrinForIQCItems)

                       .FirstOrDefaultAsync();
            return iQCDetail;
        }
        public async Task<string> UpdateOpenGrinForIQC(OpenGrinForIQC openGrinForIQC)
        {
            openGrinForIQC.LastModifiedBy = _createdBy;
            openGrinForIQC.LastModifiedOn = DateTime.Now;
            Update(openGrinForIQC);
            string result = $"OpenGrinForIQC details of {openGrinForIQC.Id} is updated successfully!";
            return result;
        }

    }
}
