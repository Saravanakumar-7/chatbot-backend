using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

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

        public async Task<PagedList<Inventory>> GetAllInventory([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            var getAllInventory = FindAll()
                   .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ProjectNumber.Contains(searchParams.SearchValue) ||
                   inv.PartNumber.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue)
                   || inv.MftrPartNumber.Contains(searchParams.SearchValue) || inv.Warehouse.Contains(searchParams.SearchValue) || inv.PartType.Contains(searchParams.SearchValue)
                   || inv.Location.Contains(searchParams.SearchValue))));

            return PagedList<Inventory>.ToPagedList(getAllInventory, pagingParameter.PageNumber, pagingParameter.PageSize);
           

        }

        public async Task<Inventory> GetInventoryDetailsByGrinNo(string GrinNo, string ItemNumber, string ProjectNumber)
        {
            var getInventoryDetailsById = await _tipsWarehouseDbContext.Inventory.Where(x => x.GrinNo == GrinNo && x.PartNumber == ItemNumber && x.ProjectNumber == ProjectNumber)

                          .FirstOrDefaultAsync();

            return getInventoryDetailsById;
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
