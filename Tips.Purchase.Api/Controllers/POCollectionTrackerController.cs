using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;
namespace Tips.Purchase.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class POCollectionTrackerController : ControllerBase
    {
        private IPOCollectionTrackerRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public POCollectionTrackerController(IPOCollectionTrackerRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPOCollectionTracker([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<POCollectionTrackerDto>> serviceResponse = new ServiceResponse<IEnumerable<POCollectionTrackerDto>>();

            try
            {
                var pocollectionTrackerDetails = await _repository.GetAllPOCollectionTrackers(pagingParameter, searchParamess);

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
                var result = _mapper.Map<IEnumerable<POCollectionTrackerDto>>(pocollectionTrackerDetails);
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
        public async Task<IActionResult> GetPOCollectionTrackerById(int id)
        {
            ServiceResponse<POCollectionTrackerDto> serviceResponse = new ServiceResponse<POCollectionTrackerDto>();
            try
            {
                var pocollectionTrackerById = await _repository.GetPOCollectionTrackerById(id);

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
                    var result = _mapper.Map<POCollectionTrackerDto>(pocollectionTrackerById);
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
        public async Task<IActionResult> GetPOCollectionTrackerByVendorId(string vendorId)
        {
            ServiceResponse<POCollectionTrackerDetailsDto> serviceResponse = new ServiceResponse<POCollectionTrackerDetailsDto>();
            try
            {
                var pocollectionTrackerById = await _repository.GetPOCollectionTrackerByVendorId(vendorId);
                var openPurchaseOrderDetails = await _repository.GetOpenPODetailsByVendorId(vendorId);
                pocollectionTrackerById.OpenPurchaseOrderDetails = openPurchaseOrderDetails;
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
                    var result = _mapper.Map<POCollectionTrackerDetailsDto>(pocollectionTrackerById);
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
        [HttpPost]
        public async Task<IActionResult> CreatePOCollectionTracker([FromBody] POCollectionTrackerPostDto pocollectionTrackerPostDto)
        {
            ServiceResponse<POCollectionTrackerDto> serviceResponse = new ServiceResponse<POCollectionTrackerDto>();

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

                var pocollectionTrackerItems = _mapper.Map<IEnumerable<POBreakDown>>(pocollectionTrackerPostDto.POBreakDown);

                var pocollectionTracker = _mapper.Map<POCollectionTracker>(pocollectionTrackerPostDto);

                pocollectionTracker.POBreakDown = pocollectionTrackerItems.ToList();

                await _repository.CreatePOCollectionTracker(pocollectionTracker);

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

        public async Task<IActionResult> UpdatePOCollectionTracker(int id, [FromBody] POCollectionTrackerUpdateDto pocollectionTrackerUpdateDto)
        {
            ServiceResponse<POCollectionTrackerUpdateDto> serviceResponse = new ServiceResponse<POCollectionTrackerUpdateDto>();
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
                var pocollectionTrackerById = await _repository.GetPOCollectionTrackerById(id);
                if (pocollectionTrackerById is null)
                {
                    _logger.LogError($"Update POCollectionTracker with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update POCollectionTracker with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var pocollectionTrackerItems = _mapper.Map<IEnumerable<POBreakDown>>(pocollectionTrackerUpdateDto.POBreakDown);

                var pocollectionTracker = _mapper.Map(pocollectionTrackerUpdateDto, pocollectionTrackerById);

                pocollectionTracker.POBreakDown = pocollectionTrackerItems.ToList();

                string result = await _repository.UpdatePOCollectionTracker(pocollectionTracker);
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
        public async Task<IActionResult> DeletePOCollectionTracker(int id)
        {
            ServiceResponse<POCollectionTrackerDto> serviceResponse = new ServiceResponse<POCollectionTrackerDto>();
            try
            {
                var pocollectionTrackerById = await _repository.GetPOCollectionTrackerById(id);
                if (pocollectionTrackerById == null)
                {
                    _logger.LogError($"Delete POCollectionTracker with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete POCollectionTracker with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeletePOCollectionTracker(pocollectionTrackerById);
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
    }
}
