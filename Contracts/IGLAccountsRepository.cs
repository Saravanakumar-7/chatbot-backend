using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IGLAccountsRepository
    {
        Task<IEnumerable<GlAccounts>> GetAllGLAccounts(SearchParames searchParams);
        Task<GlAccounts> GetGLAccountById(int id);
        Task<IEnumerable<GlAccounts>> GetAllActiveGLAccounts(SearchParames searchParams);
        Task<int?> CreateGLAccount(GlAccounts glAccount);
        Task<string> UpdateGLAccount(GlAccounts glAccount);
        Task<string> DeleteGLAccount(GlAccounts glAccount);
    }
}
