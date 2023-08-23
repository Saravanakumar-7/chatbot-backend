using System.Net;
using System.Net.Http;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Repository;
using Newtonsoft.Json.Linq;
using Entities.Enums;
using System.Collections.Generic;

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CoverageReportController : ControllerBase
    {
        private ICollectionTrackerRepository _repository;
        private ISalesOrderItemsRepository _salesOrderItemsRepository;
        private ICoverageReportRepository _coverageRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;


        public CoverageReportController(ICollectionTrackerRepository repository, ISalesOrderItemsRepository salesOrderItemsRepository, HttpClient httpClient, IConfiguration config, ICoverageReportRepository coverageReportRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _coverageRepository = coverageReportRepository;
            _salesOrderItemsRepository = salesOrderItemsRepository;

        }

        //get consumption report in FG Level
        //[HttpGet]
        //public async Task<List<OpenSalesCoverageReport>> GenerateCoverageFGLevelReport()
        //{
        //    var salesOrders = await _salesOrderItemsRepository.GetAllSalesOrderFGOrTGItemDetails();
        //    List<OpenSalesCoverageReport> openSalesCoverageReports = new List<OpenSalesCoverageReport>();

        //    foreach (var salesOrderItem in salesOrders)
        //    {

        //        var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
        //                      "GetConsumptionInventoryByItemNo?", "itemNumber=", salesOrderItem.FGItemNumber));

        //        var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
        //        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
        //        dynamic inventoryObject = inventoryObjectData.data;
        //        if (inventoryObject != null)
        //        {
        //            OpenSalesCoverageReport coverageReport = new OpenSalesCoverageReport
        //            {
        //                FGOrTGPartNumber = salesOrderItem.FGItemNumber
        //            };

        //            foreach (var inventory in inventoryObject)
        //            {
        //                coverageReport.Stock = coverageReport.Stock + inventoryObject.balance_Quantity;
        //            }

        //            coverageReport.OpenSOQty = salesOrderItem.Balance_Qty - coverageReport.Stock;

        //            var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
        //                          "GetAllOpenTGPoDetails?", "itemNumber=", salesOrderItem.FGItemNumber));

        //            if (purchaseObjectResult != null && purchaseObjectResult.StatusCode == HttpStatusCode.OK)
        //            {
        //                var purchaseObjectResults = await purchaseObjectResult.Content.ReadAsStringAsync();
        //                dynamic purchaseObjectData = JsonConvert.DeserializeObject(purchaseObjectResults);
        //                dynamic purchaseObject = purchaseObjectData.data;
        //                if (purchaseObject != null)
        //                {
        //                    foreach (var purchase in purchaseObject)
        //                    {
        //                        coverageReport.OpenPoQty = coverageReport.OpenPoQty + purchase.balanceQty;
        //                    }
        //                }
        //            }
        //            coverageReport.BalanceToOrder = coverageReport.OpenSOQty - (coverageReport.Stock + coverageReport.OpenPoQty);
        //            openSalesCoverageReports.Add(coverageReport);
        //        }
        //    }
        //    return openSalesCoverageReports;

        //}


       [HttpGet]
        public async Task<IActionResult> GenerateCoverageFGLevelReport()
        {

            ServiceResponse<List<OpenSalesCoverageReport>> serviceResponse = new ServiceResponse<List<OpenSalesCoverageReport>>();

            try
            {
                List<OpenSalesCoverageReport> openSalesCoverageReports = await FGLevelCoverageReport();

                serviceResponse.Data = openSalesCoverageReports;
                serviceResponse.Message = $"Returned CoverageReport Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GenerateCoverageFGLevelReport action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task<List<OpenSalesCoverageReport>> FGLevelCoverageReport()
        {
            var salesOrders = await _salesOrderItemsRepository.GetAllSalesOrderFGOrTGItemDetails();
            List<OpenSalesCoverageReport> openSalesCoverageReports = new List<OpenSalesCoverageReport>();

            List<string?> itemNumberList = salesOrders.Select(x => x.FGItemNumber).ToList();

            var itemNoListJson = JsonConvert.SerializeObject(itemNumberList);
            var itemNoListString = new StringContent(itemNoListJson, Encoding.UTF8, "application/json");
            var responses = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "GetItemPartTypeByItemNumber?"), itemNoListString);

            var itemNoPartTypeString = await responses.Content.ReadAsStringAsync();
            dynamic itemNoPartTypeData = JsonConvert.DeserializeObject(itemNoPartTypeString);
            List<ItemWithPartTypeDto> itemNoWithPartType = (List<ItemWithPartTypeDto>)itemNoPartTypeData.data;

            foreach (var salesOrderItem in salesOrders)
            {

                var coverageReport = new OpenSalesCoverageReport
                {
                    ItemNumber = salesOrderItem.FGItemNumber,
                    TotalRequiredQty = salesOrderItem.Balance_Qty

                };
                // Calculate Stock

                var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                              "GetConsumptionInventoryByItemNo?", "itemNumber=", salesOrderItem.FGItemNumber));

                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                dynamic inventoryObject = inventoryObjectData.data;

                if (inventoryObject != null)
                {
                    foreach (var inventory in inventoryObject)
                    {
                        coverageReport.Stock = coverageReport.Stock + inventory.balance_Quantity;
                    }
                }

                // Calculate OpenSOQty
                coverageReport.OpenSOQty = salesOrderItem.Balance_Qty - coverageReport.Stock;

                PartType itemPartType = PartType.TG;

                foreach (var item in itemNoWithPartType)
                {
                    if (item.ItemNumber == salesOrderItem.FGItemNumber)
                    {
                        itemPartType = item.PartType;
                        break;
                    }
                }
                if (itemPartType == PartType.TG)
                {

                    // Calculate OpenPoQty
                    var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                  "GetAllOpenTGPoDetails?", "itemNumber=", salesOrderItem.FGItemNumber));

                    if (purchaseObjectResult != null && purchaseObjectResult.StatusCode == HttpStatusCode.OK)
                    {
                        var purchaseObjectResults = await purchaseObjectResult.Content.ReadAsStringAsync();
                        dynamic purchaseObjectData = JsonConvert.DeserializeObject(purchaseObjectResults);
                        dynamic purchaseObject = purchaseObjectData.data;

                        if (purchaseObject != null)
                        {
                            foreach (var purchase in purchaseObject)
                            {
                                coverageReport.OpenPoQty = coverageReport.OpenPoQty + purchase.balanceQty;
                            }
                        }
                    }
                }

                // Calculate BalanceToOrder
                var balanceToOrderQty = coverageReport.OpenSOQty - (coverageReport.Stock + coverageReport.OpenPoQty);

                coverageReport.BalanceToOrder = balanceToOrderQty <= 0 ? 0 : balanceToOrderQty;
                openSalesCoverageReports.Add(coverageReport);
            }

            return openSalesCoverageReports;
        }

        //bom
        //itemtype        
        //reqqty -->from bom item by fg itemnumber
        //totalrequ ---> reqqty * balcetoriqty 



 
        















        //private async Task<decimal> CalculateTotalRequiredQtyForItem(string itemNumber, decimal? balanceToOrderQty)
        //{
        //    decimal totalRequiredQty = 0;

        //    var enggBOM = await GetEnggBOM(itemNumber);

        //    if (enggBOM != null)
        //    {
        //        totalRequiredQty = await CalculateTotalRequiredQtyRecursive(enggBOM, balanceToOrderQty);
        //    }

        //    return totalRequiredQty;
        //}

        //private async Task<> GetEnggBOM(string itemNumber)
        //{
        //    var enggBomDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"],
        //                            "GetLatestEnggBomVersionDetailByIemNumber?", "&fgPartNumber=", itemNumber));

        //    if (enggBomDetails != null && enggBomDetails.IsSuccessStatusCode)
        //    {
        //        var enggBomObjectString = await enggBomDetails.Content.ReadAsStringAsync();
        //        dynamic enggBomObjectData = JsonConvert.DeserializeObject(enggBomObjectString);
        //        dynamic enggBomObject = enggBomObjectData.data;

        //        if (enggBomObject != null)
        //        {
        //            // Parse and construct an EnggBom object based on the retrieved data
        //            EnggBom enggBom = new EnggBom
        //            {
        //                ItemNumber = enggBomObject.ItemNumber,
        //                // ... Populate other properties based on enggBomObject
        //            };

        //            return enggBom;
        //        }
        //    }

        //    return null; // Return null if no valid EnggBom is retrieved
        //}


        //private async Task<decimal> CalculateTotalRequiredQtyRecursive(EnggBom enggBOM, decimal parentQty)
        //{
        //    decimal totalRequiredQty = 0;

        //    foreach (var enggChildItem in enggBOM.EnggChildItems)
        //    {
        //        if (enggChildItem.PartType == PartType.SA)
        //        {
        //            var childEnggBOM = await GetEnggBOM(enggChildItem.ItemNumber);

        //            if (childEnggBOM != null)
        //            {
        //                decimal childQty = enggChildItem.Quantity * parentQty;
        //                decimal childRequiredQty = await CalculateTotalRequiredQtyRecursive(childEnggBOM, childQty);
        //                totalRequiredQty += childRequiredQty;
        //            }
        //        }
        //        else
        //        {
        //            totalRequiredQty += enggChildItem.Quantity * parentQty;
        //        }
        //    }

        //    return totalRequiredQty;
        //}


        //test consumption report
        [HttpGet]
        public async Task<List<CoverageReport>> GenerateCoverageReportAsync()
        {
            var coverageReports = new Dictionary<string, CoverageReport>();

            // Get all sales orders where status is 'Forecast'
            var salesOrders = await _coverageRepository.GetAllForecastSalesOrderDetails();


            foreach (var salesOrder in salesOrders)
            {
                int salesOrderId = salesOrder.Id;
                // Get the sales order items for the current sales order
                var salesOrderItems = await _coverageRepository.GetAllSalesOrderItemDetails(salesOrderId);

                foreach (var salesOrderItem in salesOrderItems)
                {
                    // Get the BOM for the current sales order item recursively and calculate required quantities
                    await GetBomAndCalculateRequiredQuantitiesRecursivelyAsync(salesOrderItem.ItemNumber, salesOrderItem.BalanceQty, coverageReports);
                }
            }

            // At this point, 'coverageReports' will have the coverage report for each item number
            return coverageReports.Values.ToList();
        }


        [HttpGet]
        private async Task GetBomAndCalculateRequiredQuantitiesRecursivelyAsync(string itemNumber, decimal requiredQtyMultiplier, Dictionary<string, CoverageReport> coverageReports)
        {
            var enggBomDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"],
                            "GetLatestEnggBomVersionDetailByIemNumber?", "&fgPartNumber=", itemNumber));

            var enggBomObjectString = await enggBomDetails.Content.ReadAsStringAsync();
            dynamic enggBomObjectData = JsonConvert.DeserializeObject(enggBomObjectString);
            dynamic enggBomObject = enggBomObjectData.data;

            foreach (var enggBom in enggBomObject)
            {
                // Get all the child items recursively
                var enggChildItems = await GetEnggChildItemsRecursivelyAsync(enggBom.Id);

                foreach (var childItem in enggChildItems)
                {
                    var totalRequiredQty = requiredQtyMultiplier * childItem.RequiredQty;
                    if (!coverageReports.TryGetValue(childItem.ItemNumber, out CoverageReport coverageReport))

                    // Check if the coverage report for this item number already exists
                    //if (!coverageReports.TryGetValue(childItem.ItemNumber, out var coverageReport))
                    {
                        coverageReport = new CoverageReport
                        {
                            ItemNumber = childItem.ItemNumber
                        };
                        coverageReports[childItem.ItemNumber] = coverageReport;
                    }

                    coverageReport.TotalChildReqQty += totalRequiredQty;

                    // Get the inventory for the current child item
                    //var inventory = await _context.Inventory
                    //    .FirstOrDefaultAsync(i => i.ItemNumber == childItem.ItemNumber && i.IsStockAvailable);

                    var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                              "GetInventoryByItemNo?", "itemNumber=", childItem.ItemNumber));

                    var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                    dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                    dynamic inventoryObject = inventoryObjectData.data;

                    if (inventoryObject != null)
                    {
                        foreach (var inventory in inventoryObject)
                        {
                            coverageReport.FGStock += inventoryObject.balance_Quantity;
                        }
                    }

                    // Get the open purchase order items for the current child item

                    var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                              "GetAllOpenPoDetails?", "itemNumber=", childItem.ItemNumber));
                    if (purchaseObjectResult != null && purchaseObjectResult.StatusCode == HttpStatusCode.OK)
                    {
                        var balanceQty = 0;
                        var purchaseObjectResults = await purchaseObjectResult.Content.ReadAsStringAsync();
                        dynamic purchaseObjectData = JsonConvert.DeserializeObject(purchaseObjectResults);
                        dynamic purchaseObject = purchaseObjectData.data;
                        balanceQty = 0;
                        if (purchaseObject != null)
                        {
                            foreach (var purchase in purchaseObject)
                            {
                                coverageReport.OpenPOQty += purchase.qty;

                            }
                        }
                    }

                    // Calculate the balance quantity to order
                    coverageReport.BalanceQtyToOrder = coverageReport.TotalChildReqQty - (coverageReport.FGStock + coverageReport.OpenPOQty);

                    // If the part type is 'SA', continue the recursion
                    if (enggBom.PartType == "SA")
                    {
                        await GetBomAndCalculateRequiredQuantitiesRecursivelyAsync(childItem.ItemNumber, totalRequiredQty, coverageReports);
                    }
                }
            }
        }

        [HttpGet]
        private async Task<List<EnggChildItem>> GetEnggChildItemsRecursivelyAsync(int parentId)
        {
            var childItems = new List<EnggChildItem>();

            var enggBomDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"],
                            "GetEnggChildItemNumberByEnggbom?", "&bomId=", parentId));

            var enggBomObjectString = await enggBomDetails.Content.ReadAsStringAsync();
            dynamic enggBomObjectData = JsonConvert.DeserializeObject(enggBomObjectString);
            dynamic enggBomObject = enggBomObjectData.data;

            foreach (var enggBom in enggBomObject)
            {
                var indirectChildItems = await GetEnggChildItemsRecursivelyAsync(enggBom.EnggBomId);
                childItems.AddRange(indirectChildItems);
            }

            return childItems;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetCoverageReport()
        //{
        //    ServiceResponse<IEnumerable<CoverageReportDto>> serviceResponse = new ServiceResponse<IEnumerable<CoverageReportDto>>();

        //    try
        //    {
        //        var getAllForecastLpCosting = await _coverageRepository.GetAllSalesOrderDetails();

        //        _logger.LogInfo("Returned Coverage Report");
        //        var result = _mapper.Map<List<CoverageReportDto>>(getAllForecastLpCosting);
        //        //bom details
        //        var json = JsonConvert.SerializeObject(result);
        //        var data = new StringContent(json, Encoding.UTF8, "application/json");
        //        var response = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "CoverageEnggBomChildDetails"), data);

        //        //var inventoryObjectResult = await _httpClient.PostAsync(string.Concat(_config["ItemMasterAPI"], "GetItemsRoutingDetailsForLpCosting"), content);
        //        //var itemsRoutingDetailsJsonString = await inventoryObjectResult.Content.ReadAsStringAsync();
        //        //dynamic itemsRoutingDetailsJson = JsonConvert.DeserializeObject(itemsRoutingDetailsJsonString);
        //        //var data = itemsRoutingDetailsJson.data;

        //        //serviceResponse.Data = result;
        //        serviceResponse.Message = "Returned Coverage Report Successfully";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
        // Get all ForeCastEngg 


        // CoverageReportController

        //[HttpGet("step1/{itemNumber}")]
        //public async Task<ActionResult<CoverageReportItem>> GetFgTgCoverage(string itemNumber)
        //{
        //    var coverageReportResponse = new ServiceResponse<CoverageReportItem>();

        //    try
        //    {
        //        // Call Inventory API
        //        var inventoryResponse = await _httpClient.GetAsync("https://inventoryservice/stock-available/" + itemNumber);
        //        var stockAvailable = await inventoryResponse.Content.ReadAsAsync<ServiceResponse<decimal>>();

        //        // Call PurchaseOrder API 
        //        var purchaseOrderResponse = await _httpClient.GetAsync("https://purchaseorderservice/open-po-quantity/" + itemNumber);
        //        var openPoQty = await purchaseOrderResponse.Content.ReadAsAsync<ServiceResponse<decimal>>();

        //        // Call SalesOrder API
        //        var salesOrderResponse = await _httpClient.GetAsync("https://salesorderservice/open-sales-order-quantity/" + itemNumber);
        //        var openSalesOrderQty = await salesOrderResponse.Content.ReadAsAsync<ServiceResponse<decimal>>();

        //        var reportItem = new CoverageReportItem
        //        {
        //            ItemNumber = itemNumber,
        //            StockAvailable = stockAvailable,
        //            OpenPoQty = openPoQty,
        //            OpenSalesOrderQty = openSalesOrderQty
        //        };

        //        coverageReportResponse.Data = reportItem;
        //        coverageReportResponse.Message = "Retrieved coverage report step 1";
        //        coverageReportResponse.Success = true;

        //        return Ok(coverageReportResponse);

        //    }
        //    catch (Exception ex)
        //    {
        //        coverageReportResponse.Success = false;
        //        coverageReportResponse.Message = "Error generating coverage report";
        //        return StatusCode(500, coverageReportResponse);
        //    }
        //}

    }
}
