using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.Enums;
using Tips.Production.Api.Repository;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Entities.Migrations;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using Mysqlx.Crud;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MaterialRequestsController : ControllerBase
    {

        private IMaterialRequestsRepository _materialRequestRepository;
        private IMapper _mapper;
        private ILoggerManager _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;


        public MaterialRequestsController(IHttpClientFactory clientFactory, IConfiguration config, HttpClient httpClient,IMaterialRequestsRepository materialRequestRepository, IMapper mapper, ILoggerManager logger)
        {
            _materialRequestRepository = materialRequestRepository;
            _mapper = mapper;
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllMaterialRequest([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParammes)
        {
            ServiceResponse<IEnumerable<MaterialRequestsDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsDto>>();

            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetAllMaterialRequest(pagingParameter, searchParammes);
                var metadata = new
                {
                    materialRequestDetails.TotalCount,
                    materialRequestDetails.PageSize,
                    materialRequestDetails.CurrentPage,
                    materialRequestDetails.HasNext,
                    materialRequestDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all listOfmaterials");
                var result = _mapper.Map<IEnumerable<MaterialRequestsDto>>(materialRequestDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequest Successfully";
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
        public async Task<IActionResult> MaterialRequestSPReport()
        {
            var products = await _materialRequestRepository.MaterialRequestSPReport();

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> GetMaterialRequestSPReportWithParam([FromBody] MaterialRequestReportWithParamDto materialRequestReportWithParamDto)
        {
            ServiceResponse<IEnumerable<MaterialRequestSPReport>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestSPReport>>();
            try
            {
                var products = await _materialRequestRepository.GetMaterialRequestSPReportWithParam(materialRequestReportWithParamDto.MRNumber, 
                                                                            materialRequestReportWithParamDto.ProjectNumber, materialRequestReportWithParamDto.KPN,
                                                                            materialRequestReportWithParamDto.ShopOrderNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialRequest hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialRequest hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MaterialRequest Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetMaterialRequestSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetMaterialRequestSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<MaterialRequestSPReport>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestSPReport>>();
            try
            {
                var products = await _materialRequestRepository.GetMaterialRequestSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialRequest hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialRequest hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MaterialRequest Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetMaterialRequestSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetMaterialIssueAgainstMaterialRequestSPReportWithParam([FromBody] MaterialIssueAgainstMaterialRequestReportWithParamDto materialIssueAgainstMaterialRequestReportWithParamDto)
        {
            ServiceResponse<IEnumerable<MaterialRequestSPReport>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestSPReport>>();
            try
            {
                var products = await _materialRequestRepository.GetMaterialIssueAgainstMaterialRequestSPReportWithParam(materialIssueAgainstMaterialRequestReportWithParamDto.MRNumber,
                                                                            materialIssueAgainstMaterialRequestReportWithParamDto.ProjectType, materialIssueAgainstMaterialRequestReportWithParamDto.ProjectNumber,
                                                                            materialIssueAgainstMaterialRequestReportWithParamDto.ShopOrderNumber, materialIssueAgainstMaterialRequestReportWithParamDto.KPN);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssueAgainstMaterialRequest hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialIssueAgainstMaterialRequest hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MaterialIssueAgainstMaterialRequest Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetMaterialIssueAgainstMaterialRequestSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetMaterialIssueAgainstMaterialRequestSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<MaterialRequestSPReport>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestSPReport>>();
            try
            {
                var products = await _materialRequestRepository.GetMaterialIssueAgainstMaterialRequestSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MaterialIssueAgainstMaterialRequest hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MaterialIssueAgainstMaterialRequest hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MaterialIssueAgainstMaterialRequest Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetMaterialIssueAgainstMaterialRequestSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMROpenStatus([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParammes)
        {
            ServiceResponse<IEnumerable<MaterialRequestsDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsDto>>();

            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetAllMRStatusOpen(pagingParameter, searchParammes);

                var metadata = new
                {
                    materialRequestDetails.TotalCount,
                    materialRequestDetails.PageSize,
                    materialRequestDetails.CurrentPage,
                    materialRequestDetails.HasNext,
                    materialRequestDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all getAllMaterialRequestStatusOpen");
                var result = _mapper.Map<IEnumerable<MaterialRequestsDto>>(materialRequestDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequestStatusOpen Successfully";
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
        public async Task<IActionResult> GetAllMROpenwithPartialStatus([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParammes)
        {
            ServiceResponse<IEnumerable<MaterialRequestsDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsDto>>();

            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetAllMROpenwithPartialStatus(pagingParameter, searchParammes);

                var metadata = new
                {
                    materialRequestDetails.TotalCount,
                    materialRequestDetails.PageSize,
                    materialRequestDetails.CurrentPage,
                    materialRequestDetails.HasNext,
                    materialRequestDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogError("Returned all getAllMaterialRequestStatusOpen");
                var result = _mapper.Map<IEnumerable<MaterialRequestsDto>>(materialRequestDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequestStatusOpen Successfully";
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
        public async Task<IActionResult> GetAllMRCloseStatus()
        {
            ServiceResponse<IEnumerable<MaterialRequestsDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsDto>>();

            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetAllMRStatusClose();

                _logger.LogError("Returned all getAllMaterialRequestStatusClose");
                var result = _mapper.Map<IEnumerable<MaterialRequestsDto>>(materialRequestDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequestStatusClose Successfully";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialRequestById(int id)
        {
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetMaterialRequestById(id);

                if (materialRequestDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"materialrequest with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"materialrequest with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned materialrequest with id: {id}");

                    MaterialRequestsDto materialRequestDto = _mapper.Map<MaterialRequestsDto>(materialRequestDetails);


                    List<MaterialRequestItemsDto> materialRequestItemDtos = new List<MaterialRequestItemsDto>();

                    if (materialRequestDetails.MaterialRequestItems != null)
                    {

                        foreach (var materialitemDetails in materialRequestDetails.MaterialRequestItems)
                        {
                            MaterialRequestItemsDto materialRequestItemDto = _mapper.Map<MaterialRequestItemsDto>(materialitemDetails);
                            materialRequestItemDto.MRStockDetails = _mapper.Map<List<MRStockDetailsDto>>(materialitemDetails.MRStockDetails);

                            materialRequestItemDtos.Add(materialRequestItemDto);
                        }
                    }

                    materialRequestDto.MaterialRequestItems = materialRequestItemDtos;
                    serviceResponse.Data = materialRequestDto;
                    serviceResponse.Message = $"Returned materialrequest with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetmaterialRequestById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialReqByShopOrderNumber(string ShopOrderNo)
        {
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

            try
            {
                var getSObySONo = await _materialRequestRepository.GetMaterialReqByShopOrderNumber(ShopOrderNo);

                if (getSObySONo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Get MaterialRequest Details By ShopOrderNo with id: {ShopOrderNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Get MaterialRequest Details By ShopOrderNo with shoporderNo: {ShopOrderNo}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Get MaterialRequest Details By ShopOrderNo with shoporderNo: {ShopOrderNo}");

                    MaterialRequestsDto materialRequestDto = _mapper.Map<MaterialRequestsDto>(getSObySONo);

                    List<MaterialRequestItemsDto> materialRequestItemDtos = new List<MaterialRequestItemsDto>();
                    foreach (var materialReqbyMRNo in getSObySONo.MaterialRequestItems)
                    {
                        MaterialRequestItemsDto materialRequestItemDto = _mapper.Map<MaterialRequestItemsDto>(materialReqbyMRNo);
                        materialRequestItemDto.MRStockDetails = _mapper.Map<List<MRStockDetailsDto>>(materialReqbyMRNo.MRStockDetails);

                        materialRequestItemDtos.Add(materialRequestItemDto);
                    }
                    materialRequestDto.MaterialRequestItems = materialRequestItemDtos;
                    serviceResponse.Data = materialRequestDto;
                    serviceResponse.Message = $"Returned Get MaterialRequest Details By ShopOrderNo with ShopOrderNo: {ShopOrderNo}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMRDetailsByShopOrderNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
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
        [HttpPost]
        public async Task<IActionResult> CreateMaterialRequest([FromBody] MaterialRequestsPostDto materialRequestPostDto)
        {
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

            try
            {
                string serverKey = GetServerKey();
                if (materialRequestPostDto is null)
                {
                    _logger.LogError("MaterialRequest object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialRequest object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialRequest object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid MaterialRequest object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var createMaterialReq = _mapper.Map<MaterialRequests>(materialRequestPostDto);
                var materialReqDto = materialRequestPostDto.MaterialRequestItems;

                var materialReqItemList = new List<MaterialRequestItems>();

                if (materialReqDto != null)
                {

                    for (int i = 0; i < materialReqDto.Count; i++)
                    {
                        MaterialRequestItems materialItemListDetail = _mapper.Map<MaterialRequestItems>(materialReqDto[i]);
                        materialItemListDetail.MRStockDetails = _mapper.Map<List<MRStockDetails>>(materialReqDto[i].MRStockDetails);
                        materialReqItemList.Add(materialItemListDetail);

                    }
                }

                createMaterialReq.MaterialRequestItems = materialReqItemList;
                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));



                //var newcount = await _materialRequestRepository.GetMRNumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    createMaterialReq.MRNumber = days + months + years + "MR" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    createMaterialReq.MRNumber = days + months + years + "MR" + (e);
                //}

                if (serverKey == "trasccon")
                {
                    var dateFormat = days + months + years;
                    var mrNumber = await _materialRequestRepository.GenerateMRNumber();
                    createMaterialReq.MRNumber = mrNumber;
                }
                else if (serverKey == "keus")
                {
                    var dateFormat = days + months + years;
                    var mrNumber = await _materialRequestRepository.GenerateMRNumber();
                    createMaterialReq.MRNumber = mrNumber;
                }
                else
                {
                    var mrNumber = await _materialRequestRepository.GenerateMRNumberForAvision();
                    createMaterialReq.MRNumber = mrNumber;
                }

                await _materialRequestRepository.CreateMaterialRequest(createMaterialReq);

                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialRequest Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetMaterialRequestById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialRequest(int id, [FromBody] MaterialRequestUpdateDto materialRequestUpdateDto)
        {
             ServiceResponse<MaterialRequestUpdateDto> serviceResponse = new ServiceResponse<MaterialRequestUpdateDto>();

            try
            {
                if (materialRequestUpdateDto is null)
                {
                    _logger.LogError("MaterialRequest object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update MaterialRequest object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialRequest object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update MaterialRequest object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getMaterialRequest = await _materialRequestRepository.GetMaterialRequestById(id);
                if (getMaterialRequest is null)
                {
                    _logger.LogError($"materialReq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update materialReq with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateMaterialReqquest = _mapper.Map<MaterialRequests>(getMaterialRequest);

                var materialReqItemDto = materialRequestUpdateDto.MaterialRequestItems;

                var materialReqItemList = new List<MaterialRequestItems>();

                for (int i = 0; i < materialReqItemDto.Count; i++)
                {
                    MaterialRequestItems materialItemDetail = _mapper.Map<MaterialRequestItems>(materialReqItemDto[i]);
                    materialItemDetail.MRStockDetails = _mapper.Map<List<MRStockDetails>>(materialReqItemDto[i].MRStockDetails);

                    materialReqItemList.Add(materialItemDetail);

                }


                updateMaterialReqquest.MaterialRequestItems = materialReqItemList;
                var updateMaterialReq = _mapper.Map(materialRequestUpdateDto, getMaterialRequest);
               // updateMaterialReq.MaterialRequestItems = materialReqItemList;
                string result = await _materialRequestRepository.UpdateMaterialRequest(updateMaterialReq);
                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "materialReq Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        


        [HttpPut("{id}")]
        public async Task<IActionResult> IssueMaterialRequest(int id, [FromBody] MaterialRequestUpdateDto materialRequestUpdateDto)
        {
            ServiceResponse<MaterialRequestUpdateDto> serviceResponse = new ServiceResponse<MaterialRequestUpdateDto>();


            try
            {
                if (materialRequestUpdateDto is null)
                {
                    _logger.LogError("MaterialRequest object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update MaterialRequest object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid MaterialRequest object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update MaterialRequest object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getMaterialRequest = await _materialRequestRepository.GetMaterialRequestById(id);
                if (getMaterialRequest is null)
                {
                    _logger.LogError($"materialReq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update materialReq with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateMaterialReqquest = _mapper.Map<MaterialRequests>(getMaterialRequest);

                var materialReqItemDto = materialRequestUpdateDto.MaterialRequestItems;

                var materialReqItemList = new List<MaterialRequestItems>();
                var shopOrderNumber = materialRequestUpdateDto.ShopOrderNumber;
                var projectNo = materialRequestUpdateDto.ProjectNumber;
                var mRNumber = materialRequestUpdateDto.MRNumber;
                HttpStatusCode updateMaterialRequestResp = HttpStatusCode.OK;

                List<InventoryDtoForMaterialRequest> inventoryDtos = new List<InventoryDtoForMaterialRequest>();

                
                for (int i = 0; i < materialReqItemDto.Count; i++)
                {
                    MaterialRequestItems materialItemDetail = _mapper.Map<MaterialRequestItems>(materialReqItemDto[i]);
                    List<MRStockDetails> mrStockDetails = _mapper.Map<List<MRStockDetails>>(materialReqItemDto[i].MRStockDetails);
                    materialItemDetail.MRStockDetails = mrStockDetails;
                    var issuestock = mrStockDetails.Select(x => x.Qty).ToArray();
                    materialItemDetail.IssuedQty = issuestock.Sum();
                    materialReqItemList.Add(materialItemDetail);
                    foreach (var item in mrStockDetails)
                    {
                        //add material request data to somaterialissue tracker
                        InventoryDtoForMaterialRequest inventoryDtoForMaterialRequest = new InventoryDtoForMaterialRequest
                        {
                            PartNumber = materialItemDetail.PartNumber,
                            ProjectNumber = projectNo,
                            DataFrom = "MR",
                            IssueQty = materialItemDetail.IssuedQty,
                            ShopOrderNumber = shopOrderNumber,
                            MftrPartNumber = materialItemDetail.MftrPartNumber,
                            Description = materialItemDetail.PartDescription,
                            PartType = materialItemDetail.PartType,
                            MRNumber = materialRequestUpdateDto.MRNumber,
                            Warehouse = item.Warehouse,
                            Location = item.Location,
                            UOM = materialItemDetail.UOM,
                            Unit = materialRequestUpdateDto.Unit,

                        };
                        inventoryDtos.Add(inventoryDtoForMaterialRequest);                     

                    }


                } 
                var mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<MaterialRequests, UpdateInventoryBalanceQty>()
                    .ForMember(dest => dest.ProjectNumber, opt => opt.MapFrom(src => src.ProjectNumber));
                    cfg.CreateMap<MaterialRequests, UpdateInventoryBalanceQty>()
                    .ForMember(dest => dest.MRNumber, opt => opt.MapFrom(src => src.MRNumber));
                    cfg.CreateMap<MaterialRequestItems, UpdateInventoryBalanceQty>()
                        .ForMember(dest => dest.PartNumber, opt => opt.MapFrom(src => src.PartNumber))
                        .ForMember(dest => dest.MRNWarehouseList, opt => opt.MapFrom(src => src.MRStockDetails.Select(detail => new InventoryUpdateDtoForMRWarehouse
                        {
                            Warehouse = detail.Warehouse,
                            Location = detail.Location,
                            LocationStock = detail.LocationStock,
                            Qty = detail.Qty,
                        }).ToList()));
                });

                var mapper = mapperConfiguration.CreateMapper();

                var materialRequestDetails = materialReqItemList
                    .Select(item =>
                    {
                        var updateInventoryBalanceQty = mapper.Map<UpdateInventoryBalanceQty>(item);
                        updateInventoryBalanceQty.ProjectNumber = projectNo;
                        updateInventoryBalanceQty.MRNumber = mRNumber;
                        updateInventoryBalanceQty.ShopOrderNumber = shopOrderNumber;
                        return updateInventoryBalanceQty;
                    })
                    .ToList();

                var json = JsonConvert.SerializeObject(materialRequestDetails);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "MaterialInventoryBalanceQty"), data);

                var client1 = _clientFactory.CreateClient();
                var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                "MaterialInventoryBalanceQty"))
                {
                    Content = data
                };
                request1.Headers.Add("Authorization", token1);

                var response = await client1.SendAsync(request1);
                
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    updateMaterialRequestResp = response.StatusCode;
                }
                

                updateMaterialReqquest.MaterialRequestItems = materialReqItemList;
                var updateMaterialReq = _mapper.Map(materialRequestUpdateDto, getMaterialRequest);
                // updateMaterialReq.MaterialRequestItems = materialReqItemList;
                int? totalitems = updateMaterialReq.MaterialRequestItems.Count();
                if (totalitems > 0)
                {
                    if ((updateMaterialReq.MaterialRequestItems.Where(x => x.IssueStatus == IssuedStatus.Open).Count()) == totalitems) updateMaterialReq.MrStatus = MaterialStatus.Open;
                    else if ((updateMaterialReq.MaterialRequestItems.Where(x => x.IssueStatus == IssuedStatus.FullyIssued).Count()) == totalitems) updateMaterialReq.MrStatus = MaterialStatus.Closed;
                    else if (((updateMaterialReq.MaterialRequestItems.Where(x => x.IssueStatus == IssuedStatus.PartiallyIssued).Count()) > 0) || (updateMaterialReq.MaterialRequestItems.Where(x => x.IssueStatus == IssuedStatus.Open).Count() > 0)) updateMaterialReq.MrStatus = MaterialStatus.PartiallyClosed;

                }
                string result = await _materialRequestRepository.UpdateMaterialRequest(updateMaterialReq);

                if (updateMaterialRequestResp == HttpStatusCode.OK)
                {
                    _materialRequestRepository.SaveAsync();

                }
                else
                {
                    _logger.LogError($"Something went wrong inside UpdateMaterialRequest action. Inventory update action MaterialInventoryBalanceQty failed! ");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Internal server error";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
               

                serviceResponse.Data = null;
                serviceResponse.Message = "materialReq Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialRequest(int id)
        {
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

            try
            {
                var MaterialReqDetails = await _materialRequestRepository.GetMaterialRequestById(id);
                if (MaterialReqDetails == null)
                {
                    _logger.LogError($"materialReq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete materialReq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _materialRequestRepository.DeleteMaterialRequest(MaterialReqDetails);
                _logger.LogError(result);
                _materialRequestRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "MaterialReq Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteMaterialRequest action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenMRIdNoList()
        {
            ServiceResponse<IEnumerable<MaterialRequestIdNoDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestIdNoDto>>();
            try
            {
                var getAllMaterialReq = await _materialRequestRepository.GetAllOpenMRIdNoList();
                var result = _mapper.Map<IEnumerable<MaterialRequestIdNoDto>>(getAllMaterialReq);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All MaterialRequests";
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

        [HttpGet("{MRNumber}")]
        public async Task<IActionResult> GetMaterialReqByMRNumber(string MRNumber)
        {
            ServiceResponse<MaterialRequestsDto> serviceResponse = new ServiceResponse<MaterialRequestsDto>();

            try
            {
                var getMRbyMRNo = await _materialRequestRepository.GetMaterialReqByMRNumber(MRNumber);

                if (getMRbyMRNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GetMRNoDetailsById with id: {MRNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GetMRNoDetailsById with id: {MRNumber}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned GetMRNoDetailsById with id: {MRNumber}");

                    MaterialRequestsDto materialRequestDto = _mapper.Map<MaterialRequestsDto>(getMRbyMRNo);

                    List<MaterialRequestItemsDto> materialRequestItemDtos = new List<MaterialRequestItemsDto>();
                    foreach (var materialReqbyMRNo in getMRbyMRNo.MaterialRequestItems)
                    {
                        MaterialRequestItemsDto materialRequestItemDto = _mapper.Map<MaterialRequestItemsDto>(materialReqbyMRNo);
                        materialRequestItemDto.MRStockDetails = _mapper.Map<List<MRStockDetailsDto>>(materialReqbyMRNo.MRStockDetails);

                        materialRequestItemDtos.Add(materialRequestItemDto);
                    }
                    materialRequestDto.MaterialRequestItems = materialRequestItemDtos;
                    serviceResponse.Data = materialRequestDto;
                    serviceResponse.Message = $"Returned GetMRNoDetailsById with id: {MRNumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetMRNoDetailsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchMaterialRequests([FromQuery] SearchParamess searchParams)
        {
            ServiceResponse<IEnumerable<MaterialRequestsReportDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsReportDto>>();
            try
            {
                var MaterialRequestsDetails = await _materialRequestRepository.SearchMaterialRequests(searchParams);
                _logger.LogInfo("Returned all MaterialRequestsDetails");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<MaterialRequestsDto, MaterialRequests>().ReverseMap()
                //    .ForMember(dest => dest.MaterialRequestItems, opt => opt.MapFrom(src => src.MaterialRequestItems));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialRequests, MaterialRequestsReportDto>()
                       .ForMember(dest => dest.MaterialRequestItems, opt => opt.MapFrom(src => src.MaterialRequestItems
                       .Select(materialRequestItems => new MaterialRequestItemsReportDto
                       {
                           Id = materialRequestItems.Id,
                           MRNumber = src.MRNumber,
                           PartNumber = materialRequestItems.PartNumber,
                           PartDescription = materialRequestItems.PartDescription,
                           PartType = materialRequestItems.PartType,
                           Stock = materialRequestItems.Stock,
                           IssuedQty = materialRequestItems.IssuedQty,
                           IssueStatus = materialRequestItems.IssueStatus,
                           RequiredQty = materialRequestItems.RequiredQty,
                           MRStockDetails = materialRequestItems.MRStockDetails
                           .Select(mrStockDetails => new MRStockDetailsDto
                           {
                               Id = mrStockDetails.Id,
                               Warehouse = mrStockDetails.Warehouse,
                               Location = mrStockDetails.Location,
                               Qty = mrStockDetails.Qty,
                               LocationStock = mrStockDetails.LocationStock,
                           }).ToList()
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<MaterialRequestsReportDto>>(MaterialRequestsDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequestsDetails";
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
        public async Task<IActionResult> GetAllMaterialRequestsWithItems([FromBody] MaterialRequestSearchDto materialRequestSearch)
        {
            ServiceResponse<IEnumerable<MaterialRequestsReportDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsReportDto>>();
            try
            {
                var materialRequestDetails = await _materialRequestRepository.GetAllMaterialRequestsWithItems(materialRequestSearch);

                _logger.LogInfo("Returned all materialRequestDetails");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<MaterialRequestsDto, MaterialRequests>().ReverseMap()
                //    .ForMember(dest => dest.MaterialRequestItems, opt => opt.MapFrom(src => src.MaterialRequestItems));
                //});
                //var mapper = config.CreateMapper();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialRequests, MaterialRequestsReportDto>()
                       .ForMember(dest => dest.MaterialRequestItems, opt => opt.MapFrom(src => src.MaterialRequestItems
                       .Select(materialRequestItems => new MaterialRequestItemsReportDto
                       {
                           Id = materialRequestItems.Id,
                           MRNumber = src.MRNumber,
                           PartNumber = materialRequestItems.PartNumber,
                           PartDescription = materialRequestItems.PartDescription,
                           PartType = materialRequestItems.PartType,
                           Stock = materialRequestItems.Stock,
                           IssuedQty = materialRequestItems.IssuedQty,
                           IssueStatus = materialRequestItems.IssueStatus,
                           RequiredQty = materialRequestItems.RequiredQty,
                           MRStockDetails = materialRequestItems.MRStockDetails
                           .Select(mrStockDetails => new MRStockDetailsDto
                           {
                               Id = mrStockDetails.Id,
                               Warehouse = mrStockDetails.Warehouse,
                               Location = mrStockDetails.Location,
                               Qty = mrStockDetails.Qty,
                               LocationStock = mrStockDetails.LocationStock,
                           }).ToList()
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<MaterialRequestsReportDto>>(materialRequestDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all MaterialRequestsDetails";
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
        public async Task<IActionResult> SearchMaterialRequestsDate([FromQuery] SearchDateparames searchDateParam)
        {
            ServiceResponse<IEnumerable<MaterialRequestsReportDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestsReportDto>>();
            try
            {
                var materialRequestDetails = await _materialRequestRepository.SearchMaterialRequestsDate(searchDateParam);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<MaterialRequests, MaterialRequestsReportDto>()
                       .ForMember(dest => dest.MaterialRequestItems, opt => opt.MapFrom(src => src.MaterialRequestItems
                       .Select(materialRequestItems => new MaterialRequestItemsReportDto
                       {
                           Id = materialRequestItems.Id,
                           MRNumber = src.MRNumber,
                           PartNumber = materialRequestItems.PartNumber,
                           PartDescription = materialRequestItems.PartDescription,
                           PartType = materialRequestItems.PartType,
                           Stock = materialRequestItems.Stock,
                           IssuedQty = materialRequestItems.IssuedQty,
                           IssueStatus = materialRequestItems.IssueStatus,
                           RequiredQty = materialRequestItems.RequiredQty,
                           MRStockDetails = materialRequestItems.MRStockDetails
                           .Select(mrStockDetails => new MRStockDetailsDto
                           {
                               Id = mrStockDetails.Id,
                               Warehouse = mrStockDetails.Warehouse,
                               Location = mrStockDetails.Location,
                               Qty = mrStockDetails.Qty,
                               LocationStock = mrStockDetails.LocationStock,
                           }).ToList()
                       })
                           )
                       );
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<MaterialRequestsReportDto>>(materialRequestDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all materialRequestDetails";
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
        public async Task<IActionResult> GetInventoryItemNoAndDescriptionDetailsByProjectNo(string projectNumber)
        {
            ServiceResponse<IEnumerable<MaterialRequestInventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestInventoryDto>>();

            try
            {
                //var inventoryItemNoAndDescriptionList = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                           // "GetInventoryItemNoAndDescriptionListByProjectNo?", "&ProjectNumber=", projectNumber));

                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();

                var encodedProjectNumber = Uri.EscapeDataString(projectNumber);

                var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                    $"GetInventoryItemNoAndDescriptionListByProjectNo?ProjectNumber={encodedProjectNumber}"));
                request.Headers.Add("Authorization", token);

                var inventoryItemNoAndDescriptionList = await client.SendAsync(request);

                var inventoryObjectString = await inventoryItemNoAndDescriptionList.Content.ReadAsStringAsync();
                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                dynamic inventoryObject = inventoryObjectData.data;

                if (inventoryObject == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory ItemNo And Description with ProjectNumber: {projectNumber}, hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"Inventory ItemNo And Description with ProjectNumber: {projectNumber}, hasn't been found.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned Inventory ItemNo And Description with ProjectNumber: {projectNumber}");

                    var materialRequestInventoryDtoList = new List<MaterialRequestInventoryDto>();

                    foreach (var item in inventoryObjectData.data)
                    {
                        var partNumber = item.partNumber?.ToString();
                        var description = item.description?.ToString();

                        var materialRequestInventoryDto = new MaterialRequestInventoryDto
                        {
                            PartNumber = partNumber,
                            Description = description
                        };

                        materialRequestInventoryDtoList.Add(materialRequestInventoryDto);
                    }

                    serviceResponse.Data = materialRequestInventoryDtoList;
                    serviceResponse.Message = $"Returned Inventory ItemNo and Description List Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryItemNoAndDescriptionDetailsByProjectNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetGeneralInventoryItemNoAndDescriptionList()
        {
            ServiceResponse<IEnumerable<MaterialRequestInventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<MaterialRequestInventoryDto>>();

            try
            {
                //var inventoryItemNoAndDescriptionList = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                //            "GetInventoryItemNoAndDescriptionList?"));

                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();

                var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                    $"GetInventoryItemNoAndDescriptionList?"));

                request.Headers.Add("Authorization", token);

                var inventoryItemNoAndDescriptionList = await client.SendAsync(request);

                var inventoryObjectString = await inventoryItemNoAndDescriptionList.Content.ReadAsStringAsync();
                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                dynamic inventoryObject = inventoryObjectData.data;

                if (inventoryObject == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory ItemNo And Description hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"Inventory ItemNo And Description hasn't been found.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned Inventory ItemNo And Description ");

                    var materialRequestInventoryDtoList = new List<MaterialRequestInventoryDto>();

                    foreach (var item in inventoryObjectData.data)
                    {
                        var partNumber = item.partNumber?.ToString();
                        var description = item.description?.ToString();

                        var materialRequestInventoryDto = new MaterialRequestInventoryDto
                        {
                            PartNumber = partNumber,
                            Description = description
                        };

                        materialRequestInventoryDtoList.Add(materialRequestInventoryDto);
                    }

                    serviceResponse.Data = materialRequestInventoryDtoList;
                    serviceResponse.Message = $"Returned All Inventory ItemNo and Description List Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGeneralInventoryItemNoAndDescriptionList action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

    }
}