using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SourceController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public SourceController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<AuditFrequencyController>
        [HttpGet]
        public async Task<IActionResult> GetAllSources()
        {
            ServiceResponse<IEnumerable<SourceDto>> serviceResponse = new ServiceResponse<IEnumerable<SourceDto>>();
            try
            {

                var sourceList = await _repository.sourceRepository.GetAllSources();
                _logger.LogInfo("Returned all Sources");
                var result = _mapper.Map<IEnumerable<SourceDto>>(sourceList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Sources Successfully";
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

        public async Task<IActionResult> GetAllActiveSources()
        {
            ServiceResponse<IEnumerable<SourceDto>> serviceResponse = new ServiceResponse<IEnumerable<SourceDto>>();

            try
            {
                var sources = await _repository.sourceRepository.GetAllActiveSources();
                _logger.LogInfo("Returned all sources");
                var result = _mapper.Map<IEnumerable<SourceDto>>(sources);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active sources Successfully";
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
        // GET api/<AuditFrequencyController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSourceById(int id)
        {
            ServiceResponse<SourceDto> serviceResponse = new ServiceResponse<SourceDto>();

            try
            {
                var Sources = await _repository.sourceRepository.GetSourceById(id);
                if (Sources == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Sources with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Sources with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned Sources with id: {id}");
                    var result = _mapper.Map<SourceDto>(Sources);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Sources with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSourceById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<AuditFrequencyController>
        [HttpPost]
        public IActionResult CreateSource([FromBody] SourceDtoPost sourceDtoPost)
        {
            ServiceResponse<SourceDtoPost> serviceResponse = new ServiceResponse<SourceDtoPost>();

            try
            {
                if (sourceDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Source object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Source object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Source object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Source object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var Sources = _mapper.Map<Source>(sourceDtoPost);
                _repository.sourceRepository.CreateSource(Sources);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Source Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetSourceById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateSource action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<AuditFrequencyController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSource(int id, [FromBody] SourceDtoUpdate sourceDtoUpdate)
        {
            ServiceResponse<SourceDtoUpdate> serviceResponse = new ServiceResponse<SourceDtoUpdate>();

            try
            {
                if (sourceDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Source object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Source object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update Source object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Source object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var sources = await _repository.sourceRepository.GetSourceById(id);
                if (sources is null)
                {
                    _logger.LogError($"Update sources with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update sources with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(sourceDtoUpdate, sources);
                string result = await _repository.sourceRepository.UpdateSource(sources);
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
                _logger.LogError($"Something went wrong inside UpdateSource action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<AuditFrequencyController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSource(int id)
        {
            ServiceResponse<SourceDto> serviceResponse = new ServiceResponse<SourceDto>();

            try
            {
                var sources = await _repository.sourceRepository.GetSourceById(id);
                if (sources == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete source object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete source with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.sourceRepository.DeleteSource(sources);
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
                _logger.LogError($"Something went wrong inside DeleteSource action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateSource(int id)
        {
            ServiceResponse<SourceDto> serviceResponse = new ServiceResponse<SourceDto>();

            try
            {
                var Sources = await _repository.sourceRepository.GetSourceById(id);
                if (Sources is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Source object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Source with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Sources.IsActive = true;
                string result = await _repository.sourceRepository.UpdateSource(Sources);
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
                _logger.LogError($"Something went wrong inside ActivateSource action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateSource(int id)
        {
            ServiceResponse<SourceDto> serviceResponse = new ServiceResponse<SourceDto>();

            try
            {
                var Sources = await _repository.sourceRepository.GetSourceById(id);
                if (Sources is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "auditFrequency object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"auditFrequency with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Sources.IsActive = false;
                string result = await _repository.sourceRepository.UpdateSource(Sources);
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
                _logger.LogError($"Something went wrong inside DeactivatedSources action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
