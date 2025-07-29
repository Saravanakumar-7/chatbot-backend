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
using System.Linq;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
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
        private IDocumentUploadRepository _documentUploadRepository;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        private readonly IHttpClientFactory _clientFactory;
        public RfqController(IRfqCustomGroupRepository rfqCustomGroupRepository, IHttpClientFactory clientFactory, IDocumentUploadRepository documentUploadRepository,IItemPriceListRepository itemPriceListRepository, IRfqCustomFieldRepository rfqCustomFieldRepository
            , IRfqEnggItemRepository rfqenggItemRepository, IReleaseLpRepository releaseLpRepository, IHttpContextAccessor httpContextAccessor, 
            IRfqCustomerSupportRepository repository, IRfqCustomerSupportItemRepository rfqCustomerSupportItemRepository,
            IRfqRepository rfqRepository, IRfqLPCostingRepository rfqLPCostingRepository, IRfqEnggRepository rfqEnggRepository,
            ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _repository = repository;
            _documentUploadRepository = documentUploadRepository;
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
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        //rfq getall 
        [HttpGet]
        public async Task<IActionResult> GetAllRfq([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<RfqDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqDto>>();

            try
            {
                var getAllRfq = await _rfqRepository.GetAllRfq(pagingParameter, searchParammes);

                for(int i = 0; i < getAllRfq.Count(); i++)
                {
                    var rfq = getAllRfq[i].RfqNumber;
                    var revNO = getAllRfq[i].RevisionNumber;

                    var rfqCsCount = await _itemRepository.GetRfqCustomerSupportItemByRfqNumber(rfq,revNO);
                    //if (getAllRfq[i].isSourcingAvailable == true)
                    //{

                        var isFullyReleased = await _itemRepository.IsFullyReleasedRfqEngg(rfq, revNO);
                        var isNotYetReleased = await _itemRepository.IsNotYetReleasedRfqEngg(rfq, revNO);
                       

                        if (isFullyReleased)
                        {
                            getAllRfq[i].IsEnggRelease = CsRelease.FullyRelease;
                        }
                        else if (isNotYetReleased)
                        {
                            getAllRfq[i].IsEnggRelease = CsRelease.NotYetReleased;
                        }
                        else
                        {
                            getAllRfq[i].IsEnggRelease = CsRelease.PartiallyRelease;
                        }
                        var rfqEnggCount = await _rfqenggItemRepository.GetRfqEnggCountByRfqNumber(rfq, revNO);

                        if (rfqEnggCount.Count() != 0)
                        {
                            getAllRfq[i].EnggComplete = EnggStatus.EnggCompleted;
                        }
                        else
                        {
                            getAllRfq[i].EnggComplete = EnggStatus.EnggNotYetCompleted;
                        }

                        // Use the 'status' variable as needed


                        //var rfqEnggRelese = await _rfqenggItemRepository.GetRfqEnggRelesedDetailsByRfqNumber(rfq);
                        //var rfqEnggUnRelesedCount = rfqEnggCount.Count() - rfqEnggRelese.Count();
                        //if (rfqEnggRelese.Count() == 0)
                        //{
                        //    getAllRfq[i].IsEnggRelease = CsRelease.NotYetReleased;
                        //}
                        //if (rfqEnggUnRelesedCount == 0)
                        //{
                        //    getAllRfq[i].IsEnggRelease = CsRelease.FullyRelease;
                        //}
                        //if (rfqEnggUnRelesedCount != 0 && rfqEnggRelese.Count() != 0)
                        //{
                        //    getAllRfq[i].IsEnggRelease = CsRelease.PartiallyRelease;
                        //}
                        //if (rfqEnggCount.Count() != 0)
                        //{
                        //    getAllRfq[i].EnggComplete = EnggStatus.EnggCompleted;
                        //}
                        //else
                        //{
                        //    getAllRfq[i].EnggComplete = EnggStatus.EnggNotYetCompleted;
                        //}
                    //}
                    
                    var isFullyReleasedCs = await _itemRepository.IsFullyReleasedRfqCs(rfq, revNO);
                    var isNotYetReleasedCs = await _itemRepository.IsNotYetReleasedRfqCs(rfq, revNO);
               

                    if (isFullyReleasedCs)
                    {
                        getAllRfq[i].IsCsRelease = CsRelease.FullyRelease;
                    }
                    else if (isNotYetReleasedCs)
                    {
                        getAllRfq[i].IsCsRelease = CsRelease.NotYetReleased;
                    }
                    else
                    {
                        getAllRfq[i].IsCsRelease = CsRelease.PartiallyRelease;
                    }


                    //var rfqCsRelesed = await _itemRepository.GetRfqCustomerSupportRelesedDetailsByRfqNumber(rfq);

                    //var rfqCsUnRelesedCount = rfqCsCount.Count() - rfqCsRelesed.Count();
                    //if(rfqCsRelesed.Count() == 0 )
                    //{
                    //    getAllRfq[i].IsCsRelease = CsRelease.NotYetReleased;
                    //}
                    //if(rfqCsUnRelesedCount == 0 && rfqCsCount.Count() != 0)
                    ////if (rfqCsUnRelesedCount == 0)
                    //{
                    //    getAllRfq[i].IsCsRelease = CsRelease.FullyRelease;
                    //}
                    //if(rfqCsUnRelesedCount != 0 && rfqCsRelesed.Count() != 0)
                    ////if (rfqCsUnRelesedCount != 0)
                    //{
                    //    getAllRfq[i].IsCsRelease = CsRelease.PartiallyRelease;
                    //}
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
                _logger.LogError($"Error Occured in GetAllRfq API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllRfq API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllRfqForKeus([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<RfqDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqDto>>();

            try
            {
                var getAllRfq = await _rfqRepository.GetAllRfqs(pagingParameter, searchParammes);

                for (int i = 0; i < getAllRfq.Count(); i++)
                {
                    var rfq = getAllRfq[i].RfqNumber;
                    var revNO = getAllRfq[i].RevisionNumber;

                    var rfqCsCount = await _itemRepository.GetRfqCustomerSupportItemByRfqNumber(rfq, revNO);
                    //if (getAllRfq[i].isSourcingAvailable == true)
                    //{

                    var isFullyReleased = await _itemRepository.IsFullyReleasedRfqEngg(rfq, revNO);
                    var isNotYetReleased = await _itemRepository.IsNotYetReleasedRfqEngg(rfq, revNO);


                    if (isFullyReleased)
                    {
                        getAllRfq[i].IsEnggRelease = CsRelease.FullyRelease;
                    }
                    else if (isNotYetReleased)
                    {
                        getAllRfq[i].IsEnggRelease = CsRelease.NotYetReleased;
                    }
                    else
                    {
                        getAllRfq[i].IsEnggRelease = CsRelease.PartiallyRelease;
                    }
                    var rfqEnggCount = await _rfqenggItemRepository.GetRfqEnggCountByRfqNumber(rfq, revNO);

                    if (rfqEnggCount.Count() != 0)
                    {
                        getAllRfq[i].EnggComplete = EnggStatus.EnggCompleted;
                    }
                    else
                    {
                        getAllRfq[i].EnggComplete = EnggStatus.EnggNotYetCompleted;
                    }

                    // Use the 'status' variable as needed


                    //var rfqEnggRelese = await _rfqenggItemRepository.GetRfqEnggRelesedDetailsByRfqNumber(rfq);
                    //var rfqEnggUnRelesedCount = rfqEnggCount.Count() - rfqEnggRelese.Count();
                    //if (rfqEnggRelese.Count() == 0)
                    //{
                    //    getAllRfq[i].IsEnggRelease = CsRelease.NotYetReleased;
                    //}
                    //if (rfqEnggUnRelesedCount == 0)
                    //{
                    //    getAllRfq[i].IsEnggRelease = CsRelease.FullyRelease;
                    //}
                    //if (rfqEnggUnRelesedCount != 0 && rfqEnggRelese.Count() != 0)
                    //{
                    //    getAllRfq[i].IsEnggRelease = CsRelease.PartiallyRelease;
                    //}
                    //if (rfqEnggCount.Count() != 0)
                    //{
                    //    getAllRfq[i].EnggComplete = EnggStatus.EnggCompleted;
                    //}
                    //else
                    //{
                    //    getAllRfq[i].EnggComplete = EnggStatus.EnggNotYetCompleted;
                    //}
                    //}

                    var isFullyReleasedCs = await _itemRepository.IsFullyReleasedRfqCs(rfq, revNO);
                    var isNotYetReleasedCs = await _itemRepository.IsNotYetReleasedRfqCs(rfq, revNO);


                    if (isFullyReleasedCs)
                    {
                        getAllRfq[i].IsCsRelease = CsRelease.FullyRelease;
                    }
                    else if (isNotYetReleasedCs)
                    {
                        getAllRfq[i].IsCsRelease = CsRelease.NotYetReleased;
                    }
                    else
                    {
                        getAllRfq[i].IsCsRelease = CsRelease.PartiallyRelease;
                    }


                    //var rfqCsRelesed = await _itemRepository.GetRfqCustomerSupportRelesedDetailsByRfqNumber(rfq);

                    //var rfqCsUnRelesedCount = rfqCsCount.Count() - rfqCsRelesed.Count();
                    //if(rfqCsRelesed.Count() == 0 )
                    //{
                    //    getAllRfq[i].IsCsRelease = CsRelease.NotYetReleased;
                    //}
                    //if(rfqCsUnRelesedCount == 0 && rfqCsCount.Count() != 0)
                    ////if (rfqCsUnRelesedCount == 0)
                    //{
                    //    getAllRfq[i].IsCsRelease = CsRelease.FullyRelease;
                    //}
                    //if(rfqCsUnRelesedCount != 0 && rfqCsRelesed.Count() != 0)
                    ////if (rfqCsUnRelesedCount != 0)
                    //{
                    //    getAllRfq[i].IsCsRelease = CsRelease.PartiallyRelease;
                    //}
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
                _logger.LogError($"Error Occured in GetAllRfqForKeus API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllRfqForKeus API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        //passing rfqnumber and revision number to get the data

        [HttpGet]
        public async Task<IActionResult> GetRfqDeatailsByRfqNoAndRevNo(string rfqNumber, int revisionNumber)
        {
            ServiceResponse<RfqDto> serviceResponse = new ServiceResponse<RfqDto>();
            try
            {
                var rfqDetail = await _rfqRepository.GetRfqDeatailsByRfqNoAndRevNo(rfqNumber, revisionNumber);

                if (rfqDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Rfq  hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Rfq with id: {rfqNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {rfqNumber}");
                    RfqDto rfqDto = _mapper.Map<RfqDto>(rfqDetail);
                    serviceResponse.Data = rfqDto;
                    serviceResponse.Message = "Returned RfqDetailsByRFQNoAndRevNo Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqDeatailsByRfqNoAndRevNo API for the following rfqNumber:{rfqNumber} and revisionNumber : {revisionNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqDeatailsByRfqNoAndRevNo API for the following rfqNumber:{rfqNumber} and revisionNumber : {revisionNumber} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllRfqCustomerSupport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllRfqCustomerSupport API : \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllActiveRfqNumberList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveRfqNumberList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRfqNumberList()
        {
            ServiceResponse<IEnumerable<RfqNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqNumberListDto>>();
            try
            {
                var rfqNumberList = await _rfqRepository.GetAllRfqNumberList();

                var result = _mapper.Map<IEnumerable<RfqNumberListDto>>(rfqNumberList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned RfqNumberList Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllRfqNumberList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllRfqNumberList API : \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllRfqLPCosting API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllRfqLPCosting API : \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllRfqEngg API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllRfqEngg API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        //get list of child item under fg bom

        //[HttpGet("{RfqNumber}")]
        //public async Task<IActionResult> GetFGBomChildItemDetails(string RfqNumber)
        //{
        //    ServiceResponse<IEnumerable<RfqSourcingItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqSourcingItemsDto>>();
        //    try
        //    {
        //        var getAllRfqEnggRelesedList = await _rfqenggItemRepository.GetRfqEnggRelesedDetailsByRfqNumber(RfqNumber);
        //        List<string?>? itemDetails = getAllRfqEnggRelesedList?.Select(x => x.ItemNumber).ToList();
        //        List<EnggBomFGItemNumberWithQtyDto>? itemsRoutingDetailsDynamic = new List<EnggBomFGItemNumberWithQtyDto>();

        //        if (itemDetails != null)
        //        {
        //            var itemDetailsString = JsonConvert.SerializeObject(itemDetails);
        //            var content = new StringContent(itemDetailsString, Encoding.UTF8, "application/json");
        //            var inventoryObjectResult = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "GetFGBomItemsChildDetails"), content);
        //            var itemsRoutingDetailsJsonString = await inventoryObjectResult.Content.ReadAsStringAsync();
        //            dynamic itemsRoutingDetailsJson = JsonConvert.DeserializeObject(itemsRoutingDetailsJsonString);
        //            var data = itemsRoutingDetailsJson.data;
        //            itemsRoutingDetailsDynamic = data.ToObject<List<EnggBomFGItemNumberWithQtyDto>>();
        //        }
        //        var result = _mapper.Map<IEnumerable<RfqSourcingItemsDto>>(itemsRoutingDetailsDynamic);
        //        serviceResponse.Data = result;
        //        serviceResponse.Message = "Returned all getAllRfqEnggRelesedList";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong inside getAllRfqEnggRelesedList action";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}




        //pass rfq id and get customer support data

        [HttpGet("{RfqNumber}")]
        public async Task<IActionResult> GetFGBomChildItemDetails(string RfqNumber)
        {
            ServiceResponse<IEnumerable<RfqSourcingItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqSourcingItemsDto>>();
            try
            {
                var getAllRfqEnggRelesedList = await _rfqenggItemRepository.GetRfqEnggRelesedDetailsByRfqNumber(RfqNumber);
                List<RfqEnggItem> rfqEnggSourcingDtos = _mapper.Map<List<RfqEnggItem>>(getAllRfqEnggRelesedList);
                List<EnggBomFGItemNumberWithQtyDto>? itemsRoutingDetailsDynamic = new List<EnggBomFGItemNumberWithQtyDto>();
                if (rfqEnggSourcingDtos != null)
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var itemDetailsString = JsonConvert.SerializeObject(rfqEnggSourcingDtos);
                    var content = new StringContent(itemDetailsString, Encoding.UTF8, "application/json");
                    //var inventoryObjectResult = await _httpClient.PostAsync(string.Concat(_config["EngineeringBomAPI"], "GetFGBomItemsChildDetails"), content);
                    var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["EngineeringBomAPI"],"GetFGBomItemsChildDetails"))
                    {
                        Content= content
                    };
                    request.Headers.Add("Authorization", token);

                    var inventoryObjectResult = await client.SendAsync(request);
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
                _logger.LogError($"Error Occured in GetFGBomChildItemDetails API for RfqNumber:{RfqNumber} : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetFGBomChildItemDetails API for RfqNumber:{RfqNumber} : \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

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
                    serviceResponse.Message = $"RfqCustomerSupport with this RfqNumber hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqCustomerSupportByRfqNumber with id: {RfqNumber}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned RfqCustomerSupportByRfqNumber with id: {RfqNumber}");

                    RfqCustomerSupportDto rfqCSDto = _mapper.Map<RfqCustomerSupportDto>(getRfqCSByRfqNO);
                    var rfqCSRevNo = Convert.ToInt32(rfqCSDto.RevisionNumber);
                    var rfqDetails = await _rfqRepository.GetRfqDeatailsByRfqNoAndRevNo(rfqCSDto.RFQNumber, rfqCSRevNo);
                    rfqCSDto.SalesPerson = rfqDetails.SalesPerson;
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
                _logger.LogError($"Error Occured in RfqCustomerSupportByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in RfqCustomerSupportByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }


        }
        //get cs and forecast list of project number
        //charan
        [HttpGet]
        public async Task<IActionResult> GetRfqCsandForecastCsProjectNumberList()
        {
            ServiceResponse<IEnumerable<string>> serviceResponse = new ServiceResponse<IEnumerable<string>>();
            try
            {
                var rfqCsandForecastCsByItemNo = await _itemRepository.GetRfqCsandForecastCsProjectNumberList(); 
                serviceResponse.Data = rfqCsandForecastCsByItemNo;
                serviceResponse.Message = "Returned all RfqCsandForecastCsDetailList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqCsandForecastCsProjectNumberList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqCsandForecastCsProjectNumberList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRfqandForecastNumberList()
        {
            ServiceResponse<IEnumerable<string>> serviceResponse = new ServiceResponse<IEnumerable<string>>();
            try
            {
                var rfqandForecastNoDetails = await _itemRepository.GetAllRfqandForecastNumberList();
                _logger.LogInfo("Returned all RfqandForecastNoList");
                serviceResponse.Data = rfqandForecastNoDetails;
                serviceResponse.Message = "Returned all RfqandForecastNoList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllRfqandForecastNumberList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllRfqandForecastNumberList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        //GetRfqCsandForecastCsDetailListByItemNumber 
        [HttpGet]
        public async Task<IActionResult> GetRfqCsandForecastCsDetailListByItemNumber(string itemNumber)
        {
            ServiceResponse<IEnumerable<string>> serviceResponse = new ServiceResponse<IEnumerable<string>>();
            try
            {
                var rfqCsandForecastCsByItemNo = await _itemRepository.GetRfqCsandForecastCsDetailListByItemNumber(itemNumber);
                serviceResponse.Data = rfqCsandForecastCsByItemNo;
                serviceResponse.Message = "Returned all RfqCsandForecastCsDetailList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqCsandForecastCsDetailListByItemNumber API for the following itemNumber:{itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqCsandForecastCsDetailListByItemNumber API for the following itemNumber:{itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRfqEnggandForecastCsDetailListByItemNumber(string itemNumber)
        {
            ServiceResponse<IEnumerable<string>> serviceResponse = new ServiceResponse<IEnumerable<string>>();
            try
            {
                var rfqEnggandForecastCsByItemNo = await _itemRepository.GetRfqEnggandForecastCsDetailListByItemNumber(itemNumber);
                //var result = _mapper.Map<IEnumerable<RfqCsandForecastCsprojectNumber>>(rfqCsandForecastCsByItemNo);
                serviceResponse.Data = rfqEnggandForecastCsByItemNo;
                serviceResponse.Message = "Returned all RfqEnggandForecastCsDetailList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqEnggandForecastCsDetailListByItemNumber API for the following itemNumber:{itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqEnggandForecastCsDetailListByItemNumber API for the following itemNumber:{itemNumber} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //getrfqandforecastnumber
        [HttpGet]
        public async Task<IActionResult> GetRfqEnggandForecastCsProjectNumberList()
        {
            ServiceResponse<IEnumerable<string>> serviceResponse = new ServiceResponse<IEnumerable<string>>();
            try
            {
                var rfqEnggandForecastCsByItemNo = await _itemRepository.GetRfqEnggandForecastCsProjectList();
                //var result = _mapper.Map<IEnumerable<RfqCsandForecastCsprojectNumber>>(rfqCsandForecastCsByItemNo);
                serviceResponse.Data = rfqEnggandForecastCsByItemNo;
                serviceResponse.Message = "Returned all RfqEnggandForecastCsProjectList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqEnggandForecastCsProjectNumberList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqEnggandForecastCsProjectNumberList API : \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllActiveRfqCustomerSupportItemsByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveRfqCustomerSupportItemsByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message} ";
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
                _logger.LogError($"Error Occured in GetAllActiveRfqNumberListByCustomerId API for the following CustomerId:{CustomerId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveRfqNumberListByCustomerId API for the following CustomerId:{CustomerId} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllODORfqNumberListByCustomerId(string customerId)
        {
            ServiceResponse<IEnumerable<ODORfqNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<ODORfqNumberListDto>>();
            try
            {
                var oDORfqNumberListByCustomerId = await _rfqRepository.GetAllODORfqNumberListByCustomerId(customerId);

                if (oDORfqNumberListByCustomerId.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqNumberList with  with CustomerId: {customerId}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqNumberList with CustomerId: {customerId}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }

                var result = _mapper.Map<IEnumerable<ODORfqNumberListDto>>(oDORfqNumberListByCustomerId);
                _logger.LogInfo("Returned all ODORfqNumberList By CustomerId");
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ODORfqNumberList By CustomerId";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllODORfqNumberListByCustomerId API for the following CustomerId:{customerId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllODORfqNumberListByCustomerId API for the following CustomerId:{customerId} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllActiveRfqEnggItemByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveRfqEnggItemByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message}";
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
                var rfqEnggDetails = await _rfqenggRepository.GetRfqReleasedEnggByRfqNumberRevNo(rfqNumber);

                List<string?>? itemDetails = rfqEnggDetails?.RfqEnggItems?.Select(x => x.ItemNumber).ToList();
                
                List<ItemMasterRoutingListDto>? itemsRoutingDetailsDynamic = new List<ItemMasterRoutingListDto>();
                if (itemDetails != null)
                {
                    var itemDetailsString = JsonConvert.SerializeObject(itemDetails);
                    var content = new StringContent(itemDetailsString, Encoding.UTF8, "application/json");
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterAPI"],"GetItemsRoutingDetailsForLpCosting"))
                    { Content= content };
                    request.Headers.Add("Authorization", token);

                    var inventoryObjectResult = await client.SendAsync(request);
                    if (!inventoryObjectResult.IsSuccessStatusCode)
                    {
                        _logger.LogError($"ItemMaster Routing Details not found for Engg Released Items in RFQ: {rfqNumber}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"ItemMaster Routing Details not found for Engg Released Items in RFQ: {rfqNumber}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        return StatusCode(404, serviceResponse);
                    }
                    // var inventoryObjectResult = await _httpClient.PostAsync(string.Concat(_config["ItemMasterAPI"], "GetItemsRoutingDetailsForLpCosting"), content);
                    var itemsRoutingDetailsJsonString = await inventoryObjectResult.Content.ReadAsStringAsync();
                    dynamic itemsRoutingDetailsJson = JsonConvert.DeserializeObject(itemsRoutingDetailsJsonString);
                    var data = itemsRoutingDetailsJson.data;
                    itemsRoutingDetailsDynamic = data.ToObject<List<ItemMasterRoutingListDto>>();
                }

                //var rfqLpCostingDetails = await _rfqlpcostingRepository.GetRfqLPCostingByRfqNumber(rfqNumber);
                //if (rfqLpCostingDetails != null)
                //{
                //    var rfqLpCostingitemDetails = rfqLpCostingDetails.RfqLPCostingItems;
                //} 
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
                        LandedPrice = item.LandedPrice,
                        MOQCost = item.MOQCost,
                        TotalCost = 0,
                        MaterialCost = 0,
                        MarkUpForMaterial = 0,
                        RfqLPCostingProcesses = processStepsList,
                        RfqLPCostingNREConsumables = null,
                        RfqLPCostingOtherCharges = null
                    };
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EngineeringBomAPI"], $"GetEnggBomByItemNoAndRevNo?itemNumber={item.ItemNumber}&revisionNumber={item.CostingBomVersionNo}"));
                    request.Headers.Add("Authorization", token);

                    var BomDetails = await client.SendAsync(request);
                    var BomDetailsJsonString = await BomDetails.Content.ReadAsStringAsync();
                    dynamic BomDetailsJson = JsonConvert.DeserializeObject(BomDetailsJsonString);
                    var data1 = BomDetailsJson.data.bomNREConsumableDto;
                    List<RfqLPCostingNREConsumable> nREConsumables = new List<RfqLPCostingNREConsumable>();
                    foreach (var nre in data1)
                    {
                        RfqLPCostingNREConsumable nREConsumable = new RfqLPCostingNREConsumable
                        {
                            NREQty = nre.nreQuantity,
                            NRECost = nre.nreCost,
                            RfqLPCostingItemId = rfqLPCostingItem.Id,
                        };
                        nREConsumables.Add(nREConsumable);
                    }
                    rfqLPCostingItem.RfqLPCostingNREConsumables = nREConsumables;
                    rfqLPCostingItems.Add(rfqLPCostingItem);
                }

                var rfqLPCostingDetail = new RfqLPCosting();
                rfqLPCostingDetail.RfqNumber = rfqEnggDetails.RFQNumber;
                rfqLPCostingDetail.CustomerName = rfqEnggDetails.CustomerName;
                rfqLPCostingDetail.RfqLPCostingItems = rfqLPCostingItems;
                rfqLPCostingDetail.RevisionNumber= rfqEnggDetails.RevisionNumber;
                RfqLPCostingDto rfqLPCostingDto = _mapper.Map<RfqLPCostingDto>(rfqLPCostingDetail);

                _logger.LogInfo($"Returned DetailsForRfqLPCosting with rfqNumber: {rfqNumber} Successfully");
                serviceResponse.Data = rfqLPCostingDto;
                serviceResponse.Message = $"Returned DetailsForRfqLPCosting with rfqNumber: {rfqNumber}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDetailsForLPCostingByRfqNumber API for the following rfqNumber:{rfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDetailsForLPCostingByRfqNumber API for the following rfqNumber:{rfqNumber} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetRfqLPCostingByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqLPCostingByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message}";
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
                    serviceResponse.Message = $"GetRfqEngg with this RfqNumber hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GetRfqEnggByRfqNumber with id: {RfqNumber}, hasn't been found in db.");
                    return Ok(serviceResponse);
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
                    serviceResponse.Message = $"Returned RfqEnggByRfqNumber with RfqNumber: {RfqNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqEnggByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqEnggByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetRfqById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqById API for the following id:{id} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetRfqEnggById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqEnggById API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetRfqEnggByRfqNoAndRevNo(string rfqNumber, decimal revisionNumber)
        {
            ServiceResponse<RfqEnggDto> serviceResponse = new ServiceResponse<RfqEnggDto>();

            try
            {
                var rfqEnggByRfqNoAndRevNo = await _rfqenggRepository.GetRfqEnggByRfqNoAndRevNo(rfqNumber, revisionNumber);

                if (rfqEnggByRfqNoAndRevNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqEngg not found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqEngg hasn't been found in db.");
                    return Ok(serviceResponse);
                }
            
                else
                {
                    _logger.LogInfo($"Returned RfqEngg with");
                    var result = _mapper.Map<RfqEnggDto>(rfqEnggByRfqNoAndRevNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned RfqEngg";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqEnggByRfqNoAndRevNo API for the following rfqNumber:{rfqNumber} and  revisionNumber : {revisionNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqEnggByRfqNoAndRevNo API for the following rfqNumber:{rfqNumber} and  revisionNumber : {revisionNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetRfqEnggLatestRevNoByRfqNumber(string rfqNumber)
        {
            ServiceResponse<RfqEnggDto> serviceResponse = new ServiceResponse<RfqEnggDto>();

            try
            {
                var rfqEnggLatestRevNoByRfqNo = await _rfqenggRepository.GetRfqEnggLatestRevNoByRfqnumber(rfqNumber);

                if (rfqEnggLatestRevNoByRfqNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqEnggLatestRevNo with rfqNumber: {rfqNumber}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqEnggLatestRevNo with RfqNumber: {rfqNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned RfqEnggLatestRevNo with rfqNumber: {rfqNumber}");
                    var result = _mapper.Map<RfqEnggDto>(rfqEnggLatestRevNoByRfqNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned RfqEnggLatestRevNo with RfqNumber: {rfqNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqEnggLatestRevNoByRfqNumber API for the following rfqNumber:{rfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqEnggLatestRevNoByRfqNumber API for the following rfqNumber:{rfqNumber} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetRfqLPCostingById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqLPCostingById API for the following id:{id} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetRfqCustomerSupportById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqCustomerSupportById API for the following id:{id} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetRfqCsbyRfqNumberandRevNo(string rfqNumber, decimal revisionNumber)
        {
            ServiceResponse<RfqCustomerSupportDto> serviceResponse = new ServiceResponse<RfqCustomerSupportDto>();

            try
            {
                var rfqCsByRfqNoAndRevNo = await _repository.GetRfqCsByRfqNoAndRevNo(rfqNumber, revisionNumber);

                if (rfqCsByRfqNoAndRevNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqCustomerSupport Not Created.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqCustomerSupport Not Created..");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned RfqCustomerSupport with id");

                    RfqCustomerSupportDto rfCSqDto = _mapper.Map<RfqCustomerSupportDto>(rfqCsByRfqNoAndRevNo);

                    List<RfqCustomerSupportItemDto> rfqItemsDtos = new List<RfqCustomerSupportItemDto>();
                    var rfqDetails = await _rfqRepository.GetRfqDeatailsByRfqNoAndRevNo(rfqNumber,Convert.ToInt32(revisionNumber));
                    rfCSqDto.SalesPerson = rfqDetails.SalesPerson;
                    foreach (var rfqCSItemDetail in rfqCsByRfqNoAndRevNo.RfqCustomerSupportItems)
                    {
                        RfqCustomerSupportItemDto rfqCSItemDto = _mapper.Map<RfqCustomerSupportItemDto>(rfqCSItemDetail);
                        rfqCSItemDto.RfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliveryScheduleDto>>(rfqCSItemDetail.RfqCSDeliverySchedule);
                        rfqItemsDtos.Add(rfqCSItemDto);
                    }
                    rfCSqDto.RfqCustomerSupportItems = rfqItemsDtos;

                    serviceResponse.Data = rfCSqDto;
                    serviceResponse.Message = $"Returned RfqCustomerSupport";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqCsbyRfqNumberandRevNo API for the following rfqNumber:{rfqNumber} and revisionNumber : {revisionNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqCsbyRfqNumberandRevNo API for the following rfqNumber:{rfqNumber} and revisionNumber : {revisionNumber} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetRfqCsLatestRevNoByRfqnumber(string rfqNumber)
        {
            ServiceResponse<RfqCustomerSupportDto> serviceResponse = new ServiceResponse<RfqCustomerSupportDto>();

            try
            {
                var rfqCsLatestRevNoByRfqNo = await _repository.GetRfqCsLatestRevNoByRfqnumber(rfqNumber);

                if (rfqCsLatestRevNoByRfqNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqCustomerSupport hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqCustomerSupport with id,hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned RfqCustomerSupport with id");

                    RfqCustomerSupportDto rfCSqDto = _mapper.Map<RfqCustomerSupportDto>(rfqCsLatestRevNoByRfqNo);

                    serviceResponse.Data = rfCSqDto;
                    serviceResponse.Message = $"Returned RfqCustomerSupportLatestRevNo";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqCsLatestRevNoByRfqnumber API for the following rfqNumber:{rfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqCsLatestRevNoByRfqnumber API for the following rfqNumber:{rfqNumber} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in UpdateRfqCustomerSupportRelease API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateRfqCustomerSupportRelease API : \n {ex.Message}";
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
                _logger.LogError($"Error Occured in UpdateRfqEnggItemRelease API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateRfqEnggItemRelease API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //private List<DocumentUpload> CoCDocumentSave(List<RfqCustomerSupportItemPostDto>? grinPartsDto, RfqCustomerSupport grins, string number, int i, List<DocumentUpload> grinPartsDocumentUploadDtoList)
        //{
        //    var cocUploadDocs = grinPartsDto[i].Upload;

        //    foreach (var cocUpload in cocUploadDocs)
        //    {
        //        var fileContent = cocUpload.FileByte;

        //        string fileName = cocUpload.FileName + "." + cocUpload.FileExtension;
        //        string FileExt = Path.GetExtension(fileName).ToUpper();

        //        //Guid guid = Guid.NewGuid();
        //        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "CSItems",/* guid.ToString() + "_" +*/ fileName);
        //        using (MemoryStream ms = new MemoryStream(fileContent))
        //        {
        //            ms.Position = 0;
        //            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //            {
        //                ms.WriteTo(fileStream);
        //            }
        //            var uploadedFile = new DocumentUpload
        //            {
        //                FileName = fileName,
        //                FileExtension = FileExt,
        //                FilePath = filePath,
        //                ParentId = number,          //It Should be changed to GrinPartsId
        //                DocumentFrom = "CSItemDocument",
        //            };

        //            _documentUploadRepository.CreateUploadDocument(uploadedFile);
        //            _documentUploadRepository.SaveAsync();

        //            if (uploadedFile != null)
        //            {
        //                DocumentUpload poFileDetails = _mapper.Map<DocumentUpload>(uploadedFile);
        //                grinPartsDocumentUploadDtoList.Add(poFileDetails);
        //            }

        //        }
        //        grins.RfqCustomerSupportItems[i].Upload = grinPartsDocumentUploadDtoList;

        //    }
        //    return grinPartsDocumentUploadDtoList;
        //}

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

                var CSitemDocumentUploadDtoList = new List<DocumentUpload>();
                var rfqCustomerSupportLists = new List<RfqCustomerSupportItems>();
                for (int i = 0; i < rfqCSItemDto.Count; i++)
                {
                    List<DocumentUpload>? files = null;
                    RfqCustomerSupportItems rfqCSItems = _mapper.Map<RfqCustomerSupportItems>(rfqCSItemDto[i]);
                    //if (rfqCSItemDto[i].Upload != null && rfqCSItemDto[i].Upload.Count > 0)
                    //{
                    //    files = CoCDocumentSave(rfqCSItemDto, createRfqCS, rfqCSItems.Id.ToString(), i, CSitemDocumentUploadDtoList);
                    //}
                    //rfqCSItems.Upload = _mapper.Map<List<DocumentUpload>>(files);
                    rfqCSItems.RfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliverySchedule>>(rfqCSItemDto[i].RfqCSDeliverySchedule);
                    rfqCustomerSupportLists.Add(rfqCSItems);

                }
                //var rfqCustomerSupportLists = new List<RfqCustomerSupportItems>();
                //for (int i = 0; i < rfqCSItemDto.Count; i++)
                //{
                //    RfqCustomerSupportItems rfqCSItems = _mapper.Map<RfqCustomerSupportItems>(rfqCSItemDto[i]);
                //    rfqCSItems.RfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliverySchedule>>(rfqCSItemDto[i].RfqCSDeliverySchedule);
                //    rfqCustomerSupportLists.Add(rfqCSItems);

                //}
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
                _logger.LogError($"Error Occured in CreateRfqCustomerSupport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateRfqCustomerSupport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRfqCustomerSupportItemFileUpload([FromBody] List<DocumentUploadPostDto> documentUploadPostDtos)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (documentUploadPostDtos is null)
                {
                    _logger.LogError("RfqCustomerSupportItemFile object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RfqCustomerSupportItemFile object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid RfqCustomerSupportItemFile object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                List<string>? id_s = new List<string>();
                var FileUploadDetails = documentUploadPostDtos;
                foreach (var FileUploadDetail in FileUploadDetails)
                {
                    Guid guids = Guid.NewGuid();
                    byte[] fileContent = Convert.FromBase64String(FileUploadDetail.FileByte);
                    //var itemNumber = fileUploadPostDtos.ItemNumber;
                    string fileName = guids.ToString() + "_" + FileUploadDetail.FileName + "." + FileUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    //Guid guids = Guid.NewGuid();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "CSItems", fileName);
                    using (MemoryStream ms = new MemoryStream(fileContent))
                    {
                        ms.Position = 0;
                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            ms.WriteTo(fileStream);
                        }
                        var uploadedFile = new DocumentUpload
                        {
                            FileName = fileName,
                            FileExtension = FileExt,
                            FilePath = filePath,
                            ParentId = "Rfq",
                            DocumentFrom = "RfqCustomerSupportItemFile Document",
                            FileByte = FileUploadDetail.FileByte
                        };
                        _documentUploadRepository.CreateUploadDocument(uploadedFile);
                        _documentUploadRepository.SaveAsync();                        
                        id_s.Add(uploadedFile.Id.ToString());
                    }
                }
                serviceResponse.Data = id_s;
                serviceResponse.Message = " RfqCustomerSupportItemFile Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateRfqCustomerSupportItemFileUpload API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateRfqCustomerSupportItemFileUpload API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetailsforRfqCustomerSupportItemFiles(string fileids)
        {
            ServiceResponse<List<DocumentUploadDto>> serviceResponse = new ServiceResponse<List<DocumentUploadDto>>();
            try
            {
                string serverKey = GetServerKey();
                var itemsFiles = await _documentUploadRepository.GetDownloadUrlDetails(fileids);
                if (itemsFiles == null)
                {
                    _logger.LogError($"DownloadDetail with id: {fileids}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DownloadDetail with id: {fileids}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid RfqCustomerSupport UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid RfqCustomerSupport UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<DocumentUploadDto> fileUploads = new List<DocumentUploadDto>();
                if (itemsFiles != null)
                {
                    foreach (var fileUploadDetails in itemsFiles)
                    {
                        DocumentUploadDto fileUploadDto = _mapper.Map<DocumentUploadDto>(fileUploadDetails);
                        if (serverKey == "avision")
                        {
                            var baseUrl = $"{_config["RfqAPI"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/Rfq/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        else
                        {
                            var baseUrl = $"{_config["RfqAPI"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/api/Rfq/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                                                
                        fileUploads.Add(fileUploadDto);
                    }
                }
                _logger.LogInfo($"Returned DownloadDetail with id: {fileids}");
                
                serviceResponse.Data = fileUploads;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDownloadUrlDetailsforRfqCustomerSupportItemFiles API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDownloadUrlDetailsforRfqCustomerSupportItemFiles API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //rfq create function
        [HttpGet]
        public async Task<IActionResult> GetAllActiveLatestRfqNumbers()
        {
            ServiceResponse<IEnumerable<LatestRfqNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<LatestRfqNumberListDto>>();
            try
            {
                var latestRfqNumberList = await _rfqRepository.GetAllActiveLatestRfqNumbers();
                var result = _mapper.Map<IEnumerable<LatestRfqNumberListDto>>(latestRfqNumberList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ActiveRfqNumberListByCustomerId";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveLatestRfqNumbers API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveLatestRfqNumbers API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRfq([FromBody] RfqPostDto rfqPostDto)
        {
            ServiceResponse<RfqDto> serviceResponse = new ServiceResponse<RfqDto>();

            try
            {
                string serverKey = GetServerKey();

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
                createRfq.IsLpRelease = true;
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                ////var newcount = await _rfqRepository.GetRfqNumberAutoIncrementCount(date);
                if (serverKey == "trasccon")
                {
                    var rfqNumber = await _rfqRepository.GenerateRFQNumberForTransccon();
                    createRfq.RfqNumber = rfqNumber; 
                }
                else if (serverKey == "keus")
                {
                    var dateFormat = days + months + years;
                    var rfqNumber = await _rfqRepository.GenerateRFQNumber();
                    createRfq.RfqNumber = dateFormat + rfqNumber;
                }
                else
                {
                    //var dateFormat = days + months + years;
                    //var rfqNumber = await _rfqRepository.GenerateRFQNumberAvision();
                    createRfq.RfqNumber = rfqPostDto.RfqNumber; 
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
                _logger.LogError($"Error Occured in CreateRfq API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateRfq API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadFile(string Filename)
        {
            ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();
            var filename = Uri.UnescapeDataString(Filename);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "CSItems", filename);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var ContentType))
            {
                ContentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var DownloadFilename = filename.Split('_');
            var downloadFilename = string.IsNullOrWhiteSpace(DownloadFilename[1]) ? Path.GetFileName(filePath) : DownloadFilename[1];

            return File(bytes, ContentType, downloadFilename);
        }
        private string GetServerKey()
        {
            var serverName = Environment.MachineName;
            var serverConfiguration = _config.GetSection("ServerConfiguration");

            if (serverConfiguration.GetValue<bool?>("Server1:EnableKeus") == true)
            {
                return "keus";
            }
            else if (serverConfiguration.GetValue<bool?>("Server1:EnableAvision") == true)
            {
                return "avision";

            }
            else
            {
                return "trasccon";
            }
        }

        //get revision number by passing rfqnumber
        [HttpGet("{RfqNumber}")]
        public async Task<IActionResult> GetRevNumberByRfqNumberList(string RfqNumber)
        {
            ServiceResponse<IEnumerable<RevNumberByRfqNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<RevNumberByRfqNumberListDto>>();
            try
            {
                var revNumberDetailsbyRfqNumber = await _rfqRepository.GetRevNumberByRfqNumberList(RfqNumber);
                var result = _mapper.Map<IEnumerable<RevNumberByRfqNumberListDto>>(revNumberDetailsbyRfqNumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RevisionNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRevNumberByRfqNumberList API for the following RfqNumber:{RfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRevNumberByRfqNumberList API for the following RfqNumber:{RfqNumber} \n {ex.Message} ";
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
                var updateRfqIsLpCosting = await _rfqRepository.RfqLpcostingByRfqNumbers(rfqUpdate, createRfqLPCosting.RevisionNumber);
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
                _logger.LogError($"Error Occured in CreateRfqLPcosting API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateRfqLPcosting API : \n {ex.Message}";
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

                var rfqEnggItemDto = rfqEnggDtoPost.RfqEnggItems;
                var rfqEnggLists = new List<RfqEnggItem>();
                for (int i = 0; i < rfqEnggItemDto.Count; i++)
                {
                    RfqEnggItem rfqEnggItems = _mapper.Map<RfqEnggItem>(rfqEnggItemDto[i]);
                    rfqEnggLists.Add(rfqEnggItems);
                }
                createRfqEngg.RfqEnggItems = rfqEnggLists;

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
                _logger.LogError($"Error Occured in CreateRfqEngg API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateRfqEngg API : \n {ex.Message}";
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
                

                //var updaterfq = _mapper.Map<Rfq>(rfqUpdateDto);
                //await _rfqRepository.UpdateRfqRevNo(updaterfq);
                //updaterfq.RevisionNumber += 1;
                //_logger.LogInfo(result);
                string serverKey = GetServerKey();
                var updaterfq = _mapper.Map<Rfq>(rfqUpdateDto);
                await _rfqRepository.UpdateRfqRevNo(updaterfq, serverKey);
                _rfqRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateRfq API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateRfq API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // Update rfqengg
        //[HttpPut]
        //public async Task<IActionResult> UpdateRfqEngg([FromBody] RfqEnggDtoUpdate rfqEnggDtoUpdate)
        //{
        //    ServiceResponse<RfqEnggDto> serviceResponse = new ServiceResponse<RfqEnggDto>();

        //    try
        //    {
        //        if (rfqEnggDtoUpdate is null)
        //        {
        //            _logger.LogError("RfqEngg object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Update RfqEngg object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid Rfqengg object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid Update Rfqengg object sent from client.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }

        //        var rfqEnggItems = _mapper.Map<IEnumerable<RfqEnggItem>>(rfqEnggDtoUpdate.RfqEnggItems);
        //        var rfqEnggRiskIdentificationItems = _mapper.Map<IEnumerable<RfqEnggRiskIdentification>>(rfqEnggDtoUpdate.RfqEnggRiskIdentifications);

        //        var updateRfqEngg = _mapper.Map<RfqEngg>(rfqEnggDtoUpdate);

        //        updateRfqEngg.RfqEnggItems = rfqEnggItems.ToList();
        //        updateRfqEngg.RfqEnggRiskIdentifications = rfqEnggRiskIdentificationItems.ToList();

        //        var rfqNumber = updateRfqEngg.RFQNumber;
        //        var enggReleasedItems = await _rfqenggItemRepository.RfqEnggReleasedItemList(rfqNumber);
        //        var updatedItems = new List<RfqEnggItemDtoUpdate>();

        //        foreach (var itemList in rfqEnggDtoUpdate.RfqEnggItems)
        //        {
        //            var releaseItem = enggReleasedItems.FirstOrDefault(item => item.ItemNumber == itemList.ItemNumber);

        //            if (releaseItem != null)
        //            {
        //                itemList.ReleaseStatus = true;
        //            }
        //            else
        //            {
        //                itemList.ReleaseStatus = false;
        //            }

        //            updatedItems.Add(itemList);
        //        }
        //        var rfqDetailsByRfqNumber = await _rfqRepository.RfqDetailsByRfqNumbers(rfqNumber);

        //        var version = 1;

        //        rfqDetailsByRfqNumber.RevisionNumber = rfqDetailsByRfqNumber.RevisionNumber + version;


        //        await _rfqenggRepository.UpdateRfqEnggRevNo(updateRfqEngg);
        //        _rfqRepository.Update(rfqDetailsByRfqNumber);
        //        //string result = await _rfqenggRepository.UpdateRfqEngg(updateRfqEngg);
        //        //_logger.LogInfo(result);
        //        _rfqenggRepository.SaveAsync();
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Updated Successfully";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside UpdateRfqEngg action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
        //Update RfqLPCosting

        [HttpPut]
        public async Task<IActionResult> UpdateRfqEngg([FromBody] RfqEnggDtoUpdate rfqEnggDtoUpdate)
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
                var rfqEnggItems = _mapper.Map<IEnumerable<RfqEnggItem>>(rfqEnggDtoUpdate.RfqEnggItems);
                var rfqEnggRiskIdentificationItems = _mapper.Map<IEnumerable<RfqEnggRiskIdentification>>(rfqEnggDtoUpdate.RfqEnggRiskIdentifications);
                var updateRfqEngg = _mapper.Map<RfqEngg>(rfqEnggDtoUpdate);
                updateRfqEngg.RfqEnggItems = rfqEnggItems.ToList();
                updateRfqEngg.RfqEnggRiskIdentifications = rfqEnggRiskIdentificationItems.ToList();
                var rfqNumber = updateRfqEngg.RFQNumber;
                var enggReleasedItems = await _rfqenggItemRepository.RfqEnggReleasedItemList(rfqNumber);
                var updatedItems = new List<RfqEnggItemDtoUpdate>();
                var rfqDetailsByRfqNumber = await _rfqRepository.RfqDetailsByRfqNumbers(rfqNumber);
                rfqDetailsByRfqNumber.RevisionNumber = rfqDetailsByRfqNumber.RevisionNumber + 1;
                int flag = 0;
                foreach (var itemList in rfqEnggDtoUpdate.RfqEnggItems)
                {
                    bool releaseItem = enggReleasedItems.Any(item => item.Id == itemList.Id);
                    if (releaseItem)
                    {
                        itemList.ReleaseStatus = true;
                        flag = 1;
                    }
                    else
                    {
                        itemList.ReleaseStatus = false;
                        rfqDetailsByRfqNumber.IsSourcing = false;
                        if (flag == 0)
                        {
                            rfqDetailsByRfqNumber.IsEnggRelease = CsRelease.NotYetReleased;
                        }
                        else
                        {
                            rfqDetailsByRfqNumber.IsEnggRelease = CsRelease.PartiallyRelease;
                        }
                        rfqDetailsByRfqNumber.IsEnggComplete = false;
                        rfqDetailsByRfqNumber.EnggComplete = EnggStatus.EnggNotYetCompleted;
                    }
                    itemList.Id = 0;
                    updatedItems.Add(itemList);
                }
                updateRfqEngg.RfqEnggItems = _mapper.Map<List<RfqEnggItem>>(updatedItems);                
                await _rfqenggRepository.UpdateRfqEnggRevNo(updateRfqEngg);
                rfqDetailsByRfqNumber.Id = 0;
                _rfqRepository.Create(rfqDetailsByRfqNumber);
                // creating CS for new RFQ version
                string serverKey = GetServerKey();
                if (!serverKey.Equals("keus"))
                {
                    await _repository.UpdateRfqCSRev(rfqDetailsByRfqNumber.RfqNumber, rfqDetailsByRfqNumber.RevisionNumber);
                }
                _repository.SaveAsync();
                _rfqenggRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateRfqEngg API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateRfqEngg API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
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
                var getRfqLpCostingByid = await _rfqlpcostingRepository.GetRfqLPCostingByIdNoTracking(id);
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

                var updateData = _mapper.Map(rfqLPCostingDtoUpdate, getRfqLpCostingByid);
                updateData.RfqLPCostingItems = rfqlpcostingitemList;
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
                _logger.LogError($"Error Occured in UpdateRfqLPCosting API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateRfqLPCosting API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //[HttpPut]
        //public async Task<IActionResult> UpdateRfqCustomerSupport([FromBody] RfqCustomerSupportUpdateDto rfqCustomerSupportUpdateDto)
        //{
        //    ServiceResponse<RfqCustomerSupportDto> serviceResponse = new ServiceResponse<RfqCustomerSupportDto>();

        //    try
        //    {
        //        if (rfqCustomerSupportUpdateDto is null)
        //        {
        //            _logger.LogError("RfqCustomerSupport object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Update RfqCustomerSupport object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid RfqCustomerSupport object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid Update RfqCustomerSupport object sent from client.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }


        //        var updateRfqCS = _mapper.Map<RfqCustomerSupport>(rfqCustomerSupportUpdateDto);


        //        var rfqNumber = updateRfqCS.RfqNumber;

        //        var csReleasedItems = await _itemRepository.RfqCsReleasedItemList(rfqNumber);
        //        var updatedItems = new List<RfqCustomerSupportItemUpdateDto>();

        //        foreach (var itemList in rfqCustomerSupportUpdateDto.RfqCustomerSupportItems)
        //        {
        //            var releaseItem = csReleasedItems.FirstOrDefault(item => item.ItemNumber == itemList.ItemNumber);

        //            if (releaseItem != null)
        //            {
        //                itemList.ReleaseStatus = true;
        //            }
        //            else
        //            {
        //                itemList.ReleaseStatus = false;
        //            }

        //            updatedItems.Add(itemList);
        //        }

        //        rfqCustomerSupportUpdateDto.RfqCustomerSupportItems = updatedItems;



        //        var rfqDetailsByRfqNumber = await _rfqRepository.RfqDetailsByRfqNumbers(rfqNumber);

        //        var version = 1;

        //        rfqDetailsByRfqNumber.RevisionNumber = rfqDetailsByRfqNumber.RevisionNumber + version;

        //        //test


        //        var rfqCSItemDto = rfqCustomerSupportUpdateDto.RfqCustomerSupportItems;

        //        var rfqCsItemList = new List<RfqCustomerSupportItems>();
        //        if (rfqCSItemDto != null)
        //        {
        //            for (int i = 0; i < rfqCSItemDto.Count; i++)
        //            {
        //                RfqCustomerSupportItems rfqCSItemDetail = _mapper.Map<RfqCustomerSupportItems>(rfqCSItemDto[i]);
        //                rfqCSItemDetail.RfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliverySchedule>>(rfqCSItemDto[i].RfqCSDeliverySchedule);
        //                rfqCsItemList.Add(rfqCSItemDetail);

        //            }
        //        }
        //        updateRfqCS.RfqCustomerSupportItems = rfqCsItemList;
        //        var data = _mapper.Map(rfqCustomerSupportUpdateDto, updateRfqCS);
        //        await _repository.UpdateRfqcsRevNo(data);
        //        //string result = await _repository.UpdateRfqCustomerSupport(data);
        //        //_logger.LogInfo(result);
        //        _rfqRepository.Update(rfqDetailsByRfqNumber);
        //        _repository.SaveAsync();
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Updated Successfully";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside UpdateRfqCustomerSupport action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        //Delete RFq

        //private List<DocumentUpload> CocDocumentSave(List<RfqCustomerSupportItemUpdateDto>? grinPartsDto, RfqCustomerSupport grins, string number, int i, List<DocumentUpload> grinPartsDocumentUploadDtoList)
        //{
        //    var cocUploadDocs = grinPartsDto[i].Upload;

        //    foreach (var cocUpload in cocUploadDocs)
        //    {
        //        var fileContent = cocUpload.FileByte;

        //        string fileName = cocUpload.FileName + "." + cocUpload.FileExtension;
        //        string FileExt = Path.GetExtension(fileName).ToUpper();

        //        //Guid guid = Guid.NewGuid();
        //        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "CSItems",/* guid.ToString() + "_" +*/ fileName);
        //        using (MemoryStream ms = new MemoryStream(fileContent))
        //        {
        //            ms.Position = 0;
        //            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //            {
        //                ms.WriteTo(fileStream);
        //            }
        //            var uploadedFile = new DocumentUpload
        //            {
        //                FileName = fileName,
        //                FileExtension = FileExt,
        //                FilePath = filePath,
        //                ParentId = number,          //It Should be changed to GrinPartsId
        //                DocumentFrom = "CSItemDocument",
        //            };

        //            _documentUploadRepository.CreateUploadDocument(uploadedFile);
        //            _documentUploadRepository.SaveAsync();

        //            if (uploadedFile != null)
        //            {
        //                DocumentUpload poFileDetails = _mapper.Map<DocumentUpload>(uploadedFile);
        //                grinPartsDocumentUploadDtoList.Add(poFileDetails);
        //            }

        //        }
        //        //  grins.RfqCustomerSupportItems[i].Upload = grinPartsDocumentUploadDtoList;

        //    }
        //    return grinPartsDocumentUploadDtoList;
        //}

        [HttpPut]
        public async Task<IActionResult> UpdateRfqCustomerSupport([FromBody] RfqCustomerSupportUpdateDto rfqCustomerSupportUpdateDto)
        {
            ServiceResponse<RfqCustomerSupportUpdateDto> serviceResponse = new ServiceResponse<RfqCustomerSupportUpdateDto>();
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
                Rfq oldversionRFQdetails = await _rfqRepository.RfqDetailsByRfqNumbers(rfqCustomerSupportUpdateDto.RfqNumber);
                oldversionRFQdetails.RfqNumber = rfqCustomerSupportUpdateDto.RfqNumber;
                oldversionRFQdetails.RevisionNumber = rfqCustomerSupportUpdateDto.RevisionNumber;
                oldversionRFQdetails.CustomerId = rfqCustomerSupportUpdateDto.CustomerId;
                oldversionRFQdetails.LeadId = rfqCustomerSupportUpdateDto.CustomerId;
                oldversionRFQdetails.CustomerName = rfqCustomerSupportUpdateDto.CustomerName;
                oldversionRFQdetails.CustomerAliasName = rfqCustomerSupportUpdateDto.CustomerAliasName;
                oldversionRFQdetails.CustomerRfqNumber = rfqCustomerSupportUpdateDto.CustomerRfqNumber;
                oldversionRFQdetails.RequestReceivedate = rfqCustomerSupportUpdateDto.RequestReceivedate;
                oldversionRFQdetails.QuoteExpectdate = rfqCustomerSupportUpdateDto.QuoteExpectdate;
                oldversionRFQdetails.TypeOfSolution = rfqCustomerSupportUpdateDto.TypeOfSolution;
                oldversionRFQdetails.ProductType = rfqCustomerSupportUpdateDto.ProductType;
                oldversionRFQdetails.Remarks = rfqCustomerSupportUpdateDto.Remarks;
                oldversionRFQdetails.ReasonForModification = rfqCustomerSupportUpdateDto.ReasonForModification;
                oldversionRFQdetails.RevisionNumber = oldversionRFQdetails.RevisionNumber + 1;
                var oldRFQcsReleasedItems = await _itemRepository.RfqCsReleasedItemList(rfqCustomerSupportUpdateDto.RfqNumber);
                var updatedItems = new List<RfqCustomerSupportItemUpdateDto>();
                int flag = 0;
                foreach (var itemList in rfqCustomerSupportUpdateDto.RfqCustomerSupportItems)
                {
                    bool releaseItem = oldRFQcsReleasedItems.Any(item => item == itemList.Id);
                    if (releaseItem)
                    {
                        itemList.ReleaseStatus = true;
                        flag = 1;
                    }
                    else
                    {
                        itemList.ReleaseStatus = false;
                        oldversionRFQdetails.IsSourcing = false;
                        oldversionRFQdetails.IsCsComplete = false;
                        oldversionRFQdetails.CsComplete = CsStatus.CsNotYetCompleted;
                        if (flag == 0)
                        {
                            oldversionRFQdetails.IsCsRelease = CsRelease.NotYetReleased;
                            oldversionRFQdetails.IsEnggRelease = CsRelease.NotYetReleased;
                        }
                        else
                        {
                            oldversionRFQdetails.IsCsRelease = CsRelease.PartiallyRelease;
                            oldversionRFQdetails.IsEnggRelease = CsRelease.PartiallyRelease;
                        }
                        oldversionRFQdetails.IsEnggComplete = false;
                        oldversionRFQdetails.EnggComplete = EnggStatus.EnggNotYetCompleted;
                    }
                    updatedItems.Add(itemList);
                }
                oldversionRFQdetails.Id = 0;
                oldversionRFQdetails.CreatedBy = _createdBy;
                oldversionRFQdetails.CreatedOn = DateTime.Now;
                _rfqRepository.Create(oldversionRFQdetails);
                rfqCustomerSupportUpdateDto.RfqCustomerSupportItems = null;
                rfqCustomerSupportUpdateDto.RfqCustomerSupportItems = updatedItems;
                RfqCustomerSupport CS = null;
                var rfqCSItemDto = rfqCustomerSupportUpdateDto.RfqCustomerSupportItems;
                var rfqCsItemList = new List<RfqCustomerSupportItems>();
                var CSitemDocumentUploadDtoList = new List<DocumentUpload>();
                if (rfqCSItemDto != null)
                {
                    for (int i = 0; i < rfqCSItemDto.Count; i++)
                    {
                        //List<DocumentUpload>? files = null;
                        RfqCustomerSupportItems rfqCSItemDetail = _mapper.Map<RfqCustomerSupportItems>(rfqCSItemDto[i]);
                        //if (rfqCSItemDto[i].Upload != null && rfqCSItemDto[i].Upload.Count > 0)
                        //{
                        //    files = CocDocumentSave(rfqCSItemDto, CS, rfqCSItemDetail.Id.ToString(), i, CSitemDocumentUploadDtoList);
                        //}
                        //rfqCSItemDetail.Upload = _mapper.Map<List<DocumentUpload>>(files);
                        rfqCSItemDetail.RfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliverySchedule>>(rfqCSItemDto[i].RfqCSDeliverySchedule);
                        rfqCSItemDetail.Id = 0;
                        rfqCsItemList.Add(rfqCSItemDetail);
                    }
                }
                var rfqCSnotedto = rfqCustomerSupportUpdateDto.RfqCustomerSupportNotes;
                var rfqCsnotelist = new List<RfqCustomerSupportNotes>();
                if (rfqCSnotedto != null)
                {
                    for (int i = 0; i < rfqCSnotedto.Count; i++)
                    {
                        RfqCustomerSupportNotes rfqCSnoteDetail = _mapper.Map<RfqCustomerSupportNotes>(rfqCSnotedto[i]);
                        rfqCsnotelist.Add(rfqCSnoteDetail);
                    }
                }
                RfqCustomerSupport oldCSdetails = await _repository.GetRfqCustomerSupportDetailsbyrfqnumber(rfqCustomerSupportUpdateDto.RfqNumber);
                oldCSdetails.Id = 0;
                oldCSdetails.LeadId = oldversionRFQdetails.LeadId;
                oldCSdetails.CustomerName = oldversionRFQdetails.CustomerName;
                oldCSdetails.RevisionNumber = oldversionRFQdetails.RevisionNumber;
                oldCSdetails.CustomerAliasName = oldversionRFQdetails.CustomerAliasName;
                oldCSdetails.CustomerRfqNumber = oldversionRFQdetails.CustomerRfqNumber;
                oldCSdetails.RequestReceivedate = oldversionRFQdetails.RequestReceivedate;
                oldCSdetails.QuoteExpectdate = oldversionRFQdetails.QuoteExpectdate;
                oldCSdetails.TypeOfSolution = oldversionRFQdetails.TypeOfSolution;
                oldCSdetails.ProductType = oldversionRFQdetails.ProductType;
                oldCSdetails.Unit = oldversionRFQdetails.Unit;
                oldCSdetails.RfqCustomerSupportItems = rfqCsItemList;
                oldCSdetails.RfqCustomerSupportNotes = rfqCsnotelist;
                await _repository.UpdateRfqcsRevNo(oldCSdetails);
                // creating engg for new RFQ version
                //string serverKey = GetServerKey();
                //if (!serverKey.Equals("keus"))
                //{
                //    await _rfqenggRepository.UpdateRfqEnggRev(oldversionRFQdetails.RfqNumber, oldversionRFQdetails.RevisionNumber, oldversionRFQdetails);
                //    _rfqenggRepository.SaveAsync();
                //}
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateRfqCustomerSupport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateRfqCustomerSupport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRfqCustomerSupportForKeus([FromBody] RfqCustomerSupportUpdateDto rfqCustomerSupportUpdateDto)
        {
            ServiceResponse<RfqCustomerSupportUpdateDto> serviceResponse = new ServiceResponse<RfqCustomerSupportUpdateDto>();
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

                int latestRfqrevNo = await _rfqRepository.GetLastestRfqRevNoByRfqNumber(rfqCustomerSupportUpdateDto.RfqNumber);

                Rfq oldversionRFQdetails = await _rfqRepository.RfqDetailsById(rfqCustomerSupportUpdateDto.Id);
                oldversionRFQdetails.RfqNumber = rfqCustomerSupportUpdateDto.RfqNumber;
                //oldversionRFQdetails.RevisionNumber = rfqCustomerSupportUpdateDto.RevisionNumber;
                oldversionRFQdetails.CustomerId = rfqCustomerSupportUpdateDto.CustomerId;
                oldversionRFQdetails.LeadId = rfqCustomerSupportUpdateDto.CustomerId;
                oldversionRFQdetails.CustomerName = rfqCustomerSupportUpdateDto.CustomerName;
                oldversionRFQdetails.CustomerAliasName = rfqCustomerSupportUpdateDto.CustomerAliasName;
                oldversionRFQdetails.CustomerRfqNumber = rfqCustomerSupportUpdateDto.CustomerRfqNumber;
                oldversionRFQdetails.RequestReceivedate = rfqCustomerSupportUpdateDto.RequestReceivedate;
                oldversionRFQdetails.QuoteExpectdate = rfqCustomerSupportUpdateDto.QuoteExpectdate;
                oldversionRFQdetails.TypeOfSolution = rfqCustomerSupportUpdateDto.TypeOfSolution;
                oldversionRFQdetails.ProductType = rfqCustomerSupportUpdateDto.ProductType;
                oldversionRFQdetails.Remarks = rfqCustomerSupportUpdateDto.Remarks;
                oldversionRFQdetails.ReasonForModification = rfqCustomerSupportUpdateDto.ReasonForModification;
                oldversionRFQdetails.RevisionNumber = latestRfqrevNo + 1;
                var oldRFQcsReleasedItems = await _itemRepository.RfqCsReleasedItemsList(rfqCustomerSupportUpdateDto.RfqNumber, rfqCustomerSupportUpdateDto.RevisionNumber);
                var updatedItems = new List<RfqCustomerSupportItemUpdateDto>();
                int flag = 0;
                foreach (var itemList in rfqCustomerSupportUpdateDto.RfqCustomerSupportItems)
                {
                    bool releaseItem = oldRFQcsReleasedItems.Any(item => item == itemList.Id);
                    if (releaseItem)
                    {
                        itemList.ReleaseStatus = true;
                        flag = 1;
                    }
                    else
                    {
                        itemList.ReleaseStatus = false;
                        oldversionRFQdetails.IsSourcing = false;
                        oldversionRFQdetails.IsCsComplete = false;
                        oldversionRFQdetails.CsComplete = CsStatus.CsNotYetCompleted;
                        if (flag == 0)
                        {
                            oldversionRFQdetails.IsCsRelease = CsRelease.NotYetReleased;
                            oldversionRFQdetails.IsEnggRelease = CsRelease.NotYetReleased;
                        }
                        else
                        {
                            oldversionRFQdetails.IsCsRelease = CsRelease.PartiallyRelease;
                            oldversionRFQdetails.IsEnggRelease = CsRelease.PartiallyRelease;
                        }
                        oldversionRFQdetails.IsEnggComplete = false;
                        oldversionRFQdetails.EnggComplete = EnggStatus.EnggNotYetCompleted;
                    }
                    updatedItems.Add(itemList);
                }
                oldversionRFQdetails.Id = 0;
                oldversionRFQdetails.CreatedBy = _createdBy;
                oldversionRFQdetails.CreatedOn = DateTime.Now;
                await _rfqRepository.Create(oldversionRFQdetails);

                rfqCustomerSupportUpdateDto.RfqCustomerSupportItems = null;
                rfqCustomerSupportUpdateDto.RfqCustomerSupportItems = updatedItems;

                RfqCustomerSupport CS = null;
                var rfqCSItemDto = rfqCustomerSupportUpdateDto.RfqCustomerSupportItems;
                var rfqCsItemList = new List<RfqCustomerSupportItems>();
                var CSitemDocumentUploadDtoList = new List<DocumentUpload>();
                if (rfqCSItemDto != null)
                {
                    for (int i = 0; i < rfqCSItemDto.Count; i++)
                    {
                        //List<DocumentUpload>? files = null;
                        RfqCustomerSupportItems rfqCSItemDetail = _mapper.Map<RfqCustomerSupportItems>(rfqCSItemDto[i]);
                        //if (rfqCSItemDto[i].Upload != null && rfqCSItemDto[i].Upload.Count > 0)
                        //{
                        //    files = CocDocumentSave(rfqCSItemDto, CS, rfqCSItemDetail.Id.ToString(), i, CSitemDocumentUploadDtoList);
                        //}
                        //rfqCSItemDetail.Upload = _mapper.Map<List<DocumentUpload>>(files);
                        rfqCSItemDetail.RfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliverySchedule>>(rfqCSItemDto[i].RfqCSDeliverySchedule);
                        rfqCSItemDetail.Id = 0;
                        rfqCsItemList.Add(rfqCSItemDetail);
                    }
                }
                var rfqCSnotedto = rfqCustomerSupportUpdateDto.RfqCustomerSupportNotes;
                var rfqCsnotelist = new List<RfqCustomerSupportNotes>();
                if (rfqCSnotedto != null)
                {
                    for (int i = 0; i < rfqCSnotedto.Count; i++)
                    {
                        RfqCustomerSupportNotes rfqCSnoteDetail = _mapper.Map<RfqCustomerSupportNotes>(rfqCSnotedto[i]);
                        rfqCsnotelist.Add(rfqCSnoteDetail);
                    }
                }
                RfqCustomerSupport oldCSdetails = await _repository.GetRfqCustomerSupportDetailsbyRfqNoAndRevNo(rfqCustomerSupportUpdateDto.RfqNumber, rfqCustomerSupportUpdateDto.RevisionNumber);
                oldCSdetails.Id = 0;
                oldCSdetails.LeadId = oldversionRFQdetails.LeadId;
                oldCSdetails.CustomerName = oldversionRFQdetails.CustomerName;
                oldCSdetails.RevisionNumber = latestRfqrevNo+1;
                oldCSdetails.CustomerAliasName = oldversionRFQdetails.CustomerAliasName;
                oldCSdetails.CustomerRfqNumber = oldversionRFQdetails.CustomerRfqNumber;
                oldCSdetails.RequestReceivedate = oldversionRFQdetails.RequestReceivedate;
                oldCSdetails.QuoteExpectdate = oldversionRFQdetails.QuoteExpectdate;
                oldCSdetails.TypeOfSolution = oldversionRFQdetails.TypeOfSolution;
                oldCSdetails.ProductType = oldversionRFQdetails.ProductType;
                oldCSdetails.Unit = oldversionRFQdetails.Unit;
                oldCSdetails.RfqCustomerSupportItems = rfqCsItemList;
                oldCSdetails.RfqCustomerSupportNotes = rfqCsnotelist;
                await _repository.UpdateRfqcsRevNo(oldCSdetails);
                // creating engg for new RFQ version
                //string serverKey = GetServerKey();
                //if (!serverKey.Equals("keus"))
                //{
                //    await _rfqenggRepository.UpdateRfqEnggRev(oldversionRFQdetails.RfqNumber, oldversionRFQdetails.RevisionNumber, oldversionRFQdetails);
                //    _rfqenggRepository.SaveAsync();
                //}
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateRfqCustomerSupportForKeus API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateRfqCustomerSupportForKeus API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //public async Task<IActionResult> UpdateRfqCustomerSupport([FromBody] RfqCustomerSupportUpdateDto rfqCustomerSupportUpdateDto)
        //{
        //    ServiceResponse<RfqCustomerSupportUpdateDto> serviceResponse = new ServiceResponse<RfqCustomerSupportUpdateDto>();
        //    try
        //    {
        //        if (rfqCustomerSupportUpdateDto is null)
        //        {
        //            _logger.LogError("RfqCustomerSupport object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Update RfqCustomerSupport object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid RfqCustomerSupport object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid Update RfqCustomerSupport object sent from client.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }

        //        Rfq oldversionRFQdetails = await _rfqRepository.RfqDetailsById(rfqCustomerSupportUpdateDto.Id);
        //        oldversionRFQdetails.RfqNumber = rfqCustomerSupportUpdateDto.RfqNumber;
        //        oldversionRFQdetails.RevisionNumber = rfqCustomerSupportUpdateDto.RevisionNumber;
        //        oldversionRFQdetails.CustomerId = rfqCustomerSupportUpdateDto.CustomerId;
        //        oldversionRFQdetails.CustomerName = rfqCustomerSupportUpdateDto.CustomerName;
        //        oldversionRFQdetails.CustomerAliasName = rfqCustomerSupportUpdateDto.CustomerAliasName;
        //        oldversionRFQdetails.CustomerRfqNumber = rfqCustomerSupportUpdateDto.CustomerRfqNumber;
        //        oldversionRFQdetails.RequestReceivedate = rfqCustomerSupportUpdateDto.RequestReceivedate;
        //        oldversionRFQdetails.QuoteExpectdate = rfqCustomerSupportUpdateDto.QuoteExpectdate;
        //        oldversionRFQdetails.TypeOfSolution = rfqCustomerSupportUpdateDto.TypeOfSolution;
        //        oldversionRFQdetails.ProductType = rfqCustomerSupportUpdateDto.ProductType;
        //        oldversionRFQdetails.Remarks = rfqCustomerSupportUpdateDto.Remarks;
        //        oldversionRFQdetails.ReasonForModification = rfqCustomerSupportUpdateDto.ReasonForModification;
        //        oldversionRFQdetails.RevisionNumber = oldversionRFQdetails.RevisionNumber + 1;

        //        var oldRFQcsReleasedItems = await _itemRepository.RfqCsReleasedItemList(rfqCustomerSupportUpdateDto.RfqNumber);
        //        var updatedItems = new List<RfqCustomerSupportItemUpdateDto>();
        //        int flag = 0;
        //        foreach (var itemList in rfqCustomerSupportUpdateDto.RfqCustomerSupportItems)
        //        {
        //            bool releaseItem = oldRFQcsReleasedItems.Any(item => item == itemList.Id);
        //            if (releaseItem)
        //            {
        //                itemList.ReleaseStatus = true;
        //                flag = 1;
        //            }
        //            else
        //            {
        //                itemList.ReleaseStatus = false;
        //                oldversionRFQdetails.IsSourcing = false;
        //                oldversionRFQdetails.IsCsComplete = false;
        //                oldversionRFQdetails.CsComplete = CsStatus.CsNotYetCompleted;
        //                if (flag == 0)
        //                {
        //                    oldversionRFQdetails.IsCsRelease = CsRelease.NotYetReleased;
        //                    oldversionRFQdetails.IsEnggRelease = CsRelease.NotYetReleased;
        //                }
        //                else
        //                {
        //                    oldversionRFQdetails.IsCsRelease = CsRelease.PartiallyRelease;
        //                    oldversionRFQdetails.IsEnggRelease = CsRelease.PartiallyRelease;
        //                }
        //                oldversionRFQdetails.IsEnggComplete = false;
        //                oldversionRFQdetails.EnggComplete = EnggStatus.EnggNotYetCompleted;
        //            }
        //            updatedItems.Add(itemList);
        //        }
        //        oldversionRFQdetails.Id = 0;
        //        _rfqRepository.Create(oldversionRFQdetails);
        //        rfqCustomerSupportUpdateDto.RfqCustomerSupportItems = null;
        //        rfqCustomerSupportUpdateDto.RfqCustomerSupportItems = updatedItems;

        //        var rfqCSItemDto = rfqCustomerSupportUpdateDto.RfqCustomerSupportItems;
        //        var rfqCsItemList = new List<RfqCustomerSupportItems>();
        //        if (rfqCSItemDto != null)
        //        {
        //            for (int i = 0; i < rfqCSItemDto.Count; i++)
        //            {
        //                RfqCustomerSupportItems rfqCSItemDetail = _mapper.Map<RfqCustomerSupportItems>(rfqCSItemDto[i]);
        //                rfqCSItemDetail.RfqCSDeliverySchedule = _mapper.Map<List<RfqCSDeliverySchedule>>(rfqCSItemDto[i].RfqCSDeliverySchedule);
        //                rfqCSItemDetail.Id = 0;
        //                rfqCsItemList.Add(rfqCSItemDetail);
        //            }
        //        }

        //        var rfqCSnotedto = rfqCustomerSupportUpdateDto.RfqCustomerSupportNotes;
        //        var rfqCsnotelist = new List<RfqCustomerSupportNotes>();
        //        if (rfqCSnotedto != null)
        //        {
        //            for (int i = 0; i < rfqCSnotedto.Count; i++)
        //            {
        //                RfqCustomerSupportNotes rfqCSnoteDetail = _mapper.Map<RfqCustomerSupportNotes>(rfqCSnotedto[i]);
        //                rfqCsnotelist.Add(rfqCSnoteDetail);
        //            }
        //        }

        //        RfqCustomerSupport oldCSdetails = await _repository.GetRfqCustomerSupportDetailsbyrfqnumber(rfqCustomerSupportUpdateDto.RfqNumber);
        //        oldCSdetails.Id = 0;
        //        oldCSdetails.CustomerName = oldversionRFQdetails.CustomerName;
        //        oldCSdetails.RevisionNumber = oldversionRFQdetails.RevisionNumber;
        //        oldCSdetails.CustomerAliasName = oldversionRFQdetails.CustomerAliasName;
        //        oldCSdetails.CustomerRfqNumber = oldversionRFQdetails.CustomerRfqNumber;
        //        oldCSdetails.RequestReceivedate = oldversionRFQdetails.RequestReceivedate;
        //        oldCSdetails.QuoteExpectdate = oldversionRFQdetails.QuoteExpectdate;
        //        oldCSdetails.TypeOfSolution = oldversionRFQdetails.TypeOfSolution;
        //        oldCSdetails.ProductType = oldversionRFQdetails.ProductType;
        //        oldCSdetails.Unit = oldversionRFQdetails.Unit;
        //        oldCSdetails.RfqCustomerSupportItems = rfqCsItemList;
        //        oldCSdetails.RfqCustomerSupportNotes = rfqCsnotelist;


        //        await _repository.UpdateRfqcsRevNo(oldCSdetails);

        //        // creating engg for new RFQ version
        //        //string serverKey = GetServerKey();
        //        //if (!serverKey.Equals("keus"))
        //        //{
        //        //    await _rfqenggRepository.UpdateRfqEnggRev(oldversionRFQdetails.RfqNumber, oldversionRFQdetails.RevisionNumber, oldversionRFQdetails);
        //        //    _rfqenggRepository.SaveAsync();
        //        //}

        //        _repository.SaveAsync();
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Updated Successfully";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside UpdateRfqCustomerSupport action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
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
                _logger.LogError($"Error Occured in DeleteRfq API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteRfq API for the following id:{id} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in DeleteRfqengg API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteRfqengg API for the following id:{id} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in DeleteRfqLPCosting API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteRfqLPCosting API for the following id:{id} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in DeleteRfqCustomerSupport API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteRfqCustomerSupport API for the following id:{id} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetRfqReleaseLpByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqReleaseLpByRfqNumber API for the following RfqNumber:{RfqNumber} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in BulkRelease API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in BulkRelease API : \n {ex.Message}";
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
                _logger.LogError($"Error Occured in UpdateRfqEnggItemUnRelease API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateRfqEnggItemUnRelease API for the following id:{id} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in UpdateRfqRfqCustomerSupportItemUnRelease API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateRfqRfqCustomerSupportItemUnRelease API for the following id:{id} \n {ex.Message} ";
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
                _logger.LogError($"Error Occured in GetAllRfqCustomGroup API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllRfqCustomGroup API : \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetRfqCustomGroupById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqCustomGroupById API for the following id:{id} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllCustomGroupList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllCustomGroupList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // POST: api/<RfqCustomGroupController>
        [HttpPost]
        public IActionResult CreateRfqCustomGroup([FromBody] RfqCustomGroupPostDto rfqCustomGroupPostDto)
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
                var cteateRfqCustomGroup = _mapper.Map<RfqCustomGroup>(rfqCustomGroupPostDto);

                //foreach (var customGroupdetails in cteateRfqCustomGroup)
                //{

                    _rfqCustomGroupRepository.CreateRfqCustomGroup(cteateRfqCustomGroup);

                //}

                _rfqCustomGroupRepository.SaveAsync();
                serviceResponse.Message = "RfqCustomGroup Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateRfqCustomGroup API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateRfqCustomGroup API : \n {ex.Message} \n{ex.InnerException}");
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

                serviceResponse.Message = $"Error Occured in UpdateRfqCustomGroup API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateRfqCustomGroup API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
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
                serviceResponse.Message = $"Error Occured in DeleteRfqCustomGroup API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteRfqCustomGroup API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
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
                _logger.LogError($"Error Occured in GetAllRfqCustomField API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllRfqCustomField API : \n {ex.Message}";
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
                         _logger.LogError($"Error Occured in GetRfqCustomFieldById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Error Occured in GetRfqCustomFieldById API for the following id:{id} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetRfqCustomFieldByCustomGroup API for the following CustomGroup:{CustomGroup} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqCustomFieldByCustomGroup API for the following CustomGroup:{CustomGroup} \n {ex.Message}";
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
                        serviceResponse.Message = $"Error Occured in CreateRfqCustomField API : \n {ex.Message} ";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        _logger.LogError($"Error Occured in CreateRfqCustomField API : \n {ex.Message} \n{ex.InnerException}");
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
                        serviceResponse.Message = $"Error Occured in UpdateRfqCustomField API for the following id:{id} \n {ex.Message}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        _logger.LogError($"Error Occured in UpdateRfqCustomField API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
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
                        serviceResponse.Message = $"Error Occured in DeleteRfqCustomField API for the following id:{id} \n {ex.Message}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        _logger.LogError($"Error Occured in DeleteRfqCustomField API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                        return StatusCode(500, serviceResponse);
                    }
                }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetRfqSPReportForTras(RfqSpReportDto rfqSpReportDto)

        {
            ServiceResponse<IEnumerable<RfqSPReportForTras>> serviceResponse = new ServiceResponse<IEnumerable<RfqSPReportForTras>>();
            try
            {
                var products = await _rfqRepository.GetRfqSPReportForTras(rfqSpReportDto.CustomerName, rfqSpReportDto.CustomerId, rfqSpReportDto.RfqNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqSPReportForKeus hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqSPReportForKeus hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned RfqSPReportForKeus Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqSPReportForTras API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqSPReportForTras API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetRfqSPReportWithDateForTras([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<RfqSPReportForTras>> serviceResponse = new ServiceResponse<IEnumerable<RfqSPReportForTras>>();
            try
            {
                var products = await _rfqRepository.GetRfqSPReportWithDateForTras(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqSPReportForKeus hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqSPReportForKeus hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned RfqSPReportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqSPReportWithDateForTras API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqSPReportWithDateForTras API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetRfqSPReport(RfqSpReportDto rfqSpReportDto)

        {
            ServiceResponse<IEnumerable<RfqSPReport>> serviceResponse = new ServiceResponse<IEnumerable<RfqSPReport>>();
            try
            {
                var products = await _rfqRepository.GetRfqSPReport(rfqSpReportDto.CustomerName,rfqSpReportDto.CustomerId,rfqSpReportDto.RfqNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GetRfqSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqSPReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqSPReport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetRfqSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<RfqSPReport>> serviceResponse = new ServiceResponse<IEnumerable<RfqSPReport>>();
            try
            {
                var products = await _rfqRepository.GetRfqSPReportWithDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RfqSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RfqSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned RfqSPReportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRfqSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost] 
        public async Task<IActionResult> GetRFQSalesorderConfirmationSPReportWithParamForTrans([FromBody] RFQSalesorderConfirmationSPReportDTO salesOrderSPResport)

        {
            ServiceResponse<IEnumerable<RFQSalesorderConfirmationSPReport>> serviceResponse = new ServiceResponse<IEnumerable<RFQSalesorderConfirmationSPReport>>();
            try
            {
                var products = await _rfqRepository.GetRFQSalesorderConfirmationSPReportWithParamForTrans(salesOrderSPResport.CustomerName, salesOrderSPResport.SalesOrderNumber, 
                                                                                                        salesOrderSPResport.KPN, salesOrderSPResport.SOStatus,salesOrderSPResport.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RFQSalesorderConfirmation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RFQSalesorderConfirmation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned RFQSalesorderConfirmationSPReportWithParamForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRFQSalesorderConfirmationSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRFQSalesorderConfirmationSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] 
        public async Task<IActionResult> GetRFQSalesorderConfirmationSPReportWithDateForTrans([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<RFQSalesorderConfirmationSPReport>> serviceResponse = new ServiceResponse<IEnumerable<RFQSalesorderConfirmationSPReport>>();
            try
            {
                var products = await _rfqRepository.GetRFQSalesorderConfirmationSPReportWithDateForTrans(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RFQSalesorderConfirmation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"RFQSalesorderConfirmation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GetRFQSalesorderConfirmationSPReportWithDateForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetRFQSalesorderConfirmationSPReportWithDateForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRFQSalesorderConfirmationSPReportWithDateForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost]
        public async Task<IActionResult> ExportRFQSalesorderConfirmationSPReportToExcel([FromBody] RFQSalesorderConfirmationSPReportDTO salesOrderSPResport)
        {
            try
            {

                var salesOrderConfirmationDetails = await _rfqRepository.GetRFQSalesorderConfirmationSPReportWithParamForTrans(salesOrderSPResport.CustomerName, salesOrderSPResport.SalesOrderNumber,
                                                                                                        salesOrderSPResport.KPN, salesOrderSPResport.SOStatus, salesOrderSPResport.ProjectNumber);

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("RFQSalesorderConfirmation");

                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Sales Order Number");
                headerRow.CreateCell(1).SetCellValue("SO Status");
                headerRow.CreateCell(2).SetCellValue("Project Number");
                headerRow.CreateCell(3).SetCellValue("Customer ID");
                headerRow.CreateCell(4).SetCellValue("Customer Name");
                headerRow.CreateCell(5).SetCellValue("Lead ID");
                headerRow.CreateCell(6).SetCellValue("Order Type");
                headerRow.CreateCell(7).SetCellValue("Type Of Solution");
                headerRow.CreateCell(8).SetCellValue("Product Type");
                headerRow.CreateCell(9).SetCellValue("Material Group");
                headerRow.CreateCell(10).SetCellValue("Item Type");
                headerRow.CreateCell(11).SetCellValue("Sales Person");
                headerRow.CreateCell(12).SetCellValue("SO Date");
                headerRow.CreateCell(13).SetCellValue("KPN");
                headerRow.CreateCell(14).SetCellValue("KPN Description");
                headerRow.CreateCell(15).SetCellValue("UOC");
                headerRow.CreateCell(16).SetCellValue("UOM");
                headerRow.CreateCell(17).SetCellValue("Price List");
                headerRow.CreateCell(18).SetCellValue("Unit Price");
                headerRow.CreateCell(19).SetCellValue("Basic Amount");
                headerRow.CreateCell(20).SetCellValue("Discount Type");
                headerRow.CreateCell(21).SetCellValue("Discount");
                headerRow.CreateCell(22).SetCellValue("SGST");
                headerRow.CreateCell(23).SetCellValue("CGST");
                headerRow.CreateCell(24).SetCellValue("IGST");
                headerRow.CreateCell(25).SetCellValue("UTGST");
                headerRow.CreateCell(26).SetCellValue("Item Price List");
                headerRow.CreateCell(27).SetCellValue("Total Amount");
                headerRow.CreateCell(28).SetCellValue("Order Qty");
                headerRow.CreateCell(29).SetCellValue("Dispatch Qty");
                headerRow.CreateCell(30).SetCellValue("Balance Qty");
                headerRow.CreateCell(31).SetCellValue("Requested Date");
                headerRow.CreateCell(32).SetCellValue("Confirmation Date");
                headerRow.CreateCell(33).SetCellValue("Confirmation Qty");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in salesOrderConfirmationDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.SalesOrderNumber);
                    row.CreateCell(1).SetCellValue(item.SOStatus.HasValue ? Enum.GetName(typeof(OrderStatus), item.SOStatus) : "");
                    row.CreateCell(2).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(3).SetCellValue(item.CustomerId);
                    row.CreateCell(4).SetCellValue(item.CustomerName);
                    row.CreateCell(5).SetCellValue(item.LeadId);
                    row.CreateCell(6).SetCellValue(item.OrderType);
                    row.CreateCell(7).SetCellValue(item.TypeOfSolution);
                    row.CreateCell(8).SetCellValue(item.ProductType);
                    row.CreateCell(9).SetCellValue(item.MaterialGroup);
                    row.CreateCell(10).SetCellValue(item.ItemType ?? "");
                    row.CreateCell(11).SetCellValue(item.SalesPerson);
                    row.CreateCell(12).SetCellValue(item.SODate.HasValue ? item.SODate.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(13).SetCellValue(item.KPN);
                    row.CreateCell(14).SetCellValue(item.KPNDescription);
                    row.CreateCell(15).SetCellValue(item.UOC);
                    row.CreateCell(16).SetCellValue(item.UOM);
                    row.CreateCell(17).SetCellValue(item.PriceList);
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(item.UnitPrice));
                    row.CreateCell(19).SetCellValue(Convert.ToDouble(item.BasicAmount));
                    row.CreateCell(20).SetCellValue(item.DiscountType);
                    row.CreateCell(21).SetCellValue(item.Discount);
                    row.CreateCell(22).SetCellValue(Convert.ToDouble(item.SGST));
                    row.CreateCell(23).SetCellValue(Convert.ToDouble(item.CGST));
                    row.CreateCell(24).SetCellValue(Convert.ToDouble(item.IGST));
                    row.CreateCell(25).SetCellValue(Convert.ToDouble(item.UTGST));
                    row.CreateCell(26).SetCellValue(Convert.ToDouble(item.ItemPriceList));
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.TotalAmount));
                    row.CreateCell(28).SetCellValue(Convert.ToDouble(item.OrderQty));
                    row.CreateCell(29).SetCellValue(Convert.ToDouble(item.DispatchQty));
                    row.CreateCell(30).SetCellValue(Convert.ToDouble(item.BalanceQty));
                    row.CreateCell(31).SetCellValue(item.RequestedDate.HasValue ? item.RequestedDate.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(32).SetCellValue(item.ConfirmationDate.HasValue ? item.ConfirmationDate.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(33).SetCellValue(Convert.ToDouble(item.ConfirmationQty));
                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RFQSalesorderConfirmationReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"error occurred in ExportRFQSalesorderConfirmationSPReportToExcel API: {ex.Message}");
            }
        }


    }

}

