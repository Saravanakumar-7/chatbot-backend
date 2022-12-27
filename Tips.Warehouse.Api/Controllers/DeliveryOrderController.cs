using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.Dto;

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

        // GET: api/<DeliveryOrderController>
        [HttpGet]
        public async Task<IActionResult> GetAllDeliveryOrder([FromQuery] PagingParameter pagingParameter,string DeliveryOrderNumber)
        {
            ServiceResponse<IEnumerable<DeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<DeliveryOrderDto>>();
            try
            {
                var listOfDeliveryOrder = await _repository.GetAllDeliveryOrder(pagingParameter, DeliveryOrderNumber);
                var metadata = new
                {
                    listOfDeliveryOrder.TotalCount,
                    listOfDeliveryOrder.PageSize,
                    listOfDeliveryOrder.CurrentPage,
                    listOfDeliveryOrder.HasNext,
                    listOfDeliveryOrder.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all DeliveryOrder");
                var result = _mapper.Map<IEnumerable<DeliveryOrderDto>>(listOfDeliveryOrder);
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

        // GET api/<DeliveryOrderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveryOrderById(int id,string DeliveryOrderNumber)
        {
            ServiceResponse<DeliveryOrderDto> serviceResponse = new ServiceResponse<DeliveryOrderDto>();
            try
            {
                var deliveryOrderDetails = await _repository.GetDeliveryOrderById(id, DeliveryOrderNumber);

                if (deliveryOrderDetails == null)
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
                    DeliveryOrderDto deliveryOrderDto = _mapper.Map<DeliveryOrderDto>(deliveryOrderDetails);
                    List<DeliveryOrderItemsDto> deliveryOrderItemsDtoList = new List<DeliveryOrderItemsDto>();
                    foreach (var itemDetails in deliveryOrderDetails.deliveryOrderItems)
                    {
                        DeliveryOrderItemsDto deliveryOrderItemsDtos = _mapper.Map<DeliveryOrderItemsDto>(itemDetails);
                        deliveryOrderItemsDtoList.Add(deliveryOrderItemsDtos);
                    }
                    deliveryOrderDto.deliveryOrderItemsDto = deliveryOrderItemsDtoList;

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

        // POST api/<DeliveryOrderController>
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
                var deliveryOrderitemsDto = deliveryOrderDtoPost.deliveryOrderItemsDtoPost;

                var deliveryOrderItemsDtoList = new List<DeliveryOrderItems>();
                for (int i = 0; i < deliveryOrderitemsDto.Count; i++)
                {
                    DeliveryOrderItems deliveryOrderItemsDetails = _mapper.Map<DeliveryOrderItems>(deliveryOrderitemsDto[i]);
                    deliveryOrderItemsDtoList.Add(deliveryOrderItemsDetails);
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

        // PUT api/<DeliveryOrderController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeliveryOrder(int id,string DeliveryOrderNumber, [FromBody] DeliveryOrderDtoPost deliveryOrderDtoUpdate)
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
                var updateDeliveryOrder = await _repository.GetDeliveryOrderById(id, DeliveryOrderNumber);
                if (updateDeliveryOrder is null)
                {
                    _logger.LogError($"Update DeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update DeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var deliveryOrderList = _mapper.Map<DeliveryOrder>(updateDeliveryOrder);

                var deliveryOrderitemsDto = deliveryOrderDtoUpdate.deliveryOrderItemsDtoPost;

                var deliveryOrderitemsList = new List<DeliveryOrderItems>();
                for (int i = 0; i < deliveryOrderitemsDto.Count; i++)
                {
                    DeliveryOrderItems deliveryOrderItemsDetails = _mapper.Map<DeliveryOrderItems>(deliveryOrderitemsDto[i]);

                }
                deliveryOrderList.deliveryOrderItems = deliveryOrderitemsList;
                var data = _mapper.Map(deliveryOrderDtoUpdate, deliveryOrderList);

                string result = await _repository.UpdateDeliveryOrder(data, DeliveryOrderNumber);
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

        // DELETE api/<DeliveryOrderController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeliveryOrder(int id,string DeliveryOrderNumber)
        {
            ServiceResponse<DeliveryOrderDto> serviceResponse = new ServiceResponse<DeliveryOrderDto>();
            try
            {
                var deleteDeliveryOrder = await _repository.GetDeliveryOrderById(id, DeliveryOrderNumber);
                if (deleteDeliveryOrder == null)
                {
                    _logger.LogError($"Delete DeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete DeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteDeliveryOrder(deleteDeliveryOrder);
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
