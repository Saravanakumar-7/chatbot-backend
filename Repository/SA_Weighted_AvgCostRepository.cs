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
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<List<SA_Weighted_AvgCost>> GetAllSA_Weighted_AvgCost()
        {
            var SA_Weighted_AvgCost = await _tipsMasterDbContext.SA_Weighted_AvgCost.AsNoTracking().ToListAsync();
            return SA_Weighted_AvgCost;
        } 
        public async Task<SA_Weighted_AvgCost?> GetSA_Weighted_AvgCost(string Itemnumber)
        {
            var SA_Weighted_AvgCost = await _tipsMasterDbContext.SA_Weighted_AvgCost.Where(x=>x.Itemnumber==Itemnumber).AsNoTracking().FirstOrDefaultAsync();
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
