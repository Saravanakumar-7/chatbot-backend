using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using NuGet.Protocol.Core.Types;
using System.Net;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Repository;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Data;
using System.Data.SqlClient;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Entities.Enums;
using Tips.Warehouse.Api.Services;

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]

    public class ConsumptionReportController : ControllerBase
    {
        private IInvoiceRepository _invoiceRepository;
        private IBTODeliveryOrderRepository _bTODeliveryOrderRepository;
        private IMaterialIssueTrackerRepository _materialIssueTrackerRepository;
        private IConsumptionReportRepository _consumptionReportReposiory;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ICacheService _cacheService;
        public ConsumptionReportController(IConsumptionReportRepository consumptionReportReposiory, IMaterialIssueTrackerRepository materialIssueTrackerRepository, IBTODeliveryOrderRepository bTODeliveryOrderRepository, IHttpClientFactory clientFactory, IInvoiceRepository invoiceRepository, HttpClient httpClient, IConfiguration config, ILoggerManager logger, IMapper mapper, ICacheService cacheService)
        {
            _invoiceRepository = invoiceRepository;
            _bTODeliveryOrderRepository = bTODeliveryOrderRepository;
            _materialIssueTrackerRepository = materialIssueTrackerRepository;
            _consumptionReportReposiory = consumptionReportReposiory;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
            _cacheService = cacheService;
        }


        [HttpGet]
        public async Task<IActionResult> GenerateTGConsumptionReportByDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {

            ServiceResponse<List<ConsumptionSPReport>> serviceResponse = new ServiceResponse<List<ConsumptionSPReport>>();

            try
            {
                List<ConsumptionSPReport> openSalesCoverageReports = await TGConsumptionReport(FromDate, ToDate);

                serviceResponse.Data = openSalesCoverageReports;
                serviceResponse.Message = $"Returned ConceptionReport Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GenerateConsumptionReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GenerateConsumptionReport API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task<List<ConsumptionSPReport>> TGConsumptionReport(DateTime? FromDate, DateTime? ToDate)
        {
            List<ConsumptionSPReport> consumptionReportList = new List<ConsumptionSPReport>();
            try
            {
                // Step 1: Get invoice BTO details
                var InvoiceBTOTGDetails = await _invoiceRepository.GetTGInvoiceBTODetailsByDate(FromDate, ToDate);
                if (!InvoiceBTOTGDetails.Any())
                {
                    _logger.LogInfo("No TG invoice BTO details found for the given date range.");
                    return consumptionReportList;
                }

                // Step 2: Prepare data for parallel API calls
                List<string?> partNumberList = InvoiceBTOTGDetails.Select(item => item.FGItemNumber).Distinct().ToList();
                List<string?> LotNoList = InvoiceBTOTGDetails.Select(item => item.LotNumber).Distinct().ToList();

                // Step 3: Execute parallel API calls for better performance
                var grinTask = GetGrinComsumptionDetailsByPartNo(partNumberList, LotNoList);
                var missingPartNumbers = partNumberList;
                var missingLotNumbers = LotNoList;
                var openGrinTask = GetOpenGrinComsumptionDetailsByPartNoAndLotNo(missingPartNumbers, missingLotNumbers);

                // Wait for both API calls to complete
                await Task.WhenAll(grinTask, openGrinTask);

                var grinConsumpDetials = await grinTask;
                var openGrinConsumpDetials = await openGrinTask;

                // Step 4: Create lookup dictionaries for faster data access
                var grinLookup = grinConsumpDetials.ToLookup(g => $"{g.PartNumber}|{g.LotNumber}");
                var openGrinLookup = openGrinConsumpDetials.ToLookup(og => $"{og.ItemNumber}|{og.LotNumber}");

                // Step 5: Process invoice data efficiently
                var processedItems = new HashSet<string>();

                foreach (var invoice in InvoiceBTOTGDetails)
                {
                    var lookupKey = $"{invoice.FGItemNumber}|{invoice.LotNumber}";

                    // First try to match with GRIN data
                    var matchingGrinItems = grinLookup[lookupKey];

                    if (matchingGrinItems.Any())
                    {
                        processedItems.Add(lookupKey);

                        foreach (var grin in matchingGrinItems)
                        {
                            consumptionReportList.Add(CreateTGReportFromGrin(invoice, grin));
                        }
                    }
                    else
                    {
                        // If not found in GRIN, try OpenGRIN
                        var matchingOpenGrinItems = openGrinLookup[lookupKey];

                        foreach (var openGrin in matchingOpenGrinItems)
                        {
                            consumptionReportList.Add(CreateTGReportFromOpenGrin(invoice, openGrin));
                        }
                    }
                }

                // Step 6: Bulk insert all records
                if (consumptionReportList.Any())
                {
                    await _consumptionReportReposiory.CreateConsumptionReports(consumptionReportList);
                     _consumptionReportReposiory.SaveAsync();
                }

                _logger.LogInfo($"TG Consumption Report generated successfully with {consumptionReportList.Count} records.");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in TGConsumptionReport: {ex.Message}");
                throw;
            }

            return consumptionReportList;
        }

        private ConsumptionSPReport CreateTGReportFromGrin(InvoiceBTODetailsDto invoice, GrinComsumpDto grin)
        {
            return new ConsumptionSPReport
            {
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceQty = invoice.InvoicedQty,
                DoNumber = invoice.DONumber,
                FGItemNumber = invoice.FGItemNumber,
                PPLotNumber = invoice.LotNumber,
                GrinNumber = grin.GrinNumber,
                GrinDate = grin.GrinDate,
                Vendor = grin.VendorName,
                PoNumber = grin.PONumber,
                BOENo = grin.BOENo,
                GrinQty = grin.GrinQty,
                UnitPrice = grin.GrinUnitPrice,
                Tax = grin.Tax,
                OtherCosts = grin.OtherCosts,
                UOM = grin.UOM,
                UOC = grin.UOC
            };
        }

        private ConsumptionSPReport CreateTGReportFromOpenGrin(InvoiceBTODetailsDto invoice, OpenGrinComsumpDto openGrin)
        {
            return new ConsumptionSPReport
            {
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceQty = invoice.InvoicedQty,
                DoNumber = invoice.DONumber,
                FGItemNumber = invoice.FGItemNumber,
                PPLotNumber = invoice.LotNumber,
                GrinNumber = openGrin.OpenGrinNumber,
                GrinDate = openGrin.OpenGrinDate,
                Vendor = openGrin.SenderName,
                PoNumber = null,
                BOENo = null,
                GrinQty = openGrin.OpenGrinQty,
                UnitPrice = null,
                Tax = null,
                OtherCosts = null,
                UOM = openGrin.UOM,
                UOC = null
            };
        }

        [HttpGet]
        public async Task<IActionResult> GenerateConsumptionReport([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {

            ServiceResponse<List<ConsumptionSPReport>> serviceResponse = new ServiceResponse<List<ConsumptionSPReport>>();

            try
            {
                List<ConsumptionSPReport> openSalesCoverageReports = await ConsumptionReport(FromDate, ToDate);

                //serviceResponse.Data = openSalesCoverageReports;
                serviceResponse.Data = null;
                serviceResponse.Message = $"Returned ConceptionReport Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GenerateConsumptionReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GenerateConsumptionReport API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task<List<ConsumptionSPReport>> ConsumptionReport(DateTime? FromDate, DateTime? ToDate)
        {
            List<ConsumptionSPReport> consumptionReportList = new List<ConsumptionSPReport>();
            try
            {
                var invoiceBTODetails = await _invoiceRepository.GetInvoiceBTODetailsByDate(FromDate, ToDate);


                // Step 2: Get distinct lot numbers from invoice BTO details
                List<string?> lotNumberList = invoiceBTODetails
                                                .Select(x => x.LotNumber)
                                                .Where(x => !string.IsNullOrEmpty(x))
                                                .Distinct()
                                                .ToList();

                // Step 3: Get shop order consumption details
                List<ShopOrderComsumpDto> shopOrderConsumpDetails = await GetShopOrderComsumptionDetailsByLotNo(lotNumberList);

                // Step 4: Combine the data using LINQ join
                var invoiceBTOShopOrderList = (from invoiceBTO in invoiceBTODetails
                                               join shopOrder in shopOrderConsumpDetails
                                                   on new { LotNumber = invoiceBTO.LotNumber, ItemNumber = invoiceBTO.FGItemNumber }
                                                   equals new { LotNumber = shopOrder.ShopOrderNumber, ItemNumber = shopOrder.ItemNumber }
                                                   into shopOrderGroup
                                               from shopOrder in shopOrderGroup.DefaultIfEmpty()
                                               select new InvoiceBTOShopOrderDetailsDto
                                               {
                                                   InvoiceNumber = invoiceBTO.InvoiceNumber,
                                                   InvoiceDate = invoiceBTO.InvoiceDate,
                                                   DONumber = invoiceBTO.DONumber,
                                                   FGItemNumber = invoiceBTO.FGItemNumber,
                                                   InvoicedQty = invoiceBTO.InvoicedQty,
                                                   SalesOrderNumber = invoiceBTO.SalesOrderNumber,
                                                   LotNumber = invoiceBTO.LotNumber,
                                                   ReleaseQty = shopOrder?.ReleaseQty ?? 0,
                                                   WipQty = shopOrder?.WipQty ?? 0
                                               }).ToList();

                // Fetch Somit consumption details based on Shop Order numbers
                //Dictionary<string, string?> shopOrderToItemNumberDict = shopOrderConsumpDetials.ToDictionary(x => x.ShopOrderNumber, x => x.ItemNumber);
                List<SomitConsumpWithBOMVersionDto> somitConsumpDetails = await GetSomitConsumpDetailsByShopOrderNumbers(invoiceBTOShopOrderList);


                // Fetch GRIN consumption details based on Part numbers and LotNo
                List<string?> partNumberList = somitConsumpDetails.Select(item => item.PartNumber).Distinct().ToList();
                List<string?> somitLotNoList = somitConsumpDetails.Select(item => item.LotNumber).Distinct().ToList();
                List<GrinComsumpDto> grinConsumpDetials = await GetGrinComsumptionDetailsByPartNo(partNumberList, somitLotNoList);

                // Track which items were found in GRIN
                var foundInGrinItems = new HashSet<string>();

                // Process GRIN data first (prioritize GRIN)
                foreach (var somit in somitConsumpDetails)
                {
                    var matchingGrinItems = grinConsumpDetials.Where(g => g.PartNumber == somit.PartNumber && g.LotNumber == somit.LotNumber).ToList();
                    
                    if (matchingGrinItems.Any())
                    {
                        foundInGrinItems.Add($"{somit.PartNumber}|{somit.LotNumber}");
                        
                        foreach (var grin in matchingGrinItems)
                        {
                            var reportDto = new ConsumptionSPReport
                            {
                                InvoiceNumber = somit.InvoiceNumber,
                                InvoiceDate = somit.InvoiceDate,
                                InvoiceQty = somit.InvoicedQty,
                                DoNumber = somit.BTONumber,
                                FGItemNumber = somit.FGItemNumber,
                                WorkOrderNumber = somit.ShopOrderNumber,
                                WorkOrderQty = somit.ShopOrderReleaseQty,
                                WorkOrderWipQty = somit.ShopOrderWipQty,
                                WorkOrderConvertedToFGQty = somit.ConvertedToFgQty,
                                CusumedQty = somit.ConsumedQtyByInvoice,
                                PartNumber = somit.PartNumber,
                                MftrPartnumber = somit.MftrPartNumber,
                                PPLotNumber = somit.LotNumber,
                                PPWipQty = somit.PPWipQty,
                                MaterialissueDate = somit.SomitDate,
                                TransactionFrom = somit.DataFrom,
                                GrinNumber = grin.GrinNumber,
                                GrinDate = grin.GrinDate,
                                Vendor = grin.VendorName,
                                PoNumber = grin.PONumber,
                                BOENo = grin.BOENo,
                                GrinQty = grin.GrinQty,
                                UnitPrice = grin.GrinUnitPrice,
                                Tax = grin.Tax,
                                OtherCosts = grin.OtherCosts,
                                UOM = grin.UOM,
                                UOC = grin.UOC
                            };

                            consumptionReportList.Add(reportDto);
                        }
                    }
                }

                // Get missing items not found in GRIN
                var missingItems = somitConsumpDetails
                    .Where(somit => !foundInGrinItems.Contains($"{somit.PartNumber}|{somit.LotNumber}"))
                    .ToList();

                if (missingItems.Any())
                {
                    // Fetch missing items from OpenGRIN
                    List<string?> missingPartNumbers = missingItems.Select(item => item.PartNumber).Distinct().ToList();
                    List<string?> missingLotNumbers = missingItems.Select(item => item.LotNumber).Distinct().ToList();
                    List<OpenGrinComsumpDto> openGrinConsumpDetails = await GetOpenGrinComsumptionDetailsByPartNoAndLotNo(missingPartNumbers, missingLotNumbers);

                    // Process OpenGRIN data for missing items
                    foreach (var somit in missingItems)
                    {
                        var matchingOpenGrinItems = openGrinConsumpDetails.Where(og => og.ItemNumber == somit.PartNumber && og.LotNumber == somit.LotNumber).ToList();
                        
                        foreach (var openGrin in matchingOpenGrinItems)
                        {
                            var reportDto = new ConsumptionSPReport
                            {
                                InvoiceNumber = somit.InvoiceNumber,
                                InvoiceDate = somit.InvoiceDate,
                                InvoiceQty = somit.InvoicedQty,
                                DoNumber = somit.BTONumber,
                                FGItemNumber = somit.FGItemNumber,
                                WorkOrderNumber = somit.ShopOrderNumber,
                                WorkOrderQty = somit.ShopOrderReleaseQty,
                                WorkOrderWipQty = somit.ShopOrderWipQty,
                                WorkOrderConvertedToFGQty = somit.ConvertedToFgQty,
                                CusumedQty = somit.ConsumedQtyByInvoice,
                                PartNumber = somit.PartNumber,
                                MftrPartnumber = somit.MftrPartNumber,
                                PPLotNumber = somit.LotNumber,
                                PPWipQty = somit.PPWipQty,
                                MaterialissueDate = somit.SomitDate,
                                TransactionFrom = somit.DataFrom,
                                // Map OpenGRIN fields to GRIN structure, set null where fields don't exist
                                GrinNumber = openGrin.OpenGrinNumber,
                                GrinDate = openGrin.OpenGrinDate,
                                Vendor = openGrin.SenderName, // Map SenderName to Vendor
                                PoNumber = null, // Not available in OpenGRIN
                                BOENo = null, // Not available in OpenGRIN
                                GrinQty = openGrin.OpenGrinQty,
                                UnitPrice = null, // Not available in OpenGRIN
                                Tax = null, // Not available in OpenGRIN
                                OtherCosts = null, // Not available in OpenGRIN
                                UOM = openGrin.UOM,
                                UOC = null // Not available in OpenGRIN
                            };

                            consumptionReportList.Add(reportDto);
                        }
                    }
                }
                await _consumptionReportReposiory.CreateConsumptionReports(consumptionReportList);
                _consumptionReportReposiory.SaveAsync();
                _logger.LogInfo($"Consumption Report generated successfully with {consumptionReportList.Count} records.");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ConsumptionReport: {ex.Message}");
                throw;
            }

            //return consumptionReportList;
            return null;
        }



        private async Task<List<ShopOrderComsumpDto>> GetShopOrderComsumptionDetailsByLotNo(List<string> lotNumberList)
        {
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            var lotNoListJson = JsonConvert.SerializeObject(lotNumberList);
            var lotNoListString = new StringContent(lotNoListJson, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ProductionAPI"],
                                $"GetShopOrderComsumptionDetialsBySaleOrderNos"))
            {
                Content = lotNoListString
            };

            request.Headers.Add("Authorization", token);
            var shopOrderResponse = await client.SendAsync(request);
            var shopOrderString = await shopOrderResponse.Content.ReadAsStringAsync();
            dynamic shopOrderData = JsonConvert.DeserializeObject(shopOrderString);

            List<ShopOrderComsumpDto> shopOrderConsumpList = new List<ShopOrderComsumpDto>();

            foreach (var item in shopOrderData.data)
            {
                ShopOrderComsumpDto dto = JsonConvert.DeserializeObject<ShopOrderComsumpDto>(item.ToString());
                shopOrderConsumpList.Add(dto);
            }

            return shopOrderConsumpList;
        }


        private async Task<List<SomitConsumpWithBOMVersionDto>> GetSomitConsumpDetailsByShopOrderNumbers(List<InvoiceBTOShopOrderDetailsDto> invoiceBTOShopOrderDetailsDto)
        {
            if (invoiceBTOShopOrderDetailsDto == null || !invoiceBTOShopOrderDetailsDto.Any())
            {
                return new List<SomitConsumpWithBOMVersionDto>();
            }

            var SomitConsumpDtoList = new List<SomitConsumpWithBOMVersionDto>();

            try
            {

            // Group by FG Item Number to batch process BOM calls
            var groupedByFGItem = invoiceBTOShopOrderDetailsDto
                .GroupBy(x => x.FGItemNumber)
                .ToList();

            // Step 1: Get all SOMIT details in parallel batches
            var somitTasks = invoiceBTOShopOrderDetailsDto
                .Select(async item => new
                {
                    Item = item,
                    SomitDetails = await _materialIssueTrackerRepository
                        .GetSomitConsumpDetailsByShopOrderNumbers(item.LotNumber, item.FGItemNumber)
                })
                .ToList();

            var somitResults = await Task.WhenAll(somitTasks);

            // Step 2: Collect all unique BOM versions for batch API calls
            var bomVersionLookup = somitResults
                .Where(result => result.SomitDetails.Any() && result.SomitDetails.Select(x => x.Bomversion).FirstOrDefault() > 0)
                .GroupBy(result => result.Item.FGItemNumber)
                .ToDictionary(
                    g => g.Key,
                    g => g.First().SomitDetails.Select(x => x.Bomversion).FirstOrDefault()
                );

            // Step 3: Batch fetch BOM details in parallel
            var bomTasks = bomVersionLookup
                .Select(async kvp => new
                {
                    FGItemNumber = kvp.Key,
                    BomVersion = kvp.Value,
                    BomDetails = await GetEnggBomComsumpDetailsByFgItemNoAndBOMVersion(kvp.Key, kvp.Value)
                })
                .ToList();

            var bomResults = await Task.WhenAll(bomTasks);
            var bomLookup = bomResults
                .GroupBy(x => x.FGItemNumber)
                .ToDictionary(g => g.Key, g => g.First().BomDetails);

            // Step 4: Process all somit details efficiently
            foreach (var somitResult in somitResults)
            {
                var invoiceItem = somitResult.Item;
                var somitDetails = somitResult.SomitDetails;

                if (!bomLookup.TryGetValue(invoiceItem.FGItemNumber, out var bomDetails))
                    continue;

                await ProcessSomitDetailsForConsumption(
                    SomitConsumpDtoList,
                    somitDetails,
                    bomDetails,
                    invoiceItem.LotNumber,
                    invoiceItem.FGItemNumber,
                    invoiceItem.InvoiceNumber,
                    invoiceItem.InvoicedQty,
                    invoiceItem.InvoiceDate,
                    invoiceItem.DONumber
                );
            }

            // Step 5: Group and aggregate results
            var itemsRequiredQtyGrouped = SomitConsumpDtoList
                .Where(item => item.PartNumber != null)
                .GroupBy(item => item.PartNumber)
                .Select(group => new SomitConsumpWithBOMVersionDto
                {
                    FGItemNumber = group.FirstOrDefault()?.FGItemNumber,
                    PartNumber = group.Key,
                    MftrPartNumber = group.FirstOrDefault()?.MftrPartNumber,
                    LotNumber = group.FirstOrDefault()?.LotNumber,
                    SomitDate = group.FirstOrDefault()?.SomitDate,
                    ShopOrderNumber = group.FirstOrDefault()?.ShopOrderNumber,
                    PartType = group.FirstOrDefault()?.PartType,
                    DataFrom = group.FirstOrDefault()?.DataFrom,

                    // Aggregated fields
                    ShopOrderReleaseQty = group.Sum(item => item.ShopOrderReleaseQty),
                    ShopOrderWipQty = group.Sum(item => item.ShopOrderWipQty),
                    ConvertedToFgQty = group.Sum(item => item.ConvertedToFgQty),
                    IssuedQty = group.Sum(item => item.IssuedQty),
                    InvoicedQty = group.Sum(item => item.InvoicedQty),
                    BomQty = group.Sum(item => item.BomQty),
                    ConsumedQtyByInvoice = group.Sum(item => item.ConsumedQtyByInvoice),
                    PPWipQty = group.Sum(item => item.PPWipQty),

                    // Non-aggregated fields (take first occurrence)
                    InvoiceNumber = group.FirstOrDefault()?.InvoiceNumber,
                    InvoiceDate = group.FirstOrDefault()?.InvoiceDate,
                    BTONumber = group.FirstOrDefault()?.BTONumber,
                    Bomversion = group.FirstOrDefault()?.Bomversion ?? 0
                })
                .ToList();

            return itemsRequiredQtyGrouped;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetSomitConsumpDetailsByShopOrderNumbers: {ex.Message}");

                // Log details about the problematic data for debugging
                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    _logger.LogError($"Duplicate key error. Invoice items count: {invoiceBTOShopOrderDetailsDto.Count}");
                    var duplicateFGItems = invoiceBTOShopOrderDetailsDto
                        .GroupBy(x => x.FGItemNumber)
                        .Where(g => g.Count() > 1)
                        .Select(g => new { FGItemNumber = g.Key, Count = g.Count() });

                    foreach (var dup in duplicateFGItems)
                    {
                        _logger.LogError($"Duplicate FG Item: {dup.FGItemNumber}, Count: {dup.Count}");
                    }
                }

                throw;
            }
        }

        private async Task ProcessSomitDetailsForConsumption(
            List<SomitConsumpWithBOMVersionDto> somitConsumpDtoList,
            List<SomitConsumpWithBOMVersionDto> somitDetails,
            List<EnggChildBomComsumpDetailsDto> bomDetails,
            string shopOrderNo,
            string fgItemNumber,
            string invoiceNumber,
            decimal invoicedQty,
            DateTime? invoiceDate,
            string btoNumber)
        {
            // Create BOM lookup for faster access - handle duplicates by taking first occurrence
            var bomLookup = bomDetails
                .GroupBy(x => x.ItemNumber)
                .ToDictionary(g => g.Key, g => g.First().Quantity);

            foreach (var somitDetail in somitDetails)
            {
                if (somitDetail.PartType == PartType.PurchasePart)
                {
                    var bomQty = bomLookup.GetValueOrDefault(somitDetail.PartNumber, 0);
                    var consumedInvoiceQty = bomQty * invoicedQty;

                    var somitConsump = new SomitConsumpWithBOMVersionDto
                    {
                        FGItemNumber = fgItemNumber,
                        PartNumber = somitDetail.PartNumber,
                        MftrPartNumber = somitDetail.MftrPartNumber,
                        LotNumber = somitDetail.LotNumber,
                        SomitDate = somitDetail.SomitDate,
                        ShopOrderNumber = somitDetail.ShopOrderNumber,
                        ShopOrderReleaseQty = somitDetail.ShopOrderReleaseQty,
                        ShopOrderWipQty = somitDetail.ShopOrderWipQty,
                        PartType = somitDetail.PartType,
                        DataFrom = somitDetail.DataFrom,
                        ConvertedToFgQty = somitDetail.ConvertedToFgQty,
                        IssuedQty = somitDetail.IssuedQty,
                        InvoiceNumber = invoiceNumber,
                        InvoicedQty = invoicedQty,
                        InvoiceDate = invoiceDate,
                        BTONumber = btoNumber,
                        Bomversion = somitDetail.Bomversion,
                        BomQty = bomQty,
                        ConsumedQtyByInvoice = consumedInvoiceQty,
                        PPWipQty = somitDetail.IssuedQty - somitDetail.ConvertedToFgQty
                    };

                    somitConsumpDtoList.Add(somitConsump);
                }
                // Note: Sub-assembly processing (recursive logic) would go here if needed
                // For now, focusing on purchase parts as per the original logic
            }
        }


        private async Task SomitConsumpDetailsForComsumptionReportByShopOrderNo(List<SomitConsumpWithBOMVersionDto> SomitConsumpDtoList, string shopOrderNo, string fgItemNumber, string invoiceNumber, decimal invoicedQty, DateTime? invoiceDate, string btoNumber)
        {
            try
            {
                Dictionary<string, decimal> saItem = new Dictionary<string, decimal>();

                var somitDetails = await _materialIssueTrackerRepository
                      .GetSomitConsumpDetailsByShopOrderNumbers(shopOrderNo, fgItemNumber);

                var BomVersion = somitDetails.Select(x => x.Bomversion).FirstOrDefault();

                List<EnggChildBomComsumpDetailsDto> enggChildBomComsumpDetailsDtos = await GetEnggBomComsumpDetailsByFgItemNoAndBOMVersion(fgItemNumber, BomVersion);

                if (somitDetails != null && somitDetails.Count() > 0)
                {
                    foreach (var somitDetail in somitDetails)
                    {
                        var bomQty = enggChildBomComsumpDetailsDtos.Where(x => x.ItemNumber == somitDetail.PartNumber).Select(x => x.Quantity).FirstOrDefault();
                        var consumedInvoiceQty = bomQty * invoicedQty;

                        if (somitDetail.PartType == PartType.PurchasePart)
                        {

                            SomitConsumpWithBOMVersionDto SomitConsump = new SomitConsumpWithBOMVersionDto
                            {
                                FGItemNumber = fgItemNumber,
                                PartNumber = somitDetail.PartNumber,
                                MftrPartNumber = somitDetail.MftrPartNumber,
                                LotNumber = somitDetail.LotNumber,
                                SomitDate = somitDetail.SomitDate,
                                ShopOrderNumber = somitDetail.ShopOrderNumber,
                                ShopOrderReleaseQty = somitDetail.ShopOrderReleaseQty,
                                ShopOrderWipQty = somitDetail.ShopOrderWipQty,
                                PartType = somitDetail.PartType,
                                DataFrom = somitDetail.DataFrom,
                                ConvertedToFgQty = somitDetail.ConvertedToFgQty,
                                IssuedQty = somitDetail.IssuedQty,
                                InvoiceNumber = invoiceNumber,
                                InvoicedQty = invoicedQty,
                                InvoiceDate = invoiceDate,
                                BTONumber = btoNumber,
                                Bomversion = BomVersion,
                                BomQty = bomQty,
                                ConsumedQtyByInvoice = consumedInvoiceQty,
                                PPWipQty = somitDetail.IssuedQty - somitDetail.ConvertedToFgQty
                            };

                            SomitConsumpDtoList.Add(SomitConsump);
                        }
                        else
                        {
                            decimal convertedFgStock = 0;
                            string saItemNumber = somitDetail.PartNumber;
                            string saShopOrderNumber = somitDetail.LotNumber;
                            if (saItem.ContainsKey(saItemNumber))
                            {
                                convertedFgStock = saItem[saItemNumber];
                            }

                            //var saShopOrderNo = await GetShopOrderComsumptionDetailsBySaItemNo(saItemNumber, fgItemNumber);

                            await SomitConsumpDetailsForComsumptionReportByShopOrderNo(SomitConsumpDtoList, saShopOrderNumber, fgItemNumber, invoiceNumber, consumedInvoiceQty, invoiceDate, btoNumber);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task<List<EnggChildBomComsumpDetailsDto>> GetEnggBomComsumpDetailsByFgItemNoAndBOMVersion(string fgItemNumber, decimal bomVersion)
        {
            var cacheKey = $"BOM_{fgItemNumber}_{bomVersion}";

            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EngineeringBomAPI"],
                                    $"GetEnggChildBomQtyDetailsByFgItemNoAndRevNo?itemNumber={fgItemNumber}&revisionNumber={bomVersion}"));

                request.Headers.Add("Authorization", token);
                var enggBomChildResponse = await client.SendAsync(request);
                var enggBomChildString = await enggBomChildResponse.Content.ReadAsStringAsync();
                dynamic enggBomChildData = JsonConvert.DeserializeObject(enggBomChildString);

                List<EnggChildBomComsumpDetailsDto> enggChildBomComsumpDetailsDto = new List<EnggChildBomComsumpDetailsDto>();

                if (enggBomChildData?.data != null)
                {
                    foreach (var item in enggBomChildData.data)
                    {
                        EnggChildBomComsumpDetailsDto dto = JsonConvert.DeserializeObject<EnggChildBomComsumpDetailsDto>(item.ToString());
                        enggChildBomComsumpDetailsDto.Add(dto);
                    }
                }

                return enggChildBomComsumpDetailsDto;
            }, TimeSpan.FromHours(24)); // BOM data rarely changes, cache for 24 hours
        }

        private async Task<string> GetShopOrderComsumptionDetailsBySaItemNo(string saItemNumber, string fgItemNumber)
        {
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ProductionAPI"],
                                $"GetShopOrderComsumptionDetialsBySaItemNo?saItemNumber={saItemNumber}&fgItemNumber={fgItemNumber}"));

            request.Headers.Add("Authorization", token);
            var shopOrderResponse = await client.SendAsync(request);
            var shopOrderString = await shopOrderResponse.Content.ReadAsStringAsync();
            dynamic shopOrderData = JsonConvert.DeserializeObject(shopOrderString);


            var shopOrderComsumpDto = JsonConvert.DeserializeObject<string>(shopOrderData.data.ToString());


            return shopOrderComsumpDto;
        }

        private async Task<List<GrinComsumpDto>> GetGrinComsumptionDetailsByPartNo(List<string> partNoListString, List<string> lotNoListString)
        {
            // Create cache key based on sorted part and lot numbers for consistent caching
            var sortedPartNos = partNoListString.Where(x => !string.IsNullOrEmpty(x)).OrderBy(x => x).ToList();
            var sortedLotNos = lotNoListString.Where(x => !string.IsNullOrEmpty(x)).OrderBy(x => x).ToList();
            var cacheKey = $"GRIN_{string.Join(",", sortedPartNos)}_{string.Join(",", sortedLotNos)}";

            return await _cacheService.GetOrSetAsync(cacheKey, async () =>
            {
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();

                var payload = new
                {
                    PartNumber = partNoListString,
                    LotNumber = lotNoListString
                };

                var jsonString = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["GrinAPI"],
                                    $"GetGrinComsumptionDetialsByPartNos"))
                {
                    Content = content
                };

                request.Headers.Add("Authorization", token);
                var grinResponse = await client.SendAsync(request);

                var grinString = await grinResponse.Content.ReadAsStringAsync();
                dynamic grinData = JsonConvert.DeserializeObject(grinString);

                List<GrinComsumpDto> grinConsumpList = new List<GrinComsumpDto>();

                if (grinData?.data != null)
                {
                    foreach (var item in grinData.data)
                    {
                        GrinComsumpDto dto = JsonConvert.DeserializeObject<GrinComsumpDto>(item.ToString());
                        grinConsumpList.Add(dto);
                    }
                }

                return grinConsumpList;
            }, TimeSpan.FromMinutes(15)); // GRIN data changes more frequently, cache for 15 minutes
        }

        private async Task<List<OpenGrinComsumpDto>> GetOpenGrinComsumptionDetailsByPartNoAndLotNo(List<string> partNoListString, List<string> lotNoListString)
        {
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            var payload = new
            {
                PartNumber = partNoListString,
                LotNumber = lotNoListString
            };

            var jsonString = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["GrinAPI"],
                                $"GetOpenGrinComsumptionDetailsByPartNoAndLotNo"))
            {
                Content = content
            };

            request.Headers.Add("Authorization", token);
            var openGrinResponse = await client.SendAsync(request);

            var openGrinString = await openGrinResponse.Content.ReadAsStringAsync();
            dynamic openGrinData = JsonConvert.DeserializeObject(openGrinString);

            List<OpenGrinComsumpDto> openGrinConsumpList = new List<OpenGrinComsumpDto>();

            if (openGrinData?.data != null)
            {
                foreach (var item in openGrinData.data)
                {
                    OpenGrinComsumpDto dto = JsonConvert.DeserializeObject<OpenGrinComsumpDto>(item.ToString());
                    openGrinConsumpList.Add(dto);
                }
            }

            return openGrinConsumpList;
        }


    }

}
