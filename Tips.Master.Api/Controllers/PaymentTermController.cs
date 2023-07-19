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
    public class PaymentTermController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public PaymentTermController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<PaymentTermController>
        [HttpGet]
        public async Task<IActionResult> GetAllPaymentTerms([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<PaymentTermDto>> serviceResponse = new ServiceResponse<IEnumerable<PaymentTermDto>>();
            try
            {

                var PaymentTermList = await _repository.PaymentTermRepository.GetAllpaymentTerms(searchParams);
                _logger.LogInfo("Returned all PaymentTermList");
                var result = _mapper.Map<IEnumerable<PaymentTermDto>>(PaymentTermList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PaymentTermList Successfully";
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
        public async Task<IActionResult> GetAllActivePaymentTerms([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<PaymentTermDto>> serviceResponse = new ServiceResponse<IEnumerable<PaymentTermDto>>();

            try
            {
                var Patmentterms = await _repository.PaymentTermRepository.GetAllActivepaymentTerms(searchParams);
                _logger.LogInfo("Returned all Patmentterms");
                var result = _mapper.Map<IEnumerable<PaymentTermDto>>(Patmentterms);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Patmentterms Successfully";
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

        // GET api/<PaymentTermController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentTermById(int id)
        {
            ServiceResponse<PaymentTermDto> serviceResponse = new ServiceResponse<PaymentTermDto>();

            try
            {
                var PaymentTerm = await _repository.PaymentTermRepository.GetpaymentTermById(id);
                if (PaymentTerm == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PaymentTerm with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PaymentTerm with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<PaymentTermDto>(PaymentTerm);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPaymentTermById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<PaymentTermController>
        [HttpPost]
        public IActionResult CreatePaymentTerm([FromBody] PaymentTermDtoPost paymentTermDtoPost)
        {
            ServiceResponse<AuditFrequencyDto> serviceResponse = new ServiceResponse<AuditFrequencyDto>();

            try
            {
                if (paymentTermDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PaymentTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PaymentTerm object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PaymentTerm object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PaymentTerm object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var PaymentTerm = _mapper.Map<PaymentTerm>(paymentTermDtoPost);
                _repository.PaymentTermRepository.CreatePaymentTerm(PaymentTerm);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetPaymentTermById", serviceResponse);
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

        // PUT api/<PaymentTermController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePaymentTerm(int id, [FromBody] PaymentTermDtoUpdate paymentTermDtoUpdate)
        {
            ServiceResponse<PaymentTermDto> serviceResponse = new ServiceResponse<PaymentTermDto>();

            try
            {
                if (paymentTermDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update PaymentTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update PaymentTerm object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update PaymentTerm object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update PaymentTerm object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var PaymentTerm = await _repository.PaymentTermRepository.GetpaymentTermById(id);
                if (PaymentTerm is null)
                {
                    _logger.LogError($"Update PaymentTerm with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update PaymentTerm with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(paymentTermDtoUpdate, PaymentTerm);
                string result = await _repository.PaymentTermRepository.UpdatePaymentTerm(PaymentTerm);
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
                _logger.LogError($"Something went wrong inside UpdatePaymentTerms action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<PaymentTermController>/5
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentTerm(int id)
        {
            ServiceResponse<PaymentTermDto> serviceResponse = new ServiceResponse<PaymentTermDto>();

            try
            {
                var PaymentTerm = await _repository.PaymentTermRepository.GetpaymentTermById(id);
                if (PaymentTerm == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PaymentTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PaymentTerm with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.PaymentTermRepository.DeletePaymentTerm(PaymentTerm);
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
                _logger.LogError($"Something went wrong inside DeletePaymentTerm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivatePaymentTerm(int id)
        {
            ServiceResponse<PaymentTermDto> serviceResponse = new ServiceResponse<PaymentTermDto>();

            try
            {
                var PaymentTerm = await _repository.PaymentTermRepository.GetpaymentTermById(id);
                if (PaymentTerm is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PaymentTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PaymentTerm with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                PaymentTerm.IsActive = true;
                string result = await _repository.PaymentTermRepository.UpdatePaymentTerm(PaymentTerm);
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
                _logger.LogError($"Something went wrong inside ActivatedPaymentTerm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivatePaymentTerm(int id)
        {
            ServiceResponse<PaymentTermDto> serviceResponse = new ServiceResponse<PaymentTermDto>();

            try
            {
                var PaymentTerm = await _repository.PaymentTermRepository.GetpaymentTermById(id);
                if (PaymentTerm is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PaymentTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PaymentTerm with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                PaymentTerm.IsActive = false;
                string result = await _repository.PaymentTermRepository.UpdatePaymentTerm(PaymentTerm);
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
                _logger.LogError($"Something went wrong inside DeactivatePaymentTerm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
