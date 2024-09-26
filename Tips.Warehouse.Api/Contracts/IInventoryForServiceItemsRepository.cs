using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IInventoryForServiceItemsRepository:IRepositoryBase<InventoryForServiceItems>
    {
        Task<int?> CreateInventoryForServiceItems(InventoryForServiceItems inventoryforserviceitems);
        Task<InventoryForServiceItems> GetInventoryForServiceDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId, string ItemNumber, string ProjectNumber);
        Task<InventoryForServiceItems> GetInventoryForServiceItemsById(int id);
        Task<string> UpdateInventoryForServiceItems(InventoryForServiceItems inventoryForServiceItems);
    }
}
