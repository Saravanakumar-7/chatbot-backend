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
using System.Collections;
using System.Security.Claims;

namespace Tips.Warehouse.Api.Repository
{
    public class InventoryRepository : RepositoryBase<Inventory>, IInventoryRepository
    {
        private readonly string _connectionString;
        private readonly MySqlConnection _connection;
        private TipsWarehouseDbContext _tipsWarehouseDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;

        public InventoryRepository(TipsWarehouseDbContext repositoryContext, IHttpContextAccessor httpContextAccessor, MySqlConnection connection) : base(repositoryContext)
        {
            _connection = connection;
            _tipsWarehouseDbContext = repositoryContext;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        public async Task<IEnumerable<Inventory>> SearchInventoryDetailsWithSumOfStock(InventoryItemNo inventoryItemNo)
        {

            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.Inventories.AsQueryable();

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
        //public async Task<IEnumerable<Inventory>> GetInventoryDetailsWithSumOfBalQty(InventoryDetailsBalQty inventoryDetailsBalQty)
        //{
        //    using (var context = _tipsWarehouseDbContext)
        //    {
        //        var query = _tipsWarehouseDbContext.Inventories.AsQueryable();

        //        // Check if inventoryBalQty object is not null
        //        if (inventoryDetailsBalQty == null || (inventoryDetailsBalQty.PartNumber.Count == 0 && inventoryDetailsBalQty.Warehouse.Count == 0 && inventoryDetailsBalQty.Location.Count == 0 && inventoryDetailsBalQty.ProjectNumber.Count == 0))
        //        {
        //            query = FindAll().OrderByDescending(x => x.Id);

        //        }
        //        else
        //        {
        //            // Apply filtering based on the inventoryBalQty properties if they are not null
        //            if (inventoryDetailsBalQty.PartNumber != null && inventoryDetailsBalQty.PartNumber.Any())
        //            {
        //                query = query.Where(inv => inventoryDetailsBalQty.PartNumber.Contains(inv.PartNumber));
        //            }

        //            if (inventoryDetailsBalQty.Warehouse != null && inventoryDetailsBalQty.Warehouse.Any())
        //            {
        //                //query = query.Where(inv => inventoryDetailsBalQty.Warehouse.Contains(inv.Warehouse));
        //                query = query.Where(inv => inventoryDetailsBalQty.Warehouse.Contains(inv.Warehouse) && inv.Warehouse != "WIP");

        //            }

        //            if (inventoryDetailsBalQty.Location != null && inventoryDetailsBalQty.Location.Any())
        //            {
        //                //query = query.Where(inv => inventoryDetailsBalQty.Location.Contains(inv.Location));
        //                query = query.Where(inv => inventoryDetailsBalQty.Location.Contains(inv.Location) && inv.Location != "WIP");

        //            }

        //            if (inventoryDetailsBalQty.ProjectNumber != null && inventoryDetailsBalQty.ProjectNumber.Any())
        //            {
        //                query = query.Where(inv => inventoryDetailsBalQty.ProjectNumber.Contains(inv.ProjectNumber));
        //            }
        //        }

        //        var inventoryItems = await query.ToListAsync();

        //        var groupedItems = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Inventory>>>>>();

        //        foreach (var item in inventoryItems)
        //        {
        //            var partNumber = item.PartNumber;
        //            var warehouse = item.Warehouse;
        //            var location = item.Location;
        //            var projectNumber = item.ProjectNumber;

        //            if (!groupedItems.ContainsKey(partNumber))
        //            {
        //                groupedItems[partNumber] = new Dictionary<string, Dictionary<string, Dictionary<string, List<Inventory>>>>();
        //            }

        //            if (!groupedItems[partNumber].ContainsKey(warehouse))
        //            {
        //                groupedItems[partNumber][warehouse] = new Dictionary<string, Dictionary<string, List<Inventory>>>();
        //            }

        //            if (!groupedItems[partNumber][warehouse].ContainsKey(location))
        //            {
        //                groupedItems[partNumber][warehouse][location] = new Dictionary<string, List<Inventory>>();
        //            }

        //            if (!groupedItems[partNumber][warehouse][location].ContainsKey(projectNumber))
        //            {
        //                groupedItems[partNumber][warehouse][location][projectNumber] = new List<Inventory>();
        //            }

        //            groupedItems[partNumber][warehouse][location][projectNumber].Add(item);
        //        }

        //        foreach (var partNumberGroup in groupedItems.Values)
        //        {
        //            foreach (var warehouseGroup in partNumberGroup.Values)
        //            {
        //                foreach (var locationGroup in warehouseGroup.Values)
        //                {
        //                    foreach (var projectGroup in locationGroup.Values)
        //                    {
        //                        var sum = projectGroup.Sum(inv => inv.Balance_Quantity);
        //                        var firstItem = projectGroup.First();
        //                        firstItem.Balance_Quantity = sum;
        //                    }
        //                }
        //            }
        //        }

        //        return groupedItems.Values
        //            .SelectMany(partNumberGroup => partNumberGroup.Values
        //                .SelectMany(warehouseGroup => warehouseGroup.Values
        //                    .SelectMany(locationGroup => locationGroup.Values
        //                        .Select(projectGroup => projectGroup.First()))));
        //    }
        //}
        public async Task<IEnumerable<Inventory>> GetInventoryDetailsWithSumOfBalQty(InventoryDetailsBalQty inventoryDetailsBalQty)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.Inventories.AsQueryable();

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
                        query = query.Where(inv => inventoryDetailsBalQty.Warehouse.Contains(inv.Warehouse) && inv.Warehouse != "WIP");
                    }

                    if (inventoryDetailsBalQty.Location != null && inventoryDetailsBalQty.Location.Any())
                    {
                        query = query.Where(inv => inventoryDetailsBalQty.Location.Contains(inv.Location) && inv.Location != "WIP");
                    }

                    if (inventoryDetailsBalQty.ProjectNumber != null && inventoryDetailsBalQty.ProjectNumber.Any())
                    {
                        query = query.Where(inv => inventoryDetailsBalQty.ProjectNumber.Contains(inv.ProjectNumber));
                    }
                }

