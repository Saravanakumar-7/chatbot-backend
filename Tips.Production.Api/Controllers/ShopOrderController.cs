using AutoMapper;
using Tips.Production.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities;
using Contracts;
using System.Collections.Generic;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShopOrderController : ControllerBase
    {
        private IShopOrderRepository _shopOrderRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public ShopOrderController(IShopOrderRepository shopOrderRepository, ILoggerManager logger, IMapper mapper)
        {
            _logger = logger;
            _shopOrderRepository = shopOrderRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShopOrders()
        {
            ServiceResponse<IEnumerable<ShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderDto>>();

            try
            {
                var shopOrderList = await _shopOrderRepository.GetAllShopOrders();
                _logger.LogInfo("Returned all ShopOrders()s");
                var result = _mapper.Map<IEnumerable<ShopOrderDto>>(shopOrderList);
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
        public async Task<IActionResult> GetShopOrderById(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrders = await _shopOrderRepository.GetShopOrderById(id);
                if (shopOrders == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {id}");
                    var result = _mapper.Map<ShopOrderDto>(shopOrders);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateShopOrder([FromBody] ShopOrderDtoPost shopOrderDtoPost)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                if (shopOrderDtoPost == null)
                {
                    _logger.LogError("ShopOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShopOrder object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ShopOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var shopOrderEntity = _mapper.Map<ShopOrder>(shopOrderDtoPost);

                _shopOrderRepository.CreateShopOrder(shopOrderEntity);
                _shopOrderRepository.SaveAsync();
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
        public async Task<IActionResult> UpdateShopOrder(int id, [FromBody] ShopOrderDto ShopOrderDtoUpdate)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                if (ShopOrderDtoUpdate is null)
                {
                    _logger.LogError("ShopOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ShopOrder object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ShopOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var shopOrders = await _shopOrderRepository.GetShopOrderById(id);
                if (shopOrders is null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var shopOrderEntity = _mapper.Map(ShopOrderDtoUpdate, shopOrders);

                string result = await _shopOrderRepository.UpdateShopOrder(shopOrderEntity);
                _logger.LogInfo(result);
                _shopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShopOrder(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrders = await _shopOrderRepository.GetShopOrderById(id);
                if (shopOrders == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                
                 shopOrders.IsDeleted = true;
                string result = await _shopOrderRepository.UpdateShopOrder(shopOrders);
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        private string Delete(ShopOrder shopOrders)
        {
            throw new NotImplementedException();
        }

        [HttpGet("salesOrderNo")]
        public async Task<IActionResult> GetShopOrderBySalesOrderNo(string salesOrderNo)
        {
            ServiceResponse <ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrders = await _shopOrderRepository.GetShopOrderBySalesOrderNo(salesOrderNo);
                if (shopOrders == null)
                {
                    _logger.LogError($"ShopOrder with id: {salesOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id: {salesOrderNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {salesOrderNo}");
                    var result = _mapper.Map<ShopOrderDto>(shopOrders);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
       
        [HttpGet("shopOrderNo")]
        public async Task<IActionResult> GetShopOrderByShopOrderNo(string shopOrderNo)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrders = await _shopOrderRepository.GetShopOrderShopOrderNo(shopOrderNo);
                if (shopOrders == null)
                {
                    _logger.LogError($"ShopOrder with id: {shopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id: {shopOrderNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {shopOrderNo}");
                    var result = _mapper.Map<ShopOrderDto>(shopOrders);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOpenShopOrders()
        {
            ServiceResponse<IEnumerable<ShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderDto>>();

            try
            {
                var shopOrderList = await _shopOrderRepository.GetAllOpenShopOrders();
                _logger.LogInfo("Returned all ShopOrders()s");
                var result = _mapper.Map<IEnumerable<ShopOrderDto>>(shopOrderList);
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
        public async Task<IActionResult> ShortCloseShopOrder(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrders = await _shopOrderRepository.GetShopOrderById(id);
                if (shopOrders == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shopOrders.IsShortClosed = true;
                shopOrders.ShorClosedBy = "Admin";
                shopOrders.ShortClosedOn =  DateTime.Now;
                string result = await _shopOrderRepository.UpdateShopOrder(shopOrders);
                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrder have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
