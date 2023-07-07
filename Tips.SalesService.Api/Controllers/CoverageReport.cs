using System.Net;
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
    public class CoverageReport : ControllerBase
    {
        private ICollectionTrackerRepository _repository;
        private ICoverageReportRepository _coverageRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public CoverageReport(ICollectionTrackerRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetCoverageReport([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        //{
        //    ServiceResponse<List<GetCoverageReportDto>> serviceResponse = new ServiceResponse<List<GetCoverageReportDto>>();

        //    try
        //    {
        //        var getAllForecastLpCosting = await _coverageRepository.GetAllSalesOrderDetails();
               
        //        _logger.LogInfo("Returned Coverage Report");
        //        var result = _mapper.Map<IEnumerable<GetCoverageReportDto>>(getAllForecastLpCosting);
        //        serviceResponse.Data = getAllForecastLpCosting;
        //        serviceResponse.Message = "Returned Coverage Report Successfully";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
        // Get all ForeCastEngg 
    }
}
