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
        // GET: api/<CostCenterController>
        [HttpGet]

        public async Task<IActionResult> GetAllCostCenters()
        {
            ServiceResponse<IEnumerable<CostCenterDto>> serviceResponse = new ServiceResponse<IEnumerable<CostCenterDto>>();
            try
            {

                var CostCenterList = await _repository.CostCenterRepository.GetAllCostCenters();
                _logger.LogInfo("Returned all CostCenters");
                var result = _mapper.Map<IEnumerable<CostCenterDto>>(CostCenterList);
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
        public async Task<IActionResult> GetAllActiveCostCenters()
        {
            ServiceResponse<IEnumerable<CostCenterDto>> serviceResponse = new ServiceResponse<IEnumerable<CostCenterDto>>();

            try
            {
                var CostCenters = await _repository.CostCenterRepository.GetAllActiveCostCenters();
                _logger.LogInfo("Returned all CostCenters");
                var result = _mapper.Map<IEnumerable<CostCenterDto>>(CostCenters);
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

        // GET api/<CostCenterController>/5
        [HttpGet("{id}")]
            public async Task<IActionResult> GetCostCenterById(int id)
        {
            ServiceResponse<CostCenterDto> serviceResponse = new ServiceResponse<CostCenterDto>();

            try
            {
                var CostCenter = await _repository.CostCenterRepository.GetCostCenterById(id);
                if (CostCenter == null)
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

                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<CostCenterDto>(CostCenter);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
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
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<CostCenterController>
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
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Costcenter object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Costcenter object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var costcenter = _mapper.Map<CostCenter>(costCenterDtoPost);
                _repository.CostCenterRepository.CreateCostCenter(costcenter);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfylly Created";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Created("GetCostCenterById", "Successfully Created");
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

        // PUT api/<CostCenterController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCostCenter(int id, [FromBody] CostCenterDtoUpdate costCenterDtoUpdate)
        {
            ServiceResponse<CostCenterDto> serviceResponse = new ServiceResponse<CostCenterDto>();

            try
            {
                if (costCenterDtoUpdate is null)
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
                var costcenter = await _repository.CostCenterRepository.GetCostCenterById(id);
                if (costcenter is null)
                {
                    _logger.LogError($"costcenter with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(costCenterDtoUpdate, costcenter);
                string result = await _repository.CostCenterRepository.UpdateCostCenter(costcenter);
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
                _logger.LogError($"Something went wrong inside UpdateCostcenter action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<CostCenterController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCostCenter(int id)
        {
            ServiceResponse<CostCenterDto> serviceResponse = new ServiceResponse<CostCenterDto>();

            try
            {
                var costcenter = await _repository.CostCenterRepository.GetCostCenterById(id);
                if (costcenter == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Costcenter object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Costcenter with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.CostCenterRepository.DeleteCostCenter(costcenter);
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
        public async Task<IActionResult> ActivateCostCenter(int id)
        {
            ServiceResponse<CostCenterDto> serviceResponse = new ServiceResponse<CostCenterDto>();

            try
            {
                var Costcenter = await _repository.CostCenterRepository.GetCostCenterById(id);
                if (Costcenter is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Costcenter object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Costcenter with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Costcenter.IsActive = true;
                string result = await _repository.CostCenterRepository.UpdateCostCenter(Costcenter);
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
                _logger.LogError($"Something went wrong inside ActivateCostcenter action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCostCenter(int id)
        {
            ServiceResponse<CostCenterDto> serviceResponse = new ServiceResponse<CostCenterDto>();

            try
            {
                var Costcenter = await _repository.CostCenterRepository.GetCostCenterById(id);
                if (Costcenter is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Costcenter object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"purchasegroup with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Costcenter.IsActive = false;
                string result = await _repository.CostCenterRepository.UpdateCostCenter(Costcenter);
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
                _logger.LogError($"Something went wrong inside DeactivateCostcenter action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
 