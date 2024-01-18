using System;
using System.Buffers.Text;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Azure;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;
using Tips.Purchase.Api.Entities.Enums;

namespace Tips.Purchase.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PurchaseRequisitionController : ControllerBase
    {
        private IPurchaseRequisitionRepository _repository;
        private IPrItemsRepository _prItemRepository;
        private ILoggerManager _logger;
        private IConfiguration _config;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private IDocumentUploadRepository _prdocumentUploadRepository;
        private IPRItemsDocumentUploadRepository _prItemsDocumentUploadRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public static IWebHostEnvironment _webHostEnvironment { get; set; }
        public PurchaseRequisitionController(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IPRItemsDocumentUploadRepository prItemsDocumentUploadRepository, IPrItemsRepository prItemRepository, IPurchaseRequisitionRepository repository, IWebHostEnvironment webHostEnvironment, IDocumentUploadRepository prdocumentUploadRepository, ILoggerManager logger, IMapper mapper, IConfiguration config)
        {
            _prItemsDocumentUploadRepository = prItemsDocumentUploadRepository;
            _repository = repository;
            _prItemRepository = prItemRepository;
            _logger = logger;
            _config = config;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
            _prdocumentUploadRepository = prdocumentUploadRepository;
            _webHostEnvironment = webHostEnvironment;
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseRequistions([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionDto>>();
            try
            {
                var purchaseRequisitionDetails = await _repository.GetAllPurchaseRequisitions(pagingParameter, searchParamess);
                var metadata = new
                {
                    purchaseRequisitionDetails.TotalCount,
                    purchaseRequisitionDetails.PageSize,
                    purchaseRequisitionDetails.CurrentPage,
                    purchaseRequisitionDetails.HasNext,
                    purchaseRequisitionDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all PurchaseRequisitions");
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionDto>>(purchaseRequisitionDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseRequisitions";
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
        public async Task<IActionResult> GetAllLastestPurchaseRequistions([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionDto>>();
            try
            {
                var lastestPurchaseRequisitionDetails = await _repository.GetAllLastestPurchaseRequisitions(pagingParameter, searchParamess);
                var metadata = new
                {
                    lastestPurchaseRequisitionDetails.TotalCount,
                    lastestPurchaseRequisitionDetails.PageSize,
                    lastestPurchaseRequisitionDetails.CurrentPage,
                    lastestPurchaseRequisitionDetails.HasNext,
                    lastestPurchaseRequisitionDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all LastestPurchaseRequisitions");
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionDto>>(lastestPurchaseRequisitionDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LastestPurchaseRequisitions";
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
        public async Task<IActionResult> GetPurchaseRequisitionByPRNoAndRevNo(string prNumber, int revisionNumber)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();
            try
            {
                var purchaseRequisitionDetail = await _repository.GetPurchaseRequisitionByPRNoAndRevNo(prNumber, revisionNumber);

                if (purchaseRequisitionDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseRequisition  hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseRequisition with id: {prNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {prNumber}");

                    PurchaseRequisitionDto purchaseRequisitionDto = _mapper.Map<PurchaseRequisitionDto>(purchaseRequisitionDetail);
                    List<PrItemsDto> prItemDtoList = new List<PrItemsDto>();

                    List<DocumentUploadDto> documentUplaodDtoList = new List<DocumentUploadDto>();

                    if (purchaseRequisitionDto.PrFiles.Count() != 0)
                    {
                        foreach (var documentUploadDetails in purchaseRequisitionDto.PrFiles)
                        {
                            DocumentUploadDto poItemDtos = _mapper.Map<DocumentUploadDto>(documentUploadDetails);
                            documentUplaodDtoList.Add(poItemDtos);
                        }
                    }
                    purchaseRequisitionDto.PrFiles = documentUplaodDtoList;
                    if (purchaseRequisitionDetail.PrItemsDtoList != null)
                    {
                        foreach (var prItemDetails in purchaseRequisitionDetail.PrItemsDtoList)
                        {
                            PrItemsDto prItemDtos = _mapper.Map<PrItemsDto>(prItemDetails);
                            prItemDtos.PrAddprojectsDtoList = _mapper.Map<List<PrAddProjectDto>>(prItemDetails.prAddprojectsDtoList);
                            prItemDtos.PrAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliveryScheduleDto>>(prItemDetails.prAddDeliverySchedulesDtoList);
                            prItemDtos.prSpecialInstructionsDtoList = _mapper.Map<List<PrSpecialInstructionDto>>(prItemDetails.prSpecialInstructionsDtoList);
                            prItemDtoList.Add(prItemDtos);
                        }
                    }
                    purchaseRequisitionDto.PrItemsDtoList = prItemDtoList;
                    serviceResponse.Data = purchaseRequisitionDto;
                    serviceResponse.Message = "Returned PurchaseRequisitionByPONoAndRevNo Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPurchaseRequisitionByPONoAndRevNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpGet("{PRNumber}")]
        public async Task<IActionResult> GetAllRevisionNumberListByPRNumber(string PRNumber)
        {
            ServiceResponse<IEnumerable<PurchaseRequistionRevNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequistionRevNoListDto>>();
            try
            {
                var revNumberDetailsbyPRNumber = await _repository.GetAllRevisionNumberListByPRNumber(PRNumber);
                var result = _mapper.Map<IEnumerable<PurchaseRequistionRevNoListDto>>(revNumberDetailsbyPRNumber);
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
                serviceResponse.Message = $"Something went wrong inside GetAllRevisionNumberListByPRNumber action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchPurchaseRequisitionDate([FromQuery] SearchDatesParams searchDateParam)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionReportDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionReportDto>>();
            try
            {
                var purchaseRequisitionList = await _repository.SearchPurchaseRequisitionDate(searchDateParam);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();

                    cfg.CreateMap<PurchaseRequisition, PurchaseRequisitionReportDto>()
                        .ForMember(dest => dest.PrItemsDtoList, opt => opt.MapFrom(src => src.PrItemsDtoList
                            .Select(prItem => new PrItemsReportDto
                            {
                                PrNumber = src.PrNumber,
                                ItemNumber = prItem.ItemNumber,
                                MftrItemNumber = prItem.MftrItemNumber,
                                Description = prItem.Description,
                                UOM = prItem.UOM,
                                PartType = prItem.PartType,
                                Qty = prItem.Qty,
                                prAddprojectsDtoList = prItem.prAddprojectsDtoList
                                    .Select(prAddProject => new PrAddProjectReportDto
                                    {
                                        Id = prItem.Id,
                                        PrNumber = src.PrNumber,
                                        ItemNumber = prItem.ItemNumber,
                                        ProjectNumber = prAddProject.ProjectNumber,
                                        ProjectQty = prAddProject.ProjectQty
                                    }).ToList(),
                                prAddDeliverySchedulesDtoList = prItem.prAddDeliverySchedulesDtoList
                                    .Select(prAddDeliverySchedule => new PrAddDeliveryScheduleReportDto
                                    {
                                        Id = prAddDeliverySchedule.Id,
                                        PrNumber = src.PrNumber,
                                        ItemNumber = prItem.ItemNumber,
                                        PrDeliveryDate = prAddDeliverySchedule.PrDeliveryDate,
                                        PrDeliveryQty = prAddDeliverySchedule.PrDeliveryQty
                                    }).ToList(),
                                prSpecialInstructionsDtoList = prItem.prSpecialInstructionsDtoList
                                    .Select(prSpecialInstruction => new PrSpecialInstructionDto
                                    {
                                        Id = prSpecialInstruction.Id,
                                        SpecialInstruction = prSpecialInstruction.SpecialInstruction
                                    }).ToList()
                            })
                         ));
                });

                var mapper = config.CreateMapper();


                var result = mapper.Map<IEnumerable<PurchaseRequisitionReportDto>>(purchaseRequisitionList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseRequisitionItems";
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
        public async Task<IActionResult> SearchPurchaseRequisition([FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionReportDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionReportDto>>();
            try
            {
                var purchaseRequisitionList = await _repository.SearchPurchaseRequisition(searchParams);

                _logger.LogInfo("Returned all purchaseRequisition");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<PurchaseRequisitionDto, PurchaseRequisition>().ReverseMap()
                //    .ForMember(dest => dest.PrItemsDtoList, opt => opt.MapFrom(src => src.PrItemsDtoList));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();

                    cfg.CreateMap<PurchaseRequisition, PurchaseRequisitionReportDto>()
                        .ForMember(dest => dest.PrItemsDtoList, opt => opt.MapFrom(src => src.PrItemsDtoList
                            .Select(prItem => new PrItemsReportDto
                            {
                                PrNumber = src.PrNumber,
                                ItemNumber = prItem.ItemNumber,
                                MftrItemNumber = prItem.MftrItemNumber,
                                Description = prItem.Description,
                                UOM = prItem.UOM,
                                PartType = prItem.PartType,
                                Qty = prItem.Qty,
                                prAddprojectsDtoList = prItem.prAddprojectsDtoList
                                    .Select(prAddProject => new PrAddProjectReportDto
                                    {
                                        Id = prAddProject.Id,
                                        PrNumber = src.PrNumber,
                                        ItemNumber = prItem.ItemNumber,
                                        ProjectNumber = prAddProject.ProjectNumber,
                                        ProjectQty = prAddProject.ProjectQty
                                    }).ToList(),
                                prAddDeliverySchedulesDtoList = prItem.prAddDeliverySchedulesDtoList
                                    .Select(prAddDeliverySchedule => new PrAddDeliveryScheduleReportDto
                                    {
                                        Id = prAddDeliverySchedule.Id,
                                        PrNumber = src.PrNumber,
                                        ItemNumber = prItem.ItemNumber,
                                        PrDeliveryDate = prAddDeliverySchedule.PrDeliveryDate,
                                        PrDeliveryQty = prAddDeliverySchedule.PrDeliveryQty
                                    }).ToList(),
                                prSpecialInstructionsDtoList = prItem.prSpecialInstructionsDtoList
                                    .Select(prSpecialInstruction => new PrSpecialInstructionDto
                                    {
                                        Id = prSpecialInstruction.Id,
                                        SpecialInstruction = prSpecialInstruction.SpecialInstruction
                                    }).ToList()
                            })
                         ));
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<PurchaseRequisitionReportDto>>(purchaseRequisitionList);

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseRequisitions";
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
        public async Task<IActionResult> GetAllPurchaseRequisitionWithItems([FromBody] PurchaseRequisitionSearchDto purchaseRequisitionSearch)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionReportDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionReportDto>>();
            try
            {
                var purchaseRequisitionList = await _repository.GetAllPurchaseRequisitionWithItems(purchaseRequisitionSearch);

                _logger.LogInfo("Returned all PurchaseRequisition");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<PurchaseRequisitionDto, PurchaseRequisition>().ReverseMap()
                //    .ForMember(dest => dest.PrItemsDtoList, opt => opt.MapFrom(src => src.PrItemsDtoList));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();

                    cfg.CreateMap<PurchaseRequisition, PurchaseRequisitionReportDto>()
                        .ForMember(dest => dest.PrItemsDtoList, opt => opt.MapFrom(src => src.PrItemsDtoList
                            .Select(prItem => new PrItemsReportDto
                            {
                                PrNumber = src.PrNumber,
                                ItemNumber = prItem.ItemNumber,
                                MftrItemNumber = prItem.MftrItemNumber,
                                Description = prItem.Description,
                                UOM = prItem.UOM,
                                PartType = prItem.PartType,
                                Qty = prItem.Qty,
                                prAddprojectsDtoList = prItem.prAddprojectsDtoList
                                    .Select(prAddProject => new PrAddProjectReportDto
                                    {
                                        Id = prItem.Id,
                                        PrNumber = src.PrNumber,
                                        ItemNumber = prItem.ItemNumber,
                                        ProjectNumber = prAddProject.ProjectNumber,
                                        ProjectQty = prAddProject.ProjectQty
                                    }).ToList(),
                                prAddDeliverySchedulesDtoList = prItem.prAddDeliverySchedulesDtoList
                                    .Select(prAddDeliverySchedule => new PrAddDeliveryScheduleReportDto
                                    {
                                        Id = prAddDeliverySchedule.Id,
                                        PrNumber = src.PrNumber,
                                        ItemNumber = prItem.ItemNumber,
                                        PrDeliveryDate = prAddDeliverySchedule.PrDeliveryDate,
                                        PrDeliveryQty = prAddDeliverySchedule.PrDeliveryQty
                                    }).ToList(),
                                prSpecialInstructionsDtoList = prItem.prSpecialInstructionsDtoList
                                    .Select(prSpecialInstruction => new PrSpecialInstructionDto
                                    {
                                        Id = prSpecialInstruction.Id,
                                        SpecialInstruction = prSpecialInstruction.SpecialInstruction
                                    }).ToList()
                            })
                         ));
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<PurchaseRequisitionReportDto>>(purchaseRequisitionList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseRequisition";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseRequisitionById(int id)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();
            try
            {
                var purchaseRequisitionDetailById = await _repository.GetPurchaseRequisitionById(id);

                if (purchaseRequisitionDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseRequisition  hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseRequisition with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned purchaseRequisitionDetails with id: {id}");

                    PurchaseRequisitionDto purchaseRequisitionDto = _mapper.Map<PurchaseRequisitionDto>(purchaseRequisitionDetailById);
                    List<PrItemsDto> prItemDtoList = new List<PrItemsDto>();

                    if (purchaseRequisitionDetailById.PrItemsDtoList != null)
                    {
                        foreach (var itemDetails in purchaseRequisitionDetailById.PrItemsDtoList)
                        {
                            PrItemsDto prItemDtos = _mapper.Map<PrItemsDto>(itemDetails);
                            //List<PRItemsDocumentUploadDto> fileUploads = new List<PRItemsDocumentUploadDto>();
                            //if (prItemDtos.Upload.Count() != 0)
                            //{
                            //    foreach (var fileUploadDetails in prItemDtos.Upload)
                            //    {
                            //        PRItemsDocumentUploadDto fileUploadDto = _mapper.Map<PRItemsDocumentUploadDto>(fileUploadDetails);
                            //        fileUploadDto.FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PRDocument", fileUploadDto.FileName);
                            //        fileUploads.Add(fileUploadDto);
                            //    }
                            //}
                            if (itemDetails.PRFileIds == null || itemDetails.PRFileIds == "")
                            {
                                prItemDtos.PRItemFiles = null;
                            }
                            else
                            {
                                List<PRItemsDocumentUploadDto> prd = new List<PRItemsDocumentUploadDto>();
                                string[]? ids = itemDetails.PRFileIds.Split(',');
                                for (int i = 0; i < ids.Count(); i++)
                                {
                                    var file1 = await _prItemsDocumentUploadRepository.GetUploadDocById(Convert.ToInt32(ids[i]));
                                    PRItemsDocumentUploadDto doc = _mapper.Map<PRItemsDocumentUploadDto>(file1);
                                    prd.Add(doc);
                                }
                                prItemDtos.PRItemFiles = prd;
                            }
                            // prItemDtos.Upload = _mapper.Map<List<PRItemsDocumentUploadDto>>(fileUploads);
                            prItemDtos.PrAddprojectsDtoList = _mapper.Map<List<PrAddProjectDto>>(itemDetails.prAddprojectsDtoList);
                            prItemDtos.PrAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliveryScheduleDto>>(itemDetails.prAddDeliverySchedulesDtoList);
                            prItemDtos.prSpecialInstructionsDtoList = _mapper.Map<List<PrSpecialInstructionDto>>(itemDetails.prSpecialInstructionsDtoList);
                            prItemDtoList.Add(prItemDtos);
                        }
                    }

                    purchaseRequisitionDto.PrItemsDtoList = prItemDtoList;
                    serviceResponse.Data = purchaseRequisitionDto;
                    serviceResponse.Message = "Returned PurchaseRequisitionById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPurchaseRequisitionById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        // [HttpGet]
        [HttpGet]
        public async Task<ActionResult> DownloadFile(string Filename)
        {
            ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PRDocument", Filename);
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
        //private List<PRItemsDocumentUpload> CoCDocumentSave(List<PrItemsPostDto>? grinPartsDto, PurchaseRequisition grins, string number, int i, List<PRItemsDocumentUpload> grinPartsDocumentUploadDtoList)
        //{
        //    var cocUploadDocs = grinPartsDto[i].Upload;

        //    foreach (var cocUpload in cocUploadDocs)
        //    {
        //        var fileContent = cocUpload.FileByte;
        //        byte[] imageContent = Convert.FromBase64String(cocUpload.FileByte);
        //        string fileName = cocUpload.FileName + "." + cocUpload.FileExtension;
        //        string FileExt = Path.GetExtension(fileName).ToUpper();

        //        Guid guid = Guid.NewGuid();
        //        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PRDocument",guid.ToString() + "_" + fileName);
        //        using (MemoryStream ms = new MemoryStream(imageContent))
        //        {
        //            ms.Position = 0;
        //            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //            {
        //                ms.WriteTo(fileStream);
        //            }
        //            var uploadedFile = new PRItemsDocumentUpload
        //            {
        //                FileName = fileName,
        //                FileExtension = FileExt,
        //                FilePath = filePath,
        //                ParentNumber = number,          //It Should be changed to GrinPartsId
        //                DocumentFrom = "PRItemDocument",
        //            };

        //            _prItemsDocumentUploadRepository.CreateUploadDocumentPO(uploadedFile);
        //            _prItemsDocumentUploadRepository.SaveAsync();

        //            if (uploadedFile != null)
        //            {
        //                PRItemsDocumentUpload poFileDetails = _mapper.Map<PRItemsDocumentUpload>(uploadedFile);
        //                grinPartsDocumentUploadDtoList.Add(poFileDetails);
        //            }

        //        }
        //        // grins.PrItemsDtoList[i].Upload = grinPartsDocumentUploadDtoList;

        //    }
        //    return grinPartsDocumentUploadDtoList;
        //}
        [HttpPost]
        public async Task<IActionResult> CreatePRFileUpload([FromBody] List<PRItemsDocumentUploadPostDto> ListofFiles)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (ListofFiles is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PRFiles are null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseRequisition object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PRFiles object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseRequisition object sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<string>? id_s = new List<string>();
                foreach (var prUploadDetail in ListofFiles)
                {
                    Guid guid = Guid.NewGuid();
                    var fileContent = prUploadDetail.FileByte;
                    byte[] imageContent = Convert.FromBase64String(prUploadDetail.FileByte);
                    string fileName = guid.ToString() + "_" + prUploadDetail.FileName + "." + prUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PRDocument", /*guid.ToString() + "_" +*/ fileName);
                    using (MemoryStream ms = new MemoryStream(imageContent))
                    {
                        ms.Position = 0;
                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            ms.WriteTo(fileStream);
                        }
                        var uploadedFile = new PRItemsDocumentUpload
                        {
                            FileName = fileName,
                            FileExtension = FileExt,
                            FilePath = filePath,
                            FileByte = fileContent,
                            ParentNumber = "PRItems",
                            DocumentFrom = "PRDocument",
                        };

                        _prItemsDocumentUploadRepository.CreateUploadDocumentPO(uploadedFile);
                        _prdocumentUploadRepository.SaveAsync();
                        //int k = Convert.ToInt32(ids);
                        id_s.Add(uploadedFile.Id.ToString());
                    }
                }

                serviceResponse.Data = id_s;
                serviceResponse.Message = " PurchaseRequisition Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside PRFileUpload action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreatePurchaseRequisition([FromBody] PurchaseRequisitionPostDto purchaseRequistionPostDto)
        {
            ServiceResponse<PurchaseRequisitionPostDto> serviceResponse = new ServiceResponse<PurchaseRequisitionPostDto>();
            try
            {
                string serverKey = GetServerKey();
                if (purchaseRequistionPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseRequisition object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseRequisition object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseRequisition object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseRequisition object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var purchaseRequisitionDetails = _mapper.Map<PurchaseRequisition>(purchaseRequistionPostDto);
                var prItemDto = purchaseRequistionPostDto.PrItemsDtoPostList;
                var prItemDtoList = new List<PrItem>();
                var poDocumentUploadDtoList = new List<DocumentUpload>();

                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                //var newcount = await _repository.GetPRNumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    purchaseRequisitionDetails.PrNumber = days + months + years + "PR" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    purchaseRequisitionDetails.PrNumber = days + months + years + "PR" + (e);
                //}

                if (serverKey == "trasccon")
                {
                    var dateFormat = days + months + years;
                    var prNumber = await _repository.GeneratePRNumber();
                    purchaseRequisitionDetails.PrNumber = dateFormat + prNumber;
                }
                else if (serverKey == "keus")
                {
                    var dateFormat = days + months + years;
                    var prNumber = await _repository.GeneratePRNumber();
                    purchaseRequisitionDetails.PrNumber = dateFormat + prNumber;
                }
                else
                {
                    var prNumber = await _repository.GeneratePRNumberAvision();
                    purchaseRequisitionDetails.PrNumber = prNumber;
                }


                //// Pr Upload

                var prUploadDetails = purchaseRequistionPostDto.PrFiles;
                foreach (var prUploadDetail in prUploadDetails)
                {
                    var fileContent = prUploadDetail.FileByte;
                    byte[] imageContent = Convert.FromBase64String(prUploadDetail.FileByte);
                    var prNumbers = purchaseRequisitionDetails.PrNumber;
                    string fileName = prUploadDetail.FileName + "." + prUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    Guid guid = Guid.NewGuid();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PRDocument", guid.ToString() + "_" + fileName);
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
                            ParentNumber = prNumbers,
                            DocumentFrom = "PRDocument",
                        };

                        _prdocumentUploadRepository.CreateUploadDocumentPO(uploadedFile);
                        _prdocumentUploadRepository.SaveAsync();

                        if (uploadedFile != null)
                        {
                            DocumentUpload prFileDetails = _mapper.Map<DocumentUpload>(uploadedFile);
                            poDocumentUploadDtoList.Add(prFileDetails);
                        }

                    }

                }


                //if (prItemDto != null)
                //{
                //    for (int i = 0; i < prItemDto.Count; i++)
                //    {
                //        PrItem prItemDetails = _mapper.Map<PrItem>(prItemDto[i]);
                //        prItemDetails.prAddprojectsDtoList = _mapper.Map<List<PrAddProject>>(prItemDto[i].PrAddprojectsDtoPostList);
                //        prItemDetails.prAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliverySchedule>>(prItemDto[i].PrAddDeliverySchedulesDtoPostList);
                //        prItemDetails.prSpecialInstructionsDtoList = _mapper.Map<List<PrSpecialInstruction>>(prItemDto[i].prSpecialInstructionsPostList);
                //        prItemDtoList.Add(prItemDetails);
                //    }
                //}
                // var CSitemDocumentUploadDtoList = new List<PRItemsDocumentUpload>();
                if (prItemDto != null)
                {
                    for (int i = 0; i < prItemDto.Count; i++)
                    {
                        //List<PRItemsDocumentUpload>? files = null;
                        PrItem prItemDetails = _mapper.Map<PrItem>(prItemDto[i]);
                        //if (prItemDto[i].Upload != null && prItemDto[i].Upload.Count > 0)
                        //{
                        //    files = CoCDocumentSave(prItemDto, purchaseRequisitionDetails, prItemDetails.Id.ToString(), i, CSitemDocumentUploadDtoList);
                        //}
                        //prItemDetails.Upload = _mapper.Map<List<PRItemsDocumentUpload>>(files);
                        prItemDetails.prAddprojectsDtoList = _mapper.Map<List<PrAddProject>>(prItemDto[i].PrAddprojectsDtoPostList);
                        prItemDetails.prAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliverySchedule>>(prItemDto[i].PrAddDeliverySchedulesDtoPostList);
                        prItemDetails.prSpecialInstructionsDtoList = _mapper.Map<List<PrSpecialInstruction>>(prItemDto[i].prSpecialInstructionsPostList);
                        prItemDtoList.Add(prItemDetails);
                    }
                }
                purchaseRequisitionDetails.PrItemsDtoList = prItemDtoList;

                purchaseRequisitionDetails.PrFiles = poDocumentUploadDtoList;
                await _repository.CreatePurchaseRequisition(purchaseRequisitionDetails);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseRequisition Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreatePurchaseRequisition action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //Test
        //private List<PRItemsDocumentUpload> CocDocumentSave(List<PrItemsUpdateDto>? grinPartsDto, PurchaseRequisition grins, string number, int i, List<PRItemsDocumentUpload> grinPartsDocumentUploadDtoList)
        //{
        //    var cocUploadDocs = grinPartsDto[i].Upload;

        //    foreach (var cocUpload in cocUploadDocs)
        //    {
        //        var fileContent = cocUpload.FileByte;

        //        string fileName = cocUpload.FileName + "." + cocUpload.FileExtension;
        //        string FileExt = Path.GetExtension(fileName).ToUpper();

        //        //Guid guid = Guid.NewGuid();
        //        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PRDocument",/* guid.ToString() + "_" +*/ fileName);
        //        using (MemoryStream ms = new MemoryStream(fileContent))
        //        {
        //            ms.Position = 0;
        //            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //            {
        //                ms.WriteTo(fileStream);
        //            }
        //            var uploadedFile = new PRItemsDocumentUpload
        //            {
        //                FileName = fileName,
        //                FileExtension = FileExt,
        //                FilePath = filePath,
        //                ParentNumber = number,          //It Should be changed to GrinPartsId
        //                DocumentFrom = "PRItemDocument",
        //            };

        //            _prItemsDocumentUploadRepository.CreateUploadDocumentPO(uploadedFile);
        //            _prItemsDocumentUploadRepository.SaveAsync();

        //            if (uploadedFile != null)
        //            {
        //                PRItemsDocumentUpload poFileDetails = _mapper.Map<PRItemsDocumentUpload>(uploadedFile);
        //                grinPartsDocumentUploadDtoList.Add(poFileDetails);
        //            }

        //        }
        //        // grins.PrItemsDtoList[i].Upload = grinPartsDocumentUploadDtoList;

        //    }
        //    return grinPartsDocumentUploadDtoList;
        //}


        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetails_Pritems(string prNumber, string prItemNumber)
        {
            ServiceResponse<IEnumerable<GetDownloadUrlDto>> serviceResponse = new ServiceResponse<IEnumerable<GetDownloadUrlDto>>();

            try
            {
                string serverKey = GetServerKey();

                var getDownloadDetailByPrNumber = await _repository.GetDownloadUrlDetails(prNumber, prItemNumber);

                if (getDownloadDetailByPrNumber.Count() == 0)
                {
                    _logger.LogError($"DownloadDetail with id: {prItemNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DownloadDetail with id: {prItemNumber}, hasn't been found.";
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
                if (getDownloadDetailByPrNumber != null)
                {
                    foreach (var getDownloadUrlByFilename in getDownloadDetailByPrNumber)
                    {
                        GetDownloadUrlDto downloadUrlDto = _mapper.Map<GetDownloadUrlDto>(getDownloadUrlByFilename);
                        if (serverKey == "avision")
                        {
                            var baseUrl = $"{_config["PurchaseBaseUrl"]}";
                            downloadUrlDto.DownloadUrl = $"{baseUrl}/apigateway/tips/PurchaseRequisition/DownloadFile?Filename={downloadUrlDto.FileName}";
                        }
                        else
                        {
                            var baseUrl = $"{Request.Scheme}://{_config["PurchaseBaseUrl"]}";
                            downloadUrlDto.DownloadUrl = $"{baseUrl}/api/PurchaseRequisition/DownloadFile?Filename={downloadUrlDto.FileName}";
                        }
                        downloadUrls.Add(downloadUrlDto);
                    }
                }
                _logger.LogInfo($"Returned DownloadDetail with id: {prNumber}");
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
        public async Task<IActionResult> UpdatePurchaseRequisition([FromBody] PurchaseRequisitionUpdateDto purchaseRequistionPostDto)
        {
            ServiceResponse<PurchaseRequisitionUpdateDto> serviceResponse = new ServiceResponse<PurchaseRequisitionUpdateDto>();
            try
            {
                if (purchaseRequistionPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseRequisition object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseRequisition object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseRequisition object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseRequisition object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var purchaseRequisitionDetails = _mapper.Map<PurchaseRequisition>(purchaseRequistionPostDto);
                var prItemDto = purchaseRequistionPostDto.PrItemsDtoUpdateList;
                var prItemDtoList = new List<PrItem>();
                //var CSitemDocumentUploadDtoList = new List<PRItemsDocumentUpload>();
                if (prItemDto != null)
                {
                    for (int i = 0; i < prItemDto.Count; i++)
                    {
                        // List<PRItemsDocumentUpload>? files = null;
                        PrItem prItemDetails = _mapper.Map<PrItem>(prItemDto[i]);
                        //if (prItemDto[i].Upload != null && prItemDto[i].Upload.Count > 0)
                        //{
                        //    files = CocDocumentSave(prItemDto, purchaseRequisitionDetails, prItemDetails.Id.ToString(), i, CSitemDocumentUploadDtoList);
                        //}
                        //prItemDetails.Upload = _mapper.Map<List<PRItemsDocumentUpload>>(files);
                        prItemDetails.prAddprojectsDtoList = _mapper.Map<List<PrAddProject>>(prItemDto[i].PrAddprojectsDtoUpdateList);
                        prItemDetails.prAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliverySchedule>>(prItemDto[i].PrAddDeliverySchedulesDtoUpdateList);
                        prItemDetails.prSpecialInstructionsDtoList = _mapper.Map<List<PrSpecialInstruction>>(prItemDto[i].prSpecialInstructionsUpdateList);
                        prItemDtoList.Add(prItemDetails);
                    }
                }
                //if (prItemDto != null)
                //{
                //    for (int i = 0; i < prItemDto.Count; i++)
                //    {
                //        PrItem prItemDetails = _mapper.Map<PrItem>(prItemDto[i]);
                //        prItemDetails.prAddprojectsDtoList = _mapper.Map<List<PrAddProject>>(prItemDto[i].PrAddprojectsDtoUpdateList);
                //        prItemDetails.prAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliverySchedule>>(prItemDto[i].PrAddDeliverySchedulesDtoUpdateList);
                //        prItemDetails.prSpecialInstructionsDtoList = _mapper.Map<List<PrSpecialInstruction>>(prItemDto[i].prSpecialInstructionsUpdateList);
                //        prItemDtoList.Add(prItemDetails);
                //    }
                //}
                purchaseRequisitionDetails.PrItemsDtoList = prItemDtoList;
                await _repository.ChangePurchaseRequisitionVersion(purchaseRequisitionDetails);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseRequisition Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdatePurchaseRequisition action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //test


        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdatePurchaseRequisition(int id, [FromBody] PurchaseRequisitionUpdateDto purchaseRequisitionUpdateDto)
        //{
        //    ServiceResponse<PurchaseRequisitionUpdateDto> serviceResponse = new ServiceResponse<PurchaseRequisitionUpdateDto>();
        //    try
        //    {
        //        if (purchaseRequisitionUpdateDto is null)
        //        {
        //            _logger.LogError("Update PurchaseRequisition object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Update PurchaseRequisition object is null.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid Update PurchaseRequisition object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid Update PurchaseRequisition object.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        var purchaseRequistionDetailById = await _repository.GetPurchaseRequisitionById(id);
        //        if (purchaseRequistionDetailById is null)
        //        {
        //            _logger.LogError($"Update PurchaseRequisition with id: {id}, hasn't been found in db.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"Update PurchaseRequisition hasn't been found.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }

        //        var purchaseRequisitionDetails = _mapper.Map<PurchaseRequisition>(purchaseRequistionDetailById);
        //        var prItemDto = purchaseRequisitionUpdateDto.PrItemsDtoUpdateList;
        //        var prItemList = new List<PrItem>();

        //        if (prItemDto != null)
        //        {
        //            for (int i = 0; i < prItemDto.Count; i++)
        //            {
        //                PrItem prItemDetails = _mapper.Map<PrItem>(prItemDto[i]);
        //                prItemDetails.prAddprojectsDtoList = _mapper.Map<List<PrAddProject>>(prItemDto[i].PrAddprojectsDtoUpdateList);
        //                prItemDetails.prAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliverySchedule>>(prItemDto[i].PrAddDeliverySchedulesDtoUpdateList);
        //                prItemList.Add(prItemDetails);
        //            }
        //        }

        //        purchaseRequisitionDetails.PrItemsDtoList = prItemList;
        //        var updatePurchaseRequisition = _mapper.Map(purchaseRequisitionUpdateDto, purchaseRequisitionDetails);
        //        string result = await _repository.UpdatePurchaseRequisition(updatePurchaseRequisition);
        //        _logger.LogInfo(result);
        //        _repository.SaveAsync();
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = " PurchaseRequisition Successfully Updated";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside UpdatePurchaseRequisition action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = $"Something went wrong ,try again";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseRequisition(int id)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();
            try
            {
                var purchaseRequisitionDetailById = await _repository.GetPurchaseRequisitionById(id);
                if (purchaseRequisitionDetailById == null)
                {
                    _logger.LogError($"Delete PurchaseRequisition with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete PurchaseRequisition hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeletePurchaseRequisition(purchaseRequisitionDetailById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " PurchaseRequisition Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeletePurchaseRequisition action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivePurchaseRequisitionNameList()
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>>();
            try
            {
                var activePRNameList = await _repository.GetAllActivePurchaseRequisitionNameList();
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(activePRNameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ActivePurchaseRequisitionNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActivePurchaseRequisitionNameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItemMasterFileUploadListByPRItemNumber(string itemNumber)
        {
            ServiceResponse<IEnumerable<ItemMasterFileUploadDtoList>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterFileUploadDtoList>>();
            try
            {
                var itemMasterDetails = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                    "GetItemMasterFileUploadListByItemNumber?", "&ItemNumber=", itemNumber));

                var itemMasterObjectString = await itemMasterDetails.Content.ReadAsStringAsync();
                dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);

                dynamic itemMasterObject = itemMasterObjectData.data;
                List<ItemMasterFileUploadDtoList> itemMasterFileUploadDtoList = new List<ItemMasterFileUploadDtoList>();

                foreach (var item in itemMasterObject)
                {
                    ItemMasterFileUploadDtoList newItem = new ItemMasterFileUploadDtoList();

                    newItem.Id = item.id;
                    newItem.FileName = item.fileName;
                    newItem.FileExtension = item.fileExtension;
                    newItem.FilePath = item.filePath;
                    newItem.FileByte = item.fileByte;
                    newItem.DocumentFrom = item.documentFrom;
                    newItem.ParentId = item.parentId;
                    newItem.CreatedBy = item.createdBy;
                    newItem.CreatedOn = item.createdOn;
                    newItem.LastModifiedBy = item.lastModifiedBy;
                    newItem.LastModifiedOn = item.lastModifiedOn;

                    itemMasterFileUploadDtoList.Add(newItem);
                }

                serviceResponse.Data = itemMasterFileUploadDtoList;
                serviceResponse.Message = "Returned all ItemMasterFileUploadList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetItemMasterFileUploadListByPRItemNumber action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseRequisitionNameList()
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>>();
            try
            {
                var prNumberList = await _repository.GetAllPurchaseRequisitionNameList();
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(prNumberList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseRequisitionNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPurchaseRequisitionNameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseRequisitionApprovalINameList()
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>>();
            try
            {
                var pendingPRApprovalINameList = await _repository.GetAllPendingPRApprovalINameList();
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(pendingPRApprovalINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIPurchaseRequisition";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPRApprovalINameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseRequisitionApprovalIINameList()
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>>();
            try
            {
                var pendingPRApprovalIINameList = await _repository.GetAllPendingPRApprovalIINameList();
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(pendingPRApprovalIINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIIPurchaseRequisition";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPRApprovalIINameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseRequisitionApprovalIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>>();
            try
            {
                var pendingPRApprovalINameList = await _repository.GetAllPendingPRApprovalIList(pagingParameter, searchParams);
                var metadata = new
                {
                    pendingPRApprovalINameList.TotalCount,
                    pendingPRApprovalINameList.PageSize,
                    pendingPRApprovalINameList.CurrentPage,
                    pendingPRApprovalINameList.HasNext,
                    pendingPRApprovalINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(pendingPRApprovalINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIPurchaseRequisition";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPRApprovalINameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLastestPendingPurchaseRequisitionApprovalIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>>();
            try
            {
                var lastestPendingPRApprovalINameList = await _repository.GetAllLastestPendingPRApprovalIList(pagingParameter, searchParams);
                var metadata = new
                {
                    lastestPendingPRApprovalINameList.TotalCount,
                    lastestPendingPRApprovalINameList.PageSize,
                    lastestPendingPRApprovalINameList.CurrentPage,
                    lastestPendingPRApprovalINameList.HasNext,
                    lastestPendingPRApprovalINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(lastestPendingPRApprovalINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LastestPendingApprovalIPurchaseRequisition";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllLastestPendingPRApprovalINameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPendingPurchaseRequisitionApprovalIIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>>();
            try
            {
                var pendingPRApprovalIINameList = await _repository.GetAllPendingPRApprovalIIList(pagingParameter, searchParams);
                var metadata = new
                {
                    pendingPRApprovalIINameList.TotalCount,
                    pendingPRApprovalIINameList.PageSize,
                    pendingPRApprovalIINameList.CurrentPage,
                    pendingPRApprovalIINameList.HasNext,
                    pendingPRApprovalIINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(pendingPRApprovalIINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PendingApprovalIIPurchaseRequisition";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllPendingPRApprovalIINameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLastestPendingPurchaseRequisitionApprovalIIList([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionIdNameListDto>>();
            try
            {
                var lastestPendingPRApprovalIINameList = await _repository.GetAllLastestPendingPRApprovalIIList(pagingParameter, searchParams);
                var metadata = new
                {
                    lastestPendingPRApprovalIINameList.TotalCount,
                    lastestPendingPRApprovalIINameList.PageSize,
                    lastestPendingPRApprovalIINameList.CurrentPage,
                    lastestPendingPRApprovalIINameList.HasNext,
                    lastestPendingPRApprovalIINameList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                var result = _mapper.Map<IEnumerable<PurchaseRequisitionIdNameListDto>>(lastestPendingPRApprovalIINameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LastestPendingApprovalIIPurchaseRequisition";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllLastestPendingPRApprovalIINameList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{PRNumber}")]
        public async Task<IActionResult> ActivatePurchaseRequisitionApprovalI(string PRNumber, int RevNo)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();

            try
            {
                var purchaseRequisitionDetailByPRNumber = await _repository.GetPurchaseRequisitionByPRNumber(PRNumber, RevNo);
                if (purchaseRequisitionDetailByPRNumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseRequisitionApprovalI object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseRequisitionApprovalI with string: {PRNumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchaseRequisitionDetailByPRNumber.PrApprovalI = true;
                purchaseRequisitionDetailByPRNumber.PrApprovedIBy = _createdBy;
                purchaseRequisitionDetailByPRNumber.PrApprovedIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseRequisition_ForApproval(purchaseRequisitionDetailByPRNumber);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "PurchaseRequisitionApprovalI Activated Successfully";
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
                _logger.LogError($"Something went wrong inside ActivatePurchaseRequisitionApprovalI action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{PRNumber}")]
        public async Task<IActionResult> ActivatePurchaseRequisitionApprovalII(string PRNumber, int RevNo)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();

            try
            {
                var purchaseRequisitionDetailByPRNumber = await _repository.GetPurchaseRequisitionByPRNumber(PRNumber, RevNo);
                if (purchaseRequisitionDetailByPRNumber is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseRequisitionApprovalII object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PurchaseRequisitionApprovalII with string: {PRNumber}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchaseRequisitionDetailByPRNumber.PrApprovalII = true;
                purchaseRequisitionDetailByPRNumber.PrApprovedIIBy = _createdBy;
                purchaseRequisitionDetailByPRNumber.PrApprovedIIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseRequisition_ForApproval(purchaseRequisitionDetailByPRNumber);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "PurchaseRequisitionApprovalII Activated Successfully";
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
                _logger.LogError($"Something went wrong inside ActivatePurchaseRequisitionApprovalII action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        //pr updated uploaded file

        [HttpPost]
        public async Task<IActionResult> UpdatePRUploadDocument([FromBody] List<DocumentUploadPostDto> uploadDocumentDto, string prNumber)
        {
            ServiceResponse<DocumentUploadPostDto> serviceResponse = new ServiceResponse<DocumentUploadPostDto>();
            try
            {
                if (uploadDocumentDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseRequisition UploadDocument object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseRequisition UploadDocument sent from client is null.");
                    return BadRequest(serviceResponse);
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


                //var purchaseOrderDetails = await _repository.GetPurchaseRequisitionByPRNumber(prNumber);
                //var Id = purchaseOrderDetails.Id;

                foreach (var prUploadDetail in uploadDocumentDto)
                {
                    var fileContent = prUploadDetail.FileByte;
                    byte[] imageContent = Convert.FromBase64String(prUploadDetail.FileByte);
                    string fileName = prUploadDetail.FileName + "." + prUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    Guid guid = Guid.NewGuid();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PRDocument", guid.ToString() + "_" + fileName);
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
                            ParentNumber = prNumber,
                            //PurchaseOrderId = Id,
                            DocumentFrom = "PRDocument",
                        };
                        var prUploadDoc = _mapper.Map<DocumentUpload>(uploadedFile);

                        await _prdocumentUploadRepository.CreateUploadDocumentPO(prUploadDoc);
                        _prdocumentUploadRepository.SaveAsync();

                    }
                }

                serviceResponse.Data = null;
                serviceResponse.Message = " PRUploadDocument Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdatePRUploadDocument action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //delete upload document

        [HttpDelete]
        public async Task<IActionResult> DeletePRUploadDocument(int id)
        {
            ServiceResponse<IEnumerable<DocumentUploadDto>> serviceResponse = new ServiceResponse<IEnumerable<DocumentUploadDto>>();

            try
            {
                var documentUploadDetails = await _prdocumentUploadRepository.GetUploadDocById(id);
                var fileName = documentUploadDetails.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PRDocument", /*guid.ToString() + "_" */ fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    string result = await _prdocumentUploadRepository.DeleteUploadFile(documentUploadDetails);
                    _logger.LogInfo(result);
                    _prdocumentUploadRepository.SaveAsync();

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

        //[HttpGet]
        //public async Task<ActionResult> DownloadFile(string Filename)
        //{
        //    ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();

        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PRDocument", Filename);
        //    var provider = new FileExtensionContentTypeProvider();
        //    if (!provider.TryGetContentType(filePath, out var ContentType))
        //    {
        //        ContentType = "application/octet-stream";
        //    }
        //    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

        //    return File(bytes, ContentType, Path.GetFileName(filePath));
        //}

        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetail(string prNumber)
        {
            ServiceResponse<IEnumerable<GetPRDownloadUrlDto>> serviceResponse = new ServiceResponse<IEnumerable<GetPRDownloadUrlDto>>();

            try
            {
                string serverKey = GetServerKey();

                var downloadDetailByPrNumber = await _repository.GetDownloadUrlDetail(prNumber);
                if (downloadDetailByPrNumber.Count() == 0)
                {
                    _logger.LogError($"DownloadDetail with id: {prNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DownloadDetail with id: {prNumber}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return Ok(serviceResponse);
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

                foreach (var getDownloadUrlByFilenames in downloadDetailByPrNumber)
                {
                    if (serverKey == "avision")
                    {
                        var baseUrl = $"{_config["PurchaseBaseUrl"]}";
                        getDownloadUrlByFilenames.DownloadUrl = $"{baseUrl}/apigateway/tips/PurchaseRequisition/DownloadFile?Filename={getDownloadUrlByFilenames.FileName}";
                    }
                    else
                    {
                        var baseUrl = $"{Request.Scheme}://{_config["PurchaseBaseUrl"]}";
                        getDownloadUrlByFilenames.DownloadUrl = $"{baseUrl}/api/PurchaseRequisition/DownloadFile?Filename={getDownloadUrlByFilenames.FileName}";
                    }
                }
                _logger.LogInfo($"Returned DownloadDetail with id: {prNumber}");
                var result = _mapper.Map<IEnumerable<GetPRDownloadUrlDto>>(downloadDetailByPrNumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned PRDownloadUrlDetail";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetDownloadUrlDetails action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ShortClosePrItemSatusByPrItemId(int prItemId, string? ReasonforShortClose)
        {
            ServiceResponse<PrItemsDto> serviceResponse = new ServiceResponse<PrItemsDto>();

            try
            {
                var prItemDetailByPrItemId = await _prItemRepository.ClosePrItemSatusByPrItemId(prItemId);
                if (prItemDetailByPrItemId == null)
                {
                    _logger.LogError($"PrItem with prItemId: {prItemId}, hasn't been found.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PrItem with prItemId hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                string? reasonforShortClose = ReasonforShortClose;
                prItemDetailByPrItemId.ReasonforShortClose = reasonforShortClose;
                prItemDetailByPrItemId.PrStatus = PrStatus.ShortClosed;
                string result = await _prItemRepository.UpdatePrItem(prItemDetailByPrItemId);
                _prItemRepository.SaveAsync();

                //Update PurchaseRequisition Table Status
                var prItemOpenStatuscount = await _prItemRepository.GetPrItemOpenStatusCount(prItemDetailByPrItemId.PurchaseRequistionId);

                if (prItemOpenStatuscount == 0)
                {
                    var prDetails = await _repository.GetPurchaseRequisitionById(prItemDetailByPrItemId.PurchaseRequistionId);
                    prDetails.PrStatus = PrStatus.ShortClosed;
                    await _repository.UpdatePurchaseRequisition(prDetails);
                    _repository.SaveAsync();
                }
                else
                {
                    var poDetails = await _repository.GetPurchaseRequisitionById(prItemDetailByPrItemId.PurchaseRequistionId);
                    poDetails.PrStatus = PrStatus.PartiallyClosed;
                    await _repository.UpdatePurchaseRequisition(poDetails);
                    _repository.SaveAsync();
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "PrItem Status have been closed";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ChangePrItemSatusByPrItemId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}