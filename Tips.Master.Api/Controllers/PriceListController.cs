using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PriceListController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public PriceListController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<PriceListController>
        [HttpGet]
        public async Task<IActionResult> GetAllPriceLists([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<PriceListDto>> serviceResponse = new ServiceResponse<IEnumerable<PriceListDto>>();
            try
            {

                var PriceList = await _repository.PriceListRepository.GetAllPriceLists(searchParams);
                _logger.LogInfo("Returned all PriceLists");
                var result = _mapper.Map<IEnumerable<PriceListDto>>(PriceList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PriceLists Successfully";
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
        public async Task<IActionResult> GetLatestPriceLists()
        {
            ServiceResponse<PriceListDto> serviceResponse = new ServiceResponse<PriceListDto>();
            try
            {
                var PriceList = await _repository.PriceListRepository.GetLatestPriceLists();
                _logger.LogInfo("Returned Latest PriceLists");
                var result = _mapper.Map<PriceListDto>(PriceList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned Latest PriceLists Successfully";
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

        public async Task<IActionResult> GetAllActivePriceLists([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<PriceListDto>> serviceResponse = new ServiceResponse<IEnumerable<PriceListDto>>();

            try
            {
                var PriceList = await _repository.PriceListRepository.GetAllActivePriceLists(searchParams);
                _logger.LogInfo("Returned all PriceLists");
                var result = _mapper.Map<IEnumerable<PriceListDto>>(PriceList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active PriceLists Successfully";
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

        // GET api/<PriceListController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPriceListById(int id)
        {
            ServiceResponse<PriceListDto> serviceResponse = new ServiceResponse<PriceListDto>();

            try
            {
                var PriceList = await _repository.PriceListRepository.GetPriceListById(id);
                if (PriceList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PriceList with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PriceList with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned PriceList with id: {id}");
                    var result = _mapper.Map<PriceListDto>(PriceList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned PriceList with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPriceListById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<PriceListController>
        [HttpPost]
        public IActionResult CreatePriceList([FromBody] PriceListDtoPost priceListDtoPost)
        {
            ServiceResponse<PriceListDto> serviceResponse = new ServiceResponse<PriceListDto>();

            try
            {
                if (priceListDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PriceList object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PriceList object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PriceList object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PriceList object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var PriceList = _mapper.Map<PriceList>(priceListDtoPost);
                _repository.PriceListRepository.CreatePriceList(PriceList);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetPriceListById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreatePriceList action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<PriceListController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePriceList(int id, [FromBody] PriceListDtoUpdate priceListDtoUpdate)
        {
            ServiceResponse<PriceListDto> serviceResponse = new ServiceResponse<PriceListDto>();

            try
            {
                if (priceListDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update PriceList object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update PriceList object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update PriceList object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update PriceList object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var PriceList = await _repository.PriceListRepository.GetPriceListById(id);
                if (PriceList is null)
                {
                    _logger.LogError($"Update PriceList with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update PriceList with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(priceListDtoUpdate, PriceList);
                string result = await _repository.PriceListRepository.UpdatePriceList(PriceList);
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
                _logger.LogError($"Something went wrong inside UpdatePriceList action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<PriceListController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePriceList(int id)
        {
            ServiceResponse<PriceListDto> serviceResponse = new ServiceResponse<PriceListDto>();

            try
            {
                var PriceList = await _repository.PriceListRepository.GetPriceListById(id);
                if (PriceList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete PriceList object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete PriceList with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.PriceListRepository.DeletePriceList(PriceList);
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
                _logger.LogError($"Something went wrong inside PriceList action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivatePriceList(int id)
        {
            ServiceResponse<PriceListDto> serviceResponse = new ServiceResponse<PriceListDto>();

            try
            {
                var PriceList = await _repository.PriceListRepository.GetPriceListById(id);
                if (PriceList is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PriceList object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PriceList with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                PriceList.IsActive = true;
                string result = await _repository.PriceListRepository.UpdatePriceList(PriceList);
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
                _logger.LogError($"Something went wrong inside ActivatedPriceList action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivatePriceList(int id)
        {
            ServiceResponse<PriceListDto> serviceResponse = new ServiceResponse<PriceListDto>();

            try
            {
                var PriceList = await _repository.PriceListRepository.GetPriceListById(id);
                if (PriceList is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PriceList object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PriceList with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                PriceList.IsActive = false;
                string result = await _repository.PriceListRepository.UpdatePriceList(PriceList);
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
                _logger.LogError($"Something went wrong inside DeactivatePriceList action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
