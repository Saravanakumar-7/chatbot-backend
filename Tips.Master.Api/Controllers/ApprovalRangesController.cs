using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System.Net;
using System.Text;

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
        private IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public ApprovalRangesController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper, IHttpClientFactory clientFactory, IConfiguration config)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _clientFactory = clientFactory;
            _config = config;
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
                if (approvalRangesPostDto.Ranges.Count() != 3)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "The No. of Ranges per Procurement can't excide more then 3";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Error occured in CreateApprovalRanges: Error occured in CreateApprovalRanges: The No. of Ranges per Procurement can't excide more then 3");
                    return BadRequest(serviceResponse);
                } 
                if (approvalRangesPostDto.Ranges[2].RangeTo != null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "The 3rd Range's must not have a limit and must be sent as NULL";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Error occured in CreateApprovalRanges: The 3rd Range's must not have a limit and must be sent as NULL");
                    return BadRequest(serviceResponse);
                }
                if(approvalRangesPostDto.Ranges[0].RangeFrom != 1)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "The 1st Range must begin from 1";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Error occured in CreateApprovalRanges: The 1st Range must begin from 1");
                    return BadRequest(serviceResponse);
                } 
                if(approvalRangesPostDto.Ranges[0].RangeTo >= approvalRangesPostDto.Ranges[1].RangeFrom)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "The 2nd Range must begin from value greater then 1st Range";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Error occured in CreateApprovalRanges: The 2nd Range must begin from value greater then 1st Range");
                    return BadRequest(serviceResponse);
                }
                if(approvalRangesPostDto.Ranges[1].RangeTo >= approvalRangesPostDto.Ranges[2].RangeFrom)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "The 3rd Range must begin from value greater then 2nd Range";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Error occured in CreateApprovalRanges: The 3rd Range must begin from value greater then 2nd Range");
                    return BadRequest(serviceResponse);
                }                
                var approvalRanges = await _repository.ApprovalRangesRepository.GetApprovalRangesByProcurementType(approvalRangesPostDto.ProcurementName);
                if (approvalRanges != null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "An Range has already been created for this Procurement Type. Please update it or select other Procurement type";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    _logger.LogError($"An Range has already been created for this Procurement Type: {approvalRanges.ProcurementName}. Please update it or select other Procurement type");
                    return StatusCode(406,serviceResponse);
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
        [HttpGet]
        public async Task<IActionResult> GetAllApprovalRanges([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<List<ApprovalRangesDto>> serviceResponse = new ServiceResponse<List<ApprovalRangesDto>>();

            try
            {
                var getAllApprovalRanges = await _repository.ApprovalRangesRepository.GetAllApprovalRanges(pagingParameter, searchParams);
                _logger.LogInfo("Returned all ApprovalRanges");
                var metadata = new
                {
                    getAllApprovalRanges.TotalCount,
                    getAllApprovalRanges.PageSize,
                    getAllApprovalRanges.CurrentPage,
                    getAllApprovalRanges.HasNext,
                    getAllApprovalRanges.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var result = _mapper.Map<List<ApprovalRangesDto>>(getAllApprovalRanges);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ApprovalRanges Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetAllApprovalRanges: {ex.Message} \n {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetAllApprovalRanges: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetListofProcurementType()
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                var getAllApprovalRanges = await _repository.ApprovalRangesRepository.GetListofProcurementType();
                serviceResponse.Data = getAllApprovalRanges;
                serviceResponse.Message = "Returned ListofProcurementType from ApprovalRanges Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetListofProcurementType: {ex.Message} \n {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetListofProcurementType: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateApprovalRange([FromBody] ApprovalRangesUpdateDto approvalRangesUpdateDto)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (approvalRangesUpdateDto is null)
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
                if (approvalRangesUpdateDto.Ranges.Count() != 3)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "The No. of Ranges per Procurement can't excide more then 3 or be lesser then 3";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Error occured in UpdateApprovalRange: The No. of Ranges per Procurement can't excide more then 3 or be lesser then 3");
                    return BadRequest(serviceResponse);
                }
                if (approvalRangesUpdateDto.Ranges[2].RangeTo != null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "The No. of Ranges per Procurement can't excide more then 3";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Error occured in CreateApprovalRanges: The No. of Ranges per Procurement can't excide more then 3");
                    return BadRequest(serviceResponse);
                }
                if (approvalRangesUpdateDto.Ranges[0].RangeFrom != 1)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "The 1st Range must begin from 1";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Error occured in UpdateApprovalRange: The 1st Range must begin from 1");
                    return BadRequest(serviceResponse);
                }
                if (approvalRangesUpdateDto.Ranges[0].RangeTo >= approvalRangesUpdateDto.Ranges[1].RangeFrom)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "The 2nd Range must begin from value greater then 1st Range";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Error occured in UpdateApprovalRange: The 2nd Range must begin from value greater then 1st Range");
                    return BadRequest(serviceResponse);
                }
                if (approvalRangesUpdateDto.Ranges[1].RangeTo >= approvalRangesUpdateDto.Ranges[2].RangeFrom)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "The 3rd Range must begin from value greater then 2nd Range";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Error occured in UpdateApprovalRange: The 3rd Range must begin from value greater then 2nd Range");
                    return BadRequest(serviceResponse);
                }
                var approvalRanges = await _repository.ApprovalRangesRepository.GetApprovalRangesById(approvalRangesUpdateDto.Id);
                if (approvalRanges == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ApprovalRanges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ApprovalRanges with id: {approvalRangesUpdateDto.Id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                await _repository.ApprovalRangesRepository.UpdateApprovalRange(approvalRanges);

                var NewApproval = _mapper.Map<ApprovalRanges>(approvalRangesUpdateDto);
                NewApproval.Id = 0;
                NewApproval.Version = approvalRanges.Version;
                NewApproval.Ranges.ForEach(x => x.Id = 0);
                NewApproval = await _repository.ApprovalRangesRepository.CreateNewApprovalRangeVersion(NewApproval);
                _repository.SaveAsync();
                var AppforPO = _mapper.Map<ApprovalRangesDto>(NewApproval);
                var jsons = JsonConvert.SerializeObject(AppforPO);
                var data1 = new StringContent(jsons, Encoding.UTF8, "application/json");
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                var request = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["PurchaseService"], "PurchaseOrder/UpdatePurchaseOrdersApprovalRange")){
                    Content= data1
                };
                request.Headers.Add("Authorization", token);
                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error occured in UpdateApprovalRange: PurchaseOrder/UpdatePurchaseOrdersApprovalRange API return error");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Error occured in UpdateApprovalRange: PurchaseOrder/UpdatePurchaseOrdersApprovalRange API return error";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                serviceResponse.Data = null;
                serviceResponse.Message = "ApprovalRanges Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in UpdateApprovalRange: {ex.Message} \n {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in UpdateApprovalRange: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
