using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SourceDetailsController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public SourceDetailsController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllSourceDetails()
        {
            ServiceResponse<IEnumerable<SourceDetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<SourceDetailsDto>>();
            try
            {

                var SourceDetailsList = await _repository.SourceDetailsRepository.GetAllSourceDetails();
                _logger.LogInfo("Returned all SourceDetails");
                var result = _mapper.Map<IEnumerable<SourceDetailsDto>>(SourceDetailsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SourceDetails Successfully";
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
        public async Task<IActionResult> GetAllActiveSourceDetails()
        {
            ServiceResponse<IEnumerable<SourceDetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<SourceDetailsDto>>();

            try
            {
                var SourceDetailsList = await _repository.SourceDetailsRepository.GetAllActiveSourceDetails();
                _logger.LogInfo("Returned all sourceDetails");
                var result = _mapper.Map<IEnumerable<SourceDetailsDto>>(SourceDetailsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active sourceDetails Successfully";
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
        public async Task<IActionResult> GetSourceDetailsById(int id)
        {
            ServiceResponse<SourceDetailsDto> serviceResponse = new ServiceResponse<SourceDetailsDto>();

            try
            {
                var sourceDetailById = await _repository.SourceDetailsRepository.GetSourceDetailsById(id);
                if (sourceDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"sourceDetail with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"sourceDetail with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned SourceDetails with id: {id}");
                    var result = _mapper.Map<SourceDetailsDto>(sourceDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned SourceDetails with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSourceDetailsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateSourceDetails([FromBody] SourceDetailsPostDto sourceDetailsPostDto)
        {
            ServiceResponse<TypeOfHomePostDto> serviceResponse = new ServiceResponse<TypeOfHomePostDto>();

            try
            {
                if (sourceDetailsPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SourceDetails object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("SourceDetails object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SourceDetails object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid SourceDetails object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var sourceDetails = _mapper.Map<SourceDetails>(sourceDetailsPostDto);
                _repository.SourceDetailsRepository.CreateSourceDetails(sourceDetails);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "sourceDetails Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetsourceDetailsById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateSourceDetails action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSourceDetails(int id, [FromBody] SourceDetailsUpdateDto SourceDetailsUpdateDto)
        {
            ServiceResponse<SourceDetailsDto> serviceResponse = new ServiceResponse<SourceDetailsDto>();

            try
            {
                if (SourceDetailsUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update sourceDetails object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update sourceDetails object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update sourceDetails object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update sourceDetails object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var sourceDetails = await _repository.SourceDetailsRepository.GetSourceDetailsById(id);
                if (sourceDetails is null)
                {
                    _logger.LogError($"Update sourceDetails with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update sourceDetails with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(SourceDetailsUpdateDto, sourceDetails);
                string result = await _repository.SourceDetailsRepository.UpdateSourceDetails(sourceDetails);
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
                _logger.LogError($"Something went wrong inside UpdateSourceDetails action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSourceDetails(int id)
        {
            ServiceResponse<SourceDetailsDto> serviceResponse = new ServiceResponse<SourceDetailsDto>();

            try
            {
                var sourceDetails = await _repository.SourceDetailsRepository.GetSourceDetailsById(id);
                if (sourceDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete sourceDetails object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete sourceDetails with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.SourceDetailsRepository.DeleteSourceDetails(sourceDetails);
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
                _logger.LogError($"Something went wrong inside DeleteSourceDetails action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateSourceDetails(int id)
        {
            ServiceResponse<SourceDetailsDto> serviceResponse = new ServiceResponse<SourceDetailsDto>();

            try
            {
                var sourceDetails = await _repository.SourceDetailsRepository.GetSourceDetailsById(id);
                if (sourceDetails is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "sourceDetails object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"sourceDetails with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                sourceDetails.IsActive = true;
                string result = await _repository.SourceDetailsRepository.UpdateSourceDetails(sourceDetails);
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
                _logger.LogError($"Something went wrong inside ActivateSourceDetails action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateSourceDetails(int id)
        {
            ServiceResponse<SourceDetailsDto> serviceResponse = new ServiceResponse<SourceDetailsDto>();

            try
            {
                var sourceDetail = await _repository.SourceDetailsRepository.GetSourceDetailsById(id);
                if (sourceDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "sourceDetail object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"sourceDetail with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                sourceDetail.IsActive = false;
                string result = await _repository.SourceDetailsRepository.UpdateSourceDetails(sourceDetail);
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
                _logger.LogError($"Something went wrong inside DeactivatedsourceDetail action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
