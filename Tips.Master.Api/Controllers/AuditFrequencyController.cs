using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuditFrequencyController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public AuditFrequencyController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<AuditFrequencyController>
        [HttpGet]
        public async Task<IActionResult> GetAllAuditFrequencies([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<AuditFrequencyDto>> serviceResponse = new ServiceResponse<IEnumerable<AuditFrequencyDto>>();
            try
            {

                var GetallAuditFrequencies = await _repository.AuditFrequencyRepository.GetAllAuditFrequencies(searchParams);

                _logger.LogInfo("Returned all AuditFrequencies");
                var result = _mapper.Map<IEnumerable<AuditFrequencyDto>>(GetallAuditFrequencies);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all AuditFrequencies Successfully";
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

        public async Task<IActionResult> GetAllActiveAuditFrequencies([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<AuditFrequencyDto>> serviceResponse = new ServiceResponse<IEnumerable<AuditFrequencyDto>>();

            try
            {
                var AllActiveAuditFrequencies = await _repository.AuditFrequencyRepository.GetAllActiveAuditFrequencies(searchParams);
                _logger.LogInfo("Returned all AuditFrequencies");
                var result = _mapper.Map<IEnumerable<AuditFrequencyDto>>(AllActiveAuditFrequencies);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active AuditFrequencies Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode= HttpStatusCode.OK;
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
        // GET api/<AuditFrequencyController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuditFrequencyById(int id)
        {
            ServiceResponse<AuditFrequencyDto> serviceResponse = new ServiceResponse<AuditFrequencyDto>();

            try
            {
                var AuditFrequenybyId = await _repository.AuditFrequencyRepository.GetAuditFrequenyById(id);
                if (AuditFrequenybyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"AuditFrequencies with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"AuditFrequencies with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned AuditFrequency with id: {id}");
                    var result = _mapper.Map<AuditFrequencyDto>(AuditFrequenybyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned AuditFrequency with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAuditFrequenyById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        
        // POST api/<AuditFrequencyController>
        [HttpPost]
        public IActionResult CreateAuditFrequency([FromBody] AuditFrequencyDtoPost auditFrequencyDtoPost)
        {
            ServiceResponse<AuditFrequencyDto> serviceResponse = new ServiceResponse<AuditFrequencyDto>();

            try
            {
                if (auditFrequencyDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "AuditFrequeny object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("AuditFrequeny object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid AuditFrequeny object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid AuditFrequeny object sent from client.");
                    
                    return BadRequest(serviceResponse);
                }
                var AuditFrequenyCreate = _mapper.Map<AuditFrequency>(auditFrequencyDtoPost);
                _repository.AuditFrequencyRepository.CreateAuditFrequency(AuditFrequenyCreate);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "AuditFrequency Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetAuditFrequencyById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateAuditFrequency action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<AuditFrequencyController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuditFrequency(int id, [FromBody] AuditFrequencyDtoUpdate auditFrequencyDtoUpdate)
        {
            ServiceResponse<AuditFrequencyDto> serviceResponse = new ServiceResponse<AuditFrequencyDto>();

            try
            {
                if (auditFrequencyDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update AuditFrequency object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update AuditFrequency object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update auditFrequency object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update auditFrequency object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var auditFrequencyUpdate = await _repository.AuditFrequencyRepository.GetAuditFrequenyById(id);
                if (auditFrequencyUpdate is null)
                {
                    _logger.LogError($"Update auditFrequency with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update auditFrequency with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode= HttpStatusCode.NotFound;  
                    return NotFound(serviceResponse);
                }
                _mapper.Map(auditFrequencyDtoUpdate, auditFrequencyUpdate);
                string result = await _repository.AuditFrequencyRepository.UpdateAuditFrequency(auditFrequencyUpdate);
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
                _logger.LogError($"Something went wrong inside UpdateauditFrequency action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<AuditFrequencyController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuditFrequency(int id)
        {
            ServiceResponse<AuditFrequencyDto> serviceResponse = new ServiceResponse<AuditFrequencyDto>();

            try
            {
                var DeleteAuditfrequency = await _repository.AuditFrequencyRepository.GetAuditFrequenyById(id);
                if (DeleteAuditfrequency == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete auditFrequency object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete auditFrequency with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.AuditFrequencyRepository.DeleteAuditFrequency(DeleteAuditfrequency);
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
                _logger.LogError($"Something went wrong inside DeleteAuditFrequency action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateAuditFrequency(int id)
        {
            ServiceResponse<AuditFrequencyDto> serviceResponse = new ServiceResponse<AuditFrequencyDto>();

            try
            {
                var AuditFrequencyActivate = await _repository.AuditFrequencyRepository.GetAuditFrequenyById(id);
                if (AuditFrequencyActivate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "auditFrequency object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"auditFrequency with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                AuditFrequencyActivate.IsActive = true;
                string result = await _repository.AuditFrequencyRepository.UpdateAuditFrequency(AuditFrequencyActivate);
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
                _logger.LogError($"Something went wrong inside ActivatedauditFrequency action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateAuditFrequency(int id)
        {
            ServiceResponse<AuditFrequencyDto> serviceResponse = new ServiceResponse<AuditFrequencyDto>();

            try
            {
                var AuditFrequencyDeactivate = await _repository.AuditFrequencyRepository.GetAuditFrequenyById(id);
                if (AuditFrequencyDeactivate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "auditFrequency object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"auditFrequency with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                AuditFrequencyDeactivate.IsActive = false;
                string result = await _repository.AuditFrequencyRepository.UpdateAuditFrequency(AuditFrequencyDeactivate);
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
                _logger.LogError($"Something went wrong inside DeactivatedauditFrequency action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
