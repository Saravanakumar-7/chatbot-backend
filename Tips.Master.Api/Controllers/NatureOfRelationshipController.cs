using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NatureOfRelationshipController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public NatureOfRelationshipController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<NatureOfRelationshipController>
        [HttpGet]
        public async Task<IActionResult> GetAllNatureOfRelationships([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<NatureOfRelationshipDto>> serviceResponse = new ServiceResponse<IEnumerable<NatureOfRelationshipDto>>();
            try
            {

                var natureOfRelationshiplist = await _repository.NatureOfRelationshipRepository.GetAllNatureOfRelationships(pagingParameter, searchParams);

                var metadata = new
                {
                    natureOfRelationshiplist.TotalCount,
                    natureOfRelationshiplist.PageSize,
                    natureOfRelationshiplist.CurrentPage,
                    natureOfRelationshiplist.HasNext,
                    natureOfRelationshiplist.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all NatureOfRelationships");
                var result = _mapper.Map<IEnumerable<NatureOfRelationshipDto>>(natureOfRelationshiplist);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all NatureOfRelationships Successfully";
                serviceResponse.Success = true;
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
        public async Task<IActionResult> GetAllActiveNatureOfRelationships([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<NatureOfRelationshipDto>> serviceResponse = new ServiceResponse<IEnumerable<NatureOfRelationshipDto>>();

            try
            {
                var NatureOfRelationships = await _repository.NatureOfRelationshipRepository.GetAllActiveNatureOfRelationships(pagingParameter, searchParams);
                _logger.LogInfo("Returned all NatureOfRelationships");
                var result = _mapper.Map<IEnumerable<NatureOfRelationshipDto>>(NatureOfRelationships);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all NatureOfRelationships Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }

        // GET api/<NatureOfRelationshipController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNatureOfRelationshipById(int id)
        {
            ServiceResponse<NatureOfRelationshipDto> serviceResponse = new ServiceResponse<NatureOfRelationshipDto>();

            try
            {
                var NatureOfRelationship = await _repository.NatureOfRelationshipRepository.GetNatureOfRelationshipById(id);
                if (NatureOfRelationship == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"NatureOfRelationship with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"NatureOfRelationship with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<NatureOfRelationshipDto>(NatureOfRelationship);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetNatureOfRelationshipById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<NatureOfRelationshipController>
        [HttpPost]
        public IActionResult CreateNatureOfRelationship([FromBody] NatureOfRelationshipDtoPost natureofRelationshipDtoPost)
        {
            ServiceResponse<NatureOfRelationshipDto> serviceResponse = new ServiceResponse<NatureOfRelationshipDto>();

            try
            {
                if (natureofRelationshipDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "natureofRelationship object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("natureofRelationship object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid natureofRelationship object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseGroup object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var NatureofRelationship = _mapper.Map<NatureOfRelationship>(natureofRelationshipDtoPost);
                _repository.NatureOfRelationshipRepository.CreateNatureOfRelationship(NatureofRelationship);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetNatureOfRelationshipById", serviceResponse);
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

        // PUT api/<NatureOfRelationshipController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNatureOfRelationship(int id, [FromBody] NatureOfRelationshipDtoUpdate natureOfRelationshipDtoUpdate)
        {
            ServiceResponse<NatureOfRelationshipDto> serviceResponse = new ServiceResponse<NatureOfRelationshipDto>();

            try
            {
                if (natureOfRelationshipDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update NatureOfRelationship object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update NatureOfRelationship object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update NatureOfRelationship object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update NatureOfRelationship object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var NatureOfRelationship = await _repository.NatureOfRelationshipRepository.GetNatureOfRelationshipById(id);
                if (NatureOfRelationship is null)
                {
                    _logger.LogError($"Update NatureOfRelationship with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update PurchaseGroup with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(natureOfRelationshipDtoUpdate, NatureOfRelationship);
                string result = await _repository.NatureOfRelationshipRepository.UpdateNatureOfRelationship(NatureOfRelationship);
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
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateNatureOfRelationship action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<NatureOfRelationshipController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNatureOfRelationship(int id)
        {
            ServiceResponse<NatureOfRelationshipDto> serviceResponse = new ServiceResponse<NatureOfRelationshipDto>();

            try
            {
                var NatureOfRelationship = await _repository.NatureOfRelationshipRepository.GetNatureOfRelationshipById(id);
                if (NatureOfRelationship == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete NatureOfRelationship object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete NatureOfRelationship with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.NatureOfRelationshipRepository.DeleteNatureOfRelationship(NatureOfRelationship);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateNatureOfRelationship(int id)
        {
            ServiceResponse<NatureOfRelationshipDto> serviceResponse = new ServiceResponse<NatureOfRelationshipDto>();

            try
            {
                var NatureOfRelationship = await _repository.NatureOfRelationshipRepository.GetNatureOfRelationshipById(id);
                if (NatureOfRelationship is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "NatureOfRelationship object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"NatureOfRelationship with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                NatureOfRelationship.IsActive = true;
                string result = await _repository.NatureOfRelationshipRepository.UpdateNatureOfRelationship(NatureOfRelationship);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateNatureOfRelationship action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeActivateNatureOfRelationship(int id)
        {
            ServiceResponse<NatureOfRelationshipDto> serviceResponse = new ServiceResponse<NatureOfRelationshipDto>();

            try
            {
                var NatureOfRelationship = await _repository.NatureOfRelationshipRepository.GetNatureOfRelationshipById(id);
                if (NatureOfRelationship is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "NatureOfRelationship object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"NatureOfRelationship with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                NatureOfRelationship.IsActive = false;
                string result = await _repository.NatureOfRelationshipRepository.UpdateNatureOfRelationship(NatureOfRelationship);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateNatureOfRelationship action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}