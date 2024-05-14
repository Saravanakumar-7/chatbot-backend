using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class IncoTermController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public IncoTermController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
 
        [HttpGet]
        public async Task<IActionResult> GetAllIncoTerms([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<IncoTermDto>> serviceResponse = new ServiceResponse<IEnumerable<IncoTermDto>>();

            try
            {
                var incoTerms = await _repository.IncoTermRepository.GetAllIncoTerm(searchParams);

                _logger.LogInfo("Returned all Inco Term");
                var result = _mapper.Map<IEnumerable<IncoTermDto>>(incoTerms);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Inco Terms Successfully";
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
        public async Task<IActionResult> GetAllActiveIncoTerms([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<IncoTermDto>> serviceResponse = new ServiceResponse<IEnumerable<IncoTermDto>>();

            try
            {
                var incoTerms = await _repository.IncoTermRepository.GetAllActiveIncoTerm(searchParams);
                _logger.LogInfo("Returned all IncoTerms");
                var result = _mapper.Map<IEnumerable<IncoTermDto>>(incoTerms);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active IncoTerms Successfully";
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



        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncoTermById(int id)
        {
            ServiceResponse<IncoTermDto> serviceResponse = new ServiceResponse<IncoTermDto>();

            try
            {
                var incoTerm = await _repository.IncoTermRepository.GetIncoTermById(id);
                if (incoTerm == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IncoTerm with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IncoTerm with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned IncoTerm with id: {id}");
                    var result = _mapper.Map<IncoTermDto>(incoTerm);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned IncoTerm with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside GetIncoTermsById action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

 
        [HttpPost]
        public IActionResult CreateIncoTerm([FromBody] IncoTermPostDto incoTerm)
        {
            ServiceResponse<IncoTermDto> serviceResponse = new ServiceResponse<IncoTermDto>();

            try
            {
                if (incoTerm is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IncoTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("IncoTerm object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid IncoTerm object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid IncoTerm object sent from client.");
                    return BadRequest(serviceResponse);
                } 

                var incoTerms = _mapper.Map<IncoTerm>(incoTerm);
                _repository.IncoTermRepository.CreateIncoTerm(incoTerms);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfylly Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetIncoTermById", serviceResponse);
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

 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncoTerm(int id, [FromBody] IncoTermUpdateDto incoTerm)
        {
            ServiceResponse<IncoTermDto> serviceResponse = new ServiceResponse<IncoTermDto>();

            try
            {
                if (incoTerm is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update IncoTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update IncoTerm object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update IncoTerm object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid v IncoTerm object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var incoterms = await _repository.IncoTermRepository.GetIncoTermById(id);
                if (incoterms is null)
                {
                    _logger.LogError($"Update IncoTerm with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update IncoTerm with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(incoTerm, incoterms);
                string result = await _repository.IncoTermRepository.UpdateIncoTerm(incoterms);
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
                _logger.LogError($"Something went wrong inside UpdateIncoTerm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncoTerm(int id)
        {
            ServiceResponse<IncoTermDto> serviceResponse = new ServiceResponse<IncoTermDto>();

            try
            {
                var incoTerm = await _repository.IncoTermRepository.GetIncoTermById(id);
                if (incoTerm == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete IncoTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete IncoTerm with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.IncoTermRepository.DeleteIncoTerm(incoTerm);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
 

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateIncoTerm(int id)
        {
            ServiceResponse<IncoTermDto> serviceResponse = new ServiceResponse<IncoTermDto>();

            try
            {
                var incoTerm = await _repository.IncoTermRepository.GetIncoTermById(id);
                if (incoTerm is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IncoTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"IncoTerm with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                incoTerm.IsActive = true;
                string result = await _repository.IncoTermRepository.UpdateIncoTerm(incoTerm);
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
                _logger.LogError($"Something went wrong inside ActivateIncoTerm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateIncoTerm(int id)
        {
            ServiceResponse<IncoTermDto> serviceResponse = new ServiceResponse<IncoTermDto>();

            try
            {
                var incoTerm = await _repository.IncoTermRepository.GetIncoTermById(id);
                if (incoTerm is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IncoTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"IncoTerm with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                incoTerm.IsActive = false;
                string result = await _repository.IncoTermRepository.UpdateIncoTerm(incoTerm);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeactivateIncoTerm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    
    
    
    }
}
