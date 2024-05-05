using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ShipmentModeController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public ShipmentModeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<ShipmentModeController>
        [HttpGet]
        public async Task<IActionResult> GetAllShipmentModes([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ShipmentModeDto>> serviceResponse = new ServiceResponse<IEnumerable<ShipmentModeDto>>();
            try
            {

                var ShipmentMode = await _repository.ShipmentModeRepository.GetAllShipmentModes(searchParams);
                _logger.LogInfo("Returned all ShipmentModes");
                var result = _mapper.Map<IEnumerable<ShipmentModeDto>>(ShipmentMode);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ShipmentModes Successfully";
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
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]

        public async Task<IActionResult> GetAllActiveShipmentMode([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ShipmentModeDto>> serviceResponse = new ServiceResponse<IEnumerable<ShipmentModeDto>>();

            try
            {
                var ShipmentMode = await _repository.ShipmentModeRepository.GetAllActiveShipmentModes(searchParams);
                _logger.LogInfo("Returned all AuditFrequencies");
                var result = _mapper.Map<IEnumerable<ShipmentModeDto>>(ShipmentMode);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active ShipmentModes Successfully";
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

        // GET api/<ShipmentModeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShipmentModeById(int id)
        {
            ServiceResponse<ShipmentModeDto> serviceResponse = new ServiceResponse<ShipmentModeDto>();

            try
            {
                var ShipmentModes = await _repository.ShipmentModeRepository.GetShipmentModeById(id);
                if (ShipmentModes == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShipmentModes with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ShipmentModes with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ShipmentModes with id: {id}");
                    var result = _mapper.Map<ShipmentModeDto>(ShipmentModes);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned ShipmentModes with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShipmentModesById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<ShipmentModeController>
        [HttpPost]
        public IActionResult CreateShipmentMode([FromBody] ShipmentModeDtoPost shipmentModeDtoPost)
        {
            ServiceResponse<ShipmentModeDto> serviceResponse = new ServiceResponse<ShipmentModeDto>();

            try
            {
                if (shipmentModeDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShipmentMode object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ShipmentMode object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ShipmentMode object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ShipmentMode object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var ShipmentMode = _mapper.Map<ShipmentMode>(shipmentModeDtoPost);
                _repository.ShipmentModeRepository.CreateShipmentMode(ShipmentMode);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetShipmentModeById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateShipmentMode action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<ShipmentModeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipmentMode(int id, [FromBody] ShipmentModeDtoUpdate shipmentModeDtoUpdate)
        {
            ServiceResponse<ShipmentModeDto> serviceResponse = new ServiceResponse<ShipmentModeDto>();

            try
            {
                if (shipmentModeDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update ShipmentMode object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update ShipmentMode object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update ShipmentMode object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update ShipmentMode object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var ShipmentMode = await _repository.ShipmentModeRepository.GetShipmentModeById(id);
                if (ShipmentMode is null)
                {
                    _logger.LogError($"Update ShipmentMode with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update ShipmentMode with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(shipmentModeDtoUpdate, ShipmentMode);
                string result = await _repository.ShipmentModeRepository.UpdateShipmentMode(ShipmentMode);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateShipmentMode action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        // DELETE api/<ShipmentModeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuditShipmentMode(int id)
        {
            ServiceResponse<ShipmentModeDto> serviceResponse = new ServiceResponse<ShipmentModeDto>();

            try
            {
                var ShipmentMode = await _repository.ShipmentModeRepository.GetShipmentModeById(id);
                if (ShipmentMode == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete ShipmentMode object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete ShipmentMode with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.ShipmentModeRepository.DeleteShipmentMode(ShipmentMode);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside ShipmentMode action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateShipmentMode(int id)
        {
            ServiceResponse<ShipmentModeDto> serviceResponse = new ServiceResponse<ShipmentModeDto>();

            try
            {
                var ShipmentMode = await _repository.ShipmentModeRepository.GetShipmentModeById(id);
                if (ShipmentMode is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShipmentMode object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ShipmentMode with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                ShipmentMode.IsActive = true;
                string result = await _repository.ShipmentModeRepository.UpdateShipmentMode(ShipmentMode);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside ActivatedShipmentMode action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateShipmentMode(int id)
        {
            ServiceResponse<ShipmentModeDto> serviceResponse = new ServiceResponse<ShipmentModeDto>();

            try
            {
                var ShipmentMode = await _repository.ShipmentModeRepository.GetShipmentModeById(id);
                if (ShipmentMode is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShipmentMode object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ShipmentMode with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                ShipmentMode.IsActive = false;
                string result = await _repository.ShipmentModeRepository.UpdateShipmentMode(ShipmentMode);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deactivated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivatedShipmentMode action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
