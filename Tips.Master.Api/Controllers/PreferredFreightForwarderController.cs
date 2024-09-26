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
    public class PreferredFreightForwarderController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public PreferredFreightForwarderController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<PreferredFreightForwarderController>
        [HttpGet]
        public async Task<IActionResult> GetAllPreferredFreightForwarders([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<PreferredFreightForwarderDto>> serviceResponse = new ServiceResponse<IEnumerable<PreferredFreightForwarderDto>>();
            try
            {

                var PreferredFreightForwarderList = await _repository.PreferredFreightForwarderRepository.GetAllPreferredFreightForwarders(searchParams);
                _logger.LogInfo("Returned all PaymentTermList");
                var result = _mapper.Map<IEnumerable<PreferredFreightForwarderDto>>(PreferredFreightForwarderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PreferredFreightForwarderList Successfully";
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
        public async Task<IActionResult> GetAllActivePreferredFreightForwarders([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<PreferredFreightForwarderDto>> serviceResponse = new ServiceResponse<IEnumerable<PreferredFreightForwarderDto>>();

            try
            {
                var PreferredFreightForwarders = await _repository.PreferredFreightForwarderRepository.GetAllPreferredFreightForwarders(searchParams);
                _logger.LogInfo("Returned all PreferredFreightForwarders");
                var result = _mapper.Map<IEnumerable<PreferredFreightForwarderDto>>(PreferredFreightForwarders);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active PreferredFreightForwarders Successfully";
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
        // GET api/<PreferredFreightForwarderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPreferredFreightForwarderById(int id)
        {
            ServiceResponse<PreferredFreightForwarderDto> serviceResponse = new ServiceResponse<PreferredFreightForwarderDto>();

            try
            {
                var PreferredFreightForwarder = await _repository.PreferredFreightForwarderRepository.GetPreferredFreightForwarderById(id);
                if (PreferredFreightForwarder == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PreferredFreightForwarder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PreferredFreightForwarder with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned PreferredFreightForwarder with id: {id}");
                    var result = _mapper.Map<PreferredFreightForwarderDto>(PreferredFreightForwarder);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPaymentTermById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<PreferredFreightForwarderController>
        [HttpPost]
        public IActionResult CreatePreferredFreightForwarder([FromBody] PreferredFreightForwarderDtoPost prefferedfreightForwarderDtoPost)
        {
            ServiceResponse<PreferredFreightForwarderDto> serviceResponse = new ServiceResponse<PreferredFreightForwarderDto>();

            try
            {
                if (prefferedfreightForwarderDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PreferredFreightForwarder object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PreferredFreightForwarder object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PreferredFreightForwarder object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PreferredFreightForwarder object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var PreferredFreightForwarder = _mapper.Map<PreferredFreightForwarder>(prefferedfreightForwarderDtoPost);
                _repository.PreferredFreightForwarderRepository.CreatePreferredFreightForwarder(PreferredFreightForwarder);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetPreferredFreightForwarderById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<PreferredFreightForwarderController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePreferredFreightForwarder(int id, [FromBody] PreferredFreightForwarderDtoUpdate preferredFreightForwarderDtoUpdate)
        {
            ServiceResponse<PreferredFreightForwarderDto> serviceResponse = new ServiceResponse<PreferredFreightForwarderDto>();

            try
            {
                if (preferredFreightForwarderDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update PreferredFreightForwarder object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update PreferredFreightForwarder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update PreferredFreightForwarder object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update PreferredFreightForwarder object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var PreferredFreightForwarder = await _repository.PreferredFreightForwarderRepository.GetPreferredFreightForwarderById(id);
                if (PreferredFreightForwarder is null)
                {
                    _logger.LogError($"Update PreferredFreightForwarder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update PreferredFreightForwarder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(preferredFreightForwarderDtoUpdate, PreferredFreightForwarder);
                string result = await _repository.PreferredFreightForwarderRepository.UpdatePreferredFreightForwarder(PreferredFreightForwarder);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdatePreferredFreightForwarder action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<PreferredFreightForwarderController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePreferredFreightForwarder(int id)
        {
            ServiceResponse<PreferredFreightForwarderDto> serviceResponse = new ServiceResponse<PreferredFreightForwarderDto>();

            try
            {
                var PreferredFreightForwarder = await _repository.PreferredFreightForwarderRepository.GetPreferredFreightForwarderById(id);
                if (PreferredFreightForwarder == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PreferredFreightForwarder object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PreferredFreightForwarder with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.PreferredFreightForwarderRepository.DeletePreferredFreightForwarder(PreferredFreightForwarder);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeletePreferredFreightForwarder action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivatePreferredFreightForwarder(int id)
        {
            ServiceResponse<PreferredFreightForwarderDto> serviceResponse = new ServiceResponse<PreferredFreightForwarderDto>();

            try
            {
                var PreferredFreightForwarder = await _repository.PreferredFreightForwarderRepository.GetPreferredFreightForwarderById(id);
                if (PreferredFreightForwarder is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PreferredFreightForwarder object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PreferredFreightForwarder with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                PreferredFreightForwarder.IsActive = true;
                string result = await _repository.PreferredFreightForwarderRepository.UpdatePreferredFreightForwarder(PreferredFreightForwarder);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside ActivatePreferredFreightForwarder action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivatePreferredFreightForwarder(int id)
        {
            ServiceResponse<PreferredFreightForwarderDto> serviceResponse = new ServiceResponse<PreferredFreightForwarderDto>();

            try
            {
                var PreferredFreightForwarder = await _repository.PreferredFreightForwarderRepository.GetPreferredFreightForwarderById(id);
                if (PreferredFreightForwarder is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PreferredFreightForwarder object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PreferredFreightForwarder with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                PreferredFreightForwarder.IsActive = false;
                string result = await _repository.PreferredFreightForwarderRepository.UpdatePreferredFreightForwarder(PreferredFreightForwarder);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeactivatePreferredFreightForwarder action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
