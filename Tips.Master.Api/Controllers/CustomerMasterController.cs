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
                var getAllCustomerMastersList = await _repository.CustomerMasterRepository.GetAllCustomerMasters(pagingParameter);
                //_logger.LogInfo("Returned all CustomerMaster");
                var metadata = new
                {
                    getAllCustomerMastersList.TotalCount,
                    getAllCustomerMastersList.PageSize,
                    getAllCustomerMastersList.CurrentPage,
                    getAllCustomerMastersList.HasNext,
                    getAllCustomerMastersList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var result = _mapper.Map<IEnumerable<CustomerMasterDto>>(getAllCustomerMastersList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CustomerMasters Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                 serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllCustomerMasters action: {ex.Message}";
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
                var getCustomerMasterById = await _repository.CustomerMasterRepository.GetCustomerMasterById(id);

                if (getCustomerMasterById == null)
                {
                    serviceResponse.Data= null;
                    serviceResponse.Message = $"CustomerMaster with id hasn't been found in db.";
                    serviceResponse.Success=false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CustomerMaster with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CustomerMaster with id: {id}");
                    var result = _mapper.Map<CustomerMasterDto>(getCustomerMasterById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CustomerMasterById Successfully";
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

        [HttpGet]
        public async Task<IActionResult> GetCustomerMasterByCustomerNo(string customerNumber)
        {
            ServiceResponse<CustomerMasterDto> serviceResponse = new ServiceResponse<CustomerMasterDto>();
            try
            {
                var customerMasterDetails = await _repository.CustomerMasterRepository.GetCustomerMasterByCustomerNo(customerNumber);

                if (customerMasterDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CustomerMaster with CustomerNumber hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CustomerMaster with CustomerNumber: {customerNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CustomerMaster with CustomerNumber: {customerNumber}");
                    var result = _mapper.Map<CustomerMasterDto>(customerMasterDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CustomerMasterByCustomerNo Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCustomerMasterByCustomerNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCustomerMasterByCustomerNo action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerMaster([FromBody] CustomerMasterDtoPost customerMasterDtoPost)
        {
            ServiceResponse<CustomerMasterDto> serviceResponse = new ServiceResponse<CustomerMasterDto>();
            //CustomerMaster customerDetail = null;

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

                var customerDetails = await _repository.CustomerMasterRepository.GetCSNumberAutoIncrementCount();
                var newcount = customerDetails?.Id;
                if (newcount > 0)
                {
                    var number = newcount + 1;
                    string e = String.Format("{0:D4}", number);
                    customerMaster.CustomerNumber = "CS" + (e);
                }
                else
                {
                    var count = 1;
                    var e = count.ToString("D4");
                    customerMaster.CustomerNumber = "CS" + (e);
                }


                await _repository.CustomerMasterRepository.CreateCustomerMaster(customerMaster);


                _repository.SaveAsync();

                var customerMasterDetails = await _repository.CustomerMasterRepository.GetCSNumberAutoIncrementCount();
                var customerData = _mapper.Map<CustomerMasterDto>(customerMasterDetails);



                serviceResponse.Data = customerData;
                serviceResponse.Message = "CustomerMaster Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.Created;
                return Created("GetCustomerMaster", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateCustomerMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpPut("{id}")]

        public async Task<IActionResult> UpdateCustomerMaster(int id, [FromBody] CustomerMasterDtoUpdate customerMasterDtoUpdate)
        {
            ServiceResponse<CustomerMasterDtoUpdate> serviceResponse = new ServiceResponse<CustomerMasterDtoUpdate>();
            try
            {
                if (customerMasterDtoUpdate is null)
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
                    serviceResponse.Message = $"Update CustomerMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

               
                var addresses = _mapper.Map<IEnumerable<CustomerAddresses>>(customerMasterDtoUpdate.CustomerAddress);
                var contacts = _mapper.Map<IEnumerable<CustomerContacts>>(customerMasterDtoUpdate.CustomerContacts);
                var shippingAddresses = _mapper.Map<IEnumerable<CustomerShippingAddresses>>(customerMasterDtoUpdate.CustomerShippingAddresses);
                var banking = _mapper.Map<IEnumerable<CustomerBanking>>(customerMasterDtoUpdate.CustomerBankings);
                var HeadcountDetails = _mapper.Map<IEnumerable<CustomerMasterHeadCounting>>(customerMasterDtoUpdate.CustomerMasterHeadCountings);

                var customerMasters = _mapper.Map(customerMasterDtoUpdate, updateCustomerMaster);

                customerMasters.CustomerAddresses= addresses.ToList();
                customerMasters.CustomerContacts= contacts.ToList();
                customerMasters.CustomerShippingAddresses= shippingAddresses.ToList();
                customerMasters.CustomerBanking= banking.ToList();
                customerMasters.CustomerMasterHeadCountings = HeadcountDetails.ToList();
                string result = await _repository.CustomerMasterRepository.UpdateCustomerMaster(customerMasters);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CustomerMaster Successfully Updated"; ;
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
                var deleteCustomerMaster = await _repository.CustomerMasterRepository.GetCustomerMasterById(id);
                if (deleteCustomerMaster == null)
                {
                    _logger.LogError($"Delete CustomerMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete CustomerMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.CustomerMasterRepository.DeleteCustomerMaster(deleteCustomerMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CustomerMaster Successfully Deleted"; 
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteCustomerMaster action: {ex.Message}");
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
                var listOfActiveCustomerMaster = await _repository.CustomerMasterRepository.GetAllActiveCustomerMasterIdNameList();
                //_logger.LogInfo("Returned all CustomerMaster");
                var result = _mapper.Map<IEnumerable<CustomerIdNameListDto>>(listOfActiveCustomerMaster);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveCustomerIdNameList";
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
