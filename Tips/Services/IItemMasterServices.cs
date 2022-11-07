using Tips.Model;

namespace Tips.Services
{
    public interface IItemMasterServices
    {
        public Task<List<ItemMaster>> GetAllItems();
        public Task<List<ItemMaster>> GetItemById(int id);
        public Task<long> CreateItem(ItemMaster itemMaster);
        Task<ItemMaster> UpdateItem(ItemMaster itemMaster, int id);
        public Task DeleteItem(int id);
        //public Task<ItemMaster> GetItemByDescription(string description);
        //public Task<List<ItemMaster>> GetAllActiveItems();
        //public Task<ItemMaster> GetAllInActiveItems();
        //public Task<ItemMaster> GetAllObsoleteItems();
        //public Task<string> ActivateItem(int id);
        //public Task<string> DeactivateItem(int id);

    }
}
