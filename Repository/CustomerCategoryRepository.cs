using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerCategoryRepository : RepositoryBase<CustomerCategory>, ICustomerCategoryRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public CustomerCategoryRepository(TipsMasterDbContext repositoryContext, IHttpContextAccessor httpContextAccessor) : base(repositoryContext)
        {
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        public async Task<int?> CreateCustomerCategory(CustomerCategory customerCategory)
        {
            customerCategory.CreatedBy = _createdBy;
            customerCategory.CreatedOn = DateTime.Now;
            customerCategory.Unit = _unitname;
            var result = await Create(customerCategory);

            return result.Id;
        }

        public async Task<string> DeleteCustomerCategory(CustomerCategory customerCategory)
        {
            Delete(customerCategory);
            string result = $"customerCategory details of {customerCategory.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CustomerCategory>> GetAllActiveCustomerCategory([FromQuery] SearchParames searchParams)
        {
            var customerCategoryDetails = FindAll()
                              .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CustomerCategoryName.Contains(searchParams.SearchValue) ||
                                     inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return customerCategoryDetails;
        }

        public async Task<IEnumerable<CustomerCategory>> GetAllCustomerCategory([FromQuery] SearchParames searchParams)
        {
            var customerCategoryDetails = FindAll().OrderByDescending(x => x.Id)
                               .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.CustomerCategoryName.Contains(searchParams.SearchValue) ||
                                      inv.Unit.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue))));
            return customerCategoryDetails;
        }

        public async Task<CustomerCategory> GetCustomerCategoryById(int id)
        {
            var customerCategorybyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return customerCategorybyId;
        }

        public async Task<string> UpdateCustomerCategory(CustomerCategory customerCategory)
        {
            customerCategory.LastModifiedBy = _createdBy;
            customerCategory.LastModifiedOn = DateTime.Now;
            Update(customerCategory);
            string result = $" customerCategory of Detail {customerCategory.Id} is updated successfully!";
            return result;
        }
    }
}
