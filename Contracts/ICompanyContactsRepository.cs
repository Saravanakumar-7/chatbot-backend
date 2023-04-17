using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface ICompanyContactsRepository
    {
        Task<IEnumerable<CompanyContacts>> GetAllCompanyContacts();
        Task<CompanyContacts> GetCompanyContactsById(int id);
        Task<IEnumerable<CompanyContacts>> GetAllActiveCompanyContacts();
        Task<int?> CreateCompanyContacts(CompanyContacts companyContacts);
        Task<string> UpdateCompanyContacts(CompanyContacts companyContacts);
        Task<string> DeleteCompanyContacts(CompanyContacts companyContacts);
    }
}
