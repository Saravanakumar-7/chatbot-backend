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
using System.Collections;

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
            var responses = await _httpClient.PostAsync(string.Concat(_config["ItemMastersAPI"], "GetItemPartTypeByItemNumber"), itemNoListString);

            var itemNoPartTypeString = await responses.Content.ReadAsStringAsync();
            dynamic itemNoPartTypeData = JsonConvert.DeserializeObject(itemNoPartTypeString);
            //List<ItemNoWithPartTypeDto> itemNoWithPartType = (List<ItemNoWithPartTypeDto>)itemNoPartTypeData.data;

            List<ItemNoWithPartTypeDto> itemNoWithPartType = new List<ItemNoWithPartTypeDto>();

            foreach (var item in itemNoPartTypeData.data)
            {
                ItemNoWithPartTypeDto dto = JsonConvert.DeserializeObject<ItemNoWithPartTypeDto>(item.ToString());
                itemNoWithPartType.Add(dto);
            }

            foreach (var salesOrderItem in salesOrders)
            {
                // Calculate Stock

                var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                              "GetConsumptionInventoryByItemNo?", "itemNumber=", salesOrderItem.FGItemNumber));

                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                dynamic inventoryObject = inventoryObjectData.data;

                if (inventoryObject != null)
                {
                    try
                    {
                        var coverageReport = new OpenSalesCoverageReport
                        {
                            ItemNumber = salesOrderItem.FGItemNumber,
                            TotalRequiredQty = salesOrderItem.Balance_Qty

                        };

                        //foreach (var inventory in inventoryObject)

                        //{
                        decimal balanceQuantity = (decimal)inventoryObject.balance_Quantity; // Convert to decimal
                        coverageReport.Stock = coverageReport.Stock + balanceQuantity;
                         
                        //}

                        // Calculate OpenSOQty
                        coverageReport.OpenSOQty = salesOrderItem.Balance_Qty - coverageReport.Stock;

                        PartType itemPartType = PartType.TG;

                        foreach (var item in itemNoWithPartType)
                        {
                            if (item.ItemNumber == salesOrderItem.FGItemNumber)
                            {
                                itemPartType = item.PartType;
                                coverageReport.PartType = itemPartType;
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
                                    //foreach (var purchase in purchaseObject)
                                    //{
                                        decimal OpenPoQty = (decimal)purchaseObject.balanceQty; 
                                        coverageReport.OpenPoQty = coverageReport.OpenPoQty + OpenPoQty;
                                    //}
                                }
                            }
                        }

                        // Calculate BalanceToOrder
                        var balanceToOrderQty = coverageReport.OpenSOQty - (coverageReport.Stock + coverageReport.OpenPoQty);
                        coverageReport.BalanceToOrder = balanceToOrderQty.HasValue && balanceToOrderQty.Value > 0 ? balanceToOrderQty.Value : 0;

                        //coverageReport.BalanceToOrder = balanceToOrderQty <= 0 ? 0 : balanceToOrderQty;
                        openSalesCoverageReports.Add(coverageReport);
                    }
                    catch (Exception ex)
                    {

                    }
            }
            }

            return openSalesCoverageReports;
        }

   

        [HttpGet]
        public async Task<IActionResult> GenerateCoverageReportForFgChildItems()
        {
            ServiceResponse<List<CoverageReportDtoForChildItem>> serviceResponse = new ServiceResponse<List<CoverageReportDtoForChildItem>>();
            
            try
            {
                List<CoverageReportDtoForChildItem> coverageReportDtoForChildItemList = new List<CoverageReportDtoForChildItem>();
                List<OpenSalesCoverageReport> openSalesCoverageReports = await FGLevelCoverageReport();
                //List<OpenSalesCoverageReport> openSalesCoverageReports = new List<OpenSalesCoverageReport>();
                //OpenSalesCoverageReport openSalesCoverageReport = new OpenSalesCoverageReport
                //{
                //        ItemNumber = "1002370",
                //        PartType = PartType.FG,
                //        OpenSOQty = 100,
                //        TotalRequiredQty = 80,
                //        Stock = 40,
                //        OpenPoQty = 20,
                //        BalanceToOrder=20,
                        
                // };
                //OpenSalesCoverageReport openSalesCoverageReport1 = new OpenSalesCoverageReport
                //{
                //    ItemNumber = "102010038069",
                //    PartType = PartType.FG,
                //    OpenSOQty = 100,
                //    TotalRequiredQty = 80,
                //    Stock = 40,
                //    OpenPoQty = 20,
                //    BalanceToOrder = 20,
                //};
                //openSalesCoverageReports.Add(openSalesCoverageReport1);
                //openSalesCoverageReports.Add(openSalesCoverageReport);

                if (openSalesCoverageReports != null)
                {
                    List<OpenSalesCoverageReport> openFGCoverageDetails = openSalesCoverageReports
                        .Where(x => x.PartType == PartType.FG && x.BalanceToOrder > 0).ToList();
                    if (openFGCoverageDetails != null)
                    {
                        // Child Item Required Qty from BOM
                        List<CoverageReportChildItemReqQtyDto> childItemReqQtyDtos = await GetChildItemRequiredQtyFromBom(openFGCoverageDetails);

                        if (childItemReqQtyDtos != null)
                        {
                            List<string?> itemNumberList = childItemReqQtyDtos.Select(x => x.ItemNumber).ToList();

                            
                            var itemNoListJson = JsonConvert.SerializeObject(itemNumberList);
                            var itemNoListString = new StringContent(itemNoListJson, Encoding.UTF8, "application/json");

                            //Open Stock with WIP Quantity
                            List<ChildItemStockWithWipDto> itemStockWithWipList = await GetStockWithWipQtyForChildItems(itemNoListString); 

                            //Open PO Qty for Child Items
                            List<OpenPoQuantityDto> openPoQtyList = await GetOpenPoQtyForChildItems(itemNoListString);

                            if (itemStockWithWipList != null)
                            {
                                foreach (var item in childItemReqQtyDtos)
                                {
                                    CoverageReportDtoForChildItem coverageDetailOfChildItem = new CoverageReportDtoForChildItem
                                    {
                                        ItemNumber = item.ItemNumber,
                                        PartType = item.PartType,
                                        RequiredQty = item.TotalRequiredQty,
                                        Stock = itemStockWithWipList.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.BalanceQuantity).FirstOrDefault(),
                                        WipQty = itemStockWithWipList.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.WipQuantity).FirstOrDefault(),
                                        OpenPoQty = openPoQtyList.Where(x => x.ItemNumber == item.ItemNumber).Select(x => x.OpenPoQty).FirstOrDefault()
                                    };
                                    coverageDetailOfChildItem.BalanceToOrder = coverageDetailOfChildItem.RequiredQty - (coverageDetailOfChildItem.Stock
                                        + coverageDetailOfChildItem.OpenPoQty + coverageDetailOfChildItem.WipQty);
                                    coverageReportDtoForChildItemList.Add(coverageDetailOfChildItem);
                                }
                            }

                        }
                    }
                }
                serviceResponse.Data = coverageReportDtoForChildItemList;
                serviceResponse.Message = $"Returned Child Item CoverageReport Successfully in GenerateCoverageReportForFgChildItems ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Something went wrong inside GenerateCoverageReportForFgChildItems action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task<List<ChildItemStockWithWipDto>> GetStockWithWipQtyForChildItems(StringContent itemNoListString)
        {
            var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "GetConsumptionChildItemStockWithWipQty"), itemNoListString);
            var itemStockWithWipString = await responses.Content.ReadAsStringAsync();
            dynamic itemStockWithWipData = JsonConvert.DeserializeObject(itemStockWithWipString);
            //List<ChildItemStockWithWipDto> itemStockWithWipList = (List<ChildItemStockWithWipDto>)itemStockWithWipData.data;


            List<ChildItemStockWithWipDto> childItemStockWithWipQty = new List<ChildItemStockWithWipDto>();

            foreach (var item in itemStockWithWipData.data)
            {
                ChildItemStockWithWipDto dto = JsonConvert.DeserializeObject<ChildItemStockWithWipDto>(item.ToString());
                childItemStockWithWipQty.Add(dto);
            }

            return childItemStockWithWipQty; 
        }

        //private async Task<List<ChildItemStockWithWipDto>> GetStockWithWipQtyForChildItems(StringContent itemNoListString)
        //{
        //    var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "GetConsumptionChildItemStockWithWipQty"), itemNoListString);

        //    if (responses.IsSuccessStatusCode)
        //    {
        //        var itemStockWithWipString = await responses.Content.ReadAsStringAsync();

        //        dynamic itemStockWithWipData = JsonConvert.DeserializeObject(itemStockWithWipString);

        //        if (itemStockWithWipData != null && itemStockWithWipData.data != null)
        //        {
        //            List<ChildItemStockWithWipDto> itemStockWithWipList =
        //                JsonConvert.DeserializeObject<List<ChildItemStockWithWipDto>>(itemStockWithWipData.data.ToString());

        //            return itemStockWithWipList;
        //        }
        //        else
        //        {
        //            // Handle the case where the data is null or doesn't contain the expected structure.
        //            // You might want to return an empty list or handle the error as appropriate.
        //            return new List<ChildItemStockWithWipDto>();
        //        }
        //    }
        //    else
        //    {
        //        // Handle the case where the HTTP request was not successful.
        //        // You might want to throw an exception or handle the error as appropriate.
        //        return new List<ChildItemStockWithWipDto>();
        //    }
        //}


        private async Task<List<OpenPoQuantityDto>> GetOpenPoQtyForChildItems(StringContent itemNoListString)
        {
            var openPoQtyResponse = await _httpClient.PostAsync(string.Concat(_config["PurchaseAPI"],
                                            "GetListOfOpenPOQtyByItemNoList"), itemNoListString);
            var openPoQtyString = await openPoQtyResponse.Content.ReadAsStringAsync();
            dynamic openPoQtyData = JsonConvert.DeserializeObject(openPoQtyString);
            //List<OpenPoQuantityDto> openPoQtyList = (List<OpenPoQuantityDto>)openPoQtyData.data;
            List<OpenPoQuantityDto> openPoQtyList = new List<OpenPoQuantityDto>();

            foreach (var item in openPoQtyData.data)
            {
                OpenPoQuantityDto dto = JsonConvert.DeserializeObject<OpenPoQuantityDto>(item.ToString());
                openPoQtyList.Add(dto);
            }

            return openPoQtyList; 
        }
         
        private async Task<List<CoverageReportChildItemReqQtyDto>> GetChildItemRequiredQtyFromBom(List<OpenSalesCoverageReport> openFGCoverageDetails) 
        {
            var openFGCoverageDetailsJson = JsonConvert.SerializeObject(openFGCoverageDetails);
            var openFGCoverageDetailsString = new StringContent(openFGCoverageDetailsJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "GetBomDetailsForCoverageReport"), openFGCoverageDetailsString); 

            var childItemRequiredQtyString = await response.Content.ReadAsStringAsync();
            dynamic childItemRequiredQtyData = JsonConvert.DeserializeObject(childItemRequiredQtyString);
            //List<CoverageReportChildItemReqQtyDto> childItemReqQtyDtos = (List<CoverageReportChildItemReqQtyDto>)childItemRequiredQtyData.data;

            List<CoverageReportChildItemReqQtyDto> childItemReqQtyDtos = new List<CoverageReportChildItemReqQtyDto>();

            foreach (var item in childItemRequiredQtyData.data)
            {
                CoverageReportChildItemReqQtyDto dto = JsonConvert.DeserializeObject<CoverageReportChildItemReqQtyDto>(item.ToString());
                childItemReqQtyDtos.Add(dto);
            }

            return childItemReqQtyDtos;


        }


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