                var inventoryItems = await query.ToListAsync();

                var groupedItems = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<Inventory>>>>>();

                foreach (var item in inventoryItems)
                {
                    var partNumber = item.PartNumber;
                    var warehouse = item.Warehouse;
                    var location = item.Location;
                    var projectNumber = item.ProjectNumber;

                    // Skip items with 'WIP' Location or Warehouse
                    if (warehouse == "WIP" || location == "WIP")
                    {
                        continue;
                    }

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
                var query = _tipsWarehouseDbContext.Inventories.AsQueryable();

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
                        query = query.Where(inv => inventoryBalQty.PartNumber.Contains(inv.PartNumber) && inv.Warehouse != "Grin" && inv.Warehouse != "IQC");
                    }

                    if (inventoryBalQty.Warehouse != null && inventoryBalQty.Warehouse.Any())
                    {
                        query = query.Where(inv => inventoryBalQty.Warehouse.Contains(inv.Warehouse) && inv.Warehouse != "Grin" && inv.Warehouse != "IQC");
                    }

                    if (inventoryBalQty.Location != null && inventoryBalQty.Location.Any())
                    {
                        query = query.Where(inv => inventoryBalQty.Location.Contains(inv.Location) && inv.Location != "Grin" && inv.Warehouse != "IQC");
                    }
                    //if (inventoryBalQty.PartNumber != null && inventoryBalQty.PartNumber.Any())
                    //{
                    //    query = query.Where(inv => inventoryBalQty.PartNumber.Contains(inv.PartNumber));
                    //}

                    //if (inventoryBalQty.Warehouse != null && inventoryBalQty.Warehouse.Any())
                    //{
                    //    query = query.Where(inv => inventoryBalQty.Warehouse.Contains(inv.Warehouse));
                    //}

                    //if (inventoryBalQty.Location != null && inventoryBalQty.Location.Any())
                    //{
                    //    query = query.Where(inv => inventoryBalQty.Location.Contains(inv.Location));
                    //}
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
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
            var inventoryDetail = _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true && !skipWareHouse.Contains(x.Warehouse))
                          .Sum(x => x.Balance_Quantity);

