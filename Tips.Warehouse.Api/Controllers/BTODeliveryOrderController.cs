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
    public class BTODeliveryOrderController : ControllerBase
    {
        private IBTODeliveryOrderRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public BTODeliveryOrderController(IBTODeliveryOrderRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllBTODeliveryOrder([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderDto>>();
            try
            {
                var allBTODeliveryOrdersDetails = await _repository.GetAllBTODeliveryOrder(pagingParameter);
                var metadata = new
                {
                    allBTODeliveryOrdersDetails.TotalCount,
                    allBTODeliveryOrdersDetails.PageSize,
                    allBTODeliveryOrdersDetails.CurrentPage,
                    allBTODeliveryOrdersDetails.HasNext,
                    allBTODeliveryOrdersDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all BTODeliveryOrder");
                var result = _mapper.Map<IEnumerable<BTODeliveryOrderDto>>(allBTODeliveryOrdersDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BTODeliveryOrder";
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
        public async Task<IActionResult> GetBTODeliveryOrderById(int id)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var bTODeliveryOrderDetailById = await _repository.GetBTODeliveryOrderById(id);

                if (bTODeliveryOrderDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BTODeliveryOrder  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");

                    BTODeliveryOrderDto bTODeliveryOrderDto = _mapper.Map<BTODeliveryOrderDto>(bTODeliveryOrderDetailById);

                    List<BTODeliveryOrderItemsDto> BTODeliveryOrderItemsDtoList = new List<BTODeliveryOrderItemsDto>();

                    if (bTODeliveryOrderDetailById.BTODeliveryOrderItems != null)
                    {

                        foreach (var deliveryOrderitemDetails in bTODeliveryOrderDetailById.BTODeliveryOrderItems)
                        {
                            BTODeliveryOrderItemsDto BTODeliveryOrderItemsDtos = _mapper.Map<BTODeliveryOrderItemsDto>(deliveryOrderitemDetails);
                            BTODeliveryOrderItemsDtos.BTOSerialNumberDto = _mapper.Map<List<BTOSerialNumberDto>>(deliveryOrderitemDetails.BTOSerialNumbers);
                            BTODeliveryOrderItemsDtoList.Add(BTODeliveryOrderItemsDtos);
                        }
                    }

                    bTODeliveryOrderDto.BTODeliveryOrderItemsDto = BTODeliveryOrderItemsDtoList;

                    serviceResponse.Data = bTODeliveryOrderDto;
                    serviceResponse.Message = "Returned BTODeliveryOrder";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBTODeliveryOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateBTODeliveryOrder([FromBody] BTODeliveryOrderDtoPost bTODeliveryOrderDtoPost)
        {
            ServiceResponse<BTODeliveryOrderDtoPost> serviceResponse = new ServiceResponse<BTODeliveryOrderDtoPost>();
            try
            {
                if (bTODeliveryOrderDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BTODeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("BTODeliveryOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid BTODeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid BTODeliveryOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var bTODeliveryOrder = _mapper.Map<BTODeliveryOrder>(bTODeliveryOrderDtoPost);

                var bTODeliveryOrderitemsDto = bTODeliveryOrderDtoPost.BTODeliveryOrderItemsDtoPost;

                var bTODeliveryOrderItemsDtoList = new List<BTODeliveryOrderItems>();

                if (bTODeliveryOrderitemsDto != null)
                {
                    for (int i = 0; i < bTODeliveryOrderitemsDto.Count; i++)
                    {
                        BTODeliveryOrderItems bTODeliveryOrderItemsDetails = _mapper.Map<BTODeliveryOrderItems>(bTODeliveryOrderitemsDto[i]);
                        bTODeliveryOrderItemsDetails.BTOSerialNumbers = _mapper.Map<List<BTOSerialNumber>>(bTODeliveryOrderitemsDto[i].BTOSerialNumberDtoPost);
                        bTODeliveryOrderItemsDtoList.Add(bTODeliveryOrderItemsDetails);
                    }
                }

                bTODeliveryOrder.BTODeliveryOrderItems = bTODeliveryOrderItemsDtoList;

                await _repository.CreateBTODeliveryOrder(bTODeliveryOrder);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside BtoDelivaryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBTODeliveryOrder(int id, [FromBody] BTODeliveryOrderDtoUpdate bTODeliveryOrderDtoUpdate)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                if (bTODeliveryOrderDtoUpdate is null)
                {
                    _logger.LogError("Update BTODeliveryOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update BTODeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update BTODeliveryOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update BTODeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var bTODeliveryOrderbyId = await _repository.GetBTODeliveryOrderById(id);
                if (bTODeliveryOrderbyId is null)
                {
                    _logger.LogError($"Update BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update BTODeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var bTODeliveryOrder = _mapper.Map<BTODeliveryOrder>(bTODeliveryOrderDtoUpdate);

                var bTODeliveryOrderitemsDto = bTODeliveryOrderDtoUpdate.BTODeliveryOrderItemsDto;

                var bTODeliveryOrderitemsList = new List<BTODeliveryOrderItems>();

                if (bTODeliveryOrderitemsDto != null)
                {
                    for (int i = 0; i < bTODeliveryOrderitemsDto.Count; i++)
                    {
                        BTODeliveryOrderItems bTODeliveryOrderItems = _mapper.Map<BTODeliveryOrderItems>(bTODeliveryOrderitemsDto[i]);
                        bTODeliveryOrderItems.BTOSerialNumbers = _mapper.Map<List<BTOSerialNumber>>(bTODeliveryOrderitemsDto[i].BTOSerialNumberDto);

                    }
                }

                bTODeliveryOrder.BTODeliveryOrderItems = bTODeliveryOrderitemsList;
                var data = _mapper.Map(bTODeliveryOrderDto, bTODeliveryOrder);

                string result = await _repository.UpdateBTODeliveryOrder(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateBTODeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBTODeliveryOrder(int id)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var bTODeliveryOrderDetailById = await _repository.GetBTODeliveryOrderById(id);
                if (bTODeliveryOrderDetailById == null)
                {
                    _logger.LogError($"Delete BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete BTODeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteBTODeliveryOrder(bTODeliveryOrderDetailById);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Deleted";
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
