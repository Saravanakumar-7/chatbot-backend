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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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
    public class PurchaseRequisitionController : ControllerBase
    {
        private IPurchaseRequisitionRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IDocumentUploadRepository _prdocumentUploadRepository;
        public static IWebHostEnvironment _webHostEnvironment { get; set; }



        public PurchaseRequisitionController(IPurchaseRequisitionRepository repository, IWebHostEnvironment webHostEnvironment, IDocumentUploadRepository prdocumentUploadRepository ,ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _prdocumentUploadRepository = prdocumentUploadRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseRequistions([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<PurchaseRequisitionDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionDto>>();
            try
            {
                var purchaseRequisitionDetails = await _repository.GetAllPurchaseRequisitions(pagingParameter,searchParamess);
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
            ServiceResponse<IEnumerable<PurchaseRequisitionDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionDto>>();
            try
            {
                var purchaseRequisitionList = await _repository.SearchPurchaseRequisitionDate(searchDateParam);

                var result = _mapper.Map<IEnumerable<PurchaseRequisitionDto>>(purchaseRequisitionList);
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
            ServiceResponse<IEnumerable<PurchaseRequisitionDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionDto>>();
            try
            {
                var purchaseRequisitionList = await _repository.SearchPurchaseRequisition(searchParams);

                _logger.LogInfo("Returned all purchaseRequisition");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<PurchaseRequisitionDto, PurchaseRequisition>().ReverseMap()
                    .ForMember(dest => dest.PrItemsDtoList, opt => opt.MapFrom(src => src.PrItemsDtoList));
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<PurchaseRequisitionDto>>(purchaseRequisitionList);

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
            ServiceResponse<IEnumerable<PurchaseRequisitionDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseRequisitionDto>>();
            try
            {
                var purchaseRequisitionList = await _repository.GetAllPurchaseRequisitionWithItems(purchaseRequisitionSearch);

                _logger.LogInfo("Returned all PurchaseRequisition");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<PurchaseRequisitionDto, PurchaseRequisition>().ReverseMap()
                    .ForMember(dest => dest.PrItemsDtoList, opt => opt.MapFrom(src => src.PrItemsDtoList));
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<PurchaseRequisitionDto>>(purchaseRequisitionList);
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
                            prItemDtos.PrAddprojectsDtoList = _mapper.Map<List<PrAddProjectDto>>(itemDetails.prAddprojectsDtoList);
                            prItemDtos.PrAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliveryScheduleDto>>(itemDetails.prAddDeliverySchedulesDtoList);
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

                var dateFormat = days + months + years;
                var prNumber = await _repository.GeneratePRNumber();
                purchaseRequisitionDetails.PrNumber = dateFormat + prNumber;


                //// Pr Upload

                var prUploadDetails = purchaseRequistionPostDto.PrFiles;
                foreach (var prUploadDetail in prUploadDetails)
                {
                    var fileContent = prUploadDetail.FileByte;
                    var prNumbers = purchaseRequisitionDetails.PrNumber;
                    string fileName = prUploadDetail.FileName + "." + prUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();

                    //Guid guid = Guid.NewGuid();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "PRDocument", /*guid.ToString() + "_" +*/ fileName);
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


                if (prItemDto != null)
                {
                    for (int i = 0; i < prItemDto.Count; i++)
                    {
                        PrItem prItemDetails = _mapper.Map<PrItem>(prItemDto[i]);
                        prItemDetails.prAddprojectsDtoList = _mapper.Map<List<PrAddProject>>(prItemDto[i].PrAddprojectsDtoPostList);
                        prItemDetails.prAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliverySchedule>>(prItemDto[i].PrAddDeliverySchedulesDtoPostList);
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

                if (prItemDto != null)
                {
                    for (int i = 0; i < prItemDto.Count; i++)
                    {
                        PrItem prItemDetails = _mapper.Map<PrItem>(prItemDto[i]);
                        prItemDetails.prAddprojectsDtoList = _mapper.Map<List<PrAddProject>>(prItemDto[i].PrAddprojectsDtoUpdateList);
                        prItemDetails.prAddDeliverySchedulesDtoList = _mapper.Map<List<PrAddDeliverySchedule>>(prItemDto[i].PrAddDeliverySchedulesDtoUpdateList);
                        prItemDtoList.Add(prItemDetails);
                    }
                }
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

        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetail(string prNumber)
        {
            ServiceResponse<IEnumerable<GetPRDownloadUrlDto>> serviceResponse = new ServiceResponse<IEnumerable<GetPRDownloadUrlDto>>();

            try
            {
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
 
                    getDownloadUrlByFilenames.DownloadUrl = Url.Action("DownloadFile", "PurchaseRequisition", new { Filename = getDownloadUrlByFilenames.FileName }, protocol: HttpContext.Request.Scheme);
                    //getDownloadUrlByFilename.DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/PurchaseOrder/DownloadFile?Filename={getDownloadUrlByFilename.FileName}";

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

    }
}