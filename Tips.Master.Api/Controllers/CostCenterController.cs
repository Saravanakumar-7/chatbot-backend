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
    public class CostCenterController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public CostCenterController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

         [HttpGet]
        public async Task<IActionResult> GetAllCostCenters([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CostCenterDto>> serviceResponse = new ServiceResponse<IEnumerable<CostCenterDto>>();
            try
            {

                var GetallCostCenters = await _repository.CostCenterRepository.GetAllCostCenters(searchParams);

                _logger.LogInfo("Returned all CostCenters");
                var result = _mapper.Map<IEnumerable<CostCenterDto>>(GetallCostCenters);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CostCenters Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllActiveCostCenters([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CostCenterDto>> serviceResponse = new ServiceResponse<IEnumerable<CostCenterDto>>();

            try
            {
                var AllActiveCostCenters = await _repository.CostCenterRepository.GetAllActiveCostCenters(searchParams);
                _logger.LogInfo("Returned all CostCenters");
                var result = _mapper.Map<IEnumerable<CostCenterDto>>(AllActiveCostCenters);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active CostCenters Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode=HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }

         [HttpGet("{id}")]
            public async Task<IActionResult> GetCostCenterById(int id)
        {
            ServiceResponse<CostCenterDto> serviceResponse = new ServiceResponse<CostCenterDto>();

            try
            {
                var CostCenterbyId = await _repository.CostCenterRepository.GetCostCenterById(id);
                if (CostCenterbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CostCenter with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CostCenter with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned CostCenter with id: {id}");
                    var result = _mapper.Map<CostCenterDto>(CostCenterbyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CostCenter with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCostCenterById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpPost]
        public IActionResult CreateCostCenter([FromBody] CostCenterDtoPost costCenterDtoPost)
        {
            ServiceResponse<CostCenterDto> serviceResponse = new ServiceResponse<CostCenterDto>();

            try
            {
                if (costCenterDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Costcenter object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Costcenter object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Costcenter object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Costcenter object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var CreateCostcenter = _mapper.Map<CostCenter>(costCenterDtoPost);
                _repository.CostCenterRepository.CreateCostCenter(CreateCostcenter);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetCostCenterById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside Costcenter action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCostCenter(int id, [FromBody] CostCenterDtoUpdate costCenterDtoUpdate)
        {
            ServiceResponse<CostCenterDto> serviceResponse = new ServiceResponse<CostCenterDto>();

            try
            {
                if (costCenterDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Costcenter object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update Costcenter object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Costcenter object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Costcenter object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var UpdateCostcenter = await _repository.CostCenterRepository.GetCostCenterById(id);
                if (UpdateCostcenter is null)
                {
                    _logger.LogError($"Update costcenter with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update costcenter with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(costCenterDtoUpdate, UpdateCostcenter);
                string result = await _repository.CostCenterRepository.UpdateCostCenter(UpdateCostcenter);
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
                _logger.LogError($"Something went wrong inside UpdateCostcenter action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCostCenter(int id)
        {
            ServiceResponse<CostCenterDto> serviceResponse = new ServiceResponse<CostCenterDto>();

            try
            {
                var DeleteCostcenter = await _repository.CostCenterRepository.GetCostCenterById(id);
                if (DeleteCostcenter == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Costcenter object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Delete Costcenter with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                string result = await _repository.CostCenterRepository.DeleteCostCenter(DeleteCostcenter);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteCostcenter action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCostCenter(int id)
        {
            ServiceResponse<CostCenterDto> serviceResponse = new ServiceResponse<CostCenterDto>();

            try
            {
                var CostcenterActivate = await _repository.CostCenterRepository.GetCostCenterById(id);
                if (CostcenterActivate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Costcenter object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Costcenter with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                CostcenterActivate.IsActive = true;
                string result = await _repository.CostCenterRepository.UpdateCostCenter(CostcenterActivate);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside ActivateCostcenter action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCostCenter(int id)
        {
            ServiceResponse<CostCenterDto> serviceResponse = new ServiceResponse<CostCenterDto>();

            try
            {
                var CostcenterDeactivate = await _repository.CostCenterRepository.GetCostCenterById(id);
                if (CostcenterDeactivate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Costcenter object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"purchasegroup with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                CostcenterDeactivate.IsActive = false;
                string result = await _repository.CostCenterRepository.UpdateCostCenter(CostcenterDeactivate);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeactivateCostcenter action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }

        }
    }
}
 