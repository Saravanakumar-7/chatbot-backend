using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ItemMasterRepository:RepositoryBase<ItemMaster>,IItemMasterRepository
    {
        public ItemMasterRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<long> CreateItem(ItemMaster itemMaster)
        {
            itemMaster.CreatedBy = "Admin";
            itemMaster.CreatedOn = DateTime.Now;
            var result = await Create(itemMaster);
            itemMaster.Unit = "Bangalore";
            return result.Id;
        }

        public async Task<string> DeleteItem(ItemMaster itemMaster)
        {
            Delete(itemMaster);
            string result = $"LeadTime details of {itemMaster.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllActiveItems()
        {
            var LeadTimeList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return LeadTimeList;
        }

        public async Task<PagedList<ItemMaster>> GetAllItems(PagingParameter pagingParameter)
        {
            var itemmasterList = PagedList<ItemMaster>.ToPagedList(FindAll()
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
                                 
            return itemmasterList;
        }
        public async Task<PagedList<ItemMaster>> GetAllFGItems(PagingParameter pagingParameter)
        {
            var itemmasterList = PagedList<ItemMaster>.ToPagedList(FindAll().Where(a => a.ItemType == "Fg")
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return itemmasterList;
        }
        public async Task<PagedList<ItemMaster>> GetAllSAItems(PagingParameter pagingParameter)
        {
            var itemmasterList = PagedList<ItemMaster>.ToPagedList(FindAll().Where(a => a.ItemType == "Sa")
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return itemmasterList;
        }
        public async Task<PagedList<ItemMaster>> GetAllFgSaItems(PagingParameter pagingParameter)
        {
            var itemmasterList = PagedList<ItemMaster>.ToPagedList(FindAll().Where(a => a.ItemType == "Sa" || a.ItemType == "Fg")
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return itemmasterList;
        }

        public async Task<ItemMaster> GetItemById(int id)
        {
            var ItemMasterList = await TipsMasterDbContext.ItemMasters
                            .Where(x => x.Id == id)
                             .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(p => p.ItemMasterWarehouse)
                             .FirstOrDefaultAsync();
            //var ItemMasterList = await FindByCondition(x => x.Id == id).FirstOrDefaultAsync();

            return ItemMasterList;
        }

        public async Task<string> UpdateItem(ItemMaster itemMaster)
     {
            itemMaster.LastModifiedBy = "Admin";
            itemMaster.LastModifiedOn = DateTime.Now;
            Update(itemMaster);
            string result = $"LeadTime details of {itemMaster.Id} is updated successfully!";
            return result;
        }
        public async Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveItemMasterIdNoList()
        {
            IEnumerable<ItemMasterIdNoListDto> ItemMasterDetails = await TipsMasterDbContext.ItemMasters
                                .Select(c => new ItemMasterIdNoListDto()
                                {
                                    id = c.Id,
                                    ItemNumber = c.ItemNumber,
                                    Description= c.Description,
                                    
                                   
                                })
                              .ToListAsync();

            return ItemMasterDetails;
        }
        public async Task<ItemMaster> GetItemByItemNumber(string ItemNumber)
        {
            var ItemNumberList = await FindByCondition(x => x.ItemNumber == ItemNumber)

                             .FirstOrDefaultAsync();
            return ItemNumberList;
        }       
         
    }
}
