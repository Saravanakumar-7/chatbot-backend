using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Entities;

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


        public ReturnInvoiceController(IReturnInvoiceRepository returnInvoiceRepository, ILoggerManager logger, IMapper mapper)
        {
            _returnInvoiceRepository = returnInvoiceRepository;
            _logger = logger;
            _mapper = mapper;
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
                var invoiceEntity = _mapper.Map<ReturnInvoice>(ReturnInvoiceDtoPost);
                invoiceEntity.ReturnInvoiceItems = returnInvoiceItem.ToList();


                await _returnInvoiceRepository.CreateReturnInvoice(invoiceEntity);
                _returnInvoiceRepository.SaveAsync();
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
