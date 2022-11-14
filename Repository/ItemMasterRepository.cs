using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities;
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
            return result.Id;
        }

        public async Task<string> DeleteItem(ItemMaster itemMaster)
        {
            Delete(itemMaster);
            string result = $"LeadTime details of {itemMaster.Id} is deleted successfully!";
            return result;
        }

        public async Task<List<ItemMaster>> GetAllActiveItems()
        {
            var LeadTimeList = await FindByCondition(x => x.IsActive == true).ToListAsync();
            return LeadTimeList;
        }

        public async Task<IEnumerable<ItemMaster>> GetAllItems()
        {
            var itemmasterList = await TipsMasterDbContext.ItemMasters
                                .Include(t => t.ItemmasterAlternate)
                                .Include(x => x.ItemMasterApprovedVendor)
                                .Include(m => m.ItemMasterFileUpload)
                                .Include(s => s.ItemMasterRouting)
                                .Include(f => f.ItemMasterWarehouse)
                                .ToListAsync();
             
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

        Task<IEnumerable<ItemMaster>> IItemMasterRepository.GetAllActiveItems()
        {
            throw new NotImplementedException();
        }
    }
}
