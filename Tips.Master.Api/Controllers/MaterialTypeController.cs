using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                var MaterialTypeList = await _repository.MaterialTypeRepository.GetAllActiveMaterialType();
                _logger.LogInfo("Returned all MaterialTypes");
                var result = _mapper.Map<IEnumerable<MaterialTypeDto>>(MaterialTypeList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<MaterialTypeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialTypeById(int id)
        {
            try
            {
                var materialType = await _repository.MaterialTypeRepository.GetMaterialTypeById(id);
                if (materialType == null)
                {
                    _logger.LogError($"MaterialType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<MaterialTypeDto>(materialType);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMaterialTypeById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<LeadTimeController>
        [HttpPost]
        public IActionResult CreateMaterialType([FromBody] MaterialTypeDtoPost materialTypeDtoPost)
        {
            try
            {
                if (materialTypeDtoPost is null)
                {
                    _logger.LogError("MaterialType object sent from client is null.");
                    return BadRequest("MaterialType object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialType object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var materialTypeEntity = _mapper.Map<MaterialType>(materialTypeDtoPost);
                _repository.MaterialTypeRepository.CreateMaterialType(materialTypeEntity);
                _repository.SaveAsync();

                return Created("GetMaterialTypeById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        // PUT api/<LeadTimeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialType(int id, [FromBody] MaterialTypeDtoUpdate materialTypeDtoUpdate)
        {
            try
            {
                if (materialTypeDtoUpdate is null)
                {
                    _logger.LogError("MaterialType object sent from client is null.");
                    return BadRequest("MaterialType object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialType object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var materialTypeEntity = await _repository.MaterialTypeRepository.GetMaterialTypeById(id);
                if (materialTypeEntity is null)
                {
                    _logger.LogError($"MaterialType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(materialTypeDtoUpdate, materialTypeEntity);
                string result = await _repository.MaterialTypeRepository.UpdateMaterialType(materialTypeEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMaterialType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<LeadTimeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialType(int id)
        {
            try
            {
                var materialType = await _repository.MaterialTypeRepository.GetMaterialTypeById(id);
                if (materialType == null)
                {
                    _logger.LogError($"MaterialType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.MaterialTypeRepository.DeleteMaterialType(materialType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateMaterialType(int id)
        {
            try
            {
                var materialType = await _repository.MaterialTypeRepository.GetMaterialTypeById(id);
                if (materialType is null)
                {
                    _logger.LogError($"MaterialType with id: {id}, hasn't been found in db.");
                    return BadRequest("MaterialType object is null");
                }
                materialType.IsActive = true;
                string result = await _repository.MaterialTypeRepository.UpdateMaterialType(materialType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateMaterialType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateMaterialType(int id)
        {
            try
            {
                var materialType = await _repository.MaterialTypeRepository.GetMaterialTypeById(id);
                if (materialType is null)
                {
                    _logger.LogError($"MaterialType with id: {id}, hasn't been found in db.");
                    return BadRequest("MaterialType object is null");
                }
                materialType.IsActive = false;
                string result = await _repository.MaterialTypeRepository.UpdateMaterialType(materialType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateMaterialType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