            return inventoryDetail;
        }
        public async Task<decimal> GetStockDetailsForAllLocationWarehouseByItemNoAndProjectNo(string ItemNumber,string projectNo)
        {
            var inventoryDetail = _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == ItemNumber && 
            x.ProjectNumber == projectNo  && x.IsStockAvailable == true)
                          .Sum(x => x.Balance_Quantity);

            return inventoryDetail;
        }
        public async Task<IEnumerable<Inventory>> GetAllInventoryWithItems(InventorySearchDto inventorySearch)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.Inventories.AsQueryable();
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
            var inventoryDetails = _tipsWarehouseDbContext.Inventories
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
                var query = _tipsWarehouseDbContext.Inventories.AsQueryable();
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
            List<InventoryItemNoStock> inventoryItemNoStock = _tipsWarehouseDbContext.Inventories
                       .GroupBy(l => new { l.PartNumber })
                       .Select(group => new InventoryItemNoStock
                       {
                           PartNumber = group.Key.PartNumber,
                           Balance_Quantity = group.Sum(c => c.Balance_Quantity),

                       }).ToList();

            return inventoryItemNoStock;
        }
        public async Task<List<InventoryDetailsLocationStock>> GetInventoryDetailsWithInventoryStock(string partNumber, string warehouse, string location, string projectNumber)
        {
            List<InventoryDetailsLocationStock> inventoryItemNoStock = _tipsWarehouseDbContext.Inventories
                 .Where(x => x.PartNumber == partNumber && x.Warehouse == warehouse && x.Location == location && x.ProjectNumber == projectNumber)
                       .GroupBy(l => new { l.PartNumber, l.Warehouse, l.Location, l.ProjectNumber })
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
            inventory.CreatedBy = _createdBy;
            inventory.CreatedOn = DateTime.Now;
            inventory.Unit = _unitname;
            if (inventory.Balance_Quantity == 0) inventory.IsStockAvailable = false;
            else inventory.IsStockAvailable = true;
            var result = await Create(inventory);

            return result.Id;
        }

        public async Task<string> DeleteInventory(Inventory inventory)
        {
            Delete(inventory);
            string result = $"NaterialIssue details of {inventory.Id} is deleted successfully!";
            return result;
        }
        public async Task<Inventory?> GetInventorybyItemProjectWarehouseLocation(string itemNumber, string projectNumber, string warehouse, string location)
        {
            var invdetails = await _tipsWarehouseDbContext.Inventories
                .Where(x => x.PartNumber == itemNumber && x.ProjectNumber == projectNumber && x.Warehouse == warehouse && x.Location == location).FirstOrDefaultAsync();
            return invdetails;
        }
        public async Task<List<InventoryQtyforDO>> GetInventorybyItemandProject(string itemNumber, string projectNumber)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
            var invdetails = await FindAll().Where(x => x.PartNumber == itemNumber && x.ProjectNumber == projectNumber && !skipWareHouse.Contains(x.Warehouse)).Select(x => new GetInventoryQtyforDO()
            {
                Warehouse = x.Warehouse,
                Location = x.Location,
                BalanceQty = x.Balance_Quantity
            }
            ).ToListAsync();

            List<InventoryQtyforDO> exists = new List<InventoryQtyforDO>();
            foreach (var getitem in invdetails)
            {
                InventoryQtyforDO newly = new InventoryQtyforDO();
                List<InventoryQtyforDOLocation> newlocations = new List<InventoryQtyforDOLocation>();
                int existflag = 0;
                foreach (var ex in exists)
                {
                    if (getitem.Warehouse == ex.Warehouse)
                    {
                        existflag = 1;
                        int locflag = 0;
                        InventoryQtyforDOLocation locations = new InventoryQtyforDOLocation();
                        foreach (var exloco in ex.inventoryQtyforDOLocations)
                        {
                            if (getitem.Location == exloco.Location)
                            {
                                locflag = 1;
                                exloco.BalanceQty = exloco.BalanceQty + getitem.BalanceQty;
                            }
                        }
                        if (locflag == 0)
                        {
                            locations.Location = getitem.Location;
                            locations.BalanceQty = getitem.BalanceQty;
                            ex.inventoryQtyforDOLocations.Add(locations);
                        }
                    }
                }
                if (existflag == 0)
                {
                    InventoryQtyforDOLocation newlylocations = new InventoryQtyforDOLocation();
                    newly.Warehouse = getitem.Warehouse;
                    newlylocations.Location = getitem.Location;
                    newlylocations.BalanceQty = getitem.BalanceQty;
                    newlocations.Add(newlylocations);
                    newly.inventoryQtyforDOLocations = newlocations;
                    exists.Add(newly);
                }
            }


            return exists;
        }
        public async Task<List<InventoryQtyforDO>> GetInventorybyItem(string itemNumber)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };            
            var invdetails = await FindAll().Where(x => x.PartNumber == itemNumber && !skipWareHouse.Contains(x.Warehouse)).Select(x => new GetInventoryQtyforDO()
            {
                Warehouse = x.Warehouse,
                Location = x.Location,
                BalanceQty = x.Balance_Quantity
            }
            ).ToListAsync();

            List<InventoryQtyforDO> exists = new List<InventoryQtyforDO>();
            foreach (var getitem in invdetails)
            {
                InventoryQtyforDO newly = new InventoryQtyforDO();
                List<InventoryQtyforDOLocation> newlocations = new List<InventoryQtyforDOLocation>();
                int existflag = 0;
                foreach (var ex in exists)
                {
                    if (getitem.Warehouse == ex.Warehouse)
                    {
                        existflag = 1;
                        int locflag = 0;
                        InventoryQtyforDOLocation locations = new InventoryQtyforDOLocation();
                        foreach (var exloco in ex.inventoryQtyforDOLocations)
                        {
                            if (getitem.Location == exloco.Location)
                            {
                                locflag = 1;
                                exloco.BalanceQty = exloco.BalanceQty + getitem.BalanceQty;
                            }
                        }
                        if (locflag == 0)
                        {
                            locations.Location = getitem.Location;
                            locations.BalanceQty = getitem.BalanceQty;
                            ex.inventoryQtyforDOLocations.Add(locations);
                        }
                    }
                }
                if (existflag == 0)
                {
                    InventoryQtyforDOLocation newlylocations = new InventoryQtyforDOLocation();
                    newly.Warehouse = getitem.Warehouse;
                    newlylocations.Location = getitem.Location;
                    newlylocations.BalanceQty = getitem.BalanceQty;
                    newlocations.Add(newlylocations);
                    newly.inventoryQtyforDOLocations = newlocations;
                    exists.Add(newly);
                }
            }


            return exists;
        }
        public async Task UpdateInventoryforBTO(List<BtoDeliveryOrderItemQtyDistribution> bToitemDis)
        {
            var itemNumber = bToitemDis.Select(x => x.PartNumber).FirstOrDefault();
            var projectNumber = bToitemDis.Select(x => x.ProjectNumber).FirstOrDefault();
            var invdetails = await FindAll().Where(x => x.PartNumber == itemNumber && x.ProjectNumber == projectNumber).ToListAsync();
            foreach (var eachDis in bToitemDis)
            {
                foreach (var eachinv in invdetails)
                {
                    if (eachinv.Warehouse == eachDis.Warehouse && eachinv.Location == eachDis.Location)
                    {
                        if (eachDis.DistributingQty <= eachinv.Balance_Quantity)
                        {
                            eachinv.Balance_Quantity = eachinv.Balance_Quantity - eachDis.DistributingQty;
                            if (eachinv.Balance_Quantity == 0)
                            {
                                eachinv.IsStockAvailable = false;
                            }
                            Update(eachinv);
                            //SaveAsync();
                            break;
                            //invdetails.Remove(eachinv);
                        }
                        else if (eachDis.DistributingQty > eachinv.Balance_Quantity)
                        {
                            eachDis.DistributingQty = eachDis.DistributingQty - eachinv.Balance_Quantity;
                            eachinv.Balance_Quantity = 0;
                            eachinv.IsStockAvailable = false;
                            Update(eachinv);
                            //SaveAsync();
                            //invdetails.Remove(eachinv);
                        }
                    }
                }
            }
        }
        public async Task UpdateInventoryforBTO_Keus(List<BtoDeliveryOrderItemQtyDistribution> bToitemDis)
        {
            var itemNumber = bToitemDis.Select(x => x.PartNumber).FirstOrDefault();
           // var projectNumber = bToitemDis.Select(x => x.ProjectNumber).FirstOrDefault();
            var invdetails = await FindAll().Where(x => x.PartNumber == itemNumber).ToListAsync();
            foreach (var eachDis in bToitemDis)
            {
                foreach (var eachinv in invdetails)
                {
                    if (eachinv.Warehouse == eachDis.Warehouse && eachinv.Location == eachDis.Location)
                    {
                        if (eachDis.DistributingQty <= eachinv.Balance_Quantity)
                        {
                            eachinv.Balance_Quantity = eachinv.Balance_Quantity - eachDis.DistributingQty;
                            if (eachinv.Balance_Quantity == 0)
                            {
                                eachinv.IsStockAvailable = false;
                            }
                            Update(eachinv);
                            //SaveAsync();
                            break;
                            //invdetails.Remove(eachinv);
                        }
                        else if (eachDis.DistributingQty > eachinv.Balance_Quantity)
                        {
                            eachDis.DistributingQty = eachDis.DistributingQty - eachinv.Balance_Quantity;
                            eachinv.Balance_Quantity = 0;
                            eachinv.IsStockAvailable = false;
                            Update(eachinv);
                            //SaveAsync();
                            //invdetails.Remove(eachinv);
                        }
                    }
                }
            }
        }
        public async Task UpdateInventoryforODO(List<OpenDeliveryOrderPartsQtyDistribution> ODOitemDis)
        {
            var itemNumber = ODOitemDis.Select(x => x.PartNumber).FirstOrDefault();
            var projectNumber = ODOitemDis.Select(x => x.ProjectNumber).FirstOrDefault();
            var invdetails = await FindAll().Where(x => x.PartNumber == itemNumber && x.ProjectNumber == projectNumber).ToListAsync();
            foreach (var eachDis in ODOitemDis)
            {
                foreach (var eachinv in invdetails)
                {
                    if (eachinv.Warehouse == eachDis.Warehouse && eachinv.Location == eachDis.Location)
                    {
                        if (eachDis.DistributingQty <= eachinv.Balance_Quantity)
                        {
                            eachinv.Balance_Quantity = eachinv.Balance_Quantity - eachDis.DistributingQty;
                            if (eachinv.Balance_Quantity == 0)
                            {
                                eachinv.IsStockAvailable = false;
                            }
                            Update(eachinv);
                           // SaveAsync();
                            break;
                            //invdetails.Remove(eachinv);
                        }
                        else if (eachDis.DistributingQty > eachinv.Balance_Quantity)
                        {
                            eachDis.DistributingQty = eachDis.DistributingQty - eachinv.Balance_Quantity;
                            eachinv.Balance_Quantity = 0;
                            eachinv.IsStockAvailable = false;
                            Update(eachinv);
                           // SaveAsync();
                            //invdetails.Remove(eachinv);
                        }
                    }
                }
            }
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
        public async Task<Inventory> GetInventoryDetailsByItemNoandProjectNoandShopOrderNo(string ItemNumber, string ProjectNumber, string shopOrderNo)
        {
            var getInventoryDetailsById = await _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == ItemNumber && x.ProjectNumber == ProjectNumber && x.shopOrderNo == shopOrderNo)

                          .FirstOrDefaultAsync();

            return getInventoryDetailsById;
        }

        public async Task<Inventory> GetInventoryDetailsByGrinNo(string GrinNo, string ItemNumber, string ProjectNumber)
        {
            var getInventoryDetailsById = await _tipsWarehouseDbContext.Inventories.Where(x => x.GrinNo == GrinNo && x.PartNumber == ItemNumber && x.ProjectNumber == ProjectNumber)

                          .FirstOrDefaultAsync();

            return getInventoryDetailsById;
        }
        //passing grinid

        public async Task<Inventory> GetInventoryDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId, string ItemNumber, string ProjectNumber)
        {
            var getInventoryDetailsById = await _tipsWarehouseDbContext.Inventories.Where(x => x.GrinNo == GrinNo &&
                                        x.GrinPartId == GrinPartsId && x.PartNumber == ItemNumber &&
                                        x.ProjectNumber == ProjectNumber && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();

            return getInventoryDetailsById;
        }
        public async Task<IEnumerable<GetInventoryItemNoAndDescriptionList>> GetInventoryItemNoAndDescriptionByProjectNo(string projectNumber)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };

            IEnumerable<GetInventoryItemNoAndDescriptionList> inventoryDetails = await _tipsWarehouseDbContext.Inventories.
                Where(x => x.ProjectNumber == projectNumber && !skipWareHouse.Contains(x.Warehouse) && !skipWareHouse.Contains(x.Location))
                 .Select(x => new GetInventoryItemNoAndDescriptionList()
                 {
                     PartNumber = x.PartNumber,
                     Description = x.Description

                 })
                 .ToListAsync();

            return inventoryDetails;
        }
        public async Task<IEnumerable<GetInventoryItemNoAndDescriptionList>> GetInventoryItemNoAndDescriptionList()
        {
            IEnumerable<GetInventoryItemNoAndDescriptionList> getInventoryDetailsById = await _tipsWarehouseDbContext.Inventories
                 .Select(x => new GetInventoryItemNoAndDescriptionList()
                 {
                     PartNumber = x.PartNumber,
                     Description = x.Description

                 })
                 .ToListAsync();

            return getInventoryDetailsById;
        }
        public async Task<IEnumerable<Inventory>> GetInventoryDetailsByItemNumberandLocation(string ItemNumber, string Location, string Warehouse, string projectNumber)

        {
            var getInventoryDetailsByItemAndLoc = await _tipsWarehouseDbContext.Inventories
                .Where(x => x.PartNumber == ItemNumber && x.ProjectNumber == projectNumber && x.Location == Location && x.Warehouse == Warehouse && x.IsStockAvailable == true).ToListAsync();

            return getInventoryDetailsByItemAndLoc;
        }
        public async Task<IEnumerable<Inventory>> GetInventoryDetailsByItemNoandLocationandwarehouse(string ItemNumber, string Location, string Warehouse, string projectNumber)

        {
            var getInventoryDetailsByItemAndLoc = await _tipsWarehouseDbContext.Inventories
                .Where(x => x.PartNumber == ItemNumber && x.Location == Location && x.Warehouse == Warehouse
                && x.IsStockAvailable == true && x.ProjectNumber == projectNumber).ToListAsync();

            return getInventoryDetailsByItemAndLoc;
        }
        public async Task<Inventory> GetSingleInventoryDetailsByItemNumberandLocation(string ItemNumber, string Location, string Warehouse)
        {
            var getInventoryDetailsByItemAndLoc = await _tipsWarehouseDbContext.Inventories
                .Where(x => x.PartNumber == ItemNumber && x.Location == Location && x.Warehouse == Warehouse && x.IsStockAvailable == true).FirstOrDefaultAsync();

            return getInventoryDetailsByItemAndLoc;
        }
        public async Task<List<Inventory>> GetInventoryDetailsByItemNoandProjectNo(string ItemNumber, string ProjectNo)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
            var inventoryDetail = await _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == ItemNumber
            && x.IsStockAvailable == true && x.ProjectNumber == ProjectNo && !skipWareHouse.Contains(x.Warehouse))
                          .ToListAsync();

            return inventoryDetail;
        }

        //Get Inventory WIP from location and warehouse
        public async Task<List<Inventory>> GetWIPInventoryDetailsByItemNo(string ItemNumber, string ShopOrderNumber)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == ItemNumber
            && x.IsStockAvailable == true && x.Location == "WIP" && x.Warehouse == "WIP" && x.shopOrderNo == ShopOrderNumber)
                          .ToListAsync();
            return inventoryDetail;
        }


        public async Task<List<Inventory>> GetInventoryByItemNo(string itemNumber)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == itemNumber
           /* && x.IsStockAvailable == true*/)
                          .ToListAsync();
            return inventoryDetail;
        }

        public async Task<decimal> GetInventoryBySAItemNo(string itemNumber)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventories
                .Where(x => x.PartNumber == itemNumber && x.IsStockAvailable == true && x.PartType == PartType.SA)
                .SumAsync(x => x.Balance_Quantity);
            return inventoryDetail;
        }
        //aravind
        public async Task<List<ConsumptionInventoryDto>> GetConsumptionInventoryByItemNotest(List<string> ItemNumberList)
        {
            List<PartType>? partTypes = new List<PartType> { PartType.FG, PartType.TG };

            var inventoryDetails = await _tipsWarehouseDbContext.Inventories
            .Where(x => ItemNumberList.Contains(x.PartNumber) && x.IsStockAvailable == true && x.Balance_Quantity > 0
            && partTypes.Contains(x.PartType) /*&& x.Warehouse =="FG"*/)
            .GroupBy(x => x.PartNumber)
            .Select(group => new ConsumptionInventoryDto
            {
                PartNumber = group.Key,
                Balance_Quantity = group.Sum(x => x.Balance_Quantity) // Calculate the sum of quantities
            })
            .ToListAsync();

            return inventoryDetails;
        }
        public async Task<List<ConsumptionInventoryByProjectNoDto>> GetConsumptionInventoryByItemNoAndProjectNotest1(string ItemNumber,string projectNo)
        {
            List<PartType>? partTypes = new List<PartType> { PartType.FG, PartType.TG };

            var inventoryDetails = await _tipsWarehouseDbContext.Inventories
            .Where(x => x.PartNumber == ItemNumber && x.ProjectNumber == projectNo && x.IsStockAvailable == true && x.Balance_Quantity > 0
            && partTypes.Contains(x.PartType) /*&& x.Warehouse =="FG"*/)
            .GroupBy(x =>new { x.PartNumber,x.ProjectNumber })
            .Select(group => new ConsumptionInventoryByProjectNoDto
            {
                PartNumber = group.Key.PartNumber,
                ProjectNumber = group.Key.ProjectNumber,
                Balance_Quantity = group.Sum(x => x.Balance_Quantity) // Calculate the sum of quantities
            })
            .ToListAsync();

            return inventoryDetails;
        }

        public async Task<ConsumptionInventoryDto> GetConsumptionInventoryByItemNo(string itemNumber)
        {
            var partTypes = new PartType[] { PartType.FG, PartType.TG };

            var inventoryDetails = await _tipsWarehouseDbContext.Inventories
            .Where(x => x.PartNumber == itemNumber && x.IsStockAvailable == true && x.Balance_Quantity > 0
            && partTypes.Contains(x.PartType) /*&& x.Warehouse =="FG"*/)
            .GroupBy(x => x.PartNumber)
            .Select(group => new ConsumptionInventoryDto
            {
                PartNumber = group.Key,
                Balance_Quantity = group.Sum(x => x.Balance_Quantity) // Calculate the sum of quantities
            })
            .FirstOrDefaultAsync();

            return inventoryDetails;
        }


        //passing Project Number and itemnumber
        public async Task<ConsumptionInventoryDto> GetConsumptionInventoryByItemNoAndProjectNo(string itemNumber, string projectNumber)
        {
            var partTypes = new PartType[] { PartType.FG, PartType.TG };

            var inventoryDetails = await _tipsWarehouseDbContext.Inventories
            .Where(x => x.PartNumber == itemNumber && x.IsStockAvailable == true && x.ProjectNumber == projectNumber && x.Balance_Quantity > 0
            && partTypes.Contains(x.PartType) /*&& x.Warehouse =="FG"*/)
            .GroupBy(x => x.PartNumber)
            .Select(group => new ConsumptionInventoryDto
            {
                PartNumber = group.Key,
                Balance_Quantity = group.Sum(x => x.Balance_Quantity) // Calculate the sum of quantities
            })
            .FirstOrDefaultAsync();

            return inventoryDetails;
        }

        public async Task<List<ConsumptionChildItemInventoryDto>> GetConsumptionChildItemStockWithWipQty(List<string> itemNumberList)
        {
            var partTypes = new PartType[] { PartType.PurchasePart };
            string[] skipWareHouse = { "Reject", "Scrap", "Rework", "FG" };

            string wipWarehouse = "WIP";

            var itemStock = await _tipsWarehouseDbContext.Inventories
                .Where(x => itemNumberList.Contains(x.PartNumber) && x.IsStockAvailable == true && x.Balance_Quantity > 0 &&
                            partTypes.Contains(x.PartType) && !skipWareHouse.Contains(x.Warehouse))
                .GroupBy(x => x.PartNumber)
                .Select(group => new ConsumptionChildItemInventoryDto
                {
                    PartNumber = group.Key,
                    BalanceQuantity = group.Any(x => x.Warehouse != wipWarehouse)
                        ? group.Where(x => x.Warehouse != wipWarehouse).Sum(x => x.Balance_Quantity)
                        : 0, // Sum Balance_Quantity only when Warehouse is not "WIP"
                    WipQuantity = group.Any(x => x.Warehouse == wipWarehouse)
                        ? group.Where(x => x.Warehouse == wipWarehouse).Sum(x => x.Balance_Quantity)
                        : 0 // Set WIPQty based on presence of WIP warehouse
                })
                .ToListAsync();

            return itemStock;
        }
        public async Task<List<ConsumptionChildItemInventoryDto>> GetConsumptionChildItemStockWithWipQtyByProjectNo(string projectNo,List<string> itemNumberList)
        {
            var partTypes = new PartType[] { PartType.PurchasePart };
            string[] skipWareHouse = { "Reject", "Scrap", "Rework", "FG" };

            string wipWarehouse = "WIP";

            var itemStock = await _tipsWarehouseDbContext.Inventories
                .Where(x => itemNumberList.Contains(x.PartNumber)&& x.ProjectNumber == projectNo && x.IsStockAvailable == true && x.Balance_Quantity > 0 &&
                            partTypes.Contains(x.PartType) && !skipWareHouse.Contains(x.Warehouse))
                .GroupBy(x => x.PartNumber)
                .Select(group => new ConsumptionChildItemInventoryDto
                {
                    PartNumber = group.Key,
                    BalanceQuantity = group.Any(x => x.Warehouse != wipWarehouse)
                        ? group.Where(x => x.Warehouse != wipWarehouse).Sum(x => x.Balance_Quantity)
                        : 0, // Sum Balance_Quantity only when Warehouse is not "WIP"
                    WipQuantity = group.Any(x => x.Warehouse == wipWarehouse)
                        ? group.Where(x => x.Warehouse == wipWarehouse).Sum(x => x.Balance_Quantity)
                        : 0 // Set WIPQty based on presence of WIP warehouse
                })
                .ToListAsync();

            return itemStock;
        }


        public async Task<IEnumerable<Inventory>> GetInventoryDetailsByItemNoandPartTypes(string ItemNumber)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true)
                          .ToListAsync();

            return inventoryDetail;
        }


        public async Task<decimal> GetStockQtyForRetailSalesOrderItem(string ItemNumber)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
            var inventoryDetail = _tipsWarehouseDbContext.Inventories
                .Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true && !skipWareHouse.Contains(x.Warehouse))
                          .Sum(x => x.Balance_Quantity);

            return inventoryDetail;
            //var inventoryDetail = _tipsWarehouseDbContext.Inventories
            //    .Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true)
            //              .Sum(x=>x.Balance_Quantity);

            //return inventoryDetail;
        }
        //aravind

        public async Task<decimal> GetStockQtyForBtpSalesOrderItem(string ItemNumber, List<string> shopOrderNumbers)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };

            var inventoryDetail = _tipsWarehouseDbContext.Inventories
                .Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true && shopOrderNumbers.Contains(x.shopOrderNo)
                && !skipWareHouse.Contains(x.Warehouse)
                )
                          .Sum(x => x.Balance_Quantity);

            return inventoryDetail;
        }


        public async Task<Inventory> GetInventoryDetailsByItemAndProjectNo(string itemNumber, string projectNumber)
        {
            var inventoryDetailsByProjectNo = await _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == itemNumber && x.ProjectNumber == projectNumber && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();
            return inventoryDetailsByProjectNo;
        }

        public async Task<List<InventoryBalanceQtyMaterialIssue>> GetInventoryStockByItemAndProjectNo(string itemNumber, string projectNumber)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };


            List<InventoryBalanceQtyMaterialIssue> result = await _tipsWarehouseDbContext.Inventories
                   .Where(x => x.PartNumber == itemNumber && x.ProjectNumber == projectNumber && x.IsStockAvailable == true && !skipWareHouse.Contains(x.Warehouse))
                   .GroupBy(l => new { l.PartNumber, l.ProjectNumber })
                   .Select(group => new InventoryBalanceQtyMaterialIssue
                   {
                       PartNumber = group.Key.PartNumber,
                       BalanceQty = group.Sum(c => c.Balance_Quantity),
                       ProjectNumber = group.Key.ProjectNumber
                   }).ToListAsync();

            return result;
        }



        public async Task<Inventory> GetInventoryDetailsByItemNoProjectNoUnitWarehouseAndLocation(string itemNumber, string projectNumber, string unit, string warehouse, string location)
        {
            var inventoryDetails = await _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == itemNumber
            && x.ProjectNumber == projectNumber && x.Unit == unit && x.Warehouse == warehouse && x.Location == location && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();

            return inventoryDetails;
        }
        public async Task<IEnumerable<GetInventoryListByItemNo>> GetInventoryListByItemNo(string ItemNumber)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
            var partTypes = new PartType[] { PartType.FG, PartType.TG, PartType.FRU, PartType.PurchasePart };

            IEnumerable<GetInventoryListByItemNo> getInventoryListByItemNo = await _tipsWarehouseDbContext.Inventories
                .Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true && !skipWareHouse.Contains(x.Warehouse) && partTypes.Contains(x.PartType))
                                .Select(x => new GetInventoryListByItemNo()
                                {
                                    InventoryId = x.Id,
                                    ItemNumber = x.PartNumber,
                                    Balance_Quantity = x.Balance_Quantity

                                })
                                .OrderBy(on => on.InventoryId)
                                .ToListAsync();
            return getInventoryListByItemNo;
            //IEnumerable<GetInventoryListByItemNo> getInventoryListByItemNo = await _tipsWarehouseDbContext.Inventories
            //    .Where(x =>x.PartNumber == ItemNumber && x.IsStockAvailable == true)
            //                    .Select(x => new GetInventoryListByItemNo()
            //                    {
            //                        InventoryId = x.Id,
            //                        ItemNumber = x.PartNumber,
            //                        Balance_Quantity = x.Balance_Quantity

            //                    })
            //                    .OrderBy(on => on.InventoryId)

            //                  .ToListAsync();

            //return getInventoryListByItemNo;
        }
        public async Task<Inventory> GetInventoryById(int id)
        {
            var getInventoryById = await _tipsWarehouseDbContext.Inventories.Where(x => x.Id == id)

                          .FirstOrDefaultAsync();

            return getInventoryById;
        }

        public async Task<Inventory> GetInventoryStockByItemAndShopOrderNo(string itemNumber, string shopordernumber)
        {
            var getInventoryById = await _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == itemNumber && x.shopOrderNo == shopordernumber)
                          .FirstOrDefaultAsync();

            return getInventoryById;
        }

        public async Task<List<Inventory>> GetFGInventoryStockByItem(string itemNumber)
        {
            var getInventoryById = await _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == itemNumber && x.Warehouse == "FG" && x.Location == "FG" && x.Balance_Quantity>0)
                          .ToListAsync();

            return getInventoryById;
        }
        public async Task<List<Inventory>> GetSAInventoryStockByItem(string itemNumber)
        {
            var getInventoryById = await _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == itemNumber && x.Warehouse == "SA" && x.Location == "SA")
                          .ToListAsync();

            return getInventoryById;
        }
        public async Task<string> UpdateInventory(Inventory inventory)
        {
            inventory.LastModifiedBy = _createdBy;
            inventory.LastModifiedOn = DateTime.Now;
            if (inventory.Balance_Quantity == 0) inventory.IsStockAvailable = false;
            else inventory.IsStockAvailable = true;
            Update(inventory);
            string result = $"materialIssue of Detail {inventory.Id} is updated successfully!";
            return result;
        }

        public async Task<Inventory> GetInventoryDetails(string ItemNumber)
        {
            var getSalesOrderDetailsBySOandItemNo = await _tipsWarehouseDbContext.Inventories
                 .Where(x => x.PartNumber == ItemNumber)
                          .FirstOrDefaultAsync();

            return getSalesOrderDetailsBySOandItemNo;
        }

        //get inventory details from fg partnumber

        public async Task<Inventory> GetInventoryFGDetailsByItemNumber(string ItemNumber)
        {
            try
            {
                string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
                var partTypes = new PartType[] { PartType.FG, PartType.TG, PartType.FRU };

                var getSalesOrderDetailsBy = await _tipsWarehouseDbContext.Inventories
                    .Where(x => x.PartNumber == ItemNumber && partTypes.Contains(x.PartType) && x.IsStockAvailable == true && !skipWareHouse.Contains(x.Warehouse) && x.Balance_Quantity > 0)
                    .FirstOrDefaultAsync();

                return getSalesOrderDetailsBy;
            }
            catch (Exception ex)
            {
                var data = ex.InnerException;
                return null;
            }

            // return null;
        }



        public async Task<List<Inventory>> ReturnInventoryFGDetailsByItemNumber(string ItemNumber)
        {
            string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
            var partTypes = new PartType[] { PartType.FG, PartType.TG, PartType.FRU };

            var getSalesOrderDetailsBy = await _tipsWarehouseDbContext.Inventories
                .Where(x => x.PartNumber == ItemNumber && partTypes.Contains(x.PartType) && x.IsStockAvailable == true && !skipWareHouse.Contains(x.Warehouse) && x.Balance_Quantity > 0)
                .ToListAsync();

            return getSalesOrderDetailsBy;
        }

        public async Task<List<Inventory>> GetInventoryByItemNumber(string ItemNumber)
        {

            try
            {
                string[] skipWareHouse = { "WIP", "Reject", "Scrap", "Rework", "IQC", "GRIN" };
                var getInventoryDetails = await _tipsWarehouseDbContext.Inventories
                     .Where(x => x.PartNumber == ItemNumber && x.IsStockAvailable == true && !skipWareHouse.Contains(x.Warehouse))
                              .ToListAsync();
                return getInventoryDetails;
            }
            catch (Exception ex)
            {
                var data = ex.InnerException;
                return null;
            }

            return null;
        }


        public async Task<IEnumerable<ListOfLocationTransferDto>> GetInventoryDetailsForLocationTransfer(string ItemNumber)
        {

            IEnumerable<ListOfLocationTransferDto> getBtoNumberList = await _tipsWarehouseDbContext.Inventories
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



        public async Task<decimal> GetTotalStockOfItemNumber(string itemNumber)
        {
            var locationNames = new string[] { "Rework", "Scrap" };
            return await _tipsWarehouseDbContext.Inventories
        .Where(i => i.PartNumber == itemNumber && i.IsStockAvailable == true && i.Balance_Quantity > 0
                && !locationNames.Contains(i.Location))
        .SumAsync(i => i.Balance_Quantity);
        }

        public async Task<decimal> GetTotalStockOfSAItemNumberAndProjectNo(string itemNumber , string projectNo)
        {
            var locationNames = new string[] { "Rework", "Scrap" };
            return await _tipsWarehouseDbContext.Inventories
        .Where(i => i.PartNumber == itemNumber && i.ProjectNumber == projectNo && i.IsStockAvailable == true && i.Balance_Quantity > 0
                && !locationNames.Contains(i.Location) && i.PartType == PartType.SA)
        .SumAsync(i => i.Balance_Quantity);
        }

        public async Task<List<Inventory>> GetWipInventoryDetailsByLotNumber(string itemNumber, string lotNumber)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventories.Where(x => x.PartNumber == itemNumber
            && x.IsStockAvailable == true && x.Location == "WIP" && x.Warehouse == "WIP" && x.LotNumber == lotNumber)
                          .ToListAsync();
            return inventoryDetail;
        }
    }
}
