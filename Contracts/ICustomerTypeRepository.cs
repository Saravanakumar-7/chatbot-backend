using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICustomerTypeRepository
    {
        Task<IEnumerable<CustomerType>> GetAllCustomerTypes(SearchParames searchParams);
        Task<CustomerType> GetCustomerTypeById(int id);
        Task<IEnumerable<CustomerType>> GetAllActiveCustomerTypes(SearchParames searchParams);
        Task<int?> CreateCustomerType(CustomerType customerType);
        Task<string> UpdateCustomerType(CustomerType customerType);
        Task<string> DeleteCustomerType(CustomerType customerType);
    }
}
