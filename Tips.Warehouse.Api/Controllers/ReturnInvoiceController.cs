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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReturnInvoiceController : ControllerBase
    {
        private IReturnInvoiceRepository _returnInvoiceRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private IInventoryRepository _inventoryRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private IBTODeliveryOrderItemsRepository _bTODeliveryOrderItemsRepository;
        private IBTODeliveryOrderHistoryRepository _bTODeliveryOrderHistoryRepository;
        private IBTODeliveryOrderRepository _bTODeliveryOrderRepository;


        public ReturnInvoiceController(IReturnInvoiceRepository returnInvoiceRepository, HttpClient httpClient, IConfiguration config, IBTODeliveryOrderRepository bTODeliveryOrderRepository, IBTODeliveryOrderHistoryRepository bTODeliveryOrderHistoryRepository, IBTODeliveryOrderItemsRepository bTODeliveryOrderItemsRepository, IInventoryTranctionRepository inventoryTranctionRepository, IInventoryRepository inventoryRepository, ILoggerManager logger, IMapper mapper)
        {
            _returnInvoiceRepository = returnInvoiceRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _inventoryRepository = inventoryRepository;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _bTODeliveryOrderItemsRepository = bTODeliveryOrderItemsRepository;
            _bTODeliveryOrderHistoryRepository = bTODeliveryOrderHistoryRepository;
           _bTODeliveryOrderRepository = bTODeliveryOrderRepository;
        } 

        [HttpGet]
        public async Task<IActionResult> GetAllReturnInvoice([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ReturnInvoiceDto>> serviceResponse = new ServiceResponse<IEnumerable<ReturnInvoiceDto>>();

            try
            {
                var getAllReturnInvoiceDetails = await _returnInvoiceRepository.GetAllReturnInvoice(pagingParameter);

                var metadata = new
                {
                    getAllReturnInvoiceDetails.TotalCount,
                    getAllReturnInvoiceDetails.PageSize,
                    getAllReturnInvoiceDetails.CurrentPage,
                    getAllReturnInvoiceDetails.HasNext,
                    getAllReturnInvoiceDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all ReturnInvoice");
                var result = _mapper.Map<IEnumerable<ReturnInvoiceDto>>(getAllReturnInvoiceDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ReturnInvoice Successfully";
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
        public async Task<IActionResult> GetReturnInvoiceById(int id)
        {
            ServiceResponse<ReturnInvoiceDto> serviceResponse = new ServiceResponse<ReturnInvoiceDto>();

            try
            {
                var getReturnInvoiceDetailById = await _returnInvoiceRepository.GetReturnInvoiceById(id);

                if (getReturnInvoiceDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GetReturnInvoice with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GetReturnInvoice with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned OpenDeliveryOrdersDetails with id: {id}");
                    var result = _mapper.Map<ReturnInvoiceDto>(getReturnInvoiceDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned GetReturnInvoiceById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetReturnInvoiceById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateReturnInvoice([FromBody] ReturnInvoiceDtoPost ReturnInvoiceDtoPost)
        {
            ServiceResponse<InvoiceDto> serviceResponse = new ServiceResponse<InvoiceDto>();

            try
            {
                if (ReturnInvoiceDtoPost == null)
                {
                    _logger.LogError("ReturnInvoice object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invoice object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ReturnInvoice object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var returnInvoiceItem = _mapper.Map<IEnumerable<ReturnInvoiceItem>>(ReturnInvoiceDtoPost.ReturnInvoiceItems);
                var returnInvoiceDetails = _mapper.Map<ReturnInvoice>(ReturnInvoiceDtoPost);
                // invoiceEntity.ReturnInvoiceItems = returnInvoiceItem.ToList();
                var returnInvoiceItemDto = ReturnInvoiceDtoPost.ReturnInvoiceItems;
                var returnInvoiceItemsDtoList = new List<ReturnInvoiceItem>();
                if (returnInvoiceItem != null)
                {
                    for (int i = 0; i < returnInvoiceItemDto.Count; i++)
                    {
                        ReturnInvoiceItem returnInvoiceItems = _mapper.Map<ReturnInvoiceItem>(returnInvoiceItemDto[i]);
                        returnInvoiceItemsDtoList.Add(returnInvoiceItems);

                        //Update Inventory balanced Quantity 
                        var PartNumber = returnInvoiceItemDto[i].FGPartNumber;
                        var DONumber = returnInvoiceItemDto[i].DONumber;
                        var getInventoryDetails = await _inventoryRepository.GetInventoryDetails(PartNumber);
                        decimal ReturnQty = Convert.ToDecimal(returnInvoiceItemDto[i].ReturnQty);
                        if (getInventoryDetails != null)
                        {
                            getInventoryDetails.Balance_Quantity = getInventoryDetails.Balance_Quantity + ReturnQty;
                            getInventoryDetails.IsStockAvailable = true;
                        }
                        //add return details in to inventory table

                        InventoryTranction inventoryTranction = new InventoryTranction();
                        inventoryTranction.PartNumber = returnInvoiceItemsDtoList[i].FGPartNumber;
                        inventoryTranction.MftrPartNumber = returnInvoiceItemsDtoList[i].FGPartNumber;
                        inventoryTranction.Description = returnInvoiceItemsDtoList[i].Description;
                        inventoryTranction.Issued_Quantity = Convert.ToDecimal(returnInvoiceItemsDtoList[i].ReturnQty);
                        inventoryTranction.UOM = returnInvoiceItemsDtoList[i].UOM;
                        inventoryTranction.Issued_DateTime = DateTime.Now;
                        inventoryTranction.ReferenceID = returnInvoiceItemsDtoList[i].DONumber;
                        inventoryTranction.ReferenceIDFrom = "Return Invoice Delivery Order";
                        inventoryTranction.Issued_By = "Admin";
                        inventoryTranction.CreatedOn = DateTime.Now;
                        inventoryTranction.Unit = "Bangalore";
                        inventoryTranction.CreatedBy = "Admin";
                        inventoryTranction.LastModifiedBy = "Admin";
                        inventoryTranction.LastModifiedOn = DateTime.Now;
                        inventoryTranction.ModifiedStatus = false;
                        inventoryTranction.From_Location = "FG";
                        inventoryTranction.TO_Location = "Invoice";
                        inventoryTranction.Remarks = "Return,Invoice";

                        var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                        _inventoryTranctionRepository.SaveAsync();

                        _inventoryRepository.Update(getInventoryDetails);
                        _inventoryRepository.SaveAsync();

                      //update balance Qty and Dispatch Qty in Bto Delivery Order Table

                        var getBtoDeliveryOrderDetails = await _bTODeliveryOrderItemsRepository.GetBtoDelieveryOrderItemDetails(PartNumber, DONumber);
                        decimal BtoReturnQty = Convert.ToDecimal(returnInvoiceItemsDtoList[i].ReturnQty);
                        getBtoDeliveryOrderDetails.BalanceDoQty = getBtoDeliveryOrderDetails.BalanceDoQty + BtoReturnQty;
                        getBtoDeliveryOrderDetails.DispatchQty -= BtoReturnQty;
                        //passing BtoNumber GetBtoDetails
                        var getBtoDeliveryorderByBtoNo = await _bTODeliveryOrderRepository.GetBtoDetailsByBtoNo(DONumber);

                        // Add return details in to btodeliveryorderhistory table

                        BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                        bTODeliveryOrderHistory.BTONumber = returnInvoiceItemsDtoList[i].DONumber;
                        bTODeliveryOrderHistory.CustomerName = returnInvoiceDetails.CustomerName;
                        bTODeliveryOrderHistory.CustomerAliasName = returnInvoiceDetails.CustomerAliasName;
                        bTODeliveryOrderHistory.CustomerId = getBtoDeliveryorderByBtoNo.CustomerId;
                        bTODeliveryOrderHistory.PONumber = getBtoDeliveryorderByBtoNo.PONumber;
                        bTODeliveryOrderHistory.IssuedTo = getBtoDeliveryorderByBtoNo.IssuedTo;
                        bTODeliveryOrderHistory.DODate = Convert.ToDateTime(returnInvoiceDetails.CreatedOn);
                        bTODeliveryOrderHistory.FGItemNumber = returnInvoiceItemsDtoList[i].FGPartNumber;
                        bTODeliveryOrderHistory.SalesOrderId = getBtoDeliveryOrderDetails.SalesOrderId;
                        bTODeliveryOrderHistory.Description = returnInvoiceItemsDtoList[i].Description;
                        bTODeliveryOrderHistory.BalanceDoQty = Convert.ToDecimal(getBtoDeliveryOrderDetails.BalanceDoQty);
                        bTODeliveryOrderHistory.UnitPrice = Convert.ToDecimal(returnInvoiceItemsDtoList[i].UnitPrice);
                        bTODeliveryOrderHistory.UOC = getBtoDeliveryOrderDetails.UOC;
                        bTODeliveryOrderHistory.UOM = returnInvoiceItemsDtoList[i].UOM;
                        bTODeliveryOrderHistory.FGOrderQty = Convert.ToDecimal(getBtoDeliveryOrderDetails.FGOrderQty);
                        bTODeliveryOrderHistory.OrderBalanceQty = Convert.ToDecimal(getBtoDeliveryOrderDetails.OrderBalanceQty);
                        bTODeliveryOrderHistory.FGStock = Convert.ToDecimal(getBtoDeliveryOrderDetails.FGStock);
                        bTODeliveryOrderHistory.Discount = getBtoDeliveryOrderDetails.Discount;
                        bTODeliveryOrderHistory.NetValue = getBtoDeliveryOrderDetails.NetValue;
                        bTODeliveryOrderHistory.DispatchQty = Convert.ToDecimal(getBtoDeliveryOrderDetails.DispatchQty);
                        bTODeliveryOrderHistory.InvoicedQty = getBtoDeliveryOrderDetails.InvoicedQty;
                        bTODeliveryOrderHistory.SerialNo = getBtoDeliveryOrderDetails.SerialNo;
                        bTODeliveryOrderHistory.CreatedBy = returnInvoiceDetails.CreatedBy;
                        bTODeliveryOrderHistory.LastModifiedOn = returnInvoiceDetails.LastModifiedOn;
                        bTODeliveryOrderHistory.Remark = "From Return Invoice";

                        var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);

                        await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);
                        _bTODeliveryOrderHistoryRepository.SaveAsync();


                        _bTODeliveryOrderItemsRepository.Update(getBtoDeliveryOrderDetails);
                        _bTODeliveryOrderItemsRepository.SaveAsync();
                    }
                }

                returnInvoiceDetails.ReturnInvoiceItems = returnInvoiceItemsDtoList;

                await _returnInvoiceRepository.CreateReturnInvoice(returnInvoiceDetails);
                _returnInvoiceRepository.SaveAsync();

               //update balance qty and dispatch qty in sales order table for return Invoice concept

                var btoDeliveryReturnDetails = _mapper.Map<List<BtoDOReturnQtyDetailsDto>>(returnInvoiceItemDto);

                var json = JsonConvert.SerializeObject(btoDeliveryReturnDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "ReturnInvoiceUpdateDispatchDetails"), data);

                serviceResponse.Data = null;
                serviceResponse.Message = "Invoice Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetInvoiceById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateInvoice action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReturnInvoice(int id, [FromBody] ReturnInvoiceDtoUpdate returnInvoiceDtoUpdate)
        {
            ServiceResponse<ReturnInvoiceDtoUpdate> serviceResponse = new ServiceResponse<ReturnInvoiceDtoUpdate>();

            try
            {
                if (returnInvoiceDtoUpdate is null)
                {
                    _logger.LogError("Invoice object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Invoice object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Invoice object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var getReturninvoiceDetailById = await _returnInvoiceRepository.GetReturnInvoiceById(id);
                if (getReturninvoiceDetailById is null)
                {
                    _logger.LogError($"Invoice with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var updateReturnInvoice = _mapper.Map(returnInvoiceDtoUpdate, getReturninvoiceDetailById);

                string result = await _returnInvoiceRepository.UpdateReturnInvoice(updateReturnInvoice);
                _logger.LogInfo(result);
                _returnInvoiceRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Invoice Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateInvoice action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReturnInvoice(int id)
        {
            ServiceResponse<ReturnInvoiceDto> serviceResponse = new ServiceResponse<ReturnInvoiceDto>();

            try
            {
                var deleteReturnInvoiceDetail = await _returnInvoiceRepository.GetReturnInvoiceById(id);
                if (deleteReturnInvoiceDetail == null)
                {
                    _logger.LogError($"Delete Invoice with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Invoice with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _returnInvoiceRepository.DeleteReturnInvoice(deleteReturnInvoiceDetail);
                _logger.LogInfo(result);
                _returnInvoiceRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Invoice Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteInvoice action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
