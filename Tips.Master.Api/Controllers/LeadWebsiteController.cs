using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LeadWebsiteController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public LeadWebsiteController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllLeadWebsite([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LeadWebsiteDto>> serviceResponse = new ServiceResponse<IEnumerable<LeadWebsiteDto>>();
            try
            {

                var leadWebsites = await _repository.leadWebsiteRepository.GetAllLeadWebsite(pagingParameter, searchParams);

                var metadata = new
                {
                    leadWebsites.TotalCount,
                    leadWebsites.PageSize,
                    leadWebsites.CurrentPage,
                    leadWebsites.HasNext,
                    leadWebsites.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all BHK");
                var result = _mapper.Map<IEnumerable<LeadWebsiteDto>>(leadWebsites);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all leadWebsites  Successfully";
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
        public async Task<IActionResult> GetAllActiveLeadWebsite([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LeadWebsiteDto>> serviceResponse = new ServiceResponse<IEnumerable<LeadWebsiteDto>>();

            try
            {
                var leadWebsites = await _repository.leadWebsiteRepository.GetAllActiveLeadWebsite(pagingParameter, searchParams);
                _logger.LogInfo("Returned all leadWebsites");
                var result = _mapper.Map<IEnumerable<LeadWebsiteDto>>(leadWebsites);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active leadWebsites Successfully";
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
        public async Task<IActionResult> GetLeadWebsiteById(int id)
        {
            ServiceResponse<LeadWebsiteDto> serviceResponse = new ServiceResponse<LeadWebsiteDto>();

            try
            {
                var leadWebsiteDetail = await _repository.leadWebsiteRepository.GetLeadWebsiteById(id);
                if (leadWebsiteDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"LeadWebsite with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"LeadWebsite with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned LeadWebsite with id: {id}");
                    var result = _mapper.Map<LeadWebsiteDto>(leadWebsiteDetail);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned LeadWebsite with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetLeadWebsiteById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateLeadWebsite([FromBody] LeadWebsitePostDto leadWebsitePostDto)
        {
            ServiceResponse<LeadWebsitePostDto> serviceResponse = new ServiceResponse<LeadWebsitePostDto>();

            try
            {
                if (leadWebsitePostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "leadwebsite object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("leadwebsite object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Leadwebsite object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Leadwebsite object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var architect = _mapper.Map<LeadWebsite>(leadWebsitePostDto);
                _repository.leadWebsiteRepository.CreateLeadWebsite(architect);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Leadwebsite Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetLeadwebsiteById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateArcitecture action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeadWebsite(int id, [FromBody] LeadWebsiteUpdateDto leadWebsiteUpdateDto)
        {
            ServiceResponse<LeadWebsiteUpdateDto> serviceResponse = new ServiceResponse<LeadWebsiteUpdateDto>();

            try
            {
                if (leadWebsiteUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update LeadWebsite object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update LeadWebsite object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update LeadWebsite object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update LeadWebsite object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var leadWebsiteDetail = await _repository.leadWebsiteRepository.GetLeadWebsiteById(id);
                if (leadWebsiteDetail is null)
                {
                    _logger.LogError($"Update LeadWebsite with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update LeadWebsite with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(leadWebsiteUpdateDto, leadWebsiteDetail);
                string result = await _repository.leadWebsiteRepository.UpdateLeadWebsite(leadWebsiteDetail);
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
                _logger.LogError($"Something went wrong inside UpdateArchitecture action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeadWebsite(int id)
        {
            ServiceResponse<LeadWebsiteDto> serviceResponse = new ServiceResponse<LeadWebsiteDto>();

            try
            {
                var leadWebsiteDetail = await _repository.leadWebsiteRepository.GetLeadWebsiteById(id);
                if (leadWebsiteDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete LeadWebsite object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete LeadWebsite with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.leadWebsiteRepository.DeleteLeadWebsite(leadWebsiteDetail);
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
                _logger.LogError($"Something went wrong inside DeleteLeadWebsite action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateLeadWebsite(int id)
        {
            ServiceResponse<LeadWebsiteDto> serviceResponse = new ServiceResponse<LeadWebsiteDto>();

            try
            {
                var leadWebsiteDetail = await _repository.leadWebsiteRepository.GetLeadWebsiteById(id);
                if (leadWebsiteDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "LeadWebsite object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"LeadWebsite with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                leadWebsiteDetail.IsActive = true;
                string result = await _repository.leadWebsiteRepository.UpdateLeadWebsite(leadWebsiteDetail);
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
                _logger.LogError($"Something went wrong inside ActivateLeadWebsite action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeActivateLeadWebsite(int id)
        {
            ServiceResponse<LeadWebsiteDto> serviceResponse = new ServiceResponse<LeadWebsiteDto>();

            try
            {
                var leadWebsiteDetail = await _repository.leadWebsiteRepository.GetLeadWebsiteById(id);
                if (leadWebsiteDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "LeadWebsite object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"LeadWebsite with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                leadWebsiteDetail.IsActive = true;
                string result = await _repository.leadWebsiteRepository.UpdateLeadWebsite(leadWebsiteDetail);
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
                _logger.LogError($"Something went wrong inside DeActivateLeadWebsite action: {ex.Message}");
                return StatusCode(500, serviceResponse);


            }
        }
    }
}
