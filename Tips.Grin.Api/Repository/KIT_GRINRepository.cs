using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;

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

                return $"ASPL|KGRN|{currentYear:D2}-{nextYear:D2}|{grinNumberEntity.CurrentValue:D4}";
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
                return $"G-{grinNumberEntity.CurrentValue:D6}";
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
    }
}
