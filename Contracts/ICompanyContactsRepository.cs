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
        Task<PagedList<CompanyContacts>> GetAllCompanyContacts(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CompanyContacts> GetCompanyContactsById(int id);
        Task<PagedList<CompanyContacts>> GetAllActiveCompanyContacts(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCompanyContacts(CompanyContacts companyContacts);
        Task<string> UpdateCompanyContacts(CompanyContacts companyContacts);
        Task<string> DeleteCompanyContacts(CompanyContacts companyContacts);
    }
}
