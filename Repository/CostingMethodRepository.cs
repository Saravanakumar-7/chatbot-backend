using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CostingMethodRepository : RepositoryBase<CostingMethod>, ICostingMethodRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public CostingMethodRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateCostingMethod(CostingMethod costingMethod)
        {
            costingMethod.CreatedBy = _createdBy;
            costingMethod.CreatedOn = DateTime.Now;
            costingMethod.Unit = _unitname;
            var result = await Create(costingMethod);
            
            return result.Id;
        }

        public async Task<string> DeleteCostingMethod(CostingMethod costingMethod)
        {
            Delete(costingMethod);
            string result = $"Costing Method details of {costingMethod.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CostingMethod>> GetAllActiveCostingMethods([FromQuery] SearchParames searchParams)
        {
            var costingMethodDetails = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CostingMethodName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return costingMethodDetails;
        }

        public async Task<IEnumerable<CostingMethod>> GetAllCostingMethods([FromQuery] SearchParames searchParams)
        {
            var costingMethodDetails = FindAll().OrderByDescending(x => x.Id)
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CostingMethodName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return costingMethodDetails;
        }

        public async Task<CostingMethod> GetCostingMethodById(int id)
        {

            var CostingMethodbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return CostingMethodbyId;
        }

        public async Task<string> UpdateCostingMethod(CostingMethod costingMethod)
        {

            costingMethod.LastModifiedBy = _createdBy;
            costingMethod.LastModifiedOn = DateTime.Now;
            Update(costingMethod);
            string result = $"Costing Method details of {costingMethod.Id} is updated successfully!";
            return result;
        }
    }
}
