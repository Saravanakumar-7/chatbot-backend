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
    public class CostingMethodController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public CostingMethodController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
 
        [HttpGet]
         public async Task<IActionResult> GetAllCostingMethods()
        {
            ServiceResponse<IEnumerable<CostingMethodDto>> serviceResponse = new ServiceResponse<IEnumerable<CostingMethodDto>>();
            try
            {

                var CostingMethodList = await _repository.CostingMethodRepository.GetAllCostingMethods();
                _logger.LogInfo("Returned all CostingMethods");
                var result = _mapper.Map<IEnumerable<CostingMethodDto>>(CostingMethodList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveCostingMethods()
        {
            ServiceResponse<IEnumerable<CostingMethodDto>> serviceResponse = new ServiceResponse<IEnumerable<CostingMethodDto>>();

            try
            {
                var CostingMethods = await _repository.CostingMethodRepository.GetAllActiveCostingMethods();
                _logger.LogInfo("Returned all CostingMethods");
                var result = _mapper.Map<IEnumerable<CostingMethodDto>>(CostingMethods);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                return StatusCode(500, "Internal server error");

            }
        }

        [HttpGet("{id}")]
       public async Task<IActionResult> GetCostingMethodById(int id)
        {
            ServiceResponse<CostingMethodDto> serviceResponse = new ServiceResponse<CostingMethodDto>();

            try
            {
                var CostingMethods = await _repository.CostingMethodRepository.GetCostingMethodById(id);
                if (CostingMethods == null)
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
                    var result = _mapper.Map<CostingMethodDto>(CostingMethods);
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
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
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
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CostingMethod object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid CostingMethod object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var CostingMethods = _mapper.Map<CostingMethod>(costingMethodDtoPost);
                _repository.CostingMethodRepository.CreateCostingMethod(CostingMethods);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfylly Created";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;


                return Created("GetCostingMethodById", "Successfully Created");
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
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
                var CostingMethods = await _repository.CostingMethodRepository.GetCostingMethodById(id);
                if (CostingMethods is null)
                {
                    _logger.LogError($"CostingMethod with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(costingMethodDtoUpdate, CostingMethods);
                string result = await _repository.CostingMethodRepository.UpdateCostingMethod(CostingMethods);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside UpdateCostingMethod action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

         [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCostingMethod(int id)
        {
            ServiceResponse<CostingMethodDto> serviceResponse = new ServiceResponse<CostingMethodDto>();

            try
            {
                var CostingMethod = await _repository.CostingMethodRepository.GetCostingMethodById(id);
                if (CostingMethod == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CostingMethod object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"CostingMethod with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.CostingMethodRepository.DeleteCostingMethod(CostingMethod);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();

            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCostingMethod(int id)
        {
            ServiceResponse<CostingMethodDto> serviceResponse = new ServiceResponse<CostingMethodDto>();

            try
            {
                var CostingMethod = await _repository.CostingMethodRepository.GetCostingMethodById(id);
                if (CostingMethod is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CostingMethod object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"CostingMethod with id: {id}, hasn't been found in db.");
                    return BadRequest("CostingMethod object is null");
                }
                CostingMethod.IsActive = true;
                string result = await _repository.CostingMethodRepository.UpdateCostingMethod(CostingMethod);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateCostingMethod action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCostingMethod(int id)
        {
            ServiceResponse<CostingMethodDto> serviceResponse = new ServiceResponse<CostingMethodDto>();

            try
            {
                var CostingMethod = await _repository.CostingMethodRepository.GetCostingMethodById(id);
                if (CostingMethod is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CostingMethod object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"CostingMethod with id: {id}, hasn't been found in db.");
                    return BadRequest("CostingMethod object is null");
                }
                CostingMethod.IsActive = false;
                string result = await _repository.CostingMethodRepository.UpdateCostingMethod(CostingMethod);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deactivated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateCostingMethod action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
