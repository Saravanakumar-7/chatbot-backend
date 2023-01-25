using System.Net;
using System.Net.Http;
using System.Text;
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
        private IInventoryRepository _inventoryRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;



        public BTODeliveryOrderController(IBTODeliveryOrderRepository repository, HttpClient httpClient, IConfiguration config, IInventoryRepository inventoryRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _inventoryRepository = inventoryRepository;
            _config = config;

        }


        [HttpGet]
        public async Task<IActionResult> GetAllBTODeliveryOrders([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<BTODeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<BTODeliveryOrderDto>>();
            try
            {
                var getAllBTODeliveryOrdersDetails = await _repository.GetAllBTODeliveryOrders(pagingParameter);
                var metadata = new
                {
                    getAllBTODeliveryOrdersDetails.TotalCount,
                    getAllBTODeliveryOrdersDetails.PageSize,
                    getAllBTODeliveryOrdersDetails.CurrentPage,
                    getAllBTODeliveryOrdersDetails.HasNext,
                    getAllBTODeliveryOrdersDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all BTODeliveryOrder");
                var result = _mapper.Map<IEnumerable<BTODeliveryOrderDto>>(getAllBTODeliveryOrdersDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BTODeliveryOrders";
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
                var getBTODeliveryOrderDetailById = await _repository.GetBTODeliveryOrderById(id);

                if (getBTODeliveryOrderDetailById == null)
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

                    BTODeliveryOrderDto bTODeliveryOrderDto = _mapper.Map<BTODeliveryOrderDto>(getBTODeliveryOrderDetailById);

                    List<BTODeliveryOrderItemsDto> bTODeliveryOrderItemsDtoList = new List<BTODeliveryOrderItemsDto>();

                    if (getBTODeliveryOrderDetailById.BTODeliveryOrderItems != null)
                    {

                        foreach (var deliveryOrderitemDetails in getBTODeliveryOrderDetailById.BTODeliveryOrderItems)
                        {
                            BTODeliveryOrderItemsDto bTODeliveryOrderItemsDtos = _mapper.Map<BTODeliveryOrderItemsDto>(deliveryOrderitemDetails);
                            bTODeliveryOrderItemsDtos.BTOSerialNumberDto = _mapper.Map<List<BTOSerialNumberDto>>(deliveryOrderitemDetails.BTOSerialNumbers);
                            bTODeliveryOrderItemsDtoList.Add(bTODeliveryOrderItemsDtos);
                        }
                    }

                    bTODeliveryOrderDto.BTODeliveryOrderItemsDto = bTODeliveryOrderItemsDtoList;

                    serviceResponse.Data = bTODeliveryOrderDto;
                    serviceResponse.Message = "Returned BTODeliveryOrderById Successfully";
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
                        string cps = "";
                        var data = bTODeliveryOrderitemsDto[i].BTOSerialNumberDtoPost.ToList();
                        if (data.Count() != 0)
                        { 
                            for (int j = 0; j < data.Count(); j++)
                            {
                                cps += data[j].SerialNumber.Trim() + ",";
                            }
                            cps = cps.TrimEnd(',');
                            bTODeliveryOrderitemsDto[i].SerialNo = cps;
                        }
                        BTODeliveryOrderItems bTODeliveryOrderItemsDetails = _mapper.Map<BTODeliveryOrderItems>(bTODeliveryOrderitemsDto[i]);
                        bTODeliveryOrderItemsDetails.BTOSerialNumbers = _mapper.Map<List<BTOSerialNumber>>(bTODeliveryOrderitemsDto[i].BTOSerialNumberDtoPost);
                        bTODeliveryOrderItemsDtoList.Add(bTODeliveryOrderItemsDetails);
                    }
                }

                bTODeliveryOrder.BTODeliveryOrderItems = bTODeliveryOrderItemsDtoList;
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                

                var newcount = await _repository.GetBTONumberAutoIncrementCount(date);

                if (newcount > 0)
                {
                    var number = newcount + 1;
                    string e = String.Format("{0:D4}", number);
                    bTODeliveryOrder.BTONumber = days + months + years + "BTO" + (e);
                }
                else
                {
                    var count = 1;
                    var e = count.ToString("D4");
                    bTODeliveryOrder.BTONumber = days + months + years + "BTO" + (e);
                }
                await _repository.CreateBTODeliveryOrder(bTODeliveryOrder);
                _repository.SaveAsync();

                //update balance qty and dispatch qty in salesorder table
                var btoDeliveryDispatchDetails = _mapper.Map<BtoDeliveryOrderDispatchQtyDetailsDto>(bTODeliveryOrderitemsDto);

                if (btoDeliveryDispatchDetails != null)
                {
                    //for (int i = 0; i < bTODeliveryOrderitemsDto.Count; i++)
                    //{
                        ////var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["SalesOrderAPI"], "GetBtoDeliveryOrderDetailsBySOandItemNo?", "ItemNumber=", bTODeliveryOrderitemsDto[i].FGItemNumber, "&SalesOrderId=", bTODeliveryOrderitemsDto[i].SalesOrderId));
                        ////var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                        ////dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        ////dynamic inventoryObject = inventoryObjectData.data;
                        ////inventoryObject.BalanceQty = inventoryObject.BalanceQty - bTODeliveryOrderitemsDto[i].DispatchQty;
                        ////inventoryObject.DispatchQty = inventoryObject.DispatchQty + bTODeliveryOrderitemsDto[i].DispatchQty;
                        var json = JsonConvert.SerializeObject(btoDeliveryDispatchDetails);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "UpdateDispatchDetails"), data);

                    //}
                }


                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateBTODelivaryOrder action: {ex.Message}");
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
            ServiceResponse<BTODeliveryOrderDtoUpdate> serviceResponse = new ServiceResponse<BTODeliveryOrderDtoUpdate>();
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


                var bTODeliveryOrder = _mapper.Map<BTODeliveryOrder>(bTODeliveryOrderbyId);
                var bTODeliveryOrderitemsDto = bTODeliveryOrderDtoUpdate.BTODeliveryOrderItemsDtoUpdate;
                var bTODeliveryOrderitemsList = new List<BTODeliveryOrderItems>();

                if (bTODeliveryOrderitemsDto != null)
                {
                    for (int i = 0; i < bTODeliveryOrderitemsDto.Count; i++)
                    {
                        BTODeliveryOrderItems bTODeliveryOrderItems = _mapper.Map<BTODeliveryOrderItems>(bTODeliveryOrderitemsDto[i]);
                        bTODeliveryOrderItems.BTOSerialNumbers = _mapper.Map<List<BTOSerialNumber>>(bTODeliveryOrderitemsDto[i].BTOSerialNumberDtoUpdate);
                        bTODeliveryOrderitemsList.Add(bTODeliveryOrderItems);
                    }
                }

                bTODeliveryOrder.BTODeliveryOrderItems = bTODeliveryOrderitemsList;
                var updateBTODeliveryOrder = _mapper.Map(bTODeliveryOrderDtoUpdate, bTODeliveryOrder);

                string result = await _repository.UpdateBTODeliveryOrder(updateBTODeliveryOrder);
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
                var deleteBTODeliveryOrder = await _repository.GetBTODeliveryOrderById(id);
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
                _logger.LogError($"Something went wrong inside DeleteBTODeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetBtoDeliveryOrderNumberList()
        {
            ServiceResponse<IEnumerable<ListofBtoDeliveryOrderDetails>> serviceResponse = new ServiceResponse<IEnumerable<ListofBtoDeliveryOrderDetails>>();

            try
            {
                var getBtoDeliveryOrderNumberList = await _repository.GetBtoDeliveryOrderNumberList();
                if (getBtoDeliveryOrderNumberList == null)
                {
                    _logger.LogError("BtoDeliveryOrderDetail Not Found");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BtoDeliveryOrderDetail Not Found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo("Return BtoDeliveryOrderDetail");
                    var result = _mapper.Map<IEnumerable<ListofBtoDeliveryOrderDetails>>(getBtoDeliveryOrderNumberList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong BtoDeliveryOrder Details: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetInventoryListByItemNo(string ItemNumber)
        {
            ServiceResponse<IEnumerable<GetInventoryListByItemNo>> serviceResponse = new ServiceResponse<IEnumerable<GetInventoryListByItemNo>>();

            try
            {
                var getInventoryListByItemNo = await _inventoryRepository.GetInventoryListByItemNo(ItemNumber);
                if (getInventoryListByItemNo == null)
                {
                    _logger.LogError($"InventoryDetails with id: {ItemNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryDetails with id: {ItemNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned InventoryDetails with id: {ItemNumber}");
                    var result = _mapper.Map<IEnumerable<GetInventoryListByItemNo>>(getInventoryListByItemNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SalesDetail action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }

}
