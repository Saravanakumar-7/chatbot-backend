using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface ICustomerContactsRepository : IRepositoryBase<CustomerContacts>
    {
        Task<PagedList<CustomerContacts>> GetAllCustomerContacts(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CustomerContacts> GetCustomerContactsById(int id);
        Task<PagedList<CustomerContacts>> GetAllActiveCustomerContacts(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCustomerContacts(CustomerContacts customerContacts);
        Task<string> UpdateCustomerContacts(CustomerContacts customerContacts);
        Task<string> DeleteCustomerContacts(CustomerContacts customerContacts);
    }
}
