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
    public class BankController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public BankController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }


        // GET: api/<BankController>
        [HttpGet]
        public async Task<IActionResult> GetAllBankDetails()
        {
            try
            {
                var banks = await _repository.BankRepository.GetAllActiveBank();
                _logger.LogInfo("Returned all Bank");
                var result = _mapper.Map<IEnumerable<BankDto>>(banks);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        // GET api/<BankController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankById(int id)
        {
            try
            {
                var bank = await _repository.BankRepository.GetBankById(id);
                if (bank == null)
                {
                    _logger.LogError($"Bank with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<BankDto>(bank);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBankById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<BankController>
        [HttpPost]
        public IActionResult CreateBank([FromBody] BankPostDto bank)
        {
            try
            {
                if (bank is null)
                {
                    _logger.LogError("Bank object sent from client is null.");
                    return BadRequest("Bank object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Bank object sent from client.");
                    return BadRequest("Invalid model object");
                } 

                var banks = _mapper.Map<Bank>(bank);
                _repository.BankRepository.CreateBank(banks);
                _repository.SaveAsync();


                return Created("GetBankById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<BankController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBank(int id, [FromBody] BankUpdateDto bankUpdateDto)
        {
            try
            {
                if (bankUpdateDto is null)
                {
                    _logger.LogError("Bank object sent from client is null.");
                    return BadRequest("Bank object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Bank object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var bank = await _repository.BankRepository.GetBankById(id);
                if (bank is null)
                {
                    _logger.LogError($"Bank with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(bankUpdateDto, bank);
                string result = await _repository.BankRepository.UpdateBank(bank);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateBank action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<BankController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBank(int id)
        {
            try
            {
                var bank = await _repository.BankRepository.GetBankById(id);
                if (bank == null)
                {
                    _logger.LogError($"Bank with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.BankRepository.DeleteBank(bank);
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
        public async Task<IActionResult> ActivateBank(int id)
        {
            try
            {
                var bank = await _repository.BankRepository.GetBankById(id);
                if (bank is null)
                {
                    _logger.LogError($"bank with id: {id}, hasn't been found in db.");
                    return BadRequest("bank object is null");
                }
                bank.IsActive = true;
                string result = await _repository.BankRepository.UpdateBank(bank);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateBank action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateBank(int id)
        {
            try
            {
                var bank = await _repository.BankRepository.GetBankById(id);
                if (bank is null)
                {
                    _logger.LogError($"bank with id: {id}, hasn't been found in db.");
                    return BadRequest("bank object is null");
                }
                bank.IsActive = false;
                string result = await _repository.BankRepository.UpdateBank(bank);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Deactivate Bank action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
