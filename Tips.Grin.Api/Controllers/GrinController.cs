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
using static Org.BouncyCastle.Math.EC.ECCurve;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Org.BouncyCastle.Asn1.Ocsp;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Grin.Api.Controllers
{
    
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class GrinController : ControllerBase
    {
        private IGrinRepository _repository;
        private IGrinPartsRepository _grinPartsRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IDocumentUploadRepository _documentUploadRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
 

        public GrinController(IGrinRepository repository, IHttpContextAccessor httpContextAccessor, IDocumentUploadRepository documentUploadRepository, IGrinPartsRepository grinPartsRepository,
            IWebHostEnvironment webHostEnvironment, ILoggerManager logger, IMapper mapper, HttpClient httpClient,IConfiguration config)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;          
            _grinPartsRepository = grinPartsRepository;
            _logger = logger;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _documentUploadRepository = documentUploadRepository;
            _httpClient = httpClient;
            _config = config;

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
         public async Task<IActionResult> GetAllGrin([FromQuery] PagingParameter pagingParameter,[FromQuery] SearchParams searchParams)

        {
            ServiceResponse<IEnumerable<GrinDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinDto>>();

            try
            {
                var GetallGrins = await _repository.GetAllGrin(pagingParameter,searchParams);

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
                serviceResponse.Message = $"Internal server error { ex.Message}{ex.InnerException}";
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

                                COCUpload = grinParts.CoCUpload
                                    .Select(documentUpload => new DocumentUploadDto
                                    {
                                        Id = documentUpload.Id,
                                        FileName = documentUpload.FileName,
                                        FileExtension = documentUpload.FileExtension,
                                        FilePath = documentUpload.FilePath,
                                        CreatedBy = documentUpload.CreatedBy,
                                        CreatedOn = documentUpload.CreatedOn,
                                        LastModifiedBy = documentUpload.LastModifiedBy,
                                        LastModifiedOn = documentUpload.LastModifiedOn,
                                    }).ToList(),

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

                                COCUpload = grinParts.CoCUpload
                                    .Select(documentUpload => new DocumentUploadDto
                                    {
                                        Id = documentUpload.Id,
                                        FileName = documentUpload.FileName,
                                        FileExtension = documentUpload.FileExtension,
                                        FilePath = documentUpload.FilePath,
                                        CreatedBy = documentUpload.CreatedBy,
                                        CreatedOn = documentUpload.CreatedOn,
                                        LastModifiedBy = documentUpload.LastModifiedBy,
                                        LastModifiedOn = documentUpload.LastModifiedOn,
                                    }).ToList(),

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

                                COCUpload = grinParts.CoCUpload
                                    .Select(documentUpload => new DocumentUploadDto
                                    {
                                        Id = documentUpload.Id,
                                        FileName = documentUpload.FileName,
                                        FileExtension = documentUpload.FileExtension,
                                        FilePath = documentUpload.FilePath,
                                        CreatedBy = documentUpload.CreatedBy,
                                        CreatedOn = documentUpload.CreatedOn,
                                        LastModifiedBy = documentUpload.LastModifiedBy,
                                        LastModifiedOn = documentUpload.LastModifiedOn,
                                    }).ToList(),

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


                        //var httpClient = new HttpClient();

                        //// Include the token in the Authorization header
                        //var tokenValue = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                        //if (!string.IsNullOrEmpty(tokenValue) && tokenValue.StartsWith("Bearer "))
                        //{
                        //    var token = tokenValue.Substring(7);
                        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        //}

                        //Add ItemMasterEnggDetails in GrinParts
                        var ItemNumber = grinPartsItemMasterEnggDto.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var itemMasterDetails = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                            "GetItemMasterByItemNumber?", "&ItemNumber=", encodedItemNumber));

                        var inventoryObjectString = await itemMasterDetails.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        //dynamic inventoryObject = new List<dynamic>();
                        dynamic inventoryObject = inventoryObjectData.data;
                        //inventoryObject = inventoryObjectData;
                        //foreach (var item in inventoryObject)
                        //{
                            grinPartsItemMasterEnggDto.DrawingNo = inventoryObject.drawingNo;
                            grinPartsItemMasterEnggDto.DocRet = inventoryObject.docRet;
                            grinPartsItemMasterEnggDto.RevNo = inventoryObject.revNo;
                            grinPartsItemMasterEnggDto.IsCocRequired = inventoryObject.isCocRequired;
                            grinPartsItemMasterEnggDto.IsRohsItem = inventoryObject.isRohsItem;
                            grinPartsItemMasterEnggDto.IsShelfLife = inventoryObject.isShelfLife;
                            grinPartsItemMasterEnggDto.IsReachItem = inventoryObject.isReachItem;
                            grinPartsItemMasterEnggDto.FileUpload = inventoryObject.fileUpload.ToObject<List<DocumentUpload>>();

                            grinPartsItemMasterEnggList.Add(grinPartsItemMasterEnggDto);
                        //}

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

                //var newcount = await _repository.GetGrinNumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    grins.GrinNumber = days + months + years + "G" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    grins.GrinNumber = days + months + years + "G" + (e);
                //}

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

                List<GrinParts> grinPartsList  = new List<GrinParts>();
                //var grinPartsList = new List<GrinParts>();
                var grinDocumentUploadDtoList = new List<DocumentUpload>();
                var grinPartsDocumentUploadDtoList = new List<DocumentUpload>();

                //// grin upload



                ////parts coc upload

                //var grinPartsDetails = grinPostDto.GrinParts;
                //foreach (var grinCoCUpload in grinPartsDetails)
                //{

                //    var cocUploadDocs = grinCoCUpload.COCUpload;

                //    foreach (var cocUpload in cocUploadDocs)
                //    {
                //        var fileContent = cocUpload.FileByte;
                //        var grinNumber = grins.GrinNumber;
                //        string fileName = cocUpload.FileName + "." + cocUpload.FileExtension;
                //        string FileExt = Path.GetExtension(fileName).ToUpper();

                //        Guid guid = Guid.NewGuid();
                //        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinCoCUpload", guid.ToString() + "_" + fileName);
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
                //                ParentId = grinNumber,
                //                DocumentFrom = "GrinCoCDocument",
                //            };

                //            _documentUploadRepository.CreateUploadDocumentGrin(uploadedFile);
                //            _documentUploadRepository.SaveAsync();

                //            if (uploadedFile != null)
                //            {
                //                DocumentUpload poFileDetail = _mapper.Map<DocumentUpload>(uploadedFile);
                //                grinPartsDocumentUploadDtoList.Add(poFileDetail);
                //            }
                //        }

                //    }

                //}

                //end cocupload
                var totalGrinCost = (grins.Freight + grins.Insurance + grins.LoadingorUnLoading + grins.Transport)*grins.CurrencyConversion;
                if (grinPartsDto != null && totalGrinCost != 0)
                {
                    for (int i = 0; i < grinPartsDto.Count; i++)
                    {
                        GrinParts grinParts = _mapper.Map<GrinParts>(grinPartsDto[i]);
                        grinParts.ProjectNumbers = _mapper.Map<List<ProjectNumbers>>(grinPartsDto[i].ProjectNumbers);
                        decimal? EP = grinParts.Qty * grinParts.UnitPrice;
                        decimal? a = totalGrinCost * grinParts.Qty;
                        decimal? b = (EP / a) * totalGrinCost;
                        decimal? c = EP + b;
                        grinParts.AverageCost = c / grinParts.Qty;

                        grinPartsList.Add(grinParts);

                    }
                }
                //if (grinPartsDto != null)
                //{
                //    decimal? sumOfTotal = 0;
                //    decimal? total = 0;

                //    for (int i = 0; i < grinPartsDto.Count; i++)
                //    {
                //        GrinParts grinParts = _mapper.Map<GrinParts>(grinPartsDto[i]);
                //        sumOfTotal += grinParts.Qty * grinParts.UnitPrice;
                //    }
                //    var totalGrinCost = grins.Freight + grins.Insurance + grins.LoadingorUnLoading + grins.CurrencyConversion + grins.Transport + grins.BECurrencyValue;
                //    if (totalGrinCost != 0)
                //    {
                //        for (int i = 0; i < grinPartsDto.Count; i++)
                //        { 

                //            GrinParts grinParts = _mapper.Map<GrinParts>(grinPartsDto[i]);
                //            total = grinParts.Qty * grinParts.UnitPrice;
                //            var Cost = totalGrinCost * total / sumOfTotal;
                //            var finalCost = Cost / grinParts.Qty;
                //            var weightageCost = grinParts.UnitPrice + finalCost;
                //            total = 0;
                //            grinParts.WeightedAverage = weightageCost;
                //            grinParts.ItemType = grinPartsDto[i].ItemType;
                //            grinParts.ProjectNumbers = _mapper.Map<List<ProjectNumbers>>(grinPartsDto[i].ProjectNumbers);
                //            grinPartsList.Add(grinParts);

                //        }
                //    }
                //    else
                //    {
                //        for (int i = 0; i < grinPartsDto.Count; i++)
                //        {
                //            GrinParts grinParts = _mapper.Map<GrinParts>(grinPartsDto[i]);                    
                //            grinParts.ItemType = grinPartsDto[i].ItemType;
                //            grinParts.ProjectNumbers = _mapper.Map<List<ProjectNumbers>>(grinPartsDto[i].ProjectNumbers);
                //            grinPartsList.Add(grinParts);

                //        }

                //    }
                //}

                grins.GrinParts = grinPartsList;
                grins.IsGrinCompleted = true;
                //grins.GrinDocuments = grinDocumentUploadDtoList;
                //grins.GrinDocuments = grinPostDto.GrinDocuments;


                if (grins.GrinDocuments != null && grins.GrinDocuments.Count > 0)
                {
                    await GrinDocumentSave(grinPostDto, grins, grinDocumentUploadDtoList);
                }
                if (grinPartsDto != null)
                {
                    for (int i = 0; i < grinPartsDto.Count; i++)
                    {
                        if (grinPartsDto[i].COCUpload != null && grinPartsDto[i].COCUpload.Count > 0)
                        {
                            CoCDocumentSave(grinPartsDto, grins, grins.GrinNumber, i, grinPartsDocumentUploadDtoList); 
                        }

                    }
                }

                await _repository.CreateGrin(grins);
                _repository.SaveAsync();

                if (grins.GrinParts != null)
                {
                    foreach (var grinPart in grins.GrinParts)
                    {
                        var grinPartsId = await _grinPartsRepository.GetGrinPartsById(grinPart.Id);
                        grinPartsId.LotNumber = grins.GrinNumber + grinPartsId.Id;
                        await _grinPartsRepository.UpdateGrinQty(grinPartsId);
                        _grinPartsRepository.SaveAsync();
                    }
                }

                foreach (var parts in grinPartsList)
                {
                    if (parts.ProjectNumbers != null)
                    {
                        foreach (var project in parts.ProjectNumbers)
                        {
                            //var lotNumber = await _grinPartsRepository.GetGrinPartsById(parts.Id);

                            GrinInventoryDto grinInventoryDto = new GrinInventoryDto();
                            grinInventoryDto.PartNumber = parts.ItemNumber;
                            grinInventoryDto.LotNumber = parts.LotNumber;
                            grinInventoryDto.MftrPartNumber = parts.MftrItemNumber;
                            grinInventoryDto.Description = parts.ItemDescription;
                            grinInventoryDto.ProjectNumber = project.ProjectNumber;
                            grinInventoryDto.Balance_Quantity = Convert.ToDecimal(project.ProjectQty);
                            grinInventoryDto.UOM = parts.UOM; 
                            grinInventoryDto.Warehouse = "GRIN";
                            grinInventoryDto.Location = "GRIN";
                            grinInventoryDto.GrinNo = grins.GrinNumber; 
                            grinInventoryDto.GrinPartId = parts.Id;
                            grinInventoryDto.PartType = parts.ItemType;  //We need to check this
                            grinInventoryDto.ReferenceID = Convert.ToString(parts.Id);
                            grinInventoryDto.ReferenceIDFrom = "GRIN";
                            grinInventoryDto.GrinMaterialType = "";
                            grinInventoryDto.ShopOrderNo = "";

                            var json = JsonConvert.SerializeObject(grinInventoryDto);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            // Include the token in the Authorization header
                            var tokenValuess = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                            if (!string.IsNullOrEmpty(tokenValuess) && tokenValuess.StartsWith("Bearer "))
                            {
                                var token = tokenValuess.Substring(7);
                                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            }
                            var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventoryFromGrin"), data);
                             
                            // Handle the response here


                        }
                    }
                }

                //Add in InventoryTranction
                foreach (var parts in grinPartsList)
                {
                    if (parts.ProjectNumbers != null)
                    {
                        foreach (var project in parts.ProjectNumbers)
                        {
                            GrinInventoryTranctionDto grinInventoryTranctionDto = new GrinInventoryTranctionDto();
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
                            grinInventoryTranctionDto.PartType = parts.ItemType;  //We need to check this
                            grinInventoryTranctionDto.ReferenceID = Convert.ToString(parts.Id);
                            grinInventoryTranctionDto.ReferenceIDFrom = "GRIN";
                            grinInventoryTranctionDto.GrinMaterialType = "";
                            grinInventoryTranctionDto.ShopOrderNo = "";
                            grinInventoryTranctionDto.IsStockAvailable = true;

                            var json = JsonConvert.SerializeObject(grinInventoryTranctionDto);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            // Include the token in the Authorization header
                            var tokenValuesss = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                            if (!string.IsNullOrEmpty(tokenValuesss) && tokenValuesss.StartsWith("Bearer "))
                            {
                                var token = tokenValuesss.Substring(7);
                                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            }
                            var response = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranctionFromGrin"), data);

                        }
                    }
                }

                ////update balance qty  in Purchase order table for grin concept

                var grinPartsDetail = _mapper.Map<List<GrinUpdateQtyDetailsDto>>(grinPartsDto);
                //
                var jsons = JsonConvert.SerializeObject(grinPartsDetail);
                var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                // Include the token in the Authorization header
                var tokenValue = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(tokenValue) && tokenValue.StartsWith("Bearer "))
                {
                    var token = tokenValue.Substring(7);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                var responses = await _httpClient.PostAsync(string.Concat(_config["PurchaseAPI"], "UpdateBalanceQtyDetails"), datas);

                //Update PoStatus in Purchase order And PoItem table

                var grinPartsDetails = _mapper.Map<List<GrinQtyPoStatusUpdateDto>>(grinPartsDto);
                var jsonCon = JsonConvert.SerializeObject(grinPartsDetails);
                var datass = new StringContent(jsonCon, Encoding.UTF8, "application/json");
                // Include the token in the Authorization header
                var tokenValues = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(tokenValues) && tokenValues.StartsWith("Bearer "))
                {
                    var token = tokenValues.Substring(7);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                
                var result = await _httpClient.PostAsync(string.Concat(_config["PurchaseAPI"], "UpdatePoStatus"), datass);


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

        private void CoCDocumentSave(List<GrinPartsPostDto>? grinPartsDto, Grins grins, string number, int i, List<DocumentUpload> grinPartsDocumentUploadDtoList)
        {
            var cocUploadDocs = grinPartsDto[i].COCUpload;

            foreach (var cocUpload in cocUploadDocs)
            {
                var fileContent = cocUpload.FileByte;

                string fileName = cocUpload.FileName + "." + cocUpload.FileExtension;
                string FileExt = Path.GetExtension(fileName).ToUpper();

                //Guid guid = Guid.NewGuid();
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinCoCUpload",/* guid.ToString() + "_" +*/ fileName);
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
                        ParentId = number + "-" + "I",          //It Should be changed to GrinPartsId
                        DocumentFrom = "GrinCoCDocument",
                    };

                    _documentUploadRepository.CreateUploadDocumentGrin(uploadedFile);
                    _documentUploadRepository.SaveAsync();

                    if (uploadedFile != null)
                    {
                        DocumentUpload poFileDetails = _mapper.Map<DocumentUpload>(uploadedFile);
                        grinPartsDocumentUploadDtoList.Add(poFileDetails);
                    }

                }
                grins.GrinParts[i].CoCUpload = grinPartsDocumentUploadDtoList;

            }
        }

        private async Task GrinDocumentSave(GrinPostDto grinPostDto, Grins grins, List<DocumentUpload> grinDocumentUploadDtoList)
        {
            var grinUploadDetails = grinPostDto.GrinDocuments;
            foreach (var grinUploadDetail in grinUploadDetails)
            {
                var fileContent = grinUploadDetail.FileByte;
                var grinNumber = grins.GrinNumber;
                string fileName = grinUploadDetail.FileName + "." + grinUploadDetail.FileExtension;
                string FileExt = Path.GetExtension(fileName).ToUpper();

                //Guid guid = Guid.NewGuid();
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinDocument",/*guid.ToString() + "_" +*/ fileName);
                using (MemoryStream ms = new MemoryStream(fileContent))
                {
                    ms.Position = 0;
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        ms.WriteTo(fileStream);
                    }
                    var uploadedFiles = new DocumentUpload
                    {
                        FileName = fileName,
                        FileExtension = FileExt,
                        FilePath = filePath,
                        ParentId = grinNumber,
                        DocumentFrom = "GrinDocument",

                    };

                    DocumentUpload poFileDetailsd = _mapper.Map<DocumentUpload>(uploadedFiles);

                    await _documentUploadRepository.CreateUploadDocumentGrin(poFileDetailsd);
                    _documentUploadRepository.SaveAsync();

                    if (uploadedFiles != null)
                    {
                        DocumentUpload poFileDetails = _mapper.Map<DocumentUpload>(uploadedFiles);
                        grinDocumentUploadDtoList.Add(poFileDetails);
                    }
                }
                grins.GrinDocuments = grinDocumentUploadDtoList;
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
               

                var grinparts = _mapper.Map<IEnumerable<GrinParts>>(grinDto.GrinParts);

                var grinList = _mapper.Map<Grins>(grinDto);

                var grinPartsDto = grinDto.GrinParts;

                var GrinpartsList = new List<GrinParts>();
                //for (int i = 0; i < grinPartsDto.Count; i++)
                //{
                //    GrinParts grinPartsDetail = _mapper.Map<GrinParts>(grinPartsDto[i]);
                //    grinPartsDetail.ProjectNumbers = _mapper.Map<List<ProjectNumbers>>(grinPartsDto[i].ProjectNumbers);

                //    GrinpartsList.Add(grinPartsDetail);
                //    //

                //}
                var totalGrinCost = (grinList.Freight + grinList.Insurance + grinList.LoadingorUnLoading + grinList.Transport) * grinList.CurrencyConversion;
                if (grinPartsDto != null)
                {
                    for (int i = 0; i < grinPartsDto.Count; i++)
                    {
                        GrinParts grinParts = _mapper.Map<GrinParts>(grinPartsDto[i]);
                        grinParts.ProjectNumbers = _mapper.Map<List<ProjectNumbers>>(grinPartsDto[i].ProjectNumbers);
                        decimal? EP = grinParts.Qty * grinParts.UnitPrice;
                        decimal? a = totalGrinCost * grinParts.Qty;
                        decimal? b = (EP / a) * totalGrinCost;
                        decimal? c = EP + b;
                        grinParts.AverageCost = c / grinParts.Qty;

                        GrinpartsList.Add(grinParts);

                    }
                }


                var data = _mapper.Map(grinDto, updategrin);


                data.GrinParts = grinparts.ToList();

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
                    //getDownloadUrlByFilename.DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/PurchaseOrder/DownloadFile?Filename={getDownloadUrlByFilename.FileName}";

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
                    //getDownloadUrlByFilename.DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/PurchaseOrder/DownloadFile?Filename={getDownloadUrlByFilename.FileName}";

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

        public async Task<IActionResult> GetAllGrinParts([FromQuery] PagingParameter pagingParameter,[FromQuery] SearchParams searchParams)

        {
            ServiceResponse<IEnumerable<GrinPartsDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinPartsDto>>();

            try
            {
                var GetallGrinsParts = await _grinPartsRepository.GetAllGrinParts(pagingParameter,searchParams);

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

    }
}
