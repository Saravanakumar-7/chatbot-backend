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
using static Tips.SalesService.Api.Repository.RfqEnggItemRepository;
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
        private IForeCastReleaseLpRepository _releaseLpRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IForeCastEnggRepository _forecastenggRepository;
        private IForeCastEnggItemsRepository _foreCastEnggItemsRepository;
        private IForecastLpCostingRepository _lpcostingRepository;
        private IForeCastCustomGroupRepository _forecastcustomgroupRepository;
        private IForeCastCustomFieldRepository _forecastcustomfieldRepository;
        public ForeCastController(IForeCastCustomFieldRepository foreCastCustomFieldRepository, IForeCastEnggItemsRepository foreCastEnggItemsRepository, IForeCastReleaseLpRepository foreCastReleaseLpRepository, IForeCastCustomGroupRepository foreCastCustomGroupRepository, IForeCastRepository foreCastRepository, IForecastSourcingRepository forecastSourcingRepository, IForecastLpCostingRepository forecastLpCostingRepository, IForeCastEnggRepository foreCastEnggRepository, IForeCastCustomerSupportRepository foreCastCustomerSupportRepository,IForeCastCustomerSupportItemRepository foreCastCustomerSupportItemRepository ,ILoggerManager logger, IMapper mapper)
        {
           _Forecastrepository= foreCastRepository;
            _releaseLpRepository = foreCastReleaseLpRepository;
            _logger = logger;
            _mapper = mapper;
            _itemRepository = foreCastCustomerSupportItemRepository;
            _repository = foreCastCustomerSupportRepository;
            _forecastenggRepository = foreCastEnggRepository;
            _foreCastEnggItemsRepository = foreCastEnggItemsRepository;
            _sourcingrepository = forecastSourcingRepository;
            _lpcostingRepository = forecastLpCostingRepository;
            _forecastcustomgroupRepository = foreCastCustomGroupRepository;
            _forecastcustomfieldRepository = foreCastCustomFieldRepository;
        }
        // GET: api/<ForeCastController>
        [HttpGet]
        public async Task<IActionResult> GetAllForeCast([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ForeCastDto>> serviceResponse = new ServiceResponse<IEnumerable<ForeCastDto>>();

            try
            {
                var getAllForeCast = await _Forecastrepository.GetAllForeCast(pagingParameter);
                var metadata = new
                {
                    getAllForeCast.TotalCount,
                    getAllForeCast.PageSize,
                    getAllForeCast.CurrentPage,
                    getAllForeCast.HasNext,
                    getAllForeCast.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ForeCast");
                var result = _mapper.Map<IEnumerable<ForeCastDto>>(getAllForeCast);
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
                var getAllForecastLpCosting = await _lpcostingRepository.GetAllForecastLpCosting(pagingParameter);
                var metadata = new
                {
                    getAllForecastLpCosting.TotalCount,
                    getAllForecastLpCosting.PageSize,
                    getAllForecastLpCosting.CurrentPage,
                    getAllForecastLpCosting.HasNext,
                    getAllForecastLpCosting.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ForeCastLpcosting");
                var result = _mapper.Map<IEnumerable<ForecastLpCostingDto>>(getAllForecastLpCosting);
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
                var getAllForeCastEngg = await _forecastenggRepository.GetAllForeCastEngg(pagingParameter);
                var metadata = new
                {
                    getAllForeCastEngg.TotalCount,
                    getAllForeCastEngg.PageSize,
                    getAllForeCastEngg.CurrentPage,
                    getAllForeCastEngg.HasNext,
                    getAllForeCastEngg.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ForeCastEngg");
                var result = _mapper.Map<IEnumerable<ForeCastEnggDto>>(getAllForeCastEngg);
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
                var getAllForeCastSourcing = await _sourcingrepository.GetAllForeCastSourcing(pagingParameter);
                var metadata = new
                {
                    getAllForeCastSourcing.TotalCount,
                    getAllForeCastSourcing.PageSize,
                    getAllForeCastSourcing.CurrentPage,
                    getAllForeCastSourcing.HasNext,
                    getAllForeCastSourcing.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all GetAllForeCastSourcings");
                var result = _mapper.Map<IEnumerable<ForecastSourcingDto>>(getAllForeCastSourcing);
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
                var getAllForeCastCustomerSupports = await _repository.GetAllForeCastCustomerSupports(pagingParameter);
                var metadata = new
                {
                    getAllForeCastCustomerSupports.TotalCount,
                    getAllForeCastCustomerSupports.PageSize,
                    getAllForeCastCustomerSupports.CurrentPage,
                    getAllForeCastCustomerSupports.HasNext,
                    getAllForeCastCustomerSupports.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ForeCastCustomerSupport");
                var result = _mapper.Map<IEnumerable<ForeCastCustomerSupportDto>>(getAllForeCastCustomerSupports);
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
                var foreCastEnggByForeCastNumber = await _forecastenggRepository.ForeCastEnggByForeCastNumber(ForeCastNumber);

                if (foreCastEnggByForeCastNumber == null)
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

                    ForeCastEnggDto forecstenggDto = _mapper.Map<ForeCastEnggDto>(foreCastEnggByForeCastNumber);

                    List<ForeCastEnggItemsDto> forecastenggItemsDtos = new List<ForeCastEnggItemsDto>();

                    foreach (var ForecastItemDetail in foreCastEnggByForeCastNumber.ForeCastEnggItems)
                    {
                        ForeCastEnggItemsDto forecastenggItemDto = _mapper.Map<ForeCastEnggItemsDto>(ForecastItemDetail);
                        forecastenggItemsDtos.Add(forecastenggItemDto);
                    }
                    forecstenggDto.ForeCastEnggItems = forecastenggItemsDtos;

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
                var getLpcostingByFCNo = await _lpcostingRepository.GetForecastLpCostingByForeCastNumber(ForeCastNumber);

                if (getLpcostingByFCNo == null)
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
                    _logger.LogInfo($"Returned ForecastLpCostingByForecastNumber with id: {getLpcostingByFCNo}");

                    ForecastLpCostingDto forecastLpCostingDto = _mapper.Map<ForecastLpCostingDto>(getLpcostingByFCNo);

                    List<ForecastLpCostingItemDto> forecastLpCostingItemDtos = new List<ForecastLpCostingItemDto>();
                    foreach (var FCLpCodtingitemDetail in getLpcostingByFCNo.ForecastLpCostingItems)
                    {
                        ForecastLpCostingItemDto forecastLpCostingItemDto = _mapper.Map<ForecastLpCostingItemDto>(FCLpCodtingitemDetail);
                        forecastLpCostingItemDto.ForecastLpCostingProcesses = _mapper.Map<List<ForecastLpCostingProcessDto>>(forecastLpCostingItemDto.ForecastLpCostingProcesses);
                        forecastLpCostingItemDto.ForecastLpCostingNREConsumables = _mapper.Map<List<ForecastLpCostingNREConsumableDto>>(forecastLpCostingItemDto.ForecastLpCostingNREConsumables);
                        forecastLpCostingItemDto.ForecastLpCostingOtherCharges = _mapper.Map<List<ForecastLpCostingOtherChargesDto>>(forecastLpCostingItemDto.ForecastLpCostingOtherCharges);

                        forecastLpCostingItemDtos.Add(forecastLpCostingItemDto);
                    }
                    forecastLpCostingDto.ForecastLpCostingItems = forecastLpCostingItemDtos;

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
                var foreCastCSByForeCastNumber = await _repository.ForeCastCustomerSupportByForeCastNumber(ForeCastNumber);

                if (foreCastCSByForeCastNumber == null)
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

                    ForeCastCustomerSupportDto forecastDto = _mapper.Map<ForeCastCustomerSupportDto>(foreCastCSByForeCastNumber);

                    List<ForeCastCustomerSupportItemDto> forecastItemsDtos = new List<ForeCastCustomerSupportItemDto>();
                    foreach (var itemDetails in foreCastCSByForeCastNumber.ForeCastCustomerSupportItems)
                    {
                        ForeCastCustomerSupportItemDto forecastItemDto = _mapper.Map<ForeCastCustomerSupportItemDto>(itemDetails);
                        forecastItemDto.ForeCastCSDeliverySchedule = _mapper.Map<List<ForeCastCSDeliveryScheduleDto>>(itemDetails.ForeCastCSDeliverySchedule);
                        forecastItemsDtos.Add(forecastItemDto);
                    }
                    forecastDto.ForeCastCustomerSupportItems = forecastItemsDtos;

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
                var getForeCastEnggById = await _forecastenggRepository.GetForeCastEnggById(id);

                if (getForeCastEnggById == null)
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
                    var result = _mapper.Map<ForeCastEnggDto>(getForeCastEnggById);
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
                var getForeCastSourcingById = await _sourcingrepository.GetForeCastSourcingById(id);

                if (getForeCastSourcingById == null)
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
                    ForecastSourcingDto forecastSourceDto = _mapper.Map<ForecastSourcingDto>(getForeCastSourcingById);//Main model mapping

                    //below mapping is child under child  

                    List<ForecastSourcingItemsDto> forecastSourseItemDtos = new List<ForecastSourcingItemsDto>();

                    foreach (var sourceitemDetails in getForeCastSourcingById.ForecastSourcingItems)
                    {
                        ForecastSourcingItemsDto forecastSourceItemDto = _mapper.Map<ForecastSourcingItemsDto>(sourceitemDetails);
                        forecastSourceItemDto.ForecastSourcingVendors = _mapper.Map<List<ForecastSourcingVendorDto>>(sourceitemDetails.ForecastSourcingVendors);
                        forecastSourseItemDtos.Add(forecastSourceItemDto);
                    }

                    forecastSourceDto.ForecastSourcingItems = forecastSourseItemDtos;
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
                var getForecastLpCostingById = await _lpcostingRepository.GetForecastLpCostingById(id);

                if (getForecastLpCostingById == null)
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

                    ForecastLpCostingDto forecastLpCostingDto = _mapper.Map<ForecastLpCostingDto>(getForecastLpCostingById);

                    List<ForecastLpCostingItemDto> forecastLpCostingItemDtos = new List<ForecastLpCostingItemDto>();

                    foreach (var lpcostingitemDetail in getForecastLpCostingById.ForecastLpCostingItems)
                    {
                        ForecastLpCostingItemDto forecastLpCostingItemDto = _mapper.Map<ForecastLpCostingItemDto>(lpcostingitemDetail);
                        forecastLpCostingItemDto.ForecastLpCostingProcesses = _mapper.Map<List<ForecastLpCostingProcessDto>>(forecastLpCostingItemDto.ForecastLpCostingProcesses);
                        forecastLpCostingItemDto.ForecastLpCostingNREConsumables = _mapper.Map<List<ForecastLpCostingNREConsumableDto>>(forecastLpCostingItemDto.ForecastLpCostingNREConsumables);
                        forecastLpCostingItemDto.ForecastLpCostingOtherCharges = _mapper.Map<List<ForecastLpCostingOtherChargesDto>>(forecastLpCostingItemDto.ForecastLpCostingOtherCharges);

                        forecastLpCostingItemDtos.Add(forecastLpCostingItemDto);
                    }

                    forecastLpCostingDto.ForecastLpCostingItems = forecastLpCostingItemDtos;
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
                var getForeCastById = await _Forecastrepository.GetForeCastById(id);

                if (getForeCastById == null)
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
                    var result = _mapper.Map<ForeCastDto>(getForeCastById);
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
                    foreach (var itemDetails in forecast.ForeCastCustomerSupportItems)
                    {
                        ForeCastCustomerSupportItemDto forecastItemDto = _mapper.Map<ForeCastCustomerSupportItemDto>(itemDetails);
                        forecastItemDto.ForeCastCSDeliverySchedule = _mapper.Map<List<ForeCastCSDeliveryScheduleDto>>(itemDetails.ForeCastCSDeliverySchedule);
                        forecastItemsDtos.Add(forecastItemDto);
                    }
                    forecastDto.ForeCastCustomerSupportItems = forecastItemsDtos;

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

        [HttpPut]
        public async Task<IActionResult> UpdateForeCastEnggItemRelease([FromBody] List<int> itemIds)
        {
            ServiceResponse<ForeCastEnggDto> serviceResponse = new ServiceResponse<ForeCastEnggDto>();

            try
            {
                if (itemIds is null)
                {
                    _logger.LogError("ForecastItemid object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update ForecCstItemid object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                foreach (var id in itemIds)
                {
                    if (id == null)
                    {
                        _logger.LogError($"RfqItem with item id: {id}, hasn't been found in db.");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Update RfqItem hasn't been found in db.";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(serviceResponse);
                    }

                    var getForeCastEnggItemById = await _foreCastEnggItemsRepository.GetForeCastEnggItemsById(id);
                    getForeCastEnggItemById.ReleaseStatus = true;
                    string result = await _foreCastEnggItemsRepository.ActivateForeCastEnggItemById(getForeCastEnggItemById);
                    _logger.LogInfo(result);
                    _repository.SaveAsync();
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "ForeCastItem  Release Activated Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateForeCastItem action: {ex.Message}");
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
                var createForecastSource = _mapper.Map<ForecastSourcing>(forecastSourcingDtoPost);
                var forecastSourceData = createForecastSource.ForeCastNumber;

                var forecastSourceUpdate = await _Forecastrepository.ForeCastSourcingByForecasrNumbers(forecastSourceData);

                forecastSourceUpdate.IsSourcing = true;

                var forecastSourceDto = forecastSourcingDtoPost.ForecastSourcingItems;

                var sourceItemList = new List<ForecastSourcingItems>();
                for (int i = 0; i < forecastSourceDto.Count; i++)
                {
                    ForecastSourcingItems sourceItemListDetail = _mapper.Map<ForecastSourcingItems>(forecastSourceDto[i]);
                    sourceItemListDetail.ForecastSourcingVendors = _mapper.Map<List<ForecastSourcingVendor>>(forecastSourceDto[i].ForecastSourcingVendors);
                    sourceItemList.Add(sourceItemListDetail);

                }
                createForecastSource.ForecastSourcingItems = sourceItemList;

                await _sourcingrepository.CreateForeCastSourcing(createForecastSource);
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
                var createForecastLp = _mapper.Map<ForecastLpCosting>(forecastLPCostingDtoPost);
                var FCLPCostingDto = forecastLPCostingDtoPost.ForecastLPCostingItems;

                var lpcostingItemList = new List<ForecastLpCostingItem>();
                for (int i = 0; i < FCLPCostingDto.Count; i++)
                {
                    ForecastLpCostingItem lpcostingItemListDetail = _mapper.Map<ForecastLpCostingItem>(FCLPCostingDto[i]);
                    lpcostingItemListDetail.ForecastLpCostingProcesses = _mapper.Map<List<ForecastLpCostingProcess>>(FCLPCostingDto[i].ForecastLPCostingProcesses);
                    lpcostingItemListDetail.ForecastLPCostingNREConsumables = _mapper.Map<List<ForecastLPCostingNREConsumable>>(FCLPCostingDto[i].ForecastLPCostingNREConsumables);
                    lpcostingItemListDetail.ForecastLpCostingOtherCharges = _mapper.Map<List<ForecastLpCostingOtherCharges>>(FCLPCostingDto[i].ForecastLPCostingOtherCharges);
                    lpcostingItemList.Add(lpcostingItemListDetail);

                }
                createForecastLp.ForecastLpCostingItems = lpcostingItemList;


                _lpcostingRepository.CreateForecastLpCosting(createForecastLp);
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
                var createForecast = _mapper.Map<ForeCast>(foreCastPostDto);

                await _Forecastrepository.CreateForeCast(createForecast);
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
                var createForecast =  _mapper.Map<ForeCastEngg>(foreCastEnggPost);
                _forecastenggRepository.CreateForeCastEngg(createForecast);
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


                var forecastCustomerSupportItemDto = foreCastCustomerSupportPostDto.ForeCastCustomerSupportItems;

                var forecastCustomerSupportLists = new List<ForeCastCustomerSupportItem>();
                for (int i = 0; i < forecastCustomerSupportItemDto.Count; i++)
                {
                    ForeCastCustomerSupportItem forecastCSItems = _mapper.Map<ForeCastCustomerSupportItem>(forecastCustomerSupportItemDto[i]);
                    forecastCSItems.ForeCastCSDeliverySchedule = _mapper.Map<List<ForeCastCSDeliverySchedule>>(forecastCustomerSupportItemDto[i].ForeCastCSDeliverySchedule);
                    forecastCustomerSupportLists.Add(forecastCSItems);

                }
                forecastCustomerSupportList.ForeCastCustomerSupportItems = forecastCustomerSupportLists;

               await _repository.CreateForeCastCustomerSupport(forecastCustomerSupportList);

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

        [HttpPost]
        public async Task<IActionResult> BulkRelease([FromBody] List<ForecastReleaseLpDtoPost> forecastReleaseLpDtoPosts)
        {
            ServiceResponse<ForecastReleaseLpDtoPost> serviceResponse = new ServiceResponse<ForecastReleaseLpDtoPost>();

            try
            {
                if (forecastReleaseLpDtoPosts == null)
                {
                    _logger.LogError("BulkRelease details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BulkRelease details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid BulkRelease details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var bulklist = _mapper.Map<List<ForeCastReleaseLp>>(forecastReleaseLpDtoPosts);


                var bulkData = bulklist[0].ForeCastNumber;

                var lpreleases = await _Forecastrepository.ForeCastLpCostingReleaseByForeCastNumbers(bulkData);
                lpreleases.IsLpCostingRelease = true;

                foreach (var releaseLpdetails in bulklist)
                {

                    _releaseLpRepository.BulkRelease(releaseLpdetails);
                }
                _Forecastrepository.Update(lpreleases);
                _releaseLpRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("ReleaseLpById", serviceResponse);
            }

            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Create ReleaseLp action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
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
                var getForecastLpCostingById = await _lpcostingRepository.GetForecastLpCostingById(id);
                if (getForecastLpCostingById is null)
                {
                    _logger.LogError($"forecastlpcosting with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update forecastlpcosting with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var forelpcosting = _mapper.Map<ForecastLpCosting>(forecastLPCostingDtoUpdate);

                var flpcostingitemDto = forecastLPCostingDtoUpdate.ForecastLPCostingItems;

                var forecastlpcostingitemList = new List<ForecastLpCostingItem>();

                if (flpcostingitemDto != null) {
                    for (int i = 0; i < flpcostingitemDto.Count; i++)
                    {
                        ForecastLpCostingItem flpcostingItemDetail = _mapper.Map<ForecastLpCostingItem>(flpcostingitemDto[i]);
                        flpcostingItemDetail.ForecastLpCostingProcesses = _mapper.Map<List<ForecastLpCostingProcess>>(flpcostingitemDto[i].ForecastLPCostingProcesses);
                        flpcostingItemDetail.ForecastLPCostingNREConsumables = _mapper.Map<List<ForecastLPCostingNREConsumable>>(flpcostingitemDto[i].ForecastLPCostingNREConsumables);
                        flpcostingItemDetail.ForecastLpCostingOtherCharges = _mapper.Map<List<ForecastLpCostingOtherCharges>>(flpcostingitemDto[i].ForecastLPCostingOthers);

                        forecastlpcostingitemList.Add(flpcostingItemDetail);

                    }
                }
                var updateData = _mapper.Map(forecastLPCostingDtoUpdate, getForecastLpCostingById);

                string result = await _lpcostingRepository.UpdateForecastLpCosting(updateData);
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
                var getForeCastById = await _Forecastrepository.GetForeCastById(id);
                if (getForeCastById is null)
                {
                    _logger.LogError($"Forecast with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Forecast with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var updateforeCast = _mapper.Map(foreCastUpdateDto, getForeCastById);

                string result = await _Forecastrepository.UpdateForeCast(updateforeCast);
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
                var getForeCastSourcingById = await _sourcingrepository.GetForeCastSourcingById(id);
                if (getForeCastSourcingById is null)
                {
                    _logger.LogError($"ForeCastSource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update ForeCastSource with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var forecastsourceList = _mapper.Map<ForecastSourcing>(forecastSourcingDtoUpdate);

                var sourceitemtemDto = forecastSourcingDtoUpdate.ForecastSourcingItems;

                var sourceItemList = new List<ForecastSourcingItems>();

                if (sourceitemtemDto != null)
                {
                    for (int i = 0; i < sourceitemtemDto.Count; i++)
                    {
                        ForecastSourcingItems sourceItemDetail = _mapper.Map<ForecastSourcingItems>(sourceitemtemDto[i]);
                        sourceItemDetail.ForecastSourcingVendors = _mapper.Map<List<ForecastSourcingVendor>>(sourceitemtemDto[i].ForecastSourcingVendors);

                        sourceItemList.Add(sourceItemDetail);

                    }
                }
                var updateForeCastSource = _mapper.Map(forecastSourcingDtoUpdate, getForeCastSourcingById);

                updateForeCastSource.ForecastSourcingItems = sourceItemList;


                string result = await _sourcingrepository.UpdateForeCastSourcing(updateForeCastSource);
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
                var getForeCastEnggById = await _forecastenggRepository.GetForeCastEnggById(id);
                if (getForeCastEnggById is null)
                {
                    _logger.LogError($"forecastengg with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update forecastengg with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var updateForecastEngg = _mapper.Map(foreCastEnggUpdate, getForeCastEnggById);



                string result = await _forecastenggRepository.UpdateForeCastEngg(updateForecastEngg);
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
                var GetForeCastCSById = await _repository.GetForeCastCustomerSupportById(id);
                if (GetForeCastCSById is null)
                {
                    _logger.LogError($"ForeCastCustomerSupport with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update ForeCastCustomerSupport with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
               
                var updateForeCastCS = _mapper.Map<ForeCastCustomerSupport>(GetForeCastCSById);

                var forecastItemDto = foreCastCustomerSupportUpdateDto.ForeCastCustomerSupportItems;

                var forecastCsItemList = new List<ForeCastCustomerSupportItem>();

                if (forecastItemDto != null)
                {
                    for (int i = 0; i < forecastItemDto.Count; i++)
                    {
                        ForeCastCustomerSupportItem forecastItemDetail = _mapper.Map<ForeCastCustomerSupportItem>(forecastItemDto[i]);
                        forecastItemDetail.ForeCastCSDeliverySchedule = _mapper.Map<List<ForeCastCSDeliverySchedule>>(forecastItemDto[i].ForeCastCSDeliverySchedule);
                        forecastCsItemList.Add(forecastItemDetail);

                    }
                }
                updateForeCastCS.ForeCastCustomerSupportItems = forecastCsItemList;

                var data = _mapper.Map(foreCastCustomerSupportUpdateDto, updateForeCastCS);

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
                var getForecastLpCostingById = await _lpcostingRepository.GetForecastLpCostingById(id);
                if (getForecastLpCostingById == null)
                {
                    _logger.LogError($"forecastLPCosting with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete forecastLPCosting with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _lpcostingRepository.DeleteForecastLpCosting(getForecastLpCostingById);
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
                var getForeCastById = await _Forecastrepository.GetForeCastById(id);
                if (getForeCastById == null)
                {
                    _logger.LogError($"Forecast with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Forecast with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _Forecastrepository.DeleteForeCast(getForeCastById );
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
                var foreCastenggById = await _forecastenggRepository.GetForeCastEnggById(id);
                if (foreCastenggById == null)
                {
                    _logger.LogError($"forecastengg with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete forecastengg with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _forecastenggRepository.DeleteForeCastEngg(foreCastenggById);
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
                var getForeCastSourcingById = await _sourcingrepository.GetForeCastSourcingById(id);
                if (getForeCastSourcingById == null)
                {
                    _logger.LogError($"ForeCastSource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete ForeCastSource with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _sourcingrepository.DeleteForeCastSourcing(getForeCastSourcingById);
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
                var getForeCastCSById = await _repository.GetForeCastCustomerSupportById(id);
                if (getForeCastCSById == null)
                {
                    _logger.LogError($"ForecastCustomerSupport with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete ForecastCustomerSupport with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteForeCastCustomerSupport(getForeCastCSById);
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

        // GET: api/<ForeCastCustomGroupController> 
        [HttpGet]
        public async Task<IActionResult> GetAllForeCastCustomGroup([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ForeCastCustomGroupDto>> serviceResponse = new ServiceResponse<IEnumerable<ForeCastCustomGroupDto>>();
            try
            {
                var getAllForeCastCustomGroup = await _forecastcustomgroupRepository.GetAllForeCastCustomGroup(pagingParameter);
                var metadata = new
                {
                    getAllForeCastCustomGroup.TotalCount,
                    getAllForeCastCustomGroup.PageSize,
                    getAllForeCastCustomGroup.CurrentPage,
                    getAllForeCastCustomGroup.HasNext,
                    getAllForeCastCustomGroup.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ForeCastCustomGroup");
                var foreCastCustomGroupEntity = _mapper.Map<IEnumerable<ForeCastCustomGroupDto>>(getAllForeCastCustomGroup);
                serviceResponse.Data = foreCastCustomGroupEntity;
                serviceResponse.Message = "Returned all ForeCastCustomGroup";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET: api/<ForeCastCustomGroupController>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetForeCastCustomGroupById(int id)
        {
            ServiceResponse<ForeCastCustomGroupDto> serviceResponse = new ServiceResponse<ForeCastCustomGroupDto>();

            try
            {
                var getForeCastCustomGroupById = await _forecastcustomgroupRepository.GetForeCastCustomGroupById(id);
                if (getForeCastCustomGroupById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForeCastCustomGroup hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForeCastCustomGroup with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var foreCastCustomGroupEntity = _mapper.Map<ForeCastCustomGroupDto>(getForeCastCustomGroupById);
                    serviceResponse.Data = foreCastCustomGroupEntity;
                    serviceResponse.Message = "Returned ForeCastCustomGroup Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetForeCastCustomGroupById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST: api/<ForeCastCustomGroupController>
        [HttpPost]
        public IActionResult CreateForeCastCustomGroup([FromBody] ForeCastCustomGroupDtoPost foreCastCustomGroupDtoPost)
        {
            ServiceResponse<ForeCastCustomGroupDtoPost> serviceResponse = new ServiceResponse<ForeCastCustomGroupDtoPost>();

            try
            {
                if (foreCastCustomGroupDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ForeCastCustomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ForeCastCustomGroup object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ForeCastCustomGroup object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ForeCastCustomGroup object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var createForeCastCustomGroup = _mapper.Map<ForeCastCustomGroup>(foreCastCustomGroupDtoPost);
                _forecastcustomgroupRepository.CreateForeCastCustomGroup(createForeCastCustomGroup);
                _forecastcustomgroupRepository.SaveAsync();
                serviceResponse.Message = "ForeCastCustomGroup Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateForeCastCustomGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT: api/<ForeCastCustomGroupController>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForeCastCustomGroup(int id, [FromBody] ForeCastCustomGroupDtoUpdate foreCastCustomGroupDtoUpdate)
        {
            ServiceResponse<ForeCastCustomGroupDtoUpdate> serviceResponse = new ServiceResponse<ForeCastCustomGroupDtoUpdate>();

            try
            {
                if (foreCastCustomGroupDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update ForeCastCustomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update ForeCastCustomGroup object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ForeCastCustomGroup object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update ForeCastCustomGroup object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var getForeCastCustomGroupById = await _forecastcustomgroupRepository.GetForeCastCustomGroupById(id);
                if (getForeCastCustomGroupById is null)
                {
                    _logger.LogError($"Update ForeCastCustomGroup with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " UpdateForeCastCustomGroup hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(foreCastCustomGroupDtoUpdate, getForeCastCustomGroupById);
                string result = await _forecastcustomgroupRepository.UpdateForeCastCustomGroup(getForeCastCustomGroupById);
                _logger.LogInfo(result);
                _forecastcustomgroupRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ForeCastCustomGroup Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateForeCastCustomGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE: api/<ForeCastCustomGroupController>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForeCastCustomGroup(int id)
        {
            ServiceResponse<ForeCastCustomGroupDto> serviceResponse = new ServiceResponse<ForeCastCustomGroupDto>();

            try
            {
                var getForeCastCustomGroupById = await _forecastcustomgroupRepository.GetForeCastCustomGroupById(id);
                if (getForeCastCustomGroupById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete ForeCastCustomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete ForeCastCustomGroup with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _forecastcustomgroupRepository.DeleteForeCastCustomGroup(getForeCastCustomGroupById);
                _logger.LogInfo(result);
                _forecastcustomgroupRepository.SaveAsync();
                serviceResponse.Message = "ForeCastCustomGroup Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteForeCastCustomGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // GET: api/<ForeCastCustomFieldController>
        [HttpGet]
        public async Task<IActionResult> GetAllForeCastCustomField([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ForeCastCustomFieldDto>> serviceResponse = new ServiceResponse<IEnumerable<ForeCastCustomFieldDto>>();
            try
            {
                var getAllForeCastCustomField = await _forecastcustomfieldRepository.GetAllForeCastCustomField(pagingParameter);
                var metadata = new
                {
                    getAllForeCastCustomField.TotalCount,
                    getAllForeCastCustomField.PageSize,
                    getAllForeCastCustomField.CurrentPage,
                    getAllForeCastCustomField.HasNext,
                    getAllForeCastCustomField.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all ForeCastCustomField");
                var foreCastCustomFieldEntity = _mapper.Map<IEnumerable<ForeCastCustomFieldDto>>(getAllForeCastCustomField);
                serviceResponse.Data = foreCastCustomFieldEntity;
                serviceResponse.Message = "Returned all ForeCastCustomField";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET: api/<ForeCastCustomFieldController>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetForeCastCustomFieldById(int id)
        {
            ServiceResponse<ForeCastCustomFieldDto> serviceResponse = new ServiceResponse<ForeCastCustomFieldDto>();

            try
            {
                var getForeCastCustomFieldById = await _forecastcustomfieldRepository.GetForeCastCustomFieldById(id);
                if (getForeCastCustomFieldById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ForeCastCustomField hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"ForeCastCustomField with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ForeCastCustomField with id: {id}");
                    var foreCastCustomFieldEntity = _mapper.Map<ForeCastCustomFieldDto>(getForeCastCustomFieldById);
                    serviceResponse.Data = foreCastCustomFieldEntity;
                    serviceResponse.Message = "Returned ForeCastCustomField Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetForeCastCustomFieldById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST: api/<ForeCastCustomFieldController>
        [HttpPost]
        public IActionResult CreateForeCastCustomField([FromBody] ForeCastCustomFieldDtoPost foreCastCustomFieldDtoPost)
        {
            ServiceResponse<ForeCastCustomFieldDtoPost> serviceResponse = new ServiceResponse<ForeCastCustomFieldDtoPost>();

            try
            {
                if (foreCastCustomFieldDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ForeCastCustomField object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("ForeCastCustomField object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ForeCastCustomField object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid ForeCastCustomField object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var foreCastCustomFieldEntity = _mapper.Map<ForeCastCustomField>(foreCastCustomFieldDtoPost);
                _forecastcustomfieldRepository.CreateForeCastCustomField(foreCastCustomFieldEntity);
                _forecastcustomfieldRepository.SaveAsync();
                serviceResponse.Message = "ForeCastCustomField Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateForeCastCustomField action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT: api/<ForeCastCustomFieldController>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForeCastCustomField(int id, [FromBody] ForeCastCustomFieldDtoUpdate foreCastCustomFieldDtoUpdate)
        {
            ServiceResponse<ForeCastCustomFieldDtoUpdate> serviceResponse = new ServiceResponse<ForeCastCustomFieldDtoUpdate>();

            try
            {
                if (foreCastCustomFieldDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update ForeCastCustomField object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update ForeCastCustomField object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update ForeCastCustomField object sent from client";
                       serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update ForeCastCustomField object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var getForeCastCustomFieldById = await _forecastcustomfieldRepository.GetForeCastCustomFieldById(id);
                if (getForeCastCustomFieldById is null)
                {
                    _logger.LogError($"Update ForeCastCustomField with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " UpdateForeCastCustomField hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(foreCastCustomFieldDtoUpdate, getForeCastCustomFieldById);
                string result = await _forecastcustomfieldRepository.UpdateForeCastCustomField(getForeCastCustomFieldById);
                _logger.LogInfo(result);
                _forecastcustomfieldRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ForeCastCustomField Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateForeCastCustomField action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE: api/<ForeCastCustomFieldController>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForeCastCustomField(int id)
        {
            ServiceResponse<ForeCastCustomFieldDto> serviceResponse = new ServiceResponse<ForeCastCustomFieldDto>();

            try
            {
                var getForeCastCustomFieldById = await _forecastcustomfieldRepository.GetForeCastCustomFieldById(id);
                if (getForeCastCustomFieldById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete ForeCastCustomField object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete ForeCastCustomField with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _forecastcustomfieldRepository.DeleteForeCastCustomField(getForeCastCustomFieldById);
                _logger.LogInfo(result);
                _forecastcustomfieldRepository.SaveAsync();
                serviceResponse.Message = "ForeCastCustomField Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteForeCastCustomField action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
