using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public interface IBHKRepository : IRepositoryBase<BHK>
    {
        Task<IEnumerable<BHK>> GetAllBHK();
        Task<BHK> GetBHKById(int id);
        Task<IEnumerable<BHK>> GetAllActiveBHK();
        Task<int?> CreateBHK(BHK bHK);
        Task<string> UpdateBHK(BHK bHK);
        Task<string> DeleteBHK(BHK bHK);
    }
}