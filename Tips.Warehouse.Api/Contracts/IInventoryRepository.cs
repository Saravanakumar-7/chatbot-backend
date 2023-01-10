using Contracts;
using Entities;
using Entities.Helper;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IInventoryRepository : IRepositoryBase<Inventory>
    {
        Task<PagedList<Inventory>> GetAllInventory(PagingParameter pagingParameter);

        Task<int?> CreateInventory(Inventory inventory);
        Task<string> UpdateInventory(Inventory inventory);
        Task<string> DeleteInventory(Inventory inventory);

        Task<Inventory> GetInventoryById(int id);

        Task<Inventory> GetInventoryDetailsByGrinNo(string GrinNo, string ItemNumber, string ProjectNumber);

        

    }
}
