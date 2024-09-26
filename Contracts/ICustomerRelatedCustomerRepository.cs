using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICustomerRelatedCustomerRepository
    {
        Task<IEnumerable<CustomerRelatedCustomer>> GetAllCustomerRelatedCustomer();
        Task<CustomerRelatedCustomer> GetCustomerRelatedCustomerById(int id);
        Task<int?> CreateCustomerRelatedCustomer(CustomerRelatedCustomer customerRelatedCustomer);
        Task<string> UpdateCustomerRelatedCustomer(CustomerRelatedCustomer customerRelatedCustomer);
        Task<string> DeleteCustomerRelatedCustomer(CustomerRelatedCustomer customerRelatedCustomer);
    }
}
