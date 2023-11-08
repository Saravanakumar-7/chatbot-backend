using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerCategoryController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public CustomerCategoryController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
      
        [HttpGet]
        public async Task<IActionResult> GetAllCustomerCategory([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CustomerCategory>> serviceResponse = new ServiceResponse<IEnumerable<CustomerCategory>>();

            try
            {
                var customerCategories = await _repository.CustomerCategoryRepository.GetAllCustomerCategory(searchParams);
                _logger.LogInfo("Returned all customerCategories");
                var result = _mapper.Map<IEnumerable<CustomerCategory>>(customerCategories);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all customerCategories Successfully";
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
        public async Task<IActionResult> GetAllActiveCustomerCategory([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CustomerCategoryDto>> serviceResponse = new ServiceResponse<IEnumerable<CustomerCategoryDto>>();

            try
            {
                var customerCategories = await _repository.CustomerCategoryRepository.GetAllActiveCustomerCategory(searchParams);
                _logger.LogInfo("Returned all companyCategories");
                var result = _mapper.Map<IEnumerable<CustomerCategoryDto>>(customerCategories);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active customerCategories Successfully";
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
        public async Task<IActionResult> GetCustomerCategoryById(int id)
        {
            ServiceResponse<CustomerCategoryDto> serviceResponse = new ServiceResponse<CustomerCategoryDto>();

            try
            {
                var customerCategoryDetailById = await _repository.CustomerCategoryRepository.GetCustomerCategoryById(id);
                if (customerCategoryDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CustomerCategory with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"CustomerCategory with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CustomerCategory with id: {id}");
                    var result = _mapper.Map<CustomerCategoryDto>(customerCategoryDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CustomerCategory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCustomerCategoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateCustomerCategory([FromBody] CustomerCategoryPostDto customerCategoryPost)
        {
            ServiceResponse<CustomerCategoryPostDto> serviceResponse = new ServiceResponse<CustomerCategoryPostDto>();

            try
            {
                if (customerCategoryPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CustomerCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("CustomerCategory object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CustomerCategory object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid CustomerCategory object sent from client.");
                    return BadRequest(serviceResponse);
                }


                var companyCategory = _mapper.Map<CustomerCategory>(customerCategoryPost);
                await _repository.CustomerCategoryRepository.CreateCustomerCategory(companyCategory);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                return Created("GetCustomerCategoryById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateCustomerCategory action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerCategory(int id, [FromBody] CustomerCategoryUpdateDto customerCategoryUpdate)
        {
            ServiceResponse<CustomerCategoryUpdateDto> serviceResponse = new ServiceResponse<CustomerCategoryUpdateDto>();

            try
            {
                if (customerCategoryUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update CustomerCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update CustomerCategory object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update CustomerCategory object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update Invalid CustomerCategory object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var customerCategoryById = await _repository.CustomerCategoryRepository.GetCustomerCategoryById(id);
                if (customerCategoryById is null)
                {
                    _logger.LogError($"Update CustomerCategory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update CustomerCategory with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                _mapper.Map(customerCategoryUpdate, customerCategoryById);
                string result = await _repository.CustomerCategoryRepository.UpdateCustomerCategory(customerCategoryById);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateCustomerCategory action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<VendorCategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerCategory(int id)
        {
            ServiceResponse<CustomerCategory> serviceResponse = new ServiceResponse<CustomerCategory>();

            try
            {
                var customerCategory = await _repository.CustomerCategoryRepository.GetCustomerCategoryById(id);
                if (customerCategory == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete CustomerCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete CustomerCategory with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.CustomerCategoryRepository.DeleteCustomerCategory(customerCategory);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeleteCustomerCategory action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCustomerCategory(int id)
        {
            ServiceResponse<CustomerCategory> serviceResponse = new ServiceResponse<CustomerCategory>();

            try
            {
                var customerCategory = await _repository.CustomerCategoryRepository.GetCustomerCategoryById(id);
                if (customerCategory is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "customerCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"customerCategory with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                customerCategory.IsActive = true;
                string result = await _repository.CustomerCategoryRepository.UpdateCustomerCategory(customerCategory);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateCustomerCategory action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCustomerCategory(int id)
        {
            ServiceResponse<CustomerCategory> serviceResponse = new ServiceResponse<CustomerCategory>();

            try
            {
                var customerCategory = await _repository.CustomerCategoryRepository.GetCustomerCategoryById(id);
                if (customerCategory is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "customerCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"customerCategory with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                customerCategory.IsActive = false;
                string result = await _repository.CustomerCategoryRepository.UpdateCustomerCategory(customerCategory);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateCustomerCategory action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
       