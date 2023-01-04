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
                var sAShopOrderList = await _sashopOrderRepository.GetAllSAShopOrders();
                _logger.LogInfo("Returned all SAShopOrders()s");
                var result = _mapper.Map<IEnumerable<SAShopOrderDto>>(sAShopOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "SAShopOrder Successfully Returned";
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
                var sAShopOrders = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (sAShopOrders == null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned SAShopOrder with id: {id}");
                    var result = _mapper.Map<SAShopOrderDto>(sAShopOrders);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "SAShopOrderById Successfully Returned";
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
        public IActionResult CreateSAShopOrder([FromBody] SAShopOrderDtoPost sAShopOrderDtoPost)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                if (sAShopOrderDtoPost == null)
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
                var sAShopOrderEntity = _mapper.Map<SAShopOrder>(sAShopOrderDtoPost);

                _sashopOrderRepository.CreateSAShopOrder(sAShopOrderEntity);
                _sashopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "SAShopOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetSAShopOrderById", serviceResponse);
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
        public async Task<IActionResult> UpdateSAShopOrder(int id, [FromBody] SAShopOrderDto sAShopOrderDtoUpdate)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                if (sAShopOrderDtoUpdate is null)
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

                var sAShopOrders = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (sAShopOrders is null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var sAShopOrderEntity = _mapper.Map(sAShopOrderDtoUpdate, sAShopOrders);

                string result = await _sashopOrderRepository.UpdateSAShopOrder(sAShopOrderEntity);
                _logger.LogInfo(result);
                _sashopOrderRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "SAShopOrder Updated Successfully";
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
                var sAShopOrders = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (sAShopOrders == null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                sAShopOrders.IsDeleted = true;
                string result = await _sashopOrderRepository.UpdateSAShopOrder(sAShopOrders);
                serviceResponse.Data = null;
                serviceResponse.Message = "SAShopOrder Deleted Successfully";
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
                var sAShopOrders = await _sashopOrderRepository.GetSAShopOrderBySalesOrderNo(salesOrderNo);
                if (sAShopOrders == null)
                {
                    _logger.LogError($"SAShopOrder with id: {salesOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned SAShopOrder with id: {salesOrderNo}");
                    var result = _mapper.Map<SAShopOrderDto>(sAShopOrders);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "SAShopOrderBySalesOrderNumber Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSAShopOrderBySalesOrderNo action: {ex.Message}");
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
                var sAShopOrders = await _sashopOrderRepository.GetSAShopOrderBySAShopOrderNo(SAshopOrderNo);
                if (sAShopOrders == null)
                {
                    _logger.LogError($"SAShopOrder with id: {SAshopOrderNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned SAShopOrder with id: {SAshopOrderNo}");
                    var result = _mapper.Map<SAShopOrderDto>(sAShopOrders);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "SAShopOrderByShopOrderNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSAShopOrderByShopOrderNo action: {ex.Message}");
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
                var sAShopOrderList = await _sashopOrderRepository.GetAllOpenSAShopOrders();
                _logger.LogInfo("Returned all ShopOrders()s");
                var result = _mapper.Map<IEnumerable<SAShopOrderDto>>(sAShopOrderList);
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
        public async Task<IActionResult> ShortCloseSAShopOrder(int id)
        {
            ServiceResponse<SAShopOrderDto> serviceResponse = new ServiceResponse<SAShopOrderDto>();

            try
            {
                var sAShopOrders = await _sashopOrderRepository.GetSAShopOrderById(id);
                if (sAShopOrders == null)
                {
                    _logger.LogError($"SAShopOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAShopOrder with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                sAShopOrders.IsShortClosed = true;
                sAShopOrders.ShorClosedBy = "Admin";
                sAShopOrders.ShortClosedOn = DateTime.Now;
                string result = await _sashopOrderRepository.UpdateSAShopOrder(sAShopOrders);
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
