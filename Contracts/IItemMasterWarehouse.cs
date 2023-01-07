using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IItemMasterWarehouse
    {
        Task<IEnumerable<ItemMasterWarehouse>> GetAllItemMasterWarehouses();
        Task<ItemMasterWarehouse> GetItemMasterWarehouseById(int id);
        Task<IEnumerable<ItemMasterWarehouse>> GetAllActiveItemMasterWarehouses();
        Task<int?> CreateItemMasterWarehouse(ItemMasterWarehouse itemMasterWarehouse);
        Task<string> UpdateItemMasterWarehouse(ItemMasterWarehouse itemMasterWarehouse);
        Task<string> DeleteItemMasterWarehouse(ItemMasterWarehouse itemMasterWarehouse);
    }
}
