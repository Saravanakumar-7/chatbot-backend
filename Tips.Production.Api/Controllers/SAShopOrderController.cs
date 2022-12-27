using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SAShopOrderController : ControllerBase
    {
        private ISAShopOrderRepository _sashopOrderRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public SAShopOrderController(ISAShopOrderRepository sashopOrderRepository, ILoggerManager logger, IMapper mapper)
        {
            _logger = logger;
            _sashopOrderRepository = sashopOrderRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSAShopOrders()
        {
            ServiceResponse<IEnumerable<SAShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<SAShopOrderDto>>();

            try
            {
                var SashopOrderList = await _sashopOrderRepository.GetAllSAShopOrders();
                _logger.LogInfo("Returned all SAShopOrders()s");
                var result = _mapper.Map<IEnumerable<SAShopOrderDto>>(SashopOrderList);
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
        public async Task<IActionResult> GetSAShopOrderById(int id)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                var SAshopOrders = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (SAshopOrders == null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned SAShopOrder with id: {id}");
                    var result = _mapper.Map<SAShopOrderDto>(SAshopOrders);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSAshopOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateSAShopOrder([FromBody] SAShopOrderDtoPost sashopOrderDtoPost)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                if (sashopOrderDtoPost == null)
                {
                    _logger.LogError("SAShopOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SAShopOrder object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid SAShopOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var SAshopOrderEntity = _mapper.Map<SAShopOrder>(sashopOrderDtoPost);

                _sashopOrderRepository.CreateSAShopOrder(SAshopOrderEntity);
                _sashopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetSAShopOrderById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateSAShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSAShopOrder(int id, [FromBody] SAShopOrderDto SAShopOrderDtoUpdate)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                if (SAShopOrderDtoUpdate is null)
                {
                    _logger.LogError("SAShopOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update SAShopOrder object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid SAShopOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var SAshopOrders = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (SAshopOrders is null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var SAshopOrderEntity = _mapper.Map(SAShopOrderDtoUpdate, SAshopOrders);

                string result = await _sashopOrderRepository.UpdateSAShopOrder(SAshopOrderEntity);
                _logger.LogInfo(result);
                _sashopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateSAShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSAShopOrder(int id)
        {
            ServiceResponse<ShopOrderDto> serviceResponse = new ServiceResponse<ShopOrderDto>();

            try
            {
                var shopOrders = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (shopOrders == null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shopOrders.IsDeleted = true;
                string result = await _sashopOrderRepository.UpdateSAShopOrder(shopOrders);
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteSAShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        
        [HttpGet("salesOrderNo")]
        public async Task<IActionResult> GetSAShopOrderBySalesOrderNo(string salesOrderNo)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                var sashopOrders = await _sashopOrderRepository.GetSAShopOrderBySalesOrderNo(salesOrderNo);
                if (sashopOrders == null)
                {
                    _logger.LogError($"SAShopOrder with id: {salesOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id: {salesOrderNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned SAShopOrder with id: {salesOrderNo}");
                    var result = _mapper.Map<SAShopOrderDto>(sashopOrders);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSAshopOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("SAshopOrderNo")]
        public async Task<IActionResult> GetSAShopOrderByShopOrderNo(string SAshopOrderNo)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                var sashopOrders = await _sashopOrderRepository.GetSAShopOrderShopOrderNo(SAshopOrderNo);
                if (sashopOrders == null)
                {
                    _logger.LogError($"SAShopOrder with id: {SAshopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id: {SAshopOrderNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned SAShopOrder with id: {SAshopOrderNo}");
                    var result = _mapper.Map<SAShopOrderDto>(sashopOrders);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetshopOrderBySAShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOpenSAShopOrders()
        {
            ServiceResponse<IEnumerable<SAShopOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<SAShopOrderDto>>();

            try
            {
                var sashopOrderList = await _sashopOrderRepository.GetAllOpenSAShopOrders();
                _logger.LogInfo("Returned all ShopOrders()s");
                var result = _mapper.Map<IEnumerable<SAShopOrderDto>>(sashopOrderList);
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
        public async Task<IActionResult> ShortCloseSAShopOrder(int id)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                var sashopOrders = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (sashopOrders == null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                sashopOrders.IsShortClosed = true;
                sashopOrders.ShorClosedBy = "Admin";
                sashopOrders.ShortClosedOn = DateTime.Now;
                string result = await _sashopOrderRepository.UpdateSAShopOrder(sashopOrders);
                serviceResponse.Data = null;
                serviceResponse.Message = "SAShopOrder have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteSAShopOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
