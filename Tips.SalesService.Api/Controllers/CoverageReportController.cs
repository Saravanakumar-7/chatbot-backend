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
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;


namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CoverageReportController : ControllerBase
    {
        private ICollectionTrackerRepository _repository;
        private ICoverageReportRepository _coverageRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public CoverageReportController(ICollectionTrackerRepository repository, HttpClient httpClient, IConfiguration config,ICoverageReportRepository coverageReportRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _coverageRepository = coverageReportRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetCoverageReport()
        {
            ServiceResponse<IEnumerable<CoverageReportDto>> serviceResponse = new ServiceResponse<IEnumerable<CoverageReportDto>>();

            try
            {
                var getAllForecastLpCosting = await _coverageRepository.GetAllSalesOrderDetails();

                _logger.LogInfo("Returned Coverage Report");
                var result = _mapper.Map<List<CoverageReportDto>>(getAllForecastLpCosting);
                //bom details
                var json = JsonConvert.SerializeObject(result);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "CoverageEnggBomChildDetails"), data);
                
                //var inventoryObjectResult = await _httpClient.PostAsync(string.Concat(_config["ItemMasterAPI"], "GetItemsRoutingDetailsForLpCosting"), content);
                //var itemsRoutingDetailsJsonString = await inventoryObjectResult.Content.ReadAsStringAsync();
                //dynamic itemsRoutingDetailsJson = JsonConvert.DeserializeObject(itemsRoutingDetailsJsonString);
                //var data = itemsRoutingDetailsJson.data;

                //serviceResponse.Data = result;
                serviceResponse.Message = "Returned Coverage Report Successfully";
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
        // Get all ForeCastEngg 
    }
}
