using Contracts;
using Entities;
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
    public class PriceListRepository : RepositoryBase<PriceList>, IPriceListRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public PriceListRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;

            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreatePriceList(PriceList priceList)
        {
            priceList.CreatedBy = _createdBy;
            priceList.CreatedOn = DateTime.Now;
            priceList.Unit = _unitname;
            var result = await Create(priceList);
            
            return result.Id;
        }

        public async Task<string> DeletePriceList(PriceList priceList)
        {
            Delete(priceList);
            string result = $"PriceList details of {priceList.Id} is deleted successfully!";
            return result;
        }
        public async Task<PriceList> GetLatestPriceLists()
        {
            var priceListDetails = await TipsMasterDbContext.PriceLists.Where(x => x.IsActive == true).OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync();
            return priceListDetails;
        }

        //public async Task<IEnumerable<PriceList>> GetAllTruePriceListCount()
        //{
        //    var falsePriceListDetails = await TipsMasterDbContext.PriceLists.Where(x => x.IsActive == true).ToListAsync();
        //    return falsePriceListDetails;
        //}
        public async Task<IEnumerable<PriceList>> GetAllTruePriceListCount()
        {
            var priceLists = await TipsMasterDbContext.PriceLists
                .Where(x => x.IsActive == true)
                .OrderByDescending(x => x.CreatedOn) // Assuming there is a CreatedDate field
                .Skip(1) // Skip the last record
                .ToListAsync();

            return priceLists;
        }
        public async Task<IEnumerable<PriceList>> GetAllActivePriceLists([FromQuery] SearchParames searchParams)
        {
            var priceListDetails = FindAll()
                              .Where(inv =>inv.IsActive == true && ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.Pricelist.Contains(searchParams.SearchValue) ||
                              inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return priceListDetails;
        }

        public async Task<IEnumerable<PriceList>> GetAllPriceLists([FromQuery] SearchParames searchParams)
        {
            var priceListDetails = FindAll().OrderByDescending(x => x.Id).Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue)
            || inv.Pricelist.Contains(searchParams.SearchValue) || inv.Unit.Contains(searchParams.SearchValue)
            || inv.Description.Contains(searchParams.SearchValue))));
            return priceListDetails;
        }

        public async Task<PriceList> GetPriceListById(int id)
        {

            var PriceListbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return PriceListbyId;
        }

        public async Task<IEnumerable<PriceList>> GetLatestPriceListName()
        {

            var itemPriceList = FindAll().Where(x => x.IsActive == true).OrderByDescending(d => d.CreatedOn)
                        .ToList();

             return itemPriceList;
        }

        public async Task<string> UpdatePriceList(PriceList priceList)
        {
            priceList.LastModifiedBy = _createdBy;
            priceList.LastModifiedOn = DateTime.Now;
            Update(priceList);
            string result = $"PriceList details of {priceList.Id} is updated successfully!";
            return result;
        }
    }
}
