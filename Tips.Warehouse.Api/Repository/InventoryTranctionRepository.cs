using Entities;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Repository
{
    public class InventoryTranctionRepository : RepositoryBase<InventoryTranction>, IInventoryTranctionRepository
    {
        public InventoryTranctionRepository(TipsWarehouseDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<InventoryTranction> CreateInventoryTransaction(InventoryTranction inventoryTranction)
        {
            inventoryTranction.CreatedBy = "Admin";
            inventoryTranction.CreatedOn = DateTime.Now;
            inventoryTranction.Unit = "Bangalore";
            var result = await Create(inventoryTranction);

            return result;
        }

        public async Task<string> DeleteInventoryTranction(InventoryTranction inventoryTranction)
        {
            Delete(inventoryTranction);
            string result = $"NaterialIssue details of {inventoryTranction.Id} is deleted successfully!";
            return result;
        }

        public async Task<PagedList<InventoryTranction>> GetAllInventoryTranction([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {


            var getAllInventoryTranction = FindAll().OrderByDescending(x => x.Id)
                   .Where(inv => ((string.IsNullOrWhiteSpace(searchParams.SearchValue) || inv.ProjectNumber.Contains(searchParams.SearchValue) ||
                   inv.PartNumber.Contains(searchParams.SearchValue) || inv.Description.Contains(searchParams.SearchValue)
                   || inv.MftrPartNumber.Contains(searchParams.SearchValue))));

            return PagedList<InventoryTranction>.ToPagedList(getAllInventoryTranction, pagingParameter.PageNumber, pagingParameter.PageSize);
        }

        public async Task<InventoryTranction> GetInventoryTranctionById(int id)
        {
            var getInventoryTranctionById = await _tipsWarehouseDbContext.InventoryTranctions.Where(x => x.Id == id)

                          .FirstOrDefaultAsync();

            return getInventoryTranctionById;
        }

        public async Task<string> UpdateInventoryTraction(InventoryTranction inventoryTranction)
        {
            inventoryTranction.LastModifiedBy = "Admin";
            inventoryTranction.LastModifiedOn = DateTime.Now;
            Update(inventoryTranction);
            string result = $"materialIssue of Detail {inventoryTranction.Id} is updated successfully!";
            return result;
        }
    }
}
