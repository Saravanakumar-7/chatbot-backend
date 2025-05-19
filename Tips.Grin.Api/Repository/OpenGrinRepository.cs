using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Repository
{
    public class OpenGrinRepository : RepositoryBase<OpenGrin>, IOpenGrinRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OpenGrinRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<OpenGrin> CreateOpenGrin(OpenGrin openGrin)
        {
            openGrin.CreatedBy = _createdBy;
            openGrin.CreatedOn = DateTime.Now;
            openGrin.Unit = _unitname;
            var result = await Create(openGrin);
            return result;
        }
        public async Task<IEnumerable<OpenGrin_SPReport>> GetOpenGrinSPReportWithParam(string? openGrinNumber, string? senderName, string? receiptRefNo)
        {
            var result = _tipsGrinDbContext
            .Set<OpenGrin_SPReport>()
            .FromSqlInterpolated($"CALL Open_Grin_Report_withparameter({openGrinNumber},{senderName},{receiptRefNo})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<OpenGrinSpReportForTrans>> GetOpenGrinSPReportWithParamForTrans(string? itemNumber, string? openGrinNumber, string? senderName, string? receiptRefNo
                                                                                                                                         , string? ProjectNumber)
        {
            var result = _tipsGrinDbContext
            .Set<OpenGrinSpReportForTrans>()
            .FromSqlInterpolated($"CALL Open_Grin_Report_withparameter_tras({itemNumber},{openGrinNumber},{senderName},{receiptRefNo},{ProjectNumber})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<OpenGrin_SPReport>> GetOpenGrinSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsGrinDbContext.Set<OpenGrin_SPReport>()
                      .FromSqlInterpolated($"CALL Open_Grin_Report_withdate({FromDate},{ToDate})")
                      .ToList();

            return results;
        }
        public async Task<IEnumerable<OpenGrinSpReportForTrans>> GetOpenGrinSPReportWithDateForTrans(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsGrinDbContext.Set<OpenGrinSpReportForTrans>()
                      .FromSqlInterpolated($"CALL Open_Grin_Report_withdate_tras({FromDate},{ToDate})")
                      .ToList();

            return results;
        }
        public async Task<string> DeleteOpenGrin(OpenGrin openGrin)
        {
            Delete(openGrin);
            string result = $"OpenGrin details of {openGrin.Id} is deleted successfully!";
            return result;
        }
        public async Task<PagedList<OpenGrin_SPReport>> GetOpenGrinSPReport(PagingParameter pagingParameter)
        {

            var results = _tipsGrinDbContext.Set<OpenGrin_SPReport>()
                      .FromSqlInterpolated($"CALL Open_Grin_Report")
                      .ToList();

            return PagedList<OpenGrin_SPReport>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<PagedList<OpenGrinSpReportForTrans>> GetOpenGrinSPReportForTrans(PagingParameter pagingParameter)
        {

            var results = _tipsGrinDbContext.Set<OpenGrinSpReportForTrans>()
                      .FromSqlInterpolated($"CALL Open_Grin_Report_tras")
                      .ToList();

            return PagedList<OpenGrinSpReportForTrans>.ToPagedList(results.AsQueryable(), pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<string> GenerateOpenGrinNumber()
        {
            using var transaction = await _tipsGrinDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var poNumberEntity = await _tipsGrinDbContext.OpenGrinNumbers.SingleAsync();
                poNumberEntity.CurrentValue += 1;
                _tipsGrinDbContext.Update(poNumberEntity);
                await _tipsGrinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"OpenGrin-{poNumberEntity.CurrentValue:D5}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<string> GenerateOpenGrinNumberForAvision()
        {
            using var transaction = await _tipsGrinDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var poNumberEntity = await _tipsGrinDbContext.OpenGrinNumbers.SingleAsync();
                poNumberEntity.CurrentValue += 1;
                _tipsGrinDbContext.Update(poNumberEntity);
                await _tipsGrinDbContext.SaveChangesAsync();
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

                return $"ASPL|OPGN|{currentYear:D2}-{nextYear:D2}|{poNumberEntity.CurrentValue:D3}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<PagedList<OpenGrin>> GetAllOpenGrinDetails(PagingParameter pagingParameter, SearchParams searchParams)
        {
            var openGrinDetails = FindAll()
               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
               || inv.SenderName.Contains(searchParams.SearchValue)
               || inv.SenderId.Contains(searchParams.SearchValue)
               || inv.OpenGrinNumber.Contains(searchParams.SearchValue)
               )))
               .OrderByDescending(x => x.Id);

            return PagedList<OpenGrin>.ToPagedList(openGrinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);

            //var openGrinDetails = FindAll()
            //    .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.SenderName.Contains(searchParams.SearchValue)
            //    || inv.SenderId.Contains(searchParams.SearchValue))));

            //return PagedList<OpenGrin>.ToPagedList(openGrinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<OpenGrin>> GetAllOpenGrinWithItems(OpenGrinSearchDto openGrinSearchDto)
        {
            using (var context = _tipsGrinDbContext)
            {
                var query = _tipsGrinDbContext.OpenGrin.Include("OpenGrinParts");
                if (openGrinSearchDto != null || (openGrinSearchDto.OpenGrinNumber.Any())
               && openGrinSearchDto.SenderName.Any() /*&& openGrinSearchDto.ReturnedBy.Any()*/ && openGrinSearchDto.ReceiptRefNo.Any())
                {
                    query = query.Where
                    (og => (openGrinSearchDto.OpenGrinNumber.Any() ? openGrinSearchDto.OpenGrinNumber.Contains(og.OpenGrinNumber) : true)
                   && (openGrinSearchDto.SenderName.Any() ? openGrinSearchDto.SenderName.Contains(og.SenderName) : true)
                   //&& (openGrinSearchDto.ReturnedBy.Any() ? openGrinSearchDto.ReturnedBy.Contains(og.ReturnedBy) : true)
                   && (openGrinSearchDto.ReceiptRefNo.Any() ? openGrinSearchDto.ReceiptRefNo.Contains(og.ReceiptRefNo) : true))
                    .Include(item => item.OpenGrinParts)
                    .ThenInclude(op => op.OpenGrinDetails);
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<OpenGrin>> SearchOpenGrin([FromQuery] SearchParames searchParames)
        {
            using (var context = _tipsGrinDbContext)
            {
                var query = _tipsGrinDbContext.OpenGrin.Include("OpenGrinParts");
                if (!string.IsNullOrEmpty(Convert.ToString(searchParames.SearchValue)))
                {
                    query = query.Where(og => og.OpenGrinNumber.Contains(searchParames.SearchValue)
                    || og.SenderName.Contains(searchParames.SearchValue)
                    || og.SenderId.Contains(searchParames.SearchValue)
                    || og.ReceiptRefNo.Contains(searchParames.SearchValue)
                    || og.ReturnedBy.Contains(searchParames.SearchValue)
                    || og.Remarks.Contains(searchParames.SearchValue))
                        .Include(item => item.OpenGrinParts)
                    .ThenInclude(op => op.OpenGrinDetails);
                }
                return query.ToList();
            }
        }

        public async Task<IEnumerable<OpenGrin>> SearchOpenGrinDate([FromQuery] SearchDateParames searchParames)
        {
            var openGrinDetails = _tipsGrinDbContext.OpenGrin
            .Where(inv => ((inv.CreatedOn >= searchParames.SearchFromDate &&
            inv.CreatedOn <= searchParames.SearchToDate
            )))
            .Include(itm => itm.OpenGrinParts)
            .ThenInclude(op => op.OpenGrinDetails)
            .ToList();
            
            return openGrinDetails;
        }

        public async Task<OpenGrin> GetOpenGrinDetailsbyId(int id)
        {
            var openGrinDetailsById = await _tipsGrinDbContext.OpenGrin.Where(x => x.Id == id)
                               .Include(t => t.OpenGrinParts)
                               .ThenInclude(o=>o.OpenGrinDetails)
                            .FirstOrDefaultAsync();

            return openGrinDetailsById;
        }
        public async Task<OpenGrinParts> GetOpenGrinPartsbyId(int id)
        {
            var openGrinPartDetailsById = await _tipsGrinDbContext.OpenGrinParts.Where(x => x.Id == id)
                            .FirstOrDefaultAsync();

            return openGrinPartDetailsById;
        }
        public async Task<OpenGrinDetails> GetOpenGrinPartDetailsbyId(int id)
        {
            var openGrinPartDetailsById = await _tipsGrinDbContext.OpenGrinDetails.Where(x => x.Id == id)
                            .FirstOrDefaultAsync();

            return openGrinPartDetailsById;
        }
        public async Task<IEnumerable<OpenGrinDataListDto>> GetAllOpenGrinDataList()
        {
            IEnumerable<OpenGrinDataListDto> openGrinDataList = await _tipsGrinDbContext.OpenGrin
                           .Select(x => new OpenGrinDataListDto()
                           {
                               Id = x.Id,
                               OpenGrinNumber = x.OpenGrinNumber,
                               SenderName = x.SenderName,
                               SenderId = x.SenderId,
                               Remarks = x.Remarks,
                               ReturnedBy = x.ReturnedBy,
                               ReceiptRefNo = x.ReceiptRefNo,
                               CustomerSupplied = x.CustomerSupplied,

                           }).ToListAsync();


            return openGrinDataList;

        }
        public async Task<IEnumerable<OpenGrinPartSORefDto>> GetAllOpenGrinSORefDetails()
        {
            IEnumerable<OpenGrinPartSORefDto> openGrinDataList = await _tipsGrinDbContext.OpenGrinParts
                            .GroupBy(p=>p.ReferenceSONumber)
                           .Select(x => new OpenGrinPartSORefDto()
                           {
                               ReferenceSONumber = x.Key,

                           }).ToListAsync();


            return openGrinDataList;

        }
        public async Task<string> UpdateOpenGrin(OpenGrin openGrin)
        {
            openGrin.LastModifiedBy = _createdBy;
            openGrin.LastModifiedOn = DateTime.Now;
            Update(openGrin);
            string result = $"OpenGrin details of {openGrin.Id} is updated successfully!";
            return result;
        }
    }
}
