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
using MySqlX.XDevAPI.Common;

namespace Repository
{
    public class UOMRepository : RepositoryBase<UOM>, IUOMRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public UOMRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<int?> CreateUOM(UOM uom)
        { 
            uom.CreatedBy = _createdBy;
            uom.CreatedOn = DateTime.Now;
            uom.Unit = _unitname;
            var result = await Create(uom);

            return result.Id;
        }

        public async Task<string> DeleteUOM(UOM uom)
        {
            Delete(uom);
            string result = $"UOM details of {uom.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<UOM>> GetAllActiveUOM([FromQuery] SearchParames searchParams)
        {
            var uomDetails = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.UOMName.Contains(searchParams.SearchValue) ||
           inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return uomDetails;
        }
        public async Task<IEnumerable<UOM>> GetAllUOM([FromQuery] SearchParames searchParams)
        {
            var uomDetails = FindAll()
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.UOMName.Contains(searchParams.SearchValue) ||
           inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return uomDetails;
        }

        public async Task<UOM> GetUOMById(int id)
        {
            var UombyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return UombyId;
        }

        public async Task<string> UpdateUOM(UOM uom)
        {
            uom.LastModifiedBy = _createdBy;
            uom.LastModifiedOn = DateTime.Now;
            Update(uom);
            string result = $"UOM details of {uom.Id} is updated successfully!";
            return result;
        }
    }
}
