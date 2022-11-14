using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IUOMRepository : IRepositoryBase<UOM>
    {
        Task<IEnumerable<UOM>> GetAllUOM();
        Task<UOM> GetUOMById(int id);
        Task<IEnumerable<UOM>> GetAllActiveUOM();
        Task<int?> CreateUOM(UOM uom);
        Task<string> UpdateUOM(UOM uom);
        Task<string> DeleteUOM(UOM uom);

    }
}
