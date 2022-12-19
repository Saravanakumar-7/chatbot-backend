using AutoMapper;
using Tips.SalesService.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Repository;
using Contracts;
using Entities.DTOs;
using Entities;
using Entities.Helper;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Build.Framework;
using NuGet.Protocol.Core.Types;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ForeCastController : ControllerBase
    {
        private IForecastSourcingRepository _sourcingrepository;
        private IForeCastCustomerSupportRepository _repository;
        private IForeCastCustomerSupportItemRepository _itemRepository;
        private IForeCastRepository  _Forecastrepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IForeCastEnggRepository _forecastenggRepository;
        private IForecastLpCostingRepository _lpcostingRepository;
        public ForeCastController(IForeCastRepository foreCastRepository, IForecastSourcingRepository forecastSourcingRepository, IForecastLpCostingRepository forecastLpCostingRepository, IForeCastEnggRepository foreCastEnggRepository, IForeCastCustomerSupportRepository foreCastCustomerSupportRepository,IForeCastCustomerSupportItemRepository foreCastCustomerSupportItemRepository ,ILoggerManager logger, IMapper mapper)
        {
           _Forecastrepository= foreCastRepository;
            _logger = logger;
            _mapper = mapper;
            _itemRepository = foreCastCustomerSupportItemRepository;
            _repository = foreCastCustomerSupportRepository;
            _forecastenggRepository = foreCastEnggRepository;
            _sourcingrepository = forecastSourcingRepository;
            _lpcostingRepository = forecastLpCostingRepository;
        }
        // GET: api/<ForeCastController>
        [HttpGet]
        public async Task<IActionResult> GetAllForeCast([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ForeCastDto>> serviceResponse = new ServiceResponse<IEnumerable<ForeCastDto>>();

            try
            {
                var listOfforecast = await _Forecastrepository.GetAllForeCast(pagingParameter);
                var metadata = new
                {
                    listOfforecast.TotalCount,
                    listOfforecast.PageSize,
                    listOfforecast.CurrentPage,
                    listOfforecast.HasNext,
                    listOfforecast.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ForeCast");
                var result = _mapper.Map<IEnumerable<ForeCastDto>>(listOfforecast);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all forecasts Successfully";
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
        // Getall Lpcosting
        [HttpGet]
        public async Task<IActionResult> GetAllForeCastLpCosting([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ForecastLpCostingDto>> serviceResponse = new ServiceResponse<IEnumerable<ForecastLpCostingDto>>();

            try
            {
                var listOfforecastLPCosting = await _lpcostingRepository.GetAllForecastLpCosting(pagingParameter);
                var metadata = new
                {
                    listOfforecastLPCosting.TotalCount,
                    listOfforecastLPCosting.PageSize,
                    listOfforecastLPCosting.CurrentPage,
                    listOfforecastLPCosting.HasNext,
                    listOfforecastLPCosting.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ForeCastLpcosting");
                var result = _mapper.Map<IEnumerable<ForecastLpCostingDto>>(listOfforecastLPCosting);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ForecastLpCosting Successfully";
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
        [HttpGet]
        public async Task<IActionResult> GetAllForeCastEngg([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ForeCastEnggDto>> serviceResponse = new ServiceResponse<IEnumerable<ForeCastEnggDto>>();

            try
            {
                var listOfforecastengg = await _forecastenggRepository.GetAllForeCastEngg(pagingParameter);
                var metadata = new
                {
                    listOfforecastengg.TotalCount,
                    listOfforecastengg.PageSize,
                    listOfforecastengg.CurrentPage,
                    listOfforecastengg.HasNext,
                    listOfforecastengg.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ForeCastEngg");
                var result = _mapper.Map<IEnumerable<ForeCastEnggDto>>(listOfforecastengg);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ForeCastEngg Successfully";
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
        //get Forecastsourcing
        [HttpGet]
        public async Task<IActionResult> GetAllForeCastSourcings([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ForecastSourcingDto>> serviceResponse = new ServiceResponse<IEnumerable<ForecastSourcingDto>>();

            try
            {
                var listOfforecastSourcing = await _sourcingrepository.GetAllForeCastSourcing(pagingParameter);
                var metadata = new
                {
                    listOfforecastSourcing.TotalCount,
                    listOfforecastSourcing.PageSize,
                    listOfforecastSourcing.CurrentPage,
                    listOfforecastSourcing.HasNext,
                    listOfforecastSourcing.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all listOfforecastSourcing");
                var result = _mapper.Map<IEnumerable<ForecastSourcingDto>>(listOfforecastSourcing);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ForeCastSourcing Successfully";
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
        [HttpGet]
        public async Task<IActionResult> GetAllForeCastCustomerSupport([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ForeCastCustomerSupportDto>> serviceResponse = new ServiceResponse<IEnumerable<ForeCastCustomerSupportDto>>();

            try
            {
                var listOfforecast = await _repository.GetAllForeCastCustomerSupports(pagingParameter);
                var metadata = new
                {
                    listOfforecast.TotalCount,
                    listOfforecast.PageSize,
                    listOfforecast.CurrentPage,
                    listOfforecast.HasNext,
                    listOfforecast.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ForeCastCustomerSupport");
                var result = _mapper.Map<IEnumerable<ForeCastCustomerSupportDto>>(listOfforecast);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ForeCastCustomerSupport Successfully";
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
        //get ForecastEngg by ForeCastnumber
        [HttpGet("{ForeCastNumber}")]
        public async Task<IActionResult> GetForeCastEnggByForeCastNumber(string ForeCastNumber)
        {
            ServiceResponse<ForeCastEnggDto> serviceResponse = new ServiceResponse<ForeCastEnggDto>();

            try
            {
                var forecastEngg = await _forecastenggRepository.ForeCastEnggByForeCastNumber(ForeCastNumber);

                if (forecastEngg == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForecastEnggByForeCastNumber with id: {ForeCastNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForecastEnggByForeCastNumber with id: {ForeCastNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ForecastEnggByForeCastNumber with id: {ForeCastNumber}");

                    ForeCastEnggDto forecstenggDto = _mapper.Map<ForeCastEnggDto>(forecastEngg);

                    List<ForeCastEnggItemsDto> forecastenggItemsDtos = new List<ForeCastEnggItemsDto>();
                    foreach (var itemDetails in forecastEngg.foreCastEnggItems)
                    {
                        ForeCastEnggItemsDto forecastenggItemDto = _mapper.Map<ForeCastEnggItemsDto>(itemDetails);
                        forecastenggItemsDtos.Add(forecastenggItemDto);
                    }
                    forecstenggDto.foreCastEnggItems = forecastenggItemsDtos;

                    //var result = _mapper.Map<RfqCustomerSupportDto>(rfq);
                    serviceResponse.Data = forecstenggDto;
                    serviceResponse.Message = $"Returned ForecastEnggByForeCastNumber with id: {ForeCastNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ForecastEnggByForeCastNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //LpCostingGetByForecastNumber
        [HttpGet("{ForeCastNumber}")]
        public async Task<IActionResult> GetForeCastLpCostingByForeCastNumber(string ForeCastNumber)
        {
            ServiceResponse<ForecastLpCostingDto> serviceResponse = new ServiceResponse<ForecastLpCostingDto>();

            try
            {
                var forelpcosting = await _lpcostingRepository.ForecastLpCostingByForeCastNumber(ForeCastNumber);

                if (forelpcosting == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForecastLpCostingByForecastNumber with id: {ForeCastNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForecastLpCostingByForecastNumber with id: {ForeCastNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ForecastLpCostingByForecastNumber with id: {forelpcosting}");

                    ForecastLpCostingDto forecastLpCostingDto = _mapper.Map<ForecastLpCostingDto>(forelpcosting);

                    List<ForecastLpCostingItemDto> forecastLpCostingItemDtos = new List<ForecastLpCostingItemDto>();
                    foreach (var itemDetails in forelpcosting.forecastLpCostingItems)
                    {
                        ForecastLpCostingItemDto forecastLpCostingItemDto = _mapper.Map<ForecastLpCostingItemDto>(itemDetails);
                        forecastLpCostingItemDto.forecastLpCostingProcesses = _mapper.Map<List<ForecastLpCostingProcessDto>>(itemDetails.forecastLpCostingProcesses);
                        forecastLpCostingItemDto.forecastLpCostingNREConsumables = _mapper.Map<List<ForecastLpCostingNREConsumableDto>>(itemDetails.forecastLPCostingNREConsumables);
                        forecastLpCostingItemDto.forecastLpCostingOtherCharges = _mapper.Map<List<ForecastLpCostingOtherChargesDto>>(forecastLpCostingItemDto.forecastLpCostingOtherCharges);

                        forecastLpCostingItemDtos.Add(forecastLpCostingItemDto);
                    }
                    forecastLpCostingDto.forecastLpCostingItems = forecastLpCostingItemDtos;

                    //var result = _mapper.Map<RfqCustomerSupportDto>(rfq);
                    serviceResponse.Data = forecastLpCostingDto;
                    serviceResponse.Message = $"Returned ForecastNumber with id: {ForeCastNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside forecastByForecastNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    

    [HttpGet("{ForeCastNumber}")]
        public async Task<IActionResult> ForeCastCustomerSupportByForeCastNumber(string ForeCastNumber)
        {
            ServiceResponse<ForeCastCustomerSupportDto> serviceResponse = new ServiceResponse<ForeCastCustomerSupportDto>();

            try
            {
                var forecast = await _repository.ForeCastCustomerSupportByForeCastNumber(ForeCastNumber);

                if (forecast == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForecastCustomerSupportByForeCastNumber with id: {ForeCastNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForecastCustomerSupportByForeCastNumber with id: {ForeCastNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ForecastCustomerSupportByForeCastNumber with id: {ForeCastNumber}");

                    ForeCastCustomerSupportDto forecastDto = _mapper.Map<ForeCastCustomerSupportDto>(forecast);

                    List<ForeCastCustomerSupportItemDto> forecastItemsDtos = new List<ForeCastCustomerSupportItemDto>();
                    foreach (var itemDetails in forecast.foreCastCustomerSupportItems)
                    {
                        ForeCastCustomerSupportItemDto forecastItemDto = _mapper.Map<ForeCastCustomerSupportItemDto>(itemDetails);
                        forecastItemDto.foreCastCSDeliverySchedule = _mapper.Map<List<ForeCastCSDeliveryScheduleDto>>(itemDetails.foreCastCSDeliverySchedule);
                        forecastItemsDtos.Add(forecastItemDto);
                    }
                    forecastDto.foreCastCustomerSupportItems = forecastItemsDtos;

                    //var result = _mapper.Map<RfqCustomerSupportDto>(rfq);
                    serviceResponse.Data = forecastDto;
                    serviceResponse.Message = $"Returned ForecastCustomerSupportByForeCastNumber with id: {ForeCastNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ForecastCustomerSupportByForeCastNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }


        }
        // forecastEngg GetbyId
        [HttpGet("{id}")]
        public async Task<IActionResult> GetForeCastEnggById(int id)
        {
            ServiceResponse<ForeCastEnggDto> serviceResponse = new ServiceResponse<ForeCastEnggDto>();

            try
            {
                var forecastengg = await _forecastenggRepository.GetForeCastEnggById(id);

                if (forecastengg == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForecastEngg with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForecastEngg with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ForecastEngg with id: {id}");
                    var result = _mapper.Map<ForeCastEnggDto>(forecastengg);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned ForecastEngg with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetForeCastEnggById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        //Get by Id ForecastSourcing
        [HttpGet("{id}")]
        public async Task<IActionResult> GetForeCastSourcingById(int id)
        {
            ServiceResponse<ForecastSourcingDto> serviceResponse = new ServiceResponse<ForecastSourcingDto>();

            try
            {
                var forecastsourcing = await _sourcingrepository.GetForeCastSourcingById(id);

                if (forecastsourcing == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForeCastsourcing with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Forecastsourcing with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ForeCastsourcing with id: {id}");
                    //var result = _mapper.Map<RfqSourcingDto>(rfqsourcing);
                    ForecastSourcingDto forecastSourceDto = _mapper.Map<ForecastSourcingDto>(forecastsourcing);//Main model mapping

                    //below mapping is child under child  

                    List<ForecastSourcingItemsDto> forecastSourseItemDtos = new List<ForecastSourcingItemsDto>();

                    foreach (var sourceitemDetails in forecastsourcing.forecastSourcingItems)
                    {
                        ForecastSourcingItemsDto forecastSourceItemDto = _mapper.Map<ForecastSourcingItemsDto>(sourceitemDetails);
                        forecastSourceItemDto.forecastSourcingVendors = _mapper.Map<List<ForecastSourcingVendorDto>>(sourceitemDetails.forecastSourcingVendors);
                        forecastSourseItemDtos.Add(forecastSourceItemDto);
                    }

                    forecastSourceDto.forecastSourcingItems = forecastSourseItemDtos;
                    serviceResponse.Data = forecastSourceDto;
                    serviceResponse.Message = $"Returned ForeCastsourcing with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetForeCatsourcingById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        //GetbyId LPcosting
        [HttpGet("{id}")]
        public async Task<IActionResult> GetForeCastLpCostingById(int id)
        {
            ServiceResponse<ForecastLpCostingDto> serviceResponse = new ServiceResponse<ForecastLpCostingDto>();

            try
            {
                var forecastlpcosting = await _lpcostingRepository.GetForecastLpCostingById(id);

                if (forecastlpcosting == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"forecastlpcosting with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"forecastlpcosting with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned forecastlpcosting with id: {id}");
                    //var result = _mapper.Map<RfqLPCostingDto>(rfqlpcosting);
                    ForecastLpCostingDto forecastLpCostingDto = _mapper.Map<ForecastLpCostingDto>(forecastlpcosting);//Main model mapping

                    //below mapping is child under child  

                    List<ForecastLpCostingItemDto> forecastLpCostingItemDtos = new List<ForecastLpCostingItemDto>();

                    foreach (var lpcostingitemDetails in forecastlpcosting.forecastLpCostingItems)
                    {
                        ForecastLpCostingItemDto forecastLpCostingItemDto = _mapper.Map<ForecastLpCostingItemDto>(lpcostingitemDetails);
                        forecastLpCostingItemDto.forecastLpCostingProcesses = _mapper.Map<List<ForecastLpCostingProcessDto>>(forecastLpCostingItemDto.forecastLpCostingProcesses);
                        forecastLpCostingItemDto.forecastLpCostingNREConsumables = _mapper.Map<List<ForecastLpCostingNREConsumableDto>>(forecastLpCostingItemDto.forecastLpCostingNREConsumables);
                        forecastLpCostingItemDto.forecastLpCostingOtherCharges = _mapper.Map<List<ForecastLpCostingOtherChargesDto>>(forecastLpCostingItemDto.forecastLpCostingOtherCharges);

                        forecastLpCostingItemDtos.Add(forecastLpCostingItemDto);
                    }

                    forecastLpCostingDto.forecastLpCostingItems = forecastLpCostingItemDtos;
                    serviceResponse.Data = forecastLpCostingDto;
                    serviceResponse.Message = $"Returned forecastLpCosting with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside getforecastLPCostingById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // GET api/<ForeCastController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetForeCastById(int id)
        {
            ServiceResponse<ForeCastDto> serviceResponse = new ServiceResponse<ForeCastDto>();

            try
            {
                var forecast = await _Forecastrepository.GetForeCastById(id);

                if (forecast == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Forecast with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Forecast with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned forecast with id: {id}");
                    var result = _mapper.Map<ForeCastDto>(forecast);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned Forecast with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetForeCastById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetForeCastCustomerSupportById(int id)
        {
            ServiceResponse<ForeCastCustomerSupportDto> serviceResponse = new ServiceResponse<ForeCastCustomerSupportDto>();

            try
            {
                var forecast = await _repository.GetForeCastCustomerSupportById(id);

                if (forecast == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForeCastCustomerSupport with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForeCastCustomerSupport with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned ForeCastCustomerSupport with id: {id}");

                    ForeCastCustomerSupportDto forecastDto = _mapper.Map<ForeCastCustomerSupportDto>(forecast);

                    List<ForeCastCustomerSupportItemDto> forecastItemsDtos = new List<ForeCastCustomerSupportItemDto>();
                    foreach (var itemDetails in forecast.foreCastCustomerSupportItems)
                    {
                        ForeCastCustomerSupportItemDto forecastItemDto = _mapper.Map<ForeCastCustomerSupportItemDto>(itemDetails);
                        forecastItemDto.foreCastCSDeliverySchedule = _mapper.Map<List<ForeCastCSDeliveryScheduleDto>>(itemDetails.foreCastCSDeliverySchedule);
                        forecastItemsDtos.Add(forecastItemDto);
                    }
                    forecastDto.foreCastCustomerSupportItems = forecastItemsDtos;

                    //var result = _mapper.Map<RfqCustomerSupportDto>(rfq);
                    serviceResponse.Data = forecastDto;
                    serviceResponse.Message = $"Returned ForecastCustomerSupport with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetForeCastCustomerSupportById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpPut]
        public async Task<IActionResult> UpdateForeCastCustomerSupportRelease([FromBody] List<int> itemIds)
        {
            ServiceResponse<ForeCastCustomerSupportDto> serviceResponse = new ServiceResponse<ForeCastCustomerSupportDto>();

            try
            {
                if (itemIds is null)
                {
                    _logger.LogError("ForeCastCustomerSupport Itemid object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ForeCastCustomerSupport Itemid object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                foreach (var id in itemIds)
                {
                    if (id == null)
                    {
                        _logger.LogError($"ForeCastCustomerSupport with item id: {id}, hasn't been found in db.");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Update ForeCastCustomerSupport with item id: {id}, hasn't been found in db.";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(serviceResponse);
                    }

                    var forecastCustomerSupport = await _itemRepository.GetForeCastCustomerSupportItemById(id);
                    forecastCustomerSupport.ReleaseStatus = true;
                    string result = await _itemRepository.ActivateForeCastCustomerSupportItemById(forecastCustomerSupport);
                    _logger.LogInfo(result);
                    _repository.SaveAsync();
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "Forecast CustomerSupport Release Activated Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateForeCast action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Create forecastsourcing
        [HttpPost]
        public async Task<IActionResult> CreateForeCastSourcing([FromBody] ForecastSourcingDtoPost forecastSourcingDtoPost)
        {
            ServiceResponse<ForecastSourcingDto> serviceResponse = new ServiceResponse<ForecastSourcingDto>();

            try
            {
                if (forecastSourcingDtoPost is null)
                {
                    _logger.LogError("ForeCastSourcing object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ForeCastSourcing object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ForeCastSourcing object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ForeCastSourcing object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                //var sourceitems = _mapper.Map<IEnumerable<RfqSourcingItems>>(rfqSourcingDtoPost.rfqSourcingItems);
                var forecastsource = _mapper.Map<ForecastSourcing>(forecastSourcingDtoPost);
                var forecastSourceDto = forecastSourcingDtoPost.forecastSourcingItems;

                var sourceItemList = new List<ForecastSourcingItems>();
                for (int i = 0; i < forecastSourceDto.Count; i++)
                {
                    ForecastSourcingItems sourceItemListDetail = _mapper.Map<ForecastSourcingItems>(forecastSourceDto[i]);
                    sourceItemListDetail.forecastSourcingVendors = _mapper.Map<List<ForecastSourcingVendor>>(forecastSourceDto[i].forecastSourcingVendors);
                    sourceItemList.Add(sourceItemListDetail);

                }
                forecastsource.forecastSourcingItems = sourceItemList;

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                _sourcingrepository.CreateForeCastSourcing(forecastsource);

                _sourcingrepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetForeCastSourceById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateForeCastSource action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Create Lpcosting
        [HttpPost]
        public async Task<IActionResult> CreateForeCastLpCosting([FromBody] ForecastLPCostingDtoPost forecastLPCostingDtoPost)
        {
            ServiceResponse<ForecastLpCostingDto> serviceResponse = new ServiceResponse<ForecastLpCostingDto>();

            try
            {
                if (forecastLPCostingDtoPost is null)
                {
                    _logger.LogError("forecastLPCosting object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "forecastLPCosting object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid forecastLPCosting object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid forecastLPCosting object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var forecastLp = _mapper.Map<ForecastLpCosting>(forecastLPCostingDtoPost);
                var foreLPCostingDto = forecastLPCostingDtoPost.forecastLPCostingItems;

                var lpcostingItemList = new List<ForecastLpCostingItem>();
                for (int i = 0; i < foreLPCostingDto.Count; i++)
                {
                    ForecastLpCostingItem lpcostingItemListDetail = _mapper.Map<ForecastLpCostingItem>(foreLPCostingDto[i]);
                    lpcostingItemListDetail.forecastLpCostingProcesses = _mapper.Map<List<ForecastLpCostingProcess>>(foreLPCostingDto[i].forecastLPCostingProcesses);
                    lpcostingItemListDetail.forecastLPCostingNREConsumables = _mapper.Map<List<ForecastLPCostingNREConsumable>>(foreLPCostingDto[i].forecastLPCostingNREConsumables);
                    lpcostingItemListDetail.forecastLpCostingOtherCharges = _mapper.Map<List<ForecastLpCostingOtherCharges>>(foreLPCostingDto[i].forecastLPCostingOtherCharges);
                    lpcostingItemList.Add(lpcostingItemListDetail);

                }
                forecastLp.forecastLpCostingItems = lpcostingItemList;


                _lpcostingRepository.CreateForecastLpCosting(forecastLp);
                _lpcostingRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetForecastLPCostingById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateForecastLPCostong action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);


            }
        }
        // POST api/<ForeCastController>
        [HttpPost]
        public async Task<IActionResult> CreateForeCast([FromBody] ForeCastPostDto foreCastPostDto)
        {
            ServiceResponse<ForeCastDto> serviceResponse = new ServiceResponse<ForeCastDto>();

            try
            {
                if (foreCastPostDto is null)
                {
                    _logger.LogError("ForeCast object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ForeCast object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ForeCast object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ForeCast object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var forecasts = _mapper.Map<ForeCast>(foreCastPostDto);

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                _Forecastrepository.CreateForeCast(forecasts);

                _Forecastrepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetForecastById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateForecast action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Create forecastEngg
        [HttpPost]
        public async Task<IActionResult> CreateForeCastEngg([FromBody] ForeCastEnggPostDto foreCastEnggPost)
        {
            ServiceResponse<ForeCastEnggDto> serviceResponse = new ServiceResponse<ForeCastEnggDto>();

            try
            {
                if (foreCastEnggPost is null)
                {
                    _logger.LogError("ForecastEngg object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ForecastEngg object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ForecastEngg object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ForecastEngg object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var forecastenggs = _mapper.Map<ForeCastEngg>(foreCastEnggPost);

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                _forecastenggRepository.CreateForeCastEngg(forecastenggs);
                _forecastenggRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetForeCastById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateForeCastEngg action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateForeCastCustomerSupport([FromBody] ForeCastCustomerSupportPostDto foreCastCustomerSupportPostDto)
        {
            ServiceResponse<ForeCastCustomerSupportDto> serviceResponse = new ServiceResponse<ForeCastCustomerSupportDto>();

            try
            {
                if (foreCastCustomerSupportPostDto is null)
                {
                    _logger.LogError("ForeCastCustomerSupport object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ForeCastCustomerSupport object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ForeCastCustomerSupport object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ForeCastCustomerSupport object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }               

                var forecastCustomerSupportList = _mapper.Map<ForeCastCustomerSupport>(foreCastCustomerSupportPostDto);


                var forecastCustomerSupportItemDto = foreCastCustomerSupportPostDto.foreCastCustomerSupportItems;

                var forecastCustomerSupportLists = new List<ForeCastCustomerSupportItem>();
                for (int i = 0; i < forecastCustomerSupportItemDto.Count; i++)
                {
                    ForeCastCustomerSupportItem forecastCSItems = _mapper.Map<ForeCastCustomerSupportItem>(forecastCustomerSupportItemDto[i]);
                    forecastCSItems.foreCastCSDeliverySchedule = _mapper.Map<List<ForeCastCSDeliverySchedule>>(forecastCustomerSupportItemDto[i].foreCastCSDeliverySchedule);
                    forecastCustomerSupportLists.Add(forecastCSItems);

                }
                forecastCustomerSupportList.foreCastCustomerSupportItems = forecastCustomerSupportLists;

                _repository.CreateForeCastCustomerSupport(forecastCustomerSupportList);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetForeCastCustomerSupportById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateForeCastCustomerSupport action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Update lpcosting
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForeCastLpCosting(int id, [FromBody] ForecastLPCostingDtoUpdate forecastLPCostingDtoUpdate)
        {
            ServiceResponse<ForecastLpCostingDto> serviceResponse = new ServiceResponse<ForecastLpCostingDto>();

            try
            {
                if (forecastLPCostingDtoUpdate is null)
                {
                    _logger.LogError("ForecastLpCosting object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ForecastLpCosting object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ForecastLpCosting object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ForecastLpCosting object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var forecastlpcosting = await _lpcostingRepository.GetForecastLpCostingById(id);
                if (forecastlpcosting is null)
                {
                    _logger.LogError($"forecastlpcosting with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update forecastlpcosting with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var forelpcostingList = _mapper.Map<ForecastLpCosting>(forecastLPCostingDtoUpdate);

                var lpcostingitemDto = forecastLPCostingDtoUpdate.forecastLPCostingItems;

                var forecastlpcostingitemList = new List<ForecastLpCostingItem>();
                for (int i = 0; i < lpcostingitemDto.Count; i++)
                {
                    ForecastLpCostingItem lpcostingItemDetail = _mapper.Map<ForecastLpCostingItem>(lpcostingitemDto[i]);
                    lpcostingItemDetail.forecastLpCostingProcesses = _mapper.Map<List<ForecastLpCostingProcess>>(lpcostingitemDto[i].forecastLPCostingProcesses);
                    lpcostingItemDetail.forecastLPCostingNREConsumables = _mapper.Map<List<ForecastLPCostingNREConsumable>>(lpcostingitemDto[i].forecastLPCostingNREConsumables);
                    lpcostingItemDetail.forecastLpCostingOtherCharges = _mapper.Map<List<ForecastLpCostingOtherCharges>>(lpcostingitemDto[i].forecastLPCostingOthers);

                    forecastlpcostingitemList.Add(lpcostingItemDetail);

                }
                var data = _mapper.Map(forecastLPCostingDtoUpdate, forecastlpcosting);

                string result = await _lpcostingRepository.UpdateForecastLpCosting(data);
                _logger.LogInfo(result);
                _lpcostingRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateForecastLPCosting action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // PUT api/<ForeCastController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForeCast(int id, [FromBody] ForeCastUpdateDto foreCastUpdateDto)
        {
            ServiceResponse<ForeCastDto> serviceResponse = new ServiceResponse<ForeCastDto>();

            try
            {
                if (foreCastUpdateDto is null)
                {
                    _logger.LogError("Forecast object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Forecast object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Forecast object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Forecast object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var forecast = await _Forecastrepository.GetForeCastById(id);
                if (forecast is null)
                {
                    _logger.LogError($"Forecast with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Forecast with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var data = _mapper.Map(foreCastUpdateDto, forecast);


                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                string result = await _Forecastrepository.UpdateForeCast(data);
                _logger.LogInfo(result);
                _Forecastrepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateForecast action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Update ForeCastSourcing
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForeCastSourcing(int id, [FromBody] ForecastSourcingDtoUpdate forecastSourcingDtoUpdate)
        {
            ServiceResponse<ForecastSourcingDto> serviceResponse = new ServiceResponse<ForecastSourcingDto>();

            try
            {
                if (forecastSourcingDtoUpdate is null)
                {
                    _logger.LogError("ForeCastSourcing object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ForeCastSourcing object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ForeCastSourcing object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ForeCastSourcing object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var forecastsource = await _sourcingrepository.GetForeCastSourcingById(id);
                if (forecastsource is null)
                {
                    _logger.LogError($"ForeCastSource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update ForeCastSource with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                //_mapper.Map(rfqUpdateDto, rfq);
                //var rfqsourceing = _mapper.Map<IEnumerable<RfqSourcingItems>>(rfqsource.rfqSourcingItems);
                var forecastsourceList = _mapper.Map<ForecastSourcing>(forecastSourcingDtoUpdate);

                var sourceitemtemDto = forecastSourcingDtoUpdate.forecastSourcingItems;

                var sourceItemList = new List<ForecastSourcingItems>();
                for (int i = 0; i < sourceitemtemDto.Count; i++)
                {
                    ForecastSourcingItems sourceItemDetail = _mapper.Map<ForecastSourcingItems>(sourceitemtemDto[i]);
                    sourceItemDetail.forecastSourcingVendors = _mapper.Map<List<ForecastSourcingVendor>>(sourceitemtemDto[i].forecastSourcingVendors);

                    sourceItemList.Add(sourceItemDetail);

                }
                var data = _mapper.Map(forecastSourcingDtoUpdate, forecastsource);

                data.forecastSourcingItems = sourceItemList;

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                string result = await _sourcingrepository.UpdateForeCastSourcing(data);
                _logger.LogInfo(result);
                _sourcingrepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateForeCastSourcing action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // Update ForeCastengg
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForeCastEngg(int id, [FromBody] ForeCastEnggUpdateDto foreCastEnggUpdate)
        {
            ServiceResponse<ForeCastEnggDto> serviceResponse = new ServiceResponse<ForeCastEnggDto>();

            try
            {
                if (foreCastEnggUpdate is null)
                {
                    _logger.LogError("ForeCastEngg object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ForeCastEngg object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ForeCastEngg object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ForeCastEngg object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var forecastengg = await _forecastenggRepository.GetForeCastEnggById(id);
                if (forecastengg is null)
                {
                    _logger.LogError($"forecastengg with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update forecastengg with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var data = _mapper.Map(foreCastEnggUpdate, forecastengg);


                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                string result = await _forecastenggRepository.UpdateForeCastEngg(data);
                _logger.LogInfo(result);
                _forecastenggRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateForeCastEngg action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForeCastCustomerSupport(int id, [FromBody] ForeCastCustomerSupportUpdateDto foreCastCustomerSupportUpdateDto)
        {
            ServiceResponse<ForeCastCustomerSupportDto> serviceResponse = new ServiceResponse<ForeCastCustomerSupportDto>();

            try
            {
                if (foreCastCustomerSupportUpdateDto is null)
                {
                    _logger.LogError("ForeCastCustomerSupport object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ForeCsatCustomerSupport object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ForeCastCustomerSupport object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ForecastCustomerSupport object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var forecast = await _repository.GetForeCastCustomerSupportById(id);
                if (forecast is null)
                {
                    _logger.LogError($"ForeCastCustomerSupport with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update ForeCastCustomerSupport with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
               
                var forecastcustomerlist = _mapper.Map<ForeCastCustomerSupport>(forecast);

                var forecastItemDto = foreCastCustomerSupportUpdateDto.foreCastCustomerSupportItems;

                var forecastCsItemList = new List<ForeCastCustomerSupportItem>();
                for (int i = 0; i < forecastItemDto.Count; i++)
                {
                    ForeCastCustomerSupportItem forecastItemDetail = _mapper.Map<ForeCastCustomerSupportItem>(forecastItemDto[i]);
                    forecastItemDetail.foreCastCSDeliverySchedule = _mapper.Map<List<ForeCastCSDeliverySchedule>>(forecastItemDto[i].foreCastCSDeliverySchedule);
                    forecastCsItemList.Add(forecastItemDetail);

                }
                forecastcustomerlist.foreCastCustomerSupportItems = forecastCsItemList;

                var data = _mapper.Map(foreCastCustomerSupportUpdateDto, forecastcustomerlist);

                string result = await _repository.UpdateForeCastCustomerSupport(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateForeCastCustomerSupport action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Delete lpcosting
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForeCastLpCosting(int id)
        {
            ServiceResponse<ForecastLpCostingDto> serviceResponse = new ServiceResponse<ForecastLpCostingDto>();

            try
            {
                var forecastlpcosting = await _lpcostingRepository.GetForecastLpCostingById(id);
                if (forecastlpcosting == null)
                {
                    _logger.LogError($"forecastLPCosting with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete forecastLPCosting with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _lpcostingRepository.DeleteForecastLpCosting(forecastlpcosting);
                _logger.LogInfo(result);
                _lpcostingRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went forecastLPCosting inside Deleteforecast action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<ForeCastController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForeCast(int id)
        {
            ServiceResponse<ForeCastDto> serviceResponse = new ServiceResponse<ForeCastDto>();

            try
            {
                var forecast = await _Forecastrepository.GetForeCastById(id);
                if (forecast == null)
                {
                    _logger.LogError($"Forecast with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Forecast with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _Forecastrepository.DeleteForeCast(forecast);
                _logger.LogInfo(result);
                _Forecastrepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteForecast action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        } //delete ForeCastEngg
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForeCastengg(int id)
        {
            ServiceResponse<ForeCastEnggDto> serviceResponse = new ServiceResponse<ForeCastEnggDto>();

            try
            {
                var forecastengg = await _forecastenggRepository.GetForeCastEnggById(id);
                if (forecastengg == null)
                {
                    _logger.LogError($"forecastengg with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete forecastengg with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _forecastenggRepository.DeleteForeCastEngg(forecastengg);
                _logger.LogInfo(result);
                _forecastenggRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went ForecastEngg inside DeleteForeCast action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Delete Forecastsourcing
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForeCastSourcing(int id)
        {
            ServiceResponse<ForecastSourcingDto> serviceResponse = new ServiceResponse<ForecastSourcingDto>();

            try
            {
                var forecastsource = await _sourcingrepository.GetForeCastSourcingById(id);
                if (forecastsource == null)
                {
                    _logger.LogError($"ForeCastSource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete ForeCastSource with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _sourcingrepository.DeleteForeCastSourcing(forecastsource);
                _logger.LogInfo(result);
                _sourcingrepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteForeCastSource action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForeCastCustomerSupport(int id)
        {
            ServiceResponse<ForeCastCustomerSupportDto> serviceResponse = new ServiceResponse<ForeCastCustomerSupportDto>();

            try
            {
                var forecast = await _repository.GetForeCastCustomerSupportById(id);
                if (forecast == null)
                {
                    _logger.LogError($"ForecastCustomerSupport with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete ForecastCustomerSupport with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteForeCastCustomerSupport(forecast);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteForeCastCustomerSupport action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
