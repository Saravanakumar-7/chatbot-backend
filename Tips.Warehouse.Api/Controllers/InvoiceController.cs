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
        private ILoggerManager _logger;
        private IMapper _mapper;


        public InvoiceController(IInvoiceRepository invoiceRepository, ILoggerManager logger, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoice([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<InvoiceDto>> serviceResponse = new ServiceResponse<IEnumerable<InvoiceDto>>();

            try
            {
                var getAllInvoices = await _invoiceRepository.GetAllInvoice(pagingParameter);
                var metadata = new
                {
                    getAllInvoices.TotalCount,
                    getAllInvoices.PageSize,
                    getAllInvoices.CurrentPage,
                    getAllInvoices.HasNext,
                    getAllInvoices.HasPreviuos
                };
                
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<InvoiceDto>>(getAllInvoices);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
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
                var invoiceDetailById = await _invoiceRepository.GetInvoiceById(id);
                if (invoiceDetailById == null)
                {
                    _logger.LogError($"Invoice with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Invoice with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned Invoice with id: {id}");
                    var result = _mapper.Map<InvoiceDto>(invoiceDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
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
            ServiceResponse<InvoiceDto> serviceResponse = new ServiceResponse<InvoiceDto>();

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
                var invoiveList = _mapper.Map<IEnumerable<InvoiceChildItem>>(invoicePostDto.InvoiceChildItems);
                var invoiceEntity = _mapper.Map<Invoice>(invoicePostDto);
                invoiceEntity.InvoiceChildItems = invoiveList.ToList();


                await _invoiceRepository.CreateInvoice(invoiceEntity);
                _invoiceRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
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
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceDto InvoiceUpdateDto)
        {
            ServiceResponse<InvoiceDto> serviceResponse = new ServiceResponse<InvoiceDto>();

            try
            {
                if (InvoiceUpdateDto is null)
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

                var invoiceEntity = _mapper.Map(InvoiceUpdateDto, getinvoiceDetailById);

                string result = await _invoiceRepository.UpdateInvoice(invoiceEntity);
                _logger.LogInfo(result);
                _invoiceRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
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
                var getInvoiceDetailById = await _invoiceRepository.GetInvoiceById(id);
                if (getInvoiceDetailById == null)
                {
                    _logger.LogError($"Delete Invoice with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Invoice with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _invoiceRepository.DeleteInvoice(getInvoiceDetailById);
                _logger.LogInfo(result);
                _invoiceRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
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

