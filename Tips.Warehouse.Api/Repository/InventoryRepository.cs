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

                // Group the inventory items by PartNumber using LINQ's GroupBy
                //var groupedItems = inventoryItems.GroupBy(inv => inv.PartNumber)
                //    .ToDictionary(g => g.Key, g => g.ToList());
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
                //    foreach (var group in groupedItems.Values)
                //{
                //    var sum = group.Sum(inv => inv.Balance_Quantity);
                //    var firstItem = group.First();
                //    firstItem.Balance_Quantity = sum;
                //}

                // Return the updated first items from each group
                return groupedItems.Values.Select(group => group.First());
            }
            //using (var context = _tipsWarehouseDbContext)
            //{
            //    var query = _tipsWarehouseDbContext.Inventory.AsQueryable();

            //    // Check if inventoryBalQty object is not null
            //    if (InventoryItemNo != null)
            //    {
            //        // Apply filtering based on the inventoryBalQty properties if they are not null
            //        if (InventoryItemNo.PartNumber != null && InventoryItemNo.PartNumber.Any())
            //        {
            //            query = query.Where(inv => InventoryItemNo.PartNumber.Contains(inv.PartNumber));
            //        }
            //    }

            //    // Retrieve the filtered inventory items
            //    var inventoryItems = await query.ToListAsync();

            //    // Group the inventory items by PartNumber using a for loop
            //    var groupedItems = new Dictionary<string, List<Inventory>>();
            //    foreach (var item in inventoryItems)
            //    {
            //        var key = item.PartNumber;
            //        if (!groupedItems.ContainsKey(key))
            //        {
            //            groupedItems[key] = new List<Inventory> { item };
            //        }
            //        else
            //        {
            //            groupedItems[key].Add(item);
            //        }
            //    }


            //    // Calculate the sum of Balance_Quantity for each group and update the first item in the group
            //    foreach (var group in groupedItems)
            //    {
            //        var sum = group.Value.Sum(inv => inv.Balance_Quantity);
            //        var firstItem = group.Value.First();
            //        firstItem.Balance_Quantity = sum;

            //    }

            //    // Return the updated first items from each group
            //    return groupedItems.Values.Select(group => group.First());
       // }
        }

        //public async Task<IEnumerable<ConsumptionReport>> ExecuteStoredProcedure(string? itemNumber, string? salesOrderNumber)
        //{

        //    //var context = new TipsWarehouseDbContext();
        //    //var nameParam = new SqlParameter("@Name", name);
        //    //var ageParam = new SqlParameter("@Age", age);

        //    //var students = context.FromSql("EXEC GetStudents @Name, @Age", nameParam, ageParam)
        //    //                      .ToList();

        //    var ItemNumber = new SqlParameter("@ItemNumber", itemNumber);
        //    var SalesOrderNumber = new SqlParameter("@SalesOrderNumber", salesOrderNumber);

        //    var result = _tipsWarehouseDbContext.ConsumptionReport.FromSql("ProductDemand_Vs_AvailableStock_With_Parameter {0}, {1}", ItemNumber, SalesOrderNumber).ToList();

        //    //var result = _tipsWarehouseDbContext.Database.SqlQuery<ConsumptionReport>("EXEC ProductDemand_Vs_AvailableStock_With_Parameter @ItemNumber, @SalesOrderNumber", ItemNumber, SalesOrderNumber)
        //    //    .ToList();
            
        //    //var result = _tipsWarehouseDbContext.Database.FromSql<ConsumptionReport>($"ProductDemand_Vs_AvailableStock_With_Parameter {itemNumber}").ToList();

        //    ////string conStr = ConfigurationManager.ConnectionStrings["TipsWarehouseDbContext"].ConnectionString;

        //    ////string connectionString = "server=10.10.201.144;database=getapcs_keus_warehouse;user=nayagam;password=%FJx9rpr=Z+SAYE8;";

        //    //List<ConsumptionDto> consumptionList = new List<ConsumptionDto>();

        //    //using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    //{
        //    //    connection.Open();

        //    //    // Execute stored procedure
        //    //    using (MySqlCommand command = new MySqlCommand("ProductDemand_Vs_AvailableStock_With_Parameter", connection))
        //    //    {
        //    //        command.CommandType = CommandType.StoredProcedure;

        //    //        // Set input parameters if needed
        //    //        command.Parameters.AddWithValue("@ItemNumber", itemNumber);
        //    //        command.Parameters.AddWithValue("@SalesOrderNumber", salesOrderNumber);
        //    //        try
        //    //        {
        //    //            // Execute the command
        //    //            using (MySqlDataReader reader = command.ExecuteReader())
        //    //            {
        //    //                // Process the results

        //    //                while (reader.Read())
        //    //                {
        //    //                    // Create a new ConsumptionDto instance
        //    //                    ConsumptionDto consumption = new ConsumptionDto();

        //    //                    // Set the properties of ConsumptionDto from the reader
        //    //                    consumption.ItemNumber = reader.GetString("ItemNumber");
        //    //                    consumption.SalesOrderNumber = reader.GetString("SalesOrderNumber");
        //    //                    consumption.Description = reader.GetString("Description");
        //    //                    consumption.ForecastQty = reader.GetDecimal("ForecastQty");
        //    //                    consumption.FGStock = reader.GetDecimal("FGStock");
        //    //                    consumption.BalanceForecastQty = reader.GetDecimal("BalanceForecastQty");
        //    //                    consumption.Child = reader.GetString("Child");
        //    //                    consumption.ChildPartDescription = reader.GetString("ChildPartDescription");
        //    //                    consumption.QtyPerUnit = reader.GetDecimal("QtyPerUnit");
        //    //                    consumption.ChildRequiredQty = reader.GetDecimal("ChildRequiredQty");
        //    //                    consumption.TotalChildReqQty = reader.GetDecimal("TotalChildReqQty");

        //    //                    // Add the ConsumptionDto instance to the list
        //    //                    consumptionList.Add(consumption);
        //    //                }


        //    //            }
        //    //        }
        //    //        catch (MySql.Data.MySqlClient.MySqlException ex)
        //    //        {
        //    //            return null;
        //    //        }
        //    //    }
        //    //}

        //    return result;

        //}

        public async Task<IEnumerable<Inventory>> GetInventoryDetailsWithSumOfStock(InventoryBalQty inventoryBalQty)
        {
            using (var context = _tipsWarehouseDbContext)
            {
                var query = _tipsWarehouseDbContext.Inventory.AsQueryable();

                // Check if inventoryBalQty object is not null
                if (inventoryBalQty != null)
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
         
        //public async Task<IEnumerable<ConsumptionDto>> ExecuteYourStoredProcedure(string itemNumber, string salesOrderNumber)
        //{
           
        //}

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
        public async Task<IEnumerable<Inventory>> GetInventoryDetailsByItemNumberandLocation(string ItemNumber, string Location, string Warehouse)
        
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
        public async Task<Inventory> GetInventoryDetailsByItemNo(string ItemNumber)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventory.Where(x =>x.PartNumber == ItemNumber && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();

            return inventoryDetail;
        }

        public async Task<Inventory> GetInventoryDetailsByItemNoandPartType(string ItemNumber, string PartType)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == ItemNumber && x.PartType == PartType && x.IsStockAvailable == true)
                          .FirstOrDefaultAsync();

            return inventoryDetail;
        }

        public async Task<IEnumerable<Inventory>> GetInventoryDetailsByItemNoandPartTypes(string ItemNumber)
        {
            var inventoryDetail = await _tipsWarehouseDbContext.Inventory.Where(x => x.PartNumber == ItemNumber  && x.IsStockAvailable == true)
                          .ToListAsync();

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
