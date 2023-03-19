using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CommodityRepository : RepositoryBase<Commodity>, ICommodityRepository
    {
        public CommodityRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateCommodity(Commodity commodity)
        {
            commodity.CreatedBy = "Admin";
            commodity.CreatedOn = DateTime.Now;
            commodity.Unit = "Bangalore";
            var result = await Create(commodity);
            
            return result.Id;
        }

        public async Task<string> DeleteCommodity(Commodity commodity)
        {
            Delete(commodity);
            string result = $"Commodity details of {commodity.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<Commodity>> GetAllActiveCommodity([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var commodityDetails = FindAll()
                                  .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CommodityType.Contains(searchParams.SearchValue) ||
                                  inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<Commodity>.ToPagedList(commodityDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<Commodity>> GetAllCommodity([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var commodityDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CommodityType.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<Commodity>.ToPagedList(commodityDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }


        public async Task<Commodity> GetCommodityById(int id)
        {
            var CommoditybyId= await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return CommoditybyId;
        }

        public async Task<string> UpdateCommodity(Commodity commodity)
        {
            commodity.LastModifiedBy = "Admin";
            commodity.LastModifiedOn = DateTime.Now;
            Update(commodity);
            string result = $"Commodity details of {commodity.Id} is updated successfully!";
            return result;
        }
    }
}
