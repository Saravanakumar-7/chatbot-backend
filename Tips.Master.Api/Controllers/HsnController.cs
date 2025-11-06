using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Net;
using System.Security.Claims;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class HsnController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public HsnController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<HsnController>
        [HttpGet]
        public async Task<IActionResult> GetAllHsn([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<HsnDto>> serviceResponse = new ServiceResponse<IEnumerable<HsnDto>>();

            try
            {
                var HsnList = await _repository.HSNRepository.GetAllHSN(searchParams);
                _logger.LogInfo("Returned all Hsn");
                var result = _mapper.Map<IEnumerable<HsnDto>>(HsnList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Hsn Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllHsn API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllHsn API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveHsns([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<HsnDto>> serviceResponse = new ServiceResponse<IEnumerable<HsnDto>>();

            try
            {
                var HsnList = await _repository.HSNRepository.GetAllActiveHSN(searchParams);
                _logger.LogInfo("Returned all Hsn");
                var result = _mapper.Map<IEnumerable<HsnDto>>(HsnList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Hsn Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveHsns API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveHsns API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }
        // GET api/<HsnController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHsnById(int id)
        {
            ServiceResponse<HsnDto> serviceResponse = new ServiceResponse<HsnDto>();

            try
            {
                var Hsn = await _repository.HSNRepository.GetHSNById(id);
                if (Hsn == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Hsn with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Hsn with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<HsnDto>(Hsn);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetHsnById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetHsnById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<HsnController>
        [HttpPost]
        public IActionResult CreateHsn([FromBody] HsnPostDto HsnDtoPost)
        {
            ServiceResponse<HsnDto> serviceResponse = new ServiceResponse<HsnDto>();

            try
            {
                if (HsnDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Hsn object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Hsn object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Hsn object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Hsn object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var HsnEntity = _mapper.Map<Hsn>(HsnDtoPost);
                _repository.HSNRepository.CreateHSN(HsnEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetHsnById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateHsn API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateHsn API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<HsnController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHsn(int id, [FromBody] HsnUpdateDto HsnDtoUpdate)
        {
            ServiceResponse<HsnDto> serviceResponse = new ServiceResponse<HsnDto>();

            try
            {
                if (HsnDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Hsn object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Department object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Hsn object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var HsnEntity = await _repository.HSNRepository.GetHSNById(id);
                if (HsnEntity is null)
                {
                    _logger.LogError($"Hsn with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(HsnDtoUpdate, HsnEntity);
                string result = await _repository.HSNRepository.UpdateHSN(HsnEntity);
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
                serviceResponse.Message = $"Error Occured in UpdateHsn API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateHsn API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<HsnController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHsn(int id)
        {
            ServiceResponse<HsnDto> serviceResponse = new ServiceResponse<HsnDto>();

            try
            {
                var Hsn = await _repository.HSNRepository.GetHSNById(id);
                if (Hsn == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "c Hsn object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Hsn Hsn with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.HSNRepository.DeleteHSN(Hsn);
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
                serviceResponse.Message = $"Error Occured in DeleteHsn API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteHsn API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateHsn(int id)
        {
            ServiceResponse<HsnDto> serviceResponse = new ServiceResponse<HsnDto>();

            try
            {
                var Hsn = await _repository.HSNRepository.GetHSNById(id);
                if (Hsn is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Hsn object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Hsn with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Hsn.IsActive = true;
                string result = await _repository.HSNRepository.UpdateHSN(Hsn);
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
                serviceResponse.Message = $"Error Occured in ActivateHsn API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in ActivateHsn API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateHsn(int id)
        {
            ServiceResponse<HsnDto> serviceResponse = new ServiceResponse<HsnDto>();

            try
            {
                var Hsn = await _repository.HSNRepository.GetHSNById(id);
                if (Hsn is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Hsn object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Hsn with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Hsn.IsActive = false;
                string result = await _repository.HSNRepository.UpdateHSN(Hsn);
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
                serviceResponse.Message = $"Error Occured in DeactivateHsn API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in DeactivateHsn API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
