using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using MimeKit;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NuGet.Common;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities.Enum;
using Tips.SalesService.Api.Repository;

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SalesOrderController : ControllerBase
    {
        private ISalesOrderRepository _repository;
        private ISalesOrderItemsRepository _salesOrderItemsRepository;
        private ISalesAdditionalChargesRepository _salesAdditionalChargesRepository;
        private ISoConfirmationDateRepository _soConfirmationDateRepository;
        private ISoConfirmationDateHistoryRepository _soConfirmationDateHistoryRepository;
        private IScheduleDateHistoryRepository _scheduleDateHistoryRepository;
        private ISalesOrderAdditionalChargesHistoryRepository _salesOrderAdditionalChargesHistoryRepository;
        private ISalesOrderEmailsDetailsRepository _salesOrderEmailsDetailsRepository;
        private ISalesOrderMainLevelHistoryRepository _salesOrderMainLevelHistoryRepository;
        private ISalesOrderItemLevelHistoryRepository _salesOrderItemLevelHistoryRepository;
        private ISOAdditionalChargesHistoryRepository _sOAdditionalChargesHistoryRepository;
        private IQuoteRepository _quoteRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private ISalesOrderHistoryRepository _salesOrderHistory;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        private readonly IHttpClientFactory _clientFactory;
        public SalesOrderController(ISOAdditionalChargesHistoryRepository sOAdditionalChargesHistoryRepository, ISalesOrderItemLevelHistoryRepository salesOrderItemLevelHistoryRepository, ISalesOrderMainLevelHistoryRepository salesOrderMainLevelHistoryRepository, ISalesOrderEmailsDetailsRepository salesOrderEmailsDetailsRepository, ISalesOrderAdditionalChargesHistoryRepository salesOrderAdditionalChargesHistoryRepository, IHttpClientFactory clientFactory, IScheduleDateHistoryRepository scheduleDateHistoryRepository, ISoConfirmationDateHistoryRepository soConfirmationDateHistoryRepository, ISoConfirmationDateRepository soConfirmationDateRepository, IConfiguration config, HttpClient httpClient, ISalesAdditionalChargesRepository salesAdditionalChargesRepository,
            ISalesOrderRepository repository, ISalesOrderHistoryRepository salesOrderHistoryRepository, IQuoteRepository quoteRepository, IHttpContextAccessor httpContextAccessor,
            ISalesOrderItemsRepository salesOrderItemsRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _clientFactory = clientFactory;
            _salesOrderItemsRepository = salesOrderItemsRepository;
            _salesOrderHistory = salesOrderHistoryRepository;
            _salesAdditionalChargesRepository = salesAdditionalChargesRepository;
            _soConfirmationDateRepository = soConfirmationDateRepository;
            _soConfirmationDateHistoryRepository = soConfirmationDateHistoryRepository;
            _scheduleDateHistoryRepository = scheduleDateHistoryRepository;
            _salesOrderAdditionalChargesHistoryRepository = salesOrderAdditionalChargesHistoryRepository;
            _salesOrderEmailsDetailsRepository = salesOrderEmailsDetailsRepository;
            _salesOrderMainLevelHistoryRepository = salesOrderMainLevelHistoryRepository;
            _salesOrderItemLevelHistoryRepository = salesOrderItemLevelHistoryRepository;
            _sOAdditionalChargesHistoryRepository = sOAdditionalChargesHistoryRepository;
            _quoteRepository = quoteRepository;
            _httpClient = httpClient;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        // GET: api/<SalesOrderController>
        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrder([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<SalesOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderDto>>();
            try
            {
                var getAllSalesOrder = await _repository.GetAllSalesOrder(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllSalesOrder.TotalCount,
                    getAllSalesOrder.PageSize,
                    getAllSalesOrder.CurrentPage,
                    getAllSalesOrder.HasNext,
                    getAllSalesOrder.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all SalesOrders");
                var result = _mapper.Map<IEnumerable<SalesOrderDto>>(getAllSalesOrder);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogInfo($"Returned owner with id: {ex.Message}{ex.InnerException}");

                serviceResponse.Data = null;
                serviceResponse.Message = ($"Returned owner with id: {ex.Message}{ex.InnerException}");
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //get all without Forecast sales order details
        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrderRfq([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<SalesOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderDto>>();
            try
            {
                var getAllSalesOrder = await _repository.GetAllSalesOrderRfq(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllSalesOrder.TotalCount,
                    getAllSalesOrder.PageSize,
                    getAllSalesOrder.CurrentPage,
                    getAllSalesOrder.HasNext,
                    getAllSalesOrder.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all SalesOrders");
                var result = _mapper.Map<IEnumerable<SalesOrderDto>>(getAllSalesOrder);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogInfo($"Returned owner with id: {ex.Message}{ex.InnerException}");

                serviceResponse.Data = null;
                serviceResponse.Message = ($"Returned owner with id: {ex.Message}{ex.InnerException}");
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        //get all forecast sales order details

        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrderForecast([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<SalesOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderDto>>();
            try
            {
                var getAllSalesOrder = await _repository.GetAllSalesOrderForecast(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllSalesOrder.TotalCount,
                    getAllSalesOrder.PageSize,
                    getAllSalesOrder.CurrentPage,
                    getAllSalesOrder.HasNext,
                    getAllSalesOrder.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all SalesOrders");
                var result = _mapper.Map<IEnumerable<SalesOrderDto>>(getAllSalesOrder);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogInfo($"Returned owner with id: {ex.Message}{ex.InnerException}");

                serviceResponse.Data = null;
                serviceResponse.Message = ($"Returned owner with id: {ex.Message}{ex.InnerException}");
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSalesOrderSPReport([FromQuery] PagingParameter pagingParameter)
        {
            //var result = await _repository.GetSalesOrderSPResport(pagingParameter);

            ServiceResponse<IEnumerable<SalesOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderSPReport>>();

            try
            {
                var products = await _repository.GetSalesOrderSPReport(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all SalesOrderSPResport");
                var result = _mapper.Map<IEnumerable<SalesOrderSPReport>>(products);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrderSPResport Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<PurchaseOrderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderById(int id)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();
            try
            {
                var salesOrderById = await _repository.GetSalesOrderById(id);
                string serverKey = GetServerKey();
                if (salesOrderById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    SalesOrderDto salesOrderDto = _mapper.Map<SalesOrderDto>(salesOrderById);

                    var quoteDetails = await _quoteRepository.GetQuoteByQuoteNumber(salesOrderDto.QuoteNumber);
                    if (quoteDetails != null)
                    {
                        salesOrderDto.QuoteRef = quoteDetails.QuoteRef;
                    }

                    List<SalesOrderItemsDto> salesOrderItemsDtoList = new List<SalesOrderItemsDto>();
                    var salesAdditionalChargesList = salesOrderDto.SalesOrderAdditionalCharges;

                    //var salesAdditionalChargesList = _mapper.Map<List<SalesOrderAdditionalChargesDto>>(salesAdditionalChargesDto);

                    string salesOrderNo = salesOrderDto.SalesOrderNumber;
                    SalesOrderStatus salesOrderStatus1 = salesOrderDto.SalesOrderStatus;
                    int salesOrderStatus = (int)salesOrderStatus1;
                    if (serverKey == "keus")
                    {
                        List<string> itemNumberList = salesOrderById?.SalesOrdersItems?.Select(x => x.ItemNumber).Distinct().ToList();
                        if (itemNumberList != null)
                        {
                            var json = JsonConvert.SerializeObject(itemNumberList);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();
                            var encodedSONumber = Uri.EscapeDataString(salesOrderNo);
                            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                            $"GetAvailableStockQtyForSalesOrderItems?salesOrderNo={encodedSONumber}&salesOrderStatus={salesOrderStatus}"))
                            {
                                Content = data
                            };
                            request.Headers.Add("Authorization", token);

                            var inventoryQtyResponse = await client.SendAsync(request);
                            var inventoryItemQtyDetails = await inventoryQtyResponse.Content.ReadAsStringAsync();
                            Dictionary<string, decimal> inventoryItemWithStockDetails = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(inventoryItemQtyDetails);


                            foreach (var salesOrderItemDetails in salesOrderById.SalesOrdersItems)
                            {
                                SalesOrderItemsDto salesOrderItemsDtos = _mapper.Map<SalesOrderItemsDto>(salesOrderItemDetails);
                                salesOrderItemsDtos.ShopOrderReleaseQty = salesOrderItemDetails.ShopOrderQty;
                                salesOrderItemsDtos.ScheduleDates = _mapper.Map<List<ScheduleDateDto>>(salesOrderItemDetails.ScheduleDates);
                                salesOrderItemsDtos.SoConfirmationDates = _mapper.Map<List<SoConfirmationDateDto>>(salesOrderItemDetails.SoConfirmationDates);
                                var ItemHistory = await _salesOrderHistory.GetSalesOrderHistoryBySONoAndItemNumberifShortCLosed(salesOrderNo, salesOrderItemsDtos.ItemNumber);
                                if (ItemHistory != null) salesOrderItemsDtos.ShortClosedQty = ItemHistory.Sum(x => x.ShortClosedQty);
                                string itemNumber = salesOrderItemsDtos.ItemNumber;
                                if (inventoryItemWithStockDetails.ContainsKey(itemNumber))
                                {
                                    salesOrderItemsDtos.AvailableStock = inventoryItemWithStockDetails[itemNumber];
                                }
                                else
                                {
                                    salesOrderItemsDtos.AvailableStock = 0;
                                }


                                salesOrderItemsDtoList.Add(salesOrderItemsDtos);
                            }
                        }
                    }
                    else
                    {
                        List<(string, string)> itemNumberList = salesOrderById?.SalesOrdersItems?.Select(x => (x.ItemNumber, x.ProjectNumber)).Distinct().ToList();

                        foreach (var salesOrderItemDetails in salesOrderById.SalesOrdersItems)
                        {
                            SalesOrderItemsDto salesOrderItemsDtos = _mapper.Map<SalesOrderItemsDto>(salesOrderItemDetails);
                            salesOrderItemsDtos.ShopOrderReleaseQty = salesOrderItemDetails.ShopOrderQty;
                            salesOrderItemsDtos.ScheduleDates = _mapper.Map<List<ScheduleDateDto>>(salesOrderItemDetails.ScheduleDates);
                            salesOrderItemsDtos.SoConfirmationDates = _mapper.Map<List<SoConfirmationDateDto>>(salesOrderItemDetails.SoConfirmationDates);
                            //var ItemHistory = await _salesOrderHistory.GetSalesOrderHistoryBySONoAndItemNumberifShortCLosed(salesOrderNo, salesOrderItemsDtos.ItemNumber);
                            //if (ItemHistory != null && ItemHistory.Count() > 0) salesOrderItemsDtos.ShortClosedQty = ItemHistory.Sum(x => x.ShortClosedQty);
                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();
                            var itemNumber = salesOrderItemsDtos.ItemNumber;
                            var encodedItemNumber = Uri.EscapeDataString(itemNumber);
                            var projectNo = salesOrderItemsDtos.ProjectNumber;
                            var encodedProjectNo = Uri.EscapeDataString(projectNo);

                            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryDetailsByItemNo?itemNumber={encodedItemNumber}&projectNo={encodedProjectNo}"));
                            request.Headers.Add("Authorization", token);

                            var inventoryQtyResponse = await client.SendAsync(request);
                            var inventoryItemQtyDetails = await inventoryQtyResponse.Content.ReadAsStringAsync();
                            var inventoryItemWithStockDetails = JsonConvert.DeserializeObject<InventoryItemdetailsDto>(inventoryItemQtyDetails);
                            if (inventoryItemWithStockDetails.data != null)
                            {
                                salesOrderItemsDtos.AvailableStock = inventoryItemWithStockDetails.data.Sum(x => x.balance_Quantity);
                            }
                            else
                            {
                                salesOrderItemsDtos.AvailableStock = 0;
                            }
                            salesOrderItemsDtoList.Add(salesOrderItemsDtos);
                        }
                    }



                    salesOrderDto.SalesOrdersItems = salesOrderItemsDtoList;
                    salesOrderDto.SalesOrderAdditionalCharges = salesAdditionalChargesList;
                    serviceResponse.Data = salesOrderDto;
                    serviceResponse.Message = $"Returned SalesOrder with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderDetialsById(int id)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();
            try
            {
                var salesOrderById = await _repository.GetSalesOrderById(id);
                string serverKey = GetServerKey();
                if (salesOrderById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    SalesOrderDto salesOrderDto = _mapper.Map<SalesOrderDto>(salesOrderById);


                    List<SalesOrderItemsDto> salesOrderItemsDtoList = new List<SalesOrderItemsDto>();

                    var salesAdditionalCharges = salesOrderById.SalesOrderAdditionalCharges;

                    //var salesAdditionalChargesList = _mapper.Map<List<SalesOrderAdditionalChargesDto>>(salesAdditionalCharges);
                    List<SalesOrderAdditionalChargesDto> salesAdditionalChargesList = _mapper.Map<List<SalesOrderAdditionalChargesDto>>(salesAdditionalCharges.ToList());

                    foreach (var salesOrderItemDetails in salesOrderById.SalesOrdersItems)
                    {
                        SalesOrderItemsDto salesOrderItemsDtos = _mapper.Map<SalesOrderItemsDto>(salesOrderItemDetails);
                        salesOrderItemsDtos.ScheduleDates = _mapper.Map<List<ScheduleDateDto>>(salesOrderItemDetails.ScheduleDates);
                        salesOrderItemsDtos.SoConfirmationDates = _mapper.Map<List<SoConfirmationDateDto>>(salesOrderItemDetails.SoConfirmationDates);

                        salesOrderItemsDtoList.Add(salesOrderItemsDtos);
                    }

                    salesOrderDto.SalesOrdersItems = salesOrderItemsDtoList;
                    salesOrderDto.SalesOrderAdditionalCharges = salesAdditionalChargesList;
                    serviceResponse.Data = salesOrderDto;
                    serviceResponse.Message = $"Returned SalesOrderDetails with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderDetialsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesOrderDetialsBySalesOrderNumber(string SalesOrderNumber)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();
            try
            {
                var salesOrderById = await _repository.GetSalesOrderDetailsBySONumber(SalesOrderNumber);
                string serverKey = GetServerKey();
                if (salesOrderById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrder with SalesOrderNumber: {SalesOrderNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with salesorderNumber: {SalesOrderNumber}");
                    SalesOrderDto salesOrderDto = _mapper.Map<SalesOrderDto>(salesOrderById);


                    List<SalesOrderItemsDto> salesOrderItemsDtoList = new List<SalesOrderItemsDto>();

                    var salesAdditionalCharges = salesOrderById.SalesOrderAdditionalCharges;

                    //var salesAdditionalChargesList = _mapper.Map<List<SalesOrderAdditionalChargesDto>>(salesAdditionalCharges);
                    List<SalesOrderAdditionalChargesDto> salesAdditionalChargesList = _mapper.Map<List<SalesOrderAdditionalChargesDto>>(salesAdditionalCharges.ToList());

                    foreach (var salesOrderItemDetails in salesOrderById.SalesOrdersItems)
                    {
                        SalesOrderItemsDto salesOrderItemsDtos = _mapper.Map<SalesOrderItemsDto>(salesOrderItemDetails);
                        salesOrderItemsDtos.ScheduleDates = _mapper.Map<List<ScheduleDateDto>>(salesOrderItemDetails.ScheduleDates);
                        salesOrderItemsDtos.SoConfirmationDates = _mapper.Map<List<SoConfirmationDateDto>>(salesOrderItemDetails.SoConfirmationDates);

                        salesOrderItemsDtoList.Add(salesOrderItemsDtos);
                    }

                    salesOrderDto.SalesOrdersItems = salesOrderItemsDtoList;
                    salesOrderDto.SalesOrderAdditionalCharges = salesAdditionalChargesList;
                    serviceResponse.Data = salesOrderDto;
                    serviceResponse.Message = $"Returned SalesOrderDetails with SalesOrderNumber: {SalesOrderNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderDetialsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderDetailsWithOutClosedAdditionalChargesById(int id)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();
            try
            {
                var salesOrderById = await _repository.GetSalesOrderById(id);
                string serverKey = GetServerKey();
                if (salesOrderById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    SalesOrderDto salesOrderDto = _mapper.Map<SalesOrderDto>(salesOrderById);

                    var quoteDetails = await _quoteRepository.GetQuoteByQuoteNumber(salesOrderDto.QuoteNumber);
                    if (quoteDetails != null)
                    {
                        salesOrderDto.QuoteRef = quoteDetails.QuoteRef;
                    }

                    var salesAdditionalChargesDto = salesOrderDto.SalesOrderAdditionalCharges;

                    var salesAdditionalChargesList = new List<SalesOrderAdditionalChargesDto>();
                    if (salesAdditionalChargesDto != null)
                    {
                        for (int i = 0; i < salesAdditionalChargesDto.Count; i++)
                        {
                            SalesOrderAdditionalChargesDto additionalChargesDetails = _mapper.Map<SalesOrderAdditionalChargesDto>(salesAdditionalChargesDto[i]);
                            if (additionalChargesDetails.SOAdditionalStatus != SoStatus.Closed)
                            {
                                salesAdditionalChargesList.Add(additionalChargesDetails);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    salesOrderDto.SalesOrdersItems = null;
                    salesOrderDto.SalesOrderAdditionalCharges = salesAdditionalChargesList;

                    _logger.LogInfo($"Returned SalesOrder with id: {id}");
                    serviceResponse.Data = salesOrderDto;
                    serviceResponse.Message = $"Returned SalesOrder with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<PurchaseOrderController>
        [HttpPost]
        public async Task<IActionResult> CreateSalesOrder([FromBody] SalesOrderPostDto salesOrderDtoPost)
        {
            ServiceResponse<SalesOrderPostDto> serviceResponse = new ServiceResponse<SalesOrderPostDto>();
            try
            {
                string serverKey = GetServerKey();

                if (salesOrderDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var createSalesOrder = _mapper.Map<SalesOrder>(salesOrderDtoPost);
                var salesOrderItemsDto = salesOrderDtoPost.SalesOrderItemsPostDtos;
                var salesOrderItemsList = new List<SalesOrderItems>();
                var SalesAdditionalChargesList = _mapper.Map<IEnumerable<SalesOrderAdditionalCharges>>(salesOrderDtoPost.SalesOrderAdditionalChargesPostDtos);
                //if (salesOrderItemsDto != null)
                //{
                //    for (int i = 0; i < salesOrderItemsDto.Count; i++)
                //    {
                //        SalesOrdersItems salesOrderItems = _mapper.Map<SalesOrdersItems>(salesOrderItemsDto[i]);
                //        salesOrderItemsList.Add(salesOrderItems);
                //    }
                //}
                //createSalesOrder.SalesOrdersItems = salesOrderItemsList;


                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                //var newcount = await _repository.GetSONumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    createSalesOrder.SalesOrderNumber = days + months + years + "SO" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    createSalesOrder.SalesOrderNumber = days + months + years + "SO" + (e);
                //}

                if (serverKey == "avision")
                {
                    var soNumber = await _repository.GenerateSONumberForAvision();
                    createSalesOrder.SalesOrderNumber = soNumber;
                }
                else
                {
                    var dateFormat = days + months + years;
                    var soNumber = await _repository.GenerateSONumber();
                    createSalesOrder.SalesOrderNumber = dateFormat + soNumber;
                }
                if (salesOrderItemsDto != null)
                {
                    for (int i = 0; i < salesOrderItemsDto.Count; i++)
                    {
                        SalesOrderItems salesOrderItems = _mapper.Map<SalesOrderItems>(salesOrderItemsDto[i]);
                        salesOrderItems.SalesOrderNumber = createSalesOrder.SalesOrderNumber;
                        salesOrderItems.BalanceQty = salesOrderItemsDto[i].OrderQty;
                        salesOrderItems.StatusEnum = OrderStatus.Open;
                        salesOrderItemsList.Add(salesOrderItems);
                    }
                }
                createSalesOrder.SOStatus = OrderStatus.Open;
                createSalesOrder.SalesOrdersItems = salesOrderItemsList;
                createSalesOrder.SalesOrderAdditionalCharges = SalesAdditionalChargesList.ToList();
                await _repository.CreateSalesOrder(createSalesOrder);

                //ShortClose Quote Once SalesOrder Created
                var quoteDetails = await _quoteRepository.GetQuoteByQuoteNumber(createSalesOrder.QuoteNumber);
                if (quoteDetails != null)
                {
                    quoteDetails.QuoteStatus = OrderStatus.Closed;
                    _quoteRepository.UpdateQuote(quoteDetails);
                    _quoteRepository.SaveAsync();
                }
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " SalesOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private string GetServerKey()
        {
            var serverName = Environment.MachineName;
            var serverConfiguration = _config.GetSection("ServerConfiguration");

            if (serverConfiguration.GetValue<bool?>("Server1:EnableKeus") == true)
            {
                return "keus";
            }
            else if (serverConfiguration.GetValue<bool?>("Server1:EnableAvision") == true)
            {
                return "avision";

            }
            else
            {
                return "trasccon";
            }
        }

        //From Date and To Date filter 
        [HttpGet]
        public async Task<IActionResult> SearchSalesOrderDate([FromQuery] SearchDateParam searchDateParam)
        {
            ServiceResponse<IEnumerable<SalesOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderReportDto>>();
            try
            {
                var salesOrderList = await _repository.SearchSalesOrderDate(searchDateParam);
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<SalesOrder, SalesOrderReportDto>();
                    cfg.CreateMap<SalesOrderItems, SalesOrderItemsReportDto>()
                        .ForMember(dest => dest.ScheduleDates, opt => opt.MapFrom(src => src.ScheduleDates
                        .Select(scheduleDate => new ScheduleDateReportDto
                        {
                            Id = scheduleDate.Id,
                            ItemNumber = src.ItemNumber,
                            SalesOrderNumber = src.SalesOrderNumber,
                            ProjectNumber = src.ProjectNumber,
                            Date = scheduleDate.Date,
                            Quantity = scheduleDate.Quantity
                        })
                            )
                        );
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<SalesOrderReportDto>>(salesOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrdersItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchSalesOrder([FromQuery] SearchParammes searchParams)
        {
            ServiceResponse<IEnumerable<SalesOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderReportDto>>();
            try
            {
                var salesOrderList = await _repository.SearchSalesOrder(searchParams);

                _logger.LogInfo("Returned all SalesOrders");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<SalesOrder, SalesOrderReportDto>()
                    .ForMember(dest => dest.SalesOrdersItems, opt => opt.MapFrom(src => src.SalesOrdersItems
                            .Select(prItem => new SalesOrderItemsReportDto
                            {
                                Id = prItem.Id,
                                ItemNumber = prItem.ItemNumber,
                                Description = prItem.Description,
                                SalesOrderNumber = prItem.SalesOrderNumber,
                                PriceList = prItem.PriceList,
                                ProjectNumber = prItem.ProjectNumber,
                                StatusEnum = prItem.StatusEnum,
                                BalanceQty = prItem.BalanceQty,
                                DispatchQty = prItem.DispatchQty,
                                ShopOrderQty = prItem.ShopOrderQty,
                                UOM = prItem.UOM,
                                Currency = prItem.Currency,
                                UnitPrice = prItem.UnitPrice,
                                OrderQty = prItem.OrderQty,
                                SGST = prItem.SGST,
                                CGST = prItem.CGST,
                                UTGST = prItem.UTGST,
                                IGST = prItem.IGST,
                                TotalAmount = prItem.TotalAmount,
                                BasicAmount = prItem.BasicAmount,
                                Discount = prItem.Discount,
                                RoomName = prItem.RoomName,
                                DiscountType = prItem.DiscountType,
                                RequestedDate = prItem.RequestedDate,
                                Remarks = prItem.Remarks,
                                ScheduleDates = prItem.ScheduleDates
                                    .Select(scheduleDate => new ScheduleDateReportDto
                                    {
                                        Id = scheduleDate.Id,
                                        ItemNumber = prItem.ItemNumber,
                                        SalesOrderNumber = src.SalesOrderNumber,
                                        Date = scheduleDate.Date,
                                        Quantity = scheduleDate.Quantity,
                                    }).ToList()
                            })
                         ));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<SalesOrderReportDto>>(salesOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrdersItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // sales order item level search

        [HttpGet]
        public async Task<IActionResult> SearchSalesOrderItem([FromQuery] SearchParammes searchParams)
        {
            ServiceResponse<IEnumerable<SalesOrderItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderItemsDto>>();
            try
            {
                var salesOrderList = await _salesOrderItemsRepository.SearchSalesOrderItem(searchParams);
                if (salesOrderList is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrderItem object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrderItem object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var result = _mapper.Map<IEnumerable<SalesOrderItemsDto>>(salesOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrdersItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //sales order with items filter concept
        [HttpPost]
        public async Task<IActionResult> GetAllSalesOrderWithItems([FromBody] SalesOrderSearchDto salesOrderSearch)
        {
            ServiceResponse<IEnumerable<SalesOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderDto>>();
            try
            {
                var salesOrderList = await _repository.GetAllSalesOrderWithItems(salesOrderSearch);

                _logger.LogInfo("Returned all SalesOrders");

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<SalesOrderItems, SalesOrderItemsReportDto>()
                        .ForMember(dest => dest.ScheduleDates, opt => opt.MapFrom(src => src.ScheduleDates
                        .Select(scheduleDate => new ScheduleDateReportDto
                        {
                            Id = scheduleDate.Id,
                            ItemNumber = src.ItemNumber,
                            SalesOrderNumber = src.SalesOrderNumber,
                            Date = scheduleDate.Date,
                            Quantity = scheduleDate.Quantity
                        })
                            )
                        );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<SalesOrderDto>>(salesOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SalesOrdersItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        // PUT api/<PurchaseOrderController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalesOrder(int id, [FromBody] SalesOrderUpdateDto salesOrderDtoUpdate)
        {
            ServiceResponse<SalesOrderPostDto> serviceResponse = new ServiceResponse<SalesOrderPostDto>();
            try
            {
                if (salesOrderDtoUpdate is null)
                {
                    _logger.LogError("Update SalesOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update SalesOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update SalesOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update SalesOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var salesOrderDetailBeforeUpdate = await _repository.GetSalesOrderById(id);
                var salesOrderItemDetailBeforeUpdate = salesOrderDetailBeforeUpdate.SalesOrdersItems;
                var salesOrderNumber = salesOrderDetailBeforeUpdate.SalesOrderNumber;

                if (salesOrderDetailBeforeUpdate is null)
                {
                    _logger.LogError($"Update SalesOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update SalesOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var salesOrderDetails = _mapper.Map<SalesOrder>(salesOrderDtoUpdate);
                var salesOrderItemsDto = salesOrderDtoUpdate.SalesOrderItemsUpdateDtos;
                var salesAdditionalChargesDto = salesOrderDtoUpdate.SalesOrderAdditionalChargesUpdateDtos;
                var salesOrderItemsList = new List<SalesOrderItems>();
                var salesAdditionalChargesList = new List<SalesOrderAdditionalCharges>();
                if (salesAdditionalChargesDto != null)
                {
                    for (int i = 0; i < salesAdditionalChargesDto.Count; i++)
                    {
                        SalesOrderAdditionalCharges additionalChargesDetails = _mapper.Map<SalesOrderAdditionalCharges>(salesAdditionalChargesDto[i]);
                        if (salesOrderDetailBeforeUpdate.SalesOrderAdditionalCharges != null && salesOrderDetailBeforeUpdate.SalesOrderAdditionalCharges.Count > 0)
                        {
                            if (salesOrderDetailBeforeUpdate.SalesOrderAdditionalCharges[i] != null && i < salesOrderDetailBeforeUpdate.SalesOrderAdditionalCharges.Count)
                            {
                                var oldSOAddCharges = salesOrderDetailBeforeUpdate.SalesOrderAdditionalCharges[i];
                                additionalChargesDetails.Id = oldSOAddCharges.Id;
                            }
                        }
                        salesAdditionalChargesList.Add(additionalChargesDetails);
                    }
                }
                if (salesOrderItemsDto != null)
                {
                    for (int i = 0; i < salesOrderItemsDto.Count; i++)
                    {
                        SalesOrderItems salesOrderItemsDetail = _mapper.Map<SalesOrderItems>(salesOrderItemsDto[i]);
                        if (salesOrderDetailBeforeUpdate.SalesOrdersItems != null && salesOrderDetailBeforeUpdate.SalesOrdersItems.Count > 0)
                        {
                            if (i < salesOrderDetailBeforeUpdate.SalesOrdersItems.Count && salesOrderDetailBeforeUpdate.SalesOrdersItems[i] != null)
                            {
                                var oldSOItem = salesOrderDetailBeforeUpdate.SalesOrdersItems[i];
                                salesOrderItemsDetail.Id = oldSOItem.Id;
                            }
                        }
                        if (salesOrderItemsDetail.StatusEnum != OrderStatus.ShortClosed)
                        {
                            salesOrderItemsDetail.BalanceQty = salesOrderItemsDetail.OrderQty - salesOrderItemsDetail.DispatchQty;
                            salesOrderItemsDetail.SalesOrderNumber = salesOrderNumber;
                        }
                        salesOrderItemsList.Add(salesOrderItemsDetail);

                    }

                    foreach (var salesOrderItemDetail in salesOrderDetailBeforeUpdate.SalesOrdersItems)
                    {
                        SalesOrderHistory salesOrderHistory = new SalesOrderHistory();
                        salesOrderHistory.LeadId = salesOrderDetailBeforeUpdate.LeadId;
                        salesOrderHistory.SalesOrderNumber = salesOrderDetailBeforeUpdate.SalesOrderNumber;
                        salesOrderHistory.SalesPerson = salesOrderDetailBeforeUpdate.SalesPerson;
                        salesOrderHistory.ProjectNumber = salesOrderDetailBeforeUpdate.ProjectNumber;
                        salesOrderHistory.QuoteNumber = salesOrderDetailBeforeUpdate.QuoteNumber;
                        salesOrderHistory.QuoteRevisionNumber = salesOrderDetailBeforeUpdate.QuoteRevisionNumber;
                        salesOrderHistory.OrderDate = salesOrderDetailBeforeUpdate.OrderDate;
                        salesOrderHistory.OrderType = salesOrderDetailBeforeUpdate.OrderType;
                        salesOrderHistory.CustomerName = salesOrderDetailBeforeUpdate.CustomerName;
                        salesOrderHistory.CustomerId = salesOrderDetailBeforeUpdate.CustomerId;
                        salesOrderHistory.RevisionNumber = salesOrderDetailBeforeUpdate.RevisionNumber;
                        salesOrderHistory.SOStatus = salesOrderDetailBeforeUpdate.SOStatus;
                        salesOrderHistory.ProductType = salesOrderDetailBeforeUpdate.ProductType;
                        salesOrderHistory.TypeOfSolution = salesOrderDetailBeforeUpdate.TypeOfSolution;
                        salesOrderHistory.PONumber = salesOrderDetailBeforeUpdate.PONumber;
                        salesOrderHistory.PODate = salesOrderDetailBeforeUpdate.PODate;
                        salesOrderHistory.ReceivedDate = salesOrderDetailBeforeUpdate.ReceivedDate;
                        salesOrderHistory.BillTo = salesOrderDetailBeforeUpdate.BillTo;
                        salesOrderHistory.BillToId = salesOrderDetailBeforeUpdate.BillToId;
                        salesOrderHistory.ShipTo = salesOrderDetailBeforeUpdate.ShipTo;
                        salesOrderHistory.ShipToId = salesOrderDetailBeforeUpdate.ShipToId;
                        salesOrderHistory.PaymentTerms = salesOrderDetailBeforeUpdate.PaymentTerms;
                        salesOrderHistory.ReasonForModification = salesOrderDetailBeforeUpdate.ReasonForModification;
                        salesOrderHistory.InstallationCharges = salesOrderDetailBeforeUpdate.InstallationCharges;
                        salesOrderHistory.TotalAmountWithInstallationCharges = salesOrderDetailBeforeUpdate.TotalAmountWithInstallationCharges;
                        salesOrderHistory.TotalAdditionalCharges = salesOrderDetailBeforeUpdate.TotalAdditionalCharges;
                        salesOrderHistory.SpecialDiscountType = salesOrderDetailBeforeUpdate.SpecialDiscountType;
                        salesOrderHistory.SpecialDiscountAmount = salesOrderDetailBeforeUpdate.SpecialDiscountAmount;
                        salesOrderHistory.Total = salesOrderDetailBeforeUpdate.Total;
                        salesOrderHistory.TotalFinalAmount = salesOrderDetailBeforeUpdate.TotalFinalAmount;
                        salesOrderHistory.ConfirmStatus = salesOrderDetailBeforeUpdate.ConfirmStatus;
                        salesOrderHistory.ApproveStatus = salesOrderDetailBeforeUpdate.ApproveStatus;
                        salesOrderHistory.ConfirmDate = salesOrderDetailBeforeUpdate.ConfirmDate;
                        salesOrderHistory.SoConfirmationStatus = salesOrderDetailBeforeUpdate.SoConfirmationStatus;
                        salesOrderHistory.Unit = salesOrderDetailBeforeUpdate.Unit;
                        salesOrderHistory.IsShortClosed = salesOrderDetailBeforeUpdate.IsShortClosed;
                        salesOrderHistory.ShortClosedBy = salesOrderDetailBeforeUpdate.ShortClosedBy;
                        salesOrderHistory.ShortClosedOn = salesOrderDetailBeforeUpdate.ShortClosedOn;
                        salesOrderHistory.CreatedBy = salesOrderDetailBeforeUpdate.CreatedBy;
                        salesOrderHistory.CreatedOn = salesOrderDetailBeforeUpdate.CreatedOn;
                        salesOrderHistory.LastModifiedBy = salesOrderDetailBeforeUpdate.LastModifiedBy;
                        salesOrderHistory.LastModifiedOn = salesOrderDetailBeforeUpdate.LastModifiedOn;
                        salesOrderHistory.ItemNumber = salesOrderItemDetail.ItemNumber;
                        salesOrderHistory.CustomerItemNumber = salesOrderItemDetail.CustomerItemNumber;
                        salesOrderHistory.Description = salesOrderItemDetail.Description;
                        salesOrderHistory.PartType = salesOrderItemDetail.PartType;
                        salesOrderHistory.StatusEnum = salesOrderItemDetail.StatusEnum;
                        salesOrderHistory.UOM = salesOrderItemDetail.UOM;
                        salesOrderHistory.Currency = salesOrderItemDetail.Currency;
                        salesOrderHistory.TotalAmount = salesOrderItemDetail.TotalAmount;
                        salesOrderHistory.BasicAmount = salesOrderItemDetail.BasicAmount;
                        salesOrderHistory.Discount = salesOrderItemDetail.Discount;
                        salesOrderHistory.RoomName = salesOrderItemDetail.RoomName;
                        salesOrderHistory.DiscountType = salesOrderItemDetail.DiscountType;
                        salesOrderHistory.UnitPrice = salesOrderItemDetail.UnitPrice;
                        salesOrderHistory.OrderQty = salesOrderItemDetail.OrderQty;
                        salesOrderHistory.BalanceQty = salesOrderItemDetail.BalanceQty;
                        salesOrderHistory.DispatchQty = salesOrderItemDetail.DispatchQty;
                        salesOrderHistory.ShopOrderQty = salesOrderItemDetail.ShopOrderQty;
                        salesOrderHistory.SGST = salesOrderItemDetail.SGST;
                        salesOrderHistory.UTGST = salesOrderItemDetail.UTGST;
                        salesOrderHistory.CGST = salesOrderItemDetail.CGST;
                        salesOrderHistory.IGST = salesOrderItemDetail.IGST;
                        salesOrderHistory.ReceivedDate = salesOrderItemDetail.RequestedDate;
                        salesOrderHistory.Remarks = salesOrderItemDetail.Remarks;
                        salesOrderHistory.SalesOrderId = salesOrderItemDetail.SalesOrderId;
                        salesOrderHistory.PriceList = salesOrderItemDetail.PriceList;

                        var salesOrderHistories = _mapper.Map<SalesOrderHistory>(salesOrderHistory);
                        await _salesOrderHistory.CreateSalesOrderHistory(salesOrderHistories);


                        foreach (var scheduleDateDetial in salesOrderItemDetail.ScheduleDates)
                        {
                            ScheduleDateHistory scheduleDateHistory = new ScheduleDateHistory();
                            scheduleDateHistory.Date = scheduleDateDetial.Date;
                            scheduleDateHistory.Quantity = scheduleDateDetial.Quantity;
                            scheduleDateHistory.SalesOrderHistoriesId = salesOrderHistory.Id;

                            var ScheduleDateHistories = _mapper.Map<ScheduleDateHistory>(scheduleDateHistory);
                            await _scheduleDateHistoryRepository.CreateScheduleDateHistory(ScheduleDateHistories);
                        }

                    }

                    foreach (var soAdditionalCharges in salesOrderDetailBeforeUpdate.SalesOrderAdditionalCharges)
                    {
                        SalesOrderAdditionalChargesHistory SalesOrderAdditionalChargesHistory = new SalesOrderAdditionalChargesHistory();
                        SalesOrderAdditionalChargesHistory.AdditionalChargesLabelName = soAdditionalCharges.AdditionalChargesLabelName;
                        SalesOrderAdditionalChargesHistory.AddtionalChargesValueType = soAdditionalCharges.AddtionalChargesValueType;
                        SalesOrderAdditionalChargesHistory.AddtionalChargesValueAmount = soAdditionalCharges.AddtionalChargesValueAmount;
                        SalesOrderAdditionalChargesHistory.IGST = soAdditionalCharges.IGST;
                        SalesOrderAdditionalChargesHistory.CGST = soAdditionalCharges.CGST;
                        SalesOrderAdditionalChargesHistory.UTGST = soAdditionalCharges.UTGST;
                        SalesOrderAdditionalChargesHistory.SGST = soAdditionalCharges.SGST;
                        SalesOrderAdditionalChargesHistory.TotalValue = soAdditionalCharges.TotalValue;
                        SalesOrderAdditionalChargesHistory.InvoicedValue = soAdditionalCharges.InvoicedValue;
                        SalesOrderAdditionalChargesHistory.SOAdditionalStatus = soAdditionalCharges.SOAdditionalStatus;
                        SalesOrderAdditionalChargesHistory.SalesOrderNumber = salesOrderDetailBeforeUpdate.SalesOrderNumber;
                        SalesOrderAdditionalChargesHistory.RevisionNumber = salesOrderDetailBeforeUpdate.RevisionNumber;

                        var SalesOrderAdditionalChargesHistories = _mapper.Map<SalesOrderAdditionalChargesHistory>(SalesOrderAdditionalChargesHistory);
                        await _salesOrderAdditionalChargesHistoryRepository.CreateSalesOrderAdditionalChargesHistory(SalesOrderAdditionalChargesHistories);
                    }

                    _salesOrderHistory.SaveAsync();
                    _scheduleDateHistoryRepository.SaveAsync();
                    _salesOrderAdditionalChargesHistoryRepository.SaveAsync();
                }

                //Update History Table
                await CreateSalesOrderHistory(salesOrderDetailBeforeUpdate);

                var updateData = _mapper.Map(salesOrderDtoUpdate, salesOrderDetailBeforeUpdate);
                updateData.SalesOrdersItems = salesOrderItemsList;
                updateData.SalesOrderAdditionalCharges = salesAdditionalChargesList;
                string result = await _repository.UpdateSalesOrder(updateData);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " SalesOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateSalesOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        private async Task<IActionResult> CreateSalesOrderHistory([FromBody] SalesOrder salesOrder)
        {
            ServiceResponse<SalesOrderMainLevelHistory> serviceResponse = new ServiceResponse<SalesOrderMainLevelHistory>();
            try
            {

                if (salesOrder is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrderHistory object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrderHistory object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrderHistory object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrderHistory object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var exsitingSalesOrderHistory = await _salesOrderMainLevelHistoryRepository.GetSalesOrderMainLevelHistoryBySalesOrderIdAndRevNo(salesOrder.Id, salesOrder.RevisionNumber);
                if (exsitingSalesOrderHistory == null)
                {
                    var salesOrderMainLevelHistory = _mapper.Map<SalesOrderMainLevelHistory>(salesOrder);
                    salesOrderMainLevelHistory.Id = 0;
                    salesOrderMainLevelHistory.SalesOrderId = salesOrder.Id;
                    salesOrderMainLevelHistory.CreatedBy = _createdBy;
                    salesOrderMainLevelHistory.CreatedOn = DateTime.Now;
                    salesOrderMainLevelHistory.LastModifiedBy = null;
                    salesOrderMainLevelHistory.LastModifiedOn = null;

                    var salesOrderItems = salesOrder.SalesOrdersItems;
                    var SalesOrderItemLevelHistoryList = new List<SalesOrderItemLevelHistory>();
                    var SalesOrderScheduleDateHistoryList = new List<SalesOrderScheduleDateHistory>();
                    var SOAdditionalChargesHistoryList = new List<SOAdditionalChargesHistory>();

                    if (salesOrder.SalesOrderAdditionalCharges != null && salesOrder.SalesOrderAdditionalCharges.Count > 0)
                    {
                        foreach (var SalesOrderAdditionalCharges in salesOrder.SalesOrderAdditionalCharges)
                        {
                            SOAdditionalChargesHistory soAdditionalChargesHistory = _mapper.Map<SOAdditionalChargesHistory>(SalesOrderAdditionalCharges);
                            soAdditionalChargesHistory.Id = 0;
                            soAdditionalChargesHistory.SOAdditionalChargeId = SalesOrderAdditionalCharges.Id;
                            SOAdditionalChargesHistoryList.Add(soAdditionalChargesHistory);
                        }
                    }

                    if (salesOrderItems != null && salesOrderItems.Count > 0)
                    {
                        for (int i = 0; i < salesOrderItems.Count; i++)
                        {
                            SalesOrderItemLevelHistory salesOrderItemLevelHistory = _mapper.Map<SalesOrderItemLevelHistory>(salesOrderItems[i]);
                            salesOrderItemLevelHistory.Id = 0;
                            salesOrderItemLevelHistory.SalesOrderItemId = salesOrderItems[i].Id;
                            salesOrderItemLevelHistory.RevisionNumber = salesOrder.RevisionNumber;

                            if (salesOrderItems[i].ScheduleDates != null && salesOrderItems[i].ScheduleDates.Count > 0)
                            {
                                foreach (var ScheduleDate in salesOrderItems[i].ScheduleDates)
                                {
                                    SalesOrderScheduleDateHistory salesOrderScheduleDateHistory = _mapper.Map<SalesOrderScheduleDateHistory>(ScheduleDate);
                                    salesOrderScheduleDateHistory.Id = 0;
                                    salesOrderScheduleDateHistory.SOScheduleDateId = ScheduleDate.Id;
                                    SalesOrderScheduleDateHistoryList.Add(salesOrderScheduleDateHistory);
                                }
                            }
                            salesOrderItemLevelHistory.SalesOrderScheduleDateHistory = SalesOrderScheduleDateHistoryList;
                            SalesOrderItemLevelHistoryList.Add(salesOrderItemLevelHistory);

                        }
                    }

                    salesOrderMainLevelHistory.SalesOrderItemLevelHistory = SalesOrderItemLevelHistoryList;
                    salesOrderMainLevelHistory.SOAdditionalChargesHistory = SOAdditionalChargesHistoryList;

                    await _salesOrderMainLevelHistoryRepository.CreateSalesOrderMainLevelHistory(salesOrderMainLevelHistory);
                    _salesOrderMainLevelHistoryRepository.SaveAsync();
                }
                else
                {
                    var salesOrderMainLevelHistoryId = await _salesOrderMainLevelHistoryRepository.GetSalesOrderMainLevelHistoryIdBySalesOrderIdAndRevNo(salesOrder.Id, salesOrder.RevisionNumber);

                    var salesOrderMainLevelHistory = _mapper.Map(salesOrder, exsitingSalesOrderHistory);
                    salesOrderMainLevelHistory.Id = salesOrderMainLevelHistoryId;
                    salesOrderMainLevelHistory.SalesOrderId = salesOrder.Id;
                    salesOrderMainLevelHistory.LastModifiedBy = _createdBy;
                    salesOrderMainLevelHistory.LastModifiedOn = DateTime.Now;

                    var salesOrderItems = salesOrder.SalesOrdersItems;
                    var SalesOrderItemLevelHistoryList = new List<SalesOrderItemLevelHistory>();
                    var SalesOrderScheduleDateHistoryList = new List<SalesOrderScheduleDateHistory>();

                    if (salesOrderItems != null && salesOrderItems.Count > 0)
                    {
                        for (int i = 0; i < salesOrderItems.Count; i++)
                        {
                            var exsitingSalesOrderItemLevelHistory = await _salesOrderItemLevelHistoryRepository.GetSalesOrderItemLevelHistoryBySalesOrderItemIdAndRevNo(salesOrderItems[i].Id, salesOrder.RevisionNumber);
                            if (exsitingSalesOrderItemLevelHistory != null)
                            {
                                var salesOrderItemLevelHistoryId = await _salesOrderItemLevelHistoryRepository.GetSalesOrderItemLevelHistoryIdBySalesOrderItemIdAndRevNo(salesOrderItems[i].Id, salesOrder.RevisionNumber);

                                var salesOrderItemLevelHistory = _mapper.Map(salesOrderItems[i], exsitingSalesOrderItemLevelHistory);
                                salesOrderItemLevelHistory.Id = salesOrderItemLevelHistoryId;
                                salesOrderItemLevelHistory.SalesOrderItemId = salesOrderItems[i].Id;
                                SalesOrderItemLevelHistoryList.Add(salesOrderItemLevelHistory);
                                salesOrderMainLevelHistory.SalesOrderItemLevelHistory = SalesOrderItemLevelHistoryList;

                            }
                            else
                            {
                                var salesOrderItemLevelHistory = _mapper.Map<SalesOrderItemLevelHistory>(salesOrderItems[i]);
                                salesOrderItemLevelHistory.Id = 0;
                                salesOrderItemLevelHistory.SalesOrderItemId = salesOrderItems[i].Id;
                                salesOrderItemLevelHistory.RevisionNumber = salesOrder.RevisionNumber;
                                salesOrderItemLevelHistory.SalesOrderMainLevelHistoryId = salesOrderMainLevelHistoryId;

                                if (salesOrderItems[i].ScheduleDates != null && salesOrderItems[i].ScheduleDates.Count > 0)
                                {
                                    foreach (var ScheduleDate in salesOrderItems[i].ScheduleDates)
                                    {
                                        SalesOrderScheduleDateHistory salesOrderScheduleDateHistory = _mapper.Map<SalesOrderScheduleDateHistory>(ScheduleDate);
                                        salesOrderScheduleDateHistory.Id = 0;
                                        salesOrderScheduleDateHistory.SOScheduleDateId = ScheduleDate.Id;
                                        SalesOrderScheduleDateHistoryList.Add(salesOrderScheduleDateHistory);
                                    }
                                }

                                salesOrderItemLevelHistory.SalesOrderScheduleDateHistory = SalesOrderScheduleDateHistoryList;

                                await _salesOrderItemLevelHistoryRepository.CreateSalesOrderItemLevelHistory(salesOrderItemLevelHistory);
                                _salesOrderItemLevelHistoryRepository.SaveAsync();
                            }

                        }
                    }


                    await _salesOrderMainLevelHistoryRepository.UpdateSalesOrderMainLevelHistory(salesOrderMainLevelHistory);
                    _salesOrderMainLevelHistoryRepository.SaveAsync();
                }

                serviceResponse.Data = null;
                serviceResponse.Message = " UpdateSalesOrderHistory Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);


            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateSalesOrderHistory action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        // DELETE api/<CompanyMasterController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrder(int id)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();
            try
            {
                var getSalesOrderById = await _repository.GetSalesOrderById(id);
                if (getSalesOrderById == null)
                {
                    _logger.LogError($"Delete SalesOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete SalesOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteSalesOrder(getSalesOrderById);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " SalesOrder Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> ShortCloseShopOrder(int id)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();

            try
            {
                var salesOrderShortCloseById = await _repository.GetSalesOrderById(id);
                if (salesOrderShortCloseById == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                //
                salesOrderShortCloseById.IsShortClosed = true;
                salesOrderShortCloseById.ShortClosedBy = _createdBy;
                salesOrderShortCloseById.ShortClosedOn = DateTime.Now;
                string result = await _repository.UpdateSalesOrder(salesOrderShortCloseById);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "SalesOrder have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SalesOrderShortClosed action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet("ItemNo")]
        public async Task<IActionResult> GetprojectNoByItemNo(string itemNo)
        {
            ServiceResponse<IEnumerable<ListOfProjectNoDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfProjectNoDto>>();

            try
            {
                var getProjectByItemNo = await _salesOrderItemsRepository.GetprojectNoByItemNo(itemNo);
                if (getProjectByItemNo.Count() == 0)
                {
                    _logger.LogError($"ProjectNo with id: {itemNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ProjectNo with itemNo, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ProjectNumber with itemNo: {itemNo}");
                    var result = _mapper.Map<List<ListOfProjectNoDto>>(getProjectByItemNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Successfully Returned ListOfProjectNo";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ProjectNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
        //get salesorder Detais by CustomerId

        [HttpGet]
        public async Task<IActionResult> GetSalesOrderDetailsByCustomerId(string Customerid)
        {
            ServiceResponse<IEnumerable<ListofSalesOrderDetails>> serviceResponse = new ServiceResponse<IEnumerable<ListofSalesOrderDetails>>();

            try
            {
                var getSalesDetailByCustomerId = await _repository.GetSalesOrderDetailsByCustomerId(Customerid);
                if (getSalesDetailByCustomerId == null)
                {
                    _logger.LogError($"SalesOrderDetail with id: {Customerid}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderDetail with id: {Customerid}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned SalesOrderDetail with id: {Customerid}");
                    var result = _mapper.Map<IEnumerable<ListofSalesOrderDetails>>(getSalesDetailByCustomerId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SalesDetail action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesOrderNoDetailsByCustomerId(string Customerid)
        {
            ServiceResponse<IEnumerable<ListofSalesOrderDetails>> serviceResponse = new ServiceResponse<IEnumerable<ListofSalesOrderDetails>>();

            try
            {
                var getSalesDetailByCustomerId = await _repository.GetSalesOrderNoDetailsByCustomerId(Customerid);
                if (getSalesDetailByCustomerId == null)
                {
                    _logger.LogError($"SalesOrderDetail with id: {Customerid}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderDetail with id: {Customerid}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned SalesOrderDetail with id: {Customerid}");
                    var result = _mapper.Map<IEnumerable<ListofSalesOrderDetails>>(getSalesDetailByCustomerId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderNoDetailsByCustomerId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesOrderTotalBySalesOrderId(int salesOrderId)
        {
            ServiceResponse<IEnumerable<SalesOrder>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrder>>();

            try
            {
                var salesOrderTotalBySalesOrderId = await _repository.GetSalesOrderTotalBySalesOrderId(salesOrderId);
                if (salesOrderTotalBySalesOrderId == null)
                {
                    _logger.LogError($"SalesOrderDetail with salesOrderId: {salesOrderId}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderDetail with salesOrderId: {salesOrderId}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned SalesOrderDetail with salesOrderId: {salesOrderId}");
                    //var result = _mapper.Map<IEnumerable<SalesOrder>>(salesOrderTotalBySalesOrderId);
                    //serviceResponse.Data = result;
                    //serviceResponse.Message = "Success";
                    //serviceResponse.Success = true;
                    //serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(salesOrderTotalBySalesOrderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderDetailsBySalesOrderId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //getprojectnumberbyitemnumber

        //getsalesorderDetailByprojectNoanditemNo --
        [HttpGet]
        public async Task<IActionResult> getSalesOrderDetailByProjectNoandItemNo(string ItemNo, string ProjectNo)
        {
            ServiceResponse<IEnumerable<GetSalesOrderDetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<GetSalesOrderDetailsDto>>();

            try
            {
                var getSalesDetail = await _salesOrderItemsRepository.getSalesOrderDetailByProjectNoandItemNo(ItemNo, ProjectNo);
                if (getSalesDetail.Count() == 0)
                {
                    _logger.LogError($"SalesOrderDetail with ItemNo: {ItemNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderDetail with ItemNo: {ItemNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned SalesOrderDetail with ItemNo: {ItemNo}");
                    var result = _mapper.Map<List<GetSalesOrderDetailsDto>>(getSalesDetail);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Successfully Returned SalesOrderDetail";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SalesDetail action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        //pass data from btodeliveryorder using _httpclient warehoouse service to salesservice



        [HttpPost]
        public async Task<IActionResult> UpdateDispatchDetails([FromBody] List<SalesOrderDispatchQtyDto> salesOrderDispatchQtyDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (salesOrderDispatchQtyDto == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder object sent from the client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrder object sent from the client is null.");
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrder object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrder object sent from the client.");
                    return BadRequest(serviceResponse);
                }

                foreach (var item in salesOrderDispatchQtyDto)
                {
                    IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.GetSalesOrderItemDetailsByIdandItemNo(item.FGItemNumber, item.SalesOrderId);
                    var dispatchedQty = item.DispatchQty;
                    if (salesOrderItems != null && salesOrderItems.Count() > 0)
                    {
                        foreach (var salesOrderItem in salesOrderItems)
                        {
                            var balanceQty = salesOrderItem.BalanceQty;

                            if (salesOrderItem.BalanceQty > dispatchedQty)
                            {
                                salesOrderItem.BalanceQty -= dispatchedQty;
                                salesOrderItem.DispatchQty += dispatchedQty;
                                dispatchedQty = 0;
                                salesOrderItem.StatusEnum = OrderStatus.PartiallyClosed;
                            }
                            else
                            {
                                salesOrderItem.BalanceQty = 0;
                                salesOrderItem.DispatchQty += balanceQty;
                                dispatchedQty -= balanceQty;
                                balanceQty = 0;
                                salesOrderItem.StatusEnum = OrderStatus.Closed;
                            }

                            await _salesOrderItemsRepository.UpdateSalesOrderItem(salesOrderItem);

                            if (dispatchedQty <= 0)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside UpdateDispatchDetails action in SalesOrder Controller");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Internal error in SalesOrderUpdate";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                }
                _salesOrderItemsRepository.SaveAsync();

                var salesdetails = await _repository.GetSalesOrderById(salesOrderDispatchQtyDto[0].SalesOrderId);

                int? soItemStatusCount = salesdetails.SalesOrdersItems?.Where(x => x.StatusEnum != OrderStatus.Closed || x.StatusEnum != OrderStatus.ShortClosed).Count() ?? 0;

                int? soAddStatusCount = salesdetails.SalesOrderAdditionalCharges?.Where(x => x.SOAdditionalStatus != SoStatus.Closed).Count() ?? 0;

                if (soItemStatusCount == 0 && soAddStatusCount == 0)
                {
                    salesdetails.SOStatus = OrderStatus.Closed;
                }
                else
                {
                    salesdetails.SOStatus = OrderStatus.PartiallyClosed;
                }

                await _repository.UpdateSalesOrderShortClose(salesdetails);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "SalesOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);


            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateDispatchDetails action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal error in SalesOrderUpdate";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        //below method old dispatch qty greater then new dispatch qty in bto edit part

        [HttpPost]
        public async Task<IActionResult> UpdateDispatchQtyGreaterThenNewDispatchQty([FromBody] List<SalesOrderDispatchQtyDto> salesOrderDispatchQtyDto)
        {
            //dynamic dispatchDetials
            //we have to write code for same itemnumber in multiple rows
            // Deserialise and store it in dynamic varibale
            //lopp thori=ug the dynamic variable an pass hte item number and so id to salesorderitemdetials, get 
            //the item object change the balanceqty and disoatchqty and pass the data to update method of service.
            foreach (var item in salesOrderDispatchQtyDto)
            {
                IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.GetSalesOrderItemDetailsByIdandItemNo(item.FGItemNumber, item.SalesOrderId);
                var orderItem = salesOrderItems.FirstOrDefault();
                orderItem.BalanceQty = orderItem.BalanceQty + item.DispatchQty;
                orderItem.DispatchQty -= item.DispatchQty;
                _salesOrderItemsRepository.UpdateSalesOrderItem(orderItem);
            }

            _salesOrderItemsRepository.SaveAsync();
            return Ok();
        }

        //below method old dispatch qty Smaller then new dispatch qty in bto edit part

        [HttpPost]
        public async Task<IActionResult> UpdateDispatchQtySmallerThenNewDispatchQty([FromBody] List<SalesOrderDispatchQtyDto> salesOrderDispatchQtyDto)
        {
            //dynamic dispatchDetials
            //we have to write code for same itemnumber in multiple rows
            // Deserialise and store it in dynamic varibale
            //lopp thori=ug the dynamic variable an pass hte item number and so id to salesorderitemdetials, get 
            //the item object change the balanceqty and disoatchqty and pass the data to update method of service.
            foreach (var item in salesOrderDispatchQtyDto)
            {
                IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.GetSalesOrderItemDetailsByIdandItemNo(item.FGItemNumber, item.SalesOrderId);
                var orderItem = salesOrderItems.FirstOrDefault();
                orderItem.BalanceQty = orderItem.BalanceQty - item.DispatchQty;
                orderItem.DispatchQty += item.DispatchQty;
                _salesOrderItemsRepository.UpdateSalesOrderItem(orderItem);
            }

            _salesOrderItemsRepository.SaveAsync();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> ReturnDOUpdateDispatchDetails([FromBody] List<ReturnDOSalesOrderDispatchQtyDto> salesOrderDispatchQtyDto)
        {
            try
            {
                ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
                if (salesOrderDispatchQtyDto == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder object sent from the client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrder object sent from the client is null.");
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrder object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrder object sent from the client.");
                    return BadRequest(serviceResponse);
                }
                int SalesorderId = 0;
                foreach (var item in salesOrderDispatchQtyDto)
                {
                    var doReturnQty = item.ReturnQty;
                    IEnumerable<SalesOrderItems>? salesOrderItems = await _salesOrderItemsRepository.GetSalesOrderItemDetailsForReturnByIdandItemNo(item.FGPartNumber, item.SalesOrderId);
                    SalesorderId = salesOrderItems.First().SalesOrderId;
                    if (salesOrderItems != null && salesOrderItems.Count() > 0)
                    {
                        foreach (var salesOrderDetails in salesOrderItems)
                        {
                            var salesOrderDisQty = salesOrderDetails.DispatchQty;

                            if (salesOrderDetails.DispatchQty <= doReturnQty)
                            {
                                salesOrderDetails.BalanceQty += salesOrderDisQty;
                                salesOrderDetails.DispatchQty = 0;
                                doReturnQty -= salesOrderDisQty;
                            }
                            else
                            {
                                salesOrderDetails.BalanceQty += doReturnQty;
                                salesOrderDetails.DispatchQty -= doReturnQty;
                                doReturnQty = 0;
                            }
                            if (salesOrderDetails.BalanceQty == salesOrderDetails.OrderQty)
                            {
                                salesOrderDetails.StatusEnum = OrderStatus.Open;
                            }
                            else if (salesOrderDetails.BalanceQty < salesOrderDetails.OrderQty && salesOrderDetails.BalanceQty > 0)
                            {
                                salesOrderDetails.StatusEnum = OrderStatus.PartiallyClosed;
                            }
                            else
                            {
                                salesOrderDetails.StatusEnum = OrderStatus.Closed;
                            }
                            await _salesOrderItemsRepository.UpdateSalesOrderItem(salesOrderDetails);
                            if (doReturnQty <= 0)
                            {
                                break;
                            }
                        }

                        _salesOrderItemsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"salesOrderItems Details is null");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Internal error in SalesOrderUpdate";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(serviceResponse);
                    }
                }
                if (SalesorderId > 0)
                {
                    //var salesdetails=await _repository.GetSalesOrderById(SalesorderId);
                    //if (salesdetails.SalesOrdersItems.Count() == salesdetails.SalesOrdersItems.Where(x => x.StatusEnum == OrderStatus.Closed || x.StatusEnum == OrderStatus.ShortClosed).Count()) salesdetails.SOStatus = OrderStatus.Closed;
                    //else if (salesdetails.SalesOrdersItems.Count() == salesdetails.SalesOrdersItems.Where(x => x.StatusEnum == OrderStatus.Open).Count()) salesdetails.SOStatus = OrderStatus.Open;
                    //else salesdetails.SOStatus = OrderStatus.PartiallyClosed;

                    var salesdetails = await _repository.GetSalesOrderById(salesOrderDispatchQtyDto[0].SalesOrderId);

                    int? soItemStatusCount = salesdetails.SalesOrdersItems?.Where(x => x.StatusEnum != OrderStatus.Closed || x.StatusEnum != OrderStatus.ShortClosed).Count() ?? 0;

                    int? soAddStatusCount = salesdetails.SalesOrderAdditionalCharges?.Where(x => x.SOAdditionalStatus != SoStatus.Closed).Count() ?? 0;

                    if (soItemStatusCount == 0 && soAddStatusCount == 0)
                    {
                        salesdetails.SOStatus = OrderStatus.Closed;
                    }
                    else
                    {
                        salesdetails.SOStatus = OrderStatus.PartiallyClosed;
                    }
                    await _repository.UpdateSalesOrderShortClose(salesdetails);
                    _repository.SaveAsync();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ReturnDOUpdateDispatchDetails action in SalesOrder Controller {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
        //Update Balancre Qty and DispatchQty Using ReturnInvoice 
        [HttpPost]
        public async Task<IActionResult> ReturnInvoiceUpdateDispatchDetails([FromBody] List<ReturnDOSalesOrderDispatchQtyDto> salesOrderDispatchQtyDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();

            try
            {
                if (salesOrderDispatchQtyDto == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder object sent from the client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrder object sent from the client is null.");
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrder object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrder object sent from the client.");
                    return BadRequest(serviceResponse);
                }
                int SalesorderId = 0;
                foreach (var item in salesOrderDispatchQtyDto)
                {
                    var invoiceReturnQty = item.ReturnQty;

                    IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.GetSalesOrderItemDetailsForReturnByIdandItemNo
                                                                                                                            (item.FGPartNumber, item.SalesOrderId);
                    SalesorderId = salesOrderItems.First().SalesOrderId;
                    if (salesOrderItems != null && salesOrderItems.Count() > 0)
                    {
                        foreach (var salesOrderDetails in salesOrderItems)
                        {
                            var salesOrderDisQty = salesOrderDetails.DispatchQty;

                            if (salesOrderDetails.DispatchQty <= invoiceReturnQty)
                            {
                                salesOrderDetails.BalanceQty += salesOrderDisQty;
                                salesOrderDetails.DispatchQty = 0;
                                invoiceReturnQty -= salesOrderDisQty;
                            }
                            else
                            {
                                salesOrderDetails.BalanceQty += invoiceReturnQty;
                                salesOrderDetails.DispatchQty -= invoiceReturnQty;
                                invoiceReturnQty = 0;

                            }

                            if (salesOrderDetails.BalanceQty == salesOrderDetails.OrderQty)
                            {
                                salesOrderDetails.StatusEnum = OrderStatus.Open;
                            }
                            else if (salesOrderDetails.BalanceQty < salesOrderDetails.OrderQty && salesOrderDetails.BalanceQty > 0)
                            {
                                salesOrderDetails.StatusEnum = OrderStatus.PartiallyClosed;
                            }
                            else
                            {
                                salesOrderDetails.StatusEnum = OrderStatus.Closed;
                            }
                            await _salesOrderItemsRepository.UpdateSalesOrderItem(salesOrderDetails);

                            if (invoiceReturnQty <= 0)
                            {
                                break;
                            }

                        }
                        _salesOrderItemsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"salesOrderItems Details is null");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Internal error in SalesOrderUpdate";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(serviceResponse);
                    }
                }
                if (SalesorderId > 0)
                {
                    //var salesdetails = await _repository.GetSalesOrderById(SalesorderId);
                    //if (salesdetails.SalesOrdersItems.Count() == salesdetails.SalesOrdersItems.Where(x => x.StatusEnum == OrderStatus.Closed || x.StatusEnum == OrderStatus.ShortClosed).Count()) salesdetails.SOStatus = OrderStatus.Closed;
                    //else if (salesdetails.SalesOrdersItems.Count() == salesdetails.SalesOrdersItems.Where(x => x.StatusEnum == OrderStatus.Open).Count()) salesdetails.SOStatus = OrderStatus.Open;
                    //else salesdetails.SOStatus = OrderStatus.PartiallyClosed;

                    var salesdetails = await _repository.GetSalesOrderById(salesOrderDispatchQtyDto[0].SalesOrderId);

                    int? soItemStatusCount = salesdetails.SalesOrdersItems?.Where(x => x.StatusEnum != OrderStatus.Closed || x.StatusEnum != OrderStatus.ShortClosed).Count() ?? 0;

                    int? soAddStatusCount = salesdetails.SalesOrderAdditionalCharges?.Where(x => x.SOAdditionalStatus != SoStatus.Closed).Count() ?? 0;

                    if (soItemStatusCount == 0 && soAddStatusCount == 0)
                    {
                        salesdetails.SOStatus = OrderStatus.Closed;
                    }
                    else
                    {
                        salesdetails.SOStatus = OrderStatus.PartiallyClosed;
                    }
                    await _repository.UpdateSalesOrderShortClose(salesdetails);
                    _repository.SaveAsync();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ReturnInvoiceUpdateDispatchDetails action in SalesOrder Controller {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
        //Update shoporder Qty in salesorder while create shoporderUpdateShopOrderQty
        //[HttpPost]
        //public async Task<IActionResult> UpdateShopOrderQty([FromBody] ShopOrderReleaseQtyDto shopOrderReleaseQtyDto)
        //{

        //        IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.UpdateShopOrderBySalesOrderNoandItemNo(shopOrderReleaseQtyDto.SalesOrderNumber
        //                                                                                     ,shopOrderReleaseQtyDto.FGItemNumber, shopOrderReleaseQtyDto.ProjectNumber);
        //        var orderItem = salesOrderItems.FirstOrDefault();

        //        orderItem.ShopOrderQty += shopOrderReleaseQtyDto.ReleaseQty;
        //        await _salesOrderItemsRepository.UpdateSalesOrderItem(orderItem);

        //     _salesOrderItemsRepository.SaveAsync();
        //     return Ok();
        //}
        [HttpPost]
        public async Task<IActionResult> UpdateShopOrderQty([FromBody] ShopOrderReleaseQtyDto shopOrderReleaseQtyDto)
        {

            IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.UpdateShopOrderBySalesOrderNoandItemNo(shopOrderReleaseQtyDto.SalesOrderNumber
                                                                                             , shopOrderReleaseQtyDto.FGItemNumber, shopOrderReleaseQtyDto.ProjectNumber);
            if (salesOrderItems.Count() > 0)
                foreach (var item in salesOrderItems)
                {
                    if (item.BalanceQty >= shopOrderReleaseQtyDto.ReleaseQty)
                    {
                        item.ShopOrderQty += shopOrderReleaseQtyDto.ReleaseQty;
                        await _salesOrderItemsRepository.UpdateSalesOrderItem(item);

                        _salesOrderItemsRepository.SaveAsync();
                        break;
                    }
                    else
                    {
                        item.ShopOrderQty = item.BalanceQty;
                        shopOrderReleaseQtyDto.ReleaseQty -= item.BalanceQty;
                        await _salesOrderItemsRepository.UpdateSalesOrderItem(item);

                        _salesOrderItemsRepository.SaveAsync();
                    }
                }
            else return NotFound();
            return Ok();
        }

        //Update Pending shoporder Qty in salesorder
        [HttpPost]
        public async Task<IActionResult> UpdatePendingShopOrderQty([FromBody] UpdatePendingShopOrderConfirmationQtyDto updatePendingShopOrderConfirmationQtyDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                IEnumerable<SalesOrderItems> salesOrderItems = await _salesOrderItemsRepository.UpdateShopOrderQtyBySalesOrderNoandItemNo(updatePendingShopOrderConfirmationQtyDto.SalesOrderNumber,
                                                                                                       updatePendingShopOrderConfirmationQtyDto.FGItemNumber, updatePendingShopOrderConfirmationQtyDto.ProjectNumber);

                var pendingSOConfQty = updatePendingShopOrderConfirmationQtyDto.PendingSoConfirmationQty;

                if (salesOrderItems.Count() > 0)
                {
                    foreach (var item in salesOrderItems)
                    {
                        if (item.ShopOrderQty >= pendingSOConfQty)
                        {
                            item.ShopOrderQty -= pendingSOConfQty;
                            pendingSOConfQty = 0;
                        }
                        else
                        {
                            pendingSOConfQty -= item.ShopOrderQty;
                            item.ShopOrderQty = 0;
                        }

                        await _salesOrderItemsRepository.UpdateSalesOrderItem(item);

                        if (pendingSOConfQty <= 0)
                        {
                            break;
                        }
                    }
                    _salesOrderItemsRepository.SaveAsync();
                }
                else
                {
                    _logger.LogError($"Something went wrong inside UpdateShopOrderQtyBySalesOrderNoandItemNo action:{updatePendingShopOrderConfirmationQtyDto.SalesOrderNumber}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder Not Found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrderQty in SalesOrder have been Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdatePendingShopOrderQty action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal error in SalesOrderUpdate";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        //Update Invoiced Value and DispatchQty Using Invoice 
        [HttpPost]
        public async Task<IActionResult> AdditionalChargeUpdateFromInvoice([FromBody] List<SoAdditionalChargeUpdateDto> soAdditionalChargeUpdateDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (soAdditionalChargeUpdateDto == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder object sent from the client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrder object sent from the client is null.");
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrder object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrder object sent from the client.");
                    return BadRequest(serviceResponse);
                }
                foreach (var item in soAdditionalChargeUpdateDto)
                {
                    var salesAdditionalCharges = await _salesAdditionalChargesRepository.GetSalesAdditionalChargesById(item.SalesOrderId, item.SalesAdditionalChargeId);

                    salesAdditionalCharges.InvoicedValue += item.InvoicedValue;
                    salesAdditionalCharges.SOAdditionalStatus = item.SOAdditionalStatus;
                    await _salesAdditionalChargesRepository.UpdateSalesAdditionalCharges(salesAdditionalCharges);


                }
                _salesAdditionalChargesRepository.SaveAsync();

                var salesdetails = await _repository.GetSalesOrderById(soAdditionalChargeUpdateDto[0].SalesOrderId);
                if (salesdetails.SOStatus != OrderStatus.ShortClosed)
                {
                    int? soItemStatusCount = salesdetails.SalesOrdersItems?.Where(x => x.StatusEnum != OrderStatus.Closed || x.StatusEnum != OrderStatus.ShortClosed).Count() ?? 0;

                    int? soAddStatusCount = salesdetails.SalesOrderAdditionalCharges?.Where(x => x.SOAdditionalStatus != SoStatus.Closed).Count() ?? 0;

                    if (soItemStatusCount == 0 && soAddStatusCount == 0)
                    {
                        salesdetails.SOStatus = OrderStatus.Closed;
                    }
                    else
                    {
                        salesdetails.SOStatus = OrderStatus.PartiallyClosed;
                    }

                    await _repository.UpdateSalesOrderShortClose(salesdetails);
                    _repository.SaveAsync();
                }
                //else
                //{
                //    foreach (var item in soAdditionalChargeUpdateDto)
                //    {
                //        var salesAdditionalCharges = await _salesAdditionalChargesRepository.GetSalesAdditionalChargesById(item.SalesOrderId, item.SalesAdditionalChargeId);
                //        if (salesAdditionalCharges != null)
                //        {
                //            var soAdditionalChargeHistory = await _sOAdditionalChargesHistoryRepository.GetSOAdditionalChargesDetailsById(salesAdditionalCharges.Id);
                //            if (soAdditionalChargeHistory != null)
                //            {
                //                soAdditionalChargeHistory.InvoicedValue = salesAdditionalCharges.InvoicedValue;
                //                soAdditionalChargeHistory.SOAdditionalStatus = salesAdditionalCharges.SOAdditionalStatus;
                //                await _sOAdditionalChargesHistoryRepository.UpdateSOAdditionalChargesHistory(soAdditionalChargeHistory);
                //            }
                //        }
                //    }
                //    _sOAdditionalChargesHistoryRepository.SaveAsync();
                //}
                serviceResponse.Data = null;
                serviceResponse.Message = "SalesOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside AdditionalChargeUpdateFromInvoice action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal error in SalesOrderUpdate";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AdditionalChargeUpdateFromReturnInvoice([FromBody] List<SoAdditionalChargeUpdateFromReturnDto> soAdditionalChargeUpdateDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (soAdditionalChargeUpdateDto == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrder object sent from the client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SalesOrder object sent from the client is null.");
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SalesOrder object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SalesOrder object sent from the client.");
                    return BadRequest(serviceResponse);
                }
                foreach (var item in soAdditionalChargeUpdateDto)
                {
                    var salesAdditionalCharges = await _salesAdditionalChargesRepository.GetSalesAdditionalChargesById(item.SalesOrderId, item.SalesAdditionalChargeId);

                    salesAdditionalCharges.InvoicedValue -= item.ReturnInvoicedValue;

                    if (salesAdditionalCharges.TotalValue == salesAdditionalCharges.InvoicedValue)
                    {
                        salesAdditionalCharges.SOAdditionalStatus = SoStatus.Closed;
                    }
                    else if (salesAdditionalCharges.TotalValue > salesAdditionalCharges.InvoicedValue && salesAdditionalCharges.InvoicedValue != 0)
                    {
                        salesAdditionalCharges.SOAdditionalStatus = SoStatus.PartiallyClosed;
                    }
                    else
                    {
                        salesAdditionalCharges.SOAdditionalStatus = SoStatus.Open;
                    }
                    await _salesAdditionalChargesRepository.UpdateSalesAdditionalCharges(salesAdditionalCharges);

                }
                _salesAdditionalChargesRepository.SaveAsync();

                var salesdetails = await _repository.GetSalesOrderById(soAdditionalChargeUpdateDto[0].SalesOrderId);

                if (salesdetails.SOStatus != OrderStatus.ShortClosed)
                {
                    int? soItemStatusCount = salesdetails.SalesOrdersItems?.Where(x => x.StatusEnum != OrderStatus.Closed || x.StatusEnum != OrderStatus.ShortClosed).Count() ?? 0;

                    int? soAddStatusCount = salesdetails.SalesOrderAdditionalCharges?.Where(x => x.SOAdditionalStatus != SoStatus.Closed).Count() ?? 0;

                    if (soItemStatusCount == 0 && soAddStatusCount == 0)
                    {
                        salesdetails.SOStatus = OrderStatus.Closed;
                    }
                    else
                    {
                        salesdetails.SOStatus = OrderStatus.PartiallyClosed;
                    }

                    await _repository.UpdateSalesOrderShortClose(salesdetails);
                    _repository.SaveAsync();
                }

                //else
                //{
                //    foreach (var item in soAdditionalChargeUpdateDto) 
                //    {
                //        var salesAdditionalCharges = await _salesAdditionalChargesRepository.GetSalesAdditionalChargesById(item.SalesOrderId, item.SalesAdditionalChargeId);
                //        if (salesAdditionalCharges != null)
                //        {
                //            var soAdditionalChargeHistory = await _sOAdditionalChargesHistoryRepository.GetSOAdditionalChargesDetailsById(salesAdditionalCharges.Id);
                //            if (soAdditionalChargeHistory != null)
                //            {
                //                soAdditionalChargeHistory.InvoicedValue = salesAdditionalCharges.InvoicedValue;
                //                soAdditionalChargeHistory.SOAdditionalStatus = salesAdditionalCharges.SOAdditionalStatus;
                //                await _sOAdditionalChargesHistoryRepository.UpdateSOAdditionalChargesHistory(soAdditionalChargeHistory);
                //            }
                //        }
                //    }
                //    _sOAdditionalChargesHistoryRepository.SaveAsync();
                //}

                serviceResponse.Data = null;
                serviceResponse.Message = "SalesOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside AdditionalChargeUpdateFromReturnInvoice action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal error in SalesOrderUpdate";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //getsalesorderdetailbyitemnoandsalesorderId

        [HttpPost]
        public async Task<IActionResult> GetFGSalesOrderDetailsByItemNo([FromBody] ItemdetailsDto itemdetailsDto)
        {
            ServiceResponse<ItemDetailsForShopOrderDto> serviceResponse = new ServiceResponse<ItemDetailsForShopOrderDto>();
            try
            {
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                string item = JsonConvert.SerializeObject(itemdetailsDto.itemNumber);
                var content = new StringContent(item, Encoding.UTF8, "application/json");
                //var bomDetails = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"],
                //    "GetAllProductionBomFGListByItemNumber?"), content);
                var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["EngineeringBomAPI"],
                            $"GetAllProductionBomFGListByItemNumber"))
                { Content = content };
                request.Headers.Add("Authorization", token);

                var bomDetails = await client.SendAsync(request);
                var bomDetailsString = await bomDetails.Content.ReadAsStringAsync();
                dynamic bomDetailsStringData = JsonConvert.DeserializeObject(bomDetailsString);
                dynamic bomData = bomDetailsStringData.data;
                string jsonString = JsonConvert.SerializeObject(bomData[0].bomVersionNo);
                JArray jArray = JArray.Parse(jsonString);
                decimal[] bomVersionNo = jArray.ToObject<decimal[]>();

                ItemDetailsForShopOrderDto itemDetailsDto = new ItemDetailsForShopOrderDto();
                itemDetailsDto.ItemNumber = bomData[0].itemNumber;
                itemDetailsDto.ItemType = bomData[0].itemType;
                itemDetailsDto.BomVersionNo = bomVersionNo[0] == 0 ? null : bomVersionNo;


                var projectSODetails = await _repository.GetProjectDetailsByItemNo(itemdetailsDto.itemNumber, itemdetailsDto.projectType);
                foreach (var project in projectSODetails)
                {
                    project.SalesOrderQtyDetails = await _repository.GetSalesOrderQtyDetailsByItemNo(itemdetailsDto.itemNumber, project.ProjectNumber);
                }
                itemDetailsDto.ProjectSODetails = projectSODetails;

                serviceResponse.Data = itemDetailsDto;
                serviceResponse.Message = "Returned all SalesOrderFGDetails";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetFGSalesOrderDetailsByItemNo action {ex.InnerException},{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> GetFGSalesOrderDetailsByItemNo(string itemNumber)
        //{
        //    ServiceResponse<ItemDetailsForShopOrderDto> serviceResponse = new ServiceResponse<ItemDetailsForShopOrderDto>();
        //    try
        //    {
        //        var bomDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"],
        //            "GetAllProductionBomFGListByItemNumber?", "itemNumber=", itemNumber));

        //        var bomDetailsString = await bomDetails.Content.ReadAsStringAsync();
        //        dynamic bomDetailsStringData = JsonConvert.DeserializeObject(bomDetailsString);
        //        dynamic bomData = bomDetailsStringData.data;
        //        string jsonString = JsonConvert.SerializeObject(bomData[0].bomVersionNo);
        //        JArray jArray = JArray.Parse(jsonString);
        //        decimal[] bomVersionNo = jArray.ToObject<decimal[]>();

        //        ItemDetailsForShopOrderDto itemDetailsDto = new ItemDetailsForShopOrderDto();
        //        itemDetailsDto.ItemNumber = bomData[0].itemNumber;
        //        itemDetailsDto.ItemType = bomData[0].itemType;
        //        itemDetailsDto.BomVersionNo = bomVersionNo;

        //        var projectSODetails = await _repository.GetProjectDetailsByItemNo(itemNumber);
        //        foreach (var project in projectSODetails)
        //        {
        //            project.SalesOrderQtyDetails = await _repository.GetSalesOrderQtyDetailsByItemNo(itemNumber, project.ProjectNumber);
        //        }
        //        itemDetailsDto.ProjectSODetails = projectSODetails;

        //        serviceResponse.Data = itemDetailsDto;
        //        serviceResponse.Message = "Returned all SalesOrderFGDetails";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong inside GetFGSalesOrderDetailsByItemNo action {ex.InnerException},{ex.Message}";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetSalesOrderSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<SalesOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderSPReport>>();
            try
            {
                var products = await _repository.GetSalesOrderSPReportWithDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned SalesOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSalesOrderSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }



        //        [HttpGet()] // Adjust your route as needed
        //        public async Task<IActionResult> GetSalesorderReportWithParam([FromQuery] string? CustomerName,[FromQuery] string? SalesOrderNumber,
        //                [FromQuery] string? PartNumber
        //)
        //        {
        //            ServiceResponse<IEnumerable<SalesOrderSPResport>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderSPResport>>();
        //            try
        //            {
        //                var result = await _repository.GetSalesorderReportWithParam(CustomerName, SalesOrderNumber, PartNumber);

        //                if (result == null)
        //                {
        //                    serviceResponse.Data = null;
        //                    serviceResponse.Message = $"SalesOrder hasn't been found.";
        //                    serviceResponse.Success = false;
        //                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //                    _logger.LogError($"SalesOrder hasn't been found in db.");
        //                    return NotFound(serviceResponse);
        //                }
        //                else
        //                {
        //                    serviceResponse.Data = result;
        //                    serviceResponse.Message = "Returned SalesOrder Details";
        //                    serviceResponse.Success = true;
        //                    serviceResponse.StatusCode = HttpStatusCode.OK;
        //                    return Ok(serviceResponse);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex.Message);
        //                serviceResponse.Data = null;
        //                serviceResponse.Message = $"Something went wrong inside GetSASalesOrderDetailsByItemNo action";
        //                serviceResponse.Success = false;
        //                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //                return StatusCode(500, serviceResponse);
        //            }
        //        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetSalesOrderSPReportWithParam([FromBody] SalesOrderSPResportDTO salesOrderSPResport)

        {
            ServiceResponse<IEnumerable<SalesOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderSPReport>>();
            try
            {
                var products = await _repository.GetSalesOrderSPReportWithParam(salesOrderSPResport.CustomerName, salesOrderSPResport.SalesOrderNumber, salesOrderSPResport.KPN);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned SalesOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside SalesOrderSPResport action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesOrderDashboardSPReportWithParam()
        {
            ServiceResponse<List<SalesOrderDashboardSPReport_Details>> serviceResponse = new ServiceResponse<List<SalesOrderDashboardSPReport_Details>>();
            try
            {
                List<SalesOrderDashboardSPReport_Details> salesOrderDashboardSPReport_Details = new List<SalesOrderDashboardSPReport_Details>();
                List<string> Bucket_Id = new List<string>();
                Bucket_Id.Add("bucket_Id1");
                Bucket_Id.Add("bucket_Id2");
                Bucket_Id.Add("bucket_Id3");
                Bucket_Id.Add("bucket_Id4");

                foreach (var buck in Bucket_Id)
                {
                    SalesOrderDashboardSPReport_Details salesOrderDashboardSPReport = new SalesOrderDashboardSPReport_Details();
                    salesOrderDashboardSPReport.Title = buck;
                    salesOrderDashboardSPReport.Items = await _repository.GetSalesOrderDashboardSPReportWithParam(buck);
                    salesOrderDashboardSPReport_Details.Add(salesOrderDashboardSPReport);
                }


                if (salesOrderDashboardSPReport_Details == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = salesOrderDashboardSPReport_Details;
                    serviceResponse.Message = "Returned SalesOrderDashboardSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSalesOrderDashboardSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionDashboardSPReportWithParam()
        {
            ServiceResponse<List<TransactionDashboardSPReport_Details>> serviceResponse = new ServiceResponse<List<TransactionDashboardSPReport_Details>>();
            try
            {
                List<TransactionDashboardSPReport_Details> transactionDashboardSPReport_Details = new List<TransactionDashboardSPReport_Details>();


                TransactionDashboardSPReport_Details transactionDashboardSPReport_Details1 = new TransactionDashboardSPReport_Details();
                TransactionDashboardSPReport_Details transactionDashboardSPReport_Details2 = new TransactionDashboardSPReport_Details();
                TransactionDashboardSPReport_Details transactionDashboardSPReport_Details3 = new TransactionDashboardSPReport_Details();
                TransactionDashboardSPReport_Details transactionDashboardSPReport_Details4 = new TransactionDashboardSPReport_Details();
                TransactionDashboardSPReport_Details transactionDashboardSPReport_Details5 = new TransactionDashboardSPReport_Details();
                transactionDashboardSPReport_Details1.Title = "bucket_Id1";
                transactionDashboardSPReport_Details2.Title = "bucket_Id2";
                transactionDashboardSPReport_Details3.Title = "bucket_Id3";
                transactionDashboardSPReport_Details4.Title = "bucket_Id4";
                transactionDashboardSPReport_Details5.Title = "bucket_Id5";
                transactionDashboardSPReport_Details1.Items1 = await _repository.GetTransactionDashboardSPReportWithParam_bucketId1();
                transactionDashboardSPReport_Details2.Items2 = await _repository.GetTransactionDashboardSPReportWithParam_bucketId2();
                transactionDashboardSPReport_Details3.Items3 = await _repository.GetTransactionDashboardSPReportWithParam_bucketId3();
                transactionDashboardSPReport_Details4.Items4 = await _repository.GetTransactionDashboardSPReportWithParam();
                transactionDashboardSPReport_Details5.Items5 = await _repository.GetTransactionDashboardSPReportWithParam_bucketId5();
                transactionDashboardSPReport_Details.Add(transactionDashboardSPReport_Details1);
                transactionDashboardSPReport_Details.Add(transactionDashboardSPReport_Details2);
                transactionDashboardSPReport_Details.Add(transactionDashboardSPReport_Details3);
                transactionDashboardSPReport_Details.Add(transactionDashboardSPReport_Details4);
                transactionDashboardSPReport_Details.Add(transactionDashboardSPReport_Details5);



                if (transactionDashboardSPReport_Details == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Transaction hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Transaction hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = transactionDashboardSPReport_Details;
                    serviceResponse.Message = "Returned TransactionDashboardSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetTransactionDashboardSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFinancialYearDashboardSPReportWithParam()
        {
            ServiceResponse<List<FinancialYearDashboardSPReport_Details>> serviceResponse = new ServiceResponse<List<FinancialYearDashboardSPReport_Details>>();
            try
            {
                List<FinancialYearDashboardSPReport_Details> financialYearDashboardSPReport_Details = new List<FinancialYearDashboardSPReport_Details>();
                List<string> Bucket_Id = new List<string>();
                Bucket_Id.Add("bucket_Id1");
                Bucket_Id.Add("bucket_Id2");
                Bucket_Id.Add("bucket_Id3");
                Bucket_Id.Add("bucket_Id4");
                Bucket_Id.Add("bucket_Id5");

                foreach (var buck in Bucket_Id)
                {
                    FinancialYearDashboardSPReport_Details financialYearDashboardSPReport_Details1 = new FinancialYearDashboardSPReport_Details();
                    financialYearDashboardSPReport_Details1.Title = buck;
                    financialYearDashboardSPReport_Details1.Items = await _repository.GetFinancialYearDashboardSPReportWithParam(buck);
                    financialYearDashboardSPReport_Details.Add(financialYearDashboardSPReport_Details1);
                }

                if (financialYearDashboardSPReport_Details == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"FinancialYear hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"FinancialYear hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = financialYearDashboardSPReport_Details;
                    serviceResponse.Message = "Returned FinancialYearDashboardSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetFinancialYearDashboardSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetSOSummarySPReportWithParam([FromBody] SOSummarySPResportDTO soSummarySPResportDTO)

        {
            ServiceResponse<IEnumerable<SOSummarySPReport>> serviceResponse = new ServiceResponse<IEnumerable<SOSummarySPReport>>();
            try
            {
                var products = await _repository.GetSOSummarySPReportWithParam(soSummarySPResportDTO.CustomerId, soSummarySPResportDTO.SalesOrderNumber, soSummarySPResportDTO.KPN);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderSummarySPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrderSummarySPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned SalesOrderSummarySPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSOSummarySPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetSOSummarySPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<SOSummarySPReport>> serviceResponse = new ServiceResponse<IEnumerable<SOSummarySPReport>>();
            try
            {
                var products = await _repository.GetSOSummarySPReportWithDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderSummarySPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrderSummarySPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned SalesOrderSummarySPReportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSOSummarySPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }


        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetSOMonthlyConsumptionSPReportWithParam([FromBody] SOMonthlyConsumptionDto soMonthlyConsumptionDto)

        {
            ServiceResponse<IEnumerable<SOMonthlyConsumptionSPReport>> serviceResponse = new ServiceResponse<IEnumerable<SOMonthlyConsumptionSPReport>>();
            try
            {
                var products = await _repository.GetSOMonthlyConsumptionSPReportWithParam(soMonthlyConsumptionDto.CustomerId);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderMonthlyConsumptionSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrderMonthlyConsumptionSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned SalesOrderMonthlyConsumptionSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSOMonthlyConsumptionSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderDetialsById_SP(int id)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();
            try
            {
                var product = await _repository.GetSalesOrderDetialsById_SP(id);
                string items, addit;
                items = product.SalesOrdersItems;
                addit = product.SalesOrderAdditionalCharges;
                product.SalesOrdersItems = null;
                product.SalesOrderAdditionalCharges = null;
                var result = _mapper.Map<SalesOrderDto>(product);
                if (items != null)
                {
                    result.SalesOrdersItems = JsonConvert.DeserializeObject<List<SalesOrderItemsDto>>(items);
                }
                if (addit != null)
                {
                    result.SalesOrderAdditionalCharges = JsonConvert.DeserializeObject<List<SalesOrderAdditionalChargesDto>>(addit);
                }
                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GetSalesOrderDetialsById_SP hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrderMonthlyConsumptionSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned GetSalesOrderDetialsById_SP Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderDetialsById_SP action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetCustomerWiseTransactionSPReportWithParam([FromBody] SOMonthlyConsumptionDto customerWiseTransactionSPReport)

        {
            ServiceResponse<IEnumerable<CustomerWiseTransactionSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CustomerWiseTransactionSPReport>>();
            try
            {
                var products = await _repository.GetCustomerWiseTransactionSPReportWithParam(customerWiseTransactionSPReport.CustomerId);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CustomerWiseTransactionSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CustomerWiseTransactionSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned CustomerWiseTransactionSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCustomerWiseTransactionSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetSOMonthlyConsumptionSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<SOMonthlyConsumptionSPReport>> serviceResponse = new ServiceResponse<IEnumerable<SOMonthlyConsumptionSPReport>>();
            try
            {
                var products = await _repository.GetSOMonthlyConsumptionSPReportWithDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderMonthlyConsumptionSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrderMonthlyConsumptionSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned SalesOrderMonthlyConsumptionSPReportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSOMonthlyConsumptionSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }



        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetRfqSalesOrderSPReportWithParam([FromBody] RfqSalesOrderSPResportDTO rfqSalesOrderSPResportDTO)

        {
            ServiceResponse<IEnumerable<RfqSalesOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<RfqSalesOrderSPReport>>();
            try
            {
                var products = await _repository.GetRfqSalesOrderSPReportWithParam(rfqSalesOrderSPResportDTO.CustomerName, rfqSalesOrderSPResportDTO.SalesOrderNumber,
                                                                                                                        rfqSalesOrderSPResportDTO.KPN, rfqSalesOrderSPResportDTO.SOStatus);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqSalesOrderSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqSalesOrderSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned RfqSalesOrderSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetRfqSalesOrderSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetRfqSalesOrderRoomWiseSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)

        {
            ServiceResponse<IEnumerable<RfqSalesOrderRoomWiseSPReport>> serviceResponse = new ServiceResponse<IEnumerable<RfqSalesOrderRoomWiseSPReport>>();
            try
            {
                var products = await _repository.GetRfqSalesOrderRoomWiseSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqSalesOrderRoomWiseSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqSalesOrderRoomWiseSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned RfqSalesOrderRoomWiseSPReportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetRfqSalesOrderRoomWiseSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetRfqSalesOrderRoomWiseSPReportWithParam([FromBody] SalesOrderSPResportDTO rfqSalesOrderRoomWiseSPResportDTO)

        {
            ServiceResponse<IEnumerable<RfqSalesOrderRoomWiseSPReport>> serviceResponse = new ServiceResponse<IEnumerable<RfqSalesOrderRoomWiseSPReport>>();
            try
            {
                var products = await _repository.GetRfqSalesOrderRoomWiseSPReportWithParam(rfqSalesOrderRoomWiseSPResportDTO.CustomerName, rfqSalesOrderRoomWiseSPResportDTO.SalesOrderNumber,
                                                                                                                        rfqSalesOrderRoomWiseSPResportDTO.KPN);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqSalesOrderRoomWiseSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqSalesOrderRoomWiseSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned RfqSalesOrderRoomWiseSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetRfqSalesOrderRoomWiseSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetRfqSalesOrderSPReportWithParamForTrans([FromBody] RfqSalesOrderSPResportDTOForTrans rfqSalesOrderSPResportDTO)

        {
            ServiceResponse<IEnumerable<RfqSalesOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<RfqSalesOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.GetRfqSalesOrderSPReportWithParamForTrans(rfqSalesOrderSPResportDTO.CustomerName, rfqSalesOrderSPResportDTO.SalesOrderNumber,
                                                                                                                        rfqSalesOrderSPResportDTO.KPN, rfqSalesOrderSPResportDTO.SOStatus
                                                                                                                        , rfqSalesOrderSPResportDTO.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqSalesOrderSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqSalesOrderSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned RfqSalesOrderSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetRfqSalesOrderSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetRfqSalesOrderSPReportWithParamForAvision([FromBody] RfqSalesOrderSPResportDTO rfqSalesOrderSPResportDTO)

        {
            ServiceResponse<IEnumerable<RfqSalesOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<RfqSalesOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.GetRfqSalesOrderSPReportWithParamForAvision(rfqSalesOrderSPResportDTO.CustomerName, rfqSalesOrderSPResportDTO.SalesOrderNumber,
                                                                                                                        rfqSalesOrderSPResportDTO.KPN, rfqSalesOrderSPResportDTO.SOStatus);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqSalesOrderSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqSalesOrderSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned RfqSalesOrderSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetRfqSalesOrderSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetRfqSalesOrderSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<RfqSalesOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<RfqSalesOrderSPReport>>();
            try
            {
                var products = await _repository.GetRfqSalesOrderSPReportWithDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqSalesOrderSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqSalesOrderSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned RfqSalesOrderSPReportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetRfqSalesOrderSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetRfqSalesOrderSPReportWithDateForTransAvision([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<RfqSalesOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<RfqSalesOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.GetRfqSalesOrderSPReportWithDateForTransAvision(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqSalesOrderSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqSalesOrderSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned RfqSalesOrderSPReportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetRfqSalesOrderSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetForecastSalesOrderSPReportWithParam([FromBody] ForecastSalesOrderSPResportDTO forecastSalesOrderSPResportDTO)

        {
            ServiceResponse<IEnumerable<ForecastSalesOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<ForecastSalesOrderSPReport>>();
            try
            {
                var products = await _repository.GetForecastSalesOrderSPReportWithParam(forecastSalesOrderSPResportDTO.CustomerName,
                                                                             forecastSalesOrderSPResportDTO.SalesOrderNumber, forecastSalesOrderSPResportDTO.KPN, forecastSalesOrderSPResportDTO.SOStatus);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForecastSalesOrderSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForecastSalesOrderSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ForecastSalesOrderSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetForecastSalesOrderSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetForecastSalesOrderSPReportWithParamForTrans([FromBody] ForecastSalesOrderSPResportDTOForTrans forecastSalesOrderSPResportDTO)

        {
            ServiceResponse<IEnumerable<ForecastSalesOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<ForecastSalesOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.GetForecastSalesOrderSPReportWithParamForTrans(forecastSalesOrderSPResportDTO.CustomerName,
                                                                             forecastSalesOrderSPResportDTO.SalesOrderNumber, forecastSalesOrderSPResportDTO.KPN, forecastSalesOrderSPResportDTO.SOStatus,
                                                                             forecastSalesOrderSPResportDTO.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForecastSalesOrderSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForecastSalesOrderSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ForecastSalesOrderSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetForecastSalesOrderSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetForecastSalesOrderSPReportWithParamForAvision([FromBody] ForecastSalesOrderSPResportDTO forecastSalesOrderSPResportDTO)

        {
            ServiceResponse<IEnumerable<ForecastSalesOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<ForecastSalesOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.GetForecastSalesOrderSPReportWithParamForAvision(forecastSalesOrderSPResportDTO.CustomerName,
                                                                             forecastSalesOrderSPResportDTO.SalesOrderNumber, forecastSalesOrderSPResportDTO.KPN, forecastSalesOrderSPResportDTO.SOStatus);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForecastSalesOrderSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForecastSalesOrderSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ForecastSalesOrderSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetForecastSalesOrderSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetForecastSalesOrderSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<ForecastSalesOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<ForecastSalesOrderSPReport>>();
            try
            {
                var products = await _repository.GetForecastSalesOrderSPReportWithDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForecastSalesOrderSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForecastSalesOrderSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ForecastSalesOrderSPReportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetForecastSalesOrderSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetForecastSalesOrderSPReportWithDateForTransAvision([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<ForecastSalesOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<ForecastSalesOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.GetForecastSalesOrderSPReportWithDateForTransAvision(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForecastSalesOrderSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForecastSalesOrderSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ForecastSalesOrderSPReportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetForecastSalesOrderSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost]
        public async Task<IActionResult> ExportSalesOrderSPReportToExcel([FromBody] SalesOrderSPResportDTO salesOrderSPReport)
        {
            try
            {
                // Get data from repository using stored procedure
                var salesOrderSPReportDetails = await _repository.GetSalesOrderSPReportWithParam(salesOrderSPReport.CustomerName, salesOrderSPReport.SalesOrderNumber, salesOrderSPReport.KPN);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("SalesOrderSPReport");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Sales Order Number");
                headerRow.CreateCell(1).SetCellValue("Customer ID");
                headerRow.CreateCell(2).SetCellValue("Customer Name");
                headerRow.CreateCell(3).SetCellValue("Lead ID");
                headerRow.CreateCell(4).SetCellValue("Order Type");
                headerRow.CreateCell(5).SetCellValue("Type Of Solution");
                headerRow.CreateCell(6).SetCellValue("Product Type");
                headerRow.CreateCell(7).SetCellValue("Material Group");
                headerRow.CreateCell(8).SetCellValue("Item Type");
                headerRow.CreateCell(9).SetCellValue("Sales Person");
                headerRow.CreateCell(10).SetCellValue("SO Date");
                headerRow.CreateCell(11).SetCellValue("KPN");
                headerRow.CreateCell(12).SetCellValue("KPN Description");
                headerRow.CreateCell(13).SetCellValue("UOC");
                headerRow.CreateCell(14).SetCellValue("UOM");
                headerRow.CreateCell(15).SetCellValue("Price List");
                headerRow.CreateCell(16).SetCellValue("Unit Price");
                headerRow.CreateCell(17).SetCellValue("Basic Amount");
                headerRow.CreateCell(18).SetCellValue("DiscountType");
                headerRow.CreateCell(19).SetCellValue("Discount");
                headerRow.CreateCell(20).SetCellValue("SGST");
                headerRow.CreateCell(21).SetCellValue("CGST");
                headerRow.CreateCell(22).SetCellValue("IGST");
                headerRow.CreateCell(23).SetCellValue("UTGST");
                headerRow.CreateCell(24).SetCellValue("ItemPriceList");
                headerRow.CreateCell(25).SetCellValue("Total Amount");
                headerRow.CreateCell(26).SetCellValue("Order Qty");
                headerRow.CreateCell(27).SetCellValue("Dispatch Qty");
                headerRow.CreateCell(28).SetCellValue("Balance Qty");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in salesOrderSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.SalesOrderNumber);
                    row.CreateCell(1).SetCellValue(item.CustomerId);
                    row.CreateCell(2).SetCellValue(item.CustomerName);
                    row.CreateCell(3).SetCellValue(item.LeadId);
                    row.CreateCell(4).SetCellValue(item.OrderType);
                    row.CreateCell(5).SetCellValue(item.TypeOfSolution);
                    row.CreateCell(6).SetCellValue(item.ProductType);
                    row.CreateCell(7).SetCellValue(item.MaterialGroup);
                    row.CreateCell(8).SetCellValue(item.ItemType.HasValue ? Enum.GetName(typeof(PartType), item.ItemType) : ""); // Assuming ItemType is nullable int
                    row.CreateCell(9).SetCellValue(item.SalesPerson);
                    row.CreateCell(10).SetCellValue(item.sodate.HasValue ? item.sodate.Value.ToString("MM/dd/yyyy") : ""); // Assuming sodate is nullable DateTime
                    row.CreateCell(11).SetCellValue(item.KPN);
                    row.CreateCell(12).SetCellValue(item.KPNDescription);
                    row.CreateCell(13).SetCellValue(item.UOC);
                    row.CreateCell(14).SetCellValue(item.UOM);
                    row.CreateCell(15).SetCellValue(item.PriceList);
                    row.CreateCell(16).SetCellValue(Convert.ToDouble(item.UnitPrice)); // Assuming UnitPrice is decimal
                    row.CreateCell(17).SetCellValue(Convert.ToDouble(item.BasicAmount)); // Assuming BasicAmount is decimal
                    row.CreateCell(18).SetCellValue(item.DiscountType);
                    row.CreateCell(19).SetCellValue(item.Discount);
                    row.CreateCell(20).SetCellValue(Convert.ToDouble(item.SGST)); // Assuming SGST is decimal
                    row.CreateCell(21).SetCellValue(Convert.ToDouble(item.CGST)); // Assuming CGST is decimal
                    row.CreateCell(22).SetCellValue(Convert.ToDouble(item.IGST)); // Assuming IGST is decimal
                    row.CreateCell(23).SetCellValue(Convert.ToDouble(item.UTGST)); // Assuming UTGST is decimal
                    row.CreateCell(24).SetCellValue(Convert.ToDouble(item.itempricelist)); // Assuming itempricelist is decimal
                    row.CreateCell(25).SetCellValue(Convert.ToDouble(item.TotalAmount)); // Assuming TotalAmount is decimal
                    row.CreateCell(26).SetCellValue(Convert.ToDouble(item.OrderQty)); // Assuming OrderQty is decimal
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.DispatchQty)); // Assuming DispatchQty is decimal
                    row.CreateCell(28).SetCellValue(Convert.ToDouble(item.BalanceQty)); // Assuming BalanceQty is decimal
                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesOrderSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportSalesOrderRFQSPReportToExcel([FromBody] RfqSalesOrderSPResportDTO salesOrderSPReport)
        {
            try
            {
                // Get data from repository using stored procedure
                var salesOrderSPReportDetails = await _repository.GetRfqSalesOrderSPReportWithParam(salesOrderSPReport.CustomerName, salesOrderSPReport.SalesOrderNumber, salesOrderSPReport.KPN, salesOrderSPReport.SOStatus);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("RFQSPReport");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Sales Order Number");
                headerRow.CreateCell(1).SetCellValue("SOStatus");
                headerRow.CreateCell(2).SetCellValue("Project Number");
                headerRow.CreateCell(3).SetCellValue("Customer ID");
                headerRow.CreateCell(4).SetCellValue("Customer Name");
                headerRow.CreateCell(5).SetCellValue("Lead ID");
                headerRow.CreateCell(6).SetCellValue("Order Type");
                headerRow.CreateCell(7).SetCellValue("Type Of Solution");
                headerRow.CreateCell(8).SetCellValue("Product Type");
                headerRow.CreateCell(9).SetCellValue("Material Group");
                headerRow.CreateCell(10).SetCellValue("Item Type");
                headerRow.CreateCell(11).SetCellValue("Sales Person");
                headerRow.CreateCell(12).SetCellValue("SO Date");
                headerRow.CreateCell(13).SetCellValue("KPN");
                headerRow.CreateCell(14).SetCellValue("KPN Description");
                headerRow.CreateCell(15).SetCellValue("UOC");
                headerRow.CreateCell(16).SetCellValue("UOM");
                headerRow.CreateCell(17).SetCellValue("Price List");
                headerRow.CreateCell(18).SetCellValue("Unit Price");
                headerRow.CreateCell(19).SetCellValue("Basic Amount");
                headerRow.CreateCell(20).SetCellValue("DiscountType");
                headerRow.CreateCell(21).SetCellValue("Discount");
                headerRow.CreateCell(22).SetCellValue("SGST");
                headerRow.CreateCell(23).SetCellValue("CGST");
                headerRow.CreateCell(24).SetCellValue("IGST");
                headerRow.CreateCell(25).SetCellValue("UTGST");
                headerRow.CreateCell(26).SetCellValue("ItemPriceList");
                headerRow.CreateCell(27).SetCellValue("Total Amount");
                headerRow.CreateCell(28).SetCellValue("Order Qty");
                headerRow.CreateCell(29).SetCellValue("Dispatch Qty");
                headerRow.CreateCell(30).SetCellValue("Balance Qty");
                headerRow.CreateCell(31).SetCellValue("Indent Qnty");
                headerRow.CreateCell(32).SetCellValue("RequestedDate");
                headerRow.CreateCell(33).SetCellValue("MSL");
                headerRow.CreateCell(34).SetCellValue("StdCost");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in salesOrderSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.SalesOrderNumber);
                    row.CreateCell(1).SetCellValue(item.SOStatus.HasValue ? Enum.GetName(typeof(OrderStatus), item.SOStatus) : ""); // Assuming SOStatus is nullable int
                    row.CreateCell(2).SetCellValue(item.ProjectNumber ?? ""); // Assuming ProjectNumber is nullable string
                    row.CreateCell(3).SetCellValue(item.CustomerId);
                    row.CreateCell(4).SetCellValue(item.CustomerName);
                    row.CreateCell(5).SetCellValue(item.LeadId);
                    row.CreateCell(6).SetCellValue(item.OrderType);
                    row.CreateCell(7).SetCellValue(item.TypeOfSolution);
                    row.CreateCell(8).SetCellValue(item.ProductType);
                    row.CreateCell(9).SetCellValue(item.MaterialGroup);
                    row.CreateCell(10).SetCellValue(item.ItemType); // Assuming ItemType is nullable int
                    row.CreateCell(11).SetCellValue(item.SalesPerson);
                    row.CreateCell(12).SetCellValue(item.sodate.HasValue ? item.sodate.Value.ToString("MM/dd/yyyy") : ""); // Assuming sodate is nullable DateTime
                    row.CreateCell(13).SetCellValue(item.KPN);
                    row.CreateCell(14).SetCellValue(item.KPNDescription);
                    row.CreateCell(15).SetCellValue(item.UOC);
                    row.CreateCell(16).SetCellValue(item.UOM);
                    row.CreateCell(17).SetCellValue(item.PriceList);
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(item.UnitPrice)); // Assuming UnitPrice is decimal
                    row.CreateCell(19).SetCellValue(Convert.ToDouble(item.BasicAmount)); // Assuming BasicAmount is decimal
                    row.CreateCell(20).SetCellValue(item.DiscountType);
                    row.CreateCell(21).SetCellValue(item.Discount);
                    row.CreateCell(22).SetCellValue(Convert.ToDouble(item.SGST)); // Assuming SGST is decimal
                    row.CreateCell(23).SetCellValue(Convert.ToDouble(item.CGST)); // Assuming CGST is decimal
                    row.CreateCell(24).SetCellValue(Convert.ToDouble(item.IGST)); // Assuming IGST is decimal
                    row.CreateCell(25).SetCellValue(Convert.ToDouble(item.UTGST)); // Assuming UTGST is decimal
                    row.CreateCell(26).SetCellValue(Convert.ToDouble(item.itempricelist)); // Assuming itempricelist is decimal
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.TotalAmount)); // Assuming TotalAmount is decimal
                    row.CreateCell(28).SetCellValue(Convert.ToDouble(item.OrderQty)); // Assuming OrderQty is decimal
                    row.CreateCell(29).SetCellValue(Convert.ToDouble(item.DispatchQty)); // Assuming DispatchQty is decimal
                    row.CreateCell(30).SetCellValue(Convert.ToDouble(item.BalanceQty)); // Assuming BalanceQty is decimal
                    row.CreateCell(31).SetCellValue(Convert.ToDouble(item.indent_qnty)); // Assuming BalanceQty is decimal
                    row.CreateCell(32).SetCellValue(item.RequestedDate.HasValue ? item.RequestedDate.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(33).SetCellValue(Convert.ToDouble(item.MSL)); // Assuming BalanceQty is decimal
                    row.CreateCell(34).SetCellValue(Convert.ToDouble(item.StdCost)); // Assuming BalanceQty is decimal
                }


                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RFQSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportSalesOrderRFQRoomWiseSPReportToExcel([FromBody] SalesOrderSPResportDTO rfqRoomWiseSPReport)
        {
            try
            {
                // Get data from repository using stored procedure
                var salesOrderSPReportDetails = await _repository.GetRfqSalesOrderRoomWiseSPReportWithParam(rfqRoomWiseSPReport.CustomerName, rfqRoomWiseSPReport.SalesOrderNumber, rfqRoomWiseSPReport.KPN);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("RFQRoomWiseSPReport");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Sales Order Number");
                headerRow.CreateCell(1).SetCellValue("Customer ID");
                headerRow.CreateCell(2).SetCellValue("Customer Name");
                headerRow.CreateCell(3).SetCellValue("Lead ID");
                headerRow.CreateCell(4).SetCellValue("Order Type");
                headerRow.CreateCell(5).SetCellValue("Type Of Solution");
                headerRow.CreateCell(6).SetCellValue("Product Type");
                headerRow.CreateCell(7).SetCellValue("Material Group");
                headerRow.CreateCell(8).SetCellValue("Sales Person");
                headerRow.CreateCell(9).SetCellValue("SO Date");
                headerRow.CreateCell(10).SetCellValue("RoomName");
                headerRow.CreateCell(11).SetCellValue("KPN");
                headerRow.CreateCell(12).SetCellValue("KPN Description");
                headerRow.CreateCell(13).SetCellValue("UOC");
                headerRow.CreateCell(14).SetCellValue("UOM");
                headerRow.CreateCell(15).SetCellValue("Price List");
                headerRow.CreateCell(16).SetCellValue("Unit Price");
                headerRow.CreateCell(17).SetCellValue("Basic Amount");
                headerRow.CreateCell(18).SetCellValue("DiscountType");
                headerRow.CreateCell(19).SetCellValue("Discount");
                headerRow.CreateCell(20).SetCellValue("SGST");
                headerRow.CreateCell(21).SetCellValue("CGST");
                headerRow.CreateCell(22).SetCellValue("IGST");
                headerRow.CreateCell(23).SetCellValue("UTGST");
                headerRow.CreateCell(24).SetCellValue("ItemPriceList");
                headerRow.CreateCell(25).SetCellValue("Total Amount");
                headerRow.CreateCell(26).SetCellValue("Order Qty");
                headerRow.CreateCell(27).SetCellValue("Dispatch Qty");
                headerRow.CreateCell(28).SetCellValue("Balance Qty");
                headerRow.CreateCell(29).SetCellValue("Indent Qnty");


                // Populate data rows
                int rowIndex = 1;
                foreach (var item in salesOrderSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.SalesOrderNumber);
                    row.CreateCell(1).SetCellValue(item.CustomerId);
                    row.CreateCell(2).SetCellValue(item.CustomerName);
                    row.CreateCell(3).SetCellValue(item.LeadId);
                    row.CreateCell(4).SetCellValue(item.OrderType);
                    row.CreateCell(5).SetCellValue(item.TypeOfSolution);
                    row.CreateCell(6).SetCellValue(item.ProductType);
                    row.CreateCell(7).SetCellValue(item.MaterialGroup);// Assuming ItemType is nullable int
                    row.CreateCell(8).SetCellValue(item.SalesPerson);
                    row.CreateCell(9).SetCellValue(item.sodate.HasValue ? item.sodate.Value.ToString("MM/dd/yyyy") : ""); // Assuming sodate is nullable DateTime
                    row.CreateCell(10).SetCellValue(item.RoomName);
                    row.CreateCell(11).SetCellValue(item.KPN);
                    row.CreateCell(12).SetCellValue(item.KPNDescription);
                    row.CreateCell(13).SetCellValue(item.UOC);
                    row.CreateCell(14).SetCellValue(item.UOM);
                    row.CreateCell(15).SetCellValue(item.PriceList);
                    row.CreateCell(16).SetCellValue(Convert.ToDouble(item.UnitPrice)); // Assuming UnitPrice is decimal
                    row.CreateCell(17).SetCellValue(Convert.ToDouble(item.BasicAmount)); // Assuming BasicAmount is decimal
                    row.CreateCell(18).SetCellValue(item.DiscountType);
                    row.CreateCell(19).SetCellValue(item.Discount);
                    row.CreateCell(20).SetCellValue(Convert.ToDouble(item.SGST)); // Assuming SGST is decimal
                    row.CreateCell(21).SetCellValue(Convert.ToDouble(item.CGST)); // Assuming CGST is decimal
                    row.CreateCell(22).SetCellValue(Convert.ToDouble(item.IGST)); // Assuming IGST is decimal
                    row.CreateCell(23).SetCellValue(Convert.ToDouble(item.UTGST)); // Assuming UTGST is decimal
                    row.CreateCell(24).SetCellValue(Convert.ToDouble(item.itempricelist)); // Assuming itempricelist is decimal
                    row.CreateCell(25).SetCellValue(Convert.ToDouble(item.TotalAmount)); // Assuming TotalAmount is decimal
                    row.CreateCell(26).SetCellValue(Convert.ToDouble(item.OrderQty)); // Assuming OrderQty is decimal
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.DispatchQty)); // Assuming DispatchQty is decimal
                    row.CreateCell(28).SetCellValue(Convert.ToDouble(item.BalanceQty)); // Assuming BalanceQty is decimal
                    row.CreateCell(29).SetCellValue(Convert.ToDouble(item.indent_qnty)); // Assuming BalanceQty is decimal
                }


                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RFQRoomWiseSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportSalesOrderRFQSPReportToExcelForTrans([FromBody] RfqSalesOrderSPResportDTOForTrans salesOrderSPReport)
        {
            try
            {
                // Get data from repository using stored procedure
                var salesOrderSPReportDetails = await _repository.GetRfqSalesOrderSPReportWithParamForTrans(salesOrderSPReport.CustomerName, salesOrderSPReport.SalesOrderNumber, salesOrderSPReport.KPN, salesOrderSPReport.SOStatus
                                                                                                                            , salesOrderSPReport.ProjectNumber);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("RFQSPReport");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Sales Order Number");
                headerRow.CreateCell(1).SetCellValue("SOStatus");
                headerRow.CreateCell(2).SetCellValue("Project Number");
                headerRow.CreateCell(3).SetCellValue("Customer ID");
                headerRow.CreateCell(4).SetCellValue("Customer Name");
                headerRow.CreateCell(5).SetCellValue("Lead ID");
                headerRow.CreateCell(6).SetCellValue("Order Type");
                headerRow.CreateCell(7).SetCellValue("Type Of Solution");
                headerRow.CreateCell(8).SetCellValue("Product Type");
                headerRow.CreateCell(9).SetCellValue("Material Group");
                headerRow.CreateCell(10).SetCellValue("Item Type");
                headerRow.CreateCell(11).SetCellValue("Sales Person");
                headerRow.CreateCell(12).SetCellValue("SO Date");
                headerRow.CreateCell(13).SetCellValue("KPN");
                headerRow.CreateCell(14).SetCellValue("KPN Description");
                headerRow.CreateCell(15).SetCellValue("UOC");
                headerRow.CreateCell(16).SetCellValue("UOM");
                headerRow.CreateCell(17).SetCellValue("Price List");
                headerRow.CreateCell(18).SetCellValue("Unit Price");
                headerRow.CreateCell(19).SetCellValue("Basic Amount");
                headerRow.CreateCell(20).SetCellValue("DiscountType");
                headerRow.CreateCell(21).SetCellValue("Discount");
                headerRow.CreateCell(22).SetCellValue("SGST");
                headerRow.CreateCell(23).SetCellValue("CGST");
                headerRow.CreateCell(24).SetCellValue("IGST");
                headerRow.CreateCell(25).SetCellValue("UTGST");
                headerRow.CreateCell(26).SetCellValue("ItemPriceList");
                headerRow.CreateCell(27).SetCellValue("Total Amount");
                headerRow.CreateCell(28).SetCellValue("Order Qty");
                headerRow.CreateCell(29).SetCellValue("Dispatch Qty");
                headerRow.CreateCell(30).SetCellValue("Balance Qty");
                headerRow.CreateCell(31).SetCellValue("scheduledate");
                headerRow.CreateCell(32).SetCellValue("scheduleqnty");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in salesOrderSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.SalesOrderNumber);
                    row.CreateCell(1).SetCellValue(item.SOStatus.HasValue ? Enum.GetName(typeof(OrderStatus), item.SOStatus) : ""); // Assuming SOStatus is nullable int//
                    row.CreateCell(2).SetCellValue(item.ProjectNumber ?? ""); // Assuming ProjectNumber is nullable string
                    row.CreateCell(3).SetCellValue(item.CustomerId);
                    row.CreateCell(4).SetCellValue(item.CustomerName);
                    row.CreateCell(5).SetCellValue(item.LeadId);
                    row.CreateCell(6).SetCellValue(item.OrderType);
                    row.CreateCell(7).SetCellValue(item.TypeOfSolution);
                    row.CreateCell(8).SetCellValue(item.ProductType);
                    row.CreateCell(9).SetCellValue(item.MaterialGroup);
                    row.CreateCell(10).SetCellValue(item.ItemType); // Assuming ItemType is nullable int
                    row.CreateCell(11).SetCellValue(item.SalesPerson);
                    row.CreateCell(12).SetCellValue(item.sodate.HasValue ? item.sodate.Value.ToString("MM/dd/yyyy") : ""); // Assuming sodate is nullable DateTime
                    row.CreateCell(13).SetCellValue(item.KPN);
                    row.CreateCell(14).SetCellValue(item.KPNDescription);
                    row.CreateCell(15).SetCellValue(item.UOC);
                    row.CreateCell(16).SetCellValue(item.UOM);
                    row.CreateCell(17).SetCellValue(item.PriceList);
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(item.UnitPrice)); // Assuming UnitPrice is decimal
                    row.CreateCell(19).SetCellValue(Convert.ToDouble(item.BasicAmount)); // Assuming BasicAmount is decimal
                    row.CreateCell(20).SetCellValue(item.DiscountType);
                    row.CreateCell(21).SetCellValue(item.Discount);
                    row.CreateCell(22).SetCellValue(Convert.ToDouble(item.SGST)); // Assuming SGST is decimal
                    row.CreateCell(23).SetCellValue(Convert.ToDouble(item.CGST)); // Assuming CGST is decimal
                    row.CreateCell(24).SetCellValue(Convert.ToDouble(item.IGST)); // Assuming IGST is decimal
                    row.CreateCell(25).SetCellValue(Convert.ToDouble(item.UTGST)); // Assuming UTGST is decimal
                    row.CreateCell(26).SetCellValue(Convert.ToDouble(item.itempricelist)); // Assuming itempricelist is decimal
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.TotalAmount)); // Assuming TotalAmount is decimal
                    row.CreateCell(28).SetCellValue(Convert.ToDouble(item.OrderQty)); // Assuming OrderQty is decimal
                    row.CreateCell(29).SetCellValue(Convert.ToDouble(item.DispatchQty)); // Assuming DispatchQty is decimal
                    row.CreateCell(30).SetCellValue(Convert.ToDouble(item.BalanceQty)); // Assuming BalanceQty is decimal
                    row.CreateCell(31).SetCellValue(item.scheduledate.HasValue ? item.scheduledate.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(32).SetCellValue(Convert.ToDouble(item.scheduleqnty));
                }


                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RFQSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportSalesOrderRFQSPReportToExcelForAvision([FromBody] RfqSalesOrderSPResportDTO salesOrderSPReport)
        {
            try
            {
                // Get data from repository using stored procedure
                var salesOrderSPReportDetails = await _repository.GetRfqSalesOrderSPReportWithParamForAvision(salesOrderSPReport.CustomerName, salesOrderSPReport.SalesOrderNumber, salesOrderSPReport.KPN, salesOrderSPReport.SOStatus);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("RFQSPReport");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Sales Order Number");
                headerRow.CreateCell(1).SetCellValue("SOStatus");
                headerRow.CreateCell(2).SetCellValue("Project Number");
                headerRow.CreateCell(3).SetCellValue("Customer ID");
                headerRow.CreateCell(4).SetCellValue("Customer Name");
                headerRow.CreateCell(5).SetCellValue("Lead ID");
                headerRow.CreateCell(6).SetCellValue("Order Type");
                headerRow.CreateCell(7).SetCellValue("Type Of Solution");
                headerRow.CreateCell(8).SetCellValue("Product Type");
                headerRow.CreateCell(9).SetCellValue("Material Group");
                headerRow.CreateCell(10).SetCellValue("Item Type");
                headerRow.CreateCell(11).SetCellValue("Sales Person");
                headerRow.CreateCell(12).SetCellValue("SO Date");
                headerRow.CreateCell(13).SetCellValue("KPN");
                headerRow.CreateCell(14).SetCellValue("KPN Description");
                headerRow.CreateCell(15).SetCellValue("UOC");
                headerRow.CreateCell(16).SetCellValue("UOM");
                headerRow.CreateCell(17).SetCellValue("Price List");
                headerRow.CreateCell(18).SetCellValue("Unit Price");
                headerRow.CreateCell(19).SetCellValue("Basic Amount");
                headerRow.CreateCell(20).SetCellValue("DiscountType");
                headerRow.CreateCell(21).SetCellValue("Discount");
                headerRow.CreateCell(22).SetCellValue("SGST");
                headerRow.CreateCell(23).SetCellValue("CGST");
                headerRow.CreateCell(24).SetCellValue("IGST");
                headerRow.CreateCell(25).SetCellValue("UTGST");
                headerRow.CreateCell(26).SetCellValue("ItemPriceList");
                headerRow.CreateCell(27).SetCellValue("Total Amount");
                headerRow.CreateCell(28).SetCellValue("Order Qty");
                headerRow.CreateCell(29).SetCellValue("Dispatch Qty");
                headerRow.CreateCell(30).SetCellValue("Balance Qty");
                headerRow.CreateCell(31).SetCellValue("scheduledate");
                headerRow.CreateCell(32).SetCellValue("scheduleqnty");


                // Populate data rows
                int rowIndex = 1;
                foreach (var item in salesOrderSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.SalesOrderNumber);
                    row.CreateCell(1).SetCellValue(item.SOStatus.HasValue ? Enum.GetName(typeof(OrderStatus), item.SOStatus) : ""); // Assuming SOStatus is nullable int
                    row.CreateCell(2).SetCellValue(item.ProjectNumber ?? ""); // Assuming ProjectNumber is nullable string
                    row.CreateCell(3).SetCellValue(item.CustomerId);
                    row.CreateCell(4).SetCellValue(item.CustomerName);
                    row.CreateCell(5).SetCellValue(item.LeadId);
                    row.CreateCell(6).SetCellValue(item.OrderType);
                    row.CreateCell(7).SetCellValue(item.TypeOfSolution);
                    row.CreateCell(8).SetCellValue(item.ProductType);
                    row.CreateCell(9).SetCellValue(item.MaterialGroup);
                    row.CreateCell(10).SetCellValue(item.ItemType); // Assuming ItemType is nullable int
                    row.CreateCell(11).SetCellValue(item.SalesPerson);
                    row.CreateCell(12).SetCellValue(item.sodate.HasValue ? item.sodate.Value.ToString("MM/dd/yyyy") : ""); // Assuming sodate is nullable DateTime
                    row.CreateCell(13).SetCellValue(item.KPN);
                    row.CreateCell(14).SetCellValue(item.KPNDescription);
                    row.CreateCell(15).SetCellValue(item.UOC);
                    row.CreateCell(16).SetCellValue(item.UOM);
                    row.CreateCell(17).SetCellValue(item.PriceList);
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(item.UnitPrice)); // Assuming UnitPrice is decimal
                    row.CreateCell(19).SetCellValue(Convert.ToDouble(item.BasicAmount)); // Assuming BasicAmount is decimal
                    row.CreateCell(20).SetCellValue(item.DiscountType);
                    row.CreateCell(21).SetCellValue(item.Discount);
                    row.CreateCell(22).SetCellValue(Convert.ToDouble(item.SGST)); // Assuming SGST is decimal
                    row.CreateCell(23).SetCellValue(Convert.ToDouble(item.CGST)); // Assuming CGST is decimal
                    row.CreateCell(24).SetCellValue(Convert.ToDouble(item.IGST)); // Assuming IGST is decimal
                    row.CreateCell(25).SetCellValue(Convert.ToDouble(item.UTGST)); // Assuming UTGST is decimal
                    row.CreateCell(26).SetCellValue(Convert.ToDouble(item.itempricelist)); // Assuming itempricelist is decimal
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.TotalAmount)); // Assuming TotalAmount is decimal
                    row.CreateCell(28).SetCellValue(Convert.ToDouble(item.OrderQty)); // Assuming OrderQty is decimal
                    row.CreateCell(29).SetCellValue(Convert.ToDouble(item.DispatchQty)); // Assuming DispatchQty is decimal
                    row.CreateCell(30).SetCellValue(Convert.ToDouble(item.BalanceQty)); // Assuming BalanceQty is decimal
                    row.CreateCell(31).SetCellValue(item.scheduledate.HasValue ? item.scheduledate.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(32).SetCellValue(Convert.ToDouble(item.scheduleqnty));
                }


                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RFQSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportSalesOrderForecastSPReportToExcel([FromBody] ForecastSalesOrderSPResportDTO salesOrderSPReport)
        {
            try
            {
                // Get data from repository using stored procedure
                var salesOrderSPReportDetails = await _repository.GetForecastSalesOrderSPReportWithParam(salesOrderSPReport.CustomerName, salesOrderSPReport.SalesOrderNumber, salesOrderSPReport.KPN, salesOrderSPReport.SOStatus);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("ForecastSPReport");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Sales Order Number");
                headerRow.CreateCell(1).SetCellValue("SOStatus");
                headerRow.CreateCell(2).SetCellValue("Project Number");
                headerRow.CreateCell(3).SetCellValue("Customer ID");
                headerRow.CreateCell(4).SetCellValue("Customer Name");
                headerRow.CreateCell(5).SetCellValue("Lead ID");
                headerRow.CreateCell(6).SetCellValue("Order Type");
                headerRow.CreateCell(7).SetCellValue("Type Of Solution");
                headerRow.CreateCell(8).SetCellValue("Product Type");
                headerRow.CreateCell(9).SetCellValue("Material Group");
                headerRow.CreateCell(10).SetCellValue("Item Type");
                headerRow.CreateCell(11).SetCellValue("Sales Person");
                headerRow.CreateCell(12).SetCellValue("SO Date");
                headerRow.CreateCell(13).SetCellValue("KPN");
                headerRow.CreateCell(14).SetCellValue("KPN Description");
                headerRow.CreateCell(15).SetCellValue("UOC");
                headerRow.CreateCell(16).SetCellValue("UOM");
                headerRow.CreateCell(17).SetCellValue("Price List");
                headerRow.CreateCell(18).SetCellValue("Unit Price");
                headerRow.CreateCell(19).SetCellValue("Basic Amount");
                headerRow.CreateCell(20).SetCellValue("DiscountType");
                headerRow.CreateCell(21).SetCellValue("Discount");
                headerRow.CreateCell(22).SetCellValue("SGST");
                headerRow.CreateCell(23).SetCellValue("CGST");
                headerRow.CreateCell(24).SetCellValue("IGST");
                headerRow.CreateCell(25).SetCellValue("UTGST");
                headerRow.CreateCell(26).SetCellValue("ItemPriceList");
                headerRow.CreateCell(27).SetCellValue("Total Amount");
                headerRow.CreateCell(28).SetCellValue("Order Qty");
                headerRow.CreateCell(29).SetCellValue("Dispatch Qty");
                headerRow.CreateCell(30).SetCellValue("Balance Qty");
                headerRow.CreateCell(31).SetCellValue("RequestedDate");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in salesOrderSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.SalesOrderNumber);
                    row.CreateCell(1).SetCellValue(item.SOStatus.HasValue ? Enum.GetName(typeof(OrderStatus), item.SOStatus) : ""); // Adding SOStatus
                    row.CreateCell(2).SetCellValue(item.ProjectNumber ?? ""); // Adding ProjectNumber
                    row.CreateCell(3).SetCellValue(item.CustomerId);
                    row.CreateCell(4).SetCellValue(item.CustomerName);
                    row.CreateCell(5).SetCellValue(item.LeadId);
                    row.CreateCell(6).SetCellValue(item.OrderType);
                    row.CreateCell(7).SetCellValue(item.TypeOfSolution);
                    row.CreateCell(8).SetCellValue(item.ProductType);
                    row.CreateCell(9).SetCellValue(item.MaterialGroup);
                    row.CreateCell(10).SetCellValue(item.ItemType); // Assuming ItemType is nullable int
                    row.CreateCell(11).SetCellValue(item.SalesPerson);
                    row.CreateCell(12).SetCellValue(item.sodate.HasValue ? item.sodate.Value.ToString("MM/dd/yyyy") : ""); // Assuming sodate is nullable DateTime
                    row.CreateCell(13).SetCellValue(item.KPN);
                    row.CreateCell(14).SetCellValue(item.KPNDescription);
                    row.CreateCell(15).SetCellValue(item.UOC);
                    row.CreateCell(16).SetCellValue(item.UOM);
                    row.CreateCell(17).SetCellValue(item.PriceList);
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(item.UnitPrice)); // Assuming UnitPrice is decimal
                    row.CreateCell(19).SetCellValue(Convert.ToDouble(item.BasicAmount)); // Assuming BasicAmount is decimal
                    row.CreateCell(20).SetCellValue(item.DiscountType);
                    row.CreateCell(21).SetCellValue(item.Discount);
                    row.CreateCell(22).SetCellValue(Convert.ToDouble(item.SGST)); // Assuming SGST is decimal
                    row.CreateCell(23).SetCellValue(Convert.ToDouble(item.CGST)); // Assuming CGST is decimal
                    row.CreateCell(24).SetCellValue(Convert.ToDouble(item.IGST)); // Assuming IGST is decimal
                    row.CreateCell(25).SetCellValue(Convert.ToDouble(item.UTGST)); // Assuming UTGST is decimal
                    row.CreateCell(26).SetCellValue(Convert.ToDouble(item.itempricelist)); // Assuming itempricelist is decimal
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.TotalAmount)); // Assuming TotalAmount is decimal
                    row.CreateCell(28).SetCellValue(Convert.ToDouble(item.OrderQty)); // Assuming OrderQty is decimal
                    row.CreateCell(29).SetCellValue(Convert.ToDouble(item.DispatchQty)); // Assuming DispatchQty is decimal
                    row.CreateCell(30).SetCellValue(Convert.ToDouble(item.BalanceQty)); // Assuming BalanceQty is decimal
                    row.CreateCell(31).SetCellValue(item.RequestedDate.HasValue ? item.RequestedDate.Value.ToString("MM/dd/yyyy") : "");
                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ForecastSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Return appropriate error response to the client
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportSalesOrderForecastSPReportToExcelForTrans([FromBody] ForecastSalesOrderSPResportDTOForTrans salesOrderSPReport)
        {
            try
            {
                // Get data from repository using stored procedure
                var salesOrderSPReportDetails = await _repository.GetForecastSalesOrderSPReportWithParamForTrans(salesOrderSPReport.CustomerName, salesOrderSPReport.SalesOrderNumber, salesOrderSPReport.KPN, salesOrderSPReport.SOStatus
                                                                                                                                            , salesOrderSPReport.ProjectNumber);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("ForecastSPReport");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Sales Order Number");
                headerRow.CreateCell(1).SetCellValue("SOStatus");
                headerRow.CreateCell(2).SetCellValue("Project Number");
                headerRow.CreateCell(3).SetCellValue("Customer ID");
                headerRow.CreateCell(4).SetCellValue("Customer Name");
                headerRow.CreateCell(5).SetCellValue("Lead ID");
                headerRow.CreateCell(6).SetCellValue("Order Type");
                headerRow.CreateCell(7).SetCellValue("Type Of Solution");
                headerRow.CreateCell(8).SetCellValue("Product Type");
                headerRow.CreateCell(9).SetCellValue("Material Group");
                headerRow.CreateCell(10).SetCellValue("Item Type");
                headerRow.CreateCell(11).SetCellValue("Sales Person");
                headerRow.CreateCell(12).SetCellValue("SO Date");
                headerRow.CreateCell(13).SetCellValue("KPN");
                headerRow.CreateCell(14).SetCellValue("KPN Description");
                headerRow.CreateCell(15).SetCellValue("UOC");
                headerRow.CreateCell(16).SetCellValue("UOM");
                headerRow.CreateCell(17).SetCellValue("Price List");
                headerRow.CreateCell(18).SetCellValue("Unit Price");
                headerRow.CreateCell(19).SetCellValue("Basic Amount");
                headerRow.CreateCell(20).SetCellValue("DiscountType");
                headerRow.CreateCell(21).SetCellValue("Discount");
                headerRow.CreateCell(22).SetCellValue("SGST");
                headerRow.CreateCell(23).SetCellValue("CGST");
                headerRow.CreateCell(24).SetCellValue("IGST");
                headerRow.CreateCell(25).SetCellValue("UTGST");
                headerRow.CreateCell(26).SetCellValue("ItemPriceList");
                headerRow.CreateCell(27).SetCellValue("Total Amount");
                headerRow.CreateCell(28).SetCellValue("Order Qty");
                headerRow.CreateCell(29).SetCellValue("Dispatch Qty");
                headerRow.CreateCell(30).SetCellValue("Balance Qty");
                headerRow.CreateCell(31).SetCellValue("scheduledate");
                headerRow.CreateCell(32).SetCellValue("scheduleqnty");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in salesOrderSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.SalesOrderNumber);
                    row.CreateCell(1).SetCellValue(item.SOStatus.HasValue ? Enum.GetName(typeof(OrderStatus), item.SOStatus) : ""); // Adding SOStatus
                    row.CreateCell(2).SetCellValue(item.ProjectNumber ?? ""); // Adding ProjectNumber
                    row.CreateCell(3).SetCellValue(item.CustomerId);
                    row.CreateCell(4).SetCellValue(item.CustomerName);
                    row.CreateCell(5).SetCellValue(item.LeadId);
                    row.CreateCell(6).SetCellValue(item.OrderType);
                    row.CreateCell(7).SetCellValue(item.TypeOfSolution);
                    row.CreateCell(8).SetCellValue(item.ProductType);
                    row.CreateCell(9).SetCellValue(item.MaterialGroup);
                    row.CreateCell(10).SetCellValue(item.ItemType); // Assuming ItemType is nullable int
                    row.CreateCell(11).SetCellValue(item.SalesPerson);
                    row.CreateCell(12).SetCellValue(item.sodate.HasValue ? item.sodate.Value.ToString("MM/dd/yyyy") : ""); // Assuming sodate is nullable DateTime
                    row.CreateCell(13).SetCellValue(item.KPN);
                    row.CreateCell(14).SetCellValue(item.KPNDescription);
                    row.CreateCell(15).SetCellValue(item.UOC);
                    row.CreateCell(16).SetCellValue(item.UOM);
                    row.CreateCell(17).SetCellValue(item.PriceList);
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(item.UnitPrice)); // Assuming UnitPrice is decimal
                    row.CreateCell(19).SetCellValue(Convert.ToDouble(item.BasicAmount)); // Assuming BasicAmount is decimal
                    row.CreateCell(20).SetCellValue(item.DiscountType);
                    row.CreateCell(21).SetCellValue(item.Discount);
                    row.CreateCell(22).SetCellValue(Convert.ToDouble(item.SGST)); // Assuming SGST is decimal
                    row.CreateCell(23).SetCellValue(Convert.ToDouble(item.CGST)); // Assuming CGST is decimal
                    row.CreateCell(24).SetCellValue(Convert.ToDouble(item.IGST)); // Assuming IGST is decimal
                    row.CreateCell(25).SetCellValue(Convert.ToDouble(item.UTGST)); // Assuming UTGST is decimal
                    row.CreateCell(26).SetCellValue(Convert.ToDouble(item.itempricelist)); // Assuming itempricelist is decimal
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.TotalAmount)); // Assuming TotalAmount is decimal
                    row.CreateCell(28).SetCellValue(Convert.ToDouble(item.OrderQty)); // Assuming OrderQty is decimal
                    row.CreateCell(29).SetCellValue(Convert.ToDouble(item.DispatchQty)); // Assuming DispatchQty is decimal
                    row.CreateCell(30).SetCellValue(Convert.ToDouble(item.BalanceQty)); // Assuming BalanceQty is decimal
                    row.CreateCell(31).SetCellValue(item.scheduledate.HasValue ? item.scheduledate.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(32).SetCellValue(Convert.ToDouble(item.scheduleqnty));
                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ForecastSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Return appropriate error response to the client
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportSalesOrderForecastSPReportToExcelForAvision([FromBody] ForecastSalesOrderSPResportDTO salesOrderSPReport)
        {
            try
            {
                // Get data from repository using stored procedure
                var salesOrderSPReportDetails = await _repository.GetForecastSalesOrderSPReportWithParamForAvision(salesOrderSPReport.CustomerName, salesOrderSPReport.SalesOrderNumber, salesOrderSPReport.KPN, salesOrderSPReport.SOStatus);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("ForecastSPReport");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Sales Order Number");
                headerRow.CreateCell(1).SetCellValue("SOStatus");
                headerRow.CreateCell(2).SetCellValue("Project Number");
                headerRow.CreateCell(3).SetCellValue("Customer ID");
                headerRow.CreateCell(4).SetCellValue("Customer Name");
                headerRow.CreateCell(5).SetCellValue("Lead ID");
                headerRow.CreateCell(6).SetCellValue("Order Type");
                headerRow.CreateCell(7).SetCellValue("Type Of Solution");
                headerRow.CreateCell(8).SetCellValue("Product Type");
                headerRow.CreateCell(9).SetCellValue("Material Group");
                headerRow.CreateCell(10).SetCellValue("Item Type");
                headerRow.CreateCell(11).SetCellValue("Sales Person");
                headerRow.CreateCell(12).SetCellValue("SO Date");
                headerRow.CreateCell(13).SetCellValue("KPN");
                headerRow.CreateCell(14).SetCellValue("KPN Description");
                headerRow.CreateCell(15).SetCellValue("UOC");
                headerRow.CreateCell(16).SetCellValue("UOM");
                headerRow.CreateCell(17).SetCellValue("Price List");
                headerRow.CreateCell(18).SetCellValue("Unit Price");
                headerRow.CreateCell(19).SetCellValue("Basic Amount");
                headerRow.CreateCell(20).SetCellValue("DiscountType");
                headerRow.CreateCell(21).SetCellValue("Discount");
                headerRow.CreateCell(22).SetCellValue("SGST");
                headerRow.CreateCell(23).SetCellValue("CGST");
                headerRow.CreateCell(24).SetCellValue("IGST");
                headerRow.CreateCell(25).SetCellValue("UTGST");
                headerRow.CreateCell(26).SetCellValue("ItemPriceList");
                headerRow.CreateCell(27).SetCellValue("Total Amount");
                headerRow.CreateCell(28).SetCellValue("Order Qty");
                headerRow.CreateCell(29).SetCellValue("Dispatch Qty");
                headerRow.CreateCell(30).SetCellValue("Balance Qty");
                headerRow.CreateCell(31).SetCellValue("scheduledate");
                headerRow.CreateCell(32).SetCellValue("scheduleqnty");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in salesOrderSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.SalesOrderNumber);
                    row.CreateCell(1).SetCellValue(item.SOStatus.HasValue ? Enum.GetName(typeof(OrderStatus), item.SOStatus) : "");  // Adding SOStatus
                    row.CreateCell(2).SetCellValue(item.ProjectNumber ?? ""); // Adding ProjectNumber
                    row.CreateCell(3).SetCellValue(item.CustomerId);
                    row.CreateCell(4).SetCellValue(item.CustomerName);
                    row.CreateCell(5).SetCellValue(item.LeadId);
                    row.CreateCell(6).SetCellValue(item.OrderType);
                    row.CreateCell(7).SetCellValue(item.TypeOfSolution);
                    row.CreateCell(8).SetCellValue(item.ProductType);
                    row.CreateCell(9).SetCellValue(item.MaterialGroup);
                    row.CreateCell(10).SetCellValue(item.ItemType); // Assuming ItemType is nullable int
                    row.CreateCell(11).SetCellValue(item.SalesPerson);
                    row.CreateCell(12).SetCellValue(item.sodate.HasValue ? item.sodate.Value.ToString("MM/dd/yyyy") : ""); // Assuming sodate is nullable DateTime
                    row.CreateCell(13).SetCellValue(item.KPN);
                    row.CreateCell(14).SetCellValue(item.KPNDescription);
                    row.CreateCell(15).SetCellValue(item.UOC);
                    row.CreateCell(16).SetCellValue(item.UOM);
                    row.CreateCell(17).SetCellValue(item.PriceList);
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(item.UnitPrice)); // Assuming UnitPrice is decimal
                    row.CreateCell(19).SetCellValue(Convert.ToDouble(item.BasicAmount)); // Assuming BasicAmount is decimal
                    row.CreateCell(20).SetCellValue(item.DiscountType);
                    row.CreateCell(21).SetCellValue(item.Discount);
                    row.CreateCell(22).SetCellValue(Convert.ToDouble(item.SGST)); // Assuming SGST is decimal
                    row.CreateCell(23).SetCellValue(Convert.ToDouble(item.CGST)); // Assuming CGST is decimal
                    row.CreateCell(24).SetCellValue(Convert.ToDouble(item.IGST)); // Assuming IGST is decimal
                    row.CreateCell(25).SetCellValue(Convert.ToDouble(item.UTGST)); // Assuming UTGST is decimal
                    row.CreateCell(26).SetCellValue(Convert.ToDouble(item.itempricelist)); // Assuming itempricelist is decimal
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.TotalAmount)); // Assuming TotalAmount is decimal
                    row.CreateCell(28).SetCellValue(Convert.ToDouble(item.OrderQty)); // Assuming OrderQty is decimal
                    row.CreateCell(29).SetCellValue(Convert.ToDouble(item.DispatchQty)); // Assuming DispatchQty is decimal
                    row.CreateCell(30).SetCellValue(Convert.ToDouble(item.BalanceQty)); // Assuming BalanceQty is decimal
                    row.CreateCell(31).SetCellValue(item.scheduledate.HasValue ? item.scheduledate.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(32).SetCellValue(Convert.ToDouble(item.scheduleqnty));
                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ForecastSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Return appropriate error response to the client
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetReceivableReportsWithCustomerID(string CustomerId)
        {
            ServiceResponse<IEnumerable<RecievableCustomer>> serviceResponse = new ServiceResponse<IEnumerable<RecievableCustomer>>();
            try
            {
                var products = await _repository.GetRecievableCustomersWithCustomerID(CustomerId);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned SalesOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetReceivableReportsWithCustomerID action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetSASalesOrderDetailsByItemNo(string itemNumber)
        {
            ServiceResponse<SARevisionNumber> serviceResponse = new ServiceResponse<SARevisionNumber>();
            try
            {
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                var encodedItemNumber = Uri.EscapeDataString(itemNumber);

                var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EngineeringBomAPI"],
                    $"GetAllProductionBomSAListByItemNumber?ItemNumber={encodedItemNumber}"));
                request.Headers.Add("Authorization", token);

                var saFgItemDetailsWithBomQty = await client.SendAsync(request);
                //var saFgItemDetailsWithBomQty = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"],
                //    "GetAllProductionBomSAListByItemNumber?", "ItemNumber=", itemNumber));
                var saFgItemDetailsWithBomQtyString = await saFgItemDetailsWithBomQty.Content.ReadAsStringAsync();
                dynamic saFgItemDetailsWithBomQtyData = JsonConvert.DeserializeObject(saFgItemDetailsWithBomQtyString);
                dynamic saFgItemDetailWithBomQty = saFgItemDetailsWithBomQtyData.data;

                string jsonString = JsonConvert.SerializeObject(saFgItemDetailWithBomQty.bomVersionNo);
                JArray jArray = JArray.Parse(jsonString);
                decimal[] bomVersionNo = jArray.ToObject<decimal[]>();

                string jString = JsonConvert.SerializeObject(saFgItemDetailWithBomQty.fgItemNumberWithSaBomQty);
                Dictionary<string, decimal> fgItemNumberWithSqBomQty = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(jString);
                List<string> fgItemNoList = fgItemNumberWithSqBomQty.Select(x => x.Key).ToList();

                //string jString = JsonConvert.SerializeObject(saFgItemDetailWithBomQty.fgItemNumberWithSaBomQty);
                //JArray jsonArray = JArray.Parse(jString);
                //Dictionary<string, decimal> fgItemNumberWithSqBomQty = jsonArray.ToObject<Dictionary<string, decimal>>();
                //List<string> fgItemNoList= fgItemNumberWithSqBomQty.Select(x=>x.Key).ToList();
                SARevisionNumber itemDetailsDto = new SARevisionNumber();
                List<ProjectSOSADetailDto>? projectSOSADetailDtos = new List<ProjectSOSADetailDto>();
                itemDetailsDto.ItemNumber = saFgItemDetailWithBomQty.itemNumber;
                itemDetailsDto.FGItemNumber = fgItemNoList;
                itemDetailsDto.ItemType = saFgItemDetailWithBomQty.itemType;
                itemDetailsDto.BomVersionNo = bomVersionNo;

                foreach (var fgItemNo in fgItemNumberWithSqBomQty)
                {
                    decimal BomQty = fgItemNo.Value;
                    var projectSODetails = await _repository.GetProjectDetailsBySAItemNo(fgItemNo.Key);
                    foreach (var project in projectSODetails)
                    {
                        project.SalesOrderQtyDetails = await _repository.GetSASalesOrderQtyDetailsByItemNo(fgItemNo.Key, project.ProjectNumber, BomQty);
                        // itemDetailsDto.ProjectSODetails = projectSODetails;
                        projectSOSADetailDtos.Add(project);
                    }
                }
                var allProj = projectSOSADetailDtos.DistinctBy(x => x.ProjectNumber).ToList();
                List<ProjectSOSADetailDto>? projectSOSADetailUniq = new List<ProjectSOSADetailDto>();
                foreach (var pros in projectSOSADetailDtos)
                {
                    int flag = 0;
                    foreach (var proj in projectSOSADetailUniq)
                    {
                        if (proj.ProjectNumber == pros.ProjectNumber)
                        {
                            flag = 1;
                            proj.SalesOrderQtyDetails.AddRange(pros.SalesOrderQtyDetails);
                        }
                    }
                    if (flag == 0)
                    {
                        projectSOSADetailUniq.Add(pros);
                    }
                }
                itemDetailsDto.ProjectSODetails = projectSOSADetailUniq;
                serviceResponse.Data = itemDetailsDto;
                serviceResponse.Message = "Returned all SalesOrderSADetails";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSASalesOrderDetailsByItemNo action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //get sa details
        private async Task<dynamic> GetSAQuantityFromBom(string item, string itemNumber)
        {
            var client = _clientFactory.CreateClient();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            var encodedItemNumber = Uri.EscapeDataString(itemNumber);
            var encodedItem = Uri.EscapeDataString(item);

            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EngineeringBomAPI"],
                $"GetSABomListByItemNumber?fgPartNumber={encodedItem}&saItemNumber={encodedItemNumber}"));
            request.Headers.Add("Authorization", token);

            var enggBomQtyDetails = await client.SendAsync(request);
            //var enggBomQtyDetails = await _httpClient.GetAsync(string.Concat(_config["EngineeringBomAPI"],
            //            "GetSABomListByItemNumber?", "fgPartNumber=", item, "&saItemNumber=", itemNumber));

            var enggBomQtyObjectString = await enggBomQtyDetails.Content.ReadAsStringAsync();
            dynamic enggBomQtyObjectData = JsonConvert.DeserializeObject(enggBomQtyObjectString);
            dynamic enggBomQtyObject = enggBomQtyObjectData.data;

            return enggBomQtyObject;
        }

        //receivable report 


        [HttpGet]
        public async Task<IActionResult> GetAllActiveSalesOrderIdNameList()
        {
            ServiceResponse<IEnumerable<SalesOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderIdNameListDto>>();
            try
            {
                var listOfActiveSalesOrderName = await _repository.GetAllActiveSalesOrderNameList();

                var result = _mapper.Map<IEnumerable<SalesOrderIdNameListDto>>(listOfActiveSalesOrderName);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveSalesOrderIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveSalesOrderIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrderIdNameList()
        {
            ServiceResponse<IEnumerable<SalesOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderIdNameListDto>>();
            try
            {
                var listOfSalesOrderName = await _repository.GetAllSalesOrderIdNameList();

                var result = _mapper.Map<IEnumerable<SalesOrderIdNameListDto>>(listOfSalesOrderName);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All SalesOrderIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllSalesOrderIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrderFGItemNoListByProjectNo(string projectNo)
        {
            ServiceResponse<IEnumerable<SalesOrderFGItemNumberDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderFGItemNumberDto>>();
            try
            {
                var fgItemNumberList = await _repository.GetAllSalesOrderFGItemNoListByProjectNo(projectNo);

                var result = _mapper.Map<IEnumerable<SalesOrderFGItemNumberDto>>(fgItemNumberList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All SalesOrder FGItemNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllSalesOrderFGItemNoListByProjectNo action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActivateSalesOrderApprovalStatus(string salesOrderNumber)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();

            try
            {
                var salesOrderDetails = await _repository.GetSalesOrderDetailsBySONumber(salesOrderNumber);
                if (salesOrderDetails is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrderApprovalStatus object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"SalesOrderApprovalStatus with string: {salesOrderNumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                salesOrderDetails.ApproveStatus = true;
                string result = await _repository.UpdateSalesOrder(salesOrderDetails);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "SalesOrderApprovalStatus Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateSalesOrderApprovalStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut]
        public async Task<IActionResult> ActivateSalesOrderConfirmStatus(string salesOrderNumber, DateTime confirmDate)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();

            try
            {
                var salesOrderDetails = await _repository.GetSalesOrderDetailsBySONumber(salesOrderNumber);
                if (salesOrderDetails is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SalesOrderConfirmStatus object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"SalesOrderConfirmStatus with string: {salesOrderNumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }

                salesOrderDetails.ConfirmDate = confirmDate;
                salesOrderDetails.ConfirmStatus = true;
                string result = await _repository.UpdateSalesOrder(salesOrderDetails);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "SalesOrderConfirmStatus Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateSalesOrderConfirmStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ShortCloseSalesOrderItemById(int soItemId)
        {
            ServiceResponse<SalesOrderItemsDto> serviceResponse = new ServiceResponse<SalesOrderItemsDto>();

            try
            {
                var soItemDetailBySOItemId = await _salesOrderItemsRepository.GetSOItemDetailById(soItemId);
                if (soItemDetailBySOItemId == null)
                {
                    _logger.LogError($"SalesOrderItems with soItemId: {soItemId}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderItems with soItemId hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

                soItemDetailBySOItemId.StatusEnum = OrderStatus.ShortClosed;
                soItemDetailBySOItemId.BalanceQty = 0;
                string result = await _salesOrderItemsRepository.UpdateSalesOrderItem(soItemDetailBySOItemId);
                _salesOrderItemsRepository.SaveAsync();

                //Update SalesOrder Table Status
                var soItemOpenStatuscount = await _salesOrderItemsRepository.GetSOItemOpenStatusCount(soItemDetailBySOItemId.SalesOrderId);

                if (soItemOpenStatuscount == 0)
                {
                    var salesOrderDetails = await _repository.GetSalesOrderById(soItemDetailBySOItemId.SalesOrderId);
                    salesOrderDetails.SOStatus = OrderStatus.ShortClosed;
                    await _repository.UpdateSalesOrder(salesOrderDetails);
                    _repository.SaveAsync();
                }
                else
                {
                    var salesOrderDetails = await _repository.GetSalesOrderById(soItemDetailBySOItemId.SalesOrderId);
                    salesOrderDetails.SOStatus = OrderStatus.PartiallyClosed;
                    await _repository.UpdateSalesOrder(salesOrderDetails);
                    _repository.SaveAsync();
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "SalesOrderItems Status have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CloseSOItemSatusBySOItemId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShortCloseAllSalesOrderItemBySoId(int salesOrderId)
        {
            ServiceResponse<SalesOrderItemsDto> serviceResponse = new ServiceResponse<SalesOrderItemsDto>();

            try
            {
                var salesOrderDetailBySOId = await _repository.GetSalesOrderById(salesOrderId);
                if (salesOrderDetailBySOId == null)
                {
                    _logger.LogError($"SalesOrderItems with salesOrderId: {salesOrderId}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderItems with salesOrderId hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (salesOrderDetailBySOId.SalesOrdersItems != null)
                {
                    foreach (var item in salesOrderDetailBySOId.SalesOrdersItems)
                    {
                        item.StatusEnum = OrderStatus.ShortClosed;
                        item.BalanceQty = 0;
                        string result = await _salesOrderItemsRepository.UpdateSalesOrderItem(item);
                    }

                    _salesOrderItemsRepository.SaveAsync();
                }
                else
                {
                    _logger.LogError($"SalesOrderItems is Null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrderItems with salesOrderId hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                //Update SalesOrder Table Status

                var soItemOpenStatuscount = await _salesOrderItemsRepository.GetSOItemShortCloseCount(salesOrderId);

                if (soItemOpenStatuscount != 0)
                {
                    var salesOrderDetails = await _repository.GetSalesOrderById(salesOrderId);
                    salesOrderDetails.SOStatus = OrderStatus.ShortClosed;
                    await _repository.UpdateSalesOrder(salesOrderDetails);
                    _repository.SaveAsync();
                }
                else
                {
                    _logger.LogError($"SalesOrderItems not ShortClosed.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Not All SalesOrderItems  has been ShortClosed.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "SalesOrder have been Shortclosed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ShortCloseAllSalesOrderItemById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SoConfirmationStatus(List<List<SoItemConfirmationDateDto>> soItemConfirmationDateDto)
        {
            ServiceResponse<SoItemConfirmationDateDto> serviceResponse = new ServiceResponse<SoItemConfirmationDateDto>();

            try
            {
                string serverKey = GetServerKey();
                var soid_1 = soItemConfirmationDateDto.First();
                var SoId = soid_1.First();
                var SODetails = await _repository.GetSalesOrderById(SoId.SalesOrderId);
                foreach (var soItemConfirmationDateSet in soItemConfirmationDateDto)
                {
                    if (!soItemConfirmationDateSet.Any())
                    {
                        // Skip empty sets
                        continue;
                    }

                    var firstItemInSet = soItemConfirmationDateSet.First(); // Get the first item in the set

                    var salesOrderDetailById = await _repository.GetSalesOrderById(firstItemInSet.SalesOrderId);
                    if (salesOrderDetailById == null)
                    {
                        _logger.LogError($"SalesOrder with SalesOrderId: {firstItemInSet.SalesOrderId}, hasn't been found in db.");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"SalesOrder with SalesOrderId hasn't been found.";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(serviceResponse);
                    }

                    //Updating SoConfirmationDate table
                    var soItemConfirmationDateDtoDetails = await _soConfirmationDateRepository.GetSoConfirmationDateDetailsById(firstItemInSet.SalesOrderItemsId);

                    if (soItemConfirmationDateDtoDetails.Count() == 0)
                    {
                        var soConfirmationDateDtoDetails = soItemConfirmationDateSet
                            .Select(soConfirmationDate => new SoConfirmationDate
                            {
                                ConfirmationDate = soConfirmationDate.ConfirmationDate,
                                Qty = soConfirmationDate.Qty,
                                SalesOrderItemsId = soConfirmationDate.SalesOrderItemsId
                            })
                            .ToList();

                        await _soConfirmationDateRepository.CreateSoConfirmationDateList(soConfirmationDateDtoDetails);
                        _soConfirmationDateRepository.SaveAsync();
                    }

                    else
                    {
                        await _soConfirmationDateRepository.DeleteSoConfirmationDateList(soItemConfirmationDateDtoDetails);
                        _soConfirmationDateRepository.SaveAsync();

                        var soItemConfirmationDateDtoList = await _soConfirmationDateRepository.GetSoConfirmationDateDetailsById(firstItemInSet.SalesOrderItemsId);

                        if (soItemConfirmationDateDtoList.Count() == 0)
                        {
                            var soConfirmationDateDtoDetails = soItemConfirmationDateSet
                                .Select(soConfirmationDate => new SoConfirmationDate
                                {
                                    ConfirmationDate = soConfirmationDate.ConfirmationDate,
                                    Qty = soConfirmationDate.Qty,
                                    SalesOrderItemsId = soConfirmationDate.SalesOrderItemsId
                                })
                                .ToList();

                            await _soConfirmationDateRepository.CreateSoConfirmationDateList(soConfirmationDateDtoDetails);
                            _soConfirmationDateRepository.SaveAsync();
                        }
                        else
                        {
                            serviceResponse.Data = null;
                            serviceResponse.Message = "SoConfirmationDateDetails have been Found";
                            serviceResponse.Success = true;
                            serviceResponse.StatusCode = HttpStatusCode.OK;
                            return Ok(serviceResponse);
                        }

                    }

                    //Update SoConfirmationHistory Table
                    var soItemConfirmationDateDetails = await _soConfirmationDateRepository.GetSoConfirmationDateDetailsById(firstItemInSet.SalesOrderItemsId);
                    if (soItemConfirmationDateDetails != null)
                    {
                        foreach (var soConfirmationDate in soItemConfirmationDateDetails)
                        {
                            var purchaseOrderItemDetailById = await _salesOrderItemsRepository.GetSalesOrderItemDetailsById(firstItemInSet.SalesOrderItemsId);
                            SoConfirmationDateHistory soConfirmationDateHistory = new SoConfirmationDateHistory();
                            soConfirmationDateHistory.ItemNumber = purchaseOrderItemDetailById.ItemNumber;
                            soConfirmationDateHistory.Description = purchaseOrderItemDetailById.Description;
                            soConfirmationDateHistory.SalesOrderNumber = purchaseOrderItemDetailById.SalesOrderNumber;
                            soConfirmationDateHistory.ProjectNumber = purchaseOrderItemDetailById.ProjectNumber;
                            soConfirmationDateHistory.StatusEnum = purchaseOrderItemDetailById.StatusEnum;
                            soConfirmationDateHistory.Qty = soConfirmationDate.Qty;
                            soConfirmationDateHistory.ConfirmationDate = soConfirmationDate.ConfirmationDate;
                            soConfirmationDateHistory.UOM = purchaseOrderItemDetailById.UOM;
                            soConfirmationDateHistory.Currency = purchaseOrderItemDetailById.Currency;
                            soConfirmationDateHistory.TotalAmount = purchaseOrderItemDetailById.TotalAmount;
                            soConfirmationDateHistory.BasicAmount = purchaseOrderItemDetailById.BasicAmount;
                            soConfirmationDateHistory.Discount = purchaseOrderItemDetailById.Discount;
                            soConfirmationDateHistory.RoomName = purchaseOrderItemDetailById.RoomName;
                            soConfirmationDateHistory.DiscountType = purchaseOrderItemDetailById.DiscountType;
                            soConfirmationDateHistory.UnitPrice = purchaseOrderItemDetailById.UnitPrice;
                            soConfirmationDateHistory.OrderQty = purchaseOrderItemDetailById.OrderQty;
                            soConfirmationDateHistory.BalanceQty = purchaseOrderItemDetailById.BalanceQty;
                            soConfirmationDateHistory.DispatchQty = purchaseOrderItemDetailById.DispatchQty;
                            soConfirmationDateHistory.ShopOrderQty = purchaseOrderItemDetailById.ShopOrderQty;
                            soConfirmationDateHistory.SGST = purchaseOrderItemDetailById.SGST;
                            soConfirmationDateHistory.CGST = purchaseOrderItemDetailById.CGST;
                            soConfirmationDateHistory.UTGST = purchaseOrderItemDetailById.UTGST;
                            soConfirmationDateHistory.IGST = purchaseOrderItemDetailById.IGST;
                            soConfirmationDateHistory.RequestedDate = purchaseOrderItemDetailById.RequestedDate;
                            soConfirmationDateHistory.PriceList = purchaseOrderItemDetailById.PriceList;
                            soConfirmationDateHistory.Remarks = purchaseOrderItemDetailById.Remarks;
                            soConfirmationDateHistory.CreatedBy = _createdBy;
                            soConfirmationDateHistory.CreatedOn = DateTime.Now;

                            await _soConfirmationDateHistoryRepository.CreateSoConfirmationHistory(soConfirmationDateHistory);
                        }
                        _soConfirmationDateHistoryRepository.SaveAsync();
                    }

                    //Update SoConfirmationStatus in SalesOrder Table

                    salesOrderDetailById.SoConfirmationStatus = true;
                    string result = await _repository.UpdateSalesOrder(salesOrderDetailById);
                    _repository.SaveAsync();
                }
                serviceResponse.Data = null;
                serviceResponse.Message = "SoConfirmationStatus have been Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SoConfirmationStatus action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut]
        public async Task<IActionResult> ShortCloseForSalesOrder([FromBody] SalesOrderUpdateDto salesOrderDtoUpdate)
        {
            ServiceResponse<SalesOrderItemsDto> serviceResponse = new ServiceResponse<SalesOrderItemsDto>();
            try
            {
                if (salesOrderDtoUpdate is null)
                {
                    _logger.LogError("ShortClose SalesOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShortClose SalesOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ShortClose SalesOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ShortClose SalesOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var salesOrderDetailBeforeUpdate = await _repository.GetSalesOrderById(salesOrderDtoUpdate.Id);
                var salesOrderNumber = salesOrderDetailBeforeUpdate.SalesOrderNumber;

                if (salesOrderDetailBeforeUpdate is null)
                {
                    _logger.LogError($"ShortClose SalesOrder with id: {salesOrderDtoUpdate.Id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update SalesOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var salesOrderDetails = _mapper.Map<SalesOrder>(salesOrderDtoUpdate);
                var salesOrderItemsDto = salesOrderDtoUpdate.SalesOrderItemsUpdateDtos;
                var salesAdditionalChargesDto = salesOrderDtoUpdate.SalesOrderAdditionalChargesUpdateDtos;
                var salesOrderItemsList = new List<SalesOrderItems>();

                if (salesOrderItemsDto != null && salesOrderItemsDto.Count > 0)
                {
                    for (int i = 0; i < salesOrderItemsDto.Count; i++)
                    {
                        SalesOrderItems salesOrderItemsDetail = _mapper.Map<SalesOrderItems>(salesOrderItemsDto[i]);

                        var oldSOItem = salesOrderDetailBeforeUpdate.SalesOrdersItems[i];
                        salesOrderItemsDetail.Id = oldSOItem.Id;
                        //salesOrderItemsDetail.ShortClosedQty += oldSOItem.ShortClosedQty;
                        if (salesOrderItemsDto[i].NowShortClosed == true)
                        {
                            salesOrderItemsDetail.ShortClosedQty += oldSOItem.ShortClosedQty;
                            salesOrderItemsDetail.ShortClosedBy = _createdBy;
                            salesOrderItemsDetail.ShortClosedOn = DateTime.Now;
                            SalesOrderHistory salesOrderHistory = new SalesOrderHistory();
                            salesOrderHistory.SalesOrderNumber = salesOrderDetailBeforeUpdate.SalesOrderNumber;
                            salesOrderHistory.ProjectNumber = salesOrderDetailBeforeUpdate.ProjectNumber;
                            salesOrderHistory.QuoteNumber = salesOrderDetailBeforeUpdate.QuoteNumber;
                            salesOrderHistory.OrderDate = salesOrderDetailBeforeUpdate.OrderDate;
                            salesOrderHistory.OrderType = salesOrderDetailBeforeUpdate.OrderType;
                            salesOrderHistory.CustomerName = salesOrderDetailBeforeUpdate.CustomerName;
                            salesOrderHistory.CustomerId = salesOrderDetailBeforeUpdate.CustomerId;
                            salesOrderHistory.RevisionNumber = salesOrderDetailBeforeUpdate.RevisionNumber;
                            salesOrderHistory.PONumber = salesOrderDetailBeforeUpdate.PONumber;
                            salesOrderHistory.PODate = salesOrderDetailBeforeUpdate.PODate;
                            salesOrderHistory.ReceivedDate = salesOrderDetailBeforeUpdate.ReceivedDate;
                            salesOrderHistory.BillTo = salesOrderDetailBeforeUpdate.BillTo;
                            salesOrderHistory.BillToId = salesOrderDetailBeforeUpdate.BillToId;
                            salesOrderHistory.ShipTo = salesOrderDetailBeforeUpdate.ShipTo;
                            salesOrderHistory.ShipToId = salesOrderDetailBeforeUpdate.ShipToId;
                            salesOrderHistory.PaymentTerms = salesOrderDetailBeforeUpdate.PaymentTerms;
                            salesOrderHistory.Total = salesOrderDetailBeforeUpdate.Total;
                            salesOrderHistory.Unit = salesOrderDetailBeforeUpdate.Unit;
                            salesOrderHistory.IsShortClosed = salesOrderDetailBeforeUpdate.IsShortClosed;
                            salesOrderHistory.ShortClosedBy = salesOrderItemsDetail.ShortClosedBy;
                            salesOrderHistory.ShortClosedOn = salesOrderItemsDetail.ShortClosedOn;
                            salesOrderHistory.CreatedBy = _createdBy;
                            salesOrderHistory.CreatedOn = DateTime.Now;
                            salesOrderHistory.LastModifiedBy = salesOrderDetailBeforeUpdate.LastModifiedBy;
                            salesOrderHistory.LastModifiedOn = salesOrderDetailBeforeUpdate.LastModifiedOn;
                            salesOrderHistory.ItemNumber = oldSOItem.ItemNumber;
                            salesOrderHistory.Description = oldSOItem.Description;
                            salesOrderHistory.BalanceQty = oldSOItem.BalanceQty;
                            salesOrderHistory.DispatchQty = oldSOItem.DispatchQty;
                            salesOrderHistory.ShopOrderQty = oldSOItem.ShopOrderQty;
                            salesOrderHistory.UOM = oldSOItem.UOM;
                            salesOrderHistory.Currency = oldSOItem.Currency;
                            salesOrderHistory.TotalAmount = oldSOItem.TotalAmount;
                            salesOrderHistory.BasicAmount = oldSOItem.BasicAmount;
                            salesOrderHistory.Discount = oldSOItem.Discount;
                            salesOrderHistory.UnitPrice = oldSOItem.UnitPrice;
                            salesOrderHistory.OrderQty = oldSOItem.OrderQty;
                            salesOrderHistory.SGST = oldSOItem.SGST;
                            salesOrderHistory.UTGST = oldSOItem.UTGST;
                            salesOrderHistory.CGST = oldSOItem.CGST;
                            salesOrderHistory.IGST = oldSOItem.IGST;
                            salesOrderHistory.ReceivedDate = oldSOItem.RequestedDate;
                            salesOrderHistory.ShortClosedQty = salesOrderItemsDto[i].ShortClosedQty;
                            salesOrderHistory.Remarks = "Item ShortClosed";
                            salesOrderHistory.PriceList = oldSOItem.PriceList;
                            var salesOrderHistories = _mapper.Map<SalesOrderHistory>(salesOrderHistory);
                            await _salesOrderHistory.CreateSalesOrderHistory(salesOrderHistories);

                        }
                        List<ScheduleDate>? listSch = _mapper.Map<List<ScheduleDate>>(salesOrderItemsDto[i].ScheduleDates);
                        List<SoConfirmationDate>? listCon = _mapper.Map<List<SoConfirmationDate>>(salesOrderItemsDto[i].SoConfirmationDates);

                        salesOrderItemsDetail.SalesOrderNumber = salesOrderNumber;
                        salesOrderItemsDetail.ScheduleDates = listSch;
                        salesOrderItemsDetail.SoConfirmationDates = listCon;
                        salesOrderItemsList.Add(salesOrderItemsDetail);
                    }
                }


                var salesAdditionalChargesList = new List<SalesOrderAdditionalCharges>();
                if (salesAdditionalChargesDto != null && salesAdditionalChargesDto.Count > 0)
                {
                    for (int i = 0; i < salesAdditionalChargesDto.Count; i++)
                    {
                        SalesOrderAdditionalCharges additionalChargesDetail = _mapper.Map<SalesOrderAdditionalCharges>(salesAdditionalChargesDto[i]);

                        var oldSOAddCharges = salesOrderDetailBeforeUpdate.SalesOrderAdditionalCharges[i];

                        additionalChargesDetail.Id = oldSOAddCharges.Id;

                        salesAdditionalChargesList.Add(additionalChargesDetail);
                    }
                }

                //ShortClose History Table
                await CreateShortCloseSalesOrderHistory(salesOrderDetailBeforeUpdate, salesOrderDtoUpdate);

                var updateData = _mapper.Map(salesOrderDtoUpdate, salesOrderDetailBeforeUpdate);

                updateData.SalesOrdersItems = salesOrderItemsList;
                updateData.SalesOrderAdditionalCharges = salesAdditionalChargesList;
                string result = await _repository.UpdateSalesOrderShortClose(updateData);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                _salesOrderHistory.SaveAsync();


                serviceResponse.Data = null;
                serviceResponse.Message = " SalesOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateSalesOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        private async Task<IActionResult> CreateShortCloseSalesOrderHistory(SalesOrder salesOrder, SalesOrderUpdateDto salesOrderDtoUpdate)
        {
            ServiceResponse<SalesOrderMainLevelHistory> serviceResponse = new ServiceResponse<SalesOrderMainLevelHistory>();
            try
            {
                if (salesOrderDtoUpdate.NowShortClosed != true)
                {
                    var exsitingSalesOrderHistory = await _salesOrderMainLevelHistoryRepository.GetSalesOrderMainLevelHistoryBySalesOrderIdAndRevNo(salesOrder.Id, salesOrder.RevisionNumber);
                    if (exsitingSalesOrderHistory == null)
                    {
                        var salesOrderMainLevelHistory = _mapper.Map<SalesOrderMainLevelHistory>(salesOrder);
                        salesOrderMainLevelHistory.Id = 0;
                        salesOrderMainLevelHistory.SalesOrderId = salesOrder.Id;
                        salesOrderMainLevelHistory.IsShortClosed = true;
                        salesOrderMainLevelHistory.CreatedBy = _createdBy;
                        salesOrderMainLevelHistory.CreatedOn = DateTime.Now;
                        salesOrderMainLevelHistory.LastModifiedBy = null;
                        salesOrderMainLevelHistory.LastModifiedOn = null;

                        var salesOrderItems = salesOrder.SalesOrdersItems;
                        var SalesOrderItemLevelHistoryList = new List<SalesOrderItemLevelHistory>();
                        var SalesOrderScheduleDateHistoryList = new List<SalesOrderScheduleDateHistory>();
                        var SOAdditionalChargesHistoryList = new List<SOAdditionalChargesHistory>();

                        if (salesOrder.SalesOrderAdditionalCharges != null && salesOrder.SalesOrderAdditionalCharges.Count > 0)
                        {
                            foreach (var SalesOrderAdditionalCharges in salesOrder.SalesOrderAdditionalCharges)
                            {
                                SOAdditionalChargesHistory soAdditionalChargesHistory = _mapper.Map<SOAdditionalChargesHistory>(SalesOrderAdditionalCharges);
                                soAdditionalChargesHistory.Id = 0;
                                soAdditionalChargesHistory.SOAdditionalChargeId = SalesOrderAdditionalCharges.Id;

                                SOAdditionalChargesHistoryList.Add(soAdditionalChargesHistory);
                            }
                        }

                        if (salesOrderItems != null && salesOrderItems.Count > 0)
                        {
                            for (int i = 0; i < salesOrderItems.Count; i++)
                            {
                                if (salesOrderDtoUpdate.SalesOrderItemsUpdateDtos[i].NowShortClosed == true)
                                {
                                    SalesOrderItemLevelHistory salesOrderItemLevelHistory = _mapper.Map<SalesOrderItemLevelHistory>(salesOrderItems[i]);
                                    salesOrderItemLevelHistory.Id = 0;
                                    salesOrderItemLevelHistory.SalesOrderItemId = salesOrderItems[i].Id;
                                    salesOrderItemLevelHistory.RevisionNumber = salesOrder.RevisionNumber;
                                    salesOrderItemLevelHistory.ShortClosedQty = salesOrderDtoUpdate.SalesOrderItemsUpdateDtos[i].ShortClosedQty;
                                    salesOrderItemLevelHistory.StatusEnum = salesOrderDtoUpdate.SalesOrderItemsUpdateDtos[i].StatusEnum;
                                    salesOrderItemLevelHistory.ShortClosedBy = _createdBy;
                                    salesOrderItemLevelHistory.ShortClosedOn = DateAndTime.Now;
                                    salesOrderItemLevelHistory.Remarks = "Item ShortClosed";

                                    if (salesOrderItems[i].ScheduleDates != null && salesOrderItems[i].ScheduleDates.Count > 0)
                                    {
                                        foreach (var ScheduleDate in salesOrderItems[i].ScheduleDates)
                                        {
                                            SalesOrderScheduleDateHistory salesOrderScheduleDateHistory = _mapper.Map<SalesOrderScheduleDateHistory>(ScheduleDate);
                                            salesOrderScheduleDateHistory.Id = 0;
                                            salesOrderScheduleDateHistory.SOScheduleDateId = ScheduleDate.Id;
                                            SalesOrderScheduleDateHistoryList.Add(salesOrderScheduleDateHistory);
                                        }
                                    }
                                    salesOrderItemLevelHistory.SalesOrderScheduleDateHistory = SalesOrderScheduleDateHistoryList;
                                    SalesOrderItemLevelHistoryList.Add(salesOrderItemLevelHistory);
                                }
                            }
                        }

                        salesOrderMainLevelHistory.SalesOrderItemLevelHistory = SalesOrderItemLevelHistoryList;
                        salesOrderMainLevelHistory.SOAdditionalChargesHistory = SOAdditionalChargesHistoryList;

                        await _salesOrderMainLevelHistoryRepository.CreateSalesOrderMainLevelHistory(salesOrderMainLevelHistory);
                        _salesOrderMainLevelHistoryRepository.SaveAsync();
                    }
                    else
                    {
                        var salesOrderMainLevelHistoryId = await _salesOrderMainLevelHistoryRepository.GetSalesOrderMainLevelHistoryIdBySalesOrderIdAndRevNo(salesOrder.Id, salesOrder.RevisionNumber);


                        var salesOrderItems = salesOrder.SalesOrdersItems;
                        var SalesOrderItemLevelHistoryList = new List<SalesOrderItemLevelHistory>();
                        var SalesOrderScheduleDateHistoryList = new List<SalesOrderScheduleDateHistory>();


                        if (salesOrderItems != null && salesOrderItems.Count > 0)
                        {
                            for (int i = 0; i < salesOrderItems.Count; i++)
                            {
                                if (salesOrderDtoUpdate.SalesOrderItemsUpdateDtos[i].NowShortClosed == true)
                                {
                                    var exsitingSalesOrderItemLevelHistory = await _salesOrderItemLevelHistoryRepository.GetShortCloseSalesOrderItemLevelHistoryBySalesOrderItemIdAndRevNo(salesOrderItems[i].Id, salesOrder.RevisionNumber);
                                    if (exsitingSalesOrderItemLevelHistory != null)
                                    {
                                        var salesOrderItemLevelHistoryId = await _salesOrderItemLevelHistoryRepository.GetShortCloseSalesOrderItemLevelHistoryIdBySalesOrderItemIdAndRevNo(salesOrderItems[i].Id, salesOrder.RevisionNumber);

                                        var salesOrderItemLevelHistory = _mapper.Map(salesOrderItems[i], exsitingSalesOrderItemLevelHistory);
                                        salesOrderItemLevelHistory.Id = salesOrderItemLevelHistoryId;
                                        salesOrderItemLevelHistory.SalesOrderItemId = salesOrderItems[i].Id;
                                        salesOrderItemLevelHistory.ShortClosedQty += salesOrderDtoUpdate.SalesOrderItemsUpdateDtos[i].ShortClosedQty;
                                        salesOrderItemLevelHistory.StatusEnum = salesOrderDtoUpdate.SalesOrderItemsUpdateDtos[i].StatusEnum;
                                        salesOrderItemLevelHistory.ShortClosedBy = _createdBy;
                                        salesOrderItemLevelHistory.ShortClosedOn = DateAndTime.Now;
                                        salesOrderItemLevelHistory.Remarks = "Item ShortClosed";
                                        await _salesOrderItemLevelHistoryRepository.UpdateSalesOrderItemLevelHistory(salesOrderItemLevelHistory);
                                        _salesOrderItemLevelHistoryRepository.SaveAsync();

                                    }
                                    else
                                    {
                                        var salesOrderItemLevelHistory = _mapper.Map<SalesOrderItemLevelHistory>(salesOrderItems[i]);
                                        salesOrderItemLevelHistory.Id = 0;
                                        salesOrderItemLevelHistory.SalesOrderItemId = salesOrderItems[i].Id;
                                        salesOrderItemLevelHistory.RevisionNumber = salesOrder.RevisionNumber;
                                        salesOrderItemLevelHistory.ShortClosedQty = salesOrderDtoUpdate.SalesOrderItemsUpdateDtos[i].ShortClosedQty;
                                        salesOrderItemLevelHistory.StatusEnum = salesOrderDtoUpdate.SalesOrderItemsUpdateDtos[i].StatusEnum;
                                        salesOrderItemLevelHistory.ShortClosedBy = _createdBy;
                                        salesOrderItemLevelHistory.ShortClosedOn = DateAndTime.Now;
                                        salesOrderItemLevelHistory.Remarks = "Item ShortClosed";
                                        salesOrderItemLevelHistory.SalesOrderMainLevelHistoryId = salesOrderMainLevelHistoryId;

                                        if (salesOrderItems[i].ScheduleDates != null && salesOrderItems[i].ScheduleDates.Count > 0)
                                        {
                                            foreach (var ScheduleDate in salesOrderItems[i].ScheduleDates)
                                            {
                                                SalesOrderScheduleDateHistory salesOrderScheduleDateHistory = _mapper.Map<SalesOrderScheduleDateHistory>(ScheduleDate);
                                                salesOrderScheduleDateHistory.Id = 0;
                                                salesOrderScheduleDateHistory.SOScheduleDateId = ScheduleDate.Id;
                                                SalesOrderScheduleDateHistoryList.Add(salesOrderScheduleDateHistory);
                                            }
                                        }
                                        salesOrderItemLevelHistory.SalesOrderScheduleDateHistory = SalesOrderScheduleDateHistoryList;

                                        await _salesOrderItemLevelHistoryRepository.CreateSalesOrderItemLevelHistory(salesOrderItemLevelHistory);
                                        _salesOrderItemLevelHistoryRepository.SaveAsync();
                                    }


                                }
                            }
                        }
                    }
                }
                else
                {
                    var exsitingSalesOrderHistory = await _salesOrderMainLevelHistoryRepository.GetSalesOrderMainLevelHistoryBySalesOrderIdAndRevNo(salesOrder.Id, salesOrder.RevisionNumber);
                    if (exsitingSalesOrderHistory == null)
                    {
                        var salesOrderMainLevelHistory = _mapper.Map<SalesOrderMainLevelHistory>(salesOrderDtoUpdate);
                        salesOrderMainLevelHistory.Id = 0;
                        salesOrderMainLevelHistory.SalesOrderId = salesOrder.Id;
                        salesOrderMainLevelHistory.SalesOrderNumber = salesOrder.SalesOrderNumber;
                        salesOrderMainLevelHistory.RevisionNumber = salesOrder.RevisionNumber;
                        salesOrderMainLevelHistory.IsShortClosed = true;
                        salesOrderMainLevelHistory.CreatedBy = _createdBy;
                        salesOrderMainLevelHistory.CreatedOn = DateTime.Now;
                        salesOrderMainLevelHistory.LastModifiedBy = null;
                        salesOrderMainLevelHistory.LastModifiedOn = null;

                        var salesOrderItems = salesOrderDtoUpdate.SalesOrderItemsUpdateDtos;
                        var salesOrderItemDetails = salesOrder.SalesOrdersItems;
                        var SalesOrderItemLevelHistoryList = new List<SalesOrderItemLevelHistory>();
                        var SalesOrderScheduleDateHistoryList = new List<SalesOrderScheduleDateHistory>();
                        var SOAdditionalChargesHistoryList = new List<SOAdditionalChargesHistory>();

                        if (salesOrderDtoUpdate.SalesOrderAdditionalChargesUpdateDtos != null && salesOrderDtoUpdate.SalesOrderAdditionalChargesUpdateDtos.Count > 0)
                        {
                            for (int i = 0; i < salesOrderDtoUpdate.SalesOrderAdditionalChargesUpdateDtos.Count; i++)
                            {
                                SOAdditionalChargesHistory soAdditionalChargesHistory = _mapper.Map<SOAdditionalChargesHistory>(salesOrderDtoUpdate.SalesOrderAdditionalChargesUpdateDtos[i]);
                                soAdditionalChargesHistory.Id = 0;
                                soAdditionalChargesHistory.SOAdditionalChargeId = salesOrder.SalesOrderAdditionalCharges[i].Id;

                                SOAdditionalChargesHistoryList.Add(soAdditionalChargesHistory);
                            }
                        }

                        if (salesOrderItems != null && salesOrderItems.Count > 0)
                        {
                            for (int i = 0; i < salesOrderItems.Count; i++)
                            {
                                if (salesOrderItems[i].NowShortClosed == true)
                                {
                                    SalesOrderItemLevelHistory salesOrderItemLevelHistory = _mapper.Map<SalesOrderItemLevelHistory>(salesOrderItems[i]);
                                    salesOrderItemLevelHistory.Id = 0;
                                    salesOrderItemLevelHistory.SalesOrderItemId = salesOrder.SalesOrdersItems[i].Id;
                                    salesOrderItemLevelHistory.SalesOrderNumber = salesOrder.SalesOrderNumber;
                                    salesOrderItemLevelHistory.RevisionNumber = salesOrder.RevisionNumber;
                                    //salesOrderItemLevelHistory.ShortClosedQty = salesOrderDtoUpdate.SalesOrderItemsUpdateDtos[i].ShortClosedQty;
                                    salesOrderItemLevelHistory.ShortClosedBy = _createdBy;
                                    salesOrderItemLevelHistory.ShortClosedOn = DateAndTime.Now;
                                    salesOrderItemLevelHistory.Remarks = "Item ShortClosed";

                                    if (salesOrderItems[i].ScheduleDates != null && salesOrderItems[i].ScheduleDates.Count > 0)
                                    {
                                        for (int j = 0; j < salesOrderItems[i].ScheduleDates.Count; j++)
                                        {
                                            SalesOrderScheduleDateHistory salesOrderScheduleDateHistory = _mapper.Map<SalesOrderScheduleDateHistory>(salesOrderItems[i].ScheduleDates);
                                            salesOrderScheduleDateHistory.Id = 0;
                                            salesOrderScheduleDateHistory.SOScheduleDateId = salesOrderItemDetails[i].ScheduleDates[j].Id;
                                            SalesOrderScheduleDateHistoryList.Add(salesOrderScheduleDateHistory);
                                        }
                                    }
                                    salesOrderItemLevelHistory.SalesOrderScheduleDateHistory = SalesOrderScheduleDateHistoryList;
                                    SalesOrderItemLevelHistoryList.Add(salesOrderItemLevelHistory);
                                }
                            }
                        }

                        salesOrderMainLevelHistory.SalesOrderItemLevelHistory = SalesOrderItemLevelHistoryList;
                        salesOrderMainLevelHistory.SOAdditionalChargesHistory = SOAdditionalChargesHistoryList;

                        await _salesOrderMainLevelHistoryRepository.CreateSalesOrderMainLevelHistory(salesOrderMainLevelHistory);
                        _salesOrderMainLevelHistoryRepository.SaveAsync();
                    }
                    else
                    {
                        var salesOrderMainLevelHistoryId = await _salesOrderMainLevelHistoryRepository.GetSalesOrderMainLevelHistoryIdBySalesOrderIdAndRevNo(salesOrder.Id, salesOrder.RevisionNumber);

                        var salesOrderMainLevelHistory = _mapper.Map(salesOrderDtoUpdate, exsitingSalesOrderHistory);
                        salesOrderMainLevelHistory.Id = salesOrderMainLevelHistoryId;
                        salesOrderMainLevelHistory.SalesOrderId = salesOrder.Id;
                        salesOrderMainLevelHistory.SalesOrderNumber = salesOrder.SalesOrderNumber;
                        salesOrderMainLevelHistory.RevisionNumber = salesOrder.RevisionNumber;
                        salesOrderMainLevelHistory.LastModifiedBy = _createdBy;
                        salesOrderMainLevelHistory.LastModifiedOn = DateTime.Now;

                        var salesOrderItems = salesOrderDtoUpdate.SalesOrderItemsUpdateDtos;
                        var salesOrderItemDetails = salesOrder.SalesOrdersItems;
                        var SalesOrderItemLevelHistoryList = new List<SalesOrderItemLevelHistory>();
                        var SalesOrderScheduleDateHistoryList = new List<SalesOrderScheduleDateHistory>();
                        var SOAdditionalChargesHistoryList = new List<SOAdditionalChargesHistory>();

                        if (salesOrderDtoUpdate.SalesOrderAdditionalChargesUpdateDtos != null && salesOrderDtoUpdate.SalesOrderAdditionalChargesUpdateDtos.Count > 0)
                        {
                            for (int i = 0; i < salesOrderDtoUpdate.SalesOrderAdditionalChargesUpdateDtos.Count; i++)
                            {
                                SOAdditionalChargesHistory soAdditionalChargesHistory = _mapper.Map<SOAdditionalChargesHistory>(salesOrderDtoUpdate.SalesOrderAdditionalChargesUpdateDtos[i]);
                                soAdditionalChargesHistory.Id = 0;
                                soAdditionalChargesHistory.SOAdditionalChargeId = salesOrder.SalesOrderAdditionalCharges[i].Id;

                                SOAdditionalChargesHistoryList.Add(soAdditionalChargesHistory);
                            }
                        }

                        if (salesOrderItems != null && salesOrderItems.Count > 0)
                        {
                            for (int i = 0; i < salesOrderItems.Count; i++)
                            {

                                var exsitingSalesOrderItemLevelHistory = await _salesOrderItemLevelHistoryRepository.GetShortCloseSalesOrderItemLevelHistoryBySalesOrderItemIdAndRevNo(salesOrderItemDetails[i].Id, salesOrder.RevisionNumber);
                                if (exsitingSalesOrderItemLevelHistory != null)
                                {
                                    var salesOrderItemLevelHistoryId = await _salesOrderItemLevelHistoryRepository.GetShortCloseSalesOrderItemLevelHistoryBySalesOrderItemIdAndRevNo(salesOrderItemDetails[i].Id, salesOrder.RevisionNumber);

                                    var salesOrderItemLevelHistory = _mapper.Map(salesOrderItems[i], exsitingSalesOrderItemLevelHistory);
                                    salesOrderItemLevelHistory.Id = salesOrderItemLevelHistoryId.Id;
                                    salesOrderItemLevelHistory.SalesOrderItemId = salesOrderItemDetails[i].Id;
                                    salesOrderItemLevelHistory.SalesOrderNumber = salesOrder.SalesOrderNumber;
                                    salesOrderItemLevelHistory.RevisionNumber = salesOrder.RevisionNumber;
                                    salesOrderItemLevelHistory.ShortClosedQty += salesOrderItemDetails[i].ShortClosedQty;
                                    salesOrderItemLevelHistory.ShortClosedBy = _createdBy;
                                    salesOrderItemLevelHistory.ShortClosedOn = DateAndTime.Now;
                                    salesOrderItemLevelHistory.Remarks = "Item ShortClosed";
                                    SalesOrderItemLevelHistoryList.Add(salesOrderItemLevelHistory);
                                    salesOrderMainLevelHistory.SalesOrderItemLevelHistory = SalesOrderItemLevelHistoryList;

                                }
                                else
                                {
                                    var salesOrderItemLevelHistory = _mapper.Map<SalesOrderItemLevelHistory>(salesOrderItems[i]);
                                    salesOrderItemLevelHistory.Id = 0;
                                    salesOrderItemLevelHistory.SalesOrderItemId = salesOrderItemDetails[i].Id;
                                    salesOrderItemLevelHistory.SalesOrderNumber = salesOrder.SalesOrderNumber;
                                    salesOrderItemLevelHistory.RevisionNumber = salesOrder.RevisionNumber;
                                    salesOrderItemLevelHistory.ShortClosedBy = _createdBy;
                                    salesOrderItemLevelHistory.ShortClosedOn = DateTime.Now;
                                    salesOrderItemLevelHistory.Remarks = "Item ShortClosed";
                                    salesOrderItemLevelHistory.SalesOrderMainLevelHistoryId = salesOrderMainLevelHistoryId;

                                    if (salesOrderItems[i].ScheduleDates != null && salesOrderItems[i].ScheduleDates.Count > 0)
                                    {
                                        for (int j = 0; j < salesOrderItems[i].ScheduleDates.Count; j++)
                                        {
                                            var salesOrderScheduleDateHistory = _mapper.Map<SalesOrderScheduleDateHistory>(salesOrderItems[i].ScheduleDates[j]);
                                            salesOrderScheduleDateHistory.Id = 0;
                                            salesOrderScheduleDateHistory.SOScheduleDateId = salesOrderItemDetails[i].ScheduleDates[j].Id;
                                            SalesOrderScheduleDateHistoryList.Add(salesOrderScheduleDateHistory);
                                        }
                                    }

                                    salesOrderItemLevelHistory.SalesOrderScheduleDateHistory = SalesOrderScheduleDateHistoryList;

                                    await _salesOrderItemLevelHistoryRepository.CreateSalesOrderItemLevelHistory(salesOrderItemLevelHistory);
                                    _salesOrderItemLevelHistoryRepository.SaveAsync();


                                }

                            }
                        }

                        salesOrderMainLevelHistory.SOAdditionalChargesHistory = SOAdditionalChargesHistoryList;
                        await _salesOrderMainLevelHistoryRepository.UpdateSalesOrderMainLevelHistory(salesOrderMainLevelHistory);
                        _salesOrderMainLevelHistoryRepository.SaveAsync();
                    }
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "ShortClose SalesOrderHistory Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);


            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateShortCloseSalesOrderHistory action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderByIdExcepetClosedORShortClosed(int id)
        {
            ServiceResponse<SalesOrderDto> serviceResponse = new ServiceResponse<SalesOrderDto>();
            try
            {
                var salesOrderById = await _repository.GetSalesOrderById(id);
                string serverKey = GetServerKey();
                if (salesOrderById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SalesOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SalesOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    SalesOrderDto salesOrderDto = _mapper.Map<SalesOrderDto>(salesOrderById);

                    var quoteDetails = await _quoteRepository.GetQuoteByQuoteNumber(salesOrderDto.QuoteNumber);
                    if (quoteDetails != null)
                    {
                        salesOrderDto.QuoteRef = quoteDetails.QuoteRef;
                    }

                    List<SalesOrderItemsDto> salesOrderItemsDtoList = new List<SalesOrderItemsDto>();
                    var salesAdditionalChargesDto = salesOrderDto.SalesOrderAdditionalCharges;

                    var salesAdditionalChargesList = new List<SalesOrderAdditionalChargesDto>();
                    if (salesAdditionalChargesDto != null)
                    {
                        for (int i = 0; i < salesAdditionalChargesDto.Count; i++)
                        {
                            SalesOrderAdditionalChargesDto additionalChargesDetails = _mapper.Map<SalesOrderAdditionalChargesDto>(salesAdditionalChargesDto[i]);
                            if (additionalChargesDetails.SOAdditionalStatus != SoStatus.Closed)
                            {
                                salesAdditionalChargesList.Add(additionalChargesDetails);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    string salesOrderNo = salesOrderDto.SalesOrderNumber;
                    SalesOrderStatus salesOrderStatus1 = salesOrderDto.SalesOrderStatus;
                    //int salesOrderStatus = 1;
                    int salesOrderStatus = (int)salesOrderStatus1;
                    if (serverKey == "keus")
                    {
                        List<string> itemNumberList = salesOrderById?.SalesOrdersItems?.Where(x => x.StatusEnum != OrderStatus.ShortClosed && x.StatusEnum != OrderStatus.Closed).Select(x => x.ItemNumber).Distinct().ToList();
                        if (itemNumberList != null)
                        {
                            var json = JsonConvert.SerializeObject(itemNumberList);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            // var inventoryQtyResponse = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "GetAvailableStockQtyForSalesOrderItems?", "salesOrderNo=", salesOrderNo, "&salesOrderStatus=", salesOrderStatus), data);
                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();
                            var encodedSONumber = Uri.EscapeDataString(salesOrderNo);
                            var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                            $"GetAvailableStockQtyForSalesOrderItems?salesOrderNo={encodedSONumber}&salesOrderStatus={salesOrderStatus}"))
                            {
                                Content = data
                            };
                            request.Headers.Add("Authorization", token);

                            var inventoryQtyResponse = await client.SendAsync(request);
                            var inventoryItemQtyDetails = await inventoryQtyResponse.Content.ReadAsStringAsync();
                            Dictionary<string, decimal> inventoryItemWithStockDetails = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(inventoryItemQtyDetails);


                            foreach (var salesOrderItemDetails in salesOrderById.SalesOrdersItems.Where(x => x.StatusEnum != OrderStatus.ShortClosed && x.StatusEnum != OrderStatus.Closed))
                            {
                                SalesOrderItemsDto salesOrderItemsDtos = _mapper.Map<SalesOrderItemsDto>(salesOrderItemDetails);
                                salesOrderItemsDtos.ScheduleDates = _mapper.Map<List<ScheduleDateDto>>(salesOrderItemDetails.ScheduleDates);
                                salesOrderItemsDtos.SoConfirmationDates = _mapper.Map<List<SoConfirmationDateDto>>(salesOrderItemDetails.SoConfirmationDates);
                                string itemNumber = salesOrderItemsDtos.ItemNumber;
                                if (inventoryItemWithStockDetails.ContainsKey(itemNumber))
                                {
                                    salesOrderItemsDtos.AvailableStock = inventoryItemWithStockDetails[itemNumber];
                                }
                                else
                                {
                                    salesOrderItemsDtos.AvailableStock = 0;
                                }


                                salesOrderItemsDtoList.Add(salesOrderItemsDtos);
                            }
                        }
                    }
                    else
                    {
                        //List<(string, string)> itemNumberList = salesOrderById?.SalesOrdersItems?.Where(x => x.StatusEnum != OrderStatus.ShortClosed || x.StatusEnum != OrderStatus.Closed).Select(x => (x.ItemNumber, x.ProjectNumber)).Distinct().ToList();

                        foreach (var salesOrderItemDetails in salesOrderById.SalesOrdersItems.Where(x => x.StatusEnum != OrderStatus.ShortClosed && x.StatusEnum != OrderStatus.Closed))
                        {
                            SalesOrderItemsDto salesOrderItemsDtos = _mapper.Map<SalesOrderItemsDto>(salesOrderItemDetails);
                            salesOrderItemsDtos.ScheduleDates = _mapper.Map<List<ScheduleDateDto>>(salesOrderItemDetails.ScheduleDates);
                            salesOrderItemsDtos.SoConfirmationDates = _mapper.Map<List<SoConfirmationDateDto>>(salesOrderItemDetails.SoConfirmationDates);
                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();
                            var itemNumber = salesOrderItemsDtos.ItemNumber;
                            var encodedItemNumber = Uri.EscapeDataString(itemNumber);
                            var projectNo = salesOrderItemsDtos.ProjectNumber;
                            var encodedProjectNo = Uri.EscapeDataString(projectNo);
                            //var encodedProjectNo=Uri.EscapeDataString(itemNumberList.Where(x => x.Item1 == itemNumber).Select(x => x.Item2).ToString());
                            //var encodedProjectNo = Uri.EscapeDataString(
                            //                         string.Join(",", itemNumberList
                            //                        .Where(x => x.Item1 == itemNumber)
                            //                        .Select(x => x.Item2)));

                            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryDetailsByItemNo?itemNumber={encodedItemNumber}&projectNo={encodedProjectNo}"));
                            request.Headers.Add("Authorization", token);

                            var inventoryQtyResponse = await client.SendAsync(request);
                            //var inventoryQtyResponse = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"], "GetInventoryDetailsByItemNo?", "itemNumber=", itemNumber, "&projectNo=", itemNumberList.Where(x=>x.Item1==itemNumber).Select(x=>x.Item2)));
                            var inventoryItemQtyDetails = await inventoryQtyResponse.Content.ReadAsStringAsync();
                            var inventoryItemWithStockDetails = JsonConvert.DeserializeObject<InventoryItemdetailsDto>(inventoryItemQtyDetails);
                            if (inventoryItemWithStockDetails != null)
                            {
                                salesOrderItemsDtos.AvailableStock = inventoryItemWithStockDetails.data.Sum(x => x.balance_Quantity);
                            }
                            else
                            {
                                salesOrderItemsDtos.AvailableStock = 0;
                            }
                            salesOrderItemsDtoList.Add(salesOrderItemsDtos);
                        }
                    }



                    salesOrderDto.SalesOrdersItems = salesOrderItemsDtoList;
                    salesOrderDto.SalesOrderAdditionalCharges = salesAdditionalChargesList;
                    serviceResponse.Data = salesOrderDto;
                    serviceResponse.Message = $"Returned SalesOrder with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalesOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> SendEmailandWhatsAppMessageforSalesOrder([FromBody] SalesOrderEmailPostDto salesOrderEmailPostDto)
        {
            ServiceResponse<SOEmailMessageSuccessMessage> serviceResponse = new ServiceResponse<SOEmailMessageSuccessMessage>();
            try
            {
                if (salesOrderEmailPostDto.WhatsAppPhoneNos.IsNullOrEmpty())
                {
                    _logger.LogError($"The WhatsApp Numbers is Empty these are required");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(400, serviceResponse);
                }
                var whatsappNumbers = salesOrderEmailPostDto.WhatsAppPhoneNos.Split(',').ToList();

                var salesorderDetails = await _repository.GetSalesOrderById(salesOrderEmailPostDto.SalesOrderid);
                string? whatsapptemplate;
                string? FileName;
                string? emaildetails;
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();

                if (salesorderDetails.TypeOfSolution == "Automation" || salesorderDetails.TypeOfSolution == "Upsell - Automation")
                {
                    emaildetails = $"Your Confirmed Keus Automation Sales Order - {salesorderDetails.SalesOrderNumber}: Version - {salesorderDetails.RevisionNumber}";
                    FileName = "SalesOrder_Automation_Book";
                    whatsapptemplate = "advait_sale_closed_automation";
                }
                else if (salesorderDetails.TypeOfSolution == "Accessories" || salesorderDetails.TypeOfSolution == "Lock")
                {
                    emaildetails = $"Your Confirmed Keus Accessories Sales Order - {salesorderDetails.SalesOrderNumber}: Version - {salesorderDetails.RevisionNumber}";
                    whatsapptemplate = "advait_quote_automaiton";
                    FileName = "SalesOrder_Accessories_Book";
                }
                else
                {
                    emaildetails = $"Your Confirmed Keus Lights Sales Order - {salesorderDetails.SalesOrderNumber}: Version - {salesorderDetails.RevisionNumber}";
                    whatsapptemplate = "advait_saleclosed_light";
                    FileName = "SalesOrder_Lights_Book";
                }
                if (emaildetails.IsNullOrEmpty())
                {
                    _logger.LogError($"The Subject of the Email is Empty as the Type of Solution has not matched any Type Of Solution");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                if (whatsapptemplate.IsNullOrEmpty())
                {
                    _logger.LogError($"The Template for Whatsapp is Empty as the Type of Solution has not matched any Type Of Solution");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                var Operations = "From";
                var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                request1.Headers.Add("Authorization", token);
                var response1 = await client.SendAsync(request1);
                var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                var emaildetails1 = JsonConvert.DeserializeObject<SalesEmailIDsDto>(EmailTempString1);


                var httpclientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(httpclientHandler);
                var mails = salesOrderEmailPostDto.SentTo.Split(',').ToList();
                if (!salesOrderEmailPostDto.CusEmail.IsNullOrEmpty())
                {
                    mails.Add(salesOrderEmailPostDto.CusEmail);
                }
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));
                email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));
                email.Subject = emaildetails;
                string? body;

                if (salesorderDetails.TypeOfSolution == "Automation" || salesorderDetails.TypeOfSolution == "Upsell - Automation" || salesorderDetails.TypeOfSolution == "Accessories" || salesorderDetails.TypeOfSolution == "Lock")
                {
                    string htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "keus-automation-salesorder.html");
                    body = System.IO.File.ReadAllText(htmlFilePath);
                    body = body.Replace("{{Sales-Order Number}}", salesorderDetails.SalesOrderNumber);
                    body = body.Replace("{{Customer Name}}", salesorderDetails.CustomerName);
                }
                else
                {
                    string htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "keus-light-salesorder.html");
                    body = System.IO.File.ReadAllText(htmlFilePath);
                    body = body.Replace("{{Customer Name}}", salesorderDetails.CustomerName);
                }
                string base64;
                var builder = new BodyBuilder();
                builder.HtmlBody = body;
                using (HttpClient client1 = new HttpClient())
                {
                    client1.Timeout = TimeSpan.FromMinutes(5);
                    var request2 = new HttpRequestMessage(HttpMethod.Get, salesOrderEmailPostDto.jasperfileUrl);

                    request2.Headers.Add("Authorization", "Basic amFzcGVyYWRtaW46Uk11aExncXdkOXBJUGI0");
                    request2.Headers.Add("X-Remote-Domain", "1");

                    var response2 = await client1.SendAsync(request2);
                    byte[] fileBytes = await response2.Content.ReadAsByteArrayAsync();
                    builder.Attachments.Add(FileName, fileBytes, ContentType.Parse("application/pdf"));
                    base64 = Convert.ToBase64String(fileBytes);
                }
                email.Body = builder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                smtp.Send(email);
                smtp.Disconnect(true);
                var jsonpayload = "{\r\n  \"recipient_type\": \"individual\",\r\n  \"to\": \"\",\r\n  \"type\": \"template\",\r\n  \"template\": {\r\n    \"name\": \"\",\r\n    \"language\": {\r\n      \"policy\": \"deterministic\",\r\n      \"code\": \"en\"\r\n    },\r\n    \"components\": [\r\n      {\r\n        \"type\": \"header\",\r\n        \"parameters\": [\r\n          {\r\n            \"type\": \"document\",\r\n            \"document\": {\r\n              \"filename\": \"\"\r\n            }\r\n          }\r\n        ]\r\n      }\r\n    ]\r\n  },\r\n  \"metadata\": {\r\n    \"messageId\": \"\",\r\n    \"media\": {\r\n      \"mimeType\": \"application/pdf\",\r\n      \"content\": \"\"\r\n    }\r\n  }\r\n}";
                WhatsAppMessagePayload whatsAppMessagePayload = JsonConvert.DeserializeObject<WhatsAppMessagePayload>(jsonpayload);
                whatsAppMessagePayload.Template.Name = whatsapptemplate;
                whatsAppMessagePayload.Template.Components[0].Parameters[0].Document.Filename = FileName;
                whatsAppMessagePayload.Metadata.Media.Content = base64;
                WhatsAppCreateTokenResponse whatsAppCreateTokenResponse;


                var request = new HttpRequestMessage(HttpMethod.Post, "https://auth.aclwhatsapp.com/realms/ipmessaging/protocol/openid-connect/token");
                request.Headers.Add("cache-control", "no-cache");
                var content = new StringContent($"grant_type=password&client_id=ipmessaging-client&username=keuspd&password=keuspd30", Encoding.UTF8, "application/x-www-form-urlencoded");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Content = content;
                var response = await client.SendAsync(request);

                whatsAppCreateTokenResponse = JsonConvert.DeserializeObject<WhatsAppCreateTokenResponse>(await response.Content.ReadAsStringAsync());
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Unable to Generate Token for Whatsapp Message");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }

                MsisdnListRequest msisdnListRequest = new MsisdnListRequest
                {
                    MsisdnList = whatsappNumbers
                };
                var No_s = JsonConvert.SerializeObject(msisdnListRequest);
                var data = new StringContent(No_s, Encoding.UTF8, "application/json");
                var request3 = new HttpRequestMessage(HttpMethod.Post, "https://optin.aclwhatsapp.com/api/v1/optin/bulk")
                {
                    Content = data
                };
                request3.Headers.Add("Authorization", "Bearer " + whatsAppCreateTokenResponse.AccessToken);
                var response3 = await client.SendAsync(request3);
                if (!response3.IsSuccessStatusCode)
                {
                    _logger.LogError($"Unable to OptinNumbers for Whatsapp Message");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }

                foreach (var number in whatsappNumbers)
                {
                    whatsAppMessagePayload.To = number;
                    var whatsappCreate = JsonConvert.SerializeObject(whatsAppMessagePayload);
                    var data4 = new StringContent(whatsappCreate, Encoding.UTF8, "application/json");
                    var request4 = new HttpRequestMessage(HttpMethod.Post, "https://api.aclwhatsapp.com/pull-platform-receiver/v2/wa/messages")
                    {
                        Content = data4
                    };
                    request4.Headers.Add("Authorization", "Bearer " + whatsAppCreateTokenResponse.AccessToken);
                    var response4 = await client.SendAsync(request4);
                    if (!response4.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Unable to Create a Whatsapp Message");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Something went wrong ,try again";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                }
                SalesOrderEmailsDetails salesOrderEmailsDetails = new SalesOrderEmailsDetails()
                {
                    SalesOrderNumber = salesorderDetails.SalesOrderNumber,
                    RevisionNumber = salesorderDetails.RevisionNumber,
                    ProjectNumber = salesorderDetails.ProjectNumber,
                    SentTo = salesOrderEmailPostDto.SentTo,
                    CustomerEmailId = salesOrderEmailPostDto.CusEmail,
                    CustomerId = salesorderDetails.CustomerId,
                    CustomerName = salesorderDetails.CustomerName,
                    SalesOrderId = salesorderDetails.Id,
                    SalesOrderValue = salesorderDetails.TotalFinalAmount,
                    TypeOfSolution = salesorderDetails.TypeOfSolution,
                    SentBy = _createdBy,
                    SentOn = DateTime.Now,
                    WhatsAppPhoneNos = salesOrderEmailPostDto.WhatsAppPhoneNos
                };
                await _salesOrderEmailsDetailsRepository.CreateSalesOrderEmailsDetails(salesOrderEmailsDetails);
                _salesOrderEmailsDetailsRepository.SaveAsync();

                SOEmailMessageSuccessMessage emailMessageSuccessMessage = new SOEmailMessageSuccessMessage()
                {
                    SalesOrderNumber = salesorderDetails.SalesOrderNumber,
                    RevisionNumber = salesorderDetails.RevisionNumber ?? 0
                };

                serviceResponse.Data = emailMessageSuccessMessage;
                serviceResponse.Message = $"Email sent successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SendEmailforSalesOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalesOrderforKeus([FromQuery] PagingParameter pagingParameter, [FromQuery] string? SearchTerm, [FromQuery] int Offset, [FromQuery] int Limit)
        {
            ServiceResponse<IEnumerable<SalesOrderforKeusDto>> serviceResponse = new ServiceResponse<IEnumerable<SalesOrderforKeusDto>>();
            try
            {
                var getAllSalesOrder = await _repository.GetAllSalesOrderforKeus(SearchTerm, Offset, Limit);
                var TotalCount = await _repository.GetAllSalesOrderCountforKeus(SearchTerm);

                var metadata = new
                {
                    TotalCount,
                    pagingParameter.PageSize,
                    CurrentPage = pagingParameter.PageNumber
                };
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all SalesOrders for Keus");
                serviceResponse.Data = getAllSalesOrder;
                serviceResponse.Message = "Returned all SalesOrders for Keus";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogInfo($"Returned owner with id: {ex.Message}{ex.InnerException}");

                serviceResponse.Data = null;
                serviceResponse.Message = ($"Returned owner with id: {ex.Message}{ex.InnerException}");
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //[HttpGet]
        //public async Task<IActionResult> GetSalesOrderNoDetailsByCustomerId(string Customerid)
        //{
        //    ServiceResponse<IEnumerable<ListofSalesOrderDetails>> serviceResponse = new ServiceResponse<IEnumerable<ListofSalesOrderDetails>>();

        //    try
        //    {
        //        var getSalesDetailByCustomerId = await _repository.GetSalesOrderNoDetailsByCustomerId(Customerid);
        //        if (getSalesDetailByCustomerId == null)
        //        {
        //            _logger.LogError($"SalesOrderDetail with id: {Customerid}, hasn't been found in db.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"SalesOrderDetail with id: {Customerid}, hasn't been found in db.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }
        //        else
        //        {
        //            _logger.LogInfo($"Returned SalesOrderDetail with id: {Customerid}");
        //            var result = _mapper.Map<IEnumerable<ListofSalesOrderDetails>>(getSalesDetailByCustomerId);
        //            serviceResponse.Data = result;
        //            serviceResponse.Message = "Success";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Ok(serviceResponse);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside GetSalesOrderNoDetailsByCustomerId action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Inter server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
    }


}





