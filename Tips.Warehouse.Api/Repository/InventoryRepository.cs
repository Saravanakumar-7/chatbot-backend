using Entities;
using Entities.DTOs;
using Entities.Helper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using System.Data.SqlClient;
using System.Linq;
using Entities.Enums;

namespace Tips.Warehouse.Api.Repository
{
    public class InventoryRepository : RepositoryBase<Inventory>, IInventoryRepository
    {
        private readonly string _connectionString;
        private readonly MySqlConnection _connection;
 
        public InventoryRepository(TipsWarehouseDbContext repositoryContext, MySqlConnection connection) : base(repositoryContext)
        {
            _connection = connection; 
        }
        public async Task<IEnumerable<Inventory>> SearchInventoryDetailsWithSumOfStock(InventoryItemNo inventoryItemNo)
        {

            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.Inventory.AsQueryable();

                // Check if inventoryItemNo object is not null
                if (inventoryItemNo != null)
                {
                    // Apply filtering based on the inventoryItemNo properties if they are not null
                    if (inventoryItemNo.PartNumber != null && inventoryItemNo.PartNumber.Any())
                    {
                        query = query.Where(inv => inventoryItemNo.PartNumber.Contains(inv.PartNumber));
                    }
                }

                // Retrieve the filtered inventory items
                var inventoryItems = await query.ToListAsync();
                 
                var groupedItems = new Dictionary<string, List<Inventory>>();
                foreach (var item in inventoryItems)
                {
                    var key = item.PartNumber;
                    if (!groupedItems.ContainsKey(key))
                    {
                        groupedItems[key] = new List<Inventory> { item };
                    }
                    else
                    {
                        groupedItems[key].Add(item);
                    }
                }
                // Calculate the sum of Balance_Quantity for each group and update the first item in the group
                foreach (var group in groupedItems)
                {
                    var sum = group.Value.Sum(inv => inv.Balance_Quantity);
                    var firstItem = group.Value.First();
                    firstItem.Balance_Quantity = sum;

                } 
                return groupedItems.Values.Select(group => group.First());
            } 
        }


        public async Task<IEnumerable<ConsumptionReport>> ConsumptionReports()
        {
            var result = _tipsWarehouseDbContext.Set<ConsumptionReport>()
    .FromSqlRaw("CALL ProductDemand_Vs_AvailableStock_WO_Parameter();")
    .ToList<ConsumptionReport>(); 

            return result;

        } 
        public async Task<IEnumerable<Inventory>> GetInventoryDetailsWithSumOfBalQty(InventoryDetailsBalQty inventoryDetailsBalQty)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.Inventory.AsQueryable();

                // Check if inventoryBalQty object is not null
                if (inventoryDetailsBalQty == null || (inventoryDetailsBalQty.PartNumber.Count == 0 && inventoryDetailsBalQty.Warehouse.Count == 0 && inventoryDetailsBalQty.Location.Count == 0 && inventoryDetailsBalQty.ProjectNumber.Count == 0))
                {
                    query = FindAll().OrderByDescending(x => x.Id);

                }
                else
                {
                    // Apply filtering based on the inventoryBalQty properties if they are not null
                    if (inventoryDetailsBalQty.PartNumber != null && inventoryDetailsBalQty.PartNumber.Any())
                    {
                        query = query.Where(inv => inventoryDetailsBalQty.PartNumber.Contains(inv.PartNumber));
                    }

                    if (inventoryDetailsBalQty.Warehouse != null && inventoryDetailsBalQty.Warehouse.Any())
                    {
                        query = query.Where(inv => inventoryDetailsBalQty.Warehouse.Contains(inv.Warehouse));
                    }

                    if (inventoryDetailsBalQty.Location != null && inventoryDetailsBalQty.Location.Any())
                    {
                        query = query.Where(inv => inventoryDetailsBalQty.Location.Contains(inv.Location));
                    }

                    if (inventoryDetailsBalQty.ProjectNumber != null && inventoryDetailsBalQty.ProjectNumber.Any())
                    {
                        query = query.Where(inv => inventoryDetailsBalQty.ProjectNumber.Contains(inv.ProjectNumber));
                    }
                }

