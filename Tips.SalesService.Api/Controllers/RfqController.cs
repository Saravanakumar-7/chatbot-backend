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
using RfqCSDeliveryScheduleDto = Tips.SalesService.Api.Entities.DTOs.RfqCSDeliveryScheduleDto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RfqController : ControllerBase
    {
        private IRfqCustomerSupportRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IRfqRepository _rfqRepository;
        private IRfqEnggRepository _rfqenggRepository;
        private IRfqLPCostingRepository _rfqlpcostingRepository;
        public RfqController(IRfqCustomerSupportRepository repository,IRfqRepository rfqRepository, IRfqLPCostingRepository rfqLPCostingRepository, IRfqEnggRepository rfqEnggRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _rfqRepository = rfqRepository;
            _rfqenggRepository = rfqEnggRepository;
            _rfqlpcostingRepository = rfqLPCostingRepository;

        }

        //rfq getall 
        [HttpGet]
        public async Task<IActionResult> GetAllRfq([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<RfqDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqDto>>();

            try
            {
                var listOfRfq = await _rfqRepository.GetAllRfq(pagingParameter);
                var metadata = new
                {
                    listOfRfq.TotalCount,
                    listOfRfq.PageSize,
                    listOfRfq.CurrentPage,
                    listOfRfq.HasNext,
                    listOfRfq.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all rfq");
                var result = _mapper.Map<IEnumerable<RfqDto>>(listOfRfq);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Rfqs Successfully";
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
        public async Task<IActionResult> GetAllRfqCustomerSupport([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<RfqCustomerSupportDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqCustomerSupportDto>>();

            try
            {
                var listOfRfq = await _repository.GetAllRfqCustomerSupport(pagingParameter);
                var metadata = new
                {
                    listOfRfq.TotalCount,
                    listOfRfq.PageSize,
                    listOfRfq.CurrentPage,
                    listOfRfq.HasNext,
                    listOfRfq.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all RfqCustomerSupport");
                var result = _mapper.Map<IEnumerable<RfqCustomerSupportDto>>(listOfRfq);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RfqCustomerSupport Successfully";
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
        public async Task<IActionResult> GetAllActiveRfqNumberList()
        {
            ServiceResponse<IEnumerable<RfqNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqNumberListDto>>();
            try
            {
                var listOfrfqnumber = await _rfqRepository.GetAllActiveRfqNumberList();
                //_logger.LogInfo("Returned all CustomerMaster");
                var result = _mapper.Map<IEnumerable<RfqNumberListDto>>(listOfrfqnumber);
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
                serviceResponse.Message = $"Something went wrong inside GetAllActiveRfqNumberList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Get all RfqLPCosting

        [HttpGet]
        public async Task<IActionResult> GetAllRfqLPCosting([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<RfqLPCostingDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqLPCostingDto>>();

            try
            {
                var listOfRfqLPCosting = await _rfqlpcostingRepository.GetAllRfqLPCosting(pagingParameter);
                var metadata = new
                {
                    listOfRfqLPCosting.TotalCount,
                    listOfRfqLPCosting.PageSize,
                    listOfRfqLPCosting.CurrentPage,
                    listOfRfqLPCosting.HasNext,
                    listOfRfqLPCosting.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all RfqEngg");
                var result = _mapper.Map<IEnumerable<RfqLPCostingDto>>(listOfRfqLPCosting);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RfqLPCosting Successfully";
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
        // Get all Rfq Engg 
        [HttpGet]
        public async Task<IActionResult> GetAllRfqEngg([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<RfqEnggDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqEnggDto>>();

            try
            {
                var listOfRfqengg = await _rfqenggRepository.GetAllRfqEngg(pagingParameter);
                var metadata = new
                {
                    listOfRfqengg.TotalCount,
                    listOfRfqengg.PageSize,
                    listOfRfqengg.CurrentPage,
                    listOfRfqengg.HasNext,
                    listOfRfqengg.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all RfqEngg");
                var result = _mapper.Map<IEnumerable<RfqEnggDto>>(listOfRfqengg);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RfqEngg Successfully";
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
        //pass rfq id and get customer support data

        [HttpGet("{RfqNumber}")]
        public async Task<IActionResult> RfqCustomerSupportByRfqNumber(string RfqNumber)
        {
            ServiceResponse<RfqCustomerSupportDto> serviceResponse = new ServiceResponse<RfqCustomerSupportDto>();

            try
            {
                var rfq = await _repository.RfqCustomerSupportByRfqNumber(RfqNumber);

                if (rfq == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqCustomerSupportByRfqNumber with id: {RfqNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqCustomerSupportByRfqNumber with id: {RfqNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned RfqCustomerSupportByRfqNumber with id: {RfqNumber}");

                    RfqCustomerSupportDto rfqDto = _mapper.Map<RfqCustomerSupportDto>(rfq);

                    List<RfqCustomerSupportItemDto> rfqItemsDtos = new List<RfqCustomerSupportItemDto>();
                    foreach (var itemDetails in rfq.rfqCustomerSupportItems)
                    {
                        RfqCustomerSupportItemDto rfqItemDto = _mapper.Map<RfqCustomerSupportItemDto>(itemDetails);
                        rfqItemDto.rfqCSDeliverySchedules = _mapper.Map<List<RfqCSDeliveryScheduleDto>>(itemDetails.rfqCSDeliverySchedule);
                        rfqItemsDtos.Add(rfqItemDto);
                    }
                    rfqDto.rfqCustomerSupportItems = rfqItemsDtos;

                    //var result = _mapper.Map<RfqCustomerSupportDto>(rfq);
                    serviceResponse.Data = rfqDto;
                    serviceResponse.Message = $"Returned RfqCustomerSupportByRfqNumber with id: {RfqNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside RfqCustomerSupportByRfqNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }


        }
        //get RfqLPCosting by Rfqnumber

        [HttpGet("{RfqNumber}")]
        public async Task<IActionResult> RfqLPCostingByRfqNumber(string RfqNumber)
        {
            ServiceResponse<RfqLPCostingDto> serviceResponse = new ServiceResponse<RfqLPCostingDto>();

            try
            {
                var rfqlpcosting = await _rfqlpcostingRepository.RfqLPCostingByRfqNumber(RfqNumber);

                if (rfqlpcosting == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqlpcostingByRfqNumber with id: {RfqNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqlpcostingByRfqNumber with id: {RfqNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned RfqlpcostingByRfqNumber with id: {rfqlpcosting}");

                    RfqLPCostingDto rfqlpcostingDto = _mapper.Map<RfqLPCostingDto>(rfqlpcosting);

                    List<RfqLPCostingItemDto> rfqlpcostingItemsDtos = new List<RfqLPCostingItemDto>();
                    foreach (var itemDetails in rfqlpcosting.rfqLPCostingItems)
                    {
                        RfqLPCostingItemDto rfqlpcostingItemDto = _mapper.Map<RfqLPCostingItemDto>(itemDetails);
                        rfqlpcostingItemDto.rfqLPCostingProcesses = _mapper.Map<List<RfqLPCostingProcessDto>>(itemDetails.rfqLPCostingProcesses);
                       // rfqlpcostingItemDto.rfqLPCostingNREConsumables = _mapper.Map<List<RfqLPCostingNREConsumableDto>>(itemDetails.rfqLPCostingNREConsumables);
                       // rfqlpcostingItemDto.rfqLPCostingOtherCharges = _mapper.Map<List<RfqLPCostingOtherChargesDto>>(rfqlpcostingItemDto.rfqLPCostingOtherCharges);

                        rfqlpcostingItemsDtos.Add(rfqlpcostingItemDto);
                    }
                    rfqlpcostingDto.rfqLPCostingItems = rfqlpcostingItemsDtos;

                    //var result = _mapper.Map<RfqCustomerSupportDto>(rfq);
                    serviceResponse.Data = rfqlpcostingDto;
                    serviceResponse.Message = $"Returned RfqEnggByRfqNumber with id: {RfqNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside RfqCustomerSupportByRfqNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }


        }

        //get RfqEngg by Rfqnumber
        [HttpGet("{RfqNumber}")]
        public async Task<IActionResult> RfqEnggByRfqNumber(string RfqNumber)
        {
            ServiceResponse<RfqEnggDto> serviceResponse = new ServiceResponse<RfqEnggDto>();

            try
            {
                var rfqEngg = await _rfqenggRepository.RfqEnggByRfqNumber(RfqNumber);

                if (rfqEngg == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqEnggByRfqNumber with id: {RfqNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqEnggByRfqNumber with id: {RfqNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned RfqEnggByRfqNumber with id: {RfqNumber}");

                    RfqEnggDto rfqenggDto = _mapper.Map<RfqEnggDto>(rfqEngg);

                    List<RfqEnggItemDto> rfqenggItemsDtos = new List<RfqEnggItemDto>();
                    foreach (var itemDetails in rfqEngg.rfqEnggItems)
                    {
                        RfqEnggItemDto rfqenggItemDto = _mapper.Map<RfqEnggItemDto>(itemDetails);
                        rfqenggItemsDtos.Add(rfqenggItemDto);
                    }
                    rfqenggDto.rfqEnggItems = rfqenggItemsDtos;

                    //var result = _mapper.Map<RfqCustomerSupportDto>(rfq);
                    serviceResponse.Data = rfqenggDto;
                    serviceResponse.Message = $"Returned RfqEnggByRfqNumber with id: {RfqNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside RfqCustomerSupportByRfqNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }


        }


        //rfq getByid function
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRfqById(int id)
        {
            ServiceResponse<RfqDto> serviceResponse = new ServiceResponse<RfqDto>();

            try
            {
                var rfq = await _rfqRepository.GetRfqById(id);

                if (rfq == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Rfq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Rfq with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<RfqDto>(rfq);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned Rfq with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetrfqById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        // RfqEngg GetbyId
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRfqEnggById(int id)
        {
            ServiceResponse<RfqEnggDto> serviceResponse = new ServiceResponse<RfqEnggDto>();

            try
            {
                var rfqengg = await _rfqenggRepository.GetRfqEnggById(id);

                if (rfqengg == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqEngg with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqEngg with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned RfqEngg with id: {id}");
                    var result = _mapper.Map<RfqEnggDto>(rfqengg);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned RfqEngg with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetrfqEnggById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        //Get RfqLPCostingById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRfqLPCostingById(int id)
        {
            ServiceResponse<RfqLPCostingDto> serviceResponse = new ServiceResponse<RfqLPCostingDto>();

            try
            {
                var rfqlpcosting = await _rfqlpcostingRepository.GetRfqLPCostingById(id);

                if (rfqlpcosting == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqLPCosting with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqLPCosting with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned RfqLPCosting with id: {id}");
                    //var result = _mapper.Map<RfqLPCostingDto>(rfqlpcosting);
                    RfqLPCostingDto rfqlpcostingDto = _mapper.Map<RfqLPCostingDto>(rfqlpcosting);//Main model mapping

                    //below mapping is child under child  

                    List<RfqLPCostingItemDto> rfqlpcostingItemDtos = new List<RfqLPCostingItemDto>();

                    foreach (var lpcostingitemDetails in rfqlpcosting.rfqLPCostingItems)
                    {
                        RfqLPCostingItemDto rfqlpcostingItemDto = _mapper.Map<RfqLPCostingItemDto>(lpcostingitemDetails);
                        rfqlpcostingItemDto.rfqLPCostingProcesses = _mapper.Map<List<RfqLPCostingProcessDto>>(rfqlpcostingItemDto.rfqLPCostingProcesses);
                        rfqlpcostingItemDto.rfqLPCostingNREConsumables = _mapper.Map<List<RfqLPCostingNREConsumableDto>>(rfqlpcostingItemDto.rfqLPCostingNREConsumables);
                        rfqlpcostingItemDto.rfqLPCostingOtherCharges = _mapper.Map<List<RfqLPCostingOtherChargesDto>>(rfqlpcostingItemDto.rfqLPCostingOtherCharges);

                        rfqlpcostingItemDtos.Add(rfqlpcostingItemDto);
                    }

                    rfqlpcostingDto.rfqLPCostingItems = rfqlpcostingItemDtos;
                    serviceResponse.Data = rfqlpcostingDto;
                    serviceResponse.Message = $"Returned RfqLPCosting with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside getRfqLPCostingById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRfqCustomerSupportById(int id)
        {
            ServiceResponse<RfqCustomerSupportDto> serviceResponse = new ServiceResponse<RfqCustomerSupportDto>();

            try
            {
                var rfq = await _repository.GetRfqCustomerSupportById(id);

                if (rfq == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqCustomerSupport with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqCustomerSupport with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned RfqCustomerSupport with id: {id}");
                    
                    RfqCustomerSupportDto rfqDto = _mapper.Map<RfqCustomerSupportDto>(rfq);

                    List<RfqCustomerSupportItemDto> rfqItemsDtos = new List<RfqCustomerSupportItemDto>();
                    foreach (var itemDetails in rfq.rfqCustomerSupportItems)
                    {
                        RfqCustomerSupportItemDto rfqItemDto = _mapper.Map<RfqCustomerSupportItemDto>(itemDetails);
                        rfqItemDto.rfqCSDeliverySchedules = _mapper.Map<List<RfqCSDeliveryScheduleDto>>(itemDetails.rfqCSDeliverySchedule);
                        rfqItemsDtos.Add(rfqItemDto);
                    }
                    rfqDto.rfqCustomerSupportItems = rfqItemsDtos;

                    //var result = _mapper.Map<RfqCustomerSupportDto>(rfq);
                    serviceResponse.Data = rfqDto;
                    serviceResponse.Message = $"Returned RfqCustomerSupport with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRfqCustomerSupportById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
             
        }

         [HttpPost]
        public async Task<IActionResult> CreateRfqCustomerSupport([FromBody] RfqCustomerSupportPostDto rfqCustomerSupportDto)
        {
            ServiceResponse<RfqCustomerSupportDto> serviceResponse = new ServiceResponse<RfqCustomerSupportDto>();

            try
            {
                if (rfqCustomerSupportDto is null)
                {
                    _logger.LogError("RfqCustomerSupport object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RfqCustomerSupport object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid RfqCustomerSupport object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid RfqCustomerSupport object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                //var customfield = _mapper.Map<IEnumerable<RfqCustomerSupportItems>>(rfqCustomerSupportDto.rfqCustomerSupportItems);
                //var customernotes = _mapper.Map<IEnumerable<RfqCustomerSupportNotes>>(rfqCustomerSupportDto.rfqCustomerSupportNotes);

                //var rfqs = _mapper.Map<RfqCustomerSupport>(rfqCustomerSupportDto);

                //rfqs.rfqCustomerSupportItems = customfield.ToList();
                //rfqs.rfqCustomerSupportNotes = customernotes.ToList();

                var rfqCustomerSupportList = _mapper.Map<RfqCustomerSupport>(rfqCustomerSupportDto);


                var rfqCustomerSupportItemDto = rfqCustomerSupportDto.rfqCustomerSupportItems;

                var rfqCustomerSupportLists = new List<RfqCustomerSupportItems>();
                for (int i = 0; i < rfqCustomerSupportItemDto.Count; i++)
                {
                    RfqCustomerSupportItems rfqCSItems = _mapper.Map<RfqCustomerSupportItems>(rfqCustomerSupportItemDto[i]);
                    rfqCSItems.rfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliverySchedule>>(rfqCustomerSupportItemDto[i].rfqCSDeliverySchedules);
                    rfqCustomerSupportLists.Add(rfqCSItems);

                }
                rfqCustomerSupportList.rfqCustomerSupportItems = rfqCustomerSupportLists;

                _repository.CreateRfqCustomerSupport(rfqCustomerSupportList);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetRfqCustomerSupportById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateRfqCustomerSupport action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //rfq create function


        [HttpPost]
        public async Task<IActionResult> CreateRfq([FromBody] RfqPostDto rfqPostDto)
        {
            ServiceResponse<RfqDto> serviceResponse = new ServiceResponse<RfqDto>();

            try
            {
                if (rfqPostDto is null)
                {
                    _logger.LogError("Rfq object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Rfq object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Rfq object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Rfq object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                 var rfqs = _mapper.Map<Rfq>(rfqPostDto);

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                _rfqRepository.CreateRfq(rfqs);

                _rfqRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetRfqById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateRfq action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Create RfqLPCosting
        [HttpPost]
        public async Task<IActionResult> CreateRfqLPcosting([FromBody] RfqLPCostingDtoPost rfqLPCostingDtoPost)
        {
            ServiceResponse<RfqLPCostingDto> serviceResponse = new ServiceResponse<RfqLPCostingDto>();

            try
            {
                if (rfqLPCostingDtoPost is null)
                {
                    _logger.LogError("rfqLPCosting object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "rfqLPCosting object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid rfqLPCosting object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid rfqLPCosting object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse); 
                }
                var rfqLPCosting = _mapper.Map<RfqLPCosting>(rfqLPCostingDtoPost);
                var rfqLPCostingDto = rfqLPCostingDtoPost.rfqLPCostingItems;

                var lpcostingItemList = new List<RfqLPCostingItem>();
                for (int i = 0; i < rfqLPCostingDto.Count; i++)
                {
                    RfqLPCostingItem lpcostingItemListDetail = _mapper.Map<RfqLPCostingItem>(rfqLPCostingDto[i]);
                    lpcostingItemListDetail.rfqLPCostingProcesses = _mapper.Map<List<RfqLPCostingProcess>>(rfqLPCostingDto[i].rfqLPCostingProcesses);
                    lpcostingItemListDetail.rfqLPCostingNREConsumables = _mapper.Map<List<RfqLPCostingNREConsumable>>(rfqLPCostingDto[i].rfqLPCostingNREConsumables);
                    lpcostingItemListDetail.rfqLPCostingOtherCharges = _mapper.Map<List<RfqLPCostingOtherCharges>>(rfqLPCostingDto[i].rfqLPCostingOtherCharges);
                    lpcostingItemList.Add(lpcostingItemListDetail);

                }
                rfqLPCosting.rfqLPCostingItems = lpcostingItemList;
               

                _rfqlpcostingRepository.CreateRfqLPCosting(rfqLPCosting);
                _rfqlpcostingRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetRfqLPCostingById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateRfqLPCostong action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Create RfqEngg
        [HttpPost]
        public async Task<IActionResult> CreateRfqEngg([FromBody] RfqEnggDtoPost rfqEnggDtoPost)
        {
            ServiceResponse<RfqEnggDto> serviceResponse = new ServiceResponse<RfqEnggDto>();

            try
            {
                if (rfqEnggDtoPost is null)
                {
                    _logger.LogError("RfqEngg object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RfqEngg object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid RfqEngg object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid RfqEngg object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var rfqenggs = _mapper.Map<RfqEngg>(rfqEnggDtoPost);

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                _rfqenggRepository.CreateRfqEngg(rfqenggs);
                _rfqenggRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetRfqById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateRfqEngg action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //update rfq function
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfq(int id, [FromBody] RfqUpdateDto rfqUpdateDto)
        {
            ServiceResponse<RfqDto> serviceResponse = new ServiceResponse<RfqDto>();

            try
            {
                if (rfqUpdateDto is null)
                {
                    _logger.LogError("Rfq object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Rfq object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Rfq object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Rfq object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var rfq = await _rfqRepository.GetRfqById(id);
                if (rfq is null)
                {
                    _logger.LogError($"Rfq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Rfq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                 var data = _mapper.Map(rfqUpdateDto, rfq);

 
                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                string result = await _rfqRepository.UpdateRfq(data);
                _logger.LogInfo(result);
                _rfqRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRfq action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // Update rfqengg
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfqEngg(int id, [FromBody] RfqEnggDtoUpdate rfqEnggDtoUpdate)
        {
            ServiceResponse<RfqEnggDto> serviceResponse = new ServiceResponse<RfqEnggDto>();

            try
            {
                if (rfqEnggDtoUpdate is null)
                {
                    _logger.LogError("RfqEngg object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update RfqEngg object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Rfqengg object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Rfqengg object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var rfqengg = await _rfqenggRepository.GetRfqEnggById(id);
                if (rfqengg is null)
                {
                    _logger.LogError($"RfqEngg with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Rfqengg with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var data = _mapper.Map(rfqEnggDtoUpdate, rfqengg);


                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                string result = await _rfqenggRepository.UpdateRfqEngg(data);
                _logger.LogInfo(result);
                _rfqenggRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRfqEngg action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Update RfqLPCosting
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfqLPCosting(int id, [FromBody] RfqLPCostingDtoUpdate rfqLPCostingDtoUpdate)
        {
            ServiceResponse<RfqLPCostingDto> serviceResponse = new ServiceResponse<RfqLPCostingDto>();

            try
            {
                if (rfqLPCostingDtoUpdate is null)
                {
                    _logger.LogError("RfqLPCosting object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update RfqLPCosting object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid RfqLPCosting object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update RfqLPCosting object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var rfqlpcosting = await _rfqlpcostingRepository.GetRfqLPCostingById(id);
                if (rfqlpcosting is null)
                {
                    _logger.LogError($"RfqLPCosting with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update RfqLPCosting with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var rfqlpcostingList = _mapper.Map<RfqLPCosting>(rfqLPCostingDtoUpdate);

                var lpcostingitemtemDto = rfqLPCostingDtoUpdate.rfqLPCostingItems;

                var rfqlpcostingitemList = new List<RfqLPCostingItem>();
                for (int i = 0; i < lpcostingitemtemDto.Count; i++)
                {
                    RfqLPCostingItem lpcostingItemDetail = _mapper.Map<RfqLPCostingItem>(lpcostingitemtemDto[i]);
                    lpcostingItemDetail.rfqLPCostingProcesses = _mapper.Map<List<RfqLPCostingProcess>>(lpcostingitemtemDto[i].rfqLPCostingProcesses);
                    lpcostingItemDetail.rfqLPCostingNREConsumables = _mapper.Map<List<RfqLPCostingNREConsumable>>(lpcostingitemtemDto[i].rfqLPCostingNREConsumables);
                    lpcostingItemDetail.rfqLPCostingOtherCharges = _mapper.Map<List<RfqLPCostingOtherCharges>>(lpcostingitemtemDto[i].rfqLPCostingOtherCharges);

                    rfqlpcostingitemList.Add(lpcostingItemDetail);

                }
                var data = _mapper.Map(rfqLPCostingDtoUpdate, rfqlpcosting);               

                string result = await _rfqlpcostingRepository.UpdateRfqLPCosting(data);
                _logger.LogInfo(result);
                _rfqlpcostingRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRfqLPCosting action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfqCustomerSupport(int id, [FromBody] RfqCustomerSupportUpdateDto rfqCustomerSupportUpdateDto)
        {
            ServiceResponse<RfqCustomerSupportDto> serviceResponse = new ServiceResponse<RfqCustomerSupportDto>();

            try
            {
                if (rfqCustomerSupportUpdateDto is null)
                {
                    _logger.LogError("RfqCustomerSupport object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update RfqCustomerSupport object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid RfqCustomerSupport object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update RfqCustomerSupport object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var rfq = await _repository.GetRfqCustomerSupportById(id);
                if (rfq is null)
                {
                    _logger.LogError($"RfqCustomerSupport with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update RfqCustomerSupport with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                // var customfield = _mapper.Map<IEnumerable<RfqCustomerSupportItems>>(rfq.rfqCustomerSupportItems);
                //var customnotes = _mapper.Map<IEnumerable<RfqCustomerSupportNotes>>(rfq.rfqCustomerSupportNotes);

                //                var data = _mapper.Map(rfqCustomerSupportUpdateDto, rfq);

                //              data.rfqCustomerSupportItems = customfield.ToList();
                //            data.rfqCustomerSupportNotes = customnotes.ToList();

                var rfqcustomerlist = _mapper.Map<RfqCustomerSupport>(rfq);

                var rfqItemDto = rfqCustomerSupportUpdateDto.rfqCustomerSupportItems;

                var rfqCsItemList = new List<RfqCustomerSupportItems>();
                for (int i = 0; i < rfqItemDto.Count; i++)
                {
                    RfqCustomerSupportItems rfqItemDetail = _mapper.Map<RfqCustomerSupportItems>(rfqItemDto[i]);
                    rfqItemDetail.rfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliverySchedule>>(rfqItemDto[i].rfqCSDeliverySchedules);
                    rfqCsItemList.Add(rfqItemDetail);

                }
                rfqcustomerlist.rfqCustomerSupportItems = rfqCsItemList;

                var data = _mapper.Map(rfqCustomerSupportUpdateDto, rfqcustomerlist);

                string result = await _repository.UpdateRfqCustomerSupport(data);
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
                _logger.LogError($"Something went wrong inside UpdateRfqCustomerSupport action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //Delete RFq
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRfq(int id)
        {
            ServiceResponse<RfqDto> serviceResponse = new ServiceResponse<RfqDto>();

            try
            {
                var rfq = await _rfqRepository.GetRfqById(id);
                if (rfq == null)
                {
                    _logger.LogError($"Rfq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Rfq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _rfqRepository.DeleteRfq(rfq);
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
                _logger.LogError($"Something went wrong inside DeleteRfq action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //delete RfqEngg
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRfqengg(int id)
        {
            ServiceResponse<RfqEnggDto> serviceResponse = new ServiceResponse<RfqEnggDto>();

            try
            {
                var rfqengg = await _rfqenggRepository.GetRfqEnggById(id);
                if (rfqengg == null)
                {
                    _logger.LogError($"Rfqengg with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Rfqengg with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _rfqenggRepository.DeleteRfqEngg(rfqengg);
                _logger.LogInfo(result);
                _rfqenggRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went RfqEngg inside DeleteRfq action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //Delete RfqLPCosting

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRfqLPCosting(int id)
        {
            ServiceResponse<RfqLPCostingDto> serviceResponse = new ServiceResponse<RfqLPCostingDto>();

            try
            {
                var rfqlpcosting = await _rfqlpcostingRepository.GetRfqLPCostingById(id);
                if (rfqlpcosting == null)
                {
                    _logger.LogError($"RfqLPCosting with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete RfqLPCosting with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _rfqlpcostingRepository.DeleteRfqLPCosting(rfqlpcosting);
                _logger.LogInfo(result);
                _rfqlpcostingRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went RfqLPCosting inside DeleteRfq action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRfqCustomerSupport(int id)
        {
            ServiceResponse<RfqCustomerSupportDto> serviceResponse = new ServiceResponse<RfqCustomerSupportDto>();

            try
            {
                var rfq = await _repository.GetRfqCustomerSupportById(id);
                if (rfq == null)
                {
                    _logger.LogError($"RfqCustomerSupport with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete RfqCustomerSupport with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteRfqCustomerSupport(rfq);
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
                _logger.LogError($"Something went wrong inside DeleteRfqCustomerSupport action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
