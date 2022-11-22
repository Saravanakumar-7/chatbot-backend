using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaterialTypeController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public MaterialTypeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<MaterialTypeController>
        [HttpGet]
        public async Task<IActionResult> GetAllMaterialType()
        {
            ServiceResponse<IEnumerable<MaterialTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialTypeDto>>();

            try
            {
                var MaterialTypeList = await _repository.MaterialTypeRepository.GetAllMaterialType();
                _logger.LogInfo("Returned all MaterialTypes");
                var result = _mapper.Map<IEnumerable<MaterialTypeDto>>(MaterialTypeList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialTypes Successfully";
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
        public async Task<IActionResult> GetAllActiveMaterialTypes()
        {
            ServiceResponse<IEnumerable<MaterialTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialTypeDto>>();

            try
            {
                var MaterialTypes = await _repository.MaterialTypeRepository.GetAllActiveMaterialType();
                _logger.LogInfo("Returned all MaterialTypes");
                var result = _mapper.Map<IEnumerable<MaterialTypeDto>>(MaterialTypes);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Languages Successfully";
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

        // GET api/<MaterialTypeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialTypeById(int id)
        {
            ServiceResponse<MaterialTypeDto> serviceResponse = new ServiceResponse<MaterialTypeDto>();

            try
            {
                var materialType = await _repository.MaterialTypeRepository.GetMaterialTypeById(id);
                if (materialType == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialType with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialType with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned MaterialType with id: {id}");
                    var result = _mapper.Map<MaterialTypeDto>(materialType);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned MaterialType with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMaterialTypeById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<LeadTimeController>
        [HttpPost]
        public IActionResult CreateMaterialType([FromBody] MaterialTypeDtoPost materialTypeDtoPost)
        {
            ServiceResponse<MaterialTypeDto> serviceResponse = new ServiceResponse<MaterialTypeDto>();

            try
            {
                if (materialTypeDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("MaterialType object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid MaterialType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var materialTypeEntity = _mapper.Map<MaterialType>(materialTypeDtoPost);
                _repository.MaterialTypeRepository.CreateMaterialType(materialTypeEntity);
                _repository.SaveAsync();
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetMaterialTypeById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        // PUT api/<LeadTimeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialType(int id, [FromBody] MaterialTypeDtoUpdate materialTypeDtoUpdate)
        {
            ServiceResponse<MaterialTypeDto> serviceResponse = new ServiceResponse<MaterialTypeDto>();

            try
            {
                if (materialTypeDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update MaterialType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update MaterialType object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update MaterialType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update MaterialType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var materialTypeEntity = await _repository.MaterialTypeRepository.GetMaterialTypeById(id);
                if (materialTypeEntity is null)
                {
                    _logger.LogError($"Update MaterialType with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update MaterialType with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(materialTypeDtoUpdate, materialTypeEntity);
                string result = await _repository.MaterialTypeRepository.UpdateMaterialType(materialTypeEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
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
                _logger.LogError($"Something went wrong inside UpdateMaterialType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<LeadTimeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialType(int id)
        {
            ServiceResponse<MaterialTypeDto> serviceResponse = new ServiceResponse<MaterialTypeDto>();

            try
            {
                var materialType = await _repository.MaterialTypeRepository.GetMaterialTypeById(id);
                if (materialType == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete MaterialType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete MaterialType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.MaterialTypeRepository.DeleteMaterialType(materialType);
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
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateMaterialType(int id)
        {
            ServiceResponse<MaterialTypeDto> serviceResponse = new ServiceResponse<MaterialTypeDto>();

            try
            {
                var materialType = await _repository.MaterialTypeRepository.GetMaterialTypeById(id);
                if (materialType is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "v object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"MaterialType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                materialType.IsActive = true;
                string result = await _repository.MaterialTypeRepository.UpdateMaterialType(materialType);
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
                _logger.LogError($"Something went wrong inside ActivateMaterialType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateMaterialType(int id)
        {
            ServiceResponse<MaterialTypeDto> serviceResponse = new ServiceResponse<MaterialTypeDto>();

            try
            {
                var materialType = await _repository.MaterialTypeRepository.GetMaterialTypeById(id);
                if (materialType is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"MaterialType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                materialType.IsActive = false;
                string result = await _repository.MaterialTypeRepository.UpdateMaterialType(materialType);
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
                _logger.LogError($"Something went wrong inside DeactivateMaterialType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
