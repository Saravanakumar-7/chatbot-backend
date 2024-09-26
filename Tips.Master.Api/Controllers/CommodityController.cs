using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CommodityController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public CommodityController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<CommodityController>
        [HttpGet]
        public async Task<IActionResult> GetAllCommodity([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CommodityDto>> serviceResponse = new ServiceResponse<IEnumerable<CommodityDto>>();

            try
            {
                var GetallCommodity = await _repository.CommodityRepository.GetAllCommodity(searchParams);

                _logger.LogInfo("Returned all Commodity");
                var result = _mapper.Map<IEnumerable<CommodityDto>>(GetallCommodity);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Commodity Successfully";
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

        [HttpGet]
        public async Task<IActionResult> GetAllActiveCommoditys([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<CommodityDto>> serviceResponse = new ServiceResponse<IEnumerable<CommodityDto>>();

            try
            {
                var AllActiveCommodity = await _repository.CommodityRepository.GetAllActiveCommodity(searchParams);
                _logger.LogInfo("Returned all Commodity");
                var result = _mapper.Map<IEnumerable<CommodityDto>>(AllActiveCommodity);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Commodity successfully";
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

        // GET api/<CommodityController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommodityById(int id)
        {
            ServiceResponse<CommodityDto> serviceResponse = new ServiceResponse<CommodityDto>();

            try
            {
                var CommoditybyId = await _repository.CommodityRepository.GetCommodityById(id);
                if (CommoditybyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Commodity with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<CommodityDto>(CommoditybyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Commodity with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCommodityById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500,serviceResponse);
            }
        }

        // POST api/<CommodityController>
        [HttpPost]
        public IActionResult CreateCommodity([FromBody] CommodityDtoPost commodityDtoPost)
        {
            ServiceResponse<CommodityDto> serviceResponse = new ServiceResponse<CommodityDto>();

            try
            {
                if (commodityDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Commodity object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Commodity object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Commodity object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Commodity object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var CommodityCreate = _mapper.Map<Commodity>(commodityDtoPost);
                _repository.CommodityRepository.CreateCommodity(CommodityCreate);
                _repository.SaveAsync(); serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Created("GetCommodityById",serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<CommodityController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCommodity(int id, [FromBody] CommodityDtoUpdate commodityDtoUpdate)
        {
            ServiceResponse<CommodityDto> serviceResponse = new ServiceResponse<CommodityDto>();

            try
            {
                if (commodityDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Commodity object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Commodity object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Commodity object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Commodity object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var CommodityUpdate = await _repository.CommodityRepository.GetCommodityById(id);
                if (CommodityUpdate is null)
                {
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Commodity with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(commodityDtoUpdate, CommodityUpdate);
                string result = await _repository.CommodityRepository.UpdateCommodity(CommodityUpdate);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside UpdateCommodity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<CommodityController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommodity(int id)
        {
            ServiceResponse<CommodityDto> serviceResponse = new ServiceResponse<CommodityDto>();

            try
            {
                var CommodityDelete = await _repository.CommodityRepository.GetCommodityById(id);
                if (CommodityDelete == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Commodity object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.CommodityRepository.DeleteCommodity(CommodityDelete);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCommodity(int id)
        {
            ServiceResponse<CommodityDto> serviceResponse = new ServiceResponse<CommodityDto>();

            try
            {
                var CommodityActivate = await _repository.CommodityRepository.GetCommodityById(id);
                if (CommodityActivate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "purchasegroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                CommodityActivate.IsActive = true;
                string result = await _repository.CommodityRepository.UpdateCommodity(CommodityActivate);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateCommodity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCommodity(int id)
        {
            ServiceResponse<CommodityDto> serviceResponse = new ServiceResponse<CommodityDto>();

            try
            {
                var CommodityDeactivate = await _repository.CommodityRepository.GetCommodityById(id);
                if (CommodityDeactivate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "purchasegroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                CommodityDeactivate.IsActive = false;
                string result = await _repository.CommodityRepository.UpdateCommodity(CommodityDeactivate);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateCommodity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
