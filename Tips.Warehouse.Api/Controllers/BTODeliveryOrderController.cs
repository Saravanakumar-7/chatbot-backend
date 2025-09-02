using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Repository;
//aravind
namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BTODeliveryOrderController : ControllerBase
    {
        private IBTODeliveryOrderRepository _repository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private IBTODeliveryOrderHistoryRepository _bTODeliveryOrderHistoryRepository;
        private IBTODeliveryOrderInventoryHistoryRepository _bTODeliveryOrderInventoryHistoryRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IInventoryRepository _inventoryRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        private readonly IHttpClientFactory _clientFactory;
        public BTODeliveryOrderController(IHttpClientFactory clientFactory, IBTODeliveryOrderInventoryHistoryRepository bTODeliveryOrderInventoryHistoryRepository, IBTODeliveryOrderRepository repository, IInventoryTranctionRepository inventoryTranctionRepository, IBTODeliveryOrderHistoryRepository bTODeliveryOrderHistoryRepository, HttpClient httpClient, IConfiguration config, IInventoryRepository inventoryRepository, ILoggerManager logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _inventoryRepository = inventoryRepository;
            _config = config;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _bTODeliveryOrderHistoryRepository = bTODeliveryOrderHistoryRepository;
            _bTODeliveryOrderInventoryHistoryRepository = bTODeliveryOrderInventoryHistoryRepository;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }


        [HttpGet]
        public async Task<IActionResult> GetAllBTODeliveryOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderDto>>();
            try
            {
                var getAllBTODeliveryOrdersDetails = await _repository.GetAllBTODeliveryOrders(pagingParameter, searchParams);
                var metadata = new
                {
                    getAllBTODeliveryOrdersDetails.TotalCount,
                    getAllBTODeliveryOrdersDetails.PageSize,
                    getAllBTODeliveryOrdersDetails.CurrentPage,
                    getAllBTODeliveryOrdersDetails.HasNext,
                    getAllBTODeliveryOrdersDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all BTODeliveryOrder");
                var result = _mapper.Map<IEnumerable<BTODeliveryOrderDto>>(getAllBTODeliveryOrdersDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BTODeliveryOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllBTODeliveryOrders API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllBTODeliveryOrders API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeliveryOrderSPReport([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<DeliveryOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<DeliveryOrderSPReport>>();
            try
            {
                var products = await _repository.DeliveryOrderSPReport(pagingParameter);
                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all DeliveryOrderSPReport");



                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned DeliveryOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeliveryOrderSPReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeliveryOrderSPReport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeliveryOrderSPReportdate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<DeliveryOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<DeliveryOrderSPReport>>();
            try
            {
                var products = await _repository.DeliveryOrderSPReportdate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned DeliveryOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeliveryOrderSPReportdate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeliveryOrderSPReportdate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeliveryOrderSPReportdateForTrans([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<DeliveryOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<DeliveryOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.DeliveryOrderSPReportdateForTrans(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned DeliveryOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeliveryOrderSPReportdateForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeliveryOrderSPReportdateForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DOSPReportdateForTrans([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<DOSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<DOSPReportForTrans>>();
            try
            {
                var products = await _repository.DOSPReportdateForTrans(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned DO Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DOSPReportdateForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DOSPReportdateForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetDeliveryOrderSPReportsWithParam([FromBody] DeliveryOrderReportWithParamDTO deliveryOrderSPReport)
        {
            ServiceResponse<IEnumerable<DeliveryOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<DeliveryOrderSPReport>>();
            try
            {
                var products = await _repository.GetDeliveryOrderSPReportsWithParam(deliveryOrderSPReport.DoNumber, deliveryOrderSPReport.CustomerName,
                                                                                    deliveryOrderSPReport.CustomerAliasName, deliveryOrderSPReport.CustomerId,
                                                                                    deliveryOrderSPReport.SalesOrderNumber, deliveryOrderSPReport.ProductType,
                                                                                    deliveryOrderSPReport.Warehouse, deliveryOrderSPReport.Location,
                                                                                    deliveryOrderSPReport.KPN, deliveryOrderSPReport.MPN, deliveryOrderSPReport.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned DeliveryOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDeliveryOrderSPReportsWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDeliveryOrderSPReportsWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDoVsInvoiceSpReportSPReportsWithParam([FromBody] DoVsInvoiceInputParamDto doVsInvoiceInputParamDto)
        {
            ServiceResponse<IEnumerable<DoVsInvoiceSpReport>> serviceResponse = new ServiceResponse<IEnumerable<DoVsInvoiceSpReport>>();
            try
            {
                var products = await _repository.GetDoVsInvoiceSpReportSPReportsWithParam(doVsInvoiceInputParamDto.SalesOrderNumber, doVsInvoiceInputParamDto.DONumber,
                                                         doVsInvoiceInputParamDto.ItemNumber,doVsInvoiceInputParamDto.InvoiceNumber, doVsInvoiceInputParamDto.CustomerId);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DoVsInvoice hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DoVsInvoice hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned DoVsInvoice Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDoVsInvoiceSpReportSPReportsWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDoVsInvoiceSpReportSPReportsWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    

    [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetDeliveryOrderSPReportsWithParamForAvi([FromBody] DeliveryOrderReportWithParamDTO deliveryOrderSPReport)
        {
            ServiceResponse<IEnumerable<DeliveryOrderSPReportForAvi>> serviceResponse = new ServiceResponse<IEnumerable<DeliveryOrderSPReportForAvi>>();
            try
            {
                var products = await _repository.GetDeliveryOrderSPReportsWithParamForAvi(deliveryOrderSPReport.DoNumber, deliveryOrderSPReport.CustomerName,
                                                                                    deliveryOrderSPReport.CustomerAliasName, deliveryOrderSPReport.CustomerId,
                                                                                    deliveryOrderSPReport.SalesOrderNumber, deliveryOrderSPReport.ProductType,
                                                                                    deliveryOrderSPReport.Warehouse, deliveryOrderSPReport.Location,
                                                                                    deliveryOrderSPReport.KPN, deliveryOrderSPReport.MPN, deliveryOrderSPReport.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned DeliveryOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDeliveryOrderSPReportsWithParamForAvi API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDeliveryOrderSPReportsWithParamForAvi API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeliveryOrderSPReportdateForAvi([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<DeliveryOrderSPReportForAvi>> serviceResponse = new ServiceResponse<IEnumerable<DeliveryOrderSPReportForAvi>>();
            try
            {
                var products = await _repository.DeliveryOrderSPReportdateForAvi(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned DeliveryOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeliveryOrderSPReportdateForAvi API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeliveryOrderSPReportdateForAvi API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetDeliveryOrderSPReportsWithParamForTrans([FromBody] DeliveryOrderReportWithParamDTOForTrans deliveryOrderSPReport)
        {
            ServiceResponse<IEnumerable<DeliveryOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<DeliveryOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.GetDeliveryOrderSPReportsWithParamForTrans(deliveryOrderSPReport.DoNumber, deliveryOrderSPReport.CustomerName,
                                                                                    deliveryOrderSPReport.SalesOrderNumber, deliveryOrderSPReport.ProductType,
                                                                                    deliveryOrderSPReport.Warehouse, deliveryOrderSPReport.Location,
                                                                                    deliveryOrderSPReport.ItemNumber, deliveryOrderSPReport.MPN, deliveryOrderSPReport.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned DeliveryOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDeliveryOrderSPReportsWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDeliveryOrderSPReportsWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetDOSPReportsWithParamForTrans([FromBody] DeliveryOrderReportWithParamDTOForTrans deliveryOrderSPReport)
        {
            ServiceResponse<IEnumerable<DOSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<DOSPReportForTrans>>();
            try
            {
                var products = await _repository.GetDOSPReportsWithParamForTrans(deliveryOrderSPReport.DoNumber, deliveryOrderSPReport.CustomerName,
                                                                                    deliveryOrderSPReport.SalesOrderNumber, deliveryOrderSPReport.ProductType,
                                                                                    deliveryOrderSPReport.Warehouse, deliveryOrderSPReport.Location,
                                                                                    deliveryOrderSPReport.ItemNumber, deliveryOrderSPReport.MPN, deliveryOrderSPReport.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned DO Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDOSPReportsWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDOSPReportsWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBTODeliveryOrderById(int id)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var getBTODeliveryOrderDetailById = await _repository.GetBTODeliveryOrderById(id);

                if (getBTODeliveryOrderDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BTODeliveryOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");

                    BTODeliveryOrderDto bTODeliveryOrderDto = _mapper.Map<BTODeliveryOrderDto>(getBTODeliveryOrderDetailById);

                    List<BTODeliveryOrderItemsDto> bTODeliveryOrderItemsDtoList = new List<BTODeliveryOrderItemsDto>();

                    if (getBTODeliveryOrderDetailById.bTODeliveryOrderItems != null)
                    {

                        foreach (var deliveryOrderitemDetails in getBTODeliveryOrderDetailById.bTODeliveryOrderItems)
                        {
                            BTODeliveryOrderItemsDto bTODeliveryOrderItemsDtos = _mapper.Map<BTODeliveryOrderItemsDto>(deliveryOrderitemDetails);
                            bTODeliveryOrderItemsDtos.QtyDistribution = _mapper.Map<List<BtoDeliveryOrderItemQtyDistributionDto>>(deliveryOrderitemDetails.QtyDistribution);
                            //bTODeliveryOrderItemsDtos.BTOSerialNumberDto = _mapper.Map<List<BTOSerialNumberDto>>(deliveryOrderitemDetails.BTOSerialNumbers);
                            bTODeliveryOrderItemsDtoList.Add(bTODeliveryOrderItemsDtos);
                        }
                    }
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    //var salesOrderObjectResult = await _httpClient.GetAsync(string.Concat(_config["SalesOrderAPI"],
                    //                         "GetSalesOrderTotalBySalesOrderId?", "&SalesOrderId=", bTODeliveryOrderDto.SalesOrderId));
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["SalesOrderAPI"],
                           $"GetSalesOrderTotalBySalesOrderId?SalesOrderId={bTODeliveryOrderDto.SalesOrderId}"));
                    request.Headers.Add("Authorization", token);

                    var salesOrderObjectResult = await client.SendAsync(request);
                    var salesOrderObjectString = await salesOrderObjectResult.Content.ReadAsStringAsync();
                    dynamic salesOrderObjectData = JsonConvert.DeserializeObject(salesOrderObjectString);
                    dynamic salesOrderObject = salesOrderObjectData;
                    decimal salesOrderTotal = Convert.ToDecimal(salesOrderObject);
                    bTODeliveryOrderDto.SOTotal = salesOrderTotal;

                    bTODeliveryOrderDto.bTODeliveryOrderItems = bTODeliveryOrderItemsDtoList;

                    serviceResponse.Data = bTODeliveryOrderDto;
                    serviceResponse.Message = "Returned BTODeliveryOrderById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBTODeliveryOrderById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBTODeliveryOrderById API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //get bto number list passing customer leadid
        [HttpGet]
        public async Task<IActionResult> SearchBTODeliveryOrderDate([FromQuery] SearchsDateParms searchDateParam)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderReportDto>>();
            try
            {
                var bTODeliveryOrders = await _repository.SearchBTODeliveryOrderDate(searchDateParam);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<BTODeliveryOrder, BTODeliveryOrderReportDto>()
                       .ForMember(dest => dest.bTODeliveryOrderItems, opt => opt.MapFrom(src => src.bTODeliveryOrderItems
                       .Select(bTODeliveryOrderItems => new BTODeliveryOrderItemsReportDto
                       {
                           Id = bTODeliveryOrderItems.Id,
                           BTONumber = src.BTONumber,
                           FGItemNumber = bTODeliveryOrderItems.FGItemNumber,
                           SalesOrderId = bTODeliveryOrderItems.SalesOrderId,
                           Description = bTODeliveryOrderItems.Description,
                           BalanceDoQty = bTODeliveryOrderItems.BalanceDoQty,
                           InvoicedQty = bTODeliveryOrderItems.InvoicedQty,
                           UnitPrice = bTODeliveryOrderItems.UnitPrice,
                           UOC = bTODeliveryOrderItems.UOC,
                           UOM = bTODeliveryOrderItems.UOM,
                           FGOrderQty = bTODeliveryOrderItems.FGOrderQty,
                           OrderBalanceQty = bTODeliveryOrderItems.OrderBalanceQty,
                           FGStock = bTODeliveryOrderItems.FGStock,
                           Discount = bTODeliveryOrderItems.Discount,
                           NetValue = bTODeliveryOrderItems.NetValue,
                           DispatchQty = bTODeliveryOrderItems.DispatchQty,
                           SerialNo = bTODeliveryOrderItems.SerialNo,
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<BTODeliveryOrderReportDto>>(bTODeliveryOrders);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BTODeliveryOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchBTODeliveryOrderDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchBTODeliveryOrderDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchBTODeliveryOrder([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderReportDto>>();
            try
            {
                var btoDeliveyOrderList = await _repository.SearchBTODeliveryOrder(searchParams);

                _logger.LogInfo("Returned all BTODeliveryOrder");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<BTODeliveryOrder, BTODeliveryOrderDto>().ReverseMap()
                //    .ForMember(dest => dest.bTODeliveryOrderItems, opt => opt.MapFrom(src => src.bTODeliveryOrderItems));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<BTODeliveryOrder, BTODeliveryOrderReportDto>()
                       .ForMember(dest => dest.bTODeliveryOrderItems, opt => opt.MapFrom(src => src.bTODeliveryOrderItems
                       .Select(bTODeliveryOrderItems => new BTODeliveryOrderItemsReportDto
                       {
                           Id = bTODeliveryOrderItems.Id,
                           BTONumber = src.BTONumber,
                           FGItemNumber = bTODeliveryOrderItems.FGItemNumber,
                           SalesOrderId = bTODeliveryOrderItems.SalesOrderId,
                           Description = bTODeliveryOrderItems.Description,
                           BalanceDoQty = bTODeliveryOrderItems.BalanceDoQty,
                           InvoicedQty = bTODeliveryOrderItems.InvoicedQty,
                           UnitPrice = bTODeliveryOrderItems.UnitPrice,
                           UOC = bTODeliveryOrderItems.UOC,
                           UOM = bTODeliveryOrderItems.UOM,
                           FGOrderQty = bTODeliveryOrderItems.FGOrderQty,
                           OrderBalanceQty = bTODeliveryOrderItems.OrderBalanceQty,
                           FGStock = bTODeliveryOrderItems.FGStock,
                           Discount = bTODeliveryOrderItems.Discount,
                           NetValue = bTODeliveryOrderItems.NetValue,
                           DispatchQty = bTODeliveryOrderItems.DispatchQty,
                           SerialNo = bTODeliveryOrderItems.SerialNo,
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<BTODeliveryOrderReportDto>>(btoDeliveyOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BTODeliveryOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchBTODeliveryOrder API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchBTODeliveryOrder API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllBTODeliveryOrderWithItems([FromBody] BTODeliveryOrderSearchDto bTODeliveryOrderSearch)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderReportDto>>();
            try
            {
                var bTODeliveryOrders = await _repository.GetAllBTODeliveryOrderWithItems(bTODeliveryOrderSearch);
                _logger.LogInfo("Returned all bTODeliveryOrders");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<BTODeliveryOrderDto, BTODeliveryOrder>().ReverseMap()
                //    .ForMember(dest => dest.bTODeliveryOrderItems, opt => opt.MapFrom(src => src.bTODeliveryOrderItems));
                //});

                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<BTODeliveryOrder, BTODeliveryOrderReportDto>()
                       .ForMember(dest => dest.bTODeliveryOrderItems, opt => opt.MapFrom(src => src.bTODeliveryOrderItems
                       .Select(bTODeliveryOrderItems => new BTODeliveryOrderItemsReportDto
                       {
                           Id = bTODeliveryOrderItems.Id,
                           BTONumber = src.BTONumber,
                           FGItemNumber = bTODeliveryOrderItems.FGItemNumber,
                           SalesOrderId = bTODeliveryOrderItems.SalesOrderId,
                           Description = bTODeliveryOrderItems.Description,
                           BalanceDoQty = bTODeliveryOrderItems.BalanceDoQty,
                           InvoicedQty = bTODeliveryOrderItems.InvoicedQty,
                           UnitPrice = bTODeliveryOrderItems.UnitPrice,
                           UOC = bTODeliveryOrderItems.UOC,
                           UOM = bTODeliveryOrderItems.UOM,
                           FGOrderQty = bTODeliveryOrderItems.FGOrderQty,
                           OrderBalanceQty = bTODeliveryOrderItems.OrderBalanceQty,
                           FGStock = bTODeliveryOrderItems.FGStock,
                           Discount = bTODeliveryOrderItems.Discount,
                           NetValue = bTODeliveryOrderItems.NetValue,
                           DispatchQty = bTODeliveryOrderItems.DispatchQty,
                           SerialNo = bTODeliveryOrderItems.SerialNo,
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<BTODeliveryOrderReportDto>>(bTODeliveryOrders);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all bTODeliveryOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllBTODeliveryOrderWithItems API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllBTODeliveryOrderWithItems API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet()] // Adjust your route as needed
        public async Task<IActionResult> GetDailyDeliveryOrderReports([FromQuery] string? LeadId, [FromQuery] string? SONumber, [FromQuery] string? DOnumber, [FromQuery] string? DispatchKPN)
        {
            var products = await _repository.GetDailyDeliveryOrderReports(LeadId, SONumber, DOnumber, DispatchKPN);

            return Ok(products);
        }
        [HttpGet]
        public async Task<IActionResult> GetBtoNumberListByCustomerId(string customerLeadId)
        {
            ServiceResponse<IEnumerable<ListOfBtoNumberDetails>> serviceResponse = new ServiceResponse<IEnumerable<ListOfBtoNumberDetails>>();
            try
            {
                var getBTONumberList = await _repository.GetBtoNumberListByCustomerId(customerLeadId);

                if (getBTONumberList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BTODeliveryOrderNumbers not found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BTODeliveryOrderNumbersList with id: {customerLeadId},not found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {customerLeadId}");

                    var bTODeliveryNumberList = _mapper.Map<IEnumerable<ListOfBtoNumberDetails>>(getBTONumberList);


                    serviceResponse.Data = bTODeliveryNumberList;
                    serviceResponse.Message = "Returned BTODeliveryNumberList Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBtoNumberListByCustomerId API for the following customerLeadId:{customerLeadId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBtoNumberListByCustomerId API for the following customerLeadId:{customerLeadId} \n {ex.Message}";
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
        [HttpPost]
        public async Task<IActionResult> CreateBTODeliveryOrder([FromBody] BTODeliveryOrderDtoPost bTODeliveryOrderDtoPost)
        {
            ServiceResponse<BTODeliveryOrderDtoPost> serviceResponse = new ServiceResponse<BTODeliveryOrderDtoPost>();
            try
            {
                string serverKey = GetServerKey();
                if (bTODeliveryOrderDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BTODeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("BTODeliveryOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid BTODeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid BTODeliveryOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var bTODeliveryOrder = _mapper.Map<BTODeliveryOrder>(bTODeliveryOrderDtoPost);

                var bTODeliveryOrderItemsListDto = bTODeliveryOrderDtoPost.BTODeliveryOrderItemsDtoPost;

                var bTODoItemList = new List<BTODeliveryOrderItems>();

                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                if (serverKey == "trasccon")
                {
                    var dateFormat = days + months + years;
                    var btoNumber = await _repository.GenerateBTONumber();
                    bTODeliveryOrder.BTONumber = dateFormat + btoNumber;
                }
                else if (serverKey == "keus")
                {
                    var dateFormat = days + months + years;
                    var btoNumber = await _repository.GenerateBTONumber();
                    bTODeliveryOrder.BTONumber = dateFormat + btoNumber;
                }
                else
                {
                    var btoNumber = await _repository.GenerateBTONumberAvision();
                    bTODeliveryOrder.BTONumber = btoNumber;
                }

                if (bTODeliveryOrderItemsListDto != null)
                {

                    for (int i = 0; i < bTODeliveryOrderItemsListDto.Count; i++)
                    {
                        BTODeliveryOrderItems bTODeliveryOrderItemsDetails = _mapper.Map<BTODeliveryOrderItems>(bTODeliveryOrderItemsListDto[i]);
                        bTODeliveryOrderItemsDetails.QtyDistribution = _mapper.Map<List<BtoDeliveryOrderItemQtyDistribution>>(bTODeliveryOrderItemsListDto[i].QtyDistribution);
                        bTODeliveryOrderItemsDetails.InitialDispatchQty = bTODeliveryOrderItemsDetails.DispatchQty;
                        bTODeliveryOrderItemsDetails.OrderBalanceQty = bTODeliveryOrderItemsDetails.FGOrderQty - bTODeliveryOrderItemsDetails.DispatchQty;
                        bTODeliveryOrderItemsDetails.BalanceDoQty = bTODeliveryOrderItemsDetails.DispatchQty;
                        bTODeliveryOrderItemsDetails.BTONumber = bTODeliveryOrder.BTONumber;
                        bTODoItemList.Add(bTODeliveryOrderItemsDetails);


                        var distriution = _mapper.Map<List<BtoDeliveryOrderItemQtyDistribution>>(bTODeliveryOrderItemsListDto[i].QtyDistribution);
                        //Update Inventory balanced Quantity 
                        if (serverKey == "keus")
                        {
                            await _inventoryRepository.UpdateInventoryforBTO_Keus(distriution, bTODeliveryOrder.BTONumber);
                        }
                        else
                        {
                            await _inventoryRepository.UpdateInventoryforBTO(distriution, bTODeliveryOrder.BTONumber);
                        }

                        //}

                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = bTODoItemList[i].FGItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request1.Headers.Add("Authorization", token1);

                        var itemMasterObjectResult = await client1.SendAsync(request1);

                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        var itemMasterObjectData = JsonConvert.DeserializeObject<ReturnBTONumberInvDetails>(itemMasterObjectString);
                        var itemMasterObject = itemMasterObjectData.data;


                        ////Add BTO Detail Into Inventory transaction Table
                        foreach (var eachbin in bTODeliveryOrderItemsDetails.QtyDistribution)
                        {
                            //InventoryTranction inventoryTranction = new InventoryTranction();
                            //inventoryTranction.PartNumber = bTODoItemList[i].FGItemNumber;
                            //inventoryTranction.LotNumber = eachbin.LotNumber;
                            //inventoryTranction.ProjectNumber = eachbin.ProjectNumber;
                            //inventoryTranction.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                            //inventoryTranction.PartType = PartType.FG;
                            //inventoryTranction.Description = bTODoItemList[i].Description;
                            //inventoryTranction.Issued_Quantity = eachbin.DistributingQty;
                            //inventoryTranction.IsStockAvailable = true;
                            //inventoryTranction.UOM = bTODoItemList[i].UOM;
                            //inventoryTranction.Issued_DateTime = DateTime.Now;
                            //inventoryTranction.Issued_By = _createdBy;
                            //inventoryTranction.ReferenceID = bTODeliveryOrder.BTONumber;
                            //inventoryTranction.ReferenceIDFrom = "BTO Delivery Order";
                            //inventoryTranction.From_Location = eachbin.Location;
                            //inventoryTranction.TO_Location = "BTO";
                            //inventoryTranction.Warehouse = eachbin.Warehouse;
                            //inventoryTranction.Remarks = "Create BTO";

                            InventoryTranction inventoryTranctionPost1 = new InventoryTranction();
                            inventoryTranctionPost1.PartNumber = bTODoItemList[i].FGItemNumber;
                            inventoryTranctionPost1.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                            inventoryTranctionPost1.ProjectNumber = eachbin.ProjectNumber;
                            inventoryTranctionPost1.Description = bTODoItemList[i].Description;
                            inventoryTranctionPost1.Issued_Quantity = eachbin.DistributingQty;
                            inventoryTranctionPost1.LotNumber = eachbin.LotNumber;
                            inventoryTranctionPost1.UOM = bTODoItemList[i].UOM;
                            inventoryTranctionPost1.Unit = _unitname;
                            inventoryTranctionPost1.IsStockAvailable = false;
                            inventoryTranctionPost1.Warehouse = eachbin.Warehouse;
                            inventoryTranctionPost1.From_Location = "BTO";
                            inventoryTranctionPost1.TO_Location = eachbin.Location;
                            inventoryTranctionPost1.PartType = bTODoItemList[i].PartType;
                            inventoryTranctionPost1.ReferenceID = bTODeliveryOrder.BTONumber;
                            inventoryTranctionPost1.ReferenceIDFrom = "BTO Delivery Order";
                            inventoryTranctionPost1.Remarks = "BTO Delivery Order";
                            inventoryTranctionPost1.TransactionType = InventoryType.Outward;

                           // var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


                            await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost1);



                            // Add Bto detail in to btodeliveryorderhistory table

                            BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                            bTODeliveryOrderHistory.BTONumber = bTODeliveryOrder.BTONumber;
                            bTODeliveryOrderHistory.CustomerName = bTODeliveryOrder.CustomerName;
                            bTODeliveryOrderHistory.CustomerAliasName = bTODeliveryOrder.CustomerAliasName;
                            bTODeliveryOrderHistory.CustomerId = bTODeliveryOrder.CustomerId;
                            bTODeliveryOrderHistory.PONumber = bTODeliveryOrder.PONumber;
                            bTODeliveryOrderHistory.IssuedTo = bTODeliveryOrder.IssuedTo;
                            bTODeliveryOrderHistory.DODate = bTODeliveryOrder.DODate;
                            bTODeliveryOrderHistory.FGItemNumber = bTODoItemList[i].FGItemNumber;
                            bTODeliveryOrderHistory.SalesOrderId = bTODoItemList[i].SalesOrderId;
                            bTODeliveryOrderHistory.Description = bTODoItemList[i].Description;
                            bTODeliveryOrderHistory.BalanceDoQty = bTODoItemList[i].BalanceDoQty;
                            bTODeliveryOrderHistory.UnitPrice = bTODoItemList[i].UnitPrice;
                            bTODeliveryOrderHistory.UOC = bTODoItemList[i].UOC;
                            bTODeliveryOrderHistory.UOM = bTODoItemList[i].UOM;
                            bTODeliveryOrderHistory.FGOrderQty = bTODoItemList[i].FGOrderQty;
                            bTODeliveryOrderHistory.OrderBalanceQty = bTODoItemList[i].OrderBalanceQty;
                            bTODeliveryOrderHistory.FGStock = bTODoItemList[i].FGStock;
                            bTODeliveryOrderHistory.Discount = Convert.ToDecimal(bTODoItemList[i].Discount);
                            bTODeliveryOrderHistory.NetValue = bTODoItemList[i].NetValue;
                            bTODeliveryOrderHistory.DispatchQty = eachbin.DistributingQty;
                            bTODeliveryOrderHistory.InvoicedQty = bTODoItemList[i].InvoicedQty;
                            bTODeliveryOrderHistory.SerialNo = bTODoItemList[i].SerialNo;
                            bTODeliveryOrderHistory.Location = eachbin.Location;
                            bTODeliveryOrderHistory.Warehouse = eachbin.Warehouse;
                            bTODeliveryOrderHistory.LotNumber = eachbin.LotNumber;
                            //bTODeliveryOrderHistory.CreatedBy = bTODoItemList[i].CreatedBy;
                            //bTODeliveryOrderHistory.LastModifiedOn = bTODoItemList[i].LastModifiedOn;
                            bTODeliveryOrderHistory.Remark = "From Create BTO";


                            var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);


                            await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);

                        }
                    }
                }

                bTODeliveryOrder.bTODeliveryOrderItems = bTODoItemList;

                await _repository.CreateBTODeliveryOrder(bTODeliveryOrder);


                //update balance qty and dispatch qty in salesorder table
                var btoDeliveryDispatchDetails = _mapper.Map<List<BtoDeliveryOrderDispatchQtyDetailsDto>>(bTODeliveryOrderItemsListDto);

                var json = JsonConvert.SerializeObject(btoDeliveryDispatchDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                //var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "UpdateDispatchDetails"), data);
                var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["SalesOrderAPI"],"UpdateDispatchDetails"))
                {
                    Content=data
                };
                request.Headers.Add("Authorization", token);

                var response = await client.SendAsync(request);
                if ((response.StatusCode == HttpStatusCode.OK))
                {
                    _inventoryRepository.SaveAsync();
                    _bTODeliveryOrderHistoryRepository.SaveAsync();
                    _repository.SaveAsync();
                }
                else
                {
                    _logger.LogError($"Something went wrong inside CreateBTODelivaryOrder action inside SalesOrder Controller UpdateDispatchDetails action");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                _logger.LogInfo($"CreateBTODelivaryOrder action: {response}");
                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateBTODeliveryOrder API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateBTODeliveryOrder API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBTODeliveryOrder_AV([FromBody] BTODeliveryOrderDtoPost bTODeliveryOrderDtoPost)
        {
            ServiceResponse<BTODeliveryOrderDtoPost> serviceResponse = new ServiceResponse<BTODeliveryOrderDtoPost>();
            try
            {
                string serverKey = GetServerKey();
                if (bTODeliveryOrderDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BTODeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("BTODeliveryOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid BTODeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid BTODeliveryOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var bTODeliveryOrder = _mapper.Map<BTODeliveryOrder>(bTODeliveryOrderDtoPost);

                var bTODeliveryOrderItemsListDto = bTODeliveryOrderDtoPost.BTODeliveryOrderItemsDtoPost;

                var bTODoItemList = new List<BTODeliveryOrderItems>();

                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                if (serverKey == "trasccon")
                {
                    var dateFormat = days + months + years;
                    var btoNumber = await _repository.GenerateBTONumber();
                    bTODeliveryOrder.BTONumber = dateFormat + btoNumber;
                }
                else if (serverKey == "keus")
                {
                    var dateFormat = days + months + years;
                    var btoNumber = await _repository.GenerateBTONumber();
                    bTODeliveryOrder.BTONumber = dateFormat + btoNumber;
                }
                else
                {
                    var btoNumber = await _repository.GenerateBTONumberAvision();
                    bTODeliveryOrder.BTONumber = btoNumber;
                }

                if (bTODeliveryOrderItemsListDto != null)
                {

                    for (int i = 0; i < bTODeliveryOrderItemsListDto.Count; i++)
                    {
                        BTODeliveryOrderItems bTODeliveryOrderItemsDetails = _mapper.Map<BTODeliveryOrderItems>(bTODeliveryOrderItemsListDto[i]);


                        bTODeliveryOrderItemsDetails.OrderBalanceQty = bTODeliveryOrderItemsDetails.FGOrderQty - bTODeliveryOrderItemsDetails.DispatchQty;
                        bTODeliveryOrderItemsDetails.BalanceDoQty = bTODeliveryOrderItemsDetails.DispatchQty;
                        bTODeliveryOrderItemsDetails.BTONumber = bTODeliveryOrder.BTONumber;
                        bTODoItemList.Add(bTODeliveryOrderItemsDetails);



                        //Update Inventory balanced Quantity 

                        // await _inventoryRepository.UpdateInventoryforBTO(bTODeliveryOrderItemsDetails.QtyDistribution);

                        var ItemNumber = bTODoItemList[i].FGItemNumber;
                        var getInventoryFGDetailsByItemnumber = await _inventoryRepository.GetInventoryByItemNumber(ItemNumber); //pass projectNo
                        decimal dispatchQuantity = Convert.ToDecimal(bTODeliveryOrderItemsListDto[i].DispatchQty);
                        if (getInventoryFGDetailsByItemnumber != null)
                        {
                            foreach (var inventory in getInventoryFGDetailsByItemnumber)
                            {
                                var stockAvailable = inventory.Balance_Quantity;
                                if (dispatchQuantity != 0 && stockAvailable >= dispatchQuantity)
                                {
                                    inventory.Balance_Quantity -= dispatchQuantity;
                                    stockAvailable -= dispatchQuantity;
                                    dispatchQuantity = 0;

                                    if (stockAvailable == 0)
                                    {
                                        inventory.IsStockAvailable = false;
                                    }


                                }
                                else if (dispatchQuantity != 0 && stockAvailable < dispatchQuantity)
                                {
                                    dispatchQuantity -= stockAvailable;
                                    inventory.Balance_Quantity = 0;
                                    inventory.IsStockAvailable = false;


                                }

                                _inventoryRepository.Update(inventory);
                                if (dispatchQuantity <= 0)
                                {
                                    break;
                                }
                            }

                        }






                        // Add Bto detail in to btodeliveryorderhistory table

                        BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                        bTODeliveryOrderHistory.BTONumber = bTODeliveryOrder.BTONumber;
                        bTODeliveryOrderHistory.CustomerName = bTODeliveryOrder.CustomerName;
                        bTODeliveryOrderHistory.CustomerAliasName = bTODeliveryOrder.CustomerAliasName;
                        bTODeliveryOrderHistory.CustomerId = bTODeliveryOrder.CustomerId;
                        bTODeliveryOrderHistory.PONumber = bTODeliveryOrder.PONumber;
                        bTODeliveryOrderHistory.IssuedTo = bTODeliveryOrder.IssuedTo;
                        bTODeliveryOrderHistory.DODate = bTODeliveryOrder.DODate;
                        bTODeliveryOrderHistory.FGItemNumber = bTODoItemList[i].FGItemNumber;
                        bTODeliveryOrderHistory.SalesOrderId = bTODoItemList[i].SalesOrderId;
                        bTODeliveryOrderHistory.Description = bTODoItemList[i].Description;
                        bTODeliveryOrderHistory.BalanceDoQty = bTODoItemList[i].BalanceDoQty;
                        bTODeliveryOrderHistory.UnitPrice = bTODoItemList[i].UnitPrice;
                        bTODeliveryOrderHistory.UOC = bTODoItemList[i].UOC;
                        bTODeliveryOrderHistory.UOM = bTODoItemList[i].UOM;
                        bTODeliveryOrderHistory.FGOrderQty = bTODoItemList[i].FGOrderQty;
                        bTODeliveryOrderHistory.OrderBalanceQty = bTODoItemList[i].OrderBalanceQty;
                        bTODeliveryOrderHistory.FGStock = bTODoItemList[i].FGStock;
                        bTODeliveryOrderHistory.Discount = Convert.ToDecimal(bTODoItemList[i].Discount);
                        bTODeliveryOrderHistory.NetValue = bTODoItemList[i].NetValue;
                        bTODeliveryOrderHistory.DispatchQty = bTODoItemList[i].DispatchQty;
                        bTODeliveryOrderHistory.InvoicedQty = bTODoItemList[i].InvoicedQty;
                        bTODeliveryOrderHistory.SerialNo = bTODoItemList[i].SerialNo;
                        //bTODeliveryOrderHistory.CreatedBy = bTODoItemList[i].CreatedBy;
                        //bTODeliveryOrderHistory.LastModifiedOn = bTODoItemList[i].LastModifiedOn;
                        bTODeliveryOrderHistory.Remark = "From Create BTO";


                        var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);


                        await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);


                    }

                }

                bTODeliveryOrder.bTODeliveryOrderItems = bTODoItemList;

                await _repository.CreateBTODeliveryOrder(bTODeliveryOrder);


                //update balance qty and dispatch qty in salesorder table
                var btoDeliveryDispatchDetails = _mapper.Map<List<BtoDeliveryOrderDispatchQtyDetailsDto>>(bTODeliveryOrderItemsListDto);

                var json = JsonConvert.SerializeObject(btoDeliveryDispatchDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                // Include the token in the Authorization header
                var tokenValues = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(tokenValues) && tokenValues.StartsWith("Bearer "))
                {
                    var token = tokenValues.Substring(7);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "UpdateDispatchDetails"), data);
                if ((response.StatusCode == HttpStatusCode.OK))
                {
                    _inventoryRepository.SaveAsync();
                    _bTODeliveryOrderHistoryRepository.SaveAsync();
                    _repository.SaveAsync();
                }
                else
                {
                    _logger.LogError($"Something went wrong inside CreateBTODelivaryOrder action inside SalesOrder Controller UpdateDispatchDetails action");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                _logger.LogError($"Something went wrong inside CreateBTODelivaryOrder action: {response}");
                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateBTODeliveryOrder_AV API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateBTODeliveryOrder_AV API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet()] // Adjust your route as needed
        public async Task<IActionResult> GetAllDailyDeliveryOrderReports()
        {
            var products = await _repository.GetDailyDeliveryOrderReports();

            return Ok(products);
        }
        [HttpGet]
        public async Task<IActionResult> GetBTONumberListBySalesOrderId(int salesOrderId)
        {
            ServiceResponse<IEnumerable<ListOfBtoNumberDetails>> serviceResponse = new ServiceResponse<IEnumerable<ListOfBtoNumberDetails>>();
            try
            {
                var btoNumberList = await _repository.GetBtoNumberListBySalesOrderId(salesOrderId);

                if (btoNumberList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BTODeliveryOrderNumberList not found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BTODeliveryOrderNumbersList with salesOrderId: {salesOrderId},not found.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {salesOrderId}");

                    var bTODeliveryNumberList = _mapper.Map<IEnumerable<ListOfBtoNumberDetails>>(btoNumberList);


                    serviceResponse.Data = bTODeliveryNumberList;
                    serviceResponse.Message = "Returned BTODeliveryNumberList Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBTONumberListBySalesOrderId API for the following salesOrderId:{salesOrderId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBTONumberListBySalesOrderId API for the following salesOrderId:{salesOrderId} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBTODeliveryOrder(int id, [FromBody] BTODeliveryOrderDtoUpdate bTODeliveryOrderDtoUpdate)
        {
            ServiceResponse<BTODeliveryOrderDtoUpdate> serviceResponse = new ServiceResponse<BTODeliveryOrderDtoUpdate>();
            try
            {
                if (bTODeliveryOrderDtoUpdate is null)
                {
                    _logger.LogError("Update BTODeliveryOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update BTODeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update BTODeliveryOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update BTODeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var bTODeliveryOrderbyId = await _repository.GetBTODeliveryOrderById(id);
                if (bTODeliveryOrderbyId is null)
                {
                    _logger.LogError($"Update BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update BTODeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var bTODeliveryOrder = _mapper.Map<BTODeliveryOrder>(bTODeliveryOrderbyId);

                var getOldbtoDeliveryOrderItemsDetails = bTODeliveryOrder.bTODeliveryOrderItems;

                var bTODeliveryOrderitemsDto = bTODeliveryOrderDtoUpdate.BTODeliveryOrderItemsDtoUpdate;
                var bTODeliveryOrderitemsList = new List<BTODeliveryOrderItems>();

                if (bTODeliveryOrderitemsDto != null)
                {
                    for (int j = 0; j < getOldbtoDeliveryOrderItemsDetails.Count; j++)
                    {
                        for (int i = 0; i < bTODeliveryOrderitemsDto.Count; i++)
                        {
                            BTODeliveryOrderItems bTODeliveryOrderItems = _mapper.Map<BTODeliveryOrderItems>(bTODeliveryOrderitemsDto[i]);
                            // bTODeliveryOrderItems.BTOSerialNumbers = _mapper.Map<List<BTOSerialNumber>>(bTODeliveryOrderitemsDto[i].BTOSerialNumberDtoUpdate);
                            bTODeliveryOrderitemsList.Add(bTODeliveryOrderItems);

                            //Update Inventory balanced Quantity 

                            //var PartNumber = bTODeliveryOrderitemsDto[i].FGItemNumber;
                            //var getInventoryDetails = await _inventoryRepository.GetInventoryDetails(PartNumber);
                            //decimal Quantity = Convert.ToDecimal(bTODeliveryOrderitemsDto[i].DispatchQty);
                            //if (getInventoryDetails != null)
                            //{
                            //    if (getOldbtoDeliveryOrderItemsDetails[j].DispatchQty > Quantity && Quantity !=0)
                            //    {
                            //        var diff = getOldbtoDeliveryOrderItemsDetails[j].DispatchQty - Quantity;

                            //            getInventoryDetails.Balance_Quantity = getInventoryDetails.Balance_Quantity + diff;

                            //            if (getInventoryDetails.Balance_Quantity != 0)
                            //            {
                            //                getInventoryDetails.IsStockAvailable = true;
                            //            }

                            //        //Add BTO Detail Into Inventory transaction Table

                            //        InventoryTranction inventoryTranction = new InventoryTranction();
                            //        inventoryTranction.PartNumber = bTODeliveryOrderitemsDto[i].FGItemNumber;
                            //        inventoryTranction.MftrPartNumber = bTODeliveryOrderitemsDto[i].FGItemNumber;
                            //        inventoryTranction.Description = bTODeliveryOrderitemsDto[i].Description;
                            //        inventoryTranction.Issued_Quantity = diff;
                            //        inventoryTranction.UOM = bTODeliveryOrderitemsDto[i].UOM;
                            //        inventoryTranction.Issued_DateTime = DateTime.Now;
                            //        inventoryTranction.ReferenceID = bTODeliveryOrder.BTONumber;
                            //        inventoryTranction.ReferenceIDFrom = "Update BTO Delivery Order";
                            //        inventoryTranction.Issued_By = "Admin";
                            //        inventoryTranction.CreatedOn = DateTime.Now;
                            //        inventoryTranction.Unit = "Bangalore";
                            //        inventoryTranction.CreatedBy = "Admin";
                            //        inventoryTranction.LastModifiedBy = "Admin";
                            //        inventoryTranction.LastModifiedOn = DateTime.Now;
                            //        inventoryTranction.ModifiedStatus = false;
                            //        inventoryTranction.From_Location = "FG";
                            //        inventoryTranction.TO_Location = "BTO";
                            //        inventoryTranction.Remarks = "Update,BTO";

                            //        var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


                            //        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                            //        _inventoryTranctionRepository.SaveAsync();
                            //    }
                            //    if (getOldbtoDeliveryOrderItemsDetails[j].DispatchQty < Quantity && Quantity != 0)
                            //    {
                            //        var diff = Quantity - getOldbtoDeliveryOrderItemsDetails[j].DispatchQty;
                            //        if (getInventoryDetails.Balance_Quantity >= diff)
                            //        {
                            //            getInventoryDetails.Balance_Quantity = getInventoryDetails.Balance_Quantity - diff;

                            //            if (getInventoryDetails.Balance_Quantity == 0)
                            //            {
                            //                getInventoryDetails.IsStockAvailable = false;
                            //            }
                            //        }

                            //        //Add BTO Detail Into Inventory transaction Table

                            //        InventoryTranction inventoryTranction = new InventoryTranction();
                            //        inventoryTranction.PartNumber = bTODeliveryOrderItemsListDto[i].FGItemNumber;
                            //        inventoryTranction.MftrPartNumber = bTODeliveryOrderItemsListDto[i].FGItemNumber;
                            //        inventoryTranction.Description = bTODeliveryOrderItemsListDto[i].Description;
                            //        inventoryTranction.Issued_Quantity = diff;
                            //        inventoryTranction.UOM = bTODeliveryOrderItemsListDto[i].UOM;
                            //        inventoryTranction.Issued_DateTime = DateTime.Now;
                            //        inventoryTranction.ReferenceID = bTODeliveryOrder.BTONumber;
                            //        inventoryTranction.ReferenceIDFrom = "Update BTO Delivery Order";
                            //        inventoryTranction.Issued_By = "Admin";
                            //        inventoryTranction.CreatedOn = DateTime.Now;
                            //        inventoryTranction.Unit = "Bangalore";
                            //        inventoryTranction.CreatedBy = "Admin";
                            //        inventoryTranction.LastModifiedBy = "Admin";
                            //        inventoryTranction.LastModifiedOn = DateTime.Now;
                            //        inventoryTranction.ModifiedStatus = false;
                            //        inventoryTranction.From_Location = "FG";
                            //        inventoryTranction.TO_Location = "BTO";
                            //        inventoryTranction.Remarks = "Update,BTO";

                            //        var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


                            //        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                            //        _inventoryTranctionRepository.SaveAsync();
                            //    }                                 

                            //}
                            //_inventoryRepository.Update(getInventoryDetails);
                            //_inventoryRepository.SaveAsync();

                            ////update dispatch qty and balance qty in sales order 

                            //if (getOldbtoDeliveryOrderItemsDetails[j].DispatchQty > Quantity && Quantity != 0)
                            //{
                            //    var diff = getOldbtoDeliveryOrderItemsDetails[j].DispatchQty - Quantity;
                            //    Quantity = 0;
                            //    bTODeliveryOrderitemsDto[i].DispatchQty = diff;
                            //    var btoDeliveryDispatchDetails = _mapper.Map<List<BtoDeliveryOrderDispatchQtyDetailsDto>>(bTODeliveryOrderitemsDto);

                            //    var json = JsonConvert.SerializeObject(btoDeliveryDispatchDetails);
                            //    var data = new StringContent(json, Encoding.UTF8, "application/json");
                            //    var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "UpdateDispatchQtyGreaterThenNewDispatchQty"), data);
                            //}


                            //if (getOldbtoDeliveryOrderItemsDetails[j].DispatchQty < Quantity && Quantity != 0)
                            //{
                            //    var diff = Quantity - getOldbtoDeliveryOrderItemsDetails[j].DispatchQty;
                            //    Quantity = 0;
                            //    bTODeliveryOrderitemsDto[i].DispatchQty = diff;
                            //    var btoDeliveryDispatchDetails = _mapper.Map<List<BtoDeliveryOrderDispatchQtyDetailsDto>>(bTODeliveryOrderitemsDto);

                            //    var json = JsonConvert.SerializeObject(btoDeliveryDispatchDetails);
                            //    var data = new StringContent(json, Encoding.UTF8, "application/json");
                            //    var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "UpdateDispatchQtySmallerThenNewDispatchQty"), data);
                            //}

                            //// Add Bto detail in to btodeliveryorderhistory table

                            //BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                            //bTODeliveryOrderHistory.BTONumber = bTODeliveryOrder.BTONumber;
                            //bTODeliveryOrderHistory.CustomerName = bTODeliveryOrder.CustomerName;
                            //bTODeliveryOrderHistory.CustomerAliasName = bTODeliveryOrder.CustomerAliasName;
                            //bTODeliveryOrderHistory.CustomerId = bTODeliveryOrder.CustomerId;
                            //bTODeliveryOrderHistory.PONumber = bTODeliveryOrder.PONumber;
                            //bTODeliveryOrderHistory.IssuedTo = bTODeliveryOrder.IssuedTo;
                            //bTODeliveryOrderHistory.DODate = bTODeliveryOrder.DODate;
                            //bTODeliveryOrderHistory.FGItemNumber = bTODeliveryOrderItemsListDto[i].FGItemNumber;
                            //bTODeliveryOrderHistory.SalesOrderId = bTODeliveryOrderItemsListDto[i].SalesOrderId;
                            //bTODeliveryOrderHistory.Description = bTODeliveryOrderItemsListDto[i].Description;
                            //bTODeliveryOrderHistory.BalanceDoQty = bTODeliveryOrderItemsListDto[i].BalanceDoQty;
                            //bTODeliveryOrderHistory.UnitPrice = bTODeliveryOrderItemsListDto[i].UnitPrice;
                            //bTODeliveryOrderHistory.UOC = bTODeliveryOrderItemsListDto[i].UOC;
                            //bTODeliveryOrderHistory.UOM = bTODeliveryOrderItemsListDto[i].UOM;
                            //bTODeliveryOrderHistory.FGOrderQty = bTODeliveryOrderItemsListDto[i].FGOrderQty;
                            //bTODeliveryOrderHistory.OrderBalanceQty = bTODeliveryOrderItemsListDto[i].OrderBalanceQty;
                            //bTODeliveryOrderHistory.FGStock = bTODeliveryOrderItemsListDto[i].FGStock;
                            //bTODeliveryOrderHistory.Discount = bTODeliveryOrderItemsListDto[i].Discount;
                            //bTODeliveryOrderHistory.NetValue = bTODeliveryOrderItemsListDto[i].NetValue;
                            //bTODeliveryOrderHistory.DispatchQty = bTODeliveryOrderItemsListDto[i].DispatchQty;
                            //bTODeliveryOrderHistory.InvoicedQty = bTODeliveryOrderItemsListDto[i].InvoicedQty;
                            //bTODeliveryOrderHistory.SerialNo = bTODeliveryOrderItemsListDto[i].SerialNo;
                            //bTODeliveryOrderHistory.CreatedBy = bTODeliveryOrderItemsListDto[i].CreatedBy;
                            //bTODeliveryOrderHistory.LastModifiedOn = bTODeliveryOrderItemsListDto[i].LastModifiedOn;
                            //bTODeliveryOrderHistory.Remark = "From Update BTO";


                            //var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);


                            //await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);
                            //_bTODeliveryOrderHistoryRepository.SaveAsync();
                        }
                    }
                }

                bTODeliveryOrder.bTODeliveryOrderItems = bTODeliveryOrderitemsList;
                var updateBTODeliveryOrder = _mapper.Map(bTODeliveryOrderDtoUpdate, bTODeliveryOrder);

                string result = await _repository.UpdateBTODeliveryOrder(updateBTODeliveryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateBTODeliveryOrder API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateBTODeliveryOrder API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBTODeliveryOrder(int id)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var deleteBTODeliveryOrder = await _repository.GetBTODeliveryOrderById(id);
                if (deleteBTODeliveryOrder == null)
                {
                    _logger.LogError($"Delete BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete BTODeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteBTODeliveryOrder(deleteBTODeliveryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteBTODeliveryOrder API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteBTODeliveryOrder API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetBtoDeliveryOrderNumberList()
        {
            ServiceResponse<IEnumerable<ListofBtoDeliveryOrderDetails>> serviceResponse = new ServiceResponse<IEnumerable<ListofBtoDeliveryOrderDetails>>();

            try
            {
                var getBtoDeliveryOrderNumberList = await _repository.GetBtoDeliveryOrderNumberList();
                if (getBtoDeliveryOrderNumberList == null)
                {
                    _logger.LogError("BtoDeliveryOrderDetail Not Found");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BtoDeliveryOrderDetail Not Found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo("Return BtoDeliveryOrderDetail");
                    var result = _mapper.Map<IEnumerable<ListofBtoDeliveryOrderDetails>>(getBtoDeliveryOrderNumberList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBtoDeliveryOrderNumberList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBtoDeliveryOrderNumberList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetInventoryListByItemNo(string ItemNumber)
        {
            ServiceResponse<IEnumerable<GetInventoryListByItemNo>> serviceResponse = new ServiceResponse<IEnumerable<GetInventoryListByItemNo>>();

            try
            {
                var getInventoryListByItemNo = await _inventoryRepository.GetInventoryListByItemNo(ItemNumber);
                if (getInventoryListByItemNo == null)
                {
                    _logger.LogError($"InventoryDetails with id: {ItemNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryDetails with id: {ItemNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned InventoryDetails with id: {ItemNumber}");
                    var result = _mapper.Map<IEnumerable<GetInventoryListByItemNo>>(getInventoryListByItemNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetInventoryListByItemNo API for the following ItemNumber:{ItemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetInventoryListByItemNo API for the following ItemNumber:{ItemNumber} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBTOIdNameIdNameList()
        {
            ServiceResponse<IEnumerable<BtoIDNameList>> serviceResponse = new ServiceResponse<IEnumerable<BtoIDNameList>>();
            try
            {
                var listOfAllBtoIdNames = await _repository.GetAllBTOIdNameIdNameList();
                var result = _mapper.Map<IEnumerable<BtoIDNameList>>(listOfAllBtoIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All GetAllBTOIdNameIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllBTOIdNameIdNameList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllBTOIdNameIdNameList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSalesOrderNoAndIdByBTONo(string btoNumber)
        {
            ServiceResponse<SalesOrderNoandIdDto> serviceResponse = new ServiceResponse<SalesOrderNoandIdDto>();
            try
            {
                var listOfAllBtoIdNames = await _repository.GetAllSalesOrderNoAndIdByBTONo(btoNumber);
                var result = _mapper.Map<SalesOrderNoandIdDto>(listOfAllBtoIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All SalesOrderNoAndIdList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetSalesOrderNoAndIdByBTONo API for the following btoNumber:{btoNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetSalesOrderNoAndIdByBTONo API for the following btoNumber:{btoNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBTODeliveryOrderByIdExcludingClosed(int id)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var getBTODeliveryOrderDetailById = await _repository.GetBTODeliveryOrderByIdExcludingClosed(id);

                if (getBTODeliveryOrderDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BTODeliveryOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");

                    BTODeliveryOrderDto bTODeliveryOrderDto = _mapper.Map<BTODeliveryOrderDto>(getBTODeliveryOrderDetailById);

                    List<BTODeliveryOrderItemsDto> bTODeliveryOrderItemsDtoList = new List<BTODeliveryOrderItemsDto>();

                    if (getBTODeliveryOrderDetailById.bTODeliveryOrderItems != null)
                    {

                        foreach (var deliveryOrderitemDetails in getBTODeliveryOrderDetailById.bTODeliveryOrderItems)
                        {
                            BTODeliveryOrderItemsDto bTODeliveryOrderItemsDtos = _mapper.Map<BTODeliveryOrderItemsDto>(deliveryOrderitemDetails);
                            bTODeliveryOrderItemsDtos.QtyDistribution = _mapper.Map<List<BtoDeliveryOrderItemQtyDistributionDto>>(deliveryOrderitemDetails.QtyDistribution);
                           // bTODeliveryOrderItemsDtos.DoStatus = Status.Open;
                            //bTODeliveryOrderItemsDtos.BTOSerialNumberDto = _mapper.Map<List<BTOSerialNumberDto>>(deliveryOrderitemDetails.BTOSerialNumbers);
                            bTODeliveryOrderItemsDtoList.Add(bTODeliveryOrderItemsDtos);
                        }
                    }
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    //var salesOrderObjectResult = await _httpClient.GetAsync(string.Concat(_config["SalesOrderAPI"],
                    //                         "GetSalesOrderTotalBySalesOrderId?", "&SalesOrderId=", bTODeliveryOrderDto.SalesOrderId));
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["SalesOrderAPI"],
                           $"GetSalesOrderTotalBySalesOrderId?SalesOrderId={bTODeliveryOrderDto.SalesOrderId}"));
                    request.Headers.Add("Authorization", token);

                    var salesOrderObjectResult = await client.SendAsync(request);
                    var salesOrderObjectString = await salesOrderObjectResult.Content.ReadAsStringAsync();
                    dynamic salesOrderObjectData = JsonConvert.DeserializeObject(salesOrderObjectString);
                    dynamic salesOrderObject = salesOrderObjectData;
                    decimal salesOrderTotal = Convert.ToDecimal(salesOrderObject);
                    bTODeliveryOrderDto.SOTotal = salesOrderTotal;

                    bTODeliveryOrderDto.bTODeliveryOrderItems = bTODeliveryOrderItemsDtoList;

                    serviceResponse.Data = bTODeliveryOrderDto;
                    serviceResponse.Message = "Returned BTODeliveryOrderById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBTODeliveryOrderByIdExcludingClosed API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBTODeliveryOrderByIdExcludingClosed API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBtoNumberListByCustomerIdExcludingClosed(string customerLeadId)
        {
            ServiceResponse<IEnumerable<ListOfBtoNumberDetails>> serviceResponse = new ServiceResponse<IEnumerable<ListOfBtoNumberDetails>>();
            try
            {
                var getBTONumberList = await _repository.GetBtoNumberListByCustomerIdExcludingClosed(customerLeadId);

                if (getBTONumberList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BTODeliveryOrderNumbers not found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GetBtoNumberListByCustomerIdExcludingClosed with id: {customerLeadId},not found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {customerLeadId}");

                    var bTODeliveryNumberList = _mapper.Map<IEnumerable<ListOfBtoNumberDetails>>(getBTONumberList);


                    serviceResponse.Data = bTODeliveryNumberList;
                    serviceResponse.Message = "Returned GetBtoNumberListByCustomerIdExcludingClosed Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBtoNumberListByCustomerIdExcludingClosed API for the following customerLeadId:{customerLeadId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBtoNumberListByCustomerIdExcludingClosed API for the following customerLeadId:{customerLeadId} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDOLotNumberListByBTONoAndItemNo(string btoNumber, string itemNumber)
        {
            ServiceResponse<IEnumerable<DoLotNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<DoLotNumberListDto>>();
            try
            {
                var doLotNumberList = await _repository.GetDOLotNumberListByBTONoAndItemNo(btoNumber, itemNumber);
                var result = _mapper.Map<IEnumerable<DoLotNumberListDto>>(doLotNumberList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All DOLotNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDOLotNumberListByBTONoAndItemNo API for the following btoNumber:{btoNumber} and itemNumber : {itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDOLotNumberListByBTONoAndItemNo API for the following btoNumber:{btoNumber} and itemNumber : {itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }

}
