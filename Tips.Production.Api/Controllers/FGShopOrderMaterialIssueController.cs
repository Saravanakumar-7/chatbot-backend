using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FGShopOrderMaterialIssueController : ControllerBase
    {
        private IFGShopOrderMaterialIssueRepository _fGShopOrderMaterialIssueRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public FGShopOrderMaterialIssueController(IFGShopOrderMaterialIssueRepository FGShopOrderMaterialIssueRepository, ILoggerManager logger, IMapper mapper)
        {
            _fGShopOrderMaterialIssueRepository = FGShopOrderMaterialIssueRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFGShopOrderMaterialIssues([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<FGShopOrderMaterialIssueDto>> serviceResponse = new ServiceResponse<IEnumerable<FGShopOrderMaterialIssueDto>>();
            try
            {
                var fGShopOrderMaterialIssueDetails = await _fGShopOrderMaterialIssueRepository.GetAllFGShopOrderMaterialIssues(pagingParameter);
                var metadata = new
                {
                    fGShopOrderMaterialIssueDetails.TotalCount,
                    fGShopOrderMaterialIssueDetails.PageSize,
                    fGShopOrderMaterialIssueDetails.CurrentPage,
                    fGShopOrderMaterialIssueDetails.HasNext,
                    fGShopOrderMaterialIssueDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all FGShopOrderMaterialIssues");
                var result = _mapper.Map<IEnumerable<FGShopOrderMaterialIssueDto>>(fGShopOrderMaterialIssueDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FGShopOrderMaterialIssues";
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

        // GET api/<FGShopOrderMaterialIssueController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFGShopOrderMaterialIssueById(int id)
        {
            ServiceResponse<FGShopOrderMaterialIssueDto> serviceResponse = new ServiceResponse<FGShopOrderMaterialIssueDto>();

            try
            {
                var fGShopOrderMaterialIssueDetailById = await _fGShopOrderMaterialIssueRepository.GetFGShopOrderMaterialIssueById(id);

                if (fGShopOrderMaterialIssueDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"FGShopOrderMaterialIssue with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"FGShopOrderMaterialIssue with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned FGShopOrderMaterialIssue with id: {id}");

                    FGShopOrderMaterialIssueDto fGShopOrderMaterialIssueDtos = _mapper.Map<FGShopOrderMaterialIssueDto>(fGShopOrderMaterialIssueDetailById);
                    List<FGShopOrderMaterialIssueGeneralDto> fGShopOrderMaterialIssueGeneralDtoList = new List<FGShopOrderMaterialIssueGeneralDto>();
                    if (fGShopOrderMaterialIssueDetailById.FGShopOrderMaterialIssueGeneralList != null)
                    {
                        foreach (var itemDetails in fGShopOrderMaterialIssueDetailById.FGShopOrderMaterialIssueGeneralList)
                        {
                            FGShopOrderMaterialIssueGeneralDto fGShopOrderMaterialIssueGeneralDtos = _mapper.Map<FGShopOrderMaterialIssueGeneralDto>(itemDetails);
                            fGShopOrderMaterialIssueGeneralDtoList.Add(fGShopOrderMaterialIssueGeneralDtos);
                        }
                    }
                    fGShopOrderMaterialIssueDtos.FGShopOrderMaterialIssueGeneralDtos = fGShopOrderMaterialIssueGeneralDtoList;
                    serviceResponse.Data = fGShopOrderMaterialIssueDtos;
                    serviceResponse.Message = $"Returned FGShopOrderMaterialIssueById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetFGShopOrderMaterialIssueById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<FGShopOrderMaterialIssueController>
        [HttpPost]
        public async Task<IActionResult> CreateFGShopOrderMaterialIssue([FromBody] FGShopOrderMaterialIssuePostDto fGShopOrderMaterialIssuePostDto)
        {
            ServiceResponse<FGShopOrderMaterialIssueDto> serviceResponse = new ServiceResponse<FGShopOrderMaterialIssueDto>();

            try
            {
                if (fGShopOrderMaterialIssuePostDto is null)
                {
                    _logger.LogError("FGShopOrderMaterialIssueDetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "FGShopOrderMaterialIssueDetails object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid FGShopOrderMaterialIssueDetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid FGShopOrderMaterialIssueDetails object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var fGShopOrderMaterialIssueGeneral = _mapper.Map<IEnumerable<FGShopOrderMaterialIssueGeneral>>(fGShopOrderMaterialIssuePostDto.FGShopOrderMaterialIssueGeneralPostDtos);
                var fGShopOrderMaterialIssue = _mapper.Map<FGShopOrderMaterialIssue>(fGShopOrderMaterialIssuePostDto);
                fGShopOrderMaterialIssue.FGShopOrderMaterialIssueGeneralList = fGShopOrderMaterialIssueGeneral.ToList();
                await _fGShopOrderMaterialIssueRepository.CreateFGShopOrderMaterialIssue(fGShopOrderMaterialIssue);
                _fGShopOrderMaterialIssueRepository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "FGShopOrderMaterialIssue Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetFGShopOrderMaterialIssueById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateFGShopOrderMaterialIssue action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<FGShopOrderMaterialIssueController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFGShopOrderMaterialIssue(int id, [FromBody] FGShopOrderMaterialIssueUpdateDto fGShopOrderMaterialIssueUpdateDto)
        {
            ServiceResponse<FGShopOrderMaterialIssueUpdateDto> serviceResponse = new ServiceResponse<FGShopOrderMaterialIssueUpdateDto>();

            try
            {
                if (fGShopOrderMaterialIssueUpdateDto is null)
                {
                    _logger.LogError("Update FGShopOrderMaterialIssue object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update FGShopOrderMaterialIssue object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update FGShopOrderMaterialIssue object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update FGShopOrderMaterialIssue object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var fGShopOrderMaterialIssueDetailById = await _fGShopOrderMaterialIssueRepository.GetFGShopOrderMaterialIssueById(id);
                if (fGShopOrderMaterialIssueDetailById is null)
                {
                    _logger.LogError($"Update FGShopOrderMaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update FGShopOrderMaterialIssue with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var fGShopOrderMaterialIssueGeneral = _mapper.Map<IEnumerable<FGShopOrderMaterialIssueGeneral>>(fGShopOrderMaterialIssueUpdateDto.FGShopOrderMaterialIssueGeneralUpdateDtos);
                var fGShopOrderMaterialIssue = _mapper.Map(fGShopOrderMaterialIssueUpdateDto, fGShopOrderMaterialIssueDetailById);
                fGShopOrderMaterialIssue.FGShopOrderMaterialIssueGeneralList = fGShopOrderMaterialIssueGeneral.ToList();
                string result = await _fGShopOrderMaterialIssueRepository.UpdateFGShopOrderMaterialIssue(fGShopOrderMaterialIssue);
                _logger.LogInfo(result);
                _fGShopOrderMaterialIssueRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "FGShopOrderMaterialIssue Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateFGShopOrderMaterialIssue action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // DELETE api/<FGShopOrderMaterialIssueController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFGShopOrderMaterialIssue(int id)
        {
            ServiceResponse<FGShopOrderMaterialIssueDto> serviceResponse = new ServiceResponse<FGShopOrderMaterialIssueDto>();

            try
            {
                var fGShopOrderMaterialIssueDetailsById = await _fGShopOrderMaterialIssueRepository.GetFGShopOrderMaterialIssueById(id);
                if (fGShopOrderMaterialIssueDetailsById == null)
                {
                    _logger.LogError($"Delete FGShopOrderMaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete FGShopOrderMaterialIssue with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _fGShopOrderMaterialIssueRepository.DeleteFGShopOrderMaterialIssue(fGShopOrderMaterialIssueDetailsById);
                _logger.LogInfo(result);
                _fGShopOrderMaterialIssueRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "FGShopOrderMaterialIssue Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteFGShopOrderMaterialIssue action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
