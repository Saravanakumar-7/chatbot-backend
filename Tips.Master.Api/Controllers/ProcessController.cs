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
    public class ProcessController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public ProcessController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<ProcessController>
        [HttpGet]
        public async Task<IActionResult> GetAllProcesses([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ProcessDto>> serviceResponse = new ServiceResponse<IEnumerable<ProcessDto>>();
            try
            {

                var processlist = await _repository.ProcessRepository.GetAllProcesses(searchParams);
                _logger.LogInfo("Returned all processlist");
                var result = _mapper.Map<IEnumerable<ProcessDto>>(processlist);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all processlist Successfully";
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

        public async Task<IActionResult> GetAllActiveProcesses([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ProcessDto>> serviceResponse = new ServiceResponse<IEnumerable<ProcessDto>>();

            try
            {
                var processlist = await _repository.ProcessRepository.GetAllActiveProcesses(searchParams);
                _logger.LogInfo("Returned all processlist");
                var result = _mapper.Map<IEnumerable<ProcessDto>>(processlist);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active processlist Successfully";
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

        // GET api/<ProcessController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProcessById(int id)
        {
            ServiceResponse<ProcessDto> serviceResponse = new ServiceResponse<ProcessDto>();

            try
            {
                var Process = await _repository.ProcessRepository.GetProcessById(id);
                if (Process == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Process with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Process with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned Process with id: {id}");
                    var result = _mapper.Map<ProcessDto>(Process);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Process with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetProcessById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<ProcessController>
        [HttpPost]
        public IActionResult CreateProcess([FromBody] ProcessDtoPost processDtoPost)
        {
            ServiceResponse<ProcessDto> serviceResponse = new ServiceResponse<ProcessDto>();

            try
            {
                if (processDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Process object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Process object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Process object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Process object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var Process = _mapper.Map<Process>(processDtoPost);
                _repository.ProcessRepository.CreateProcess(Process);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Process Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetProcessById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateProcess action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<ProcessController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProcess(int id, [FromBody] ProcessDtoUpdate processDtoUpdate)
        {
            ServiceResponse<ProcessDto> serviceResponse = new ServiceResponse<ProcessDto>();

            try
            {
                if (processDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Process object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Process object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update Process object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Process object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var process = await _repository.ProcessRepository.GetProcessById(id);
                if (process is null)
                {
                    _logger.LogError($"Update Process with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Process with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(processDtoUpdate, process);
                string result = await _repository.ProcessRepository.UpdateProcess(process);
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
                _logger.LogError($"Something went wrong inside UpdateProcess action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<ProcessController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcess(int id)
        {
            ServiceResponse<ProcessDto> serviceResponse = new ServiceResponse<ProcessDto>();

            try
            {
                var process = await _repository.ProcessRepository.GetProcessById(id);
                if (process == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete process object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete process with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.ProcessRepository.DeleteProcess(process);
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
                _logger.LogError($"Something went wrong inside Deleteprocess action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }       
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateProcess(int id)
        {
            ServiceResponse<ProcessDto> serviceResponse = new ServiceResponse<ProcessDto>();

            try
            {
                var process = await _repository.ProcessRepository.GetProcessById(id);
                if (process is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Process object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Process with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                process.IsActive = true;
                string result = await _repository.ProcessRepository.UpdateProcess(process);
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
                _logger.LogError($"Something went wrong inside ActivatedProcess action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateProcess(int id)
        {
            ServiceResponse<ProcessDto> serviceResponse = new ServiceResponse<ProcessDto>();

            try
            {
                var Process = await _repository.ProcessRepository.GetProcessById(id);
                if (Process is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Process object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Process with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Process.IsActive = false;
                string result = await _repository.ProcessRepository.UpdateProcess(Process);
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
                _logger.LogError($"Something went wrong inside DeactivatedProcesses action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
