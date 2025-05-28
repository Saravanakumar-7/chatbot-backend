using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Entities.Helper;

namespace Contracts
{
    public interface IItemMasterRepository
    {
        Task<PagedList<ItemMaster>> GetAllItemMasters(PagingParameter pagingParameter, SearchParames searchParams);
        Task<IEnumerable<ItemMaster>> GetAllFGItems();
        Task<IEnumerable<ItemMaster>> GetAllSAItems();
        Task<IEnumerable<ItemMaster>> GetAllFgSaItems();
        Task<IEnumerable<ItemMaster>> GetAllKITItems();
        Task<IEnumerable<ItemMaster>> GetAllFgSaFruItems();
        Task<IEnumerable<ItemMaster>> GetAllSAPurchasePartItems();
        Task<IEnumerable<ItemMaster>> GetAllKitComponentItemList();
        Task<GetDownloadUrlDtos> GetDownloadUrlDetails(long itemMasterId);
        Task<ItemMaster> GetItemMasterById(int id);
        Task<IEnumerable<ItemMaster>> GetAllActiveItemMasters();
        Task<long> CreateItemMaster(ItemMaster itemMaster);
        Task<string> UpdateItemMaster(ItemMaster itemMaster);
        Task<string> DeleteItemMaster(ItemMaster itemMaster);
        Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveItemMasterIdNoList();
        Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveAndInActiveItemMasterIdNoList();
        Task<IEnumerable<ItemMasterIdNoListDto>> GetAllItemMasterIdNoList();
        Task<List<ItemWithPartTypeDto>> GetItemPartTypeByItemNo(List<string> ItemNumberList);
        Task<List<ItemMaster>> GetItemDetailsByItemNumberList(List<string> ItemNumbers);
        Task<ItemMaster> GetItemMasterByItemNumber(string ItemNumber);
        Task<ItemMaster> GetItemMasterByItemNumberAndPartType(string ItemNumber, PartType partType);
        Task<bool> CheckItemMasterExists(string itemnumber);
        Task<List<ItemMasterMtrPartNoDto>> GetItemMasterByPartNo(string partNumber);
        Task<IEnumerable<ItemMaster>> SearchItemMasterDate(SearchDateParamess searchDateParam);
        Task<IEnumerable<ItemMaster>> GetAllItemMasterWithItems(ItemMasterSearchDto itemMasterSearch);
        Task<IEnumerable<ItemMaster>> SearchItemMaster(SearchParames searchParames);
        Task<IEnumerable<FileUpload>> GetAllItemMasterFileUploadList(string itemNumber);
        Task<IEnumerable<ItemNoListDtos>> GetAllActiveItemNumberListbyPartType(PartType partType);
        Task<IEnumerable<ItemMasterIdNoListDto>> GetAllFgTgItemMasterItemNoList();
        Task<IEnumerable<ItemNoListDtos>> GetAllPurchasePartItemNoList();
        Task<IEnumerable<ItemNoListDtos>> GetAllOnlyServiceItemsPurchasePartItemNoList();
        Task<IEnumerable<ItemNoListDtos>> GetAllPurchasePartItemNoListExcludingServiceItems();
        Task<IEnumerable<ItemNoListDtos>> GetAllIsPRRequiredStatusTruePPItemNoList();
        Task<IEnumerable<ItemMasterIdNoListDto>> GetAllOpenGrinStatusTrueItemMasterIdNoList();
        Task<string> GetClosedIqcItemMasterItemNo(string ItemNumber);
        Task<Dictionary<string, int?>> GetItemsImageIds(List<string> ItemNumbers);
        Task<List<GetDownloadUrlswithitemnumber>> GetImageDetails(Dictionary<string, int?> itemImageids);
        Task<List<ItemWithPartTypeAndMinDto>> GetItemMasterPartTypeAndMinByItemNumber(List<string> ItemNumberList);
        Task<IEnumerable<string>> GetAllFGItemNumberList();
    }
}
