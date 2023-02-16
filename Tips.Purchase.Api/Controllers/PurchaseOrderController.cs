using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AutoMapper;
using Azure;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
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
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IDocumentUploadRepository _documentUploadRepository;


        public PurchaseOrderController(IPurchaseOrderRepository repository, IDocumentUploadRepository documentUploadRepository , ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _documentUploadRepository = documentUploadRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrders([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<PurchaseOrderDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseOrderDto>>();
            try
            {
                var purchaseOrderDetails = await _repository.GetAllPurchaseOrders(pagingParameter);
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

                    if (purchaseOrderDetailbyId.POItemList != null)
                    {
                        foreach (var poItemDetails in purchaseOrderDetailbyId.POItemList)
                        {
                            PoItemsDto poItemDtos = _mapper.Map<PoItemsDto>(poItemDetails);
                            poItemDtos.POAddprojects = _mapper.Map<List<PoAddProjectDto>>(poItemDetails.POAddprojects);
                            poItemDtos.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliveryScheduleDto>>(poItemDetails.POAddDeliverySchedules);
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


                var date = DateTime.Now;
                purchaseOrderPostDto.QuotationDate = date;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                var newcount = await _repository.GetPONumberAutoIncrementCount(date);

                if (newcount > 0)
                {
                    var number = newcount + 1;
                    string e = String.Format("{0:D4}", number);
                    purchaseOrderDetails.PONumber = days + months + years + "PO" + (e);
                }
                else
                {
                    var count = 1;
                    var e = count.ToString("D4");
                    purchaseOrderDetails.PONumber = days + months + years + "PO" + (e);
                }

                //// Po Upload

                var poUploadDetails = purchaseOrderPostDto.POFiles;
                foreach (var poUploadDetail in poUploadDetails)
                {
                    var fileContent = poUploadDetail.FileByte;
                    var poNumber = purchaseOrderDetails.PONumber;
                    string fileName = poUploadDetail.FileName + "." + poUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    Guid guid = Guid.NewGuid();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PODocument", guid.ToString() + "_" + fileName);
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
                        poItemDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                        poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);

                        poItemDtoList.Add(poItemDetails);
                    }
                } 

                purchaseOrderDetails.POItemList= poItemDtoList;
                purchaseOrderDetails.POFiles = poDocumentUploadDtoList;
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
                var downloadUrl = $"{Request.Scheme}://{Request.Host}/api/PurchaseOrder/DownloadFiles/{filename}";

                return Ok(new { FilePath = filePath, DownloadUrl = downloadUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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

                if (poItemDto != null)
                {
                    for (int i = 0; i < poItemDto.Count; i++)
                    {
                        PoItem poItemDetails = _mapper.Map<PoItem>(poItemDto[i]);
                        poItemDetails.POAddprojects = _mapper.Map<List<PoAddProject>>(poItemDto[i].POAddprojects);
                        poItemDetails.POAddDeliverySchedules = _mapper.Map<List<PoAddDeliverySchedule>>(poItemDto[i].POAddDeliverySchedules);

                        poItemDtoList.Add(poItemDetails);
                    }
                }

                purchaseOrderDetails.POItemList = poItemDtoList;
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

        //        purchaseOrderDetails.POItemList = poItemList;
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
