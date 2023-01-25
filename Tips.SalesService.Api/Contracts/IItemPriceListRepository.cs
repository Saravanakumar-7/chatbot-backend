using Entities.Helper;
using Entities;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities;

namespace Tips.SalesService.Api.Contracts
{
    public interface IItemPriceListRepository : IRepositoryBase<ItemPriceList>
    {
        Task<PagedList<ItemPriceList>> GetAllItemPriceList(PagingParameter pagingParameter);
        Task<ItemPriceList> GetItemPriceListById(int id);
        Task<IEnumerable<ItemPriceList>> GetItemPriceListByItemNo(string itemNo);
        Task<IEnumerable<ItemPriceList>> GetItemPriceListByItemNoAndPriceListName(string itemNo,string priceListName);

        Task<ItemPriceList> CreateFromReleaseLp(ItemPriceList itemPriceList);
        Task<long> CreateItemPriceList(ItemPriceList itemPriceList);
        Task<string> UpdateItemPriceList(ItemPriceList itemPriceList);

        Task<string> DeleteItemPriceList(ItemPriceList itemPriceList);
    }
}
