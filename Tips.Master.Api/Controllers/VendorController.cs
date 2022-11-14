using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using AutoMapper;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Entities.Migrations;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public VendorController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<VendorController>
        [HttpGet]
        public async Task<IActionResult> GetAllVendors()
        {
            ServiceResponse<IEnumerable<VendorMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<VendorMasterDto>>();

            try
            {
                var listOfVendors = await _repository.VendorRepository.GetAllVendors();
                _logger.LogInfo("Returned all Vendors");
                var result = _mapper.Map<IEnumerable<VendorMasterDto>>(listOfVendors);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<VendorController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorById(int id)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();

            try
            {
                var vendorDetails = await _repository.VendorRepository.GetVendorById(id);

                if (vendorDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Vendor with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Vendor with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<VendorMasterDto>(vendorDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetVendorDepartmentById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<VendorController>
        [HttpPost]
        public async Task<IActionResult> CreateVendor([FromBody] VendorMasterPostDto vendorMasterPost)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();

            try
            {
                if (vendorMasterPost is null)
                {
                    _logger.LogError("VendorDetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "VendorDetails object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
               if (!ModelState.IsValid)
               {
                    _logger.LogError("Invalid VendorDetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var vendor = _mapper.Map<VendorMaster>(vendorMasterPost);
                var address = _mapper.Map<IEnumerable<VendorAddress>>(vendorMasterPost.Addresses);
                var contact = _mapper.Map<IEnumerable<VendorContacts>>(vendorMasterPost.Contacts);
                var banking = _mapper.Map<IEnumerable<VendorBanking>>(vendorMasterPost.VendorBankings);               


                _repository.VendorRepository.CreateVendor(vendor); 

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetVendorCategoryById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<VendorController>/5
        [HttpPut("{id}")]
      
        public async Task<IActionResult> UpdateVolumeUom(int id, [FromBody] VendorMasterDto vendorMasterUpdateDto)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();

            try
            {
                if (vendorMasterUpdateDto is null)
                {
                    _logger.LogError("Update Vendor object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Vendor object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update Vendor object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updatevendor = await _repository.VendorRepository.GetVendorById(id);
                if (updatevendor is null)
                {
                    _logger.LogError($"Update Vendor with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Vendor with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

              

                var address = _mapper.Map<IEnumerable<VendorAddress>>(vendorMasterUpdateDto.Addresses);
                 
                var contact = _mapper.Map<IEnumerable<VendorContacts>>(vendorMasterUpdateDto.Contacts);

                var banking = _mapper.Map<IEnumerable<VendorBanking>>(vendorMasterUpdateDto.VendorBankings);
                
                var data = _mapper.Map(vendorMasterUpdateDto, updatevendor);


                data.Addresses = address.ToList();
                data.Contacts = contact.ToList();
                data.VendorBankings = banking.ToList();

                string result = await _repository.VendorRepository.UpdateVendor(data);
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
                _logger.LogError($"Something went wrong inside UpdateVolumeUom action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<VendorController>/5
        [HttpDelete("{id}")]
     public async Task<IActionResult> DeleteVendor(int id)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();

            try
            {
                var deleteVendor = await _repository.VendorRepository.GetVendorById(id);
                if (deleteVendor == null)
                {
                    _logger.LogError($"Delete vendor with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete vendor hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.VendorRepository.DeleteVendor(deleteVendor);
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
                return StatusCode(500, "Internal server error");
            }
        }
           
    }
}
