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
        Task<IEnumerable<TypeOfCompany>> GetAllTypeOfCompanies();
        Task<TypeOfCompany> GetTypeOfCompanyById(int id);
        Task<IEnumerable<TypeOfCompany>> GetAllActiveTypeofCompanies();
        Task<int?> CreateTypeOfCompany(TypeOfCompany typeOfCompany);
        Task<string> UpdateTypeOfCompany(TypeOfCompany typeOfCompany);
        Task<string> DeleteTypeOfCompany(TypeOfCompany typeOfCompany);
    }
}
