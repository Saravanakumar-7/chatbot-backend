using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface ICompanyBankingRepository
    {
        Task<PagedList<CompanyBanking>> GetAllCompanyBankings(PagingParameter pagingParameter, SearchParames searchParams);
        Task<CompanyBanking> GetCompanyBankingById(int id);
        Task<PagedList<CompanyBanking>> GetAllActiveCompanyBankings(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCompanyBanking(CompanyBanking CompanyBanking);
        Task<string> UpdateCompanyBanking(CompanyBanking CompanyBanking);
        Task<string> DeleteCompanyBanking(CompanyBanking CompanyBanking);
    }
}
