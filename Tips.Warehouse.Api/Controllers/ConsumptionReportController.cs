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
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public ConsumptionReportController(IMaterialIssueTrackerRepository materialIssueTrackerRepository, IBTODeliveryOrderRepository bTODeliveryOrderRepository, IHttpClientFactory clientFactory, IInvoiceRepository invoiceRepository, HttpClient httpClient, IConfiguration config, ILoggerManager logger, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _bTODeliveryOrderRepository = bTODeliveryOrderRepository;
            _materialIssueTrackerRepository = materialIssueTrackerRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GenerateConsumptionReport([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {

            ServiceResponse<List<ConsumptionSPReportDto>> serviceResponse = new ServiceResponse<List<ConsumptionSPReportDto>>();

            try
            {
                List<ConsumptionSPReportDto> openSalesCoverageReports = await ConsumptionReport(FromDate, ToDate);

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

        private async Task<List<ConsumptionSPReportDto>> ConsumptionReport(DateTime? FromDate, DateTime? ToDate)
        {
            List<ConsumptionSPReportDto> consumptionReportList = new List<ConsumptionSPReportDto>();
            try
            {
                // Fetch invoice data
                var invoiceConsumpDetails = await _invoiceRepository.GetInvoiceDetialsbyDate(FromDate, ToDate);


                // Fetch DO details based on DO number
                List<string?> doNumberList = invoiceConsumpDetails.Select(item => item.DONumber).Distinct().ToList();
                var doConsumpDetails = await _bTODeliveryOrderRepository.GetDoConsumpDetailsByBTONumberList(doNumberList);


                // Fetch shop order details based on Lot number
                List<string?> lotNumberList = doConsumpDetails.Select(x => x.LotNumber).Distinct().ToList();
                List<ShopOrderComsumpDto> shopOrderConsumpDetials = await GetShopOrderComsumptionDetailsByLotNo(lotNumberList);


                // Fetch Somit consumption details based on Shop Order numbers
                Dictionary<string, string?> shopOrderToItemNumberDict = shopOrderConsumpDetials.ToDictionary(x => x.ShopOrderNumber, x => x.ItemNumber);
                List<SomitConsumpDto> somitConsumpDetails = await GetSomitConsumpDetailsByShopOrderNumbers(shopOrderToItemNumberDict);


                // Fetch Grin consumption details based on Part numbers and LotNo
                List<string?> partNumberList = somitConsumpDetails.Select(item => item.PartNumber).Distinct().ToList();
                List<string?> somitLotNoList = somitConsumpDetails.Select(item => item.LotNumber).Distinct().ToList();
                List<GrinComsumpDto> grinConsumpDetials = await GetGrinComsumptionDetailsByPartNo(partNumberList, somitLotNoList);

                // Combine data and map to the desired output format
                foreach (var invoice in invoiceConsumpDetails)
                {
                    foreach (var doDetail in doConsumpDetails.Where(d => d.BTONumber == invoice.DONumber))
                    {
                        foreach (var shopOrder in shopOrderConsumpDetials.Where(s => s.ShopOrderNumber == doDetail.LotNumber))
                        {
                            foreach (var somit in somitConsumpDetails.Where(s => s.ShopOrderNumber == shopOrder.ShopOrderNumber))
                            {
                                foreach (var grin in grinConsumpDetials.Where(g => g.PartNumber == somit.PartNumber && g.LotNumber == somit.LotNumber))
                                {
                                    var reportDto = new ConsumptionSPReportDto
                                    {
                                        InvoiceNumber = invoice.InvoiceNumber,
                                        InvoiceDate = invoice.InvoiceDate,
                                        DoNumber = invoice.DONumber,
                                        FGItemNumber = invoice.FGItemNumber,
                                        DoLotNumber = doDetail.LotNumber,
                                        WorkOrderNumber = shopOrder.ShopOrderNumber,
                                        WorkOrderWipQty = shopOrder.WipQty,
                                        WorkOrderQty = shopOrder.ReleaseQty,
                                        InvoiceQty = invoice.InvoicedQty,
                                        PartNumber = somit.PartNumber,
                                        CusumedQty = somit.ConvertedToFgQty,
                                        TransactionFrom = somit.DataFrom,
                                        MftrPartnumber = somit.MftrPartNumber,
                                        PPLotNumber =somit.LotNumber ,
                                        MaterialissueDate = somit.CreatedOn,
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
                    }
                }



            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ConceptionReport: {ex.Message}");
            }
            return consumptionReportList;
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


        private async Task<List<SomitConsumpDto>> GetSomitConsumpDetailsByShopOrderNumbers(Dictionary<string, string?> shopOrderToItemNumberDict)
        {
            List<SomitConsumpDto> SomitConsumpDtoList = new List<SomitConsumpDto>();

            if (shopOrderToItemNumberDict != null && shopOrderToItemNumberDict.Count > 0)
            {
                foreach (var dic in shopOrderToItemNumberDict)
                {
                    string shopOrderNo = dic.Key;
                    string? fgItemNumber = dic.Value;

                    await SomitConsumpDetailsForComsumptionReportByShopOrderNo(SomitConsumpDtoList, shopOrderNo, fgItemNumber);
                }
            }

            var itemsRequiredQtyGrouped = SomitConsumpDtoList
                             .GroupBy(item => item.PartNumber) 
                             .Select(group => new SomitConsumpDto
                             {
                                 PartNumber = group.Key, 
                                 MftrPartNumber = group.FirstOrDefault().MftrPartNumber, 
                                 ShopOrderNumber = group.FirstOrDefault().ShopOrderNumber,
                                 LotNumber = group.FirstOrDefault().LotNumber,
                                 CreatedOn = group.FirstOrDefault().CreatedOn,
                                 PartType = group.FirstOrDefault().PartType,  
                                 DataFrom = group.FirstOrDefault().DataFrom,  
                                 ConvertedToFgQty = group.Sum(item => item.ConvertedToFgQty),  
                             })
                             .ToList();


            return itemsRequiredQtyGrouped;
        }


        private async Task SomitConsumpDetailsForComsumptionReportByShopOrderNo(List<SomitConsumpDto> SomitConsumpDtoList, string shopOrderNo, string fgItemNumber)
        {
            try
            {
                Dictionary<string, decimal> saItem = new Dictionary<string, decimal>();

                var somitDetails = await _materialIssueTrackerRepository
                      .GetSomitConsumpDetailsByShopOrderNumbers(shopOrderNo);

                if (somitDetails != null && somitDetails.Count() > 0)
                {
                    foreach (var somitDetail in somitDetails)
                    {
                        List<string> itemDetails = new List<string>();
                        if (somitDetail.PartType == PartType.PurchasePart)
                        {

                            SomitConsumpDto SomitConsump = new SomitConsumpDto
                            {
                                PartNumber = somitDetail.PartNumber,
                                MftrPartNumber = somitDetail.MftrPartNumber,
                                LotNumber = somitDetail.LotNumber,
                                CreatedOn = somitDetail.CreatedOn,
                                ShopOrderNumber = somitDetail.ShopOrderNumber,
                                PartType = somitDetail.PartType,
                                DataFrom = somitDetail.DataFrom,
                                ConvertedToFgQty = somitDetail.ConvertedToFgQty
                            };

                            SomitConsumpDtoList.Add(SomitConsump);
                        }
                        else
                        {
                            decimal convertedFgStock = 0;
                            string saItemNumber = somitDetail.PartNumber;
                            if (saItem.ContainsKey(saItemNumber))
                            {
                                convertedFgStock = saItem[saItemNumber];
                            }

                            var saShopOrderNo = await GetShopOrderComsumptionDetailsBySaItemNo(saItemNumber, fgItemNumber);

                            await SomitConsumpDetailsForComsumptionReportByShopOrderNo(SomitConsumpDtoList, saShopOrderNo, fgItemNumber);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
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
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            var payload = new
            {
                PartNoListString = partNoListString,
                LotNoListString = lotNoListString
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

            foreach (var item in grinData.data)
            {
                GrinComsumpDto dto = JsonConvert.DeserializeObject<GrinComsumpDto>(item.ToString());
                grinConsumpList.Add(dto);
            }

            return grinConsumpList;
        }

    }

}
