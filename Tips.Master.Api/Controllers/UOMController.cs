using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UOMController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public UOMController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<UOMController>
        [HttpGet]
        public async Task<IActionResult> GetAllUOM()
        {
            try
            {
                var UOMList = await _repository.UOMRepository.GetAllUOM();
                _logger.LogInfo("Returned all UOM");
                var result = _mapper.Map<IEnumerable<UOMDto>>(UOMList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<UOMController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUOMById(int id)
        {
            try
            {
                var UOM = await _repository.UOMRepository.GetUOMById(id);
                if (UOM == null)
                {
                    _logger.LogError($"UOM with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<UOMDto>(UOM);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUOMById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<UOMController>
        [HttpPost]
        public IActionResult CreateUOM([FromBody] UOMDtoPost UomDtoPost)
        {
            try
            {
                if (UomDtoPost is null)
                {
                    _logger.LogError("UOM object sent from client is null.");
                    return BadRequest("UOM object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid UOM object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var UOMEntity = _mapper.Map<UOM>(UomDtoPost);
                _repository.UOMRepository.CreateUOM(UOMEntity);
                _repository.SaveAsync();

                return Created("GetUOMById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<UOMController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUOM(int id, [FromBody] UOMDtoUpdate UomDtoUpdate)
        {
            try
            {
                if (UomDtoUpdate is null)
                {
                    _logger.LogError("UOM object sent from client is null.");
                    return BadRequest("UOM object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid UOM object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var UOMEntity = await _repository.UOMRepository.GetUOMById(id);
                if (UOMEntity is null)
                {
                    _logger.LogError($"UOM with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(UomDtoUpdate, UOMEntity);
                string result = await _repository.UOMRepository.UpdateUOM(UOMEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateUOM action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<UOMController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUOM(int id)
        {
            try
            {
                var UOM = await _repository.UOMRepository.GetUOMById(id);
                if (UOM == null)
                {
                    _logger.LogError($"UOM with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.UOMRepository.DeleteUOM(UOM);
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
        public async Task<IActionResult> ActivateUOM(int id)
        {
            try
            {
                var UOM = await _repository.UOMRepository.GetUOMById(id);
                if (UOM is null)
                {
                    _logger.LogError($"UOM with id: {id}, hasn't been found in db.");
                    return BadRequest("UOM object is null");
                }
                UOM.ActiveStatus = true;
                string result = await _repository.UOMRepository.UpdateUOM(UOM);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateUOM action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateUOM(int id)
        {
            try
            {
                var UOM = await _repository.UOMRepository.GetUOMById(id);
                if (UOM is null)
                {
                    _logger.LogError($"UOM with id: {id}, hasn't been found in db.");
                    return BadRequest("UOM object is null");
                }
                UOM.ActiveStatus = false;
                string result = await _repository.UOMRepository.UpdateUOM(UOM);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateUOM action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
