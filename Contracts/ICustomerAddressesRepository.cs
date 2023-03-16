using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface ICustomerAddressesRepository : IRepositoryBase<CustomerAddresses>
    {
        Task<PagedList<CustomerAddresses>> GetAllCustomerAddresses(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CustomerAddresses> GetCustomerAddressesById(int id);
        Task<PagedList<CustomerAddresses>> GetAllActiveCustomerAddresses(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCustomerAddresses(CustomerAddresses customerAddresses);
        Task<string> UpdateCustomerAddresses(CustomerAddresses customerAddresses);
        Task<string> DeleteCustomerAddresses(CustomerAddresses customerAddresses);
    }
}
