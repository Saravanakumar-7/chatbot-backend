using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ScopeOfSupplyController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public ScopeOfSupplyController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<ScopeOfSupplyController>
        [HttpGet]
        public async Task<IActionResult> GetAllScopeOfSupply([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ScopeOfSupplyDto>> serviceResponse = new ServiceResponse<IEnumerable<ScopeOfSupplyDto>>();

            try
            {
                var scopeOfSupplies = await _repository.ScopeOfSupplyRepository.GetAllScopeOfSupply(searchParams);
                _logger.LogInfo("Returned all ScopeOfSupply");
                var result = _mapper.Map<IEnumerable<ScopeOfSupplyDto>>(scopeOfSupplies);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ScopeOfSupply Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllScopeOfSupply API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllScopeOfSupply API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveScopeOfSupply([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ScopeOfSupplyDto>> serviceResponse = new ServiceResponse<IEnumerable<ScopeOfSupplyDto>>();

            try
            {
                var scopeOfSupplies = await _repository.ScopeOfSupplyRepository.GetAllActiveScopeOfSupply(searchParams);
                _logger.LogInfo("Returned all ScopeOfSupply");
                var result = _mapper.Map<IEnumerable<ScopeOfSupplyDto>>(scopeOfSupplies);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active ScopeOfSupply Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveScopeOfSupply API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveScopeOfSupply API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // GET api/<ScopeOfSupplyController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScopeOfSupplyById(int id)
        {
            ServiceResponse<ScopeOfSupplyDto> serviceResponse = new ServiceResponse<ScopeOfSupplyDto>();

            try
            {
                var scopeOfSupply = await _repository.ScopeOfSupplyRepository.GetScopeOfSupplyById(id);
                if (scopeOfSupply == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ScopeOfSupply with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ScopeOfSupply with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<ScopeOfSupplyDto>(scopeOfSupply);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetScopeOfSupplyById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetScopeOfSupplyById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // POST api/<ScopeOfSupplyController>
        [HttpPost]
        public IActionResult CreateScopeOfSupply([FromBody] ScopeOfSupplyPostDto scopeOfSupplyPostDto)
        {
            ServiceResponse<ScopeOfSupplyDto> serviceResponse = new ServiceResponse<ScopeOfSupplyDto>();

            try
            {
                if (scopeOfSupplyPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ScopeOfSupply object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ScopeOfSupply object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ScopeOfSupply object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ScopeOfSupply object sent from client.");
                    return BadRequest(serviceResponse);
                }
                //var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                //var id = _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                //_repository.SaveAsync();

                var scopeOfSupply = _mapper.Map<ScopeOfSupply>(scopeOfSupplyPostDto);
                _repository.ScopeOfSupplyRepository.CreateScopeOfSupply(scopeOfSupply);
                _repository.SaveAsync();
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Created("GetScopeOfSupplyById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateScopeOfSupply API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateScopeOfSupply API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<ScopeOfSupplyController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateScopeOfSupply(int id, [FromBody] ScopeOfSupplyUpdateDto scopeOfSupplyUpdateDto)
        {
            ServiceResponse<ScopeOfSupplyDto> serviceResponse = new ServiceResponse<ScopeOfSupplyDto>();

            try
            {
                if (scopeOfSupplyUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update ScopeOfSupply object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update ScopeOfSupply object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ScopeOfSupply object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid update ScopeOfSupply object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var scopeOfSupply = await _repository.ScopeOfSupplyRepository.GetScopeOfSupplyById(id);
                if (scopeOfSupply is null)
                {
                    _logger.LogError($"Update ScopeOfSupply with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update ScopeOfSupply with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(scopeOfSupplyUpdateDto, scopeOfSupply);
                string result = await _repository.ScopeOfSupplyRepository.UpdateScopeOfSupply(scopeOfSupply);
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
                serviceResponse.Message = $"Error Occured in UpdateScopeOfSupply API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in UpdateScopeOfSupply API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<ScopeOfSupplyController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScopeOfSupply(int id)
        {
            ServiceResponse<ScopeOfSupplyDto> serviceResponse = new ServiceResponse<ScopeOfSupplyDto>();

            try
            {
                var scopeOfSupply = await _repository.ScopeOfSupplyRepository.GetScopeOfSupplyById(id);
                if (scopeOfSupply == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete ScopeOfSupply object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete ScopeOfSupply with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.ScopeOfSupplyRepository.DeleteScopeOfSupply(scopeOfSupply);
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
                serviceResponse.Message = $"Error Occured in DeleteScopeOfSupply API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteScopeOfSupply API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateScopeOfSupply(int id)
        {
            ServiceResponse<ScopeOfSupplyDto> serviceResponse = new ServiceResponse<ScopeOfSupplyDto>();

            try
            {
                var scopeOfSupply = await _repository.ScopeOfSupplyRepository.GetScopeOfSupplyById(id);
                if (scopeOfSupply is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ScopeOfSupply object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ScopeOfSupply with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                scopeOfSupply.IsActive = true;
                string result = await _repository.ScopeOfSupplyRepository.UpdateScopeOfSupply(scopeOfSupply);
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
                serviceResponse.Message = $"Error Occured in ActivateScopeOfSupply API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in ActivateScopeOfSupply API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateScopeOfSupply(int id)
        {
            ServiceResponse<ScopeOfSupplyDto> serviceResponse = new ServiceResponse<ScopeOfSupplyDto>();

            try
            {
                var scopeOfSupply = await _repository.ScopeOfSupplyRepository.GetScopeOfSupplyById(id);
                if (scopeOfSupply is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ScopeOfSupply object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ScopeOfSupply with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                scopeOfSupply.IsActive = false;
                string result = await _repository.ScopeOfSupplyRepository.UpdateScopeOfSupply(scopeOfSupply);
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
                serviceResponse.Message = $"Error Occured in DeactivateScopeOfSupply API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeactivateScopeOfSupply API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
