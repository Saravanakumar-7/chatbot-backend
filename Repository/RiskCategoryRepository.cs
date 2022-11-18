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
    public class RiskCategoryRepository : RepositoryBase<RiskCategory>, IRiskCategoryRepository
    {
        public RiskCategoryRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateRiskCategory(RiskCategory riskCategory)
        {
            riskCategory.CreatedBy = "Admin";
            riskCategory.CreatedOn = DateTime.Now;
            var result = await Create(riskCategory);
            riskCategory.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteRiskCategory(RiskCategory riskCategory)
        {
            Delete(riskCategory);
            string result = $"RiskCategory details of {riskCategory.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<RiskCategory>> GetAllActiveRiskCategory()
        {
            var riskCategoryList = await FindByCondition(x => x.ActiveStatus == true).ToListAsync();
            return riskCategoryList;
        }

        public async Task<IEnumerable<RiskCategory>> GetAllRiskCategory()
        {

            var riskCategoryList = await FindAll().ToListAsync();
            return riskCategoryList;
        }

        public async Task<RiskCategory> GetRiskCategoryById(int id)
        {
            var riskCategoryList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            return riskCategoryList;
        }

        public async Task<string> UpdateRiskCategory(RiskCategory riskCategory)
        {
            riskCategory.LastModifiedBy = "Admin";
            riskCategory.LastModifiedOn = DateTime.Now;
            Update(riskCategory);
            string result = $"RiskCategory details of {riskCategory.Id} is updated successfully!";
            return result;
        }
    }
}
