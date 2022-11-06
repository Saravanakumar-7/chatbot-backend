using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICurrencyRepository : IRepositoryBase<Currency>
    {
        Task<IEnumerable<Currency>> GetAllCurrency();
        Task<Currency> GetCurrencyById(int id);
        Task<IEnumerable<Currency>> GetAllActiveCurrency();
        Task<int?> CreateCurrency(Currency currency);
        Task<string> UpdateCurrency(Currency currency);
        Task<string> DeleteCurrency(Currency currency);
    }
}
