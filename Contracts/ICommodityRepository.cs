using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface ICommodityRepository : IRepositoryBase<Commodity>
    {
        Task<IEnumerable<Commodity>> GetAllCommodity(SearchParames searchParams);
        Task<Commodity> GetCommodityById(int id);
        Task<IEnumerable<Commodity>> GetAllActiveCommodity(SearchParames searchParams);
        Task<int?> CreateCommodity(Commodity commodity);
        Task<string> UpdateCommodity(Commodity commodity);
        Task<string> DeleteCommodity(Commodity commodity);
    }
}
