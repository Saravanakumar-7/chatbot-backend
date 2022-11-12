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
    public class ExportUnitTypeController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public ExportUnitTypeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<ExportUnitTypeController>
        [HttpGet]
        public async Task<IActionResult> GetAllExportUnitTypes()
        {
            ServiceResponse<IEnumerable<ExportUnitTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<ExportUnitTypeDto>>();
            try
            {

                var ExportUnitTypeList = await _repository.ExportUnitTypeRepository.GetAllExportUnitTypes();
                _logger.LogInfo("Returned all ExportUnitTypeList");
                var result = _mapper.Map<IEnumerable<ExportUnitTypeDto>>(ExportUnitTypeList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllActiveExportUnitTypes()
        {
            ServiceResponse<IEnumerable<ExportUnitTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<ExportUnitTypeDto>>();

            try
            {
                var ExportUnitTypes = await _repository.ExportUnitTypeRepository.GetAllActiveExportUnitTypes();
                _logger.LogInfo("Returned all ExportUnitTypes");
                var result = _mapper.Map<IEnumerable<ExportUnitTypeDto>>(ExportUnitTypes);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                return StatusCode(500, "Internal server error");

            }
        }
        // GET api/<ExportUnitTypeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExportUnitTypeById(int id)
        {
            ServiceResponse<ExportUnitTypeDto> serviceResponse = new ServiceResponse<ExportUnitTypeDto>();

            try
            {
                var ExportUnitTypes = await _repository.ExportUnitTypeRepository.GetExportUnitTypeById(id);
                if (ExportUnitTypes == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ExportUnitTypes with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ExportUnitTypes with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<ExportUnitTypeDto>(ExportUnitTypes);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetExportUnitTypeById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<ExportUnitTypeController>
        [HttpPost]
        public IActionResult CreateExportUnitType([FromBody] ExportUnitTypeDtoPost exportUnitTypeDtoPost)
        {
            ServiceResponse<ExportUnitTypeDto> serviceResponse = new ServiceResponse<ExportUnitTypeDto>();

            try
            {
                if (exportUnitTypeDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ExportUnitType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ExportUnitType object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ExportUnitType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ExportUnitType object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var ExportUnitType = _mapper.Map<ExportUnitType>(exportUnitTypeDtoPost);
                _repository.ExportUnitTypeRepository.CreateExportUnitType(ExportUnitType);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfylly Created";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;


                return Created("GetExportUnitTypeById", "Successfully Created");
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<ExportUnitTypeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExportUnitType(int id, [FromBody] ExportUnitTypeDtoUpdate exportUnitTypeDtoUpdate)
        {
            ServiceResponse<ExportUnitTypeDto> serviceResponse = new ServiceResponse<ExportUnitTypeDto>();

            try
            {
                if (exportUnitTypeDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ExportUnitType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ExportUnitType object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ExportUnitType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ExportUnitType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var ExportUnitType = await _repository.ExportUnitTypeRepository.GetExportUnitTypeById(id);
                if (ExportUnitType is null)
                {
                    _logger.LogError($"ExportUnitType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(exportUnitTypeDtoUpdate, ExportUnitType);
                string result = await _repository.ExportUnitTypeRepository.UpdateExportUnitType(ExportUnitType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside UpdateExportUnitType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        // DELETE api/<ExportUnitTypeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExportUnitType(int id)
        {
            ServiceResponse<ExportUnitTypeDto> serviceResponse = new ServiceResponse<ExportUnitTypeDto>();

            try
            {
                var ExportUnitType = await _repository.ExportUnitTypeRepository.GetExportUnitTypeById(id);
                if (ExportUnitType == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ExportUnitType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ExportUnitType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.ExportUnitTypeRepository.DeleteExportUnitType(ExportUnitType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateExportUnitType(int id)
        {
            ServiceResponse<ExportUnitTypeDto> serviceResponse = new ServiceResponse<ExportUnitTypeDto>();

            try
            {
                var ExportUnitType = await _repository.ExportUnitTypeRepository.GetExportUnitTypeById(id);
                if (ExportUnitType is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ExportUnitType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ExportUnitType with id: {id}, hasn't been found in db.");
                    return BadRequest("ExportUnitType object is null");
                }
                ExportUnitType.IsActive = true;
                string result = await _repository.ExportUnitTypeRepository.UpdateExportUnitType(ExportUnitType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateExportUnitType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeActivateExportUnitType(int id)
        {
            ServiceResponse<ExportUnitTypeDto> serviceResponse = new ServiceResponse<ExportUnitTypeDto>();

            try
            {
                var ExportUnitType = await _repository.ExportUnitTypeRepository.GetExportUnitTypeById(id);
                if (ExportUnitType is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ExportUnitType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ExportUnitType with id: {id}, hasn't been found in db.");
                    return BadRequest("ExportUnitType object is null");
                }
                ExportUnitType.IsActive = false;
                string result = await _repository.ExportUnitTypeRepository.UpdateExportUnitType(ExportUnitType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deactivated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateExportUnitType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
