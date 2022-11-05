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
    public class BasicOfApprovalController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public BasicOfApprovalController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<BasicOfApprovalController>
        [HttpGet]
        public async Task<IActionResult> GetAllBasicOfApproval()
        {
            try
            {
                var basicOfApprovals = await _repository.BasicOfApprovalRepository.GetAllActiveBasicOfApproval();
                _logger.LogInfo("Returned all BasicOfApproval");
                var result = _mapper.Map<IEnumerable<BasicOfApproval>>(basicOfApprovals);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<BasicOfApprovalController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasicOfApprovalById(int id)
        {
            try
            {
                var basicOfApproval = await _repository.BasicOfApprovalRepository.GetBasicOfApprovalById(id);
                if (basicOfApproval == null)
                {
                    _logger.LogError($"BasicOfApproval with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<BasicOfApprovalDto>(basicOfApproval);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBasicOfApprovalById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<BasicOfApprovalController>
        [HttpPost]
        public IActionResult CreateBasicOfApproval([FromBody] BasicOfApprovalPostDto basicOfApprovalPostDto)
        {
            try
            {
                if (basicOfApprovalPostDto is null)
                {
                    _logger.LogError("BasicOfApproval object sent from client is null.");
                    return BadRequest("BasicOfApproval object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid BasicOfApproval object sent from client.");
                    return BadRequest("Invalid model object");
                }
                //var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                //var id = _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                //_repository.SaveAsync();

                var basicOfApproval = _mapper.Map<BasicOfApproval>(basicOfApprovalPostDto);
                _repository.BasicOfApprovalRepository.CreateBasicOfApproval(basicOfApproval);
                _repository.SaveAsync();


                return Created("GetBasicOfApprovalById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<BasicOfApprovalController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBasicOfApproval(int id, [FromBody] BasicOfApprovalUpdateDto basicOfApprovalUpdateDto)
        {
            try
            {
                if (basicOfApprovalUpdateDto is null)
                {
                    _logger.LogError("BasicOfApproval object sent from client is null.");
                    return BadRequest("BasicOfApproval object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid BasicOfApproval object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var basicOfApproval = await _repository.BasicOfApprovalRepository.GetBasicOfApprovalById(id);
                if (basicOfApproval is null)
                {
                    _logger.LogError($"BasicOfApproval with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(basicOfApprovalUpdateDto, basicOfApproval);
                string result = await _repository.BasicOfApprovalRepository.UpdateBasicOfApproval(basicOfApproval);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateBasicOfApproval action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<BasicOfApprovalController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasicOfApproval(int id)
        {
            try
            {
                var basicOfApproval = await _repository.BasicOfApprovalRepository.GetBasicOfApprovalById(id);
                if (basicOfApproval == null)
                {
                    _logger.LogError($"BasicOfApproval with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.BasicOfApprovalRepository.DeleteBasicOfApproval(basicOfApproval);
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
        public async Task<IActionResult> ActivateBasicOfApproval(int id)
        {
            try
            {
                var basicOfApproval = await _repository.BasicOfApprovalRepository.GetBasicOfApprovalById(id);
                if (basicOfApproval is null)
                {
                    _logger.LogError($"BasicOfApproval with id: {id}, hasn't been found in db.");
                    return BadRequest("BasicOfApproval object is null");
                }
                basicOfApproval.IsActive = true;
                string result = await _repository.BasicOfApprovalRepository.UpdateBasicOfApproval(basicOfApproval);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateBasicOfApproval action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateBasicOfApproval(int id)
        {
            try
            {
                var basicOfApproval = await _repository.BasicOfApprovalRepository.GetBasicOfApprovalById(id);
                if (basicOfApproval is null)
                {
                    _logger.LogError($"BasicOfApproval with id: {id}, hasn't been found in db.");
                    return BadRequest("BasicOfApproval object is null");
                }
                basicOfApproval.IsActive = false;
                string result = await _repository.BasicOfApprovalRepository.UpdateBasicOfApproval(basicOfApproval);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateBasicOfApproval action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
