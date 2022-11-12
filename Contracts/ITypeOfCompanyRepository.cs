using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITypeOfCompanyRepository : IRepositoryBase<TypeOfCompany>
    {
        Task<IEnumerable<TypeOfCompany>> GetAllCustomerTypes();
        Task<TypeOfCompany> GetCustomerTypeById(int id);
        Task<IEnumerable<TypeOfCompany>> GetAllActiveCustomerTypes();
        Task<int?> CreateCustomerType(TypeOfCompany typeOfCompany);
        Task<string> UpdateCustomerType(TypeOfCompany typeOfCompany);
        Task<string> DeleteCustomerType(TypeOfCompany typeOfCompany);
    }
}
