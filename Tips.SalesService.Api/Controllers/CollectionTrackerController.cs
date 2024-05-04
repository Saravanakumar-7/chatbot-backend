using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CollectionTrackerController : ControllerBase
    {
        private ICollectionTrackerRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly IHttpClientFactory _clientFactory;
        public CollectionTrackerController(ICollectionTrackerRepository repository, ILoggerManager logger, IHttpClientFactory clientFactory, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCollectionTracker([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<CollectionTrackerDto>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerDto>>();

            try
            {
                var collectionTrackerDetails = await _repository.GetAllCollectionTrackers(pagingParameter, searchParammes);

                var metadata = new
                {
                    collectionTrackerDetails.TotalCount,
                    collectionTrackerDetails.PageSize,
                    collectionTrackerDetails.CurrentPage,
                    collectionTrackerDetails.HasNext,
                    collectionTrackerDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all CollectionTracker");
                var result = _mapper.Map<IEnumerable<CollectionTrackerDto>>(collectionTrackerDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CollectionTracker Successfully";
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
        public async Task<IActionResult> GetCollectionTrackerById(int id)
        {
            ServiceResponse<CollectionTrackerDto> serviceResponse = new ServiceResponse<CollectionTrackerDto>();
            try
            {
                var collectionTrackerById = await _repository.GetCollectionTrackerById(id);

                if (collectionTrackerById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTracker with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTracker with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CollectionTracker with id: {id}");
                    var result = _mapper.Map<CollectionTrackerDto>(collectionTrackerById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CollectionTrackerById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCollectionTrackerById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerById action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCollectionTrackerByCustomerId(string customerId)
        {
            ServiceResponse<CollectionTrackerDetailsDto> serviceResponse = new ServiceResponse<CollectionTrackerDetailsDto>();
            try
            {
                var collectionTrackerById = await _repository.GetSOCollectionTrackerByCustomerId(customerId);
                var openSalesOrderDetails = await _repository.GetOpenSODetailsByCustomerId(customerId);
                collectionTrackerById.OpenSalesOrderDetails = openSalesOrderDetails;
                if (collectionTrackerById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTracker with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTracker with id: {customerId}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CollectionTracker with id: {customerId}");
                    var result = _mapper.Map<CollectionTrackerDetailsDto>(collectionTrackerById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CollectionTrackerById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCollectionTrackerById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerById action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchCollectionTrackerDate([FromQuery] SearchDateParam searchDateParam)
        {
            ServiceResponse<IEnumerable<CollectionTrackerDto>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerDto>>();
            try
            {
                var collectionTrackerDetails = await _repository.SearchCollectionTrackerDate(searchDateParam);
                var result = _mapper.Map<IEnumerable<CollectionTrackerDto>>(collectionTrackerDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CollectionTracker By Date";
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
        public async Task<IActionResult> SearchCollectionTracker([FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<CollectionTrackerDto>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerDto>>();
            try
            {
                var collectionTrackerDetails = await _repository.SearchCollectionTracker(searchParammes);

                _logger.LogInfo("Returned all CollectionTracker");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<CollectionTracker, CollectionTrackerDto>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<CollectionTrackerDto>>(collectionTrackerDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CollectionTrackerDetails";
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
        public async Task<IActionResult> GetAllCollectionTrackerWithItems([FromBody] CollectionTrackerSearchDto collectionTrackerSearch)
        {
            ServiceResponse<IEnumerable<CollectionTrackerDto>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerDto>>();
            try
            {
                var collectionTrackerDetails = await _repository.GetAllCollectionTrackerWithItems(collectionTrackerSearch);

                _logger.LogInfo("Returned all CollectionTracker");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<CollectionTracker, CollectionTrackerDto>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<CollectionTrackerDto>>(collectionTrackerDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CollectionTrackerDetails";
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
        public async Task<IActionResult> CreateCollectionTracker([FromBody] CollectionTrackerPostDto collectionTrackerPostDto)
        {
            ServiceResponse<CollectionTrackerDto> serviceResponse = new ServiceResponse<CollectionTrackerDto>();
            
            try
            {
                if (collectionTrackerPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CollectionTracker object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("CollectionTracker object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CollectionTracker object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid CollectionTracker object sent from client.");
                    return BadRequest("Invalid model object");
                }
                 
                var collectionTrackerItems = _mapper.Map<IEnumerable<SOBreakDown>>(collectionTrackerPostDto.SOBreakDown);
                 
                 var collectionTracker = _mapper.Map<CollectionTracker>(collectionTrackerPostDto);

                collectionTracker.SOBreakDown = collectionTrackerItems.ToList();
               
                await _repository.CreateCollectionTracker(collectionTracker);

                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "CollectionTracker Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.Created;
                return Created("GetCollectionTracker", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateCollectionTracker action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateCollectionTracker(int id, [FromBody] CollectionTrackerUpdateDto collectionTrackerUpdateDto)
        {
            ServiceResponse<CollectionTrackerUpdateDto> serviceResponse = new ServiceResponse<CollectionTrackerUpdateDto>();
            try
            {
                if (collectionTrackerUpdateDto is null)
                {
                    _logger.LogError("Update CollectionTracker object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update CollectionTracker object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update CollectionTracker object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update CollectionTracker object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest("Invalid model object");
                }
                var collectionTrackerById = await _repository.GetCollectionTrackerById(id);
                if (collectionTrackerById is null)
                {
                    _logger.LogError($"Update CollectionTracker with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update CollectionTracker with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var collectionTrackerItems = _mapper.Map<IEnumerable<SOBreakDown>>(collectionTrackerUpdateDto.SOBreakDown);

                var collectionTracker = _mapper.Map(collectionTrackerUpdateDto, collectionTrackerById);

                collectionTracker.SOBreakDown = collectionTrackerItems.ToList();

                string result = await _repository.UpdateCollectionTracker(collectionTracker);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CollectionTracker Successfully Updated"; ;
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCollectionTracker action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollectionTracker(int id)
        {
            ServiceResponse<CollectionTrackerDto> serviceResponse = new ServiceResponse<CollectionTrackerDto>();
            try
            {
                var collectionTrackerById = await _repository.GetCollectionTrackerById(id);
                if (collectionTrackerById == null)
                {
                    _logger.LogError($"Delete CollectionTracker with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete CollectionTracker with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteCollectionTracker(collectionTrackerById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CollectionTracker Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteCollectionTracker action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetCollectionTrackerSPReportWithParam([FromBody] CollectionTrackerSPResportSPResportDTO collectionTrackerSPResportSPResportDTO)

        {
            ServiceResponse<IEnumerable<CollectionTrackerSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerSPReport>>();
            try
            {
                var collectionTrackerSPReports = await _repository.GetCollectionTrackerSPReportWithParam(collectionTrackerSPResportSPResportDTO.CustomerId);

                if (collectionTrackerSPReports == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTracker SPResport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTracker SPResport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = collectionTrackerSPReports;
                    serviceResponse.Message = "Returned CollectionTracker SPResport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CollectionTrackerSPResport action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetCollectionTrackerSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<CollectionTrackerSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerSPReport>>();
            try
            {
                var collectionTrackerSPReportWithDate = await _repository.GetCollectionTrackerSPReportWithDate(FromDate, ToDate);
                if (collectionTrackerSPReportWithDate == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTrackerSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTrackerSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = collectionTrackerSPReportWithDate;
                    serviceResponse.Message = "Returned CollectionTrackerSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetCollectionTrackerByCustomerIdSPReportWithParam([FromBody] CollectionTrackerByCustomerIdSPReportDTO collectionTrackerByCustomerIdSPReportDTO)

        {
            ServiceResponse<IEnumerable<CollectionTrackerByCustomerIdSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerByCustomerIdSPReport>>();
            try
            {
                var collectionTrackerByCustomerIdSPReports = await _repository.GetCollectionTrackerByCustomerIdSPReportWithParam(collectionTrackerByCustomerIdSPReportDTO.CustomerId);

                if (collectionTrackerByCustomerIdSPReports == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTrackerByCustomerId SPResport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTrackerByCustomerId SPResport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = collectionTrackerByCustomerIdSPReports;
                    serviceResponse.Message = "Returned CollectionTrackerByCustomerId SPResport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerByCustomerIdSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetCollectionTrackerByCustomerIdSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<CollectionTrackerByCustomerIdSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerByCustomerIdSPReport>>();
            try
            {
                var collectionTrackerByCustomerIdSPReportWithDate = await _repository.GetCollectionTrackerByCustomerIdSPReportWithDate(FromDate, ToDate);
                if (collectionTrackerByCustomerIdSPReportWithDate == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTrackerByCustomerId hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTrackerByCustomerId hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = collectionTrackerByCustomerIdSPReportWithDate;
                    serviceResponse.Message = "Returned CollectionTrackerByCustomerId SPResport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerByCustomerIdSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetCollectionTrackerBySalesOrderNoSPReportWithParam([FromBody] CollectionTrackerByCustomerIdSPReportDTO collectionTrackerByCustomerIdSPReportDTO)

        {
            ServiceResponse<IEnumerable<CollectionTrackerBySalesOrderNoSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerBySalesOrderNoSPReport>>();
            try
            {
                var collectionTrackerBySalesOrderNoSPReports = await _repository.GetCollectionTrackerBySalesOrderNoSPReportWithParam(collectionTrackerByCustomerIdSPReportDTO.CustomerId);

                if (collectionTrackerBySalesOrderNoSPReports == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTrackerBySalesOrderNo SPResport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTrackerBySalesOrderNo SPResport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = collectionTrackerBySalesOrderNoSPReports;
                    serviceResponse.Message = "Returned CollectionTrackerBySalesOrderNo SPResport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerBySalesOrderNoSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetCollectionTrackerBySalesOrderNoSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<CollectionTrackerBySalesOrderNoSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerBySalesOrderNoSPReport>>();
            try
            {
                var collectionTrackerBySalesOrderNoSPReportWithDate = await _repository.GetCollectionTrackerBySalesOrderNoSPReportWithDate(FromDate, ToDate);
                if (collectionTrackerBySalesOrderNoSPReportWithDate == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTrackerBySalesOrderNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTrackerBySalesOrderNo hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = collectionTrackerBySalesOrderNoSPReportWithDate;
                    serviceResponse.Message = "Returned CollectionTrackerBySalesOrderNo SPResport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerBySalesOrderNoSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetCollectionTrackerWithCustomerWiseSPReportWithParam([FromBody] CollectionTrackerWithCustomerWiseSPReportDTO collectionTrackerWithCustomerWiseSPReportDTO)

        {
            ServiceResponse<IEnumerable<CollectionTrackerWithCustomerWiseSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerWithCustomerWiseSPReport>>();
            try
            {
                var collectionTrackerWithCustomerWiseSPReportWithParam = await _repository.GetCollectionTrackerWithCustomerWiseSPReportWithParam(collectionTrackerWithCustomerWiseSPReportDTO.CustomerId);

                if (collectionTrackerWithCustomerWiseSPReportWithParam == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTrackerWithCustomerWise SPResport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTrackerWithCustomerWise SPResport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = collectionTrackerWithCustomerWiseSPReportWithParam;
                    serviceResponse.Message = "Returned CollectionTrackerWithCustomerWise SPResport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerWithCustomerWiseSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetCollectionTrackerWithCustomerWiseSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<CollectionTrackerWithCustomerWiseSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerWithCustomerWiseSPReport>>();
            try
            {
                var collectionTrackerWithCustomerWiseSPReportWithDate = await _repository.GetCollectionTrackerWithCustomerWiseSPReportWithDate(FromDate, ToDate);
                if (collectionTrackerWithCustomerWiseSPReportWithDate == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTrackerWithCustomerWise hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTrackerWithCustomerWise hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = collectionTrackerWithCustomerWiseSPReportWithDate;
                    serviceResponse.Message = "Returned CollectionTrackerWithCustomerWise SPResportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerWithCustomerWiseSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetCollectionTrackerWithSalesOrderNoWiseSPReportWithParam([FromBody] CollectionTrackerWithSalesOrderNoWiseSPReportDTO collectionTrackerWithSalesOrderNoWiseSPReportDTO)

        {
            ServiceResponse<IEnumerable<CollectionTrackerWithSalesOrderNoWiseSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerWithSalesOrderNoWiseSPReport>>();
            try
            {
                var collectionTrackerWithCustomerWiseSPReportWithParam = await _repository.GetCollectionTrackerWithSalesOrderNoWiseSPReportWithParam(collectionTrackerWithSalesOrderNoWiseSPReportDTO.SalesOrderNumber);

                if (collectionTrackerWithCustomerWiseSPReportWithParam == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTrackerWithSalesOrderNoWise SPResport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTrackerWithSalesOrderNoWise SPResport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = collectionTrackerWithCustomerWiseSPReportWithParam;
                    serviceResponse.Message = "Returned CollectionTrackerWithSalesOrderNoWise SPResport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerWithSalesOrderNoWiseSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetCollectionTrackerWithSalesOrderNoWiseSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<CollectionTrackerWithSalesOrderNoWiseSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerWithSalesOrderNoWiseSPReport>>();
            try
            {
                var collectionTrackerWithCustomerWiseSPReportWithDate = await _repository.GetCollectionTrackerWithSalesOrderNoWiseSPReportWithDate(FromDate, ToDate);
                if (collectionTrackerWithCustomerWiseSPReportWithDate == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTrackerWithSalesOrderNoWise hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTrackerWithSalesOrderNoWise hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = collectionTrackerWithCustomerWiseSPReportWithDate;
                    serviceResponse.Message = "Returned CollectionTrackerWithSalesOrderNoWise SPResportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerWithSalesOrderNoWiseSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
    }
}
