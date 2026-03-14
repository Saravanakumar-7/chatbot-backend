using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeeDetailsController : ControllerBase
    {
        private readonly IRepositoryWrapperForMaster _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeeDetailsController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> GetAllEmployeeDetails([FromQuery] SearchParames searchParames)
        {
            ServiceResponse<IEnumerable<EmployeeDetailsDto>> response = new ServiceResponse<IEnumerable<EmployeeDetailsDto>>();

            try
            {
                var employeeDetails = await _repository.EmployeeDetailsRepository.GetAllEmployeeDetails(searchParames);
                var result = _mapper.Map<IEnumerable<EmployeeDetailsDto>>(employeeDetails);
                _logger.LogInfo($"Employee details returned successfully");
                response.Data = result;
                response.Message = "Employee details returned successfully";
                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                return Ok(response);

            }

            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetAllEmployeeDetails API \n {ex.Message} \n {ex.InnerException}");
                response.Data = null;
                response.Message = $"Error occured in GetAllEmployeeDetails API \n {ex.Message}";
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeDetailsById(int id)
        {
            ServiceResponse<EmployeeDetailsDto> response = new ServiceResponse<EmployeeDetailsDto>();

            try
            {
                var employee = await _repository.EmployeeDetailsRepository.GetEmployeeDetailsById(id);

                if (employee == null)
                {
                    _logger.LogError($"Employee with {id} hasn't been found in db");
                    response.Data = null;
                    response.Message = $"Employee with {id} hasn't been found in db";
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(response);
                }

                else
                {

                    _logger.LogInfo($"Returned employeedetails with id: {id}");
                    var result = _mapper.Map<EmployeeDetailsDto>(employee);
                    response.Data = result;
                    response.Message = "Returned employeedetails with id Successfully";
                    response.Success = true;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetEmployeeDetailsById API \n {ex.Message} \n {ex.InnerException}");
                response.Data = null;
                response.Message = $"Error occured in GetEmployeeDetailsById API \n {ex.Message}";
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, response);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeDetails([FromBody] EmployeeDetailsPostDto employeeDetailsPostDto)
        {
            ServiceResponse<EmployeeDetailsDto> response = new ServiceResponse<EmployeeDetailsDto>();
            try
            {
                if (employeeDetailsPostDto is null)
                {
                    _logger.LogError("employeedetails object sent from client is null.");
                    response.Data = null;
                    response.Message = "employeedetails object sent from client is null";
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(response);
                }
                if (!ModelState.IsValid)
                {
                    response.Data = null;
                    response.Message = "Invalid employeedetails object sent from client";
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid employeedetails object sent from client.");
                    return BadRequest(response);
                }
                var employee = _mapper.Map<EmployeeDetails>(employeeDetailsPostDto);
                _repository.EmployeeDetailsRepository.CreateEmployeeDetails(employee);
                _repository.SaveAsync();
                response.Message = "Successfully Created";
                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                return Created("Employeedetails", response);
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = "Error Occured in CreateEmployee API : \n {ex.Message}";
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreatEmployee API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeDetails(int id, [FromBody] EmployeeDetailsUpdateDto employeeDetailsUpdateDto)
        {
            ServiceResponse<EmployeeDetailsDto> response = new ServiceResponse<EmployeeDetailsDto>();
            try
            {
                if (employeeDetailsUpdateDto is null)
                {
                    _logger.LogError("employeedetails object sent from client is null.");
                    response.Data = null;
                    response.Message = "employeedetails object sent from client is null";
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(response);
                }
                if (!ModelState.IsValid)
                {
                    response.Data = null;
                    response.Message = "Invalid employeedetails object sent from client";
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid employeedetails object sent from client.");
                    return BadRequest(response);
                }
                var employeeDetailsEntity = await _repository.EmployeeDetailsRepository.GetEmployeeDetailsById(employeeDetailsUpdateDto.Id);
                if (employeeDetailsEntity == null)
                {
                    _logger.LogError($"Employee details with id: {employeeDetailsUpdateDto.Id} hasn't been found in db");
                    response.Data = null;
                    response.Message = $"Employee details with id: {employeeDetailsUpdateDto.Id} hasn't been found in db";
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(response);
                }
                _mapper.Map(employeeDetailsUpdateDto, employeeDetailsEntity);
                _repository.EmployeeDetailsRepository.UpdateEmployeeDetails(employeeDetailsEntity);
                _repository.SaveAsync();
                response.Message = $"Employee details with id: {employeeDetailsUpdateDto.Id} has been updated successfully!";
                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = $"Error Occured in UpdateEmployeeDetails API : \n {ex.Message}";
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateEmployeeDetails API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, response);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeDetails(int id)
        {
            ServiceResponse<EmployeeDetailsDto> response = new ServiceResponse<EmployeeDetailsDto>();
            try
            {
                var employeeDetailsEntity = await _repository.EmployeeDetailsRepository.GetEmployeeDetailsById(id);
                if (employeeDetailsEntity == null)
                {
                    _logger.LogError($"Employee details with id: {id} hasn't been found in db");
                    response.Data = null;
                    response.Message = $"Employee details with id: {id} hasn't been found in db";
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(response);
                }
                string result = await _repository.EmployeeDetailsRepository.DeleteEmployeeDetails(employeeDetailsEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                response.Message = $"Employee details with id: {id} has been deleted successfully!";
                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.Message = $"Error Occured in DeleteEmployeeDetails API : \n {ex.Message}";
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteEmployeeDetails API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, response);
            }
        }
    }
}
