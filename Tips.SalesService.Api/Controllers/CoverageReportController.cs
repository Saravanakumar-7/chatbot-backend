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
using Microsoft.AspNetCore.Authorization;

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CoverageReportController : ControllerBase
    {
        private ICollectionTrackerRepository _repository;
        private ISalesOrderItemsRepository _salesOrderItemsRepository;
        private ICoverageReportRepository _coverageRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        public CoverageReportController(ICollectionTrackerRepository repository, IHttpClientFactory clientFactory, ISalesOrderItemsRepository salesOrderItemsRepository, HttpClient httpClient, IConfiguration config, ICoverageReportRepository coverageReportRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _coverageRepository = coverageReportRepository;
            _salesOrderItemsRepository = salesOrderItemsRepository;
            _clientFactory = clientFactory;
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
        [HttpPost]
        public async Task<IActionResult> GenerateCoverageFGLevelReportByMultipleProjectNumber([FromBody] CoverageReportProjectDto coverageReportProjectDto)
        {

            ServiceResponse<List<OpenSalesCoverageReportByProjectNumber>> serviceResponse = new ServiceResponse<List<OpenSalesCoverageReportByProjectNumber>>();

            try
            {
                List<OpenSalesCoverageReportByProjectNumber> openSalesCoverageReportsByprojectNo = await FGLevelCoverageReportByMultipleProjectNumber(coverageReportProjectDto);

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

        [HttpGet]
        public async Task<IActionResult> GenerateCoverageFGLevelReportByProjectNumberAndItemNo(string projectNumber, string ItemNumber)
        {

            ServiceResponse<List<OpenSalesCoverageReportByProjectNumber>> serviceResponse = new ServiceResponse<List<OpenSalesCoverageReportByProjectNumber>>();

            try
            {
                List<OpenSalesCoverageReportByProjectNumber> openSalesCoverageReportsByprojectNo = await FGLevelCoverageReportByProjectNumberAndItemNo(projectNumber, ItemNumber);

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
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                //var responses = await _httpClient.PostAsync(string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"), itemNoListString);
                var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"))
                {
                    Content = itemNoListString
                };
                request.Headers.Add("Authorization", token);

                var responses = await client.SendAsync(request);
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
                var client1 = _clientFactory.CreateClient();
                var token1 = HttpContext.Request.Headers["Authorization"].ToString();
                var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"], "GetConsumptionInventoryByItemAndProjectNotest2"))
                {
                    Content = salesOrderItemandProjDetailsString
                };
                request1.Headers.Add("Authorization", token1);

                var response = await client1.SendAsync(request1);
                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"],
                //                                                                "GetConsumptionInventoryByItemAndProjectNotest2"), salesOrderItemandProjDetailsString);
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
                                            MftrItemNumber = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.MftrItemNumber).FirstOrDefault(),
                                            Description = salesOrderDetails.Description,
                                            ProjectNumber = salesOrderDetails.ProjectNumber,
                                            UOM = salesOrderDetails.UOM,
                                            TotalRequiredQty = salesOrderDetails.Balance_Qty,
                                            PartType = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.PartType).FirstOrDefault()
                                        };

                                        decimal balanceQuantity = (decimal)Inventory.balance_Quantity; ; // Convert to decimal
                                        coverageReport.Stock = balanceQuantity;



                                        PartType itemPartType = coverageReport.PartType;


                                        if (itemPartType == PartType.TG)
                                        {
                                            // Calculate OpenPoQty
                                            //var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                            //              "GetOpenPOTGDetailsByItemAndProjecNoForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber,
                                            //                                                                            "&ProjectNumber=", salesOrderDetails.ProjectNumber));
                                            var encodedItemNumber = Uri.EscapeDataString(salesOrderDetails.FGItemNumber);
                                            var encodedProjectNo = Uri.EscapeDataString(projectNumber);
                                            var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PurchaseAPI"], $"GetOpenPOTGDetailsByItemAndProjecNoForCoverage?itemNumber={encodedItemNumber}&ProjectNumber={encodedProjectNo}"));
                                            request2.Headers.Add("Authorization", token);
                                            var purchaseObjectResult = await client.SendAsync(request2);
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
                                MftrItemNumber = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.MftrItemNumber).FirstOrDefault(),
                                Description = salesOrderDetails.Description,
                                ProjectNumber = salesOrderDetails.ProjectNumber,
                                UOM = salesOrderDetails.UOM,
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

                                var encodedItemNumber = Uri.EscapeDataString(salesOrderDetails.FGItemNumber);
                                var encodedProjectNo = Uri.EscapeDataString(projectNumber);
                                var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PurchaseAPI"], $"GetOpenPOTGDetailsByItemAndProjecNoForCoverage?itemNumber={encodedItemNumber}&ProjectNumber={encodedProjectNo}"));
                                request2.Headers.Add("Authorization", token);
                                var purchaseObjectResult = await client.SendAsync(request2);
                                // Calculate OpenPoQty
                                //var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                //"GetOpenPOTGDetailsByItemAndProjecNoForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber,
                                //                                                                      "&ProjectNumber=", projectNumber));

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
        private async Task<List<OpenSalesCoverageReportByProjectNumber>> FGLevelCoverageReportByMultipleProjectNumber(CoverageReportProjectDto coverageReportProjectDto)
        {
            List<OpenSalesCoverageReportByProjectNumber> openSalesCoverageReports = new List<OpenSalesCoverageReportByProjectNumber>();
            try
            {
                var salesOrders = await _salesOrderItemsRepository.GetAllSalesOrderFGOrTGItemDetailsByMultipleProjectNo(coverageReportProjectDto.ProjectNumber);


                //List<(string?, decimal?)> itemNumberList = salesOrders.Select(x => (x.FGItemNumber, x.Balance_Qty)).ToList();

                List<string?> itemNumberList = salesOrders.Select(x => x.FGItemNumber).ToList();
                //change
                var itemNoListJson = JsonConvert.SerializeObject(itemNumberList);
                var itemNoListString = new StringContent(itemNoListJson, Encoding.UTF8, "application/json");
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                //var responses = await _httpClient.PostAsync(string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"), itemNoListString);
                var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"))
                {
                    Content = itemNoListString
                };
                request.Headers.Add("Authorization", token);

                var responses = await client.SendAsync(request);
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
                var client1 = _clientFactory.CreateClient();
                var token1 = HttpContext.Request.Headers["Authorization"].ToString();
                var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"], "GetConsumptionInventoryByItemAndProjectNotest2"))
                {
                    Content = salesOrderItemandProjDetailsString
                };
                request1.Headers.Add("Authorization", token1);

                var response = await client1.SendAsync(request1);
                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"],
                //                                                                "GetConsumptionInventoryByItemAndProjectNotest2"), salesOrderItemandProjDetailsString);
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
                                            MftrItemNumber = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.MftrItemNumber).FirstOrDefault(),
                                            Description = salesOrderDetails.Description,
                                            ProjectNumber = salesOrderDetails.ProjectNumber,
                                            UOM = salesOrderDetails.UOM,
                                            TotalRequiredQty = salesOrderDetails.Balance_Qty,
                                            PartType = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.PartType).FirstOrDefault()
                                        };

                                        decimal balanceQuantity = (decimal)Inventory.balance_Quantity; ; // Convert to decimal
                                        coverageReport.Stock = balanceQuantity;



                                        PartType itemPartType = coverageReport.PartType;


                                        if (itemPartType == PartType.TG)
                                        {
                                            // Calculate OpenPoQty
                                            //var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                            //              "GetOpenPOTGDetailsByItemAndProjecNoForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber,
                                            //                                                                            "&ProjectNumber=", salesOrderDetails.ProjectNumber));
                                            var encodedItemNumber = Uri.EscapeDataString(salesOrderDetails.FGItemNumber);
                                            //var encodedProjectNo = Uri.EscapeDataString(projectNumber);
                                            var encodedProjectNos = new List<string>();
                                            foreach (var projectNo in coverageReportProjectDto.ProjectNumber)
                                            {
                                                encodedProjectNos.Add(Uri.EscapeDataString(projectNo));
                                            }

                                            var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PurchaseAPI"], $"GetOpenPOTGDetailsByItemAndMultipleProjecNoForCoverage?itemNumber={encodedItemNumber}&ProjectNumber={encodedProjectNos}"));
                                            request2.Headers.Add("Authorization", token);
                                            var purchaseObjectResult = await client.SendAsync(request2);
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
                                MftrItemNumber = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.MftrItemNumber).FirstOrDefault(),
                                Description = salesOrderDetails.Description,
                                ProjectNumber = salesOrderDetails.ProjectNumber,
                                UOM = salesOrderDetails.UOM,
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

                                var encodedItemNumber = Uri.EscapeDataString(salesOrderDetails.FGItemNumber);
                                //var encodedProjectNo = Uri.EscapeDataString(projectNumber);
                                var encodedProjectNos = new List<string>();
                                foreach (var projectNo in coverageReportProjectDto.ProjectNumber)
                                {
                                    encodedProjectNos.Add(Uri.EscapeDataString(projectNo));
                                }
                                var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PurchaseAPI"], $"GetOpenPOTGDetailsByItemAndMultipleProjecNoForCoverage?itemNumber={encodedItemNumber}&ProjectNumber={encodedProjectNos}"));
                                request2.Headers.Add("Authorization", token);
                                var purchaseObjectResult = await client.SendAsync(request2);
                                // Calculate OpenPoQty
                                //var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                //"GetOpenPOTGDetailsByItemAndProjecNoForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber,
                                //                                                                      "&ProjectNumber=", projectNumber));

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

        private async Task<List<OpenSalesCoverageReportByProjectNumber>> FGLevelCoverageReportByProjectNumberAndItemNo(string projectNumber, string itemNumber)
        {
            List<OpenSalesCoverageReportByProjectNumber> openSalesCoverageReports = new List<OpenSalesCoverageReportByProjectNumber>();
            try
            {
                var salesOrders = await _salesOrderItemsRepository.GetAllSalesOrderFGOrTGItemDetailsByProjectNoAndItemNo(projectNumber, itemNumber);


                //List<(string?, decimal?)> itemNumberList = salesOrders.Select(x => (x.FGItemNumber, x.Balance_Qty)).ToList();

                List<string?> itemNumberList = salesOrders.Select(x => x.FGItemNumber).ToList();
                //change
                var itemNoListJson = JsonConvert.SerializeObject(itemNumberList);
                var itemNoListString = new StringContent(itemNoListJson, Encoding.UTF8, "application/json");
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                //var responses = await _httpClient.PostAsync(string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"), itemNoListString);
                var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"))
                {
                    Content = itemNoListString
                };
                request.Headers.Add("Authorization", token);

                var responses = await client.SendAsync(request);
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
                var client1 = _clientFactory.CreateClient();
                var token1 = HttpContext.Request.Headers["Authorization"].ToString();
                var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"], "GetConsumptionInventoryByItemAndProjectNotest2"))
                {
                    Content = salesOrderItemandProjDetailsString
                };
                request1.Headers.Add("Authorization", token1);

                var response = await client1.SendAsync(request1);
                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"],
                //                                                                "GetConsumptionInventoryByItemAndProjectNotest2"), salesOrderItemandProjDetailsString);
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
                                            MftrItemNumber = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.MftrItemNumber).FirstOrDefault(),
                                            Description = salesOrderDetails.Description,
                                            ProjectNumber = salesOrderDetails.ProjectNumber,
                                            UOM = salesOrderDetails.UOM,
                                            TotalRequiredQty = salesOrderDetails.Balance_Qty,
                                            PartType = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.PartType).FirstOrDefault()
                                        };

                                        decimal balanceQuantity = (decimal)Inventory.balance_Quantity; ; // Convert to decimal
                                        coverageReport.Stock = balanceQuantity;



                                        PartType itemPartType = coverageReport.PartType;


                                        if (itemPartType == PartType.TG)
                                        {
                                            // Calculate OpenPoQty
                                            //var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                            //              "GetOpenPOTGDetailsByItemAndProjecNoForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber,
                                            //                                                                            "&ProjectNumber=", salesOrderDetails.ProjectNumber));
                                            var encodedItemNumber = Uri.EscapeDataString(salesOrderDetails.FGItemNumber);
                                            var encodedProjectNo = Uri.EscapeDataString(projectNumber);
                                            var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PurchaseAPI"], $"GetOpenPOTGDetailsByItemAndProjecNoForCoverage?itemNumber={encodedItemNumber}&ProjectNumber={encodedProjectNo}"));
                                            request2.Headers.Add("Authorization", token);
                                            var purchaseObjectResult = await client.SendAsync(request2);
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
                                MftrItemNumber = itemNoWithPartType.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.MftrItemNumber).FirstOrDefault(),
                                Description = salesOrderDetails.Description,
                                ProjectNumber = salesOrderDetails.ProjectNumber,
                                UOM = salesOrderDetails.UOM,
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

                                var encodedItemNumber = Uri.EscapeDataString(salesOrderDetails.FGItemNumber);
                                var encodedProjectNo = Uri.EscapeDataString(projectNumber);
                                var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PurchaseAPI"], $"GetOpenPOTGDetailsByItemAndProjecNoForCoverage?itemNumber={encodedItemNumber}&ProjectNumber={encodedProjectNo}"));
                                request2.Headers.Add("Authorization", token);
                                var purchaseObjectResult = await client.SendAsync(request2);
                                // Calculate OpenPoQty
                                //var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                //"GetOpenPOTGDetailsByItemAndProjecNoForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber,
                                //                                                                      "&ProjectNumber=", projectNumber));

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
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                // var responses = await _httpClient.PostAsync(string.Concat(_config["ItemMasterMainAPI"], "GetItemMasterPartTypeAndMinByItemNumber"), itemNoListString);
                var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterMainAPI"],
                             "GetItemMasterPartTypeAndMinByItemNumber"))
                {
                    Content = itemNoListString
                };
                request.Headers.Add("Authorization", token);

                var responses = await client.SendAsync(request);
                var itemNoPartTypeString = await responses.Content.ReadAsStringAsync();
                dynamic itemNoPartTypeData = JsonConvert.DeserializeObject(itemNoPartTypeString);
                //List<ItemNoWithPartTypeDto> itemNoWithPartType = (List<ItemNoWithPartTypeDto>)itemNoPartTypeData.data;

                List<ItemNoWithPartTypeAndMinDto> itemNoWithPartTypeAndMin = new List<ItemNoWithPartTypeAndMinDto>();

                //for this loop we need to check
                foreach (var item in itemNoPartTypeData.data)
                {
                    ItemNoWithPartTypeAndMinDto dto = JsonConvert.DeserializeObject<ItemNoWithPartTypeAndMinDto>(item.ToString());
                    itemNoWithPartTypeAndMin.Add(dto);
                }

                var salesOrderItemListjson = JsonConvert.SerializeObject(itemNumberList);
                var salesOrderItemDetailsString = new StringContent(salesOrderItemListjson, Encoding.UTF8, "application/json");
                var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                            "GetConsumptionInventoryByItemNotest1"))
                {
                    Content = salesOrderItemDetailsString
                };
                request1.Headers.Add("Authorization", token);

                var response = await client.SendAsync(request1);
                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "GetConsumptionInventoryByItemNotest1"), salesOrderItemDetailsString);
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
                                            PartType = itemNoWithPartTypeAndMin.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.PartType).FirstOrDefault(),
                                            MSL = itemNoWithPartTypeAndMin.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.Min).FirstOrDefault(),
                                            UOM = itemNoWithPartTypeAndMin.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.UOM).FirstOrDefault()
                                        };

                                        decimal balanceQuantity = (decimal)Inventory.balance_Quantity; ; // Convert to decimal
                                        coverageReport.Stock = balanceQuantity;



                                        PartType itemPartType = coverageReport.PartType;


                                        if (itemPartType == PartType.TG)
                                        {
                                            // Calculate OpenPoQty
                                            //var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                            //              "GetOpenPOTGDetailsByItemForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber));

                                            var encodedItemNumber = Uri.EscapeDataString(salesOrderDetails.FGItemNumber);
                                            var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PurchaseAPI"], $"GetOpenPOTGDetailsByItemForCoverage?itemNumber={encodedItemNumber}"));
                                            request2.Headers.Add("Authorization", token);

                                            var purchaseObjectResult = await client.SendAsync(request2);
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
                                        //Calculate OpenRetailSOQty
                                        var fGItemNumber = salesOrderDetails.FGItemNumber;

                                        var salesOrderRetailDetails = await _salesOrderItemsRepository.GetAllSalesOrderFGOrTGRetailItemDetails(fGItemNumber);
                                        if (salesOrderRetailDetails != null)
                                        {
                                            coverageReport.OpenRetailSOQty = salesOrderRetailDetails.Balance_Qty;
                                        }
                                        else
                                        {
                                            coverageReport.OpenRetailSOQty = 0;
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
                                PartType = itemNoWithPartTypeAndMin.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.PartType).FirstOrDefault(),
                                MSL = itemNoWithPartTypeAndMin.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.Min).FirstOrDefault(),
                                UOM = itemNoWithPartTypeAndMin.Where(x => x.ItemNumber == salesOrderDetails.FGItemNumber).Select(i => i.UOM).FirstOrDefault()
                            };

                            decimal balanceQuantity = 0;
                            coverageReport.Stock = 0;

                            // Calculate OpenSOQty
                            coverageReport.OpenSOQty = salesOrderDetails.Balance_Qty - coverageReport.Stock;

                            //Calculate OpenRetailSOQty
                            var fGItemNumber = salesOrderDetails.FGItemNumber;

                            var salesOrderRetailDetails = await _salesOrderItemsRepository.GetAllSalesOrderFGOrTGRetailItemDetails(fGItemNumber);
                            coverageReport.OpenRetailSOQty = salesOrderRetailDetails.Balance_Qty;

                            PartType itemPartType = coverageReport.PartType;


                            if (itemPartType == PartType.TG)
                            {
                                // Calculate OpenPoQty
                                //var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                                //              "GetOpenPOTGDetailsByItemForCoverage?", "itemNumber=", salesOrderDetails.FGItemNumber));
                                var encodedItemNumber = Uri.EscapeDataString(salesOrderDetails.FGItemNumber);
                                var request3 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PurchaseAPI"], $"GetOpenPOTGDetailsByItemForCoverage?itemNumber={encodedItemNumber}"));
                                request3.Headers.Add("Authorization", token);

                                var purchaseObjectResult = await client.SendAsync(request3);
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
                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();
                            // var responses = await _httpClient.PostAsync(string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"), itemNoListString);
                            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"))
                            {
                                Content = itemNoListString
                            };
                            request.Headers.Add("Authorization", token);

                            var responses = await client.SendAsync(request);
                            var itemNoPartTypeString = await responses.Content.ReadAsStringAsync();
                            dynamic itemNoPartTypeData = JsonConvert.DeserializeObject(itemNoPartTypeString);

                            List<ItemNoWithPartTypeDto> itemNoWithPartType = new List<ItemNoWithPartTypeDto>();

                            //for this loop we need to check
                            foreach (var item in itemNoPartTypeData.data)
                            {
                                ItemNoWithPartTypeDto dto = JsonConvert.DeserializeObject<ItemNoWithPartTypeDto>(item.ToString());
                                itemNoWithPartType.Add(dto);
                            }

                            //Open Stock with WIP Quantity
                            List<ChildItemStockWithWipDto> itemStockWithWipList = await GetStockWithWipQtyForChildItemsByProjectNo(itemNoListString, projectNumber);

                            //Open PO Qty for Child Items
                            List<OpenPoQuantityDto> openPoQtyList = await GetOpenPoQtyForChildItemsByProjectNo(itemNoListString, projectNumber);


                            foreach (var item in childItemReqQtyDtos)
                            {
                                CoverageReportByProjectNumberDtoForChildItem coverageDetailOfChildItem = new CoverageReportByProjectNumberDtoForChildItem
                                {
                                    ItemNumber = item.ItemNumber,
                                    MftrItemNumber = itemNoWithPartType.Where(x => x.ItemNumber == item.ItemNumber).Select(i => i.MftrItemNumber).FirstOrDefault(),
                                    Version = item.Version,
                                    Description = item.Description,
                                    ProjectNumber = projectNumber,
                                    UOM = item.UOM,
                                    PartType = item.PartType,
                                    RequiredQty = Math.Round(item.RequiredQty, MidpointRounding.AwayFromZero),
                                    Stock = itemStockWithWipList?.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.BalanceQuantity).FirstOrDefault(),
                                    WipQty = itemStockWithWipList?.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.WipQuantity).FirstOrDefault(),
                                    //OpenPoQty = openPoQtyList?.Where(x => x.ItemNumber == item.ItemNumber).Select(x => x.OpenPoQty).FirstOrDefault()
                                };

                                //Binning Qty 
                                List<BinningQuantityDto> binningQtyList = await GetBinningQtyForChildItemsByProjectNo(item.ItemNumber, projectNumber);
                                
                                var OpenPoQty = openPoQtyList?.Where(x => x.ItemNumber == item.ItemNumber).Select(x => x.OpenPoQty).FirstOrDefault();
                                var binningQty = binningQtyList?.Where(x => x.ItemNumber == item.ItemNumber).Select(x => x.BinningQty).FirstOrDefault();

                                //Open PoQty Calculate
                                if (binningQtyList != null && binningQtyList.Count() > 0)
                                {
                                    coverageDetailOfChildItem.OpenPoQty = OpenPoQty - binningQty;
                                }
                                else
                                {
                                    coverageDetailOfChildItem.OpenPoQty = OpenPoQty - binningQty;
                                }

                                decimal? balanceRequiredQty = coverageDetailOfChildItem.RequiredQty - (coverageDetailOfChildItem.Stock
                                   + coverageDetailOfChildItem.OpenPoQty + coverageDetailOfChildItem.WipQty);

                                coverageDetailOfChildItem.BalanceToOrder = balanceRequiredQty <= 0 ? 0 : Math.Round(balanceRequiredQty.Value, MidpointRounding.AwayFromZero);

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

        [HttpPost]
        public async Task<IActionResult> GenerateCoverageReportForFgChildItByMultipleProjectNumber([FromBody] CoverageReportProjectDto coverageReportProjectDto)
        {
            ServiceResponse<List<CoverageReportByProjectNumberDtoForChildItem>> serviceResponse = new ServiceResponse<List<CoverageReportByProjectNumberDtoForChildItem>>();

            try
            {
                List<CoverageReportByProjectNumberDtoForChildItem> coverageReportDtoForChildItemList = new List<CoverageReportByProjectNumberDtoForChildItem>();
                List<OpenSalesCoverageReportByProjectNumber> openSalesCoverageReportsByProjectNo = await FGLevelCoverageReportByMultipleProjectNumber(coverageReportProjectDto);


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
                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();
                            // var responses = await _httpClient.PostAsync(string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"), itemNoListString);
                            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"))
                            {
                                Content = itemNoListString
                            };
                            request.Headers.Add("Authorization", token);

                            var responses = await client.SendAsync(request);
                            var itemNoPartTypeString = await responses.Content.ReadAsStringAsync();
                            dynamic itemNoPartTypeData = JsonConvert.DeserializeObject(itemNoPartTypeString);

                            List<ItemNoWithPartTypeDto> itemNoWithPartType = new List<ItemNoWithPartTypeDto>();

                            //for this loop we need to check
                            foreach (var item in itemNoPartTypeData.data)
                            {
                                ItemNoWithPartTypeDto dto = JsonConvert.DeserializeObject<ItemNoWithPartTypeDto>(item.ToString());
                                itemNoWithPartType.Add(dto);
                            }
                            var projNoListJson = JsonConvert.SerializeObject(coverageReportProjectDto.ProjectNumber);
                            var projNoListString = new StringContent(projNoListJson, Encoding.UTF8, "application/json");

                            //Open Stock with WIP Quantity
                            List<ChildItemStockWithProjectListWipDto> itemStockWithWipList = await GetStockWithWipQtyForChildItemsByMultipleProjectNo(itemNumberList, coverageReportProjectDto.ProjectNumber);

                            //Open PO Qty for Child Items
                            List<OpenPoQuantityDto> openPoQtyList = await GetOpenPoQtyForChildItemsByMultipleProjectNo(itemNumberList, coverageReportProjectDto.ProjectNumber);


                            foreach (var item in childItemReqQtyDtos)
                            {
                                CoverageReportByProjectNumberDtoForChildItem coverageDetailOfChildItem = new CoverageReportByProjectNumberDtoForChildItem
                                {
                                    ItemNumber = item.ItemNumber,
                                    MftrItemNumber = itemNoWithPartType.Where(x => x.ItemNumber == item.ItemNumber).Select(i => i.MftrItemNumber).FirstOrDefault(),
                                    Version = item.Version,
                                    Description = item.Description,
                                    ProjectNumber = itemStockWithWipList?.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.ProjectNumber).FirstOrDefault(),
                                    UOM = item.UOM,
                                    PartType = item.PartType,
                                    RequiredQty = Math.Round(item.RequiredQty, MidpointRounding.AwayFromZero),
                                    Stock = itemStockWithWipList?.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.BalanceQuantity).FirstOrDefault(),
                                    WipQty = itemStockWithWipList?.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.WipQuantity).FirstOrDefault(),
                                    //OpenPoQty = openPoQtyList?.Where(x => x.ItemNumber == item.ItemNumber).Select(x => x.OpenPoQty).FirstOrDefault()
                                };

                                //Binning Qty 
                                List<BinningQuantityDto> binningQtyList = await GetBinningQtyForChildItemsByMultipleProjectNo(item.ItemNumber, coverageReportProjectDto.ProjectNumber);

                                var OpenPoQty = openPoQtyList?.Where(x => x.ItemNumber == item.ItemNumber).Select(x => x.OpenPoQty).FirstOrDefault();
                                var binningQty = binningQtyList?.Where(x => x.ItemNumber == item.ItemNumber).Select(x => x.BinningQty).FirstOrDefault();

                                //Open PoQty Calculate
                                if (binningQtyList != null && binningQtyList.Count() > 0)
                                {
                                    coverageDetailOfChildItem.OpenPoQty = OpenPoQty - binningQty;
                                }
                                else
                                {
                                    coverageDetailOfChildItem.OpenPoQty = OpenPoQty - binningQty;
                                }

                                decimal? balanceRequiredQty = coverageDetailOfChildItem.RequiredQty - (coverageDetailOfChildItem.Stock
                                   + coverageDetailOfChildItem.OpenPoQty + coverageDetailOfChildItem.WipQty);

                                coverageDetailOfChildItem.BalanceToOrder = balanceRequiredQty <= 0 ? 0 : Math.Round(balanceRequiredQty.Value, MidpointRounding.AwayFromZero);

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
        public async Task<IActionResult> GenerateCoverageReportForFgChildItByProjectNumberAndItemNo(string projectNumber, string itemNumber)
        {
            ServiceResponse<List<CoverageReportByProjectNumberDtoForChildItem>> serviceResponse = new ServiceResponse<List<CoverageReportByProjectNumberDtoForChildItem>>();

            try
            {
                List<CoverageReportByProjectNumberDtoForChildItem> coverageReportDtoForChildItemList = new List<CoverageReportByProjectNumberDtoForChildItem>();
                List<OpenSalesCoverageReportByProjectNumber> openSalesCoverageReportsByProjectNo = await FGLevelCoverageReportByProjectNumberAndItemNo(projectNumber, itemNumber);


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
                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();
                            // var responses = await _httpClient.PostAsync(string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"), itemNoListString);
                            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterMainAPI"], "GetItemPartTypeByItemNumber"))
                            {
                                Content = itemNoListString
                            };
                            request.Headers.Add("Authorization", token);

                            var responses = await client.SendAsync(request);
                            var itemNoPartTypeString = await responses.Content.ReadAsStringAsync();
                            dynamic itemNoPartTypeData = JsonConvert.DeserializeObject(itemNoPartTypeString);

                            List<ItemNoWithPartTypeDto> itemNoWithPartType = new List<ItemNoWithPartTypeDto>();

                            //for this loop we need to check
                            foreach (var item in itemNoPartTypeData.data)
                            {
                                ItemNoWithPartTypeDto dto = JsonConvert.DeserializeObject<ItemNoWithPartTypeDto>(item.ToString());
                                itemNoWithPartType.Add(dto);
                            }

                            //Open Stock with WIP Quantity
                            List<ChildItemStockWithWipDto> itemStockWithWipList = await GetStockWithWipQtyForChildItemsByProjectNo(itemNoListString, projectNumber);

                            //Open PO Qty for Child Items
                            List<OpenPoQuantityDto> openPoQtyList = await GetOpenPoQtyForChildItemsByProjectNo(itemNoListString, projectNumber);


                            foreach (var item in childItemReqQtyDtos)
                            {
                                CoverageReportByProjectNumberDtoForChildItem coverageDetailOfChildItem = new CoverageReportByProjectNumberDtoForChildItem
                                {
                                    ItemNumber = item.ItemNumber,
                                    MftrItemNumber = itemNoWithPartType.Where(x => x.ItemNumber == item.ItemNumber).Select(i => i.MftrItemNumber).FirstOrDefault(),
                                    Description = item.Description,
                                    ProjectNumber = projectNumber,
                                    UOM = item.UOM,
                                    PartType = item.PartType,
                                    RequiredQty = Math.Round(item.RequiredQty, MidpointRounding.AwayFromZero),
                                    Stock = itemStockWithWipList?.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.BalanceQuantity).FirstOrDefault(),
                                    WipQty = itemStockWithWipList?.Where(x => x.PartNumber == item.ItemNumber).Select(x => x.WipQuantity).FirstOrDefault(),
                                    OpenPoQty = openPoQtyList?.Where(x => x.ItemNumber == item.ItemNumber).Select(x => x.OpenPoQty).FirstOrDefault()
                                };

                                //test



                                decimal? balanceRequiredQty = coverageDetailOfChildItem.RequiredQty - (coverageDetailOfChildItem.Stock
                                   + coverageDetailOfChildItem.OpenPoQty + coverageDetailOfChildItem.WipQty);

                                coverageDetailOfChildItem.BalanceToOrder = balanceRequiredQty <= 0 ? 0 : Math.Round(balanceRequiredQty.Value, MidpointRounding.AwayFromZero); ;

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
                                    UOM = item.UOM,
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
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                           "GetConsumptionChildItemStockWithWipQty"))
            {
                Content = itemNoListString
            };
            request.Headers.Add("Authorization", token);

            var responses = await client.SendAsync(request);
            //var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "GetConsumptionChildItemStockWithWipQty"), itemNoListString);
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
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var encodedProjectNo = Uri.EscapeDataString(projectno);
            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"], $"GetConsumptionChildItemStockWithWipQtyByProjectNo?ProjectNo={encodedProjectNo}"))
            {
                Content = itemNoListString
            };
            request.Headers.Add("Authorization", token);

            var responses = await client.SendAsync(request);
            //var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "GetConsumptionChildItemStockWithWipQtyByProjectNo?",
            //                                                                            "ProjectNo=", projectno), itemNoListString);
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

        private async Task<List<ChildItemStockWithProjectListWipDto>> GetStockWithWipQtyForChildItemsByMultipleProjectNo(List<string> itemNumberList, List<string> projectNoList)
        {
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            // Serialize the lists into a single JSON object
            var requestData = new
            {
                itemNumberList = itemNumberList,
                projectNo = projectNoList
            };
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"], "GetConsumptionChildItemStockWithWipQtyByMultipleProjectNo"))
            {
                Content = content
            };
            request.Headers.Add("Authorization", token);

            var responses = await client.SendAsync(request);
            //var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "GetConsumptionChildItemStockWithWipQtyByProjectNo?",
            //                                                                            "ProjectNo=", projectno), itemNoListString);
            var itemStockWithWipString = await responses.Content.ReadAsStringAsync();
            dynamic itemStockWithWipData = JsonConvert.DeserializeObject(itemStockWithWipString);
            //List<ChildItemStockWithWipDto> itemStockWithWipList = (List<ChildItemStockWithWipDto>)itemStockWithWipData.data;


            List<ChildItemStockWithProjectListWipDto> childItemStockWithWipQty = new List<ChildItemStockWithProjectListWipDto>();

            foreach (var item in itemStockWithWipData.data)
            {
                ChildItemStockWithProjectListWipDto dto = JsonConvert.DeserializeObject<ChildItemStockWithProjectListWipDto>(item.ToString());
                childItemStockWithWipQty.Add(dto);
            }

            return childItemStockWithWipQty;
        }

        private async Task<List<OpenPoQuantityDto>> GetOpenPoQtyForChildItems(StringContent itemNoListString)
        {
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"],
                           "GetListOfOpenPOQtyByItemNoList"))
            {
                Content = itemNoListString
            };
            request.Headers.Add("Authorization", token);

            var openPoQtyResponse = await client.SendAsync(request);
            //var openPoQtyResponse = await _httpClient.PostAsync(string.Concat(_config["PurchaseAPI"],
            //                                "GetListOfOpenPOQtyByItemNoList"), itemNoListString);
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
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var encodedProjectNo = Uri.EscapeDataString(projectNo);
            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"], $"GetListOfOpenPOQtyByItemNoListByProjectNo?ProjectNo={encodedProjectNo}"))
            {
                Content = itemNoListString
            };
            request.Headers.Add("Authorization", token);

            var openPoQtyResponse = await client.SendAsync(request);
            //var openPoQtyResponse = await _httpClient.PostAsync(string.Concat(_config["PurchaseAPI"],
            //                                "GetListOfOpenPOQtyByItemNoListByProjectNo?", "ProjectNo=", projectNo), itemNoListString);
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

        private async Task<List<OpenPoQuantityDto>> GetOpenPoQtyForChildItemsByMultipleProjectNo(List<string> itemNumberList, List<string> projectNoList)
        {
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            var requestData = new 
            {
                itemNumberList = itemNumberList,
                projectNo = projectNoList
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"], $"GetListOfOpenPOQtyByItemNoListByMultipleProjectNo"))
            {
                Content = content
            };
            request.Headers.Add("Authorization", token);

            var openPoQtyResponse = await client.SendAsync(request);
            //var openPoQtyResponse = await _httpClient.PostAsync(string.Concat(_config["PurchaseAPI"],
            //                                "GetListOfOpenPOQtyByItemNoListByProjectNo?", "ProjectNo=", projectNo), itemNoListString);
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

        private async Task<List<BinningQuantityDto>> GetBinningQtyForChildItemsByProjectNo(string itemNumber, string projectNo)
        {
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var encodedProjectNo = Uri.EscapeDataString(projectNo);
            var encodedItemNo = Uri.EscapeDataString(itemNumber);
            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["BinningAPI"], $"GetListOfBinningQtyByItemNoListByProjectNo?ProjectNo={encodedProjectNo}&ItemNumber={encodedItemNo}"));

            request.Headers.Add("Authorization", token);

            var openPoQtyResponse = await client.SendAsync(request);
            var openPoQtyString = await openPoQtyResponse.Content.ReadAsStringAsync();
            dynamic openPoQtyData = JsonConvert.DeserializeObject(openPoQtyString);
            List<BinningQuantityDto> openPoQtyList = new List<BinningQuantityDto>();

            foreach (var item in openPoQtyData.data)
            {
                BinningQuantityDto dto = JsonConvert.DeserializeObject<BinningQuantityDto>(item.ToString());
                openPoQtyList.Add(dto);
            }

            return openPoQtyList;
        }
        private async Task<List<BinningQuantityDto>> GetBinningQtyForChildItemsByMultipleProjectNo(string itemNumber, List<string> projectNoList)
        {
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            var requestData = new
            {
                ItemNumber = itemNumber,
                projectNo = projectNoList
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["BinningAPI"], "GetListOfBinningQtyByItemNoListByMultipleProjectNo"))
            {
                Content = content
            };

            request.Headers.Add("Authorization", token);

            var openPoQtyResponse = await client.SendAsync(request);
            var openPoQtyString = await openPoQtyResponse.Content.ReadAsStringAsync();
            dynamic openPoQtyData = JsonConvert.DeserializeObject(openPoQtyString);
            List<BinningQuantityDto> openPoQtyList = new List<BinningQuantityDto>();

            foreach (var item in openPoQtyData.data)
            {
                BinningQuantityDto dto = JsonConvert.DeserializeObject<BinningQuantityDto>(item.ToString());
                openPoQtyList.Add(dto);
            }

            return openPoQtyList;
        }
        private async Task<List<CoverageReportChildItemReqQtyDataDto>> GetChildItemRequiredQtyFromBom(List<OpenSalesCoverageReport> openFGCoverageDetails)
        {
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var openFGCoverageDetailsJson = JsonConvert.SerializeObject(openFGCoverageDetails);
            var openFGCoverageDetailsString = new StringContent(openFGCoverageDetailsJson, Encoding.UTF8, "application/json");
            //var response = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "GetBomDetailsForCoverageReport"), openFGCoverageDetailsString);
            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["EngineeringBomAPI"],
                           "GetBomDetailsForCoverageReport"))
            {
                Content = openFGCoverageDetailsString
            };
            request.Headers.Add("Authorization", token);

            var response = await client.SendAsync(request);
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
            var client = _clientFactory.CreateClient();
            //client.Timeout = TimeSpan.FromMinutes(10); // Set to 5 minutes, or whatever duration you need

            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var openFGCoverageDetailsJson = JsonConvert.SerializeObject(openFGCoverageDetails);
            var openFGCoverageDetailsString = new StringContent(openFGCoverageDetailsJson, Encoding.UTF8, "application/json");
            //var response = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "GetBomDetailsByProjectNoForCoverageReport"),
            //                                                                                                                openFGCoverageDetailsString);
            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["EngineeringBomAPI"], "GetBomDetailsByProjectNoForCoverageReport"))
            {
                Content = openFGCoverageDetailsString
            };
            request.Headers.Add("Authorization", token);

            var response = await client.SendAsync(request);
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

        private async Task<List<CoverageReportChildItemReqQtyDataByProjectNoDto>> GetChildItemRequiredQtyFromBomByMultipleProjectNo(List<OpenSalesCoverageReportByProjectNumber> openFGCoverageDetails)
        {
            var client = _clientFactory.CreateClient();
            // Increase timeout if needed
            //client.Timeout = TimeSpan.FromMinutes(10);

            var token = HttpContext.Request.Headers["Authorization"].ToString();
            const int batchSize = 200; // Define your batch size
            var childItemReqQtyDtos = new List<CoverageReportChildItemReqQtyDataByProjectNoDto>();

            var totalBatches = (int)Math.Ceiling((double)openFGCoverageDetails.Count / batchSize);

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                // Create a batch of items
                var batch = openFGCoverageDetails.Skip(batchIndex * batchSize).Take(batchSize).ToList();
                var openFGCoverageDetailsJson = JsonConvert.SerializeObject(batch);
                var openFGCoverageDetailsString = new StringContent(openFGCoverageDetailsJson, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_config["EngineeringBomAPI"]}GetBomDetailsByProjectNoForCoverageReport")
                {
                    Content = openFGCoverageDetailsString
                };
                request.Headers.Add("Authorization", token);

                try
                {
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode(); // Throws if the status code is not 2xx

                    var childItemRequiredQtyString = await response.Content.ReadAsStringAsync();
                    dynamic childItemRequiredQtyData = JsonConvert.DeserializeObject(childItemRequiredQtyString);

                    foreach (var item in childItemRequiredQtyData.data)
                    {
                        CoverageReportChildItemReqQtyDataByProjectNoDto dto = JsonConvert.DeserializeObject<CoverageReportChildItemReqQtyDataByProjectNoDto>(item.ToString());
                        childItemReqQtyDtos.Add(dto);
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the exception as needed
                    // You might want to continue processing other batches even if one fails
                }
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
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            //var enggBomDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"],
            //                "GetLatestEnggBomVersionDetailByIemNumber?", "&fgPartNumber=", itemNumber));
            var encodedItemNumber = Uri.EscapeDataString(itemNumber);
            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EngineeringBomAPI"], $"GetLatestEnggBomVersionDetailByIemNumber?fgPartNumber={encodedItemNumber}"));
            request.Headers.Add("Authorization", token);

            var enggBomDetails = await client.SendAsync(request);
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

                    //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                    //          "GetInventoryByItemNo?", "itemNumber=", childItem.ItemNumber));
                    var encodedItemNumber1 = Uri.EscapeDataString(childItem.ItemNumber);
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"], $"GetInventoryByItemNo?itemNumber={encodedItemNumber1}"));
                    request1.Headers.Add("Authorization", token);

                    var inventoryObjectResult = await client.SendAsync(request1);
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

                    //var purchaseObjectResult = await _httpClient.GetAsync(string.Concat(_config["PurchaseAPI"],
                    //          "GetAllOpenPoDetails?", "itemNumber=", childItem.ItemNumber));
                    var encodedItemNumber2 = Uri.EscapeDataString(childItem.ItemNumber);
                    var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PurchaseAPI"], $"GetAllOpenPoDetails?itemNumber={encodedItemNumber2}"));
                    request2.Headers.Add("Authorization", token);

                    var purchaseObjectResult = await client.SendAsync(request2);
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
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            //var enggBomDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"],
            //                "GetEnggChildItemNumberByEnggbom?", "&bomId=", parentId));
            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EngineeringBomAPI"],
                           $"GetEnggChildItemNumberByEnggbom?bomId={parentId}"));
            request.Headers.Add("Authorization", token);

            var enggBomDetails = await client.SendAsync(request);
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
