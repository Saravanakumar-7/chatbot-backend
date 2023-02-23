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
    internal class CostingMethodRepository : RepositoryBase<CostingMethod>, ICostingMethodRepository
    {
        public CostingMethodRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateCostingMethod(CostingMethod costingMethod)
        {
            costingMethod.CreatedBy = "Admin";
            costingMethod.CreatedOn = DateTime.Now;
            costingMethod.Unit = "Bangalore";
            var result = await Create(costingMethod);
            
            return result.Id;
        }

        public async Task<string> DeleteCostingMethod(CostingMethod costingMethod)
        {
            Delete(costingMethod);
            string result = $"Costing Method details of {costingMethod.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CostingMethod>> GetAllActiveCostingMethods()
        {
            var AllActiveCostingmethods = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveCostingmethods;
        }

        public async Task<IEnumerable<CostingMethod>> GetAllCostingMethods()
        {
            var GetallCostingMethods = await FindAll().ToListAsync();

            return GetallCostingMethods;
        }

        public async Task<CostingMethod> GetCostingMethodById(int id)
        {

            var CostingMethodbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return CostingMethodbyId;
        }

        public async Task<string> UpdateCostingMethod(CostingMethod costingMethod)
        {

            costingMethod.LastModifiedBy = "Admin";
            costingMethod.LastModifiedOn = DateTime.Now;
            Update(costingMethod);
            string result = $"Costing Method details of {costingMethod.Id} is updated successfully!";
            return result;
        }
    }
}
