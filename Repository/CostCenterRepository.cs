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
    public class CostCenterRepository : RepositoryBase<CostCenter>, ICostCenterRepository
    {
        public CostCenterRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateCostCenter(CostCenter costCenter)
        {
            costCenter.CreatedBy = "Admin";
            costCenter.CreatedOn = DateTime.Now;
            costCenter.Unit = "Bangalore";
            var result = await Create(costCenter);
            
            return result.Id;
        }

        public async Task<string> DeleteCostCenter(CostCenter costCenter)
        {

            Delete(costCenter);
            string result = $"Costcenter details of {costCenter.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<CostCenter>> GetAllActiveCostCenters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            var costCenterDetails = FindAll()
                                .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CostCenterName.Contains(searchParams.SearchValue) ||
                                inv.Description.Contains(searchParams.SearchValue))));
            return PagedList<CostCenter>.ToPagedList(costCenterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<PagedList<CostCenter>> GetAllCostCenters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {

            var costCenterDetails = FindAll().OrderByDescending(x => x.Id)
            .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CostCenterName.Contains(searchParams.SearchValue) ||
            inv.Description.Contains(searchParams.SearchValue))));

            return PagedList<CostCenter>.ToPagedList(costCenterDetails, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<CostCenter> GetCostCenterById(int id)
        {

            var CostCenterbyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return CostCenterbyId;
        }

        public async Task<string> UpdateCostCenter(CostCenter costCenter)
        {
            costCenter.LastModifiedBy = "Admin";
            costCenter.LastModifiedOn = DateTime.Now;
            Update(costCenter);
            string result = $"CostCenter details of {costCenter.Id} is updated successfully!";
            return result;
        }
    }
}
