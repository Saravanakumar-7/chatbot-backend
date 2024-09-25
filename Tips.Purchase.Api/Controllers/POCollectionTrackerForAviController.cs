using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;
namespace Tips.Purchase.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class POCollectionTrackerForAviController : ControllerBase
    {
        private IPOCollectionTrackerForAviRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IPurchaseOrderRepository _PoRepository;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        public POCollectionTrackerForAviController(IPOCollectionTrackerForAviRepository repository, IConfiguration config, IHttpClientFactory clientFactory, IPurchaseOrderRepository PoRepository, ILoggerManager logger, IMapper mapper)
        {
            _config = config;
            _clientFactory = clientFactory;
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _PoRepository = PoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPO_GRIN_IQC_POBreakDownDetailsBYPONo([FromQuery] string Ponumber)
        {
            ServiceResponse<PO_GRIN_IQC_POBreakDownDetailsDto> serviceResponse = new ServiceResponse<PO_GRIN_IQC_POBreakDownDetailsDto>();

            try
            {
                if (Ponumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Ponumber object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Ponumber object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Ponumber object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Ponumber object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var podetails = await _PoRepository.GetLastestPurchaseOrderByPONumber(Ponumber);
                podetails.POIncoTerms.ForEach(x => x.PurchaseOrder = null);
                podetails.POItems.ForEach(x =>
                {
                    x.PurchaseOrder = null;
                    x.POAddprojects.ForEach(z => z.POItemDetail = null);
                    x.POAddDeliverySchedules.ForEach(z => z.POItemDetail = null);
                    x.POSpecialInstructions.ForEach(z => z.POItemDetail = null);
                    x.POConfirmationDates.ForEach(z => z.POItemDetail = null);
                    x.PrDetails.ForEach(z => z.POItemDetail = null);
                });
                var FinalReportData = new PO_GRIN_IQC_POBreakDownDetailsDto();
                FinalReportData.PurchaseOrder = podetails;
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                var encodedPONumber = Uri.EscapeDataString(Ponumber);

                if (podetails.ProcurementType != "Service")
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["GRINService"],
                        $"Grin/GetGrinAndIqcsByPurchaseOrder?Ponumber={encodedPONumber}"));
                    request.Headers.Add("Authorization", token);
                    var GRINandIQCObjectResult = await client.SendAsync(request);
                    if (GRINandIQCObjectResult.StatusCode == HttpStatusCode.OK)
                    {
                        var GRINandIQCObjectString = await GRINandIQCObjectResult.Content.ReadAsStringAsync();
                        var GRINandIQCObjectData = JsonConvert.DeserializeObject<GrinandIqcDetail>(GRINandIQCObjectString);
                        FinalReportData.Grins = GRINandIQCObjectData.Data.Grins;
                        FinalReportData.Iqcs = GRINandIQCObjectData.Data.Iqcs;
                    }
                }
                else
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["GRINService"],
                        $"GrinsForServiceItems/GetGrinForServiceItemsAndIqcsForServiceItemsByPurchaseOrder?Ponumber={encodedPONumber}"));
                    request.Headers.Add("Authorization", token);
                    var GRINForServiceItemsandIQCForServiceItemsObjectResult = await client.SendAsync(request);
                    if (GRINForServiceItemsandIQCForServiceItemsObjectResult.StatusCode == HttpStatusCode.OK)
                    {
                        var GRINandIQCObjectString = await GRINForServiceItemsandIQCForServiceItemsObjectResult.Content.ReadAsStringAsync();
                        var GRINandIQCObjectData = JsonConvert.DeserializeObject<GrinForServiceItemsandIqcForServiceItemsDetail>(GRINandIQCObjectString);
                        FinalReportData.grinsForServiceItems = GRINandIQCObjectData.Data.grinsForServiceItems;
                        FinalReportData.iqcsForServiceItems = GRINandIQCObjectData.Data.iqcsForServiceItems;
                    }
                }
                var breakdownDetails = await _repository.GetAllPOCollectionTrackersForAviByPonumber(Ponumber);
                breakdownDetails.ForEach(x => x.POBreakDownForAvi.ForEach(z => z.POCollectionTrackerForAvi = null));
                FinalReportData.POCollectionTrackers = breakdownDetails;
                serviceResponse.Data = FinalReportData;
                serviceResponse.Message = "Returned GetPO_GRIN_IQC_POBreakDownDetailsBYPONo Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"There was An Issue in GetPO_GRIN_IQC_POBreakDownDetailsBYPONo : {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPOCollectionTrackerForAvi([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<POCollectionTrackerForAviDto>> serviceResponse = new ServiceResponse<IEnumerable<POCollectionTrackerForAviDto>>();

            try
            {
                var pocollectionTrackerDetails = await _repository.GetAllPOCollectionTrackersForAvi(pagingParameter, searchParamess);

                var metadata = new
                {
                    pocollectionTrackerDetails.TotalCount,
                    pocollectionTrackerDetails.PageSize,
                    pocollectionTrackerDetails.CurrentPage,
                    pocollectionTrackerDetails.HasNext,
                    pocollectionTrackerDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all CollectionTracker");
                var result = _mapper.Map<IEnumerable<POCollectionTrackerForAviDto>>(pocollectionTrackerDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all POCollectionTracker Successfully";
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPOCollectionTrackerForAviById(int id)
        {
            ServiceResponse<POCollectionTrackerForAviDto> serviceResponse = new ServiceResponse<POCollectionTrackerForAviDto>();
            try
            {
                var pocollectionTrackerById = await _repository.GetPOCollectionTrackerForAviById(id);

                if (pocollectionTrackerById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"POCollectionTracker with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"POCollectionTracker with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned POCollectionTracker with id: {id}");
                    var result = _mapper.Map<POCollectionTrackerForAviDto>(pocollectionTrackerById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned POCollectionTrackerById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPOCollectionTrackerById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetPOCollectionTrackerById action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetPOCollectionTrackerForAviByVendorId(string vendorId)
        {
            ServiceResponse<POCollectionTrackerForAviDetailsDto> serviceResponse = new ServiceResponse<POCollectionTrackerForAviDetailsDto>();
            try
            {
                var pocollectionTrackerById = await _repository.GetPOCollectionTrackerForAviByVendorId(vendorId);
                var openPurchaseOrderDetails = await _repository.GetOpenPOForAviDetailsByVendorId(vendorId);
                pocollectionTrackerById.OpenPurchaseOrderForAviDetails = openPurchaseOrderDetails;
                if (pocollectionTrackerById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"POCollectionTracker with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"POCollectionTracker with id: {vendorId}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned POCollectionTracker with id: {vendorId}");
                    var result = _mapper.Map<POCollectionTrackerForAviDetailsDto>(pocollectionTrackerById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned POCollectionTrackerById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPOCollectionTrackerById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetPOCollectionTrackerById action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchPOCollectionTrackerForAviDate([FromQuery] SearchDatesParams searchDatesParams)
        {
            ServiceResponse<IEnumerable<POCollectionTrackerForAviDto>> serviceResponse = new ServiceResponse<IEnumerable<POCollectionTrackerForAviDto>>();
            try
            {
                var poCollectionTrackerDetails = await _repository.SearchPOCollectionTrackerForAviDate(searchDatesParams);
                var result = _mapper.Map<IEnumerable<POCollectionTrackerForAviDto>>(poCollectionTrackerDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all POCollectionTracker By Date";
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
        public async Task<IActionResult> SearchPOCollectionTrackerForAvi([FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<POCollectionTrackerForAviDto>> serviceResponse = new ServiceResponse<IEnumerable<POCollectionTrackerForAviDto>>();
            try
            {
                var poCollectionTrackerDetails = await _repository.SearchPOCollectionTrackerForAvi(searchParamess);

                _logger.LogInfo("Returned all POCollectionTracker");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<POCollectionTrackerForAvi, POCollectionTrackerForAviDto>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<POCollectionTrackerForAviDto>>(poCollectionTrackerDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all POCollectionTrackerDetails";
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

        [HttpPost]
        public async Task<IActionResult> GetAllPOCollectionTrackerForAviWithItems([FromBody] POCollectionTrackerForAviSearchDto poCollectionTrackerSearch)
        {
            ServiceResponse<IEnumerable<POCollectionTrackerForAviDto>> serviceResponse = new ServiceResponse<IEnumerable<POCollectionTrackerForAviDto>>();
            try
            {
                var poCollectionTrackerDetails = await _repository.GetAllPOCollectionTrackerForAviWithItems(poCollectionTrackerSearch);

                _logger.LogInfo("Returned all POCollectionTracker");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<POCollectionTrackerForAvi, POCollectionTrackerForAviDto>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<POCollectionTrackerForAviDto>>(poCollectionTrackerDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all POCollectionTrackerDetails";
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

        [HttpPost]
        public async Task<IActionResult> CreatePOCollectionTrackerForAvi([FromBody] POCollectionTrackerForAviPostDto pocollectionTrackerPostDto)
        {
            ServiceResponse<POCollectionTrackerForAviDto> serviceResponse = new ServiceResponse<POCollectionTrackerForAviDto>();

            try
            {
                if (pocollectionTrackerPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "POCollectionTracker object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("POCollectionTracker object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid POCollectionTracker object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid POCollectionTracker object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var pocollectionTrackerItems = _mapper.Map<IEnumerable<POBreakDownForAvi>>(pocollectionTrackerPostDto.POBreakDownForAvi);

                var pocollectionTracker = _mapper.Map<POCollectionTrackerForAvi>(pocollectionTrackerPostDto);

                pocollectionTracker.POBreakDownForAvi = pocollectionTrackerItems.ToList();

                await _repository.CreatePOCollectionTrackerForAvi(pocollectionTracker);

                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "POCollectionTracker Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.Created;
                return Created("GetPOCollectionTracker", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreatePOCollectionTracker action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdatePOCollectionTrackerForAvi(int id, [FromBody] POCollectionTrackerForAviUpdateDto pocollectionTrackerUpdateDto)
        {
            ServiceResponse<POCollectionTrackerForAviUpdateDto> serviceResponse = new ServiceResponse<POCollectionTrackerForAviUpdateDto>();
            try
            {
                if (pocollectionTrackerUpdateDto is null)
                {
                    _logger.LogError("Update POCollectionTracker object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update POCollectionTracker object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update POCollectionTracker object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update POCollectionTracker object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest("Invalid model object");
                }
                var pocollectionTrackerById = await _repository.GetPOCollectionTrackerForAviById(id);
                if (pocollectionTrackerById is null)
                {
                    _logger.LogError($"Update POCollectionTracker with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update POCollectionTracker with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var pocollectionTrackerItems = _mapper.Map<IEnumerable<POBreakDownForAvi>>(pocollectionTrackerUpdateDto.POBreakDownForAvi);

                var pocollectionTracker = _mapper.Map(pocollectionTrackerUpdateDto, pocollectionTrackerById);

                pocollectionTracker.POBreakDownForAvi = pocollectionTrackerItems.ToList();

                string result = await _repository.UpdatePOCollectionTrackerForAvi(pocollectionTracker);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " POCollectionTracker Successfully Updated"; ;
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdatePOCollectionTracker action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePOCollectionTrackerForAvi(int id)
        {
            ServiceResponse<POCollectionTrackerForAviDto> serviceResponse = new ServiceResponse<POCollectionTrackerForAviDto>();
            try
            {
                var pocollectionTrackerById = await _repository.GetPOCollectionTrackerForAviById(id);
                if (pocollectionTrackerById == null)
                {
                    _logger.LogError($"Delete POCollectionTracker with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete POCollectionTracker with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeletePOCollectionTrackerForAvi(pocollectionTrackerById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " POCollectionTracker Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeletePOCollectionTracker action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetPayableSPReportWithParam([FromBody] PayableSPReportWithParamDTO purchaseOrderApprovalSPReport)

        {
            ServiceResponse<IEnumerable<PayableSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PayableSPReport>>();
            try
            {
                var products = await _repository.GetPayableSPReportWithParam(purchaseOrderApprovalSPReport.PONumber, purchaseOrderApprovalSPReport.VendorName,
                                                                                    purchaseOrderApprovalSPReport.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Payable hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Payable hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PayableSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetPayableSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetPayableSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<PayableSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PayableSPReport>>();
            try
            {
                var products = await _repository.GetPayableSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Payable hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Payable hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PayableSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetPayableSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
