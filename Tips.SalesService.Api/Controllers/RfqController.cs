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
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Tips.SalesService.Api.Entities.Enum;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RfqController : ControllerBase
    {
        private IRfqCustomerSupportRepository _repository;
        private IRfqCustomerSupportItemRepository _itemRepository;
        private IItemPriceListRepository _itemPriceListRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IRfqRepository _rfqRepository;
        private IRfqEnggRepository _rfqenggRepository;
        private IRfqEnggItemRepository _rfqenggItemRepository;
        private IRfqLPCostingRepository _rfqlpcostingRepository;
        private IReleaseLpRepository _releaseLpRepository;
        private IRfqCustomFieldRepository _rfqCustomFieldRepository;
        private IRfqCustomGroupRepository _rfqCustomGroupRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public RfqController(IRfqCustomGroupRepository rfqCustomGroupRepository, IItemPriceListRepository itemPriceListRepository, IRfqCustomFieldRepository rfqCustomFieldRepository
            , IRfqEnggItemRepository rfqenggItemRepository, IReleaseLpRepository releaseLpRepository, 
            IRfqCustomerSupportRepository repository, IRfqCustomerSupportItemRepository rfqCustomerSupportItemRepository,
            IRfqRepository rfqRepository, IRfqLPCostingRepository rfqLPCostingRepository, IRfqEnggRepository rfqEnggRepository,
            ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _rfqRepository = rfqRepository;
            _rfqenggRepository = rfqEnggRepository;
            _rfqenggItemRepository = rfqenggItemRepository;
            _rfqlpcostingRepository = rfqLPCostingRepository;
            _itemRepository = rfqCustomerSupportItemRepository;
            _releaseLpRepository = releaseLpRepository;
            _rfqCustomFieldRepository = rfqCustomFieldRepository;
            _rfqCustomGroupRepository = rfqCustomGroupRepository;
            _itemPriceListRepository = itemPriceListRepository;
            _httpClient = httpClient;
            _config = config;
        }

        //rfq getall 
        [HttpGet]
        public async Task<IActionResult> GetAllRfq([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<RfqDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqDto>>();

            try
            {
                var getAllRfq = await _rfqRepository.GetAllRfq(pagingParameter, searchParammes);
                for(int i = 0; i < getAllRfq.Count; i++)
                {
                    var rfq = getAllRfq[i].RfqNumber;
                    var rfqCsCount = await _itemRepository.GetRfqCustomerSupportItemByRfqNumber(rfq);
                    if (getAllRfq[i].isSourcingAvailable == true)
                    {

                        var rfqEnggCount = await _rfqenggItemRepository.GetRfqEnggCountByRfqNumber(rfq);
                        var rfqEnggRelese = await _rfqenggItemRepository.GetRfqEnggRelesedDetailsByRfqNumber(rfq);
                        var rfqEnggUnRelesedCount = rfqEnggCount.Count() - rfqEnggRelese.Count();
                        if (rfqEnggRelese.Count() == 0)
                        {
                            getAllRfq[i].IsEnggRelease = CsRelease.NotYetReleased;
                        }
                        if (rfqEnggUnRelesedCount == 0)
                        {
                            getAllRfq[i].IsEnggRelease = CsRelease.FullyRelease;
                        }
                        if (rfqEnggUnRelesedCount != 0 && rfqEnggRelese.Count() != 0)
                        {
                            getAllRfq[i].IsEnggRelease = CsRelease.PartiallyRelease;
                        }
                        if (rfqEnggCount.Count() != 0)
                        {
                            getAllRfq[i].EnggComplete = EnggStatus.EnggCompleted;
                        }
                        else
                        {
                            getAllRfq[i].EnggComplete = EnggStatus.EnggNotYetCompleted;
                        }
                    }
                   
                    var rfqCsRelesed = await _itemRepository.GetRfqCustomerSupportRelesedDetailsByRfqNumber(rfq);
            
                    var rfqCsUnRelesedCount = rfqCsCount.Count() - rfqCsRelesed.Count();
                    if(rfqCsRelesed.Count() == 0 )
                    {
                        getAllRfq[i].IsCsRelease = CsRelease.NotYetReleased;
                    }
                    //if(rfqCsUnRelesedCount == 0 && rfqCsCount.Count() != 0)
                    if (rfqCsUnRelesedCount == 0)
                    {
                        getAllRfq[i].IsCsRelease = CsRelease.FullyRelease;
                    }
                    //if(rfqCsUnRelesedCount != 0 && rfqCsRelesed.Count() != 0)
                    if (rfqCsUnRelesedCount != 0)
                    {
                        getAllRfq[i].IsCsRelease = CsRelease.PartiallyRelease;
                    }
                    if (rfqCsCount.Count() != 0)
                    {
                        getAllRfq[i].CsComplete = CsStatus.CsCompleted;
                    }
                    else
                    {
                        getAllRfq[i].CsComplete = CsStatus.CsNotYetCompleted;
                    }
                }
                var metadata = new
                {
                    getAllRfq.TotalCount,
                    getAllRfq.PageSize,
                    getAllRfq.CurrentPage,
                    getAllRfq.HasNext,
                    getAllRfq.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all rfq");
                
                var result = _mapper.Map<IEnumerable<RfqDto>>(getAllRfq);
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
        public async Task<IActionResult> GetAllRfqCustomerSupport([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<RfqCustomerSupportDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqCustomerSupportDto>>();

            try
            {
                var getAllRfqCS = await _repository.GetAllRfqCustomerSupport(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllRfqCS.TotalCount,
                    getAllRfqCS.PageSize,
                    getAllRfqCS.CurrentPage,
                    getAllRfqCS.HasNext,
                    getAllRfqCS.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all RfqCustomerSupport");
                var result = _mapper.Map<IEnumerable<RfqCustomerSupportDto>>(getAllRfqCS);
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
                var getAllActiveRfqNos = await _rfqRepository.GetAllActiveRfqNumberList();

                var result = _mapper.Map<IEnumerable<RfqNumberListDto>>(getAllActiveRfqNos);
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
        public async Task<IActionResult> GetAllRfqLPCosting([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<RfqLPCostingDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqLPCostingDto>>();

            try
            {
                var getAllRfqLPCosting = await _rfqlpcostingRepository.GetAllRfqLPCosting(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllRfqLPCosting.TotalCount,
                    getAllRfqLPCosting.PageSize,
                    getAllRfqLPCosting.CurrentPage,
                    getAllRfqLPCosting.HasNext,
                    getAllRfqLPCosting.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all Rfqlpcosting");
                var result = _mapper.Map<IEnumerable<RfqLPCostingDto>>(getAllRfqLPCosting);
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
        public async Task<IActionResult> GetAllRfqEngg([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<RfqEnggDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqEnggDto>>();

            try
            {
                var getAllRfqengg = await _rfqenggRepository.GetAllRfqEngg(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllRfqengg.TotalCount,
                    getAllRfqengg.PageSize,
                    getAllRfqengg.CurrentPage,
                    getAllRfqengg.HasNext,
                    getAllRfqengg.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all RfqEngg");
                var result = _mapper.Map<IEnumerable<RfqEnggDto>>(getAllRfqengg);
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
        //get list of child item under fg bom

        [HttpGet("{RfqNumber}")]
        public async Task<IActionResult> GetFGBomChildItemDetails(string RfqNumber)
        {
            ServiceResponse<IEnumerable<RfqSourcingItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqSourcingItemsDto>>();
            try
            {
                var getAllRfqEnggRelesedList = await _rfqenggItemRepository.GetRfqEnggRelesedDetailsByRfqNumber(RfqNumber);
                List<string?>? itemDetails = getAllRfqEnggRelesedList?.Select(x => x.ItemNumber).ToList();
                List<EnggBomFGItemNumberWithQtyDto>? itemsRoutingDetailsDynamic = new List<EnggBomFGItemNumberWithQtyDto>();

                if (itemDetails != null)
                {
                    var itemDetailsString = JsonConvert.SerializeObject(itemDetails);
                    var content = new StringContent(itemDetailsString, Encoding.UTF8, "application/json");
                    var inventoryObjectResult = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "GetFGBomItemsChildDetails"), content);
                    var itemsRoutingDetailsJsonString = await inventoryObjectResult.Content.ReadAsStringAsync();
                    dynamic itemsRoutingDetailsJson = JsonConvert.DeserializeObject(itemsRoutingDetailsJsonString);
                    var data = itemsRoutingDetailsJson.data;
                    itemsRoutingDetailsDynamic = data.ToObject<List<EnggBomFGItemNumberWithQtyDto>>();
                }
                var result = _mapper.Map<IEnumerable<RfqSourcingItemsDto>>(itemsRoutingDetailsDynamic);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all getAllRfqEnggRelesedList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside getAllRfqEnggRelesedList action";
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
                var getRfqCSByRfqNO = await _repository.GetRfqCustomerSupportByRfqNumber(RfqNumber);

                if (getRfqCSByRfqNO == null)
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

                    RfqCustomerSupportDto rfqCSDto = _mapper.Map<RfqCustomerSupportDto>(getRfqCSByRfqNO);

                    List<RfqCustomerSupportItemDto> rfqItemsDtos = new List<RfqCustomerSupportItemDto>();
                    foreach (var rfqCSItemDetail in getRfqCSByRfqNO.RfqCustomerSupportItems)
                    {
                        RfqCustomerSupportItemDto rfqItemDto = _mapper.Map<RfqCustomerSupportItemDto>(rfqCSItemDetail);
                        rfqItemDto.RfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliveryScheduleDto>>(rfqCSItemDetail.RfqCSDeliverySchedule);
                        rfqItemsDtos.Add(rfqItemDto);
                    }
                    rfqCSDto.RfqCustomerSupportItems = rfqItemsDtos;

                    serviceResponse.Data = rfqCSDto;
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

        [HttpGet("{RfqNumber}")]
        public async Task<IActionResult> GetAllActiveRfqCustomerSupportItemsByRfqNumber(string RfqNumber)
        {
            ServiceResponse<IEnumerable<RfqCustomerSupportItemDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqCustomerSupportItemDto>>();
            try
            {
                var getAllActiveRfqCSItemsByRfqNo = await _itemRepository.GetAllActiveRfqCustomerSupportItemsByRfqNumber(RfqNumber);
                var result = _mapper.Map<IEnumerable<RfqCustomerSupportItemDto>>(getAllActiveRfqCSItemsByRfqNo);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ActiveRfqCustomerSupportItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveRfqCustomerSupportItemsByRfqNumber action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{CustomerId}")]
        public async Task<IActionResult> GetAllActiveRfqNumberListByCustomerId(string CustomerId)
        {
            ServiceResponse<IEnumerable<RfqNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqNumberListDto>>();
            try
            {
                var getAllActiveRfqNumberListByCustomerId = await _rfqRepository.GetAllActiveRfqNumberListByCustomerId(CustomerId);
                var result = _mapper.Map<IEnumerable<RfqNumberListDto>>(getAllActiveRfqNumberListByCustomerId);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ActiveRfqNumberListByCustomerId";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveRfqNumberListByCustomerId action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{RfqNumber}")]
        public async Task<IActionResult> GetAllActiveRfqEnggItemByRfqNumber(string RfqNumber)
        {
            ServiceResponse<IEnumerable<RfqEnggItemDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqEnggItemDto>>();
            try
            {
                var getAllActiveRfqEnggByRfqNo = await _rfqenggItemRepository.GetAllActiveRfqEnggItemByRfqNumber(RfqNumber);
                var result = _mapper.Map<IEnumerable<RfqEnggItemDto>>(getAllActiveRfqEnggByRfqNo);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ActiveRfqEnggItem";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveRfqEnggItemByRfqNumber action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //nayagam

        [HttpGet]
        public async Task<IActionResult> GetDetailsForLPCostingByRfqNumber(string rfqNumber)
        {
            ServiceResponse<RfqLPCostingDto> serviceResponse = new ServiceResponse<RfqLPCostingDto>();

            try
            {   
                var rfqEnggDetails = await _rfqenggRepository.GetRfqEnggByRfqNumber(rfqNumber);

                List<string?>? itemDetails = rfqEnggDetails?.RfqEnggItems?.Select(x => x.ItemNumber).ToList();
                
                List<ItemMasterRoutingListDto>? itemsRoutingDetailsDynamic = new List<ItemMasterRoutingListDto>();
                if (itemDetails != null)
                {
                    var itemDetailsString = JsonConvert.SerializeObject(itemDetails);
                    var content = new StringContent(itemDetailsString, Encoding.UTF8, "application/json");
                    var inventoryObjectResult = await _httpClient.PostAsync(string.Concat(_config["ItemMasterAPI"], "GetItemsRoutingDetailsForLpCosting"), content);
                    var itemsRoutingDetailsJsonString = await inventoryObjectResult.Content.ReadAsStringAsync();
                    dynamic itemsRoutingDetailsJson = JsonConvert.DeserializeObject(itemsRoutingDetailsJsonString);
                    var data = itemsRoutingDetailsJson.data;
                    itemsRoutingDetailsDynamic = data.ToObject<List<ItemMasterRoutingListDto>>();
                }


                List<RfqLPCostingItem> rfqLPCostingItems = new List<RfqLPCostingItem>();

                foreach (var item in rfqEnggDetails.RfqEnggItems)
                {
                    var itemProcessList = itemsRoutingDetailsDynamic.Where(i => i.ItemNumber == item.ItemNumber).ToList();
                    List<RfqLPCostingProcess> processStepsList = _mapper.Map<List<RfqLPCostingProcess>>(itemProcessList);
                    RfqLPCostingItem rfqLPCostingItem = new RfqLPCostingItem
                    {
                        ItemNumber = item.ItemNumber,
                        Description = item.Description,
                        CustomerItemNumber = item.CustomerItemNumber,
                        TotalCost = 0,
                        MaterialCost =0,
                        MarkUpForMaterial =0,
                        RfqLPCostingProcesses = processStepsList,
                        RfqLPCostingNREConsumables =null,
                        RfqLPCostingOtherCharges =null
                    };
                    rfqLPCostingItems.Add(rfqLPCostingItem);
                }

                var rfqLPCostingDetail = new RfqLPCosting();
                rfqLPCostingDetail.RfqNumber = rfqEnggDetails.RFQNumber;
                rfqLPCostingDetail.CustomerName = rfqEnggDetails.CustomerName;
                rfqLPCostingDetail.RfqLPCostingItems = rfqLPCostingItems;

                RfqLPCostingDto rfqLPCostingDto = _mapper.Map<RfqLPCostingDto>(rfqLPCostingDetail);
                
                serviceResponse.Data = rfqLPCostingDto;
                serviceResponse.Message = $"Returned RfqEnggByRfqNumber with rfqNumber: {rfqNumber}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
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
        public async Task<IActionResult> GetRfqLPCostingByRfqNumber(string RfqNumber)
        {
            ServiceResponse<RfqLPCostingDto> serviceResponse = new ServiceResponse<RfqLPCostingDto>();

            try
            {
                var getRfqLPCostingByRfqNumber = await _rfqlpcostingRepository.GetRfqLPCostingByRfqNumber(RfqNumber);

                if (getRfqLPCostingByRfqNumber == null)
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
                    _logger.LogInfo($"Returned RfqlpcostingByRfqNumber with id: {getRfqLPCostingByRfqNumber}");

                    RfqLPCostingDto rfqLpCostingDto = _mapper.Map<RfqLPCostingDto>(getRfqLPCostingByRfqNumber);

                    List<RfqLPCostingItemDto> rfqLpCostingItemsDtos = new List<RfqLPCostingItemDto>();

                    foreach (var lpCostingitemDetail in getRfqLPCostingByRfqNumber.RfqLPCostingItems)
                    {
                        RfqLPCostingItemDto rfqlpcostingItemDto = _mapper.Map<RfqLPCostingItemDto>(lpCostingitemDetail);
                        rfqlpcostingItemDto.RfqLPCostingProcesses = _mapper.Map<List<RfqLPCostingProcessDto>>(lpCostingitemDetail.RfqLPCostingProcesses);
                        rfqlpcostingItemDto.RfqLPCostingNREConsumables = _mapper.Map<List<RfqLPCostingNREConsumableDto>>(lpCostingitemDetail.RfqLPCostingNREConsumables);
                        rfqlpcostingItemDto.RfqLPCostingOtherCharges = _mapper.Map<List<RfqLPCostingOtherChargesDto>>(lpCostingitemDetail.RfqLPCostingOtherCharges);

                        rfqLpCostingItemsDtos.Add(rfqlpcostingItemDto);
                    }
                    rfqLpCostingDto.RfqLPCostingItems = rfqLpCostingItemsDtos;

                    serviceResponse.Data = rfqLpCostingDto;
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
        public async Task<IActionResult> GetRfqEnggByRfqNumber(string RfqNumber)
        {
            ServiceResponse<RfqEnggDto> serviceResponse = new ServiceResponse<RfqEnggDto>();

            try
            {
                var getRfqEnggByRfqNo = await _rfqenggRepository.GetRfqEnggByRfqNumber(RfqNumber);

                if (getRfqEnggByRfqNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GetRfqEnggByRfqNumber with id: {RfqNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GetRfqEnggByRfqNumber with id: {RfqNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned RfqEnggByRfqNumber with id: {RfqNumber}");

                    RfqEnggDto rfqEnggDto = _mapper.Map<RfqEnggDto>(getRfqEnggByRfqNo);

                    List<RfqEnggItemDto> rfqEnggItemsDtos = new List<RfqEnggItemDto>();

                    foreach (var itemDetails in getRfqEnggByRfqNo.RfqEnggItems)
                    {
                        RfqEnggItemDto rfqenggItemDto = _mapper.Map<RfqEnggItemDto>(itemDetails);
                        rfqEnggItemsDtos.Add(rfqenggItemDto);
                    }
                    rfqEnggDto.RfqEnggItems = rfqEnggItemsDtos;

                    serviceResponse.Data = rfqEnggDto;
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
                var getRfqById = await _rfqRepository.GetRfqById(id);

                if (getRfqById == null)
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
                    var result = _mapper.Map<RfqDto>(getRfqById);
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
                var getRfqEnggById = await _rfqenggRepository.GetRfqEnggById(id);

                if (getRfqEnggById == null)
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
                    var result = _mapper.Map<RfqEnggDto>(getRfqEnggById);
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
                var getRfqlpcostingById = await _rfqlpcostingRepository.GetRfqLPCostingById(id);

                if (getRfqlpcostingById == null)
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

                    RfqLPCostingDto rfqLpCostingDto = _mapper.Map<RfqLPCostingDto>(getRfqlpcostingById);



                    List<RfqLPCostingItemDto> rfqLpCostingItemDtos = new List<RfqLPCostingItemDto>();

                    foreach (var lpCostingItemDetail in getRfqlpcostingById.RfqLPCostingItems)
                    {
                        RfqLPCostingItemDto rfqLpCostingItemDto = _mapper.Map<RfqLPCostingItemDto>(lpCostingItemDetail);
                        rfqLpCostingItemDto.RfqLPCostingProcesses = _mapper.Map<List<RfqLPCostingProcessDto>>(rfqLpCostingItemDto.RfqLPCostingProcesses);
                        rfqLpCostingItemDto.RfqLPCostingNREConsumables = _mapper.Map<List<RfqLPCostingNREConsumableDto>>(rfqLpCostingItemDto.RfqLPCostingNREConsumables);
                        rfqLpCostingItemDto.RfqLPCostingOtherCharges = _mapper.Map<List<RfqLPCostingOtherChargesDto>>(rfqLpCostingItemDto.RfqLPCostingOtherCharges);

                        rfqLpCostingItemDtos.Add(rfqLpCostingItemDto);
                    }

                    rfqLpCostingDto.RfqLPCostingItems = rfqLpCostingItemDtos;
                    serviceResponse.Data = rfqLpCostingDto;
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
                var getRfqCSById = await _repository.GetRfqCustomerSupportById(id);

                if (getRfqCSById == null)
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

                    RfqCustomerSupportDto rfCSqDto = _mapper.Map<RfqCustomerSupportDto>(getRfqCSById);

                    List<RfqCustomerSupportItemDto> rfqItemsDtos = new List<RfqCustomerSupportItemDto>();
                    foreach (var rfqCSItemDetail in getRfqCSById.RfqCustomerSupportItems)
                    {
                        RfqCustomerSupportItemDto rfqCSItemDto = _mapper.Map<RfqCustomerSupportItemDto>(rfqCSItemDetail);
                        rfqCSItemDto.RfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliveryScheduleDto>>(rfqCSItemDetail.RfqCSDeliverySchedule);
                        rfqItemsDtos.Add(rfqCSItemDto);
                    }
                    rfCSqDto.RfqCustomerSupportItems = rfqItemsDtos;

                    serviceResponse.Data = rfCSqDto;
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

        //aravind
        //release active API
        [HttpPut]
        public async Task<IActionResult> UpdateRfqCustomerSupportRelease([FromBody] List<int> itemIds)
        {
            ServiceResponse<RfqCustomerSupportDto> serviceResponse = new ServiceResponse<RfqCustomerSupportDto>();

            try
            {
                if (itemIds is null)
                {
                    _logger.LogError("RfqCustomerSupport Itemid object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update RfqCustomerSupport Itemid object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }


                //if (getIsSourcingAvailable.isSourcingAvailable == false)
                //{

                //    var rfqEnggDetail = _mapper.Map<RfqEngg>(rfqCustomerSupportDto);

                //    var rfqDetails = rfqCustomerSupportDto.RfqCustomerSupportItems;


                //    var rfqEnggItemList = new List<RfqEnggItem>();
                //    for (int i = 0; i < rfqDetails.Count; i++)
                //    {
                //        RfqEnggItem rfqenggItemDto = _mapper.Map<RfqEnggItem>(rfqDetails[i]);
                //        rfqenggItemDto.CustomerItemNumber = rfqCustomerSupportDto.CustomerRfqNumber;
                //        rfqenggItemDto.ReleaseStatus = true;
                //        rfqEnggItemList.Add(rfqenggItemDto);
                //    }
                //    rfqEnggDetail.RfqEnggItems = rfqEnggItemList;


                //    _rfqenggRepository.CreateRfqEngg(rfqEnggDetail);
                //    _rfqenggRepository.SaveAsync();
                //    //test
                //} 

                foreach (var id in itemIds)
                {
                    if (id == null)
                    {
                        _logger.LogError($"RfqCustomerSupport with item id: {id}, hasn't been found in db.");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Update RfqCustomerSupport with item id: {id}, hasn't been found in db.";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(serviceResponse);
                    }

                    var getRfqCSItemById = await _itemRepository.GetRfqCustomerSupportItemById(id);
                    getRfqCSItemById.ReleaseStatus = true;
                    string result = await _itemRepository.ActivateRfqCustomerSupportItemById(getRfqCSItemById);
                    _logger.LogInfo(result);
                    _repository.SaveAsync();
                     
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "Rfq CustomerSupport Release Activated Successfully ";
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


        //release enggactive API
        [HttpPut]
        public async Task<IActionResult> UpdateRfqEnggItemRelease([FromBody] List<int> itemIds)
        {
            ServiceResponse<RfqEnggDto> serviceResponse = new ServiceResponse<RfqEnggDto>();

            try
            {
                if (itemIds is null)
                {
                    _logger.LogError("RfqItemid object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update RfqItemid object is null";
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

                    var getRfqEnggItemById = await _rfqenggItemRepository.GetRfqEnggItemById(id);
                    getRfqEnggItemById.ReleaseStatus = true;
                    string result = await _rfqenggItemRepository.ActivateRfqEnggItemById(getRfqEnggItemById);
                    _logger.LogInfo(result);
                    _repository.SaveAsync();
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "RfqItem  Release Activated Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRfqItem action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //aravind
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

                var createRfqCS = _mapper.Map<RfqCustomerSupport>(rfqCustomerSupportDto);

                var rfqCsData = createRfqCS.RfqNumber;

                var rfqIsCsCompleteUpdate = await _rfqRepository.RfqCsByRfqNumbers(rfqCsData);

                //rfqIsCsCompleteUpdate.IsCsComplete = true;

                var rfqCSItemDto = rfqCustomerSupportDto.RfqCustomerSupportItems;

                var rfqCustomerSupportLists = new List<RfqCustomerSupportItems>();
                for (int i = 0; i < rfqCSItemDto.Count; i++)
                {
                    RfqCustomerSupportItems rfqCSItems = _mapper.Map<RfqCustomerSupportItems>(rfqCSItemDto[i]);
                    rfqCSItems.RfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliverySchedule>>(rfqCSItemDto[i].RfqCSDeliverySchedule);
                    rfqCustomerSupportLists.Add(rfqCSItems);

                }
                createRfqCS.RfqCustomerSupportItems = rfqCustomerSupportLists;

                _repository.CreateRfqCustomerSupport(createRfqCS);
                _rfqRepository.Update(rfqIsCsCompleteUpdate);
                _repository.SaveAsync();
                var rfqNumber = createRfqCS.RfqNumber;
                var getIsSourcingAvailable = await _rfqRepository.RfqDetailsByRfqNumbers(rfqNumber);

                //if (getIsSourcingAvailable.isSourcingAvailable == false)
                //{

                //    var rfqEnggDetail = _mapper.Map<RfqEngg>(rfqCustomerSupportDto);

                //    var rfqDetails = rfqCustomerSupportDto.RfqCustomerSupportItems;


                //    var rfqEnggItemList = new List<RfqEnggItem>();
                //    for (int i = 0; i < rfqDetails.Count; i++)
                //    {
                //        RfqEnggItem rfqenggItemDto = _mapper.Map<RfqEnggItem>(rfqDetails[i]);
                //        rfqenggItemDto.CustomerItemNumber = rfqCustomerSupportDto.CustomerRfqNumber;
                //        rfqenggItemDto.ReleaseStatus = true;
                //        rfqEnggItemList.Add(rfqenggItemDto);
                //    }
                //    rfqEnggDetail.RfqEnggItems = rfqEnggItemList;


                //    _rfqenggRepository.CreateRfqEngg(rfqEnggDetail);
                //    _rfqenggRepository.SaveAsync();
                //    //test
                //}

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

                var createRfq = _mapper.Map<Rfq>(rfqPostDto);
                createRfq.RevisionNumber = 1;
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                var newcount = await _rfqRepository.GetRfqNumberAutoIncrementCount(date);
            
                if (newcount > 0)
                {
                    var number = newcount + 1;
                    string e = String.Format("{0:D4}", number);
                    createRfq.RfqNumber = days + months + years + "R" + (e);
                }
                else
                {
                    var count = 1;
                    var e = count.ToString("D4");
                    createRfq.RfqNumber = days + months + years + "R" + (e);
                }
                await _rfqRepository.CreateRfq(createRfq);
                var rfqDetails = _mapper.Map<RfqDto>(createRfq);

                _rfqRepository.SaveAsync();
                serviceResponse.Data = rfqDetails;
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
                var createRfqLPCosting = _mapper.Map<RfqLPCosting>(rfqLPCostingDtoPost);
                var rfqUpdate = createRfqLPCosting.RfqNumber;
                var updateRfqIsLpCosting = await _rfqRepository.RfqLpcostingByRfqNumbers(rfqUpdate);
                updateRfqIsLpCosting.IsLpCosting = true;
                var rfqLPCostingDto = rfqLPCostingDtoPost.RfqLPCostingItems;

                var lpCostingItemList = new List<RfqLPCostingItem>();
                if (rfqLPCostingDto != null)
                {
                    for (int i = 0; i < rfqLPCostingDto.Count; i++)
                    {
                        RfqLPCostingItem lpcostingItemListDetail = _mapper.Map<RfqLPCostingItem>(rfqLPCostingDto[i]);
                        lpcostingItemListDetail.RfqLPCostingProcesses = _mapper.Map<List<RfqLPCostingProcess>>(rfqLPCostingDto[i].RfqLPCostingProcesses);
                        lpcostingItemListDetail.RfqLPCostingNREConsumables = _mapper.Map<List<RfqLPCostingNREConsumable>>(rfqLPCostingDto[i].RfqLPCostingNREConsumables);
                        lpcostingItemListDetail.RfqLPCostingOtherCharges = _mapper.Map<List<RfqLPCostingOtherCharges>>(rfqLPCostingDto[i].RfqLPCostingOtherCharges);
                        lpCostingItemList.Add(lpcostingItemListDetail);

                    }
                }
                createRfqLPCosting.RfqLPCostingItems = lpCostingItemList;


                await _rfqlpcostingRepository.CreateRfqLPCosting(createRfqLPCosting);
                _rfqRepository.Update(updateRfqIsLpCosting);
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
                var createRfqEngg =  _mapper.Map<RfqEngg>(rfqEnggDtoPost);

                var rfqEnggData = createRfqEngg.RFQNumber;

                var rfqIsEnggCompleteUpdate = await _rfqRepository.RfqEnggByRfqNumbers(rfqEnggData);

                rfqIsEnggCompleteUpdate.IsEnggComplete = true;

                _rfqenggRepository.CreateRfqEngg(createRfqEngg);
                _rfqRepository.Update(rfqIsEnggCompleteUpdate);
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
        [HttpPut]
        public async Task<IActionResult> UpdateRfq([FromBody] RfqUpdateDto rfqUpdateDto)
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
                

                var updaterfq = _mapper.Map<Rfq>(rfqUpdateDto);
                await _rfqRepository.UpdateRfqRevNo(updaterfq);
                updaterfq.RevisionNumber += 1;
                //_logger.LogInfo(result);
                _rfqRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
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
                var getRfqEnggById = await _rfqenggRepository.GetRfqEnggById(id);
                if (getRfqEnggById is null)
                {
                    _logger.LogError($"RfqEngg with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Rfqengg with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var updateRfqEngg = _mapper.Map(rfqEnggDtoUpdate, getRfqEnggById);

                string result = await _rfqenggRepository.UpdateRfqEngg(updateRfqEngg);
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
                var getRfqLpCostingByid = await _rfqlpcostingRepository.GetRfqLPCostingById(id);
                if (getRfqLpCostingByid is null)
                {
                    _logger.LogError($"RfqLPCosting with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update RfqLPCosting with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateRfqlpCosting = _mapper.Map<RfqLPCosting>(rfqLPCostingDtoUpdate);

                var lpCostingItemtemDto = rfqLPCostingDtoUpdate.RfqLPCostingItems;

                var rfqlpcostingitemList = new List<RfqLPCostingItem>();
                if (lpCostingItemtemDto != null)
                {
                    for (int i = 0; i < lpCostingItemtemDto.Count; i++)
                    {
                        RfqLPCostingItem lpCostingItemDetail = _mapper.Map<RfqLPCostingItem>(lpCostingItemtemDto[i]);
                        lpCostingItemDetail.RfqLPCostingProcesses = _mapper.Map<List<RfqLPCostingProcess>>(lpCostingItemtemDto[i].RfqLPCostingProcesses);
                        lpCostingItemDetail.RfqLPCostingNREConsumables = _mapper.Map<List<RfqLPCostingNREConsumable>>(lpCostingItemtemDto[i].RfqLPCostingNREConsumables);
                        lpCostingItemDetail.RfqLPCostingOtherCharges = _mapper.Map<List<RfqLPCostingOtherCharges>>(lpCostingItemtemDto[i].RfqLPCostingOtherCharges);

                        rfqlpcostingitemList.Add(lpCostingItemDetail);

                    }
                }
                var updateData = _mapper.Map(rfqLPCostingDtoUpdate, updateRfqlpCosting);

                string result = await _rfqlpcostingRepository.UpdateRfqLPCosting(updateData);
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
                var GetRfqCSById = await _repository.GetRfqCustomerSupportById(id);
                if (GetRfqCSById is null)
                {
                    _logger.LogError($"RfqCustomerSupport with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update RfqCustomerSupport with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var updateRfqCS = _mapper.Map<RfqCustomerSupport>(GetRfqCSById);

                //tets


                var rfqNumber = updateRfqCS.RfqNumber;

                var rfqDetailsByRfqNumber = await _rfqRepository.RfqDetailsByRfqNumbers(rfqNumber);

                var version = 1;

                rfqDetailsByRfqNumber.RevisionNumber = rfqDetailsByRfqNumber.RevisionNumber + version;

                //test


                var rfqCSItemDto = rfqCustomerSupportUpdateDto.RfqCustomerSupportItems;

                var rfqCsItemList = new List<RfqCustomerSupportItems>();
                if (rfqCSItemDto != null)
                {
                    for (int i = 0; i < rfqCSItemDto.Count; i++)
                    {
                        RfqCustomerSupportItems rfqCSItemDetail = _mapper.Map<RfqCustomerSupportItems>(rfqCSItemDto[i]);
                        rfqCSItemDetail.RfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliverySchedule>>(rfqCSItemDto[i].RfqCSDeliverySchedule);
                        rfqCsItemList.Add(rfqCSItemDetail);

                    }
                }
                updateRfqCS.RfqCustomerSupportItems = rfqCsItemList;
                var data = _mapper.Map(rfqCustomerSupportUpdateDto, updateRfqCS);
                 
                string result = await _repository.UpdateRfqCustomerSupport(data);
                _logger.LogInfo(result);
                _rfqRepository.Update(rfqDetailsByRfqNumber);
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
                var getRfqById = await _rfqRepository.GetRfqById(id);
                if (getRfqById == null)
                {
                    _logger.LogError($"Rfq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Rfq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _rfqRepository.DeleteRfq(getRfqById);
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
                var getRfqEnggById = await _rfqenggRepository.GetRfqEnggById(id);
                if (getRfqEnggById == null)
                {
                    _logger.LogError($"Rfqengg with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Rfqengg with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _rfqenggRepository.DeleteRfqEngg(getRfqEnggById);
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
                var getRfqLPCostingById = await _rfqlpcostingRepository.GetRfqLPCostingById(id);
                if (getRfqLPCostingById == null)
                {
                    _logger.LogError($"RfqLPCosting with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete RfqLPCosting with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _rfqlpcostingRepository.DeleteRfqLPCosting(getRfqLPCostingById);
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
                var getRfqCSById = await _repository.GetRfqCustomerSupportById(id);
                if (getRfqCSById == null)
                {
                    _logger.LogError($"RfqCustomerSupport with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete RfqCustomerSupport with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteRfqCustomerSupport(getRfqCSById);
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

        //Get BulkReleaseData By RfqNumber
        [HttpGet("{RfqNumber}")]
        public async Task<IActionResult> GetRfqReleaseLpByRfqNumber(string RfqNumber)
        {
            ServiceResponse<IEnumerable<ReleaseLpDto>> serviceResponse = new ServiceResponse<IEnumerable<ReleaseLpDto>>();
            try
            {
                var getRfqReleaseLpByRfqNumber = await _releaseLpRepository.GetRfqReleaseLpByRfqNumber(RfqNumber);
                var result = _mapper.Map<IEnumerable<ReleaseLpDto>>(getRfqReleaseLpByRfqNumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BulkRelease Data";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetRfqReleaseLpByRfqNumber action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }




        [HttpPost]
        public async Task<IActionResult> BulkRelease([FromBody] List<ReleaseLpDtoPost> releaseLpDtoPosts)
        {
            ServiceResponse<ReleaseLpDtoPost> serviceResponse = new ServiceResponse<ReleaseLpDtoPost>();

            try
            {
                if (releaseLpDtoPosts == null)
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
                var releasseLpListEntity = _mapper.Map<List<ReleaseLp>>(releaseLpDtoPosts);

                var rfqNumber = releasseLpListEntity[0].RfqNumber;

                
                foreach (var releaseLpdetails in releasseLpListEntity)
                {

                    _releaseLpRepository.BulkRelease(releaseLpdetails);
 
                }
                
                _releaseLpRepository.SaveAsync();

                var lpReleases = await _rfqRepository.RfqLpCostingReleaseByRfqNumbers(rfqNumber);
                lpReleases.IsLpCostingRelease = true;
                _rfqRepository.Update(lpReleases);
                _rfqRepository.SaveAsync();

                //create data to itempricelist table

                var itemPriceLists = _mapper.Map<List<ItemPriceList>>(releaseLpDtoPosts);
                //await _itemPriceListRepository.CreateFromReleaseLp(bulklists);

                foreach (var item in itemPriceLists)
                {
                    //item.ReleaseLpId = need to work on this storing the LpId.
                    await _itemPriceListRepository.CreateFromReleaseLp(item);
                    
                }

                _itemPriceListRepository.SaveAsync();
                //var createRfq = _mapper.Map<Rfq>(rfqPostDto);
                //aravind

                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Released Bulkdata";
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

        //Unrelease active API
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfqEnggItemUnRelease(int id)
        {
            ServiceResponse<RfqEnggDto> serviceResponse = new ServiceResponse<RfqEnggDto>();

            try
            {
                if (id == null)
                {
                    _logger.LogError($"RfqEnngItem with item id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update RfqEnggItem hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var rfqEnggItem = await _rfqenggItemRepository.GetRfqEnggItemById(id);
                rfqEnggItem.ReleaseStatus = false;
                string result = await _rfqenggItemRepository.DeactivateRfqEnggItemById(rfqEnggItem);
                _logger.LogInfo(result);
                _repository.SaveAsync();


                serviceResponse.Data = null;
                serviceResponse.Message = "RfqEnggItem  UnReleased Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRfqEnggItemUnRelease action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //Unrelease active API
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfqRfqCustomerSupportItemUnRelease(int id)
        {
            ServiceResponse<RfqCustomerSupportDto> serviceResponse = new ServiceResponse<RfqCustomerSupportDto>();

            try
            {
                if (id == null)
                {
                    _logger.LogError($"RfqCustomerSupportItem with item id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update RfqCustomerSupportItem hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var getRfqCSItemById = await _itemRepository.GetRfqCustomerSupportItemById(id);
                getRfqCSItemById.ReleaseStatus = false;
                string result = await _itemRepository.DeactivateRfqCustomerSupportItemById(getRfqCSItemById);
                _logger.LogInfo(result);
                _repository.SaveAsync();


                serviceResponse.Data = null;
                serviceResponse.Message = "RfqCustomerSupportItem  UnReleased Successfully ";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRfqCustomerSupportItemUnRelease action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET: api/<RfqCustomGroupController>
        [HttpGet]
        public async Task<IActionResult> GetAllRfqCustomGroup([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<RfqCustomGroupDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqCustomGroupDto>>();
            try
            {
                var getAllRfqCustomGroup = await _rfqCustomGroupRepository.GetAllRfqCustomGroup(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllRfqCustomGroup.TotalCount,
                    getAllRfqCustomGroup.PageSize,
                    getAllRfqCustomGroup.CurrentPage,
                    getAllRfqCustomGroup.HasNext,
                    getAllRfqCustomGroup.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all BomGroup");
                var rfqCustomGroupEntity = _mapper.Map<IEnumerable<RfqCustomGroupDto>>(getAllRfqCustomGroup);
                serviceResponse.Data = rfqCustomGroupEntity;
                serviceResponse.Message = "Returned all RfqCustomGroup";
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

        // GET: api/<RfqCustomGroupController>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRfqCustomGroupById(int id)
        {
            ServiceResponse<RfqCustomGroupDto> serviceResponse = new ServiceResponse<RfqCustomGroupDto>();

            try
            {
                var getRfqCustomGroupById = await _rfqCustomGroupRepository.GetRfqCustomGroupById(id);
                if (getRfqCustomGroupById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqCustomGroup hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqCustomGroup with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var rfqCustomGroupEntity = _mapper.Map<RfqCustomGroupDto>(getRfqCustomGroupById);
                    serviceResponse.Data = rfqCustomGroupEntity;
                    serviceResponse.Message = "Returned RfqCustomGroup Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRfqCustomGroupById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCustomGroupList()
        {
            ServiceResponse<IEnumerable<ListOfCustomGroupDto>> serviceResponse = new ServiceResponse<IEnumerable<ListOfCustomGroupDto>>();
            try
            {
                var getAllCustomGroupList = await _rfqCustomGroupRepository.GetAllCustomGroupList();
                var result = _mapper.Map<IEnumerable<ListOfCustomGroupDto>>(getAllCustomGroupList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CustomGroupList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllCustomGroupList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // POST: api/<RfqCustomGroupController>
        [HttpPost]
        public IActionResult CreateRfqCustomGroup([FromBody] List<RfqCustomGroupPostDto> rfqCustomGroupPostDto)
        {
            ServiceResponse<RfqCustomGroupPostDto> serviceResponse = new ServiceResponse<RfqCustomGroupPostDto>();

            try
            {
                if (rfqCustomGroupPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RfqCustomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("RfqCustomGroup object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid RfqCustomGroup object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid RfqCustomGroup object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var cteateRfqCustomGroup = _mapper.Map<List<RfqCustomGroup>>(rfqCustomGroupPostDto);

                foreach (var customGroupdetails in cteateRfqCustomGroup)
                {

                    _rfqCustomGroupRepository.CreateRfqCustomGroup(customGroupdetails);

                }

                _rfqCustomGroupRepository.SaveAsync();
                serviceResponse.Message = "RfqCustomGroup Successfully Created";
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
                _logger.LogError($"Something went wrong inside CreateRfqCustomGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        // PUT: api/<RfqCustomGroupController>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfqCustomGroup(int id, [FromBody] RfqCustomGroupUpdateDto rfqCustomGroupUpdateDto)
        {
            ServiceResponse<RfqCustomGroupUpdateDto> serviceResponse = new ServiceResponse<RfqCustomGroupUpdateDto>();

            try
            {
                if (rfqCustomGroupUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update RfqCustomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update RfqCustomGroup object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update RfqCustomGroup object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update RfqCustomGroup object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var getRfqCustomGroupById = await _rfqCustomGroupRepository.GetRfqCustomGroupById(id);
                if (getRfqCustomGroupById is null)
                {
                    _logger.LogError($"Update RfqCustomGroup with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " UpdateRfqCustomGroup hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(rfqCustomGroupUpdateDto, getRfqCustomGroupById);
                string result = await _rfqCustomGroupRepository.UpdateRfqCustomGroup(getRfqCustomGroupById);
                _logger.LogInfo(result);
                _rfqCustomGroupRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RfqCustomGroup Updated Successfully";
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
                _logger.LogError($"Something went wrong inside UpdateRfqCustomGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE: api/<RfqCustomGroupController>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRfqCustomGroup(int id)
        {
            ServiceResponse<RfqCustomGroupDto> serviceResponse = new ServiceResponse<RfqCustomGroupDto>();

            try
            {
                var getRfqCustomGroupById = await _rfqCustomGroupRepository.GetRfqCustomGroupById(id);
                if (getRfqCustomGroupById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete RfqCustomGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete RfqCustomGroup with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _rfqCustomGroupRepository.DeleteRfqCustomGroup(getRfqCustomGroupById);
                _logger.LogInfo(result);
                _rfqCustomGroupRepository.SaveAsync();
                serviceResponse.Message = "RfqCustomGroup Deleted Successfully";
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
                _logger.LogError($"Something went wrong inside DeleteRfqCustomGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // GET: api/<RfqCustomFieldController>
        [HttpGet]
        public async Task<IActionResult> GetAllRfqCustomField([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<RfqCustomFieldDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqCustomFieldDto>>();
            try
            {
                var getAllRfqCustomField = await _rfqCustomFieldRepository.GetAllRfqCustomField(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllRfqCustomField.TotalCount,
                    getAllRfqCustomField.PageSize,
                    getAllRfqCustomField.CurrentPage,
                    getAllRfqCustomField.HasNext,
                    getAllRfqCustomField.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all RfqCustomField");
                var rfqCustomFieldEntity = _mapper.Map<IEnumerable<RfqCustomFieldDto>>(getAllRfqCustomField);
                serviceResponse.Data = rfqCustomFieldEntity;
                serviceResponse.Message = "Returned all RfqCustomField";
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


        // GET: api/<RfqCustomFieldController>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRfqCustomFieldById(int id)
                {
                    ServiceResponse<RfqCustomFieldDto> serviceResponse = new ServiceResponse<RfqCustomFieldDto>();

                    try
                    {
                        var getRfqCustomFieldById = await _rfqCustomFieldRepository.GetRfqCustomFieldById(id);
                        if (getRfqCustomFieldById == null)
                        {
                            serviceResponse.Data = null;
                            serviceResponse.Message = $"RfqCustomField hasn't been found in db.";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.NotFound;
                            _logger.LogError($"RfqCustomField with id: {id}, hasn't been found in db.");
                            return NotFound(serviceResponse);
                        }
                        else
                        {
                            _logger.LogInfo($"Returned RfqCustomField with id: {id}");
                            var rfqCustomFieldEntity = _mapper.Map<RfqCustomFieldDto>(getRfqCustomFieldById);
                            serviceResponse.Data = rfqCustomFieldEntity;
                            serviceResponse.Message = "Returned RfqCustomField Successfully";
                            serviceResponse.Success = true;
                            serviceResponse.StatusCode = HttpStatusCode.OK;
                            return Ok(serviceResponse);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Something went wrong inside GetRfqCustomFieldById action: {ex.Message}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Something went wrong. Please try again!";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                }
        [HttpGet("{CustomGroup}")]
        public async Task<IActionResult> GetRfqCustomFieldByCustomGroup(string CustomGroup)
        {
            ServiceResponse<IEnumerable<RfqCustomFieldDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqCustomFieldDto>>();

            try
            {
                var getCustomFieldList = await _rfqCustomFieldRepository.GetRfqCustomFieldByCustomGroup(CustomGroup);
                if (getCustomFieldList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CustomFieldList with id: {CustomGroup}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"CustomFieldList with id: {CustomGroup}, hasn't been found.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned CustomFieldList with id: {CustomGroup}");
                    var result = _mapper.Map<IEnumerable<RfqCustomFieldDto>>(getCustomFieldList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CustomFieldList with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRfqCustomFieldByCustomGroup action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST: api/<RfqCustomFieldController>
        [HttpPost]
        public IActionResult CreateRfqCustomField([FromBody] List<RfqCustomFieldDtoPost> rfqCustomFieldDtoPost)
                {
                    ServiceResponse<RfqCustomFieldDtoPost> serviceResponse = new ServiceResponse<RfqCustomFieldDtoPost>();

                    try
                    {
                        if (rfqCustomFieldDtoPost is null)
                        {
                            serviceResponse.Data = null;
                            serviceResponse.Message = "RfqCustomField object sent from client is null";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            _logger.LogError("RfqCustomField object sent from client is null.");
                            return BadRequest(serviceResponse);
                        }
                        if (!ModelState.IsValid)
                        {
                            serviceResponse.Data = null;
                            serviceResponse.Message = "Invalid RfqCustomField object sent from client";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            _logger.LogError("Invalid RfqCustomField object sent from client.");
                            return BadRequest(serviceResponse);
                        }
                        var createRfqCustomField = _mapper.Map<List<RfqCustomField>>(rfqCustomFieldDtoPost);


                foreach (var customFielddetails in createRfqCustomField)
                {

                    _rfqCustomFieldRepository.CreateRfqCustomField(customFielddetails);

                }
               // _rfqCustomFieldRepository.CreateRfqCustomField(createRfqCustomField);
                        _rfqCustomFieldRepository.SaveAsync();
                        serviceResponse.Message = "RfqCustomField Successfully Created";
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
                        _logger.LogError($"Something went wrong inside CreateRfqCustomField action: {ex.Message}");
                        return StatusCode(500, serviceResponse);
                    }
                }

                // PUT: api/<RfqCustomFieldController>
         [HttpPut("{id}")]
         public async Task<IActionResult> UpdateRfqCustomField(int id, [FromBody] RfqCustomFieldDtoUpdate rfqCustomFieldDtoUpdate)
                {
                    ServiceResponse<RfqCustomFieldDtoUpdate> serviceResponse = new ServiceResponse<RfqCustomFieldDtoUpdate>();

                    try
                    {
                        if (rfqCustomFieldDtoUpdate is null)
                        {
                            serviceResponse.Data = null;
                            serviceResponse.Message = "update RfqCustomField object sent from client is null";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            _logger.LogError("update RfqCustomField object sent from client is null.");
                            return BadRequest(serviceResponse);
                        }
                        if (!ModelState.IsValid)
                        {
                            serviceResponse.Data = null;
                            serviceResponse.Message = "Invalid Update RfqCustomField object sent from client";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            _logger.LogError("Invalid Update RfqCustomField object sent from client.");
                            return BadRequest(serviceResponse);
                        }
                        var getRfqCustomFieldById = await _rfqCustomFieldRepository.GetRfqCustomFieldById(id);
                        if (getRfqCustomFieldById is null)
                        {
                            _logger.LogError($"Update RfqCustomField with id: {id}, hasn't been found in db.");
                            serviceResponse.Data = null;
                            serviceResponse.Message = " UpdateRfqCustomField hasn't been found in db.";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.NotFound;
                            return NotFound(serviceResponse);
                        }
                        _mapper.Map(rfqCustomFieldDtoUpdate, getRfqCustomFieldById);
                        string result = await _rfqCustomFieldRepository.UpdateRfqCustomField(getRfqCustomFieldById);
                        _logger.LogInfo(result);
                        _rfqCustomFieldRepository.SaveAsync();
                        serviceResponse.Data = null;
                        serviceResponse.Message = "RfqCustomField Updated Successfully";
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
                        _logger.LogError($"Something went wrong inside UpdateRfqCustomField action: {ex.Message}");
                        return StatusCode(500, serviceResponse);
                    }
                }

                // DELETE: api/<RfqCustomFieldController>
          [HttpDelete("{id}")]
          public async Task<IActionResult> DeleteRfqCustomField(int id)
                {
                    ServiceResponse<RfqCustomFieldDto> serviceResponse = new ServiceResponse<RfqCustomFieldDto>();

                    try
                    {
                        var getRfqCustomFieldById = await _rfqCustomFieldRepository.GetRfqCustomFieldById(id);
                        if (getRfqCustomFieldById == null)
                        {
                            serviceResponse.Data = null;
                            serviceResponse.Message = "Delete RfqCustomField object sent from client is null";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            _logger.LogError($"Delete RfqCustomField with id: {id}, hasn't been found in db.");
                            return BadRequest(serviceResponse);
                        }
                        string result = await _rfqCustomFieldRepository.DeleteRfqCustomField(getRfqCustomFieldById);
                        _logger.LogInfo(result);
                        _rfqCustomFieldRepository.SaveAsync();
                        serviceResponse.Message = "RfqCustomField Deleted Successfully";
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
                        _logger.LogError($"Something went wrong inside DeleteRfqCustomField action: {ex.Message}");
                        return StatusCode(500, serviceResponse);
                    }
                }

    }

}

