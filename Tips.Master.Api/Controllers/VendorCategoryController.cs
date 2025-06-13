using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class VendorCategoryController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public VendorCategoryController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<VendorCategoryController>
        [HttpGet]
        public async Task<IActionResult> GetAllVendorCategory([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<VendorCategory>> serviceResponse = new ServiceResponse<IEnumerable<VendorCategory>>();

            try
            {
                var vendorCategories = await _repository.VendorCategoryRepository.GetAllVendorCategory(searchParams);
                _logger.LogInfo("Returned all VendorCategory");
                var result = _mapper.Map<IEnumerable<VendorCategory>>(vendorCategories);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all VendorCategory Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllVendorCategory API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllVendorCategory API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveVendorCatefories([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<VendorCategoryDto>> serviceResponse = new ServiceResponse<IEnumerable<VendorCategoryDto>>();

            try
            {
                var vendorCategories = await _repository.VendorCategoryRepository.GetAllActiveVendorCategory(searchParams);
                _logger.LogInfo("Returned all VendorCategory");
                var result = _mapper.Map<IEnumerable<VendorCategoryDto>>(vendorCategories);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active VendorCategory Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveVendorCatefories API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveVendorCatefories API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }
        // GET api/<VendorCategoryController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorCategoryById(int id)
        {
            ServiceResponse<VendorCategoryDto> serviceResponse = new ServiceResponse<VendorCategoryDto>();

            try
            {
                var vendorCategory = await _repository.VendorCategoryRepository.GetVendorCategoryById(id);
                if (vendorCategory == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Department with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"VendorCategory with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<VendorCategoryDto>(vendorCategory);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetVendorCategoryById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetVendorCategoryById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        // POST api/<VendorCategoryController>
        [HttpPost]
        public IActionResult CreateVendorCategory([FromBody] VendorCategoryPostDto vendorCategoryPostDto)
        {
            ServiceResponse<VendorCategory> serviceResponse = new ServiceResponse<VendorCategory>();

            try
            {
                if (vendorCategoryPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("VendorCategory object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid VendorCategory object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid VendorCategory object sent from client.");
                    return BadRequest(serviceResponse);
                }
                //var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                //var id = _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                //_repository.SaveAsync();

                var vendorCategory = _mapper.Map<VendorCategory>(vendorCategoryPostDto);
                _repository.VendorCategoryRepository.CreateVendorCategory(vendorCategory);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                return Created("GetVendorCategoryById",serviceResponse);                 
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateVendorCategory API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in CreateVendorCategory API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<VendorCategoryController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorCategory(int id, [FromBody] VendorCategoryUpdateDto vendorCategoryUpdateDto)
        {
            ServiceResponse<VendorCategory> serviceResponse = new ServiceResponse<VendorCategory>();

            try
            {
                if (vendorCategoryUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update VendorCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update VendorCategory object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update VendorCategory object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update Invalid VendorCategory object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var vendorCategory = await _repository.VendorCategoryRepository.GetVendorCategoryById(id);
                if (vendorCategory is null)
                {
                    _logger.LogError($"Update VendorCategory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update VendorCategory with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(vendorCategoryUpdateDto, vendorCategory);
                string result = await _repository.VendorCategoryRepository.UpdateVendorCategory(vendorCategory);
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
                serviceResponse.Message = $"Error Occured in UpdateVendorCategory API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateVendorCategory API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500,serviceResponse);
            }
        }

        // DELETE api/<VendorCategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorCategory(int id)
        {
            ServiceResponse<VendorCategory> serviceResponse = new ServiceResponse<VendorCategory>();

            try
            {
                var vendorCategory = await _repository.VendorCategoryRepository.GetVendorCategoryById(id);
                if (vendorCategory == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete VendorCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete VendorCategory with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.VendorCategoryRepository.DeleteVendorCategory(vendorCategory);
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
                serviceResponse.Message = $"Error Occured in DeleteVendorCategory API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in DeleteVendorCategory API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateVendorCategory(int id)
        {
            ServiceResponse<VendorCategory> serviceResponse = new ServiceResponse<VendorCategory>();

            try
            {
                var vendorCategory = await _repository.VendorCategoryRepository.GetVendorCategoryById(id);
                if (vendorCategory is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "VendorCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"VendorCategory with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                vendorCategory.IsActive = true;
                string result = await _repository.VendorCategoryRepository.UpdateVendorCategory(vendorCategory);
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
                serviceResponse.Message = $"Error Occured in ActivateVendorCategory API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in ActivateVendorCategory API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateVendorCategory(int id)
        {
            ServiceResponse<VendorCategory> serviceResponse = new ServiceResponse<VendorCategory>();

            try
            {
                var vendorCategory = await _repository.VendorCategoryRepository.GetVendorCategoryById(id);
                if (vendorCategory is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"VendorCategory with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                vendorCategory.IsActive = false;
                string result = await _repository.VendorCategoryRepository.UpdateVendorCategory(vendorCategory);
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
                serviceResponse.Message = $"Error Occured in DeactivateVendorCategory API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                  serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in DeactivateVendorCategory API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
