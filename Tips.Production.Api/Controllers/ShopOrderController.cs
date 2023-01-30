using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;

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
                var shopOrderDetails = await _shopOrderRepository.GetAllShopOrders(pagingParameter);
                _logger.LogInfo("Returned all ShopOrders()s");
                var result = _mapper.Map<IEnumerable<ShopOrderDto>>(shopOrderDetails);
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
                serviceResponse.Message = "Internal server error";
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
                var shopOrderDetailById = await _shopOrderRepository.GetShopOrderById(id);
                if (shopOrderDetailById == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {id}");
                    ShopOrderDto shopOrderDto =  _mapper.Map<ShopOrderDto>(shopOrderDetailById);
                     
                    List<ShopOrderItemDto> shopOrderItemDtos= new List<ShopOrderItemDto>();
                    if (shopOrderDetailById.ShopOrderItems != null)
                    {
                        foreach (var shopOrderItemdetail in shopOrderDetailById.ShopOrderItems)
                        {
                            ShopOrderItemDto shopOrderItemDto = _mapper.Map<ShopOrderItemDto>(shopOrderItemdetail);
                            shopOrderItemDtos.Add(shopOrderItemDto);
                        }
                    }
                    shopOrderDto.ShopOrderItems = shopOrderItemDtos;
                    serviceResponse.Data = shopOrderDto;
                    serviceResponse.Message = "GetShopOrderById Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateShopOrder([FromBody] ShopOrderPostDto shopOrderPostDto)
        {
            ServiceResponse<ShopOrderPostDto> serviceResponse = new ServiceResponse<ShopOrderPostDto>();

            try
            {
                if (shopOrderPostDto == null)
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
                    serviceResponse.Message = "Invalid ShopOrder object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var shopOrder = _mapper.Map<ShopOrder>(shopOrderPostDto);
                var shopOrderDto = shopOrderPostDto.ShopOrderItems;
                var ShoporderItemList = new List<ShopOrderItem>();
                if (shopOrderDto != null)
                {
                    for (int i = 0; i < shopOrderDto.Count; i++)
                    {
                        ShopOrderItem shopOrderItemDetail = _mapper.Map<ShopOrderItem>(shopOrderDto[i]);
                        ShoporderItemList.Add(shopOrderItemDetail);

                    }
                }
                shopOrder.ShopOrderItems= ShoporderItemList;               
                await _shopOrderRepository.CreateShopOrder(shopOrder);

                _shopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ShopOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetShopOrderItemById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShopOrder(int id, [FromBody] ShopOrderUpdateDto ShopOrderDtoUpdate)
        {
            ServiceResponse<ShopOrderUpdateDto> serviceResponse = new ServiceResponse<ShopOrderUpdateDto>();

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
                    serviceResponse.Message = "Invalid Update ShopOrder object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var shopOrderDetailById = await _shopOrderRepository.GetShopOrderById(id);
                if (shopOrderDetailById is null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update ShopOrder with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var shopOrders = _mapper.Map<ShopOrder>(shopOrderDetailById);
                var shopOrderItemDto = ShopOrderDtoUpdate.ShopOrderItems;
                var shopOrderItemList = new List<ShopOrderItem>();
                if(shopOrderItemDto != null)
                for (int i = 0; i < shopOrderItemDto.Count; i++)
                {
                    ShopOrderItem shoporderItemDetail = _mapper.Map<ShopOrderItem>(shopOrderItemDto[i]);
                    shopOrderItemList.Add(shoporderItemDetail);

                }
                shopOrders.ShopOrderItems = shopOrderItemList;
                var updateShopOrder = _mapper.Map(ShopOrderDtoUpdate, shopOrderDetailById);
                string result = await _shopOrderRepository.UpdateShopOrder(updateShopOrder);
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
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShopOrder(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrderDetailById = await _shopOrderRepository.GetShopOrderById(id);
                if (shopOrderDetailById == null)
                {
                    _logger.LogError($"ShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shopOrderDetailById.IsDeleted = true;
                string result = await _shopOrderRepository.UpdateShopOrder(shopOrderDetailById);
                _shopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
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
                var shopOrderBySalesOrderNo = await _shopOrderRepository.GetShopOrderBySalesOrderNo(salesOrderNo);
                if (shopOrderBySalesOrderNo == null)
                {
                    _logger.LogError($"ShopOrder with id: {salesOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with salesOrderNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {salesOrderNo}");
                    var result = _mapper.Map<ShopOrderDto>(shopOrderBySalesOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderBySalesOrderNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("shopOrderNo")]
        public async Task<IActionResult> GetShopOrderByShopOrderNo(string shopOrderNo)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrderByShopOrderNo = await _shopOrderRepository.GetShopOrderBySalesOrderNo(shopOrderNo);
                if (shopOrderByShopOrderNo == null)
                {
                    _logger.LogError($"ShopOrder with id: {shopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with shopOrderNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {shopOrderNo}");
                    var result = _mapper.Map<ShopOrderDto>(shopOrderByShopOrderNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderByShopOrderNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetShopOrderByItemType(string itemType)
        {
            ServiceResponse<IEnumerable<ListOfShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfShopOrderDto>>();

            try
            {
                var shopOrderByItemType = await _shopOrderRepository.GetShopOrderByItemType(itemType);
                if (shopOrderByItemType == null)
                {
                    _logger.LogError($"ShopOrder with id: {itemType}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with itemType hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {itemType}");
                    var result = _mapper.Map<IEnumerable<ListOfShopOrderDto>>(shopOrderByItemType);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderByItemType Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderByItemType action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetShopOrderByFGNo(string fGNumber)
        {
            ServiceResponse<IEnumerable<ListOfShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfShopOrderDto>>();

            try
            {
                var shopOrderByFGNo = await _shopOrderRepository.GetShopOrderByFGNo(fGNumber);
                if (shopOrderByFGNo == null)
                {
                    _logger.LogError($"ShopOrder with id: {fGNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrder with fGNumber hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {fGNumber}");
                    var result = _mapper.Map<IEnumerable<ListOfShopOrderDto>>(shopOrderByFGNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderByFGNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderByFGNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShopOrderByFGNoAndSANo(string fGNumber, string sANumber)
        {
            ServiceResponse<IEnumerable<ListOfShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfShopOrderDto>>();

            try
            {
                var shopOrderByFGNoAndSANo = await _shopOrderRepository.GetShopOrderByFGNoAndSANo(fGNumber, sANumber);
                if (shopOrderByFGNoAndSANo == null)
                {
                    _logger.LogError($"ShopOrder with id: {fGNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"shopOrderByFGNoAndSANo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ShopOrder with id: {fGNumber}");
                    var result = _mapper.Map<IEnumerable<ListOfShopOrderDto>>(shopOrderByFGNoAndSANo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderByFGNoAndSANo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetShopOrderByFGNoAndSANo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOpenShopOrders()
        {
            ServiceResponse<IEnumerable<ShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderDto>>();

            try
            {
                var openShopOrderDetials = await _shopOrderRepository.GetAllOpenShopOrders();
                _logger.LogInfo("Returned all ShopOrders");
                var result = _mapper.Map<IEnumerable<ShopOrderDto>>(openShopOrderDetials);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenShopOrderDetails Successfully ";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> ShortCloseShopOrder(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shortCloseShopOrderById = await _shopOrderRepository.GetShopOrderById(id);
                if (shortCloseShopOrderById == null)
                {
                    _logger.LogError($"ShortCloseShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShortCloseShopOrder with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shortCloseShopOrderById.IsShortClosed = true;
                shortCloseShopOrderById.ShortClosedBy = "Admin";
                shortCloseShopOrderById.ShortClosedOn = DateTime.Now;
                string result = await _shopOrderRepository.UpdateShopOrder(shortCloseShopOrderById);
                _shopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
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
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