                // Retrieve the filtered inventory items
                var inventoryItems = await query.ToListAsync();

                // Group the inventory items by PartNumber, Warehouse, Location, and ProjectNumber
                var groupedItems = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Inventory>>>>>();

                foreach (var item in inventoryItems)
                {
                    var partNumber = item.PartNumber;
                    var warehouse = item.Warehouse;
                    var location = item.Location;
                    var projectNumber = item.ProjectNumber;

                    if (!groupedItems.ContainsKey(partNumber))
                    {
                        groupedItems[partNumber] = new Dictionary<string, Dictionary<string, Dictionary<string, List<Inventory>>>>();
                    }

                    if (!groupedItems[partNumber].ContainsKey(warehouse))
                    {
                        groupedItems[partNumber][warehouse] = new Dictionary<string, Dictionary<string, List<Inventory>>>();
                    }

                    if (!groupedItems[partNumber][warehouse].ContainsKey(location))
                    {
                        groupedItems[partNumber][warehouse][location] = new Dictionary<string, List<Inventory>>();
                    }

                    if (!groupedItems[partNumber][warehouse][location].ContainsKey(projectNumber))
                    {
                        groupedItems[partNumber][warehouse][location][projectNumber] = new List<Inventory>();
                    }

                    groupedItems[partNumber][warehouse][location][projectNumber].Add(item);
                }

                // Calculate the sum of Balance_Quantity for each group and update the first item in each group
                foreach (var partNumberGroup in groupedItems.Values)
                {
                    foreach (var warehouseGroup in partNumberGroup.Values)
                    {
                        foreach (var locationGroup in warehouseGroup.Values)
                        {
                            foreach (var projectGroup in locationGroup.Values)
                            {
                                var sum = projectGroup.Sum(inv => inv.Balance_Quantity);
                                var firstItem = projectGroup.First();
                                firstItem.Balance_Quantity = sum;
                            }
                        }
                    }
                }

