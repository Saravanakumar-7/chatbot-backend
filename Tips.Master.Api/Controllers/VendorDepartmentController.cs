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
    public class VendorDepartmentController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public VendorDepartmentController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<VendorDepartmentController>
        [HttpGet]
        public async Task<IActionResult> GetAllVendorDepartment([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<VendorDepartmentDto>> serviceResponse = new ServiceResponse<IEnumerable<VendorDepartmentDto>>();

            try
            {
                var vendorDepartments = await _repository.VendorDepartmentRepository.GetAllVendorDepartment(searchParams);
                _logger.LogInfo("Returned all Vendor Department");
                var result = _mapper.Map<IEnumerable<VendorDepartmentDto>>(vendorDepartments);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Vendor Department Successfully";
                serviceResponse.Success = true;
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
        public async Task<IActionResult> GetAllActiveVendorDepartments([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<VendorDepartmentDto>> serviceResponse = new ServiceResponse<IEnumerable<VendorDepartmentDto>>();

            try
            {
                var vendorDepartments = await _repository.VendorDepartmentRepository.GetAllActiveVendorDepartment(searchParams);
                _logger.LogInfo("Returned all Vendor Department");
                var result = _mapper.Map<IEnumerable<VendorDepartmentDto>>(vendorDepartments);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Vendor Departments Successfully";
                serviceResponse.Success = true;
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
        // GET api/<VendorDepartmentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorDepartmentById(int id)
        {
            ServiceResponse<VendorDepartmentDto> serviceResponse = new ServiceResponse<VendorDepartmentDto>();

            try
            {
                var vendorDepartment = await _repository.VendorDepartmentRepository.GetVendorDepartmentById(id);
                if (vendorDepartment == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Vendor Department with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Vendor Department with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<VendorDepartmentDto>(vendorDepartment);
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
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<VendorDepartmentController>
        [HttpPost]
        public IActionResult CreateVendorDepartment([FromBody] VendorDepartmentPostDto vendorDepartmentPostDto)
        {
            ServiceResponse<VendorDepartmentDto> serviceResponse = new ServiceResponse<VendorDepartmentDto>();

            try
            {
                if (vendorDepartmentPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "VendorDepartment object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("VendorDepartment object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid VendorDepartment object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid VendorDepartment object sent from client.");
                    return BadRequest(serviceResponse);
                }
                //var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                //var id = _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                //_repository.SaveAsync();

                var vendorDepartment = _mapper.Map<VendorDepartment>(vendorDepartmentPostDto);
                _repository.VendorDepartmentRepository.CreateVendorDepartment(vendorDepartment);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetVendorDepartmentById", serviceResponse);
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

        // PUT api/<VendorDepartmentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorDepartment(int id, [FromBody] VendorDepartmentUpdateDto vendorDepartmentUpdateDto)
        {
            ServiceResponse<VendorDepartmentDto> serviceResponse = new ServiceResponse<VendorDepartmentDto>();

            try
            {
                if (vendorDepartmentUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update VendorDepartment object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update VendorDepartment object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update VendorDepartment object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Invalid VendorDepartment object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var vendorDepartment = await _repository.VendorDepartmentRepository.GetVendorDepartmentById(id);
                if (vendorDepartment is null)
                {
                    _logger.LogError($"update VendorDepartment with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update VendorDepartment with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(vendorDepartmentUpdateDto, vendorDepartment);
                string result = await _repository.VendorDepartmentRepository.UpdateVendorDepartment(vendorDepartment);
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
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateVendorDepartment action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<VendorDepartmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorDepartment(int id)
        {
            ServiceResponse<VendorDepartmentDto> serviceResponse = new ServiceResponse<VendorDepartmentDto>();

            try
            {
                var vendorDepartment = await _repository.VendorDepartmentRepository.GetVendorDepartmentById(id);
                if (vendorDepartment == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "c VendorDepartment object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"VendorDepartment VendorDepartment with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.VendorDepartmentRepository.DeleteVendorDepartment(vendorDepartment);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateVendorDepartment(int id)
        {
            ServiceResponse<VendorDepartmentDto> serviceResponse = new ServiceResponse<VendorDepartmentDto>();

            try
            {
                var vendorDepartment = await _repository.VendorDepartmentRepository.GetVendorDepartmentById(id);
                if (vendorDepartment is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "VendorDepartment object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"VendorDepartment with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                vendorDepartment.IsActive = true;
                string result = await _repository.VendorDepartmentRepository.UpdateVendorDepartment(vendorDepartment);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside ActivateVendorDepartment action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateVendorDepartment(int id)
        {
            ServiceResponse<VendorDepartmentDto> serviceResponse = new ServiceResponse<VendorDepartmentDto>();

            try
            {
                var vendorDepartment = await _repository.VendorDepartmentRepository.GetVendorDepartmentById(id);
                if (vendorDepartment is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "VendorDepartment object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"VendorDepartment with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                vendorDepartment.IsActive = false;
                string result = await _repository.VendorDepartmentRepository.UpdateVendorDepartment(vendorDepartment);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeactivateVendorDepartment action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


    }
}
