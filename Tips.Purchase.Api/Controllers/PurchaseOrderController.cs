using System;
using System.Buffers.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities.Enums;
using Tips.Purchase.Api.Repository;
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
        public static IWebHostEnvironment _webHostEnvironment { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        private readonly HttpClient _httpClient;
        public PurchaseOrderController(IPrItemsRepository purchaseRequisitionItemRepository, IHttpClientFactory clientFactory, HttpClient httpClient, IPRItemsDocumentUploadRepository pRItemsDocumentUploadRepository, IHttpContextAccessor httpContextAccessor, IPoConfirmationDateRepository poConfirmationDateRepository, IPurchaseRequisitionRepository purchaseRequisitionRepository, IPoConfirmationHistoryRepository poConfirmationHistoryRepository, IPoConfirmationDateHistoryRepository poConfirmationDateHistoryRepository, IPurchaseOrderRepository repository, IWebHostEnvironment webHostEnvironment, IPoItemsRepository poItemsRepository, IPoAddprojectRepository poAddprojectRepository, IDocumentUploadRepository documentUploadRepository, ILoggerManager logger, IMapper mapper, IConfiguration config)
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
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
                _logger.LogError($"Something went wrong inside PurchaseOrderByPONumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
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
                _logger.LogError($"Something went wrong inside PurchaseOrderByPONumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllRevisionNumberListByPoNumber action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside PurchaseOrderDetails action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside PurchaseOrderDetails action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetPurchaseOrderSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                var products = await _repository.GetPurchaseOrderSPReportWithParam(purchaseOrderSPReport.VendorName, purchaseOrderSPReport.PONumber, purchaseOrderSPReport.ItemNumber,
                                                                                                                        purchaseOrderSPReport.RecordType, purchaseOrderSPReport.Postatus);

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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetPurchaseOrderSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseOrderApprovalSPReportWithParam([FromBody] PurchaseOrderApprovalSPReportWithParamDTO purchaseOrderApprovalSPReport)

        {
            ServiceResponse<IEnumerable<PurchaseOrderSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderSPReport>>();
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetPurchaseOrderApprovalSPReportWithParam action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside PurchaseOrderDetails action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetListOfOpenPOQtyByItemNoListByProjectNo action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetPRNumberandQtyListByItemNumber action";
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
                _logger.LogError($"Something went wrong inside GetPurchaseOrderByPoNoAndRevNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
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

                    if (purchaseOrderDetailbyId.POItems != null)
                    {
                        foreach (var poItemDetails in purchaseOrderDetailbyId.POItems)
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
                    serviceResponse.Message = "Returned PurchaseOrderById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPurchaseOrderById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPONumberListByVendorId action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPoNumberListByVendorIdForAvision action";
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

                var purchaseOrderDetails = _mapper.Map<PurchaseOrder>(purchaseOrderPostDto);
                var AmountInWords = GetTotalValueInWords(purchaseOrderDetails.TotalAmount);
                purchaseOrderDetails.AmountInWords = AmountInWords;
                var poItemDto = purchaseOrderPostDto.POItems;
                var prDetailsPostDto = poItemDto[0].PrDetails;
                var poFile = purchaseOrderPostDto.POFiles;
                var poItemDtoList = new List<PoItem>();
                //var poDocumentUploadDtoList = new List<DocumentUpload>();
                var poIncoTermList = _mapper.Map<IEnumerable<PoIncoTerm>>(purchaseOrderPostDto.POIncoTerms);

                var date = DateTime.Now;
                purchaseOrderPostDto.QuotationDate = date;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                //var newcount = await _repository.GetPONumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    purchaseOrderDetails.PONumber = days + months + years + "PO" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    purchaseOrderDetails.PONumber = days + months + years + "PO" + (e);
                //}
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
                //// Po Upload

                //var poUploadDetails = purchaseOrderPostDto.POFiles;

                //foreach (var poUploadDetail in poUploadDetails)
                //{
                //    Guid guid = Guid.NewGuid();
                //    var fileContent = poUploadDetail.FileByte;
                //    byte[] imageContent = Convert.FromBase64String(poUploadDetail.FileByte);
                //    var poNumbers = purchaseOrderDetails.PONumber;
                //    string fileName = guid.ToString() + "_" + poUploadDetail.FileName + "." + poUploadDetail.FileExtension;
                //    string FileExt = Path.GetExtension(fileName).ToUpper();


                //    //string filename_1 = guid.ToString() + "_" + fileName;
                //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PODocument", fileName);
                //    using (MemoryStream ms = new MemoryStream(imageContent))
                //    {
                //        ms.Position = 0;
                //        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                //        {
                //            ms.WriteTo(fileStream);
                //        }
                //        var uploadedFile = new DocumentUpload
                //        {
                //            FileName = fileName,
                //            FileExtension = FileExt,
                //            FilePath = filePath,
                //            ParentNumber = poNumbers,
                //            DocumentFrom = "PODocument",
                //        };
                //        _documentUploadRepository.CreateUploadDocumentPO(uploadedFile);                        

                //        if (uploadedFile != null)
                //        {
                //            DocumentUpload poFileDetails = _mapper.Map<DocumentUpload>(uploadedFile);
                //            poDocumentUploadDtoList.Add(poFileDetails);
                //        }

                //    }

                //}

                if (poItemDto != null)
                {
                    for (int i = 0; i < poItemDto.Count; i++)
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
                }

                purchaseOrderDetails.POItems = poItemDtoList;
                // purchaseOrderDetails.POFiles = poDocumentUploadDtoList;
                purchaseOrderDetails.POIncoTerms = poIncoTermList.ToList();
                await _repository.CreatePurchaseOrder(purchaseOrderDetails);


                //Adding data in PoConfirmationDateHistory
                //foreach (var poItems in poItemDtoList)
                //{
                //    if (poItems.POConfirmationDates != null)
                //    {
                //        foreach (var poConfirmationDate in poItems.POConfirmationDates)
                //        {
                //            PoConfirmationDateHistory poConfirmationDateHistory = new PoConfirmationDateHistory();
                //            poConfirmationDateHistory.ConfirmationDate = poConfirmationDate.ConfirmationDate;
                //            poConfirmationDateHistory.Qty = poConfirmationDate.Qty;

                //            var poConfirmationDateHistoryDetails = _mapper.Map<PoConfirmationDateHistory>(poConfirmationDateHistory);

                //            await _poConfirmationDateHistoryRepository.CreatePoConfirmationDateHistory(poConfirmationDateHistoryDetails);
                //            _poConfirmationDateHistoryRepository.SaveAsync();
                //        }
                //    }
                //}

                //Update PrUploadDocu
                //if (prDetailsPostDto.Count > 0 && prDetailsPostDto[0].PrDetailDocumentUploadPostDtos !=null)
                //{

                //        foreach (var prDetailsDto in prDetailsPostDto[0].PrDetailDocumentUploadPostDtos)
                //        {
                //            var prUploadDocument = await _pRItemsDocumentUploadRepository.GetUploadDocByFileName(prDetailsDto.FileName);
                //            if (prUploadDocument != null)
                //            {
                //                prUploadDocument.Checked = true;
                //                await _pRItemsDocumentUploadRepository.UpdateUploadDoc(prUploadDocument);
                //            }
                //            _pRItemsDocumentUploadRepository.SaveAsync();

                //        }

                //}
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
                //Changing Status in Pr and PrItems 
                //foreach (var poItems in poItemDtoList)
                //{
                //    foreach (var prDetails in poItems.PrDetails)
                //    {
                //        var prItemDetail = await _purchaseRequisitionItemRepository.GetPrItemByPRNo(prDetails.PRNumber, prDetails.Qty);
                //        if (prItemDetail != null)
                //        {
                //            prItemDetail.PrStatus = PrStatus.Closed;
                //            await _purchaseRequisitionItemRepository.UpdatePrItem(prItemDetail);
                //            _purchaseRequisitionItemRepository.SaveAsync();
                //        }

                //        var prItemClosedStatusCount = await _purchaseRequisitionItemRepository.GetPrItemClosedStatusCount(prDetails.PRNumber, prDetails.Qty);
                //        if (prItemClosedStatusCount == 0)
                //        {
                //            var prDetail = await _repository.GetPrDetailsByPrNumber(prDetails.PRNumber);
                //            prDetail.PrStatus = PrStatus.Closed;
                //            await _purchaseRequisitionRepository.UpdatePurchaseRequisition(prDetail);
                //            _purchaseRequisitionRepository.SaveAsync();
                //        }
                //    }


                //}

                foreach (var poItems in poItemDtoList)
                {
                    foreach (var prDetails in poItems.PrDetails)
                    {
                        var prItemDetail = await _purchaseRequisitionItemRepository.GetPrItemByPRNo(prDetails.PRNumber, poItems.ItemNumber);
                        if (prItemDetail != null)
                        {
                            prItemDetail.PrStatus = PrStatus.Closed;
                            await _purchaseRequisitionItemRepository.UpdatePrItem(prItemDetail);
                            _purchaseRequisitionItemRepository.SaveAsync();
                        }

                        var prItemClosedStatusCount = await _purchaseRequisitionItemRepository.GetPrItemClosedStatusCount(prDetails.PRNumber);
                        var prDetail = await _repository.GetPrDetailsByPrNumber(prDetails.PRNumber);
                        prDetail.PrStatus = prItemClosedStatusCount;
                        await _purchaseRequisitionRepository.UpdatePurchaseRequisition(prDetail);
                    }
                }
                _documentUploadRepository.SaveAsync();
                _repository.SaveAsync();
                _pRItemsDocumentUploadRepository.SaveAsync();
                _purchaseRequisitionRepository.SaveAsync();
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"],"GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,CreatePurchaseOrder";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"],$"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);
                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    var mails = (emaildetails1.data.Where(x=>x.operation== "CreatePurchaseOrder").Select(x=>x.emailIds).FirstOrDefault()).Split(',');
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
                    smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                    smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                    smtp.Send(email);
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
                _logger.LogError($"Something went wrong inside CreatePurchaseOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
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

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PODocument", Filename);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var ContentType))
            {
                ContentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(bytes, ContentType, Path.GetFileName(filePath));
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
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
                _logger.LogError($"Something went wrong inside UpdatePOUploadDocument action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
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
                _logger.LogError($"Something went wrong inside UpdateUploadDocument action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
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
                _logger.LogError($"Something went wrong inside PoFiles action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
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
                _logger.LogError($"Something went wrong inside PRFiles action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                purchaseOrderDetails.POIncoTerms = poIncoTermsList;

                if (poItemDto != null)
                {
                    for (int i = 0; i < poItemDto.Count; i++)
                    {
                        PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);
                        poItemDetails.BalanceQty = poItemDto[i].Qty;
                        poItemDetails.PoPartsStatus = false;
                        poItemDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                        for (int j = 0; j < poItemDetails.POAddprojects.Count; j++)
                        {
                            PoAddProject poaddproject = poItemDetails.POAddprojects[j];
                            poaddproject.BalanceQty = poaddproject.ProjectQty;
                        }

                        poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);
                        poItemDetails.POSpecialInstructions = _mapper.Map<List<PoSpecialInstruction>>(poItemDto[i].POSpecialInstructions);
                        poItemDetails.PrDetails = _mapper.Map<List<PrDetails>>(poItemDto[i].PrDetails);
                        poItemDetails.PONumber = purchaseOrderUpdateDto.PONumber;
                        poItemDtoList.Add(poItemDetails);
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
                                    prItemDetail.PrStatus = PrStatus.Closed;
                                    await _purchaseRequisitionItemRepository.UpdatePrItem(prItemDetail);
                                    _purchaseRequisitionItemRepository.SaveAsync();
                                }

                                var prItemClosedStatusCount = await _purchaseRequisitionItemRepository.GetPrItemClosedStatusCount(prDetails.PRNumber);
                                if (prItemClosedStatusCount == 0)
                                {
                                    var prDetail = await _repository.GetPrDetailsByPrNumber(prDetails.PRNumber);
                                    prDetail.PrStatus = PrStatus.Closed;
                                    await _purchaseRequisitionRepository.UpdatePurchaseRequisition(prDetail);

                                }
                            }
                        }
                    }
                }
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
                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    var mails = "accounts@avisionsystems.com";
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    var podate = purchaseOrderDetails.PODate.ToString().Split(" ");
                    email.To.Add(MailboxAddress.Parse(mails));
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
                    smtp.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate("erp@avisionsystems.com", "R#9183753474150W");

                    smtp.Send(email);
                    smtp.Disconnect(true);

                }
                _repository.SaveAsync();
                _pRItemsDocumentUploadRepository.SaveAsync();
                _purchaseRequisitionRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseOrder Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateurchaseOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
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
                    poItem.ReceivedQty = item.Qty;
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
                IEnumerable<PoAddProject> poProjectNoDetails = await _poAddprojectRepository.GetPOProjectNoDetailsByProjectNo(item.ItemNumber, item.ProjectNumber,item.PoItemId);
                decimal dispatchedQty = item.ProjectQty;

                foreach (var poProjectNos in poProjectNoDetails)
                {
                    poProjectNos.ReceivedQty = item.ProjectQty;
                    if (poProjectNos.BalanceQty >= dispatchedQty)
                    {
                        if (poProjectNos.BalanceQty == dispatchedQty)
                        {
                            poProjectNos.PoAddProjectStatus = true;
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
                IEnumerable<PoItem> poItems = await _poItemsRepository.GetPoItemDetailsByPONumberandItemNo(item.ItemNumber, item.PONumber, item.PoItemId);

                foreach (var poItem in poItems)
                {
                    if (poItem.Qty == item.Qty)
                    {
                        poItem.PoStatus = PoStatus.Closed;
                        await _poItemsRepository.UpdatePOOrderItem(poItem);

                    }
                    else
                    {
                        poItem.PoStatus = PoStatus.PartiallyClosed;
                        await _poItemsRepository.UpdatePOOrderItem(poItem);
                    }

                }
            }
            _poItemsRepository.SaveAsync();

            var poItemsPartiallyClosedStatusCount = await _poItemsRepository.GetPoItemsPartiallyClosedStatusCount(purchaseOrderStatusUpdateDto[0].PONumber);

            if (poItemsPartiallyClosedStatusCount != 0)
            {
                var purchaseOrderDetails = await _repository.GetLastestPurchaseOrderByPONumber(purchaseOrderStatusUpdateDto[0].PONumber);
                purchaseOrderDetails.PoStatus = PoStatus.PartiallyClosed;
                await _repository.UpdatePurchaseOrder(purchaseOrderDetails);

            }
            else
            {
                var purchaseOrderDetails = await _repository.GetLastestPurchaseOrderByPONumber(purchaseOrderStatusUpdateDto[0].PONumber);
                purchaseOrderDetails.PoStatus = PoStatus.Closed;
                await _repository.UpdatePurchaseOrder(purchaseOrderDetails);
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
                _logger.LogError($"Something went wrong inside DeletePurchasrOrder action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivePurchaseOrderNameList()
        {
            ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderIdNameListDto>>();
            try
            {
                var activePONameList = await _repository.GetAllActivePurchaseOrderNameList();
                var result = _mapper.Map<IEnumerable<PurchaseOrderIdNameListDto>>(activePONameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ActivePurchaseOrderNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActivePurchaseOrderNameList action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPurchaseOrderNameList action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllLatestRevNoPurchaseOrderNameList action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPOApprovalINameList action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPOApprovalIINameList action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPurchaseOrderApprovalIList action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllLastestPendingPOApprovalINameList action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPOApprovalIINameList action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPurchaseOrderApprovalIIIListForAvision action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPurchaseOrderApprovalIVListForAvision action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllLastestPendingPOApprovalIINameList action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllLastestPendingPOApprovalIIIListForAvision action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllLastestPendingPOApprovalIVListForAvision action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{PONumber}")]
        public async Task<IActionResult> ActivatePurchaseOrderApprovalI(string PONumber)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
                string serverKey = GetServerKey();
                var purchaseOrderDetailByPONumber = await _repository.GetPurchaseOrderByPONumber(PONumber);
                if (purchaseOrderDetailByPONumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrderApprovalI object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseOrderApprovalI with string: {PONumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchaseOrderDetailByPONumber.POApprovalI = true;
                purchaseOrderDetailByPONumber.POApprovedIBy = _createdBy;
                purchaseOrderDetailByPONumber.POApprovedIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseOrder_ForApproval(purchaseOrderDetailByPONumber);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"],"GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,Approval1";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);
                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    //var mails = new List<string>() {"bala@avisionsystems.com","anilyadav@avisionsystems.com" };
                    var mails = (emaildetails1.data.Where(x => x.operation == "Approval1").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));
                    //email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    var podate = purchaseOrderDetailByPONumber.PODate.ToString().Split(" ");
                    email.To.AddRange(mails.Select(x=> MailboxAddress.Parse(x)));

                    email.Subject = emaildetails.data.subject;
                    string body = emaildetails.data.template;
                    body = body.Replace("{{PO Number}}", purchaseOrderDetailByPONumber.PONumber);
                    body = body.Replace("{{PO Revision No}}", purchaseOrderDetailByPONumber.RevisionNumber.ToString());
                    body = body.Replace("{{PO Date}}", podate[0]);
                    body = body.Replace("{{PO Value}}", purchaseOrderDetailByPONumber.TotalAmount.ToString());
                    body = body.Replace("{{Vendor Name}}", purchaseOrderDetailByPONumber.VendorName.ToString());
                    body = body.Replace("{{Approval1}}", purchaseOrderDetailByPONumber.POApprovedIBy);
                    body = body.Replace("{{Approval2}}", "Awaiting");
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivatePurchaseOrderApprovalI action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{PONumber}")]
        public async Task<IActionResult> ActivatePurchaseOrderApprovalII(string PONumber)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
                string serverKey = GetServerKey();
                var purchaseOrderDetailByPONumber = await _repository.GetPurchaseOrderByPONumber(PONumber);
                if (purchaseOrderDetailByPONumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrderApprovalII object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseOrderApprovalII with string: {PONumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchaseOrderDetailByPONumber.POApprovalII = true;
                purchaseOrderDetailByPONumber.POApprovedIIBy = _createdBy;
                purchaseOrderDetailByPONumber.POApprovedIIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseOrder_ForApproval(purchaseOrderDetailByPONumber);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"],"GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,Approval2";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);

                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);                   
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));

                    // email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    var podate = purchaseOrderDetailByPONumber.PODate.ToString().Split(" ");
                    if (purchaseOrderDetailByPONumber.ApprovalCount > 2)
                    {
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
                        //var mails = new List<string>() { "scm@avisionsystems.com", "purchase@avisionsystems.com" };
                        //email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));
                        var mails = (emaildetails1.data.Where(x => x.operation == "Approval2").Select(x => x.emailIds).FirstOrDefault()).Split(',');
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivatePurchaseOrderApprovalII action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{PONumber}")]
        public async Task<IActionResult> ActivatePurchaseOrderApprovalIIIForAvision(string PONumber)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
                string serverKey = GetServerKey();
                var purchaseOrderDetailByPONumber = await _repository.GetPurchaseOrderByPONumber(PONumber);
                if (purchaseOrderDetailByPONumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrderApprovalII object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseOrderApprovalII with string: {PONumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchaseOrderDetailByPONumber.POApprovalIII = true;
                purchaseOrderDetailByPONumber.POApprovedIIIBy = _createdBy;
                purchaseOrderDetailByPONumber.POApprovedIIIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseOrder_ForApproval(purchaseOrderDetailByPONumber);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"],"GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,Approval3";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);

                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    //var mails =new List<string>() { "eyalbn@uvisionuav.com", "yonatan@uvisionuav.com"};
                    var mails = (emaildetails1.data.Where(x => x.operation == "Approval3").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                    var email = new MimeMessage();
                    //email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));
                    var podate = purchaseOrderDetailByPONumber.PODate.ToString().Split(" ");
                    email.To.AddRange(mails.Select(x=> MailboxAddress.Parse(x)));

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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivatePurchaseOrderApprovalIII action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{PONumber}")]
        public async Task<IActionResult> ActivatePurchaseOrderApprovalIVForAvision(string PONumber)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
                string serverKey = GetServerKey();
                var purchaseOrderDetailByPONumber = await _repository.GetPurchaseOrderByPONumber(PONumber);
                if (purchaseOrderDetailByPONumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseOrderApprovalIV object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseOrderApprovalIV with string: {PONumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }//
                purchaseOrderDetailByPONumber.POApprovalIV = true;
                purchaseOrderDetailByPONumber.POApprovedIVBy = _createdBy;
                purchaseOrderDetailByPONumber.POApprovedIVDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseOrder_ForApproval(purchaseOrderDetailByPONumber);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"],"GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,CreatePurchaseOrder";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);

                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    // var mails = new List<string>() { "scm@avisionsystems.com", "purchase@avisionsystems.com"};
                    var mails = (emaildetails1.data.Where(x => x.operation == "Approval4").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                    var email = new MimeMessage();
                    //email.From.Add(MailboxAddress.Parse("erp@avisionsystems.com"));
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));
                    var podate = purchaseOrderDetailByPONumber.PODate.ToString().Split(" ");
                    email.To.AddRange(mails.Select(x=> MailboxAddress.Parse(x)));

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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivatePurchaseOrderApprovalIV action: {ex.Message}");
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPONumberListByVendorName action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPOItemNumberListByPoNumber action";
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
                string result = await _repository.UpdatePurchaseOrder(purchaseOrderDetailById);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "PurchaseOrder have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Purchaseorderclosed action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                var poid_1 = poItemConfirmationDateDto.First();
                var PoId = poid_1.First();
                var PODetails = await _repository.GetPurchaseOrderById(PoId.PoId);
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

                    //Update PoConfirmationStatus in PurchaseOrder Table
                    purchaseOrderDetailById.PoConfirmationStatus = true;
                    string result = await _repository.UpdatePurchaseOrder(purchaseOrderDetailById);
                    _repository.SaveAsync();
                } // If the loop completes without returning a response, it means all sets were processed successfully
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    // var response = await _httpClient.GetAsync(string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"],"GetEmailTemplatebyProcessType?ProcessType=CreatePurchaseOrder"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,POConformation";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response.Content.ReadAsStringAsync();
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
                _logger.LogError($"Something went wrong inside PoConfirmationStatus action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                    await _repository.UpdatePurchaseOrder(poDetails);
                    _repository.SaveAsync();
                }
                else
                {
                    var poDetails = await _repository.GetPurchaseOrderById(poItemDetailByPoItemId.PurchaseOrderId);
                    poDetails.PoStatus = PoStatus.PartiallyClosed;
                    await _repository.UpdatePurchaseOrder(poDetails);
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
                _logger.LogError($"Something went wrong inside ChangePoItemSatusByPoItemId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside Tras_POSPReport action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside Tras_PurchaseOrder action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> PurchaseOrderReportswithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<PurchaseOrder_ReportDto> serviceResponse = new ServiceResponse<PurchaseOrder_ReportDto>();
            try
            {
                var result = await _repository.GetPurchaseOrderReportswithDate(FromDate, ToDate);
                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" PurchaseOrderReportswithDate hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned  PurchaseOrderReportswithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside PurchaseOrderReportswithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> PurchaseOrderReportswithPara([FromBody] PurchaseOrder_ReportGetDto paramsforPurchase)
        {
            ServiceResponse<PurchaseOrder_ReportDto> serviceResponse = new ServiceResponse<PurchaseOrder_ReportDto>();
            try
            {
                var result = await _repository.GetPurchaseOrderReportswithPara(paramsforPurchase.ItemNumber, paramsforPurchase.PONumber, paramsforPurchase.VendorName, paramsforPurchase.POStatus);
                if (result == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" PurchaseOrderReportswithPara hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"DeliveryOrder hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned  PurchaseOrderReportswithPara Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside PurchaseOrderReportswithPara action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
