using Contracts;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SA_Weighted_AvgCostRepository : RepositoryBase<SA_Weighted_AvgCost>, I_SA_Weighted_AvgCostRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public SA_Weighted_AvgCostRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<EnggBom> GetEnggBomByItemNoAndRevNo(string itemNumber, decimal revisionNumber)
        {
            var EnggBomDetailsbyItemNumber = await _tipsMasterDbContext.EnggBoms.Where(x => x.ItemNumber == itemNumber && x.RevisionNumber == revisionNumber && x.IsActive == true)                               
                               .Include(t => t.EnggChildItems)
                             .FirstOrDefaultAsync();

            return EnggBomDetailsbyItemNumber;
        }
        public async Task<Dictionary<string, decimal>> GetSAsAndLatestVersion()
        {
            return await _tipsMasterDbContext.ProductionBoms.Where(x => x.ItemType == PartType.SA).GroupBy(p => p.ItemNumber).Select(g => new
            {
                ItemNumber = g.Key,
                LatestVersion = g.Max(p => p.ReleaseVersion)
            })
        .ToDictionaryAsync(x => x.ItemNumber, x => x.LatestVersion);
        }
        public async Task<List<SA_Weighted_AvgCost>> GetAllSA_Weighted_AvgCost()
        {
            var SA_Weighted_AvgCost = await _tipsMasterDbContext.SA_Weighted_AvgCost.AsNoTracking().ToListAsync();
            return SA_Weighted_AvgCost;
        }
        public async Task<SA_Weighted_AvgCost?> GetSA_Weighted_AvgCost(string Itemnumber)
        {
            var SA_Weighted_AvgCost = await _tipsMasterDbContext.SA_Weighted_AvgCost.Where(x => x.Itemnumber == Itemnumber).AsNoTracking().FirstOrDefaultAsync();
            return SA_Weighted_AvgCost;
        }
        public async Task DeleteExistingData()
        {
            await _tipsMasterDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE SA_Weighted_AvgCost");
        }
        public async Task<WeightedAvgRate> GetPPWeightedAvgCost(string Itemnumber)
        {
            var WeightedAvgRate = await _tipsMasterDbContext.weighted_avg_rate.Where(x => x.Itemnumber == Itemnumber).FirstOrDefaultAsync();
            return WeightedAvgRate;
        }
        public void CreateSA_Weighted_AvgCost(SA_Weighted_AvgCost sA_Weighted_AvgCost)
        {
            Create(sA_Weighted_AvgCost);
        }
    }
}
