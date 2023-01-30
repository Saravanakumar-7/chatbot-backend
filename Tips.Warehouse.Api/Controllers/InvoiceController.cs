using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Repository;
using Newtonsoft.Json;


namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private IInvoiceRepository _invoiceRepository;
        private IBTODeliveryOrderItemsRepository _bTODeliveryOrderItemsRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public InvoiceController(IInvoiceRepository invoiceRepository, IBTODeliveryOrderItemsRepository bTODeliveryOrderItemsRepository, ILoggerManager logger, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _mapper = mapper;
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

                if (invoiceitemsDto != null)
                {
                    for (int i = 0; i < invoiceitemsDto.Count; i++)
                    {
                        InvoiceChildItem invoiceChildItem = _mapper.Map<InvoiceChildItem>(invoiceitemsDto[i]);
                        invoiceChildItemsDtoList.Add(invoiceChildItem);
                        string qty = Convert.ToString(invoiceChildItem.Qty);
                        var getAllInvoicesList = await _bTODeliveryOrderItemsRepository.UpdateBtoDelieveryOrderBalanceQty(invoiceChildItem.FGItemNumber, invoiceChildItem.DONumber, qty);
                        _bTODeliveryOrderItemsRepository.SaveAsync();
                    }
                }

                invoice.InvoiceChildItems = invoiceChildItemsDtoList;

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
                await _invoiceRepository.CreateInvoice(invoice);
                _invoiceRepository.SaveAsync();
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

