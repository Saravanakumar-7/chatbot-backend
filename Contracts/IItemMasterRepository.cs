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
        Task<IEnumerable<ItemMaster>> GetAllFGItems();
        Task<IEnumerable<ItemMaster>> GetAllSAItems();
        Task<IEnumerable<ItemMaster>> GetAllFgSaItems();
        Task<IEnumerable<ItemMaster>> GetAllFgSaFruItems();
        Task<IEnumerable<ItemMaster>> GetAllSAPurchasePartItems();
        Task<IEnumerable<GetDownloadUrlDtos>> GetDownloadUrlDetails(long itemMasterId);
        Task<ItemMaster> GetItemMasterById(int id);
        Task<IEnumerable<ItemMaster>> GetAllActiveItemMasters();
        Task<long> CreateItemMaster(ItemMaster itemMaster);
        Task<string> UpdateItemMaster(ItemMaster itemMaster);
        Task<string> DeleteItemMaster(ItemMaster itemMaster);
        Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveItemMasterIdNoList();
        Task<IEnumerable<ItemMasterIdNoListDto>> GetAllItemMasterIdNoList();
        Task<List<ItemWithPartTypeDto>> GetItemPartTypeByItemNo(List<string> ItemNumberList);
        Task<ItemMaster> GetItemMasterByItemNumber(string shopOrderNo);
        Task<List<ItemMasterMtrPartNoDto>> GetItemMasterByPartNo(string partNumber);
        Task<IEnumerable<ItemMaster>> SearchItemMasterDate(SearchDateParamess searchDateParam);
        Task<IEnumerable<ItemMaster>> GetAllItemMasterWithItems(ItemMasterSearchDto itemMasterSearch);
        Task<IEnumerable<ItemMaster>> SearchItemMaster(SearchParames searchParames);
        Task<IEnumerable<FileUpload>> GetAllItemMasterFileUploadList(string itemNumber);
        Task<IEnumerable<ItemMasterIdNoListDto>> GetAllFgTgItemMasterItemNoList();
        Task<IEnumerable<ItemNoListDtos>> GetAllPurchasePartItemNoList();

    }
}
