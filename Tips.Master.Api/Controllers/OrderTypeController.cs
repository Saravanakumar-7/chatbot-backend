using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderTypeController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public OrderTypeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrderType([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<OrderTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<OrderTypeDto>>();

            try
            {
                var orderTypeDetails = await _repository.OrderTypeRepository.GetAllOrderType(searchParams);
                _logger.LogInfo("Returned all OrderType");
                var result = _mapper.Map<IEnumerable<OrderTypeDto>>(orderTypeDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OrderType Successfully";
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
        public async Task<IActionResult> GetAllActiveOrderType([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<OrderTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<OrderTypeDto>>();

            try
            {
                var orderTypeDetails = await _repository.OrderTypeRepository.GetAllActiveOrderType(searchParams);
                _logger.LogInfo("Returned all OrderType");
                var result = _mapper.Map<IEnumerable<OrderTypeDto>>(orderTypeDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active OrderType Successfully";
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

        [HttpPut]
        public async Task<IActionResult> UpdateOrderTypeDefaultValue(int id)
        {
            ServiceResponse<IEnumerable<OrderTypeDto>> serviceResponse = new ServiceResponse<IEnumerable<OrderTypeDto>>();

            try
            {
                var changeOrderType = await _repository.OrderTypeRepository.GetDefaultOrderType(id);
                if (changeOrderType != null)
                {
                    var orderTypeDetails = await _repository.OrderTypeRepository.GetDefaultOrderTypeValue(id);
                    _logger.LogInfo("Returned all OrderType");
                    foreach (var orderType in orderTypeDetails)
                    {
                        await _repository.OrderTypeRepository.UpdateOrderType(orderType);
                        _repository.SaveAsync();

                    }
                     
                    //      var updates = await _repository.OrderTypeRepository.Update(result);
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Default Value in OrderType Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OrderType with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OrderType with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);

                }
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
        public async Task<IActionResult> GetOrderTypeById(int id)
        {
            ServiceResponse<OrderTypeDto> serviceResponse = new ServiceResponse<OrderTypeDto>();

            try
            {
                var orderTypeDetailsById = await _repository.OrderTypeRepository.GetOrderTypeById(id);
                if (orderTypeDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OrderType with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OrderType with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned OrderType with id: {id}");
                    var result = _mapper.Map<OrderTypeDto>(orderTypeDetailsById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned OrderType with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOrderTypeById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public IActionResult CreateOrderType([FromBody] OrderTypePostDto orderTypePostDto)
        {
            ServiceResponse<OrderTypePostDto> serviceResponse = new ServiceResponse<OrderTypePostDto>();

            try
            {
                if (orderTypePostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OrderType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("OrderType object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OrderType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid OrderType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var orderTypeDetails = _mapper.Map<OrderType>(orderTypePostDto);
                orderTypeDetails.IsDefault = false;
                _repository.OrderTypeRepository.CreateOrderType(orderTypeDetails);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created OrderType";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetOrderTypeById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside OrderType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderType(int id, [FromBody] OrderTypeUpdateDto orderTypeUpdateDto)
        {
            ServiceResponse<OrderTypeUpdateDto> serviceResponse = new ServiceResponse<OrderTypeUpdateDto>();

            try
            {
                if (orderTypeUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update OrderType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("OrderType object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update OrderType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update OrderType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var orderTypeDetails = await _repository.OrderTypeRepository.GetOrderTypeById(id);
                if (orderTypeDetails is null)
                {
                    _logger.LogError($"OrderType with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(orderTypeUpdateDto, orderTypeDetails);
                string result = await _repository.OrderTypeRepository.UpdateOrderType(orderTypeDetails);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "OrderType Updated Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateOrderType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderType(int id)
        {
            ServiceResponse<OrderTypeDto> serviceResponse = new ServiceResponse<OrderTypeDto>();

            try
            {
                var orderTypeDetailsById = await _repository.OrderTypeRepository.GetOrderTypeById(id);
                if (orderTypeDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OrderType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"OrderType  with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.OrderTypeRepository.DeleteOrderType(orderTypeDetailsById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "OrderType Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside DeleteOrderType action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOrderType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateOrderType(int id)
        {
            ServiceResponse<OrderTypeDto> serviceResponse = new ServiceResponse<OrderTypeDto>();

            try
            {
                var orderTypeDetailsById = await _repository.OrderTypeRepository.GetOrderTypeById(id);
                if (orderTypeDetailsById is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OrderType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"OrderType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                orderTypeDetailsById.ActiveStatus = true;
                string result = await _repository.OrderTypeRepository.UpdateOrderType(orderTypeDetailsById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "OrderType Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateOrderType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateOrderType(int id)
        {
            ServiceResponse<OrderTypeDto> serviceResponse = new ServiceResponse<OrderTypeDto>();

            try
            {
                var orderTypeDetailsById = await _repository.OrderTypeRepository.GetOrderTypeById(id);
                if (orderTypeDetailsById is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OrderType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"OrderType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                orderTypeDetailsById.ActiveStatus = false;
                string result = await _repository.OrderTypeRepository.UpdateOrderType(orderTypeDetailsById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "OrderType Deactivated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateOrderType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
