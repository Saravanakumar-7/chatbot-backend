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
    public class ItemMasterRepository : RepositoryBase<ItemMaster>, IItemMasterRepository
    {
        public ItemMasterRepository(TipsMasterDbContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<long> CreateItemMaster(ItemMaster itemMaster)
        {
            itemMaster.CreatedBy = "Admin";
            itemMaster.CreatedOn = DateTime.Now;
            itemMaster.Unit = "Bangalore";
            var result = await Create(itemMaster);

            return result.Id;
        }

        public async Task<string> DeleteItemMaster(ItemMaster itemMaster)
        {
            Delete(itemMaster);
            string result = $"ItemMaster details of {itemMaster.Id} is deleted successfully!";
            return result;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllActiveItemMasters()
        {
            var getAllActiveItemMasters = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return getAllActiveItemMasters;
        }

        public async Task<PagedList<ItemMaster>> GetAllItemMasters(PagingParameter pagingParameter)
        {
            var getAllItemMasters = PagedList<ItemMaster>.ToPagedList(FindAll()
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllItemMasters;
        }



        public async Task<PagedList<ItemMaster>> GetAllFGItems(PagingParameter pagingParameter)
        {
            var getAllFGItems = PagedList<ItemMaster>.ToPagedList(FindAll().Where(a => a.ItemType == "Fg")
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllFGItems;
        }
        public async Task<PagedList<ItemMaster>> GetAllSAItems(PagingParameter pagingParameter)
        {
            var getAllSAItems = PagedList<ItemMaster>.ToPagedList(FindAll().Where(a => a.ItemType == "Sa")
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllSAItems;
        }
        public async Task<PagedList<ItemMaster>> GetAllFgSaItems(PagingParameter pagingParameter)
        {
            var getAllFGSAItems = PagedList<ItemMaster>.ToPagedList(FindAll().Where(a => a.ItemType == "Sa" || a.ItemType == "Fg")
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllFGSAItems;
        }
     
        public async Task<ItemMaster> GetItemMasterById(int id)
        {
            var getItemMasterById = await TipsMasterDbContext.ItemMasters
                            .Where(x => x.Id == id)
                             .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(p => p.ItemMasterWarehouse)
                             .FirstOrDefaultAsync();


            return getItemMasterById;
        }

        public async Task<string> UpdateItemMaster(ItemMaster itemMaster)
        {
            itemMaster.LastModifiedBy = "Admin";
            itemMaster.LastModifiedOn = DateTime.Now;
            Update(itemMaster);
            string result = $"ItemMaster details of {itemMaster.Id} is updated successfully!";
            return result;
        }
        public async Task<IEnumerable<ItemMasterIdNoListDto>> GetAllActiveItemMasterIdNoList()
        {
            IEnumerable<ItemMasterIdNoListDto> getAllActiveItemMasterIdNoListDto = await TipsMasterDbContext.ItemMasters
                                .Select(c => new ItemMasterIdNoListDto()
                                {
                                    id = c.Id,
                                    ItemNumber = c.ItemNumber,
                                    Description = c.Description,

                                })
                              .ToListAsync();

            return getAllActiveItemMasterIdNoListDto;
        }
        public async Task<ItemMaster> GetItemMasterByItemNumber(string ItemNumber)
        {
            var getItemMasterByItemNo = await FindByCondition(x => x.ItemNumber == ItemNumber)

                             .FirstOrDefaultAsync();
            return getItemMasterByItemNo;
        }

    } 

    }
