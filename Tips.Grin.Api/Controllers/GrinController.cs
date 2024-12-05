using Microsoft.AspNetCore.Mvc;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Entities;
using AutoMapper;
using Tips.Grin.Api.Contracts;
using Contracts;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Entities;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Entities.DTOs;
using Tips.Grin.Api.Repository;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text;
using System.Dynamic;
using Azure.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Org.BouncyCastle.Asn1.Ocsp;
using Azure;
using Mysqlx.Crud;
using Org.BouncyCastle.Asn1.Anssi;
using MySqlX.XDevAPI.Common;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using EmailTemplateDto = Tips.Grin.Api.Entities.DTOs.EmailTemplateDto;
using MailKit.Security;
using System.Security.Claims;

//Test
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Grin.Api.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class GrinController : ControllerBase
    {
        private IGrinRepository _repository;
        private IGrinPartsRepository _grinPartsRepository;
        private IIQCConfirmationRepository _iQCConfirmationRepository;
        private IIQCConfirmationItemsRepository _iQCConfirmationItemsRepository;
        private IWeightedAvgCostRepository _weightedAvgCostRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IDocumentUploadRepository _documentUploadRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private readonly String _createdBy;
        private readonly String _unitname;

        public GrinController(IIQCConfirmationRepository iQCConfirmationRepository, IHttpClientFactory clientFactory,
         IIQCConfirmationItemsRepository iQCConfirmationItemsRepository, IGrinRepository repository, IHttpContextAccessor httpContextAccessor, IDocumentUploadRepository documentUploadRepository, IGrinPartsRepository grinPartsRepository,
           IWeightedAvgCostRepository weightedAvgCostRepository, IWebHostEnvironment webHostEnvironment, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _grinPartsRepository = grinPartsRepository;
            _iQCConfirmationRepository = iQCConfirmationRepository;
            _iQCConfirmationItemsRepository = iQCConfirmationItemsRepository;
            _weightedAvgCostRepository = weightedAvgCostRepository;
            _logger = logger;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _documentUploadRepository = documentUploadRepository;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
            //var tokenValue = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            //if (!string.IsNullOrEmpty(tokenValue) && tokenValue.StartsWith("Bearer "))
            //{
            //    var token = tokenValue.Substring(7);

            //    var tokenHandler = new JwtSecurityTokenHandler();
            //    var tokenObject = tokenHandler.ReadToken(token) as JwtSecurityToken;

            //}

        }
        // GET: api/<GrinController>
        [HttpGet]
        public async Task<IActionResult> GetGrinAndIqcsByPurchaseOrder(string Ponumber)
        {
            ServiceResponse<GrinandIqcDetail> serviceResponse = new ServiceResponse<GrinandIqcDetail>();
            try
            {
                if (Ponumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Ponumber object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Ponumber object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Ponumber object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Ponumber object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var getGrinIds = await _grinPartsRepository.GetGrinIdsByPonumber(Ponumber);

                if (getGrinIds.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No Grins Create for Ponumber: {Ponumber}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"No Grins Create for Ponumber: {Ponumber}");
                    return StatusCode(404, serviceResponse);
                }
                var Grindetails = await _repository.GetGrinDetailsofPOByGrinIds(getGrinIds, Ponumber);
                var Iqcs = new Dictionary<string, List<string>>();
                foreach (var grins in Grindetails)
                {
                    grins.GrinParts.ForEach(x=>x.Grins=null);
                    grins.GrinParts.ForEach(x => x.ProjectNumbers.ForEach(z=>z.GrinParts=null));
                    grins.OtherCharges.ForEach(x => x.Grins = null);

                    var partNo = grins.GrinParts.Where(x => x.IsIqcCompleted == true).Select(x => x.ItemNumber).ToList();
                    if (partNo.Count != 0)
                    {
                        Iqcs.Add(grins.GrinNumber, partNo);
                    }
                }
                List<IQCConfirmation>? IqcDetails = null;
                if (Iqcs.Count() != 0)
                {
                    IqcDetails = await _iQCConfirmationRepository.GetIqcDetailsByGrinNoAndParts(Iqcs);
                    IqcDetails.ForEach(x=>x.IQCConfirmationItems.ForEach(z=>z.IQCConfirmation=null));
                }
                _logger.LogInfo($"Returned all GetGrinAndIqcsByPurchaseOrder {Ponumber}");

                GrinandIqcDetail result = new GrinandIqcDetail()
                {
                    grins = Grindetails.ToList(),
                    iqcs = IqcDetails
                };
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Grins and Iqcs Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetGrinAndIqcsByPurchaseOrder Faced issue :\n{ex.Message}\n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Internal server error {ex.Message}:\n{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGrin([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)

        {
            ServiceResponse<IEnumerable<GrinDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinDto>>();

            try
            {
                var GetallGrins = await _repository.GetAllGrin(pagingParameter, searchParams);

                if (GetallGrins == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin data not found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin data not found in db");
                    return NotFound(serviceResponse);
                }
                var metadata = new
                {
                    GetallGrins.TotalCount,
                    GetallGrins.PageSize,
                    GetallGrins.CurrentPage,
                    GetallGrins.HasNext,
                    GetallGrins.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all Grins");
                var result = _mapper.Map<IEnumerable<GrinDto>>(GetallGrins);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Grins Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Internal server error {ex.Message}{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchGrinsDate([FromQuery] SearchDateParames searchDateParam)
        {
            ServiceResponse<IEnumerable<GrinReportDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinReportDto>>();
            try
            {
                var searchDateParamList = await _repository.SearchGrinsDate(searchDateParam);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<Grins, GrinReportDto>()
                        .ForMember(dest => dest.GrinParts, opt => opt.MapFrom(src => src.GrinParts
                            .Select(grinParts => new GrinPartsReportDto
                            {
                                Id = grinParts.Id,
                                ItemNumber = grinParts.ItemNumber,
                                LotNumber = grinParts.LotNumber,
                                GrinNumber = src.GrinNumber,
                                GrinPartId = grinParts.Id,
                                Qty = grinParts.Qty,
                                ItemDescription = grinParts.ItemDescription,
                                PONumber = grinParts.PONumber,
                                MftrItemNumber = grinParts.MftrItemNumber,
                                ManufactureBatchNumber = grinParts.ManufactureBatchNumber,
                                UnitPrice = grinParts.UnitPrice,
                                POOrderQty = grinParts.POOrderQty,
                                POBalancedQty = grinParts.POBalancedQty,
                                POUnitPrice = grinParts.POUnitPrice,
                                AcceptedQty = grinParts.AcceptedQty,
                                RejectedQty = grinParts.RejectedQty,
                                AverageCost = grinParts.AverageCost,
                                UOM = grinParts.UOM,
                                UOC = grinParts.UOC,
                                ExpiryDate = grinParts.ExpiryDate,
                                ManufactureDate = grinParts.ManufactureDate,

                                COCUpload = grinParts.CoCUpload,
                                //.Select(documentUpload => new DocumentUploadDto
                                //{
                                //    Id = documentUpload.Id,
                                //    FileName = documentUpload.FileName,
                                //    FileExtension = documentUpload.FileExtension,
                                //    FilePath = documentUpload.FilePath,
                                //    CreatedBy = documentUpload.CreatedBy,
                                //    CreatedOn = documentUpload.CreatedOn,
                                //    LastModifiedBy = documentUpload.LastModifiedBy,
                                //    LastModifiedOn = documentUpload.LastModifiedOn,
                                //}).ToList(),

                                SGST = grinParts.SGST,
                                IGST = grinParts.IGST,
                                CGST = grinParts.CGST,
                                UTGST = grinParts.UTGST,

                                ProjectNumbers = grinParts.ProjectNumbers
                                    .Select(projectNumbers => new ProjectNumbersReportDto
                                    {
                                        Id = projectNumbers.Id,
                                        GrinNumber = src.GrinNumber,
                                        ItemNumber = grinParts.ItemNumber,
                                        ProjectNumber = projectNumbers.ProjectNumber,
                                        ProjectQty = projectNumbers.ProjectQty
                                    }).ToList()
                            })
                         ));
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<GrinReportDto>>(searchDateParamList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SearchGrinsDate";
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
        public async Task<IActionResult> SearchGrins([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<GrinReportDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinReportDto>>();
            try
            {
                var grinsList = await _repository.SearchGrins(searchParams);

                _logger.LogInfo("Returned all Grins");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<GrinDto, Grins>().ReverseMap()
                //    .ForMember(dest => dest.GrinParts, opt => opt.MapFrom(src => src.GrinParts));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<Grins, GrinReportDto>()
                        .ForMember(dest => dest.GrinParts, opt => opt.MapFrom(src => src.GrinParts
                            .Select(grinParts => new GrinPartsReportDto
                            {
                                Id = grinParts.Id,
                                ItemNumber = grinParts.ItemNumber,
                                LotNumber = grinParts.LotNumber,
                                GrinNumber = src.GrinNumber,
                                GrinPartId = grinParts.Id,
                                Qty = grinParts.Qty,
                                ItemDescription = grinParts.ItemDescription,
                                PONumber = grinParts.PONumber,
                                MftrItemNumber = grinParts.MftrItemNumber,
                                ManufactureBatchNumber = grinParts.ManufactureBatchNumber,
                                UnitPrice = grinParts.UnitPrice,
                                POOrderQty = grinParts.POOrderQty,
                                POBalancedQty = grinParts.POBalancedQty,
                                POUnitPrice = grinParts.POUnitPrice,
                                AcceptedQty = grinParts.AcceptedQty,
                                RejectedQty = grinParts.RejectedQty,
                                AverageCost = grinParts.AverageCost,
                                UOM = grinParts.UOM,
                                UOC = grinParts.UOC,
                                ExpiryDate = grinParts.ExpiryDate,
                                ManufactureDate = grinParts.ManufactureDate,

                                COCUpload = grinParts.CoCUpload,
                                //.Select(documentUpload => new DocumentUploadDto
                                //{
                                //    Id = documentUpload.Id,
                                //    FileName = documentUpload.FileName,
                                //    FileExtension = documentUpload.FileExtension,
                                //    FilePath = documentUpload.FilePath,
                                //    CreatedBy = documentUpload.CreatedBy,
                                //    CreatedOn = documentUpload.CreatedOn,
                                //    LastModifiedBy = documentUpload.LastModifiedBy,
                                //    LastModifiedOn = documentUpload.LastModifiedOn,
                                //}).ToList(),

                                SGST = grinParts.SGST,
                                IGST = grinParts.IGST,
                                CGST = grinParts.CGST,
                                UTGST = grinParts.UTGST,

                                ProjectNumbers = grinParts.ProjectNumbers
                                    .Select(projectNumbers => new ProjectNumbersReportDto
                                    {
                                        Id = projectNumbers.Id,
                                        GrinNumber = src.GrinNumber,
                                        ItemNumber = grinParts.ItemNumber,
                                        ProjectNumber = projectNumbers.ProjectNumber,
                                        ProjectQty = projectNumbers.ProjectQty
                                    }).ToList()
                            })
                         ));
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<GrinReportDto>>(grinsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Grins";
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
        public async Task<IActionResult> GetAllGrinsWithItems([FromBody] GrinSearchDto grinSearchDto)
        {
            ServiceResponse<IEnumerable<GrinReportDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinReportDto>>();
            try
            {
                var grinSearchDtoList = await _repository.GetAllGrinsWithItems(grinSearchDto);

                _logger.LogInfo("Returned all Grins");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<GrinDto, Grins>().ReverseMap()
                //    .ForMember(dest => dest.GrinParts, opt => opt.MapFrom(src => src.GrinParts));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<Grins, GrinReportDto>()
                        .ForMember(dest => dest.GrinParts, opt => opt.MapFrom(src => src.GrinParts
                            .Select(grinParts => new GrinPartsReportDto
                            {
                                Id = grinParts.Id,
                                ItemNumber = grinParts.ItemNumber,
                                LotNumber = grinParts.LotNumber,
                                GrinNumber = src.GrinNumber,
                                GrinPartId = grinParts.Id,
                                Qty = grinParts.Qty,
                                ItemDescription = grinParts.ItemDescription,
                                PONumber = grinParts.PONumber,
                                MftrItemNumber = grinParts.MftrItemNumber,
                                ManufactureBatchNumber = grinParts.ManufactureBatchNumber,
                                UnitPrice = grinParts.UnitPrice,
                                POOrderQty = grinParts.POOrderQty,
                                POBalancedQty = grinParts.POBalancedQty,
                                POUnitPrice = grinParts.POUnitPrice,
                                AcceptedQty = grinParts.AcceptedQty,
                                RejectedQty = grinParts.RejectedQty,
                                AverageCost = grinParts.AverageCost,
                                UOM = grinParts.UOM,
                                UOC = grinParts.UOC,
                                ExpiryDate = grinParts.ExpiryDate,
                                ManufactureDate = grinParts.ManufactureDate,

                                COCUpload = grinParts.CoCUpload,
                                //.Select(documentUpload => new DocumentUploadDto
                                //{
                                //    Id = documentUpload.Id,
                                //    FileName = documentUpload.FileName,
                                //    FileExtension = documentUpload.FileExtension,
                                //    FilePath = documentUpload.FilePath,
                                //    CreatedBy = documentUpload.CreatedBy,
                                //    CreatedOn = documentUpload.CreatedOn,
                                //    LastModifiedBy = documentUpload.LastModifiedBy,
                                //    LastModifiedOn = documentUpload.LastModifiedOn,
                                //}).ToList(),

                                SGST = grinParts.SGST,
                                IGST = grinParts.IGST,
                                CGST = grinParts.CGST,
                                UTGST = grinParts.UTGST,

                                ProjectNumbers = grinParts.ProjectNumbers
                                    .Select(projectNumbers => new ProjectNumbersReportDto
                                    {
                                        Id = projectNumbers.Id,
                                        GrinNumber = src.GrinNumber,
                                        ItemNumber = grinParts.ItemNumber,
                                        ProjectNumber = projectNumbers.ProjectNumber,
                                        ProjectQty = projectNumbers.ProjectQty
                                    }).ToList()
                            })
                         ));
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<GrinReportDto>>(grinSearchDtoList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Grins";
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
        // GET api/<GrinController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGrinById(int id)
        {
            ServiceResponse<GrinItemMasterEnggDto> serviceResponse = new ServiceResponse<GrinItemMasterEnggDto>();

            try
            {
                var GrinDetailsbyId = await _repository.GetGrinById(id);

                if (GrinDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else

                {
                    _logger.LogInfo($"Returned GrinDetailsById with id: {id}");
                    GrinItemMasterEnggDto grinItemMasterEnggDto = _mapper.Map<GrinItemMasterEnggDto>(GrinDetailsbyId);


                    List<GrinPartsItemMasterEnggDto> grinPartsItemMasterEnggList = new List<GrinPartsItemMasterEnggDto>();


                    foreach (var GrinpartsDetails in GrinDetailsbyId.GrinParts)
                    {
                        GrinPartsItemMasterEnggDto grinPartsItemMasterEnggDto = _mapper.Map<GrinPartsItemMasterEnggDto>(GrinpartsDetails);
                        grinPartsItemMasterEnggDto.ProjectNumbers = _mapper.Map<List<ProjectNumbersDto>>(GrinpartsDetails.ProjectNumbers);
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = grinPartsItemMasterEnggDto.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterDetails = await client.SendAsync(request);
                        var inventoryObjectString = await itemMasterDetails.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic inventoryObject = inventoryObjectData.data;
                        grinPartsItemMasterEnggDto.DrawingNo = inventoryObject.drawingNo;
                        grinPartsItemMasterEnggDto.DocRet = inventoryObject.docRet;
                        grinPartsItemMasterEnggDto.RevNo = inventoryObject.revNo;
                        grinPartsItemMasterEnggDto.IsCocRequired = inventoryObject.isCocRequired;
                        grinPartsItemMasterEnggDto.IsRohsItem = inventoryObject.isRohsItem;
                        grinPartsItemMasterEnggDto.IsShelfLife = inventoryObject.isShelfLife;
                        grinPartsItemMasterEnggDto.IsReachItem = inventoryObject.isReachItem;

                        grinPartsItemMasterEnggList.Add(grinPartsItemMasterEnggDto);

                    }

                    grinItemMasterEnggDto.GrinParts = grinPartsItemMasterEnggList;
                    serviceResponse.Data = grinItemMasterEnggDto;
                    serviceResponse.Message = $"Returned Grin with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGrinById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateGrin([FromBody] GrinPostDto grinPostDto)
        {
            ServiceResponse<GrinDto> serviceResponse = new ServiceResponse<GrinDto>();

            try
            {
                string serverKey = GetServerKey();

                if (grinPostDto is null)
                {
                    _logger.LogError("Grin object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Grin object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Grin object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Grin object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var grins = _mapper.Map<Grins>(grinPostDto);
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));
                HttpStatusCode invStatusCode = HttpStatusCode.OK;

                if (serverKey == "avision")
                {
                    var grinNum = await _repository.GenerateGrinNumberForAvision();
                    grins.GrinNumber = grinNum;
                }
                else
                {
                    var dateFormat = days + months + years;
                    var grinNumber = await _repository.GenerateGrinNumber();
                    var grinNo = dateFormat + grinNumber;
                    grins.GrinNumber = grinNo;
                }
                var grinPartsDto = grinPostDto.GrinParts;
                var grinCal = _mapper.Map<List<GrinPartscalculationofAvgcost>>(grinPartsDto);
                List<GrinParts> grinPartsList = new List<GrinParts>();

                var othercosttotal = grins.Freight + grins.Insurance + grins.LoadingorUnLoading + grins.Transport;
                decimal? conversionrate = 1;
                if (grins.CurrencyConversion > 1) conversionrate = grins.CurrencyConversion;
                foreach (var gPart in grinCal)
                {
                    decimal? EP = gPart.Qty * gPart.UnitPrice;
                    decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
                    gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
                    gPart.EPforSingleQty = gPart.EPwithTax / gPart.Qty;
                }
                decimal? SumofEPwithtax = grinCal.Sum(x => x.EPwithTax);
                foreach (var gPart in grinCal)
                {
                    decimal? distriduteOthercostforitem = (gPart.EPwithTax / SumofEPwithtax) * othercosttotal;
                    decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / gPart.Qty;
                    gPart.AverageCost = distriduteOthercostforitemsSingleQty + gPart.EPforSingleQty;
                    GrinParts grinParts = _mapper.Map<GrinParts>(gPart);
                    grinPartsList.Add(grinParts);
                }

                grins.GrinParts = grinPartsList;
                grins.IsGrinCompleted = true;

                await _repository.CreateGrin(grins);
                _repository.SaveAsync();

                if (grins.GrinParts != null)
                {
                    foreach (var grinPart in grins.GrinParts)
                    {
                        var grinPartsId = await _grinPartsRepository.GetGrinPartsById(grinPart.Id);
                        grinPartsId.LotNumber = grins.GrinNumber + grinPartsId.Id;
                        await _grinPartsRepository.UpdateGrinQty(grinPartsId);

                    }
                }
                // HttpStatusCode createinvforServiceItemsResp = HttpStatusCode.OK;
                HttpStatusCode createinvResp = HttpStatusCode.OK;
                HttpStatusCode createinvTrancResp = HttpStatusCode.OK;
                HttpStatusCode getItemmResp = HttpStatusCode.OK;
                HttpStatusCode getItemForIqcResp = HttpStatusCode.OK;
                HttpStatusCode UpdatePoStatus = HttpStatusCode.OK;
                HttpStatusCode UpdatePoQty = HttpStatusCode.OK;
                HttpStatusCode UpdatePoProjQty = HttpStatusCode.OK;

                foreach (var parts in grinPartsList)
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = parts.ItemNumber;
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                        getItemmResp = itemMasterObjectResult.StatusCode;
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;

                    if (parts.ProjectNumbers != null)
                    {
                        foreach (var project in parts.ProjectNumbers)
                        {
                            GrinInventoryDto grinInventoryDto = new GrinInventoryDto();
                            grinInventoryDto.PartNumber = parts.ItemNumber;
                            grinInventoryDto.LotNumber = parts.LotNumber;
                            grinInventoryDto.MftrPartNumber = parts.MftrItemNumber;
                            grinInventoryDto.Description = parts.ItemDescription;
                            grinInventoryDto.ProjectNumber = project.ProjectNumber;
                            grinInventoryDto.Balance_Quantity = Convert.ToDecimal(project.ProjectQty);
                            grinInventoryDto.Max = itemMasterObject.max;
                            grinInventoryDto.Min = itemMasterObject.min;
                            grinInventoryDto.UOM = parts.UOM;
                            grinInventoryDto.Warehouse = "GRIN";
                            grinInventoryDto.Location = "GRIN";
                            grinInventoryDto.GrinNo = grins.GrinNumber;
                            grinInventoryDto.GrinPartId = parts.Id;
                            grinInventoryDto.PartType = parts.ItemType;
                            grinInventoryDto.ReferenceID = grins.GrinNumber;
                            grinInventoryDto.ReferenceIDFrom = "GRIN";
                            grinInventoryDto.GrinMaterialType = "";
                            grinInventoryDto.ShopOrderNo = "";


                            var json = JsonConvert.SerializeObject(grinInventoryDto);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                            "CreateInventoryFromGrin"))
                            {
                                Content = data
                            };
                            request1.Headers.Add("Authorization", token1);

                            var response = await client1.SendAsync(request1);
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                createinvResp = response.StatusCode;
                            }
                        }
                    }
                }

                foreach (var parts in grinPartsList)
                {
                    if (parts.ProjectNumbers != null)
                    {
                        foreach (var project in parts.ProjectNumbers)
                        {
                            grinInventoryTrasactionPostDto grinInventoryTranctionDto = new grinInventoryTrasactionPostDto();
                            grinInventoryTranctionDto.PartNumber = parts.ItemNumber;
                            grinInventoryTranctionDto.LotNumber = parts.LotNumber;
                            grinInventoryTranctionDto.MftrPartNumber = parts.MftrItemNumber;
                            grinInventoryTranctionDto.Description = parts.ItemDescription;
                            grinInventoryTranctionDto.ProjectNumber = project.ProjectNumber;
                            grinInventoryTranctionDto.Issued_Quantity = Convert.ToDecimal(project.ProjectQty);
                            grinInventoryTranctionDto.Issued_DateTime = DateTime.Now;
                            grinInventoryTranctionDto.Issued_By = _createdBy;
                            grinInventoryTranctionDto.UOM = parts.UOM;
                            grinInventoryTranctionDto.Warehouse = "GRIN";
                            grinInventoryTranctionDto.From_Location = "GRIN";
                            grinInventoryTranctionDto.TO_Location = "GRIN";
                            grinInventoryTranctionDto.GrinNo = grins.GrinNumber;
                            grinInventoryTranctionDto.GrinPartId = parts.Id;
                            grinInventoryTranctionDto.PartType = parts.ItemType;
                            grinInventoryTranctionDto.ReferenceID = grins.GrinNumber;
                            grinInventoryTranctionDto.ReferenceIDFrom = "GRIN";
                            grinInventoryTranctionDto.GrinMaterialType = "GRIN";
                            grinInventoryTranctionDto.shopOrderNo = "";
                            grinInventoryTranctionDto.IsStockAvailable = true;
                            grinInventoryTranctionDto.Remarks = "GRIN";

                            var json = JsonConvert.SerializeObject(grinInventoryTranctionDto);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranctionFromGrin"))
                            {
                                Content = data
                            };
                            request1.Headers.Add("Authorization", token1);

                            var response = await client1.SendAsync(request1);
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                createinvTrancResp = response.StatusCode;
                            }
                        }
                    }
                }

                var grinPartsDetail = _mapper.Map<List<GrinUpdateQtyDetailsDto>>(grinPartsDto);
                var jsons = JsonConvert.SerializeObject(grinPartsDetail);
                var data1 = new StringContent(jsons, Encoding.UTF8, "application/json");
                var client2 = _clientFactory.CreateClient();
                var token2 = HttpContext.Request.Headers["Authorization"].ToString();
                var request2 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"],
                "UpdateBalanceQtyDetails"))
                {
                    Content = data1
                };
                request2.Headers.Add("Authorization", token2);

                var responses = await client2.SendAsync(request2);
                if (responses.StatusCode != HttpStatusCode.OK)
                {
                    UpdatePoQty = responses.StatusCode;
                }
                if (grinPartsDto.Count() > 0)
                {
                    foreach (var grinparts in grinPartsDto)
                    {
                        List<GrinUpdateProjectBalQtyDetailsDto> projectNameDtos = new List<GrinUpdateProjectBalQtyDetailsDto>();
                        foreach (var projectNo in grinparts.ProjectNumbers)
                        {
                            var grinPartsProjectNoDtoDetail = _mapper.Map<GrinUpdateProjectBalQtyDetailsDto>(projectNo);
                            grinPartsProjectNoDtoDetail.ItemNumber = grinparts.ItemNumber;
                            grinPartsProjectNoDtoDetail.PoItemId = grinparts.PoItemId;
                            projectNameDtos.Add(grinPartsProjectNoDtoDetail);
                        }

                        var jsonss = JsonConvert.SerializeObject(projectNameDtos);
                        var data2 = new StringContent(jsonss, Encoding.UTF8, "application/json");
                        var client3 = _clientFactory.CreateClient();
                        var token3 = HttpContext.Request.Headers["Authorization"].ToString();
                        var request3 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"],
                        "UpdatePoProjectNoBalanceQtyDetails"))
                        {
                            Content = data2
                        };
                        request3.Headers.Add("Authorization", token3);

                        var results = await client3.SendAsync(request3);
                        if (results.StatusCode != HttpStatusCode.OK)
                        {
                            UpdatePoProjQty = results.StatusCode;
                        }
                    }
                }

                var grinPartsDetails = _mapper.Map<List<GrinQtyPoStatusUpdateDto>>(grinPartsDto);
                var jsonCon = JsonConvert.SerializeObject(grinPartsDetails);
                var data3 = new StringContent(jsonCon, Encoding.UTF8, "application/json");
                var client4 = _clientFactory.CreateClient();
                var token4 = HttpContext.Request.Headers["Authorization"].ToString();
                var request4 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"],
                "UpdatePoStatus"))
                {
                    Content = data3
                };
                request4.Headers.Add("Authorization", token4);

                var result = await client4.SendAsync(request4);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    UpdatePoStatus = result.StatusCode;
                }
                // createinvforServiceItemsResp == HttpStatusCode.OK &&
                if (getItemmResp == HttpStatusCode.OK && UpdatePoStatus == HttpStatusCode.OK && UpdatePoQty == HttpStatusCode.OK
                    && UpdatePoProjQty == HttpStatusCode.OK && createinvTrancResp == HttpStatusCode.OK && createinvResp == HttpStatusCode.OK)
                {
                    _repository.SaveAsync();
                    _grinPartsRepository.SaveAsync();

                    if (grins.GrinParts != null)
                    {
                        List<string> grinPartsItemNoListDtos = new List<string>();
                        foreach (var grinParts in grins.GrinParts)
                        {
                            var grinPartsProjectNoDtoDetail = _mapper.Map<string>(grinParts.ItemNumber);
                            grinPartsItemNoListDtos.Add(grinPartsProjectNoDtoDetail);
                        }

                        var jsonss = JsonConvert.SerializeObject(grinPartsItemNoListDtos);
                        var data4 = new StringContent(jsonss, Encoding.UTF8, "application/json");
                        var client5 = _clientFactory.CreateClient();
                        var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                        var request5 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterEnggAPI"],
                        "GetAllClosedIqcItemMasterItemNoList"))
                        {
                            Content = data4
                        };
                        request5.Headers.Add("Authorization", token5);

                        var itemMasterObjectResult = await client5.SendAsync(request5);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                        {
                            getItemForIqcResp = itemMasterObjectResult.StatusCode;
                        }
                        if (itemMasterObjectResult.StatusCode == HttpStatusCode.OK)
                        {
                            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                            dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                            dynamic itemMasterObject = itemMasterObjectData;

                            if (itemMasterObject != null && itemMasterObject.Count > 0)
                            {
                                for (int i = 0; i < grins.GrinParts.Count; i++)
                                {
                                    var grinPartItemNo = grins.GrinParts[i].ItemNumber;
                                    for (int j = 0; j < itemMasterObject.Count; j++)
                                    {
                                        if (itemMasterObject[j] == grinPartItemNo)
                                        {
                                            var iqcGrinDetails = _mapper.Map<GrinIQCConfirmationSaveDto>(grins);
                                            iqcGrinDetails.GrinId = grins.Id;
                                            iqcGrinDetails.GrinNumber = grins.GrinNumber;
                                            iqcGrinDetails.VendorId = grins.VendorId;
                                            iqcGrinDetails.VendorNumber = grins.VendorNumber;
                                            iqcGrinDetails.VendorName = grins.VendorName;
                                            GrinIQCConfirmationItemsSaveDto grinIQCConfirmationItemsSaveDto = _mapper.Map<GrinIQCConfirmationItemsSaveDto>(grins.GrinParts[i]);
                                            grinIQCConfirmationItemsSaveDto.ItemNumber = grins.GrinParts[i].ItemNumber;
                                            grinIQCConfirmationItemsSaveDto.GrinPartId = grins.GrinParts[i].Id;
                                            grinIQCConfirmationItemsSaveDto.ReceivedQty = grins.GrinParts[i].Qty;
                                            grinIQCConfirmationItemsSaveDto.AcceptedQty = grins.GrinParts[i].Qty;
                                            grinIQCConfirmationItemsSaveDto.RejectedQty = grins.GrinParts[i].RejectedQty;
                                            iqcGrinDetails.GrinIQCConfirmationItemsPostDtos = grinIQCConfirmationItemsSaveDto;

                                            await CreateIQCConfirmationItems(iqcGrinDetails);

                                        }
                                    }
                                }
                            }

                        }

                    }

                }
                else
                {
                    _logger.LogError($"Something went wrong inside Create CreateGrin action: Other Service Calling");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Saving Failed";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                if (serverKey == "avision")
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreateGRIN"));
                    request.Headers.Add("Authorization", token);
                    var response = await client.SendAsync(request);
                    var EmailTempString = await response.Content.ReadAsStringAsync();
                    var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                    var Operations = "From,CreateGRIN";
                    var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                    request1.Headers.Add("Authorization", token);
                    var response1 = await client.SendAsync(request1);
                    var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                    var emaildetails1 = JsonConvert.DeserializeObject<Tips.Grin.Api.Entities.Dto.EmailIDsDto>(EmailTempString1);
                    var httpclientHandler = new HttpClientHandler();
                    var httpClient = new HttpClient(httpclientHandler);
                    var mails = (emaildetails1.data.Where(x => x.operation == "CreateGRIN").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));

                    email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));

                    email.Subject = emaildetails.data.subject;
                    string body = emaildetails.data.template;
                    body = body.Replace("{{GRIN Numbers}}", grins.GrinNumber);
                    body = body.Replace("{{Vendor Id}}", grins.VendorNumber);
                    body = body.Replace("{{Vendor Name}}", grins.VendorName);
                    body = body.Replace("{{Created By}}", grins.CreatedBy);
                    body = body.Replace("{{Created Dated}}", grins.CreatedOn.ToString());
                    string? ProjectNos = null;
                    List<string>? tempProj = new List<string>();
                    List<string>? tempPRno = new List<string>();
                    string? PONos = null;
                    foreach (var item in grins.GrinParts)
                    {
                        if (item.ProjectNumbers.Count > 0)
                            foreach (var project in item.ProjectNumbers)
                            {
                                if (ProjectNos.IsNullOrEmpty()) { ProjectNos = project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                                else if (!tempProj.Contains(project.ProjectNumber)) { ProjectNos = ProjectNos + ", " + project.ProjectNumber; tempProj.Add(project.ProjectNumber); }
                            }

                        if (PONos.IsNullOrEmpty()) { PONos = item.PONumber; tempPRno.Add(item.PONumber); }
                        else if (!tempPRno.Contains(item.PONumber)) { PONos = PONos + ", " + item.PONumber; tempPRno.Add(item.PONumber); }
                    }
                    body = body.Replace("{{Project Ref No}}", ProjectNos);
                    body = body.Replace("{{PurchaseOrder Number}}", PONos);

                    email.Body = new TextPart(TextFormat.Html) { Text = body };

                    using var smtp = new MailKit.Net.Smtp.SmtpClient();
                    int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                    smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                    smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                    smtp.Send(email);
                    smtp.Disconnect(true);

                }
                serviceResponse.Data = null;
                serviceResponse.Message = "Grin Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetGrinById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        private async Task<IActionResult> CreateIQCConfirmationItems([FromBody] GrinIQCConfirmationSaveDto iqcConfirmationSaveDto)
        {
            ServiceResponse<GrinIQCConfirmationSaveDto> serviceResponse = new ServiceResponse<GrinIQCConfirmationSaveDto>();

            try
            {
                string serverKey = GetServerKey();

                if (iqcConfirmationSaveDto is null)
                {
                    _logger.LogError("Create IQCConfirmation object sent from the client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Create IQCConfirmation object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Create IQCConfirmation object sent from the client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Create IQCConfirmation object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var iqcConfirmation = _mapper.Map<IQCConfirmation>(iqcConfirmationSaveDto);
                var iqcConfirmationItemsDto = iqcConfirmationSaveDto.GrinIQCConfirmationItemsPostDtos;
                var iqcConfirmationItems = _mapper.Map<IQCConfirmationItems>(iqcConfirmationItemsDto);
                var grinNumber = iqcConfirmation.GrinNumber;
                var GrinId = iqcConfirmation.GrinId;

                HttpStatusCode getInvGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode updateInvTranc = HttpStatusCode.OK;
                HttpStatusCode createInv = HttpStatusCode.OK;
                HttpStatusCode createInvTranc = HttpStatusCode.OK;
                HttpStatusCode createInvTranc1 = HttpStatusCode.OK;
                HttpStatusCode getItemmResp = HttpStatusCode.OK;

                var existingIqcConfirmation = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);

                if (existingIqcConfirmation != null)
                {
                    iqcConfirmationItems.IQCConfirmationId = existingIqcConfirmation.Id;

                    var grinPartId = iqcConfirmationItemsDto.GrinPartId;
                    var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartId);
                    if (iqcConfirmationItemsDto.GrinPartId != grinPartsDetails.Id)
                    {
                        _logger.LogError($"Invalid Grin Part Id {grinPartId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid Grin Part Id {grinPartId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    //Updating IQC Status in IQCItem
                    iqcConfirmationItems.IsIqcCompleted = true;
                    await _iQCConfirmationItemsRepository.CreateIqcItem(iqcConfirmationItems);
                    _iQCConfirmationItemsRepository.SaveAsync();

                    //Updating AcceptedQty, RejectedQty and IQC Status in GrinParts
                    var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinPartsQty(iqcConfirmationItems.GrinPartId, iqcConfirmationItems.AcceptedQty.ToString(), iqcConfirmationItems.RejectedQty.ToString());
                    var grinParts = _mapper.Map<GrinParts>(updatedGrinPartsQty);
                    grinParts.IsIqcCompleted = true;
                    await _grinPartsRepository.UpdateGrinQty(grinParts);
                    _grinPartsRepository.SaveAsync();

                    //Updating IQC Status in Grin And Iqc MainLevel
                    var grinPartsCount = await _grinPartsRepository.GetGrinPartsCount(GrinId);
                    var iqcConfomationCount = await _iQCConfirmationItemsRepository.GetIQCConformationItemsCount(existingIqcConfirmation.Id);

                    if (grinPartsCount == iqcConfomationCount)
                    {
                        var iqcDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                        iqcDetails.IsIqcCompleted = true;
                        await _iQCConfirmationRepository.UpdateIqc(iqcDetails);

                        var grinDetails = await _repository.GetGrinByGrinNo(grinNumber);
                        grinDetails.IsIqcCompleted = true;
                        await _repository.UpdateGrin(grinDetails);
                    }

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = iqcConfirmationItemsDto.ItemNumber;
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                        getItemmResp = itemMasterObjectResult.StatusCode;

                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;
                    //Inventory Update Code
                    decimal? acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    decimal rejectedQty = iqcConfirmationItemsDto.RejectedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinPartId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.ProjectNumbers)
                    {
                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var grinNo = iqcConfirmation.GrinNumber;
                        var encodedgrinNo = Uri.EscapeDataString(grinNo);
                        var grinPartsIds = projectNo.GrinPartsId;
                        var projectNos = projectNo.ProjectNumber;
                        var encodedprojectNos = Uri.EscapeDataString(projectNos);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                        request1.Headers.Add("Authorization", token1);

                        var inventoryObjectResult = await client1.SendAsync(request1);
                        if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinId = inventoryObjectResult.StatusCode;
                        var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic inventoryObject = inventoryObjectData.data;
                        if (inventoryObject != null)
                        {
                            decimal balanceQty = inventoryObject.balance_Quantity;
                            int flag1 = 0;
                            int flag2 = 0;
                            decimal bal = 0;
                            if (inventoryObject.balance_Quantity <= acceptedQty && inventoryObject.balance_Quantity != 0)
                            {
                                inventoryObject.warehouse = "IQC";
                                inventoryObject.location = "IQC";
                                inventoryObject.referenceIDFrom = "GRIN";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceIDFrom = "GRIN";
                                    flag1 = 1;
                                }
                                else
                                {
                                    bal = inventoryObject.balance_Quantity - acceptedQty;
                                    if (bal != 0)
                                    {
                                        flag2 = 1;
                                    }
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceIDFrom = "GRIN";
                                    acceptedQty = 0;
                                }
                            }

                            var json = JsonConvert.SerializeObject(inventoryObject);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var client5 = _clientFactory.CreateClient();
                            var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                            "UpdateInventory?id=", inventoryObject.id))
                            {
                                Content = data
                            };
                            request5.Headers.Add("Authorization", token5);

                            var response = await client5.SendAsync(request5);
                            if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

                            //InventoryTranction Update Code

                            IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                            iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                            iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                            iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrItemNumber;
                            iqcInventoryTranctionDto.Description = inventoryObject.description;
                            iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                            iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            iqcInventoryTranctionDto.Issued_DateTime = DateTime.Now;
                            iqcInventoryTranctionDto.Issued_By = _createdBy;
                            iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto.Warehouse = "IQC";
                            iqcInventoryTranctionDto.From_Location = "GRIN";
                            iqcInventoryTranctionDto.TO_Location = "IQC";
                            iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; 
                            iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;/* Convert.ToString(grinPartsDetails.Id);*/
                            iqcInventoryTranctionDto.ReferenceIDFrom = "IQC";
                            iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                            iqcInventoryTranctionDto.ShopOrderNo = "";
                            iqcInventoryTranctionDto.Remarks = "GRINIQC Done";
                            iqcInventoryTranctionDto.IsStockAvailable = inventoryObject.isStockAvailable;

                            string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                            var contents = new StringContent(rfqSourcingPPdetailsJsons, Encoding.UTF8, "application/json");
                            var client7 = _clientFactory.CreateClient();
                            var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranction"))
                            {
                                Content = contents
                            };
                            request7.Headers.Add("Authorization", token7);

                            var inventoryTransResponses = await client7.SendAsync(request7);
                            if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTranc = inventoryTransResponses.StatusCode;

                            if (iqcConfirmationItemsDto.RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                            {
                                IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                grinInventoryDto.PartNumber = iqcConfirmationItemsDto.ItemNumber;
                                grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                                grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                                grinInventoryDto.ProjectNumber = projectNos;
                                grinInventoryDto.Balance_Quantity = Convert.ToDecimal(iqcConfirmationItemsDto.RejectedQty);
                                grinInventoryDto.Max = itemMasterObject.max;
                                grinInventoryDto.Min = itemMasterObject.min;
                                grinInventoryDto.UOM = grinPartsDetails.UOM;
                                grinInventoryDto.Warehouse = "Reject";
                                grinInventoryDto.Location = "Reject";
                                grinInventoryDto.GrinNo = iqcConfirmation.GrinNumber;
                                grinInventoryDto.GrinPartId = iqcConfirmationItemsDto.GrinPartId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;
                                grinInventoryDto.ReferenceID = iqcConfirmation.GrinNumber;
                                grinInventoryDto.ReferenceIDFrom = "GRIN";
                                grinInventoryDto.GrinMaterialType = "GRIN";
                                grinInventoryDto.ShopOrderNo = "";
                                if (flag1 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = rejectedQty;
                                }
                                else if (flag2 == 1)
                                {
                                    grinInventoryDto.Balance_Quantity = bal;
                                    rejectedQty -= bal;
                                }

                                string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                var client6 = _clientFactory.CreateClient();
                                var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                "CreateInventoryFromGrin"))
                                {
                                    Content = content
                                };
                                request6.Headers.Add("Authorization", token6);

                                var response1 = await client6.SendAsync(request6);
                                if (response1.StatusCode != HttpStatusCode.OK) createInv = response1.StatusCode;


                                //InventoryTranction Update Code

                                IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                iqcInventoryTranctionDtos.Issued_DateTime = DateTime.Now;
                                iqcInventoryTranctionDtos.Issued_By = _createdBy;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = "GRIN";
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo; 
                                iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.GrinNo;/* Convert.ToString(grinPartsDetails.Id);*/
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "IQC";
                                iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                iqcInventoryTranctionDtos.ShopOrderNo = "";
                                iqcInventoryTranctionDtos.Remarks = "GRINIQC Done";
                                //iqcInventoryTranctionDtos.IsStockAvailable = grinInventoryDto.is

                                string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                                var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                                var client8 = _clientFactory.CreateClient();
                                var token8 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = contents1
                                };
                                request8.Headers.Add("Authorization", token8);

                                var inventoryTransResponses1 = await client8.SendAsync(request8);

                                if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTranc = inventoryTransResponses1.StatusCode;
                            }

                            
                        }
                    }


                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && updateInvTranc == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTranc == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                    {
                        _iQCConfirmationRepository.SaveAsync();
                        _repository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create IQCConfirmationWithitems action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmationItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (serverKey == "avision")
                    {
                        var grinNum = iqcConfirmation.GrinNumber;
                        if (grinNum != null)
                        {
                            var iqcNum = grinNum.Replace("GRN", "IQC");
                            iqcConfirmation.IQCNumber = iqcNum;
                        }

                    }
                    var grinPartId = iqcConfirmationItemsDto.GrinPartId;
                    var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartId);
                    if (iqcConfirmationItemsDto.GrinPartId != grinPartsDetails.Id)
                    {
                        _logger.LogError($"Invalid Grin Part Id {grinPartId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid Grin Part Id {grinPartId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    //Updating IQC Status in IqcItem
                    iqcConfirmationItems.IsIqcCompleted = true;
                    iqcConfirmationItems.IQCConfirmationId = iqcConfirmation.Id;
                    iqcConfirmation.IQCConfirmationItems = new List<IQCConfirmationItems> { iqcConfirmationItems };
                    await _iQCConfirmationRepository.CreateIqc(iqcConfirmation);
                    _iQCConfirmationRepository.SaveAsync();

                    ////update accepted qty ,rejected qty and IQC Status in grin model

                    var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinPartsQty(iqcConfirmationItems.GrinPartId, iqcConfirmationItems.AcceptedQty.ToString(), iqcConfirmationItems.RejectedQty.ToString());
                    var grinParts = _mapper.Map<GrinParts>(updatedGrinPartsQty);
                    grinParts.IsIqcCompleted = true;
                    await _grinPartsRepository.UpdateGrinQty(grinParts);
                    _grinPartsRepository.SaveAsync();

                    //Updating IQC and Grin Main Level Status
                    var grinPartsCount = await _grinPartsRepository.GetGrinPartsCount(GrinId);
                    var iqcConfomationCount = await _iQCConfirmationItemsRepository.GetIQCConformationItemsCount(iqcConfirmation.Id);

                    if (grinPartsCount == iqcConfomationCount)
                    {
                        var iqcDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                        iqcDetails.IsIqcCompleted = true;
                        await _iQCConfirmationRepository.UpdateIqc(iqcDetails);

                        var grinDetails = await _repository.GetGrinByGrinNo(grinNumber);
                        grinDetails.IsIqcCompleted = true;
                        await _repository.UpdateGrin(grinDetails);

                    }

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = iqcConfirmationItemsDto.ItemNumber;
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                        getItemmResp = itemMasterObjectResult.StatusCode;

                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;

                    //Inventory Update Code
                    decimal? acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinPartId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.ProjectNumbers)
                    {
                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var grinNo = iqcConfirmation.GrinNumber;
                        var encodedgrinNo = Uri.EscapeDataString(grinNo);
                        var grinPartsIds = projectNo.GrinPartsId;
                        var projectNos = projectNo.ProjectNumber;
                        var encodedprojectNos = Uri.EscapeDataString(projectNos);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                        request1.Headers.Add("Authorization", token1);

                        var inventoryObjectResult = await client1.SendAsync(request1);
                        var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic inventoryObject = inventoryObjectData.data;
                        if (inventoryObject != null)
                        {
                            decimal balanceQty = inventoryObject.balance_Quantity;

                            if (inventoryObject.balance_Quantity <= acceptedQty && inventoryObject.balance_Quantity != 0)
                            {
                                inventoryObject.warehouse = "IQC";
                                inventoryObject.location = "IQC";
                                inventoryObject.referenceIDFrom = "GRIN";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceIDFrom = "GRIN";
                                    inventoryObject.isStockAvailable = false;
                                }
                                else
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceIDFrom = "GRIN";
                                    acceptedQty = 0;
                                }
                            }

                            var json = JsonConvert.SerializeObject(inventoryObject);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var client5 = _clientFactory.CreateClient();
                            var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                            "UpdateInventory?id=", inventoryObject.id))
                            {
                                Content = data
                            };
                            request5.Headers.Add("Authorization", token5);

                            var response = await client5.SendAsync(request5);
                            if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

                            IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                            iqcInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                            iqcInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                            iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrItemNumber;
                            iqcInventoryTranctionDto.Description = inventoryObject.description;
                            iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                            iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            iqcInventoryTranctionDto.Issued_DateTime = DateTime.Now;
                            iqcInventoryTranctionDto.Issued_By = _createdBy;
                            iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto.Warehouse = "IQC";
                            iqcInventoryTranctionDto.From_Location = "GRIN";
                            iqcInventoryTranctionDto.TO_Location = "IQC";
                            iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; 
                            iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;/* Convert.ToString(grinPartsDetails.Id);*/
                            iqcInventoryTranctionDto.ReferenceIDFrom = "IQC";
                            iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                            iqcInventoryTranctionDto.ShopOrderNo = "";
                            iqcInventoryTranctionDto.Remarks = "GRINIQC Done";
                            iqcInventoryTranctionDto.IsStockAvailable = inventoryObject.isStockAvailable;

                            string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                            var contents = new StringContent(rfqSourcingPPdetailsJsons, Encoding.UTF8, "application/json");
                            var client7 = _clientFactory.CreateClient();
                            var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranction"))
                            {
                                Content = contents
                            };
                            request7.Headers.Add("Authorization", token7);

                            var inventoryTransResponses = await client7.SendAsync(request7);
                            if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTranc1 = inventoryTransResponses.StatusCode;

                            if (iqcConfirmationItemsDto.RejectedQty != 0 && acceptedQty == 0)
                            {
                                IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                grinInventoryDto.PartNumber = iqcConfirmationItemsDto.ItemNumber;
                                grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                                grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                                grinInventoryDto.ProjectNumber = projectNos;
                                grinInventoryDto.Balance_Quantity = Convert.ToDecimal(iqcConfirmationItemsDto.RejectedQty);
                                grinInventoryDto.Max = itemMasterObject.max;
                                grinInventoryDto.Min = itemMasterObject.min;
                                grinInventoryDto.UOM = grinPartsDetails.UOM;
                                grinInventoryDto.Warehouse = "Reject";
                                grinInventoryDto.Location = "Reject";
                                grinInventoryDto.GrinNo = iqcConfirmation.GrinNumber;
                                grinInventoryDto.GrinPartId = iqcConfirmationItemsDto.GrinPartId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
                                grinInventoryDto.ReferenceID = iqcConfirmation.GrinNumber; // Convert.ToString(iQCConfirmationItems.Id) //;
                                grinInventoryDto.ReferenceIDFrom = "IQC";
                                grinInventoryDto.GrinMaterialType = "GRIN";
                                grinInventoryDto.ShopOrderNo = "";


                                string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                //var rfqCustomerIdResponse = await _httpClient.PostAsync($"{rfqApiUrl}CreateInventoryFromGrin", content);
                                var client6 = _clientFactory.CreateClient();
                                var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                "CreateInventoryFromGrin"))
                                {
                                    Content = content
                                };
                                request6.Headers.Add("Authorization", token6);

                                var responses = await client6.SendAsync(request6);
                                if (responses.StatusCode != HttpStatusCode.OK) createInv = responses.StatusCode;


                                //InventoryTranction Update Code

                                IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                iqcInventoryTranctionDtos.Issued_DateTime = DateTime.Now;
                                iqcInventoryTranctionDtos.Issued_By = _createdBy;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = "GRIN";
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo; 
                                iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.GrinNo;/* Convert.ToString(grinPartsDetails.Id);*/
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "IQC";
                                iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                iqcInventoryTranctionDtos.ShopOrderNo = "";
                                iqcInventoryTranctionDtos.Remarks = "GRINIQC Done";

                                string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                                var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                                var client8 = _clientFactory.CreateClient();
                                var token8 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = contents1
                                };
                                request8.Headers.Add("Authorization", token8);

                                var inventoryTransResponses1 = await client8.SendAsync(request8);

                                if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTranc = inventoryTransResponses1.StatusCode;
                            }

                           
                        }

                    }

                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && updateInvTranc == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTranc == HttpStatusCode.OK && createInvTranc1 == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                    {
                        _iQCConfirmationRepository.SaveAsync();
                        _repository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create IQCConfirmationWithitems action: Inv and InvTrans Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmation and IQCConfirmationItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateIQCConfirmationItems action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateGrinFileUpload([FromBody] List<DocumentUploadPostDto> fileUploadPostDtos)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (fileUploadPostDtos is null)
                {
                    _logger.LogError("GrinFile object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GrinFile object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid GrinFile object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                List<string>? id_s = new List<string>();
                var FileUploadDetails = fileUploadPostDtos;
                foreach (var FileUploadDetail in FileUploadDetails)
                {
                    Guid guids = Guid.NewGuid();
                    byte[] fileContent = Convert.FromBase64String(FileUploadDetail.FileByte);
                    string fileName = guids.ToString() + "_" + FileUploadDetail.FileName + "." + FileUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinDocument", fileName);
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
                            ParentId = "GrinFile",
                            DocumentFrom = "GrinFile Document",
                            FileByte = FileUploadDetail.FileByte
                        };
                        await _documentUploadRepository.CreateUploadDocumentGrin(uploadedFile);
                        _documentUploadRepository.SaveAsync();
                        id_s.Add(uploadedFile.Id.ToString());

                    }
                }
                serviceResponse.Data = id_s;
                serviceResponse.Message = " GrinFile Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GrinFile action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetailsforGrinFiles(string fileids)
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
                    serviceResponse.Message = "Invalid Grin UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Grin UploadDocument sent from client.");
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
                            var baseUrl = $"{_config["GrinUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/Grin/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        else
                        {
                            var baseUrl = $"{_config["GrinUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/api/Grin/DownloadFile?Filename={fileUploadDto.FileName}";
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
                _logger.LogError($"Something went wrong inside Grin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrin(int id, [FromBody] GrinUpdateDto grinDto)
        {
            ServiceResponse<GrinDto> serviceResponse = new ServiceResponse<GrinDto>();

            try
            {
                if (grinDto is null)
                {
                    _logger.LogError("Update Grin object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Grin object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update Grin object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Grin object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updategrin = await _repository.GetGrinById(id);
                if (updategrin is null)
                {
                    _logger.LogError($"Update Grin with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Grin with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var grinparts = _mapper.Map<IEnumerable<GrinParts>>(updategrin.GrinParts);

                var grinList = _mapper.Map<Grins>(grinDto);

                var grinPartsDto = updategrin.GrinParts;
                var grinCal = _mapper.Map<List<GrinPartscalculationofAvgcost>>(grinPartsDto);
                var GrinpartsList = new List<GrinParts>();
                var othercosttotal = grinList.Freight + grinList.Insurance + grinList.LoadingorUnLoading + grinList.Transport;
                decimal? conversionrate = 1;
                if (grinList.CurrencyConversion > 1) conversionrate = grinList.CurrencyConversion;
                foreach (var gPart in grinCal)
                {
                    decimal? EP = gPart.Qty * gPart.UnitPrice;
                    decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
                    gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
                    gPart.EPforSingleQty = gPart.EPwithTax / gPart.Qty;
                }
                decimal? SumofEPwithtax = grinCal.Sum(x => x.EPwithTax);
                foreach (var gPart in grinCal)
                {
                    decimal? distriduteOthercostforitem = (gPart.EPwithTax / SumofEPwithtax) * othercosttotal;
                    decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / gPart.Qty;
                    gPart.AverageCost = distriduteOthercostforitemsSingleQty + gPart.EPforSingleQty;
                    GrinParts grinParts = _mapper.Map<GrinParts>(gPart);
                    grinParts.ProjectNumbers = _mapper.Map<List<ProjectNumbers>>(gPart.ProjectNumbers);
                    GrinpartsList.Add(grinParts);
                }
                var data = _mapper.Map(grinDto, updategrin);
                data.GrinParts = GrinpartsList;

                string result = await _repository.UpdateGrin(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Grin Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGrinSPReportWithParam([FromBody] GrinReportWithParamDto grinReportWithParam)
        {
            ServiceResponse<IEnumerable<Grin_ReportSP>> serviceResponse = new ServiceResponse<IEnumerable<Grin_ReportSP>>();
            try
            {
                var products = await _repository.GetGrinSPReportWithParam(grinReportWithParam.GrinNumber, grinReportWithParam.VendorName,
                                                                            grinReportWithParam.PONumber, grinReportWithParam.KPN,
                                                                            grinReportWithParam.MPN, grinReportWithParam.Warehouse,
                                                                            grinReportWithParam.Location);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetGrinSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGrinSPReportWithParamForTrans([FromBody] GrinReportWithParamForTransDto grinReportWithParam)
        {
            ServiceResponse<IEnumerable<GrinSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<GrinSPReportForTrans>>();
            try
            {
                var products = await _repository.GetGrinSPReportWithParamForTrans(grinReportWithParam.GrinNumber, grinReportWithParam.VendorName,
                                                                            grinReportWithParam.PONumber, grinReportWithParam.ItemNumber,
                                                                            grinReportWithParam.MPN, grinReportWithParam.Warehouse,
                                                                            grinReportWithParam.Location, grinReportWithParam.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetGrinSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetGrinSPReport([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<Grin_ReportSP>> serviceResponse = new ServiceResponse<IEnumerable<Grin_ReportSP>>();
            try
            {
                var products = await _repository.GetGrinSPReport(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all GetGrinSPReport");

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside Grin action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGrinSPReportForTrans([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<GrinSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<GrinSPReportForTrans>>();
            try
            {
                var products = await _repository.GetGrinSPReportForTrans(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all GetGrinSPReport");

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GrinSPReportForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetGrinSPReportForTrans action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetGrinSPReportWithDateForTrans([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<GrinSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<GrinSPReportForTrans>>();
            try
            {
                var products = await _repository.GetGrinSPReportWithDateForTrans(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GrinSPReportWithDateForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetGrinSPReportWithDateForTrans action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetGrinSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<Grin_ReportSP>> serviceResponse = new ServiceResponse<IEnumerable<Grin_ReportSP>>();
            try
            {
                var products = await _repository.GetGrinSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Grin hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Grin hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetGrinSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //get all grin number based bining is completed

        [HttpGet]
        public async Task<IActionResult> GetAllGrinNumberWhereBinningComplete()
        {
            ServiceResponse<IEnumerable<GrinNoForIqcAndBinning>> serviceResponse = new ServiceResponse<IEnumerable<GrinNoForIqcAndBinning>>();
            try
            {
                var getAllGrinNumberWhereBinningComplete = await _repository.GetAllGrinNumberWhereBinningComplete();
                var result = _mapper.Map<IEnumerable<GrinNoForIqcAndBinning>>(getAllGrinNumberWhereBinningComplete);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinNumberWhereBinningComplete";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllGrinNumberWhereBinningComplete action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<GrinController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrin(int id)
        {
            ServiceResponse<GrinDto> serviceResponse = new ServiceResponse<GrinDto>();

            try
            {
                var Deletegrin = await _repository.GetGrinById(id);
                if (Deletegrin == null)
                {
                    _logger.LogError($"Delete grin with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete grin with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteGrin(Deletegrin);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Grin Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadFile(string Filename)
        {
            ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinDocument", Filename);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var ContentType))
            {
                ContentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(bytes, ContentType, Path.GetFileName(filePath));
        }

        [HttpGet]
        public async Task<IActionResult> GetGrinDownloadUrlDetails(string grinNumber)
        {
            ServiceResponse<IEnumerable<GetDownloadUrlDto>> serviceResponse = new ServiceResponse<IEnumerable<GetDownloadUrlDto>>();

            try
            {
                var getDownloadDetailByGrinNumber = await _documentUploadRepository.GetGrinDownloadUrlDetails(grinNumber);


                foreach (var getDownloadUrlByFilename in getDownloadDetailByGrinNumber)
                {
                    getDownloadUrlByFilename.DownloadUrl = Url.Action("DownloadFile", "Grin", new { Filename = getDownloadUrlByFilename.FileName }, protocol: HttpContext.Request.Scheme);
                }
                if (getDownloadDetailByGrinNumber == null)
                {
                    _logger.LogError($"DownloadDetail with id: {grinNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DownloadDetail with id: {grinNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned DownloadDetail with id: {grinNumber}");
                    var result = _mapper.Map<IEnumerable<GetDownloadUrlDto>>(getDownloadDetailByGrinNumber);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SalesDetail action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //get parts download list


        [HttpGet]
        public async Task<IActionResult> GetGrinPartsDownloadUrlDetails(string grinNumber)
        {
            ServiceResponse<IEnumerable<GetDownloadUrlDto>> serviceResponse = new ServiceResponse<IEnumerable<GetDownloadUrlDto>>();

            try
            {
                var getDownloadDetailByGrinNumber = await _documentUploadRepository.GetGrinPartsDownloadUrlDetails(grinNumber);


                foreach (var getDownloadUrlByFilename in getDownloadDetailByGrinNumber)
                {
                    getDownloadUrlByFilename.DownloadUrl = Url.Action("DownloadFile", "Grin", new { Filename = getDownloadUrlByFilename.FileName }, protocol: HttpContext.Request.Scheme);
                }
                if (getDownloadDetailByGrinNumber == null)
                {
                    _logger.LogError($"DownloadDetail with id: {grinNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DownloadDetail with id: {grinNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned DownloadDetail with id: {grinNumber}");
                    var result = _mapper.Map<IEnumerable<GetDownloadUrlDto>>(getDownloadDetailByGrinNumber);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SalesDetail action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //grin update upload document
        [HttpPost]
        public async Task<IActionResult> UpdateGrinUploadDocument([FromBody] List<DocumentUploadPostDto> uploadDocumentDto, string grinNumber)
        {
            ServiceResponse<DocumentUploadPostDto> serviceResponse = new ServiceResponse<DocumentUploadPostDto>();
            try
            {
                if (uploadDocumentDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Grin UploadDocument object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Grin UploadDocument sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Grin UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Grin UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }


                foreach (var uploadDoc in uploadDocumentDto)
                {
                    var fileContent = uploadDoc.FileByte;
                    string fileName = uploadDoc.FileName + "." + uploadDoc.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    Guid guid = Guid.NewGuid();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinDocument", guid.ToString() + "_" + fileName);
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(fileContent)))
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
                            ParentId = grinNumber,
                            DocumentFrom = "GrinDocument",
                        };
                        var grinUploadDoc = _mapper.Map<DocumentUpload>(uploadedFile);

                        await _documentUploadRepository.CreateUploadDocumentGrin(grinUploadDoc);
                        _documentUploadRepository.SaveAsync();
                    }
                }

                serviceResponse.Data = null;
                serviceResponse.Message = " GrinUploadDocument Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateGrinUploadDocument action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //grin item update cocupload document
        [HttpPost]
        public async Task<IActionResult> UpdateGrinPartsUploadDocument([FromBody] List<DocumentUploadPostDto> uploadDocumentDto, string grinNumber)
        {
            ServiceResponse<DocumentUploadPostDto> serviceResponse = new ServiceResponse<DocumentUploadPostDto>();
            try
            {
                if (uploadDocumentDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Grin COCUploadDocument object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Grin COCUploadDocument sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Grin COCUploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Grin COCUploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }


                foreach (var cocUpload in uploadDocumentDto)
                {
                    var fileContent = cocUpload.FileByte;
                    string fileName = cocUpload.FileName + "." + cocUpload.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    Guid guid = Guid.NewGuid();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinCoCUpload", guid.ToString() + "_" + fileName);
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(fileContent)))
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
                            ParentId = grinNumber + "-" + "I",
                            DocumentFrom = "GrinCoCDocument",
                        };
                        var grinUploadDoc = _mapper.Map<DocumentUpload>(uploadedFile);

                        await _documentUploadRepository.CreateUploadDocumentGrin(grinUploadDoc);
                        _documentUploadRepository.SaveAsync();
                    }
                }

                serviceResponse.Data = null;
                serviceResponse.Message = " GrinCOCUploadDocument Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateGrinPartsUploadDocument action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGrinCoCUploadDocument(int id)
        {
            ServiceResponse<IEnumerable<DocumentUploadDto>> serviceResponse = new ServiceResponse<IEnumerable<DocumentUploadDto>>();

            try
            {
                var documentUploadDetails = await _documentUploadRepository.GetUploadDocById(id);
                var fileName = documentUploadDetails.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinCoCUpload", /*guid.ToString() + "_" */ fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    string result = await _documentUploadRepository.DeleteUploadFile(documentUploadDetails);
                    _logger.LogInfo(result);
                    _documentUploadRepository.SaveAsync();

                    serviceResponse.Data = null;
                    serviceResponse.Message = " GrinCoCUploadDocument Deleted Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Given GrinCoCUploadDocument file is doesn't exist");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Given GrinCoCUploadDocument file is doesn't exist";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteGrinCoCUploadDocument action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGrinUploadDocument(int id)
        {
            ServiceResponse<IEnumerable<DocumentUploadDto>> serviceResponse = new ServiceResponse<IEnumerable<DocumentUploadDto>>();

            try
            {
                var documentUploadDetails = await _documentUploadRepository.GetUploadDocById(id);
                var fileName = documentUploadDetails.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinDocument", /*guid.ToString() + "_" */ fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    string result = await _documentUploadRepository.DeleteUploadFile(documentUploadDetails);
                    _logger.LogInfo(result);
                    _documentUploadRepository.SaveAsync();

                    serviceResponse.Data = null;
                    serviceResponse.Message = " GrinUploadDocument Deleted Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Given GrinUploadDocument file is doesn't exist");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Given GrinUploadDocument file is doesn't exist";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteGrinUploadDocument action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrinParts(int id, [FromBody] GrinPartsUpdateDto grinPartsUpdateDto)
        {
            ServiceResponse<GrinPartsDto> serviceResponse = new ServiceResponse<GrinPartsDto>();

            try
            {
                if (grinPartsUpdateDto is null)
                {
                    _logger.LogError("Update GrinParts object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update GrinParts object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update GrinParts object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update GrinParts object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateGrinParts = await _grinPartsRepository.GetGrinPartsById(id);
                if (updateGrinParts is null)
                {
                    _logger.LogError($"Update GrinParts with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update GrinParts with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var projectNumbers = _mapper.Map<IEnumerable<ProjectNumbers>>(grinPartsUpdateDto.ProjectNumbers);

                var grinList = _mapper.Map<GrinParts>(grinPartsUpdateDto);

                var grinPartsDto = grinPartsUpdateDto.ProjectNumbers;

                var GrinpartsList = new List<ProjectNumbers>();
                for (int i = 0; i < grinPartsDto.Count; i++)
                {
                    ProjectNumbers ProjectNumbers = _mapper.Map<ProjectNumbers>(grinPartsDto[i]);

                    GrinpartsList.Add(ProjectNumbers);
                }

                var data = _mapper.Map(grinPartsUpdateDto, updateGrinParts);
                data.ProjectNumbers = projectNumbers.ToList();

                string result = await _grinPartsRepository.UpdateGrinQty(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "GrinParts Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateGrin action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrinParts(int id)
        {
            ServiceResponse<GrinPartsDto> serviceResponse = new ServiceResponse<GrinPartsDto>();

            try
            {
                var grinPartsById = await _grinPartsRepository.DeleteGrinPartsById(id);
                if (grinPartsById == null)
                {
                    _logger.LogError($"Delete GrinParts with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete GrinParts with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var grinid = grinPartsById.GrinsId;
                string result = await _grinPartsRepository.DeleteGrinParts(grinPartsById);
                _logger.LogInfo(result);
                _grinPartsRepository.SaveAsync();

                var grindetails = await _repository.GetGrinById(grinid);
                var grinnumber = grindetails.GrinNumber;
                var grinnumbers = grinnumber + "-" + "I";
                var documentDetails = await _documentUploadRepository.GetDocumentDetailsByGrinNo(grinnumber);
                for (int i = 0; i < documentDetails; i++)
                {

                    var uploaddocuments = await _documentUploadRepository.DeleteGrinPartsUploadDocByGrinNo(grinnumbers);
                    _documentUploadRepository.SaveAsync();
                }



                serviceResponse.Data = null;
                serviceResponse.Message = "GrinParts Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteGrinParts action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGrinNoList()
        {
            ServiceResponse<IEnumerable<GrinNumberListDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinNumberListDto>>();
            try
            {
                var AllActiveGrinNo = await _repository.GetAllActiveGrinNoList();
                var result = _mapper.Map<IEnumerable<GrinNumberListDto>>(AllActiveGrinNo);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinNoList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveGrinNoList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGrinNumberForIqc()
        {
            ServiceResponse<IEnumerable<GrinNoForIqcAndBinning>> serviceResponse = new ServiceResponse<IEnumerable<GrinNoForIqcAndBinning>>();
            try
            {
                var grinNoForIqc = await _repository.GetAllGrinNumberForIqc();
                var result = _mapper.Map<IEnumerable<GrinNoForIqcAndBinning>>(grinNoForIqc);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinNumberForIqc";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllGrinNumberForIqc action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGrinNumberForBinning()
        {
            ServiceResponse<IEnumerable<GrinNoForIqcAndBinning>> serviceResponse = new ServiceResponse<IEnumerable<GrinNoForIqcAndBinning>>();
            try
            {
                var grinNoForBinning = await _repository.GetAllGrinNumberForBinning();
                var result = _mapper.Map<IEnumerable<GrinNoForIqcAndBinning>>(grinNoForBinning);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinNumberForBinning";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllGrinNumberForBinning action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]

        public async Task<IActionResult> GetAllGrinParts([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)

        {
            ServiceResponse<IEnumerable<GrinPartsDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinPartsDto>>();

            try
            {
                var GetallGrinsParts = await _grinPartsRepository.GetAllGrinParts(pagingParameter, searchParams);

                var metadata = new
                {
                    GetallGrinsParts.TotalCount,
                    GetallGrinsParts.PageSize,
                    GetallGrinsParts.CurrentPage,
                    GetallGrinsParts.HasNext,
                    GetallGrinsParts.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all GrinsParts");
                var result = _mapper.Map<IEnumerable<GrinPartsDto>>(GetallGrinsParts);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinsParts Successfully";
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
        [HttpPut]
        public async Task<IActionResult> UpdateGrinTallyStatus(int Id, bool TallyStatus)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                var getGrindetails = await _repository.GetGrinById(Id);
                getGrindetails.TallyStatus = TallyStatus;
                await _repository.UpdateGrin_ForTally(getGrindetails);
                _repository.SaveAsync();
                _logger.LogInfo($"Successfully Updated the TallyStatus of Grin Id {Id} and is set to {TallyStatus} from the UpdateGrinTallyStatus API");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Successfully Updated the TallyStatus of Grin Id {Id} and is set to {TallyStatus}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(200, serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside UpdateGrinTallyStatus action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
