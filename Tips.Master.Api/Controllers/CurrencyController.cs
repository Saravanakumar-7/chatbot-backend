using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

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

         [HttpGet]
        public async Task<IActionResult> GetAllCurrency()
        {
            ServiceResponse<IEnumerable<CurrencyDto>> serviceResponse = new ServiceResponse<IEnumerable<CurrencyDto>>();

            try
            {
                var currencies = await _repository.CurrencyRepository.GetAllActiveCurrency();
                _logger.LogInfo("Returned all Currencies");
                var result = _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Currencies Successfully";
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
        public async Task<IActionResult> GetAllActivecurrencies()
        {
            ServiceResponse<IEnumerable<CurrencyDto>> serviceResponse = new ServiceResponse<IEnumerable<CurrencyDto>>();

            try
            {
                var currencies = await _repository.CostingMethodRepository.GetAllActiveCostingMethods();
                _logger.LogInfo("Returned all Currency");
                var result = _mapper.Map<IEnumerable<CurrencyDto>>(currencies);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Currency successfully";
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

 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurrencyById(int id)
        {
            ServiceResponse<CurrencyDto> serviceResponse = new ServiceResponse<CurrencyDto>();

            try
            {
                var currency = await _repository.CurrencyRepository.GetCurrencyById(id);
                if (currency == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Currency with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Currency with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<CurrencyDto>(currency);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCurrencyById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

 
        [HttpPost]
        public IActionResult CreateCurrency([FromBody] CurrencyPostDto currencyPostDto)
        {
            ServiceResponse<CurrencyDto> serviceResponse = new ServiceResponse<CurrencyDto>();

            try
            {
                if (currencyPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Currency object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Currency object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Currency object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Currency object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var currency = _mapper.Map<Currency>(currencyPostDto);
                _repository.CurrencyRepository.CreateCurrency(currency);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetCurrencyById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurrency(int id, [FromBody] CurrencyUpdateDto currencyUpdateDto)
        {
            ServiceResponse<CurrencyDto> serviceResponse = new ServiceResponse<CurrencyDto>();

            try
            {
                if (currencyUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update currency object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update Currency object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update currency object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update currency object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var currency = await _repository.CurrencyRepository.GetCurrencyById(id);
                if (currency is null)
                {
                    _logger.LogError($"Currency with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update BasicOfApproval with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(currencyUpdateDto, currency);
                string result = await _repository.CurrencyRepository.UpdateCurrency(currency);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
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
                _logger.LogError($"Something went wrong inside UpdateCurrency action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            ServiceResponse<CurrencyDto> serviceResponse = new ServiceResponse<CurrencyDto>();

            try
            {
                var currency = await _repository.CurrencyRepository.GetCurrencyById(id);

                if (currency == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Currency object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete Currency with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.CurrencyRepository.DeleteCurrency(currency);
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
        public async Task<IActionResult> ActivateCurrency(int id)
        {
            ServiceResponse<CurrencyDto> serviceResponse = new ServiceResponse<CurrencyDto>();

            try
            {
                var currency = await _repository.CurrencyRepository.GetCurrencyById(id);
                if (currency is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Currency object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Currency with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                currency.IsActive = true;
                string result = await _repository.CurrencyRepository.UpdateCurrency(currency);
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
                _logger.LogError($"Something went wrong inside ActivateCurrency action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCurrency(int id)
        {
            ServiceResponse<CurrencyDto> serviceResponse = new ServiceResponse<CurrencyDto>();

            try
            {
                var currency = await _repository.CurrencyRepository.GetCurrencyById(id);
                if (currency is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Currency object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Currency with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                currency.IsActive = false;
                string result = await _repository.CurrencyRepository.UpdateCurrency(currency);
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
                _logger.LogError($"Something went wrong inside DeactivateCurrency action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
