using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Migrations;
using Tips.Production.Api.Repository;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShopOrderConfirmationController : ControllerBase
    {
        private IShopOrderConfirmationRepository _shopOrderConfirmationRepository;
       // private IShopOrderRepository _shopOrderRepo;

        private ILoggerManager _logger;
        private IMapper _mapper;

        public ShopOrderConfirmationController(IShopOrderConfirmationRepository shopOrderConfirmationRepository,
            ILoggerManager logger, IMapper mapper) // IShopOrderRepository shopOrderRepository)
        {
            _logger = logger;
            _shopOrderConfirmationRepository = shopOrderConfirmationRepository;
            //_shopOrderRepo = shopOrderRepository;
            _mapper = mapper;
        }

       [HttpGet]
        public async Task<IActionResult> GetAllShopOrderConfirmation()
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationDto>>();

            try
            {
                var getAllShopOrderConfirmations = await _shopOrderConfirmationRepository.GetAllShopOrderConfirmation();
                _logger.LogInfo("Returned all ShopOrderConfirmationdetails");
                var result = _mapper.Map<IEnumerable<ShopOrderConfirmationDto>>(getAllShopOrderConfirmations);
                serviceResponse.Data = result;
                serviceResponse.Message = "ShopOrderConfirmation Successfully Returned";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShopOrderConfirmationById(int id)
        {
            ServiceResponse<ShopOrderConfirmationDto> serviceResponse = new ServiceResponse<ShopOrderConfirmationDto>();

            try
            {
                var getShopOrderConfirmation = await _shopOrderConfirmationRepository.GetShopOrderConfirmationById(id);
                if (getShopOrderConfirmation == null)
                {
                    _logger.LogError($"ShopOrderConfirmationdetails with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderConfirmationdetails with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrderConfirmationdetails with id: {id}");
                    var result = _mapper.Map<ShopOrderConfirmationDto>(getShopOrderConfirmation);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderConfirmationById Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderConfirmationById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateShopOrderConfirmation([FromBody] ShopOrderConfirmationPostDto shopOrderConfirmationPostDto)
        {
            ServiceResponse<ShopOrderConfirmationDto> serviceResponse = new ServiceResponse<ShopOrderConfirmationDto>();

            try
            {
                if (shopOrderConfirmationPostDto == null)
                {
                    _logger.LogError("ShopOrderConfirmationdetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShopOrderConfirmationdetails object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ShopOrderConfirmationdetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
 
                var shopOrderConfirmation = _mapper.Map<ShopOrderConfirmation>(shopOrderConfirmationPostDto);
                _shopOrderConfirmationRepository.CreateShopOrderConfirmation(shopOrderConfirmation);
                _shopOrderConfirmationRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrderConfirmation Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetShopOrderById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateShopOrderConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShopOrderConfirmation(int id, [FromBody] ShopOrderConfirmationDto shopOrderConfirmationUpdateDto)
        {
            ServiceResponse<ShopOrderConfirmationDto> serviceResponse = new ServiceResponse<ShopOrderConfirmationDto>();

            try
            {
                if (shopOrderConfirmationUpdateDto is null)
                {
                    _logger.LogError("Update ShopOrderConfirmationdetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ShopOrderConfirmationdetails object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ShopOrderConfirmationdetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var updateShopOrderConfirmation = await _shopOrderConfirmationRepository.GetShopOrderConfirmationById(id);
                if (updateShopOrderConfirmation is null)
                {
                    _logger.LogError($"ShopOrderConfirmationdetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var shopOrderConfirmation = _mapper.Map(shopOrderConfirmationUpdateDto, updateShopOrderConfirmation);

                string result = await _shopOrderConfirmationRepository.UpdateShopOrderConfirmation(shopOrderConfirmation);
                _logger.LogInfo(result);
                _shopOrderConfirmationRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrderConfirmation Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateShopOrderConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShopOrderConfirmation(int id)
        {
            ServiceResponse<ShopOrderConfirmationDto> serviceResponse = new ServiceResponse<ShopOrderConfirmationDto>();

            try
            {
                var deleteShopOrderConfirmation = await _shopOrderConfirmationRepository.GetShopOrderConfirmationById(id);
                if (deleteShopOrderConfirmation == null)
                {
                    _logger.LogError($"ShopOrderConfirmation with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderConfirmation with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                deleteShopOrderConfirmation.IsDeleted = true;
                string result = await _shopOrderConfirmationRepository.UpdateShopOrderConfirmation(deleteShopOrderConfirmation);
                _shopOrderConfirmationRepository.SaveAsync();
                serviceResponse.Message = "ShopOrderConfirmation Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteShopOrderConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{shopOrderNo}")]
        public async Task<IActionResult> GetAllShopOrderConfirmationByShopOrderNo(string shopOrderNo)
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationDto>>();

            try
            {
                var getAllShopOrderConfirmationByShopOrderNo = await _shopOrderConfirmationRepository.GetAllShopOrderConfirmationByShopOrderNo(shopOrderNo);
                if (getAllShopOrderConfirmationByShopOrderNo == null)
                {
                    _logger.LogError($"ShopOrderConfirmationdetails with ShopOrderNo: {shopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderConfirmationdetails with ShopOrderNo hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrderConfirmationdetails with ShopOrderNo: {shopOrderNo}");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationDto>>(getAllShopOrderConfirmationByShopOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderConfirmationByShopOrderNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetAllShopOrderConfirmationByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{shopOrderNo}")]
        public async Task<IActionResult> GetOpenDataForOqcByShopOrderNo(string shopOrderNo)
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationDto>>();

            try
            {
                var getOpenDataForOqcByShopOrderNo = await _shopOrderConfirmationRepository.GetOpenDataForOqcByShopOrderNo(shopOrderNo);
                if (getOpenDataForOqcByShopOrderNo == null)
                {
                    _logger.LogError($"Oqc with shopOrderNo: {shopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Oqc with shopOrderNo hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                   // shopOrderConfirmationList.IsOQCDone = False;
                    _logger.LogInfo($"Returned Oqc with shopOrderNo: {shopOrderNo}");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationDto>>(getOpenDataForOqcByShopOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "OpenDataForOqcByShopOrderNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOpenDataForOqcByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        

    }

}
