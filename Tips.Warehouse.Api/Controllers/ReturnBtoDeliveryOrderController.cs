using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
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
    public class ReturnBtoDeliveryOrderController : ControllerBase
    {
        private IReturnBtoDeliveryOrderRepository _repository;
        private IInventoryRepository _inventoryRepository;
        private IBTODeliveryOrderHistoryRepository _bTODeliveryOrderHistoryRepository;
        private IBTODeliveryOrderItemsRepository _bTODeliveryOrderItemsRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;

        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ReturnBtoDeliveryOrderController(IReturnBtoDeliveryOrderRepository repository, IInventoryTranctionRepository inventoryTranctionRepository , IBTODeliveryOrderHistoryRepository bTODeliveryOrderHistoryRepository , IBTODeliveryOrderItemsRepository bTODeliveryOrderItemsRepository, IInventoryRepository inventoryRepository, HttpClient httpClient, IConfiguration config, ILoggerManager logger, IMapper mapper)
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
        public async Task<IActionResult> GetAllBtoHistoryDetails([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ReturnBtoDeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ReturnBtoDeliveryOrderDto>>();
            try
            {
                var btoHistoryDetails = await _bTODeliveryOrderHistoryRepository.GetAllBtoHistoryDetails(pagingParameter);
                var metadata = new
                {
                    btoHistoryDetails.TotalCount,
                    btoHistoryDetails.PageSize,
                    btoHistoryDetails.CurrentPage,
                    btoHistoryDetails.HasNext,
                    btoHistoryDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all BTODeliveryOrderHistories");
                var result = _mapper.Map<IEnumerable<ReturnBtoDeliveryOrderDto>>(btoHistoryDetails);                
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BTODeliveryOrderHistories";
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

        [HttpGet]
        public async Task<IActionResult> GetAllReturnBtoDetails([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ReturnBtoDeliveryOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<ReturnBtoDeliveryOrderDto>>();
            try
            {
                var returnBtoDetails = await _repository.GetAllReturnBtoDeliveryOrderDetails(pagingParameter);
                var metadata = new
                {
                    returnBtoDetails.TotalCount,
                    returnBtoDetails.PageSize,
                    returnBtoDetails.CurrentPage,
                    returnBtoDetails.HasNext,
                    returnBtoDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ReturnBTODeliveryOrders");
                var result = _mapper.Map<IEnumerable<ReturnBtoDeliveryOrderDto>>(returnBtoDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ReturnBTODeliveryOrders";
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
        public async Task<IActionResult> GetBtoHistoryDetailsById(int id)
        {
            ServiceResponse<BTODeliveryOrderHistory> serviceResponse = new ServiceResponse<BTODeliveryOrderHistory>();

            try
            {
                var btoHistoryDetailById = await _bTODeliveryOrderHistoryRepository.GetBtoHistoryDetailsById(id);
                if (btoHistoryDetailById == null)
                {
                    _logger.LogError($"BtoHistoryDetail with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BtoHistoryDetail with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned BtoHistoryDetail with id: {id}");
                    var result = _mapper.Map<BTODeliveryOrderHistory>(btoHistoryDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned BtoHistoryDetail Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBtoHistoryDetailsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetReturnBtoDeliveryOrderById(int id)
        {
            ServiceResponse<ReturnBtoDeliveryOrderDto> serviceResponse = new ServiceResponse<ReturnBtoDeliveryOrderDto>();
            try
            {
                var getReturnBtoDeliveryOrderDetailById = await _repository.GetReturnBtoDeliveryOrderById(id);

                if (getReturnBtoDeliveryOrderDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ReturnBTODeliveryOrder  hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ReturnBTODeliveryOrder with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ReturnBTODeliveryOrder with id: {id}");

                    ReturnBtoDeliveryOrderDto returnBtoDeliveryOrderDto = _mapper.Map<ReturnBtoDeliveryOrderDto>(getReturnBtoDeliveryOrderDetailById);

                    List<ReturnBtoDeliveryOrderItemsDto> returnBtoDeliveryOrderItemsDtoList = new List<ReturnBtoDeliveryOrderItemsDto>();

                    if (getReturnBtoDeliveryOrderDetailById.ReturnBtoDeliveryOrderItems != null)
                    {

                        foreach (var btoDeliveryOrderitemDetails in getReturnBtoDeliveryOrderDetailById.ReturnBtoDeliveryOrderItems)
                        {
                            ReturnBtoDeliveryOrderItemsDto returnBtoDeliveryOrderItemsDtos = _mapper.Map<ReturnBtoDeliveryOrderItemsDto>(btoDeliveryOrderitemDetails);
                            returnBtoDeliveryOrderItemsDtoList.Add(returnBtoDeliveryOrderItemsDtos);
                        }
                    }

                    returnBtoDeliveryOrderDto.ReturnBtoDeliveryOrderItemsDtos = returnBtoDeliveryOrderItemsDtoList;

                    serviceResponse.Data = returnBtoDeliveryOrderDto;
                    serviceResponse.Message = "Returned ReturnBtoDeliveryOrderById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetReturnBtoDeliveryOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateReturnBtoDeliveryOrder([FromBody] ReturnBtoDeliveryOrderPostDto returnBtoDeliveryOrderPostDto)
        {
            ServiceResponse<ReturnBtoDeliveryOrderPostDto> serviceResponse = new ServiceResponse<ReturnBtoDeliveryOrderPostDto>();
            try
            {
                if (returnBtoDeliveryOrderPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReturnBtoDeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ReturnBtoDeliveryOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ReturnBtoDeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ReturnBtoDeliveryOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }
                               

                var returnBtoDeliveryOrder = _mapper.Map<ReturnBtoDeliveryOrder>(returnBtoDeliveryOrderPostDto);

                var returnBtoDeliveryOrderitemsDto = returnBtoDeliveryOrderPostDto.ReturnBtoDeliveryOrderItemsPostDtos;
                //var getBtoNumber = returnBtoDeliveryOrderPostDto.BTONumber;
                //var returnBtoNumberCount = await _repository.GetReturnBtoDeliveryOrderByBtoNo(getBtoNumber);
             

                var returnBtoDeliveryOrderItemsDtoList = new List<ReturnBtoDeliveryOrderItems>();

                if (returnBtoDeliveryOrderitemsDto != null)
                {
                    for (int i = 0; i < returnBtoDeliveryOrderitemsDto.Count; i++)
                    {

                        ReturnBtoDeliveryOrderItems returnBtoDeliveryOrderItems = _mapper.Map<ReturnBtoDeliveryOrderItems>(returnBtoDeliveryOrderitemsDto[i]);
                        returnBtoDeliveryOrderItems.ReturnQty = returnBtoDeliveryOrderItems.AlreadyReturnQty + returnBtoDeliveryOrderItems.ReturnQty;
                        returnBtoDeliveryOrderItems.AlreadyReturnQty = returnBtoDeliveryOrderItems.AlreadyReturnQty + returnBtoDeliveryOrderItems.ReturnQty;
                        returnBtoDeliveryOrderItems.DispatchQty = returnBtoDeliveryOrderItems.DispatchQty - returnBtoDeliveryOrderItems.ReturnQty;
                        returnBtoDeliveryOrderItemsDtoList.Add(returnBtoDeliveryOrderItems);

                        //Update Inventory balanced Quantity

                        var PartNumber = returnBtoDeliveryOrderitemsDto[i].FGPartNumber;
                        var BtoNumber = returnBtoDeliveryOrderitemsDto[i].BTONumber;
                        var getInventoryFGDetailsByItemnumber = await _inventoryRepository.GetInventoryFGDetailsByItemNumber(PartNumber);
                        decimal ReturnQty = Convert.ToDecimal(returnBtoDeliveryOrderitemsDto[i].ReturnQty);

                        if (getInventoryFGDetailsByItemnumber != null)
                        {
                            getInventoryFGDetailsByItemnumber.Balance_Quantity = getInventoryFGDetailsByItemnumber.Balance_Quantity + ReturnQty;

                            _inventoryRepository.Update(getInventoryFGDetailsByItemnumber);
                            _inventoryRepository.SaveAsync();
                        }
                        else
                        {
                            Inventory inventory = new Inventory();
                            inventory.PartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                            inventory.MftrPartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                            inventory.Description = returnBtoDeliveryOrderItemsDtoList[i].Description;
                            inventory.ProjectNumber = "";
                            inventory.Balance_Quantity = ReturnQty;
                            inventory.UOM = returnBtoDeliveryOrderItemsDtoList[i].UOM;
                            inventory.IsStockAvailable = true;
                            inventory.Warehouse = "FG";
                            inventory.Location = "FG";
                            inventory.GrinNo= returnBtoDeliveryOrderItems.BTONumber;
                            inventory.GrinPartId = 0;
                            inventory.PartType = "";
                            inventory.GrinMaterialType = "";
                            inventory.ReferenceID = returnBtoDeliveryOrderItems.BTONumber;
                            inventory.ReferenceIDFrom = "From BTO Delivery Order";
                            inventory.shopOrderNo = "";

                            await _inventoryRepository.CreateInventory(inventory);
                            _inventoryRepository.SaveAsync();
                        }

                        InventoryTranction inventoryTranction = new InventoryTranction();
                        inventoryTranction.PartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                        inventoryTranction.MftrPartNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                        inventoryTranction.Description = returnBtoDeliveryOrderItemsDtoList[i].Description;
                        inventoryTranction.Issued_Quantity = ReturnQty;
                        inventoryTranction.UOM = returnBtoDeliveryOrderItemsDtoList[i].UOM;
                        inventoryTranction.Issued_DateTime = DateTime.Now;
                        inventoryTranction.ReferenceID = returnBtoDeliveryOrderItems.BTONumber;
                        inventoryTranction.ReferenceIDFrom = "Return BTO Delivery Order";
                        inventoryTranction.Issued_By = "Admin";    
                        inventoryTranction.From_Location = "BTO";
                        inventoryTranction.TO_Location = "FG";
                        inventoryTranction.Remarks = "Return BTO";

                        var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);

                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                        _inventoryTranctionRepository.SaveAsync();


                        //update Dispatch Qty in Bto Delivery Order Table
                        int getBtoPartsId = returnBtoDeliveryOrderitemsDto[i].BtoDeliveryOrderPartsId;
                        var getBtoDeliveryOrderDetails = await _bTODeliveryOrderItemsRepository.GetBtoDelieveryOrderItemDetails(getBtoPartsId);
                        getBtoDeliveryOrderDetails.BalanceDoQty -= ReturnQty;
                        getBtoDeliveryOrderDetails.DispatchQty -= ReturnQty;

                        String[] strs1 = getBtoDeliveryOrderDetails.SerialNo.Split(",");
                        String[] strs2 = returnBtoDeliveryOrderitemsDto[i].SerialNo.Split(",");
                        var res = strs1.Except(strs2).Union(strs2.Except(strs1));
                        String resultd = String.Join(",", res);
                        getBtoDeliveryOrderDetails.SerialNo = resultd;

                        // Add return details in to btodeliveryorderhistory table

                        BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                        bTODeliveryOrderHistory.BTONumber = returnBtoDeliveryOrderItems.BTONumber;
                        bTODeliveryOrderHistory.CustomerName = returnBtoDeliveryOrder.CustomerName;
                        bTODeliveryOrderHistory.CustomerAliasName = returnBtoDeliveryOrder.CustomerAliasName;
                        bTODeliveryOrderHistory.CustomerId = returnBtoDeliveryOrder.CustomerId;
                        bTODeliveryOrderHistory.PONumber = returnBtoDeliveryOrder.PONumber;
                        bTODeliveryOrderHistory.IssuedTo = returnBtoDeliveryOrder.IssuedTo;
                        bTODeliveryOrderHistory.DODate = Convert.ToDateTime(returnBtoDeliveryOrder.CreatedOn);
                        bTODeliveryOrderHistory.FGItemNumber = returnBtoDeliveryOrderItemsDtoList[i].FGPartNumber;
                        bTODeliveryOrderHistory.SalesOrderId = getBtoDeliveryOrderDetails.SalesOrderId;
                        bTODeliveryOrderHistory.Description = returnBtoDeliveryOrderItemsDtoList[i].Description;
                        bTODeliveryOrderHistory.BalanceDoQty = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].BalanceQty);
                        bTODeliveryOrderHistory.UnitPrice = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].UnitPrice);
                        bTODeliveryOrderHistory.UOC = returnBtoDeliveryOrderItemsDtoList[i].UOC;
                        bTODeliveryOrderHistory.UOM = returnBtoDeliveryOrderItemsDtoList[i].UOM;
                        bTODeliveryOrderHistory.FGOrderQty = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].FGOrderQty);
                        bTODeliveryOrderHistory.OrderBalanceQty = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].OrderBalanceQty);
                        bTODeliveryOrderHistory.FGStock = Convert.ToDecimal(returnBtoDeliveryOrderItemsDtoList[i].FGStock);
                        bTODeliveryOrderHistory.Discount = getBtoDeliveryOrderDetails.Discount;
                        bTODeliveryOrderHistory.NetValue = getBtoDeliveryOrderDetails.NetValue;
                        bTODeliveryOrderHistory.DispatchQty = ReturnQty;
                        bTODeliveryOrderHistory.InvoicedQty = getBtoDeliveryOrderDetails.InvoicedQty;
                        bTODeliveryOrderHistory.SerialNo = resultd; 
                        bTODeliveryOrderHistory.Remark = "From Return BTO";

                        var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);

                        await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);
                        _bTODeliveryOrderHistoryRepository.SaveAsync();


                        _bTODeliveryOrderItemsRepository.Update(getBtoDeliveryOrderDetails);
                        _bTODeliveryOrderItemsRepository.SaveAsync();      

                    }
                }

                returnBtoDeliveryOrder.ReturnBtoDeliveryOrderItems = returnBtoDeliveryOrderItemsDtoList;

                //await _repository.CreateReturnBtoDeliveryOrder(returnBtoDeliveryOrder);
                //_repository.SaveAsync();

                 
                //update balance qty and dispatch qty in sales order table for return bto concept

                var btoDeliveryReturnDetails = _mapper.Map<List<BtoDOReturnQtyDetailsDto>>(returnBtoDeliveryOrderitemsDto);

                var json = JsonConvert.SerializeObject(btoDeliveryReturnDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"],"ReturnDOUpdateDispatchDetails"), data);

                serviceResponse.Data = null;
                serviceResponse.Message = " ReturnBTODeliveryOrder created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateReturnBtoDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReturnBtoDeliveryOrder(int id, [FromBody] ReturnBtoDeliveryOrderUpdateDto returnBtoDeliveryOrderUpdateDto)
        {
            ServiceResponse<ReturnBtoDeliveryOrderUpdateDto> serviceResponse = new ServiceResponse<ReturnBtoDeliveryOrderUpdateDto>();
            try
            {
                if (returnBtoDeliveryOrderUpdateDto is null)
                {
                    _logger.LogError("Update ReturnBtoDeliveryOrder object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ReturnBtoDeliveryOrder object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update ReturnBtoDeliveryOrder object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ReturnBtoDeliveryOrder object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var returnBtoDeliveryOrderbyId = await _repository.GetReturnBtoDeliveryOrderById(id);
                if (returnBtoDeliveryOrderbyId is null)
                {
                    _logger.LogError($"Update ReturnBtoDeliveryOrder with id: {id}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update ReturnBtoDeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var returnBtoDeliveryOrder = _mapper.Map<ReturnBtoDeliveryOrder>(returnBtoDeliveryOrderbyId);
                var returnBtoDeliveryOrderitemsDto = returnBtoDeliveryOrderUpdateDto.ReturnBtoDeliveryOrderItemsUpdateDtos;
                var returnBtoDeliveryOrderitemsList = new List<ReturnBtoDeliveryOrderItems>();

                if (returnBtoDeliveryOrderitemsDto != null)
                {
                    for (int i = 0; i < returnBtoDeliveryOrderitemsDto.Count; i++)
                    {
                        ReturnBtoDeliveryOrderItems returnDeliveryOrderItems = _mapper.Map<ReturnBtoDeliveryOrderItems>(returnBtoDeliveryOrderitemsDto[i]);
                        returnBtoDeliveryOrderitemsList.Add(returnDeliveryOrderItems);
                    }
                }

                returnBtoDeliveryOrder.ReturnBtoDeliveryOrderItems = returnBtoDeliveryOrderitemsList;
                var updateReturnBtoDeliveryOrder = _mapper.Map(returnBtoDeliveryOrderUpdateDto, returnBtoDeliveryOrder);

                string result = await _repository.UpdateReturnBtoDeliveryOrder(updateReturnBtoDeliveryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " ReturnBTODeliveryOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside updateReturnBtoDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReturnBtoDeliveryOrder(int id)
        {
            ServiceResponse<BTODeliveryOrderDto> serviceResponse = new ServiceResponse<BTODeliveryOrderDto>();
            try
            {
                var deleteReturnBtoDeliveryOrder = await _repository.GetReturnBtoDeliveryOrderById(id);
                if (deleteReturnBtoDeliveryOrder == null)
                {
                    _logger.LogError($"Delete ReturnBtoDeliveryOrder with id: {id}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete ReturnBtoDeliveryOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteReturnBtoDeliveryOrder(deleteReturnBtoDeliveryOrder);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " ReturnBtoDeliveryOrder Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteReturnBtoDeliveryOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }

}
