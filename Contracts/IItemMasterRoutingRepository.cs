using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;
using Entities.Helper;

namespace Contracts
{
    public interface IItemMasterRoutingRepository
    {
        Task<IEnumerable<ItemMasterRouting>> GetAllItemMasterRoutings();
        Task<ItemMasterRouting> GetItemMasterRoutingById(int id);
        Task<IEnumerable<ItemMasterRouting>> GetAllActiveItemMasterRoutings();
        Task<ItemMasterRouting> GetAllItemsProcessList(int id); 
        Task<int?> CreateItemMasterRouting(ItemMasterRouting itemMasterRouting);
        Task<string> UpdateItemMasterRouting(ItemMasterRouting itemMasterRouting);
        Task<string> DeleteItemMasterRouting(ItemMasterRouting itemMasterRouting);
    }
}
