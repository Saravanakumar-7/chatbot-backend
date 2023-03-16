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
 
        [HttpGet]
        public async Task<IActionResult> GetAllExportUnitTypes([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ExportUnitTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<ExportUnitTypeDto>>();
            try
            {

                var ExportUnitTypeList = await _repository.ExportUnitTypeRepository.GetAllExportUnitTypes(pagingParameter, searchParams);

                var metadata = new
                {
                    ExportUnitTypeList.TotalCount,
                    ExportUnitTypeList.PageSize,
                    ExportUnitTypeList.CurrentPage,
                    ExportUnitTypeList.HasNext,
                    ExportUnitTypeList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all ExportUnitTypeList");
                var result = _mapper.Map<IEnumerable<ExportUnitTypeDto>>(ExportUnitTypeList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ExportUnitTypes Successfully";
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
        public async Task<IActionResult> GetAllActiveExportUnitTypes([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ExportUnitTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<ExportUnitTypeDto>>();

            try
            {
                var ExportUnitTypes = await _repository.ExportUnitTypeRepository.GetAllActiveExportUnitTypes(pagingParameter, searchParams);
                _logger.LogInfo("Returned all ExportUnitTypes");
                var result = _mapper.Map<IEnumerable<ExportUnitTypeDto>>(ExportUnitTypes);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ExportUnitTypes Successfully";
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

                    _logger.LogInfo($"Returned ExportUnitTypes with id: {id}");
                    var result = _mapper.Map<ExportUnitTypeDto>(ExportUnitTypes);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned ExportUnitTypes with id Successfully";
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
                return StatusCode(500, serviceResponse);
            }
        }

 
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
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetExportUnitTypeById", serviceResponse);
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

 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExportUnitType(int id, [FromBody] ExportUnitTypeDtoUpdate exportUnitTypeDtoUpdate)
        {
            ServiceResponse<ExportUnitTypeDto> serviceResponse = new ServiceResponse<ExportUnitTypeDto>();

            try
            {
                if (exportUnitTypeDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ExportUnitType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update ExportUnitType object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ExportUnitType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update ExportUnitType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var ExportUnitType = await _repository.ExportUnitTypeRepository.GetExportUnitTypeById(id);
                if (ExportUnitType is null)
                {
                    _logger.LogError($"Update ExportUnitType with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update ExportUnitType with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(exportUnitTypeDtoUpdate, ExportUnitType);
                string result = await _repository.ExportUnitTypeRepository.UpdateExportUnitType(ExportUnitType);
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
                _logger.LogError($"Something went wrong inside UpdateExportUnitType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


 
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
                    serviceResponse.Message = "Delete ExportUnitType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete ExportUnitType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.ExportUnitTypeRepository.DeleteExportUnitType(ExportUnitType);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
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
                    return BadRequest(serviceResponse);
                }
                ExportUnitType.IsActive = true;
                string result = await _repository.ExportUnitTypeRepository.UpdateExportUnitType(ExportUnitType);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside ActivateExportUnitType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
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
                    return BadRequest(serviceResponse);
                }
                ExportUnitType.IsActive = false;
                string result = await _repository.ExportUnitTypeRepository.UpdateExportUnitType(ExportUnitType);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeactivateExportUnitType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }

        }
    }
}
