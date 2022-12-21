using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using System.Net;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LocationTransferController : ControllerBase
    {

        private ILocationTransferRepository _locationTransferRepository;
        private IMapper _mapper;
        private ILoggerManager _logger;
        public LocationTransferController(ILocationTransferRepository locationTransferRepository, ILoggerManager logger, IMapper mapper)
        {
            _locationTransferRepository = locationTransferRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLocationTransfer([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<LocationTransferDto>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferDto>>();

            try
            {
                var listOflocationsTrans = await _locationTransferRepository.GetAllLocationTransfer(pagingParameter);

                var metadata = new
                {
                    listOflocationsTrans.TotalCount,
                    listOflocationsTrans.PageSize,
                    listOflocationsTrans.CurrentPage,
                    listOflocationsTrans.HasNext,
                    listOflocationsTrans.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogError("Returned all LocationTransferdetails");
                var result = _mapper.Map<IEnumerable<LocationTransferDto>>(listOflocationsTrans);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LocationTransfer Successfully";
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
        public async Task<IActionResult> GetLocationTransferById(int id)
        {
            ServiceResponse<LocationTransferDto> serviceResponse = new ServiceResponse<LocationTransferDto>();

            try
            {
                var locationTransId = await _locationTransferRepository.GetLocationTransferById(id);

                if (locationTransId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"locationDetails with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"locationDetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned LocationTransfer with id: {id}");
                    var result = _mapper.Map<LocationTransferDto>(locationTransId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned LocationTransfer with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetLocationTransferById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateLocationTransfer([FromBody] LocationTransferDtoPost locationTransferDtoPost)
        {
            ServiceResponse<LocationTransferDto> serviceResponse = new ServiceResponse<LocationTransferDto>();

            try
            {
                if (locationTransferDtoPost is null)
                {
                    _logger.LogError("locationTransfer object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "locationTransfer object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid locationTransfer object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid locationTransfer object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var locationTransfer = _mapper.Map<LocationTransfer>(locationTransferDtoPost);

                _locationTransferRepository.CreateLocationTransfer(locationTransfer);
                _locationTransferRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetLocationTransferId", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateLocationTransfer action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocationTransfer(int id, [FromBody] LocationTransferDtoUpdate locationTransferDtoUpdate)
        {
            ServiceResponse<LocationTransferDto> serviceResponse = new ServiceResponse<LocationTransferDto>();

            try
            {
                if (locationTransferDtoUpdate is null)
                {
                    _logger.LogError("locationTransfer object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update locationTransfer object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid locationTransfer object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update locationTransfer object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var locations = await _locationTransferRepository.GetLocationTransferById(id);
                if (locations is null)
                {
                    _logger.LogError($"locationTransfer with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update locationTransfer with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var data = _mapper.Map(locationTransferDtoUpdate, locations);

                string result = await _locationTransferRepository.UpdateLocationTransfer(data);
                _logger.LogError(result);
                _locationTransferRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateLocationTransfer action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocationTransfer(int id)
        {
            ServiceResponse<LocationTransferDto> serviceResponse = new ServiceResponse<LocationTransferDto>();

            try
            {
                var deletelocation = await _locationTransferRepository.GetLocationTransferById(id);
                if (deletelocation == null)
                {
                    _logger.LogError($"Delete LocationTransfer with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete LocationTransfer with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _locationTransferRepository.DeleteLocationTransfer(deletelocation);
                _logger.LogError(result);
                _locationTransferRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}