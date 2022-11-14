using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface ICommodityRepository : IRepositoryBase<Commodity>
    {
        Task<IEnumerable<Commodity>> GetAllCommodity();
        Task<Commodity> GetCommodityById(int id);
        Task<IEnumerable<Commodity>> GetAllActiveCommodity();
        Task<int?> CreateCommodity(Commodity commodity);
        Task<string> UpdateCommodity(Commodity commodity);
        Task<string> DeleteCommodity(Commodity commodity);
    }
}
