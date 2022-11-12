using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface ICustomerContactsRepository : IRepositoryBase<CustomerContacts>
    {
        Task<IEnumerable<CustomerContacts>> GetAllCustomerContacts();
        Task<CustomerContacts> GetCustomerContactsById(int id);
        Task<IEnumerable<CustomerContacts>> GetAllActiveCustomerContacts();
        Task<int?> CreateCustomerContacts(CustomerContacts customerContacts);
        Task<string> UpdateCustomerContacts(CustomerContacts customerContacts);
        Task<string> DeleteCustomerContacts(CustomerContacts customerContacts);
    }
}
