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
    public class LeadTypeController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public LeadTypeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllLeadTypes([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LeadTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<LeadTypeDto>>();
            try
            {

                var leadTypeList = await _repository.LeadTypeRepository.GetAllLeadTypes(pagingParameter, searchParams);


                var metadata = new
                {
                    leadTypeList.TotalCount,
                    leadTypeList.PageSize,
                    leadTypeList.CurrentPage,
                    leadTypeList.HasNext,
                    leadTypeList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all LeadTypes");
                var result = _mapper.Map<IEnumerable<LeadTypeDto>>(leadTypeList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LeadTypes Successfully";
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

        public async Task<IActionResult> GetAllActiveLeadTypes([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LeadTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<LeadTypeDto>>();

            try
            {
                var leadType = await _repository.LeadTypeRepository.GetAllActiveLeadTypes(pagingParameter, searchParams);
                _logger.LogInfo("Returned all leadStatus");
                var result = _mapper.Map<IEnumerable<LeadTypeDto>>(leadType);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active leadType Successfully";
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
        public async Task<IActionResult> GetLeadTypeById(int id)
        {
            ServiceResponse<LeadTypeDto> serviceResponse = new ServiceResponse<LeadTypeDto>();

            try
            {
                var leadType = await _repository.LeadTypeRepository.GetLeadTypeById(id);
                if (leadType == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"leadType with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"leadType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned leadStatus with id: {id}");
                    var result = _mapper.Map<LeadTypeDto>(leadType);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned leadType with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetLeadTypeById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateLeadType([FromBody] LeadTypeDtoPost leadTypeDtoPost)
        {
            ServiceResponse<LeadTypeDtoPost> serviceResponse = new ServiceResponse<LeadTypeDtoPost>();

            try
            {
                if (leadTypeDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "LeadType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("LeadType object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid LeadType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid LeadType object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var leadType = _mapper.Map<LeadType>(leadTypeDtoPost);
                _repository.LeadTypeRepository.CreateLeadType(leadType);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "leadType Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetLeadTypeById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateLeadType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeadType(int id, [FromBody] LeadTypeDtoUpdate leadTypeDtoUpdate)
        {
            ServiceResponse<LeadTypeDto> serviceResponse = new ServiceResponse<LeadTypeDto>();

            try
            {
                if (leadTypeDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update LeadType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update LeadType object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update LeadType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update LeadType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var leadType = await _repository.LeadTypeRepository.GetLeadTypeById(id);
                if (leadType is null)
                {
                    _logger.LogError($"Update leadType with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update leadType with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(leadTypeDtoUpdate, leadType);
                string result = await _repository.LeadTypeRepository.UpdateLeadType(leadType);
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
        public async Task<IActionResult> DeleteLeadType(int id)
        {
            ServiceResponse<LeadTypeDto> serviceResponse = new ServiceResponse<LeadTypeDto>();

            try
            {
                var leadType = await _repository.LeadTypeRepository.GetLeadTypeById(id);
                if (leadType == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete leadType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete leadType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.LeadTypeRepository.DeleteLeadType(leadType);
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
                _logger.LogError($"Something went wrong inside DeleteleadType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateLeadType(int id)
        {
            ServiceResponse<LeadTypeDto> serviceResponse = new ServiceResponse<LeadTypeDto>();

            try
            {
                var leadType = await _repository.LeadTypeRepository.GetLeadTypeById(id);
                if (leadType is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "leadType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"leadType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                leadType.IsActive = true;
                string result = await _repository.LeadTypeRepository.UpdateLeadType(leadType);
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
                _logger.LogError($"Something went wrong inside ActivatedleadType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateLeadType(int id)
        {
            ServiceResponse<LeadTypeDto> serviceResponse = new ServiceResponse<LeadTypeDto>();

            try
            {
                var leadType = await _repository.LeadTypeRepository.GetLeadTypeById(id);
                if (leadType is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "leadType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"leadType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                leadType.IsActive = false;
                string result = await _repository.LeadTypeRepository.UpdateLeadType(leadType);
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
                _logger.LogError($"Something went wrong inside DeactivatedleadType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
