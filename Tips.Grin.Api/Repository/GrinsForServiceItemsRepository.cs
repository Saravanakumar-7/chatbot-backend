using Entities.Helper;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Contracts;
using Microsoft.EntityFrameworkCore;
using Tips.Grin.Api.Entities.DTOs;

namespace Tips.Grin.Api.Repository
{
    public class GrinsForServiceItemsRepository : RepositoryBase<GrinsForServiceItems>, IGrinsForServiceItemsRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public GrinsForServiceItemsRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<PagedList<GrinsForServiceItems>> GrinsForServiceItemsForServiceItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var getAllGrinsForServiceItemsDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.GrinsForServiceItemsNumber.Contains(searchParams.SearchValue) ||
                 inv.InvoiceNumber.Contains(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue))));

            return PagedList<GrinsForServiceItems>.ToPagedList(getAllGrinsForServiceItemsDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<GrinsForServiceItems> GetGrinsForServiceItemsById(int id)
        {
            var grinDetailsbyId = await _tipsGrinDbContext.GrinsForServiceItems.Where(x => x.Id == id).Include(t => t.GrinsForServiceItemsParts)
               .ThenInclude(d => d.GrinsForServiceItemsProjectNumbers).Include(d => d.GrinsForServiceItemsOtherCharges).FirstOrDefaultAsync();

            return grinDetailsbyId;
        }
        public async Task<IEnumerable<GrinForServiceItemsSPReport>> GetGrinsForServiceItemsSPReportWithParam(string? GrinsForServiceItemsNumber, string? VendorName, string? PONumber,
                                                                                                   string? KPN, string? MPN, string? Warehouse, string? Location)
        {
            var result = _tipsGrinDbContext
            .Set<GrinForServiceItemsSPReport>()
            .FromSqlInterpolated($"CALL GrinsForService_Report_withparameter({GrinsForServiceItemsNumber},{VendorName},{PONumber},{KPN},{MPN},{Warehouse},{Location})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<GrinForServiceItemsSPReport>> GetGrinsForServiceItemsSPReportWithParamForTrans(string? GrinsForServiceItemsNumber, string? VendorName, string? PONumber,
                                                                                                  string? KPN, string? MPN, string? Warehouse, string? Location, string? ProjectNumber)
        {
            var result = _tipsGrinDbContext
            .Set<GrinForServiceItemsSPReport>()
            .FromSqlInterpolated($"CALL GrinsForService_Report_withparameter_tras({GrinsForServiceItemsNumber},{VendorName},{PONumber},{KPN},{MPN},{Warehouse},{Location},{ProjectNumber})")
            .ToList();

            return result;
        }
        public async Task<IEnumerable<GrinForServiceItemsSPReport>> GetGrinsForServiceItemsSPReportWithDate(DateTime? FromDate, DateTime? ToDate)
        {
            var results = _tipsGrinDbContext.Set<GrinForServiceItemsSPReport>()
                      .FromSqlInterpolated($"CALL GrinsForService_Report_withdate({FromDate},{ToDate})")
                      .ToList();

            return results;
        }

        public async Task<string> GenerateGrinsForServiceItemsNumberForAvision()
        {
            using var transaction = await _tipsGrinDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var grinNumberEntity = await _tipsGrinDbContext.GrinsForServiceItemsNumber.SingleAsync();
                grinNumberEntity.CurrentValue += 1;
                _tipsGrinDbContext.Update(grinNumberEntity);
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

                return $"ASPL|GSI|{currentYear:D2}-{nextYear:D2}|{grinNumberEntity.CurrentValue:D4}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<string> GenerateGrinsForServiceItemsNumber()
        {
            using var transaction = await _tipsGrinDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var grinNumberEntity = await _tipsGrinDbContext.GrinsForServiceItemsNumber.SingleAsync();
                grinNumberEntity.CurrentValue += 1;
                _tipsGrinDbContext.Update(grinNumberEntity);
                await _tipsGrinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"GSI-{grinNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<int?> CreateGrinsForServiceItems(GrinsForServiceItems grinsForServiceItems)
        {
            var date = DateTime.Now;
            grinsForServiceItems.CreatedBy = _createdBy;
            grinsForServiceItems.CreatedOn = date.Date;
            grinsForServiceItems.Unit = _unitname;

            var result = await Create(grinsForServiceItems);
            return result.Id;
        }
        public async Task<GrinsForServiceItems> GetGrinForServiceItemsByGrinForServiceItemsNo(string grinsForServiceItemsNumber)
        {
            var grinDetailsbyGrin = await _tipsGrinDbContext.GrinsForServiceItems.Where(x => x.GrinsForServiceItemsNumber == grinsForServiceItemsNumber).Include(t => t.GrinsForServiceItemsParts).ThenInclude(p => p.GrinsForServiceItemsProjectNumbers)
            .FirstOrDefaultAsync();

            return grinDetailsbyGrin;
        }
        public async Task<string> UpdateGrinsForServiceItems(GrinsForServiceItems grinsForServiceItems)
        {
            grinsForServiceItems.LastModifiedBy = _createdBy;
            grinsForServiceItems.LastModifiedOn = DateTime.Now;
            Update(grinsForServiceItems);
            string result = $"GrinsForServiceItems Detail {grinsForServiceItems.Id} is updated successfully!";
            return result;
        }
        public async Task<int?> GetGrinsForServiceItemsIqcForServiceItemsStatusCount(string grinNo)
        {
            var grinIqcStatusCount = _tipsGrinDbContext.GrinsForServiceItems.Where(x => x.GrinsForServiceItemsNumber == grinNo && x.IsIqcForServiceItemsCompleted == true).Count();

            return grinIqcStatusCount;
        }
        public async Task<IEnumerable<GrinForServiceItemsNoForIqcForServiceItems>> GetAllGrinForServiceItemsNumberForIqcForServiceItems()
        {           
            var grinparts = await _tipsGrinDbContext.GrinsForServiceItemsParts.Where(x => x.IsIqcForServiceItemsCompleted == false).Select(x => x.GrinsForServiceItemsId).ToListAsync();
            var gId = grinparts.Distinct().ToList();
            List<GrinForServiceItemsNoForIqcForServiceItems> grinNoForIqc = await _tipsGrinDbContext.GrinsForServiceItems.Where(x => gId.Contains(x.Id)).Select(x => new GrinForServiceItemsNoForIqcForServiceItems()
            {
                GrinsForServiceItemsNumber = x.GrinsForServiceItemsNumber,
                GrinsForServiceItemsId = x.Id
            }).ToListAsync();
            return grinNoForIqc;
        }
        public async Task<IEnumerable<GrinsForServiceItems>> GetGrinDetailsofPOByGrinIds(List<int> grinIds, string Ponumber)
        {
            var grinDetailsList = await FindByCondition(x => grinIds.Contains(x.Id))
                .Include(item => item.GrinsForServiceItemsParts.Where(x => x.PONumber == Ponumber))
                    .ThenInclude(pr => pr.GrinsForServiceItemsProjectNumbers)
                    .Include(item => item.GrinsForServiceItemsParts.Where(x => x.PONumber == Ponumber))
                    .Include(o => o.GrinsForServiceItemsOtherCharges)
                .ToListAsync();
            return grinDetailsList;
        }
    }
}
