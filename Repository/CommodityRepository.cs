using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CommodityRepository : RepositoryBase<Commodity>, ICommodityRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public CommodityRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateCommodity(Commodity commodity)
        {
            commodity.CreatedBy = _createdBy;
            commodity.CreatedOn = DateTime.Now;
            commodity.Unit = _unitname;
            var result = await Create(commodity);
            
            return result.Id;
        }

        public async Task<string> DeleteCommodity(Commodity commodity)
        {
            Delete(commodity);
            string result = $"Commodity details of {commodity.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Commodity>> GetAllActiveCommodity([FromQuery] SearchParames searchParams)
        {
            var commodityDetails = FindAll()
          .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CommodityType.Contains(searchParams.SearchValue) ||
         inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return commodityDetails;
        }

        public async Task<IEnumerable<Commodity>> GetAllCommodity([FromQuery] SearchParames searchParams)
        {
            var commodityDetails = FindAll()
          .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CommodityType.Contains(searchParams.SearchValue) ||
         inv.Remarks.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            
            return commodityDetails;
        }


        public async Task<Commodity> GetCommodityById(int id)
        {
            var CommoditybyId= await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return CommoditybyId;
        }

        public async Task<string> UpdateCommodity(Commodity commodity)
        {
            commodity.LastModifiedBy = _createdBy;
            commodity.LastModifiedOn = DateTime.Now;
            Update(commodity);
            string result = $"Commodity details of {commodity.Id} is updated successfully!";
            return result;
        }
    }
}
