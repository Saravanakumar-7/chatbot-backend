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
    public class CurrencyController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public CurrencyController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }

        // GET: api/<CurrencyController>
        [HttpGet]
        public async Task<IActionResult> GetAllCurrency()
        {
            try
            {
                var currencies = await _repository.CurrencyRepository.GetAllActiveCurrency();
                _logger.LogInfo("Returned all Currencies");
                var result = _mapper.Map<IEnumerable<DeliveryTermGetDto>>(currencies);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<CurrencyController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurrencyById(int id)
        {
            try
            {
                var currency = await _repository.CurrencyRepository.GetCurrencyById(id);
                if (currency == null)
                {
                    _logger.LogError($"Currency with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<CurrencyDto>(currency);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCurrencyById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<CurrencyController>
        [HttpPost]
        public IActionResult CreateCurrency([FromBody] CurrencyPostDto currencyPostDto)
        {
            try
            {
                if (currencyPostDto is null)
                {
                    _logger.LogError("Currency object sent from client is null.");
                    return BadRequest("Currency object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Currency object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var currency = _mapper.Map<Currency>(currencyPostDto);
                _repository.CurrencyRepository.CreateCurrency(currency);
                _repository.SaveAsync();


                return Created("GetCurrencyById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<CurrencyController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurrency(int id, [FromBody] CurrencyUpdateDto currencyUpdateDto)
        {
            try
            {
                if (currencyUpdateDto is null)
                {
                    _logger.LogError("Currency object sent from client is null.");
                    return BadRequest("currency object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid currency object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var currency = await _repository.CurrencyRepository.GetCurrencyById(id);
                if (currency is null)
                {
                    _logger.LogError($"Currency with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(currencyUpdateDto, currency);
                string result = await _repository.CurrencyRepository.UpdateCurrency(currency);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCurrency action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<CurrencyController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            try
            {
                var currency = await _repository.CurrencyRepository.GetCurrencyById(id);

                if (currency == null)
                {
                    _logger.LogError($"Currency with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.CurrencyRepository.DeleteCurrency(currency);
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
        public async Task<IActionResult> ActivateCurrency(int id)
        {
            try
            {
                var currency = await _repository.CurrencyRepository.GetCurrencyById(id);
                if (currency is null)
                {
                    _logger.LogError($"Currency with id: {id}, hasn't been found in db.");
                    return BadRequest("Currency object is null");
                }
                currency.IsActive = true;
                string result = await _repository.CurrencyRepository.UpdateCurrency(currency);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateCurrency action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCurrency(int id)
        {
            try
            {
                var currency = await _repository.CurrencyRepository.GetCurrencyById(id);
                if (currency is null)
                {
                    _logger.LogError($"Currency with id: {id}, hasn't been found in db.");
                    return BadRequest("Currency object is null");
                }
                currency.IsActive = false;
                string result = await _repository.CurrencyRepository.UpdateCurrency(currency);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateCurrency action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
