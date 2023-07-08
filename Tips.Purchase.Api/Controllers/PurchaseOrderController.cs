using System;
using System.Buffers.Text;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AutoMapper;
using Azure;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private IPurchaseOrderRepository _repository;
        private IPoItemsRepository _poItemsRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IDocumentUploadRepository _documentUploadRepository;
        public static IWebHostEnvironment _webHostEnvironment { get; set; }


        public PurchaseOrderController(IPurchaseOrderRepository repository, IWebHostEnvironment webHostEnvironment, IPoItemsRepository poItemsRepository, IDocumentUploadRepository documentUploadRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _poItemsRepository = poItemsRepository;
            _logger = logger;
            _mapper = mapper;
            _documentUploadRepository = documentUploadRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrders([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<PurchaseOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderDto>>();
            try
            {
                var purchaseOrderDetails = await _repository.GetAllPurchaseOrders(pagingParameter,searchParamess);
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
                    List<DocumentUploadDto> documentUplaodDtoList = new List<DocumentUploadDto>();

                    if (purchaseOrderDto.POFiles.Count() != 0)
                    {
                        foreach (var documentUploadDetails in purchaseOrderDto.POFiles)
                        {
                            DocumentUploadDto poItemDtos = _mapper.Map<DocumentUploadDto>(documentUploadDetails);
                            documentUplaodDtoList.Add(poItemDtos);
                        }
                    }
                    purchaseOrderDto.POFiles = documentUplaodDtoList;

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

        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrderByPoNoAndRevNo(string PONumber,int revisionNumber)
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

                    List<DocumentUploadDto> documentUplaodDtoList = new List<DocumentUploadDto>();

                    if (purchaseOrderDto.POFiles.Count() != 0)
                    {
                        foreach (var documentUploadDetails in purchaseOrderDto.POFiles)
                        {
                            DocumentUploadDto poItemDtos = _mapper.Map<DocumentUploadDto>(documentUploadDetails);
                            documentUplaodDtoList.Add(poItemDtos);
                        }
                    }
                    purchaseOrderDto.POFiles = documentUplaodDtoList;

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

                    List<DocumentUploadDto> documentUplaodDtoList = new List<DocumentUploadDto>();

                    if (purchaseOrderDto.POFiles.Count() != 0)
                    {
                        foreach (var documentUploadDetails in purchaseOrderDto.POFiles)
                        {
                            DocumentUploadDto poItemDtos = _mapper.Map<DocumentUploadDto>(documentUploadDetails);
                            documentUplaodDtoList.Add(poItemDtos);
                        }
                    }
                    purchaseOrderDto.POFiles = documentUplaodDtoList;

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

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] PurchaseOrderPostDto purchaseOrderPostDto)
        {
            ServiceResponse<PurchaseOrderPostDto> serviceResponse = new ServiceResponse<PurchaseOrderPostDto>();
            try
            {
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
                var poItemDto = purchaseOrderPostDto.POItems;
                var poFile = purchaseOrderPostDto.POFiles;
                var poItemDtoList = new List<PoItem>();
                var poDocumentUploadDtoList = new List<DocumentUpload>();
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

                var dateFormat = days + months + years;
                var poNumber = await _repository.GeneratePONumber();
                purchaseOrderDetails.PONumber = dateFormat + poNumber;

                //// Po Upload

                var poUploadDetails = purchaseOrderPostDto.POFiles;
                foreach (var poUploadDetail in poUploadDetails)
                {
                    var fileContent = poUploadDetail.FileByte;
                    var poNumbers = purchaseOrderDetails.PONumber;
                    string fileName = poUploadDetail.FileName + "." + poUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    //Guid guid = Guid.NewGuid();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PODocument",/* guid.ToString() + "_" ,*/fileName);
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
                            ParentNumber = poNumber,
                            DocumentFrom = "PODocument",
                        };
                        _documentUploadRepository.CreateUploadDocumentPO(uploadedFile);
                        _documentUploadRepository.SaveAsync();

                        if (uploadedFile != null)
                        {
                            DocumentUpload poFileDetails = _mapper.Map<DocumentUpload>(uploadedFile);
                            poDocumentUploadDtoList.Add(poFileDetails);
                        }

                    }

                }

                if (poItemDto != null)
                {
                    for (int i = 0; i < poItemDto.Count; i++)
                    {
                        PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);
                        poItemDetails.BalanceQty = poItemDto[i].Qty;
                        poItemDetails.PoPartsStatus = false;
                        poItemDetails.PONumber = purchaseOrderDetails.PONumber;
                        poItemDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                        poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);
                        poItemDetails.POSpecialInstructions = _mapper.Map<List<PoSpecialInstruction>>(poItemDto[i].POSpecialInstructions);
                        poItemDetails.PrDetails = _mapper.Map<List<PrDetails>>(poItemDto[i].PrDetails);
                        poItemDtoList.Add(poItemDetails);
                    }
                }

                purchaseOrderDetails.POItems = poItemDtoList;
                purchaseOrderDetails.POFiles = poDocumentUploadDtoList;
                purchaseOrderDetails.PoIncoTerms = poIncoTermList.ToList();
                await _repository.CreatePurchaseOrder(purchaseOrderDetails);
                _repository.SaveAsync();
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
        public async Task<IActionResult> SearchPurchaseOrderDate([FromQuery] SearchDatesParams searchDateParam)
        {
            ServiceResponse<IEnumerable<PurchaseOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderDto>>();
            try
            {
                var purchaseOrderList = await _repository.SearchPurchaseOrderDate(searchDateParam);

                var result = _mapper.Map<IEnumerable<PurchaseOrderDto>>(purchaseOrderList);
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
        public async Task<IActionResult> GetAllPurchaseOrderWithItems([FromBody] PurchaseOrderSearchDto purchaseOrderSearch)
        {
            ServiceResponse<IEnumerable<PurchaseOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderDto>>();
            try
            {
                var purchaseOrderList = await _repository.GetAllPurchaseOrderWithItems(purchaseOrderSearch);

                _logger.LogInfo("Returned all PurhaseOrders");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<PurchaseOrderDto, PurchaseOrder>().ReverseMap()
                    .ForMember(dest => dest.POItems, opt => opt.MapFrom(src => src.POItems));
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<PurchaseOrderDto>>(purchaseOrderList);
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
        public async Task<IActionResult> SearchPurchaseOrder([FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<PurchaseOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderDto>>();
            try
            {
                var purchaseOrderList = await _repository.SearchPurchaseOrder(searchParams);
                _logger.LogInfo("Returned all PurchaseOrders");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<PurchaseOrderDto, PurchaseOrder>().ReverseMap()
                    .ForMember(dest => dest.POItems, opt => opt.MapFrom(src => src.POItems));
                });

                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<PurchaseOrderDto>>(purchaseOrderList);

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
                var downloadUrl = $"{Request.Scheme}://{Request.Host}/TipsPurchasePublish/api/PurchaseOrder/DownloadFile?Filename={filename}";
                //var downloadUrl = $"{Request.Scheme}://{Request.Host}/api/PurchaseOrder/DownloadFile?Filename={filename}";

                return Ok(new { FilePath = filePath, DownloadUrl = downloadUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //update uploaded files

        [HttpPost]
        public async Task<IActionResult> UpdatePOUploadDocument([FromBody] List<DocumentUploadPostDto> uploadDocumentDto, string poNumber)
        {
            ServiceResponse<DocumentUploadPostDto> serviceResponse = new ServiceResponse<DocumentUploadPostDto>();
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

                foreach (var poUploadDetail in uploadDocumentDto)
                {
                    var fileContent = poUploadDetail.FileByte;
                    string fileName = poUploadDetail.FileName + "." + poUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    Guid guid = Guid.NewGuid();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PODocument", /*guid.ToString() + "_" */ fileName);
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
                            ParentNumber = poNumber,
                            //PurchaseOrderId = Id,
                            DocumentFrom = "PODocument",

                        };
                        var poUploadDoc = _mapper.Map<DocumentUpload>(uploadedFile);

                        await _documentUploadRepository.CreateUploadDocumentPO(poUploadDoc);
                        _documentUploadRepository.SaveAsync();
                    }
                }

                serviceResponse.Data = null;
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
                var getDownloadDetailByPoNumber = await _repository.GetDownloadUrlDetails(poNumber);

                if (getDownloadDetailByPoNumber.Count() == 0 )
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

                foreach (var getDownloadUrlByFilename in getDownloadDetailByPoNumber)
                { 
                    getDownloadUrlByFilename.DownloadUrl = Url.Action("DownloadFile", "PurchaseOrder", new { Filename = getDownloadUrlByFilename.FileName }, protocol: HttpContext.Request.Scheme);
                    //getDownloadUrlByFilename.DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/PurchaseOrder/DownloadFile?Filename={getDownloadUrlByFilename.FileName}";
 
                } 
                    _logger.LogInfo($"Returned DownloadDetail with id: {poNumber}");
                    var result = _mapper.Map<IEnumerable<GetDownloadUrlDto>>(getDownloadDetailByPoNumber);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                
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


         

        [HttpPut]
        public async Task<IActionResult> UpdatePurchaseOrder([FromBody] PurchaseOrderUpdateDto purchaseOrderPostDto)
        {
            ServiceResponse<PurchaseOrderPostDto> serviceResponse = new ServiceResponse<PurchaseOrderPostDto>();
            try
            {
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
                var poItemDto = purchaseOrderPostDto.POItems;
                var poItemDtoList = new List<PoItem>();
                var poIncoTermDto = purchaseOrderPostDto.POIncoTerms;
                var poIncoTermsList = new List<PoIncoTerm>();

                if (poIncoTermDto != null)
                {
                    for (int i = 0; i < poIncoTermDto.Count; i++)
                    {
                        PoIncoTerm poIncoTermDetails = _mapper.Map<PoIncoTerm>(poIncoTermDto[i]);
                        poIncoTermsList.Add(poIncoTermDetails);
                    }
                }
                purchaseOrderDetails.PoIncoTerms = poIncoTermsList;

                if (poItemDto != null)
                {
                    for (int i = 0; i < poItemDto.Count; i++)
                    {
                        PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);
                        poItemDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                        poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);
                        poItemDetails.POSpecialInstructions = _mapper.Map<List<PoSpecialInstruction>>(poItemDto[i].POSpecialInstructions);
                        poItemDetails.PrDetails = _mapper.Map<List<PrDetails>>(poItemDto[i].PrDetails);
                        poItemDtoList.Add(poItemDetails);
                    }
                }

                purchaseOrderDetails.POItems = poItemDtoList;
                await _repository.ChangePurchaseOrderVersion(purchaseOrderDetails);
                _repository.SaveAsync();
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
                //var PoorderItem = poItems.FirstOrDefault();
                decimal dispatchedQty = item.Qty;

                foreach (var poItem in poItems)
                { 

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
            //


            _poItemsRepository.SaveAsync();
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

        [HttpPut("{PONumber}")]
        public async Task<IActionResult> ActivatePurchaseOrderApprovalI(string PONumber)
        {
            ServiceResponse<PurchaseOrderDto> serviceResponse = new ServiceResponse<PurchaseOrderDto>();

            try
            {
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
                purchaseOrderDetailByPONumber.POApprovedIBy = "Admin";
                purchaseOrderDetailByPONumber.POApprovedIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseOrder(purchaseOrderDetailByPONumber);
                _logger.LogInfo(result);
                _repository.SaveAsync();
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
                purchaseOrderDetailByPONumber.POApprovedIIBy = "Admin";
                purchaseOrderDetailByPONumber.POApprovedIIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseOrder(purchaseOrderDetailByPONumber);
                _logger.LogInfo(result);
                _repository.SaveAsync();
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
                purchaseOrderDetailById.ShortClosedBy = "Admin";
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
    }
}
