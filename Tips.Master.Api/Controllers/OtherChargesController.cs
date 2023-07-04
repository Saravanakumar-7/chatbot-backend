using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OtherChargesController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public OtherChargesController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOtherCharges()
        {
            ServiceResponse<IEnumerable<OtherChargesDto>> serviceResponse = new ServiceResponse<IEnumerable<OtherChargesDto>>();

            try
            {
                var otherChargesList = await _repository.OtherChargesRepository.GetAllOtherCharges();
                _logger.LogInfo("Returned all OtherCharges");
                var result = _mapper.Map<IEnumerable<OtherChargesDto>>(otherChargesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OtherCharges Successfully";
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
        [HttpGet]
        public async Task<IActionResult> GetAllActiveOtherCharges()
        {
            ServiceResponse<IEnumerable<OtherChargesDto>> serviceResponse = new ServiceResponse<IEnumerable<OtherChargesDto>>();

            try
            {
                var otherChargesList = await _repository.OtherChargesRepository.GetAllActiveOtherCharges();
                _logger.LogInfo("Returned all OtherCharges");
                var result = _mapper.Map<IEnumerable<OtherChargesDto>>(otherChargesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active OtherCharges Successfully";
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
        public async Task<IActionResult> GetOtherChargesById(int id)
        {
            ServiceResponse<OtherChargesDto> serviceResponse = new ServiceResponse<OtherChargesDto>();

            try
            {
                var otherChargesDetails = await _repository.OtherChargesRepository.GetOtherChargesById(id);
                if (otherChargesDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OtherCharges with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OtherCharges with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned OtherCharges with id: {id}");
                    var result = _mapper.Map<OtherChargesDto>(otherChargesDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned OtherCharges with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOtherChargesById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public IActionResult CreateOtherCharges([FromBody] OtherChargesPostDto otherChargesPostDto)
        {
            ServiceResponse<OtherChargesDto> serviceResponse = new ServiceResponse<OtherChargesDto>();

            try
            {
                if (otherChargesPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OtherCharges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("OtherCharges object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OtherCharges object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid OtherCharges object sent from client.");
                    return Ok(serviceResponse);
                }
                var otherChargesEntity = _mapper.Map<OtherCharges>(otherChargesPostDto);
                _repository.OtherChargesRepository.CreateOtherCharges(otherChargesEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetOtherChargesById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateOtherCharges action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOtherCharges(int id, [FromBody] OtherChargesUpdateDto otherChargesUpdateDto)
        {
            ServiceResponse<OtherChargesUpdateDto> serviceResponse = new ServiceResponse<OtherChargesUpdateDto>();

            try
            {
                if (otherChargesUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update OtherCharges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("OtherCharges object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update OtherCharges object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update OtherCharges object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var otherChargesById = await _repository.OtherChargesRepository.GetOtherChargesById(id);
                if (otherChargesById is null)
                {
                    _logger.LogError($"OtherCharges with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(otherChargesUpdateDto, otherChargesById);
                string result = await _repository.OtherChargesRepository.UpdateOtherCharges(otherChargesById);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateOtherCharges action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOtherCharges(int id)
        {
            ServiceResponse<OtherChargesDto> serviceResponse = new ServiceResponse<OtherChargesDto>();

            try
            {
                var otherChargesDetails = await _repository.OtherChargesRepository.GetOtherChargesById(id);
                if (otherChargesDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OtherCharges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"OtherCharges  with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                string result = await _repository.OtherChargesRepository.DeleteOtherCharges(otherChargesDetails);
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
                serviceResponse.Message = $"Something went wrong inside OtherCharges action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOtherCharges action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateOtherCharges(int id)
        {
            ServiceResponse<OtherChargesDto> serviceResponse = new ServiceResponse<OtherChargesDto>();

            try
            {
                var otherChargesDetails = await _repository.OtherChargesRepository.GetOtherChargesById(id);
                if (otherChargesDetails is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OtherCharges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"OtherCharges with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                otherChargesDetails.ActiveStatus = true;
                string result = await _repository.OtherChargesRepository.UpdateOtherCharges(otherChargesDetails);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateOtherCharges action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateOtherCharges(int id)
        {
            ServiceResponse<OtherChargesDto> serviceResponse = new ServiceResponse<OtherChargesDto>();

            try
            {
                var otherChargesDetails = await _repository.OtherChargesRepository.GetOtherChargesById(id);
                if (otherChargesDetails is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OtherCharges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"OtherCharges with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                otherChargesDetails.ActiveStatus = false;
                string result = await _repository.OtherChargesRepository.UpdateOtherCharges(otherChargesDetails);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateOtherCharges action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
