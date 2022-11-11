using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
        public async Task<IActionResult> GetAllCustomerTypes()
        {
            try
            {
                var customerTypeList = await _repository.CustomerTypeRepository.GetAllCustomerTypes();
                _logger.LogInfo("Returned all customerTypes");
                var result = _mapper.Map<IEnumerable<CustomerTypeDto>>(customerTypeList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/<CustomerTypeController>
        [HttpGet]
        public async Task<IActionResult> GetAllActiveCustomerTypes()
        {
            try
            {
                var customerTypeList = await _repository.CustomerTypeRepository.GetAllActiveCustomerTypes();
                _logger.LogInfo("Returned all customerTypes");
                var result = _mapper.Map<IEnumerable<CustomerTypeDto>>(customerTypeList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<CustomerTypeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerTypeById(int id)
        {
            try
            {
                var customerType = await _repository.CustomerTypeRepository.GetCustomerTypeById(id);
                if (customerType == null)
                {
                    _logger.LogError($"CustomerType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<CustomerTypeDto>(customerType);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCustomerTypeById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<CustomerTypeController>
        [HttpPost]
        public IActionResult CreateCustomerType([FromBody] CustomerTypeDtoPost customerTypeDtoPost)
        {
            try
            {
                if (customerTypeDtoPost is null)
                {
                    _logger.LogError("CustomerType object sent from client is null.");
                    return BadRequest("CustomerType object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid CustomerType object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var customerTypeEntity = _mapper.Map<CustomerType>(customerTypeDtoPost);
                _repository.CustomerTypeRepository.CreateCustomerType(customerTypeEntity);
                _repository.SaveAsync();

                
                return Created("GetCustomerTypeById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<CustomerTypeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerType(int id, [FromBody] CustomerTypeDtoUpdate customerTypeDtoUpdate)
        {
            try
            {
                if (customerTypeDtoUpdate is null)
                {
                    _logger.LogError("CustomerType object sent from client is null.");
                    return BadRequest("CustomerType object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid CustomerType object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var customerTypeEntity = await _repository.CustomerTypeRepository.GetCustomerTypeById(id);
                if (customerTypeEntity is null)
                {
                    _logger.LogError($"CustomerType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(customerTypeDtoUpdate, customerTypeEntity);
                string result =  await _repository.CustomerTypeRepository.UpdateCustomerType(customerTypeEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCustomerType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<CustomerTypeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerType(int id)
        {
            try
            {
                var customerType = await _repository.CustomerTypeRepository.GetCustomerTypeById(id);
                if (customerType == null)
                {
                    _logger.LogError($"CustomerType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.CustomerTypeRepository.DeleteCustomerType(customerType);
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
        public async Task<IActionResult> ActivateCustomerType(int id)
        {
            try
            {
                var customerType = await _repository.CustomerTypeRepository.GetCustomerTypeById(id);
                if (customerType is null)
                {
                    _logger.LogError($"CustomerType with id: {id}, hasn't been found in db.");
                    return BadRequest("CustomerType object is null");
                }
                customerType.IsActive = true;
                string result = await _repository.CustomerTypeRepository.UpdateCustomerType(customerType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateCustomerType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCustomerType(int id)
        {
            try
            {
                var customerType = await _repository.CustomerTypeRepository.GetCustomerTypeById(id);
                if (customerType is null)
                {
                    _logger.LogError($"CustomerType with id: {id}, hasn't been found in db.");
                    return BadRequest("CustomerType object is null");
                }
                customerType.IsActive = false;
                string result = await _repository.CustomerTypeRepository.UpdateCustomerType(customerType);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateCustomerType action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
