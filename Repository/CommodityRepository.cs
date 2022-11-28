using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
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
            var result = await Create(commodity);
            return result.Id;
        }

        public async Task<string> DeleteCommodity(Commodity commodity)
        {
            Delete(commodity);
            string result = $"Commodity details of {commodity.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<Commodity>> GetAllActiveCommodity()
        {
            var commodityList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return commodityList;
        }

        public async Task<IEnumerable<Commodity>> GetAllCommodity()
        {
            var commodityList = await FindAll().ToListAsync();
            return commodityList;
        }

        public async Task<Commodity> GetCommodityById(int id)
        {
            var commodityList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return commodityList;
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
