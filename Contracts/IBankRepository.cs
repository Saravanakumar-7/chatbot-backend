using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBankRepository
    {
        Task<IEnumerable<Bank>> GetAllBank();
        Task<Bank> GetBankById(int id);
        Task<IEnumerable<Bank>> GetAllActiveBank();
        Task<int?> CreateBank(Bank bank);
        Task<string> UpdateBank(Bank bank);
        Task<string> DeleteBank(Bank bank);
    }
}
