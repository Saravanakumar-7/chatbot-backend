using Entities.Helper;
using Entities;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Repository;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IInventoryTranctionRepository : IRepositoryBase<InventoryTranction>
    {
        Task<PagedList<InventoryTranction>> GetAllInventoryTranction(PagingParameter pagingParameter, SearchParams searchParams);

        Task<InventoryTranction> CreateInventoryTransaction(InventoryTranction inventoryTranction);
        Task<string> UpdateInventoryTraction(InventoryTranction inventoryTranction);
        Task<string> DeleteInventoryTranction(InventoryTranction inventoryTranction);
        Task<InventoryTranction> GetInventoryTranctionDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId, string ItemNumber, string ProjectNumber);
        Task<InventoryTranction> GetInventoryTranctionById(int id);
    }
}
