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
            itemMaster.Unit = "Bangalore";
            var result = await Create(itemMaster);
            
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
            var AllActiveItems= await FindByCondition(x => x.IsActive == true).ToListAsync();
            return AllActiveItems;
        }

        public async Task<PagedList<ItemMaster>> GetAllItems(PagingParameter pagingParameter)
        {
            var GetallItemmaster= PagedList<ItemMaster>.ToPagedList(FindAll()
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);
                                 
            return GetallItemmaster;
        }
        public async Task<PagedList<ItemMaster>> GetAllFGItems(PagingParameter pagingParameter)
        {
            var GetallFgItems = PagedList<ItemMaster>.ToPagedList(FindAll().Where(a => a.ItemType == "Fg")
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return GetallFgItems;
        }
        public async Task<PagedList<ItemMaster>> GetAllSAItems(PagingParameter pagingParameter)
        {
            var GetallSaItems = PagedList<ItemMaster>.ToPagedList(FindAll().Where(a => a.ItemType == "Sa")
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return GetallSaItems;
        }
        public async Task<PagedList<ItemMaster>> GetAllFgSaItems(PagingParameter pagingParameter)
        {
            var GetallFgSaItems = PagedList<ItemMaster>.ToPagedList(FindAll().Where(a => a.ItemType == "Sa" || a.ItemType == "Fg")
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return GetallFgSaItems;
        }

        public async Task<ItemMaster> GetItemById(int id)
        {
            var ItemMasterbyId = await TipsMasterDbContext.ItemMasters
                            .Where(x => x.Id == id)
                             .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(p => p.ItemMasterWarehouse)
                             .FirstOrDefaultAsync();
           

            return ItemMasterbyId;
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
            IEnumerable<ItemMasterIdNoListDto> AllActiveItemmasterIDNO = await TipsMasterDbContext.ItemMasters
                                .Select(c => new ItemMasterIdNoListDto()
                                {
                                    id = c.Id,
                                    ItemNumber = c.ItemNumber,
                                    Description= c.Description,
                                    
                                   
                                })
                              .ToListAsync();

            return AllActiveItemmasterIDNO;
        }
        public async Task<ItemMaster> GetItemByItemNumber(string ItemNumber)
        {
            var ItemByItemNo = await FindByCondition(x => x.ItemNumber == ItemNumber)

                             .FirstOrDefaultAsync();
            return ItemByItemNo;
        }       
         
    }
}
