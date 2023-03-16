using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LightningDesignerController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public LightningDesignerController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllLightningDesigners([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LightningDesignerDto>> serviceResponse = new ServiceResponse<IEnumerable<LightningDesignerDto>>();
            try
            {

                var lightningDesigners = await _repository.LightningDesignerRepository.GetAllLightningDesigners(pagingParameter, searchParams);

                var metadata = new
                {
                    lightningDesigners.TotalCount,
                    lightningDesigners.PageSize,
                    lightningDesigners.CurrentPage,
                    lightningDesigners.HasNext,
                    lightningDesigners.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all lightningDesigners");
                var result = _mapper.Map<IEnumerable<LightningDesignerDto>>(lightningDesigners);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all lightningDesigners  Successfully";
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
        public async Task<IActionResult> GetAllActiveLightningDesigners([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LightningDesignerDto>> serviceResponse = new ServiceResponse<IEnumerable<LightningDesignerDto>>();

            try
            {
                var lightningDesigners = await _repository.LightningDesignerRepository.GetAllActiveLightningDesigners(pagingParameter, searchParams);
                _logger.LogInfo("Returned all lightningDesigners");
                var result = _mapper.Map<IEnumerable<LightningDesignerDto>>(lightningDesigners);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active lightningDesigners Successfully";
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
        public async Task<IActionResult> GetLightningDesignerById(int id)
        {
            ServiceResponse<LightningDesignerDto> serviceResponse = new ServiceResponse<LightningDesignerDto>();

            try
            {
                var lightningDesignersDetail = await _repository.LightningDesignerRepository.GetLightningDesignerById(id);
                if (lightningDesignersDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"lightningDesigners with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"lightningDesigners with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned lightningDesigners with id: {id}");
                    var result = _mapper.Map<LightningDesignerDto>(lightningDesignersDetail);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned lightningDesigners with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetLightningDesignerById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateLightningDesigner([FromBody] LightningDesignerPostDto lightningDesignerPostDto)
        {
            ServiceResponse<LightningDesignerPostDto> serviceResponse = new ServiceResponse<LightningDesignerPostDto>();

            try
            {
                if (lightningDesignerPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "lightningDesigners object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("lightningDesigners object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid lightningDesigners object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid lightningDesigners object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var lightningDesigners = _mapper.Map<LightningDesigner>(lightningDesignerPostDto);
                _repository.LightningDesignerRepository.CreateLightningDesigner(lightningDesigners);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "lightningDesigners Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetPmcContractorById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateLightningDesigner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLightningDesigner(int id, [FromBody] LightningDesignerUpdateDto lightningDesignerUpdateDto)
        {
            ServiceResponse<LightningDesignerUpdateDto> serviceResponse = new ServiceResponse<LightningDesignerUpdateDto>();

            try
            {
                if (lightningDesignerUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update lightningDesigners object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update lightningDesigners object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update lightningDesigners object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update PmcContractor object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var lightningDesignerDetail = await _repository.LightningDesignerRepository.GetLightningDesignerById(id);
                if (lightningDesignerDetail is null)
                {
                    _logger.LogError($"Update lightningDesigners with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update lightningDesigners with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(lightningDesignerUpdateDto, lightningDesignerDetail);
                string result = await _repository.LightningDesignerRepository.UpdateLightningDesigner(lightningDesignerDetail);
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
                _logger.LogError($"Something went wrong inside UpdateLightningDesigner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLightningDesigner(int id)
        {
            ServiceResponse<LightningDesignerDto> serviceResponse = new ServiceResponse<LightningDesignerDto>();

            try
            {
                var lightningDesignerDetail = await _repository.LightningDesignerRepository.GetLightningDesignerById(id);
                if (lightningDesignerDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete lightningDesigners object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete lightningDesigners with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.LightningDesignerRepository.DeleteLightningDesigner(lightningDesignerDetail);
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
                _logger.LogError($"Something went wrong inside DeleteLightningDesigner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateLightningDesigner(int id)
        {
            ServiceResponse<LightningDesignerDto> serviceResponse = new ServiceResponse<LightningDesignerDto>();

            try
            {
                var lightningDesignerDetail = await _repository.LightningDesignerRepository.GetLightningDesignerById(id);
                if (lightningDesignerDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "lightningDesigner object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"lightningDesigner with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                lightningDesignerDetail.IsActive = true;
                string result = await _repository.LightningDesignerRepository.UpdateLightningDesigner(lightningDesignerDetail);
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
                _logger.LogError($"Something went wrong inside ActivateLightningDesigner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateLightningDesigner(int id)
        {
            ServiceResponse<LightningDesignerDto> serviceResponse = new ServiceResponse<LightningDesignerDto>();

            try
            {
                var lightningDesignerDetail = await _repository.LightningDesignerRepository.GetLightningDesignerById(id);
                if (lightningDesignerDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "lightningDesigner object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"lightningDesigner with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                lightningDesignerDetail.IsActive = true;
                string result = await _repository.LightningDesignerRepository.UpdateLightningDesigner(lightningDesignerDetail);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "DeActivated Successfully";
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
                _logger.LogError($"Something went wrong inside DeactivateLightningDesigner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
       
    
        }
    }
}
