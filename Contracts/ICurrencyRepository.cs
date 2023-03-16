using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICurrencyRepository : IRepositoryBase<Currency>
    {
        Task<PagedList<Currency>> GetAllCurrency(PagingParameter pagingParameter, SearchParames searchParams);
        Task<Currency> GetCurrencyById(int id);
        Task<PagedList<Currency>> GetAllActiveCurrency(PagingParameter pagingParameter, SearchParames searchParams);
        Task<int?> CreateCurrency(Currency currency);
        Task<string> UpdateCurrency(Currency currency);
        Task<string> DeleteCurrency(Currency currency);
    }
}
