using Entities;
using Entities.Helper;
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

        public async Task<PagedList<ItemPriceList>> GetAllItemPriceList(PagingParameter pagingParameter)
        {

            var getAllItemPriceList = PagedList<ItemPriceList>.ToPagedList(FindAll().OrderByDescending(x => x.Id)                                
               .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllItemPriceList;
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

        public async Task<ItemPriceList> GetItemPricesByPassingListOfItemNoAndPriceListNames(string itemNo, string priceListName)
        {
            var query = _tipsSalesServiceDbContext.ItemPriceLists.Where(b => b.ItemNumber == itemNo);
            if (!string.IsNullOrEmpty(priceListName))
            {
                query = query.Where(b => b.PriceListName == priceListName);
            }
            var itemPriceListByItemNoAndPriceListNames = await query.FirstOrDefaultAsync();

            return itemPriceListByItemNoAndPriceListNames;
        }

        //public async Task<ItemPriceList> GetItemPricesByPassingListOfItemNoAndPriceListNames(string itemNo, string priceListName)
        //{

        //    var itemPriceListByItemNoAndPriceListNames = await _tipsSalesServiceDbContext.ItemPriceLists
        //                        .Where(b => b.ItemNumber == itemNo || b.PriceListName == priceListName)
        //                       .FirstOrDefaultAsync();

        //    return itemPriceListByItemNoAndPriceListNames;
        //}
         
         
        //public async Task<IEnumerable<ItemNumberAndPriceNameListDto>> GetItemPriceListByItemNoAndPriceListNames(string itemNo, string priceListName)
        //{

        //    IEnumerable<ItemNumberAndPriceNameListDto> getItemPriceListByItemNoAndPriceListName = await _tipsSalesServiceDbContext.ItemPriceLists
        //                        .Where(b => b.ItemNumber == itemNo && b.PriceListName == priceListName)
        //                        .Select(x => new ItemNumberAndPriceNameListDto()
        //                        {
        //                            ItemNumber = x.ItemNumber,
        //                            PriceListName = x.PriceListName,
        //                        })
        //                      .ToListAsync();

        //    return getItemPriceListByItemNoAndPriceListName;
        //}

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
