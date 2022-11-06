using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProcurementTypeController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public ProcurementTypeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<ProcurementTypeController>
        [HttpGet]
        public async Task<IActionResult> GetAllProcurementType()
        {
            try
            {
                var procurementTypeList = await _repository.ProcurementTypeRepository.GetAllActiveProcurementType();
                _logger.LogInfo("Returned all ProcurementTypes");
                var result = _mapper.Map<IEnumerable<ProcurementTypeDto>>(procurementTypeList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<ProcurementTypeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProcurementTypeById(int id)
        {
            try
            {
                var procurementType = await _repository.ProcurementTypeRepository.GetProcurementTypeById(id);
                if (procurementType == null)
                {
                    _logger.LogError($"ProcurementType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<ProcurementTypeDto>(procurementType);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetProcurementTypeById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<ProcurementTypeController>
        [HttpPost]
        public IActionResult CreateProcurementType([FromBody] ProcurementTypeDtoPost procurementType)
        {
            try
            {
                if (procurementType is null)
                {
                    _logger.LogError("ProcurementType object sent from client is null.");
                    return BadRequest("ProcurementType object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ProcurementType object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var procurementTypeEntity = _mapper.Map<ProcurementType>(procurementType);
                var id = _repository.ProcurementTypeRepository.CreateProcurementType(procurementTypeEntity);
                _repository.SaveAsync();

                return CreatedAtRoute("GetProcurementTypeById", new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<ProcurementTypeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProcurementType(int id, [FromBody] ProcurementTypeDto procurementType)
        {
            try
            {
                if (procurementType is null)
                {
                    _logger.LogError("ProcurementType object sent from client is null.");
                    return BadRequest("ProcurementType object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ProcurementType object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var procurementTypeEntity = await _repository.ProcurementTypeRepository.GetProcurementTypeById(id);
                if (procurementTypeEntity is null)
                {
                    _logger.LogError($"ProcurementType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(procurementType, procurementTypeEntity);
                string result = await _repository.ProcurementTypeRepository.UpdateProcurementType(procurementTypeEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateProcurementType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<ProcurementTypeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcurementType(int id)
        {
            try
            {
                var procurementType = await _repository.ProcurementTypeRepository.GetProcurementTypeById(id);
                if (procurementType == null)
                {
                    _logger.LogError($"ProcurementType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.ProcurementTypeRepository.DeleteProcurementType(procurementType);
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
        public async Task<IActionResult> ActivateProcurementType(int id)
        {
            try
            {
                var procurementType = await _repository.ProcurementTypeRepository.GetProcurementTypeById(id);
                if (procurementType is null)
                {
                    _logger.LogError($"ProcurementType with id: {id}, hasn't been found in db.");
                    return BadRequest("ProcurementType object is null");
                }
                procurementType.IsActive = true;
                string result = await _repository.ProcurementTypeRepository.UpdateProcurementType(procurementType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateProcurementType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateProcurementType(int id)
        {
            try
            {
                var procurementType = await _repository.ProcurementTypeRepository.GetProcurementTypeById(id);
                if (procurementType is null)
                {
                    _logger.LogError($"ProcurementType with id: {id}, hasn't been found in db.");
                    return BadRequest("ProcurementType object is null");
                }
                procurementType.IsActive = false;
                string result = await _repository.ProcurementTypeRepository.UpdateProcurementType(procurementType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateProcurementType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
