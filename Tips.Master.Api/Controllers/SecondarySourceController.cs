using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SecondarySourceController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public SecondarySourceController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllSecondarySources()
        {
            ServiceResponse<IEnumerable<SecondarySourceDto>> serviceResponse = new ServiceResponse<IEnumerable<SecondarySourceDto>>();
            try
            {

                var secondarySources = await _repository.secondarySourceRepository.GetAllSecondarySources();
                _logger.LogInfo("Returned all LeadTypes");
                var result = _mapper.Map<IEnumerable<SecondarySourceDto>>(secondarySources);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all secondarySources Successfully";
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
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]

        public async Task<IActionResult> GetAllActiveSecondarySources()
        {
            ServiceResponse<IEnumerable<SecondarySourceDto>> serviceResponse = new ServiceResponse<IEnumerable<SecondarySourceDto>>();

            try
            {
                var secondarySources = await _repository.secondarySourceRepository.GetAllActiveSecondarySources();
                _logger.LogInfo("Returned all leadStatus");
                var result = _mapper.Map<IEnumerable<SecondarySourceDto>>(secondarySources);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active secondarySources Successfully";
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
        // GET api/<DemoStatusController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSecondarySourceById(int id)
        {
            ServiceResponse<SecondarySourceDto> serviceResponse = new ServiceResponse<SecondarySourceDto>();

            try
            {
                var secondarySources = await _repository.secondarySourceRepository.GetSecondarySourceById(id);
                if (secondarySources == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"secondarySources with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"secondarySources with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned leadStatus with id: {id}");
                    var result = _mapper.Map<SecondarySourceDto>(secondarySources);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned secondarySources with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSecondarySourceById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateSecondarySource([FromBody] SecondarySourceDtoPost secondarySourceDtoPost)
        {
            ServiceResponse<SecondarySourceDtoPost> serviceResponse = new ServiceResponse<SecondarySourceDtoPost>();

            try
            {
                if (secondarySourceDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "secondarySource object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("secondarySource object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid secondarySource object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid secondarySource object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var secondarySource = _mapper.Map<SecondarySource>(secondarySourceDtoPost);
                _repository.secondarySourceRepository.CreateSecondarySource(secondarySource);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "secondarySource Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetsecondarySourceById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateSecondarySource action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSecondarySource(int id, [FromBody] SecondarySourceDtoUpdate secondarySourceDtoUpdate)
        {
            ServiceResponse<SecondarySourceDtoUpdate> serviceResponse = new ServiceResponse<SecondarySourceDtoUpdate>();

            try
            {
                if (secondarySourceDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update SecondarySource object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update SecondarySource object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update SecondarySource object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update SecondarySource object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var secondarySources = await _repository.secondarySourceRepository.GetSecondarySourceById(id);
                if (secondarySources is null)
                {
                    _logger.LogError($"Update SecondarySource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update SecondarySource with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(secondarySourceDtoUpdate, secondarySources);
                string result = await _repository.secondarySourceRepository.UpdateSecondarySource(secondarySources);
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
                _logger.LogError($"Something went wrong inside UpdateLeadType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSecondarySource(int id)
        {
            ServiceResponse<SecondarySourceDto> serviceResponse = new ServiceResponse<SecondarySourceDto>();

            try
            {
                var secondarySource = await _repository.secondarySourceRepository.GetSecondarySourceById(id);
                if (secondarySource == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete secondarySource object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete secondarySource with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.secondarySourceRepository.DeleteSecondarySource(secondarySource);
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
                _logger.LogError($"Something went wrong inside DeleteSecondarySource action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateSecondarySource(int id)
        {
            ServiceResponse<SecondarySourceDto> serviceResponse = new ServiceResponse<SecondarySourceDto>();

            try
            {
                var Secondarysource = await _repository.secondarySourceRepository.GetSecondarySourceById(id);
                if (Secondarysource is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Secondarysource object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Secondarysource with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Secondarysource.IsActive = true;
                string result = await _repository.secondarySourceRepository.UpdateSecondarySource(Secondarysource);
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
                _logger.LogError($"Something went wrong inside ActivateSecondarySource action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateSecondarySource(int id)
        {
            ServiceResponse<SecondarySourceDto> serviceResponse = new ServiceResponse<SecondarySourceDto>();

            try
            {
                var secondarySource = await _repository.secondarySourceRepository.GetSecondarySourceById(id);
                if (secondarySource is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "secondarySource object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"secondarySource with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                secondarySource.IsActive = false;
                string result = await _repository.secondarySourceRepository.UpdateSecondarySource(secondarySource);
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
                _logger.LogError($"Something went wrong inside DeactivateSecondarySource action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
