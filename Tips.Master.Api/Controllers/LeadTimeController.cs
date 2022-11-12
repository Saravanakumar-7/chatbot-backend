using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LeadTimeController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public LeadTimeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<LeadTimeController>
        [HttpGet]
        public async Task<IActionResult> GetAllLeadTimes()
        {
            try
            {
                var LeadTimeList = await _repository.leadTimeRepository.GetAllActiveLeadTime();
                _logger.LogInfo("Returned all LeadTimes");
                var result = _mapper.Map<IEnumerable<LeadTimeDto>>(LeadTimeList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<LeadTimeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeadTimeById(int id)
        {
            try
            {
                var leadTime = await _repository.leadTimeRepository.GetLeadTimeById(id);
                if (leadTime == null)
                {
                    _logger.LogError($"LeadTime with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<LeadTimeDto>(leadTime);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetLeadTimeById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<LeadTimeController>
        [HttpPost]
        public IActionResult CreateLeadTime([FromBody] LeadTimeDtoPost leadTimeDtoPost)
        {
            try
            {
                if (leadTimeDtoPost is null)
                {
                    _logger.LogError("LeadTime object sent from client is null.");
                    return BadRequest("LeadTime object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid LeadTime object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var leadTimeEntity = _mapper.Map<LeadTime>(leadTimeDtoPost);
                _repository.leadTimeRepository.CreateLeadTime(leadTimeEntity);
                _repository.SaveAsync();

                return Created("GetLeadTimeById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<LeadTimeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeadTime(int id, [FromBody] LeadTimeDtoUpdate leadTimeDtoUpdate)
        {
            try
            {
                if (leadTimeDtoUpdate is null)
                {
                    _logger.LogError("LeadTime object sent from client is null.");
                    return BadRequest("LeadTime object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid LeadTime object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var leadTimeEntity = await _repository.leadTimeRepository.GetLeadTimeById(id);
                if (leadTimeEntity is null)
                {
                    _logger.LogError($"LeadTime with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(leadTimeDtoUpdate, leadTimeEntity);
                string result = await _repository.leadTimeRepository.UpdateLeadTime(leadTimeEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateLeadTime action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<LeadTimeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeadTime(int id)
        {
            try
            {
                var leadTime = await _repository.leadTimeRepository.GetLeadTimeById(id);
                if (leadTime == null)
                {
                    _logger.LogError($"LeadTime with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.leadTimeRepository.DeleteLeadTime(leadTime);
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
        public async Task<IActionResult> ActivateLeadTime(int id)
        {
            try
            {
                var leadTime = await _repository.leadTimeRepository.GetLeadTimeById(id);
                if (leadTime is null)
                {
                    _logger.LogError($"LeadTime with id: {id}, hasn't been found in db.");
                    return BadRequest("LeadTime object is null");
                }
                leadTime.IsActive = true;
                string result = await _repository.leadTimeRepository.UpdateLeadTime(leadTime);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateLeadTime action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
         
        public async Task<IActionResult> DeactivateLeadTime(int id)
        {
            try
            {
                var leadTime = await _repository.leadTimeRepository.GetLeadTimeById(id);
                if (leadTime is null)
                {
                    _logger.LogError($"LeadTime with id: {id}, hasn't been found in db.");
                    return BadRequest("LeadTime object is null");
                }
                leadTime.IsActive = false;
                string result = await _repository.leadTimeRepository.UpdateLeadTime(leadTime);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateLeadTime action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
