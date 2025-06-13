using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LeadTimeController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public LeadTimeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<LeadTimeController>
        [HttpGet]
        public async Task<IActionResult> GetAllLeadTimes([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LeadTimeDto>> serviceResponse = new ServiceResponse<IEnumerable<LeadTimeDto>>();

            try
            {
                var LeadTimeList = await _repository.leadTimeRepository.GetAllLeadTime(searchParams);

                _logger.LogInfo("Returned all LeadTimes");
                var result = _mapper.Map<IEnumerable<LeadTimeDto>>(LeadTimeList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LeadTimes Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLeadTimes API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLeadTimes API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveLeadTimes([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LeadTimeDto>> serviceResponse = new ServiceResponse<IEnumerable<LeadTimeDto>>();

            try
            {
                var LeadTimes = await _repository.leadTimeRepository.GetAllActiveLeadTime(searchParams);
                _logger.LogInfo("Returned all LeadTimes");
                var result = _mapper.Map<IEnumerable<LeadTimeDto>>(LeadTimes);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active LeadTimes Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveLeadTimes API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveLeadTimes API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }
        // GET api/<LeadTimeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeadTimeById(int id)
        {
            ServiceResponse<LeadTimeDto> serviceResponse = new ServiceResponse<LeadTimeDto>();

            try
            {
                var leadTime = await _repository.leadTimeRepository.GetLeadTimeById(id);
                if (leadTime == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Department with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"LeadTime with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Department with id: {id}");
                    var result = _mapper.Map<LeadTimeDto>(leadTime);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Department with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetLeadTimeById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetLeadTimeById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<LeadTimeController>
        [HttpPost]
        public IActionResult CreateLeadTime([FromBody] LeadTimeDtoPost leadTimeDtoPost)
        {
            ServiceResponse<LeadTimeDto> serviceResponse = new ServiceResponse<LeadTimeDto>();

            try
            {
                if (leadTimeDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "LeadTime object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("LeadTime object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid LeadTime object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid LeadTime object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var leadTimeEntity = _mapper.Map<LeadTime>(leadTimeDtoPost);
                _repository.leadTimeRepository.CreateLeadTime(leadTimeEntity);
                _repository.SaveAsync();
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetLeadTimeById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateLeadTime API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateLeadTime API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<LeadTimeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeadTime(int id, [FromBody] LeadTimeDtoUpdate leadTimeDtoUpdate)
        {
            ServiceResponse<LeadTimeDto> serviceResponse = new ServiceResponse<LeadTimeDto>();

            try
            {
                if (leadTimeDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update LeadTime object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update LeadTime object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Department object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update LeadTime object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var leadTimeEntity = await _repository.leadTimeRepository.GetLeadTimeById(id);
                if (leadTimeEntity is null)
                {
                    _logger.LogError($"Update LeadTime with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update LeadTime with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(leadTimeDtoUpdate, leadTimeEntity);
                string result = await _repository.leadTimeRepository.UpdateLeadTime(leadTimeEntity);
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
                serviceResponse.Message = $"Error Occured in UpdateLeadTime API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateLeadTime API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<LeadTimeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeadTime(int id)
        {
            ServiceResponse<LeadTimeDto> serviceResponse = new ServiceResponse<LeadTimeDto>();

            try
            {
                var leadTime = await _repository.leadTimeRepository.GetLeadTimeById(id);
                if (leadTime == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete LeadTime object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete LeadTime with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.leadTimeRepository.DeleteLeadTime(leadTime);
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
                serviceResponse.Message = $"Error Occured in DeleteLeadTime API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteLeadTime API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateLeadTime(int id)
        {
            ServiceResponse<LeadTimeDto> serviceResponse = new ServiceResponse<LeadTimeDto>();

            try
            {
                var leadTime = await _repository.leadTimeRepository.GetLeadTimeById(id);
                if (leadTime is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "LeadTime object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"LeadTime with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                leadTime.IsActive = true;
                string result = await _repository.leadTimeRepository.UpdateLeadTime(leadTime);
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
                serviceResponse.Message = $"Error Occured in ActivateLeadTime API for the following id : {id} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in ActivateLeadTime API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500,serviceResponse);
            }
        }
        

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateLeadTime(int id)
        {
            ServiceResponse<LeadTimeDto> serviceResponse = new ServiceResponse<LeadTimeDto>();

            try
            {
                var leadTime = await _repository.leadTimeRepository.GetLeadTimeById(id);
                if (leadTime is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "LeadTime object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"LeadTime with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                leadTime.IsActive = false;
                string result = await _repository.leadTimeRepository.UpdateLeadTime(leadTime);
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
                serviceResponse.Message = $"Error Occured in DeactivateLeadTime API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeactivateLeadTime API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
