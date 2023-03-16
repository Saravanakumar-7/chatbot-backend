using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<PagedList<CostingMethod>> GetAllActiveCostingMethods([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var costCenterDetails = FindAll()
                      .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CostingMethodName.Contains(searchParams.SearchValue) ||
                      inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<CostingMethod>.ToPagedList(costCenterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<CostingMethod>> GetAllCostingMethods([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var costCenterDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CostingMethodName.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<CostingMethod>.ToPagedList(costCenterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
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
