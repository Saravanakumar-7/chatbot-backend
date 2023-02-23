using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICustomerMasterHeadCountingRepository
    {
        Task<IEnumerable<CustomerMasterHeadCounting>> GetAllCustomerMasterHeadCountings();
        Task<CustomerMasterHeadCounting> GetCustomerMasterHeadCountingById(int id);
        Task<IEnumerable<CustomerMasterHeadCounting>> GetAllActiveCustomerMasterHeadCountings();
        Task<int?> CreateCustomerMasterHeadCounting(CustomerMasterHeadCounting customerMasterHeadCounting);
        Task<string> UpdateCustomerMasterHeadCounting(CustomerMasterHeadCounting customerMasterHeadCounting);
        Task<string> DeleteCustomerMasterHeadCounting(CustomerMasterHeadCounting customerMasterHeadCounting);
    }
}
