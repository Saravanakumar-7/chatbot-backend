using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICustomerCategoryRepository : IRepositoryBase<CustomerCategory>
    {
        Task<IEnumerable<CustomerCategory>> GetAllCustomerCategory(SearchParames searchParams);
        Task<CustomerCategory> GetCustomerCategoryById(int id);
        Task<IEnumerable<CustomerCategory>> GetAllActiveCustomerCategory(SearchParames searchParams);
        Task<int?> CreateCustomerCategory(CustomerCategory customerCategory);
        Task<string> UpdateCustomerCategory(CustomerCategory customerCategory);
        Task<string> DeleteCustomerCategory(CustomerCategory customerCategory);
    }
}
