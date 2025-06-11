using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public DepartmentController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }

        // GET: api/<DepartmentController>
        [HttpGet]
        public async Task<IActionResult> GetAllDepartment([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<DepartmentDto>> serviceResponse = new ServiceResponse<IEnumerable<DepartmentDto>>();

            try
            {
                var departments = await _repository.DepartmentRepository.GetAllDepartment(searchParams);

                _logger.LogInfo("Returned all Department");
                var result = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Departments Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllDepartment API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllDepartment API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveDepartments([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<DepartmentDto>> serviceResponse = new ServiceResponse<IEnumerable<DepartmentDto>>();

            try
            {
                var departments = await _repository.DepartmentRepository.GetAllActiveDepartment(searchParams);
                _logger.LogInfo("Returned all departments");
                var result = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active departments Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveDepartments API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveDepartments API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            ServiceResponse<DepartmentDto> serviceResponse = new ServiceResponse<DepartmentDto>();

            try
            {
                var department = await _repository.DepartmentRepository.GetDepartmentById(id);
                if (department == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Department with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Department with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Department with id: {id}");
                    var result = _mapper.Map<DepartmentDto>(department);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Department with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDepartmentById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDepartmentById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DepartmentController>
        [HttpPost]
        public IActionResult CreateDepartment([FromBody] DepartmentPostDto department)
        {
            ServiceResponse<DepartmentDto> serviceResponse = new ServiceResponse<DepartmentDto>();

            try
            {
                if (department is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Department object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Department object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Department object sent from client.");
                    return BadRequest(serviceResponse);
                } 
                var departments = _mapper.Map<Department>(department);
                _repository.DepartmentRepository.CreateDepartment(departments);
                _repository.SaveAsync(); 
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetDepartmentById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateDepartment API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateDepartment API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DepartmentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentUpdateDto department)
        {
            ServiceResponse<DepartmentDto> serviceResponse = new ServiceResponse<DepartmentDto>();

            try
            {
                if (department is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Department object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Department object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Department object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var departments = await _repository.DepartmentRepository.GetDepartmentById(id);
                if (departments is null)
                {
                    _logger.LogError($"Update DeliveryTerm with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update DeliveryTerm with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(department, departments);
                string result = await _repository.DepartmentRepository.UpdateDepartment(departments);
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
                serviceResponse.Message = $"Error Occured in UpdateDepartment API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateDepartment API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DepartmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            ServiceResponse<DepartmentDto> serviceResponse = new ServiceResponse<DepartmentDto>();

            try
            {
                var department = await _repository.DepartmentRepository.GetDepartmentById(id);
                if (department == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete Department with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.DepartmentRepository.DeleteDepartment(department);
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
                serviceResponse.Message = $"Error Occured in DeleteDepartment API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteDepartment API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateDepartment(int id)
        {
            ServiceResponse<DepartmentDto> serviceResponse = new ServiceResponse<DepartmentDto>();

            try
            {
                var department = await _repository.DepartmentRepository.GetDepartmentById(id);
                if (department is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Department with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                department.IsActive = true;
                string result = await _repository.DepartmentRepository.UpdateDepartment(department);
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
                serviceResponse.Message = $"Error Occured in ActivateDepartment API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in ActivateDepartment API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateDepartment(int id)
        {
            ServiceResponse<DepartmentDto> serviceResponse = new ServiceResponse<DepartmentDto>();

            try
            {
                var department = await _repository.DepartmentRepository.GetDepartmentById(id);
                if (department is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    _logger.LogError($"Department  with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                department.IsActive = false;
                string result = await _repository.DepartmentRepository.UpdateDepartment(department);
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
                serviceResponse.Message = $"Error Occured in DeactivateDepartment API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeactivateDepartment API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }



    }
}
