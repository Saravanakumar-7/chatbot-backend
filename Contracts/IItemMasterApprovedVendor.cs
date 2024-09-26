using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface IItemMasterApprovedVendor
    {
        Task<IEnumerable<ItemMasterApprovedVendor>> GetAllItemMasterApprovedVendors();
        Task<ItemMasterApprovedVendor> GetItemMasterApprovedVendorById(int id);
        Task<IEnumerable<ItemMasterApprovedVendor>> GetAllActiveItemMasterApprovedVendors();
        Task<int?> CreateItemMasterApprovedVendor(ItemMasterApprovedVendor itemMasterApprovedVendor);
        Task<string> UpdateItemMasterApprovedVendor(ItemMasterApprovedVendor itemMasterApprovedVendor);
        Task<string> DeleteItemMasterApprovedVendor(ItemMasterApprovedVendor itemMasterApprovedVendor);
    }
}
