using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
        public async Task<IActionResult> GetAllCommodity()
        {
            ServiceResponse<IEnumerable<CommodityDto>> serviceResponse = new ServiceResponse<IEnumerable<CommodityDto>>();

            try
            {
                var CommodityList = await _repository.CommodityRepository.GetAllCommodity();
                _logger.LogInfo("Returned all Commodity");
                var result = _mapper.Map<IEnumerable<CommodityDto>>(CommodityList);
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
        public async Task<IActionResult> GetAllActiveCommoditys()
        {
            ServiceResponse<IEnumerable<CommodityDto>> serviceResponse = new ServiceResponse<IEnumerable<CommodityDto>>();

            try
            {
                var Commodity = await _repository.CommodityRepository.GetAllActiveCommodity();
                _logger.LogInfo("Returned all Commodity");
                var result = _mapper.Map<IEnumerable<CommodityDto>>(Commodity);
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
                var Commodity = await _repository.CommodityRepository.GetCommodityById(id);
                if (Commodity == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseGroup with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<CommodityDto>(Commodity);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
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
                var CommodityEntity = _mapper.Map<Commodity>(commodityDtoPost);
                _repository.CommodityRepository.CreateCommodity(CommodityEntity);
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
                var CommodityEntity = await _repository.CommodityRepository.GetCommodityById(id);
                if (CommodityEntity is null)
                {
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update BasicOfApproval with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(commodityDtoUpdate, CommodityEntity);
                string result = await _repository.CommodityRepository.UpdateCommodity(CommodityEntity);
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
                var Commodity = await _repository.CommodityRepository.GetCommodityById(id);
                if (Commodity == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Purchasegroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.CommodityRepository.DeleteCommodity(Commodity);
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
                var Commodity = await _repository.CommodityRepository.GetCommodityById(id);
                if (Commodity is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "purchasegroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Commodity.ActiveStatus = true;
                string result = await _repository.CommodityRepository.UpdateCommodity(Commodity);
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
                var Commodity = await _repository.CommodityRepository.GetCommodityById(id);
                if (Commodity is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "purchasegroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Commodity with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Commodity.ActiveStatus = false;
                string result = await _repository.CommodityRepository.UpdateCommodity(Commodity);
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
