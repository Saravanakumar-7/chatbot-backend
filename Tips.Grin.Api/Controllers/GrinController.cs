using AutoMapper;
using Contracts;
using Entities;
using Entities.Enums;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using MySqlX.XDevAPI;
using Newtonsoft.Json.Linq;
using Mysqlx.Session;
using Google.Protobuf.WellKnownTypes;
using Mysqlx;
using System.Text;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using EmailTemplateDto = Tips.Grin.Api.Entities.DTOs.EmailTemplateDto;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using static NPOI.HSSF.UserModel.HeaderFooter;


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
                    serviceResponse.Message = $"GetGrinAndIqcsByPurchaseOrder:{Ponumber} sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"GetGrinAndIqcsByPurchaseOrder:{Ponumber} sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Invalid Ponumber :{Ponumber} sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Invalid Ponumber :{Ponumber} sent from client.");
                    return BadRequest(serviceResponse);
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
                    grins.GrinParts.ForEach(x => x.Grins = null);
                    grins.GrinParts.ForEach(x => x.ProjectNumbers.ForEach(z => z.GrinParts = null));
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
                    IqcDetails.ForEach(x => x.IQCConfirmationItems.ForEach(z => z.IQCConfirmation = null));
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
                _logger.LogError($"Error Occured in GetGrinAndIqcsByPurchaseOrder API for the following PoNo:{Ponumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinAndIqcsByPurchaseOrder API for the following PoNo:{Ponumber}:\n {ex.Message}";
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
                    serviceResponse.Message = $"Grin data not found.";
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


                _logger.LogInfo("Returned all Grin Detials");
                var result = _mapper.Map<IEnumerable<GrinDto>>(GetallGrins);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Grins Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllGrin API:{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllGrin API:{ex.Message} \n{ex.InnerException}";
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
                                POOrderQty = grinParts.POOrderQty ?? 0,
                                POBalancedQty = grinParts.POBalancedQty ?? 0,
                                POUnitPrice = grinParts.POUnitPrice ?? 0,
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
                _logger.LogError($"Error Occured in SearchGrinsDate API:{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchGrinsDate API:{ex.Message}";
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
                                POOrderQty = grinParts.POOrderQty ?? 0,
                                POBalancedQty = grinParts.POBalancedQty ?? 0,
                                POUnitPrice = grinParts.POUnitPrice ?? 0,
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
                                POOrderQty = grinParts.POOrderQty ?? 0,
                                POBalancedQty = grinParts.POBalancedQty ?? 0,
                                POUnitPrice = grinParts.POUnitPrice ?? 0,
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
                _logger.LogError($"Error Occured in GetAllGrinsWithItems API:{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllGrinsWithItems API:{ex.Message}";
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
                    serviceResponse.Message = $"Grin with id:{id}, hasn't been found.";
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
                        var encodedgrinNo = Uri.EscapeDataString(GrinDetailsbyId.GrinNumber);
                        GrinPartsItemMasterEnggDto grinPartsItemMasterEnggDto = _mapper.Map<GrinPartsItemMasterEnggDto>(GrinpartsDetails);
                        grinPartsItemMasterEnggDto.ProjectNumbers = _mapper.Map<List<ProjectNumbersDto>>(GrinpartsDetails.ProjectNumbers);
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();
                        var ItemNumber = grinPartsItemMasterEnggDto.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                   $"GetIQCItemInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={GrinpartsDetails.Id}&ItemNumber={encodedItemNumber}"));
                        request1.Headers.Add("Authorization", token);

                        var inventoryObjectResult = await client.SendAsync(request1);
                        var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                        var inventoryObjectData = JsonConvert.DeserializeObject<InventoryDto>(inventoryObjectString);
                        var inventoryObject = inventoryObjectData.data;

                        grinPartsItemMasterEnggDto.ProjectNumbers.ForEach(x => x.RemainingAccptedQty = (inventoryObject.Where(y => y.ProjectNumber == x.ProjectNumber).Sum(z => z.Balance_Quantity)));

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterDetails = await client.SendAsync(request);
                        var ItemmasterObjectString = await itemMasterDetails.Content.ReadAsStringAsync();
                        dynamic ItemmasterObjectData = JsonConvert.DeserializeObject(ItemmasterObjectString);
                        dynamic ItemmasterObject = ItemmasterObjectData.data;
                        grinPartsItemMasterEnggDto.DrawingNo = ItemmasterObject.drawingNo;
                        grinPartsItemMasterEnggDto.DocRet = ItemmasterObject.docRet;
                        grinPartsItemMasterEnggDto.RevNo = ItemmasterObject.revNo;
                        grinPartsItemMasterEnggDto.IsCocRequired = ItemmasterObject.isCocRequired;
                        grinPartsItemMasterEnggDto.IsRohsItem = ItemmasterObject.isRohsItem;
                        grinPartsItemMasterEnggDto.IsShelfLife = ItemmasterObject.isShelfLife;
                        grinPartsItemMasterEnggDto.IsReachItem = ItemmasterObject.isReachItem;

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
                _logger.LogError($"Error Occured in GetGrinById API for the following Id :{id}:\n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinById API for the following Id:{id}:\n {ex.Message}";
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

                decimal? othercosttotal = 1;
                if ((grins.Freight + grins.Insurance + grins.LoadingorUnLoading + grins.Transport) > 1) othercosttotal = grins.Freight + grins.Insurance + grins.LoadingorUnLoading + grins.Transport;
                decimal? conversionrate = 1;
                if (grins.CurrencyConversion > 1) conversionrate = grins.CurrencyConversion;
                foreach (var gPart in grinCal)
                {
                    decimal? EP = gPart.Qty * gPart.UnitPrice;
                    //decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
                    //*if (Itemwithtax == null || Itemwithtax == 0)*/ gPart.EPwithTax = EP * conversionrate;
                    //else gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
                    gPart.EPwithTax = EP * conversionrate;
                    gPart.EPforSingleQty = gPart.EPwithTax / gPart.Qty;
                }
                decimal? SumofEPwithtax = grinCal.Sum(x => x.EPwithTax);
                foreach (var gPart in grinCal)
                {
                    decimal? distriduteOthercostforitem = (gPart.EPwithTax / SumofEPwithtax) * othercosttotal;
                    decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / gPart.Qty;
                    gPart.AverageCost = distriduteOthercostforitemsSingleQty + gPart.EPforSingleQty;
                    if (gPart.AverageCost == null) gPart.AverageCost = 0;
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
                        if (serverKey != "Ware")
                        {
                            grinPartsId.LotNumber = grins.GrinNumber + grinPartsId.Id;
                        }
                        else
                        {
                            grinPartsId.LotNumber = grins.BondNumber;
                        }
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

                            grinInventoryTrasactionPostDto grinInventoryTranctionDto = new grinInventoryTrasactionPostDto();
                            grinInventoryTranctionDto.PartNumber = parts.ItemNumber;
                            grinInventoryTranctionDto.LotNumber = parts.LotNumber;
                            grinInventoryTranctionDto.MftrPartNumber = parts.MftrItemNumber;
                            grinInventoryTranctionDto.Description = parts.ItemDescription;
                            grinInventoryTranctionDto.ProjectNumber = project.ProjectNumber;
                            grinInventoryTranctionDto.Issued_Quantity = Convert.ToDecimal(project.ProjectQty);
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
                            grinInventoryTranctionDto.TransactionType = InventoryType.Inward;

                            var json11 = JsonConvert.SerializeObject(grinInventoryTranctionDto);
                            var data11 = new StringContent(json11, Encoding.UTF8, "application/json");
                            var client11 = _clientFactory.CreateClient();
                            var token11 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request11 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranctionFromGrin"))
                            {
                                Content = data11
                            };
                            request11.Headers.Add("Authorization", token11);

                            var response11 = await client11.SendAsync(request11);
                            if (response11.StatusCode != HttpStatusCode.OK)
                            {
                                createinvTrancResp = response11.StatusCode;
                            }
                        }
                    }
                }
                //
                if (serverKey != "Ware")
                {
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
                                grinPartsProjectNoDtoDetail.PoItemId = grinparts.PoItemId ?? 0;
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
                }
                //
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
                _logger.LogError($"Error Occured in CreateGrin API: \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateGrin API: {ex.Message}";
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
                                inventoryObject.referenceIDFrom = "IQC";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceIDFrom = "IQC";
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
                                    inventoryObject.referenceIDFrom = "IQC";
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
                            iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                            iqcInventoryTranctionDto.Description = inventoryObject.description;
                            iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                            iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto.Warehouse = "IQC";
                            iqcInventoryTranctionDto.From_Location = "GRIN";
                            iqcInventoryTranctionDto.TO_Location = "IQC";
                            iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                            iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                            iqcInventoryTranctionDto.ReferenceIDFrom = "IQC";
                            iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                            iqcInventoryTranctionDto.shopOrderNo = "";
                            iqcInventoryTranctionDto.Remarks = "GRINIQC Done";
                            iqcInventoryTranctionDto.IsStockAvailable = inventoryObject.isStockAvailable;
                            iqcInventoryTranctionDto.TransactionType = InventoryType.Inward;

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

                                IQCInventoryTranctionDto GRINInventoryTranctionDto = new IQCInventoryTranctionDto();
                                GRINInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                                GRINInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                                GRINInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                                GRINInventoryTranctionDto.Description = inventoryObject.description;
                                GRINInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                                GRINInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                GRINInventoryTranctionDto.UOM = inventoryObject.uom;
                                GRINInventoryTranctionDto.Warehouse = "GRIN";
                                GRINInventoryTranctionDto.From_Location = "GRIN";
                                GRINInventoryTranctionDto.TO_Location = "IQC";
                                GRINInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                                GRINInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                                GRINInventoryTranctionDto.PartType = inventoryObject.partType;
                                GRINInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                                GRINInventoryTranctionDto.ReferenceIDFrom = "GRIN";
                                GRINInventoryTranctionDto.GrinMaterialType = "GRIN";
                                GRINInventoryTranctionDto.shopOrderNo = "";
                                GRINInventoryTranctionDto.Remarks = "GRINIQC Done";
                                GRINInventoryTranctionDto.IsStockAvailable = false;
                                GRINInventoryTranctionDto.TransactionType = InventoryType.Outward;

                                string rfqSourcingPPdetailsJsons_1 = JsonConvert.SerializeObject(GRINInventoryTranctionDto);
                                var contents_1 = new StringContent(rfqSourcingPPdetailsJsons_1, Encoding.UTF8, "application/json");
                                var client7_1 = _clientFactory.CreateClient();
                                var token7_1 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request7_1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = contents_1
                                };
                                request7_1.Headers.Add("Authorization", token7_1);

                                var inventoryTransResponses_1 = await client7_1.SendAsync(request7_1);
                                if (inventoryTransResponses_1.StatusCode != HttpStatusCode.OK) createInvTranc = inventoryTransResponses_1.StatusCode;

                            if (iqcConfirmationItemsDto.RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                            {
                                IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                grinInventoryDto.PartNumber = iqcConfirmationItemsDto.ItemNumber;
                                grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                                grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                                grinInventoryDto.ProjectNumber = projectNos;
                                //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(iqcConfirmationItemsDto.RejectedQty);
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
                                iqcInventoryTranctionDtos.IsStockAvailable = true;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = "GRIN";
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "IQC";
                                iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                iqcInventoryTranctionDtos.shopOrderNo = "";
                                iqcInventoryTranctionDtos.Remarks = "GRINIQC Done";
                                iqcInventoryTranctionDtos.TransactionType = InventoryType.Inward;

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
                    _logger.LogInfo($"IQCConfirmationItems Created Successfully");
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
                                inventoryObject.referenceIDFrom = "IQC";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceIDFrom = "IQC";
                                    inventoryObject.isStockAvailable = false;
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
                                    inventoryObject.referenceIDFrom = "IQC";
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
                            iqcInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                            iqcInventoryTranctionDto.Description = inventoryObject.description;
                            iqcInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                            iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            iqcInventoryTranctionDto.UOM = inventoryObject.uom;
                            iqcInventoryTranctionDto.Warehouse = "IQC";
                            iqcInventoryTranctionDto.From_Location = "GRIN";
                            iqcInventoryTranctionDto.TO_Location = "IQC";
                            iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                            iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                            iqcInventoryTranctionDto.ReferenceIDFrom = "IQC";
                            iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                            iqcInventoryTranctionDto.shopOrderNo = "";
                            iqcInventoryTranctionDto.Remarks = "GRINIQC Done";
                            iqcInventoryTranctionDto.IsStockAvailable = inventoryObject.isStockAvailable;
                            iqcInventoryTranctionDto.TransactionType = InventoryType.Inward;

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


                            IQCInventoryTranctionDto GRINInventoryTranctionDto = new IQCInventoryTranctionDto();
                            GRINInventoryTranctionDto.PartNumber = inventoryObject.partNumber;
                            GRINInventoryTranctionDto.LotNumber = inventoryObject.lotNumber;
                            GRINInventoryTranctionDto.MftrPartNumber = inventoryObject.mftrPartNumber;
                            GRINInventoryTranctionDto.Description = inventoryObject.description;
                            GRINInventoryTranctionDto.ProjectNumber = inventoryObject.projectNumber;
                            GRINInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            GRINInventoryTranctionDto.UOM = inventoryObject.uom;
                            GRINInventoryTranctionDto.Warehouse = "GRIN";
                            GRINInventoryTranctionDto.From_Location = "GRIN";
                            GRINInventoryTranctionDto.TO_Location = "IQC";
                            GRINInventoryTranctionDto.GrinNo = inventoryObject.grinNo;
                            GRINInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            GRINInventoryTranctionDto.PartType = inventoryObject.partType;
                            GRINInventoryTranctionDto.ReferenceID = inventoryObject.grinNo;
                            GRINInventoryTranctionDto.ReferenceIDFrom = "GRIN";
                            GRINInventoryTranctionDto.GrinMaterialType = "GRIN";
                            GRINInventoryTranctionDto.shopOrderNo = "";
                            GRINInventoryTranctionDto.Remarks = "GRINIQC Done";
                            GRINInventoryTranctionDto.IsStockAvailable = false;
                            GRINInventoryTranctionDto.TransactionType = InventoryType.Outward;

                            string rfqSourcingPPdetailsJsons_1 = JsonConvert.SerializeObject(GRINInventoryTranctionDto);
                            var contents_1 = new StringContent(rfqSourcingPPdetailsJsons_1, Encoding.UTF8, "application/json");
                            var client7_1 = _clientFactory.CreateClient();
                            var token7_1 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7_1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranction"))
                            {
                                Content = contents_1
                            };
                            request7_1.Headers.Add("Authorization", token7_1);

                            var inventoryTransResponses_1 = await client7_1.SendAsync(request7_1);
                            if (inventoryTransResponses_1.StatusCode != HttpStatusCode.OK) createInvTranc = inventoryTransResponses_1.StatusCode;

                            if (iqcConfirmationItemsDto.RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                            {
                                IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                grinInventoryDto.PartNumber = iqcConfirmationItemsDto.ItemNumber;
                                grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                                grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                                grinInventoryDto.ProjectNumber = projectNos;
                                //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(iqcConfirmationItemsDto.RejectedQty);
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
                                iqcInventoryTranctionDtos.IsStockAvailable = true;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = "GRIN";
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.GrinNo;
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "IQC";
                                iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                iqcInventoryTranctionDtos.shopOrderNo = "";
                                iqcInventoryTranctionDtos.Remarks = "GRINIQC Done";
                                iqcInventoryTranctionDtos.TransactionType = InventoryType.Inward;

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
                    _logger.LogInfo($"IQCConfirmation and IQCConfirmationItems Created Successfully");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmation and IQCConfirmationItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateIQCConfirmationItems API: \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateIQCConfirmationItems API: \n{ex.Message}";
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
                _logger.LogInfo($"GrinFile Successfully Created");
                serviceResponse.Data = id_s;
                serviceResponse.Message = "CreateGrinFileUpload Successfull";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateGrinFileUpload API: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateGrinFileUpload API: {ex.Message}";
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
                            fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/Grin/DownloadFile?Id={fileUploadDto.Id}";
                        }
                        else
                        {
                            var baseUrl = $"{_config["GrinUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/api/Grin/DownloadFile?Id={fileUploadDto.Id}";
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
                _logger.LogError($"Error Occured in GetDownloadUrlDetailsforGrinFiles API: {ex.Message} ,\n {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetDownloadUrlDetailsforGrinFiles API: {ex.Message}";
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
            else if (serverConfiguration.GetValue<bool?>("Warehousing:EnableWare") == true)
            {
                return "Ware";
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
                decimal? othercosttotal = 1;
                if ((grinList.Freight + grinList.Insurance + grinList.LoadingorUnLoading + grinList.Transport) > 1) othercosttotal = grinList.Freight + grinList.Insurance + grinList.LoadingorUnLoading + grinList.Transport;
                decimal? conversionrate = 1;
                if (grinList.CurrencyConversion > 1) conversionrate = grinList.CurrencyConversion;
                foreach (var gPart in grinCal)
                {
                    decimal? EP = gPart.Qty * gPart.UnitPrice;
                    //decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
                    //if (Itemwithtax == null || Itemwithtax == 0) gPart.EPwithTax = EP * conversionrate;
                    //else gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
                    gPart.EPwithTax = EP * conversionrate;
                    gPart.EPforSingleQty = gPart.EPwithTax / gPart.Qty;
                }
                decimal? SumofEPwithtax = grinCal.Sum(x => x.EPwithTax);
                foreach (var gPart in grinCal)
                {
                    decimal? distriduteOthercostforitem = (gPart.EPwithTax / SumofEPwithtax) * othercosttotal;
                    decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / gPart.Qty;
                    gPart.AverageCost = distriduteOthercostforitemsSingleQty + gPart.EPforSingleQty;
                    if (gPart.AverageCost == null) gPart.AverageCost = 0;
                    GrinParts grinParts = _mapper.Map<GrinParts>(gPart);
                    grinParts.ProjectNumbers = _mapper.Map<List<ProjectNumbers>>(gPart.ProjectNumbers);
                    GrinpartsList.Add(grinParts);
                }
                var data = _mapper.Map(grinDto, updategrin);
                data.GrinParts = GrinpartsList;

                string result = await _repository.UpdateGrin(data);
                _logger.LogInfo($"Grin Updated Successfully:{result}");
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Grin Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateGrin API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateGrin API : \n{ex.Message}";
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
                    _logger.LogInfo($"Returned Grin Details:{products}");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinSPReportWithParam API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinSPReportWithParam API : \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> ExportGrinSPReportWithParamOrWithDateToExcel([FromBody] GrinReportWithParamAndDateForExcelDto grinReportWithParamAndDateForExcelDto)
        {

            ServiceResponse<Grin_ReportSP> serviceResponse = new ServiceResponse<Grin_ReportSP>();
            try
            {

                if (grinReportWithParamAndDateForExcelDto is null)
                {
                    _logger.LogError("grinReportWithParamAndDateForExcelDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "grinReportWithParamAndDateForExcelDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid GrinReportWithParamAndDateForExcelDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(grinReportWithParamAndDateForExcelDto.GrinNumber)
                               || !string.IsNullOrEmpty(grinReportWithParamAndDateForExcelDto.VendorName)
                               || !string.IsNullOrEmpty(grinReportWithParamAndDateForExcelDto.PONumber)
                               || !string.IsNullOrEmpty(grinReportWithParamAndDateForExcelDto.KPN)
                               || !string.IsNullOrEmpty(grinReportWithParamAndDateForExcelDto.MPN)
                               || !string.IsNullOrEmpty(grinReportWithParamAndDateForExcelDto.Warehouse)
                               || !string.IsNullOrEmpty(grinReportWithParamAndDateForExcelDto.Location);

                bool hasDate = grinReportWithParamAndDateForExcelDto.FromDate != null || grinReportWithParamAndDateForExcelDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (grinReportWithParamAndDateForExcelDto.FromDate == null || grinReportWithParamAndDateForExcelDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var GrinSPReportDetails = Enumerable.Empty<Grin_ReportSP>();

                if (hasParams && !hasDate)
                {
                
                    GrinSPReportDetails = await _repository.GetGrinSPReportWithParam(
                        grinReportWithParamAndDateForExcelDto.GrinNumber,
                        grinReportWithParamAndDateForExcelDto.VendorName,
                        grinReportWithParamAndDateForExcelDto.PONumber,
                        grinReportWithParamAndDateForExcelDto.KPN,
                        grinReportWithParamAndDateForExcelDto.MPN,
                        grinReportWithParamAndDateForExcelDto.Warehouse,
                        grinReportWithParamAndDateForExcelDto.Location);
                }

             
                if (!hasParams && (grinReportWithParamAndDateForExcelDto.FromDate != null && grinReportWithParamAndDateForExcelDto.ToDate != null))
                {
                
                    GrinSPReportDetails = await _repository.GetGrinSPReportWithDate(
                        grinReportWithParamAndDateForExcelDto.FromDate,
                        grinReportWithParamAndDateForExcelDto.ToDate);
                }
            


                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("GrinSPReport");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("GRIN Number");
                headerRow.CreateCell(1).SetCellValue("Vendor Name");
                headerRow.CreateCell(2).SetCellValue("Vendor ID");
                headerRow.CreateCell(3).SetCellValue("Vendor Address");
                headerRow.CreateCell(4).SetCellValue("Invoice Number");
                headerRow.CreateCell(5).SetCellValue("Invoice Date");
                headerRow.CreateCell(6).SetCellValue("PO Number");
                headerRow.CreateCell(7).SetCellValue("KPN");
                headerRow.CreateCell(8).SetCellValue("MPN");
                headerRow.CreateCell(9).SetCellValue("Item Description");
                headerRow.CreateCell(10).SetCellValue("Manufacture Batch Number");
                headerRow.CreateCell(11).SetCellValue("Unit Price");
                headerRow.CreateCell(12).SetCellValue("Qty");
                headerRow.CreateCell(13).SetCellValue("Accepted Qty");
                headerRow.CreateCell(14).SetCellValue("UOM");
                headerRow.CreateCell(15).SetCellValue("SGST");
                headerRow.CreateCell(16).SetCellValue("CGST");
                headerRow.CreateCell(17).SetCellValue("IGST");
                headerRow.CreateCell(18).SetCellValue("UTGST");
                headerRow.CreateCell(19).SetCellValue("Total Value");
                headerRow.CreateCell(20).SetCellValue("Warehouse");
                headerRow.CreateCell(21).SetCellValue("Location");
                headerRow.CreateCell(22).SetCellValue("Remarks");
                headerRow.CreateCell(23).SetCellValue("GRIN Date");
                headerRow.CreateCell(24).SetCellValue("Project Number");
                headerRow.CreateCell(25).SetCellValue("UOC");
                headerRow.CreateCell(26).SetCellValue("Tally Voucher");
                headerRow.CreateCell(27).SetCellValue("Gate Entry Date");
                headerRow.CreateCell(28).SetCellValue("Gate Entry No");

                // Populate data
                int rowIndex = 1;
                foreach (var item in GrinSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.GrinNumber ?? "");
                    row.CreateCell(1).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(2).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(3).SetCellValue(item.VendorAddress ?? "");
                    row.CreateCell(4).SetCellValue(item.InvoiceNumber ?? "");
                    row.CreateCell(5).SetCellValue(item.InvoiceDate.HasValue ? item.InvoiceDate.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(6).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(7).SetCellValue(item.KPN ?? "");
                    row.CreateCell(8).SetCellValue(item.MPN ?? "");
                    row.CreateCell(9).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(10).SetCellValue(item.ManufactureBatchNumber ?? "");
                    row.CreateCell(11).SetCellValue(item.UnitPrice.HasValue ? Convert.ToDouble(item.UnitPrice.Value) : 0);
                    row.CreateCell(12).SetCellValue(item.Qty.HasValue ? Convert.ToDouble(item.Qty.Value) : 0);
                    row.CreateCell(13).SetCellValue(item.AcceptedQty.HasValue ? Convert.ToDouble(item.AcceptedQty.Value) : 0);
                    row.CreateCell(14).SetCellValue(item.UOM ?? "");
                    row.CreateCell(15).SetCellValue(item.SGST.HasValue ? Convert.ToDouble(item.SGST.Value) : 0);
                    row.CreateCell(16).SetCellValue(item.CGST.HasValue ? Convert.ToDouble(item.CGST.Value) : 0);
                    row.CreateCell(17).SetCellValue(item.IGST.HasValue ? Convert.ToDouble(item.IGST.Value) : 0);
                    row.CreateCell(18).SetCellValue(item.UTGST.HasValue ? Convert.ToDouble(item.UTGST.Value) : 0);
                    row.CreateCell(19).SetCellValue(item.totalvalue.HasValue ? Convert.ToDouble(item.totalvalue.Value) : 0);
                    row.CreateCell(20).SetCellValue(item.Warehouse ?? "");
                    row.CreateCell(21).SetCellValue(item.Location ?? "");
                    row.CreateCell(22).SetCellValue(item.Remarks ?? "");
                    row.CreateCell(23).SetCellValue(item.GrinDate.HasValue ? item.GrinDate.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(24).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(25).SetCellValue(item.UOC ?? "");
                    row.CreateCell(26).SetCellValue(item.TallyVoucher ?? "");
                    row.CreateCell(27).SetCellValue(item.GateEntryDate.HasValue ? item.GateEntryDate.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(28).SetCellValue(item.GateEntryNo ?? "");
                }

                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "GrinSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
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
                                                                            grinReportWithParam.MPN, grinReportWithParam.ProjectNumber);

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
                    _logger.LogInfo($"Returned Grin Details:{products}");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinSPReportWithParamForTrans API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinSPReportWithParamForTrans API : \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost]
        public async Task<IActionResult> ExportGrinSPReportWithParamOrWithDateToExcelForTras([FromBody] GrinReportWithParamWithdateForExcelDtoTras grinReportWithParamWithdateForExcelDtoTras)
        {

            ServiceResponse<GrinSPReportForTrans> serviceResponse = new ServiceResponse<GrinSPReportForTrans>();
            try
            {

                if (grinReportWithParamWithdateForExcelDtoTras is null)
                {
                    _logger.LogError("grinReportWithParamAndDateForExcelDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "grinReportWithParamAndDateForExcelDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid GrinReportWithParamAndDateForExcelDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelDtoTras.GrinNumber)
                               || !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelDtoTras.VendorName)
                               || !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelDtoTras.PONumber)
                               || !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelDtoTras.ItemNumber)
                               || !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelDtoTras.MPN)
                               || !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelDtoTras.ProjectNumber);

                bool hasDate = grinReportWithParamWithdateForExcelDtoTras.FromDate != null || grinReportWithParamWithdateForExcelDtoTras.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (grinReportWithParamWithdateForExcelDtoTras.FromDate == null || grinReportWithParamWithdateForExcelDtoTras.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var GrinSPReportDetails = Enumerable.Empty<GrinSPReportForTrans>();

                if (hasParams && !hasDate)
                {

                    GrinSPReportDetails = await _repository.GetGrinSPReportWithParamForTrans(
                        grinReportWithParamWithdateForExcelDtoTras.GrinNumber,
                        grinReportWithParamWithdateForExcelDtoTras.VendorName,
                        grinReportWithParamWithdateForExcelDtoTras.PONumber,
                        grinReportWithParamWithdateForExcelDtoTras.ItemNumber,
                        grinReportWithParamWithdateForExcelDtoTras.MPN,
                        grinReportWithParamWithdateForExcelDtoTras.ProjectNumber);
                }


                if (!hasParams && (grinReportWithParamWithdateForExcelDtoTras.FromDate != null && grinReportWithParamWithdateForExcelDtoTras.ToDate != null))
                {

                    GrinSPReportDetails = await _repository.GetGrinSPReportWithDateForTrans(
                        grinReportWithParamWithdateForExcelDtoTras.FromDate,
                        grinReportWithParamWithdateForExcelDtoTras.ToDate);
                }


                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("GrinSPReport");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("PO Number");
                headerRow.CreateCell(1).SetCellValue("PO Date");
                headerRow.CreateCell(2).SetCellValue("GRIN Number");
                headerRow.CreateCell(3).SetCellValue("GRIN Date");
                headerRow.CreateCell(4).SetCellValue("Gate Entry No");
                headerRow.CreateCell(5).SetCellValue("Gate Entry Date");
                headerRow.CreateCell(6).SetCellValue("Vendor Name");
                headerRow.CreateCell(7).SetCellValue("Vendor ID");
                headerRow.CreateCell(8).SetCellValue("Vendor Address");
                headerRow.CreateCell(9).SetCellValue("Invoice Number");
                headerRow.CreateCell(10).SetCellValue("Invoice Date");
                headerRow.CreateCell(11).SetCellValue("Project Number");
                headerRow.CreateCell(12).SetCellValue("Item Number");
                headerRow.CreateCell(13).SetCellValue("Item Description");
                headerRow.CreateCell(14).SetCellValue("MPN");
                headerRow.CreateCell(15).SetCellValue("Manufacture Batch Number");
                headerRow.CreateCell(16).SetCellValue("Unit Price");
                headerRow.CreateCell(17).SetCellValue("Qty");
                headerRow.CreateCell(18).SetCellValue("Project Accepted Qty");
                headerRow.CreateCell(19).SetCellValue("Lot Number");
                headerRow.CreateCell(20).SetCellValue("UOC");
                headerRow.CreateCell(21).SetCellValue("UOM");
                headerRow.CreateCell(22).SetCellValue("SGST");
                headerRow.CreateCell(23).SetCellValue("CGST");
                headerRow.CreateCell(24).SetCellValue("IGST");
                headerRow.CreateCell(25).SetCellValue("UTGST");
                headerRow.CreateCell(26).SetCellValue("Total Value");
                headerRow.CreateCell(27).SetCellValue("Remarks");
                headerRow.CreateCell(28).SetCellValue("Tally Status");
                headerRow.CreateCell(29).SetCellValue("BE Number");
                headerRow.CreateCell(30).SetCellValue("GRIN Accepted Qty");

                // Populate data
                int rowIndex = 1;
                foreach (var item in GrinSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(1).SetCellValue(item.PODate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(2).SetCellValue(item.GrinNumber ?? "");
                    row.CreateCell(3).SetCellValue(item.GrinDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.GateEntryNo ?? "");
                    row.CreateCell(5).SetCellValue(item.GateEntryDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(6).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(7).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(8).SetCellValue(item.VendorAddress ?? "");
                    row.CreateCell(9).SetCellValue(item.InvoiceNumber ?? "");
                    row.CreateCell(10).SetCellValue(item.InvoiceDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(11).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(12).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(13).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(14).SetCellValue(item.MPN ?? "");
                    row.CreateCell(15).SetCellValue(item.ManufactureBatchNumber ?? "");
                    row.CreateCell(16).SetCellValue((double)(item.UnitPrice ?? 0));
                    row.CreateCell(17).SetCellValue((double)(item.Qty ?? 0));
                    row.CreateCell(18).SetCellValue((double)(item.ProjectAcceptedQty ?? 0));
                    row.CreateCell(19).SetCellValue(item.LotNumber ?? "");
                    row.CreateCell(20).SetCellValue(item.UOC ?? "");
                    row.CreateCell(21).SetCellValue(item.UOM ?? "");
                    row.CreateCell(22).SetCellValue((double)(item.SGST ?? 0));
                    row.CreateCell(23).SetCellValue((double)(item.CGST ?? 0));
                    row.CreateCell(24).SetCellValue((double)(item.IGST ?? 0));
                    row.CreateCell(25).SetCellValue((double)(item.UTGST ?? 0));
                    row.CreateCell(26).SetCellValue((double)(item.totalvalue ?? 0));
                    row.CreateCell(27).SetCellValue(item.Remarks ?? "");
                    row.CreateCell(28).SetCellValue(item.TallyStatus.HasValue ? (item.TallyStatus.Value ? "True" : "False") : "");
                    row.CreateCell(29).SetCellValue(item.BENumber ?? "");
                    row.CreateCell(30).SetCellValue((double)(item.GrinAcceptedQty ?? 0));
                }

                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "GrinSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGrinSPReportWithParamForAvi([FromBody] GrinReportWithParamForAviDto grinReportWithParam)
        {
            ServiceResponse<IEnumerable<GrinSPReportForAvi>> serviceResponse = new ServiceResponse<IEnumerable<GrinSPReportForAvi>>();
            try
            {
                var products = await _repository.GetGrinSPReportWithParamForAvi(grinReportWithParam.GrinNumber, grinReportWithParam.VendorName,
                                                                            grinReportWithParam.PONumber, grinReportWithParam.ItemNumber,
                                                                            grinReportWithParam.MPN,grinReportWithParam.ProjectNumber);

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
                    _logger.LogInfo($"Returned Grin Details:{products}");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinSPReportWithParamForAvi API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinSPReportWithParamForAvi API : \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportGrinSPReportWithParamOrWithDateToExcelForAvi([FromBody] GrinReportWithParamWithdateForExcelAviDto grinReportWithParamWithdateForExcelAviDto)
        {

            ServiceResponse<GrinSPReportForAvi> serviceResponse = new ServiceResponse<GrinSPReportForAvi>();
            try
            {

                if (grinReportWithParamWithdateForExcelAviDto is null)
                {
                    _logger.LogError("grinReportWithParamAndDateForExcelDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "grinReportWithParamAndDateForExcelDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid GrinReportWithParamAndDateForExcelDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelAviDto.GrinNumber)
                               || !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelAviDto.VendorName)
                               || !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelAviDto.PONumber)
                               || !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelAviDto.ItemNumber)
                               || !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelAviDto.MPN)
                               || !string.IsNullOrEmpty(grinReportWithParamWithdateForExcelAviDto.ProjectNumber);

                bool hasDate = grinReportWithParamWithdateForExcelAviDto.FromDate != null || grinReportWithParamWithdateForExcelAviDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (grinReportWithParamWithdateForExcelAviDto.FromDate == null || grinReportWithParamWithdateForExcelAviDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var GrinSPReportDetails = Enumerable.Empty<GrinSPReportForAvi>();

                if (hasParams && !hasDate)
                {

                    GrinSPReportDetails = await _repository.GetGrinSPReportWithParamForAvi(
                        grinReportWithParamWithdateForExcelAviDto.GrinNumber,
                        grinReportWithParamWithdateForExcelAviDto.VendorName,
                        grinReportWithParamWithdateForExcelAviDto.PONumber,
                        grinReportWithParamWithdateForExcelAviDto.ItemNumber,
                        grinReportWithParamWithdateForExcelAviDto.MPN,
                       grinReportWithParamWithdateForExcelAviDto.ProjectNumber);
                }


                if (!hasParams && (grinReportWithParamWithdateForExcelAviDto.FromDate != null && grinReportWithParamWithdateForExcelAviDto.ToDate != null))
                {

                    GrinSPReportDetails = await _repository.GetGrinSPReportWithDateForAvi(
                        grinReportWithParamWithdateForExcelAviDto.FromDate,
                        grinReportWithParamWithdateForExcelAviDto.ToDate);
                }



                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("GrinSPReport");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("PO Number");
                headerRow.CreateCell(1).SetCellValue("PO Date");
                headerRow.CreateCell(2).SetCellValue("GRIN Number");
                headerRow.CreateCell(3).SetCellValue("GRIN Date");
                headerRow.CreateCell(4).SetCellValue("Gate Entry No");
                headerRow.CreateCell(5).SetCellValue("Gate Entry Date");
                headerRow.CreateCell(6).SetCellValue("Vendor Name");
                headerRow.CreateCell(7).SetCellValue("Vendor ID");
                headerRow.CreateCell(8).SetCellValue("Vendor Address");
                headerRow.CreateCell(9).SetCellValue("Invoice Number");
                headerRow.CreateCell(10).SetCellValue("Invoice Date");
                headerRow.CreateCell(11).SetCellValue("Project Number");
                headerRow.CreateCell(12).SetCellValue("Item Number");
                headerRow.CreateCell(13).SetCellValue("Item Description");
                headerRow.CreateCell(14).SetCellValue("MPN");
                headerRow.CreateCell(15).SetCellValue("Manufacture Batch Number");
                headerRow.CreateCell(16).SetCellValue("Lot Number");
                headerRow.CreateCell(17).SetCellValue("Unit Price");
                headerRow.CreateCell(18).SetCellValue("Qty");
                headerRow.CreateCell(19).SetCellValue("Accepted Qty");
                headerRow.CreateCell(20).SetCellValue("UOC");
                headerRow.CreateCell(21).SetCellValue("UOM");
                headerRow.CreateCell(22).SetCellValue("SGST");
                headerRow.CreateCell(23).SetCellValue("CGST");
                headerRow.CreateCell(24).SetCellValue("IGST");
                headerRow.CreateCell(25).SetCellValue("UTGST");
                headerRow.CreateCell(26).SetCellValue("Total Value");
                headerRow.CreateCell(27).SetCellValue("Remarks");
                headerRow.CreateCell(28).SetCellValue("Project Qty");
                headerRow.CreateCell(29).SetCellValue("Tally Status");
                headerRow.CreateCell(30).SetCellValue("BE Number");

                // Populate data
                int rowIndex = 1;
                foreach (var item in GrinSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(1).SetCellValue(item.PODate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(2).SetCellValue(item.GrinNumber ?? "");
                    row.CreateCell(3).SetCellValue(item.GrinDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.GateEntryNo ?? "");
                    row.CreateCell(5).SetCellValue(item.GateEntryDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(6).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(7).SetCellValue(item.VendorId ?? "");
                    row.CreateCell(8).SetCellValue(item.VendorAddress ?? "");
                    row.CreateCell(9).SetCellValue(item.InvoiceNumber ?? "");
                    row.CreateCell(10).SetCellValue(item.InvoiceDate?.ToString("MM/dd/yyyy") ?? "");
                    row.CreateCell(11).SetCellValue(item.ProjectNumber ?? "");
                    row.CreateCell(12).SetCellValue(item.ItemNumber ?? "");
                    row.CreateCell(13).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(14).SetCellValue(item.MPN ?? "");
                    row.CreateCell(15).SetCellValue(item.ManufactureBatchNumber ?? "");
                    row.CreateCell(16).SetCellValue(item.LotNumber ?? "");
                    row.CreateCell(17).SetCellValue((double)(item.UnitPrice ?? 0));
                    row.CreateCell(18).SetCellValue((double)(item.Qty ?? 0));
                    row.CreateCell(19).SetCellValue((double)(item.AcceptedQty ?? 0));
                    row.CreateCell(20).SetCellValue(item.UOC ?? "");
                    row.CreateCell(21).SetCellValue(item.UOM ?? "");
                    row.CreateCell(22).SetCellValue((double)(item.SGST ?? 0));
                    row.CreateCell(23).SetCellValue((double)(item.CGST ?? 0));
                    row.CreateCell(24).SetCellValue((double)(item.IGST ?? 0));
                    row.CreateCell(25).SetCellValue((double)(item.UTGST ?? 0));
                    row.CreateCell(26).SetCellValue((double)(item.totalvalue ?? 0));
                    row.CreateCell(27).SetCellValue(item.Remarks ?? "");
                    row.CreateCell(28).SetCellValue((double)(item.ProjectQty ?? 0));
                    row.CreateCell(29).SetCellValue(item.TallyStatus.HasValue ? (item.TallyStatus.Value ? "True" : "False") : "");
                    row.CreateCell(30).SetCellValue(item.BENumber ?? "");
                }


                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "GrinSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPoAndGrinUnitPriceSPReportWithParam([FromBody] PoAndGrinUnitPriceSPReportDto poAndGrinUnitPriceSPReportDto)
        {
            ServiceResponse<IEnumerable<PoAndGrinUnitPriceSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PoAndGrinUnitPriceSPReport>>();
            try
            {
                var products = await _repository.GetPoAndGrinUnitPriceSPReportWithParam(poAndGrinUnitPriceSPReportDto.GrinNumber, poAndGrinUnitPriceSPReportDto.VendorName,
                                                                            poAndGrinUnitPriceSPReportDto.PONumber, poAndGrinUnitPriceSPReportDto.ItemNumber,
                                                                            poAndGrinUnitPriceSPReportDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PoAndGrinUnitPrice hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PoAndGrinUnitPrice hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"PoAndGrinUnitPrice Details Returned Successfully");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PoAndGrinUnitPrice Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPoAndGrinUnitPriceSPReportWithParam API: \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPoAndGrinUnitPriceSPReportWithParam API: \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpPost]
        public async Task<IActionResult> ExportPoAndGrinUnitPriceSPReportWithParamOrDateForTransToExcel([FromBody] PoAndGrinUnitPriceSPReportWithParamAndDateDto poAndGrinUnitPriceSPReportWithParamAndDateDto)
        {

            ServiceResponse<PoAndGrinUnitPriceSPReport> serviceResponse = new ServiceResponse<PoAndGrinUnitPriceSPReport>();
            try
            {

                if (poAndGrinUnitPriceSPReportWithParamAndDateDto is null)
                {
                    _logger.LogError("poAndGrinUnitPriceSPReportWithParamAndDateDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "poAndGrinUnitPriceSPReportWithParamAndDateDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid poAndGrinUnitPriceSPReportWithParamAndDateDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(poAndGrinUnitPriceSPReportWithParamAndDateDto.GrinNumber)
                               || !string.IsNullOrEmpty(poAndGrinUnitPriceSPReportWithParamAndDateDto.VendorName)
                               || !string.IsNullOrEmpty(poAndGrinUnitPriceSPReportWithParamAndDateDto.PONumber)
                               || !string.IsNullOrEmpty(poAndGrinUnitPriceSPReportWithParamAndDateDto.ItemNumber)
                               || !string.IsNullOrEmpty(poAndGrinUnitPriceSPReportWithParamAndDateDto.ProjectNumber);

                bool hasDate = poAndGrinUnitPriceSPReportWithParamAndDateDto.FromDate != null || poAndGrinUnitPriceSPReportWithParamAndDateDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (poAndGrinUnitPriceSPReportWithParamAndDateDto.FromDate == null || poAndGrinUnitPriceSPReportWithParamAndDateDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var PoAndGrinUnitPriceList = Enumerable.Empty<PoAndGrinUnitPriceSPReport>();

                if (hasParams && !hasDate)
                {

                    PoAndGrinUnitPriceList = await _repository.GetPoAndGrinUnitPriceSPReportWithParam(
                        poAndGrinUnitPriceSPReportWithParamAndDateDto.GrinNumber,
                        poAndGrinUnitPriceSPReportWithParamAndDateDto.VendorName,
                        poAndGrinUnitPriceSPReportWithParamAndDateDto.PONumber,
                        poAndGrinUnitPriceSPReportWithParamAndDateDto.ItemNumber,
                        poAndGrinUnitPriceSPReportWithParamAndDateDto.ProjectNumber);
                }


                if (!hasParams && (poAndGrinUnitPriceSPReportWithParamAndDateDto.FromDate != null && poAndGrinUnitPriceSPReportWithParamAndDateDto.ToDate != null))
                {

                    PoAndGrinUnitPriceList = await _repository.GetPoAndGrinUnitPriceSPReportWithDate(
                        poAndGrinUnitPriceSPReportWithParamAndDateDto.FromDate,
                        poAndGrinUnitPriceSPReportWithParamAndDateDto.ToDate);
                }



                // Create Excel workbook
                // Create Excel workbook
                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("PoAndGrinUnitPriceReport");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("PO Number");
                headerRow.CreateCell(1).SetCellValue("Part Number");
                headerRow.CreateCell(2).SetCellValue("MPN");
                headerRow.CreateCell(3).SetCellValue("Description");
                headerRow.CreateCell(4).SetCellValue("PO Qty");
                headerRow.CreateCell(5).SetCellValue("PO UOM");
                headerRow.CreateCell(6).SetCellValue("PO Unit Price");
                headerRow.CreateCell(7).SetCellValue("PO UOC");
                headerRow.CreateCell(8).SetCellValue("GRIN Number");
                headerRow.CreateCell(9).SetCellValue("GRIN Qty");
                headerRow.CreateCell(10).SetCellValue("GRIN UOM");
                headerRow.CreateCell(11).SetCellValue("GRIN Unit Price");
                headerRow.CreateCell(12).SetCellValue("GRIN UOC");
                headerRow.CreateCell(13).SetCellValue("Unit Price Difference");
                headerRow.CreateCell(14).SetCellValue("Project Number");
                headerRow.CreateCell(15).SetCellValue("Vendor Name");
                headerRow.CreateCell(16).SetCellValue("GRIN Date");

                // Populate data
                int rowIndex = 1;
                foreach (var item in PoAndGrinUnitPriceList)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.PONumber ?? "");
                    row.CreateCell(1).SetCellValue(item.PartNumber ?? "");
                    row.CreateCell(2).SetCellValue(item.MPN ?? "");
                    row.CreateCell(3).SetCellValue(item.Description ?? "");
                    row.CreateCell(4).SetCellValue((double)(item.POQty ?? 0));
                    row.CreateCell(5).SetCellValue(item.POUOM ?? "");
                    row.CreateCell(6).SetCellValue((double)(item.POUnitPrice ?? 0));
                    row.CreateCell(7).SetCellValue(item.POUOC ?? "");
                    row.CreateCell(8).SetCellValue(item.GrinNumber ?? "");
                    row.CreateCell(9).SetCellValue((double)(item.GRINQty ?? 0));
                    row.CreateCell(10).SetCellValue(item.GRINUOM ?? "");
                    row.CreateCell(11).SetCellValue((double)(item.GRINUnitPrice ?? 0));
                    row.CreateCell(12).SetCellValue(item.GRINUOC ?? "");
                    row.CreateCell(13).SetCellValue((double)(item.UnitPriceDifference ?? 0));
                    row.CreateCell(14).SetCellValue(item.projectnumber ?? "");
                    row.CreateCell(15).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(16).SetCellValue(item.grindate?.ToString("MM/dd/yyyy") ?? "");
                }



                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "Grin For PoAndGrinUnitPriceList.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }







        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetPoAndGrinUnitPriceSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<PoAndGrinUnitPriceSPReport>> serviceResponse = new ServiceResponse<IEnumerable<PoAndGrinUnitPriceSPReport>>();
            try
            {
                var products = await _repository.GetPoAndGrinUnitPriceSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PoAndGrinUnitPrice hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PoAndGrinUnitPrice hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"PoAndGrinUnitPriceWithDate Details Returned Successfully");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PoAndGrinUnitPriceWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPoAndGrinUnitPriceSPReportWithDate API: \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPoAndGrinUnitPriceSPReportWithDate API: \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetPurchaseInventorySPReportWithParam([FromBody] PurchaseInventorySPReportDto purchaseInventorySPReportDto)
        {
            ServiceResponse<IEnumerable<PurchaseInventorySPReport>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseInventorySPReport>>();
            try
            {
                var products = await _repository.GetPurchaseInventorySPReportWithParam(purchaseInventorySPReportDto.InvoiceNumber, purchaseInventorySPReportDto.GRINNumber,
                                                                            purchaseInventorySPReportDto.KPN, purchaseInventorySPReportDto.VendorName);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseInventory hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseInventory hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned PurchaseInventory Details:{products}");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseInventory Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseInventorySPReportWithParam API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseInventorySPReportWithParam API : \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }




        [HttpPost]
        public async Task<IActionResult> ExportGetPurchaseInventorySPReportWithParamOrDateToExcel([FromBody] PurchaseInventorySPReportWithParamAndDateDto PurchaseInventorySPReportWithParamAndDateDto)
        {

            ServiceResponse<PurchaseInventorySPReport> serviceResponse = new ServiceResponse<PurchaseInventorySPReport>();
            try
            {

                if (PurchaseInventorySPReportWithParamAndDateDto is null)
                {
                    _logger.LogError("PurchaseInventorySPReportWithParamAndDateDto object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseInventorySPReportWithParamAndDateDto object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid PurchaseInventorySPReportWithParamAndDateDto object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                bool hasParams = !string.IsNullOrEmpty(PurchaseInventorySPReportWithParamAndDateDto.InvoiceNumber)
                               || !string.IsNullOrEmpty(PurchaseInventorySPReportWithParamAndDateDto.GRINNumber)
                               || !string.IsNullOrEmpty(PurchaseInventorySPReportWithParamAndDateDto.KPN)
                               || !string.IsNullOrEmpty(PurchaseInventorySPReportWithParamAndDateDto.VendorName);

                bool hasDate = PurchaseInventorySPReportWithParamAndDateDto.FromDate != null || PurchaseInventorySPReportWithParamAndDateDto.ToDate != null;

                if (hasParams && hasDate)
                {
                    _logger.LogError("Input object must contain either filter parameters or date range, not both.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or a date range, not both.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!hasParams && (PurchaseInventorySPReportWithParamAndDateDto.FromDate == null || PurchaseInventorySPReportWithParamAndDateDto.ToDate == null))
                {
                    _logger.LogError("Input object must contain either at least one filter parameter or both from and to dates.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Please provide either filter parameters or both from and to dates.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);

                }

                var purchaseInventoryList = Enumerable.Empty<PurchaseInventorySPReport>();

                if (hasParams && !hasDate)
                {

                    purchaseInventoryList = await _repository.GetPurchaseInventorySPReportWithParam(PurchaseInventorySPReportWithParamAndDateDto.InvoiceNumber,
                        PurchaseInventorySPReportWithParamAndDateDto.GRINNumber,
                        PurchaseInventorySPReportWithParamAndDateDto.KPN, PurchaseInventorySPReportWithParamAndDateDto.VendorName);
                }


                if (!hasParams && (PurchaseInventorySPReportWithParamAndDateDto.FromDate != null && PurchaseInventorySPReportWithParamAndDateDto.ToDate != null))
                {

                    purchaseInventoryList = await _repository.GetPurchaseInventorySPReportWithDate(
                        PurchaseInventorySPReportWithParamAndDateDto.FromDate,
                        PurchaseInventorySPReportWithParamAndDateDto.ToDate);
                }



                // Create Excel workbook
                // Create Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("PurchaseInventoryReport");

                // Header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Invoice_Date");
                headerRow.CreateCell(1).SetCellValue("Invoice_No");
                headerRow.CreateCell(2).SetCellValue("InvoiceNumber");
                headerRow.CreateCell(3).SetCellValue("InvoiceDate");
                headerRow.CreateCell(4).SetCellValue("VoucherType");
                headerRow.CreateCell(5).SetCellValue("VendorName");
                headerRow.CreateCell(6).SetCellValue("Address1");
                headerRow.CreateCell(7).SetCellValue("Address2");
                headerRow.CreateCell(8).SetCellValue("Address3");
                headerRow.CreateCell(9).SetCellValue("SupplierPinCode");
                headerRow.CreateCell(10).SetCellValue("State");
                headerRow.CreateCell(11).SetCellValue("PlaceOfSupply");
                headerRow.CreateCell(12).SetCellValue("Country");
                headerRow.CreateCell(13).SetCellValue("GSTNNumber");
                headerRow.CreateCell(14).SetCellValue("ConsignorFromName");
                headerRow.CreateCell(15).SetCellValue("ConsignorFrom_Add1");
                headerRow.CreateCell(16).SetCellValue("ConsignorFrom_Add2");
                headerRow.CreateCell(17).SetCellValue("ConsignorFrom_Add3");
                headerRow.CreateCell(18).SetCellValue("ConsignorFrom_State");
                headerRow.CreateCell(19).SetCellValue("ConsignorFrom_Place");
                headerRow.CreateCell(20).SetCellValue("ConsignorFrom_Pincode");
                headerRow.CreateCell(21).SetCellValue("ConsignorFrom_GSTIN");
                headerRow.CreateCell(22).SetCellValue("TinNo");
                headerRow.CreateCell(23).SetCellValue("CSTNo");
                headerRow.CreateCell(24).SetCellValue("GSTRegistrationType");
                headerRow.CreateCell(25).SetCellValue("ReceiptNote_No");
                headerRow.CreateCell(26).SetCellValue("ReceiptNote_Date");
                headerRow.CreateCell(27).SetCellValue("OrderNo");
                headerRow.CreateCell(28).SetCellValue("OrderDate");
                headerRow.CreateCell(29).SetCellValue("Order_DueDate");
                headerRow.CreateCell(30).SetCellValue("LR_No");
                headerRow.CreateCell(31).SetCellValue("Despatch_Through");
                headerRow.CreateCell(32).SetCellValue("Destination");
                headerRow.CreateCell(33).SetCellValue("TermsOfPayment");
                headerRow.CreateCell(34).SetCellValue("Other_Reference");
                headerRow.CreateCell(35).SetCellValue("TermsOfDelivery");
                headerRow.CreateCell(36).SetCellValue("Purchase_Ledger");
                headerRow.CreateCell(37).SetCellValue("ItemName");
                headerRow.CreateCell(38).SetCellValue("HSN_Code");
                headerRow.CreateCell(39).SetCellValue("ItemDescription");
                headerRow.CreateCell(40).SetCellValue("TaxRate");
                headerRow.CreateCell(41).SetCellValue("BatchNo");
                headerRow.CreateCell(42).SetCellValue("MfgDate");
                headerRow.CreateCell(43).SetCellValue("ExpDate");
                headerRow.CreateCell(44).SetCellValue("Qty");
                headerRow.CreateCell(45).SetCellValue("UOM");
                headerRow.CreateCell(46).SetCellValue("Rate");
                headerRow.CreateCell(47).SetCellValue("Discount");
                headerRow.CreateCell(48).SetCellValue("Amount");
                headerRow.CreateCell(49).SetCellValue("OtherCharges_1_Ledger");
                headerRow.CreateCell(50).SetCellValue("OtherCharges_1_Amount");
                headerRow.CreateCell(51).SetCellValue("OtherCharges_2_Ledger");
                headerRow.CreateCell(52).SetCellValue("OtherCharges_2_Amount");
                headerRow.CreateCell(53).SetCellValue("OtherCharges_3_Ledger");
                headerRow.CreateCell(54).SetCellValue("OtherCharges_3_Amount");
                headerRow.CreateCell(55).SetCellValue("OtherCharges_4_Ledger");
                headerRow.CreateCell(56).SetCellValue("OtherCharges_4_Amount");
                headerRow.CreateCell(57).SetCellValue("OtherCharges_5_Ledger");
                headerRow.CreateCell(58).SetCellValue("OtherCharges_5_Amount");
                headerRow.CreateCell(59).SetCellValue("CSGT_Ledger");
                headerRow.CreateCell(60).SetCellValue("CGSTAmount");
                headerRow.CreateCell(61).SetCellValue("SGST_Ledger");
                headerRow.CreateCell(62).SetCellValue("SGSTAmount");
                headerRow.CreateCell(63).SetCellValue("IGST_Ledger");
                headerRow.CreateCell(64).SetCellValue("IGSTAmount");
                headerRow.CreateCell(65).SetCellValue("CessLedger");
                headerRow.CreateCell(66).SetCellValue("CessAmount");
                headerRow.CreateCell(67).SetCellValue("Roundoff_Ledger");
                headerRow.CreateCell(68).SetCellValue("Roundoff_Amount");
                headerRow.CreateCell(69).SetCellValue("CostCenter");
                headerRow.CreateCell(70).SetCellValue("Godown");
                headerRow.CreateCell(71).SetCellValue("Narration");
                headerRow.CreateCell(72).SetCellValue("eWay_BillNo");
                headerRow.CreateCell(73).SetCellValue("eWay_BillDate");
                headerRow.CreateCell(74).SetCellValue("Consolidated_eWay_BillNo");
                headerRow.CreateCell(75).SetCellValue("Consolidated_eWay_Date");
                headerRow.CreateCell(76).SetCellValue("SubType");
                headerRow.CreateCell(77).SetCellValue("DocumentType");
                headerRow.CreateCell(78).SetCellValue("Statusof_eWayBill");
                headerRow.CreateCell(79).SetCellValue("TransportMode");
                headerRow.CreateCell(80).SetCellValue("Distance");
                headerRow.CreateCell(81).SetCellValue("Transporter_Name");
                headerRow.CreateCell(82).SetCellValue("Vehicle_Number");
                headerRow.CreateCell(83).SetCellValue("Vehicle_Type");
                headerRow.CreateCell(84).SetCellValue("Doc_or_AirWay_BillNo");
                headerRow.CreateCell(85).SetCellValue("DocDate");
                headerRow.CreateCell(86).SetCellValue("Transporter_ID");

                // Populate data
                int rowIndex = 1;
                foreach (var item in purchaseInventoryList)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.Invoice_Date ?? "");
                    row.CreateCell(1).SetCellValue(item.Invoice_No ?? "");
                    row.CreateCell(2).SetCellValue(item.InvoiceNumber ?? "");
                    row.CreateCell(3).SetCellValue(item.InvoiceDate?.ToString("MM-dd-yyyy") ?? "");
                    row.CreateCell(4).SetCellValue(item.VoucherType ?? "");
                    row.CreateCell(5).SetCellValue(item.VendorName ?? "");
                    row.CreateCell(6).SetCellValue(item.Address1 ?? "");
                    row.CreateCell(7).SetCellValue(item.Address2 ?? "");
                    row.CreateCell(8).SetCellValue(item.Address3 ?? "");
                    row.CreateCell(9).SetCellValue(item.SupplierPinCode ?? "");
                    row.CreateCell(10).SetCellValue(item.State ?? "");
                    row.CreateCell(11).SetCellValue(item.PlaceOfSupply ?? "");
                    row.CreateCell(12).SetCellValue(item.Country ?? "");
                    row.CreateCell(13).SetCellValue(item.GSTNNumber ?? "");
                    row.CreateCell(14).SetCellValue(item.ConsignorFromName ?? "");
                    row.CreateCell(15).SetCellValue(item.ConsignorFrom_Add1 ?? "");
                    row.CreateCell(16).SetCellValue(item.ConsignorFrom_Add2 ?? "");
                    row.CreateCell(17).SetCellValue(item.ConsignorFrom_Add3 ?? "");
                    row.CreateCell(18).SetCellValue(item.ConsignorFrom_State ?? "");
                    row.CreateCell(19).SetCellValue(item.ConsignorFrom_Place ?? "");
                    row.CreateCell(20).SetCellValue(item.ConsignorFrom_Pincode ?? "");
                    row.CreateCell(21).SetCellValue(item.ConsignorFrom_GSTIN ?? "");
                    row.CreateCell(22).SetCellValue(item.TinNo ?? "");
                    row.CreateCell(23).SetCellValue(item.CSTNo ?? "");
                    row.CreateCell(24).SetCellValue(item.GSTRegistrationType ?? "");
                    row.CreateCell(25).SetCellValue(item.ReceiptNote_No ?? "");
                    row.CreateCell(26).SetCellValue(item.ReceiptNote_Date ?? "");
                    row.CreateCell(27).SetCellValue(item.OrderNo ?? "");
                    row.CreateCell(28).SetCellValue(item.OrderDate ?? "");
                    row.CreateCell(29).SetCellValue(item.Order_DueDate ?? "");
                    row.CreateCell(30).SetCellValue(item.LR_No ?? "");
                    row.CreateCell(31).SetCellValue(item.Despatch_Through ?? "");
                    row.CreateCell(32).SetCellValue(item.Destination ?? "");
                    row.CreateCell(33).SetCellValue(item.TermsOfPayment ?? "");
                    row.CreateCell(34).SetCellValue(item.Other_Reference ?? "");
                    row.CreateCell(35).SetCellValue(item.TermsOfDelivery ?? "");
                    row.CreateCell(36).SetCellValue(item.Purchase_Ledger ?? "");
                    row.CreateCell(37).SetCellValue(item.ItemName ?? "");
                    row.CreateCell(38).SetCellValue(item.HSN_Code ?? "");
                    row.CreateCell(39).SetCellValue(item.ItemDescription ?? "");
                    row.CreateCell(40).SetCellValue((double)(item.TaxRate ?? 0));
                    row.CreateCell(41).SetCellValue(item.BatchNo ?? "");
                    row.CreateCell(42).SetCellValue(item.MfgDate ?? "");
                    row.CreateCell(43).SetCellValue(item.ExpDate ?? "");
                    row.CreateCell(44).SetCellValue((double)(item.Qty ?? 0));
                    row.CreateCell(45).SetCellValue(item.UOM ?? "");
                    row.CreateCell(46).SetCellValue((double)(item.Rate ?? 0));
                    row.CreateCell(47).SetCellValue(item.Discount ?? "");
                    row.CreateCell(48).SetCellValue((double)(item.Amount ?? 0));
                    row.CreateCell(49).SetCellValue(item.OtherCharges_1_Ledger ?? "");
                    row.CreateCell(50).SetCellValue(item.OtherCharges_1_Amount ?? "");
                    row.CreateCell(51).SetCellValue(item.OtherCharges_2_Ledger ?? "");
                    row.CreateCell(52).SetCellValue(item.OtherCharges_2_Amount ?? "");
                    row.CreateCell(53).SetCellValue(item.OtherCharges_3_Ledger ?? "");
                    row.CreateCell(54).SetCellValue(item.OtherCharges_3_Amount ?? "");
                    row.CreateCell(55).SetCellValue(item.OtherCharges_4_Ledger ?? "");
                    row.CreateCell(56).SetCellValue(item.OtherCharges_4_Amount ?? "");
                    row.CreateCell(57).SetCellValue(item.OtherCharges_5_Ledger ?? "");
                    row.CreateCell(58).SetCellValue(item.OtherCharges_5_Amount ?? "");
                    row.CreateCell(59).SetCellValue((double)(item.CSGT_Ledger ?? 0));
                    row.CreateCell(60).SetCellValue((double)(item.CGSTAmount ?? 0));
                    row.CreateCell(61).SetCellValue((double)(item.SGST_Ledger ?? 0));
                    row.CreateCell(62).SetCellValue((double)(item.SGSTAmount ?? 0));
                    row.CreateCell(63).SetCellValue((double)(item.IGST_Ledger ?? 0));
                    row.CreateCell(64).SetCellValue((double)(item.IGSTAmount ?? 0));
                    row.CreateCell(65).SetCellValue(item.CessLedger ?? "");
                    row.CreateCell(66).SetCellValue(item.CessAmount ?? "");
                    row.CreateCell(67).SetCellValue(item.Roundoff_Ledger ?? "");
                    row.CreateCell(68).SetCellValue(item.Roundoff_Amount ?? "");
                    row.CreateCell(69).SetCellValue(item.CostCenter ?? "");
                    row.CreateCell(70).SetCellValue(item.Godown ?? "");
                    row.CreateCell(71).SetCellValue(item.Narration ?? "");
                    row.CreateCell(72).SetCellValue(item.eWay_BillNo ?? "");
                    row.CreateCell(73).SetCellValue(item.eWay_BillDate ?? "");
                    row.CreateCell(74).SetCellValue(item.Consolidated_eWay_BillNo ?? "");
                    row.CreateCell(75).SetCellValue(item.Consolidated_eWay_Date ?? "");
                    row.CreateCell(76).SetCellValue(item.SubType ?? "");
                    row.CreateCell(77).SetCellValue(item.DocumentType ?? "");
                    row.CreateCell(78).SetCellValue(item.Statusof_eWayBill ?? "");
                    row.CreateCell(79).SetCellValue(item.TransportMode ?? "");
                    row.CreateCell(80).SetCellValue(item.Distance ?? "");
                    row.CreateCell(81).SetCellValue(item.Transporter_Name ?? "");
                    row.CreateCell(82).SetCellValue(item.Vehicle_Number ?? "");
                    row.CreateCell(83).SetCellValue(item.Vehicle_Type ?? "");
                    row.CreateCell(84).SetCellValue(item.Doc_or_AirWay_BillNo ?? "");
                    row.CreateCell(85).SetCellValue(item.DocDate ?? "");
                    row.CreateCell(86).SetCellValue(item.Transporter_ID ?? "");
                }



                // Export Excel
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();
                    return File(excelBytes,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "purchaseInventory.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
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
                    _logger.LogInfo($"Returned Grin Details:{products}");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinSPReport API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinSPReport API : \n{ex.Message}";
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
                    _logger.LogInfo($"Returned GrinSPReportForTrans Details:{products}");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GrinSPReportForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinSPReportForTrans API:\n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinSPReportForTrans API:\n{ex.Message}";
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
                    _logger.LogInfo($"Returned GrinSPReportWithDateForTrans Details:{products}");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GrinSPReportWithDateForTrans Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinSPReportWithDateForTrans API:\n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinSPReportWithDateForTrans API:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetGrinSPReportWithDateForAvi([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<GrinSPReportForAvi>> serviceResponse = new ServiceResponse<IEnumerable<GrinSPReportForAvi>>();
            try
            {
                var products = await _repository.GetGrinSPReportWithDateForAvi(FromDate, ToDate);

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
                    _logger.LogInfo($"Returned GetGrinSPReportWithDateForAvi Details:{products}");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GetGrinSPReportWithDateForAvi Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinSPReportWithDateForAvi API: \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinSPReportWithDateForAvi API: \n{ex.Message}";
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
                    _logger.LogInfo($"Returned Grin Details:{products}");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Grin Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetItemDetailsByItemNumberList API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetItemDetailsByItemNumberList API : \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetPurchaseInventorySPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<PurchaseInventorySPReport>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseInventorySPReport>>();
            try
            {
                var products = await _repository.GetPurchaseInventorySPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseInventory hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseInventory hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned PurchaseInventory Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPurchaseInventorySPReportWithDate API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPurchaseInventorySPReportWithDate API : \n{ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllGrinNumberWhereBinningComplete API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllGrinNumberWhereBinningComplete API : \n{ex.Message}";
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
                _logger.LogError($"Error Occured in DeleteGrin API for the following Id :{id}, \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteGrin API for the following Id :{id}, \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<ActionResult> DownloadFile(int Id)//string Filename
        {
            ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();
            var itemsFiles = await _documentUploadRepository.GetDownloadUrlDetails(Id.ToString());
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinDocument", itemsFiles[0].FileName);
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
                _logger.LogError($"Error Occured in GetGrinDownloadUrlDetails API for the following grinNumber :{grinNumber}, \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinDownloadUrlDetails API for the following grinNumber :{grinNumber}, \n{ex.Message}";
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
                _logger.LogError($"Error Occured in GetGrinPartsDownloadUrlDetails API for the following grinNumber :{grinNumber}, \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinPartsDownloadUrlDetails API for the following grinNumber :{grinNumber}, \n{ex.Message}";
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
                _logger.LogError($"Error Occured in UpdateGrinUploadDocument API for the following grinNumber :{grinNumber}, \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateGrinUploadDocument API for the following grinNumber :{grinNumber}, \n{ex.Message}";
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
                _logger.LogInfo("GrinCOCUploadDocument Successfully Created");
                serviceResponse.Data = null;
                serviceResponse.Message = " GrinCOCUploadDocument Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateGrinPartsUploadDocument API for the following grinNumber :{grinNumber}, \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateGrinPartsUploadDocument API for the following grinNumber :{grinNumber}, \n{ex.Message}";
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

                    _logger.LogInfo("GrinCOCUploadDocument Successfully Deleted");
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
                _logger.LogError($"Error Occured in DeleteGrinCoCUploadDocument API for the following id :{id}, \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteGrinCoCUploadDocument API for the following id :{id}, \n{ex.Message}";
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
                    _logger.LogInfo("GrinUploadDocument Successfully Deleted");
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
                _logger.LogError($"Error Occured in DeleteGrinUploadDocument API for the following id :{id}, \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteGrinUploadDocument API for the following id :{id}, \n{ex.Message}";
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
                _logger.LogError($"Error Occured in UpdateGrinParts API for the following id :{id}, \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateGrinParts API for the following id :{id}, \n{ex.Message}";
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
                _logger.LogError($"Error Occured in DeleteGrinParts API for the following id :{id}, \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteGrinParts API for the following id :{id}, \n{ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllGrinNoList API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllGrinNoList API : \n{ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllGrinNumberForIqc API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllGrinNumberForIqc API : \n{ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllGrinNumberForBinning API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllGrinNumberForBinning API : \n{ex.Message}";
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
                _logger.LogError($"Error Occured in GetAllGrinParts API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllGrinParts API : \n{ex.Message}";
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
                _logger.LogError($"Error Occured in UpdateGrinTallyStatus API for the following id: {Id} \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateGrinTallyStatus API for the following id: {Id} \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateGRINAvgCost([FromBody] List<string> GrinNumbers)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                foreach (var grin in GrinNumbers)
                {
                    try
                    {
                        var getGrindetails = await _repository.GetGrinByGrinNo(grin);


                        var grinPartsDto = getGrindetails.GrinParts;
                        var grinCal = _mapper.Map<List<GrinPartscalculationofAvgcost>>(grinPartsDto);
                        var GrinpartsList = new List<GrinParts>();
                        decimal? othercosttotal = 1;
                        if ((getGrindetails.Freight + getGrindetails.Insurance + getGrindetails.LoadingorUnLoading + getGrindetails.Transport) > 1) othercosttotal = getGrindetails.Freight + getGrindetails.Insurance + getGrindetails.LoadingorUnLoading + getGrindetails.Transport;
                        decimal? conversionrate = 1;
                        if (getGrindetails.CurrencyConversion > 1) conversionrate = getGrindetails.CurrencyConversion;
                        foreach (var gPart in grinCal)
                        {
                            if (gPart.Qty > 0 && gPart.UnitPrice > 0)
                            {
                                try
                                {
                                    decimal? EP = gPart.Qty * gPart.UnitPrice;
                                    decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
                                    if (Itemwithtax == null || Itemwithtax == 0) gPart.EPwithTax = EP * conversionrate;
                                    else gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
                                    gPart.EPforSingleQty = gPart.EPwithTax / gPart.Qty;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(grin + " at " + gPart.ItemNumber + "\n" + ex);
                                }
                            }
                        }
                        decimal? SumofEPwithtax = grinCal.Where(x => x.Qty > 0 && x.UnitPrice > 0).Sum(x => x.EPwithTax);
                        foreach (var gPart in grinCal)
                        {
                            if (gPart.Qty > 0 && gPart.UnitPrice > 0)
                            {
                                try
                                {
                                    decimal? distriduteOthercostforitem = (gPart.EPwithTax / SumofEPwithtax) * othercosttotal;
                                    decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / gPart.Qty;
                                    gPart.AverageCost = distriduteOthercostforitemsSingleQty + gPart.EPforSingleQty;

                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(grin + " at " + gPart.ItemNumber + "\n" + ex);
                                }
                            }
                            GrinParts grinParts = _mapper.Map<GrinParts>(gPart);
                            grinParts.ProjectNumbers = _mapper.Map<List<ProjectNumbers>>(gPart.ProjectNumbers);
                            GrinpartsList.Add(grinParts);
                        }
                        getGrindetails.GrinParts = GrinpartsList;
                        await _repository.UpdateGrin_ForTally(getGrindetails);
                        _repository.SaveAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(grin + "\n" + ex);
                    }
                }
                _logger.LogInfo($"Successfully UpdateGRINAvgCost API");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Successfully UpdateGRINAvgCost";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(200, serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateGRINAvgCost API for the following GrinNumbers: {GrinNumbers} \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateGRINAvgCost API for the following GrinNumbers: {GrinNumbers} \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetGrinComsumptionDetialsByPartNos(List<string> PartNoListString)
        {
            //openpurchaseorderdto
            ServiceResponse<IEnumerable<GrinComsumpReportDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinComsumpReportDto>>();
            try
            {
                var shopOrderComsumDetails = await _repository.GetGrinComsumptionDetialsByPartNos(PartNoListString);
                var result = _mapper.Map<IEnumerable<GrinComsumpReportDto>>(shopOrderComsumDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all  Grin Details By PartNo List";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGrinComsumptionDetialsByPartNos API for the following PartNoListString: {PartNoListString} \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGrinComsumptionDetialsByPartNos API for the following PartNoListString: {PartNoListString} \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
