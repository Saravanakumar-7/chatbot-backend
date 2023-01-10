using AutoMapper;
using Tips.Production.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities;
using Contracts;
using System.Collections.Generic;
using Entities;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShopOrderController : ControllerBase
    {
        private IShopOrderRepository _shopOrderRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IMaterialIssueRepository _materialIssueRepository;

        public ShopOrderController(IShopOrderRepository shopOrderRepository, IMaterialIssueRepository materialIssueRepository, ILoggerManager logger, IMapper mapper)
        {
            _logger = logger;
            _shopOrderRepository = shopOrderRepository;
            _mapper = mapper;
            _materialIssueRepository = materialIssueRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShopOrders([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderDto>>();

            try
            {
                var getAllShopOrders = await _shopOrderRepository.GetAllShopOrders(pagingParameter);
                _logger.LogInfo("Returned all ShopOrders()s");
                var result = _mapper.Map<IEnumerable<ShopOrderDto>>(getAllShopOrders);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ShopOrders Successfully";
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
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShopOrderById(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var getShopOrderById = await _shopOrderRepository.GetShopOrderById(id);
                if (getShopOrderById == null)
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
                    ShopOrderDto shopOrderDto =  _mapper.Map<ShopOrderDto>(getShopOrderById);
                     
                    List<ShopOrderItemDto> shopOrderItemDtos= new List<ShopOrderItemDto>();

                    foreach(var shopOrderItemdetail in getShopOrderById.ShopOrderItems)
                    {
                        ShopOrderItemDto shopOrderItemDto = _mapper.Map<ShopOrderItemDto>(shopOrderItemdetail);
                        shopOrderItemDtos.Add(shopOrderItemDto);
                    }
                    shopOrderDto.ShopOrderItems = shopOrderItemDtos;
                    serviceResponse.Data = shopOrderDto;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(shopOrderDto);
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
        public async Task<IActionResult> CreateShopOrder([FromBody] ShopOrderDtoPost shopOrderDtoPost)
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
                var createShopOrder = _mapper.Map<ShopOrder>(shopOrderDtoPost);
                var shopOrderDto = shopOrderDtoPost.ShopOrderItems;

                var ShoporderItemList = new List<ShopOrderItem>();

                if (shopOrderDto != null)
                {

                    for (int i = 0; i < shopOrderDto.Count; i++)
                    {
                        ShopOrderItem shopOrderItemDetail = _mapper.Map<ShopOrderItem>(shopOrderDto[i]);
                        ShoporderItemList.Add(shopOrderItemDetail);

                    }
                }
                createShopOrder.ShopOrderItems= ShoporderItemList;               
                await _shopOrderRepository.CreateShopOrder(createShopOrder);

                _shopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetShopOrderById", serviceResponse);
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
        public async Task<IActionResult> UpdateShopOrder(int id, [FromBody] ShopOrderDtoUpdate ShopOrderDtoUpdate)
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
                    _logger.LogError("Invalid MaterialRequest object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update MaterialRequest object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getShopOrderById = await _shopOrderRepository.GetShopOrderById(id);
                if (getShopOrderById is null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update ShopOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateShopOrder = _mapper.Map<ShopOrder>(getShopOrderById);

                var shoporderItemDto = ShopOrderDtoUpdate.ShopOrderItems;

                var shoporderItemList = new List<ShopOrderItem>();
                for (int i = 0; i < shoporderItemDto.Count; i++)
                {
                    ShopOrderItem shoporderItemDetail = _mapper.Map<ShopOrderItem>(shoporderItemDto[i]);
                    shoporderItemList.Add(shoporderItemDetail);

                }

                var updateShoporder = _mapper.Map(ShopOrderDtoUpdate, getShopOrderById);
                updateShoporder.ShopOrderItems = shoporderItemList;
                string result = await _shopOrderRepository.UpdateShopOrder(updateShoporder);
                _shopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Shoporder Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShopOrder(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var getShopOrderById = await _shopOrderRepository.GetShopOrderById(id);
                if (getShopOrderById == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                getShopOrderById.IsDeleted = true;
                string result = await _shopOrderRepository.UpdateShopOrder(getShopOrderById);
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
                return StatusCode(500, serviceResponse);
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
                    var result = _mapper.Map<ShopOrderDto>(getShopOrderBySalesOrderNo);
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
                var shopOrders = await _shopOrderRepository.GetShopOrderBySalesOrderNo(shopOrderNo);
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
                var shopOrderShortCloseById = await _shopOrderRepository.GetShopOrderById(id);
                if (shopOrderShortCloseById == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shopOrderShortCloseById.IsShortClosed = true;
                shopOrderShortCloseById.ShortClosedBy = "Admin";
                shopOrderShortCloseById.ShortClosedOn = DateTime.Now;
                string result = await _shopOrderRepository.UpdateShopOrder(shopOrderShortCloseById);
                _shopOrderRepository.SaveAsync();
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
