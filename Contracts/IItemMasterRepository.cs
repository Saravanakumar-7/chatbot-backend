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
        public Task<List<ItemMaster>> GetAllItems();
        public Task<ItemMaster> GetItemById(int id);
        public Task<List<ItemMaster>> GetAllActiveItems();
        public Task<long> CreateItem(ItemMaster itemMaster);
        Task<string> UpdateItem(ItemMaster itemMaster);
        public  Task<string> DeleteItem(ItemMaster itemMaster);

        //public Task<ItemMaster> GetItemByDescription(string description);
        //public Task<ItemMaster> GetAllInActiveItems();
        //public Task<ItemMaster> GetAllObsoleteItems();
        //public Task<string> ActivateItem(int id);
        //public Task<string> DeactivateItem(int id);
    }
}
