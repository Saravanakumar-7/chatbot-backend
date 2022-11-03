using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICustomerTypeRepository : IRepositoryBase<CustomerType>
    {
        Task<IEnumerable<CustomerType>> GetAllCustomerTypes();
        Task<CustomerType> GetCustomerTypeById(int id);
        Task<IEnumerable<CustomerType>> GetAllActiveCustomerTypes();
        Task<int?> CreateCustomerType(CustomerType customerType);
        Task<string> ActivateCustomerType(int id);
        Task<string> DeactivateCustomerType(int id);
    }
}
