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
    public class KIT_GRINRepository : RepositoryBase<KIT_GRIN>, IKIT_GRINRepository
    {
        private TipsGrinDbContext _tipsGrinDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public KIT_GRINRepository(TipsGrinDbContext tipsGrinDbContext, IHttpContextAccessor httpContextAccessor) : base(tipsGrinDbContext)
        {
            _tipsGrinDbContext = tipsGrinDbContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<string> GenerateKIT_GrinNumberForAvision()
        {
            using var transaction = await _tipsGrinDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var grinNumberEntity = await _tipsGrinDbContext.KIT_GrinNumbers.SingleAsync();
                grinNumberEntity.CurrentValue += 1;
                _tipsGrinDbContext.Update(grinNumberEntity);
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

                return $"ASPL|KIT_GRN|{currentYear:D2}-{nextYear:D2}|{grinNumberEntity.CurrentValue:D4}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<string> GenerateKIT_GrinNumber()
        {
            using var transaction = await _tipsGrinDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

            try
            {
                var grinNumberEntity = await _tipsGrinDbContext.KIT_GrinNumbers.SingleAsync();
                grinNumberEntity.CurrentValue += 1;
                _tipsGrinDbContext.Update(grinNumberEntity);
                await _tipsGrinDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return $"KIT_G-{grinNumberEntity.CurrentValue:D6}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }
        public async Task<int?> CreateKIT_Grin(KIT_GRIN grins)
        {
            grins.CreatedBy = _createdBy;
            grins.CreatedOn = DateTime.Now;
            grins.Unit = _unitname;
            var result = await Create(grins);
            return result.Id;
        }
        public async Task<IEnumerable<KIT_GrinNoForKIT_IqcAndKIT_Binning>> GetAllKIT_GrinNumberForKIT_IQC()
        {           
            var grinparts = await _tipsGrinDbContext.KIT_GRINParts.Where(x => x.IsKIT_IqcCompleted == false && x.IsKIT_BinningCompleted == false).Select(x => x.KIT_GRINId).Distinct().ToListAsync();            
            List<KIT_GrinNoForKIT_IqcAndKIT_Binning> grinNoForIqc = await _tipsGrinDbContext.KIT_GRIN.Where(x => grinparts.Contains(x.Id)).Select(x => new KIT_GrinNoForKIT_IqcAndKIT_Binning()
            {
                KIT_GrinNumber = x.KIT_GrinNumber,
                KIT_GrinId = x.Id
            }).ToListAsync();
            return grinNoForIqc;
        } 
        public async Task<IEnumerable<KIT_GrinNoForKIT_IqcAndKIT_Binning>> GetAllKIT_GrinNumberForKIT_Binning()
        {           
            var grinparts = await _tipsGrinDbContext.KIT_GRINParts.Where(x => x.IsKIT_IqcCompleted == true && x.IsKIT_BinningCompleted == false).Select(x => x.KIT_GRINId).Distinct().ToListAsync();            
            List<KIT_GrinNoForKIT_IqcAndKIT_Binning> grinNoForIqc = await _tipsGrinDbContext.KIT_GRIN.Where(x => grinparts.Contains(x.Id)).Select(x => new KIT_GrinNoForKIT_IqcAndKIT_Binning()
            {
                KIT_GrinNumber = x.KIT_GrinNumber,
                KIT_GrinId = x.Id
            }).ToListAsync();
            return grinNoForIqc;
        }
        public async Task<KIT_GRIN> GetKIT_GrinById(int id)
        {
            var grinDetailsbyId = await _tipsGrinDbContext.KIT_GRIN.Where(x => x.Id == id)
              .Include(t => t.KIT_GRINParts)
               .ThenInclude(d => d.KIT_GRIN_ProjectNumbers).ThenInclude(x=>x.KIT_GRIN_KITComponents)
               .Include(d => d.KIT_GRIN_OtherCharges).FirstOrDefaultAsync();

            return grinDetailsbyId;
        }
        public async Task<string> UpdateKIT_GRINDetails(KIT_GRIN kIT_GRIN)
        {
            Update(kIT_GRIN);
            string result = $"KIT_GRIN Detail {kIT_GRIN.Id} is updated successfully!";
            return result;
        }
        public async Task<string> UpdateKIT_GRIN(KIT_GRIN kIT_GRIN)
        {
            kIT_GRIN.LastModifiedBy = _createdBy;
            kIT_GRIN.LastModifiedOn = DateTime.Now;
            kIT_GRIN.Unit = _unitname;
            Update(kIT_GRIN);
            string result = $"KIT_GRIN Detail {kIT_GRIN.Id} is updated successfully!";
            return result;
        }
        public async Task<PagedList<KIT_GRIN>> GetAllKIT_GRIN([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var getAllGrinDetails = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.KIT_GrinNumber.Contains(searchParams.SearchValue) ||
                 inv.InvoiceNumber.Contains(searchParams.SearchValue) || inv.VendorName.Contains(searchParams.SearchValue))));       

            return PagedList<KIT_GRIN>.ToPagedList(getAllGrinDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }
        public async Task<KIT_GRIN?> GetKIT_GrinByKIT_GrinNumber(string kIT_GrinNumber)
        {
            return await _tipsGrinDbContext.KIT_GRIN.Where(x => x.KIT_GrinNumber == kIT_GrinNumber).Include(t => t.KIT_GRINParts)
               .ThenInclude(d => d.KIT_GRIN_ProjectNumbers).ThenInclude(x => x.KIT_GRIN_KITComponents)
               .Include(d => d.KIT_GRIN_OtherCharges).AsQueryable().FirstOrDefaultAsync();
        }
    }
}
