using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConceptionController : ControllerBase
    {
        private IInvoiceRepository _invoiceRepository;
        private IBTODeliveryOrderRepository _bTODeliveryOrderRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public ConceptionController(IBTODeliveryOrderRepository bTODeliveryOrderRepository, IHttpClientFactory clientFactory, IInvoiceRepository invoiceRepository, HttpClient httpClient, IConfiguration config, ILoggerManager logger, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _bTODeliveryOrderRepository = bTODeliveryOrderRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GenerateConceptionReport([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {

            ServiceResponse<List<ConceptionReportDto>> serviceResponse = new ServiceResponse<List<ConceptionReportDto>>();

            try
            {
                List<ConceptionReportDto> openSalesCoverageReports = await ConceptionReport(FromDate, ToDate);

                serviceResponse.Data = openSalesCoverageReports;
                serviceResponse.Message = $"Returned ConceptionReport Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GenerateConceptionReport action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task<List<ConceptionReportDto>> ConceptionReport(DateTime? FromDate, DateTime? ToDate)
        {
            List<ConceptionReportDto> conceptionReportList = new List<ConceptionReportDto>();
            try
            {
                var invoiceDetails = await _invoiceRepository.GetInvoiceDetialsbyDate(FromDate, ToDate);

                List<string?> doNumberList = invoiceDetails
                              .SelectMany(i => i.invoiceItemConceptionDtos) 
                              .Select(item => item.DONumber) 
                              .ToList();  


                var doDetails = await _bTODeliveryOrderRepository.GetDoDetailsbyDoNumbers(doNumberList);

                List<string?> soNumberList = doDetails.Select(x =>x.SalesOrderNumber).ToList();

                //List<OpenPoQuantityDto> openPoQtyList = await GetOpenPoQtyForChildItemsByProjectNo(itemNoListString, projectNumber);


            }
            catch (Exception ex)
            {  

            }
            return conceptionReportList;
        }

    }
}
