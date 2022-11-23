using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using AutoMapper;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Entities.Migrations;
using System.Net;
using Newtonsoft.Json;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerMasterController : ControllerBase
    {
            private IRepositoryWrapperForMaster _repository;
            private ILoggerManager _logger;
            private IMapper _mapper;

            public CustomerMasterController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
            {
                _repository = repository;
                _logger = logger;
                _mapper = mapper;
            }
 
        [HttpGet]
        public async Task<IActionResult> GetAllCustomerMaster([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<CustomerMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<CustomerMasterDto>>();
            try
            {
                var listOfCustomerMaster = await _repository.CustomerMasterRepository.GetAllCustomerMaster(pagingParameter);
                //_logger.LogInfo("Returned all CustomerMaster");
                var metadata = new
                {
                    listOfCustomerMaster.TotalCount,
                    listOfCustomerMaster.PageSize,
                    listOfCustomerMaster.CurrentPage,
                    listOfCustomerMaster.HasNext,
                    listOfCustomerMaster.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var result = _mapper.Map<IEnumerable<CustomerMasterDto>>(listOfCustomerMaster);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Customers Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                 serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllCustomerMaster action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerMasterById(int id)
        {
            ServiceResponse<CustomerMasterDto> serviceResponse = new ServiceResponse<CustomerMasterDto>();
            try
            {
                var CustomerMasterDetails = await _repository.CustomerMasterRepository.GetCustomerMasterById(id);

                if (CustomerMasterDetails == null)
                {
                    serviceResponse.Data= null;
                    serviceResponse.Message = $"CustomerMaster with id: {id}, hasn't been found in db.";
                    serviceResponse.Success=false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CustomerMaster with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CustomerMaster with id: {id}");
                    var result = _mapper.Map<CustomerMasterDto>(CustomerMasterDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CustomerMaster with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCustomerMasterById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCustomerMasterById action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpPost]
        public async Task<IActionResult> CreateCustomerMaster([FromBody] CustomerMasterDtoPost customerMasterDtoPost)
        {
            ServiceResponse<CustomerMasterDtoPost> serviceResponse = new ServiceResponse<CustomerMasterDtoPost>();
            try
            {
                if (customerMasterDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CustomerMaster object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("CustomerMaster object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CustomerMaster object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid CustomerMaster object sent from client.");
                    return BadRequest("Invalid model object");
                }

                
                var contacts = _mapper.Map<IEnumerable<CustomerContacts>>(customerMasterDtoPost.CustomerContacts);
                var shippingAddresses = _mapper.Map<IEnumerable<CustomerShippingAddresses>>(customerMasterDtoPost.CustomerShippingAddresses);
                var addresses = _mapper.Map<IEnumerable<CustomerAddresses>>(customerMasterDtoPost.CustomerAddress);
                var banking = _mapper.Map<IEnumerable<CustomerBanking>>(customerMasterDtoPost.CustomerBankings);
                var headcount = _mapper.Map<IEnumerable<CustomerMasterHeadCounting>>(customerMasterDtoPost.CustomerMasterHeadCountings);

                var customerMaster = _mapper.Map<CustomerMaster>(customerMasterDtoPost);

                customerMaster.CustomerAddresses = addresses.ToList();
                customerMaster.CustomerContacts = contacts.ToList();
                customerMaster.CustomerShippingAddresses = shippingAddresses.ToList();
                customerMaster.CustomerBanking = banking.ToList();
                customerMaster.CustomerMasterHeadCountings= headcount.ToList();

                await _repository.CustomerMasterRepository.CreateCustomerMaster(customerMaster);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " CustomerMaster Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.Created;
                return Created("GetCustomerMasterById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpPut("{id}")]

        public async Task<IActionResult> UpdateCustomerMaster(int id, [FromBody] CustomerMasterDtoPost customerMasterDto)
        {
            ServiceResponse<CustomerMasterDto> serviceResponse = new ServiceResponse<CustomerMasterDto>();
            try
            {
                if (customerMasterDto is null)
                {
                    _logger.LogError("Update CustomerMaster object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update CustomerMaster object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update CustomerMaster object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update CustomerMaster object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest("Invalid model object");
                }
                var updateCustomerMaster = await _repository.CustomerMasterRepository.GetCustomerMasterById(id);
                if (updateCustomerMaster is null)
                {
                    _logger.LogError($"Update CustomerMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update CustomerMaster with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

               
                var addresses = _mapper.Map<IEnumerable<CustomerAddresses>>(customerMasterDto.CustomerAddress);
                var contacts = _mapper.Map<IEnumerable<CustomerContacts>>(customerMasterDto.CustomerContacts);
                var shippingAddresses = _mapper.Map<IEnumerable<CustomerShippingAddresses>>(customerMasterDto.CustomerShippingAddresses);
                var banking = _mapper.Map<IEnumerable<CustomerBanking>>(customerMasterDto.CustomerBankings);
                var HeadcountDetails = _mapper.Map<IEnumerable<CustomerMasterHeadCounting>>(customerMasterDto.CustomerMasterHeadCountings);

                var customerDetials = _mapper.Map(customerMasterDto, updateCustomerMaster);
                //var customerDetials = _mapper.Map<CustomerMaster>(customerMasterDto);

                customerDetials.CustomerAddresses= addresses.ToList();
                customerDetials.CustomerContacts= contacts.ToList();
                customerDetials.CustomerShippingAddresses= shippingAddresses.ToList();
                customerDetials.CustomerBanking= banking.ToList();
                customerDetials.CustomerMasterHeadCountings = HeadcountDetails.ToList();
                string result = await _repository.CustomerMasterRepository.UpdateCustomerMaster(customerDetials);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = result;
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCustomerMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerMaster(int id)
        {
            ServiceResponse<CustomerMasterDto> serviceResponse = new ServiceResponse<CustomerMasterDto>();
            try
            {
                var deleteCustomer = await _repository.CustomerMasterRepository.GetCustomerMasterById(id);
                if (deleteCustomer == null)
                {
                    _logger.LogError($"Delete Customer with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Customer with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.CustomerMasterRepository.DeleteCustomerMaster(deleteCustomer);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = result;
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

        [HttpGet]
        public async Task<IActionResult> GetAllActiveCustomerIdNameList()
        {
            ServiceResponse<IEnumerable<CustomerIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<CustomerIdNameListDto>>();
            try
            {
                var listOfCustomerMaster = await _repository.CustomerMasterRepository.GetAllActiveCustomerIdNameList();
                //_logger.LogInfo("Returned all CustomerMaster");
                var result = _mapper.Map<IEnumerable<CustomerIdNameListDto>>(listOfCustomerMaster);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveCustomerIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


    }
}
