using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPriceListRepository
    {
        Task<IEnumerable<PriceList>> GetAllPriceLists();
        Task<PriceList> GetPriceListById(int id);
        Task<IEnumerable<PriceList>> GetAllActivePriceLists();
        Task<int?> CreatePriceList(PriceList priceList);
        Task<string> UpdatePriceList(PriceList priceList);
        Task<string> DeletePriceList(PriceList priceList);
    }
}
