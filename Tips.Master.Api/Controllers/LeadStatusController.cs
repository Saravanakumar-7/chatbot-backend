using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LeadStatusController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public LeadStatusController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllLeadStatus([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LeadStatusDto>> serviceResponse = new ServiceResponse<IEnumerable<LeadStatusDto>>();
            try
            {

                var leadStatusList = await _repository.LeadStatusRepository.GetAllLeadStatus(pagingParameter, searchParams);

                var metadata = new
                {
                    leadStatusList.TotalCount,
                    leadStatusList.PageSize,
                    leadStatusList.CurrentPage,
                    leadStatusList.HasNext,
                    leadStatusList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all DemoStatus");
                var result = _mapper.Map<IEnumerable<LeadStatusDto>>(leadStatusList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LeadStatus Successfully";
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
        public async Task<IActionResult> GetAllActiveLeadStatus([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LeadStatusDto>> serviceResponse = new ServiceResponse<IEnumerable<LeadStatusDto>>();

            try
            {
                var leadStatus = await _repository.LeadStatusRepository.GetAllActiveLeadStatus(pagingParameter, searchParams);
                _logger.LogInfo("Returned all leadStatus");
                var result = _mapper.Map<IEnumerable<LeadStatusDto>>(leadStatus);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active leadStatus Successfully";
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
        public async Task<IActionResult> GetLeadStatusById(int id)
        {
            ServiceResponse<LeadStatusDto> serviceResponse = new ServiceResponse<LeadStatusDto>();

            try
            {
                var leadStatus = await _repository.LeadStatusRepository.GetLeadStatusById(id);
                if (leadStatus == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"leadStatus with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"leadStatus with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned leadStatus with id: {id}");
                    var result = _mapper.Map<LeadStatusDto>(leadStatus);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned leadStatus with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetLeadStatusById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateLeadStatus([FromBody] LeadStatusDtoPost leadStatusDtoPost)
        {
            ServiceResponse<LeadStatusDtoPost> serviceResponse = new ServiceResponse<LeadStatusDtoPost>();

            try
            {
                if (leadStatusDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "LeadStatus object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("LeadStatus object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid LeadStatus object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid LeadStatus object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var leadStatus = _mapper.Map<LeadStatus>(leadStatusDtoPost);
                _repository.LeadStatusRepository.CreateLeadStatus(leadStatus);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "leadStatus Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetLeadStatusById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateLeadStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeadStatus(int id, [FromBody] LeadStatusDtoUpdate LeadStatusDtoUpdate)
        {
            ServiceResponse<LeadStatusDto> serviceResponse = new ServiceResponse<LeadStatusDto>();

            try
            {
                if (LeadStatusDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update LeadStatus object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update LeadStatus object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update LeadStatus object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update LeadStatus object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var leadStatus = await _repository.LeadStatusRepository.GetLeadStatusById(id);
                if (leadStatus is null)
                {
                    _logger.LogError($"Update leadStatus with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update leadStatus with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(LeadStatusDtoUpdate, leadStatus);
                string result = await _repository.LeadStatusRepository.UpdateLeadStatus(leadStatus);
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
                _logger.LogError($"Something went wrong inside UpdateLeadStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeadStatus(int id)
        {
            ServiceResponse<LeadStatusDto> serviceResponse = new ServiceResponse<LeadStatusDto>();

            try
            {
                var leadStatus = await _repository.LeadStatusRepository.GetLeadStatusById(id);
                if (leadStatus == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete leadStatus object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete leadStatus with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.LeadStatusRepository.DeleteLeadStatus(leadStatus);
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
                _logger.LogError($"Something went wrong inside DeleteleadStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateLeadStatus(int id)
        {
            ServiceResponse<LeadStatusDto> serviceResponse = new ServiceResponse<LeadStatusDto>();

            try
            {
                var leadStatus = await _repository.LeadStatusRepository.GetLeadStatusById(id);
                if (leadStatus is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "leadStatus object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"leadStatus with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                leadStatus.IsActive = true;
                string result = await _repository.LeadStatusRepository.UpdateLeadStatus(leadStatus);
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
                _logger.LogError($"Something went wrong inside ActivatedLeadStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateLeadStatus(int id)
        {
            ServiceResponse<LeadStatusDto> serviceResponse = new ServiceResponse<LeadStatusDto>();

            try
            {
                var leadStatus = await _repository.LeadStatusRepository.GetLeadStatusById(id);
                if (leadStatus is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "leadStatus object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"leadStatus with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                leadStatus.IsActive = false;
                string result = await _repository.LeadStatusRepository.UpdateLeadStatus(leadStatus);
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
                _logger.LogError($"Something went wrong inside DeactivatedLeadStatus action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }

}
