using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Purchase.Api.Contracts;
using Tips.Purchase.Api.Entities;
using Tips.Purchase.Api.Entities.Dto;
using Tips.Purchase.Api.Entities.DTOs;


namespace Tips.Purchase.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PurchaseRequisitionController : ControllerBase
    {
        private IPurchaseRequisitionRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IDocumentUploadRepository _prdocumentUploadRepository;


        public PurchaseRequisitionController(IPurchaseRequisitionRepository repository, IDocumentUploadRepository prdocumentUploadRepository ,ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _prdocumentUploadRepository = prdocumentUploadRepository;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseRequistions([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionDto>>();
            try
            {
                var purchaseRequisitionDetails = await _repository.GetAllPurchaseRequisitions(pagingParameter);
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

                    if (purchaseRequisitionDetailById.PrItemList != null)
                    {
                        foreach (var itemDetails in purchaseRequisitionDetailById.PrItemList)
                        {
                            PrItemsDto prItemDtos = _mapper.Map<PrItemsDto>(itemDetails);
                            prItemDtos.PrAddprojectsDtoList = _mapper.Map<List<PrAddProjectDto>>(itemDetails.PrAddprojects);
                            prItemDtos.PrAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliveryScheduleDto>>(itemDetails.PrAddDeliverySchedules);
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


        [HttpPost]
        public async Task<IActionResult> CreatePurchaseRequisition([FromBody] PurchaseRequisitionPostDto purchaseRequistionPostDto)
        {
            ServiceResponse<PurchaseRequisitionPostDto> serviceResponse = new ServiceResponse<PurchaseRequisitionPostDto>();
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
                var prItemDto = purchaseRequistionPostDto.PrItemsDtoPostList;
                var prItemDtoList = new List<PrItem>();
                var poDocumentUploadDtoList = new List<DocumentUpload>();

                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));

                var newcount = await _repository.GetPRNumberAutoIncrementCount(date);

                if (newcount > 0)
                {
                    var number = newcount + 1;
                    string e = String.Format("{0:D4}", number);
                    purchaseRequisitionDetails.PrNumber = days + months + years + "PR" + (e);
                }
                else
                {
                    var count = 1;
                    var e = count.ToString("D4");
                    purchaseRequisitionDetails.PrNumber = days + months + years + "PR" + (e);
                }


                //// Pr Upload

                var prUploadDetails = purchaseRequistionPostDto.PrFiles;
                foreach (var prUploadDetail in prUploadDetails)
                {
                    var fileContent = prUploadDetail.FileByte;
                    var prNumber = purchaseRequisitionDetails.PrNumber;
                    string fileName = prUploadDetail.FileName + "." + prUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    Guid guid = Guid.NewGuid();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PRDocument", guid.ToString() + "_" + fileName);
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
                            ParentNumber = prNumber,
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


                if (prItemDto != null)
                {
                    for (int i = 0; i < prItemDto.Count; i++)
                    {
                        PrItem prItemDetails = _mapper.Map<PrItem>(prItemDto[i]);
                        prItemDetails.PrAddprojects = _mapper.Map<List<PrAddProject>>(prItemDto[i].PrAddprojectsDtoPostList);
                        prItemDetails.PrAddDeliverySchedules = _mapper.Map<List<PrAddDeliverySchedule>>(prItemDto[i].PrAddDeliverySchedulesDtoPostList);
                        prItemDtoList.Add(prItemDetails);
                    }
                }
                purchaseRequisitionDetails.PrItemList = prItemDtoList;

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
                _logger.LogError($"Something went wrong inside CreatePurchaseRequisition action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //Test


        [HttpPut]
        public async Task<IActionResult> UpdatePurchaseRequisition([FromBody] PurchaseRequisitionUpdateDto purchaseRequistionPostDto)
        {
            ServiceResponse<PurchaseRequisitionPostDto> serviceResponse = new ServiceResponse<PurchaseRequisitionPostDto>();
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

                if (prItemDto != null)
                {
                    for (int i = 0; i < prItemDto.Count; i++)
                    {
                        PrItem prItemDetails = _mapper.Map<PrItem>(prItemDto[i]);
                        prItemDetails.PrAddprojects = _mapper.Map<List<PrAddProject>>(prItemDto[i].PrAddprojectsDtoUpdateList);
                        prItemDetails.PrAddDeliverySchedules = _mapper.Map<List<PrAddDeliverySchedule>>(prItemDto[i].PrAddDeliverySchedulesDtoUpdateList);
                        prItemDtoList.Add(prItemDetails);
                    }
                }
                purchaseRequisitionDetails.PrItemList = prItemDtoList;
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
        //                prItemDetails.PrAddprojects = _mapper.Map<List<PrAddProject>>(prItemDto[i].PrAddprojectsDtoUpdateList);
        //                prItemDetails.PrAddDeliverySchedules = _mapper.Map<List<PrAddDeliverySchedule>>(prItemDto[i].PrAddDeliverySchedulesDtoUpdateList);
        //                prItemList.Add(prItemDetails);
        //            }
        //        }

        //        purchaseRequisitionDetails.PrItemList = prItemList;
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

        [HttpPut("{PRNumber}")]
        public async Task<IActionResult> ActivatePurchaseRequisitionApprovalI(string PRNumber)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();

            try
            {
                var purchaseRequisitionDetailByPRNumber = await _repository.GetPurchaseRequisitionByPRNumber(PRNumber);
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
                purchaseRequisitionDetailByPRNumber.PrApprovedIBy = "Admin";
                purchaseRequisitionDetailByPRNumber.PrApprovedIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseRequisition(purchaseRequisitionDetailByPRNumber);
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
        public async Task<IActionResult> ActivatePurchaseRequisitionApprovalII(string PRNumber)
        {
            ServiceResponse<PurchaseRequisitionDto> serviceResponse = new ServiceResponse<PurchaseRequisitionDto>();

            try
            {
                var purchaseRequisitionDetailByPRNumber = await _repository.GetPurchaseRequisitionByPRNumber(PRNumber);
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
                purchaseRequisitionDetailByPRNumber.PrApprovedIIBy = "Admin";
                purchaseRequisitionDetailByPRNumber.PrApprovedIIDate = DateTime.Now;
                string result = await _repository.UpdatePurchaseRequisition(purchaseRequisitionDetailByPRNumber);
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

    }
}