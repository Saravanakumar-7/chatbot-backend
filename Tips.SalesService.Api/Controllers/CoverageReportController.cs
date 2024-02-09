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
using MySqlX.XDevAPI.Common;
using Tips.SalesService.Api.Entities.Dto;

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
        [HttpGet]
        public async Task<IActionResult> GenerateCoverageFGLevelReportByProjectNumber(string projectNumber)
        {

            ServiceResponse<List<OpenSalesCoverageReportByProjectNumber>> serviceResponse = new ServiceResponse<List<OpenSalesCoverageReportByProjectNumber>>();

            try
            {
                List<OpenSalesCoverageReportByProjectNumber> openSalesCoverageReportsByprojectNo = await FGLevelCoverageReportByProjectNumber(projectNumber);

                serviceResponse.Data = openSalesCoverageReportsByprojectNo;
                serviceResponse.Message = $"Returned CoverageReportByProjectNumber Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GenerateCoverageFGLevelReportByProjectNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task<List<OpenSalesCoverageReportByProjectNumber>> FGLevelCoverageReportByProjectNumber(string projectNumber)
        {
            List<OpenSalesCoverageReportByProjectNumber> openSalesCoverageReports = new List<OpenSalesCoverageReportByProjectNumber>();
            try
            {
                var salesOrders = await _salesOrderItemsRepository.GetAllSalesOrderFGOrTGItemDetailsByProjectNo(projectNumber);


                //List<(string?, decimal?)> itemNumberList = salesOrders.Select(x => (x.FGItemNumber, x.Balance_Qty)).ToList();

                List<string?> itemNumberList = salesOrders.Select(x => x.FGItemNumber).ToList();
                //change
                var itemNoListJson = JsonConvert.SerializeObject(itemNumberList);
                var itemNoListString = new StringContent(itemNoListJson, Encoding.UTF8, "application/json");
                var responses = await _httpClient.PostAsync(string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"), itemNoListString);

                var itemNoPartTypeString = await responses.Content.ReadAsStringAsync();
                dynamic itemNoPartTypeData = JsonConvert.DeserializeObject(itemNoPartTypeString);
                //List<ItemNoWithPartTypeDto> itemNoWithPartType = (List<ItemNoWithPartTypeDto>)itemNoPartTypeData.data;

                List<ItemNoWithPartTypeDto> itemNoWithPartType = new List<ItemNoWithPartTypeDto>();

                //for this loop we need to check
                foreach (var item in itemNoPartTypeData.data)
                {
                    ItemNoWithPartTypeDto dto = JsonConvert.DeserializeObject<ItemNoWithPartTypeDto>(item.ToString());
                    itemNoWithPartType.Add(dto);
                }

                var salesOrderItemandProjList = _mapper.Map<IEnumerable<SalesOrderItemNoAndProjectNoDto>>(salesOrders);
                var salesOrderItemandProjListjson = JsonConvert.SerializeObject(salesOrderItemandProjList);
                var salesOrderItemandProjDetailsString = new StringContent(salesOrderItemandProjListjson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"],
                                                                                "GetConsumptionInventoryByItemAndProjectNotest2"), salesOrderItemandProjDetailsString);
                var inventoryObjectString = await response.Content.ReadAsStringAsync();
                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                dynamic inventoryObject = inventoryObjectData.data;

                foreach (var salesOrderDetails in salesOrders)
                {
                    if (inventoryObject != null && inventoryObject.Count > 0)
                    {
                        foreach (var Inventory in inventoryObject)
                        {
                            if (salesOrderDetails.FGItemNumber == Inventory["partNumber"]?.ToString())
                            {
                                if (Inventory != null)
                                {
                                    if (salesOrderDetails.FGItemNumber != null && salesOrderDetails.Balance_Qty != 0)
                                    {
                                        var coverageReport = new OpenSalesCoverageReportByProjectNumber
                                        {
                                            ItemNumber = salesOrderDetails.FGItemNumber,
                                            Description = salesOrderDetails.Description,
                                            ProjectNumber = salesOrderDetails.ProjectNumber,
                                            TotalRequiredQty = salesOrderDetails.Balance_Qty,
                                            PartType = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.PartType).FirstOrDefault()
                                        };

                                        decimal balanceQuantity = (decimal)Inventory.balance_Quantity; ; // Convert to decimal
                                        coverageReport.Stock = balanceQuantity;



                                        PartType itemPartType = coverageReport.PartType;


                                        if (itemPartType == PartType.TG)
                                        {
                                            // Calculate OpenPoQty
                                            var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                                          "GetOpenPOTGDetailsByItemAndProjecNoForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber,
                                                                                                                        "&ProjectNumber=", salesOrderDetails.ProjectNumber));

                                            if (purchaseObjectResult != null && purchaseObjectResult.StatusCode == HttpStatusCode.OK)
                                            {
                                                var purchaseObjectResults = await purchaseObjectResult.Content.ReadAsStringAsync();
                                                dynamic purchaseObjectData = JsonConvert.DeserializeObject(purchaseObjectResults);
                                                dynamic purchaseObject = purchaseObjectData.data;

                                                if (purchaseObject != null && purchaseObject.Count > 0)
                                                {
                                                    decimal OpenPoQty = (decimal)purchaseObject.balanceQty;
                                                    coverageReport.OpenPoQty = OpenPoQty;
                                                }
                                                else
                                                {
                                                    coverageReport.OpenPoQty = 0;
                                                }
                                            }
                                        }
                                        // Calculate OpenSOQty
                                        coverageReport.OpenSOQty = salesOrderDetails.Balance_Qty - coverageReport.Stock;
                                        // Calculate BalanceToOrder

                                        var balanceToOrderQty = coverageReport.OpenSOQty - (coverageReport.Stock + coverageReport.OpenPoQty);

                                        coverageReport.BalanceToOrder = Convert.ToDecimal(balanceToOrderQty) <= 0 ? 0 : Convert.ToDecimal(balanceToOrderQty);


                                        //var balanceToOrderQty = coverageReport.OpenSOQty - (coverageReport.Stock + coverageReport.OpenPoQty);
                                        //coverageReport.BalanceToOrder = balanceToOrderQty.HasValue && balanceToOrderQty.Value > 0 ? balanceToOrderQty.Value : 0;

                                        openSalesCoverageReports.Add(coverageReport);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (salesOrderDetails.FGItemNumber != null && salesOrderDetails.Balance_Qty != 0)
                        {
                            var coverageReport = new OpenSalesCoverageReportByProjectNumber
                            {
                                ItemNumber = salesOrderDetails.FGItemNumber,
                                Description = salesOrderDetails.Description,
                                ProjectNumber = salesOrderDetails.ProjectNumber,
                                TotalRequiredQty = salesOrderDetails.Balance_Qty,
                                PartType = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.PartType).FirstOrDefault()
                            };

                            decimal balanceQuantity = 0;
                            coverageReport.Stock = 0;

                            // Calculate OpenSOQty
                            coverageReport.OpenSOQty = salesOrderDetails.Balance_Qty - coverageReport.Stock;

                            PartType itemPartType = coverageReport.PartType;


                            if (itemPartType == PartType.TG)
                            {
                                // Calculate OpenPoQty
                                var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                              "GetOpenPOTGDetailsByItemAndProjecNoForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber,
                                                                                                                    "&ProjectNumber=", projectNumber));

                                if (purchaseObjectResult != null && purchaseObjectResult.StatusCode == HttpStatusCode.OK)
                                {
                                    var purchaseObjectResults = await purchaseObjectResult.Content.ReadAsStringAsync();
                                    dynamic purchaseObjectData = JsonConvert.DeserializeObject(purchaseObjectResults);
                                    dynamic purchaseObject = purchaseObjectData.data;

                                    if (purchaseObject != null && purchaseObject.Count > 0)
                                    {
                                        decimal OpenPoQty = (decimal)purchaseObject.balanceQty;
                                        coverageReport.OpenPoQty = OpenPoQty;
                                    }
                                    else
                                    {
                                        coverageReport.OpenPoQty = 0;
                                    }
                                }

                            }

                            // Calculate BalanceToOrder

                            var balanceToOrderQty = coverageReport.OpenSOQty - (coverageReport.Stock + coverageReport.OpenPoQty);

                            coverageReport.BalanceToOrder = Convert.ToDecimal(balanceToOrderQty) <= 0 ? 0 : Convert.ToDecimal(balanceToOrderQty);

                            openSalesCoverageReports.Add(coverageReport);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return openSalesCoverageReports;
        }

        private async Task<List<OpenSalesCoverageReport>> FGLevelCoverageReport()
        {
            List<OpenSalesCoverageReport> openSalesCoverageReports = new List<OpenSalesCoverageReport>();
            try
            {
                var salesOrders = await _salesOrderItemsRepository.GetAllSalesOrderFGOrTGItemDetails();


                //List<(string?, decimal?)> itemNumberList = salesOrders.Select(x => (x.FGItemNumber, x.Balance_Qty)).ToList();

                List<string?> itemNumberList = salesOrders.Select(x => x.FGItemNumber).ToList();
                //change
                var itemNoListJson = JsonConvert.SerializeObject(itemNumberList);
                var itemNoListString = new StringContent(itemNoListJson, Encoding.UTF8, "application/json");
                var responses = await _httpClient.PostAsync(string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"), itemNoListString);

                var itemNoPartTypeString = await responses.Content.ReadAsStringAsync();
                dynamic itemNoPartTypeData = JsonConvert.DeserializeObject(itemNoPartTypeString);
                //List<ItemNoWithPartTypeDto> itemNoWithPartType = (List<ItemNoWithPartTypeDto>)itemNoPartTypeData.data;

                List<ItemNoWithPartTypeDto> itemNoWithPartType = new List<ItemNoWithPartTypeDto>();

                //for this loop we need to check
                foreach (var item in itemNoPartTypeData.data)
                {
                    ItemNoWithPartTypeDto dto = JsonConvert.DeserializeObject<ItemNoWithPartTypeDto>(item.ToString());
                    itemNoWithPartType.Add(dto);
                }

                var salesOrderItemListjson = JsonConvert.SerializeObject(itemNumberList);
                var salesOrderItemDetailsString = new StringContent(salesOrderItemListjson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "GetConsumptionInventoryByItemNotest1"), salesOrderItemDetailsString);
                var inventoryObjectString = await response.Content.ReadAsStringAsync();
                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                dynamic inventoryObject = inventoryObjectData.data;

                foreach (var salesOrderDetails in salesOrders)
                {
                    if (inventoryObject != null && inventoryObject.Count > 0)
                    {
                        foreach (var Inventory in inventoryObject)
                        {
                            if (salesOrderDetails.FGItemNumber == Inventory["partNumber"]?.ToString())
                            {
                                if (Inventory != null)
                                {
                                    if (salesOrderDetails.FGItemNumber != null && salesOrderDetails.Balance_Qty != 0)
                                    {
                                        var coverageReport = new OpenSalesCoverageReport
                                        {
                                            ItemNumber = salesOrderDetails.FGItemNumber,
                                            TotalRequiredQty = salesOrderDetails.Balance_Qty,
                                            PartType = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.PartType).FirstOrDefault()
                                        };

                                        decimal balanceQuantity = (decimal)Inventory.balance_Quantity; ; // Convert to decimal
                                        coverageReport.Stock = balanceQuantity;



                                        PartType itemPartType = coverageReport.PartType;


                                        if (itemPartType == PartType.TG)
                                        {
                                            // Calculate OpenPoQty
                                            var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                                          "GetOpenPOTGDetailsByItemForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber));

                                            if (purchaseObjectResult != null && purchaseObjectResult.StatusCode == HttpStatusCode.OK)
                                            {
                                                var purchaseObjectResults = await purchaseObjectResult.Content.ReadAsStringAsync();
                                                dynamic purchaseObjectData = JsonConvert.DeserializeObject(purchaseObjectResults);
                                                dynamic purchaseObject = purchaseObjectData.data;

                                                if (purchaseObject != null)
                                                {
                                                    decimal OpenPoQty = (decimal)purchaseObject.balanceQty;
                                                    coverageReport.OpenPoQty = OpenPoQty;
                                                }
                                                else
                                                {
                                                    coverageReport.OpenPoQty = 0;
                                                }
                                            }
                                        }
                                        // Calculate OpenSOQty
                                        coverageReport.OpenSOQty = salesOrderDetails.Balance_Qty - coverageReport.Stock;
                                        // Calculate BalanceToOrder

                                        var balanceToOrderQty = coverageReport.OpenSOQty - (coverageReport.Stock + coverageReport.OpenPoQty);

                                        coverageReport.BalanceToOrder = Convert.ToDecimal(balanceToOrderQty) <= 0 ? 0 : Convert.ToDecimal(balanceToOrderQty);


                                        //var balanceToOrderQty = coverageReport.OpenSOQty - (coverageReport.Stock + coverageReport.OpenPoQty);
                                        //coverageReport.BalanceToOrder = balanceToOrderQty.HasValue && balanceToOrderQty.Value > 0 ? balanceToOrderQty.Value : 0;

                                        openSalesCoverageReports.Add(coverageReport);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (salesOrderDetails.FGItemNumber != null && salesOrderDetails.Balance_Qty != 0)
                        {
                            var coverageReport = new OpenSalesCoverageReport
                            {
                                ItemNumber = salesOrderDetails.FGItemNumber,
                                TotalRequiredQty = salesOrderDetails.Balance_Qty,
                                PartType = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.PartType).FirstOrDefault()
                            };

                            decimal balanceQuantity = 0;
                            coverageReport.Stock = 0;

                            // Calculate OpenSOQty
                            coverageReport.OpenSOQty = salesOrderDetails.Balance_Qty - coverageReport.Stock;

                            PartType itemPartType = coverageReport.PartType;


                            if (itemPartType == PartType.TG)
                            {
                                // Calculate OpenPoQty
                                var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                              "GetOpenPOTGDetailsByItemForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber));

                                if (purchaseObjectResult != null && purchaseObjectResult.StatusCode == HttpStatusCode.OK)
                                {
                                    var purchaseObjectResults = await purchaseObjectResult.Content.ReadAsStringAsync();
                                    dynamic purchaseObjectData = JsonConvert.DeserializeObject(purchaseObjectResults);
                                    dynamic purchaseObject = purchaseObjectData.data;

                                    if (purchaseObject != null)
                                    {
                                        decimal OpenPoQty = (decimal)purchaseObject.balanceQty;
                                        coverageReport.OpenPoQty = OpenPoQty;
                                    }
                                    else
                                    {
                                        coverageReport.OpenPoQty = 0;
                                    }
                                }

                            }

                            // Calculate BalanceToOrder

                            var balanceToOrderQty = coverageReport.OpenSOQty - (coverageReport.Stock + coverageReport.OpenPoQty);

                            coverageReport.BalanceToOrder = Convert.ToDecimal(balanceToOrderQty) <= 0 ? 0 : Convert.ToDecimal(balanceToOrderQty);

                            openSalesCoverageReports.Add(coverageReport);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return openSalesCoverageReports;
        }

        [HttpGet]
        public async Task<IActionResult> GenerateCoverageReportForFgChildItByProjectNumber(string projectNumber)
        {
            ServiceResponse<List<CoverageReportByProjectNumberDtoForChildItem>> serviceResponse = new ServiceResponse<List<CoverageReportByProjectNumberDtoForChildItem>>();

            try
            {
                List<CoverageReportByProjectNumberDtoForChildItem> coverageReportDtoForChildItemList = new List<CoverageReportByProjectNumberDtoForChildItem>();
                List<OpenSalesCoverageReportByProjectNumber> openSalesCoverageReportsByProjectNo = await FGLevelCoverageReportByProjectNumber(projectNumber);


                if (openSalesCoverageReportsByProjectNo != null && openSalesCoverageReportsByProjectNo.Count() != 0)
                {
                    List<OpenSalesCoverageReportByProjectNumber> openFGCoverageDetails = openSalesCoverageReportsByProjectNo
                        .Where(x => x.PartType == PartType.FG && x.BalanceToOrder > 0).ToList();
                    if (openFGCoverageDetails != null && openFGCoverageDetails.Count() != 0)
                    {
                        // Child Item Required Qty from BOM
                        List<CoverageReportChildItemReqQtyDataByProjectNoDto> childItemReqQtyDtos = await GetChildItemRequiredQtyFromBomByProjectNo(openFGCoverageDetails);

                        if (childItemReqQtyDtos != null && childItemReqQtyDtos.Count() != 0)
                        {
                            List<string?> itemNumberList = childItemReqQtyDtos.Select(x => x.ItemNumber).ToList();


                            var itemNoListJson = JsonConvert.SerializeObject(itemNumberList);
                            var itemNoListString = new StringContent(itemNoListJson, Encoding.UTF8, "application/json");


                            //Open Stock with WIP Quantity
                            List<ChildItemStockWithWipDto> itemStockWithWipList = await GetStockWithWipQtyForChildItemsByProjectNo(itemNoListString, projectNumber);

                            //Open PO Qty for Child Items
                            List<OpenPoQuantityDto> openPoQtyList = await GetOpenPoQtyForChildItemsByProjectNo(itemNoListString, projectNumber);


                            foreach (var item in childItemReqQtyDtos)
                            {
                                CoverageReportByProjectNumberDtoForChildItem coverageDetailOfChildItem = new CoverageReportByProjectNumberDtoForChildItem
                                {
                                    ItemNumber = item.ItemNumber,
                                    Description = item.Description,
                                    ProjectNumber = projectNumber,
                                    PartType = item.PartType,
                                    RequiredQty = item.RequiredQty,
                                    Stock = itemStockWithWipList?.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.BalanceQuantity).FirstOrDefault(),
                                    WipQty = itemStockWithWipList?.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.WipQuantity).FirstOrDefault(),
                                    OpenPoQty = openPoQtyList?.Where(x => x.ItemNumber == item.ItemNumber).Select(x => x.OpenPoQty).FirstOrDefault()
                                };

                                //test



                                decimal? balanceRequiredQty = coverageDetailOfChildItem.RequiredQty - (coverageDetailOfChildItem.Stock
                                   + coverageDetailOfChildItem.OpenPoQty + coverageDetailOfChildItem.WipQty);

                                coverageDetailOfChildItem.BalanceToOrder = balanceRequiredQty <= 0 ? 0 : balanceRequiredQty;

                                coverageReportDtoForChildItemList.Add(coverageDetailOfChildItem);
                            }


                        }
                    }
                }
                serviceResponse.Data = coverageReportDtoForChildItemList;
                serviceResponse.Message = $"Returned Child Item CoverageReport Successfully in GenerateCoverageReportForFgChildItByProjectNumber ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Something went wrong inside GenerateCoverageReportForFgChildItByProjectNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerateCoverageReportForFgChildIt()
        {
            ServiceResponse<List<CoverageReportDtoForChildItem>> serviceResponse = new ServiceResponse<List<CoverageReportDtoForChildItem>>();

            try
            {
                List<CoverageReportDtoForChildItem> coverageReportDtoForChildItemList = new List<CoverageReportDtoForChildItem>();
                List<OpenSalesCoverageReport> openSalesCoverageReports = await FGLevelCoverageReport();
                //List<OpenSalesCoverageReport> openSalesCoverageReports = new List<OpenSalesCoverageReport>();
                //OpenSalesCoverageReport openSalesCoverageReport = new OpenSalesCoverageReport
                //{
                //    ItemNumber = "1002370",
                //    PartType = PartType.FG,
                //    OpenSOQty = 100,
                //    TotalRequiredQty = 80,
                //    Stock = 40,
                //    OpenPoQty = 20,
                //    BalanceToOrder = 20,

                //};
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

                if (openSalesCoverageReports != null && openSalesCoverageReports.Count() != 0)
                {
                    List<OpenSalesCoverageReport> openFGCoverageDetails = openSalesCoverageReports
                        .Where(x => x.PartType == PartType.FG && x.BalanceToOrder > 0).ToList();
                    if (openFGCoverageDetails != null && openFGCoverageDetails.Count() != 0)
                    {
                        // Child Item Required Qty from BOM
                        List<CoverageReportChildItemReqQtyDataDto> childItemReqQtyDtos = await GetChildItemRequiredQtyFromBom(openFGCoverageDetails);

                        if (childItemReqQtyDtos != null && childItemReqQtyDtos.Count() != 0)
                        {
                            List<string?> itemNumberList = childItemReqQtyDtos.Select(x => x.ItemNumber).ToList();


                            var itemNoListJson = JsonConvert.SerializeObject(itemNumberList);
                            var itemNoListString = new StringContent(itemNoListJson, Encoding.UTF8, "application/json");

                            //Open Stock with WIP Quantity
                            List<ChildItemStockWithWipDto> itemStockWithWipList = await GetStockWithWipQtyForChildItems(itemNoListString);

                            //Open PO Qty for Child Items
                            List<OpenPoQuantityDto> openPoQtyList = await GetOpenPoQtyForChildItems(itemNoListString);


                            foreach (var item in childItemReqQtyDtos)
                            {
                                CoverageReportDtoForChildItem coverageDetailOfChildItem = new CoverageReportDtoForChildItem
                                {
                                    ItemNumber = item.ItemNumber,
                                    PartType = item.PartType,
                                    RequiredQty = item.RequiredQty,
                                    Stock = itemStockWithWipList?.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.BalanceQuantity).FirstOrDefault(),
                                    WipQty = itemStockWithWipList?.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.WipQuantity).FirstOrDefault(),
                                    OpenPoQty = openPoQtyList?.Where(x => x.ItemNumber == item.ItemNumber).Select(x => x.OpenPoQty).FirstOrDefault()
                                };

                                //test



                                decimal? balanceRequiredQty = coverageDetailOfChildItem.RequiredQty - (coverageDetailOfChildItem.Stock
                                   + coverageDetailOfChildItem.OpenPoQty + coverageDetailOfChildItem.WipQty);

                                coverageDetailOfChildItem.BalanceToOrder = balanceRequiredQty <= 0 ? 0 : balanceRequiredQty;

                                coverageReportDtoForChildItemList.Add(coverageDetailOfChildItem);
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

        private async Task<List<ChildItemStockWithWipDto>> GetStockWithWipQtyForChildItemsByProjectNo(StringContent itemNoListString, string projectno)
        {
            var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "GetConsumptionChildItemStockWithWipQtyByProjectNo?",
                                                                                        "ProjectNo=", projectno), itemNoListString);
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
        private async Task<List<OpenPoQuantityDto>> GetOpenPoQtyForChildItemsByProjectNo(StringContent itemNoListString, string projectNo)
        {
            var openPoQtyResponse = await _httpClient.PostAsync(string.Concat(_config["PurchaseAPI"],
                                            "GetListOfOpenPOQtyByItemNoListByProjectNo?", "ProjectNo=", projectNo), itemNoListString);
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

        private async Task<List<CoverageReportChildItemReqQtyDataDto>> GetChildItemRequiredQtyFromBom(List<OpenSalesCoverageReport> openFGCoverageDetails)
        {
            var openFGCoverageDetailsJson = JsonConvert.SerializeObject(openFGCoverageDetails);
            var openFGCoverageDetailsString = new StringContent(openFGCoverageDetailsJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "GetBomDetailsForCoverageReport"), openFGCoverageDetailsString);

            var childItemRequiredQtyString = await response.Content.ReadAsStringAsync();
            dynamic childItemRequiredQtyData = JsonConvert.DeserializeObject(childItemRequiredQtyString);

            List<CoverageReportChildItemReqQtyDataDto> childItemReqQtyDtos = new List<CoverageReportChildItemReqQtyDataDto>();

            foreach (var item in childItemRequiredQtyData.data)
            {
                CoverageReportChildItemReqQtyDataDto dto = JsonConvert.DeserializeObject<CoverageReportChildItemReqQtyDataDto>(item.ToString());
                childItemReqQtyDtos.Add(dto);
            }

            return childItemReqQtyDtos;


        }
        private async Task<List<CoverageReportChildItemReqQtyDataByProjectNoDto>> GetChildItemRequiredQtyFromBomByProjectNo(List<OpenSalesCoverageReportByProjectNumber> openFGCoverageDetails)
        {
            var openFGCoverageDetailsJson = JsonConvert.SerializeObject(openFGCoverageDetails);
            var openFGCoverageDetailsString = new StringContent(openFGCoverageDetailsJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "GetBomDetailsByProjectNoForCoverageReport"),
                                                                                                                            openFGCoverageDetailsString);

            var childItemRequiredQtyString = await response.Content.ReadAsStringAsync();
            dynamic childItemRequiredQtyData = JsonConvert.DeserializeObject(childItemRequiredQtyString);

            List<CoverageReportChildItemReqQtyDataByProjectNoDto> childItemReqQtyDtos = new List<CoverageReportChildItemReqQtyDataByProjectNoDto>();

            foreach (var item in childItemRequiredQtyData.data)
            {
                CoverageReportChildItemReqQtyDataByProjectNoDto dto = JsonConvert.DeserializeObject<CoverageReportChildItemReqQtyDataByProjectNoDto>(item.ToString());
                childItemReqQtyDtos.Add(dto);
            }

            return childItemReqQtyDtos;


        }
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

    }
}
