using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CostingMethodController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        //
        private ILoggerManager _logger;
        private IMapper _mapper;
        public CostingMethodController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
 
        [HttpGet]
        public async Task<IActionResult> GetAllCostingMethods([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CostingMethodDto>> serviceResponse = new ServiceResponse<IEnumerable<CostingMethodDto>>();
            try
            {

                var GetallCostingMethod = await _repository.CostingMethodRepository.GetAllCostingMethods(searchParams);

                _logger.LogInfo("Returned all CostingMethods");
                var result = _mapper.Map<IEnumerable<CostingMethodDto>>(GetallCostingMethod);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CostingMethods Successfully";
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
        public async Task<IActionResult> GetAllActiveCostingMethods([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CostingMethodDto>> serviceResponse = new ServiceResponse<IEnumerable<CostingMethodDto>>();

            try
            {
                var AllActiveCostingMethods = await _repository.CostingMethodRepository.GetAllActiveCostingMethods(searchParams);
                _logger.LogInfo("Returned all CostingMethods");
                var result = _mapper.Map<IEnumerable<CostingMethodDto>>(AllActiveCostingMethods);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CostingMethods Successfully";
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

        [HttpGet("{id}")]
       public async Task<IActionResult> GetCostingMethodById(int id)
        {
            ServiceResponse<CostingMethodDto> serviceResponse = new ServiceResponse<CostingMethodDto>();

            try
            {
                var CostingMethodsbyId = await _repository.CostingMethodRepository.GetCostingMethodById(id);
                if (CostingMethodsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CostingMethod with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CostingMethods with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<CostingMethodDto>(CostingMethodsbyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCostingMethodById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public IActionResult CreateCostingMethod([FromBody] CostingMethodDtoPost costingMethodDtoPost)
        {
            ServiceResponse<CostingMethodDto> serviceResponse = new ServiceResponse<CostingMethodDto>();

            try
            {
                if (costingMethodDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CostingMethod object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("CostingMethod object sent from client is null.");
                    
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CostingMethod object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid CostingMethod object sent from client.");

                    return BadRequest(serviceResponse);
                }
                var CostingMethodsCreate = _mapper.Map<CostingMethod>(costingMethodDtoPost);
                _repository.CostingMethodRepository.CreateCostingMethod(CostingMethodsCreate);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;               
                return Created("GetCostingMethodById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CostingMethod action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCostingMethod(int id, [FromBody] CostingMethodDtoUpdate costingMethodDtoUpdate)
        {
            ServiceResponse<CostingMethodDto> serviceResponse = new ServiceResponse<CostingMethodDto>();

            try
            {
                if (costingMethodDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update CostingMethod object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update CostingMethod object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update CostingMethod object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update CostingMethod object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var CostingMethodsUpdate = await _repository.CostingMethodRepository.GetCostingMethodById(id);
                if (CostingMethodsUpdate is null)
                {
                    _logger.LogError($"Update CostingMethod with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update CostingMethod with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(costingMethodDtoUpdate, CostingMethodsUpdate);
                string result = await _repository.CostingMethodRepository.UpdateCostingMethod(CostingMethodsUpdate);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateCostingMethod action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCostingMethod(int id)
        {
            ServiceResponse<CostingMethodDto> serviceResponse = new ServiceResponse<CostingMethodDto>();

            try
            {
                var DeleteCostingmethod = await _repository.CostingMethodRepository.GetCostingMethodById(id);
                if (DeleteCostingmethod == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete CostingMethod object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete CostingMethod with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.CostingMethodRepository.DeleteCostingMethod(DeleteCostingmethod);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeleteCostingMethod action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCostingMethod(int id)
        {
            ServiceResponse<CostingMethodDto> serviceResponse = new ServiceResponse<CostingMethodDto>();

            try
            {
                var CostingMethodActivate = await _repository.CostingMethodRepository.GetCostingMethodById(id);
                if (CostingMethodActivate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CostingMethod object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"CostingMethod with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                CostingMethodActivate.IsActive = true;
                string result = await _repository.CostingMethodRepository.UpdateCostingMethod(CostingMethodActivate);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateCostingMethod action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCostingMethod(int id)
        {
            ServiceResponse<CostingMethodDto> serviceResponse = new ServiceResponse<CostingMethodDto>();

            try
            {
                var CostingMethodDeactivate = await _repository.CostingMethodRepository.GetCostingMethodById(id);
                if (CostingMethodDeactivate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CostingMethod object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"CostingMethod with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                CostingMethodDeactivate.IsActive = false;
                string result = await _repository.CostingMethodRepository.UpdateCostingMethod(CostingMethodDeactivate);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateCostingMethod action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }

        }
    }
}
