using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]

    public class AdditionalChargesController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public AdditionalChargesController(IRepositoryWrapperForMaster repository,ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<AdditionalChargesController>
        [HttpGet]
        public async Task<IActionResult> GetAllAdditionalCharges([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<AdditionalChargesDto>> serviceResponse = new ServiceResponse<IEnumerable<AdditionalChargesDto>>();

            try
            {
                var additionalChargesList = await _repository.AdditionalChargesRepository.GetAllAdditionalCharges(searchParams);
                _logger.LogInfo("Returned all AdditionalCharges");
                var result = _mapper.Map<IEnumerable<AdditionalChargesDto>>(additionalChargesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all AdditionalCharges Successfully";
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
        public async Task<IActionResult> GetAllActiveAdditionalCharges([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<AdditionalChargesDto>> serviceResponse = new ServiceResponse<IEnumerable<AdditionalChargesDto>>();

            try
            {
                var additionalChargesList = await _repository.AdditionalChargesRepository.GetAllActiveAdditionalCharges(searchParams);
                _logger.LogInfo("Returned all AdditionalCharges");
                var result = _mapper.Map<IEnumerable<AdditionalChargesDto>>(additionalChargesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active AdditionalCharges Successfully";
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
        // GET api/<AdditionalChargesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdditionalChargesById(int id)
        {
            ServiceResponse<AdditionalChargesDto> serviceResponse = new ServiceResponse<AdditionalChargesDto>();

            try
            {
                var additionalCharges = await _repository.AdditionalChargesRepository.GetAdditionalChargesById(id);
                if (additionalCharges == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"AdditionalCharges with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"AdditionalCharges with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned AdditionalCharges with id: {id}");
                    var result = _mapper.Map<AdditionalChargesDto>(additionalCharges);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned AdditionalCharges with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAdditionalChargesById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<AdditionalChargesController>
        [HttpPost]
        public IActionResult CreateAdditionalCharges([FromBody] AdditionalChargesPostDto additionalChargesPostDto)
        {
            ServiceResponse<AdditionalChargesDto> serviceResponse = new ServiceResponse<AdditionalChargesDto>();

            try
            {
                if (additionalChargesPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "AdditionalCharges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("AdditionalCharges object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid AdditionalCharges object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid AdditionalCharges object sent from client.");
                    return Ok(serviceResponse);
                }
                var additionalChargesEntity = _mapper.Map<AdditionalCharges>(additionalChargesPostDto);
                _repository.AdditionalChargesRepository.CreateAdditionalCharges(additionalChargesEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetAdditionalChargesById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateAdditionalCharges action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<UOMController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdditionalCharges(int id, [FromBody] AdditionalChargesUpdateDto additionalChargesUpdateDto)
        {
            ServiceResponse<AdditionalChargesDto> serviceResponse = new ServiceResponse<AdditionalChargesDto>();

            try
            {
                if (additionalChargesUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update AdditionalCharges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("AdditionalCharges object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update AdditionalCharges object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update AdditionalCharges object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var additionalChargesById = await _repository.AdditionalChargesRepository.GetAdditionalChargesById(id);
                if (additionalChargesById is null)
                {
                    _logger.LogError($"AdditionalCharges with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(additionalChargesUpdateDto, additionalChargesById);
                string result = await _repository.AdditionalChargesRepository.UpdateAdditionalCharges(additionalChargesById);
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
                _logger.LogError($"Something went wrong inside UpdateAdditionalCharges action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<UOMController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdditionalCharges(int id)
        {
            ServiceResponse<AdditionalChargesDto> serviceResponse = new ServiceResponse<AdditionalChargesDto>();

            try
            {
                var additionalCharges = await _repository.AdditionalChargesRepository.GetAdditionalChargesById(id);
                if (additionalCharges == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "AdditionalCharges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"AdditionalCharges  with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                string result = await _repository.AdditionalChargesRepository.DeleteAdditionalCharges(additionalCharges);
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
                serviceResponse.Message = $"Something went wrong inside AdditionalCharges action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteAdditionalCharges action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateAdditionalCharges(int id)
        {
            ServiceResponse<AdditionalChargesDto> serviceResponse = new ServiceResponse<AdditionalChargesDto>();

            try
            {
                var additionalCharges = await _repository.AdditionalChargesRepository.GetAdditionalChargesById(id);
                if (additionalCharges is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "AdditionalCharges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"AdditionalCharges with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                additionalCharges.ActiveStatus = true;
                string result = await _repository.AdditionalChargesRepository.UpdateAdditionalCharges(additionalCharges);
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
                _logger.LogError($"Something went wrong inside ActivateAdditionalCharges action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateAdditionalCharges(int id)
        {
            ServiceResponse<AdditionalChargesDto> serviceResponse = new ServiceResponse<AdditionalChargesDto>();

            try
            {
                var additionalCharges = await _repository.AdditionalChargesRepository.GetAdditionalChargesById(id);
                if (additionalCharges is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "AdditionalCharges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"AdditionalCharges with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                additionalCharges.ActiveStatus = false;
                string result = await _repository.AdditionalChargesRepository.UpdateAdditionalCharges(additionalCharges);
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
                _logger.LogError($"Something went wrong inside DeactivateAdditionalCharges action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}

