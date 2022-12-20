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
    public interface IItemMasterRepository
    {
         Task<PagedList<ItemMaster>> GetAllItems(PagingParameter pagingParameter);

        Task<PagedList<ItemMaster>> GetAllFGItems(PagingParameter pagingParameter);
        Task<PagedList<ItemMaster>> GetAllSAItems(PagingParameter pagingParameter);
        Task<PagedList<ItemMaster>> GetAllFgSaItems(PagingParameter pagingParameter);
        Task<ItemMaster> GetItemById(int id);
        Task<IEnumerable<ItemMaster>> GetAllActiveItems();
        Task<long> CreateItem(ItemMaster itemMaster);
        Task<string> UpdateItem(ItemMaster itemMaster);
        Task<string> DeleteItem(ItemMaster itemMaster);
        Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveItemMasterIdNoList();
        Task<ItemMaster> GetItemByItemNumber(string shopOrderNo); 


    }
}
