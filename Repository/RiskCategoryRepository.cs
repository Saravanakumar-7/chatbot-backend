using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RiskCategoryRepository : RepositoryBase<RiskCategory>, IRiskCategoryRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public RiskCategoryRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateRiskCategory(RiskCategory riskCategory)
        {
            riskCategory.CreatedBy = _createdBy;
            riskCategory.CreatedOn = DateTime.Now;
            riskCategory.Unit = _unitname;
            var result = await Create(riskCategory);

            return result.Id;
        }

        public async Task<string> DeleteRiskCategory(RiskCategory riskCategory)
        {
            Delete(riskCategory);
            string result = $"RiskCategory details of {riskCategory.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<RiskCategory>> GetAllActiveRiskCategory([FromQuery] SearchParames searchParams)
        {
            var riskCategoryDetails = FindAll()
                                       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.RiskCategoryName.Contains(searchParams.SearchValue) ||
                                 inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return riskCategoryDetails;
        }

        public async Task<IEnumerable<RiskCategory>> GetAllRiskCategory([FromQuery] SearchParames searchParams)
        {
            var riskCategoryDetails = FindAll()
                                       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.RiskCategoryName.Contains(searchParams.SearchValue) ||
                                 inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return riskCategoryDetails;
        }

        public async Task<RiskCategory> GetRiskCategoryById(int id)
        {
            var RiskCategorybyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return RiskCategorybyId;
        }

        public async Task<string> UpdateRiskCategory(RiskCategory riskCategory)
        {
            riskCategory.LastModifiedBy = _createdBy;
            riskCategory.LastModifiedOn = DateTime.Now;
            Update(riskCategory);
            string result = $"RiskCategory details of {riskCategory.Id} is updated successfully!";
            return result;
        }
    }
}
