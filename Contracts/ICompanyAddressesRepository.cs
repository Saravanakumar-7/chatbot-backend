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
        Task<IEnumerable<CompanyAddresses>> GetAllCompanyAddresses();
        Task<CompanyAddresses> GetCompanyAddressesById(int id);
        Task<IEnumerable<CompanyAddresses>> GetAllActiveCompanyAddresses();
        Task<int?> CreateCompanyAddresses(CompanyAddresses companyAddresses);
        Task<string> UpdateCompanyAddresses(CompanyAddresses companyAddresses);
        Task<string> DeleteCompanyAddresses(CompanyAddresses companyAddresses);
    }
}
