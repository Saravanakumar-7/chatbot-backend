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

    }
}
