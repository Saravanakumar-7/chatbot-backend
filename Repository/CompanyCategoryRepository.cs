using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyCategoryRepository : RepositoryBase<CompanyCategory>, ICompanyCategoryRepository
    {
        public CompanyCategoryRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateCompanyCategory(CompanyCategory companyCategory)
        {
            companyCategory.CreatedBy = "Admin";
            companyCategory.CreatedOn = DateTime.Now;
            companyCategory.Unit = "Bangalore";
            var result = await Create(companyCategory);

            return result.Id;
        }

        public async Task<string> DeleteCompanyCategory(CompanyCategory companyCategory)
        {
            Delete(companyCategory);
            string result = $"companyCategory details of {companyCategory.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CompanyCategory>> GetAllActiveCompanyCategory()
        {
            var allActiveCompanyCategories = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return allActiveCompanyCategories;
        }

        public async Task<IEnumerable<CompanyCategory>> GetAllCompanyCategory()
        {
            var allCompanyCategories = await FindAll().ToListAsync();

            return allCompanyCategories;
        }

        public async Task<CompanyCategory> GetCompanyCategoryById(int id)
        {

            var companyCategorybyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return companyCategorybyId;
        }

        public async Task<string> UpdateCompanyCategory(CompanyCategory companyCategory)
        {
            companyCategory.LastModifiedBy = "Admin";
            companyCategory.LastModifiedOn = DateTime.Now;
            Update(companyCategory);
            string result = $" companyCategory of Detail {companyCategory.Id} is updated successfully!";
            return result;
        }
    }
}

       