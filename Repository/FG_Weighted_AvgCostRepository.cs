using Contracts;
using Entities;
using Entities.DTOs;
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
    public class FG_Weighted_AvgCostRepository : RepositoryBase<FG_Weighted_AvgCost>, I_FG_Weighted_AvgCostRepository
    {
        private TipsMasterDbContext _tipsMasterDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly String _createdBy;
        //private readonly String _unitname;
        public FG_Weighted_AvgCostRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            //var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            //_createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            //_unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }
        public async Task<Dictionary<string, decimal>> GetFGsAndLatestVersion()
        {
            return await _tipsMasterDbContext.ProductionBoms.Where(x => x.ItemType == PartType.FG).GroupBy(p => p.ItemNumber).Select(g => new
            {
                ItemNumber = g.Key,
                LatestVersion = g.Max(p => p.ReleaseVersion)
            })
        .ToDictionaryAsync(x => x.ItemNumber, x => x.LatestVersion);
        }
        public async Task<List<FG_Weighted_AvgCost>> GetAllFG_Weighted_AvgCost()
        {
            var FG_Weighted_AvgCost = await _tipsMasterDbContext.FG_Weighted_AvgCost.AsNoTracking().ToListAsync();
            return FG_Weighted_AvgCost;
        }
        public async Task<FG_Weighted_AvgCost?> GetFG_Weighted_AvgCost(string Itemnumber)
        {
            var FG_Weighted_AvgCost = await _tipsMasterDbContext.FG_Weighted_AvgCost.Where(x => x.Itemnumber == Itemnumber).FirstOrDefaultAsync();
            return FG_Weighted_AvgCost;
        }
        public async Task DeleteExistingData()
        {
            await _tipsMasterDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE FG_Weighted_AvgCost");
        }
        public async Task<WeightedAvgRate?> GetPPWeightedAvgCost(string Itemnumber)
        {
            var WeightedAvgRate = await _tipsMasterDbContext.weighted_avg_rate?.Where(x => x.Itemnumber == Itemnumber).FirstOrDefaultAsync();
            return WeightedAvgRate;
        }
        public async Task CreateFG_Weighted_AvgCost(FG_Weighted_AvgCost fG_Weighted_AvgCost)
        {
            await Create(fG_Weighted_AvgCost);
        }
        public async Task<List<FG_Weighted_AvgCost_Report>> FG_Weighted_AvgCost_Report_withParameter(string FGItemNumber)
        {
            var result = _tipsMasterDbContext
            .Set<FG_Weighted_AvgCost_Report>()
            .FromSqlInterpolated($"CALL FG_Costing_with_Parameter({FGItemNumber})")
            .ToList();

            return result;
        }
        public async Task<List<Weighted_AvgCost_Report>> Weighted_AvgCost_Report_withParameter(string? FGItemNumber)
        {
            var result = _tipsMasterDbContext
            .Set<Weighted_AvgCost_Report>()
            .FromSqlInterpolated($"CALL Weighted_Avg_Report({FGItemNumber})")
            .ToList();

            return result;
        }
        public async Task<List<FG_Weighted_AvgCost_Report_withDate>> FG_Weighted_AvgCost_Report_withDate(string FromDate, string ToDate)
        {
            var result = _tipsMasterDbContext
            .Set<FG_Weighted_AvgCost_Report_withDate>()
            .FromSqlInterpolated($"CALL FG_Costing_with_Date({FromDate},{ToDate})")
            .ToList();

            return result;
        }
        public async Task<List<WeightedAvgRate>> GetAllPPWeightedAvgCost()
        {
            var WeightedAvgRate = await _tipsMasterDbContext.weighted_avg_rate.ToListAsync();
            return WeightedAvgRate;
        }
    }
}
