using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ApprovalRangesController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public ApprovalRangesController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> CreateApprovalRanges([FromBody] ApprovalRangesPostDto approvalRangesPostDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (approvalRangesPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ApprovalRanges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ApprovalRanges object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ApprovalRanges object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ApprovalRanges object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var result = _mapper.Map<ApprovalRanges>(approvalRangesPostDto);
                await _repository.ApprovalRangesRepository.CreateApprovalRanges(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("CreateApprovalRanges", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateApprovalRanges: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateApprovalRanges action: {ex.Message} and {ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetApprovalRangesById(int id)
        {
            ServiceResponse<ApprovalRangesDto> serviceResponse = new ServiceResponse<ApprovalRangesDto>();

            try
            {
                var approvalRanges = await _repository.ApprovalRangesRepository.GetApprovalRangesById(id);
                if (approvalRanges == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ApprovalRanges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ApprovalRanges with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<ApprovalRangesDto>(approvalRanges);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetApprovalRangesById action: {ex.Message} \n {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong {ex.Message}. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{ProcurementType}")]
        public async Task<IActionResult> GetApprovalRangesByProcurementType(string ProcurementType)
        {
            ServiceResponse<ApprovalRangesDto> serviceResponse = new ServiceResponse<ApprovalRangesDto>();

            try
            {
                var approvalRanges = await _repository.ApprovalRangesRepository.GetApprovalRangesByProcurementType(ProcurementType);
                if (approvalRanges == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ApprovalRanges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ApprovalRanges with ProcurementType: {ProcurementType}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ApprovalRanges by ProcurementType: {ProcurementType}");
                    var result = _mapper.Map<ApprovalRangesDto>(approvalRanges);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned ApprovalRanges by ProcurementType Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetApprovalRangesByProcurementType action: {ex.Message} \n {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetApprovalRangesByProcurementType action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        
    }
}
