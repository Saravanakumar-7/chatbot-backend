using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using MySqlX.XDevAPI.Common;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CustomerTypeController : ControllerBase
    {
        private  IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private  IMapper _mapper;

        public CustomerTypeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<CustomerTypeController>
        [HttpGet]
        public async Task<IActionResult> GetAllCustomerTypes([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CustomerTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<CustomerTypeDto>>();
            try
            {

                var customerTypeList = await _repository.CustomerTypeRepository.GetAllCustomerTypes(searchParams);

                _logger.LogInfo("Returned all customerTypes");
                var result = _mapper.Map<IEnumerable<CustomerTypeDto>>(customerTypeList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all customerTypes Successfully";
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


        // GET: api/<CustomerTypeController>
        [HttpGet]
        public async Task<IActionResult> GetAllActiveCustomerTypes([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CustomerTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<CustomerTypeDto>>();

            try
            {
                var customerTypeList = await _repository.CustomerTypeRepository.GetAllActiveCustomerTypes(searchParams);
                _logger.LogInfo("Returned all customerTypes");
                var result = _mapper.Map<IEnumerable<CustomerTypeDto>>(customerTypeList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active customerTypes Successfully";
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

        // GET api/<CustomerTypeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerTypeById(int id)
        {
            ServiceResponse<CustomerTypeDto> serviceResponse = new ServiceResponse<CustomerTypeDto>();
            try
            {
                var customerType = await _repository.CustomerTypeRepository.GetCustomerTypeById(id);
                if (customerType == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CustomerType with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CustomerType with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned CustomerType with id: {id}");
                    var result = _mapper.Map<CustomerTypeDto>(customerType);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CustomerType with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCustomerTypeById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<CustomerTypeController>
        [HttpPost]
        public IActionResult CreateCustomerType([FromBody] CustomerTypeDtoPost customerTypeDtoPost)
        {
            ServiceResponse<CustomerTypeDto> serviceResponse = new ServiceResponse<CustomerTypeDto>();

            try
            {
                if (customerTypeDtoPost is null)
                {
                    _logger.LogError("CustomerType object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CustomerType object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid CustomerType object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Something went wrong. Please try again!";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var customerTypeEntity = _mapper.Map<CustomerType>(customerTypeDtoPost);
                _repository.CustomerTypeRepository.CreateCustomerType(customerTypeEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetCustomerTypeById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<CustomerTypeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerType(int id, [FromBody] CustomerTypeDtoUpdate customerTypeDtoUpdate)
        {
            ServiceResponse<CustomerTypeDto> serviceResponse = new ServiceResponse<CustomerTypeDto>();

            try
            {
                if (customerTypeDtoUpdate is null)
                {
                    _logger.LogError("update CustomerType object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update CustomerType object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid update CustomerType object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Something went wrong. Please try again!";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var customerTypeEntity = await _repository.CustomerTypeRepository.GetCustomerTypeById(id);
                if (customerTypeEntity is null)
                {
                    _logger.LogError($"update CustomerType with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update CustomerType with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(customerTypeDtoUpdate, customerTypeEntity);
                string result =  await _repository.CustomerTypeRepository.UpdateCustomerType(customerTypeEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCustomerType action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<CustomerTypeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerType(int id)
        {
            ServiceResponse<CustomerTypeDto> serviceResponse = new ServiceResponse<CustomerTypeDto>();

            try
            {
                var customerType = await _repository.CustomerTypeRepository.GetCustomerTypeById(id);
                if (customerType == null)
                {
                    _logger.LogError($"Delete CustomerType with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Purchasegroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.CustomerTypeRepository.DeleteCustomerType(customerType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCustomerType(int id)
        {
            ServiceResponse<CustomerTypeDto> serviceResponse = new ServiceResponse<CustomerTypeDto>();

            try
            {
                var customerType = await _repository.CustomerTypeRepository.GetCustomerTypeById(id);
                if (customerType is null)
                {
                    _logger.LogError($"CustomerType with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CustomerType object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                customerType.IsActive = true;
                string result = await _repository.CustomerTypeRepository.UpdateCustomerType(customerType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Activate Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateCustomerType action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCustomerType(int id)
        {
            ServiceResponse<CustomerTypeDto> serviceResponse = new ServiceResponse<CustomerTypeDto>();

            try
            {
                var customerType = await _repository.CustomerTypeRepository.GetCustomerTypeById(id);
                if (customerType is null)
                {
                    _logger.LogError($"CustomerType with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CustomerType object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                customerType.IsActive = false;
                string result = await _repository.CustomerTypeRepository.UpdateCustomerType(customerType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "DeActivate Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateCustomerType action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
