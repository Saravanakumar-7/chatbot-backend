using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UOMController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        private readonly IUOMRepository _uOMRepository;

        public UOMController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper,IUOMRepository uOMRepository)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _uOMRepository = uOMRepository;
        }

        // GET: api/<UOMController>
        [HttpGet]
        public async Task<IActionResult> GetAllUOM()
        {
            ServiceResponse<IEnumerable<UOMDto>> serviceResponse = new ServiceResponse<IEnumerable<UOMDto>>();

            try
            {
                var UOMList = await _repository.UOMRepository.GetAllUOM();
                _logger.LogInfo("Returned all UOM");
                var result = _mapper.Map<IEnumerable<UOMDto>>(UOMList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all UOM Successfully";
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
        public async Task<IActionResult> GetAllActiveUOMs()
        {
            ServiceResponse<IEnumerable<UOMDto>> serviceResponse = new ServiceResponse<IEnumerable<UOMDto>>();

            try
            {
                var UOMList = await _repository.UOMRepository.GetAllActiveUOM();
                _logger.LogInfo("Returned all UOM");
                var result = _mapper.Map<IEnumerable<UOMDto>>(UOMList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active UOM Successfully";
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
        // GET api/<UOMController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUOMById(int id)
        {
            ServiceResponse<UOMDto> serviceResponse = new ServiceResponse<UOMDto>();

            try
            {
                var UOM = await _repository.UOMRepository.GetUOMById(id);
                if (UOM == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"UOM with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"UOM with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<UOMDto>(UOM);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUOMById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<UOMController>
        [HttpPost]
        public IActionResult CreateUOM([FromBody] UOMDtoPost UomDtoPost)
        {
            ServiceResponse<UOMDto> serviceResponse = new ServiceResponse<UOMDto>();

            try
            {
                if (UomDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "UOM object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("UOM object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid UOM object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid UOM object sent from client.");
                    return BadRequest(serviceResponse);
                } 
                var UOMEntity = _mapper.Map<UOM>(UomDtoPost);
                _repository.UOMRepository.CreateUOM(UOMEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetUOMById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UOM action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<UOMController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUOM(int id, [FromBody] UOMDtoUpdate UomDtoUpdate)
        {
            ServiceResponse<UOMDto> serviceResponse = new ServiceResponse<UOMDto>();

            try
            {
                if (UomDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("UOM object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Department object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update UOM object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var UOMEntity = await _repository.UOMRepository.GetUOMById(id);
                if (UOMEntity is null)
                {
                    _logger.LogError($"UOM with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(UomDtoUpdate, UOMEntity);
                string result = await _repository.UOMRepository.UpdateUOM(UOMEntity);
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
                _logger.LogError($"Something went wrong inside UpdateUOM action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<UOMController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUOM(int id)
        {
            ServiceResponse<UOMDto> serviceResponse = new ServiceResponse<UOMDto>();

            try
            {
                var UOM = await _repository.UOMRepository.GetUOMById(id);
                if (UOM == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "c UOM object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"UOM UOM with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.UOMRepository.DeleteUOM(UOM);
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
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateUOM(int id)
        {
            ServiceResponse<UOMDto> serviceResponse = new ServiceResponse<UOMDto>();

            try
            {
                var UOM = await _repository.UOMRepository.GetUOMById(id);
                if (UOM is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "UOM object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"UOM with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                UOM.ActiveStatus = true;
                string result = await _repository.UOMRepository.UpdateUOM(UOM);
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
                _logger.LogError($"Something went wrong inside ActivateUOM action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateUOM(int id)
        {
            ServiceResponse<UOMDto> serviceResponse = new ServiceResponse<UOMDto>();

            try
            {
                var UOM = await _repository.UOMRepository.GetUOMById(id);
                if (UOM is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "UOM object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"UOM with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                UOM.ActiveStatus = false;
                string result = await _repository.UOMRepository.UpdateUOM(UOM);
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
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateUOM action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
