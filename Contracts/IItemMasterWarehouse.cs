using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Helper;

namespace Contracts
{
    public interface IItemMasterWarehouse
    {
        Task<PagedList<ItemMasterWarehouse>> GetAllItemMasterWarehouses(PagingParameter pagingParameter, SearchParames searchParames);
        Task<ItemMasterWarehouse> GetItemMasterWarehouseById(int id);
        Task<PagedList<ItemMasterWarehouse>> GetAllActiveItemMasterWarehouses(PagingParameter pagingParameter, SearchParames searchParames);
        Task<int?> CreateItemMasterWarehouse(ItemMasterWarehouse itemMasterWarehouse);
        Task<string> UpdateItemMasterWarehouse(ItemMasterWarehouse itemMasterWarehouse);
        Task<string> DeleteItemMasterWarehouse(ItemMasterWarehouse itemMasterWarehouse);
    }
}
