using Contracts;
using Entities;
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
        private readonly String _createdBy;
        private readonly String _unitname;
        public FG_Weighted_AvgCostRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _tipsMasterDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<List<FG_Weighted_AvgCost>> GetAllFG_Weighted_AvgCost()
        {
            var FG_Weighted_AvgCost = await _tipsMasterDbContext.FG_Weighted_AvgCost.ToListAsync();
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
        public async Task<WeightedAvgRate> GetPPWeightedAvgCost(string Itemnumber)
        {
            var WeightedAvgRate = await _tipsMasterDbContext.weighted_avg_rate.Where(x => x.Itemnumber == Itemnumber).FirstOrDefaultAsync();
            return WeightedAvgRate;
        }
        public async Task CreateFG_Weighted_AvgCost(FG_Weighted_AvgCost fG_Weighted_AvgCost)
        {
            await Create(fG_Weighted_AvgCost);
        }
    }
}
