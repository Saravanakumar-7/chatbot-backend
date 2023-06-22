using Contracts;
using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Tips.Warehouse.Api.Contracts
{
    public interface IInventoryRepository : IRepositoryBase<Inventory>
    {
        Task<PagedList<Inventory>> GetAllInventory(PagingParameter pagingParameter, SearchParams searchParams);

        Task<int?> CreateInventory(Inventory inventory);
        Task<string> UpdateInventory(Inventory inventory);
        Task<string> DeleteInventory(Inventory inventory);
        Task<List<InventoryItemNoStock>> GetItemNoByInventoryStock();
        Task<Inventory> GetInventoryById(int id);



        Task<Inventory> GetInventoryDetails(string ItemNumber);

        Task<Inventory> GetInventoryFGDetailsByItemNumber(string ItemNumber);

        //Task<Inventory> UpdateInventoryBalanceQty(string partNumber, string Qty);
        Task<decimal> GetStockDetailsForAllLocationWarehouseByItemNo(string ItemNumber);

        Task<IEnumerable<ListOfLocationTransferDto>> GetInventoryDetailsForLocationTransfer(string ItemNumber);
        Task<IEnumerable<Inventory>> GetInventoryDetailsWithSumOfStock(InventoryBalQty inventoryBalQty);
        Task<IEnumerable<Inventory>> SearchInventoryDetailsWithSumOfStock(InventoryItemNo inventoryItemNo);

        Task<Inventory> GetInventoryDetailsByItemAndProjectNo(string itemNumber, string projectNumber);

        Task<Inventory> GetInventoryDetailsByItemNo(string ItemNumber);

        Task<Inventory> GetInventoryDetailsByGrinNo(string GrinNo, string ItemNumber, string ProjectNumber);
        Task<Inventory> GetInventoryDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId, string ItemNumber, string ProjectNumber);
        Task<IEnumerable<GetInventoryListByItemNo>> GetInventoryListByItemNo(string ItemNumber );
        Task<Inventory> GetInventoryDetailsByItemNoProjectNoUnitWarehouseAndLocation(string itemNumber, string projectNumber, string unit, string warehouse, string location);
        Task<IEnumerable<Inventory>> GetAllInventoryWithItems(InventorySearchDto inventorySearch);
        Task<IEnumerable<Inventory>> SearchInventory([FromQuery] SearchParames searchParames);
        Task<IEnumerable<Inventory>> SearchInventoryDate([FromQuery] SearchsDateParms searchsDateParms);
        Task<IEnumerable<Inventory>> GetInventoryByItemNumber(string ItemNumber);
        Task<Inventory> GetSingleInventoryDetailsByItemNumberandLocation(string ItemNumber, string Location, string Warehouse);
        Task<IEnumerable<Inventory>> GetInventoryDetailsByItemNumberandLocation(string ItemNumber, string Location, string Warehouse);
        Task<IEnumerable<Inventory>> GetInventoryDetailsByItemNoandPartTypes(string ItemNumber);
    }
}
