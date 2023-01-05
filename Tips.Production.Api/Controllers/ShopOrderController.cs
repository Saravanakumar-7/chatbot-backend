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
                var getAllShopOrders = await _shopOrderRepository.GetAllShopOrders();
                _logger.LogInfo("Returned all ShopOrders");
                var result = _mapper.Map<IEnumerable<ShopOrderDto>>(getAllShopOrders);
                serviceResponse.Data = result;
                serviceResponse.Message = "ShopOrder Successfully Returned";
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
                var getShopOrder = await _shopOrderRepository.GetShopOrderById(id);
                if (getShopOrder == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {id}");
                    var result = _mapper.Map<ShopOrderDto>(getShopOrder);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderById Successfully Returned";
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
        public IActionResult CreateShopOrder([FromBody] ShopOrderPostDto shopOrderPostDto)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                if (shopOrderPostDto == null)
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

                var shopOrder = _mapper.Map<ShopOrder>(shopOrderPostDto);
                _shopOrderRepository.CreateShopOrder(shopOrder);
                _shopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetShopOrderById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShopOrder(int id, [FromBody] ShopOrderDto shopOrderUpdateDto)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                if (shopOrderUpdateDto is null)
                {
                    _logger.LogError("Update ShopOrder object sent from client is null.");
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

                var updateShopOrder = await _shopOrderRepository.GetShopOrderById(id);
                if (updateShopOrder is null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var shopOrder = _mapper.Map(shopOrderUpdateDto, updateShopOrder);

                string result = await _shopOrderRepository.UpdateShopOrder(shopOrder);
                _logger.LogInfo(result);
                _shopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrder Updated Successfully";
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
                var deleteShopOrder = await _shopOrderRepository.GetShopOrderById(id);
                if (deleteShopOrder == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                deleteShopOrder.IsDeleted = true;
                string result = await _shopOrderRepository.UpdateShopOrder(deleteShopOrder);
                _shopOrderRepository.SaveAsync();
                serviceResponse.Message = "ShopOrder Deleted Successfully";
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
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var getShopOrderBySalesOrderNo = await _shopOrderRepository.GetShopOrderBySalesOrderNo(salesOrderNo);
                if (getShopOrderBySalesOrderNo == null)
                {
                    _logger.LogError($"ShopOrder with salesOrderNo: {salesOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with salesOrderNo hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with salesOrderNo: {salesOrderNo}");
                    var result = _mapper.Map<ShopOrderDto>(getShopOrderBySalesOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderBySalesOrderNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderBySalesOrderNo action: {ex.Message}");
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
                var getShopOrderByShopOrderNo = await _shopOrderRepository.GetShopOrderByShopOrderNo(shopOrderNo);
                if (getShopOrderByShopOrderNo == null)
                {
                    _logger.LogError($"ShopOrder with shopOrderNo: {shopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with shopOrderNo hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with shopOrderNo: {shopOrderNo}");
                    var result = _mapper.Map<ShopOrderDto>(getShopOrderByShopOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderByShopOrderNo Successfully Returned";
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
                var getAllOpenShopOrder = await _shopOrderRepository.GetAllOpenShopOrders();
                _logger.LogInfo("Returned all ShopOrders");
                var result = _mapper.Map<IEnumerable<ShopOrderDto>>(getAllOpenShopOrder);
                serviceResponse.Data = result;
                serviceResponse.Message = "ShopOrders Successfully Returned";
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
                var getShortCloseShopOrder = await _shopOrderRepository.GetShopOrderById(id);
                if (getShortCloseShopOrder == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                getShortCloseShopOrder.IsShortClosed = true;
                getShortCloseShopOrder.ShorClosedBy = "Admin";
                getShortCloseShopOrder.ShortClosedOn = DateTime.Now;
                string result = await _shopOrderRepository.UpdateShopOrder(getShortCloseShopOrder);
                _shopOrderRepository.SaveAsync();
                serviceResponse.Message = "ShopOrder have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ShortCloseShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
