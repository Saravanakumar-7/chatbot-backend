using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DeliveryOrderController : ControllerBase
    {
        private IDeliveryOrderRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public DeliveryOrderController(IDeliveryOrderRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllDeliveryOrder([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<DeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<DeliveryOrderDto>>();
            try
            {
                var allDeliveryOrderDetails = await _repository.GetAllDeliveryOrder(pagingParameter);
                var metadata = new
                {
                    allDeliveryOrderDetails.TotalCount,
                    allDeliveryOrderDetails.PageSize,
                    allDeliveryOrderDetails.CurrentPage,
                    allDeliveryOrderDetails.HasNext,
                    allDeliveryOrderDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all DeliveryOrder");
                var result = _mapper.Map<IEnumerable<DeliveryOrderDto>>(allDeliveryOrderDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all DeliveryOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveryOrderById(int id)
        {
            ServiceResponse<DeliveryOrderDto> serviceResponse = new ServiceResponse<DeliveryOrderDto>();
            try
            {
                var deliveryOrderDetailById = await _repository.GetDeliveryOrderById(id);

                if (deliveryOrderDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DeliveryOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");

                    DeliveryOrderDto deliveryOrderDto = _mapper.Map<DeliveryOrderDto>(deliveryOrderDetailById);

                    List<DeliveryOrderItemsDto> deliveryOrderItemsDtoList = new List<DeliveryOrderItemsDto>();

                    if (deliveryOrderDetailById.deliveryOrderItems != null)
                    {

                        foreach (var itemDetails in deliveryOrderDetailById.deliveryOrderItems)
                        {
                            DeliveryOrderItemsDto deliveryOrderItemsDtos = _mapper.Map<DeliveryOrderItemsDto>(itemDetails);
                            deliveryOrderItemsDtoList.Add(deliveryOrderItemsDtos);
                        }
                    }

                    deliveryOrderDto.DeliveryOrderItemsDto = deliveryOrderItemsDtoList;

                    serviceResponse.Data = deliveryOrderDto;
                    serviceResponse.Message = "Returned DeliveryOrder";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDeliveryOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateDeliveryOrder([FromBody] DeliveryOrderDtoPost deliveryOrderDtoPost)
        {
            ServiceResponse<DeliveryOrderDtoPost> serviceResponse = new ServiceResponse<DeliveryOrderDtoPost>();
            try
            {
                if (deliveryOrderDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "DeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("DeliveryOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid DeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid DeliveryOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var deliveryOrder = _mapper.Map<DeliveryOrder>(deliveryOrderDtoPost);
                var deliveryOrderitemsDto = deliveryOrderDtoPost.DeliveryOrderItemsDtoPost;

                var deliveryOrderItemsDtoList = new List<DeliveryOrderItems>();

                if (deliveryOrderitemsDto != null)
                {
                    for (int i = 0; i < deliveryOrderitemsDto.Count; i++)
                    {
                        DeliveryOrderItems deliveryOrderItems = _mapper.Map<DeliveryOrderItems>(deliveryOrderitemsDto[i]);
                        deliveryOrderItemsDtoList.Add(deliveryOrderItems);
                    }
                }

                deliveryOrder.deliveryOrderItems = deliveryOrderItemsDtoList;

                await _repository.CreateDeliveryOrder(deliveryOrder);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " DeliveryOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

  
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeliveryOrder(int id, [FromBody] DeliveryOrderDtoPost deliveryOrderDtoUpdate)
        {
            ServiceResponse<DeliveryOrderDto> serviceResponse = new ServiceResponse<DeliveryOrderDto>();
            try
            {
                if (deliveryOrderDtoUpdate is null)
                {
                    _logger.LogError("Update DeliveryOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update DeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update DeliveryOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update DeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var deliveryOrderDetailById = await _repository.GetDeliveryOrderById(id);
                if (deliveryOrderDetailById is null)
                {
                    _logger.LogError($"Update DeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update DeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var deliveryOrder = _mapper.Map<DeliveryOrder>(deliveryOrderDetailById);

                var deliveryOrderitemsDto = deliveryOrderDtoUpdate.DeliveryOrderItemsDtoPost;

                var deliveryOrderitemsList = new List<DeliveryOrderItems>();

                if (deliveryOrderitemsDto != null)
                {
                    for (int i = 0; i < deliveryOrderitemsDto.Count; i++)
                    {
                        DeliveryOrderItems deliveryOrderItemsDetails = _mapper.Map<DeliveryOrderItems>(deliveryOrderitemsDto[i]);

                    }
                }

                deliveryOrder.deliveryOrderItems = deliveryOrderitemsList;

                var data = _mapper.Map(deliveryOrderDtoUpdate, deliveryOrder);

                string result = await _repository.UpdateDeliveryOrder(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " DeliveryOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

     
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeliveryOrder(int id)
        {
            ServiceResponse<DeliveryOrderDto> serviceResponse = new ServiceResponse<DeliveryOrderDto>();
            try
            {
                var deliveryOrderDetailById = await _repository.GetDeliveryOrderById(id);
                if (deliveryOrderDetailById == null)
                {
                    _logger.LogError($"Delete DeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete DeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteDeliveryOrder(deliveryOrderDetailById);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " DeliveryOrder Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
