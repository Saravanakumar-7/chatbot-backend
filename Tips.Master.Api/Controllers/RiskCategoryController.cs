using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RiskCategoryController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public RiskCategoryController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<RiskCategoryController>
        [HttpGet]
        public async Task<IActionResult> GetAllRiskCategory([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<RiskCategoryDto>> serviceResponse = new ServiceResponse<IEnumerable<RiskCategoryDto>>();
            try
            {
                var riskCategoryList = await _repository.RiskCategoryRepository.GetAllRiskCategory(searchParams);
                _logger.LogInfo("Returned all RiskCategory");
                var result = _mapper.Map<IEnumerable<RiskCategoryDto>>(riskCategoryList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all RiskCategory";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<RiskCategoryController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRiskCategoryById(int id)
        {
            ServiceResponse<RiskCategoryDto> serviceResponse = new ServiceResponse<RiskCategoryDto>();
            try
            {
                var riskCategory = await _repository.RiskCategoryRepository.GetRiskCategoryById(id);
                if (riskCategory == null)
                {
                    _logger.LogError($"RiskCategory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RiskCategory hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<RiskCategoryDto>(riskCategory);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "RiskCategory Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRiskCategoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<RiskCategoryController>
        [HttpPost]
        public IActionResult CreateRiskCategory([FromBody] RiskCategoryDtoPost riskCategoryDtoPost)
        {
            ServiceResponse<RiskCategoryDtoPost> serviceResponse = new ServiceResponse<RiskCategoryDtoPost>();
            try
            {
                if (riskCategoryDtoPost is null)
                {
                    _logger.LogError("RiskCategory object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RiskCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid RiskCategory object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid RiskCategory object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                var riskCategoryEntity = _mapper.Map<RiskCategory>(riskCategoryDtoPost);
                _repository.RiskCategoryRepository.CreateRiskCategory(riskCategoryEntity);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "RiskCategory Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<RiskCategoryController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRiskCategory(int id, [FromBody] RiskCategoryDtoUpdate riskCategoryDtoUpdate)
        {
            ServiceResponse<RiskCategoryDtoUpdate> serviceResponse = new ServiceResponse<RiskCategoryDtoUpdate>();
            try
            {
                if (riskCategoryDtoUpdate is null)
                {
                    _logger.LogError("RiskCategory object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RiskCategory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid RiskCategory object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid RiskCategory object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                var riskCategoryEntity = await _repository.RiskCategoryRepository.GetRiskCategoryById(id);
                if (riskCategoryEntity is null)
                {
                    _logger.LogError($"RiskCategory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RiskCategory hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                _mapper.Map(riskCategoryDtoUpdate, riskCategoryEntity);
                string result = await _repository.RiskCategoryRepository.UpdateRiskCategory(riskCategoryEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RiskCategory Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRiskCategory action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }
    

        // DELETE api/<RiskCategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRiskCategory(int id)
        {
        ServiceResponse<RiskCategoryDto> serviceResponse = new ServiceResponse<RiskCategoryDto>();
        try
            {
                var riskCategory = await _repository.RiskCategoryRepository.GetRiskCategoryById(id);
                if (riskCategory == null)
                {
                    _logger.LogError($"RiskCategory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RiskCategory hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                string result = await _repository.RiskCategoryRepository.DeleteRiskCategory(riskCategory);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RiskCategory Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateRiskCategory(int id)
        {
            ServiceResponse<RiskCategoryDto> serviceResponse = new ServiceResponse<RiskCategoryDto>();
            try
            {
                var riskCategory = await _repository.RiskCategoryRepository.GetRiskCategoryById(id);
                if (riskCategory is null)
                {
                    _logger.LogError($"RiskCategory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RiskCategory hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                riskCategory.ActiveStatus = true;
                string result = await _repository.RiskCategoryRepository.UpdateRiskCategory(riskCategory);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned ActivateRiskCategory";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateRiskCategory action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateRiskCategory(int id)
        {
            ServiceResponse<RiskCategoryDto> serviceResponse = new ServiceResponse<RiskCategoryDto>();
            try
            {
                var riskCategory = await _repository.RiskCategoryRepository.GetRiskCategoryById(id);
                if (riskCategory is null)
                {
                    _logger.LogError($"RiskCategory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RiskCategory hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                riskCategory.ActiveStatus = false;
                string result = await _repository.RiskCategoryRepository.UpdateRiskCategory(riskCategory);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned DeactivateRiskCategory";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateRiskCategory action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
