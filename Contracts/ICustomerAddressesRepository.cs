using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface ICustomerAddressesRepository : IRepositoryBase<CustomerAddresses>
    {
        Task<IEnumerable<CustomerAddresses>> GetAllCustomerAddresses();
        Task<CustomerAddresses> GetCustomerAddressesById(int id);
        Task<IEnumerable<CustomerAddresses>> GetAllActiveCustomerAddresses();
        Task<int?> CreateCustomerAddresses(CustomerAddresses customerAddresses);
        Task<string> UpdateCustomerAddresses(CustomerAddresses customerAddresses);
        Task<string> DeleteCustomerAddresses(CustomerAddresses customerAddresses);
    }
}
