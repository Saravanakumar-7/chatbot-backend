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

        public async Task<ItemPriceList> GetItemPriceListById(int id)
        {
            var getItemPriceListById = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return getItemPriceListById;
        }

        public async Task<IEnumerable<ItemPriceList>> GetItemPriceListByItemNo(string itemNo)
        {
            var getItemPriceListByItemNo = await FindByCondition(x => x.ItemNo == itemNo).ToListAsync();

            return getItemPriceListByItemNo;
        }
        public async Task<IEnumerable<ItemPriceList>> GetItemPriceListByItemNoAndPriceListName(string itemNo, string priceListName)
        {
            var getItemPriceListByItemNo = await FindByCondition(x => x.ItemNo == itemNo && x.PriceListName == priceListName).ToListAsync();

            return getItemPriceListByItemNo;
        }

        
        public async Task<string> UpdateItemPriceList(ItemPriceList itemPriceList)
        {
            itemPriceList.LastModifiedBy = "Admin";
            itemPriceList.LastModifiedOn = DateTime.Now;
            Update(itemPriceList);
            string result = $"AuditFrequency details of {itemPriceList.Id} is updated successfully!";
            return result;
        }
    }
}
