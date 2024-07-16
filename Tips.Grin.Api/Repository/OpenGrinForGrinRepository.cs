using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Repository
{
    public class OpenGrinForGrinRepository : RepositoryBase<OpenGrinForGrin>, IOpenGrinForGrinRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OpenGrinForGrinRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<PagedList<OpenGrinForGrin>> GetAllOpenGrinForGrin([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var openGrinForGrinDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.OpenGrinNumber.Contains(searchParams.SearchValue) ||
                 inv.ReceiptRefNo.Contains(searchParams.SearchValue) || inv.Remarks.Contains(searchParams.SearchValue))));

            return PagedList<OpenGrinForGrin>.ToPagedList(openGrinForGrinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<int?> CreateOpenGrinForGrin(OpenGrinForGrin openGrinForGrins)
        {
            var date = DateTime.Now;
            openGrinForGrins.CreatedBy = _createdBy;
            openGrinForGrins.CreatedOn = date.Date;
            openGrinForGrins.Unit = _unitname;

            var result = await Create(openGrinForGrins);
            return result.Id;
        }

        public async Task<OpenGrinForGrin> GetOpenGrinForGrinDetailsbyId(int id)
        {
            var openGrinForGrinDetailsById = await _tipsGrinDbContext.OpenGrinForGrins.Where(x => x.Id == id)
                               .Include(t => t.OpenGrinForGrinItems)
                               .ThenInclude(p=>p.OGNProjectNumber)

                            .FirstOrDefaultAsync();

            return openGrinForGrinDetailsById;
        }

        public async Task<IEnumerable<OpenGrinNoForOpenGrinIqcAndBinning>> GetAllOpenGrinNumberForOpenGrinIqc()
        {
            var openGrinForGrinIds = await _tipsGrinDbContext.OpenGrinForGrinItems
                .Where(x => x.IsOpenGrinForIqcCompleted == false && x.IsOpenGrinForBinningCompleted == false)
                .Select(x => x.OpenGrinForGrinId)
                .ToListAsync();

            var openGrinForGrinId = openGrinForGrinIds.Distinct().ToList();

            List<OpenGrinNoForOpenGrinIqcAndBinning> openGrinNoForOpenGrinIqc = await _tipsGrinDbContext.OpenGrinForGrins
                .Where(x => openGrinForGrinId.Contains(x.Id))
                .Select(x => new OpenGrinNoForOpenGrinIqcAndBinning()
                {
                    OpenGrinNumber = x.OpenGrinNumber,
                    OpenGrinForGrinId = x.Id
                })
                .ToListAsync();

            return openGrinNoForOpenGrinIqc;
        }

        public async Task<IEnumerable<OpenGrinNoForOpenGrinIqcAndBinning>> GetAllOpenGrinNumberForOpenGrinBinning()
        {
            var openGrinForGrinIds = await _tipsGrinDbContext.OpenGrinForGrinItems
                .Where(x => x.IsOpenGrinForIqcCompleted == true && x.IsOpenGrinForBinningCompleted == false)
                .Select(x => x.OpenGrinForGrinId)
                .ToListAsync();

            var openGrinForGrinId = openGrinForGrinIds.Distinct().ToList();

            List<OpenGrinNoForOpenGrinIqcAndBinning> openGrinNoForOpenGrinBinning = await _tipsGrinDbContext.OpenGrinForGrins
                .Where(x => openGrinForGrinId.Contains(x.Id))
                .Select(x => new OpenGrinNoForOpenGrinIqcAndBinning()
                {
                    OpenGrinNumber = x.OpenGrinNumber,
                    OpenGrinForGrinId = x.Id
                })
                .ToListAsync();

            return openGrinNoForOpenGrinBinning;
        }

        public async Task<string> GenerateOpenGrinForGrinNumber()
        {
            using var transaction = await _tipsGrinDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var openGrinForGrinNumberEntity = await _tipsGrinDbContext.OpenGrinForGrinNumbers.SingleAsync();
                openGrinForGrinNumberEntity.CurrentValue += 1;
                _tipsGrinDbContext.Update(openGrinForGrinNumberEntity);
                await _tipsGrinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"OpenGrinForGRIN-{openGrinForGrinNumberEntity.CurrentValue:D5}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<string> GenerateOpenGrinForGrinNumberForAvision()
        {
            using var transaction = await _tipsGrinDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var openGrinForGrinNumberEntity = await _tipsGrinDbContext.OpenGrinForGrinNumbers.SingleAsync();
                openGrinForGrinNumberEntity.CurrentValue += 1;
                _tipsGrinDbContext.Update(openGrinForGrinNumberEntity);
                await _tipsGrinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                DateTime currentDate = DateTime.Now;
                DateTime financeYearStart;

                if (currentDate.Month >= 4) 
                {
                    financeYearStart = new DateTime(currentDate.Year, 4, 1);
                }
                else
                {
                    financeYearStart = new DateTime(currentDate.Year - 1, 4, 1);
                }

                int currentYear = financeYearStart.Year % 100;
                int nextYear = (financeYearStart.Year + 1) % 100; 

                return $"ASPL|OPGNG|{currentYear:D2}-{nextYear:D2}|{openGrinForGrinNumberEntity.CurrentValue:D3}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<OpenGrinForGrin> GetOpenGrinForGrinDetailsByOpenGrinNo(string openGrinNumber)
        {
            var openGrinForGrinDetails = await _tipsGrinDbContext.OpenGrinForGrins.Where(x => x.OpenGrinNumber == openGrinNumber)

        .Include(t => t.OpenGrinForGrinItems)

        .FirstOrDefaultAsync();

            return openGrinForGrinDetails;
        }
        public async Task<string> UpdateOpenGrinForGrin(OpenGrinForGrin openGrinForGrin)
        {
            openGrinForGrin.LastModifiedBy = _createdBy;
            openGrinForGrin.LastModifiedOn = DateTime.Now;
            Update(openGrinForGrin);
            string result = $"OpenGrinForGrin Detail {openGrinForGrin.Id} is updated successfully!";
            return result;
        }
        public async Task<int?> GetOpenGrinForGrinIqcStatusCount(string openGrinNumber)
        {
            var openGrinForGrinIqcStatusCount = _tipsGrinDbContext.OpenGrinForGrins.Where(x => x.OpenGrinNumber == openGrinNumber && x.IsOpenGrinForIqcCompleted == true).Count();

            return openGrinForGrinIqcStatusCount;
        }
        public async Task<int?> GetOpenGrinForGrinbinningStatusCount(string openGrinNo)
        {
            var openGrinForGrinBinningStatusCount = _tipsGrinDbContext.OpenGrinForGrins.Where(x => x.OpenGrinNumber == openGrinNo && x.IsOpenGrinForBinningCompleted == true).Count();

            return openGrinForGrinBinningStatusCount;
        }
    }
}
