using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IItemMasterRepository
    {
        Task<IEnumerable<ItemMaster>> GetAllItems();
        Task<ItemMaster> GetItemById(int id);
        Task<IEnumerable<ItemMaster>> GetAllActiveItems();
        Task<long> CreateItem(ItemMaster itemMaster);
        Task<string> UpdateItem(ItemMaster itemMaster);
        Task<string> DeleteItem(ItemMaster itemMaster);
         
    }
}