                // Return the updated first items from each group
                return groupedItems.Values
                    .SelectMany(partNumberGroup => partNumberGroup.Values
                        .SelectMany(warehouseGroup => warehouseGroup.Values
                            .SelectMany(locationGroup => locationGroup.Values
                                .Select(projectGroup => projectGroup.First()))));
            }             
        }
        public async Task<IEnumerable<Inventory>> GetInventoryDetailsWithSumOfStock(InventoryBalQty inventoryBalQty)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.Inventory.AsQueryable();

                // Check if inventoryBalQty object is not null
                if (inventoryBalQty == null || (inventoryBalQty.PartNumber.Count == 0 && inventoryBalQty.Warehouse.Count == 0 && inventoryBalQty.Location.Count == 0))
                {
                    query = FindAll().OrderByDescending(x => x.Id);

                }
                else
                {
                    // Apply filtering based on the inventoryBalQty properties if they are not null
                    if (inventoryBalQty.PartNumber != null && inventoryBalQty.PartNumber.Any())
                    {
                        query = query.Where(inv => inventoryBalQty.PartNumber.Contains(inv.PartNumber));
                    }

                    if (inventoryBalQty.Warehouse != null && inventoryBalQty.Warehouse.Any())
                    {
                        query = query.Where(inv => inventoryBalQty.Warehouse.Contains(inv.Warehouse));
                    }

                    if (inventoryBalQty.Location != null && inventoryBalQty.Location.Any())
                    {
                        query = query.Where(inv => inventoryBalQty.Location.Contains(inv.Location));
                    }
                }

                // Retrieve the filtered inventory items
                var inventoryItems = await query.ToListAsync();

                // Group the inventory items by PartNumber, Warehouse, and Location using a for loop
                var groupedItems = new Dictionary<string, List<Inventory>>();
                foreach (var item in inventoryItems)
                {
                    var key = item.PartNumber;
                    if (!groupedItems.ContainsKey(key))
                    {
                        groupedItems[key] = new List<Inventory> { item };
                    }
                    else
                    {
                        groupedItems[key].Add(item);
                    }
                }

                // Calculate the sum of Balance_Quantity for each group and update the first item in the group
                foreach (var group in groupedItems)
                {
                    var sum = group.Value.Sum(inv => inv.Balance_Quantity);
                    var firstItem = group.Value.First();
                    firstItem.Balance_Quantity = sum;
                  
                }

                // Return the updated first items from each group
                return groupedItems.Values.Select(group => group.First());
            }
        }

        public async Task<decimal> GetStockDetailsForAllLocationWarehouseByItemNo(string ItemNumber)
        {
            var inventoryDetail = _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true)
                          .Sum(x => x.Balance_Quantity);

            return inventoryDetail;
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
        public async Task<List<InventoryDetailsLocationStock>> GetInventoryDetailsWithInventoryStock(string partNumber,string warehouse,string location, string projectNumber)
        {
            List<InventoryDetailsLocationStock> inventoryItemNoStock = _tipsWarehouseDbContext.Inventory
                 .Where(x => x.PartNumber == partNumber && x.Warehouse == warehouse && x.Location == location && x.ProjectNumber == projectNumber)
                       .GroupBy(l => new { l.PartNumber,l.Warehouse,l.Location,l.ProjectNumber })
                       .Select(group => new InventoryDetailsLocationStock
                       {
                           PartNumber = group.Key.PartNumber,
                           Warehouse = group.Key.Warehouse,
                           ProjectNumber = group.Key.ProjectNumber,
                           Location = group.Key.Location,
                           LocationStock = group.Sum(c => c.Balance_Quantity),
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
            var query = FindAll();
            if (!string.IsNullOrWhiteSpace(searchParams.SearchValue))
            {
                string searchTerm = searchParams.SearchValue.Trim();

                query = query.Where(inv =>
                    inv.ProjectNumber.Contains(searchTerm) ||
                    inv.PartNumber.Contains(searchTerm) ||
                    inv.Description.Contains(searchTerm) ||
                    inv.MftrPartNumber.Contains(searchTerm) ||
                    inv.Warehouse.Contains(searchTerm) ||
                    inv.PartType.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    inv.Location.Contains(searchTerm)
                );
            }

            var sortedQuery = query.OrderByDescending(x => x.Id);

            return PagedList<Inventory>.ToPagedList(sortedQuery, pagingParameter.PageNumber, pagingParameter.PageSize);
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
            var getInventoryDetailsById = await _tipsWarehouseDbContext.Inventory.Where(x => x.GrinNo == GrinNo &&
                                        x.GrinPartId == GrinPartsId && x.PartNumber == ItemNumber && 
                                        x.ProjectNumber == ProjectNumber && x.IsStockAvailable==true)
                          .FirstOrDefaultAsync();

            return getInventoryDetailsById;
        }
        public async Task<IEnumerable<Inventory>> GetInventoryDetailsByItemNumberandLocation(string ItemNumber, string Location, string Warehouse, string projectNumber)

        {
            var getInventoryDetailsByItemAndLoc = await _tipsWarehouseDbContext.Inventory
                .Where(x => x.PartNumber == ItemNumber && x.ProjectNumber == projectNumber && x.Location == Location && x.Warehouse == Warehouse && x.IsStockAvailable == true).ToListAsync();

            return getInventoryDetailsByItemAndLoc;
        }
        public async Task<IEnumerable<Inventory>> GetInventoryDetailsByItemNoandLocationandwarehouse(string ItemNumber, string Location, string Warehouse)

        {
            var getInventoryDetailsByItemAndLoc = await _tipsWarehouseDbContext.Inventory
                .Where(x => x.PartNumber == ItemNumber && x.Location == Location && x.Warehouse == Warehouse && x.IsStockAvailable == true).ToListAsync();

            return getInventoryDetailsByItemAndLoc;
        }
        public async Task<Inventory> GetSingleInventoryDetailsByItemNumberandLocation(string ItemNumber, string Location, string Warehouse)
        {
            var getInventoryDetailsByItemAndLoc = await _tipsWarehouseDbContext.Inventory
                .Where(x => x.PartNumber == ItemNumber && x.Location == Location && x.Warehouse == Warehouse && x.IsStockAvailable == true).FirstOrDefaultAsync();

            return getInventoryDetailsByItemAndLoc;
        } 
        public async Task<List<Inventory>> GetInventoryDetailsByItemNoandProjectNo(string ItemNumber, string ProjectNo)
        {
            
            var inventoryDetail = await _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == ItemNumber 
            && x.IsStockAvailable == true && x.ProjectNumber == ProjectNo && x.Location != "Reject" )
                          .ToListAsync();

            return inventoryDetail;
        }

        //Get Inventory WIP from location and warehouse
        public async Task<List<Inventory>> GetWIPInventoryDetailsByItemNo(string ItemNumber, string ShopOrderNumber)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == ItemNumber
            && x.IsStockAvailable == true && x.Location == "WIP" && x.Warehouse == "WIP" && x.shopOrderNo == ShopOrderNumber)
                          .ToListAsync();
            return inventoryDetail;
        }


        public async Task<List<Inventory>> GetInventoryByItemNo(string itemNumber)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == itemNumber
            && x.IsStockAvailable == true )
                          .ToListAsync();
            return inventoryDetail;
        }

        public async Task<ConsumptionInventoryDto> GetConsumptionInventoryByItemNo(string itemNumber)
        {
            var inventoryDetails = await _tipsWarehouseDbContext.Inventory
            .Where(x => x.PartNumber == itemNumber && x.IsStockAvailable == true)
            .GroupBy(x => x.PartNumber)  
            .Select(group => new ConsumptionInventoryDto
            {
                PartNumber = group.Key,
                Balance_Quantity = group.Sum(x => x.Balance_Quantity) // Calculate the sum of quantities
            })
            .FirstOrDefaultAsync();

            return inventoryDetails;
        }
         
        public async Task<IEnumerable<Inventory>> GetInventoryDetailsByItemNoandPartTypes(string ItemNumber)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == ItemNumber  && x.IsStockAvailable == true)
                          .ToListAsync();

            return inventoryDetail;
        }


        public async Task<decimal> GetStockQtyForRetailSalesOrderItem(string ItemNumber)
        {
            var inventoryDetail = _tipsWarehouseDbContext.Inventory
                .Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true)
                          .Sum(x=>x.Balance_Quantity);

            return inventoryDetail;
        }


        public async Task<decimal> GetStockQtyForBtpSalesOrderItem(string ItemNumber,List<string> shopOrderNumbers)
        {
            var inventoryDetail = _tipsWarehouseDbContext.Inventory
                .Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true && shopOrderNumbers.Contains(x.shopOrderNo))
                          .Sum(x => x.Balance_Quantity);

            return inventoryDetail;
        }


        public async Task<Inventory> GetInventoryDetailsByItemAndProjectNo(string itemNumber, string projectNumber)
        {
            var inventoryDetailsByProjectNo = await _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == itemNumber && x.ProjectNumber == projectNumber && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();
            return inventoryDetailsByProjectNo;
        }

        public async Task<List<InventoryBalanceQtyMaterialIssue>> GetInventoryStockByItemAndProjectNo(string itemNumber, string projectNumber)
        {

            List<InventoryBalanceQtyMaterialIssue> result = await _tipsWarehouseDbContext.Inventory
                   .Where(x => x.PartNumber == itemNumber && x.ProjectNumber == projectNumber && x.IsStockAvailable == true)
                   .GroupBy(l => new { l.PartNumber, l.ProjectNumber })
                   .Select(group => new InventoryBalanceQtyMaterialIssue
                   {
                       PartNumber = group.Key.PartNumber, 
                       BalanceQty = group.Sum(c => c.Balance_Quantity),
                       ProjectNumber = group.Key.ProjectNumber
                   }).ToListAsync();

            return result;
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

            var partTypes = new PartType[] { PartType.FG, PartType.TG, PartType.FRU };

            var getSalesOrderDetailsBy = await _tipsWarehouseDbContext.Inventory
                .Where(x => x.PartNumber == ItemNumber && partTypes.Contains(x.PartType) && x.IsStockAvailable)
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

    }
}
