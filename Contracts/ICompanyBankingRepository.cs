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
        Task<IEnumerable<CompanyBanking>> GetAllCompanyBankings();
        Task<CompanyBanking> GetCompanyBankingById(int id);
        Task<IEnumerable<CompanyBanking>> GetAllActiveCompanyBankings();
        Task<int?> CreateCompanyBanking(CompanyBanking CompanyBanking);
        Task<string> UpdateCompanyBanking(CompanyBanking CompanyBanking);
        Task<string> DeleteCompanyBanking(CompanyBanking CompanyBanking);
    }
}
