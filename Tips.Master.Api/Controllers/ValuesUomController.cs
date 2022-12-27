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
    public class ValuesUomController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public ValuesUomController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<ValuesUomController>
        [HttpGet]
        public async Task<IActionResult> GetAllVolumeUom()
        {
            ServiceResponse<IEnumerable<VolumeUomDto>> serviceResponse = new ServiceResponse<IEnumerable<VolumeUomDto>>();

            try
            {
                var volumeUomList = await _repository.VolumeUomRepo.GetAllVolumeUom();
                _logger.LogInfo("Returned all VolumeUom");
                var result = _mapper.Map<IEnumerable<VolumeUomDto>>(volumeUomList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all VolumeUom Successfully";
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
        public async Task<IActionResult> GetAllActiveVolumeUoms()
        {
            ServiceResponse<IEnumerable<VolumeUomDto>> serviceResponse = new ServiceResponse<IEnumerable<VolumeUomDto>>();

            try
            {
                var volumeUomList = await _repository.VolumeUomRepo.GetAllActiveVolumeUom();
                _logger.LogInfo("Returned all VolumeUom");
                var result = _mapper.Map<IEnumerable<VolumeUomDto>>(volumeUomList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active VolumeUom Successfully";
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
         // GET api/<ValuesUomController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVolumeUomById(int id)
        {
            ServiceResponse<VolumeUomDto> serviceResponse = new ServiceResponse<VolumeUomDto>();

            try
            {
                var volumeUom = await _repository.VolumeUomRepo.GetVolumeUomById(id);
                if (volumeUom == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"volumeUom with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"volumeUom with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<VolumeUomDto>(volumeUom);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetvolumeUomById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<ValuesUomController>
        [HttpPost]
        public IActionResult CreateVolumeUoms([FromBody] VolumeUomPostDto volumeUom)
        {
            ServiceResponse<VolumeUomDto> serviceResponse = new ServiceResponse<VolumeUomDto>();

            try
            {
                if (volumeUom is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "volumeUom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("volumeUom object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid volumeUom object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid volumeUom object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var volumeUoms = _mapper.Map<VolumeUom>(volumeUom);
                _repository.VolumeUomRepo.CreateVolumeUom(volumeUoms);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetvolumeUomById", serviceResponse);
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

        // PUT api/<ValuesUomController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVolumeUom(int id, [FromBody] VolumeUomUpdateDto uomUpdateDto)
        {
            ServiceResponse<VolumeUomDto> serviceResponse = new ServiceResponse<VolumeUomDto>();

            try
            {
                if (uomUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update VolumeUom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update VolumeUom object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update VolumeUom object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Invalid VolumeUom object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var volumeUom = await _repository.VolumeUomRepo.GetVolumeUomById(id);
                if (volumeUom is null)
                {
                    _logger.LogError($"update volumeUom with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update volumeUom with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(uomUpdateDto, volumeUom);
                string result = await _repository.VolumeUomRepo.UpdateVolumeUom(volumeUom);
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
                _logger.LogError($"Something went wrong inside UpdateVolumeUom action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<ValuesUomController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVolumeUom(int id)
        {
            ServiceResponse<VolumeUomDto> serviceResponse = new ServiceResponse<VolumeUomDto>();

            try
            {
                var volumeUom = await _repository.VolumeUomRepo.GetVolumeUomById(id);
                if (volumeUom == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete VolumeUom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete VolumeUom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.VolumeUomRepo.DeleteVolumeUom(volumeUom);
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
        public async Task<IActionResult> ActivateVolumeUom(int id)
        {
            ServiceResponse<VolumeUomDto> serviceResponse = new ServiceResponse<VolumeUomDto>();

            try
            {
                var volumeUom = await _repository.VolumeUomRepo.GetVolumeUomById(id);
                if (volumeUom is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "volumeUom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"volumeUom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                volumeUom.IsActive = true;
                string result = await _repository.VolumeUomRepo.UpdateVolumeUom(volumeUom);
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
                _logger.LogError($"Something went wrong inside ActivateVolumeUom action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateVolumeUom(int id)
        {
            ServiceResponse<VolumeUomDto> serviceResponse = new ServiceResponse<VolumeUomDto>();

            try
            {
                var volumeUom = await _repository.VolumeUomRepo.GetVolumeUomById(id);
                if (volumeUom is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "volumeUom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"volumeUom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                volumeUom.IsActive = false;
                string result = await _repository.VolumeUomRepo.UpdateVolumeUom(volumeUom);
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
                _logger.LogError($"Something went wrong inside DeactivatevolumeUom action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


    }
}
