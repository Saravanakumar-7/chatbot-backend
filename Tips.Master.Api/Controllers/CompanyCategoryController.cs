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
    public class CompanyCategoryController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public CompanyCategoryController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<VendorCategoryController>
        [HttpGet]
        public async Task<IActionResult> GetAllCompanyCategory([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CompanyCategoryDto>> serviceResponse = new ServiceResponse<IEnumerable<CompanyCategoryDto>>();

            try
            {
                var companyCategories = await _repository.CompanyCategoryRepository.GetAllCompanyCategory(searchParams);
                _logger.LogInfo("Returned all companyCategories");
                var result = _mapper.Map<IEnumerable<CompanyCategoryDto>>(companyCategories);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all companyCategories Successfully";
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
        public async Task<IActionResult> GetAllActiveCompanyCategory([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CompanyCategoryDto>> serviceResponse = new ServiceResponse<IEnumerable<CompanyCategoryDto>>();

            try
            {
                var companyCategories = await _repository.CompanyCategoryRepository.GetAllActiveCompanyCategory(searchParams);
                _logger.LogInfo("Returned all companyCategories");
                var result = _mapper.Map<IEnumerable<CompanyCategoryDto>>(companyCategories);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active companyCategories Successfully";
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
        // GET api/<VendorCategoryController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyCategoryById(int id)
        {
            ServiceResponse<CompanyCategoryDto> serviceResponse = new ServiceResponse<CompanyCategoryDto>();

            try
            {
                var companyCategoryDetail = await _repository.CompanyCategoryRepository.GetCompanyCategoryById(id);
                if (companyCategoryDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"companyCategory with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"companyCategory with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned companyCategory with id: {id}");
                    var result = _mapper.Map<CompanyCategoryDto>(companyCategoryDetail);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned companyCategory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCompanyCategoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        // POST api/<VendorCategoryController>
        [HttpPost]
        public async Task<IActionResult> CreateCompanyCategory([FromBody] CompanyCategoryPostDto companyCategoryPostDto)
        {
            ServiceResponse<CompanyCategoryPostDto> serviceResponse = new ServiceResponse<CompanyCategoryPostDto>();

            try
            {
                if (companyCategoryPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CompanyCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("CompanyCategory object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CompanyCategory object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid CompanyCategory object sent from client.");
                    return BadRequest(serviceResponse);
                }
              

                var companyCategory = _mapper.Map<CompanyCategory>(companyCategoryPostDto);
                _repository.CompanyCategoryRepository.CreateCompanyCategory(companyCategory);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                return Created("GetCompanyCategoryById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreatecompanyCategory action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<VendorCategoryController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompanyCategory(int id, [FromBody] CompanyCategoryUpdateDto companyCategoryUpdateDto)
        {
            ServiceResponse<CompanyCategoryUpdateDto> serviceResponse = new ServiceResponse<CompanyCategoryUpdateDto>();

            try
            {
                if (companyCategoryUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update CompanyCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update CompanyCategory object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update CompanyCategory object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update Invalid CompanyCategory object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var companyCategoryById = await _repository.CompanyCategoryRepository.GetCompanyCategoryById(id);
                if (companyCategoryById is null)
                {
                    _logger.LogError($"Update CompanyCategory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update CompanyCategory with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(companyCategoryUpdateDto, companyCategoryById);
                string result = await _repository.CompanyCategoryRepository.UpdateCompanyCategory(companyCategoryById);
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
                _logger.LogError($"Something went wrong inside UpdateCompanyCategory action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<VendorCategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyCategory(int id)
        {
            ServiceResponse<CompanyCategory> serviceResponse = new ServiceResponse<CompanyCategory>();

            try
            {
                var companyCategory = await _repository.CompanyCategoryRepository.GetCompanyCategoryById(id);
                if (companyCategory == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete CompanyCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete CompanyCategory with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.CompanyCategoryRepository.DeleteCompanyCategory(companyCategory);
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
                _logger.LogError($"Something went wrong inside DeleteCompanyCategory action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCompanyCategory(int id)
        {
            ServiceResponse<CompanyCategory> serviceResponse = new ServiceResponse<CompanyCategory>();

            try
            {
                var companyCategory = await _repository.CompanyCategoryRepository.GetCompanyCategoryById(id);
                if (companyCategory is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "companyCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"companyCategory with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                companyCategory.IsActive = true;
                string result = await _repository.CompanyCategoryRepository.UpdateCompanyCategory(companyCategory);
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
                _logger.LogError($"Something went wrong inside ActivateCompanyCategory action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCompanyCategory(int id)
        {
            ServiceResponse<CompanyCategory> serviceResponse = new ServiceResponse<CompanyCategory>();

            try
            {
                var companyCategory = await _repository.CompanyCategoryRepository.GetCompanyCategoryById(id);
                if (companyCategory is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "companyCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"companyCategory with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                companyCategory.IsActive = false;
                string result = await _repository.CompanyCategoryRepository.UpdateCompanyCategory(companyCategory);
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
                _logger.LogError($"Something went wrong inside DeactivateCompanyCategory action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}