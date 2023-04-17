using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface ICustomerShippingAddressesRepository:IRepositoryBase<CustomerShippingAddresses>
    {

        Task<IEnumerable<CustomerShippingAddresses>> GetAllCustomerShippingAddresses();
        Task<CustomerShippingAddresses> GetCustomerShippingAddressesById(int id);
        Task<IEnumerable<CustomerShippingAddresses>> GetAllActiveCustomerShippingAddresses();
        Task<int?> CreateCustomerShippingAddresses(CustomerShippingAddresses customerShippingAddresses);
        Task<string> UpdateCustomerShippingAddresses(CustomerShippingAddresses customerShippingAddresses);
        Task<string> DeleteCustomerShippingAddresses(CustomerShippingAddresses customerShippingAddresses);
    }
}

