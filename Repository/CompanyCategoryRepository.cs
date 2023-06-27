using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyCategoryRepository : RepositoryBase<CompanyCategory>, ICompanyCategoryRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public CompanyCategoryRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";

        }

        public async Task<int?> CreateCompanyCategory(CompanyCategory companyCategory)
        {
            companyCategory.CreatedBy = _createdBy;
            companyCategory.CreatedOn = DateTime.Now;
            companyCategory.Unit = _unitname;
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
            companyCategory.LastModifiedBy = _createdBy;
            companyCategory.LastModifiedOn = DateTime.Now;
            Update(companyCategory);
            string result = $" companyCategory of Detail {companyCategory.Id} is updated successfully!";
            return result;
        }
    }
}

       