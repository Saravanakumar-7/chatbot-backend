using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Tally.Api.Contracts;
using Tips.Tally.Api.Entities.DTOs;

namespace Tips.Tally.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TallyController : ControllerBase
    {
        private ITallyRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IConfiguration _config;       

        public TallyController(ITallyRepository repository, ILoggerManager logger, IMapper mapper, IConfiguration config)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _config = config;
        }
        [HttpGet()] // Adjust your route as needed
        public async Task<IActionResult> GetTallyVendorMasterSpReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<TallyVendorMasterSpReport>> serviceResponse = new ServiceResponse<IEnumerable<TallyVendorMasterSpReport>>();
            try
            {
                var result = await _repository.GetTallyVendorMasterSpReportWithDate(FromDate, ToDate);

                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"TallyVendorMasterSpReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"TallyVendorMasterSpReport hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned TallyVendorMasterSpReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetTallyVendorMasterSpReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetTallyVendorMasterSpReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
