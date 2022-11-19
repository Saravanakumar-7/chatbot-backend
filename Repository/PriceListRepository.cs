using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PriceListRepository : RepositoryBase<PriceList>, IPriceListRepository
    {
        public PriceListRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreatePriceList(PriceList priceList)
        {
            priceList.CreatedBy = "Admin";
            priceList.CreatedOn = DateTime.Now;
            var result = await Create(priceList);
            priceList.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeletePriceList(PriceList priceList)
        {
            Delete(priceList);
            string result = $"PriceList details of {priceList.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<PriceList>> GetAllActivePriceLists()
        {

            var priceListList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return priceListList;
        }

        public async Task<IEnumerable<PriceList>> GetAllPriceLists()
        {

            var priceListList = await FindAll().ToListAsync();

            return priceListList;
        }

        public async Task<PriceList> GetPriceListById(int id)
        {

            var priceList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return priceList;
        }

        public async Task<string> UpdatePriceList(PriceList priceList)
        {
            priceList.LastModifiedBy = "Admin";
            priceList.LastModifiedOn = DateTime.Now;
            Update(priceList);
            string result = $"PriceList details of {priceList.Id} is updated successfully!";
            return result;
        }
    }
}
