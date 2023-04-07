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
        Task<PagedList<ItemMaster>> GetAllItemMasters(PagingParameter pagingParameter, SearchParames searchParams);
        Task<PagedList<ItemMaster>> GetAllFGItems(PagingParameter pagingParameter, SearchParames searchParams);
        Task<PagedList<ItemMaster>> GetAllSAItems(PagingParameter pagingParameter, SearchParames searchParams);
        Task<PagedList<ItemMaster>> GetAllFgSaItems(PagingParameter pagingParameter, SearchParames searchParams);
        Task<IEnumerable<ItemMaster>> GetAllFgSaFruItems();
        Task<PagedList<ItemMaster>> GetAllSAPurchasePartItems(PagingParameter pagingParameter, SearchParames searchParams);
        Task<ItemMaster> GetItemMasterById(int id);
        Task<PagedList<ItemMaster>> GetAllActiveItemMasters(PagingParameter pagingParameter, SearchParames searchParams);
        Task<long> CreateItemMaster(ItemMaster itemMaster);
        Task<string> UpdateItemMaster(ItemMaster itemMaster);
        Task<string> DeleteItemMaster(ItemMaster itemMaster);
        Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveItemMasterIdNoList();
        Task<ItemMaster> GetItemMasterByItemNumber(string shopOrderNo); 


    }
}
