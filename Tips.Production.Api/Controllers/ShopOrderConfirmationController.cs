using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
//using Tips.Production.Api.Migrations;
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
                var shopOrderConfirmationList = await _shopOrderConfirmationRepository.GetAllShopOrderConfirmation();
                _logger.LogInfo("Returned all ShopOrderConfirmationdetails()s");
                var result = _mapper.Map<IEnumerable<ShopOrderConfirmationDto>>(shopOrderConfirmationList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
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
                var shopOrderConfirmationList = await _shopOrderConfirmationRepository.GetShopOrderConfirmationById(id);
                if (shopOrderConfirmationList == null)
                {
                    _logger.LogError($"ShopOrderConfirmationdetails with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderConfirmationdetails with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrderConfirmationdetails with id: {id}");
                    var result = _mapper.Map<ShopOrderConfirmationDto>(shopOrderConfirmationList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
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
        public IActionResult CreateShopOrderConfirmation([FromBody] ShopOrderConfirmationDtoPost shopOrderConfirmationDtoPost)
        {
            ServiceResponse<ShopOrderConfirmationDto> serviceResponse = new ServiceResponse<ShopOrderConfirmationDto>();

            try
            {
                if (shopOrderConfirmationDtoPost == null)
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
 

                //var shopOrderDetails = _shopOrderRepo.GetShopOrderShopOrderNo(shopOrderConfirmationDtoPost.ShopOrderNo);
                
                var shopOrderConfirmationList = _mapper.Map<ShopOrderConfirmation>(shopOrderConfirmationDtoPost);
                //shopOrderConfirmationList.ShopOrderId = shopOrderDetails.Id;
                _shopOrderConfirmationRepository.CreateShopOrderConfirmation(shopOrderConfirmationList);
                _shopOrderConfirmationRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetShopOrderById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShopOrderConfirmation(int id, [FromBody] ShopOrderConfirmationDto ShopOrderConfirmationDtoUpdate)
        {
            ServiceResponse<ShopOrderConfirmationDto> serviceResponse = new ServiceResponse<ShopOrderConfirmationDto>();

            try
            {
                if (ShopOrderConfirmationDtoUpdate is null)
                {
                    _logger.LogError("ShopOrderConfirmationdetails object sent from client is null.");
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

                var shopOrderConfirmationList = await _shopOrderConfirmationRepository.GetShopOrderConfirmationById(id);
                if (shopOrderConfirmationList is null)
                {
                    _logger.LogError($"ShopOrderConfirmationdetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var shopOrderEntity = _mapper.Map(ShopOrderConfirmationDtoUpdate, shopOrderConfirmationList);

                string result = await _shopOrderConfirmationRepository.UpdateShopOrderConfirmation(shopOrderEntity);
                _logger.LogInfo(result);
                _shopOrderConfirmationRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
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
                var shopOrderConfirmations = await _shopOrderConfirmationRepository.GetShopOrderConfirmationById(id);
                if (shopOrderConfirmations == null)
                {
                    _logger.LogError($"Confirmation with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Confirmation with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shopOrderConfirmations.IsDeleted = true;
                string result = await _shopOrderConfirmationRepository.UpdateShopOrderConfirmation(shopOrderConfirmations);
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
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
                var shopOrderConfirmationList = await _shopOrderConfirmationRepository.GetAllShopOrderConfirmationByShopOrderNo(shopOrderNo);
                if (shopOrderConfirmationList == null)
                {
                    _logger.LogError($"ShopOrderConfirmationdetails with id: {shopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderConfirmationdetails with id: {shopOrderNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrderConfirmationdetails with id: {shopOrderNo}");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationDto>>(shopOrderConfirmationList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
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

        [HttpGet("{shopOrderNo}")]
        public async Task<IActionResult> GetOpenDataForOqcByShopOrderNo(string shopOrderNo)
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationDto>>();

            try
            {
                var shopOrderConfirmationList = await _shopOrderConfirmationRepository.GetOpenDataForOqcByShopOrderNo(shopOrderNo);
                if (shopOrderConfirmationList == null)
                {
                    _logger.LogError($"ShopOrderConfirmationdetails with id: {shopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderConfirmationdetails with id: {shopOrderNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                   // shopOrderConfirmationList.IsOQCDone = False;
                    _logger.LogInfo($"Returned ShopOrderConfirmationdetails with id: {shopOrderNo}");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationDto>>(shopOrderConfirmationList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
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

        

    }

}
