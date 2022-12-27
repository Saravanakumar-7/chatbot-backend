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

        // GET: api/<BTODeliveryOrderController>
        [HttpGet]
        public async Task<IActionResult> GetAllBTODeliveryOrder([FromQuery] PagingParameter pagingParameter, string BTONumber)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderDto>>();
            try
            {
                var listOfBTODeliveryOrder = await _repository.GetAllBTODeliveryOrder(pagingParameter,BTONumber);
                var metadata = new
                {
                    listOfBTODeliveryOrder.TotalCount,
                    listOfBTODeliveryOrder.PageSize,
                    listOfBTODeliveryOrder.CurrentPage,
                    listOfBTODeliveryOrder.HasNext,
                    listOfBTODeliveryOrder.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all BTODeliveryOrder");
                var result = _mapper.Map<IEnumerable<BTODeliveryOrderDto>>(listOfBTODeliveryOrder);
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

        // GET api/<BTODeliveryOrderController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBTODeliveryOrderById(int id, string BTONumber)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var bTODeliveryOrderDetails = await _repository.GetBTODeliveryOrderById(id, BTONumber);

                if (bTODeliveryOrderDetails == null)
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
                    BTODeliveryOrderDto bTODeliveryOrderDto = _mapper.Map<BTODeliveryOrderDto>(bTODeliveryOrderDetails);
                    List<BTODeliveryOrderItemsDto> BTODeliveryOrderItemsDtoList = new List<BTODeliveryOrderItemsDto>();
                    foreach (var itemDetails in bTODeliveryOrderDetails.bTODeliveryOrderItems)
                    {
                        BTODeliveryOrderItemsDto BTODeliveryOrderItemsDtos = _mapper.Map<BTODeliveryOrderItemsDto>(itemDetails);
                        BTODeliveryOrderItemsDtos.bTOSerialNumberDto = _mapper.Map<List<BTOSerialNumberDto>>(itemDetails.bTOSerialNumbers);
                        BTODeliveryOrderItemsDtoList.Add(BTODeliveryOrderItemsDtos);
                    }
                    bTODeliveryOrderDto.bTODeliveryOrderItemsDto = BTODeliveryOrderItemsDtoList;

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

        // POST api/<BTODeliveryOrderController>
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
                var bTODeliveryOrderitemsDto = bTODeliveryOrderDtoPost.bTODeliveryOrderItemsDtoPost;

                var bTODeliveryOrderItemsDtoList = new List<BTODeliveryOrderItems>();
                for (int i = 0; i < bTODeliveryOrderitemsDto.Count; i++)
                {
                    BTODeliveryOrderItems bTODeliveryOrderItemsDetails = _mapper.Map<BTODeliveryOrderItems>(bTODeliveryOrderitemsDto[i]);
                    bTODeliveryOrderItemsDetails.bTOSerialNumbers = _mapper.Map<List<BTOSerialNumber>>(bTODeliveryOrderitemsDto[i].bTOSerialNumberDtopost);
                    bTODeliveryOrderItemsDtoList.Add(bTODeliveryOrderItemsDetails);
                }
                bTODeliveryOrder.bTODeliveryOrderItems = bTODeliveryOrderItemsDtoList;

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
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<BTODeliveryOrderController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBTODeliveryOrder(int id, string BTONumber, [FromBody] BTODeliveryOrderDtoPost bTODeliveryOrderDtoUpdate)
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
                var updateBTODeliveryOrder = await _repository.GetBTODeliveryOrderById(id, BTONumber);
                if (updateBTODeliveryOrder is null)
                {
                    _logger.LogError($"Update BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update BTODeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var bTODeliveryOrderList = _mapper.Map<BTODeliveryOrder>(updateBTODeliveryOrder);

                var bTODeliveryOrderitemsDto = bTODeliveryOrderDtoUpdate.bTODeliveryOrderItemsDtoPost;

                var bTODeliveryOrderitemsList = new List<BTODeliveryOrderItems>();
                for (int i = 0; i < bTODeliveryOrderitemsDto.Count; i++)
                {
                    BTODeliveryOrderItems bTODeliveryOrderItemsDetails = _mapper.Map<BTODeliveryOrderItems>(bTODeliveryOrderitemsDto[i]);
                    bTODeliveryOrderItemsDetails.bTOSerialNumbers = _mapper.Map<List<BTOSerialNumber>>(bTODeliveryOrderitemsDto[i].bTOSerialNumberDtopost);

                }
                bTODeliveryOrderList.bTODeliveryOrderItems = bTODeliveryOrderitemsList;
                var data = _mapper.Map(bTODeliveryOrderDtoUpdate, bTODeliveryOrderList);

                string result = await _repository.UpdateBTODeliveryOrder(data, BTONumber);
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

        // DELETE api/<BTODeliveryOrderController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBTODeliveryOrder(int id, string BTONumber)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var deleteBTODeliveryOrder = await _repository.GetBTODeliveryOrderById(id, BTONumber);
                if (deleteBTODeliveryOrder == null)
                {
                    _logger.LogError($"Delete BTODeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete BTODeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteBTODeliveryOrder(deleteBTODeliveryOrder);
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
