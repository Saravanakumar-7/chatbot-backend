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
    public class CostCenterRepository : RepositoryBase<CostCenter>, ICostCenterRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public CostCenterRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateCostCenter(CostCenter costCenter)
        {
            costCenter.CreatedBy = _createdBy;
            costCenter.CreatedOn = DateTime.Now;
            costCenter.Unit = _unitname;
            var result = await Create(costCenter);
            
            return result.Id;
        }

        public async Task<string> DeleteCostCenter(CostCenter costCenter)
        {

            Delete(costCenter);
            string result = $"Costcenter details of {costCenter.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CostCenter>> GetAllActiveCostCenters([FromQuery] SearchParames searchParams)
        {
            var costCenterDetails = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CostCenterName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return costCenterDetails;
        }

        public async Task<IEnumerable<CostCenter>> GetAllCostCenters([FromQuery] SearchParames searchParams)
        {
            var costCenterDetails = FindAll().OrderByDescending(x => x.Id)
                       .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CostCenterName.Contains(searchParams.SearchValue) ||
                      inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return costCenterDetails;
        }

        public async Task<CostCenter> GetCostCenterById(int id)
        {

            var CostCenterbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return CostCenterbyId;
        }

        public async Task<string> UpdateCostCenter(CostCenter costCenter)
        {
            costCenter.LastModifiedBy = _createdBy;
            costCenter.LastModifiedOn = DateTime.Now;
            Update(costCenter);
            string result = $"CostCenter details of {costCenter.Id} is updated successfully!";
            return result;
        }
    }
}
