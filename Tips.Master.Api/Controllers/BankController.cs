using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> GetAllBankDetails([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<BankDto>> serviceResponse = new ServiceResponse<IEnumerable<BankDto>>();

            try
            {
                var GetallBanks = await _repository.BankRepository.GetAllActiveBank(searchParams);

                _logger.LogInfo("Returned all Bank");
                var result = _mapper.Map<IEnumerable<BankDto>>(GetallBanks);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Banks Successfully";
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
        public async Task<IActionResult> GetAllActiveBankDetails([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<BankDto>> serviceResponse = new ServiceResponse<IEnumerable<BankDto>>();

            try
            {
                var allActiveBanks = await _repository.BankRepository.GetAllActiveBank(searchParams);
                _logger.LogInfo("Returned all Banks");
                var result = _mapper.Map<IEnumerable<BankDto>>(allActiveBanks);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Banks Successfully";
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
        // GET api/<BankController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankById(int id)
        {
            ServiceResponse<BankDto> serviceResponse = new ServiceResponse<BankDto>();

            try
            {
                var BankbyId = await _repository.BankRepository.GetBankById(id);
                if (BankbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Bank with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Bank with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Bank with id: {id}");
                    var result = _mapper.Map<BankDto>(BankbyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Bank with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBankById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
              
            }
        }

        // POST api/<BankController>
        [HttpPost]
        public IActionResult CreateBank([FromBody] BankPostDto bankPostDto)
        {
            ServiceResponse<BankDto> serviceResponse = new ServiceResponse<BankDto>();

            try
            {
                if (bankPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Bank object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Bank object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Bank object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Bank object sent from client.");
                    return BadRequest(serviceResponse);
                } 

                var CreateBanks = _mapper.Map<Bank>(bankPostDto);
                _repository.BankRepository.CreateBank(CreateBanks);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetBankById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<BankController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBank(int id, [FromBody] BankUpdateDto bankUpdateDto)
        {
            ServiceResponse<BankDto> serviceResponse = new ServiceResponse<BankDto>();

            try
            {
                if (bankUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Bank object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Bank object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Bank object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Bank object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var BankUpdate = await _repository.BankRepository.GetBankById(id);
                if (BankUpdate is null)
                {
                    _logger.LogError($"Update Bank with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Bank with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(bankUpdateDto, BankUpdate);
                string result = await _repository.BankRepository.UpdateBank(BankUpdate);
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
                _logger.LogError($"Something went wrong inside UpdateBank action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<BankController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBank(int id)
        {
            ServiceResponse<BankDto> serviceResponse = new ServiceResponse<BankDto>();

            try
            {
                var Deletebank = await _repository.BankRepository.GetBankById(id);
                if (Deletebank == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Bank object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Bank with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.BankRepository.DeleteBank(Deletebank);
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
        public async Task<IActionResult> ActivateBank(int id)
        {
            ServiceResponse<BankDto> serviceResponse = new ServiceResponse<BankDto>();

            try
            {
                var BankActivate = await _repository.BankRepository.GetBankById(id);
                if (BankActivate is null)
                {                   
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Bank object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"bank with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                BankActivate.IsActive = true;
                string result = await _repository.BankRepository.UpdateBank(BankActivate);
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
                _logger.LogError($"Something went wrong inside ActivateBank action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateBank(int id)
        {
            ServiceResponse<BankDto> serviceResponse = new ServiceResponse<BankDto>();

            try
            {
                var BankDeactivate = await _repository.BankRepository.GetBankById(id);
                if (BankDeactivate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Bank object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"bank with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                BankDeactivate.IsActive = false;
                string result = await _repository.BankRepository.UpdateBank(BankDeactivate);
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
                _logger.LogError($"Something went wrong inside Deactivate Bank action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
