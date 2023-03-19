using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyMasterHeadCountingRepository
    {
        Task<PagedList<CompanyMasterHeadCounting>> GetAllCompanyMasterHeadCountings(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CompanyMasterHeadCounting> GetCompanyMasterHeadCountingById(int id);
        Task<PagedList<CompanyMasterHeadCounting>> GetAllActiveCompanyMasterHeadCountings(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCompanyMasterHeadCounting(CompanyMasterHeadCounting companyMasterHeadCounting);
        Task<string> UpdateCompanyMasterHeadCounting(CompanyMasterHeadCounting companyMasterHeadCounting);
        Task<string> DeleteCompanyMasterHeadCounting(CompanyMasterHeadCounting companyMasterHeadCounting);
    }
}
