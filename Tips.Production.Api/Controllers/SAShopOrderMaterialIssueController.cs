using System.Net;
using System.Numerics;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SAShopOrderMaterialIssueController : ControllerBase
    {
        private ISAShopOrderMaterialIssueRepository _sAShopOrderMaterialIssueRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public SAShopOrderMaterialIssueController(ISAShopOrderMaterialIssueRepository SAShopOrderMaterialIssueRepository, ILoggerManager logger, IMapper mapper)
        {
            _sAShopOrderMaterialIssueRepository = SAShopOrderMaterialIssueRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSAShopOrderMaterialIssue([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<SAShopOrderMaterialIssueDto>> serviceResponse = new ServiceResponse<IEnumerable<SAShopOrderMaterialIssueDto>>();
            try
            {
                var getAllSAShopOrderMaterialIssues = await _sAShopOrderMaterialIssueRepository.GetAllSAShopOrderMaterialIssue(pagingParameter);
                var metadata = new
                {
                    getAllSAShopOrderMaterialIssues.TotalCount,
                    getAllSAShopOrderMaterialIssues.PageSize,
                    getAllSAShopOrderMaterialIssues.CurrentPage,
                    getAllSAShopOrderMaterialIssues.HasNext,
                    getAllSAShopOrderMaterialIssues.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all SAShopOrderMaterialIssues");
                var result = _mapper.Map<IEnumerable<SAShopOrderMaterialIssueDto>>(getAllSAShopOrderMaterialIssues);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SAShopOrderMaterialIssues";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<SAShopOrderMaterialIssueController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSAShopOrderMaterialIssueById(int id)
        {
            ServiceResponse<SAShopOrderMaterialIssueDto> serviceResponse = new ServiceResponse<SAShopOrderMaterialIssueDto>();

            try
            {
                var getSAShopOrderMaterialIssue = await _sAShopOrderMaterialIssueRepository.GetSAShopOrderMaterialIssueById(id);

                if (getSAShopOrderMaterialIssue == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrderMaterialIssue with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SAShopOrderMaterialIssue with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned SAShopOrderMaterialIssue with id: {id}");
                    SAShopOrderMaterialIssueDto sAShopOrderMaterialIssueDtos = _mapper.Map<SAShopOrderMaterialIssueDto>(getSAShopOrderMaterialIssue);
                    List<SAShopOrderMaterialIssueGeneralDto> sAShopOrderMaterialIssueGeneralDtoList = new List<SAShopOrderMaterialIssueGeneralDto>();
                    foreach (var itemDetails in getSAShopOrderMaterialIssue.SAShopOrderMaterialIssueGeneralList)
                    {
                        SAShopOrderMaterialIssueGeneralDto sAShopOrderMaterialIssueGeneralDtos = _mapper.Map<SAShopOrderMaterialIssueGeneralDto>(itemDetails);
                        sAShopOrderMaterialIssueGeneralDtoList.Add(sAShopOrderMaterialIssueGeneralDtos);
                    }
                    sAShopOrderMaterialIssueDtos.SAShopOrderMaterialIssueGeneralDtos = sAShopOrderMaterialIssueGeneralDtoList;
                    serviceResponse.Data = sAShopOrderMaterialIssueDtos;
                    serviceResponse.Message = $"Returned SAShopOrderMaterialIssueById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSAShopOrderMaterialIssueById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<SAShopOrderMaterialIssueController>
        [HttpPost]
        public async Task<IActionResult> CreateSAShopOrderMaterialIssue([FromBody] SAShopOrderMaterialIssuePostDto sAShopOrderMaterialIssuePostDto)
        {
            ServiceResponse<SAShopOrderMaterialIssueDto> serviceResponse = new ServiceResponse<SAShopOrderMaterialIssueDto>();

            try
            {
                if (sAShopOrderMaterialIssuePostDto is null)
                {
                    _logger.LogError("SAShopOrderMaterialIssueDetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SAShopOrderMaterialIssueDetails object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid SAShopOrderMaterialIssueDetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SAShopOrderMaterialIssueDetails object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var sAShopOrderMaterialIssueGeneral = _mapper.Map<IEnumerable<SAShopOrderMaterialIssueGeneral>>(sAShopOrderMaterialIssuePostDto.SAShopOrderMaterialIssueGeneralPostDtos);
                var sAShopOrderMaterialIssue = _mapper.Map<SAShopOrderMaterialIssue>(sAShopOrderMaterialIssuePostDto);
                sAShopOrderMaterialIssue.SAShopOrderMaterialIssueGeneralList = sAShopOrderMaterialIssueGeneral.ToList();
                _sAShopOrderMaterialIssueRepository.CreateSAShopOrderMaterialIssue(sAShopOrderMaterialIssue);
                _sAShopOrderMaterialIssueRepository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "SAShopOrderMaterialIssue Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetSAShopOrderMaterialIssueById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateSAShopOrderMaterialIssue action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<SAShopOrderMaterialIssueController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSAShopOrderMaterialIssue(int id, [FromBody] SAShopOrderMaterialIssueUpdateDto sAShopOrderMaterialIssueUpdateDto)
        {
            ServiceResponse<SAShopOrderMaterialIssueUpdateDto> serviceResponse = new ServiceResponse<SAShopOrderMaterialIssueUpdateDto>();

            try
            {
                if (sAShopOrderMaterialIssueUpdateDto is null)
                {
                    _logger.LogError("Update SAShopOrderMaterialIssue object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update SAShopOrderMaterialIssue object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update SAShopOrderMaterialIssue object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update SAShopOrderMaterialIssue object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateSAShopOrderMaterialIssue = await _sAShopOrderMaterialIssueRepository.GetSAShopOrderMaterialIssueById(id);
                if (updateSAShopOrderMaterialIssue is null)
                {
                    _logger.LogError($"Update SAShopOrderMaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update SAShopOrderMaterialIssue with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var sAShopOrderMaterialIssueGeneral = _mapper.Map<IEnumerable<SAShopOrderMaterialIssueGeneral>>(sAShopOrderMaterialIssueUpdateDto.SAShopOrderMaterialIssueGeneralUpdateDtos);
                var sAShopOrderMaterialIssue = _mapper.Map(sAShopOrderMaterialIssueUpdateDto, updateSAShopOrderMaterialIssue);
                sAShopOrderMaterialIssue.SAShopOrderMaterialIssueGeneralList = sAShopOrderMaterialIssueGeneral.ToList();
                string result = await _sAShopOrderMaterialIssueRepository.UpdateSAShopOrderMaterialIssue(sAShopOrderMaterialIssue);
                _logger.LogInfo(result);
                _sAShopOrderMaterialIssueRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "SAShopOrderMaterialIssue Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateSAShopOrderMaterialIssue action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // DELETE api/<SAShopOrderMaterialIssueController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSAShopOrderMaterialIssue(int id)
        {
            ServiceResponse<SAShopOrderMaterialIssueDto> serviceResponse = new ServiceResponse<SAShopOrderMaterialIssueDto>();

            try
            {
                var deleteSAShopOrderMaterialIssue = await _sAShopOrderMaterialIssueRepository.GetSAShopOrderMaterialIssueById(id);
                if (deleteSAShopOrderMaterialIssue == null)
                {
                    _logger.LogError($"Delete SAShopOrderMaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete SAShopOrderMaterialIssue with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _sAShopOrderMaterialIssueRepository.DeleteSAShopOrderMaterialIssue(deleteSAShopOrderMaterialIssue);
                _logger.LogInfo(result);
                _sAShopOrderMaterialIssueRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "SAShopOrderMaterialIssue Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteSAShopOrderMaterialIssue action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}