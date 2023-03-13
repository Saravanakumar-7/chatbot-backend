using Contracts;
using Entities;
using Entities.Helper;
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

        Task<IEnumerable<ListOfLocationTransferDto>> GetInventoryDetailsForLocationTransfer(string ItemNumber);
        
        Task<Inventory> GetInventoryDetailsByItemNo(string ItemNumber);

        Task<Inventory> GetInventoryDetailsByGrinNo(string GrinNo, string ItemNumber, string ProjectNumber);
        Task<IEnumerable<GetInventoryListByItemNo>> GetInventoryListByItemNo(string ItemNumber );



    }
}
