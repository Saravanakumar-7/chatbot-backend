using Contracts;
using Entities;
using Entities.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ScopeOfSupplyRepository : RepositoryBase<ScopeOfSupply>, IScopeOfSupplyRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public ScopeOfSupplyRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateScopeOfSupply(ScopeOfSupply scopeOfSupply)
        {
            scopeOfSupply.CreatedBy = _createdBy;
            scopeOfSupply.CreatedOn = DateTime.Now;
            scopeOfSupply.Unit = _unitname;
            var result = await Create(scopeOfSupply);
            
            return result.Id;
        }

        public async Task<string> DeleteScopeOfSupply(ScopeOfSupply scopeOfSupply)
        {
            Delete(scopeOfSupply);
            string result = $"Scope Of Supply details of {scopeOfSupply.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ScopeOfSupply>> GetAllActiveScopeOfSupply([FromQuery] SearchParames searchParams)
        {
            var scopeOfSuppliesDetials = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ScopeOfSupplyName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return scopeOfSuppliesDetials;
        }

        public async Task<IEnumerable<ScopeOfSupply>> GetAllScopeOfSupply([FromQuery] SearchParames searchParams)
        {
            var scopeOfSuppliesDetials = FindAll()
           .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ScopeOfSupplyName.Contains(searchParams.SearchValue) ||
          inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));

            return scopeOfSuppliesDetials;
        }

        public async Task<ScopeOfSupply> GetScopeOfSupplyById(int id)
        {
            var ScopeOfSupplybyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return ScopeOfSupplybyId;
        }

        public async Task<string> UpdateScopeOfSupply(ScopeOfSupply scopeOfSupply)
        {
            scopeOfSupply.LastModifiedBy = _createdBy;
            scopeOfSupply.LastModifiedOn = DateTime.Now;
            Update(scopeOfSupply);
            string result = $"Scope Of Supply of Detail {scopeOfSupply.Id} is updated successfully!";
            return result;
        }
    }
}
