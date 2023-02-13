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
    public class InvoiceController : ControllerBase
    {
        private IInvoiceRepository _invoiceRepository;
        private IBTODeliveryOrderItemsRepository _bTODeliveryOrderItemsRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;


        public InvoiceController(IInvoiceRepository invoiceRepository, IInventoryTranctionRepository inventoryTranctionRepository, HttpClient httpClient, IConfiguration config, IBTODeliveryOrderItemsRepository bTODeliveryOrderItemsRepository, ILoggerManager logger, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _config = config;
            _bTODeliveryOrderItemsRepository = bTODeliveryOrderItemsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoice([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<InvoiceDto>> serviceResponse = new ServiceResponse<IEnumerable<InvoiceDto>>();

            try
            {
                var getAllInvoicesList = await _invoiceRepository.GetAllInvoices(pagingParameter);
                var metadata = new
                {
                    getAllInvoicesList.TotalCount,
                    getAllInvoicesList.PageSize,
                    getAllInvoicesList.CurrentPage,
                    getAllInvoicesList.HasNext,
                    getAllInvoicesList.HasPreviuos
                };
                
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<InvoiceDto>>(getAllInvoicesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Invoices";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(int id)
        {
            ServiceResponse<InvoiceDto> serviceResponse = new ServiceResponse<InvoiceDto>();

            try
            {
                var getInvoiceDetailById = await _invoiceRepository.GetInvoiceById(id);
                if (getInvoiceDetailById == null)
                {
                    _logger.LogError($"Invoice with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Invoice with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned Invoice with id: {id}");
                    var result = _mapper.Map<InvoiceDto>(getInvoiceDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InvoiceById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInvoicestById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task< IActionResult> CreateInvoice([FromBody] InvoicePostDto invoicePostDto)
        {
            ServiceResponse<InvoicePostDto> serviceResponse = new ServiceResponse<InvoicePostDto>();

            try
            {
                if (invoicePostDto == null)
                {
                    _logger.LogError("Invoice object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invoice object is null";
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
                //var invoiceChild = _mapper.Map<IEnumerable<InvoiceChildItem>>(invoicePostDto.InvoiceChildItems);
                //var invoice = _mapper.Map<Invoice>(invoicePostDto);
                //invoice.InvoiceChildItems = invoiceChild.ToList();

                var invoice = _mapper.Map<Invoice>(invoicePostDto);
                var invoiceitemsDto = invoicePostDto.InvoiceChildItems;

                var invoiceChildItemsDtoList = new List<InvoiceChildItem>();


                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                var newcount = await _invoiceRepository.GetInvoiceNumberAutoIncrementCount(date);

                if (newcount > 0)
                {
                    var number = newcount + 1;
                    string e = String.Format("{0:D4}", number);
                    invoice.InvoiceNumber = days + months + years + "IN" + (e);
                }
                else
                {
                    var count = 1;
                    var e = count.ToString("D4");
                    invoice.InvoiceNumber = days + months + years + "IN" + (e);
                }

                if (invoiceitemsDto != null)
                {
                    for (int i = 0; i < invoiceitemsDto.Count; i++)
                    {
                        InvoiceChildItem invoiceChildItem = _mapper.Map<InvoiceChildItem>(invoiceitemsDto[i]);
                        invoiceChildItemsDtoList.Add(invoiceChildItem);
                        string qty = Convert.ToString(invoiceChildItem.InvoicedQty);
                        var doNumber = invoiceitemsDto[i].DONumber;
                        var getAllInvoicesList = await _bTODeliveryOrderItemsRepository.UpdateBtoDelieveryOrderBalanceQty(invoiceChildItem.FGItemNumber, doNumber, qty);
                        _bTODeliveryOrderItemsRepository.SaveAsync();

                        //Add inventory Transaction Table

                        InventoryTranction inventoryTranction = new InventoryTranction();
                        inventoryTranction.PartNumber = invoiceChildItemsDtoList[i].FGItemNumber;
                        inventoryTranction.MftrPartNumber = invoiceChildItemsDtoList[i].FGItemNumber;
                        inventoryTranction.Description = "";
                        inventoryTranction.Issued_Quantity = invoiceChildItem.InvoicedQty;
                        inventoryTranction.UOM = invoiceChildItemsDtoList[i].UOM;
                        inventoryTranction.Issued_DateTime = DateTime.Now;
                        inventoryTranction.ReferenceID = invoice.InvoiceNumber;
                        inventoryTranction.ReferenceIDFrom = "Invoice Delivery Order";
                        inventoryTranction.Issued_By = "Admin";
                        inventoryTranction.CreatedOn = DateTime.Now; 
                        inventoryTranction.From_Location = "BTO";
                        inventoryTranction.TO_Location = "Invoice";
                        inventoryTranction.Remarks = "Create - Invoice";

                        var inventoryTransactions = _mapper.Map<InventoryTranction>(inventoryTranction);


                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransactions);
                        _inventoryTranctionRepository.SaveAsync();

                    }
                }

                invoice.InvoiceChildItems = invoiceChildItemsDtoList;

                await _invoiceRepository.CreateInvoice(invoice);
                _invoiceRepository.SaveAsync();

                //update balance qty and dispatch qty in salesorder table
                var btoDeliveryDispatchDetails = _mapper.Map<List<BtoDeliveryOrderInvoiceQtyDetailsDto>>(invoiceitemsDto);

                var json = JsonConvert.SerializeObject(btoDeliveryDispatchDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Concat(_config["SalesOrderAPI"], "InvoiceUpdateDispatchDetails"), data);


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
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceUpdateDto invoiceUpdateDto)
        {
            ServiceResponse<InvoiceUpdateDto> serviceResponse = new ServiceResponse<InvoiceUpdateDto>();

            try
            {
                if (invoiceUpdateDto is null)
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

                var getinvoiceDetailById = await _invoiceRepository.GetInvoiceById(id);
                if (getinvoiceDetailById is null)
                {
                    _logger.LogError($"Invoice with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var updateInvoice = _mapper.Map(invoiceUpdateDto, getinvoiceDetailById);

                string result = await _invoiceRepository.UpdateInvoice(updateInvoice);
                _logger.LogInfo(result);
                _invoiceRepository.SaveAsync();
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
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            ServiceResponse<InvoiceDto> serviceResponse = new ServiceResponse<InvoiceDto>();

            try
            {
                var deleteInvoiceDetail = await _invoiceRepository.GetInvoiceById(id);
                if (deleteInvoiceDetail == null)
                {
                    _logger.LogError($"Delete Invoice with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Invoice with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _invoiceRepository.DeleteInvoice(deleteInvoiceDetail);
                _logger.LogInfo(result);
                _invoiceRepository.SaveAsync();
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

