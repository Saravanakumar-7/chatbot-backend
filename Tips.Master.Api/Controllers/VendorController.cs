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
        
            public async Task<IActionResult> GetAllVendorMasters([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
            { 
                ServiceResponse<IEnumerable<VendorMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<VendorMasterDto>>();

            try
            {
                 var getAllVendorMastersList = await _repository.VendorRepository.GetAllVendorMasters(pagingParameter, searchParams);
                var metadata = new
                {
                    getAllVendorMastersList.TotalCount,
                    getAllVendorMastersList.PageSize,
                    getAllVendorMastersList.CurrentPage,
                    getAllVendorMastersList.HasNext,
                    getAllVendorMastersList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all VendorMasters");
                var result = _mapper.Map<IEnumerable<VendorMasterDto>>(getAllVendorMastersList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all VendorMasters Successfully";
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

        // GET api/<VendorController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorMasterById(int id)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();

            try
            {
                var getvendorMasterById = await _repository.VendorRepository.GetVendorMasterById(id);

                if (getvendorMasterById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"VendorMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Vendor with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned VendorMaster with id: {id}");
                    var result = _mapper.Map<VendorMasterDto>(getvendorMasterById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned VendorMasterById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetVendorMasterById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<VendorController>
        [HttpPost]
        public async Task<IActionResult> CreateVendorMaster([FromBody] VendorMasterPostDto vendorMasterPost)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();

            try
            {
                if (vendorMasterPost is null)
                {
                    _logger.LogError("VendorMasters object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "VendorMasters object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
               if (!ModelState.IsValid)
               {
                    _logger.LogError("Invalid VendorMasters object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid VendorMasters object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var address = _mapper.Map<IEnumerable<VendorAddress>>(vendorMasterPost.Addresses);
                var contact = _mapper.Map<IEnumerable<VendorContacts>>(vendorMasterPost.Contacts);
                var banking = _mapper.Map<IEnumerable<VendorBanking>>(vendorMasterPost.VendorBankings);
                var related = _mapper.Map<IEnumerable<VendorRelatedVendor>>(vendorMasterPost.RelatedVendors);
                var headcount = _mapper.Map<IEnumerable<VendorHeadCounting>>(vendorMasterPost.HeadCountings);
                var vendorMaster = _mapper.Map<VendorMaster>(vendorMasterPost);

                vendorMaster.Addresses = address.ToList();
                vendorMaster.Contacts = contact.ToList();
                vendorMaster.VendorBankings = banking.ToList();
                vendorMaster.RelatedVendors = related.ToList();
                vendorMaster.HeadCountings = headcount.ToList();

                await _repository.VendorRepository.CreateVendorMaster(vendorMaster); 

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "VendorMaster Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetVendorById",serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateVendorMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<VendorController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorMaster(int id, [FromBody] VendorMasterDto vendorMasterUpdateDto)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();

            try
            {
                if (vendorMasterUpdateDto is null)
                {
                    _logger.LogError("Update VendorMasters object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update VendorMasters object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update VendorMasters object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update VendorMasters object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateVendorMaster = await _repository.VendorRepository.GetVendorMasterById(id);
                if (updateVendorMaster is null)
                {
                    _logger.LogError($"Update VendorMasters with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update VendorMasters with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

              

                var address = _mapper.Map<IEnumerable<VendorAddress>>(vendorMasterUpdateDto.Addresses);
                 
                var contact = _mapper.Map<IEnumerable<VendorContacts>>(vendorMasterUpdateDto.Contacts);

                var banking = _mapper.Map<IEnumerable<VendorBanking>>(vendorMasterUpdateDto.VendorBankings);
                var related = _mapper.Map<IEnumerable<VendorRelatedVendor>>(vendorMasterUpdateDto.RelatedVendors);

                var headcount = _mapper.Map<IEnumerable<VendorHeadCounting>>(vendorMasterUpdateDto.HeadCountings);

                var vendorMaster = _mapper.Map(vendorMasterUpdateDto, updateVendorMaster);


                vendorMaster.Addresses = address.ToList();
                vendorMaster.Contacts = contact.ToList();
                vendorMaster.VendorBankings = banking.ToList();
                vendorMaster.RelatedVendors = related.ToList();
                vendorMaster.HeadCountings = headcount.ToList();

                string result = await _repository.VendorRepository.UpdateVendorMaster(vendorMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "VendorMaster Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateVendorMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<VendorController>/5
        [HttpDelete("{id}")]
     public async Task<IActionResult> DeleteVendorMaster(int id)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();

            try
            {
                var deleteVendorMaster = await _repository.VendorRepository.GetVendorMasterById(id);
                if (deleteVendorMaster == null)
                {
                    _logger.LogError($"Delete VendorMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete VendorMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.VendorRepository.DeleteVendorMaster(deleteVendorMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "VendorMaster Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteVendorMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveVendorNameList()
        {
            ServiceResponse<IEnumerable<VendorIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<VendorIdNameListDto>>();
            try
            {
                var getAllActiveVendorNameList = await _repository.VendorRepository.GetAllActiveVendorMasterNameList();
                var result = _mapper.Map<IEnumerable<VendorIdNameListDto>>(getAllActiveVendorNameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveVendorNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveVendorNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVendorNameList()
        {
            ServiceResponse<IEnumerable<VendorIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<VendorIdNameListDto>>();
            try
            {
                var getAllVendorNameList = await _repository.VendorRepository.GetAllVendorMasterNameList();
                var result = _mapper.Map<IEnumerable<VendorIdNameListDto>>(getAllVendorNameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All VendorNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllVendorNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateVendorMaster(int id)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();
            try
            {
                var vendorMasters = await _repository.VendorRepository.GetVendorMasterById(id);
                if (vendorMasters is null)
                {
                    _logger.LogError($"vendorMasters with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "vendorMasters object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                vendorMasters.IsActive = true;
                string result = await _repository.VendorRepository.UpdateVendorMaster(vendorMasters);
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
                _logger.LogError($"Something went wrong inside ActivateVendorMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateVendorMaster(int id)
        {
            ServiceResponse<VendorMasterDto> serviceResponse = new ServiceResponse<VendorMasterDto>();
            try
            {
                var vendorMasters = await _repository.VendorRepository.GetVendorMasterById(id);
                if (vendorMasters is null)
                {
                    _logger.LogError($"vendorMasters with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "vendorMasters object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                vendorMasters.IsActive = false;
                string result = await _repository.VendorRepository.UpdateVendorMaster(vendorMasters);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Deactivate Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateVendorMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
