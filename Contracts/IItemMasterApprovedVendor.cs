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
        Task<PagedList<ItemMasterApprovedVendor>> GetAllItemMasterApprovedVendors(PagingParameter pagingParameter, SearchParames searchParames);
        Task<ItemMasterApprovedVendor> GetItemMasterApprovedVendorById(int id);
        Task<PagedList<ItemMasterApprovedVendor>> GetAllActiveItemMasterApprovedVendors(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateItemMasterApprovedVendor(ItemMasterApprovedVendor itemMasterApprovedVendor);
        Task<string> UpdateItemMasterApprovedVendor(ItemMasterApprovedVendor itemMasterApprovedVendor);
        Task<string> DeleteItemMasterApprovedVendor(ItemMasterApprovedVendor itemMasterApprovedVendor);
    }
}
