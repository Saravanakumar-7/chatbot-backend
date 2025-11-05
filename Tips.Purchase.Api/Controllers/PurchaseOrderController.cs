using System;
using System.Buffers.Text;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Azure;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Helper;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities.Enums;
using Tips.Purchase.Api.Repository;
using static NPOI.HSSF.Util.HSSFColor;
using EmailIDsDto = Tips.Purchase.Api.Entities.Dto.EmailIDsDto;
using EmailTemplateDto = Tips.Purchase.Api.Entities.DTOs.EmailTemplateDto;
//using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Tips.Purchase.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PurchaseOrderController : ControllerBase
    {
        private IPurchaseOrderRepository _repository;
        private IPoItemsRepository _poItemsRepository;
        private IPurchaseRequisitionRepository _purchaseRequisitionRepository;
        private IPoConfirmationDateHistoryRepository _poConfirmationDateHistoryRepository;
        private IPoConfirmationHistoryRepository _poConfirmationHistoryRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IDocumentUploadRepository _documentUploadRepository;
        private IPoConfirmationDateRepository _poConfirmationDateRepository;
        private IPRItemsDocumentUploadRepository _pRItemsDocumentUploadRepository;
        private IConfiguration _config;
        private IPrItemsRepository _purchaseRequisitionItemRepository;
        private IPoAddprojectRepository _poAddprojectRepository;
        private readonly IHttpClientFactory _clientFactory;
        private IPoItemHistoryRepository _poItemHistoryRepository;
        private IPOInitialConfirmationDateHistoryRepository _poInitialConfirmationDateHistoryRepository;
        public static IWebHostEnvironment _webHostEnvironment { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        private readonly HttpClient _httpClient;
        public PurchaseOrderController(IPOInitialConfirmationDateHistoryRepository poInitialConfirmationDateHistoryRepository, IPrItemsRepository purchaseRequisitionItemRepository, IPoItemHistoryRepository poItemHistoryRepository, IHttpClientFactory clientFactory, HttpClient httpClient, IPRItemsDocumentUploadRepository pRItemsDocumentUploadRepository, IHttpContextAccessor httpContextAccessor, IPoConfirmationDateRepository poConfirmationDateRepository, IPurchaseRequisitionRepository purchaseRequisitionRepository, IPoConfirmationHistoryRepository poConfirmationHistoryRepository, IPoConfirmationDateHistoryRepository poConfirmationDateHistoryRepository, IPurchaseOrderRepository repository, IWebHostEnvironment webHostEnvironment, IPoItemsRepository poItemsRepository, IPoAddprojectRepository poAddprojectRepository, IDocumentUploadRepository documentUploadRepository, ILoggerManager logger, IMapper mapper, IConfiguration config)
        {
            _repository = repository;
            _httpClient = httpClient;
            _poItemsRepository = poItemsRepository;
            _purchaseRequisitionRepository = purchaseRequisitionRepository;
            _logger = logger;
            _mapper = mapper;
            _documentUploadRepository = documentUploadRepository;
            _webHostEnvironment = webHostEnvironment;
            _poConfirmationDateHistoryRepository = poConfirmationDateHistoryRepository;
            _poConfirmationHistoryRepository = poConfirmationHistoryRepository;
            _poConfirmationDateRepository = poConfirmationDateRepository;
            _pRItemsDocumentUploadRepository = pRItemsDocumentUploadRepository;
            _purchaseRequisitionItemRepository = purchaseRequisitionItemRepository;
            _poAddprojectRepository = poAddprojectRepository;
            _poItemHistoryRepository = poItemHistoryRepository;
            _poInitialConfirmationDateHistoryRepository = poInitialConfirmationDateHistoryRepository;
            _config = config;
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<PurchaseOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderDto>>();
            try
            {
                var purchaseOrderDetails = await _repository.GetAllPurchaseOrders(pagingParameter, searchParamess);
                var metadata = new
                {
                    purchaseOrderDetails.TotalCount,
                    purchaseOrderDetails.PageSize,
                    purchaseOrderDetails.CurrentPage,
                    purchaseOrderDetails.HasNext,
                    purchaseOrderDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all PurchaseOrder");
                var result = _mapper.Map<IEnumerable<PurchaseOrderDto>>(purchaseOrderDetails);

                List<DocumentUploadDto> documentUploadDtos = new List<DocumentUploadDto>();

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPurchaseOrders API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPurchaseOrders API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetLatestPurchaseOrdersByPONumbers([FromBody] List<string> Ponumbers)
        {
            ServiceResponse<List<PurchaseOrder>> serviceResponse = new ServiceResponse<List<PurchaseOrder>>();
            try
            {
                var purchaseOrderDetails = await _repository.GetLatestPurchaseOrdersByPONumbers(Ponumbers);

                purchaseOrderDetails.ForEach(x => x.POItems.ForEach(z => { z.PurchaseOrder = null; z.POAddprojects.ForEach(c => c.POItemDetail = null); }));

                serviceResponse.Data = purchaseOrderDetails;
                serviceResponse.Message = "Returned all PurchaseOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetLatestPurchaseOrdersByPONumbers API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetLatestPurchaseOrdersByPONumbers API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrderTillPoBreakDownByPoNumber(string PONumber)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();
            try
            {
                var latestPo = await _repository.GetLastestPurchaseOrderByPONumber(PONumber);
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned all PurchaseOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderTillPoBreakDownByPoNumber API for the following PONumber:{PONumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderTillPoBreakDownByPoNumber API for the following PONumber:{PONumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestPODetialsByItemNumber(string itemNumber)
        {
            ServiceResponse<IEnumerable<LatestPODetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<LatestPODetailsDto>>();
            try
            {
                var latestPoDetails = await _repository.GetLatestPODetialsByItemNumber(itemNumber);
                _logger.LogInfo($"Returned Latest PurchaseOrder Details for ItemNumber: {itemNumber}");
                serviceResponse.Data = latestPoDetails;
                serviceResponse.Message = "Returned all Latest PurchaseOrderDetails";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetLatestPODetialsByItemNumber API for the following PONumber:{itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetLatestPODetialsByItemNumber API for the following PONumber:{itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLastestPurchaseOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<PurchaseOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderDto>>();
            try
            {
                var lastestPurchaseOrderDetails = await _repository.GetAllLastestPurchaseOrders(pagingParameter, searchParamess);
                var metadata = new
                {
                    lastestPurchaseOrderDetails.TotalCount,
                    lastestPurchaseOrderDetails.PageSize,
                    lastestPurchaseOrderDetails.CurrentPage,
                    lastestPurchaseOrderDetails.HasNext,
                    lastestPurchaseOrderDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all LastestPurchaseOrders");
                var result = _mapper.Map<IEnumerable<PurchaseOrderDto>>(lastestPurchaseOrderDetails);

                List<DocumentUploadDto> documentUploadDtos = new List<DocumentUploadDto>();

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LastestPurchaseOrders";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLastestPurchaseOrders API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLastestPurchaseOrders API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //get details by ponumber
        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrderByPoNumber(string PONumber)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();
            try
            {
                var purchaseOrderDetailbyPONumber = await _repository.GetPurchaseOrderByPONumber(PONumber);

                if (purchaseOrderDetailbyPONumber == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder  hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder with id: {PONumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {PONumber}");

                    PurchaseOrderDto purchaseOrderDto = _mapper.Map<PurchaseOrderDto>(purchaseOrderDetailbyPONumber);
                    List<PoItemsDto> poItemDtoList = new List<PoItemsDto>();
                    var poIncoTermList = _mapper.Map<IEnumerable<PoIncoTerm>>(purchaseOrderDto.POIncoTerms);
                    var poAdditionalChargeList = _mapper.Map<IEnumerable<PurchaseOrderAdditionalCharges>>(purchaseOrderDto.PurchaseOrderAdditionalCharges);
                    //List<DocumentUploadDto> documentUplaodDtoList = new List<DocumentUploadDto>();

                    //if (purchaseOrderDto.POFiles.Count() != 0)
                    //{
                    //    foreach (var documentUploadDetails in purchaseOrderDto.POFiles)
                    //    {
                    //        DocumentUploadDto poItemDtos = _mapper.Map<DocumentUploadDto>(documentUploadDetails);
                    //        documentUplaodDtoList.Add(poItemDtos);
                    //    }
                    //}
                    //purchaseOrderDto.POFiles = documentUplaodDtoList;

                    var poIncoTermDto = purchaseOrderDto.POIncoTerms;

                    var poIncoTermsList = new List<PoIncoTermDto>();
                    if (poIncoTermDto != null)
                    {
                        for (int i = 0; i < poIncoTermDto.Count; i++)
                        {
                            PoIncoTermDto poIncoTermDetails = _mapper.Map<PoIncoTermDto>(poIncoTermDto[i]);
                            poIncoTermsList.Add(poIncoTermDetails);
                        }
                    }
                    purchaseOrderDto.POIncoTerms = poIncoTermsList;

                    var poAdditionalChargesDto = purchaseOrderDto.PurchaseOrderAdditionalCharges;

                    var poAdditionalChargesList = new List<PurchaseOrderAdditionalChargesDto>();
                    if (poAdditionalChargesDto != null)
                    {
                        for (int i = 0; i < poAdditionalChargesDto.Count; i++)
                        {
                            PurchaseOrderAdditionalChargesDto poAdditionalChargeDetails = _mapper.Map<PurchaseOrderAdditionalChargesDto>(poAdditionalChargesDto[i]);
                            poAdditionalChargesList.Add(poAdditionalChargeDetails);
                        }
                    }
                    purchaseOrderDto.PurchaseOrderAdditionalCharges = poAdditionalChargesList;

                    if (purchaseOrderDetailbyPONumber.POItems != null)
                    {
                        foreach (var poItemDetails in purchaseOrderDetailbyPONumber.POItems)
                        {
                            PoItemsDto poItemDtos = _mapper.Map<PoItemsDto>(poItemDetails);
                            poItemDtos.POAddprojects = _mapper.Map<List<PoAddProjectDto>>(poItemDetails.POAddprojects);
                            poItemDtos.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliveryScheduleDto>>(poItemDetails.POAddDeliverySchedules);
                            poItemDtos.POSpecialInstructions = _mapper.Map<List<PoSpecialInstructionDto>>(poItemDetails.POSpecialInstructions);
                            poItemDtos.POConfirmationDates = _mapper.Map<List<PoConfirmationDateDto>>(poItemDetails.POConfirmationDates);
                            poItemDtos.PrDetails = _mapper.Map<List<PrDetailsDto>>(poItemDetails.PrDetails);
                            poItemDtoList.Add(poItemDtos);
                        }
                    }

                    purchaseOrderDto.POItems = poItemDtoList;
                    serviceResponse.Data = purchaseOrderDto;
                    serviceResponse.Message = "Returned PurchaseOrderByPONumber Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderByPoNumber API for the following PONumber:{PONumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderByPoNumber API for the following PONumber:{PONumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }


        }
        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrderItemsByPoNumber(string PONumber)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();
            try
            {
                var purchaseOrderDetailbyPONumber = await _repository.GetPurchaseOrderItemsByPONumber(PONumber);

                if (purchaseOrderDetailbyPONumber == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder  hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder with id: {PONumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {PONumber}");

                    PurchaseOrderDto purchaseOrderDto = _mapper.Map<PurchaseOrderDto>(purchaseOrderDetailbyPONumber);
                    List<PoItemsDto> poItemDtoList = new List<PoItemsDto>();
                    var poIncoTermList = _mapper.Map<IEnumerable<PoIncoTerm>>(purchaseOrderDto.POIncoTerms);
                    //List<DocumentUploadDto> documentUplaodDtoList = new List<DocumentUploadDto>();

                    //if (purchaseOrderDto.POFiles.Count() != 0)
                    //{
                    //    foreach (var documentUploadDetails in purchaseOrderDto.POFiles)
                    //    {
                    //        DocumentUploadDto poItemDtos = _mapper.Map<DocumentUploadDto>(documentUploadDetails);
                    //        documentUplaodDtoList.Add(poItemDtos);
                    //    }
                    //}
                    //purchaseOrderDto.POFiles = documentUplaodDtoList;

                    var poIncoTermDto = purchaseOrderDto.POIncoTerms;

                    var poIncoTermsList = new List<PoIncoTermDto>();
                    if (poIncoTermDto != null)
                    {
                        for (int i = 0; i < poIncoTermDto.Count; i++)
                        {
                            PoIncoTermDto poIncoTermDetails = _mapper.Map<PoIncoTermDto>(poIncoTermDto[i]);
                            poIncoTermsList.Add(poIncoTermDetails);
                        }
                    }
                    purchaseOrderDto.POIncoTerms = poIncoTermsList;

                    var poAdditionalChargesDto = purchaseOrderDto.PurchaseOrderAdditionalCharges;

                    var poAdditionalChargesList = new List<PurchaseOrderAdditionalChargesDto>();
                    if (poAdditionalChargesDto != null)
                    {
                        for (int i = 0; i < poAdditionalChargesDto.Count; i++)
                        {
                            PurchaseOrderAdditionalChargesDto poAdditionalChargeDetails = _mapper.Map<PurchaseOrderAdditionalChargesDto>(poAdditionalChargesDto[i]);
                            poAdditionalChargesList.Add(poAdditionalChargeDetails);
                        }
                    }
                    purchaseOrderDto.PurchaseOrderAdditionalCharges = poAdditionalChargesList;

                    if (purchaseOrderDetailbyPONumber.POItems != null)
                    {
                        foreach (var poItemDetails in purchaseOrderDetailbyPONumber.POItems)
                        {
                            PoItemsDto poItemDtos = _mapper.Map<PoItemsDto>(poItemDetails);
                            poItemDtos.POAddprojects = _mapper.Map<List<PoAddProjectDto>>(poItemDetails.POAddprojects);
                            poItemDtos.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliveryScheduleDto>>(poItemDetails.POAddDeliverySchedules);
                            poItemDtos.POSpecialInstructions = _mapper.Map<List<PoSpecialInstructionDto>>(poItemDetails.POSpecialInstructions);
                            poItemDtos.POConfirmationDates = _mapper.Map<List<PoConfirmationDateDto>>(poItemDetails.POConfirmationDates);
                            poItemDtos.PrDetails = _mapper.Map<List<PrDetailsDto>>(poItemDetails.PrDetails);
                            poItemDtoList.Add(poItemDtos);
                        }
                    }

                    purchaseOrderDto.POItems = poItemDtoList;
                    serviceResponse.Data = purchaseOrderDto;
                    serviceResponse.Message = "Returned GetPurchaseOrderItemsByPoNumber Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderItemsByPoNumber API for the following PONumber:{PONumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderItemsByPoNumber API for the following PONumber:{PONumber} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }


        }

        [HttpGet("{PONumber}")]
        public async Task<IActionResult> GetAllRevisionNumberListByPoNumber(string PONumber)
        {
            ServiceResponse<IEnumerable<PurchaseOrderRevNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderRevNoListDto>>();
            try
            {
                var revNumberDetailsbyPONumber = await _repository.GetAllRevisionNumberListByPoNumber(PONumber);
                var result = _mapper.Map<IEnumerable<PurchaseOrderRevNoListDto>>(revNumberDetailsbyPONumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RevisionNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllRevisionNumberListByPoNumber API for the following PONumber:{PONumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllRevisionNumberListByPoNumber API for the following PONumber:{PONumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenPoDetails(string itemNumber)
        {
            ServiceResponse<IEnumerable<OpenPurchaseOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenPurchaseOrderDto>>();
            try
            {
                var revNumberDetailsbyPONumber = await _poItemsRepository.GetOpenPODetailsByItem(itemNumber);
                var result = _mapper.Map<IEnumerable<OpenPurchaseOrderDto>>(revNumberDetailsbyPONumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Open PurchaseOrder Details";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOpenPoDetails API for the following itemNumber:{itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOpenPoDetails API for the following itemNumber:{itemNumber} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //get only open TG Parts
        [HttpGet]
        public async Task<IActionResult> GetAllOpenTGPoDetails(string itemNumber)
        {
            ServiceResponse<IEnumerable<OpenPurchaseOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenPurchaseOrderDto>>();
            try
            {
                var revNumberDetailsbyPONumber = await _poItemsRepository.GetOpenPOTGDetailsByItem(itemNumber);
                var result = _mapper.Map<IEnumerable<OpenPurchaseOrderDto>>(revNumberDetailsbyPONumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Open PurchaseOrder Details";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOpenTGPoDetails API for the following itemNumber:{itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOpenTGPoDetails API for the following itemNumber:{itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet()] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseOrderSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<PurchaseOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderSPReport>>();
            try
            {
                var products = await _repository.GetPurchaseOrderSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet()] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseOrderApprovalSPReportWithDateForTrans([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate, [FromQuery] string RecordType, [FromQuery] string Approval)
        {
            ServiceResponse<IEnumerable<PurchaseOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.GetPurchaseOrderApprovalSPReportWithDateForTrans(FromDate, ToDate, RecordType, Approval);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderApprovalSPReportWithDateForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderApprovalSPReportWithDateForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseOrderApprovalSPReportWithDate(PurchaseOrderApprovalSPReportWithDateDTO purchaseOrderApprovalSPReportWithDateDTO)
        {
            ServiceResponse<IEnumerable<PurchaseOrderApprovalSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderApprovalSPReport>>();
            try
            {
                var products = await _repository.GetPurchaseOrderApprovalSPReportWithDate(purchaseOrderApprovalSPReportWithDateDTO.FromDate, purchaseOrderApprovalSPReportWithDateDTO.ToDate,
                                                                                            purchaseOrderApprovalSPReportWithDateDTO.RecordType, purchaseOrderApprovalSPReportWithDateDTO.Approval);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderApprovalSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderApprovalSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet()] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseOrderApprovalSPReportWithDateForAvision([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate, [FromQuery] string RecordType, [FromQuery] string Approval)
        {
            ServiceResponse<IEnumerable<PurchaseOrderSPReportForAvision>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderSPReportForAvision>>();
            try
            {
                var products = await _repository.GetPurchaseOrderApprovalSPReportWithDateForAvision(FromDate, ToDate, RecordType, Approval);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderApprovalSPReportWithDateForAvision API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderApprovalSPReportWithDateForAvision API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //[HttpGet()] // Adjust your route as needed
        //public async Task<IActionResult> GetPurchaseOrderSPReportWithDateForTrans([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        //{
        //    ServiceResponse<IEnumerable<PurchaseOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderSPReportForTrans>>();
        //    try
        //    {
        //        var products = await _repository.GetPurchaseOrderSPReportWithDateForTrans(FromDate, ToDate);

        //        if (products == null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"PurchaseOrder hasn't been found.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            _logger.LogError($"PurchaseOrder hasn't been found in db.");
        //            return Ok(serviceResponse);
        //        }
        //        else
        //        {
        //            serviceResponse.Data = products;
        //            serviceResponse.Message = "Returned PurchaseOrder Details";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Ok(serviceResponse);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong inside GetPurchaseOrderSPReportWithDateForTrans action";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrderSPResport([FromQuery] PagingParameter pagingParameter)
        {

            ServiceResponse<IEnumerable<PurchaseOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderSPReport>>();

            try
            {
                var products = await _repository.GetPurchaseOrderSPResport(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all PurchaseOrderSPReport");
                var result = _mapper.Map<IEnumerable<PurchaseOrderSPReport>>(products);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseOrderSPReport Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderSPResport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderSPResport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Get_Tras_POReport_ConfirmationDate([FromQuery] PagingParameter pagingParameter)
        {

            ServiceResponse<IEnumerable<Tras_PO_ConfirmationDate>> serviceResponse = new ServiceResponse<IEnumerable<Tras_PO_ConfirmationDate>>();

            try
            {
                var products = await _repository.Get_Tras_POReport_ConfirmationDate(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all Get_Tras_POReport_ConfirmationDate");
                var result = _mapper.Map<IEnumerable<Tras_PO_ConfirmationDate>>(products);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Get_Tras_POReport_ConfirmationDate Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in Get_Tras_POReport_ConfirmationDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in Get_Tras_POReport_ConfirmationDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrderDashboardSPReportWithParam()

        {
            ServiceResponse<List<PurchaseOrderDashboardSPReport_Details>> serviceResponse = new ServiceResponse<List<PurchaseOrderDashboardSPReport_Details>>();
            try
            {
                List<PurchaseOrderDashboardSPReport_Details> purchaseOrderDashboardSPReport_Details = new List<PurchaseOrderDashboardSPReport_Details>();
                List<string> Bucket_Id = new List<string>();
                Bucket_Id.Add("bucket_Id1");
                Bucket_Id.Add("bucket_Id2");
                Bucket_Id.Add("bucket_Id3");
                Bucket_Id.Add("bucket_Id4");
                foreach (var buck in Bucket_Id)
                {
                    PurchaseOrderDashboardSPReport_Details purchaseOrderDashboardSPReport_Details1 = new PurchaseOrderDashboardSPReport_Details();
                    purchaseOrderDashboardSPReport_Details1.Title = buck;
                    purchaseOrderDashboardSPReport_Details1.Items = await _repository.GetPurchaseOrderDashboardSPReportWithParam(buck);
                    purchaseOrderDashboardSPReport_Details.Add(purchaseOrderDashboardSPReport_Details1);
                }


                if (purchaseOrderDashboardSPReport_Details == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrderDashboard hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrderDashboard hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = purchaseOrderDashboardSPReport_Details;
                    serviceResponse.Message = "Returned PurchaseOrderDashboardSPReportWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderDashboardSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderDashboardSPReportWithParam API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseOrderSPReportWithParam([FromBody] PurchaseOrderSPReportWithParamDTO purchaseOrderSPReport)

        {
            ServiceResponse<IEnumerable<PurchaseOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderSPReport>>();
            try
            {
                var products = await _repository.GetPurchaseOrderSPReportWithParam(purchaseOrderSPReport.VendorName, purchaseOrderSPReport.PONumber, purchaseOrderSPReport.ItemNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseOrderSPReportWithParamForTrans([FromBody] PurchaseOrderSPReportWithParamForTransDTO purchaseOrderSPReportWithParamForTransDTO)

        {
            ServiceResponse<IEnumerable<PurchaseOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderSPReport>>();
            try
            {
                var products = await _repository.GetPurchaseOrderSPReportWithParamForTrans(purchaseOrderSPReportWithParamForTransDTO.VendorName, purchaseOrderSPReportWithParamForTransDTO.PONumber,
                                                                                    purchaseOrderSPReportWithParamForTransDTO.ItemNumber, purchaseOrderSPReportWithParamForTransDTO.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseOrderUnitListSPReportWithParamForTrans([FromBody] PurchaseOrderUnitListSPReportWithParamForTransDTO purchaseOrderUnitListSPReportWithParamForTransDTO)

        {
            ServiceResponse<IEnumerable<PurchaseOrderUnitListSPReportWithParamForTrans>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderUnitListSPReportWithParamForTrans>>();
            try
            {
                var products = await _repository.GetPurchaseOrderUnitListSPReportWithParamForTrans(purchaseOrderUnitListSPReportWithParamForTransDTO.ItemNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrderUnitList hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrderUnitList hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseOrderUnitList Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderUnitListSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderUnitListSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseOrderApprovalSPReportWithParam([FromBody] PurchaseOrderApprovalSPReportWithParamDTO purchaseOrderApprovalSPReport)

        {
            ServiceResponse<IEnumerable<PurchaseOrderApprovalSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderApprovalSPReport>>();
            try
            {
                var products = await _repository.GetPurchaseOrderApprovalSPReportWithParam(purchaseOrderApprovalSPReport.VendorName, purchaseOrderApprovalSPReport.PONumber,
                                                                                    purchaseOrderApprovalSPReport.ItemNumber, purchaseOrderApprovalSPReport.RecordType,
                                                                                        purchaseOrderApprovalSPReport.Postatus, purchaseOrderApprovalSPReport.Approval);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseOrderApprovalSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderApprovalSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderApprovalSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseOrderApprovalSPReportWithParamForTrans([FromBody] PurchaseOrderApprovalSPReportWithParamForTransDTO purchaseOrderApprovalSPReport)

        {
            ServiceResponse<IEnumerable<PurchaseOrderSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderSPReportForTrans>>();
            try
            {
                var products = await _repository.GetPurchaseOrderApprovalSPReportWithParamForTrans(purchaseOrderApprovalSPReport.VendorName, purchaseOrderApprovalSPReport.PONumber,
                                                                                    purchaseOrderApprovalSPReport.ItemNumber, purchaseOrderApprovalSPReport.RecordType,
                                                                                        purchaseOrderApprovalSPReport.Postatus, purchaseOrderApprovalSPReport.Approval,
                                                                                        purchaseOrderApprovalSPReport.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseOrderApprovalSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderApprovalSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderApprovalSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseOrderApprovalSPReportWithParamForAvision([FromBody] PurchaseOrderApprovalSPReportWithParamForTransDTO purchaseOrderApprovalSPReport)

        {
            ServiceResponse<IEnumerable<PurchaseOrderSPReportForAvision>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderSPReportForAvision>>();
            try
            {
                var products = await _repository.GetPurchaseOrderApprovalSPReportWithParamForAvision(purchaseOrderApprovalSPReport.VendorName, purchaseOrderApprovalSPReport.PONumber,
                                                                                    purchaseOrderApprovalSPReport.ItemNumber, purchaseOrderApprovalSPReport.RecordType,
                                                                                        purchaseOrderApprovalSPReport.Postatus, purchaseOrderApprovalSPReport.Approval,
                                                                                        purchaseOrderApprovalSPReport.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseOrderApprovalSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderApprovalSPReportWithParamForAvision API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderApprovalSPReportWithParamForAvision API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetListOfOpenPOQtyByItemNoList(List<string> itemNumberList)
        {
            //openpurchaseorderdto
            ServiceResponse<IEnumerable<OpenPoQuantityDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenPoQuantityDto>>();
            try
            {
                var revNumberDetailsbyPONumber = await _poItemsRepository.GetListOfOpenPOQtyByItemNoList(itemNumberList);
                var result = _mapper.Map<IEnumerable<OpenPoQuantityDto>>(revNumberDetailsbyPONumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Open PurchaseOrder Details";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetListOfOpenPOQtyByItemNoList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetListOfOpenPOQtyByItemNoList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetListOfOpenPOQtyByItemNoListByProjectNo(string projectNo, List<string> itemNumberList)
        {
            //openpurchaseorderdto
            ServiceResponse<IEnumerable<OpenPoQuantityDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenPoQuantityDto>>();
            try
            {
                var revNumberDetailsbyPONumber = await _poItemsRepository.GetListOfOpenPOQtyByItemNoListByProjectNo(projectNo, itemNumberList);
                var result = _mapper.Map<IEnumerable<OpenPoQuantityDto>>(revNumberDetailsbyPONumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Open PurchaseOrder Details By ProjectNo and ItemNo List";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetListOfOpenPOQtyByItemNoListByProjectNo API for the following projectNo:{projectNo} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetListOfOpenPOQtyByItemNoListByProjectNo API for the following projectNo:{projectNo} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetListOfOpenPOQtyByItemNoListByMultipleProjectNo(coveragePOByMultipleProjectDto coveragePOByMultipleProjectDto)
        {
            //openpurchaseorderdto
            ServiceResponse<IEnumerable<OpenPoQuantityDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenPoQuantityDto>>();
            try
            {
                var revNumberDetailsbyPONumber = await _poItemsRepository.GetListOfOpenPOQtyByItemNoListByMultipleProjectNo(coveragePOByMultipleProjectDto.itemNumberList,
                                                                                                                                    coveragePOByMultipleProjectDto.projectNo);
                var result = _mapper.Map<IEnumerable<OpenPoQuantityDto>>(revNumberDetailsbyPONumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Open PurchaseOrder Details By ProjectNo and ItemNo List";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetListOfOpenPOQtyByItemNoListByMultipleProjectNo API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetListOfOpenPOQtyByItemNoListByMultipleProjectNo API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPRNumberandQtyListByItemNumber(string itemMaster)
        {
            ServiceResponse<IEnumerable<PRNoandQtyListDto>> serviceResponse = new ServiceResponse<IEnumerable<PRNoandQtyListDto>>();
            try
            {
                var revNumberDetailsbyPONumber = await _repository.GetPRNumberandQtyListByItemNumber(itemMaster);
                var result = _mapper.Map<IEnumerable<PRNoandQtyListDto>>(revNumberDetailsbyPONumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PRNumberandQtyList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPRNumberandQtyListByItemNumber API for the following itemMaster:{itemMaster} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPRNumberandQtyListByItemNumber API for the following itemMaster:{itemMaster} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //CoverageReport
        //[HttpGet]
        //public async Task<IActionResult> GetPODetailsByItemNo(string ItemNumber)
        //{
        //    ServiceResponse<IEnumerable<PoItem>> serviceResponse = new ServiceResponse<IEnumerable<PoItem>>();
        //    try
        //    {
        //        var poItemDetails = await _poItemsRepository.GetPODetailsByItemNo(ItemNumber);
        //        var result = _mapper.Map<IEnumerable<PoItem>>(poItemDetails);
        //        serviceResponse.Data = result;
        //        serviceResponse.Message = "Returned all PoDetails";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong inside GetPODetailsByItemNo action";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrderByPoNoAndRevNo(string PONumber, int revisionNumber)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();
            try
            {
                var purchaseOrderDetail = await _repository.GetPurchaseOrderByPONoAndRevNo(PONumber, revisionNumber);

                if (purchaseOrderDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder  hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder with id: {PONumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {PONumber}");

                    PurchaseOrderDto purchaseOrderDto = _mapper.Map<PurchaseOrderDto>(purchaseOrderDetail);
                    List<PoItemsDto> poItemDtoList = new List<PoItemsDto>();

                    //List<DocumentUploadDto> documentUplaodDtoList = new List<DocumentUploadDto>();

                    //if (purchaseOrderDto.POFiles.Count() != 0)
                    //{
                    //    foreach (var documentUploadDetails in purchaseOrderDto.POFiles)
                    //    {
                    //        DocumentUploadDto poItemDtos = _mapper.Map<DocumentUploadDto>(documentUploadDetails);
                    //        documentUplaodDtoList.Add(poItemDtos);
                    //    }
                    //}
                    //purchaseOrderDto.POFiles = documentUplaodDtoList;

                    var poIncoTermDto = purchaseOrderDto.POIncoTerms;

                    var poIncoTermsList = new List<PoIncoTermDto>();
                    if (poIncoTermDto != null)
                    {
                        for (int i = 0; i < poIncoTermDto.Count; i++)
                        {
                            PoIncoTermDto poIncoTermDetails = _mapper.Map<PoIncoTermDto>(poIncoTermDto[i]);
                            poIncoTermsList.Add(poIncoTermDetails);
                        }
                    }
                    purchaseOrderDto.POIncoTerms = poIncoTermsList;

                    var poAdditionalChargesDto = purchaseOrderDto.PurchaseOrderAdditionalCharges;

                    var poAdditionalChargesList = new List<PurchaseOrderAdditionalChargesDto>();
                    if (poAdditionalChargesDto != null)
                    {
                        for (int i = 0; i < poAdditionalChargesDto.Count; i++)
                        {
                            PurchaseOrderAdditionalChargesDto poAdditionalChargeDetails = _mapper.Map<PurchaseOrderAdditionalChargesDto>(poAdditionalChargesDto[i]);
                            poAdditionalChargesList.Add(poAdditionalChargeDetails);
                        }
                    }
                    purchaseOrderDto.PurchaseOrderAdditionalCharges = poAdditionalChargesList;

                    if (purchaseOrderDetail.POItems != null)
                    {
                        foreach (var poItemDetails in purchaseOrderDetail.POItems)
                        {
                            PoItemsDto poItemDtos = _mapper.Map<PoItemsDto>(poItemDetails);
                            poItemDtos.POAddprojects = _mapper.Map<List<PoAddProjectDto>>(poItemDetails.POAddprojects);
                            poItemDtos.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliveryScheduleDto>>(poItemDetails.POAddDeliverySchedules);
                            poItemDtos.POSpecialInstructions = _mapper.Map<List<PoSpecialInstructionDto>>(poItemDetails.POSpecialInstructions);
                            poItemDtos.POConfirmationDates = _mapper.Map<List<PoConfirmationDateDto>>(poItemDetails.POConfirmationDates);
                            poItemDtos.PrDetails = _mapper.Map<List<PrDetailsDto>>(poItemDetails.PrDetails);
                            poItemDtoList.Add(poItemDtos);
                        }
                    }

                    purchaseOrderDto.POItems = poItemDtoList;
                    serviceResponse.Data = purchaseOrderDto;
                    serviceResponse.Message = "Returned PurchaseOrderByPONoAndRevNo Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderByPoNoAndRevNo API for the following PONumber:{PONumber} and revisionNumber:{revisionNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderByPoNoAndRevNo API for the following PONumber:{PONumber} and revisionNumber:{revisionNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }


        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderById(int id)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();
            try
            {
                var purchaseOrderDetailbyId = await _repository.GetPurchaseOrderById(id);

                if (purchaseOrderDetailbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder  hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseOrder with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");

                    PurchaseOrderDto purchaseOrderDto = _mapper.Map<PurchaseOrderDto>(purchaseOrderDetailbyId);
                    List<PoItemsDto> poItemDtoList = new List<PoItemsDto>();

                    //List<DocumentUploadDto> documentUplaodDtoList = new List<DocumentUploadDto>();

                    //if (purchaseOrderDto.POFiles.Count() != 0)
                    //{
                    //    foreach (var documentUploadDetails in purchaseOrderDto.POFiles)
                    //    {
                    //        DocumentUploadDto poItemDtos = _mapper.Map<DocumentUploadDto>(documentUploadDetails);
                    //        documentUplaodDtoList.Add(poItemDtos);
                    //    }
                    //}
                    //purchaseOrderDto.POFiles = documentUplaodDtoList;

                    var poIncoTermDto = purchaseOrderDto.POIncoTerms;

                    var poIncoTermsList = new List<PoIncoTermDto>();
                    if (poIncoTermDto != null)
                    {
                        for (int i = 0; i < poIncoTermDto.Count; i++)
                        {
                            PoIncoTermDto poIncoTermDetails = _mapper.Map<PoIncoTermDto>(poIncoTermDto[i]);
                            poIncoTermsList.Add(poIncoTermDetails);
                        }
                    }
                    purchaseOrderDto.POIncoTerms = poIncoTermsList;

                    var poAdditionalChargesDto = purchaseOrderDto.PurchaseOrderAdditionalCharges;

                    var poAdditionalChargesList = new List<PurchaseOrderAdditionalChargesDto>();
                    if (poAdditionalChargesDto != null)
                    {
                        for (int i = 0; i < poAdditionalChargesDto.Count; i++)
                        {
                            PurchaseOrderAdditionalChargesDto poAdditionalChargeDetails = _mapper.Map<PurchaseOrderAdditionalChargesDto>(poAdditionalChargesDto[i]);
                            poAdditionalChargesList.Add(poAdditionalChargeDetails);
                        }
                    }
                    purchaseOrderDto.PurchaseOrderAdditionalCharges = poAdditionalChargesList;

                    if (purchaseOrderDetailbyId.POItems != null)
                    {
                        foreach (var poItemDetails in purchaseOrderDetailbyId.POItems)
                        {
                            PoItemsDto poItemDtos = _mapper.Map<PoItemsDto>(poItemDetails);

                            var poItemHistoryReceivedQty = await _poItemHistoryRepository.GetPoItemHistoryDetailsByPoItemId(poItemDtos.PONumber, poItemDtos.ItemNumber);
                            if (poItemHistoryReceivedQty != null)
                            {
                                poItemDtos.ShortClosedQty = poItemHistoryReceivedQty.Sum(x => x.ShortClosedQty);
                            }

                            poItemDtos.POAddprojects = _mapper.Map<List<PoAddProjectDto>>(poItemDtos.POAddprojects);
                            poItemDtos.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliveryScheduleDto>>(poItemDetails.POAddDeliverySchedules);
                            poItemDtos.POSpecialInstructions = _mapper.Map<List<PoSpecialInstructionDto>>(poItemDetails.POSpecialInstructions);
                            poItemDtos.POConfirmationDates = _mapper.Map<List<PoConfirmationDateDto>>(poItemDetails.POConfirmationDates);
                            poItemDtos.PrDetails = _mapper.Map<List<PrDetailsDto>>(poItemDetails.PrDetails);
                            poItemDtoList.Add(poItemDtos);
                        }
                    }

                    purchaseOrderDto.POItems = poItemDtoList;
                    serviceResponse.Data = purchaseOrderDto;
                    serviceResponse.Message = "Returned PurchaseOrderById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseOrderById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseOrderById API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //Edit upload update document apoi


        //[HttpPost]
        //public async Task<IActionResult> UploadDocument([FromBody] List<UploadDocumentDto> uploadDocumentDto)
        //{
        //    ServiceResponse<UploadDocumentDto> serviceResponse = new ServiceResponse<UploadDocumentDto>();
        //    try
        //    {
        //        if (uploadDocumentDto is null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "PurchaseOrder UploadDocument object is null.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("PurchaseOrder UploadDocument sent from client is null.");
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid PurchaseOrder UploadDocument.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("Invalid PurchaseOrder UploadDocument sent from client.");
        //            return BadRequest(serviceResponse);
        //        }

        //        //var uploadDocumentDetails = _mapper.Map<DocumentUpload>(uploadDocumentDto);
        //        //var uploadDocumentDtoList = new List<UploadDocumentDto>();

        //        foreach (var poUploadDetail in uploadDocumentDto)
        //        {
        //            var fileContent = poUploadDetail.FileByte;
        //            var poNumber = poUploadDetail.ParentNumber;
        //            string fileName = poUploadDetail.FileName + "." + poUploadDetail.FileExtension;
        //            string FileExt = Path.GetExtension(fileName).ToUpper();

        //            Guid guid = Guid.NewGuid();
        //            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PODocument", /*guid.ToString() + "_" */ fileName);
        //            using (MemoryStream ms = new MemoryStream(fileContent))
        //            {
        //                ms.Position = 0;
        //                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //                {
        //                    ms.WriteTo(fileStream);
        //                }

        //                var uploadedFile = new DocumentUpload
        //                {
        //                    FileName = fileName,
        //                    FileExtension = FileExt,
        //                    FilePath = filePath,
        //                    ParentNumber = poNumber,
        //                    DocumentFrom = "PODocument",

        //                    //PurchaseOrder = poUploadDetail.PurchaseOrder,

        //                };
        //                var poUploadDoc = _mapper.Map<DocumentUpload>(uploadedFile);

        //                await _documentUploadRepository.CreateUploadDocumentPO(poUploadDoc);
        //                _documentUploadRepository.SaveAsync();

        //            }
        //        }

        //        serviceResponse.Data = null;
        //        serviceResponse.Message = " UploadDocument Successfully Created";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside UploadDocument action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong ,try again";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}


        //image get api
        [HttpPost]

        public async Task<IActionResult> getUploadedFile([FromBody] string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PODocument", fileName);
            string FileExt = Path.GetExtension(fileName).ToUpper();
            if (System.IO.File.Exists(filePath))
            {
                byte[] a = System.IO.File.ReadAllBytes(filePath);
                return File(a, "image/" + FileExt);
            }
            return null;
        }

        //getponumber by vendorid

        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetAllPoNumberListByVendorId(string vendorId)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pONumberDetailsbyVendorId = await _repository.GetAllPONumberListByVendorId(vendorId);
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pONumberDetailsbyVendorId);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PONumberListId";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPoNumberListByVendorId API for the following vendorId:{vendorId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPoNumberListByVendorId API for the following vendorId:{vendorId} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetAllServicePoNumberListByVendorId(string vendorId)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pONumberDetailsbyVendorId = await _repository.GetAllServicePoNumberListByVendorId(vendorId);
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pONumberDetailsbyVendorId);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ServicePONumberListId";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllServicePoNumberListByVendorId API for the following vendorId:{vendorId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllServicePoNumberListByVendorId API for the following vendorId:{vendorId} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetAllNonServicePoNumberListByVendorId(string vendorId)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pONumberDetailsbyVendorId = await _repository.GetAllNonServicePoNumberListByVendorId(vendorId);
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pONumberDetailsbyVendorId);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all NonServicePONumberListId";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllNonServicePoNumberListByVendorId API for the following vendorId:{vendorId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllNonServicePoNumberListByVendorId API for the following vendorId:{vendorId} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetKIT_PoNumberListByVendorId(string vendorId)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pONumberDetailsbyVendorId = await _repository.GetKIT_PoNumberListByVendorId(vendorId);
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pONumberDetailsbyVendorId);
                serviceResponse.Data = result;
                serviceResponse.Message = $"Returned KIT_PoNumberList By VendorId: {vendorId}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                _logger.LogInfo($"Returned KIT_PoNumberList By VendorId: {vendorId}");
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetKIT_PoNumberListByVendorId for the VendorID: {vendorId}\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetKIT_PoNumberListByVendorId for the VendorID: {vendorId}\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetAllServicePoNumberListByVendorIdForAvision(string vendorId)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pONumberDetailsbyVendorId = await _repository.GetAllServicePoNumberListByVendorIdForAvision(vendorId);
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pONumberDetailsbyVendorId);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ServicePONumberListId";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllServicePoNumberListByVendorIdForAvision API for the following vendorId:{vendorId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllServicePoNumberListByVendorIdForAvision API for the following vendorId:{vendorId} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetAllNonServicePoNumberListByVendorIdForAvision(string vendorId)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pONumberDetailsbyVendorId = await _repository.GetAllNonServicePoNumberListByVendorIdForAvision(vendorId);
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pONumberDetailsbyVendorId);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all NonServicePONumberListId";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllNonServicePoNumberListByVendorIdForAvision API for the following vendorId:{vendorId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllNonServicePoNumberListByVendorIdForAvision API for the following vendorId:{vendorId} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetAllKIT_PoNumberListByVendorIdForAvision(string vendorId)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pONumberDetailsbyVendorId = await _repository.GetAllKIT_PoNumberListByVendorIdForAvision(vendorId);
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pONumberDetailsbyVendorId);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all KIT_PONumberListId";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in GetAllKIT_PoNumberListByVendorIdForAvision:\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in GetAllKIT_PoNumberListByVendorIdForAvision:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetAllPoNumberListByVendorIdForAvision(string vendorId)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pONumberDetailsbyVendorId = await _repository.GetAllPoNumberListByVendorIdForAvision(vendorId);
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pONumberDetailsbyVendorId);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PONumberListId";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPoNumberListByVendorIdForAvision API for the following vendorId:{vendorId} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPoNumberListByVendorIdForAvision API for the following vendorId:{vendorId} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //amount in words
        private string GetTotalValueInWords(decimal totalValue)
        {
            RupeesToWords a = new RupeesToWords();
            string totalValueInWords = a.words(Convert.ToDouble(totalValue), true);
            return totalValueInWords;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] PurchaseOrderPostDto purchaseOrderPostDto)
        {
            ServiceResponse<PurchaseOrderPostDto> serviceResponse = new ServiceResponse<PurchaseOrderPostDto>();
            try
            {
                string serverKey = GetServerKey();

                if (purchaseOrderPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrder object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseOrder object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }
                if (purchaseOrderPostDto.Currency != "INR" && purchaseOrderPostDto.ConvertionRateId == null && serverKey != "keus")
                {
                    serviceResponse.Message = $"Error Occured in CreatePurchaseOrder: The ConvertionRateId is required for the UOC:{purchaseOrderPostDto.Currency}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Error Occured in CreatePurchaseOrder: The ConvertionRateId is required for the UOC:{purchaseOrderPostDto.Currency}");
                    return BadRequest(serviceResponse);
                }
                var purchaseOrderDetails = _mapper.Map<PurchaseOrder>(purchaseOrderPostDto);
                var AmountInWords = GetTotalValueInWords(purchaseOrderDetails.TotalAmount);
                purchaseOrderDetails.AmountInWords = AmountInWords;
                var poItemDto = purchaseOrderPostDto.POItems;
                var prDetailsPostDto = poItemDto[0].PrDetails;
                var poFile = purchaseOrderPostDto.POFiles;
                var poItemDtoList = new List<PoItem>();
                var poAddKitProjectList = new List<PoAddKitProject>();
                var poIncoTermList = _mapper.Map<IEnumerable<PoIncoTerm>>(purchaseOrderPostDto.POIncoTerms);
                var poAdditionalChargeList = _mapper.Map<IEnumerable<PurchaseOrderAdditionalCharges>>(purchaseOrderPostDto.PurchaseOrderAdditionalCharges);
                List<EnggBomKitItemNumberWithQtyDto>? enggBomKitDetailsDynamic = new List<EnggBomKitItemNumberWithQtyDto>();

                var date = DateTime.Now;
                purchaseOrderPostDto.QuotationDate = date;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));
                if (serverKey == "avision")
                {
                    var poNum = await _repository.GeneratePONumberForAvision();
                    purchaseOrderDetails.PONumber = poNum;
                }
                else
                {
                    var dateFormat = days + months + years;
                    var poNumber = await _repository.GeneratePONumber();
                    purchaseOrderDetails.PONumber = dateFormat + poNumber;
                }
                if (poItemDto != null)
                {
                    if (purchaseOrderDetails.PoType == PoType.Kit)
                    {
                        for (int i = 0; i < poItemDto.Count; i++)
                        {
                            //Implement Kit
                            if (poItemDto[i].PartType == PoPartType.Kit)
                            {
                                PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);
                                poItemDetails.BalanceQty = poItemDto[i].Qty;
                                poItemDetails.PoPartsStatus = false;
                                poItemDetails.PONumber = purchaseOrderDetails.PONumber;
                                var poAddprojectDetails = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                                for (int j = 0; j < poAddprojectDetails.Count; j++)
                                {
                                    PoAddProject poaddproject = poAddprojectDetails[j];
                                    poaddproject.BalanceQty = poaddproject.ProjectQty;

                                    //Implement KitComponent
                                    var poAddprojectDetail = _mapper.Map<List<PoAddKitProject>>(poAddprojectDetails[j].PoAddKitProjects);
                                    for (int k = 0; k < poAddprojectDetail.Count; k++)
                                    {

                                        PoAddKitProject poAddKitProject = poAddprojectDetail[k];
                                        poAddKitProject.DrawingRevNo = poItemDetails.drawingRevNo;
                                        poAddKitProject.CreatedBy = _createdBy;
                                        poAddKitProject.CreatedOn = DateTime.Now;

                                    }
                                    poAddprojectDetails[j].PoAddKitProjects = poAddprojectDetail;

                                }
                                poItemDetails.POAddprojects = poAddprojectDetails;
                                poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);
                                poItemDetails.POSpecialInstructions = _mapper.Map<List<PoSpecialInstruction>>(poItemDto[i].POSpecialInstructions);
                                //poItemDetails.POConfirmationDates = _mapper.Map<List<PoConfirmationDate>>(poItemDto[i].POConfirmationDates);
                                poItemDetails.PrDetails = _mapper.Map<List<PrDetails>>(poItemDto[i].PrDetails);
                                poItemDtoList.Add(poItemDetails);
                            }
                            else
                            {
                                serviceResponse.Message = $"Error Occured in CreatePurchaseOrder: The PoType is kit,But PoItemDto.Parttype is not Kit:{poItemDto[i].PartType}";
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                                _logger.LogError($"Error Occured in CreatePurchaseOrder: The PoType is kit,But PoItemDto.Parttype is not Kit:{poItemDto[i].PartType}");
                                return BadRequest(serviceResponse);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < poItemDto.Count; i++)
                        {
                            if (poItemDto[i].PartType != PoPartType.Kit)
                            {
                                PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);
                                poItemDetails.BalanceQty = poItemDto[i].Qty;
                                poItemDetails.PoPartsStatus = false;
                                poItemDetails.PONumber = purchaseOrderDetails.PONumber;
                                poItemDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                                for (int j = 0; j < poItemDetails.POAddprojects.Count; j++)
                                {
                                    PoAddProject poaddproject = poItemDetails.POAddprojects[j];
                                    poaddproject.BalanceQty = poaddproject.ProjectQty;
                                }

                                poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);
                                poItemDetails.POSpecialInstructions = _mapper.Map<List<PoSpecialInstruction>>(poItemDto[i].POSpecialInstructions);
                                //poItemDetails.POConfirmationDates = _mapper.Map<List<PoConfirmationDate>>(poItemDto[i].POConfirmationDates);
                                poItemDetails.PrDetails = _mapper.Map<List<PrDetails>>(poItemDto[i].PrDetails);
                                poItemDtoList.Add(poItemDetails);
                            }
                            else
                            {
                                serviceResponse.Message = $"Error Occured in CreatePurchaseOrder: The PoItemDto.Parttype is kit,But PoType is General:{poItemDto[i].PartType}";
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                                _logger.LogError($"Error Occured in CreatePurchaseOrder: The PoItemDto.Parttype is kit,But PoType is General:{poItemDto[i].PartType}");
                                return BadRequest(serviceResponse);
                            }
                        }
                    }
                }
                purchaseOrderDetails.POItems = poItemDtoList;
                // purchaseOrderDetails.POFiles = poDocumentUploadDtoList;
                purchaseOrderDetails.POIncoTerms = poIncoTermList.ToList();
                purchaseOrderDetails.PurchaseOrderAdditionalCharges = poAdditionalChargeList.ToList();
                await _repository.CreatePurchaseOrder(purchaseOrderDetails);


                if (purchaseOrderPostDto.POItems != null)
                {
                    foreach (var pritem in purchaseOrderPostDto.POItems)
                    {
                        if (pritem.PrDetails != null)
                        {
                            foreach (var pritemdetail in pritem.PrDetails)
                            {
                                if (pritemdetail.PrDetailDocumentUploadPostDtos != null)
                                {
                                    foreach (var prDetailsDto in pritemdetail.PrDetailDocumentUploadPostDtos)
                                    {
                                        var prUploadDocument = await _pRItemsDocumentUploadRepository.GetUploadDocByFileName(prDetailsDto.FileName);
                                        if (prUploadDocument != null)
                                        {
                                            prUploadDocument.Checked = true;
                                            await _pRItemsDocumentUploadRepository.UpdateUploadDoc(prUploadDocument);
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                if (poItemDtoList != null)
                {
                    foreach (var poItems in poItemDtoList)
                    {
                        if (poItems.PrDetails != null)
                        {
                            foreach (var prDetails in poItems.PrDetails.Where(x => x.ToClosePR == true).ToList())
                            {
                                var prItemDetail = await _purchaseRequisitionItemRepository.GetPrItemByPRNo(prDetails.PRNumber, poItems.ItemNumber);
                                if (prItemDetail != null)
                                {
                                    prItemDetail.PrStatus = PrStatus.Closed;
                                    await _purchaseRequisitionItemRepository.UpdatePrItem(prItemDetail);
                                    _purchaseRequisitionItemRepository.SaveAsync();
                                }

                                var prItemClosedStatusCount = await _purchaseRequisitionItemRepository.GetPrItemClosedStatusCount(prDetails.PRNumber);
                                var prDetail = await _purchaseRequisitionRepository.GetPrDetailsByPrNumber(prDetails.PRNumber);
                                prDetail.PrStatus = prItemClosedStatusCount;
                                await _purchaseRequisitionRepository.UpdatePurchaseRequisition_ForApproval(prDetail);
                            }
                        }
                    }
                }

                _documentUploadRepository.SaveAsync();
                _pRItemsDocumentUploadRepository.SaveAsync();
                _purchaseRequisitionRepository.SaveAsync();
                _repository.SaveAsync();

                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,CreatePurchaseOrder";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);
                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    var mails = (emaildetails1.data.Where(x => x.operation == "CreatePurchaseOrder").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                    //var mails = "accounts@avisionsystems.com";
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));
                    //email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    var podate = purchaseOrderDetails.PODate.ToString().Split(" ");
                    email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));

                    email.Subject = emaildetails.data.subject;
                    string body = emaildetails.data.template;
                    body = body.Replace("{{PO Number}}", purchaseOrderDetails.PONumber);
                    body = body.Replace("{{PO Revision No}}", purchaseOrderDetails.RevisionNumber.ToString());
                    body = body.Replace("{{PO Date}}", podate[0]);
                    body = body.Replace("{{PO Value}}", purchaseOrderDetails.TotalAmount.ToString());
                    body = body.Replace("{{Vendor Name}}", purchaseOrderDetails.VendorName.ToString());
                    body = body.Replace("{{Approval1}}", "Awaiting");
                    body = body.Replace("{{Approval2}}", "Awaiting");
                    body = body.Replace("{{Approval3}}", "Awaiting");
                    body = body.Replace("{{Approval4}}", "Awaiting");
                    body = body.Replace("{{PO Conf}}", "Awaiting");
                    string? ProjectNos = null;
                    List<string>? tempProj = new List<string>();
                    List<string>? tempPRno = new List<string>();
                    string? PRNo = null;
                    foreach (var item in purchaseOrderDetails.POItems)
                    {

                        if (item.POAddprojects.Count > 0)
                            foreach (var project in item.POAddprojects)
                            {
                                if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                            }
                        if (item.PrDetails.Count > 0)
                            foreach (var pr in item.PrDetails)
                            {
                                if (PRNo.IsNullOrEmpty()) { PRNo = pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                                else if (!tempPRno.Contains(pr.PRNumber)) { PRNo = PRNo + ", " + pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                            }

                    }
                    body = body.Replace("{{Project No}}", ProjectNos);
                    body = body.Replace("{{PR Numbers}}", PRNo);

                    email.Body = new TextPart(TextFormat.Html) { Text = body };

                    using var smtp = new MailKit.Net.Smtp.SmtpClient();
                    int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                    _logger.LogInfo($"SMTP Details:Host: {(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault())} |Port:{port} |From:{(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault())} |Password:{(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault())}");

                    smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                    _logger.LogInfo("Connection Successful");
                    smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));
                    _logger.LogInfo("Authenticate Successful");

                    smtp.Send(email);
                    _logger.LogInfo("Send Successful");

                    smtp.Disconnect(true);

                }
                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseOrder Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreatePurchaseOrder API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreatePurchaseOrder API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //download file api
        //
        [HttpGet]
        public async Task<ActionResult> DownloadFile(string Filename)
        {
            ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();
            try
            {

                var filename = Uri.UnescapeDataString(Filename);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PODocument", filename);
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
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DownloadFile API for the following Filename:{Filename} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DownloadFile API for the following Filename:{Filename} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
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

        //public async Task<IActionResult> StageDocumentDownloadFile(int fileid)
        //{
        //    ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();
        //    StageDocumentUpload DownloadFilesList = await _repository.StageDocumentUploadService.StageDownloadFiles(fileid);
        //    if (DownloadFilesList == null)
        //    {
        //        return NotFound();
        //    }
        //    var DownloadFilesdata = DownloadFilesList;
        //    var stream = new FileStream(DownloadFilesdata.FilePath, FileMode.Open);
        //    return File(stream, "application/octet-stream", DownloadFilesdata.FileName);
        //}

        [HttpGet]
        public async Task<IActionResult> SearchPurchaseOrderDate([FromQuery] SearchDatesParams searchDateParam, PoVersion poVersion)
        {
            ServiceResponse<IEnumerable<PurchaseOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderReportDto>>();
            try
            {
                var purchaseOrderList = await _repository.SearchPurchaseOrderDate(searchDateParam, poVersion);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<PurchaseOrder, PurchaseOrderReportDto>()
                    .ForMember(dest => dest.POIncoTerms, opt => opt.MapFrom(src => src.POIncoTerms
                        .Select(PoIncoTerm => new PoIncoTermReportDto
                        {
                            Id = PoIncoTerm.Id,
                            IncoTermName = PoIncoTerm.IncoTermName,
                            PONumber = src.PONumber,
                        })
                            )
                        );
                    cfg.CreateMap<PoItem, PoItemsReportDto>()
                        .ForMember(dest => dest.POAddprojects, opt => opt.MapFrom(src => src.POAddprojects
                        .Select(PoAddProject => new PoAddProjectReportDto
                        {
                            Id = PoAddProject.Id,
                            PONumber = src.PONumber,
                            ItemNumber = src.ItemNumber,
                            ProjectNumber = PoAddProject.ProjectNumber,
                            ProjectQty = PoAddProject.ProjectQty,
                            BalanceQty = PoAddProject.BalanceQty,
                            ReceivedQty = PoAddProject.ReceivedQty
                        })
                            )
                        )
                        .ForMember(dest => dest.POAddDeliverySchedules, opt => opt.MapFrom(src => src.POAddDeliverySchedules
                        .Select(PoAddDeliverySchedule => new PoAddDeliveryScheduleReportDto
                        {
                            Id = PoAddDeliverySchedule.Id,
                            PONumber = src.PONumber,
                            ItemNumber = src.ItemNumber,
                            PODeliveryDate = PoAddDeliverySchedule.PODeliveryDate,
                            PODeliveryQty = PoAddDeliverySchedule.PODeliveryQty
                        })
                            )
                        )
                        .ForMember(dest => dest.POConfirmationDates, opt => opt.MapFrom(src => src.POConfirmationDates
                        .Select(PoConfirmationDate => new PoConfirmationDateReportDto
                        {
                            Id = PoConfirmationDate.Id,
                            ConfirmationDate = PoConfirmationDate.ConfirmationDate,
                            Qty = PoConfirmationDate.Qty
                        })
                        ));
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<PurchaseOrderReportDto>>(purchaseOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseOrderItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchPurchaseOrderDate API for the following poVersion : {poVersion} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchPurchaseOrderDate API for the following poVersion : {poVersion} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllPurchaseOrderWithItems([FromBody] PurchaseOrderSearchDto purchaseOrderSearch, PoVersion poVersion)
        {
            ServiceResponse<IEnumerable<PurchaseOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderReportDto>>();
            try
            {
                var purchaseOrderList = await _repository.GetAllPurchaseOrderWithItems(purchaseOrderSearch, poVersion);

                _logger.LogInfo("Returned all PurhaseOrders");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<PurchaseOrderDto, PurchaseOrder>().ReverseMap()
                //    .ForMember(dest => dest.POItems, opt => opt.MapFrom(src => src.POItems));
                //});

                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<PurchaseOrder, PurchaseOrderReportDto>()

                        .ForMember(dest => dest.POIncoTerms, opt => opt.MapFrom(src => src.POIncoTerms
                        .Select(PoIncoTerm => new PoIncoTermReportDto
                        {
                            Id = PoIncoTerm.Id,
                            IncoTermName = PoIncoTerm.IncoTermName,
                            PONumber = src.PONumber,
                        })
                            )
                        );
                    cfg.CreateMap<PoItem, PoItemsReportDto>()
                        .ForMember(dest => dest.POAddprojects, opt => opt.MapFrom(src => src.POAddprojects
                        .Select(PoAddProject => new PoAddProjectReportDto
                        {
                            Id = PoAddProject.Id,
                            PONumber = src.PONumber,
                            ItemNumber = src.ItemNumber,
                            ProjectNumber = PoAddProject.ProjectNumber,
                            ProjectQty = PoAddProject.ProjectQty,
                            BalanceQty = PoAddProject.BalanceQty,
                            ReceivedQty = PoAddProject.ReceivedQty
                        })
                            )
                        )
                        .ForMember(dest => dest.POAddDeliverySchedules, opt => opt.MapFrom(src => src.POAddDeliverySchedules
                        .Select(PoAddDeliverySchedule => new PoAddDeliveryScheduleReportDto
                        {
                            Id = PoAddDeliverySchedule.Id,
                            PONumber = src.PONumber,
                            ItemNumber = src.ItemNumber,
                            PODeliveryDate = PoAddDeliverySchedule.PODeliveryDate,
                            PODeliveryQty = PoAddDeliverySchedule.PODeliveryQty
                        })
                            )
                        )
                        .ForMember(dest => dest.POConfirmationDates, opt => opt.MapFrom(src => src.POConfirmationDates
                        .Select(PoConfirmationDate => new PoConfirmationDateReportDto
                        {
                            Id = PoConfirmationDate.Id,
                            ConfirmationDate = PoConfirmationDate.ConfirmationDate,
                            Qty = PoConfirmationDate.Qty
                        })
                        ));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<PurchaseOrderReportDto>>(purchaseOrderList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseOrderItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPurchaseOrderWithItems API for the following poVersion:{poVersion} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPurchaseOrderWithItems API for the following poVersion:{poVersion} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchPurchaseOrder([FromQuery] SearchParamess searchParams, PoVersion poVersion)
        {
            ServiceResponse<IEnumerable<PurchaseOrderReportDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderReportDto>>();
            try
            {
                var purchaseOrderList = await _repository.SearchPurchaseOrder(searchParams, poVersion);
                _logger.LogInfo("Returned all PurchaseOrders");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<PurchaseOrderDto, PurchaseOrder>().ReverseMap()
                //    .ForMember(dest => dest.POItems, opt => opt.MapFrom(src => src.POItems));
                //});

                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<PurchaseOrder, PurchaseOrderReportDto>()
                    .ForMember(dest => dest.POIncoTerms, opt => opt.MapFrom(src => src.POIncoTerms
                        .Select(PoIncoTerm => new PoIncoTermReportDto
                        {
                            Id = PoIncoTerm.Id,
                            IncoTermName = PoIncoTerm.IncoTermName,
                            PONumber = src.PONumber,
                        })
                            )
                        );

                    cfg.CreateMap<PoItem, PoItemsReportDto>()
                        .ForMember(dest => dest.POAddprojects, opt => opt.MapFrom(src => src.POAddprojects
                        .Select(PoAddProject => new PoAddProjectReportDto
                        {
                            Id = PoAddProject.Id,
                            PONumber = src.PONumber,
                            ItemNumber = src.ItemNumber,
                            ProjectNumber = PoAddProject.ProjectNumber,
                            ProjectQty = PoAddProject.ProjectQty,
                            BalanceQty = PoAddProject.BalanceQty,
                            ReceivedQty = PoAddProject.ReceivedQty
                        })
                            )
                        )
                        .ForMember(dest => dest.POAddDeliverySchedules, opt => opt.MapFrom(src => src.POAddDeliverySchedules
                        .Select(PoAddDeliverySchedule => new PoAddDeliveryScheduleReportDto
                        {
                            Id = PoAddDeliverySchedule.Id,
                            PONumber = src.PONumber,
                            ItemNumber = src.ItemNumber,
                            PODeliveryDate = PoAddDeliverySchedule.PODeliveryDate,
                            PODeliveryQty = PoAddDeliverySchedule.PODeliveryQty
                        })
                            )
                        )
                        .ForMember(dest => dest.POConfirmationDates, opt => opt.MapFrom(src => src.POConfirmationDates
                        .Select(PoConfirmationDate => new PoConfirmationDateReportDto
                        {
                            Id = PoConfirmationDate.Id,
                            ConfirmationDate = PoConfirmationDate.ConfirmationDate,
                            Qty = PoConfirmationDate.Qty
                        })
                        ));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<PurchaseOrderReportDto>>(purchaseOrderList);

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseOrderItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchPurchaseOrder API for the following poVersion : {poVersion} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchPurchaseOrder API for the following poVersion : {poVersion} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{filename}")]
        public IActionResult DownloadFiles(string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PODocument", filename);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            try
            {
                var downloadUrl = $"{Request.Scheme}://{Request.Host}/api/PurchaseOrder/DownloadFile?Filename={filename}";

                return Ok(new { FilePath = filePath, DownloadUrl = downloadUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //update uploaded files

        [HttpPost]
        public async Task<IActionResult> UpdatePOUploadDocument([FromBody] List<DocumentUploadPostDto> uploadDocumentDto)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (uploadDocumentDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrder UploadDocument object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseOrder UploadDocument sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseOrder UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseOrder UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<string>? id_s = new List<string>();
                foreach (var poUploadDetail in uploadDocumentDto)
                {
                    Guid guid = Guid.NewGuid();
                    var fileContent = poUploadDetail.FileByte;
                    byte[] imageContent = Convert.FromBase64String(poUploadDetail.FileByte);
                    string fileName = guid.ToString() + "_" + poUploadDetail.FileName + "." + poUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PODocument", fileName);
                    using (MemoryStream ms = new MemoryStream(imageContent))
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
                            ParentNumber = "Purchase Order",
                            //PurchaseOrderId = Id,
                            DocumentFrom = "PODocument",

                        };
                        //var poUploadDoc = _mapper.Map<DocumentUpload>(uploadedFile);

                        await _documentUploadRepository.CreateUploadDocumentPO(uploadedFile);
                        _documentUploadRepository.SaveAsync();

                        id_s.Add(uploadedFile.Id.ToString());
                    }
                }

                serviceResponse.Data = id_s;
                serviceResponse.Message = " POUploadDocument Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdatePOUploadDocument API : {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdatePOUploadDocument API : {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //delete uploaded file

        [HttpDelete]
        public async Task<IActionResult> DeletePOUploadDocument(int id)
        {
            ServiceResponse<IEnumerable<DocumentUploadDto>> serviceResponse = new ServiceResponse<IEnumerable<DocumentUploadDto>>();

            try
            {
                var documentUploadDetails = await _documentUploadRepository.GetUploadDocById(id);
                var fileName = documentUploadDetails.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PODocument", /*guid.ToString() + "_" */ fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    string result = await _documentUploadRepository.DeleteUploadFile(documentUploadDetails);
                    _logger.LogInfo(result);
                    _documentUploadRepository.SaveAsync();

                    serviceResponse.Data = null;
                    serviceResponse.Message = " UploadDocument Deleted Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Given UploadDocument file is doesn't exist");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Given UploadDocument file is doesn't exist";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeletePOUploadDocument API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeletePOUploadDocument API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetails(string poNumber)
        {
            ServiceResponse<IEnumerable<GetDownloadUrlDto>> serviceResponse = new ServiceResponse<IEnumerable<GetDownloadUrlDto>>();

            try
            {
                string serverKey = GetServerKey();

                var getDownloadDetailByPoNumber = await _repository.GetDownloadUrlDetails(poNumber);

                if (getDownloadDetailByPoNumber.Count() == 0)
                {
                    _logger.LogError($"DownloadDetail with id: {poNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DownloadDetail with id: {poNumber}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseOrder UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseOrder UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<GetDownloadUrlDto> downloadUrls = new List<GetDownloadUrlDto>();
                if (getDownloadDetailByPoNumber != null)
                {
                    foreach (var getDownloadUrlByFilename in getDownloadDetailByPoNumber)
                    {
                        GetDownloadUrlDto downloadUrlDto = _mapper.Map<GetDownloadUrlDto>(getDownloadUrlByFilename);
                        if (serverKey == "avision")
                        {
                            var baseUrl = $"{_config["PurchaseBaseUrl"]}";
                            downloadUrlDto.DownloadUrl = $"{baseUrl}/apigateway/tips/PurchaseOrder/DownloadFile?Filename={downloadUrlDto.FileName}";
                        }
                        else
                        {
                            var baseUrl = $"{Request.Scheme}://{_config["PurchaseBaseUrl"]}";
                            downloadUrlDto.DownloadUrl = $"{baseUrl}/api/PurchaseOrder/DownloadFile?Filename={downloadUrlDto.FileName}";

                        }
                        downloadUrls.Add(downloadUrlDto);
                    }
                }
                _logger.LogInfo($"Returned DownloadDetail with id: {poNumber}");
                //var result = _mapper.Map<IEnumerable<GetDownloadUrlDto>>(getDownloadDetailByPoNumber);
                serviceResponse.Data = downloadUrls;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDownloadUrlDetails API for the following poNumber:{poNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDownloadUrlDetails API for the following poNumber:{poNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetails_PoFiles(string FileIds)
        {
            ServiceResponse<IEnumerable<GetDownloadUrlDto>> serviceResponse = new ServiceResponse<IEnumerable<GetDownloadUrlDto>>();

            try
            {
                string serverKey = GetServerKey();

                var getDownloadDetailByPoNumber = await _repository.GetDownloadUrlPoDetails(FileIds);

                if (getDownloadDetailByPoNumber.Count() == 0)
                {
                    _logger.LogError($"DownloadDetail with id: {FileIds}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DownloadDetail with id: {FileIds}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseRequisition UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseRequisition UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<GetDownloadUrlDto> downloadUrls = new List<GetDownloadUrlDto>();
                if (getDownloadDetailByPoNumber != null)
                {
                    foreach (var getDownloadUrlByFilename in getDownloadDetailByPoNumber)
                    {
                        GetDownloadUrlDto downloadUrlDto = _mapper.Map<GetDownloadUrlDto>(getDownloadUrlByFilename);
                        if (serverKey == "avision")
                        {
                            var baseUrl = $"{_config["PurchaseBaseUrl"]}";
                            downloadUrlDto.DownloadUrl = $"{baseUrl}/apigateway/tips/PurchaseOrder/DownloadFile?Filename={Uri.EscapeDataString(downloadUrlDto.FileName)}";
                        }
                        else
                        {
                            var baseUrl = $"{Request.Scheme}://{_config["PurchaseBaseUrl"]}";
                            downloadUrlDto.DownloadUrl = $"{baseUrl}/api/PurchaseOrder/DownloadFile?Filename={Uri.EscapeDataString(downloadUrlDto.FileName)}";
                        }
                        downloadUrls.Add(downloadUrlDto);
                    }
                }
                _logger.LogInfo($"Returned DownloadDetail with id: {FileIds}");
                //var result = _mapper.Map<IEnumerable<GetDownloadUrlDto>>(getDownloadDetailByPoNumber);
                serviceResponse.Data = downloadUrls;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetDownloadUrlDetails_PoFiles API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDownloadUrlDetails_PoFiles API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut]
        public async Task<IActionResult> ReturnToVendorPOs([FromBody] List<PurchaseOrderReturnsBackDto> purchaseOrderReturns)
        {
            ServiceResponse<List<PurchaseOrderReturnsBackDto>> serviceResponse = new ServiceResponse<List<PurchaseOrderReturnsBackDto>>();
            try
            {
                if (purchaseOrderReturns is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrderReturns object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseOrderReturns object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseOrderReturns object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseOrderReturns object sent from client.");
                    return BadRequest(serviceResponse);
                }
                foreach (var po in purchaseOrderReturns)
                {
                    var PurchaseOrder = await _repository.GetLastestPurchaseOrderByPONumber(po.PurchaseOrderNo);
                    if (PurchaseOrder == null)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"In ReturnToVendorPOs: PONumber: {po.PurchaseOrderNo} was not Found";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        _logger.LogError($"In ReturnToVendorPOs: PONumber: {po.PurchaseOrderNo} was not Found");
                        return BadRequest(serviceResponse);
                    }
                    foreach (var poitem in po.purchaseOrderItems)
                    {
                        var POChild = PurchaseOrder.POItems.Where(x => x.ItemNumber == poitem.ItemNumber).FirstOrDefault();
                        POChild.PoPartsStatus = false;
                        POChild.BalanceQty += poitem.ReturnQty;
                        POChild.ReceivedQty -= poitem.ReturnQty;
                        if (POChild.Qty == POChild.BalanceQty) POChild.PoStatus = PoStatus.Open;
                        else POChild.PoStatus = PoStatus.PartiallyClosed;
                        foreach (var prj in poitem.purchaseOrderReturnProjectBackDtos)
                        {
                            var project = POChild.POAddprojects.Where(x => x.ProjectNumber == prj.ProjectNo).FirstOrDefault();
                            project.BalanceQty += prj.ReturnQty;
                            project.ReceivedQty -= prj.ReturnQty;
                            //project.PoAddProjectStatus = false;
                        }
                    }
                    if (PurchaseOrder.POItems.Where(x => x.PoStatus == PoStatus.Open).Count() == PurchaseOrder.POItems.Count()) PurchaseOrder.PoStatus = PoStatus.Open;
                    else if (PurchaseOrder.POItems.Where(x => x.PoStatus == PoStatus.Closed).Count() == PurchaseOrder.POItems.Count()) PurchaseOrder.PoStatus = PoStatus.Closed;
                    else if (PurchaseOrder.POItems.Where(x => x.PoStatus == PoStatus.ShortClosed).Count() == PurchaseOrder.POItems.Count()) PurchaseOrder.PoStatus = PoStatus.ShortClosed;
                    else PurchaseOrder.PoStatus = PoStatus.PartiallyClosed;
                    await _repository.UpdatePurchaseOrder_ForApproval(PurchaseOrder);
                }
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = $"In ReturnToVendorPOs: POs are returned Sucessfully";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(200, serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in ReturnToVendorPOs API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ReturnToVendorPOs API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdatePurchaseOrder([FromBody] PurchaseOrderUpdateDto purchaseOrderUpdateDto)
        {
            ServiceResponse<PurchaseOrderPostDto> serviceResponse = new ServiceResponse<PurchaseOrderPostDto>();
            try
            {
                string serverKey = GetServerKey();
                if (purchaseOrderUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrder object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseOrder object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }
                if (purchaseOrderUpdateDto.Currency != "INR" && purchaseOrderUpdateDto.ConvertionRateId == null && serverKey != "keus")
                {
                    serviceResponse.Message = $"Error Occured in CreatePurchaseOrder: The ConvertionRateId is required for the UOC:{purchaseOrderUpdateDto.Currency}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Error Occured in CreatePurchaseOrder: The ConvertionRateId is required for the UOC:{purchaseOrderUpdateDto.Currency}");
                    return BadRequest(serviceResponse);
                }
                var purchaseOrderDetails = _mapper.Map<PurchaseOrder>(purchaseOrderUpdateDto);
                var AmountInWords = GetTotalValueInWords(purchaseOrderDetails.TotalAmount);
                purchaseOrderDetails.AmountInWords = AmountInWords;
                var poItemDto = purchaseOrderUpdateDto.POItems;
                var poItemDtoList = new List<PoItem>();
                var poIncoTermDto = purchaseOrderUpdateDto.POIncoTerms;
                var poIncoTermsList = new List<PoIncoTerm>();
                var poAddKitProjectList = new List<PoAddKitProject>();
                List<EnggBomKitItemNumberWithQtyDto>? enggBomKitDetailsDynamic = new List<EnggBomKitItemNumberWithQtyDto>();

                if (poIncoTermDto != null)
                {
                    for (int i = 0; i < poIncoTermDto.Count; i++)
                    {
                        PoIncoTerm poIncoTermDetails = _mapper.Map<PoIncoTerm>(poIncoTermDto[i]);
                        poIncoTermsList.Add(poIncoTermDetails);
                    }
                }
                purchaseOrderDetails.POIncoTerms = poIncoTermsList;

                var poAdditionalChargesDto = purchaseOrderUpdateDto.PurchaseOrderAdditionalCharges;

                var poAdditionalChargesList = new List<PurchaseOrderAdditionalCharges>();
                if (poAdditionalChargesDto != null)
                {
                    for (int i = 0; i < poAdditionalChargesDto.Count; i++)
                    {
                        PurchaseOrderAdditionalCharges poAdditionalChargeDetails = _mapper.Map<PurchaseOrderAdditionalCharges>(poAdditionalChargesDto[i]);
                        poAdditionalChargesList.Add(poAdditionalChargeDetails);
                    }
                }
                purchaseOrderDetails.PurchaseOrderAdditionalCharges = poAdditionalChargesList;

                if (poItemDto != null)
                {
                    if (purchaseOrderDetails.PoType == PoType.Kit)
                    {
                        for (int i = 0; i < poItemDto.Count; i++)
                        {
                            //Implement Kit
                            if (poItemDto[i].PartType == PoPartType.Kit)
                            {
                                PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);
                                poItemDetails.BalanceQty = poItemDto[i].Qty;
                                poItemDetails.PoPartsStatus = false;
                                poItemDetails.PONumber = purchaseOrderDetails.PONumber;
                                var poAddprojectDetails = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                                for (int j = 0; j < poAddprojectDetails.Count; j++)
                                {
                                    PoAddProject poaddproject = poAddprojectDetails[j];
                                    poaddproject.BalanceQty = poaddproject.ProjectQty;

                                    //Implement KitComponent
                                    var poAddprojectDetail = _mapper.Map<List<PoAddKitProject>>(poAddprojectDetails[j].PoAddKitProjects);
                                    for (int k = 0; k < poAddprojectDetail.Count; k++)
                                    {

                                        PoAddKitProject poAddKitProject = poAddprojectDetail[k];
                                        poAddKitProject.DrawingRevNo = poItemDetails.drawingRevNo;
                                        poAddKitProject.CreatedBy = _createdBy;
                                        poAddKitProject.CreatedOn = DateTime.Now;

                                    }
                                    poAddprojectDetails[j].PoAddKitProjects = poAddprojectDetail;

                                }
                                poItemDetails.POAddprojects = poAddprojectDetails;
                                poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);
                                poItemDetails.POSpecialInstructions = _mapper.Map<List<PoSpecialInstruction>>(poItemDto[i].POSpecialInstructions);
                                //poItemDetails.POConfirmationDates = _mapper.Map<List<PoConfirmationDate>>(poItemDto[i].POConfirmationDates);
                                poItemDetails.PrDetails = _mapper.Map<List<PrDetails>>(poItemDto[i].PrDetails);
                                poItemDtoList.Add(poItemDetails);
                            }
                            else
                            {
                                serviceResponse.Message = $"Error Occured in UpdatePurchaseOrder: The PoType is kit,But PoItemDto.Parttype is not Kit:{poItemDto[i].PartType}";
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                                _logger.LogError($"Error Occured in UpdatePurchaseOrder: The PoType is kit,But PoItemDto.Parttype is not Kit:{poItemDto[i].PartType}");
                                return BadRequest(serviceResponse);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < poItemDto.Count; i++)
                        {
                            if (poItemDto[i].PartType != PoPartType.Kit)
                            {
                                PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);
                                poItemDetails.BalanceQty = poItemDto[i].Qty;
                                poItemDetails.PoPartsStatus = false;
                                poItemDetails.PONumber = purchaseOrderDetails.PONumber;
                                poItemDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                                for (int j = 0; j < poItemDetails.POAddprojects.Count; j++)
                                {
                                    PoAddProject poaddproject = poItemDetails.POAddprojects[j];
                                    poaddproject.BalanceQty = poaddproject.ProjectQty;
                                }

                                poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);
                                poItemDetails.POSpecialInstructions = _mapper.Map<List<PoSpecialInstruction>>(poItemDto[i].POSpecialInstructions);
                                //poItemDetails.POConfirmationDates = _mapper.Map<List<PoConfirmationDate>>(poItemDto[i].POConfirmationDates);
                                poItemDetails.PrDetails = _mapper.Map<List<PrDetails>>(poItemDto[i].PrDetails);
                                poItemDtoList.Add(poItemDetails);
                            }
                            else
                            {
                                serviceResponse.Message = $"Error Occured in UpdatePurchaseOrder: The PoItemDto.Parttype is kit,But PoType is General:{poItemDto[i].PartType}";
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                                _logger.LogError($"Error Occured in UpdatePurchaseOrder: The PoItemDto.Parttype is kit,But PoType is General:{poItemDto[i].PartType}");
                                return BadRequest(serviceResponse);
                            }
                        }
                    }
                }

                purchaseOrderDetails.POItems = poItemDtoList;
                await _repository.ChangePurchaseOrderVersion(purchaseOrderDetails);

                if (purchaseOrderUpdateDto.POItems != null)
                {
                    foreach (var pritem in purchaseOrderUpdateDto.POItems)
                    {
                        if (pritem.PrDetails != null)
                        {
                            foreach (var pritemdetail in pritem.PrDetails)
                            {
                                if (pritemdetail.PrDetailDocumentUploadUpdateDtos != null)
                                {
                                    foreach (var prDetailsDto in pritemdetail.PrDetailDocumentUploadUpdateDtos)
                                    {
                                        var prUploadDocument = await _pRItemsDocumentUploadRepository.GetUploadDocByFileName(prDetailsDto.FileName);
                                        if (prUploadDocument != null)
                                        {
                                            prUploadDocument.Checked = true;
                                            await _pRItemsDocumentUploadRepository.UpdateUploadDoc(prUploadDocument);
                                        }


                                    }
                                }
                            }
                        }
                    }
                }
                if (poItemDtoList != null)
                {
                    foreach (var poItems in poItemDtoList)
                    {
                        if (poItems.PrDetails != null)
                        {
                            foreach (var prDetails in poItems.PrDetails)
                            {
                                var prItemDetail = await _purchaseRequisitionItemRepository.GetPrItemByPRNo(prDetails.PRNumber, poItems.ItemNumber);
                                if (prItemDetail != null)
                                {
                                    if (prDetails.ToClosePR == true) prItemDetail.PrStatus = PrStatus.Closed;
                                    else prItemDetail.PrStatus = PrStatus.Open;
                                    await _purchaseRequisitionItemRepository.UpdatePrItem(prItemDetail);
                                    _purchaseRequisitionItemRepository.SaveAsync();
                                }

                                var prItemClosedStatusCount = await _purchaseRequisitionItemRepository.GetPrItemClosedStatusCount(prDetails.PRNumber);
                                var prDetail = await _purchaseRequisitionRepository.GetPrDetailsByPrNumber(prDetails.PRNumber);
                                prDetail.PrStatus = prItemClosedStatusCount;
                                await _purchaseRequisitionRepository.UpdatePurchaseRequisition_ForApproval(prDetail);
                            }
                        }
                    }
                }

                _repository.SaveAsync();
                _pRItemsDocumentUploadRepository.SaveAsync();
                _purchaseRequisitionRepository.SaveAsync();

                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"],
                           "GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);

                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,CreatePurchaseOrder";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);

                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    //var mails = "accounts@avisionsystems.com";
                    var mails = (emaildetails1.data.Where(x => x.operation == "CreatePurchaseOrder").Select(x => x.emailIds).FirstOrDefault()).Split(',');

                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));

                    //email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    var podate = purchaseOrderDetails.PODate.ToString().Split(" ");
                    //email.To.Add(MailboxAddress.Parse(mails));
                    email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));

                    email.Subject = "Purchase Order Modified Notification";
                    string body = emaildetails.data.template;
                    body = body.Replace("{{PO Number}}", purchaseOrderDetails.PONumber);
                    body = body.Replace("{{PO Revision No}}", purchaseOrderDetails.RevisionNumber.ToString());
                    body = body.Replace("{{PO Date}}", podate[0]);
                    body = body.Replace("{{PO Value}}", purchaseOrderDetails.TotalAmount.ToString());
                    body = body.Replace("{{Vendor Name}}", purchaseOrderDetails.VendorName.ToString());
                    body = body.Replace("{{Approval1}}", "Awaiting");
                    body = body.Replace("{{Approval2}}", "Awaiting");
                    body = body.Replace("{{Approval3}}", "Awaiting");
                    body = body.Replace("{{Approval4}}", "Awaiting");
                    body = body.Replace("{{PO Conf}}", "Awaiting");
                    string? ProjectNos = null;
                    List<string>? tempProj = new List<string>();
                    List<string>? tempPRno = new List<string>();
                    string? PRNo = null;
                    foreach (var item in purchaseOrderDetails.POItems)
                    {

                        if (item.POAddprojects.Count > 0)
                            foreach (var project in item.POAddprojects)
                            {
                                if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                            }
                        if (item.PrDetails.Count > 0)
                            foreach (var pr in item.PrDetails)
                            {
                                if (PRNo.IsNullOrEmpty()) { PRNo = pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                                else if (!tempPRno.Contains(pr.PRNumber)) { PRNo = PRNo + ", " + pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                            }

                    }
                    body = body.Replace("{{Project No}}", ProjectNos);
                    body = body.Replace("{{PR Numbers}}", PRNo);

                    email.Body = new TextPart(TextFormat.Html) { Text = body };

                    using var smtp = new MailKit.Net.Smtp.SmtpClient();
                    int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                    smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                    smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                    //smtp.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
                    //smtp.Authenticate("erp@avisionsystems.com", "R#9183753474150W");

                    smtp.Send(email);
                    smtp.Disconnect(true);

                }

                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdatePurchaseOrder API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdatePurchaseOrder API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateApprovalPurchaseOrder([FromBody] PurchaseOrderApprovalUpdateDto purchaseOrderUpdateDto)
        {
            ServiceResponse<PurchaseOrderPostDto> serviceResponse = new ServiceResponse<PurchaseOrderPostDto>();
            try
            {
                string serverKey = GetServerKey();
                if (purchaseOrderUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrder object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseOrder object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }
                if (purchaseOrderUpdateDto.Currency != "INR" && purchaseOrderUpdateDto.ConvertionRateId == null && serverKey != "keus")
                {
                    serviceResponse.Message = $"Error Occured in CreatePurchaseOrder: The ConvertionRateId is required for the UOC:{purchaseOrderUpdateDto.Currency}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Error Occured in CreatePurchaseOrder: The ConvertionRateId is required for the UOC:{purchaseOrderUpdateDto.Currency}");
                    return BadRequest(serviceResponse);
                }
                var purchaseOrderDetails = _mapper.Map<PurchaseOrder>(purchaseOrderUpdateDto);
                var AmountInWords = GetTotalValueInWords(purchaseOrderDetails.TotalAmount);
                purchaseOrderDetails.AmountInWords = AmountInWords;
                var poItemDto = purchaseOrderUpdateDto.POItems;
                var poItemDtoList = new List<PoItem>();
                var poIncoTermDto = purchaseOrderUpdateDto.POIncoTerms;
                var poIncoTermsList = new List<PoIncoTerm>();
                var poAddKitProjectList = new List<PoAddKitProject>();
                List<EnggBomKitItemNumberWithQtyDto>? enggBomKitDetailsDynamic = new List<EnggBomKitItemNumberWithQtyDto>();

                if (poIncoTermDto != null)
                {
                    for (int i = 0; i < poIncoTermDto.Count; i++)
                    {
                        PoIncoTerm poIncoTermDetails = _mapper.Map<PoIncoTerm>(poIncoTermDto[i]);
                        poIncoTermsList.Add(poIncoTermDetails);
                    }
                }
                purchaseOrderDetails.POIncoTerms = poIncoTermsList;

                var poAdditionalChargesDto = purchaseOrderUpdateDto.PurchaseOrderAdditionalCharges;

                var poAdditionalChargesList = new List<PurchaseOrderAdditionalCharges>();
                if (poAdditionalChargesDto != null)
                {
                    for (int i = 0; i < poAdditionalChargesDto.Count; i++)
                    {
                        PurchaseOrderAdditionalCharges poAdditionalChargeDetails = _mapper.Map<PurchaseOrderAdditionalCharges>(poAdditionalChargesDto[i]);
                        poAdditionalChargesList.Add(poAdditionalChargeDetails);
                    }
                }
                purchaseOrderDetails.PurchaseOrderAdditionalCharges = poAdditionalChargesList;

                if (poItemDto != null)
                {
                    if (purchaseOrderDetails.PoType == PoType.Kit)
                    {
                        for (int i = 0; i < poItemDto.Count; i++)
                        {
                            //Implement Kit
                            if (poItemDto[i].PartType == PoPartType.Kit)
                            {
                                PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);
                                poItemDetails.BalanceQty = poItemDto[i].Qty;
                                poItemDetails.PoPartsStatus = false;
                                poItemDetails.PONumber = purchaseOrderDetails.PONumber;
                                var poAddprojectDetails = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                                for (int j = 0; j < poAddprojectDetails.Count; j++)
                                {
                                    PoAddProject poaddproject = poAddprojectDetails[j];
                                    poaddproject.BalanceQty = poaddproject.ProjectQty;

                                    //Implement KitComponent
                                    var poAddprojectDetail = _mapper.Map<List<PoAddKitProject>>(poAddprojectDetails[j].PoAddKitProjects);
                                    for (int k = 0; k < poAddprojectDetail.Count; k++)
                                    {

                                        PoAddKitProject poAddKitProject = poAddprojectDetail[k];
                                        poAddKitProject.DrawingRevNo = poItemDetails.drawingRevNo;
                                        poAddKitProject.CreatedBy = _createdBy;
                                        poAddKitProject.CreatedOn = DateTime.Now;

                                    }
                                    poAddprojectDetails[j].PoAddKitProjects = poAddprojectDetail;

                                }
                                poItemDetails.POAddprojects = poAddprojectDetails;
                                poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);
                                poItemDetails.POSpecialInstructions = _mapper.Map<List<PoSpecialInstruction>>(poItemDto[i].POSpecialInstructions);
                                //poItemDetails.POConfirmationDates = _mapper.Map<List<PoConfirmationDate>>(poItemDto[i].POConfirmationDates);
                                poItemDetails.PrDetails = _mapper.Map<List<PrDetails>>(poItemDto[i].PrDetails);
                                poItemDtoList.Add(poItemDetails);
                            }
                            else
                            {
                                serviceResponse.Message = $"Error Occured in UpdatePurchaseOrder: The PoType is kit,But PoItemDto.Parttype is not Kit:{poItemDto[i].PartType}";
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                                _logger.LogError($"Error Occured in UpdatePurchaseOrder: The PoType is kit,But PoItemDto.Parttype is not Kit:{poItemDto[i].PartType}");
                                return BadRequest(serviceResponse);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < poItemDto.Count; i++)
                        {
                            if (poItemDto[i].PartType != PoPartType.Kit)
                            {
                                PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);
                                poItemDetails.BalanceQty = poItemDto[i].Qty;
                                poItemDetails.PoPartsStatus = false;
                                poItemDetails.PONumber = purchaseOrderDetails.PONumber;
                                poItemDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                                for (int j = 0; j < poItemDetails.POAddprojects.Count; j++)
                                {
                                    PoAddProject poaddproject = poItemDetails.POAddprojects[j];
                                    poaddproject.BalanceQty = poaddproject.ProjectQty;
                                }

                                poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);
                                poItemDetails.POSpecialInstructions = _mapper.Map<List<PoSpecialInstruction>>(poItemDto[i].POSpecialInstructions);
                                //poItemDetails.POConfirmationDates = _mapper.Map<List<PoConfirmationDate>>(poItemDto[i].POConfirmationDates);
                                poItemDetails.PrDetails = _mapper.Map<List<PrDetails>>(poItemDto[i].PrDetails);
                                poItemDtoList.Add(poItemDetails);
                            }
                            else
                            {
                                serviceResponse.Message = $"Error Occured in UpdatePurchaseOrder: The PoItemDto.Parttype is kit,But PoType is General:{poItemDto[i].PartType}";
                                serviceResponse.Success = false;
                                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                                _logger.LogError($"Error Occured in UpdatePurchaseOrder: The PoItemDto.Parttype is kit,But PoType is General:{poItemDto[i].PartType}");
                                return BadRequest(serviceResponse);
                            }
                        }
                    }
                }

                purchaseOrderDetails.POItems = poItemDtoList;
                await _repository.UpdateApprovalPurchaseOrderWithReplacement(purchaseOrderDetails);

                if (purchaseOrderUpdateDto.POItems != null)
                {
                    foreach (var pritem in purchaseOrderUpdateDto.POItems)
                    {
                        if (pritem.PrDetails != null)
                        {
                            foreach (var pritemdetail in pritem.PrDetails)
                            {
                                if (pritemdetail.PrDetailDocumentUploadUpdateDtos != null)
                                {
                                    foreach (var prDetailsDto in pritemdetail.PrDetailDocumentUploadUpdateDtos)
                                    {
                                        var prUploadDocument = await _pRItemsDocumentUploadRepository.GetUploadDocByFileName(prDetailsDto.FileName);
                                        if (prUploadDocument != null)
                                        {
                                            prUploadDocument.Checked = true;
                                            await _pRItemsDocumentUploadRepository.UpdateUploadDoc(prUploadDocument);
                                        }


                                    }
                                }
                            }
                        }
                    }
                }
                if (poItemDtoList != null)
                {
                    foreach (var poItems in poItemDtoList)
                    {
                        if (poItems.PrDetails != null)
                        {
                            foreach (var prDetails in poItems.PrDetails)
                            {
                                var prItemDetail = await _purchaseRequisitionItemRepository.GetPrItemByPRNo(prDetails.PRNumber, poItems.ItemNumber);
                                if (prItemDetail != null)
                                {
                                    if (prDetails.ToClosePR == true) prItemDetail.PrStatus = PrStatus.Closed;
                                    else prItemDetail.PrStatus = PrStatus.Open;
                                    await _purchaseRequisitionItemRepository.UpdatePrItem(prItemDetail);
                                    _purchaseRequisitionItemRepository.SaveAsync();
                                }

                                var prItemClosedStatusCount = await _purchaseRequisitionItemRepository.GetPrItemClosedStatusCount(prDetails.PRNumber);
                                var prDetail = await _purchaseRequisitionRepository.GetPrDetailsByPrNumber(prDetails.PRNumber);
                                prDetail.PrStatus = prItemClosedStatusCount;
                                await _purchaseRequisitionRepository.UpdatePurchaseRequisition_ForApproval(prDetail);
                            }
                        }
                    }
                }

                _repository.SaveAsync();
                _pRItemsDocumentUploadRepository.SaveAsync();
                _purchaseRequisitionRepository.SaveAsync();

                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"],
                           "GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);

                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,CreatePurchaseOrder";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);

                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    //var mails = "accounts@avisionsystems.com";
                    var mails = (emaildetails1.data.Where(x => x.operation == "CreatePurchaseOrder").Select(x => x.emailIds).FirstOrDefault()).Split(',');

                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));

                    //email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    var podate = purchaseOrderDetails.PODate.ToString().Split(" ");
                    //email.To.Add(MailboxAddress.Parse(mails));
                    email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));

                    email.Subject = "Purchase Order Modified Notification";
                    string body = emaildetails.data.template;
                    body = body.Replace("{{PO Number}}", purchaseOrderDetails.PONumber);
                    body = body.Replace("{{PO Revision No}}", purchaseOrderDetails.RevisionNumber.ToString());
                    body = body.Replace("{{PO Date}}", podate[0]);
                    body = body.Replace("{{PO Value}}", purchaseOrderDetails.TotalAmount.ToString());
                    body = body.Replace("{{Vendor Name}}", purchaseOrderDetails.VendorName.ToString());
                    body = body.Replace("{{Approval1}}", "Awaiting");
                    body = body.Replace("{{Approval2}}", "Awaiting");
                    body = body.Replace("{{Approval3}}", "Awaiting");
                    body = body.Replace("{{Approval4}}", "Awaiting");
                    body = body.Replace("{{PO Conf}}", "Awaiting");
                    string? ProjectNos = null;
                    List<string>? tempProj = new List<string>();
                    List<string>? tempPRno = new List<string>();
                    string? PRNo = null;
                    foreach (var item in purchaseOrderDetails.POItems)
                    {

                        if (item.POAddprojects.Count > 0)
                            foreach (var project in item.POAddprojects)
                            {
                                if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                            }
                        if (item.PrDetails.Count > 0)
                            foreach (var pr in item.PrDetails)
                            {
                                if (PRNo.IsNullOrEmpty()) { PRNo = pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                                else if (!tempPRno.Contains(pr.PRNumber)) { PRNo = PRNo + ", " + pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                            }

                    }
                    body = body.Replace("{{Project No}}", ProjectNos);
                    body = body.Replace("{{PR Numbers}}", PRNo);

                    email.Body = new TextPart(TextFormat.Html) { Text = body };

                    using var smtp = new MailKit.Net.Smtp.SmtpClient();
                    int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                    smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                    smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                    //smtp.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
                    //smtp.Authenticate("erp@avisionsystems.com", "R#9183753474150W");

                    smtp.Send(email);
                    smtp.Disconnect(true);

                }

                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdatePurchaseOrder API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdatePurchaseOrder API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateApprovalPurchaseOrder(int id, [FromBody] PurchaseOrderApprovalUpdateDto purchaseOrderUpdateDto)
        //{
        //    ServiceResponse<PurchaseOrderApprovalUpdateDto> serviceResponse = new ServiceResponse<PurchaseOrderApprovalUpdateDto>();

        //    try
        //    {
        //        if (purchaseOrderUpdateDto is null)
        //        {
        //            _logger.LogError("purchaseOrderUpdateDto object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Update purchaseOrderUpdateDto object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid ItemMaster object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid model object";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        var UpdatePurchaseOrderEntity = await _repository.GetPurchaseOrderById(id);
        //        if (UpdatePurchaseOrderEntity is null)
        //        {
        //            _logger.LogError($"PurchaseOrder with id: {id}, hasn't been found in db.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "PurchaseOrder hasn't been found in db.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }

        //        var PoItems = _mapper.Map<IEnumerable<PoItem>>(purchaseOrderUpdateDto.POItems);
        //        var POIncoTerms = _mapper.Map<IEnumerable<PoIncoTerm>>(purchaseOrderUpdateDto.POIncoTerms);
        //        var PurchaseOrderAdditionalCharges = _mapper.Map<IEnumerable<PurchaseOrderAdditionalCharges>>(purchaseOrderUpdateDto.PurchaseOrderAdditionalCharges);
        //        UpdatePurchaseOrderEntity.POItems = null;
        //        UpdatePurchaseOrderEntity.POIncoTerms = null;
        //        UpdatePurchaseOrderEntity.PurchaseOrderAdditionalCharges = null;

        //        var PurchaseOrder = _mapper.Map(purchaseOrderUpdateDto, UpdatePurchaseOrderEntity);

        //        PurchaseOrder.POItems = PoItems.ToList();
        //        PurchaseOrder.POIncoTerms = POIncoTerms.ToList();
        //        PurchaseOrder.PurchaseOrderAdditionalCharges = PurchaseOrderAdditionalCharges.ToList();

        //        string result = await _repository.UpdatePurchaseOrder(PurchaseOrder);
        //        _logger.LogInfo(result);

        //        _repository.SaveAsync();
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "PurchaseOrder Updated Successfully";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error Occured in UpdateApprovalPurchaseOrder API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Error Occured in UpdateApprovalPurchaseOrder API for the following id : {id} \n {ex.Message}";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
        [HttpPut]
        public async Task<IActionResult> ShortCloseForPurchaseOrder([FromBody] PurchaseOrderForShortCloseDto purchaseOrderUpdateDto)
        {
            ServiceResponse<PurchaseOrderPostDto> serviceResponse = new ServiceResponse<PurchaseOrderPostDto>();
            try
            {
                string serverKey = GetServerKey();
                if (purchaseOrderUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrder object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseOrder object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseOrder object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseOrder object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var poDetailBeforeUpdate = await _repository.GetPurchaseOrderById(purchaseOrderUpdateDto.Id);

                if (poDetailBeforeUpdate is null)
                {
                    _logger.LogError($"ShortClose SalesOrder with id: {poDetailBeforeUpdate.Id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update SalesOrder hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var purchaseOrderDetails = _mapper.Map<PurchaseOrder>(purchaseOrderUpdateDto);
                var AmountInWords = GetTotalValueInWords(purchaseOrderDetails.TotalAmount);
                purchaseOrderDetails.AmountInWords = AmountInWords;
                var poItemDto = purchaseOrderUpdateDto.POItems;
                var poItemDtoList = new List<PoItem>();
                var poIncoTermDto = purchaseOrderUpdateDto.POIncoTerms;
                var poIncoTermsList = new List<PoIncoTerm>();

                if (poIncoTermDto != null)
                {
                    for (int i = 0; i < poIncoTermDto.Count; i++)
                    {
                        PoIncoTerm poIncoTermDetails = _mapper.Map<PoIncoTerm>(poIncoTermDto[i]);
                        poIncoTermsList.Add(poIncoTermDetails);
                    }
                }

                var poAdditionalChargesDto = purchaseOrderUpdateDto.PurchaseOrderAdditionalCharges;

                var poAdditionalChargesList = new List<PurchaseOrderAdditionalCharges>();
                if (poAdditionalChargesDto != null)
                {
                    for (int i = 0; i < poAdditionalChargesDto.Count; i++)
                    {
                        PurchaseOrderAdditionalCharges poAdditionalChargeDetails = _mapper.Map<PurchaseOrderAdditionalCharges>(poAdditionalChargesDto[i]);
                        poAdditionalChargesList.Add(poAdditionalChargeDetails);
                    }
                }

                if (poItemDto != null)
                {
                    for (int i = 0; i < poItemDto.Count; i++)
                    {
                        PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);

                        if (poItemDto[i].NowShortClosed == true)
                        {
                            poItemDetails.ShortClosedBy = _createdBy;
                            poItemDetails.ShortClosedOn = DateTime.Now;
                            PoItemHistory poItemHistory = new PoItemHistory();
                            poItemHistory.PONumber = poDetailBeforeUpdate.PONumber;
                            poItemHistory.PODate = poDetailBeforeUpdate.PODate;
                            poItemHistory.PoStatus = poDetailBeforeUpdate.PoStatus;
                            poItemHistory.RevisionNumber = poDetailBeforeUpdate.RevisionNumber;
                            poItemHistory.BillToId = poDetailBeforeUpdate.BillToId;
                            poItemHistory.ShipToId = poDetailBeforeUpdate.ShipToId;
                            poItemHistory.ProcurementType = poDetailBeforeUpdate.ProcurementType;
                            poItemHistory.Currency = poDetailBeforeUpdate.Currency;
                            poItemHistory.CompanyAliasName = poDetailBeforeUpdate.CompanyAliasName;
                            poItemHistory.PoConfirmationStatus = poDetailBeforeUpdate.PoConfirmationStatus;
                            poItemHistory.Transports = poDetailBeforeUpdate.Transports;
                            poItemHistory.Other = poDetailBeforeUpdate.Other;
                            poItemHistory.VendorName = poDetailBeforeUpdate.VendorName;
                            poItemHistory.VendorId = poDetailBeforeUpdate.VendorId;
                            poItemHistory.VendorNumber = poDetailBeforeUpdate.VendorNumber;
                            poItemHistory.QuotationReferenceNumber = poDetailBeforeUpdate.QuotationReferenceNumber;
                            poItemHistory.QuotationDate = poDetailBeforeUpdate.QuotationDate;
                            poItemHistory.VendorAddress = poDetailBeforeUpdate.VendorAddress;
                            poItemHistory.DeliveryTerms = poDetailBeforeUpdate.DeliveryTerms;
                            poItemHistory.PaymentTerms = poDetailBeforeUpdate.PaymentTerms;
                            poItemHistory.ShippingMode = poDetailBeforeUpdate.ShippingMode;
                            poItemHistory.ShipTo = poDetailBeforeUpdate.ShipTo;
                            poItemHistory.BillTo = poDetailBeforeUpdate.BillTo;
                            poItemHistory.POFiles = poDetailBeforeUpdate.POFiles;
                            poItemHistory.RetentionPeriod = poDetailBeforeUpdate.RetentionPeriod;
                            poItemHistory.SpecialTermsAndConditions = poDetailBeforeUpdate.SpecialTermsAndConditions;
                            poItemHistory.IsDeleted = poDetailBeforeUpdate.IsDeleted;
                            poItemHistory.IsShortClosed = purchaseOrderDetails.IsShortClosed;
                            poItemHistory.ShortClosedBy = poItemDetails.ShortClosedBy;
                            poItemHistory.ShortClosedOn = poItemDetails.ShortClosedOn;
                            poItemHistory.TotalAmount = poDetailBeforeUpdate.TotalAmount;
                            poItemHistory.POApprovalI = poDetailBeforeUpdate.POApprovalI;
                            poItemHistory.POApprovedIDate = poDetailBeforeUpdate.POApprovedIDate;
                            poItemHistory.POApprovedIBy = poDetailBeforeUpdate.POApprovedIBy;
                            poItemHistory.POApprovalII = poDetailBeforeUpdate.POApprovalII;
                            poItemHistory.POApprovedIIDate = poDetailBeforeUpdate.POApprovedIIDate;
                            poItemHistory.POApprovedIIBy = poDetailBeforeUpdate.POApprovedIIBy;
                            poItemHistory.POApprovalIII = poDetailBeforeUpdate.POApprovalIII;
                            poItemHistory.POApprovedIIIDate = poDetailBeforeUpdate.POApprovedIIIDate;
                            poItemHistory.POApprovedIIIBy = poDetailBeforeUpdate.POApprovedIIIBy;
                            poItemHistory.POApprovalIV = poDetailBeforeUpdate.POApprovalIV;
                            poItemHistory.POApprovedIVDate = poDetailBeforeUpdate.POApprovedIVDate;
                            poItemHistory.POApprovedIVBy = poDetailBeforeUpdate.POApprovedIVBy;
                            poItemHistory.ApprovalCount = poDetailBeforeUpdate.ApprovalCount;
                            poItemHistory.Unit = poDetailBeforeUpdate.Unit;
                            poItemHistory.PoItemId = poItemDetails.Id;
                            poItemHistory.ItemNumber = poItemDetails.ItemNumber;
                            poItemHistory.MftrItemNumber = poItemDetails.MftrItemNumber;
                            poItemHistory.Description = poItemDetails.Description;
                            poItemHistory.UOM = poItemDetails.UOM;
                            poItemHistory.UnitPrice = poItemDetails.UnitPrice;
                            poItemHistory.Qty = poItemDetails.Qty;
                            poItemHistory.BalanceQty = poItemDetails.BalanceQty;
                            poItemHistory.ReceivedQty = poItemDetails.ReceivedQty;
                            poItemHistory.PartType = poItemDetails.PartType;
                            poItemHistory.SpecialInstruction = poItemDetails.SpecialInstruction;
                            poItemHistory.IsTechnicalDocsRequired = poItemDetails.IsTechnicalDocsRequired;
                            poItemHistory.PoPartsStatus = poItemDetails.PoPartsStatus;
                            poItemHistory.SGST = poItemDetails.SGST;
                            poItemHistory.CGST = poItemDetails.CGST;
                            poItemHistory.IGST = poItemDetails.IGST;
                            poItemHistory.UTGST = poItemDetails.UTGST;
                            poItemHistory.SubTotal = poItemDetails.SubTotal;
                            poItemHistory.TotalWithTax = poItemDetails.TotalWithTax;
                            poItemHistory.ShortClosedBy = poItemDto[i].ShortClosedBy;
                            poItemHistory.ShortClosedOn = poItemDto[i].ShortClosedOn;
                            poItemHistory.CreatedBy = poItemDetails.CreatedBy;
                            poItemHistory.CreatedOn = poItemDetails.CreatedOn;
                            poItemHistory.LastModifiedBy = poItemDetails.LastModifiedBy;
                            poItemHistory.LastModifiedOn = poItemDetails.LastModifiedOn;
                            poItemHistory.PoItemStatus = poItemDetails.PoStatus;
                            poItemHistory.ReasonforShortClose = poItemDetails.ReasonforShortClose;
                            poItemHistory.Remarks = poItemDetails.Remarks;
                            poItemHistory.PurchaseOrderId = poItemDetails.PurchaseOrderId;
                            poItemHistory.ShortClosedQty = poItemDto[i].ShortClosedQty;

                            await _poItemHistoryRepository.CreatePoItemHistory(poItemHistory);
                        }

                        poItemDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                        for (int j = 0; j < poItemDetails.POAddprojects.Count; j++)
                        {
                            PoAddProject poaddproject = poItemDetails.POAddprojects[j];
                            poaddproject.POItemDetailId = poItemDto[i].Id;
                        }

                        poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);
                        for (int j = 0; j < poItemDetails.POAddDeliverySchedules.Count; j++)
                        {
                            PoAddDeliverySchedule poaddDeliverySchedule = poItemDetails.POAddDeliverySchedules[j];
                            poaddDeliverySchedule.POItemDetailId = poItemDto[i].Id;
                        }

                        poItemDetails.POSpecialInstructions = _mapper.Map<List<PoSpecialInstruction>>(poItemDto[i].POSpecialInstructions);
                        for (int j = 0; j < poItemDetails.POSpecialInstructions.Count; j++)
                        {
                            PoSpecialInstruction poSpecialInstructions = poItemDetails.POSpecialInstructions[j];
                            poSpecialInstructions.POItemDetailId = poItemDto[i].Id;
                        }

                        poItemDetails.PrDetails = _mapper.Map<List<PrDetails>>(poItemDto[i].PrDetails);
                        for (int j = 0; j < poItemDetails.PrDetails.Count; j++)
                        {
                            PrDetails prDetails = poItemDetails.PrDetails[j];
                            prDetails.POItemDetailId = poItemDto[i].Id;
                        }

                        poItemDetails.PONumber = purchaseOrderUpdateDto.PONumber;
                        poItemDtoList.Add(poItemDetails);
                    }
                }

                if (purchaseOrderUpdateDto.POItems != null)
                {
                    foreach (var pritem in purchaseOrderUpdateDto.POItems)
                    {
                        if (pritem.PrDetails != null)
                        {
                            foreach (var pritemdetail in pritem.PrDetails)
                            {
                                if (pritemdetail.PrDetailDocumentUploadDtos != null)
                                {
                                    foreach (var prDetailsDto in pritemdetail.PrDetailDocumentUploadDtos)
                                    {
                                        var prUploadDocument = await _pRItemsDocumentUploadRepository.GetUploadDocByFileName(prDetailsDto.FileName);
                                        if (prUploadDocument != null)
                                        {
                                            prUploadDocument.Checked = true;
                                            await _pRItemsDocumentUploadRepository.UpdateUploadDoc(prUploadDocument);
                                        }


                                    }
                                }
                            }
                        }
                    }
                }

                var updateData = _mapper.Map(purchaseOrderUpdateDto, poDetailBeforeUpdate);
                updateData.POItems = poItemDtoList;
                updateData.POIncoTerms = poIncoTermsList;
                updateData.PurchaseOrderAdditionalCharges = poAdditionalChargesList;
                await _repository.UpdatePurchaseOrder_ForApproval(updateData);
                _repository.SaveAsync();
                _poItemHistoryRepository.SaveAsync();
                _pRItemsDocumentUploadRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseOrder ShortClosed Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in ShortCloseForPurchaseOrder API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ShortCloseForPurchaseOrder API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //pass data from grin using _httpclient service to purchase

        [HttpPost]
        public async Task<IActionResult> UpdateBalanceQtyDetails([FromBody] List<PurchaseOrderUpdateQtyDetailsDto> purchaseOrderUpdateQtyDetails)
        {
            foreach (var item in purchaseOrderUpdateQtyDetails)
            {
                IEnumerable<PoItem> poItems = await _poItemsRepository.GetPODetailsByPONumberandItemNo(item.ItemNumber, item.PONumber);
                decimal dispatchedQty = item.Qty;

                foreach (var poItem in poItems)
                {
                    poItem.ReceivedQty += item.Qty;
                    if (poItem.BalanceQty >= dispatchedQty)
                    {
                        if (poItem.BalanceQty == dispatchedQty)
                        {
                            poItem.PoPartsStatus = true;
                        }
                        poItem.BalanceQty -= dispatchedQty;
                        dispatchedQty = 0;
                        break;
                    }
                    else
                    {
                        dispatchedQty -= poItem.BalanceQty;
                        poItem.BalanceQty = 0;
                    }

                    _poItemsRepository.UpdatePOOrderItem(poItem);

                    if (dispatchedQty <= 0)
                    {
                        break;
                    }
                }
            }

            _poItemsRepository.SaveAsync();
            return Ok();
        }

        //pass data from grin using _httpclient service to purchase

        [HttpPost]
        public async Task<IActionResult> UpdatePoProjectNoBalanceQtyDetails([FromBody] List<PoProjectNoUpdateQtyDetailsDto> poProjectNoUpdateBalQtyDetails)
        {
            foreach (var item in poProjectNoUpdateBalQtyDetails)
            {
                IEnumerable<PoAddProject> poProjectNoDetails = await _poAddprojectRepository.GetPOProjectNoDetailsByProjectNo(item.ItemNumber, item.ProjectNumber, item.PoItemId);
                decimal dispatchedQty = item.ProjectQty;

                foreach (var poProjectNos in poProjectNoDetails)
                {
                    poProjectNos.ReceivedQty = item.ProjectQty;
                    if (poProjectNos.BalanceQty >= dispatchedQty)
                    {
                        if (poProjectNos.BalanceQty == dispatchedQty)
                        {
                            //poProjectNos.PoAddProjectStatus = true;
                        }
                        poProjectNos.BalanceQty -= dispatchedQty;
                        dispatchedQty = 0;
                        break;
                    }
                    else
                    {
                        dispatchedQty -= poProjectNos.BalanceQty;
                        poProjectNos.BalanceQty = 0;
                    }

                    await _poAddprojectRepository.UpdatePoAddproject(poProjectNos);

                    if (dispatchedQty <= 0)
                    {
                        break;
                    }
                }
            }

            _poAddprojectRepository.SaveAsync();
            return Ok();
        }

        //pass data from grin qty using _httpclient service to purchase

        [HttpPost]
        public async Task<IActionResult> UpdatePoStatus([FromBody] List<PurchaseOrderStatusUpdateDto> purchaseOrderStatusUpdateDto)
        {
            foreach (var item in purchaseOrderStatusUpdateDto)
            {
                var poItem = await _poItemsRepository.GetPoItemDetailsByPONumberandItemNo(item.ItemNumber, item.PONumber, item.PoItemId);

                if (poItem.BalanceQty == poItem.Qty)
                {
                    poItem.PoStatus = PoStatus.Open;
                }
                else if (poItem.BalanceQty < poItem.Qty && poItem.BalanceQty > 0)
                {
                    poItem.PoStatus = PoStatus.PartiallyClosed;
                }
                else
                {
                    poItem.PoStatus = PoStatus.Closed;
                }

                await _poItemsRepository.UpdatePOOrderItem(poItem);

            }
            _poItemsRepository.SaveAsync();

            var poItemsPartiallyClosedStatusCount = await _poItemsRepository.GetPoItemsPartiallyClosedStatusCount(purchaseOrderStatusUpdateDto[0].PONumber);

            if (poItemsPartiallyClosedStatusCount != 0)
            {
                var purchaseOrderDetails = await _repository.GetLastestPurchaseOrderByPONumber(purchaseOrderStatusUpdateDto[0].PONumber);
                purchaseOrderDetails.PoStatus = PoStatus.PartiallyClosed;
                await _repository.UpdatePurchaseOrder_ForApproval(purchaseOrderDetails);

            }
            else
            {
                var purchaseOrderDetails = await _repository.GetLastestPurchaseOrderByPONumber(purchaseOrderStatusUpdateDto[0].PONumber);
                purchaseOrderDetails.PoStatus = PoStatus.Closed;
                await _repository.UpdatePurchaseOrder_ForApproval(purchaseOrderDetails);
            }
            _repository.SaveAsync();

            return Ok();
        }
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdatePurchaseOrder(int id, [FromBody] PurchaseOrderUpdateDto purchaseOrderUpdateDto)
        //{
        //    ServiceResponse<PurchaseOrderUpdateDto> serviceResponse = new ServiceResponse<PurchaseOrderUpdateDto>();
        //    try
        //    {
        //        if (purchaseOrderUpdateDto is null)
        //        {
        //            _logger.LogError("Update PurchaseOrder object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Update PurchaseOrder object is null.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid Update PurchaseOrder object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid Update PurchaseOrder object.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        var purchaseOrderDetailbyId = await _repository.GetPurchaseOrderById(id);
        //        if (purchaseOrderDetailbyId is null)
        //        {
        //            _logger.LogError($"Update PurchaseOrder with id: {id}, hasn't been found in db.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"Update PurchaseOrder hasn't been found.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }

        //        var purchaseOrderDetails= _mapper.Map<PurchaseOrder>(purchaseOrderDetailbyId);
        //        var poItemDto = purchaseOrderUpdateDto.POItems;
        //        var poItemList = new List<PoItem>();

        //        if (poItemDto != null)
        //        {
        //            for (int i = 0; i < poItemDto.Count; i++)
        //            {
        //                PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);
        //                poItemDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
        //                poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);
        //                poItemList.Add(poItemDetails);
        //            }
        //        }

        //        purchaseOrderDetails.POItems = poItemList;
        //        var updatePurchaseOrder = _mapper.Map(purchaseOrderUpdateDto, purchaseOrderDetails);
        //        string result = await _repository.UpdatePurchaseOrder(updatePurchaseOrder);
        //        _logger.LogInfo(result);
        //        _repository.SaveAsync();
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = " PurchaseOrder Successfully Updated";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside UpdatePurchaseOrder action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong ,try again";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();
            try
            {
                var purchaseOrderDetailbyId = await _repository.GetPurchaseOrderById(id);
                if (purchaseOrderDetailbyId == null)
                {
                    _logger.LogError($"Delete PurchaseOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeletePurchaseOrder(purchaseOrderDetailbyId);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseOrder Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeletePurchaseOrder API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeletePurchaseOrder API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivePurchaseOrderNameList()
        {
            ServiceResponse<List<PONameList>> serviceResponse = new ServiceResponse<List<PONameList>>();
            try
            {
                var activePONameList = await _repository.GetAllActivePurchaseOrderNameList();

                serviceResponse.Data = activePONameList;
                serviceResponse.Message = "Returned all ActivePurchaseOrderNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActivePurchaseOrderNameList : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActivePurchaseOrderNameList : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrderNameList()
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var poNumberList = await _repository.GetAllPurchaseOrderNameList();
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(poNumberList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseOrderNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPurchaseOrderNameList : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPurchaseOrderNameList : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllLatestRevNoPurchaseOrderNameList()
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var poNumberList = await _repository.GetAllLatestRevNoPurchaseOrderNameList();
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(poNumberList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseOrderNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLatestRevNoPurchaseOrderNameList : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLatestRevNoPurchaseOrderNameList : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseOrderApprovalINameList()
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pendingPOApprovalINameList = await _repository.GetAllPendingPOApprovalINameList();

                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pendingPOApprovalINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPendingPurchaseOrderApprovalINameList : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPendingPurchaseOrderApprovalINameList : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseOrderApprovalIINameList()
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pendingPOApprovalIINameList = await _repository.GetAllPendingPOApprovalIINameList();
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pendingPOApprovalIINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIIPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPendingPurchaseOrderApprovalIINameList : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPendingPurchaseOrderApprovalIINameList : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseOrderApprovalIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pendingPOApprovalINameList = await _repository.GetAllPendingPOApprovalIList(pagingParameter, searchParams);
                var metadata = new
                {
                    pendingPOApprovalINameList.TotalCount,
                    pendingPOApprovalINameList.PageSize,
                    pendingPOApprovalINameList.CurrentPage,
                    pendingPOApprovalINameList.HasNext,
                    pendingPOApprovalINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pendingPOApprovalINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPendingPurchaseOrderApprovalIList : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPendingPurchaseOrderApprovalIList : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLastestPendingPurchaseOrderApprovalIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var lastestPendingPOApprovalINameList = await _repository.GetAllLastestPendingPOApprovalIList(pagingParameter, searchParams);
                var metadata = new
                {
                    lastestPendingPOApprovalINameList.TotalCount,
                    lastestPendingPOApprovalINameList.PageSize,
                    lastestPendingPOApprovalINameList.CurrentPage,
                    lastestPendingPOApprovalINameList.HasNext,
                    lastestPendingPOApprovalINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(lastestPendingPOApprovalINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LastestPendingApprovalIPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLastestPendingPurchaseOrderApprovalIList : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLastestPendingPurchaseOrderApprovalIList : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseOrderApprovalIIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pendingPOApprovalIINameList = await _repository.GetAllPendingPOApprovalIIList(pagingParameter, searchParams);
                var metadata = new
                {
                    pendingPOApprovalIINameList.TotalCount,
                    pendingPOApprovalIINameList.PageSize,
                    pendingPOApprovalIINameList.CurrentPage,
                    pendingPOApprovalIINameList.HasNext,
                    pendingPOApprovalIINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pendingPOApprovalIINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIIPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPendingPurchaseOrderApprovalIIList : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPendingPurchaseOrderApprovalIIList : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseOrderApprovalIIIListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pendingPOApprovalIINameList = await _repository.GetAllPendingPOApprovalIIIListForAvision(pagingParameter, searchParams);
                var metadata = new
                {
                    pendingPOApprovalIINameList.TotalCount,
                    pendingPOApprovalIINameList.PageSize,
                    pendingPOApprovalIINameList.CurrentPage,
                    pendingPOApprovalIINameList.HasNext,
                    pendingPOApprovalIINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pendingPOApprovalIINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIIIPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPendingPurchaseOrderApprovalIIIListForAvision : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPendingPurchaseOrderApprovalIIIListForAvision : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseOrderApprovalIVListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pendingPOApprovalIINameList = await _repository.GetAllPendingPOApprovalIVListForAvision(pagingParameter, searchParams);
                var metadata = new
                {
                    pendingPOApprovalIINameList.TotalCount,
                    pendingPOApprovalIINameList.PageSize,
                    pendingPOApprovalIINameList.CurrentPage,
                    pendingPOApprovalIINameList.HasNext,
                    pendingPOApprovalIINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pendingPOApprovalIINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIVPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPendingPurchaseOrderApprovalIVListForAvision : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPendingPurchaseOrderApprovalIVListForAvision : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLastestPendingPurchaseOrderApprovalIIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var lastestPendingPOApprovalIINameList = await _repository.GetAllLastestPendingPOApprovalIIList(pagingParameter, searchParams);
                var metadata = new
                {
                    lastestPendingPOApprovalIINameList.TotalCount,
                    lastestPendingPOApprovalIINameList.PageSize,
                    lastestPendingPOApprovalIINameList.CurrentPage,
                    lastestPendingPOApprovalIINameList.HasNext,
                    lastestPendingPOApprovalIINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(lastestPendingPOApprovalIINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LastestPendingApprovalIIPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLastestPendingPurchaseOrderApprovalIIList : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLastestPendingPurchaseOrderApprovalIIList : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLastestPendingPOApprovalIIIListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var lastestPendingPOApprovalIINameList = await _repository.GetAllLastestPendingPOApprovalIIIListForAvision(pagingParameter, searchParams);
                var metadata = new
                {
                    lastestPendingPOApprovalIINameList.TotalCount,
                    lastestPendingPOApprovalIINameList.PageSize,
                    lastestPendingPOApprovalIINameList.CurrentPage,
                    lastestPendingPOApprovalIINameList.HasNext,
                    lastestPendingPOApprovalIINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(lastestPendingPOApprovalIINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LastestPendingApprovalIIIPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLastestPendingPOApprovalIIIListForAvision : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLastestPendingPOApprovalIIIListForAvision : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLastestPendingPOApprovalIVListForAvision([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var lastestPendingPOApprovalIINameList = await _repository.GetAllLastestPendingPOApprovalIVListForAvision(pagingParameter, searchParams);
                var metadata = new
                {
                    lastestPendingPOApprovalIINameList.TotalCount,
                    lastestPendingPOApprovalIINameList.PageSize,
                    lastestPendingPOApprovalIINameList.CurrentPage,
                    lastestPendingPOApprovalIINameList.HasNext,
                    lastestPendingPOApprovalIINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(lastestPendingPOApprovalIINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LastestPendingApprovalIVPurchaseOrder";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLastestPendingPOApprovalIVListForAvision : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLastestPendingPOApprovalIVListForAvision : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActivatePurchaseOrderApprovalI([FromBody] PurchaseOrderApprovalIDto approvalDto)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
                if (approvalDto == null || string.IsNullOrEmpty(approvalDto.PONumber))
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid approval data provided";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                string serverKey = GetServerKey();
                var purchaseOrderDetailByPONumber = await _repository.GetAllPurchaseOrderbyPurchaseOrderNumber(approvalDto.PONumber);
                if (purchaseOrderDetailByPONumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrderApprovalI object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseOrderApprovalI with string: {approvalDto.PONumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }

                purchaseOrderDetailByPONumber[0].POApprovalI = true;
                purchaseOrderDetailByPONumber[0].POApprovedIBy = _createdBy;
                purchaseOrderDetailByPONumber[0].POApprovedIDate = DateTime.Now;
                purchaseOrderDetailByPONumber[0].POApprovalIRemarks = approvalDto.POApprovalIRemarks;
                purchaseOrderDetailByPONumber.ForEach(x => x.InApproval = true);
                foreach (var po in purchaseOrderDetailByPONumber) await _repository.UpdatePurchaseOrder_ForApproval(po);
                _logger.LogInfo($"ActivatePurchaseOrderApprovalI for PO: {approvalDto.PONumber}");
                _repository.SaveAsync();
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,Approval1";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);
                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    //var mails = new List<string>() {"bala@avisionsystems.com","anilyadav@avisionsystems.com" };
                    var mails = (emaildetails1.data.Where(x => x.operation == "Approval1").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));
                    //email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    var podate = purchaseOrderDetailByPONumber[0].PODate.ToString().Split(" ");
                    email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));

                    email.Subject = emaildetails.data.subject;
                    string body = emaildetails.data.template;
                    body = body.Replace("{{PO Number}}", purchaseOrderDetailByPONumber[0].PONumber);
                    body = body.Replace("{{PO Revision No}}", purchaseOrderDetailByPONumber[0].RevisionNumber.ToString());
                    body = body.Replace("{{PO Date}}", podate[0]);
                    body = body.Replace("{{PO Value}}", purchaseOrderDetailByPONumber[0].TotalAmount.ToString());
                    body = body.Replace("{{Vendor Name}}", purchaseOrderDetailByPONumber[0].VendorName.ToString());
                    body = body.Replace("{{Approval1}}", purchaseOrderDetailByPONumber[0].POApprovedIBy);
                    body = body.Replace("{{Approval2}}", "Awaiting");
                    body = body.Replace("{{Approval3}}", "Awaiting");
                    body = body.Replace("{{Approval4}}", "Awaiting");
                    body = body.Replace("{{PO Conf}}", "Awaiting");
                    string? ProjectNos = null;
                    List<string>? tempProj = new List<string>();
                    List<string>? tempPRno = new List<string>();
                    string? PRNo = null;
                    foreach (var item in purchaseOrderDetailByPONumber[0].POItems)
                    {

                        if (item.POAddprojects.Count > 0)
                            foreach (var project in item.POAddprojects)
                            {
                                if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                            }
                        if (item.PrDetails.Count > 0)
                            foreach (var pr in item.PrDetails)
                            {
                                if (PRNo.IsNullOrEmpty()) { PRNo = pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                                else if (!tempPRno.Contains(pr.PRNumber)) { PRNo = PRNo + ", " + pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                            }

                    }
                    body = body.Replace("{{Project No}}", ProjectNos);
                    body = body.Replace("{{PR Numbers}}", PRNo);

                    email.Body = new TextPart(TextFormat.Html) { Text = body };

                    using var smtp = new MailKit.Net.Smtp.SmtpClient();
                    int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                    smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                    smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));


                    smtp.Send(email);
                    smtp.Disconnect(true);

                }
                serviceResponse.Message = "PurchaseOrderApprovalI Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ActivatePurchaseOrderApprovalI API for the following PONumber:{approvalDto?.PONumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in ActivatePurchaseOrderApprovalI API for the following PONumber:{approvalDto?.PONumber} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut]
        public async Task<IActionResult> ActivatePurchaseOrderApprovalII([FromBody] PurchaseOrderApprovalIIDto approvalDto)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
                if (approvalDto == null || string.IsNullOrEmpty(approvalDto.PONumber))
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid approval data provided";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                string serverKey = GetServerKey();
                var purchaseOrderDetailByPONumber = await _repository.GetLastestPurchaseOrderByPONumber(approvalDto.PONumber);
                if (purchaseOrderDetailByPONumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrderApprovalII object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseOrderApprovalII with string: {approvalDto.PONumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchaseOrderDetailByPONumber.POApprovalII = true;
                purchaseOrderDetailByPONumber.POApprovedIIBy = _createdBy;
                purchaseOrderDetailByPONumber.POApprovedIIDate = DateTime.Now;
                purchaseOrderDetailByPONumber.POApprovalIIRemarks = approvalDto.POApprovalIIRemarks;
                string result = await _repository.UpdatePurchaseOrder_ForApproval(purchaseOrderDetailByPONumber);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);


                    if (purchaseOrderDetailByPONumber.ApprovalCount > 2)
                    {
                        var Operations = "From,Approval2";
                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                        request1.Headers.Add("Authorization", token);
                        var response1 = await client.SendAsync(request1);
                        var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                        var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);

                        var httpclientHandler = new HttpClientHandler();
                        var httpClient = new HttpClient(httpclientHandler);
                        var email = new MimeMessage();
                        email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));

                        // email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                        var podate = purchaseOrderDetailByPONumber.PODate.ToString().Split(" ");
                        //var mails = "venkat.k@avisionsystems.com";
                        var mails = (emaildetails1.data.Where(x => x.operation == "Approval2").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                        email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));
                        //email.To.Add(MailboxAddress.Parse(mails));
                        email.Subject = emaildetails.data.subject;
                        string body = emaildetails.data.template;
                        body = body.Replace("{{PO Number}}", purchaseOrderDetailByPONumber.PONumber);
                        body = body.Replace("{{PO Revision No}}", purchaseOrderDetailByPONumber.RevisionNumber.ToString());
                        body = body.Replace("{{PO Date}}", podate[0]);
                        body = body.Replace("{{PO Value}}", purchaseOrderDetailByPONumber.TotalAmount.ToString());
                        body = body.Replace("{{Vendor Name}}", purchaseOrderDetailByPONumber.VendorName.ToString());
                        body = body.Replace("{{Approval1}}", purchaseOrderDetailByPONumber.POApprovedIBy);
                        body = body.Replace("{{Approval2}}", purchaseOrderDetailByPONumber.POApprovedIIBy);
                        body = body.Replace("{{Approval3}}", "Awaiting");
                        body = body.Replace("{{Approval4}}", "Awaiting");
                        body = body.Replace("{{PO Conf}}", "Awaiting");
                        string? ProjectNos = null;
                        List<string>? tempProj = new List<string>();
                        List<string>? tempPRno = new List<string>();
                        string? PRNo = null;
                        foreach (var item in purchaseOrderDetailByPONumber.POItems)
                        {

                            if (item.POAddprojects.Count > 0)
                                foreach (var project in item.POAddprojects)
                                {
                                    if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                    else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                }
                            if (item.PrDetails.Count > 0)
                                foreach (var pr in item.PrDetails)
                                {
                                    if (PRNo.IsNullOrEmpty()) { PRNo = pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                                    else if (!tempPRno.Contains(pr.PRNumber)) { PRNo = PRNo + ", " + pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                                }

                        }
                        body = body.Replace("{{Project No}}", ProjectNos);
                        body = body.Replace("{{PR Numbers}}", PRNo);

                        email.Body = new TextPart(TextFormat.Html) { Text = body };

                        using var smtp = new MailKit.Net.Smtp.SmtpClient();
                        //smtp.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
                        //smtp.Authenticate("erp@avisionsystems.com", "R#9183753474150W");
                        int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                        smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                        smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                        smtp.Send(email);
                        smtp.Disconnect(true);
                    }
                    else
                    {
                        var Operations = "From,Approval2count2";
                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                        request1.Headers.Add("Authorization", token);
                        var response1 = await client.SendAsync(request1);
                        var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                        var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);

                        var httpclientHandler = new HttpClientHandler();
                        var httpClient = new HttpClient(httpclientHandler);
                        var email = new MimeMessage();
                        email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));

                        // email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                        var podate = purchaseOrderDetailByPONumber.PODate.ToString().Split(" ");
                        //var mails = new List<string>() { "scm@avisionsystems.com", "purchase@avisionsystems.com" };
                        //email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));
                        var mails = (emaildetails1.data.Where(x => x.operation == "Approval2count2").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                        email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));
                        email.Subject = emaildetails.data.subject;
                        string body = emaildetails.data.template;
                        body = body.Replace("{{PO Number}}", purchaseOrderDetailByPONumber.PONumber);
                        body = body.Replace("{{PO Revision No}}", purchaseOrderDetailByPONumber.RevisionNumber.ToString());
                        body = body.Replace("{{PO Date}}", podate[0]);
                        body = body.Replace("{{PO Value}}", purchaseOrderDetailByPONumber.TotalAmount.ToString());
                        body = body.Replace("{{Vendor Name}}", purchaseOrderDetailByPONumber.VendorName.ToString());
                        body = body.Replace("{{Approval1}}", purchaseOrderDetailByPONumber.POApprovedIBy);
                        body = body.Replace("{{Approval2}}", purchaseOrderDetailByPONumber.POApprovedIIBy);
                        body = body.Replace("{{Approval3}}", "--");
                        body = body.Replace("{{Approval4}}", "--");
                        body = body.Replace("{{PO Conf}}", "Awaiting");
                        string? ProjectNos = null;
                        List<string>? tempProj = new List<string>();
                        List<string>? tempPRno = new List<string>();
                        string? PRNo = null;
                        foreach (var item in purchaseOrderDetailByPONumber.POItems)
                        {

                            if (item.POAddprojects.Count > 0)
                                foreach (var project in item.POAddprojects)
                                {
                                    if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                    else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                }
                            if (item.PrDetails.Count > 0)
                                foreach (var pr in item.PrDetails)
                                {
                                    if (PRNo.IsNullOrEmpty()) { PRNo = pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                                    else if (!tempPRno.Contains(pr.PRNumber)) { PRNo = PRNo + ", " + pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                                }

                        }
                        body = body.Replace("{{Project No}}", ProjectNos);
                        body = body.Replace("{{PR Numbers}}", PRNo);

                        email.Body = new TextPart(TextFormat.Html) { Text = body };

                        using var smtp = new MailKit.Net.Smtp.SmtpClient();
                        int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                        smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                        smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                        smtp.Send(email);
                        smtp.Disconnect(true);
                    }



                }
                serviceResponse.Message = "PurchaseOrderApprovalII Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ActivatePurchaseOrderApprovalII API for the following PONumber:{approvalDto?.PONumber} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in ActivatePurchaseOrderApprovalII API for the following PONumber:{approvalDto?.PONumber} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActivatePurchaseOrderApprovalIIIForAvision([FromBody] PurchaseOrderApprovalIIIDto approvalDto)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
                if (approvalDto == null || string.IsNullOrEmpty(approvalDto.PONumber))
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid approval data provided";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                string serverKey = GetServerKey();
                var purchaseOrderDetailByPONumber = await _repository.GetLastestPurchaseOrderByPONumber(approvalDto.PONumber);
                if (purchaseOrderDetailByPONumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrderApprovalIII object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseOrderApprovalIII with string: {approvalDto.PONumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchaseOrderDetailByPONumber.POApprovalIII = true;
                purchaseOrderDetailByPONumber.POApprovedIIIBy = _createdBy;
                purchaseOrderDetailByPONumber.POApprovedIIIDate = DateTime.Now;
                purchaseOrderDetailByPONumber.POApprovalIIIRemarks = approvalDto.POApprovalIIIRemarks;
                string result = await _repository.UpdatePurchaseOrder_ForApproval(purchaseOrderDetailByPONumber);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,Approval3";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);

                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    //var mails =new List<string>() { "eyalbn@uvisionuav.com", "yonatan@uvisionuav.com"};
                    var mails = (emaildetails1.data.Where(x => x.operation == "Approval3").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                    var email = new MimeMessage();
                    //email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));
                    var podate = purchaseOrderDetailByPONumber.PODate.ToString().Split(" ");
                    email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));

                    email.Subject = emaildetails.data.subject;
                    string body = emaildetails.data.template;
                    body = body.Replace("{{PO Number}}", purchaseOrderDetailByPONumber.PONumber);
                    body = body.Replace("{{PO Revision No}}", purchaseOrderDetailByPONumber.RevisionNumber.ToString());
                    body = body.Replace("{{PO Date}}", podate[0]);
                    body = body.Replace("{{PO Value}}", purchaseOrderDetailByPONumber.TotalAmount.ToString());
                    body = body.Replace("{{Vendor Name}}", purchaseOrderDetailByPONumber.VendorName.ToString());
                    body = body.Replace("{{Approval1}}", purchaseOrderDetailByPONumber.POApprovedIBy);
                    body = body.Replace("{{Approval2}}", purchaseOrderDetailByPONumber.POApprovedIIBy);
                    body = body.Replace("{{Approval3}}", purchaseOrderDetailByPONumber.POApprovedIIIBy);
                    body = body.Replace("{{Approval4}}", "Awaiting");
                    body = body.Replace("{{PO Conf}}", "Awaiting");
                    string? ProjectNos = null;
                    List<string>? tempProj = new List<string>();
                    List<string>? tempPRno = new List<string>();
                    string? PRNo = null;
                    foreach (var item in purchaseOrderDetailByPONumber.POItems)
                    {

                        if (item.POAddprojects.Count > 0)
                            foreach (var project in item.POAddprojects)
                            {
                                if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                            }
                        if (item.PrDetails.Count > 0)
                            foreach (var pr in item.PrDetails)
                            {
                                if (PRNo.IsNullOrEmpty()) { PRNo = pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                                else if (!tempPRno.Contains(pr.PRNumber)) { PRNo = PRNo + ", " + pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                            }

                    }
                    body = body.Replace("{{Project No}}", ProjectNos);
                    body = body.Replace("{{PR Numbers}}", PRNo);

                    email.Body = new TextPart(TextFormat.Html) { Text = body };

                    using var smtp = new MailKit.Net.Smtp.SmtpClient();
                    int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                    smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                    smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                    smtp.Send(email);
                    smtp.Disconnect(true);

                }
                serviceResponse.Message = "PurchaseOrderApprovalIII Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ActivatePurchaseOrderApprovalIIIForAvision API for the following PONumber:{approvalDto?.PONumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in ActivatePurchaseOrderApprovalIIIForAvision API for the following PONumber:{approvalDto?.PONumber} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> ActivatePurchaseOrderApprovalIVForAvision([FromBody] PurchaseOrderApprovalIVDto approvalDto)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
                if (approvalDto == null || string.IsNullOrEmpty(approvalDto.PONumber))
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid approval data provided";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                string serverKey = GetServerKey();
                var purchaseOrderDetailByPONumber = await _repository.GetLastestPurchaseOrderByPONumber(approvalDto.PONumber);
                if (purchaseOrderDetailByPONumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrderApprovalIV object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseOrderApprovalIV with string: {approvalDto.PONumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchaseOrderDetailByPONumber.POApprovalIV = true;
                purchaseOrderDetailByPONumber.POApprovedIVBy = _createdBy;
                purchaseOrderDetailByPONumber.POApprovedIVDate = DateTime.Now;
                purchaseOrderDetailByPONumber.POApprovalIVRemarks = approvalDto.POApprovalIVRemarks;
                string result = await _repository.UpdatePurchaseOrder_ForApproval(purchaseOrderDetailByPONumber);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,Approval4";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);

                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    // var mails = new List<string>() { "scm@avisionsystems.com", "purchase@avisionsystems.com"};
                    var mails = (emaildetails1.data.Where(x => x.operation == "Approval4").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                    var email = new MimeMessage();
                    //email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));
                    var podate = purchaseOrderDetailByPONumber.PODate.ToString().Split(" ");
                    email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));

                    email.Subject = emaildetails.data.subject;
                    string body = emaildetails.data.template;
                    body = body.Replace("{{PO Number}}", purchaseOrderDetailByPONumber.PONumber);
                    body = body.Replace("{{PO Revision No}}", purchaseOrderDetailByPONumber.RevisionNumber.ToString());
                    body = body.Replace("{{PO Date}}", podate[0]);
                    body = body.Replace("{{PO Value}}", purchaseOrderDetailByPONumber.TotalAmount.ToString());
                    body = body.Replace("{{Vendor Name}}", purchaseOrderDetailByPONumber.VendorName.ToString());
                    body = body.Replace("{{Approval1}}", purchaseOrderDetailByPONumber.POApprovedIBy);
                    body = body.Replace("{{Approval2}}", purchaseOrderDetailByPONumber.POApprovedIIBy);
                    body = body.Replace("{{Approval3}}", purchaseOrderDetailByPONumber.POApprovedIIIBy);
                    body = body.Replace("{{Approval4}}", purchaseOrderDetailByPONumber.POApprovedIVBy);
                    body = body.Replace("{{PO Conf}}", "Awaiting");
                    string? ProjectNos = null;
                    List<string>? tempProj = new List<string>();
                    List<string>? tempPRno = new List<string>();
                    string? PRNo = null;
                    foreach (var item in purchaseOrderDetailByPONumber.POItems)
                    {

                        if (item.POAddprojects.Count > 0)
                            foreach (var project in item.POAddprojects)
                            {
                                if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                            }
                        if (item.PrDetails.Count > 0)
                            foreach (var pr in item.PrDetails)
                            {
                                if (PRNo.IsNullOrEmpty()) { PRNo = pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                                else if (!tempPRno.Contains(pr.PRNumber)) { PRNo = PRNo + ", " + pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                            }

                    }
                    body = body.Replace("{{Project No}}", ProjectNos);
                    body = body.Replace("{{PR Numbers}}", PRNo);

                    email.Body = new TextPart(TextFormat.Html) { Text = body };

                    using var smtp = new MailKit.Net.Smtp.SmtpClient();
                    //smtp.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
                    //smtp.Authenticate("erp@avisionsystems.com", "R#9183753474150W");
                    int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                    smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                    smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                    smtp.Send(email);
                    smtp.Disconnect(true);

                }
                serviceResponse.Message = "PurchaseOrderApprovalIV Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ActivatePurchaseOrderApprovalIVForAvision API for the following PONumber:{approvalDto?.PONumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in ActivatePurchaseOrderApprovalIVForAvision API for the following PONumber:{approvalDto?.PONumber} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{VendorName}")]
        public async Task<IActionResult> GetAllPoNumberListByVendorName(string VendorName)
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var pONumberDetailsbyVendorName = await _repository.GetAllPONumberListByVendorName(VendorName);
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(pONumberDetailsbyVendorName);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PONumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPoNumberListByVendorName API for the following VendorName:{VendorName} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPoNumberListByVendorName API for the following VendorName:{VendorName} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{PoNumber}")]
        public async Task<IActionResult> GetAllPoItemNumberListByPoNumber(string PoNumber)
        {
            ServiceResponse<IEnumerable<PurchaseOrderItemNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderItemNoListDto>>();
            try
            {
                var pOItemNumberDetailsbyPONumber = await _repository.GetAllPOItemNumberListByPoNumber(PoNumber);
                var result = _mapper.Map<IEnumerable<PurchaseOrderItemNoListDto>>(pOItemNumberDetailsbyPONumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all POItemNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPoItemNumberListByPoNumber API for the following PoNumber:{PoNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPoItemNumberListByPoNumber API for the following PoNumber:{PoNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ShortClosePurchaseOrder(int id)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
                var purchaseOrderDetailById = await _repository.GetPurchaseOrderById(id);
                if (purchaseOrderDetailById == null)
                {
                    _logger.LogError($"PurchaseOrder with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseOrder with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                purchaseOrderDetailById.IsShortClosed = true;
                purchaseOrderDetailById.ShortClosedBy = _createdBy;
                purchaseOrderDetailById.ShortClosedOn = DateTime.Now;
                string result = await _repository.UpdatePurchaseOrder_ForApproval(purchaseOrderDetailById);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "PurchaseOrder have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in ShortClosePurchaseOrder API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ShortClosePurchaseOrder API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PoConfirmationStatus(List<List<PoItemConfirmationDateDto>> poItemConfirmationDateDto)
        {
            ServiceResponse<PoItemConfirmationDateDto> serviceResponse = new ServiceResponse<PoItemConfirmationDateDto>();

            try
            {
                string serverKey = GetServerKey();
                //var poid_1 = poItemConfirmationDateDto.First();
                //var PoId = poid_1.First();
                //var PODetails = await _repository.GetPurchaseOrderById(PoId.PoId);

                int PoId = 0;

                foreach (var poid_1 in poItemConfirmationDateDto)
                {
                    if (poid_1.Any())
                    {
                        var poItem = poid_1.First();
                        if (poItem != null)
                        {
                            PoId = poItem.PoId;
                            break;
                        }
                    }
                }
                var PODetails = await _repository.GetPurchaseOrderById(PoId);
                foreach (var poItemConfirmationDateSet in poItemConfirmationDateDto)
                {
                    if (!poItemConfirmationDateSet.Any())
                    {
                        // Skip empty sets
                        continue;
                    }

                    var firstItemInSet = poItemConfirmationDateSet.First(); // Get the first item in the set

                    var purchaseOrderDetailById = await _repository.GetPurchaseOrderById(firstItemInSet.PoId);
                    if (purchaseOrderDetailById == null)
                    {
                        _logger.LogError($"PurchaseOrder with PoId: {firstItemInSet.PoId}, hasn't been found in db.");
                        serviceResponse.Message = $"PurchaseOrder with PoId hasn't been found.";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(serviceResponse);
                    }

                    //Updating PoConfirmationDate table
                    var poItemConfirmationDateDtoDetails = await _poConfirmationDateRepository.GetPoConfirmationDateDetailsById(firstItemInSet.POItemDetailId);

                    if (poItemConfirmationDateDtoDetails.Count() == 0)
                    {
                        var poConfirmationDateDtoDetails = poItemConfirmationDateSet
                            .Select(poConfirmationDate => new PoConfirmationDate
                            {
                                ConfirmationDate = poConfirmationDate.ConfirmationDate,
                                Qty = poConfirmationDate.Qty,
                                POItemDetailId = poConfirmationDate.POItemDetailId
                            })
                            .ToList();

                        await _poConfirmationDateRepository.CreatePoConfirmationDateList(poConfirmationDateDtoDetails);
                        _poConfirmationDateRepository.SaveAsync();
                    }

                    else
                    {
                        await _poConfirmationDateRepository.DeletePoConfirmationDateList(poItemConfirmationDateDtoDetails);
                        _poConfirmationDateRepository.SaveAsync();

                        var poItemConfirmationDateDtoList = await _poConfirmationDateRepository.GetPoConfirmationDateDetailsById(firstItemInSet.POItemDetailId);

                        if (poItemConfirmationDateDtoList.Count() == 0)
                        {
                            var poConfirmationDateDtoDetails = poItemConfirmationDateSet
                                .Select(poConfirmationDate => new PoConfirmationDate
                                {
                                    ConfirmationDate = poConfirmationDate.ConfirmationDate,
                                    Qty = poConfirmationDate.Qty,
                                    POItemDetailId = poConfirmationDate.POItemDetailId
                                })
                                .ToList();

                            await _poConfirmationDateRepository.CreatePoConfirmationDateList(poConfirmationDateDtoDetails);
                            _poConfirmationDateRepository.SaveAsync();
                        }
                        else
                        {
                            serviceResponse.Data = null;
                            serviceResponse.Message = "PoConfirmationDateDetails have been Found";
                            serviceResponse.Success = true;
                            serviceResponse.StatusCode = HttpStatusCode.OK;
                            return Ok(serviceResponse);
                        }

                    }

                    //Update PoConfirmationHistory Table
                    var poItemConfirmationDateDetails = await _poConfirmationDateRepository.GetPoConfirmationDateDetailsById(firstItemInSet.POItemDetailId);
                    if (poItemConfirmationDateDetails != null)
                    {
                        foreach (var poConfirmationDate in poItemConfirmationDateDetails)
                        {
                            var purchaseOrderItemDetailById = await _poItemsRepository.GetPoItemDetailsById(firstItemInSet.POItemDetailId);
                            PoConfirmationHistory poConfirmationHistory = new PoConfirmationHistory();
                            poConfirmationHistory.ItemNumber = purchaseOrderItemDetailById.ItemNumber;
                            poConfirmationHistory.MftrItemNumber = purchaseOrderItemDetailById.MftrItemNumber;
                            poConfirmationHistory.Description = purchaseOrderItemDetailById.Description;
                            poConfirmationHistory.UOM = purchaseOrderItemDetailById.UOM;
                            poConfirmationHistory.UnitPrice = purchaseOrderItemDetailById.UnitPrice;
                            poConfirmationHistory.Qty = poConfirmationDate.Qty;
                            poConfirmationHistory.ConfirmationDate = poConfirmationDate.ConfirmationDate;
                            poConfirmationHistory.PONumber = purchaseOrderItemDetailById.PONumber;
                            poConfirmationHistory.BalanceQty = purchaseOrderItemDetailById.BalanceQty;
                            poConfirmationHistory.ReceivedQty = purchaseOrderItemDetailById.ReceivedQty;
                            poConfirmationHistory.PartType = purchaseOrderItemDetailById.PartType;
                            poConfirmationHistory.SpecialInstruction = purchaseOrderItemDetailById.SpecialInstruction;
                            poConfirmationHistory.IsTechnicalDocsRequired = purchaseOrderItemDetailById.IsTechnicalDocsRequired;
                            poConfirmationHistory.PoPartsStatus = purchaseOrderItemDetailById.PoPartsStatus;
                            poConfirmationHistory.PoStatus = purchaseOrderItemDetailById.PoStatus;
                            poConfirmationHistory.SGST = purchaseOrderItemDetailById.SGST;
                            poConfirmationHistory.CGST = purchaseOrderItemDetailById.CGST;
                            poConfirmationHistory.IGST = purchaseOrderItemDetailById.IGST;
                            poConfirmationHistory.UTGST = purchaseOrderItemDetailById.UTGST;
                            poConfirmationHistory.SubTotal = purchaseOrderItemDetailById.SubTotal;
                            poConfirmationHistory.TotalWithTax = purchaseOrderItemDetailById.TotalWithTax;
                            poConfirmationHistory.CreatedBy = _createdBy;
                            poConfirmationHistory.CreatedOn = DateTime.Now;
                            //poConfirmationHistory.LastModifiedBy = purchaseOrderItemDetailById.LastModifiedBy;
                            //poConfirmationHistory.LastModifiedOn = purchaseOrderItemDetailById.LastModifiedOn;

                            await _poConfirmationHistoryRepository.CreatePoConfirmationHistory(poConfirmationHistory);
                        }
                        _poConfirmationHistoryRepository.SaveAsync();
                    }

                    //Update PoConfirmationStatus in POInitialConfirmationDateHistory Table
                    foreach (var poConfirmationDate in poItemConfirmationDateSet)
                    {
                        if (poConfirmationDate.IsInitialConfirmationDateDone == true)
                        {
                            var purchaseOrderItemDetailById = await _poItemsRepository.GetPoItemDetailsById(poConfirmationDate.POItemDetailId);

                            POInitialConfirmationDateHistory poInitialConfirmationDateHistory = new POInitialConfirmationDateHistory();
                            poInitialConfirmationDateHistory.PoId = purchaseOrderItemDetailById.PurchaseOrderId;
                            poInitialConfirmationDateHistory.POItemDetailId = purchaseOrderItemDetailById.Id;
                            poInitialConfirmationDateHistory.PONumber = purchaseOrderItemDetailById.PONumber;
                            poInitialConfirmationDateHistory.InitialConfirmationDate = poConfirmationDate.ConfirmationDate;
                            poInitialConfirmationDateHistory.InitialQty = poConfirmationDate.Qty;

                            await _poInitialConfirmationDateHistoryRepository.CreatePOInitialConfirmationDate(poInitialConfirmationDateHistory);
                            _poInitialConfirmationDateHistoryRepository.SaveAsync();
                        }
                    }

                    //Update PoConfirmationStatus in PurchaseOrder Table
                    purchaseOrderDetailById.PoConfirmationStatus = true;
                    string result = await _repository.UpdatePurchaseOrder_ForApproval(purchaseOrderDetailById);
                    _repository.SaveAsync();
                } // If the loop completes without returning a response, it means all sets were processed successfully
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    // var response = await _httpClient.GetAsync(string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,POConformation";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);

                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    //var mails = new List<string>() { "scm@avisionsystems.com", "purchase@avisionsystems.com", "accounts@avisionsystems.com", "bala@avisionsystems.com" };
                    var mails = (emaildetails1.data.Where(x => x.operation == "POConformation").Select(x => x.emailIds).FirstOrDefault()).Split(',');

                    var email = new MimeMessage();
                    // email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));

                    var podate = PODetails.PODate.ToString().Split(" ");
                    email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));
                    email.Subject = emaildetails.data.subject;
                    string body = emaildetails.data.template;
                    body = body.Replace("{{PO Number}}", PODetails.PONumber);
                    body = body.Replace("{{PO Revision No}}", PODetails.RevisionNumber.ToString());
                    body = body.Replace("{{PO Date}}", podate[0]);
                    body = body.Replace("{{PO Value}}", PODetails.TotalAmount.ToString());
                    body = body.Replace("{{Vendor Name}}", PODetails.VendorName.ToString());
                    body = body.Replace("{{Approval1}}", PODetails.POApprovedIBy);
                    body = body.Replace("{{Approval2}}", PODetails.POApprovedIIBy);
                    body = body.Replace("{{Approval3}}", PODetails.POApprovedIIIBy);
                    body = body.Replace("{{Approval4}}", PODetails.POApprovedIVBy);
                    body = body.Replace("{{PO Conf}}", "P Madhusudhan Rao");
                    string? ProjectNos = null;
                    List<string>? tempProj = new List<string>();
                    List<string>? tempPRno = new List<string>();
                    string? PRNo = null;
                    foreach (var item in PODetails.POItems)
                    {

                        if (item.POAddprojects.Count > 0)
                            foreach (var project in item.POAddprojects)
                            {
                                if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                            }
                        if (item.PrDetails.Count > 0)
                            foreach (var pr in item.PrDetails)
                            {
                                if (PRNo.IsNullOrEmpty()) { PRNo = pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                                else if (!tempPRno.Contains(pr.PRNumber)) { PRNo = PRNo + ", " + pr.PRNumber; tempPRno.Add(pr.PRNumber); }
                            }

                    }
                    body = body.Replace("{{Project No}}", ProjectNos);
                    body = body.Replace("{{PR Numbers}}", PRNo);

                    email.Body = new TextPart(TextFormat.Html) { Text = body };

                    using var smtp = new MailKit.Net.Smtp.SmtpClient();
                    int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                    smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                    smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                    smtp.Send(email);
                    smtp.Disconnect(true);

                }

                serviceResponse.Data = null;
                serviceResponse.Message = "PoConfirmationStatus have been Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in PoConfirmationStatus API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in PoConfirmationStatus API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> ShortClosePoItemSatusByPoItemId(int poItemId, string? ReasonforShortClose)
        {
            ServiceResponse<PoItemsDto> serviceResponse = new ServiceResponse<PoItemsDto>();

            try
            {
                var poItemDetailByPoItemId = await _poItemsRepository.ClosePoItemSatusByPoItemId(poItemId);
                if (poItemDetailByPoItemId == null)
                {
                    _logger.LogError($"PoItem with poItemId: {poItemId}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PoItem with poItemId hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

                string? reasonforShortClose = ReasonforShortClose;
                poItemDetailByPoItemId.ReasonforShortClose = reasonforShortClose;
                poItemDetailByPoItemId.PoStatus = PoStatus.ShortClosed;
                poItemDetailByPoItemId.BalanceQty = 0;
                string result = await _poItemsRepository.UpdatePOOrderItem(poItemDetailByPoItemId);
                _poItemsRepository.SaveAsync();

                var poItemOpenStatuscount = await _poItemsRepository.GetPoItemOpenStatusCount(poItemDetailByPoItemId.PurchaseOrderId);

                if (poItemOpenStatuscount == 0)
                {
                    var poDetails = await _repository.GetPurchaseOrderById(poItemDetailByPoItemId.PurchaseOrderId);
                    poDetails.PoStatus = PoStatus.ShortClosed;
                    await _repository.UpdatePurchaseOrder_ForApproval(poDetails);
                    _repository.SaveAsync();
                }
                else
                {
                    var poDetails = await _repository.GetPurchaseOrderById(poItemDetailByPoItemId.PurchaseOrderId);
                    poDetails.PoStatus = PoStatus.PartiallyClosed;
                    await _repository.UpdatePurchaseOrder_ForApproval(poDetails);
                    _repository.SaveAsync();
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "PoItem Status have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in ShortClosePoItemSatusByPoItemId API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ShortClosePoItemSatusByPoItemId API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet()]
        public async Task<IActionResult> Get_Tras_PurchaseOrderSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<Tras_POSPReport>> serviceResponse = new ServiceResponse<IEnumerable<Tras_POSPReport>>();
            try
            {
                var products = await _repository.Get_Tras_PurchaseOrderSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Tras_PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Tras_PurchaseOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Tras_POSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in Get_Tras_PurchaseOrderSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in Get_Tras_PurchaseOrderSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Get_Tras_PurchaseOrderSPResport([FromQuery] PagingParameter pagingParameter)
        {

            ServiceResponse<IEnumerable<Tras_POSPReport>> serviceResponse = new ServiceResponse<IEnumerable<Tras_POSPReport>>();

            try
            {
                var products = await _repository.Get_Tras_PurchaseOrderSPResport(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all Tras_PurchaseOrderSPReport");
                var result = _mapper.Map<IEnumerable<Tras_POSPReport>>(products);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Tras_PurchaseOrderSPReport Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in Get_Tras_PurchaseOrderSPResport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in Get_Tras_PurchaseOrderSPResport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> Get_Tras_PurchaseOrderSPReportWithParam([FromBody] Tras_POSPReportDTO tras_POSPReport)

        {
            ServiceResponse<IEnumerable<Tras_POSPReport>> serviceResponse = new ServiceResponse<IEnumerable<Tras_POSPReport>>();
            try
            {
                var products = await _repository.Get_Tras_PurchaseOrderSPReportWithParam(tras_POSPReport.VendorName, tras_POSPReport.PONumber, tras_POSPReport.PartNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Tras_PurchaseOrder hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Tras_PurchaseOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var result = _mapper.Map<IEnumerable<Tras_POSPReport>>(products);

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Tras_PurchaseOrder Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in Get_Tras_PurchaseOrderSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in Get_Tras_PurchaseOrderSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPoConfirmationSPReportwithParam([FromQuery] PagingParameter pagingParameter, [FromQuery] string? SearchTerm, [FromBody] PurchaseOrderLimitSPReportDto paramsforPurchase)
        {
            ServiceResponse<IEnumerable<poconfirmation_report_with_pagination_Dto>> serviceResponse = new ServiceResponse<IEnumerable<poconfirmation_report_with_pagination_Dto>>();
            try
            {
                var result = await _repository.GetPoConfirmationLimitSPReportwithParam(paramsforPurchase.ItemNumber, paramsforPurchase.ProjectNumber, paramsforPurchase.PONumbers, paramsforPurchase.VendorName,
                                                                                    paramsforPurchase.POStatus, paramsforPurchase.Approval, paramsforPurchase.RecordType,
                                                                                    paramsforPurchase.Offset, paramsforPurchase.Limit);

                var TotalCount = await _repository.GetAllPurchaseOrderCountForTrans(SearchTerm);

                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" PoConfirmationLimitSPReportwithParam hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PoConfirmationLimitSPReportwithParam hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var metadata = new
                    {
                        TotalCount,
                        pagingParameter.PageSize,
                        CurrentPage = pagingParameter.PageNumber
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned  PoConfirmationLimitSPReportwithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPoConfirmationSPReportwithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPoConfirmationSPReportwithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPoConfirmationSPReportwithDate([FromQuery] PagingParameter pagingParameter, [FromQuery] string? SearchTerm, [FromBody] PurchaseOrderDateLimitSPReportDto purchaseOrderDate_ReportGetDto)
        {
            ServiceResponse<IEnumerable<poconfirmation_report_with_pagination_Dto>> serviceResponse = new ServiceResponse<IEnumerable<poconfirmation_report_with_pagination_Dto>>();
            try
            {
                var result = await _repository.GetPoConfirmationLimitSPReportwithDate(purchaseOrderDate_ReportGetDto.FromDate, purchaseOrderDate_ReportGetDto.ToDate,
                                                                                                purchaseOrderDate_ReportGetDto.Approval, purchaseOrderDate_ReportGetDto.RecordType,
                                                                                                purchaseOrderDate_ReportGetDto.Offset, purchaseOrderDate_ReportGetDto.Limit);

                var TotalCount = await _repository.GetAllPurchaseOrderCountForTrans(SearchTerm);
                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" PoConfirmationLimitSPReportswithDate hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PoConfirmationLimitSPReportswithDate hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var metadata = new
                    {
                        TotalCount,
                        pagingParameter.PageSize,
                        CurrentPage = pagingParameter.PageNumber
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned  PoConfirmationLimitSPReportswithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPoConfirmationSPReportwithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPoConfirmationSPReportwithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> ExportPOReportToExcel([FromBody] PurchaseOrder_ReportGetDto paramsforPurchase)
        //{

        //    try
        //    {
        //        var poConfirmationReports = await _repository.GetPoConfirmationSPReportwithParam(paramsforPurchase.ItemNumber, paramsforPurchase.PONumbers, paramsforPurchase.VendorName,
        //                                                                           paramsforPurchase.POStatus, paramsforPurchase.Approval, paramsforPurchase.RecordType);

        //        var poDeliveryScheduleReports = await _repository.GetPoDeliverySchedulewithParam(paramsforPurchase.ItemNumber, paramsforPurchase.PONumbers, paramsforPurchase.VendorName,
        //                                                                                        paramsforPurchase.POStatus, paramsforPurchase.Approval, paramsforPurchase.RecordType);

        //        var poProjectReports = await _repository.GetPoProjectSPReportwithParam(paramsforPurchase.ItemNumber, paramsforPurchase.PONumbers, paramsforPurchase.VendorName,
        //                                                                               paramsforPurchase.POStatus, paramsforPurchase.Approval, paramsforPurchase.ProjectNumber,
        //                                                                               paramsforPurchase.RecordType);

        //        // Create a new Excel workbook
        //        XSSFWorkbook workbook = new XSSFWorkbook();
        //        ISheet sheet1 = workbook.CreateSheet("POConfirmationReport");
        //        ISheet sheet2 = workbook.CreateSheet("PODeliveryScheduleReport");
        //        ISheet sheet3 = workbook.CreateSheet("PoProjectSPReport");

        //        // Set header row
        //        var headerRow1 = sheet1.CreateRow(0);
        //        headerRow1.CreateCell(0).SetCellValue("Vendor ID");
        //        headerRow1.CreateCell(1).SetCellValue("Vendor Name");
        //        headerRow1.CreateCell(2).SetCellValue("PO Number");
        //        headerRow1.CreateCell(3).SetCellValue("PO Date");
        //        headerRow1.CreateCell(4).SetCellValue("PR Qty");
        //        headerRow1.CreateCell(5).SetCellValue("Revision Number");
        //        headerRow1.CreateCell(6).SetCellValue("Item Number");
        //        headerRow1.CreateCell(7).SetCellValue("Mftr Item Number");
        //        headerRow1.CreateCell(8).SetCellValue("Item Description");
        //        headerRow1.CreateCell(9).SetCellValue("PO Qty");
        //        headerRow1.CreateCell(10).SetCellValue("Received Qty");
        //        headerRow1.CreateCell(11).SetCellValue("Balance Qty");
        //        headerRow1.CreateCell(12).SetCellValue("Currency");
        //        headerRow1.CreateCell(13).SetCellValue("UOM");
        //        headerRow1.CreateCell(14).SetCellValue("Unit Price");
        //        headerRow1.CreateCell(15).SetCellValue("Balance Value");
        //        headerRow1.CreateCell(16).SetCellValue("PO Approved I By");
        //        headerRow1.CreateCell(17).SetCellValue("PO Approved I Date");
        //        headerRow1.CreateCell(18).SetCellValue("PO Approved II By");
        //        headerRow1.CreateCell(19).SetCellValue("PO Approved II Date");
        //        headerRow1.CreateCell(20).SetCellValue("PO Status");
        //        headerRow1.CreateCell(21).SetCellValue("Created By");
        //        headerRow1.CreateCell(22).SetCellValue("Created On");
        //        headerRow1.CreateCell(23).SetCellValue("Confirmation Date");
        //        headerRow1.CreateCell(24).SetCellValue("Confirmation Qty");


        //        var headerRow2 = sheet2.CreateRow(0);
        //        headerRow2.CreateCell(0).SetCellValue("Vendor ID");
        //        headerRow2.CreateCell(1).SetCellValue("Vendor Name");
        //        headerRow2.CreateCell(2).SetCellValue("PO Number");
        //        headerRow2.CreateCell(3).SetCellValue("PO Date");
        //        headerRow2.CreateCell(4).SetCellValue("PR Qty");
        //        headerRow2.CreateCell(5).SetCellValue("Revision Number");
        //        headerRow2.CreateCell(6).SetCellValue("Item Number");
        //        headerRow2.CreateCell(7).SetCellValue("Mftr Item Number");
        //        headerRow2.CreateCell(8).SetCellValue("Item Description");
        //        headerRow2.CreateCell(9).SetCellValue("PO Qty");
        //        headerRow2.CreateCell(10).SetCellValue("Schedule Qty");
        //        headerRow2.CreateCell(11).SetCellValue("Received Qty");
        //        headerRow2.CreateCell(12).SetCellValue("Balance Qty");
        //        headerRow2.CreateCell(13).SetCellValue("Currency");
        //        headerRow2.CreateCell(14).SetCellValue("UOM");
        //        headerRow2.CreateCell(15).SetCellValue("Unit Price");
        //        headerRow2.CreateCell(16).SetCellValue("Balance Value");
        //        headerRow2.CreateCell(17).SetCellValue("PO Approved I By");
        //        headerRow2.CreateCell(18).SetCellValue("PO Approved I Date");
        //        headerRow2.CreateCell(19).SetCellValue("PO Approved II By");
        //        headerRow2.CreateCell(20).SetCellValue("PO Approved II Date");
        //        headerRow2.CreateCell(21).SetCellValue("PO Status");
        //        headerRow2.CreateCell(22).SetCellValue("Created By");
        //        headerRow2.CreateCell(23).SetCellValue("Created On");
        //        headerRow2.CreateCell(24).SetCellValue("Schedule Date");


        //        var headerRow3 = sheet3.CreateRow(0);
        //        headerRow3.CreateCell(0).SetCellValue("Vendor ID");
        //        headerRow3.CreateCell(1).SetCellValue("Vendor Name");
        //        headerRow3.CreateCell(2).SetCellValue("PO Number");
        //        headerRow3.CreateCell(3).SetCellValue("PO Date");
        //        headerRow3.CreateCell(4).SetCellValue("PR Qty");
        //        headerRow3.CreateCell(5).SetCellValue("Revision Number");
        //        headerRow3.CreateCell(6).SetCellValue("Project Number");
        //        headerRow3.CreateCell(7).SetCellValue("Project Qty");
        //        headerRow3.CreateCell(8).SetCellValue("Item Number");
        //        headerRow3.CreateCell(9).SetCellValue("Mftr Item Number");
        //        headerRow3.CreateCell(10).SetCellValue("Item Description");
        //        headerRow3.CreateCell(11).SetCellValue("PO Qty");
        //        headerRow3.CreateCell(12).SetCellValue("Received Qty");
        //        headerRow3.CreateCell(13).SetCellValue("Balance Qty");
        //        headerRow3.CreateCell(14).SetCellValue("Currency");
        //        headerRow3.CreateCell(15).SetCellValue("UOM");
        //        headerRow3.CreateCell(16).SetCellValue("Unit Price");
        //        headerRow3.CreateCell(17).SetCellValue("Balance Value");
        //        headerRow3.CreateCell(18).SetCellValue("PO Approved I By");
        //        headerRow3.CreateCell(19).SetCellValue("PO Approved I Date");
        //        headerRow3.CreateCell(20).SetCellValue("PO Approved II By");
        //        headerRow3.CreateCell(21).SetCellValue("PO Approved II Date");
        //        headerRow3.CreateCell(22).SetCellValue("PO Status");
        //        headerRow3.CreateCell(23).SetCellValue("Created By");
        //        headerRow3.CreateCell(24).SetCellValue("Created On");

        //        // Populate data rows
        //        int rowIndex1 = 1;
        //        foreach (var item in poConfirmationReports)
        //        {
        //            var row = sheet1.CreateRow(rowIndex1++);
        //            row.CreateCell(0).SetCellValue(item.VendorId ?? ""); // VendorId
        //            row.CreateCell(1).SetCellValue(item.VendorName ?? ""); // VendorName
        //            row.CreateCell(2).SetCellValue(item.PONumber ?? ""); // PONumber
        //            row.CreateCell(3).SetCellValue(item.PODate.HasValue ? item.PODate.Value.ToString("dd/MM/yyyy") : ""); // PODate
        //            row.CreateCell(4).SetCellValue(Convert.ToDouble(item.PRQty ?? 0)); // PRQty
        //            row.CreateCell(5).SetCellValue(item.RevisionNumber ?? 0); // RevisionNumber
        //            row.CreateCell(6).SetCellValue(item.ItemNumber ?? ""); // ItemNumber
        //            row.CreateCell(7).SetCellValue(item.MftrItemNumber ?? ""); // MftrItemNumber
        //            row.CreateCell(8).SetCellValue(item.ItemDescription ?? ""); // ItemDescription
        //            row.CreateCell(9).SetCellValue(Convert.ToDouble(item.POQnty ?? 0)); // POQnty
        //            row.CreateCell(10).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0)); // ReceivedQty
        //            row.CreateCell(11).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0)); // BalanceQty
        //            row.CreateCell(12).SetCellValue(item.Currency ?? ""); // Currency
        //            row.CreateCell(13).SetCellValue(item.UOM ?? ""); // UOM
        //            row.CreateCell(14).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0)); // UnitPrice
        //            row.CreateCell(15).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0)); // BalanceValue
        //            row.CreateCell(16).SetCellValue(item.POApprovedIBy ?? ""); // POApprovedIBy
        //            row.CreateCell(17).SetCellValue(item.POApprovedIDate.HasValue ? item.POApprovedIDate.Value.ToString("dd/MM/yyyy") : ""); // POApprovedIDate
        //            row.CreateCell(18).SetCellValue(item.POApprovedIIBy ?? ""); // POApprovedIIBy
        //            row.CreateCell(19).SetCellValue(item.POApprovedIIDate.HasValue ? item.POApprovedIIDate.Value.ToString("dd/MM/yyyy") : ""); // POApprovedIIDate
        //            row.CreateCell(20).SetCellValue(item.PoStatus ?? 0); // POStatus
        //            row.CreateCell(21).SetCellValue(item.CreatedBy ?? ""); // CreatedBy
        //            row.CreateCell(22).SetCellValue(item.CreatedOn.HasValue ? item.CreatedOn.Value.ToString("dd/MM/yyyy") : ""); // CreatedOn
        //            row.CreateCell(23).SetCellValue(item.ConfirmationDate.HasValue ? item.ConfirmationDate.Value.ToString("dd/MM/yyyy") : ""); // ConfirmationDate
        //            row.CreateCell(24).SetCellValue(Convert.ToDouble(item.ConfirmationQty ?? 0)); // ConfirmationQty
        //        }

        //        int rowIndex2 = 1;
        //        foreach (var item in poDeliveryScheduleReports)
        //        {
        //            var row = sheet2.CreateRow(rowIndex2++);
        //            row.CreateCell(0).SetCellValue(item.VendorId ?? ""); // VendorId
        //            row.CreateCell(1).SetCellValue(item.VendorName ?? ""); // VendorName
        //            row.CreateCell(2).SetCellValue(item.PONumber ?? ""); // PONumber
        //            row.CreateCell(3).SetCellValue(item.PODate.HasValue ? item.PODate.Value.ToString("dd/MM/yyyy") : ""); // PODate
        //            row.CreateCell(4).SetCellValue(Convert.ToDouble(item.PRQty ?? 0)); // PRQty
        //            row.CreateCell(5).SetCellValue(item.RevisionNumber ?? 0); // RevisionNumber
        //            row.CreateCell(6).SetCellValue(item.ItemNumber ?? ""); // ItemNumber
        //            row.CreateCell(7).SetCellValue(item.MftrItemNumber ?? ""); // MftrItemNumber
        //            row.CreateCell(8).SetCellValue(item.ItemDescription ?? ""); // ItemDescription
        //            row.CreateCell(9).SetCellValue(Convert.ToDouble(item.POQnty ?? 0)); // POQnty
        //            row.CreateCell(10).SetCellValue(Convert.ToDouble(item.ScheduleQty ?? 0)); // ScheduleQty
        //            row.CreateCell(11).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0)); // ReceivedQty
        //            row.CreateCell(12).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0)); // BalanceQty
        //            row.CreateCell(13).SetCellValue(item.Currency ?? ""); // Currency
        //            row.CreateCell(14).SetCellValue(item.UOM ?? ""); // UOM
        //            row.CreateCell(15).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0)); // UnitPrice
        //            row.CreateCell(16).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0)); // BalanceValue
        //            row.CreateCell(17).SetCellValue(item.POApprovedIBy ?? ""); // POApprovedIBy
        //            row.CreateCell(18).SetCellValue(item.POApprovedIDate.HasValue ? item.POApprovedIDate.Value.ToString("dd/MM/yyyy") : ""); // POApprovedIDate
        //            row.CreateCell(19).SetCellValue(item.POApprovedIIBy ?? ""); // POApprovedIIBy
        //            row.CreateCell(20).SetCellValue(item.POApprovedIIDate.HasValue ? item.POApprovedIIDate.Value.ToString("dd/MM/yyyy") : ""); // POApprovedIIDate
        //            row.CreateCell(21).SetCellValue(item.PoStatus ?? 0); // POStatus
        //            row.CreateCell(22).SetCellValue(item.CreatedBy ?? ""); // CreatedBy
        //            row.CreateCell(23).SetCellValue(item.CreatedOn.HasValue ? item.CreatedOn.Value.ToString("dd/MM/yyyy") : ""); // CreatedOn
        //            row.CreateCell(24).SetCellValue(item.ScheduleDate.HasValue ? item.ScheduleDate.Value.ToString("dd/MM/yyyy") : ""); // ScheduleDate
        //        }

        //        int rowIndex3 = 1;
        //        foreach (var item in poProjectReports)
        //        {
        //            var row = sheet3.CreateRow(rowIndex3++);
        //            row.CreateCell(0).SetCellValue(item.VendorId ?? ""); // VendorId
        //            row.CreateCell(1).SetCellValue(item.VendorName ?? ""); // VendorName
        //            row.CreateCell(2).SetCellValue(item.PONumber ?? ""); // PONumber
        //            row.CreateCell(3).SetCellValue(item.PODate.HasValue ? item.PODate.Value.ToString("dd/MM/yyyy") : ""); // PODate
        //            row.CreateCell(4).SetCellValue(Convert.ToDouble(item.PRQty ?? 0)); // PRQty
        //            row.CreateCell(5).SetCellValue(item.RevisionNumber ?? 0); // RevisionNumber
        //            row.CreateCell(6).SetCellValue(item.ProjectNumber ?? ""); // ProjectNumber
        //            row.CreateCell(7).SetCellValue(Convert.ToDouble(item.ProjectQty ?? 0)); // ProjectQty
        //            row.CreateCell(8).SetCellValue(item.ItemNumber ?? ""); // ItemNumber
        //            row.CreateCell(9).SetCellValue(item.MftrItemNumber ?? ""); // MftrItemNumber
        //            row.CreateCell(10).SetCellValue(item.ItemDescription ?? ""); // ItemDescription
        //            row.CreateCell(11).SetCellValue(Convert.ToDouble(item.POQnty ?? 0)); // POQnty
        //            row.CreateCell(12).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0)); // ReceivedQty
        //            row.CreateCell(13).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0)); // BalanceQty
        //            row.CreateCell(14).SetCellValue(item.Currency ?? ""); // Currency
        //            row.CreateCell(15).SetCellValue(item.UOM ?? ""); // UOM
        //            row.CreateCell(16).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0)); // UnitPrice
        //            row.CreateCell(17).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0)); // BalanceValue
        //            row.CreateCell(18).SetCellValue(item.POApprovedIBy ?? ""); // POApprovedIBy
        //            row.CreateCell(19).SetCellValue(item.POApprovedIDate.HasValue ? item.POApprovedIDate.Value.ToString("dd/MM/yyyy") : ""); // POApprovedIDate
        //            row.CreateCell(20).SetCellValue(item.POApprovedIIBy ?? ""); // POApprovedIIBy
        //            row.CreateCell(21).SetCellValue(item.POApprovedIIDate.HasValue ? item.POApprovedIIDate.Value.ToString("dd/MM/yyyy") : ""); // POApprovedIIDate
        //            row.CreateCell(22).SetCellValue(item.PoStatus ?? 0); // POStatus
        //            row.CreateCell(23).SetCellValue(item.CreatedBy ?? ""); // CreatedBy
        //            row.CreateCell(24).SetCellValue(item.CreatedOn.HasValue ? item.CreatedOn.Value.ToString("dd/MM/yyyy") : ""); // CreatedOn
        //        }


        //        using (var memoryStream = new MemoryStream())
        //        {
        //            workbook.Write(memoryStream);
        //            var excelBytes = memoryStream.ToArray();

        //            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "POReports.xlsx");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        return StatusCode(500, $"An error occurred: {ex.Message},{ex.InnerException}");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> ExportPOConfirmationReportWithDateToExcel([FromBody] PurchaseOrderDate_ReportGetDto purchaseOrderDate_ReportGetDto)
        {

            try
            {
                var poConfirmationReports = await _repository.GetPoConfirmationSPReportwithDate(purchaseOrderDate_ReportGetDto.FromDate, purchaseOrderDate_ReportGetDto.ToDate,
                                                                                                purchaseOrderDate_ReportGetDto.Approval, purchaseOrderDate_ReportGetDto.RecordType);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("POConfirmationReport");

                // Set header row
                var headerRow1 = sheet.CreateRow(0);
                headerRow1.CreateCell(0).SetCellValue("Vendor ID");
                headerRow1.CreateCell(1).SetCellValue("Vendor Name");
                headerRow1.CreateCell(2).SetCellValue("PO Number");
                headerRow1.CreateCell(3).SetCellValue("PO Date");
                headerRow1.CreateCell(4).SetCellValue("Revision Number");
                headerRow1.CreateCell(5).SetCellValue("Project Number");
                headerRow1.CreateCell(6).SetCellValue("Item Number");
                headerRow1.CreateCell(7).SetCellValue("Project Qty");
                headerRow1.CreateCell(8).SetCellValue("Project Balance Qty");
                headerRow1.CreateCell(9).SetCellValue("Mftr Item Number");
                headerRow1.CreateCell(10).SetCellValue("Item Description");
                headerRow1.CreateCell(11).SetCellValue("PO Qty");
                headerRow1.CreateCell(12).SetCellValue("Received Qty");
                headerRow1.CreateCell(13).SetCellValue("Balance Qty");
                headerRow1.CreateCell(14).SetCellValue("Currency");
                headerRow1.CreateCell(15).SetCellValue("UOM");
                headerRow1.CreateCell(16).SetCellValue("Unit Price");
                headerRow1.CreateCell(17).SetCellValue("Payment Terms");
                headerRow1.CreateCell(18).SetCellValue("Balance Value");
                headerRow1.CreateCell(19).SetCellValue("PO Approved I By");
                headerRow1.CreateCell(20).SetCellValue("PO Approved I Date");
                headerRow1.CreateCell(21).SetCellValue("PO Approved II By");
                headerRow1.CreateCell(22).SetCellValue("PO Approved II Date");
                headerRow1.CreateCell(23).SetCellValue("PO Status");
                headerRow1.CreateCell(24).SetCellValue("Created By");
                headerRow1.CreateCell(25).SetCellValue("Created On");
                headerRow1.CreateCell(26).SetCellValue("Confirmation Date");
                headerRow1.CreateCell(27).SetCellValue("Confirmation Qty");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in poConfirmationReports)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(3).SetCellValue(item.PODate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.RevisionNumber ?? 0);
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(item.ProjectQty ?? 0));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(item.projectBalanceQty ?? 0));
                    row.CreateCell(9).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(10).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.POQnty ?? 0));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0));
                    row.CreateCell(13).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0));
                    row.CreateCell(14).SetCellValue(item.Currency ?? "");
                    row.CreateCell(15).SetCellValue(item.UOM ?? "");
                    row.CreateCell(16).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0));
                    row.CreateCell(17).SetCellValue(item.PaymentTerms ?? "");
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0));
                    row.CreateCell(19).SetCellValue(item.POApprovedIBy ?? "");
                    row.CreateCell(20).SetCellValue(item.POApprovedIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(21).SetCellValue(item.POApprovedIIBy ?? "");
                    row.CreateCell(22).SetCellValue(item.POApprovedIIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(23).SetCellValue(item.PoStatus ?? 0);
                    row.CreateCell(24).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(25).SetCellValue(item.CreatedOn?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(26).SetCellValue(item.ConfirmationDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.ConfirmationQty ?? 0));
                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "POConfirmationReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Return appropriate error response to the client
                return StatusCode(500, $"Error Occured in ExportPOConfirmationReportWithDateToExcel API : \n {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetPoDeliverySchedulewithParam([FromQuery] PagingParameter pagingParameter, [FromQuery] string? SearchTerm, [FromBody] PurchaseOrderLimitSPReportDto paramsforPurchase)
        {
            ServiceResponse<IEnumerable<podeliveryschedule_report_with_parameters_with_pagination_Dto>> serviceResponse = new ServiceResponse<IEnumerable<podeliveryschedule_report_with_parameters_with_pagination_Dto>>();
            try
            {
                var result = await _repository.GetPoDeliveryScheduleLimitwithParam(paramsforPurchase.ItemNumber, paramsforPurchase.ProjectNumber, paramsforPurchase.PONumbers, paramsforPurchase.VendorName,
                                                                                                paramsforPurchase.POStatus, paramsforPurchase.Approval, paramsforPurchase.RecordType,
                                                                                                 paramsforPurchase.Offset, paramsforPurchase.Limit);

                var TotalCount = await _repository.GetAllPurchaseOrderCountForTrans(SearchTerm);

                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" PoDeliveryScheduleLimitSPReportwithParam hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PoDeliveryScheduleLimitSPReportwithParam hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var metadata = new
                    {
                        TotalCount,
                        pagingParameter.PageSize,
                        CurrentPage = pagingParameter.PageNumber
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned  PoDeliveryScheduleLimitSPReportwithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPoDeliverySchedulewithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPoDeliverySchedulewithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPoDeliveryScheduleSPReportwithDate([FromQuery] PagingParameter pagingParameter, [FromQuery] string? SearchTerm, [FromBody] PurchaseOrderDateLimitSPReportDto purchaseOrderDate_ReportGetDto)
        {
            ServiceResponse<IEnumerable<podeliveryschedule_report_with_parameters_with_pagination_Dto>> serviceResponse = new ServiceResponse<IEnumerable<podeliveryschedule_report_with_parameters_with_pagination_Dto>>();
            try
            {
                var result = await _repository.GetPoDeliveryScheduleLimitSPReportwithDate(purchaseOrderDate_ReportGetDto.FromDate, purchaseOrderDate_ReportGetDto.ToDate,
                                                                                                purchaseOrderDate_ReportGetDto.Approval, purchaseOrderDate_ReportGetDto.RecordType,
                                                                                                purchaseOrderDate_ReportGetDto.Offset, purchaseOrderDate_ReportGetDto.Limit);

                var TotalCount = await _repository.GetAllPurchaseOrderCountForTrans(SearchTerm);

                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" PoDeliveryScheduleLimitSPReportswithDate hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PoDeliveryScheduleLimitSPReportswithDate hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var metadata = new
                    {
                        TotalCount,
                        pagingParameter.PageSize,
                        CurrentPage = pagingParameter.PageNumber
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned  PoDeliveryScheduleLimitSPReportswithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPoDeliveryScheduleSPReportwithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPoDeliveryScheduleSPReportwithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportPODeliveryScheduleReportToExcel([FromBody] PurchaseOrderConfor_ReportGetDto paramsforPurchase)
        {
            try
            {
                var poDeliveryScheduleReports = await _repository.GetPoDeliverySchedulewithParam(paramsforPurchase.ItemNumber, paramsforPurchase.ProjectNumber, paramsforPurchase.PONumbers, paramsforPurchase.VendorName,
                                                                                                paramsforPurchase.POStatus, paramsforPurchase.Approval, paramsforPurchase.RecordType);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("PODeliveryScheduleReport");

                // Set header row
                var headerRow2 = sheet.CreateRow(0);
                headerRow2.CreateCell(0).SetCellValue("Vendor ID");
                headerRow2.CreateCell(1).SetCellValue("Vendor Name");
                headerRow2.CreateCell(2).SetCellValue("PO Number");
                headerRow2.CreateCell(3).SetCellValue("PO Date");
                headerRow2.CreateCell(4).SetCellValue("Revision Number");
                headerRow2.CreateCell(5).SetCellValue("Project Number");
                headerRow2.CreateCell(6).SetCellValue("Item Number");
                headerRow2.CreateCell(7).SetCellValue("Mftr Item Number");
                headerRow2.CreateCell(8).SetCellValue("Item Description");
                headerRow2.CreateCell(9).SetCellValue("PO Qty");
                headerRow2.CreateCell(10).SetCellValue("Schedule Qty");
                headerRow2.CreateCell(11).SetCellValue("Received Qty");
                headerRow2.CreateCell(12).SetCellValue("Balance Qty");
                headerRow2.CreateCell(13).SetCellValue("Currency");
                headerRow2.CreateCell(14).SetCellValue("UOM");
                headerRow2.CreateCell(15).SetCellValue("Unit Price");
                headerRow2.CreateCell(16).SetCellValue("Payment Terms");
                headerRow2.CreateCell(17).SetCellValue("Balance Value");
                headerRow2.CreateCell(18).SetCellValue("PO Approved I By");
                headerRow2.CreateCell(19).SetCellValue("PO Approved I Date");
                headerRow2.CreateCell(20).SetCellValue("PO Approved II By");
                headerRow2.CreateCell(21).SetCellValue("PO Approved II Date");
                headerRow2.CreateCell(22).SetCellValue("PO Status");
                headerRow2.CreateCell(23).SetCellValue("Created By");
                headerRow2.CreateCell(24).SetCellValue("Created On");
                headerRow2.CreateCell(25).SetCellValue("Schedule Date");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in poDeliveryScheduleReports)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(3).SetCellValue(item.PODate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.RevisionNumber ?? 0);
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(7).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(8).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(item.POQnty ?? 0));
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(item.ScheduleQty ?? 0));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0));
                    row.CreateCell(13).SetCellValue(item.Currency ?? "");
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0));
                    row.CreateCell(16).SetCellValue(item.PaymentTerms ?? "");
                    row.CreateCell(17).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0));
                    row.CreateCell(18).SetCellValue(item.POApprovedIBy ?? "");
                    row.CreateCell(19).SetCellValue(item.POApprovedIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(20).SetCellValue(item.POApprovedIIBy ?? "");
                    row.CreateCell(21).SetCellValue(item.POApprovedIIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(22).SetCellValue(item.PoStatus ?? 0);
                    row.CreateCell(23).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(24).SetCellValue(item.CreatedOn?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(25).SetCellValue(item.ScheduleDate?.ToString("dd/MM/yyyy") ?? "");
                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PODeliveryScheduleReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Return appropriate error response to the client
                return StatusCode(500, $"Error Occured in ExportPODeliveryScheduleReportToExcel API : \n {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportPODeliveryScheduleReportWithDateToExcel([FromBody] PurchaseOrderDate_ReportGetDto purchaseOrderDate_ReportGetDto)
        {
            try
            {
                var poDeliveryScheduleReports = await _repository.GetPoDeliveryScheduleSPReportwithDate(purchaseOrderDate_ReportGetDto.FromDate, purchaseOrderDate_ReportGetDto.ToDate,
                                                                                                purchaseOrderDate_ReportGetDto.Approval, purchaseOrderDate_ReportGetDto.RecordType);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("PODeliveryScheduleReport");

                // Set header row
                var headerRow2 = sheet.CreateRow(0);
                headerRow2.CreateCell(0).SetCellValue("Vendor ID");
                headerRow2.CreateCell(1).SetCellValue("Vendor Name");
                headerRow2.CreateCell(2).SetCellValue("PO Number");
                headerRow2.CreateCell(3).SetCellValue("PO Date");
                headerRow2.CreateCell(4).SetCellValue("Revision Number");
                headerRow2.CreateCell(5).SetCellValue("Project Number");
                headerRow2.CreateCell(6).SetCellValue("Item Number");
                headerRow2.CreateCell(7).SetCellValue("Mftr Item Number");
                headerRow2.CreateCell(8).SetCellValue("Item Description");
                headerRow2.CreateCell(9).SetCellValue("PO Qty");
                headerRow2.CreateCell(10).SetCellValue("Schedule Qty");
                headerRow2.CreateCell(11).SetCellValue("Received Qty");
                headerRow2.CreateCell(12).SetCellValue("Balance Qty");
                headerRow2.CreateCell(13).SetCellValue("Currency");
                headerRow2.CreateCell(14).SetCellValue("UOM");
                headerRow2.CreateCell(15).SetCellValue("Unit Price");
                headerRow2.CreateCell(16).SetCellValue("Payment Terms");
                headerRow2.CreateCell(17).SetCellValue("Balance Value");
                headerRow2.CreateCell(18).SetCellValue("PO Approved I By");
                headerRow2.CreateCell(19).SetCellValue("PO Approved I Date");
                headerRow2.CreateCell(20).SetCellValue("PO Approved II By");
                headerRow2.CreateCell(21).SetCellValue("PO Approved II Date");
                headerRow2.CreateCell(22).SetCellValue("PO Status");
                headerRow2.CreateCell(23).SetCellValue("Created By");
                headerRow2.CreateCell(24).SetCellValue("Created On");
                headerRow2.CreateCell(25).SetCellValue("Schedule Date");


                // Populate data rows
                int rowIndex = 1;
                foreach (var item in poDeliveryScheduleReports)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(3).SetCellValue(item.PODate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.RevisionNumber ?? 0);
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(7).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(8).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(item.POQnty ?? 0));
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(item.ScheduleQty ?? 0));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0));
                    row.CreateCell(13).SetCellValue(item.Currency ?? "");
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0));
                    row.CreateCell(16).SetCellValue(item.PaymentTerms ?? "");
                    row.CreateCell(17).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0));
                    row.CreateCell(18).SetCellValue(item.POApprovedIBy ?? "");
                    row.CreateCell(19).SetCellValue(item.POApprovedIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(20).SetCellValue(item.POApprovedIIBy ?? "");
                    row.CreateCell(21).SetCellValue(item.POApprovedIIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(22).SetCellValue(item.PoStatus ?? 0);
                    row.CreateCell(23).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(24).SetCellValue(item.CreatedOn?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(25).SetCellValue(item.ScheduleDate?.ToString("dd/MM/yyyy") ?? "");
                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PODeliveryScheduleReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Return appropriate error response to the client
                return StatusCode(500, $"Error Occured in ExportPODeliveryScheduleReportWithDateToExcel API : \n {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPoProjectSPReportwithParam([FromQuery] PagingParameter pagingParameter, [FromQuery] string? SearchTerm, [FromBody] PurchaseOrderProLimitSPReportDto paramsforPurchase)
        {
            ServiceResponse<IEnumerable<PoProjectSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PoProjectSPReport>>();
            try
            {
                var result = await _repository.GetPoProjectLimitSPReportwithParam(paramsforPurchase.ItemNumber, paramsforPurchase.PONumbers, paramsforPurchase.VendorName,
                                                                                        paramsforPurchase.POStatus, paramsforPurchase.Approval, paramsforPurchase.ProjectNumber,
                                                                                        paramsforPurchase.RecordType, paramsforPurchase.Offset, paramsforPurchase.Limit);

                var TotalCount = await _repository.GetAllPurchaseOrderCountForTrans(SearchTerm);

                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" PoProjectLimitSPReportwithParam hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PoProjectLimitSPReportwithParam hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var metadata = new
                    {
                        TotalCount,
                        pagingParameter.PageSize,
                        CurrentPage = pagingParameter.PageNumber
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned  GetPoProjectLimitSPReportwithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPoProjectSPReportwithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPoProjectSPReportwithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPoProjectSPReportwithDate([FromQuery] PagingParameter pagingParameter, [FromQuery] string? SearchTerm, [FromBody] PurchaseOrderDateLimitSPReportDto purchaseOrderDate_ReportGetDto)
        {
            ServiceResponse<IEnumerable<PoProjectSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PoProjectSPReport>>();
            try
            {
                var result = await _repository.GetPoProjectLimitSPReportwithDate(purchaseOrderDate_ReportGetDto.FromDate, purchaseOrderDate_ReportGetDto.ToDate,
                                                                                                purchaseOrderDate_ReportGetDto.Approval, purchaseOrderDate_ReportGetDto.RecordType,
                                                                                                purchaseOrderDate_ReportGetDto.Offset, purchaseOrderDate_ReportGetDto.Limit);

                var TotalCount = await _repository.GetAllPurchaseOrderCountForTrans(SearchTerm);

                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" PoProjectLimitSPReportswithDate hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PoProjectlimitSPReportswithDate hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var metadata = new
                    {
                        TotalCount,
                        pagingParameter.PageSize,
                        CurrentPage = pagingParameter.PageNumber
                    };
                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned  PoProjectLimitSPReportwithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPoProjectSPReportwithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPoProjectSPReportwithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportPoProjectSPReportToExcel([FromBody] PurchaseOrder_ReportGetDto paramsforPurchase)
        {
            try
            {
                var poProjectReports = await _repository.GetPoProjectSPReportwithParam(paramsforPurchase.ItemNumber, paramsforPurchase.PONumbers, paramsforPurchase.VendorName,
                                                                                       paramsforPurchase.POStatus, paramsforPurchase.Approval, paramsforPurchase.ProjectNumber,
                                                                                       paramsforPurchase.RecordType);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("PoProjectSPReport");

                // Set header row
                var headerRow3 = sheet.CreateRow(0);
                headerRow3.CreateCell(0).SetCellValue("Vendor ID");
                headerRow3.CreateCell(1).SetCellValue("Vendor Name");
                headerRow3.CreateCell(2).SetCellValue("PO Number");
                headerRow3.CreateCell(3).SetCellValue("PO Date");
                headerRow3.CreateCell(4).SetCellValue("Revision Number");
                headerRow3.CreateCell(5).SetCellValue("Project Number");
                headerRow3.CreateCell(6).SetCellValue("Project Qty");
                headerRow3.CreateCell(7).SetCellValue("Item Number");
                headerRow3.CreateCell(8).SetCellValue("Mftr Item Number");
                headerRow3.CreateCell(9).SetCellValue("Item Description");
                headerRow3.CreateCell(10).SetCellValue("PO Qty");
                headerRow3.CreateCell(11).SetCellValue("Received Qty");
                headerRow3.CreateCell(12).SetCellValue("Balance Qty");
                headerRow3.CreateCell(13).SetCellValue("Currency");
                headerRow3.CreateCell(14).SetCellValue("UOM");
                headerRow3.CreateCell(15).SetCellValue("Unit Price");
                headerRow3.CreateCell(16).SetCellValue("PaymentTerms");
                headerRow3.CreateCell(17).SetCellValue("Balance Value");
                headerRow3.CreateCell(18).SetCellValue("PO Approved I By");
                headerRow3.CreateCell(19).SetCellValue("PO Approved I Date");
                headerRow3.CreateCell(20).SetCellValue("PO Approved II By");
                headerRow3.CreateCell(21).SetCellValue("PO Approved II Date");
                headerRow3.CreateCell(22).SetCellValue("PO Status");
                headerRow3.CreateCell(23).SetCellValue("Created By");
                headerRow3.CreateCell(24).SetCellValue("Created On");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in poProjectReports)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(3).SetCellValue(item.PODate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.RevisionNumber ?? 0);
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(item.ProjectQty ?? 0));
                    row.CreateCell(7).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(8).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(9).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(item.POQnty ?? 0));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0));
                    row.CreateCell(13).SetCellValue(item.Currency ?? "");
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0));
                    row.CreateCell(16).SetCellValue(item.PaymentTerms ?? "");
                    row.CreateCell(17).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0));
                    row.CreateCell(18).SetCellValue(item.POApprovedIBy ?? "");
                    row.CreateCell(19).SetCellValue(item.POApprovedIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(20).SetCellValue(item.POApprovedIIBy ?? "");
                    row.CreateCell(21).SetCellValue(item.POApprovedIIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(22).SetCellValue(item.PoStatus ?? 0);
                    row.CreateCell(23).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(24).SetCellValue(item.CreatedOn?.ToString("dd/MM/yyyy") ?? "");
                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PoProjectSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Return appropriate error response to the client
                return StatusCode(500, $"Error Occured in ExportPoProjectSPReportToExcel API : \n {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportPoProjectSPReportWithDateToExcel([FromBody] PurchaseOrderDate_ReportGetDto purchaseOrderDate_ReportGetDto)
        {
            try
            {
                var poProjectReports = await _repository.GetPoProjectSPReportwithDate(purchaseOrderDate_ReportGetDto.FromDate, purchaseOrderDate_ReportGetDto.ToDate,
                                                                                              purchaseOrderDate_ReportGetDto.Approval, purchaseOrderDate_ReportGetDto.RecordType);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("PoProjectSPReport");

                // Set header row
                var headerRow3 = sheet.CreateRow(0);
                headerRow3.CreateCell(0).SetCellValue("Vendor ID");
                headerRow3.CreateCell(1).SetCellValue("Vendor Name");
                headerRow3.CreateCell(2).SetCellValue("PO Number");
                headerRow3.CreateCell(3).SetCellValue("PO Date");
                headerRow3.CreateCell(4).SetCellValue("Revision Number");
                headerRow3.CreateCell(5).SetCellValue("Project Number");
                headerRow3.CreateCell(6).SetCellValue("Project Qty");
                headerRow3.CreateCell(7).SetCellValue("Item Number");
                headerRow3.CreateCell(8).SetCellValue("Mftr Item Number");
                headerRow3.CreateCell(9).SetCellValue("Item Description");
                headerRow3.CreateCell(10).SetCellValue("PO Qty");
                headerRow3.CreateCell(11).SetCellValue("Received Qty");
                headerRow3.CreateCell(12).SetCellValue("Balance Qty");
                headerRow3.CreateCell(13).SetCellValue("Currency");
                headerRow3.CreateCell(14).SetCellValue("UOM");
                headerRow3.CreateCell(15).SetCellValue("Unit Price");
                headerRow3.CreateCell(16).SetCellValue("PaymentTerms");
                headerRow3.CreateCell(17).SetCellValue("Balance Value");
                headerRow3.CreateCell(18).SetCellValue("PO Approved I By");
                headerRow3.CreateCell(19).SetCellValue("PO Approved I Date");
                headerRow3.CreateCell(20).SetCellValue("PO Approved II By");
                headerRow3.CreateCell(21).SetCellValue("PO Approved II Date");
                headerRow3.CreateCell(22).SetCellValue("PO Status");
                headerRow3.CreateCell(23).SetCellValue("Created By");
                headerRow3.CreateCell(24).SetCellValue("Created On");

                // Populate data rows
                int rowIndex = 1;
                foreach (var item in poProjectReports)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(3).SetCellValue(item.PODate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.RevisionNumber ?? 0);
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(item.ProjectQty ?? 0));
                    row.CreateCell(7).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(8).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(9).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(item.POQnty ?? 0));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0));
                    row.CreateCell(13).SetCellValue(item.Currency ?? "");
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0));
                    row.CreateCell(16).SetCellValue(item.PaymentTerms ?? "");
                    row.CreateCell(17).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0));
                    row.CreateCell(18).SetCellValue(item.POApprovedIBy ?? "");
                    row.CreateCell(19).SetCellValue(item.POApprovedIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(20).SetCellValue(item.POApprovedIIBy ?? "");
                    row.CreateCell(21).SetCellValue(item.POApprovedIIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(22).SetCellValue(item.PoStatus ?? 0);
                    row.CreateCell(23).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(24).SetCellValue(item.CreatedOn?.ToString("dd/MM/yyyy") ?? "");
                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PoProjectSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // Return appropriate error response to the client
                return StatusCode(500, $"Error Occured in ExportPoProjectSPReportWithDateToExcel API : \n {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportPOReportToExcel([FromBody] PurchaseOrder_ReportGetDto paramsforPurchase)
        {

            try
            {
                var poConfirmationReports = await _repository.GetPoConfirmationSPReportwithParam(paramsforPurchase.ItemNumber, paramsforPurchase.ProjectNumber, paramsforPurchase.PONumbers, paramsforPurchase.VendorName,
                                                                                   paramsforPurchase.POStatus, paramsforPurchase.Approval, paramsforPurchase.RecordType);

                var poDeliveryScheduleReports = await _repository.GetPoDeliverySchedulewithParam(paramsforPurchase.ItemNumber, paramsforPurchase.ProjectNumber, paramsforPurchase.PONumbers, paramsforPurchase.VendorName,
                                                                                                paramsforPurchase.POStatus, paramsforPurchase.Approval, paramsforPurchase.RecordType);

                var poProjectReports = await _repository.GetPoProjectSPReportwithParam(paramsforPurchase.ItemNumber, paramsforPurchase.PONumbers, paramsforPurchase.VendorName,
                                                                                       paramsforPurchase.POStatus, paramsforPurchase.Approval, paramsforPurchase.ProjectNumber,
                                                                                       paramsforPurchase.RecordType);

                // Create a new Excel workbook
                XSSFWorkbook workbook = new XSSFWorkbook();
                ISheet sheet1 = workbook.CreateSheet("POConfirmationReport");
                ISheet sheet2 = workbook.CreateSheet("PODeliveryScheduleReport");
                ISheet sheet3 = workbook.CreateSheet("PoProjectSPReport");

                // Set header row
                var headerRow1 = sheet1.CreateRow(0);
                headerRow1.CreateCell(0).SetCellValue("Vendor ID");
                headerRow1.CreateCell(1).SetCellValue("Vendor Name");
                headerRow1.CreateCell(2).SetCellValue("PO Number");
                headerRow1.CreateCell(3).SetCellValue("PO Date");
                headerRow1.CreateCell(4).SetCellValue("Revision Number");
                headerRow1.CreateCell(5).SetCellValue("Project Number");
                headerRow1.CreateCell(6).SetCellValue("Item Number");
                headerRow1.CreateCell(7).SetCellValue("Project Qty");
                headerRow1.CreateCell(8).SetCellValue("Project Balance Qty");
                headerRow1.CreateCell(9).SetCellValue("Mftr Item Number");
                headerRow1.CreateCell(10).SetCellValue("Item Description");
                headerRow1.CreateCell(11).SetCellValue("PO Qty");
                headerRow1.CreateCell(12).SetCellValue("Received Qty");
                headerRow1.CreateCell(13).SetCellValue("Balance Qty");
                headerRow1.CreateCell(14).SetCellValue("Currency");
                headerRow1.CreateCell(15).SetCellValue("UOM");
                headerRow1.CreateCell(16).SetCellValue("Unit Price");
                headerRow1.CreateCell(17).SetCellValue("Payment Terms");
                headerRow1.CreateCell(18).SetCellValue("Balance Value");
                headerRow1.CreateCell(19).SetCellValue("PO Approved I By");
                headerRow1.CreateCell(20).SetCellValue("PO Approved I Date");
                headerRow1.CreateCell(21).SetCellValue("PO Approved II By");
                headerRow1.CreateCell(22).SetCellValue("PO Approved II Date");
                headerRow1.CreateCell(23).SetCellValue("PO Status");
                headerRow1.CreateCell(24).SetCellValue("Created By");
                headerRow1.CreateCell(25).SetCellValue("Created On");
                headerRow1.CreateCell(26).SetCellValue("Confirmation Date");
                headerRow1.CreateCell(27).SetCellValue("Confirmation Qty");


                var headerRow2 = sheet2.CreateRow(0);
                headerRow2.CreateCell(0).SetCellValue("Vendor ID");
                headerRow2.CreateCell(1).SetCellValue("Vendor Name");
                headerRow2.CreateCell(2).SetCellValue("PO Number");
                headerRow2.CreateCell(3).SetCellValue("PO Date");
                headerRow2.CreateCell(4).SetCellValue("Revision Number");
                headerRow2.CreateCell(5).SetCellValue("Project Number");
                headerRow2.CreateCell(6).SetCellValue("Item Number");
                headerRow2.CreateCell(7).SetCellValue("Mftr Item Number");
                headerRow2.CreateCell(8).SetCellValue("Item Description");
                headerRow2.CreateCell(9).SetCellValue("PO Qty");
                headerRow2.CreateCell(10).SetCellValue("Schedule Qty");
                headerRow2.CreateCell(11).SetCellValue("Received Qty");
                headerRow2.CreateCell(12).SetCellValue("Balance Qty");
                headerRow2.CreateCell(13).SetCellValue("Currency");
                headerRow2.CreateCell(14).SetCellValue("UOM");
                headerRow2.CreateCell(15).SetCellValue("Unit Price");
                headerRow2.CreateCell(16).SetCellValue("Payment Terms");
                headerRow2.CreateCell(17).SetCellValue("Balance Value");
                headerRow2.CreateCell(18).SetCellValue("PO Approved I By");
                headerRow2.CreateCell(19).SetCellValue("PO Approved I Date");
                headerRow2.CreateCell(20).SetCellValue("PO Approved II By");
                headerRow2.CreateCell(21).SetCellValue("PO Approved II Date");
                headerRow2.CreateCell(22).SetCellValue("PO Status");
                headerRow2.CreateCell(23).SetCellValue("Created By");
                headerRow2.CreateCell(24).SetCellValue("Created On");
                headerRow2.CreateCell(25).SetCellValue("Schedule Date");


                var headerRow3 = sheet3.CreateRow(0);
                headerRow3.CreateCell(0).SetCellValue("Vendor ID");
                headerRow3.CreateCell(1).SetCellValue("Vendor Name");
                headerRow3.CreateCell(2).SetCellValue("PO Number");
                headerRow3.CreateCell(3).SetCellValue("PO Date");
                headerRow3.CreateCell(4).SetCellValue("Revision Number");
                headerRow3.CreateCell(5).SetCellValue("Project Number");
                headerRow3.CreateCell(6).SetCellValue("Project Qty");
                headerRow3.CreateCell(7).SetCellValue("Item Number");
                headerRow3.CreateCell(8).SetCellValue("Mftr Item Number");
                headerRow3.CreateCell(9).SetCellValue("Item Description");
                headerRow3.CreateCell(10).SetCellValue("PO Qty");
                headerRow3.CreateCell(11).SetCellValue("Received Qty");
                headerRow3.CreateCell(12).SetCellValue("Balance Qty");
                headerRow3.CreateCell(13).SetCellValue("Currency");
                headerRow3.CreateCell(14).SetCellValue("UOM");
                headerRow3.CreateCell(15).SetCellValue("Unit Price");
                headerRow3.CreateCell(16).SetCellValue("PaymentTerms");
                headerRow3.CreateCell(17).SetCellValue("Balance Value");
                headerRow3.CreateCell(18).SetCellValue("PO Approved I By");
                headerRow3.CreateCell(19).SetCellValue("PO Approved I Date");
                headerRow3.CreateCell(20).SetCellValue("PO Approved II By");
                headerRow3.CreateCell(21).SetCellValue("PO Approved II Date");
                headerRow3.CreateCell(22).SetCellValue("PO Status");
                headerRow3.CreateCell(23).SetCellValue("Created By");
                headerRow3.CreateCell(24).SetCellValue("Created On");


                // Populate data rows
                int rowIndex1 = 1;
                foreach (var item in poConfirmationReports)
                {
                    var row = sheet1.CreateRow(rowIndex1++);
                    row.CreateCell(0).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(3).SetCellValue(item.PODate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.RevisionNumber ?? 0);
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(item.ProjectQty ?? 0));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(item.projectBalanceQty ?? 0));
                    row.CreateCell(9).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(10).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.POQnty ?? 0));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0));
                    row.CreateCell(13).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0));
                    row.CreateCell(14).SetCellValue(item.Currency ?? "");
                    row.CreateCell(15).SetCellValue(item.UOM ?? "");
                    row.CreateCell(16).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0));
                    row.CreateCell(17).SetCellValue(item.PaymentTerms ?? "");
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0));
                    row.CreateCell(19).SetCellValue(item.POApprovedIBy ?? "");
                    row.CreateCell(20).SetCellValue(item.POApprovedIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(21).SetCellValue(item.POApprovedIIBy ?? "");
                    row.CreateCell(22).SetCellValue(item.POApprovedIIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(23).SetCellValue(item.PoStatus ?? 0);
                    row.CreateCell(24).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(25).SetCellValue(item.CreatedOn?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(26).SetCellValue(item.ConfirmationDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.ConfirmationQty ?? 0));
                }

                int rowIndex2 = 1;
                foreach (var item in poDeliveryScheduleReports)
                {
                    var row = sheet2.CreateRow(rowIndex2++);
                    row.CreateCell(0).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(3).SetCellValue(item.PODate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.RevisionNumber ?? 0);
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(7).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(8).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(item.POQnty ?? 0));
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(item.ScheduleQty ?? 0));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0));
                    row.CreateCell(13).SetCellValue(item.Currency ?? "");
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0));
                    row.CreateCell(16).SetCellValue(item.PaymentTerms ?? "");
                    row.CreateCell(17).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0));
                    row.CreateCell(18).SetCellValue(item.POApprovedIBy ?? "");
                    row.CreateCell(19).SetCellValue(item.POApprovedIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(20).SetCellValue(item.POApprovedIIBy ?? "");
                    row.CreateCell(21).SetCellValue(item.POApprovedIIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(22).SetCellValue(item.PoStatus ?? 0);
                    row.CreateCell(23).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(24).SetCellValue(item.CreatedOn?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(25).SetCellValue(item.ScheduleDate?.ToString("dd/MM/yyyy") ?? "");
                }

                int rowIndex3 = 1;
                foreach (var item in poProjectReports)
                {
                    var row = sheet3.CreateRow(rowIndex3++);

                    row.CreateCell(0).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(3).SetCellValue(item.PODate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.RevisionNumber ?? 0);
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(item.ProjectQty ?? 0));
                    row.CreateCell(7).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(8).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(9).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(item.POQnty ?? 0));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0));
                    row.CreateCell(13).SetCellValue(item.Currency ?? "");
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0));
                    row.CreateCell(16).SetCellValue(item.PaymentTerms ?? "");
                    row.CreateCell(17).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0));
                    row.CreateCell(18).SetCellValue(item.POApprovedIBy ?? "");
                    row.CreateCell(19).SetCellValue(item.POApprovedIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(20).SetCellValue(item.POApprovedIIBy ?? "");
                    row.CreateCell(21).SetCellValue(item.POApprovedIIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(22).SetCellValue(item.PoStatus ?? 0);
                    row.CreateCell(23).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(24).SetCellValue(item.CreatedOn?.ToString("dd/MM/yyyy") ?? "");
                }


                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "POReports.xlsx");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Error Occured in ExportPOReportToExcel API : \n {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportPOReportToExcelWithDate([FromBody] PurchaseOrderDate_ReportGetDto purchaseOrderDate_ReportGetDto)
        {

            try
            {
                var poConfirmationReports = await _repository.GetPoConfirmationSPReportwithDate(purchaseOrderDate_ReportGetDto.FromDate, purchaseOrderDate_ReportGetDto.ToDate,
                                                                                                purchaseOrderDate_ReportGetDto.Approval, purchaseOrderDate_ReportGetDto.RecordType);

                var poDeliveryScheduleReports = await _repository.GetPoDeliveryScheduleSPReportwithDate(purchaseOrderDate_ReportGetDto.FromDate, purchaseOrderDate_ReportGetDto.ToDate,
                                                                                                purchaseOrderDate_ReportGetDto.Approval, purchaseOrderDate_ReportGetDto.RecordType);

                var poProjectReports = await _repository.GetPoProjectSPReportwithDate(purchaseOrderDate_ReportGetDto.FromDate, purchaseOrderDate_ReportGetDto.ToDate,
                                                                                              purchaseOrderDate_ReportGetDto.Approval, purchaseOrderDate_ReportGetDto.RecordType);

                // Create a new Excel workbook
                XSSFWorkbook workbook = new XSSFWorkbook();
                ISheet sheet1 = workbook.CreateSheet("POConfirmationReport");
                ISheet sheet2 = workbook.CreateSheet("PODeliveryScheduleReport");
                ISheet sheet3 = workbook.CreateSheet("PoProjectSPReport");

                // Set header row
                var headerRow1 = sheet1.CreateRow(0);
                headerRow1.CreateCell(0).SetCellValue("Vendor ID");
                headerRow1.CreateCell(1).SetCellValue("Vendor Name");
                headerRow1.CreateCell(2).SetCellValue("PO Number");
                headerRow1.CreateCell(3).SetCellValue("PO Date");
                headerRow1.CreateCell(4).SetCellValue("Revision Number");
                headerRow1.CreateCell(5).SetCellValue("Project Number");
                headerRow1.CreateCell(6).SetCellValue("Item Number");
                headerRow1.CreateCell(7).SetCellValue("Project Qty");
                headerRow1.CreateCell(8).SetCellValue("Project Balance Qty");
                headerRow1.CreateCell(9).SetCellValue("Mftr Item Number");
                headerRow1.CreateCell(10).SetCellValue("Item Description");
                headerRow1.CreateCell(11).SetCellValue("PO Qty");
                headerRow1.CreateCell(12).SetCellValue("Received Qty");
                headerRow1.CreateCell(13).SetCellValue("Balance Qty");
                headerRow1.CreateCell(14).SetCellValue("Currency");
                headerRow1.CreateCell(15).SetCellValue("UOM");
                headerRow1.CreateCell(16).SetCellValue("Unit Price");
                headerRow1.CreateCell(17).SetCellValue("Payment Terms");
                headerRow1.CreateCell(18).SetCellValue("Balance Value");
                headerRow1.CreateCell(19).SetCellValue("PO Approved I By");
                headerRow1.CreateCell(20).SetCellValue("PO Approved I Date");
                headerRow1.CreateCell(21).SetCellValue("PO Approved II By");
                headerRow1.CreateCell(22).SetCellValue("PO Approved II Date");
                headerRow1.CreateCell(23).SetCellValue("PO Status");
                headerRow1.CreateCell(24).SetCellValue("Created By");
                headerRow1.CreateCell(25).SetCellValue("Created On");
                headerRow1.CreateCell(26).SetCellValue("Confirmation Date");
                headerRow1.CreateCell(27).SetCellValue("Confirmation Qty");


                var headerRow2 = sheet2.CreateRow(0);
                headerRow2.CreateCell(0).SetCellValue("Vendor ID");
                headerRow2.CreateCell(1).SetCellValue("Vendor Name");
                headerRow2.CreateCell(2).SetCellValue("PO Number");
                headerRow2.CreateCell(3).SetCellValue("PO Date");
                headerRow2.CreateCell(4).SetCellValue("Revision Number");
                headerRow2.CreateCell(5).SetCellValue("Project Number");
                headerRow2.CreateCell(6).SetCellValue("Item Number");
                headerRow2.CreateCell(7).SetCellValue("Mftr Item Number");
                headerRow2.CreateCell(8).SetCellValue("Item Description");
                headerRow2.CreateCell(9).SetCellValue("PO Qty");
                headerRow2.CreateCell(10).SetCellValue("Schedule Qty");
                headerRow2.CreateCell(11).SetCellValue("Received Qty");
                headerRow2.CreateCell(12).SetCellValue("Balance Qty");
                headerRow2.CreateCell(13).SetCellValue("Currency");
                headerRow2.CreateCell(14).SetCellValue("UOM");
                headerRow2.CreateCell(15).SetCellValue("Unit Price");
                headerRow2.CreateCell(16).SetCellValue("Payment Terms");
                headerRow2.CreateCell(17).SetCellValue("Balance Value");
                headerRow2.CreateCell(18).SetCellValue("PO Approved I By");
                headerRow2.CreateCell(19).SetCellValue("PO Approved I Date");
                headerRow2.CreateCell(20).SetCellValue("PO Approved II By");
                headerRow2.CreateCell(21).SetCellValue("PO Approved II Date");
                headerRow2.CreateCell(22).SetCellValue("PO Status");
                headerRow2.CreateCell(23).SetCellValue("Created By");
                headerRow2.CreateCell(24).SetCellValue("Created On");
                headerRow2.CreateCell(25).SetCellValue("Schedule Date");


                var headerRow3 = sheet3.CreateRow(0);
                headerRow3.CreateCell(0).SetCellValue("Vendor ID");
                headerRow3.CreateCell(1).SetCellValue("Vendor Name");
                headerRow3.CreateCell(2).SetCellValue("PO Number");
                headerRow3.CreateCell(3).SetCellValue("PO Date");
                headerRow3.CreateCell(4).SetCellValue("Revision Number");
                headerRow3.CreateCell(5).SetCellValue("Project Number");
                headerRow3.CreateCell(6).SetCellValue("Project Qty");
                headerRow3.CreateCell(7).SetCellValue("Item Number");
                headerRow3.CreateCell(8).SetCellValue("Mftr Item Number");
                headerRow3.CreateCell(9).SetCellValue("Item Description");
                headerRow3.CreateCell(10).SetCellValue("PO Qty");
                headerRow3.CreateCell(11).SetCellValue("Received Qty");
                headerRow3.CreateCell(12).SetCellValue("Balance Qty");
                headerRow3.CreateCell(13).SetCellValue("Currency");
                headerRow3.CreateCell(14).SetCellValue("UOM");
                headerRow3.CreateCell(15).SetCellValue("Unit Price");
                headerRow3.CreateCell(16).SetCellValue("PaymentTerms");
                headerRow3.CreateCell(17).SetCellValue("Balance Value");
                headerRow3.CreateCell(18).SetCellValue("PO Approved I By");
                headerRow3.CreateCell(19).SetCellValue("PO Approved I Date");
                headerRow3.CreateCell(20).SetCellValue("PO Approved II By");
                headerRow3.CreateCell(21).SetCellValue("PO Approved II Date");
                headerRow3.CreateCell(22).SetCellValue("PO Status");
                headerRow3.CreateCell(23).SetCellValue("Created By");
                headerRow3.CreateCell(24).SetCellValue("Created On");


                // Populate data rows
                int rowIndex1 = 1;
                foreach (var item in poConfirmationReports)
                {
                    var row = sheet1.CreateRow(rowIndex1++);
                    row.CreateCell(0).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(3).SetCellValue(item.PODate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.RevisionNumber ?? 0);
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(7).SetCellValue(Convert.ToDouble(item.ProjectQty ?? 0));
                    row.CreateCell(8).SetCellValue(Convert.ToDouble(item.projectBalanceQty ?? 0));
                    row.CreateCell(9).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(10).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.POQnty ?? 0));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0));
                    row.CreateCell(13).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0));
                    row.CreateCell(14).SetCellValue(item.Currency ?? "");
                    row.CreateCell(15).SetCellValue(item.UOM ?? "");
                    row.CreateCell(16).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0));
                    row.CreateCell(17).SetCellValue(item.PaymentTerms ?? "");
                    row.CreateCell(18).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0));
                    row.CreateCell(19).SetCellValue(item.POApprovedIBy ?? "");
                    row.CreateCell(20).SetCellValue(item.POApprovedIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(21).SetCellValue(item.POApprovedIIBy ?? "");
                    row.CreateCell(22).SetCellValue(item.POApprovedIIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(23).SetCellValue(item.PoStatus ?? 0);
                    row.CreateCell(24).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(25).SetCellValue(item.CreatedOn?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(26).SetCellValue(item.ConfirmationDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(27).SetCellValue(Convert.ToDouble(item.ConfirmationQty ?? 0));
                }

                int rowIndex2 = 1;
                foreach (var item in poDeliveryScheduleReports)
                {
                    var row = sheet2.CreateRow(rowIndex2++);
                    row.CreateCell(0).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(3).SetCellValue(item.PODate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.RevisionNumber ?? 0);
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(7).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(8).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(9).SetCellValue(Convert.ToDouble(item.POQnty ?? 0));
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(item.ScheduleQty ?? 0));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0));
                    row.CreateCell(13).SetCellValue(item.Currency ?? "");
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0));
                    row.CreateCell(16).SetCellValue(item.PaymentTerms ?? "");
                    row.CreateCell(17).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0));
                    row.CreateCell(18).SetCellValue(item.POApprovedIBy ?? "");
                    row.CreateCell(19).SetCellValue(item.POApprovedIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(20).SetCellValue(item.POApprovedIIBy ?? "");
                    row.CreateCell(21).SetCellValue(item.POApprovedIIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(22).SetCellValue(item.PoStatus ?? 0);
                    row.CreateCell(23).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(24).SetCellValue(item.CreatedOn?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(25).SetCellValue(item.ScheduleDate?.ToString("dd/MM/yyyy") ?? "");
                }

                int rowIndex3 = 1;
                foreach (var item in poProjectReports)
                {
                    var row = sheet3.CreateRow(rowIndex3++);

                    row.CreateCell(0).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(3).SetCellValue(item.PODate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.RevisionNumber ?? 0);
                    row.CreateCell(5).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(6).SetCellValue(Convert.ToDouble(item.ProjectQty ?? 0));
                    row.CreateCell(7).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(8).SetCellValue(item.MftrItemNumber ?? "");
                    row.CreateCell(9).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(10).SetCellValue(Convert.ToDouble(item.POQnty ?? 0));
                    row.CreateCell(11).SetCellValue(Convert.ToDouble(item.ReceivedQty ?? 0));
                    row.CreateCell(12).SetCellValue(Convert.ToDouble(item.BalanceQty ?? 0));
                    row.CreateCell(13).SetCellValue(item.Currency ?? "");
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue(Convert.ToDouble(item.UnitPrice ?? 0));
                    row.CreateCell(16).SetCellValue(item.PaymentTerms ?? "");
                    row.CreateCell(17).SetCellValue(Convert.ToDouble(item.BalanceValue ?? 0));
                    row.CreateCell(18).SetCellValue(item.POApprovedIBy ?? "");
                    row.CreateCell(19).SetCellValue(item.POApprovedIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(20).SetCellValue(item.POApprovedIIBy ?? "");
                    row.CreateCell(21).SetCellValue(item.POApprovedIIDate?.ToString("dd/MM/yyyy") ?? "");
                    row.CreateCell(22).SetCellValue(item.PoStatus ?? 0);
                    row.CreateCell(23).SetCellValue(item.CreatedBy ?? "");
                    row.CreateCell(24).SetCellValue(item.CreatedOn?.ToString("dd/MM/yyyy") ?? "");
                }



                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "POReports.xlsx");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, $"Error Occured in ExportPOReportToExcelWithDate API : \n {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePurchaseOrderTallyStatus(int Id, bool TallyStatus)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                var getPOdetails = await _repository.GetPurchaseOrderById(Id);
                getPOdetails.TallyStatus = TallyStatus;
                await _repository.UpdatePurchaseOrder_ForApproval(getPOdetails);
                _repository.SaveAsync();
                _logger.LogInfo($"Successfully Updated the TallyStatus of PO Id {Id} and is set to {TallyStatus} from the UpdatePurchaseOrderTallyStatus API");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Successfully Updated the TallyStatus of PO Id {Id} and is set to {TallyStatus}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(200, serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdatePurchaseOrderTallyStatus API for the following Id:{Id} and TallyStatus:{TallyStatus} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdatePurchaseOrderTallyStatus API for the following Id:{Id} and TallyStatus:{TallyStatus} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdatePurchaseOrdersApprovalRange([FromBody] ApprovalRangeUpdateRequest approvalRangesRequest)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                var POs = await _repository.GetAllUnApprovedLastestPOsbyProcurementType(approvalRangesRequest.ApprovalRanges.ProcurementName);
                foreach (var po in POs)
                {
                    decimal totalamount = po.TotalAmount;
                    if (po.Currency != "INR")
                    {
                        var converion = approvalRangesRequest.ConvertionRates.Where(x => x.UOC == po.Currency).FirstOrDefault();
                        if (converion == null)
                        {
                            _logger.LogError($"Error occured in UpdatePurchaseOrdersApprovalRange: The UOM {po.Currency} doesnot have any convertionrate");
                            serviceResponse.Data = null;
                            serviceResponse.Message = $"Error occured in UpdatePurchaseOrdersApprovalRange: The UOM {po.Currency} doesnot have any convertionrate";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                            return StatusCode(500, serviceResponse);
                        }
                        else
                        {
                            totalamount = totalamount * converion.ConvertionRate;
                            po.ConvertionRateId = converion.Id;
                        }
                    }
                    var range = approvalRangesRequest.ApprovalRanges.Ranges.FirstOrDefault(r => totalamount >= r.RangeFrom && (r.RangeTo != null ? totalamount <= r.RangeTo : true));
                    int count = (range.Approval1 ? 1 : 0) + (range.Approval2 ? 1 : 0) + (range.Approval3 ? 1 : 0) + (range.Approval4 ? 1 : 0);
                    po.ApprovalCount = count;
                    po.ApprovalRangeId = approvalRangesRequest.ApprovalRanges.Id;
                    await _repository.UpdatePurchaseOrder_ForApproval(po);
                }
                _repository.SaveAsync();
                _logger.LogInfo($"Successfully Updated UpdatePurchaseOrdersApprovalRange API");
                serviceResponse.Data = "Successfully";
                serviceResponse.Message = $"Successfully Updated UpdatePurchaseOrdersApprovalRange API";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(200, serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in UpdatePurchaseOrdersApprovalRange API: {ex.Message} \n {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in UpdatePurchaseOrdersApprovalRange: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateKIT_PODetails([FromBody] List<KIT_GRIN_POUpdate> kIT_GRIN_POUpdates)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                if (kIT_GRIN_POUpdates is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "UpdateKIT_PODetails object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("UpdateKIT_PODetails object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid UpdateKIT_PODetails object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid UpdateKIT_PODetails object sent from client.");
                    return BadRequest(serviceResponse);
                }
                foreach (var po in kIT_GRIN_POUpdates)
                {
                    var PurOrder = await _repository.GetLastestPurchaseOrderByPONumber(po.PONumber);
                    foreach (var poitem in po.POItems)
                    {
                        var POItem = PurOrder.POItems.Where(x => x.ItemNumber == poitem.ItemNumber).FirstOrDefault();
                        foreach (var prj in poitem.POProjects)
                        {
                            var Proj = POItem.POAddprojects.Where(x => x.ProjectNumber == prj.ProjectNumber).FirstOrDefault();
                            foreach (var comp in prj.POComponents)
                            {
                                var Component = Proj.PoAddKitProjects.Where(x => x.PartNumber == comp.PartNumber).FirstOrDefault();
                                Component.BalanceQty -= comp.KitComponentQty;
                                Component.ReceivedQty += comp.KitComponentQty;
                                if (Component.BalanceQty == 0) Component.PoAddKitProjectStatus = PoStatus.Closed;
                                else if (Component.BalanceQty != 0 && Component.ReceivedQty > 0) Component.PoAddKitProjectStatus = PoStatus.PartiallyClosed;
                            }
                            Proj.BalanceQty -= prj.ProjectQty;
                            Proj.ReceivedQty += prj.ProjectQty;
                            if (Proj.BalanceQty == 0) Proj.PoAddProjectStatus = PoStatus.Closed;
                            else if (Proj.BalanceQty != 0 && Proj.ReceivedQty > 0) Proj.PoAddProjectStatus = PoStatus.PartiallyClosed;
                        }
                        POItem.BalanceQty -= poitem.Qty;
                        POItem.ReceivedQty += poitem.Qty;
                        if (POItem.BalanceQty == 0) POItem.PoStatus = PoStatus.Closed;
                        else if (POItem.BalanceQty != 0 && POItem.ReceivedQty > 0) POItem.PoStatus = PoStatus.PartiallyClosed;
                    }
                    if (PurOrder.POItems.Count(x => x.PoStatus == PoStatus.Closed) == PurOrder.POItems.Count()) PurOrder.PoStatus = PoStatus.Closed;
                    else PurOrder.PoStatus = PoStatus.PartiallyClosed;
                    await _repository.UpdatePurchaseOrder_ForApproval(PurOrder);
                }
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = $"UpdateKIT_PODetails Was successfull";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured in UpdateKIT_PODetails:\n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in UpdateKIT_PODetails: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTallyPurchaseOrderSpReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<List<TallyPurchaseOrderSpReportDto>> serviceResponse = new();

            try
            {
                var products = await _repository.GetTallyPurchaseOrderSpReportWithDate(FromDate, ToDate);

                if (products == null || !products.Any())
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No TallyPurchaseOrderSpReport records found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"No TallyPurchaseOrderSpReport records found in DB.");
                    return NotFound(serviceResponse);
                }

                var result = products.Select(product =>
                {
                    var dto = _mapper.Map<TallyPurchaseOrderSpReportDto>(product);

                    if (!string.IsNullOrEmpty(product.POItems))
                    {
                        dto.POItems = JsonConvert.DeserializeObject<List<POItemsSpDto>>(product.POItems)
                            .OrderBy(x => x.ItemCode)
                            .ToList();
                    }

                    return dto;
                }).ToList();

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned TallyPurchaseOrderSpReport Details";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetTallyPurchaseOrderSpReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetTallyPurchaseOrderSpReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


    }
}
