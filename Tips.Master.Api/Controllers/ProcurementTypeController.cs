using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProcurementTypeController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public ProcurementTypeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<ProcurementTypeController>
        [HttpGet]
        public async Task<IActionResult> GetAllProcurementType([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ProcurementTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<ProcurementTypeDto>>();

            try
            {
                var procurementTypeList = await _repository.ProcurementTypeRepository.GetAllProcurementType(searchParams);
                _logger.LogInfo("Returned all ProcurementTypes");
                var result = _mapper.Map<IEnumerable<ProcurementTypeDto>>(procurementTypeList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ProcurementTypes Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllProcurementType API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllProcurementType API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveProcurementTypes([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ProcurementTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<ProcurementTypeDto>>();

            try
            {
                var ProcurementType = await _repository.ProcurementTypeRepository.GetAllActiveProcurementType(searchParams);
                _logger.LogInfo("Returned all ProcurementTypes");
                var result = _mapper.Map<IEnumerable<ProcurementTypeDto>>(ProcurementType);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active ProcurementTypes Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveProcurementTypes API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveProcurementTypes API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }
        // GET api/<ProcurementTypeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProcurementTypeById(int id)
        {
            ServiceResponse<ProcurementTypeDto> serviceResponse = new ServiceResponse<ProcurementTypeDto>();

            try
            {
                var procurementType = await _repository.ProcurementTypeRepository.GetProcurementTypeById(id);
                if (procurementType == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ProcurementType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ProcurementType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<ProcurementTypeDto>(procurementType);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetProcurementTypeById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetProcurementTypeById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<ProcurementTypeController>
        [HttpPost]
        public IActionResult CreateProcurementType([FromBody] ProcurementTypeDtoPost procurementTypeDtoPost)
        {
            ServiceResponse<ProcurementTypeDto> serviceResponse = new ServiceResponse<ProcurementTypeDto>();

            try
            {
                if (procurementTypeDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ProcurementType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ProcurementType object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ProcurementType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ProcurementType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var procurementTypeEntity = _mapper.Map<ProcurementType>(procurementTypeDtoPost);
                _repository.ProcurementTypeRepository.CreateProcurementType(procurementTypeEntity);
                _repository.SaveAsync();
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetProcurementTypeById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateProcurementType API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateProcurementType API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<ProcurementTypeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProcurementType(int id, [FromBody] ProcurementTypeDtoUpdate procurementTypeDtoUpdate)
        {
            ServiceResponse<ProcurementTypeDto> serviceResponse = new ServiceResponse<ProcurementTypeDto>();

            try
            {
                if (procurementTypeDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update ProcurementType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ProcurementType object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update ProcurementType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid update ProcurementType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var procurementTypeEntity = await _repository.ProcurementTypeRepository.GetProcurementTypeById(id);
                if (procurementTypeEntity is null)
                {
                    _logger.LogError($"Update ProcurementType with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update ProcurementType with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(procurementTypeDtoUpdate, procurementTypeEntity);
                string result = await _repository.ProcurementTypeRepository.UpdateProcurementType(procurementTypeEntity);
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
                serviceResponse.Message = $"Error Occured in UpdateProcurementType API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateProcurementType API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<ProcurementTypeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcurementType(int id)
        {
            ServiceResponse<ProcurementTypeDto> serviceResponse = new ServiceResponse<ProcurementTypeDto>();

            try
            {
                var procurementType = await _repository.ProcurementTypeRepository.GetProcurementTypeById(id);
                if (procurementType == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete ProcurementType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete ProcurementType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.ProcurementTypeRepository.DeleteProcurementType(procurementType);
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
                serviceResponse.Message = $"Error Occured in DeleteProcurementType API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteProcurementType API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateProcurementType(int id)
        {
            ServiceResponse<ProcurementTypeDto> serviceResponse = new ServiceResponse<ProcurementTypeDto>();

            try
            {
                var procurementType = await _repository.ProcurementTypeRepository.GetProcurementTypeById(id);
                if (procurementType is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ProcurementType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ProcurementType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                procurementType.IsActive = true;
                string result = await _repository.ProcurementTypeRepository.UpdateProcurementType(procurementType);
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
                serviceResponse.Message = $"Error Occured in ActivateProcurementType API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in ActivateProcurementType API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateProcurementType(int id)
        {
            ServiceResponse<ProcurementTypeDto> serviceResponse = new ServiceResponse<ProcurementTypeDto>();

            try
            {
                var procurementType = await _repository.ProcurementTypeRepository.GetProcurementTypeById(id);
                if (procurementType is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ProcurementType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ProcurementType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                procurementType.IsActive = false;
                string result = await _repository.ProcurementTypeRepository.UpdateProcurementType(procurementType);
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
                serviceResponse.Message = $"Error Occured in DeactivateProcurementType API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeactivateProcurementType API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
