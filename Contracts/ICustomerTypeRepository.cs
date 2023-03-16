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
        Task<PagedList<CustomerType>> GetAllCustomerTypes(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CustomerType> GetCustomerTypeById(int id);
        Task<PagedList<CustomerType>> GetAllActiveCustomerTypes(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCustomerType(CustomerType customerType);
        Task<string> UpdateCustomerType(CustomerType customerType);
        Task<string> DeleteCustomerType(CustomerType customerType);
    }
}
