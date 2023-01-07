using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IItemMasterRouting
    {
        Task<IEnumerable<ItemMasterRouting>> GetAllItemMasterRoutings();
        Task<ItemMasterRouting> GetItemMasterRoutingById(int id);
        Task<IEnumerable<ItemMasterRouting>> GetAllActiveItemMasterRoutings();
        Task<int?> CreateItemMasterRouting(ItemMasterRouting itemMasterRouting);
        Task<string> UpdateItemMasterRouting(ItemMasterRouting itemMasterRouting);
        Task<string> DeleteItemMasterRouting(ItemMasterRouting itemMasterRouting);
    }
}
