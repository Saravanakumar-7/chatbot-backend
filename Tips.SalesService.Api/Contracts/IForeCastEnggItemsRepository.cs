using Entities;
using Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IForeCastEnggItemsRepository
    {
        Task<IEnumerable<ForeCastEnggItems>> GetAllForeCastEnggItems();
        Task<ForeCastEnggItems> GetForeCastEnggItemsById(int id);
        Task<int?> CreateForeCastEnggItems(ForeCastEnggItems foreCastEnggItems);
        Task<string> UpdateForeCastEnggItems(ForeCastEnggItems foreCastEnggItems);
        Task<string> DeleteForeCastEnggItems(ForeCastEnggItems foreCastEnggItems);
    }
}
