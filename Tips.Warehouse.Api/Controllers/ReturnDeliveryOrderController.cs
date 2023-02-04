using System.Dynamic;
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
using Tips.Warehouse.Api.Repository;

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReturnDeliveryOrderController : ControllerBase
    {
        private IReturnDeliveryOrderRepository _repository;
        private IInventoryRepository _inventoryRepository;
        private IBTODeliveryOrderHistoryRepository _bTODeliveryOrderHistoryRepository;
        private IBTODeliveryOrderItemsRepository _bTODeliveryOrderItemsRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;

        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ReturnDeliveryOrderController(IReturnDeliveryOrderRepository repository, IInventoryTranctionRepository inventoryTranctionRepository , IBTODeliveryOrderHistoryRepository bTODeliveryOrderHistoryRepository , IBTODeliveryOrderItemsRepository bTODeliveryOrderItemsRepository, IInventoryRepository inventoryRepository, HttpClient httpClient, IConfiguration config, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _inventoryRepository = inventoryRepository;
            _bTODeliveryOrderItemsRepository = bTODeliveryOrderItemsRepository;
            _bTODeliveryOrderHistoryRepository = bTODeliveryOrderHistoryRepository;
            _inventoryTranctionRepository = inventoryTranctionRepository;

        }


        [HttpGet]
        public async Task<IActionResult> GetAllReturnDeliveryOrders([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ReturnDeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ReturnDeliveryOrderDto>>();
            try
            {
                var getAllReturnDeliveryOrdersDetails = await _repository.GetAllReturnDeliveryOrders(pagingParameter);
                var metadata = new
                {
                    getAllReturnDeliveryOrdersDetails.TotalCount,
                    getAllReturnDeliveryOrdersDetails.PageSize,
                    getAllReturnDeliveryOrdersDetails.CurrentPage,
                    getAllReturnDeliveryOrdersDetails.HasNext,
                    getAllReturnDeliveryOrdersDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all BTODeliveryOrder");
                var result = _mapper.Map<IEnumerable<ReturnDeliveryOrderDto>>(getAllReturnDeliveryOrdersDetails);
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
        public async Task<IActionResult> GetReturnDeliveryOrderById(int id)
        {
            ServiceResponse<ReturnDeliveryOrderDto> serviceResponse = new ServiceResponse<ReturnDeliveryOrderDto>();
            try
            {
                var getReturnDeliveryOrderDetailById = await _repository.GetReturnDeliveryOrderById(id);

                if (getReturnDeliveryOrderDetailById == null)
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

                    ReturnDeliveryOrderDto returnDeliveryOrderDto = _mapper.Map<ReturnDeliveryOrderDto>(getReturnDeliveryOrderDetailById);

                    List<ReturnDeliveryOrderItemsDto> returnDeliveryOrderItemsDtoList = new List<ReturnDeliveryOrderItemsDto>();

                    if (getReturnDeliveryOrderDetailById.ReturnDeliveryOrderItems != null)
                    {

                        foreach (var deliveryOrderitemDetails in getReturnDeliveryOrderDetailById.ReturnDeliveryOrderItems)
                        {
                            ReturnDeliveryOrderItemsDto returnDeliveryOrderItemsDtos = _mapper.Map<ReturnDeliveryOrderItemsDto>(deliveryOrderitemDetails);
                            returnDeliveryOrderItemsDtoList.Add(returnDeliveryOrderItemsDtos);
                        }
                    }

                    returnDeliveryOrderDto.ReturnDeliveryOrderItems = returnDeliveryOrderItemsDtoList;

                    serviceResponse.Data = returnDeliveryOrderDto;
                    serviceResponse.Message = "Returned ReturnDeliveryOrderById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetReturnDeliveryOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateReturnDeliveryOrder([FromBody] ReturnDeliveryOrderDtoPost returnDeliveryOrderDtoPost)
        {
            ServiceResponse<ReturnDeliveryOrderDtoPost> serviceResponse = new ServiceResponse<ReturnDeliveryOrderDtoPost>();
            try
            {
                if (returnDeliveryOrderDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReturnDeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ReturnDeliveryOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ReturnDeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ReturnDeliveryOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var returnDeliveryOrder = _mapper.Map<ReturnDeliveryOrder>(returnDeliveryOrderDtoPost);

                var returnDeliveryOrderitemsDto = returnDeliveryOrderDtoPost.ReturnDeliveryOrderItems;

                var returnDeliveryOrderItemsDtoList = new List<ReturnDeliveryOrderItems>();

                if (returnDeliveryOrderitemsDto != null)
                {
                    for (int i = 0; i < returnDeliveryOrderitemsDto.Count; i++)
                    {
                        ReturnDeliveryOrderItems returnDeliveryOrderItems = _mapper.Map<ReturnDeliveryOrderItems>(returnDeliveryOrderitemsDto[i]);
                        returnDeliveryOrderItems.ReturnQty = returnDeliveryOrderItems.AlreadyReturnQty + returnDeliveryOrderItems.ReturnQty;
                        returnDeliveryOrderItems.AlreadyReturnQty = returnDeliveryOrderItems.AlreadyReturnQty + returnDeliveryOrderItems.ReturnQty;
                        returnDeliveryOrderItems.DispatchQty = returnDeliveryOrderItems.DispatchQty - returnDeliveryOrderItems.ReturnQty;
                        returnDeliveryOrderItemsDtoList.Add(returnDeliveryOrderItems);

                        //Update Inventory balanced Quantity 
                        var PartNumber = returnDeliveryOrderitemsDto[i].FGPartNumber;
                        var BtoNumber = returnDeliveryOrderitemsDto[i].BTONumber;
                        var getInventoryDetails = await _inventoryRepository.GetInventoryDetails(PartNumber);
                        decimal ReturnQty = Convert.ToDecimal(returnDeliveryOrderitemsDto[i].ReturnQty);
                        if (getInventoryDetails != null)
                        { 
                            getInventoryDetails.Balance_Quantity = getInventoryDetails.Balance_Quantity + ReturnQty;
                            getInventoryDetails.IsStockAvailable = true;
                        }

                        //add return details in to inventory table

                        InventoryTranction inventoryTranction = new InventoryTranction();
                        inventoryTranction.PartNumber = returnDeliveryOrderItemsDtoList[i].FGPartNumber;
                        inventoryTranction.MftrPartNumber = returnDeliveryOrderItemsDtoList[i].FGPartNumber;
                        inventoryTranction.Description = returnDeliveryOrderItemsDtoList[i].Description;
                        inventoryTranction.Issued_Quantity = Convert.ToDecimal(returnDeliveryOrderItemsDtoList[i].DispatchQty);
                        inventoryTranction.UOM = returnDeliveryOrderItemsDtoList[i].UOM;
                        inventoryTranction.Issued_DateTime = DateTime.Now;
                        inventoryTranction.ReferenceID = returnDeliveryOrderItems.BTONumber;
                        inventoryTranction.ReferenceIDFrom = "Return BTO Delivery Order";
                        inventoryTranction.Issued_By = "Admin";
                        inventoryTranction.CreatedOn = DateTime.Now;
                        inventoryTranction.Unit = "Bangalore";
                        inventoryTranction.CreatedBy = "Admin";
                        inventoryTranction.LastModifiedBy = "Admin";
                        inventoryTranction.LastModifiedOn = DateTime.Now;
                        inventoryTranction.ModifiedStatus = false;
                        inventoryTranction.From_Location = "FG";
                        inventoryTranction.TO_Location = "BTO";
                        inventoryTranction.Remarks = "Return,BTO";

                        var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                        _inventoryTranctionRepository.SaveAsync();

                        _inventoryRepository.Update(getInventoryDetails);
                        _inventoryRepository.SaveAsync();

                        //update balance Qty and Dispatch Qty in Bto Delivery Order Table

                        var getBtoDeliveryOrderDetails = await _bTODeliveryOrderItemsRepository.GetBtoDelieveryOrderItemDetails(PartNumber, BtoNumber);
                        decimal BtoReturnQty = Convert.ToDecimal(returnDeliveryOrderitemsDto[i].ReturnQty);
                        getBtoDeliveryOrderDetails.BalanceDoQty = getBtoDeliveryOrderDetails.BalanceDoQty + BtoReturnQty;
                        getBtoDeliveryOrderDetails.DispatchQty -= BtoReturnQty;

                        // Add return details in to btodeliveryorderhistory table

                        BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                        bTODeliveryOrderHistory.BTONumber = returnDeliveryOrderItems.BTONumber;
                        bTODeliveryOrderHistory.CustomerName = returnDeliveryOrder.CustomerName;
                        bTODeliveryOrderHistory.CustomerAliasName = returnDeliveryOrder.CustomerAliasName;
                        bTODeliveryOrderHistory.CustomerId = returnDeliveryOrder.CustomerId;
                        bTODeliveryOrderHistory.PONumber = returnDeliveryOrder.PONumber;
                        bTODeliveryOrderHistory.IssuedTo = returnDeliveryOrder.IssuedTo;
                        bTODeliveryOrderHistory.DODate = Convert.ToDateTime(returnDeliveryOrder.CreatedOn);
                        bTODeliveryOrderHistory.FGItemNumber = returnDeliveryOrderItemsDtoList[i].FGPartNumber;
                        bTODeliveryOrderHistory.SalesOrderId = getBtoDeliveryOrderDetails.SalesOrderId;
                        bTODeliveryOrderHistory.Description = returnDeliveryOrderItemsDtoList[i].Description;
                        bTODeliveryOrderHistory.BalanceDoQty = Convert.ToDecimal(returnDeliveryOrderItemsDtoList[i].BalanceQty);
                        bTODeliveryOrderHistory.UnitPrice = Convert.ToDecimal(returnDeliveryOrderItemsDtoList[i].UnitPrice);
                        bTODeliveryOrderHistory.UOC = returnDeliveryOrderItemsDtoList[i].UOC;
                        bTODeliveryOrderHistory.UOM = returnDeliveryOrderItemsDtoList[i].UOM;
                        bTODeliveryOrderHistory.FGOrderQty = Convert.ToDecimal(returnDeliveryOrderItemsDtoList[i].FGOrderQty);
                        bTODeliveryOrderHistory.OrderBalanceQty = Convert.ToDecimal(returnDeliveryOrderItemsDtoList[i].OrderBalanceQty);
                        bTODeliveryOrderHistory.FGStock = Convert.ToDecimal(returnDeliveryOrderItemsDtoList[i].FGStock);
                        bTODeliveryOrderHistory.Discount = getBtoDeliveryOrderDetails.Discount;
                        bTODeliveryOrderHistory.NetValue = getBtoDeliveryOrderDetails.NetValue;
                        bTODeliveryOrderHistory.DispatchQty = Convert.ToDecimal(returnDeliveryOrderItemsDtoList[i].DispatchQty);
                        bTODeliveryOrderHistory.InvoicedQty = getBtoDeliveryOrderDetails.InvoicedQty;
                        bTODeliveryOrderHistory.SerialNo = returnDeliveryOrderItemsDtoList[i].SerialNo;
                        bTODeliveryOrderHistory.CreatedBy = returnDeliveryOrderItemsDtoList[i].CreatedBy;
                        bTODeliveryOrderHistory.LastModifiedOn = returnDeliveryOrderItemsDtoList[i].LastModifiedOn;
                        bTODeliveryOrderHistory.Remark = "From Return BTO";

                        var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);

                        await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);
                        _bTODeliveryOrderHistoryRepository.SaveAsync();


                        _bTODeliveryOrderItemsRepository.Update(getBtoDeliveryOrderDetails);
                        _bTODeliveryOrderItemsRepository.SaveAsync();      

                    }
                }

                returnDeliveryOrder.ReturnDeliveryOrderItems = returnDeliveryOrderItemsDtoList;

                await _repository.CreateReturnDeliveryOrder(returnDeliveryOrder);
                _repository.SaveAsync();

                 
                //update balance qty and dispatch qty in sales order table for return bto concept

                var btoDeliveryReturnDetails = _mapper.Map<List<BtoDOReturnQtyDetailsDto>>(returnDeliveryOrderitemsDto);

                var json = JsonConvert.SerializeObject(btoDeliveryReturnDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"],"ReturnDOUpdateDispatchDetails"), data);

                serviceResponse.Data = null;
                serviceResponse.Message = " BTODeliveryOrder Quantity Return Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateReturnDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReturnDeliveryOrder(int id, [FromBody] ReturnDeliveryOrderDtoUpdate returnDeliveryOrderDtoUpdate)
        {
            ServiceResponse<ReturnDeliveryOrderDtoUpdate> serviceResponse = new ServiceResponse<ReturnDeliveryOrderDtoUpdate>();
            try
            {
                if (returnDeliveryOrderDtoUpdate is null)
                {
                    _logger.LogError("Update ReturnDeliveryOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ReturnDeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update ReturnDeliveryOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ReturnDeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var returnDeliveryOrderbyId = await _repository.GetReturnDeliveryOrderById(id);
                if (returnDeliveryOrderbyId is null)
                {
                    _logger.LogError($"Update ReturnDeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update ReturnDeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var returnDeliveryOrder = _mapper.Map<ReturnDeliveryOrder>(returnDeliveryOrderbyId);
                var returnDeliveryOrderitemsDto = returnDeliveryOrderDtoUpdate.ReturnDeliveryOrderItems;
                var returnDeliveryOrderitemsList = new List<ReturnDeliveryOrderItems>();

                if (returnDeliveryOrderitemsDto != null)
                {
                    for (int i = 0; i < returnDeliveryOrderitemsDto.Count; i++)
                    {
                        ReturnDeliveryOrderItems returnDeliveryOrderItems = _mapper.Map<ReturnDeliveryOrderItems>(returnDeliveryOrderitemsDto[i]);
                        returnDeliveryOrderitemsList.Add(returnDeliveryOrderItems);
                    }
                }

                returnDeliveryOrder.ReturnDeliveryOrderItems = returnDeliveryOrderitemsList;
                var updateReturnDeliveryOrder = _mapper.Map(returnDeliveryOrderDtoUpdate, returnDeliveryOrder);

                string result = await _repository.UpdateReturnDeliveryOrder(updateReturnDeliveryOrder);
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
                _logger.LogError($"Something went wrong inside updateReturnDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReturnDeliveryOrder(int id)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var deleteReturnDeliveryOrder = await _repository.GetReturnDeliveryOrderById(id);
                if (deleteReturnDeliveryOrder == null)
                {
                    _logger.LogError($"Delete ReturnDeliveryOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete ReturnDeliveryOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteReturnDeliveryOrder(deleteReturnDeliveryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " ReturnDeliveryOrder Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteReturnDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }

}
