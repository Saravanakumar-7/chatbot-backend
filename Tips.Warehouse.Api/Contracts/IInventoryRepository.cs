using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IInventoryRepository : IRepositoryBase<Inventory>
    {
        Task<PagedList<Inventory>> GetAllInventory(PagingParameter pagingParameter, SearchParams searchParams);

        Task<int?> CreateInventory(Inventory inventory);
        Task<string> UpdateInventory(Inventory inventory);
        Task<string> DeleteInventory(Inventory inventory);

        Task<Inventory> GetInventoryById(int id);
        
        Task<Inventory> GetInventoryDetails(string ItemNumber);

        Task<Inventory> GetInventoryFGDetailsByItemNumber(string ItemNumber);

        //Task<Inventory> UpdateInventoryBalanceQty(string partNumber, string Qty);
        Task<decimal> GetStockDetailsForAllLocationWarehouseByItemNo(string ItemNumber);

        Task<IEnumerable<ListOfLocationTransferDto>> GetInventoryDetailsForLocationTransfer(string ItemNumber);

        
        Task<Inventory> GetInventoryDetailsByItemAndProjectNo(string itemNumber, string projectNumber);

        Task<Inventory> GetInventoryDetailsByItemNo(string ItemNumber);

        Task<Inventory> GetInventoryDetailsByGrinNo(string GrinNo, string ItemNumber, string ProjectNumber);
        Task<IEnumerable<GetInventoryListByItemNo>> GetInventoryListByItemNo(string ItemNumber );
        Task<Inventory> GetInventoryDetailsByItemNoProjectNoUnitWarehouseAndLocation(string itemNumber, string projectNumber, string unit, string warehouse, string location);
        Task<IEnumerable<Inventory>> GetAllInventoryWithItems(InventorySearchDto inventorySearch);
        Task<IEnumerable<Inventory>> SearchInventory([FromQuery] SearchParames searchParames);
        Task<IEnumerable<Inventory>> SearchInventoryDate([FromQuery] SearchsDateParms searchsDateParms);
        Task<IEnumerable<Inventory>> GetInventoryByItemNumber(string ItemNumber);

    }
}
