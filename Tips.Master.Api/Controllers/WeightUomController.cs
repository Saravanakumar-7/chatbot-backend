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
    public class WeightUomController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public WeightUomController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<WeightUomController>
        [HttpGet]
        public async Task<IActionResult> GetAllWeightUom()
        {
            ServiceResponse<IEnumerable<WeightUomDto>> serviceResponse = new ServiceResponse<IEnumerable<WeightUomDto>>();

            try
            {
                var WeightUom = await _repository.WeightUomRepository.GetAllWeightUom();
                _logger.LogInfo("Returned all WeightUom");
                var result = _mapper.Map<IEnumerable<WeightUomDto>>(WeightUom);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all WeightUom Successfully";
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
        public async Task<IActionResult> GetAllActiveWeightUoms()
        {
            ServiceResponse<IEnumerable<WeightUomDto>> serviceResponse = new ServiceResponse<IEnumerable<WeightUomDto>>();

            try
            {
                var WeightUom = await _repository.WeightUomRepository.GetAllActiveWeightUom();
                _logger.LogInfo("Returned all WeightUom");
                var result = _mapper.Map<IEnumerable<WeightUomDto>>(WeightUom);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active WeightUom Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }

        // GET api/<WeightUomController>/5
        [HttpGet("{id}")] 
        public async Task<IActionResult> GetWeightUomById(int id)
        {
            ServiceResponse<WeightUomDto> serviceResponse = new ServiceResponse<WeightUomDto>();

            try
            {
                var weightUom = await _repository.WeightUomRepository.GetWeightUomById(id);
                if (weightUom == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"weightUom with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"weightUom with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned weightUom with id: {id}");
                    var result = _mapper.Map<WeightUomDto>(weightUom);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned weightUom with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetweightUomById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<WeightUomController>
        [HttpPost]
        public IActionResult CreateWeightUom([FromBody] WeightUomPostDto weightUom)
        {
            ServiceResponse<WeightUomDto> serviceResponse = new ServiceResponse<WeightUomDto>();

            try
            {
                if (weightUom is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "weightUom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("weightUom object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid weightUom object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid weightUom object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var weightUoms = _mapper.Map<WeightUom>(weightUom);
                _repository.WeightUomRepository.CreateWeightUom(weightUoms);
                _repository.SaveAsync();
                serviceResponse.Message = "Successfylly Created";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetWeightUomById", serviceResponse);
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

        // PUT api/<WeightUomController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeightUom(int id, [FromBody] WeightUomUpdateDto uomDto)
        {
            ServiceResponse<WeightUomDto> serviceResponse = new ServiceResponse<WeightUomDto>();

            try
            {
                if (uomDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update WeightUom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update WeightUom object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update weightUom object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update WeightUom object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var weightUom = await _repository.WeightUomRepository.GetWeightUomById(id);
                if (weightUom is null)
                {
                    _logger.LogError($"Update weightUom with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update weightUom with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(uomDto, weightUom);
                string result = await _repository.WeightUomRepository.UpdateWeightUom(weightUom);
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
                _logger.LogError($"Something went wrong inside UpdateWeight action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<WeightUomController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeightUom(int id)
        {
            ServiceResponse<WeightUomDto> serviceResponse = new ServiceResponse<WeightUomDto>();

            try
            {
                var weightUom = await _repository.WeightUomRepository.GetWeightUomById(id);
                if (weightUom == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete weightUom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete weightUom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.WeightUomRepository.DeleteWeightUom(weightUom);
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
                return StatusCode(500,serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateWeightUom(int id)
        {
            ServiceResponse<WeightUomDto> serviceResponse = new ServiceResponse<WeightUomDto>();

            try
            {
                var weightUom = await _repository.WeightUomRepository.GetWeightUomById(id);
                if (weightUom is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "weightUom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"weightUom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                weightUom.IsActive = true;
                string result = await _repository.WeightUomRepository.UpdateWeightUom(weightUom);
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
                _logger.LogError($"Something went wrong inside ActivateweightUom action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateWeightUom(int id)
        {
            ServiceResponse<WeightUomDto> serviceResponse = new ServiceResponse<WeightUomDto>();

            try
            {
                var weightUom = await _repository.WeightUomRepository.GetWeightUomById(id);
                if (weightUom is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "weightUom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"weightUom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                weightUom.IsActive = false;
                string result = await _repository.WeightUomRepository.UpdateWeightUom(weightUom);
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
                _logger.LogError($"Something went wrong inside DeactivateweightUom action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


    }
}
