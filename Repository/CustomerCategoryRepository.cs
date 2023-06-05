using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerCategoryRepository : RepositoryBase<CustomerCategory>, ICustomerCategoryRepository
    {
        public CustomerCategoryRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<int?> CreateCustomerCategory(CustomerCategory customerCategory)
        {
            customerCategory.CreatedBy = "Admin";
            customerCategory.CreatedOn = DateTime.Now;
            customerCategory.Unit = "Bangalore";
            var result = await Create(customerCategory);

            return result.Id;
        }

        public async Task<string> DeleteCustomerCategory(CustomerCategory customerCategory)
        {
            Delete(customerCategory);
            string result = $"customerCategory details of {customerCategory.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<CustomerCategory>> GetAllActiveCustomerCategory()
        {
            var allActivecustomerCategory = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return allActivecustomerCategory;
        }

        public async Task<IEnumerable<CustomerCategory>> GetAllCustomerCategory()
        {
            var allCustomerCategories = await FindAll().ToListAsync();

            return allCustomerCategories;
        }

        public async Task<CustomerCategory> GetCustomerCategoryById(int id)
        {
            var customerCategorybyId = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return customerCategorybyId;
        }

        public async Task<string> UpdateCustomerCategory(CustomerCategory customerCategory)
        {
            customerCategory.LastModifiedBy = "Admin";
            customerCategory.LastModifiedOn = DateTime.Now;
            Update(customerCategory);
            string result = $" customerCategory of Detail {customerCategory.Id} is updated successfully!";
            return result;
        }
    }
}
