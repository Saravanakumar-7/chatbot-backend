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
        Task<PagedList<ItemMaster>> GetAllItemMasters(PagingParameter pagingParameter);
        Task<PagedList<ItemMaster>> GetAllFGItems(PagingParameter pagingParameter);
        Task<PagedList<ItemMaster>> GetAllSAItems(PagingParameter pagingParameter);
        Task<PagedList<ItemMaster>> GetAllFgSaItems(PagingParameter pagingParameter);
        Task<ItemMaster> GetItemMasterById(int id);
        Task<IEnumerable<ItemMaster>> GetAllActiveItemMasters();
        Task<long> CreateItemMaster(ItemMaster itemMaster);
        Task<string> UpdateItemMaster(ItemMaster itemMaster);
        Task<string> DeleteItemMaster(ItemMaster itemMaster);
        Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveItemMasterIdNoList();
        Task<ItemMaster> GetItemMasterByItemNumber(string shopOrderNo); 


    }
}
