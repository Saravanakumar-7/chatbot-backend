using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IItemMasterApprovedVendor
    {
        Task<IEnumerable<ItemMasterApprovedVendor>> GetAllItemMasterApprovedVendor();
        Task<ItemMasterApprovedVendor> GetItemMasterApprovedVendorById(int id);
        Task<IEnumerable<ItemMasterApprovedVendor>> GetAllActiveItemMasterApprovedVendor();
        Task<int?> CreateItemMasterApprovedVendor(ItemMasterApprovedVendor itemMasterApprovedVendor);
        Task<string> UpdateItemMasterApprovedVendor(ItemMasterApprovedVendor itemMasterApprovedVendor);
        Task<string> DeleteItemMasterApprovedVendor(ItemMasterApprovedVendor itemMasterApprovedVendor);
    }
}
