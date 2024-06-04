using System;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Azure;
using Contracts;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
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
    [Authorize]
    public class ReturnInvoiceController : ControllerBase
    {
        private IReturnInvoiceRepository _returnInvoiceRepository;
        private IInvoiceRepository _invoiceRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private IInventoryRepository _inventoryRepository;
        private IInvoiceChildRepository _invoiceChildRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private IBTODeliveryOrderItemsRepository _bTODeliveryOrderItemsRepository;
        private IBTODeliveryOrderHistoryRepository _bTODeliveryOrderHistoryRepository;
        private IBTODeliveryOrderRepository _bTODeliveryOrderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        private readonly IHttpClientFactory _clientFactory;
        public ReturnInvoiceController(IReturnInvoiceRepository returnInvoiceRepositor, IInvoiceRepository invoiceRepository, IHttpClientFactory clientFactory, IInvoiceChildRepository invoiceChildRepository, HttpClient httpClient, IConfiguration config, IBTODeliveryOrderRepository bTODeliveryOrderRepository, IBTODeliveryOrderHistoryRepository bTODeliveryOrderHistoryRepository, IBTODeliveryOrderItemsRepository bTODeliveryOrderItemsRepository, IInventoryTranctionRepository inventoryTranctionRepository, IInventoryRepository inventoryRepository, ILoggerManager logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _invoiceRepository= invoiceRepository;
            _returnInvoiceRepository = returnInvoiceRepositor;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
            _inventoryRepository = inventoryRepository;
            _invoiceChildRepository = invoiceChildRepository;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _bTODeliveryOrderItemsRepository = bTODeliveryOrderItemsRepository;
            _bTODeliveryOrderHistoryRepository = bTODeliveryOrderHistoryRepository;
            _bTODeliveryOrderRepository = bTODeliveryOrderRepository;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReturnInvoice([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<ReturnInvoiceDoNoDto>> serviceResponse = new ServiceResponse<IEnumerable<ReturnInvoiceDoNoDto>>();

            try
            {
                var getAllReturnInvoiceDetails = await _returnInvoiceRepository.GetAllReturnInvoice(pagingParameter, searchParams);

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
                var result = _mapper.Map<IEnumerable<ReturnInvoiceDoNoDto>>(getAllReturnInvoiceDetails);
                foreach (var returnInvoice in result)
                {
                    if (returnInvoice.ReturnInvoiceItems != null)
                    {
                        var doNumbers = (returnInvoice.ReturnInvoiceItems.Select(item => item.DONumber).Distinct());
                        returnInvoice.DONumber = string.Join(", ", doNumbers);
                    }
                }
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
                    List<ReturnInvoiceItemDto> returnInvoiceItemDtos = new List<ReturnInvoiceItemDto>();
                    if (getReturnInvoiceDetailById.ReturnInvoiceItems != null)
                    {
                        foreach (var returnInvoiceitemDetails in getReturnInvoiceDetailById.ReturnInvoiceItems)
                        {
                            ReturnInvoiceItemDto returnInvoiceItemDto = _mapper.Map<ReturnInvoiceItemDto>(returnInvoiceitemDetails);
                            returnInvoiceItemDto.QtyDistribution = _mapper.Map<List<ReturnInvoiceItemQtyDistributionDto>>(returnInvoiceitemDetails.QtyDistribution);
                            returnInvoiceItemDtos.Add(returnInvoiceItemDto);
                        }
                    }
                    result.ReturnInvoiceItems = returnInvoiceItemDtos;
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

                var returnInvoiceDetails = _mapper.Map<ReturnInvoice>(ReturnInvoiceDtoPost);
                var returnInvoiceItemDto = ReturnInvoiceDtoPost.ReturnInvoiceItems;
                var returnInvoiceItemsList = new List<ReturnInvoiceItem>();
                var invoiceNumber = returnInvoiceDetails.InvoiceNumber;
                HttpStatusCode salesOrderUpdateStatusCode = HttpStatusCode.OK;
                var returnInvoiceNumberCount = await _returnInvoiceRepository.GetReturnInvoiceByInvoiceNo(invoiceNumber);
                if (returnInvoiceNumberCount != null)
                {
                    int suffixNumber = int.Parse(returnInvoiceNumberCount.Substring(returnInvoiceNumberCount.LastIndexOf("-R") + 2)) + 1;
                    string suffix = "-R" + suffixNumber;
                    returnInvoiceDetails.InvoiceNumber += suffix;

                }
                else
                {
                    returnInvoiceDetails.InvoiceNumber += "-R1";

                }

                //if (returnInvoiceNumberCount != 0)
                //{
                //    int returnInvoicecount = Convert.ToInt16(returnInvoiceNumberCount + 1);
                //    returnInvoiceDetails.InvoiceNumber = invoiceNumber + "-" + "R" + "-" + returnInvoicecount;
                //}
                //else
                //{
                //    int returnInvoicecount = 1;
                //    returnInvoiceDetails.InvoiceNumber = invoiceNumber + "-" + "R" + "-" + returnInvoicecount;
                //}
                var InvoiceDetails=await _invoiceRepository.GetInvoiceByInvoiceNumber(invoiceNumber);

                if (returnInvoiceItemDto != null)
                {
                    for (int i = 0; i < returnInvoiceItemDto.Count; i++)
                    {
                        ReturnInvoiceItem returnInvoiceItems = _mapper.Map<ReturnInvoiceItem>(returnInvoiceItemDto[i]);
                        returnInvoiceItems.QtyDistribution = _mapper.Map<List<ReturnInvoiceItemQtyDistribution>>(returnInvoiceItemDto[i].QtyDistribution);
                        returnInvoiceItems.InvoicedQty = returnInvoiceItemDto[i].InvoicedQty;
                        var returnedqty = returnInvoiceItems.ReturnQty;
                        returnInvoiceItems.ReturnQty *= -1;
                        returnInvoiceItemsList.Add(returnInvoiceItems);

                        //update Dispatch Qty in Bto Delivery Order Table
                        int getBtoDeliveryOrderPartsId = returnInvoiceItemDto[i].BtoDeliveryOrderPartsId;

                        var btoDeliveryOrderItemDetails = await _bTODeliveryOrderItemsRepository.GetBtoDelieveryOrderItemDetails(getBtoDeliveryOrderPartsId);

                        btoDeliveryOrderItemDetails.OrderBalanceQty += returnedqty;
                        //btoDeliveryOrderItemDetails.OrderBalanceQty += returnInvoiceItemDto[i].ReturnQty;
                        btoDeliveryOrderItemDetails.DispatchQty -= returnedqty;
                        //btoDeliveryOrderItemDetails.DispatchQty -= returnInvoiceItemDto[i].ReturnQty;
                        if (btoDeliveryOrderItemDetails.DispatchQty == btoDeliveryOrderItemDetails.BalanceDoQty) btoDeliveryOrderItemDetails.DoStatus = Status.Open;
                        else if (btoDeliveryOrderItemDetails.BalanceDoQty == 0) btoDeliveryOrderItemDetails.DoStatus = Status.Closed;
                        else btoDeliveryOrderItemDetails.DoStatus = Status.Closed;
                       // btoDeliveryOrderItemDetails.InvoicedQty -= returnInvoiceItemDto[i].ReturnQty;
                        //Update Inventory balanced Quantity 

                        //var PartNumber = returnInvoiceItemDto[i].FGPartNumber;
                        //var DONumber = returnInvoiceItemDto[i].DONumber;
                        //var inventoryFGDetails = await _inventoryRepository.GetInventoryFGDetailsByItemNumber(PartNumber);
                        //decimal ReturnQty = returnInvoiceItemDto[i].ReturnQty;

                        //if (inventoryFGDetails != null)
                        //{
                        //    inventoryFGDetails.Balance_Quantity = inventoryFGDetails.Balance_Quantity + ReturnQty;

                        //    _inventoryRepository.Update(inventoryFGDetails);
                        //    _inventoryRepository.SaveAsync();
                        //}
                        //else
                        //{
                        //    Inventory inventory = new Inventory();
                        //    inventory.PartNumber = returnInvoiceItemsList[i].FGPartNumber;
                        //    inventory.MftrPartNumber = returnInvoiceItemsList[i].FGPartNumber;
                        //    inventory.Description = returnInvoiceItemsList[i].Description;
                        //    inventory.ProjectNumber = "";
                        //    inventory.Balance_Quantity = ReturnQty;
                        //    inventory.UOM = returnInvoiceItemsList[i].UOM;
                        //    inventory.IsStockAvailable = true;
                        //    inventory.Warehouse = "FG";
                        //    inventory.Location = "FG";
                        //    inventory.GrinNo = "";
                        //    inventory.GrinPartId = 0;
                        //    inventory.PartType = returnInvoiceItemsList[i].PartType;
                        //    inventory.GrinMaterialType = "";
                        //    inventory.ReferenceID = returnInvoiceDetails.InvoiceNumber; // return invoice number
                        //    inventory.ReferenceIDFrom = "Return Invoice";
                        //    inventory.shopOrderNo = "";

                        //    await _inventoryRepository.CreateInventory(inventory);
                        //    _inventoryRepository.SaveAsync();
                        //}
                        foreach (var eachbin in returnInvoiceItems.QtyDistribution)
                        {
                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                            var ItemNumber = returnInvoiceItems.FGPartNumber;
                            var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                            var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                                $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                            request1.Headers.Add("Authorization", token1);

                            var itemMasterObjectResult = await client1.SendAsync(request1);
                            //if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                            //    getItemmResp = itemMasterObjectResult.StatusCode;

                            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                            var itemMasterObjectData = JsonConvert.DeserializeObject<ReturnBTONumberInvDetails>(itemMasterObjectString);
                            var itemMasterObject = itemMasterObjectData.data;

                            var exInv = await _inventoryRepository.GetInventorybyItemProjectWarehouseLocation(returnInvoiceItems.FGPartNumber, eachbin.ProjectNumber, eachbin.Warehouse, eachbin.Location);
                            if (exInv == null)
                            {
                                Inventory inventory = new Inventory();
                                inventory.PartNumber = returnInvoiceItemsList[i].FGPartNumber;
                                inventory.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); 
                                inventory.Description = returnInvoiceItemsList[i].Description;
                                inventory.ProjectNumber = eachbin.ProjectNumber;
                                inventory.Balance_Quantity = eachbin.DistributingQty;
                                inventory.UOM = returnInvoiceItemsList[i].UOM;
                                inventory.Max = itemMasterObject.max;
                                inventory.Min = itemMasterObject.min;
                                inventory.IsStockAvailable = true;
                                inventory.Warehouse = eachbin.Warehouse;
                                inventory.Location = eachbin.Location;
                                inventory.GrinNo = returnInvoiceDetails.InvoiceNumber;
                                inventory.GrinPartId = 0;
                                inventory.PartType = returnInvoiceItemsList[i].PartType;  /// Check this are you getting Part Type from Front End
                                inventory.GrinMaterialType = "";
                                inventory.ReferenceID = returnInvoiceDetails.InvoiceNumber;
                                inventory.ReferenceIDFrom = "Return Invoice";
                                inventory.shopOrderNo = "";
                                //inventory.PartType = returnBtoDeliveryOrderItems.PartType;

                                await _inventoryRepository.CreateInventory(inventory);

                            }
                            else
                            {
                                //exInv.ReferenceID = returnInvoiceDetails.InvoiceNumber;
                                //exInv.ReferenceIDFrom = "Return Invoice";
                                exInv.IsStockAvailable = true;
                                exInv.Balance_Quantity += eachbin.DistributingQty;
                                await _inventoryRepository.UpdateInventory(exInv);


                            }

                            _inventoryRepository.SaveAsync();
                            //add return details in to inventory table

                            InventoryTranction inventoryTranction = new InventoryTranction();
                            inventoryTranction.PartNumber = returnInvoiceItemsList[i].FGPartNumber;
                            inventoryTranction.MftrPartNumber = itemMasterObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); 
                            inventoryTranction.Description = returnInvoiceItemsList[i].Description;
                            inventoryTranction.Issued_Quantity = eachbin.DistributingQty;
                            inventoryTranction.UOM = returnInvoiceItemsList[i].UOM;
                            inventoryTranction.Issued_DateTime = DateTime.Now;
                            inventoryTranction.ReferenceID = returnInvoiceDetails.InvoiceNumber;
                            inventoryTranction.ReferenceIDFrom = "Return Invoice";
                            inventoryTranction.Issued_By = _createdBy;
                            inventoryTranction.From_Location = "Invoice";
                            inventoryTranction.TO_Location = eachbin.Location;
                            inventoryTranction.Remarks = "Return Invoice";
                            inventoryTranction.Warehouse = eachbin.Warehouse;
                            inventoryTranction.PartType = returnInvoiceItemsList[i].PartType;  /// Check this are you getting Part Type from Front End

                            await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranction);
                            _inventoryTranctionRepository.SaveAsync();


                            //passing BtoNumber GetBtoDetails
                            var btoDeliveryorderDetails = await _bTODeliveryOrderRepository.GetBtoDetailsByBtoNo(returnInvoiceItemDto[i].DONumber);

                            // Add return details in to btodeliveryorderhistory table

                            //BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                            //bTODeliveryOrderHistory.BTONumber = returnInvoiceItemsList[i].DONumber;
                            //bTODeliveryOrderHistory.CustomerName = returnInvoiceDetails.CustomerName;
                            //bTODeliveryOrderHistory.CustomerAliasName = returnInvoiceDetails.CustomerAliasName;
                            //bTODeliveryOrderHistory.CustomerId = btoDeliveryorderDetails.CustomerId;
                            //bTODeliveryOrderHistory.PONumber = btoDeliveryorderDetails.PONumber;
                            //bTODeliveryOrderHistory.IssuedTo = btoDeliveryorderDetails.IssuedTo;
                            //bTODeliveryOrderHistory.DODate = Convert.ToDateTime(returnInvoiceDetails.CreatedOn);
                            //bTODeliveryOrderHistory.FGItemNumber = returnInvoiceItemsList[i].FGPartNumber;
                            //bTODeliveryOrderHistory.SalesOrderId = btoDeliveryOrderItemDetails.SalesOrderId;
                            //bTODeliveryOrderHistory.Description = returnInvoiceItemsList[i].Description;
                            //bTODeliveryOrderHistory.BalanceDoQty = Convert.ToDecimal(btoDeliveryOrderItemDetails.BalanceDoQty);
                            //bTODeliveryOrderHistory.UnitPrice = Convert.ToDecimal(returnInvoiceItemsList[i].UnitPrice);
                            //bTODeliveryOrderHistory.UOC = btoDeliveryOrderItemDetails.UOC;
                            //bTODeliveryOrderHistory.UOM = returnInvoiceItemsList[i].UOM;
                            //bTODeliveryOrderHistory.FGOrderQty = Convert.ToDecimal(btoDeliveryOrderItemDetails.FGOrderQty);
                            //bTODeliveryOrderHistory.OrderBalanceQty = Convert.ToDecimal(btoDeliveryOrderItemDetails.OrderBalanceQty);
                            //bTODeliveryOrderHistory.FGStock = Convert.ToDecimal(btoDeliveryOrderItemDetails.FGStock);
                            //bTODeliveryOrderHistory.Discount = btoDeliveryOrderItemDetails.Discount;
                            //bTODeliveryOrderHistory.NetValue = btoDeliveryOrderItemDetails.NetValue;
                            //bTODeliveryOrderHistory.DispatchQty = ReturnQty;
                            //bTODeliveryOrderHistory.InvoicedQty = btoDeliveryOrderItemDetails.InvoicedQty;
                            //bTODeliveryOrderHistory.SerialNo = btoDeliveryOrderItemDetails.SerialNo;
                            ////bTODeliveryOrderHistory.CreatedBy = returnInvoiceDetails.CreatedBy;
                            ////bTODeliveryOrderHistory.LastModifiedOn = returnInvoiceDetails.LastModifiedOn;
                            //bTODeliveryOrderHistory.Remark = "Return Invoice";
                            Guid guid = Guid.NewGuid();
                            BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                            bTODeliveryOrderHistory.CustomerName = returnInvoiceDetails.CustomerName;
                            bTODeliveryOrderHistory.CustomerAliasName = returnInvoiceDetails.CustomerAliasName;
                            bTODeliveryOrderHistory.CustomerId = btoDeliveryorderDetails.CustomerId;
                            bTODeliveryOrderHistory.BTONumber = btoDeliveryorderDetails.BTONumber;
                            bTODeliveryOrderHistory.PONumber = btoDeliveryorderDetails.PONumber;
                            bTODeliveryOrderHistory.IssuedTo = btoDeliveryorderDetails.IssuedTo;
                            bTODeliveryOrderHistory.DODate = Convert.ToDateTime(returnInvoiceDetails.CreatedOn);
                            bTODeliveryOrderHistory.FGItemNumber = returnInvoiceItemsList[i].FGPartNumber;
                            bTODeliveryOrderHistory.SalesOrderId = btoDeliveryOrderItemDetails.SalesOrderId;
                            bTODeliveryOrderHistory.Description = returnInvoiceItemsList[i].Description;
                            bTODeliveryOrderHistory.BalanceDoQty = Convert.ToDecimal(btoDeliveryOrderItemDetails.BalanceDoQty);
                            bTODeliveryOrderHistory.UnitPrice = Convert.ToDecimal(returnInvoiceItemsList[i].UnitPrice);
                            bTODeliveryOrderHistory.UOC = returnInvoiceItemsList[i].UOC;
                            bTODeliveryOrderHistory.UOM = returnInvoiceItemsList[i].UOM;
                            bTODeliveryOrderHistory.FGOrderQty = Convert.ToDecimal(btoDeliveryOrderItemDetails.FGOrderQty);
                            bTODeliveryOrderHistory.OrderBalanceQty = Convert.ToDecimal(btoDeliveryOrderItemDetails.OrderBalanceQty);
                            bTODeliveryOrderHistory.FGStock = Convert.ToDecimal(btoDeliveryOrderItemDetails.FGStock);
                            bTODeliveryOrderHistory.Discount = btoDeliveryOrderItemDetails.Discount;
                            bTODeliveryOrderHistory.NetValue = btoDeliveryOrderItemDetails.NetValue;
                            bTODeliveryOrderHistory.Location = eachbin.Location;
                            bTODeliveryOrderHistory.Warehouse = eachbin.Warehouse;
                            bTODeliveryOrderHistory.DispatchQty = eachbin.DistributingQty;
                            //bTODeliveryOrderHistory.InvoicedQty = getBtoDeliveryOrderDetails.InvoicedQty;
                            bTODeliveryOrderHistory.SerialNo = btoDeliveryOrderItemDetails.SerialNo;
                            bTODeliveryOrderHistory.Remark = "Return Invoice";
                            bTODeliveryOrderHistory.UniqeId = "";
                            var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);

                            await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);
                            _bTODeliveryOrderHistoryRepository.SaveAsync();

                        }
                        _bTODeliveryOrderItemsRepository.Update(btoDeliveryOrderItemDetails);
                        _bTODeliveryOrderItemsRepository.SaveAsync();
                        // var getDoDetails=await _bTODeliveryOrderRepository.GetBTODeliveryOrderById(btoDeliveryOrderItemDetails.BTODeliveryOrderId);
                        var doNumber=btoDeliveryOrderItemDetails.BTONumber;
                         var bTODeliveryOrderItemsPartiallyClosedAndOpenStatusCount = await _bTODeliveryOrderItemsRepository.GetBTODeliveryOrderItemsPartiallyClosedAndOpenStatusCount(doNumber);

                        if (bTODeliveryOrderItemsPartiallyClosedAndOpenStatusCount != 0)
                        {
                            var bTODeliveryOrderDetails = await _bTODeliveryOrderRepository.GetBtoDetailsByBtoNo(doNumber);
                            bTODeliveryOrderDetails.DoStatus = Status.PartiallyClosed;
                            await _bTODeliveryOrderRepository.UpdateBTODeliveryOrder(bTODeliveryOrderDetails);
                        }
                        else
                        {
                            var bTODeliveryOrderDetails = await _bTODeliveryOrderRepository.GetBtoDetailsByBtoNo(doNumber);
                            bTODeliveryOrderDetails.DoStatus = Status.Closed;
                            await _bTODeliveryOrderRepository.UpdateBTODeliveryOrder(bTODeliveryOrderDetails);
                        }
                        _bTODeliveryOrderRepository.SaveAsync();
                        //update Dispatch Qty in InvoiceChildItem Table
                        //int getInvoiceChildItemId = returnInvoiceItemDto[i].InvoicePartsId;
                        //var invoiceChildItemDetails = await _invoiceChildRepository.GetInvoiceChildItemDetails(getInvoiceChildItemId);
                        //invoiceChildItemDetails.InvoicedQty -= returnInvoiceItemDto[i].ReturnQty;
                        //_invoiceChildRepository.Update(invoiceChildItemDetails);
                        //_invoiceChildRepository.SaveAsync();
                        if (InvoiceDetails != null)
                        {
                            foreach (var item in InvoiceDetails.invoiceChildItems)
                            {
                                decimal? ReturnInvoiceQty=0;
                                if (item.FGItemNumber == returnInvoiceItems.FGPartNumber) { item.ReturnInvoiceQty += returnInvoiceItems.ReturnQty; ReturnInvoiceQty = item.ReturnInvoiceQty * -1; }
                                else continue;
                                if (item.InvoicedQty == ReturnInvoiceQty) item.InvoiceItemStatus = Status.Closed;
                                else if (ReturnInvoiceQty == 0) item.InvoiceItemStatus = Status.Open;
                                else if (ReturnInvoiceQty > 0 && item.InvoicedQty > ReturnInvoiceQty) item.InvoiceItemStatus = Status.PartiallyClosed;
                            }
                            if (InvoiceDetails.invoiceChildItems.Count() == InvoiceDetails.invoiceChildItems.Where(x => x.InvoiceItemStatus == Status.Closed).Count()) InvoiceDetails.InvoiceStatus = Status.Closed;
                            else if (InvoiceDetails.invoiceChildItems.Count() == InvoiceDetails.invoiceChildItems.Where(x => x.InvoiceItemStatus == Status.Open).Count()) InvoiceDetails.InvoiceStatus = Status.Open;
                            else InvoiceDetails.InvoiceStatus = Status.PartiallyClosed;
                            await _invoiceRepository.UpdateInvoiceFromReturnInvoice(InvoiceDetails);
                            _invoiceRepository.SaveAsync();
                        }
                    }
                }
                foreach(var Returninv in returnInvoiceItemsList)
                {
                    var bTODeliveryOrderDetails = await _bTODeliveryOrderRepository.GetBtoDetailsByBtoNo(Returninv.DONumber);
                    bTODeliveryOrderDetails.TotalValue = 0;
                    foreach (var doitem in bTODeliveryOrderDetails.bTODeliveryOrderItems)
                    {
                        decimal? dispatch = doitem.DispatchQty;
                        if (dispatch == 0) dispatch = 1;
                        decimal? unitafterDiscount = 0;
                        if (doitem.DiscountType == "Percentage")
                        {
                            unitafterDiscount = doitem.UnitPrice - ((doitem.UnitPrice * doitem.Discount) / 100);
                        }
                        else
                        {
                            unitafterDiscount = doitem.UnitPrice - doitem.Discount;
                        }
                        decimal? NewBasicAmt = unitafterDiscount * doitem.DispatchQty;
                        decimal? TaxPercentage = (NewBasicAmt * ((doitem.CGST + doitem.SGST + doitem.IGST + doitem.UTGST) / 100));
                        decimal? ItemDispatchvalue = NewBasicAmt + TaxPercentage;
                        bTODeliveryOrderDetails.TotalValue += ItemDispatchvalue;
                    }
                    await _bTODeliveryOrderRepository.UpdateBTODeliveryOrderFromReturnDO(bTODeliveryOrderDetails);
                    _bTODeliveryOrderRepository.SaveAsync();
                }

                returnInvoiceDetails.ReturnInvoiceItems = returnInvoiceItemsList;

                await _returnInvoiceRepository.CreateReturnInvoice(returnInvoiceDetails);


                //update balance qty and dispatch qty in sales order table for return Invoice concept

                var invoiceReturnDetails = _mapper.Map<List<BtoInvoiceReturnQtyDetailsDto>>(returnInvoiceItemDto);

                var json = JsonConvert.SerializeObject(invoiceReturnDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["SalesOrderAPI"],
                           "ReturnInvoiceUpdateDispatchDetails"))
                {
                    Content = data
                };
                request.Headers.Add("Authorization", token);

                var response = await client.SendAsync(request);
                //var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "ReturnInvoiceUpdateDispatchDetails"), data);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    salesOrderUpdateStatusCode = response.StatusCode;
                }

                if (salesOrderUpdateStatusCode == HttpStatusCode.OK)
                {
                    _returnInvoiceRepository.SaveAsync();
                }
                else
                {
                    _logger.LogError($"Something went wrong inside Create CreateReturnInvoice action: SalesOrder Service Calling");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Saving Failed";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
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

        [HttpPost]
        public async Task<IActionResult> CreateReturnInvoice_AV([FromBody] ReturnInvoiceDtoPost ReturnInvoiceDtoPost)
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

                var returnInvoiceDetails = _mapper.Map<ReturnInvoice>(ReturnInvoiceDtoPost);
                var returnInvoiceItemDto = ReturnInvoiceDtoPost.ReturnInvoiceItems;
                var returnInvoiceItemsList = new List<ReturnInvoiceItem>();
                var invoiceNumber = returnInvoiceDetails.InvoiceNumber;
                var returnInvoiceNumberCount = await _returnInvoiceRepository.GetReturnInvoiceByInvoiceNo(invoiceNumber);
                if (returnInvoiceNumberCount != null)
                {
                    int suffixNumber = int.Parse(returnInvoiceNumberCount.Substring(returnInvoiceNumberCount.LastIndexOf("-R") + 2)) + 1;
                    string suffix = "-R" + suffixNumber;
                    returnInvoiceDetails.InvoiceNumber += suffix;

                }
                else
                {
                    returnInvoiceDetails.InvoiceNumber += "-R1";

                }

                //if (returnInvoiceNumberCount != 0)
                //{
                //    int returnInvoicecount = Convert.ToInt16(returnInvoiceNumberCount + 1);
                //    returnInvoiceDetails.InvoiceNumber = invoiceNumber + "-" + "R" + "-" + returnInvoicecount;
                //}
                //else
                //{
                //    int returnInvoicecount = 1;
                //    returnInvoiceDetails.InvoiceNumber = invoiceNumber + "-" + "R" + "-" + returnInvoicecount;
                //}


                if (returnInvoiceItemDto != null)
                {
                    for (int i = 0; i < returnInvoiceItemDto.Count; i++)
                    {
                        ReturnInvoiceItem returnInvoiceItems = _mapper.Map<ReturnInvoiceItem>(returnInvoiceItemDto[i]);
                        //returnInvoiceItems.InvoicedQty -= returnInvoiceItemDto[i].ReturnQty;
                        returnInvoiceItemsList.Add(returnInvoiceItems);


                        //Update Inventory balanced Quantity 

                        var PartNumber = returnInvoiceItemDto[i].FGPartNumber;
                        var DONumber = returnInvoiceItemDto[i].DONumber;
                        var inventoryFGDetails = await _inventoryRepository.GetInventoryFGDetailsByItemNumber(PartNumber);
                        decimal ReturnQty = returnInvoiceItemDto[i].ReturnQty;

                        if (inventoryFGDetails != null)
                        {
                            inventoryFGDetails.Balance_Quantity = inventoryFGDetails.Balance_Quantity + ReturnQty;

                            _inventoryRepository.Update(inventoryFGDetails);
                            _inventoryRepository.SaveAsync();
                        }
                        else
                        {
                            Inventory inventory = new Inventory();
                            inventory.PartNumber = returnInvoiceItemsList[i].FGPartNumber;
                            inventory.MftrPartNumber = returnInvoiceItemsList[i].FGPartNumber;
                            inventory.Description = returnInvoiceItemsList[i].Description;
                            inventory.ProjectNumber = "";
                            inventory.Balance_Quantity = ReturnQty;
                            inventory.UOM = returnInvoiceItemsList[i].UOM;
                            inventory.IsStockAvailable = true;
                            inventory.Warehouse = "FG";
                            inventory.Location = "FG";
                            inventory.GrinNo = "";
                            inventory.GrinPartId = 0;
                            inventory.PartType = returnInvoiceItemsList[i].PartType;
                            inventory.GrinMaterialType = "";
                            inventory.ReferenceID = returnInvoiceDetails.InvoiceNumber; // return invoice number
                            inventory.ReferenceIDFrom = "Return Invoice";
                            inventory.shopOrderNo = "";

                            await _inventoryRepository.CreateInventory(inventory);
                            _inventoryRepository.SaveAsync();
                        }

                        //add return details in to inventory table

                        InventoryTranction inventoryTranction = new InventoryTranction();
                        inventoryTranction.PartNumber = returnInvoiceItemsList[i].FGPartNumber;
                        inventoryTranction.MftrPartNumber = returnInvoiceItemsList[i].FGPartNumber;
                        inventoryTranction.Description = returnInvoiceItemsList[i].Description;
                        inventoryTranction.Issued_Quantity = ReturnQty;
                        inventoryTranction.UOM = returnInvoiceItemsList[i].UOM;
                        inventoryTranction.Issued_DateTime = DateTime.Now;
                        inventoryTranction.ReferenceID = returnInvoiceDetails.InvoiceNumber;
                        inventoryTranction.ReferenceIDFrom = "Return Invoice";
                        inventoryTranction.From_Location = "Invoice";
                        inventoryTranction.TO_Location = "FG";
                        inventoryTranction.Remarks = returnInvoiceItemsList[i].Remarks;
                        inventoryTranction.Warehouse = "FG";

                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranction);
                        _inventoryTranctionRepository.SaveAsync();


                        //update Dispatch Qty in Bto Delivery Order Table
                        int getBtoDeliveryOrderPartsId = returnInvoiceItemDto[i].BtoDeliveryOrderPartsId;

                        var btoDeliveryOrderItemDetails = await _bTODeliveryOrderItemsRepository.GetBtoDelieveryOrderItemDetails(getBtoDeliveryOrderPartsId);
                        btoDeliveryOrderItemDetails.OrderBalanceQty += ReturnQty;
                        btoDeliveryOrderItemDetails.DispatchQty -= ReturnQty;
                        btoDeliveryOrderItemDetails.InvoicedQty -= ReturnQty;

                        //update Dispatch Qty in InvoiceChildItem Table
                        int getInvoiceChildItemId = returnInvoiceItemDto[i].InvoicePartsId;

                        var invoiceChildItemDetails = await _invoiceChildRepository.GetInvoiceChildItemDetails(getInvoiceChildItemId);
                        invoiceChildItemDetails.InvoicedQty -= ReturnQty;

                        _invoiceChildRepository.Update(invoiceChildItemDetails);
                        _invoiceChildRepository.SaveAsync();

                        //passing BtoNumber GetBtoDetails
                        var btoDeliveryorderDetails = await _bTODeliveryOrderRepository.GetBtoDetailsByBtoNo(DONumber);

                        // Add return details in to btodeliveryorderhistory table

                        BTODeliveryOrderHistory bTODeliveryOrderHistory = new BTODeliveryOrderHistory();
                        bTODeliveryOrderHistory.BTONumber = returnInvoiceItemsList[i].DONumber;
                        bTODeliveryOrderHistory.CustomerName = returnInvoiceDetails.CustomerName;
                        bTODeliveryOrderHistory.CustomerAliasName = returnInvoiceDetails.CustomerAliasName;
                        bTODeliveryOrderHistory.CustomerId = btoDeliveryorderDetails.CustomerId;
                        bTODeliveryOrderHistory.PONumber = btoDeliveryorderDetails.PONumber;
                        bTODeliveryOrderHistory.IssuedTo = btoDeliveryorderDetails.IssuedTo;
                        bTODeliveryOrderHistory.DODate = Convert.ToDateTime(returnInvoiceDetails.CreatedOn);
                        bTODeliveryOrderHistory.FGItemNumber = returnInvoiceItemsList[i].FGPartNumber;
                        bTODeliveryOrderHistory.SalesOrderId = btoDeliveryOrderItemDetails.SalesOrderId;
                        bTODeliveryOrderHistory.Description = returnInvoiceItemsList[i].Description;
                        bTODeliveryOrderHistory.BalanceDoQty = Convert.ToDecimal(btoDeliveryOrderItemDetails.BalanceDoQty);
                        bTODeliveryOrderHistory.UnitPrice = Convert.ToDecimal(returnInvoiceItemsList[i].UnitPrice);
                        bTODeliveryOrderHistory.UOC = btoDeliveryOrderItemDetails.UOC;
                        bTODeliveryOrderHistory.UOM = returnInvoiceItemsList[i].UOM;
                        bTODeliveryOrderHistory.FGOrderQty = Convert.ToDecimal(btoDeliveryOrderItemDetails.FGOrderQty);
                        bTODeliveryOrderHistory.OrderBalanceQty = Convert.ToDecimal(btoDeliveryOrderItemDetails.OrderBalanceQty);
                        bTODeliveryOrderHistory.FGStock = Convert.ToDecimal(btoDeliveryOrderItemDetails.FGStock);
                        bTODeliveryOrderHistory.Discount = btoDeliveryOrderItemDetails.Discount;
                        bTODeliveryOrderHistory.NetValue = btoDeliveryOrderItemDetails.NetValue;
                        bTODeliveryOrderHistory.DispatchQty = ReturnQty;
                        bTODeliveryOrderHistory.InvoicedQty = btoDeliveryOrderItemDetails.InvoicedQty;
                        bTODeliveryOrderHistory.SerialNo = btoDeliveryOrderItemDetails.SerialNo;
                        //bTODeliveryOrderHistory.CreatedBy = returnInvoiceDetails.CreatedBy;
                        //bTODeliveryOrderHistory.LastModifiedOn = returnInvoiceDetails.LastModifiedOn;
                        bTODeliveryOrderHistory.Remark = "Return Invoice";

                        var bTODeliveryOrderHistoryDetails = _mapper.Map<BTODeliveryOrderHistory>(bTODeliveryOrderHistory);

                        await _bTODeliveryOrderHistoryRepository.CreateBTODeliveryOrderHistory(bTODeliveryOrderHistoryDetails);
                        _bTODeliveryOrderHistoryRepository.SaveAsync();


                        _bTODeliveryOrderItemsRepository.Update(btoDeliveryOrderItemDetails);
                        _bTODeliveryOrderItemsRepository.SaveAsync();
                    }
                }

                returnInvoiceDetails.ReturnInvoiceItems = returnInvoiceItemsList;

                await _returnInvoiceRepository.CreateReturnInvoice(returnInvoiceDetails);
                _returnInvoiceRepository.SaveAsync();

                //update balance qty and dispatch qty in sales order table for return Invoice concept

                var invoiceReturnDetails = _mapper.Map<List<BtoInvoiceReturnQtyDetailsDto>>(returnInvoiceItemDto);

                var json = JsonConvert.SerializeObject(invoiceReturnDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                // Include the token in the Authorization header
                //var tokenValues = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                //if (!string.IsNullOrEmpty(tokenValues) && tokenValues.StartsWith("Bearer "))
                //{
                //    var token = tokenValues.Substring(7);
                //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                //}
                //var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "ReturnInvoiceUpdateDispatchDetails"), data);
                var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["SalesOrderAPI"],
                            "ReturnInvoiceUpdateDispatchDetails"))
                { Content= data };
                request.Headers.Add("Authorization", token);

                var response = await client.SendAsync(request);
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

        [HttpGet]
        public async Task<IActionResult> GetReturnInvoiceSPResport([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ReturnInvoiceSPResport>> serviceResponse = new ServiceResponse<IEnumerable<ReturnInvoiceSPResport>>();

            try
            {
                var products = await _returnInvoiceRepository.GetReturnInvoiceSPResport(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all ReturnInvoiceSPResport");
                var result = _mapper.Map<IEnumerable<ReturnInvoiceSPResport>>(products);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ReturnInvoiceSPResport Successfully";
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
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> ReturnInvoiceSPReportDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<ReturnInvoiceSPResport>> serviceResponse = new ServiceResponse<IEnumerable<ReturnInvoiceSPResport>>();
            try
            {
                var products = await _returnInvoiceRepository.ReturnInvoiceSPReportDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ReturnInvoiceSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ReturnInvoiceSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ReturnInvoiceSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside Invoice action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> ReturnInvoiceSPReportWithParameter([FromBody] ReturnInvoiceSPReportWithParamDTO returnInvoiceSPReportDTO)
        {
            ServiceResponse<IEnumerable<ReturnInvoiceSPResport>> serviceResponse = new ServiceResponse<IEnumerable<ReturnInvoiceSPResport>>();
            try
            {
                var products = await _returnInvoiceRepository.ReturnInvoiceSPReportWithParameter(returnInvoiceSPReportDTO.InvoiceNumber, returnInvoiceSPReportDTO.DoNumber, returnInvoiceSPReportDTO.CustomerName, returnInvoiceSPReportDTO.CustomerAliasName, returnInvoiceSPReportDTO.SalesOrderNumber, returnInvoiceSPReportDTO.Location, returnInvoiceSPReportDTO.Warehouse, returnInvoiceSPReportDTO.KPN, returnInvoiceSPReportDTO.MPN, returnInvoiceSPReportDTO.IssuedBy);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ReturnInvoiceSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ReturnInvoiceSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    //var result = _mapper.Map<IEnumerable<ReturnInvoiceSPReportDTO>>(products);

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned ReturnInvoiceSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside ReturnInvoiceSPReport action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetReturnInvoiceNumberList()
        {
            ServiceResponse<IEnumerable<ReturnInvoiceNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<ReturnInvoiceNumberListDto>>();

            try
            {
                var returnOpenDeliveryOrderNumberList = await _returnInvoiceRepository.GetReturnInvoiceNumberList();
                if (returnOpenDeliveryOrderNumberList == null)
                {
                    _logger.LogError("ReturnInvoicenumber Not Found");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ReturnInvoicenumber Not Found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo("Returned ReturnInvoicenumber");
                    var result = _mapper.Map<IEnumerable<ReturnInvoiceNumberListDto>>(returnOpenDeliveryOrderNumberList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ReturnInvoicenumber Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong GetReturnInvoiceNumberList Details: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
