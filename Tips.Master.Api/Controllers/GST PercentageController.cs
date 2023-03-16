using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GST_PercentageController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public GST_PercentageController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<GST_PercentageController>
        [HttpGet]
        public async Task<IActionResult> GetAllGST_Percentages([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<GST_PercentageDto>> serviceResponse = new ServiceResponse<IEnumerable<GST_PercentageDto>>();
            try
            {

                var GST_PercentageList = await _repository.GST_PercentageRepository.GetAllGST_Percentages(pagingParameter, searchParams);

                var metadata = new
                {
                    GST_PercentageList.TotalCount,
                    GST_PercentageList.PageSize,
                    GST_PercentageList.CurrentPage,
                    GST_PercentageList.HasNext,
                    GST_PercentageList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all GST_Percentages");
                var result = _mapper.Map<IEnumerable<GST_PercentageDto>>(GST_PercentageList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GST_PercentageList Successfully";
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

        public async Task<IActionResult> GetAllActiveGST_Percentages([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<GST_PercentageDto>> serviceResponse = new ServiceResponse<IEnumerable<GST_PercentageDto>>();

            try
            {
                var GST_Percentage = await _repository.GST_PercentageRepository.GetAllActiveGST_Percentages(pagingParameter, searchParams);
                _logger.LogInfo("Returned all GST_Percentages");
                var result = _mapper.Map<IEnumerable<GST_PercentageDto>>(GST_Percentage);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GST_Percentages Successfully";
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

        // GET api/<GST_PercentageController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGST_PercentageById(int id)
        {
            ServiceResponse<GST_PercentageDto> serviceResponse = new ServiceResponse<GST_PercentageDto>();

            try
            {
                var GST_Percentage = await _repository.GST_PercentageRepository.GetGST_PercentageById(id);
                if (GST_Percentage == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GST_Percentage with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"GST_Percentage with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned GST_Percentage with id: {id}");
                    var result = _mapper.Map<GST_PercentageDto>(GST_Percentage);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned GST_Percentage with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGST_PercentageById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<GST_PercentageController>
        [HttpPost]
        public IActionResult CreateGST_Percentage([FromBody] GST_PercentageDtoPost gst_PercentageDtoPost)
        {
            ServiceResponse<GST_PercentageDto> serviceResponse = new ServiceResponse<GST_PercentageDto>();

            try
            {
                if (gst_PercentageDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GST_Percentage object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("GST_Percentage object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid GST_Percentage object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid GST_Percentage object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var GST_Percentage = _mapper.Map<GST_Percentage>(gst_PercentageDtoPost);
                _repository.GST_PercentageRepository.CreateGST_Percentage(GST_Percentage);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetGST_PercentageById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateGST_Percentage action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<GST_PercentageController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGST_Percentage(int id, [FromBody] GST_PercentageDtoUpdate gST_PercentageDtoUpdate)
        {
            ServiceResponse<GST_PercentageDto> serviceResponse = new ServiceResponse<GST_PercentageDto>();

            try
            {
                if (gST_PercentageDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update GST_Percentage object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("updare GST_Percentage object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update GST_Percentage object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update GST_Percentage object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var GST_Percentage = await _repository.GST_PercentageRepository.GetGST_PercentageById(id);
                if (GST_Percentage is null)
                {
                    _logger.LogError($"Update GST_Percentage with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update GST_Percentage with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(gST_PercentageDtoUpdate, GST_Percentage);
                string result = await _repository.GST_PercentageRepository.UpdateGST_Percentage(GST_Percentage);
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
                _logger.LogError($"Something went wrong inside UpdateGST_Percentage action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<GST_PercentageController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGST_Percentage(int id)
        {
            ServiceResponse<GST_PercentageDto> serviceResponse = new ServiceResponse<GST_PercentageDto>();

            try
            {
                var GST_Percentage = await _repository.GST_PercentageRepository.GetGST_PercentageById(id);
                if (GST_Percentage == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete GST_Percentage object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete GST_Percentage with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.GST_PercentageRepository.DeleteGST_Percentage(GST_Percentage);
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
                _logger.LogError($"Something went wrong inside GST_Percentage action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateGST_Percentage(int id)
        {
            ServiceResponse<GST_PercentageDto> serviceResponse = new ServiceResponse<GST_PercentageDto>();

            try
            {
                var GST_Percentage = await _repository.GST_PercentageRepository.GetGST_PercentageById(id);
                if (GST_Percentage is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GST_Percentage object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"GST_Percentage with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                GST_Percentage.IsActive = true;
                string result = await _repository.GST_PercentageRepository.UpdateGST_Percentage(GST_Percentage);
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
                _logger.LogError($"Something went wrong inside ActivatedGST_Percentage action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateGST_Percentage(int id)
        {
            ServiceResponse<GST_PercentageDto> serviceResponse = new ServiceResponse<GST_PercentageDto>();

            try
            {
                var GST_Percentage = await _repository.GST_PercentageRepository.GetGST_PercentageById(id);
                if (GST_Percentage is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GST_Percentage object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"GST_Percentage with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                GST_Percentage.IsActive = false;
                string result = await _repository.GST_PercentageRepository.UpdateGST_Percentage(GST_Percentage);
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
                _logger.LogError($"Something went wrong inside DeactivatedGST_Percentage action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
