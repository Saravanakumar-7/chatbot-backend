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
        public async Task<IActionResult> GetAllFGShopOrderMaterialIssue([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<FGShopOrderMaterialIssueDto>> serviceResponse = new ServiceResponse<IEnumerable<FGShopOrderMaterialIssueDto>>();
            try
            {
                var listOfFGShopOrderMaterialIssue = await _fGShopOrderMaterialIssueRepository.GetAllFGShopOrderMaterialIssue(pagingParameter);
                var metadata = new
                {
                    listOfFGShopOrderMaterialIssue.TotalCount,
                    listOfFGShopOrderMaterialIssue.PageSize,
                    listOfFGShopOrderMaterialIssue.CurrentPage,
                    listOfFGShopOrderMaterialIssue.HasNext,
                    listOfFGShopOrderMaterialIssue.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all FGShopOrderMaterialIssue");
                var result = _mapper.Map<IEnumerable<FGShopOrderMaterialIssueDto>>(listOfFGShopOrderMaterialIssue);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FGShopOrderMaterialIssue";
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
                var fGShopOrderMaterialIssueDetails = await _fGShopOrderMaterialIssueRepository.GetFGShopOrderMaterialIssueById(id);

                if (fGShopOrderMaterialIssueDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"FGShopOrderMaterialIssue with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"FGShopOrderMaterialIssue with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned FGShopOrderMaterialIssue with id: {id}");

                    FGShopOrderMaterialIssueDto fGShopOrderMaterialIssueEntity = _mapper.Map<FGShopOrderMaterialIssueDto>(fGShopOrderMaterialIssueDetails);
                    List<FGShopOrderMaterialIssueGeneralDto> fGShopOrderMaterialIssueGeneralDtoList = new List<FGShopOrderMaterialIssueGeneralDto>();
                    foreach(var itemDetails in fGShopOrderMaterialIssueDetails.FGShopOrderMaterialIssueGeneralList)
                    {
                        FGShopOrderMaterialIssueGeneralDto fGShopOrderMaterialIssueGeneralDtos = _mapper.Map<FGShopOrderMaterialIssueGeneralDto>(itemDetails);
                        fGShopOrderMaterialIssueGeneralDtoList.Add(fGShopOrderMaterialIssueGeneralDtos);
                    }
                    fGShopOrderMaterialIssueEntity.FGShopOrderMaterialIssueGeneralDtos = fGShopOrderMaterialIssueGeneralDtoList;
                    serviceResponse.Data = fGShopOrderMaterialIssueEntity;
                    serviceResponse.Message = $"Returned FGShopOrderMaterialIssue with id Successfully";
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
        public async Task<IActionResult> CreateFGShopOrderMaterialIssue([FromBody] FGShopOrderMaterialIssueDtoPost fGShopOrderMaterialIssueDtoPost)
        {
            ServiceResponse<FGShopOrderMaterialIssueDto> serviceResponse = new ServiceResponse<FGShopOrderMaterialIssueDto>();

            try
            {
                if (fGShopOrderMaterialIssueDtoPost is null)
                {
                    _logger.LogError("FGShopOrderMaterialIssueDetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "FGShopOrderMaterialIssueDetails object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid FGShopOrderMaterialIssueDetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid FGShopOrderMaterialIssueDetails object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var fGShopOrderMaterialIssueGeneralEntity = _mapper.Map<IEnumerable<FGShopOrderMaterialIssueGeneral>>(fGShopOrderMaterialIssueDtoPost.FGShopOrderMaterialIssueGeneralPostDtos);
                var fGShopOrderMaterialIssueEntity = _mapper.Map<FGShopOrderMaterialIssue>(fGShopOrderMaterialIssueDtoPost);
                fGShopOrderMaterialIssueEntity.FGShopOrderMaterialIssueGeneralList = fGShopOrderMaterialIssueGeneralEntity.ToList();
                _fGShopOrderMaterialIssueRepository.CreateFGShopOrderMaterialIssue(fGShopOrderMaterialIssueEntity);
                _fGShopOrderMaterialIssueRepository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "FGShopOrderMaterialIssueDetails Successfully Created";
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
        public async Task<IActionResult> UpdateFGShopOrderMaterialIssue(int id, [FromBody] FGShopOrderMaterialIssueDtoUpdate fGShopOrderMaterialIssueDtoUpdate)
        {
            ServiceResponse<FGShopOrderMaterialIssueDtoUpdate> serviceResponse = new ServiceResponse<FGShopOrderMaterialIssueDtoUpdate>();

            try
            {
                if (fGShopOrderMaterialIssueDtoUpdate is null)
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
                    serviceResponse.Message = "Invalid Update FGShopOrderMaterialIssue object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateFGShopOrderMaterialIssue = await _fGShopOrderMaterialIssueRepository.GetFGShopOrderMaterialIssueById(id);
                if (updateFGShopOrderMaterialIssue is null)
                {
                    _logger.LogError($"Update FGShopOrderMaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update FGShopOrderMaterialIssue with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var fGShopOrderMaterialIssueGeneral = _mapper.Map<IEnumerable<FGShopOrderMaterialIssueGeneral>>(fGShopOrderMaterialIssueDtoUpdate.FGShopOrderMaterialIssueGeneralUpdateDtos);
                var fGShopOrderMaterialIssueEntity = _mapper.Map(fGShopOrderMaterialIssueDtoUpdate, updateFGShopOrderMaterialIssue);
                fGShopOrderMaterialIssueEntity.FGShopOrderMaterialIssueGeneralList = fGShopOrderMaterialIssueGeneral.ToList();
                string result = await _fGShopOrderMaterialIssueRepository.UpdateFGShopOrderMaterialIssue(fGShopOrderMaterialIssueEntity);
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
                var deleteFGShopOrderMaterialIssue = await _fGShopOrderMaterialIssueRepository.GetFGShopOrderMaterialIssueById(id);
                if (deleteFGShopOrderMaterialIssue == null)
                {
                    _logger.LogError($"Delete FGShopOrderMaterialIssue with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete FGShopOrderMaterialIssue with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _fGShopOrderMaterialIssueRepository.DeleteFGShopOrderMaterialIssue(deleteFGShopOrderMaterialIssue);
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
