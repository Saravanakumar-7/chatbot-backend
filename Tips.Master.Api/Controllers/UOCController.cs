using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UOCController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public UOCController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<UOMController>
        [HttpGet]
        public async Task<IActionResult> GetAllUOC([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<UOCDto>> serviceResponse = new ServiceResponse<IEnumerable<UOCDto>>();

            try
            {
                var UOCList = await _repository.UOCRepository.GetAllUOC(searchParams);
                _logger.LogInfo("Returned all UOC");
                var result = _mapper.Map<IEnumerable<UOCDto>>(UOCList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all UOC Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllUOC API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllUOC API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveUocs([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<UOCDto>> serviceResponse = new ServiceResponse<IEnumerable<UOCDto>>();

            try
            {
                var UOCList = await _repository.UOCRepository.GetAllActiveUOC(searchParams);
                _logger.LogInfo("Returned all UOC");
                var result = _mapper.Map<IEnumerable<UOCDto>>(UOCList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active UOC Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveUocs API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveUocs API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }

        // GET api/<UOCController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUOCById(int id)
        {
            ServiceResponse<UOCDto> serviceResponse = new ServiceResponse<UOCDto>();

            try
            {
                var UOC = await _repository.UOCRepository.GetUOCById(id);
                if (UOC == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Department with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<UOCDto>(UOC);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetUOCById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetUOCById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<UOMController>
        [HttpPost]
        public IActionResult CreateUOC([FromBody] UOCDtoPost UocDtoPost)
        {
            ServiceResponse<UOCDto> serviceResponse = new ServiceResponse<UOCDto>();

            try
            {
                if (UocDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "UOC object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("UOC object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid UOC object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid UOC object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var UOCEntity = _mapper.Map<UOC>(UocDtoPost);
                _repository.UOCRepository.CreateUOC(UOCEntity);
                _repository.SaveAsync();
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetUOCById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Error Occured in CreateUOC API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateUOC API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<UOMController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUOC(int id, [FromBody] UOCDtoUpdate UocDtoUpdate)
        {
            ServiceResponse<UOCDto> serviceResponse = new ServiceResponse<UOCDto>();

            try
            {
                if (UocDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update UOC object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update UOC object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update UOC object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update UOC object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var UOCEntity = await _repository.UOCRepository.GetUOCById(id);
                if (UOCEntity is null)
                {
                    _logger.LogError($"Update UOC with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update UOC with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(UocDtoUpdate, UOCEntity);
                string result = await _repository.UOCRepository.UpdateUOC(UOCEntity);
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
                serviceResponse.Message = $"Error Occured in UpdateUOC API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateUOC API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<UOCController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUOC(int id)
        {
            ServiceResponse<UOCDto> serviceResponse = new ServiceResponse<UOCDto>();

            try
            {
                var UOC = await _repository.UOCRepository.GetUOCById(id);
                if (UOC == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Uoc object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete UOC with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.UOCRepository.DeleteUOC(UOC);
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
                serviceResponse.Message = $"Error Occured in DeleteUOC API for the following id : {id} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteUOC API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateUOC(int id)
        {
            ServiceResponse<UOCDto> serviceResponse = new ServiceResponse<UOCDto>();

            try
            {
                var UOC = await _repository.UOCRepository.GetUOCById(id);
                if (UOC is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "UOC object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                UOC.ActiveStatus = true;
                string result = await _repository.UOCRepository.UpdateUOC(UOC);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ActivateUOC API for the following id : {id} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in ActivateUOC API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateUOC(int id)
        {
            ServiceResponse<UOCDto> serviceResponse = new ServiceResponse<UOCDto>();

            try
            {
                var UOC = await _repository.UOCRepository.GetUOCById(id);
                if (UOC is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "UOC object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"UOC with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                UOC.ActiveStatus = false;
                string result = await _repository.UOCRepository.UpdateUOC(UOC);
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
                serviceResponse.Message = $"Error Occured in DeactivateUOC API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in DeactivateUOC API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
