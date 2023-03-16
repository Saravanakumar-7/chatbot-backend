using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBankRepository
    {
        Task<PagedList<Bank>> GetAllBank(PagingParameter pagingParameter, SearchParames searchParams);
        Task<Bank> GetBankById(int id);
        Task<PagedList<Bank>> GetAllActiveBank(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateBank(Bank bank);
        Task<string> UpdateBank(Bank bank);
        Task<string> DeleteBank(Bank bank);
    }
}
