using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
//using MySqlX.XDevAPI.Common;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TypeOfCompanyController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public TypeOfCompanyController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<TypeOfCompanyController>
        [HttpGet]
        public async Task<IActionResult> GetAllTypeOfCompanies()
        {
            ServiceResponse<IEnumerable<TypeOfCompanyDto>> serviceResponse = new ServiceResponse<IEnumerable<TypeOfCompanyDto>>();
            try
            {

                var TypeOfCompanyList = await _repository.TypeOfCompanyRepository.GetAllTypeOfCompanies();
                _logger.LogInfo("Returned all TypeofCompanies");
                var result = _mapper.Map<IEnumerable<TypeOfCompanyDto>>(TypeOfCompanyList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all TypeofCompanies Successfully";
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
        public async Task<IActionResult> GetAllActiveTypeofCompanies()
        {
            ServiceResponse<IEnumerable<TypeOfCompanyDto>> serviceResponse = new ServiceResponse<IEnumerable<TypeOfCompanyDto>>();

            try
            {
                var TypeOfCompanyList = await _repository.TypeOfCompanyRepository.GetAllActiveTypeofCompanies();
                _logger.LogInfo("Returned all TypeofCompanies");
                var result = _mapper.Map<IEnumerable<TypeOfCompanyDto>>(TypeOfCompanyList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active TypeofCompanies Successfully";
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
        // GET api/<TypeOfCompanyController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeOfCompanyById(int id)
        {
            ServiceResponse<TypeOfCompanyDto> serviceResponse = new ServiceResponse<TypeOfCompanyDto>();

            try
            {
                var typeofcompany = await _repository.TypeOfCompanyRepository.GetTypeOfCompanyById(id);
                if (typeofcompany == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"typeofcompany with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"typeofcompany with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned typeofcompany with id: {id}");
                    var result = _mapper.Map<TypeOfCompanyDto>(typeofcompany);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned typeofcompany with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetTypeOfCompanyById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }

        // POST api/<TypeOfCompanyController>
        [HttpPost]
        public IActionResult CreateTypeOfCompany([FromBody] TypeOfCompanyDtoPost typeOfCompanyDtoPost)
        {
            ServiceResponse<TypeOfCompanyDto> serviceResponse = new ServiceResponse<TypeOfCompanyDto>();

            try
            {
                if (typeOfCompanyDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "typeofcompany object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("typeofcompany object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid typeOfCompany object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Bank object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var TypeofCompany = _mapper.Map<TypeOfCompany>(typeOfCompanyDtoPost);
                _repository.TypeOfCompanyRepository.CreateTypeOfCompany(TypeofCompany);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfylly Created";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetTypeOfCompanyById", serviceResponse);

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

        // PUT api/<TypeOfCompanyController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTypeofCompany(int id, [FromBody] TypeOfCompanyDtoUpdate typeOfCompanyDtoUpdate)
        {
            ServiceResponse<TypeOfCompanyDto> serviceResponse = new ServiceResponse<TypeOfCompanyDto>();

            try
            {
                if (typeOfCompanyDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update typeofcompany object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update typeofcompany object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update typeofcompany object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid update typeofcompany object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var TypeofCompany = await _repository.TypeOfCompanyRepository.GetTypeOfCompanyById(id);
                if (TypeofCompany is null)
                {
                    _logger.LogError($"Update TypeofCompany with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update TypeofCompany with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(typeOfCompanyDtoUpdate, TypeofCompany);
                string result = await _repository.TypeOfCompanyRepository.UpdateTypeOfCompany(TypeofCompany);
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
                _logger.LogError($"Something went wrong inside UpdateTypeofCompany action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<TypeOfCompanyController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeofCompany(int id)
        {
            ServiceResponse<TypeOfCompanyDto> serviceResponse = new ServiceResponse<TypeOfCompanyDto>();

            try
            {
                var TypeofCompany = await _repository.TypeOfCompanyRepository.GetTypeOfCompanyById(id);
                if (TypeofCompany == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete TypeofCompany object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"delete TypeofCompany with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.TypeOfCompanyRepository.DeleteTypeOfCompany(TypeofCompany);
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
        public async Task<IActionResult> ActivateTypeOfCompany(int id)
        {
            ServiceResponse<TypeOfCompanyDto> serviceResponse = new ServiceResponse<TypeOfCompanyDto>();

            try
            {
                var TypeofCompany = await _repository.TypeOfCompanyRepository.GetTypeOfCompanyById(id);
                if (TypeofCompany is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = " TypeofCompany object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"TypeofCompany with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                TypeofCompany.IsActive = true;
                string result = await _repository.TypeOfCompanyRepository.UpdateTypeOfCompany(TypeofCompany);
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
                _logger.LogError($"Something went wrong inside ActivatedTypeofCompany action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateTypeOfCompany(int id)
        {
            ServiceResponse<TypeOfCompanyDto> serviceResponse = new ServiceResponse<TypeOfCompanyDto>();

            try
            {
                var TypeofCompany = await _repository.TypeOfCompanyRepository.GetTypeOfCompanyById(id);
                if (TypeofCompany is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "TypeofCompany object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"TypeofCompany with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                TypeofCompany.IsActive = false;
                string result = await _repository.TypeOfCompanyRepository.UpdateTypeOfCompany(TypeofCompany);
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
                _logger.LogError($"Something went wrong inside DeactivatedTypeofCompany action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
