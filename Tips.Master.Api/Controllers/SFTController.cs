using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SFTController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public SFTController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllSFT()
        {
            ServiceResponse<IEnumerable<SFTDto>> serviceResponse = new ServiceResponse<IEnumerable<SFTDto>>();
            try
            {

                var sftList = await _repository.SFTRepository.GetAllSFT();
                _logger.LogInfo("Returned all LeadTypes");
                var result = _mapper.Map<IEnumerable<SFTDto>>(sftList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SFT Successfully";
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
        public async Task<IActionResult> GetAllActiveSFT()
        {
            ServiceResponse<IEnumerable<SFTDto>> serviceResponse = new ServiceResponse<IEnumerable<SFTDto>>();

            try
            {
                var sft = await _repository.SFTRepository.GetAllActiveSFT();
                _logger.LogInfo("Returned all SFT");
                var result = _mapper.Map<IEnumerable<SFTDto>>(sft);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active SFt Successfully";
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
        public async Task<IActionResult> GetSFTById(int id)
        {
            ServiceResponse<SFTDto> serviceResponse = new ServiceResponse<SFTDto>();

            try
            {
                var sft = await _repository.SFTRepository.GetSFTById(id);
                if (sft == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"sft with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"sft with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned sft with id: {id}");
                    var result = _mapper.Map<SFTDto>(sft);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned sft with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSFTById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateSFT([FromBody] SFTPostDto sFTPostDto)
        {
            ServiceResponse<SFTPostDto> serviceResponse = new ServiceResponse<SFTPostDto>();

            try
            {
                if (sFTPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BHK object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("BHK object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid BHK object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid BHK object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var sft = _mapper.Map<SFT>(sFTPostDto);
                _repository.SFTRepository.CreateSFT(sft);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "BHK Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("SFT", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateSFT action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSFT(int id, [FromBody] SFTUpdateDto sFTUpdateDto)
        {
            ServiceResponse<LeadTypeDto> serviceResponse = new ServiceResponse<LeadTypeDto>();

            try
            {
                if (sFTUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update BHK object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update BHK object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update BHK object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update BHK object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var sft = await _repository.SFTRepository.GetSFTById(id);
                if (sft is null)
                {
                    _logger.LogError($"Update BHK with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update BHK with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(sFTUpdateDto, sft);
                string result = await _repository.SFTRepository.UpdateSFT(sft);
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
                _logger.LogError($"Something went wrong inside UpdateSFT action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSFT(int id)
        {
            ServiceResponse<SFTDto> serviceResponse = new ServiceResponse<SFTDto>();

            try
            {
                var sft = await _repository.SFTRepository.GetSFTById(id);
                if (sft == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete sft object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete sft with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.SFTRepository.DeleteSFT(sft);
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
                _logger.LogError($"Something went wrong inside DeleteSFT action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateSFT(int id)
        {
            ServiceResponse<SFTDto> serviceResponse = new ServiceResponse<SFTDto>();

            try
            {
                var sft = await _repository.SFTRepository.GetSFTById(id);
                if (sft is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "sft object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"sft with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                sft.IsActive = true;
                string result = await _repository.SFTRepository.UpdateSFT(sft);
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
                _logger.LogError($"Something went wrong inside ActivatedSFt action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateSFT(int id)
        {
            ServiceResponse<SFTDto> serviceResponse = new ServiceResponse<SFTDto>();

            try
            {
                var sft = await _repository.SFTRepository.GetSFTById(id);
                if (sft is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "sft object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"sft with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                sft.IsActive = false;
                string result = await _repository.SFTRepository.UpdateSFT(sft);
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
                _logger.LogError($"Something went wrong inside DeactivatedSFT action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}