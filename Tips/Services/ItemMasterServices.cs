using Microsoft.EntityFrameworkCore;
using Tips.Data;
using Tips.Model;

namespace Tips.Services
{
    public class ItemMasterServices: IItemMasterServices
    {
        private readonly TipsDBContext _tipsDBContext;

        public ItemMasterServices(TipsDBContext tipsDBContext)
        {
            _tipsDBContext = tipsDBContext;
        }
        public async Task<List<ItemMaster>> GetAllItems()
        {
            var result = await _tipsDBContext.ItemMasters.Include(x => x.ItemmasterAlternate).Include(x => x.ItemMasterWarehouse).Include(x => x.ItemMasterApprovedVendor)
            .Include(x => x.ItemMasterFileUpload).Include(x => x.ItemMasterRouting).ToListAsync();

            return result;
        }

        public async Task<List<ItemMaster>> GetItemById(int id)
        {
            var result = await _tipsDBContext.ItemMasters.Where(x => x.Id == id).Include(x => x.ItemmasterAlternate).Include(x => x.ItemMasterWarehouse).Include(x => x.ItemMasterApprovedVendor)
            .Include(x => x.ItemMasterFileUpload).Include(x => x.ItemMasterRouting).ToListAsync();

            return result;
        }

        public async Task<long> CreateItem(ItemMaster itemMaster)
        {
            _tipsDBContext.ItemMasters.Add(itemMaster);
            await _tipsDBContext.SaveChangesAsync();
            return itemMaster.Id;
        }

        public async Task<ItemMaster> UpdateItem(ItemMaster itemMaster, int id)
        {
            itemMaster.Id = id;
            _tipsDBContext.ItemMasters.Update(itemMaster);
            await _tipsDBContext.SaveChangesAsync();
            return itemMaster;
        }
        public async Task DeleteItem(int id)
        {
            var Item = new ItemMaster()
            {
                Id = id
            };
            _tipsDBContext.ItemMasters.Remove(Item);
            await _tipsDBContext.SaveChangesAsync();

        }
    }
}
