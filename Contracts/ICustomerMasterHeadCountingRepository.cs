using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICustomerMasterHeadCountingRepository
    {
        Task<PagedList<CustomerMasterHeadCounting>> GetAllCustomerMasterHeadCountings(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CustomerMasterHeadCounting> GetCustomerMasterHeadCountingById(int id);
        Task<PagedList<CustomerMasterHeadCounting>> GetAllActiveCustomerMasterHeadCountings(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCustomerMasterHeadCounting(CustomerMasterHeadCounting customerMasterHeadCounting);
        Task<string> UpdateCustomerMasterHeadCounting(CustomerMasterHeadCounting customerMasterHeadCounting);
        Task<string> DeleteCustomerMasterHeadCounting(CustomerMasterHeadCounting customerMasterHeadCounting);
    }
}
