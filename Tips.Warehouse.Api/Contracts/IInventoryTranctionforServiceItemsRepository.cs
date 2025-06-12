using System.Threading.Tasks;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IInventoryTranctionforServiceItemsRepository : IRepositoryBase<InventoryTranctionforServiceItems>
    {
        Task<InventoryTranctionforServiceItems> CreateInventoryTranctionforServiceItems(InventoryTranctionforServiceItems inventoryTranction);
    }
}
