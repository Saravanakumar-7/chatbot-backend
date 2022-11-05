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
    public class ScopeOfSupplyController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public ScopeOfSupplyController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<ScopeOfSupplyController>
        [HttpGet]
        public async Task<IActionResult> GetAllScopeOfSupply()
        {
            try
            {
                var scopeOfSupplies = await _repository.ScopeOfSupplyRepository.GetAllActiveScopeOfSupply();
                _logger.LogInfo("Returned all ScopeOfSupply");
                var result = _mapper.Map<IEnumerable<ScopeOfSupply>>(scopeOfSupplies);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<ScopeOfSupplyController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScopeOfSupplyById(int id)
        {
            try
            {
                var scopeOfSupply = await _repository.ScopeOfSupplyRepository.GetScopeOfSupplyById(id);
                if (scopeOfSupply == null)
                {
                    _logger.LogError($"ScopeOfSupply with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<ScopeOfSupplyDto>(scopeOfSupply);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetScopeOfSupplyById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        // POST api/<ScopeOfSupplyController>
        [HttpPost]
        public IActionResult CreateScopeOfSupply([FromBody] ScopeOfSupplyPostDto scopeOfSupplyPostDto)
        {
            try
            {
                if (scopeOfSupplyPostDto is null)
                {
                    _logger.LogError("ScopeOfSupply object sent from client is null.");
                    return BadRequest("ScopeOfSupply object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ScopeOfSupply object sent from client.");
                    return BadRequest("Invalid model object");
                }
                //var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                //var id = _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                //_repository.SaveAsync();

                var scopeOfSupply = _mapper.Map<ScopeOfSupply>(scopeOfSupplyPostDto);
                _repository.ScopeOfSupplyRepository.CreateScopeOfSupply(scopeOfSupply);
                _repository.SaveAsync();


                return Created("GetScopeOfSupplyById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<ScopeOfSupplyController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateScopeOfSupply(int id, [FromBody] ScopeOfSupplyUpdateDto scopeOfSupplyUpdateDto)
        {
            try
            {
                if (scopeOfSupplyUpdateDto is null)
                {
                    _logger.LogError("ScopeOfSupply object sent from client is null.");
                    return BadRequest("ScopeOfSupply object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ScopeOfSupply object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var scopeOfSupply = await _repository.ScopeOfSupplyRepository.GetScopeOfSupplyById(id);
                if (scopeOfSupply is null)
                {
                    _logger.LogError($"ScopeOfSupply with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(scopeOfSupplyUpdateDto, scopeOfSupply);
                string result = await _repository.ScopeOfSupplyRepository.UpdateScopeOfSupply(scopeOfSupply);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateScopeOfSupply action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<ScopeOfSupplyController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScopeOfSupply(int id)
        {
            try
            {
                var scopeOfSupply = await _repository.ScopeOfSupplyRepository.GetScopeOfSupplyById(id);
                if (scopeOfSupply == null)
                {
                    _logger.LogError($"ScopeOfSupply with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.ScopeOfSupplyRepository.DeleteScopeOfSupply(scopeOfSupply);
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
        public async Task<IActionResult> ActivateScopeOfSupply(int id)
        {
            try
            {
                var scopeOfSupply = await _repository.ScopeOfSupplyRepository.GetScopeOfSupplyById(id);
                if (scopeOfSupply is null)
                {
                    _logger.LogError($"ScopeOfSupply with id: {id}, hasn't been found in db.");
                    return BadRequest("ScopeOfSupply object is null");
                }
                scopeOfSupply.IsActive = true;
                string result = await _repository.ScopeOfSupplyRepository.UpdateScopeOfSupply(scopeOfSupply);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateScopeOfSupply action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateScopeOfSupply(int id)
        {
            try
            {
                var scopeOfSupply = await _repository.ScopeOfSupplyRepository.GetScopeOfSupplyById(id);
                if (scopeOfSupply is null)
                {
                    _logger.LogError($"ScopeOfSupply with id: {id}, hasn't been found in db.");
                    return BadRequest("ScopeOfSupply object is null");
                }
                scopeOfSupply.IsActive = false;
                string result = await _repository.ScopeOfSupplyRepository.UpdateScopeOfSupply(scopeOfSupply);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateScopeOfSupply action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
