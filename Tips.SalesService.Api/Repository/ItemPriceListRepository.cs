using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Repository
{
    public class ItemPriceListRepository : RepositoryBase<ItemPriceList>, IItemPriceListRepository
    {
        private TipsSalesServiceDbContext _tipsSalesServiceDbContext;
        public ItemPriceListRepository(TipsSalesServiceDbContext repositoryContext) : base(repositoryContext)
        {
            _tipsSalesServiceDbContext = repositoryContext;
        }

        public async Task<long> CreateItemPriceList(ItemPriceList itemPriceList)
        {

            itemPriceList.CreatedBy = "Admin";
            itemPriceList.CreatedOn = DateTime.Now;
            itemPriceList.Unit = "Banglore";
            var result = await Create(itemPriceList);
            return result.Id;
        }

        public async Task<ItemPriceList> CreateFromReleaseLp(ItemPriceList itemPriceList)
        {
            itemPriceList.CreatedBy = "Admin";
            itemPriceList.CreatedOn = DateTime.Now;
            itemPriceList.Unit = "Bangalore";
            var result = await Create(itemPriceList);
            return result;
        }

        public async Task<string> DeleteItemPriceList(ItemPriceList itemPriceList)
        {
            Delete(itemPriceList);
            string result = $"ItemPriceList details of {itemPriceList.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<ItemPriceList>> GetAllItemPriceList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {

            var itemPrices = FindAll().OrderByDescending(x => x.Id)
              .Where(inv => ((string.IsNullOrWhiteSpace(searchParammes.SearchValue) || inv.ItemNumber.Contains(searchParammes.SearchValue) || inv.Description.Contains(searchParammes.SearchValue))));

            return PagedList<ItemPriceList>.ToPagedList(itemPrices, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<IEnumerable<ItemPriceListNameDto>> GetAllItemPriceNameList()
        {
            IEnumerable<ItemPriceListNameDto> getAllItemPriceListName = await _tipsSalesServiceDbContext.ItemPriceLists
                                .Select(c => new ItemPriceListNameDto()
                                {
                                    PriceListName = c.PriceListName,                                    

                                }).Distinct()
                              .ToListAsync();

            return getAllItemPriceListName;
        }

        public async Task<IEnumerable<ItemNumberListDto>> GetAllItemNumberList()
        {
            IEnumerable<ItemNumberListDto> getAllItemNumberList = await _tipsSalesServiceDbContext.ItemPriceLists
                                .Select(c => new ItemNumberListDto()
                                {
                                    ItemNumber = c.ItemNumber,
                                    Description = c.Description

                                }).Distinct()                                
                              .ToListAsync();

            return getAllItemNumberList;
        }

        public async Task<ItemPriceList> GetItemPriceListById(int id)
        {
            var getItemPriceListById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return getItemPriceListById;
        }

        public async Task<IEnumerable<ItemPriceList>> GetItemPriceListByItemNo(string itemNo)
        {
            var getItemPriceListByItemNo = await FindByCondition(x => x.ItemNumber == itemNo).ToListAsync();

            return getItemPriceListByItemNo;
        }
        public async Task<IEnumerable<ItemPriceList>> GetItemPriceListByItemNoAndPriceListName(string itemNo, string priceListName)
        {
            var getItemPriceListByItemNoAndPriceListName = await FindByCondition(x => x.ItemNumber == itemNo && x.PriceListName == priceListName).ToListAsync();

            return getItemPriceListByItemNoAndPriceListName;
        }

        public async Task<ItemPriceList> GetItemPricesByListOfItemNoAndPriceListNames(string itemNo, string priceListName)
        {
            var itemPriceListDetails = _tipsSalesServiceDbContext.ItemPriceLists.Where(b => b.ItemNumber == itemNo);
            if (!string.IsNullOrEmpty(priceListName))
            {
                itemPriceListDetails = itemPriceListDetails.Where(b => b.PriceListName == priceListName);
            }
            var itemPriceList = await itemPriceListDetails.FirstOrDefaultAsync();

            return itemPriceList;
        }
         

        public async Task<string> UpdateItemPriceList(ItemPriceList itemPriceList)
        {
            itemPriceList.LastModifiedBy = "Admin";
            itemPriceList.LastModifiedOn = DateTime.Now;
            Update(itemPriceList);
            string result = $"ItemPriceList details of {itemPriceList.Id} is updated successfully!";
            return result;
        } 
    }
}
