using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface ICompanyAddressesRepository
    {
        Task<PagedList<CompanyAddresses>> GetAllCompanyAddresses(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CompanyAddresses> GetCompanyAddressesById(int id);
        Task<PagedList<CompanyAddresses>> GetAllActiveCompanyAddresses(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCompanyAddresses(CompanyAddresses companyAddresses);
        Task<string> UpdateCompanyAddresses(CompanyAddresses companyAddresses);
        Task<string> DeleteCompanyAddresses(CompanyAddresses companyAddresses);
    }
}
