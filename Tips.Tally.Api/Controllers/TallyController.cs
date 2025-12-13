using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.Tally.Api.Contracts;
using Tips.Tally.Api.Entities.DTOs;

namespace Tips.Tally.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TallyController : ControllerBase
    {
        private ITallyRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IConfiguration _config;       

        public TallyController(ITallyRepository repository, ILoggerManager logger, IMapper mapper, IConfiguration config)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _config = config;
        }
        [HttpGet()] // Adjust your route as needed
        public async Task<IActionResult> GetTallyVendorMasterSpReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<TallyVendorMasterSpReportDto>> serviceResponse = new ServiceResponse<IEnumerable<TallyVendorMasterSpReportDto>>();
            try
            {
                var products = await _repository.GetTallyVendorMasterSpReportWithDate(FromDate, ToDate);

                if (products == null || !products.Any())
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No TallyVendorMasterSpReportDto records found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"No TallyVendorMasterSpReportDto records found in DB.");
                    return NotFound(serviceResponse);
                }

                var result = products.Select(product =>
                {
                    var dto = _mapper.Map<TallyVendorMasterSpReportDto>(product);

                    if (!string.IsNullOrEmpty(product.GSTINNo))
                    {
                        dto.GSTINNo = JsonConvert.DeserializeObject<List<GSTINNosDto>>(product.GSTINNo).OrderBy(x => x.GSTNNumber).ToList();
                    }

                    return dto;
                }).ToList();

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned TallyVendorMasterSpReportDto Details";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetTallyVendorMasterSpReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetTallyVendorMasterSpReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet()] // Adjust your route as needed
        public async Task<IActionResult> GetTallyCustomerMastertSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<TallyCustomerMasterSpReportDto>> serviceResponse = new ServiceResponse<IEnumerable<TallyCustomerMasterSpReportDto>>();
            try
            {
                var products = await _repository.GetTallyCustomerMastertSPReportWithDate(FromDate, ToDate);

                if (products == null || !products.Any())
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No TallyCustomerMasterSpReportDto records found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"No TallyCustomerMasterSpReportDto records found in DB.");
                    return NotFound(serviceResponse);
                }

                var result = products.Select(product =>
                {
                    var dto = _mapper.Map<TallyCustomerMasterSpReportDto>(product);

                    if (!string.IsNullOrEmpty(product.GSTINNo))
                    {
                        dto.GSTINNo = JsonConvert.DeserializeObject<List<GSTINNoDto>>(product.GSTINNo).ToList();
                    }

                    return dto;
                }).ToList();

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned TallyCustomerMasterSpReportDto Details";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetTallyCustomerMastertSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetTallyCustomerMastertSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet()] // Adjust your route as needed
        public async Task<IActionResult> GetTallyCurrencyMastertSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<TallyCurrencyMasterSPReport>> serviceResponse = new ServiceResponse<IEnumerable<TallyCurrencyMasterSPReport>>();
            try
            {
                var result = await _repository.GetTallyCurrencyMastertSPReportWithDate(FromDate, ToDate);

                if (result == null || !result.Any())
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"TallyCurrencyMasterSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"TallyCurrencyMasterSPReport hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned TallyCurrencyMasterSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetTallyCurrencyMastertSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetTallyCurrencyMastertSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetTallyPurchaseOrderSpReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<TallyPurchaseOrderSpReportDto>> serviceResponse = new ServiceResponse<IEnumerable<TallyPurchaseOrderSpReportDto>>();

            try
            {
                var reports = await _repository.GetTallyPurchaseOrderSpReportWithDate(FromDate, ToDate);

                if (reports == null || !reports.Any())
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No TallyPurchaseOrderSpReport records found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"No TallyPurchaseOrderSpReport records found in DB.");
                    return NotFound(serviceResponse);
                }

                var result = reports.Select(report =>
                {
                    var dto = _mapper.Map<TallyPurchaseOrderSpReportDto>(report);

                    // ✅ Deserialize GSTIN_No JSON
                    if (!string.IsNullOrWhiteSpace(report.GSTIN_No))
                    {
                        dto.GSTIN_No = JsonConvert
                            .DeserializeObject<List<GSTINNoDtos>>(report.GSTIN_No);
                    }

                    // ✅ Deserialize POData JSON
                    if (!string.IsNullOrWhiteSpace(report.POData))
                    {
                        dto.POData = JsonConvert
                            .DeserializeObject<List<PODataDtos>>(report.POData)
                            ?.OrderBy(x => x.ItemCode)
                            .ToList();
                    }

                    return dto;
                }).ToList();

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned TallyPurchaseOrderSpReport Details";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetTallyPurchaseOrderSpReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetTallyPurchaseOrderSpReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet()] // Adjust your route as needed
        public async Task<IActionResult> GetTallyStockItemSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<TallyStockItemSPReport>> serviceResponse = new ServiceResponse<IEnumerable<TallyStockItemSPReport>>();
            try
            {
                var result = await _repository.GetTallyStockItemSPReportWithDate(FromDate, ToDate);

                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"TallyStockItemSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"TallyStockItemSPReport hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned TallyStockItemSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetTallyStockItemSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetTallyStockItemSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTallybtodeliveryorderSpReportWIthDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<TallybtodeliveryorderSpReportDto>> serviceResponse = new ServiceResponse<IEnumerable<TallybtodeliveryorderSpReportDto>>();

            try
            {
                var products = await _repository.GetTallybtodeliveryorderSpReportWIthDate(FromDate, ToDate);

                if (products == null || !products.Any())
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No TallybtodeliveryorderSpReportDto records found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"No TallybtodeliveryorderSpReportDto records found in DB.");
                    return NotFound(serviceResponse);
                }

                var result = products.Select(product =>
                {
                    var dto = _mapper.Map<TallybtodeliveryorderSpReportDto>(product);

                    if (!string.IsNullOrEmpty(product.SalesOrderData))
                    {
                        dto.SalesOrderData = JsonConvert.DeserializeObject<List<SalesOrderItemDto>>(product.SalesOrderData)
                            .ToList();
                    }

                    return dto;
                }).ToList();

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned TallybtodeliveryorderSpReportDto Details";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in TallybtodeliveryorderSpReportWIthDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in TallybtodeliveryorderSpReportWIthDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet()] // Adjust your route as needed
        public async Task<IActionResult> GetTallyFGWIPMaterialIssueSpReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<TallyFGWIPMaterialIssueSpReport>> serviceResponse = new ServiceResponse<IEnumerable<TallyFGWIPMaterialIssueSpReport>>();
            try
            {
                var result = await _repository.GetTallyFGWIPMaterialIssueSpReportWithDate(FromDate, ToDate);

                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"TallyFGWIPMaterialIssueSpReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"TallyFGWIPMaterialIssueSpReport hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned TallyFGWIPMaterialIssueSpReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in TallyFGWIPMaterialIssueSpReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in TallyFGWIPMaterialIssueSpReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetTallyGrinSpReportSpReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<TallyGrinSpReportDto>> serviceResponse = new ServiceResponse<IEnumerable<TallyGrinSpReportDto>>();

            try
            {
                var products = await _repository.GetTallyGrinSpReportSpReportWithDate(FromDate, ToDate);

                if (products == null || !products.Any())
                {
                    serviceResponse.Message = "No TallyGrinSpReportDto records found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var result = products.Select(product =>
                {
                    var dto = _mapper.Map<TallyGrinSpReportDto>(product);

                    try
                    {
                        if (!string.IsNullOrEmpty(product.GRINParts))
                            dto.GRINParts = JsonConvert.DeserializeObject<List<GRINPartDto>>(product.GRINParts) ?? new();
                        else
                            dto.GRINParts = new();
                    }
                    catch (JsonException jex)
                    {
                        _logger.LogError($"Invalid JSON in GRINParts: {jex.Message}");
                        dto.GRINParts = new();
                    }

                    return dto;
                }).ToList();

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned TallyGrinSpReportSpReportWithDate Details";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in API: {ex}");
                serviceResponse.Message = $"Error: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTallySalesOrderSpReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<TallySalesOrderSpReportDto>> serviceResponse = new ServiceResponse<IEnumerable<TallySalesOrderSpReportDto>>();

            try
            {
                var products = await _repository.GetTallySalesOrderSpReportWithDate(FromDate, ToDate);

                if (products == null || !products.Any())
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No TallySalesOrderSpReportDto records found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"No TallySalesOrderSpReportDto records found in DB.");
                    return NotFound(serviceResponse);
                }

                var result = products.Select(product =>
                {
                    var dto = _mapper.Map<TallySalesOrderSpReportDto>(product);

                    if (!string.IsNullOrEmpty(product.SalesOrderItems))
                    {
                        dto.SalesOrderItems = JsonConvert.DeserializeObject<List<SalesOrderItemDtos>>(product.SalesOrderItems)
                            .ToList();
                    }

                    return dto;
                }).ToList();

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned GetTallySalesOrderSpReportWithDate Details";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetTallySalesOrderSpReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetTallySalesOrderSpReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
