using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyMasterHeadCountingRepository
    {
        Task<IEnumerable<CompanyMasterHeadCounting>> GetAllCompanyMasterHeadCountings();
        Task<CompanyMasterHeadCounting> GetCompanyMasterHeadCountingById(int id);
        Task<IEnumerable<CompanyMasterHeadCounting>> GetAllActiveCompanyMasterHeadCountings();
        Task<int?> CreateCompanyMasterHeadCounting(CompanyMasterHeadCounting companyMasterHeadCounting);
        Task<string> UpdateCompanyMasterHeadCounting(CompanyMasterHeadCounting companyMasterHeadCounting);
        Task<string> DeleteCompanyMasterHeadCounting(CompanyMasterHeadCounting companyMasterHeadCounting);
    }
}
