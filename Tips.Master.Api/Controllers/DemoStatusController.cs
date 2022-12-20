using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DemoStatusController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public DemoStatusController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllDemoStatus()
        {
            ServiceResponse<IEnumerable<DemoStatusDto>> serviceResponse = new ServiceResponse<IEnumerable<DemoStatusDto>>();
            try
            {

                var demoStatusList = await _repository.DemoStatusRepository.GetAllDemoStatus();
                _logger.LogInfo("Returned all DemoStatus");
                var result = _mapper.Map<IEnumerable<DemoStatusDto>>(demoStatusList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all DemoStatus Successfully";
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

        public async Task<IActionResult> GetAllActiveDemoStatus()
        {
            ServiceResponse<IEnumerable<DemoStatusDto>> serviceResponse = new ServiceResponse<IEnumerable<DemoStatusDto>>();

            try
            {
                var demoStatus = await _repository.DemoStatusRepository.GetAllActiveDemoStatus();
                _logger.LogInfo("Returned all Demostatus");
                var result = _mapper.Map<IEnumerable<DemoStatusDto>>(demoStatus);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active DemoStatus Successfully";
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
        // GET api/<DemoStatusController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDemoStatusById(int id)
        {
            ServiceResponse<DemoStatusDto> serviceResponse = new ServiceResponse<DemoStatusDto>();

            try
            {
                var demoStatus = await _repository.DemoStatusRepository.GetDemoStatusById(id);
                if (demoStatus == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"demoStatus with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"demoStatus with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned DemoStatus with id: {id}");
                    var result = _mapper.Map<DemoStatusDto>(demoStatus);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned DemoStatus with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDemoStatusById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateDemoStatus([FromBody] DemoStatusDtoPost demoStatusDtoPost)
        {
            ServiceResponse<DemoStatusDtoPost> serviceResponse = new ServiceResponse<DemoStatusDtoPost>();

            try
            {
                if (demoStatusDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "demoStatus object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("demoStatus object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid demoStatus object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid demoStatus object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var demoStatus = _mapper.Map<DemoStatus>(demoStatusDtoPost);
                _repository.DemoStatusRepository.CreateDemoStatus(demoStatus);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "demoStatus Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetDemoStatusById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateDemoStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDemoStatus(int id, [FromBody] DemoStatusDtoUpdate demoStatusDtoUpdate)
        {
            ServiceResponse<DemoStatusDto> serviceResponse = new ServiceResponse<DemoStatusDto>();

            try
            {
                if (demoStatusDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update DemoStatus object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update DemoStatus object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update DemoStatus object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update DemoStatus object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var demoStatus = await _repository.DemoStatusRepository.GetDemoStatusById(id);
                if (demoStatus is null)
                {
                    _logger.LogError($"Update auditFrequency with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update auditFrequency with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(demoStatusDtoUpdate, demoStatus);
                string result = await _repository.DemoStatusRepository.UpdateDemoStatus(demoStatus);
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
                _logger.LogError($"Something went wrong inside UpdateDemoStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDemoStatus(int id)
        {
            ServiceResponse<DemoStatusDto> serviceResponse = new ServiceResponse<DemoStatusDto>();

            try
            {
                var demoStatus = await _repository.DemoStatusRepository.GetDemoStatusById(id);
                if (demoStatus == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete demoStatus object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete demoStatus with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.DemoStatusRepository.DeleteDemoStatus(demoStatus);
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
                _logger.LogError($"Something went wrong inside DeletedemoStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateDemoStatus(int id)
        {
            ServiceResponse<DemoStatusDto> serviceResponse = new ServiceResponse<DemoStatusDto>();

            try
            {
                var demoStatus = await _repository.DemoStatusRepository.GetDemoStatusById(id);
                if (demoStatus is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "demoStatus object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"demoStatus with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                demoStatus.IsActive = true;
                string result = await _repository.DemoStatusRepository.UpdateDemoStatus(demoStatus);
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
                _logger.LogError($"Something went wrong inside ActivatedDemoStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateDemoStatus(int id)
        {
            ServiceResponse<DemoStatusDto> serviceResponse = new ServiceResponse<DemoStatusDto>();

            try
            {
                var demoStatus = await _repository.DemoStatusRepository.GetDemoStatusById(id);
                if (demoStatus is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "demoStatus object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"demoStatus with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                demoStatus.IsActive = false;
                string result = await _repository.DemoStatusRepository.UpdateDemoStatus(demoStatus);
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
                _logger.LogError($"Something went wrong inside DeactivateddemoStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
