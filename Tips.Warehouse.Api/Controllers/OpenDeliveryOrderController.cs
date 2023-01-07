using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Contracts;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OpenDeliveryOrderController : ControllerBase
    {
        private IOpenDeliveryOrderRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public OpenDeliveryOrderController(IOpenDeliveryOrderRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }     
        // GET: api/<OpenDeliveryOrderController>
        [HttpGet]
        public async Task<IActionResult> GetAllOpenDeliveryOrders([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<OpenDeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenDeliveryOrderDto>>();

            try
            { 
                var allOpenDeliveryOrderDetails = await _repository.GetAllOpenDeliveryOrders(pagingParameter);

                var metadata = new
                {
                    allOpenDeliveryOrderDetails.TotalCount,
                    allOpenDeliveryOrderDetails.PageSize,
                    allOpenDeliveryOrderDetails.CurrentPage,
                    allOpenDeliveryOrderDetails.HasNext,
                    allOpenDeliveryOrderDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all OpenDeliveryOrders");
                var result = _mapper.Map<IEnumerable<OpenDeliveryOrderDto>>(allOpenDeliveryOrderDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OpenDeliveryOrders Successfully";
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
        public async Task<IActionResult> GetOpenDeliveryOrderById(int id)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
                var openDeliveryOrdersDetailsById = await _repository.GetOpenDeliveryOrderById(id);

                if (openDeliveryOrdersDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenDeliveryOrdersDetails with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OpenDeliveryOrdersDetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned OpenDeliveryOrdersDetails with id: {id}");
                    var result = _mapper.Map<OpenDeliveryOrderDto>(openDeliveryOrdersDetailsById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned OpenDeliveryOrdersDetails with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOpenDeliveryOrdersById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOpenDeliveryOrder([FromBody] OpenDeliveryOrderDtoPost openDeliveryOrderDtoPost)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
                if (openDeliveryOrderDtoPost is null)
                {
                    _logger.LogError("OpenDeliveryOrderDetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenDeliveryOrderDetails object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid OpenDeliveryOrderDetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OpenDeliveryOrderDetails object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var openDeliveryOrderparts = _mapper.Map<IEnumerable<OpenDeliveryOrderParts>>(openDeliveryOrderDtoPost.OpenDeliveryOrderParts);

                var opendeliveryorder = _mapper.Map<OpenDeliveryOrder>(openDeliveryOrderDtoPost);

                opendeliveryorder.OpenDeliveryOrderParts = openDeliveryOrderparts.ToList();

                await _repository.CreateOpenDeliveryOrder(opendeliveryorder);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetOpenDeliveryOrderById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOpenDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOpenDeliveryOrder(int id, [FromBody] OpenDeliveryOrderDto OpenDeliveryOrderDtoUpdate)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
                if (OpenDeliveryOrderDtoUpdate is null)
                {
                    _logger.LogError("Update OpenDeliveryOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update OpenDeliveryOrder object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update OpenDeliveryOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update OpenDeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var openDeliveryOrderDetailbyId = await _repository.GetOpenDeliveryOrderById(id);
                if (openDeliveryOrderDetailbyId is null)
                {
                    _logger.LogError($"Update OpenDeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update OpenDeliveryOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }



                var openDelivaryOrderparts = _mapper.Map<IEnumerable<OpenDeliveryOrderParts>>(OpenDeliveryOrderDtoUpdate.OpenDeliveryOrderParts);


                var data = _mapper.Map(OpenDeliveryOrderDtoUpdate, openDeliveryOrderDetailbyId);


                data.OpenDeliveryOrderParts = openDelivaryOrderparts.ToList();

                string result = await _repository.UpdateOpenDeliveryOrder(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateOpenDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpenDeliveryOrder(int id)
        {
            ServiceResponse<OpenDeliveryOrderDto> serviceResponse = new ServiceResponse<OpenDeliveryOrderDto>();

            try
            {
                var openDeliveryOrderDetailbyId = await _repository.GetOpenDeliveryOrderById(id);
                if (openDeliveryOrderDetailbyId == null)
                {
                    _logger.LogError($"Delete OpenDeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete OpenDeliveryOrder with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteOpenDeliveryOrder(openDeliveryOrderDetailbyId);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOpenDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
