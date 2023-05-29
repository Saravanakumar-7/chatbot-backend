using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Tips.SalesService.Api.Contracts
{
    public interface IItemPriceListRepository : IRepositoryBase<ItemPriceList>
    {
        Task<PagedList<ItemPriceList>> GetAllItemPriceList(PagingParameter pagingParameter, SearchParammes searchParammes);
        Task<IEnumerable<ItemPriceListNameDto>> GetAllItemPriceNameList();

        Task<IEnumerable<ItemNumberListDto>> GetAllItemNumberList();

        Task<ItemPriceList> GetItemPriceListById(int id);
        Task<IEnumerable<ItemPriceList>> GetItemPriceListByItemNo(string itemNo);

        Task<ItemPriceList> GetItemPricesByListOfItemNoAndPriceListNames(string itemNo, string priceListName);

         //Task<IEnumerable<ItemNumberAndPriceNameListDto>> GetItemPriceListByItemNoAndPriceListNames(string itemNo, string priceListName);

        Task<IEnumerable<ItemPriceList>> GetItemPriceListByItemNoAndPriceListName(string itemNo,string priceListName);

        Task<ItemPriceList> CreateFromReleaseLp(ItemPriceList itemPriceList);
        Task<long> CreateItemPriceList(ItemPriceList itemPriceList);
        Task<string> UpdateItemPriceList(ItemPriceList itemPriceList);

        Task<string> DeleteItemPriceList(ItemPriceList itemPriceList);
        Task<PagedList<ItemPriceList>> GetItemPriceListByPriceListName(string priceListName, [FromQuery] PagingParameter pagingParameter, SearchParammes searchParammes);
    }
}
