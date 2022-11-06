using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc; 

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesUomController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public ValuesUomController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<ValuesUomController>
        [HttpGet]
        public async Task<IActionResult> GetAllVolumeUom()
        {
            try
            {
                var volumeUomList = await _repository.VolumeUomRepo.GetAllActiveVolumeUom();
                _logger.LogInfo("Returned all VolumeUom");
                var result = _mapper.Map<IEnumerable<VolumeUomDto>>(volumeUomList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }


        // GET api/<ValuesUomController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVolumeUomById(int id)
        {
            try
            {
                var volumeUom = await _repository.VolumeUomRepo.GetVolumeUomById(id);
                if (volumeUom == null)
                {
                    _logger.LogError($"volumeUom with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<VolumeUomDto>(volumeUom);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetvolumeUomById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<ValuesUomController>
        [HttpPost]
        public IActionResult CreateVolumeUoms([FromBody] VolumeUomPostDto volumeUom)
        {
            try
            {
                if (volumeUom is null)
                {
                    _logger.LogError("volumeUom object sent from client is null.");
                    return BadRequest("volumeUom object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid volumeUom object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var volumeUoms = _mapper.Map<VolumeUom>(volumeUom);
                _repository.VolumeUomRepo.CreateVolumeUom(volumeUoms);
                _repository.SaveAsync();


                return Created("GetvolumeUomById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<ValuesUomController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVolumeUom(int id, [FromBody] VolumeUomUpdateDto uomUpdateDto)
        {
            try
            {
                if (uomUpdateDto is null)
                {
                    _logger.LogError("VolumeUom object sent from client is null.");
                    return BadRequest("VolumeUom object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid VolumeUom object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var volumeUom = await _repository.VolumeUomRepo.GetVolumeUomById(id);
                if (volumeUom is null)
                {
                    _logger.LogError($"volumeUom with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(uomUpdateDto, volumeUom);
                string result = await _repository.VolumeUomRepo.UpdateVolumeUom(volumeUom);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateVolumeUom action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<ValuesUomController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVolumeUom(int id)
        {
            try
            {
                var volumeUom = await _repository.VolumeUomRepo.GetVolumeUomById(id);
                if (volumeUom == null)
                {
                    _logger.LogError($"VolumeUom with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.VolumeUomRepo.DeleteVolumeUom(volumeUom);
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
        public async Task<IActionResult> ActivateVolumeUom(int id)
        {
            try
            {
                var volumeUom = await _repository.VolumeUomRepo.GetVolumeUomById(id);
                if (volumeUom is null)
                {
                    _logger.LogError($"volumeUom with id: {id}, hasn't been found in db.");
                    return BadRequest("volumeUom object is null");
                }
                volumeUom.IsActive = true;
                string result = await _repository.VolumeUomRepo.UpdateVolumeUom(volumeUom);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateVolumeUom action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateVolumeUom(int id)
        {
            try
            {
                var volumeUom = await _repository.VolumeUomRepo.GetVolumeUomById(id);
                if (volumeUom is null)
                {
                    _logger.LogError($"volumeUom with id: {id}, hasn't been found in db.");
                    return BadRequest("volumeUom object is null");
                }
                volumeUom.IsActive = false;
                string result = await _repository.VolumeUomRepo.UpdateVolumeUom(volumeUom);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivatevolumeUom action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
