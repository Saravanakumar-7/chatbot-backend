using Entities;
using Entities.Helper;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Repository
{
    public class InventoryRepository : RepositoryBase<Inventory>, IInventoryRepository
    {
        public InventoryRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<int?> CreateInventory(Inventory inventory)
        {
            inventory.CreatedBy = "Admin";
            inventory.CreatedOn = DateTime.Now;
            inventory.Unit = "Bangalore"; 
            var result = await Create(inventory);

            return result.Id;
        }

        public async Task<string> DeleteInventory(Inventory inventory)
        {
            Delete(inventory);
            string result = $"NaterialIssue details of {inventory.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<Inventory>> GetAllInventory(PagingParameter pagingParameter)
        {
            var getAllInventory = PagedList<Inventory>.ToPagedList(FindAll()
             .OrderBy(on => on.Id), pagingParameter.PageNumber, pagingParameter.PageSize);

            return getAllInventory;
        }

        public async Task<Inventory> GetInventoryDetailsByGrinNo(string GrinNo, string ItemNumber, string ProjectNumber)
        {
            var getInventoryDetailsById = await _tipsWarehouseDbContext.Inventory.Where(x => x.GrinNo == GrinNo && x.PartNumber == ItemNumber && x.ProjectNumber == ProjectNumber)

                          .FirstOrDefaultAsync();

            return getInventoryDetailsById;
        }

        public async Task<IEnumerable<GetInventoryListByItemNo>> GetInventoryListByItemNo( string ItemNumber)
        {

            IEnumerable<GetInventoryListByItemNo> getInventoryListByItemNo = await _tipsWarehouseDbContext.Inventory
                .Where(x =>x.PartNumber == ItemNumber && x.IsStockAvailable == true)
                                .Select(x => new GetInventoryListByItemNo()
                                {
                                    InventoryId = x.Id,
                                    ItemNumber = x.PartNumber,
                                    Balance_Quantity = x.Balance_Quantity

                                })
                                .OrderBy(on => on.InventoryId)
                              
                              .ToListAsync();

            return getInventoryListByItemNo;
        }
        public async Task<Inventory> GetInventoryById(int id)
        {
            var getInventoryById = await _tipsWarehouseDbContext.Inventory.Where(x => x.Id == id)

                          .FirstOrDefaultAsync();

            return getInventoryById;
        }

        public async Task<string> UpdateInventory(Inventory inventory)
        {
            inventory.LastModifiedBy = "Admin";
            inventory.LastModifiedOn = DateTime.Now;
            Update(inventory);
            string result = $"materialIssue of Detail {inventory.Id} is updated successfully!";
            return result;
        }
    }
}
