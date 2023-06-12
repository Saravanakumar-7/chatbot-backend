using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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


        public async Task<IEnumerable<Inventory>> GetAllInventoryWithItems(InventorySearchDto inventorySearch)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.Inventory.AsQueryable();
                if (inventorySearch != null || (inventorySearch.PartNumber.Any())
                 && inventorySearch.ProjectNumber.Any() && inventorySearch.Warehouse.Any()
                 && inventorySearch.Location.Any() && inventorySearch.GrinNo.Any())

                {
                    query = query.Where
                    (inv => (inventorySearch.PartNumber.Any() ? inventorySearch.PartNumber.Contains(inv.PartNumber) : true)
                   && (inventorySearch.ProjectNumber.Any() ? inventorySearch.ProjectNumber.Contains(inv.ProjectNumber) : true)
                   && (inventorySearch.Warehouse.Any() ? inventorySearch.Warehouse.Contains(inv.Warehouse) : true)
                   && (inventorySearch.Location.Any() ? inventorySearch.Location.Contains(inv.Location) : true)
                   && (inventorySearch.GrinNo.Any() ? inventorySearch.GrinNo.Contains(inv.GrinNo) : true));
                }
                return query.ToList();
            }
        }
        public async Task<IEnumerable<Inventory>> SearchInventoryDate([FromQuery] SearchsDateParms searchsDateParms)
        {
            var inventoryDetails = _tipsWarehouseDbContext.Inventory
            .Where(inv => ((inv.CreatedOn >= searchsDateParms.SearchFromDate &&
            inv.CreatedOn <= searchsDateParms.SearchToDate
            )))
            .ToList();
            return inventoryDetails;
        }
        public async Task<IEnumerable<Inventory>> SearchInventory([FromQuery] SearchParames searchParames)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.Inventory.AsQueryable();
                if (!string.IsNullOrEmpty(searchParames.SearchValue))
                {
                    query = query.Where(inv => inv.PartNumber.Contains(searchParames.SearchValue)
                    || inv.ProjectNumber.Contains(searchParames.SearchValue)
                    || inv.Warehouse.Contains(searchParames.SearchValue)
                    || inv.Location.Contains(searchParames.SearchValue)
                    || inv.GrinNo.Contains(searchParames.SearchValue));
                }
                return query.ToList();
            }
        }
        public async Task<List<InventoryItemNoStock>> GetItemNoByInventoryStock()
        {
            List<InventoryItemNoStock> inventoryItemNoStock = _tipsWarehouseDbContext.Inventory
                       .GroupBy(l => new { l.PartNumber})
                       .Select(group => new InventoryItemNoStock
                       {
                           PartNumber = group.Key.PartNumber,
                           Balance_Quantity = group.Sum(c => c.Balance_Quantity),

                       }).ToList();

            return inventoryItemNoStock;
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
            var getAllInventory = FindAll().OrderByDescending(x => x.Id)
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
        //passing grinid

        public async Task<Inventory> GetInventoryDetailsByGrinNoandGrinId(string GrinNo,int GrinPartsId, string ItemNumber, string ProjectNumber)
        {
            var getInventoryDetailsById = await _tipsWarehouseDbContext.Inventory.Where(x => x.GrinNo == GrinNo && x.GrinPartId == GrinPartsId && x.PartNumber == ItemNumber && x.ProjectNumber == ProjectNumber)

                          .FirstOrDefaultAsync();

            return getInventoryDetailsById;
        }

        public async Task<Inventory> GetInventoryDetailsByItemNo(string ItemNumber)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventory.Where(x =>x.PartNumber == ItemNumber && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();

            return inventoryDetail;
        }

        public async Task<decimal> GetStockDetailsForAllLocationWarehouseByItemNo(string ItemNumber)
        {
            var inventoryDetail = _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true)
                          .Sum(x=>x.Balance_Quantity);

            return inventoryDetail;
        }

        public async Task<Inventory> GetInventoryDetailsByItemAndProjectNo(string itemNumber, string projectNumber)
        {
            var inventoryDetailsByProjectNo = await _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == itemNumber && x.ProjectNumber == projectNumber && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();

            return inventoryDetailsByProjectNo;
        }
        

        public async Task<Inventory> GetInventoryDetailsByItemNoProjectNoUnitWarehouseAndLocation(string itemNumber, string projectNumber,string unit,string warehouse,string location)
        {
            var inventoryDetails = await _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == itemNumber 
            && x.ProjectNumber == projectNumber && x.Unit == unit && x.Warehouse == warehouse && x.Location == location && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();

            return inventoryDetails;
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

        public async Task<Inventory> GetInventoryDetails(string ItemNumber)
        {
            var getSalesOrderDetailsBySOandItemNo = await _tipsWarehouseDbContext.Inventory
                 .Where(x => x.PartNumber == ItemNumber)
                          .FirstOrDefaultAsync();

            return getSalesOrderDetailsBySOandItemNo;
        }

        //get inventory details from fg partnumber

        public async Task<Inventory> GetInventoryFGDetailsByItemNumber(string ItemNumber)
        {
            var partTypes = new string[] { "FG", "TG","FRU" };
          
            var getSalesOrderDetailsBy = await _tipsWarehouseDbContext.Inventory
                 .Where(x => x.PartNumber == ItemNumber && partTypes.Contains(x.PartType) && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();

            return getSalesOrderDetailsBy;
        }

        public async Task<IEnumerable<Inventory>> GetInventoryByItemNumber(string ItemNumber)
        {

            var getSalesOrderDetailsBy = await _tipsWarehouseDbContext.Inventory
                 .Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true)
                          .ToListAsync();

            return getSalesOrderDetailsBy;
        }
        public async Task<IEnumerable<ListOfLocationTransferDto>> GetInventoryDetailsForLocationTransfer(string ItemNumber)
        {

            IEnumerable<ListOfLocationTransferDto> getBtoNumberList = await _tipsWarehouseDbContext.Inventory
                                .Select(x => new ListOfLocationTransferDto()
                                {
                                    PartNumber = x.PartNumber,
                                    Description = x.Description,
                                    UOM = x.UOM,
                                    Warehouse = x.Warehouse,
                                    Location = x.Location,
                                    PartType = x.PartType

                                })
                                .Where(x => x.PartNumber == ItemNumber)
                              .ToListAsync();

            return getBtoNumberList;
        }

        //public async Task<Inventory> UpdateInventoryBalanceQty(string partNumber, string Qty)
        //{
        //    var getInventoryDetails = await _tipsWarehouseDbContext.Inventory
        //            .Where(x => x.PartNumber == partNumber)
        //                  .FirstOrDefaultAsync();
        //    decimal Quantity = Convert.ToDecimal(Qty);
        //    if (getInventoryDetails != null)
        //    {
        //        if (Quantity != 0 && getInventoryDetails.Balance_Quantity >= Quantity)
        //        {
        //            getInventoryDetails.Balance_Quantity = getInventoryDetails.Balance_Quantity - Quantity;
        //            Quantity = 0;
        //            if (getInventoryDetails.Balance_Quantity == 0)
        //            {
        //                getInventoryDetails.IsStockAvailable = false;
        //            }
        //        }
        //        if (Quantity != 0 && getInventoryDetails.Balance_Quantity < Quantity)
        //        {
        //            Quantity = Quantity - getInventoryDetails.Balance_Quantity;
        //            getInventoryDetails.Balance_Quantity = 0;
        //            getInventoryDetails.IsStockAvailable = false;
        //        }
        //        Update(getInventoryDetails);
        //    }
        //    return getInventoryDetails;
        //}

    }
}
