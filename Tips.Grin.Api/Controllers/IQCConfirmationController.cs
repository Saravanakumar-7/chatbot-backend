using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Mysqlx.Crud;
using Newtonsoft.Json;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Repository;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class IQCConfirmationController : ControllerBase
    {
        private IIQCConfirmationRepository _iQCConfirmationRepository;
        private IIQCConfirmationItemsRepository _iQCConfirmationItemsRepository;
        private IGrinPartsRepository _grinPartsRepository;
        private IGrinRepository _grinRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IQCConfirmationController(IGrinRepository grinRepository,IIQCConfirmationRepository iQCConfirmationRepository, 
            IIQCConfirmationItemsRepository iQCConfirmationItemsRepository, IGrinPartsRepository grinPartsRepository, 
            ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _iQCConfirmationRepository = iQCConfirmationRepository;
            _iQCConfirmationItemsRepository = iQCConfirmationItemsRepository;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _grinPartsRepository = grinPartsRepository;
            _grinRepository = grinRepository;
            _httpContextAccessor = httpContextAccessor;
    }

        [HttpGet]
        public async Task<IActionResult> GetAllIqcDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<IQCConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationDto>>();

            try
            {
                var getAllIQCDetails = await _iQCConfirmationRepository.GetAllIqcDetails(pagingParameter,searchParams);

                var metadata = new
                {
                    getAllIQCDetails.TotalCount,
                    getAllIQCDetails.PageSize,
                    getAllIQCDetails.CurrentPage,
                    getAllIQCDetails.HasNext,
                    getAllIQCDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all IQCConfirmation details()s");
                var result = _mapper.Map<IEnumerable<IQCConfirmationDto>>(getAllIQCDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all IQCConfirmation";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchIQCConfirmationDate([FromQuery] SearchDateParames searchDateParam)
        {
            ServiceResponse<IEnumerable<IQCConfirmationReportDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationReportDto>>();
            try
            {
                //var searchDateParamIQC = await _iQCConfirmationRepository.SearchIQCConfirmationDate(searchDateParam);

                //var result = _mapper.Map<IEnumerable<IQCConfirmationDto>>(searchDateParamIQC);

                var iQCList = await _iQCConfirmationRepository.SearchIQCConfirmationDate(searchDateParam);

                // Get all the unique GrinPartIds from the iQCList
                var grinPartIds = iQCList
                    .SelectMany(iqc => iqc.IQCConfirmationItems.Select(item => item.GrinPartId))
                    .Distinct()
                    .ToList();

                // Fetch all the required GrinPart details in a single query and store them in a dictionary
                var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsByGrinPartIds(grinPartIds);
                var grinPartDetailsLookup = grinPartDetails.ToDictionary(gp => gp.Id, gp => gp);

                // Use the grinPartDetailsLookup for quick lookups while mapping the data to DTO objects
                var iqcListDto = iQCList.Select(iqc => new IQCConfirmationReportDto
                {
                    // Map IQCConfirmation properties here (assuming properties with the same name exist in the DTO)
                    Id = iqc.Id,
                    GrinId = iqc.GrinId,
                    GrinNumber = iqc.GrinNumber,
                    Unit = iqc.Unit,
                    CreatedBy = iqc.CreatedBy,
                    CreatedOn = iqc.CreatedOn,
                    LastModifiedBy = iqc.LastModifiedBy,
                    LastModifiedOn = iqc.LastModifiedOn,
                    // ...

                    IQCConfirmationItems = iqc.IQCConfirmationItems.Select(item => new IQCConfirmationItemsReportDto
                    {
                        // Map IQCConfirmationItem properties here (assuming properties with the same name exist in the DTO)
                        Id = item.Id,
                        GrinPartId = item.GrinPartId,
                        ItemNumber = item.ItemNumber,
                        ReceivedQty = item.ReceivedQty,
                        GrinNumber = iqc.GrinNumber,
                        Remarks = item.Remarks,
                        MftrItemNumber = grinPartDetailsLookup[item.GrinPartId].MftrItemNumber,
                        PONumber = grinPartDetailsLookup[item.GrinPartId].PONumber,
                        ItemDescription = grinPartDetailsLookup[item.GrinPartId].ItemDescription,
                        ManufactureBatchNumber = grinPartDetailsLookup[item.GrinPartId].ManufactureBatchNumber,
                        UOM = grinPartDetailsLookup[item.GrinPartId].UOM,
                        ExpireDate = grinPartDetailsLookup[item.GrinPartId].ExpiryDate,
                        ManufactureDate = grinPartDetailsLookup[item.GrinPartId].ManufactureDate,
  
                    }).ToList(),
                }).ToList();

                serviceResponse.Data = iqcListDto;
                serviceResponse.Message = "Returned all IQCConfirmations";
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
        public async Task<IActionResult> SearchIQCConfirmation([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<IQCConfirmationReportDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationReportDto>>();
            try
            {
                var iQCList = await _iQCConfirmationRepository.SearchIQCConfirmation(searchParams);

                // Get all the unique GrinPartIds from the iQCList
                var grinPartIds = iQCList
                    .SelectMany(iqc => iqc.IQCConfirmationItems.Select(item => item.GrinPartId))
                    .Distinct()
                    .ToList();

                // Fetch all the required GrinPart details in a single query and store them in a dictionary
                var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsByGrinPartIds(grinPartIds);
                var grinPartDetailsLookup = grinPartDetails.ToDictionary(gp => gp.Id, gp => gp);
   
                // Use the grinPartDetailsLookup for quick lookups while mapping the data to DTO objects
                var iqcListDto = iQCList.Select(iqc => new IQCConfirmationReportDto
                {
                    // Map IQCConfirmation properties here (assuming properties with the same name exist in the DTO)
                    Id = iqc.Id,
                    GrinId = iqc.GrinId,
                    GrinNumber = iqc.GrinNumber,
                    Unit = iqc.Unit,
                    CreatedBy = iqc.CreatedBy,
                    CreatedOn = iqc.CreatedOn,
                    LastModifiedBy = iqc.LastModifiedBy,
                    LastModifiedOn = iqc.LastModifiedOn,
                    // ...

                    IQCConfirmationItems = iqc.IQCConfirmationItems.Select(item => new IQCConfirmationItemsReportDto
                    {
                        // Map IQCConfirmationItem properties here (assuming properties with the same name exist in the DTO)
                        Id = item.Id,
                        GrinPartId= item.GrinPartId,
                        ItemNumber = item.ItemNumber,
                        ReceivedQty = item.ReceivedQty,
                        GrinNumber = iqc.GrinNumber,
                        Remarks = item.Remarks,
                        MftrItemNumber = grinPartDetailsLookup[item.GrinPartId].MftrItemNumber,
                        PONumber = grinPartDetailsLookup[item.GrinPartId].PONumber,
                        ItemDescription = grinPartDetailsLookup[item.GrinPartId].ItemDescription,
                        ManufactureBatchNumber = grinPartDetailsLookup[item.GrinPartId].ManufactureBatchNumber,
                        UOM = grinPartDetailsLookup[item.GrinPartId].UOM,
                        ExpireDate = grinPartDetailsLookup[item.GrinPartId].ExpiryDate,
                        ManufactureDate = grinPartDetailsLookup[item.GrinPartId].ManufactureDate,

                    }).ToList(),
                }).ToList();

                _logger.LogInfo("Returned all IQCConfirmation");
                       
                serviceResponse.Data = iqcListDto;
                serviceResponse.Message = "Returned all IQCConfirmation";
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
        public async Task<IActionResult> GetAllIQCConfirmationWithItems([FromBody] IQCConfirmationSearchDto iQCConfirmationSearch)
        {
            ServiceResponse<IEnumerable<IQCConfirmationReportDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationReportDto>>();
            try
            { 
                var iQCList = await _iQCConfirmationRepository.GetAllIQCConfirmationWithItems(iQCConfirmationSearch);

                // Get all the unique GrinPartIds from the iQCList
                var grinPartIds = iQCList
                    .SelectMany(iqc => iqc.IQCConfirmationItems.Select(item => item.GrinPartId))
                    .Distinct()
                    .ToList();

                // Fetch all the required GrinPart details in a single query and store them in a dictionary
                var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsByGrinPartIds(grinPartIds);
                var grinPartDetailsLookup = grinPartDetails.ToDictionary(gp => gp.Id, gp => gp);

                // Use the grinPartDetailsLookup for quick lookups while mapping the data to DTO objects
                var iqcListDto = iQCList.Select(iqc => new IQCConfirmationReportDto
                {
                    // Map IQCConfirmation properties here (assuming properties with the same name exist in the DTO)
                    Id = iqc.Id,
                    GrinId = iqc.GrinId,
                    GrinNumber = iqc.GrinNumber,
                    Unit = iqc.Unit,
                    CreatedBy = iqc.CreatedBy,
                    CreatedOn = iqc.CreatedOn,
                    LastModifiedBy = iqc.LastModifiedBy,
                    LastModifiedOn = iqc.LastModifiedOn,
                    // ...

                    IQCConfirmationItems = iqc.IQCConfirmationItems.Select(item => new IQCConfirmationItemsReportDto
                    {
                        // Map IQCConfirmationItem properties here (assuming properties with the same name exist in the DTO)
                        Id = item.Id,
                        GrinPartId = item.GrinPartId,
                        ItemNumber = item.ItemNumber,
                        ReceivedQty = item.ReceivedQty,
                        GrinNumber = iqc.GrinNumber,
                        Remarks = item.Remarks,
                        MftrItemNumber = grinPartDetailsLookup[item.GrinPartId].MftrItemNumber,
                        PONumber = grinPartDetailsLookup[item.GrinPartId].PONumber,
                        ItemDescription = grinPartDetailsLookup[item.GrinPartId].ItemDescription,
                        ManufactureBatchNumber = grinPartDetailsLookup[item.GrinPartId].ManufactureBatchNumber,
                        UOM = grinPartDetailsLookup[item.GrinPartId].UOM,
                        ExpireDate = grinPartDetailsLookup[item.GrinPartId].ExpiryDate,
                        ManufactureDate = grinPartDetailsLookup[item.GrinPartId].ManufactureDate,

                    }).ToList(),
                }).ToList();

                serviceResponse.Data = iqcListDto;
                serviceResponse.Message = "Returned all IQCConfirmation";
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

        [HttpGet("{grinNumber}")]
        public async Task<IActionResult> GetIqcDetailsbyGrinNo(string grinNumber)
        {
            ServiceResponse<IEnumerable<IQCConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationDto>>();

            try
            {
                var iQCDetailsByGrinNo = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                if (iQCDetailsByGrinNo == null)
                {
                    _logger.LogError($"IQCConfirmation Details with GrinNumber: {grinNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation Details with GrinNumber hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return BadRequest(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned IQCConfirmation Details with id: {grinNumber}");
                    var result = _mapper.Map<IEnumerable<IQCConfirmationDto>>(iQCDetailsByGrinNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Successfully Returned IQCConfirmationbyGrinNo";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Created("IQCDetailsByGrinNo", serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside IQCConfirmationByGrinNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIqc(int id, [FromBody] IQCConfirmationUpdateDto IQCConfirmationUpdateDto)
        {
            ServiceResponse<IQCConfirmationUpdateDto> serviceResponse = new ServiceResponse<IQCConfirmationUpdateDto>();

            try
            {
                if (IQCConfirmationUpdateDto is null)
                {
                    _logger.LogError("IQCConfirmation details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update IQCConfirmation details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid IQCConfirmation details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var iQCUpdate = await _iQCConfirmationRepository.GetIqcDetailsbyId(id);
                if (iQCUpdate is null)
                {
                    _logger.LogError($"IQCConfirmation details with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                //var iQCConfirmationUpdate = _mapper.Map(IQCConfirmationUpdateDto, iQCUpdate);

                //string result = await _iQCConfirmationRepository.UpdateIqc(iQCConfirmationUpdate);


                var iqcItems = _mapper.Map<IEnumerable<IQCConfirmationItems>>(IQCConfirmationUpdateDto.IQCConfirmationItemsUpdateDtos);

                var iqcItemsList = _mapper.Map<IQCConfirmation>(IQCConfirmationUpdateDto);

                var iQCConfirmationItemsDtos = IQCConfirmationUpdateDto.IQCConfirmationItemsUpdateDtos;

                var IqcItemsList = new List<IQCConfirmationItems>();
                for (int i = 0; i < iQCConfirmationItemsDtos.Count; i++)
                {
                    IQCConfirmationItems iQCConfirmationItems = _mapper.Map<IQCConfirmationItems>(iQCConfirmationItemsDtos[i]);
                    IqcItemsList.Add(iQCConfirmationItems);


                }



                var data = _mapper.Map(IQCConfirmationUpdateDto, iQCUpdate);


                data.IQCConfirmationItems = iqcItems.ToList();

                string result = await _iQCConfirmationRepository.UpdateIqc(data);
                _logger.LogInfo(result);
                _iQCConfirmationRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "IQCConfirmation Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Update IQCConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
        //create Iqc


        [HttpPost]
        public async Task<IActionResult> CreateIqc([FromBody] IQCConfirmationPostDto iQCConfirmationPostDto)
        {
            ServiceResponse<IQCConfirmationDto> serviceResponse = new ServiceResponse<IQCConfirmationDto>();

            try
            {
                if (iQCConfirmationPostDto == null)
                {
                    _logger.LogError("IQCConfirmation details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmation details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid IQCConfirmation details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var iQCCreate = _mapper.Map<IQCConfirmation>(iQCConfirmationPostDto);
                var iQCDto = iQCConfirmationPostDto.IQCConfirmationItemsPostDtos;
                var iQcItemNo = iQCDto[0].ItemNumber;
                var iQCItemList = new List<IQCConfirmationItems>();

                for (int i = 0; i < iQCDto.Count; i++)
                {
                    IQCConfirmationItems iQCConfirmationItems = _mapper.Map<IQCConfirmationItems>(iQCDto[i]);
                    var grinPartId = iQCDto[i].GrinPartId;

                    var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartId);
                    if (grinPartsDetails == null)
                    {
                        _logger.LogError($"Invalid Grin Part Id {grinPartId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid Grin Part Id {grinPartId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }
                    if (grinPartsDetails.Qty <= (iQCConfirmationItems.AcceptedQty + iQCConfirmationItems.RejectedQty))
                    {
                        grinPartsDetails.AcceptedQty = iQCConfirmationItems.AcceptedQty;
                        grinPartsDetails.RejectedQty = iQCConfirmationItems.RejectedQty;
                        _grinPartsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError("Grinpart Quantity should not be lesser than accepted + rejected Quantity .");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Grinpart Quantity should not be lesser than accepted + rejected Quantity ";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }
                    iQCItemList.Add(iQCConfirmationItems);

                    var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                          "GetItemMasterByItemNumber?", "&ItemNumber=", iQcItemNo));
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;
                 if (iQCDto[i].RejectedQty!=0)
                 { 
                    IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                    grinInventoryDto.PartNumber = iQCConfirmationItems.ItemNumber;
                    grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                    grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                    grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                    grinInventoryDto.ProjectNumber = "";
                    grinInventoryDto.Balance_Quantity = Convert.ToDecimal(iQCConfirmationItems.RejectedQty);
                    grinInventoryDto.UOM = grinPartsDetails.UOM;
                    grinInventoryDto.Warehouse = "Reject";
                    grinInventoryDto.Location = "Reject";
                    grinInventoryDto.GrinNo = iQCCreate.GrinNumber;
                    grinInventoryDto.GrinPartId = iQCConfirmationItems.GrinPartId;
                    grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
                    grinInventoryDto.ReferenceID = "GRIN"; // Convert.ToString(iQCConfirmationItems.Id) //;
                    grinInventoryDto.ReferenceIDFrom = "GRIN";
                    grinInventoryDto.GrinMaterialType = "GRIN";
                    grinInventoryDto.ShopOrderNo = "";

                    var httpClientHandler = new HttpClientHandler();
                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                    var httpClient = new HttpClient(httpClientHandler);
                    string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                    var rfqApiUrl = _config["InventoryAPI"];
                    var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                    var rfqCustomerIdResponse = await _httpClient.PostAsync($"{rfqApiUrl}CreateInventoryFromGrin", content);
                 }
                    decimal acceptedQty = iQCDto[i].AcceptedQty;
                    foreach (var projectNo in grinPartsDetails.ProjectNumbers)
                    {
                        var grinNo = iQCCreate.GrinNumber;
                        var grinPartsId = projectNo.GrinPartsId;
                        var itemNo = iQCDto[i].ItemNumber;
                        var projectNos = projectNo.ProjectNumber;
                        var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                            "GetInventoryDetailsByGrinNoandGrinId?", "GrinNo=", grinNo, "&GrinPartsId=",
                            grinPartsId, "&ItemNumber=", itemNo, "&ProjectNumber=", projectNos));
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
                            var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                                "UpdateInventory?id=", inventoryObject.id), data);
                        }

                        ////InventoryTranction Update Code

                        //decimal acceptedQuantity = iQCDto[i].AcceptedQty;
                        //var inventoryTranctionObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryTranctionAPI"],
                        //    "GetInventoryTranctionDetailsByGrinNoandGrinId?", "GrinNo=", grinNo, "&GrinPartsId=",
                        //    grinPartsId, "&ItemNumber=", itemNo, "&ProjectNumber=", projectNos));
                        //var inventoryTranctionObjectString = await inventoryTranctionObjectResult.Content.ReadAsStringAsync();
                        //dynamic inventoryTranctionObjectData = JsonConvert.DeserializeObject(inventoryTranctionObjectString);
                        //dynamic inventoryTranctionObject = inventoryTranctionObjectData.data;
                        //if (inventoryTranctionObject != null)
                        //{
                        //    decimal balanceQty = inventoryTranctionObject.issued_Quantity;

                        //    if (inventoryTranctionObject.issued_Quantity <= acceptedQuantity && inventoryTranctionObject.issued_Quantity != 0)
                        //    {
                        //        inventoryTranctionObject.warehouse = "IQC";
                        //        inventoryTranctionObject.from_Location = "IQC";
                        //        inventoryTranctionObject.tO_Location = "IQC";
                        //        inventoryTranctionObject.referenceIDFrom = "GRIN";
                        //        acceptedQuantity -= balanceQty;

                        //    }
                        //    else if (inventoryTranctionObject.issued_Quantity > acceptedQuantity)
                        //    {
                        //        if (acceptedQuantity == 0)
                        //        {
                        //            inventoryTranctionObject.issued_Quantity = acceptedQuantity;
                        //            inventoryTranctionObject.warehouse = "IQC";
                        //            inventoryTranctionObject.from_Location = "IQC";
                        //            inventoryTranctionObject.tO_Location = "IQC";
                        //            inventoryTranctionObject.referenceIDFrom = "GRIN";
                        //            inventoryTranctionObject.isStockAvailable = false;
                        //        }
                        //        else
                        //        {
                        //            inventoryTranctionObject.issued_Quantity = acceptedQuantity;
                        //            inventoryTranctionObject.warehouse = "IQC";
                        //            inventoryTranctionObject.from_Location = "IQC";
                        //            inventoryTranctionObject.tO_Location = "IQC";
                        //            inventoryTranctionObject.referenceIDFrom = "GRIN";
                        //            acceptedQuantity = 0;
                        //        }
                        //    }

                        //    var json = JsonConvert.SerializeObject(inventoryTranctionObject);
                        //    var data = new StringContent(json, Encoding.UTF8, "application/json");
                        //    var response = await _httpClient.PutAsync(string.Concat(_config["InventoryTranctionAPI"],
                        //        "UpdateInventoryTranction?id=", inventoryTranctionObject.id), data);
                        //}
                        //else
                        //{
                        //    IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                        //    iqcInventoryTranctionDto.PartNumber = grinPartsDetails.ItemNumber;
                        //    iqcInventoryTranctionDto.LotNumber = grinPartsDetails.LotNumber;
                        //    iqcInventoryTranctionDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                        //    iqcInventoryTranctionDto.Description = grinPartsDetails.ItemDescription;
                        //    iqcInventoryTranctionDto.ProjectNumber = projectNo.ProjectNumber;
                        //    iqcInventoryTranctionDto.Issued_Quantity = Convert.ToDecimal(projectNo.ProjectQty);
                        //    iqcInventoryTranctionDto.UOM = grinPartsDetails.UOM;
                        //    iqcInventoryTranctionDto.Warehouse = "IQC";
                        //    iqcInventoryTranctionDto.From_Location = "IQC";
                        //    iqcInventoryTranctionDto.TO_Location = "IQC";
                        //    iqcInventoryTranctionDto.GrinNo = grinNo;
                        //    iqcInventoryTranctionDto.GrinPartId = grinPartsDetails.Id;
                        //    iqcInventoryTranctionDto.PartType = grinPartsDetails.ItemType;  //We need to check this
                        //    iqcInventoryTranctionDto.ReferenceID = Convert.ToString(grinPartsDetails.Id);
                        //    iqcInventoryTranctionDto.ReferenceIDFrom = "GRIN";
                        //    iqcInventoryTranctionDto.GrinMaterialType = "";
                        //    iqcInventoryTranctionDto.ShopOrderNo = "";
                        //    iqcInventoryTranctionDto.IsStockAvailable = true;

                        //    var json = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                        //    var data = new StringContent(json, Encoding.UTF8, "application/json");

                        //    var response = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), data);

                        //}
                    }

                    ////update accepted qty and rejected qty in grin model

                    var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinPartsQty(iQCConfirmationItems.GrinPartId, iQCConfirmationItems.AcceptedQty.ToString(), iQCConfirmationItems.RejectedQty.ToString());

                    var iQCCreates = _mapper.Map<GrinParts>(updatedGrinPartsQty);

                    string result = await _grinPartsRepository.UpdateGrinQty(iQCCreates);

                    _grinPartsRepository.SaveAsync();
                }

                iQCCreate.IQCConfirmationItems = iQCItemList;
                iQCCreate.IsIqcCompleted = true;
                await _iQCConfirmationRepository.CreateIqc(iQCCreate);
                _iQCConfirmationRepository.SaveAsync();

                //Updating IQC Status in Grin
                var grinNumber = iQCCreate.GrinNumber;
                var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                grinDetails.IsIqcCompleted = true;
                await _grinRepository.UpdateGrin(grinDetails);
                _grinRepository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "IQCConfirmation Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("IQCConfirmationById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Create IQCConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);


            }
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateIqc([FromBody] IQCConfirmationPostDto iQCConfirmationPostDto)
        //{
        //    ServiceResponse<IQCConfirmationDto> serviceResponse = new ServiceResponse<IQCConfirmationDto>();

        //    try
        //    {
        //        if (iQCConfirmationPostDto == null)
        //        {
        //            _logger.LogError("IQCConfirmation details object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "IQCConfirmation details object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest();
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid IQCConfirmation details object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid model object";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }

        //        var iQCCreate = _mapper.Map<IQCConfirmation>(iQCConfirmationPostDto);
        //        var iQCDto = iQCConfirmationPostDto.IQCConfirmationItemsPostDtos;
        //        var iQcItemNo = iQCDto[0].ItemNumber;
        //        var iQCItemList = new List<IQCConfirmationItems>();

        //        for(int  i=0; i< iQCDto.Count;i++)
        //        {
        //            IQCConfirmationItems iQCConfirmationItems = _mapper.Map<IQCConfirmationItems>(iQCDto[i]);
        //            var grinPartId = iQCDto[i].GrinPartId;

        //            var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartId);
        //            if (grinPartsDetails == null)
        //            {
        //                _logger.LogError($"Invalid Grin Part Id {grinPartId}");
        //                serviceResponse.Data = null;
        //                serviceResponse.Message = $"Invalid Grin Part Id {grinPartId}";
        //                serviceResponse.Success = false;
        //                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //                return BadRequest(serviceResponse);
        //            }
        //            if (grinPartsDetails.Qty <= (iQCConfirmationItems.AcceptedQty + iQCConfirmationItems.RejectedQty))
        //            {
        //                grinPartsDetails.AcceptedQty = iQCConfirmationItems.AcceptedQty;
        //                grinPartsDetails.RejectedQty = iQCConfirmationItems.RejectedQty;                      
        //                _grinPartsRepository.SaveAsync();
        //            }
        //            else
        //            {
        //                _logger.LogError("Grinpart Quantity should not be lesser than accepted + rejected Quantity .");
        //                serviceResponse.Data = null;
        //                serviceResponse.Message = "Grinpart Quantity should not be lesser than accepted + rejected Quantity ";
        //                serviceResponse.Success = false;
        //                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //                return BadRequest(serviceResponse);
        //            }
        //            iQCItemList.Add(iQCConfirmationItems);


        //            var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
        //                  "GetItemMasterByItemNumber?", "&ItemNumber=", iQcItemNo));
        //            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
        //            dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
        //            dynamic itemMasterObject = itemMasterObjectData.data;

        //            IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
        //            grinInventoryDto.PartNumber = iQCConfirmationItems.ItemNumber;
        //            grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
        //            grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
        //            grinInventoryDto.Description = grinPartsDetails.ItemDescription;
        //            grinInventoryDto.ProjectNumber = grinPartsDetails.ProjectNumbers[i].ProjectNumber;
        //            grinInventoryDto.Balance_Quantity = Convert.ToDecimal(iQCConfirmationItems.RejectedQty);
        //            grinInventoryDto.UOM = grinPartsDetails.UOM;
        //            grinInventoryDto.Warehouse = "Reject";
        //            grinInventoryDto.Location = "Reject";
        //            grinInventoryDto.GrinNo = iQCCreate.GrinNumber;
        //            grinInventoryDto.GrinPartId = iQCConfirmationItems.GrinPartId;
        //            grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
        //            grinInventoryDto.ReferenceID = "GRIN"; /*/ Convert.ToString(iQCConfirmationItems.Id) /;*/
        //            grinInventoryDto.ReferenceIDFrom = "GRIN";
        //            grinInventoryDto.GrinMaterialType = "GRIN";
        //            grinInventoryDto.ShopOrderNo = "";

        //            var jsons = JsonConvert.SerializeObject(grinInventoryDto);
        //            var datas = new StringContent(jsons, Encoding.UTF8, "application/json");

        //            //Inventory Update Code
        //            var tokenValue = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
        //            if (!string.IsNullOrEmpty(tokenValue) && tokenValue.StartsWith("Bearer "))
        //            {
        //                var token = tokenValue.Substring(7);
        //                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //            }
        //            var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventoryFromGrin"), datas);
        //            decimal acceptedQty = iQCDto[i].AcceptedQty;
        //            foreach (var projectNo in grinPartsDetails.ProjectNumbers)
        //            {
        //                var grinNo = iQCCreate.GrinNumber;
        //                var grinPartsId = projectNo.GrinPartsId;
        //                var itemNo = iQCDto[i].ItemNumber;
        //                var projectNos = projectNo.ProjectNumber;
        //                var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
        //                    "GetInventoryDetailsByGrinNoandGrinId?", "GrinNo=", grinNo, "&GrinPartsId=",
        //                    grinPartsId, "&ItemNumber=", itemNo, "&ProjectNumber=", projectNos));
        //                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
        //                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
        //                dynamic inventoryObject = inventoryObjectData.data;
        //                if(inventoryObject !=null)
        //                {
        //                    decimal balanceQty = inventoryObject.balance_Quantity;

        //                    if (inventoryObject.balance_Quantity <= acceptedQty && inventoryObject.balance_Quantity != 0)
        //                    {
        //                        inventoryObject.warehouse = "IQC";
        //                        inventoryObject.location = "IQC";
        //                        inventoryObject.referenceIDFrom = "GRIN";
        //                        acceptedQty -= balanceQty;

        //                    }
        //                    else if (inventoryObject.balance_Quantity > acceptedQty)
        //                    {
        //                        if (acceptedQty == 0)
        //                        {
        //                            inventoryObject.balance_Quantity = acceptedQty;
        //                            inventoryObject.warehouse = "IQC";
        //                            inventoryObject.location = "IQC";
        //                            inventoryObject.referenceIDFrom = "GRIN";
        //                            inventoryObject.isStockAvailable = false;
        //                        }
        //                        else
        //                        {
        //                            inventoryObject.balance_Quantity = acceptedQty;
        //                            inventoryObject.warehouse = "IQC";
        //                            inventoryObject.location = "IQC";
        //                            inventoryObject.referenceIDFrom = "GRIN";
        //                            acceptedQty = 0;
        //                        }
        //                    }

        //                    var json = JsonConvert.SerializeObject(inventoryObject);
        //                    var data = new StringContent(json, Encoding.UTF8, "application/json");
        //                    var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
        //                        "UpdateInventory?id=", inventoryObject.id), data);
        //                }

        //            }

        //            ////update accepted qty and rejected qty in grin model

        //            var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinPartsQty(iQCConfirmationItems.GrinPartId, iQCConfirmationItems.AcceptedQty.ToString(), iQCConfirmationItems.RejectedQty.ToString());

        //            var iQCCreates = _mapper.Map<GrinParts>(updatedGrinPartsQty);

        //            string result = await _grinPartsRepository.UpdateGrinQty(iQCCreates);

        //            _grinPartsRepository.SaveAsync();
        //        }

        //        iQCCreate.IQCConfirmationItems = iQCItemList;
        //        iQCCreate.IsIqcCompleted = true;
        //        await _iQCConfirmationRepository.CreateIqc(iQCCreate);
        //        _iQCConfirmationRepository.SaveAsync();

        //        //Updating IQC Status in Grin
        //        var grinNumber = iQCCreate.GrinNumber;
        //        var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
        //        grinDetails.IsIqcCompleted = true;
        //        await _grinRepository.UpdateGrin(grinDetails);
        //        _grinRepository.SaveAsync();

        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "IQCConfirmation Successfully Created";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Created("IQCConfirmationById", serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside Create IQCConfirmation action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);


        //    }
        //}

        //check and enble the below code


        //[HttpPost]
        //public async Task<IActionResult> SaveMultipleIqc([FromBody] List<IQCConfirmationPostDto> iQCConfirmationPostDtos)
        //{
        //    ServiceResponse<IQCConfirmationPostDto> serviceResponse = new ServiceResponse<IQCConfirmationPostDto>();

        //    try
        //    {
        //        if (iQCConfirmationPostDtos == null)
        //        {
        //            _logger.LogError("IQCConfirmation details object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "IQCConfirmation details object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest();
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid IQCConfirmation details object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid model object";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }

        //        var iQCConfirmationList = _mapper.Map<List<IQCConfirmation>>(iQCConfirmationPostDtos);
        //        bool isAnyRecordCreated = false;
        //        foreach (var iQCDetails in iQCConfirmationList)
        //        {
        //            if (iQCDetails.AcceptedQty > 0 || iQCDetails.RejectedQty > 0)
        //            {
        //                await _iQCConfirmationRepository.Create(iQCDetails);
        //                isAnyRecordCreated = true;
        //            }
        //        }
        //        if (isAnyRecordCreated)
        //        {
        //            _iQCConfirmationRepository.SaveAsync();
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Successfully Created";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Created("IQCConfirmationById", serviceResponse);
        //        }
        //        else
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Nothing to save,Because accepted or rejected quantity is not greater than 0 in any rows !";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Created("IQCConfirmationById", serviceResponse);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside Create IQCConfirmation action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, "Internal server error");


        //    }
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIqcDetailsbyId(int id)
        {
            ServiceResponse<IQCConfirmationDto> serviceResponse = new ServiceResponse<IQCConfirmationDto>();

            try
            {
                var iQCDetailsbyId = await _iQCConfirmationRepository.GetIqcDetailsbyId(id);
                if (iQCDetailsbyId == null)
                {
                    _logger.LogError($"IQCConfirmation details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation details with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned IQCConfirmation details with id: {id}");
                    List<IQCConfirmationItemsDto> iQCConfirmationItemsList = new List<IQCConfirmationItemsDto>();
                    var iQcGrinNo = iQCDetailsbyId.GrinNumber;
                    var grinDetailsbyGrinNo = await _grinRepository.GetGrinByGrinNo(iQcGrinNo);
                    
                    var iQCConformationDetailsDto = _mapper.Map<IQCConfirmationDto>(grinDetailsbyGrinNo);
                    iQCConformationDetailsDto.Id = id;
                    iQCConformationDetailsDto.GrinId = iQCConformationDetailsDto.Id;
                    if (grinDetailsbyGrinNo.GrinParts.Count() != 0)
                    {
                        foreach (var grinDetails in grinDetailsbyGrinNo.GrinParts)
                        {
                            IQCConfirmationItemsDto iQCConfirmationItemsDtos = _mapper.Map<IQCConfirmationItemsDto>(grinDetails);
                            iQCConfirmationItemsDtos.Id = iQCConfirmationItemsDtos.Id;
                            iQCConfirmationItemsDtos.ReceivedQty = grinDetails.Qty;
                            iQCConfirmationItemsDtos.GrinPartId = grinDetails.Id; 
                            iQCConfirmationItemsList.Add(iQCConfirmationItemsDtos);
                        }
                    }
                    iQCConformationDetailsDto.IQCConfirmationItems = iQCConfirmationItemsList;
                    serviceResponse.Data = iQCConformationDetailsDto;
                    serviceResponse.Message = "IQCConfirmationById Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside IQCConfirmationById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIQC(int id)
        {
            ServiceResponse<IQCConfirmationDto> serviceResponse = new ServiceResponse<IQCConfirmationDto>();

            try
            {
                var iQCDetailById = await _iQCConfirmationRepository.GetIqcDetailsbyId(id);
                if (iQCDetailById == null)
                {
                    _logger.LogError($"IQCDelete with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCDelete with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                //iQCDetailById.IsDeleted = true;
                string result = await _iQCConfirmationRepository.UpdateIqc(iQCDetailById);
                serviceResponse.Message = "IQCConfirmation Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Delete IQCConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllIQCConfirmationItems()
        {
            ServiceResponse<IEnumerable<IQCConfirmationItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationItemsDto>>();

            try
            {
                var getAllIQCItemDetails = await _iQCConfirmationItemsRepository.GetAllIQCConfirmationItems();
                _logger.LogInfo("Returned all IQCConfirmationItems details()s");
                var result = _mapper.Map<IEnumerable<IQCConfirmationItemsDto>>(getAllIQCItemDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all IQCConfirmationItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveIQCConfirmationIdNameList()
        {
            ServiceResponse<IEnumerable<IQCConfirmationIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationIdNameListDto>>();
            try
            {
                var listOfActiveIQCConfirmationName = await _iQCConfirmationRepository.GetAllActiveIQCConfirmationNameList();

                var result = _mapper.Map<IEnumerable<IQCConfirmationIdNameListDto>>(listOfActiveIQCConfirmationName);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveIQCConfirmationIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveIQCConfirmationIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateIQCConfirmationItems([FromBody] IQCConfirmationSaveDto iqcConfirmationSaveDto)
        {
            ServiceResponse<IQCConfirmationSaveDto> serviceResponse = new ServiceResponse<IQCConfirmationSaveDto>();

            try
            {
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
                var iqcConfirmationItemsDto = iqcConfirmationSaveDto.IQCConfirmationItemsPostDtos;
                var iqcConfirmationItems = _mapper.Map<IQCConfirmationItems>(iqcConfirmationItemsDto);
                var grinNumber = iqcConfirmation.GrinNumber;
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
                    if (grinPartsDetails.Qty <= (iqcConfirmationItems.AcceptedQty + iqcConfirmationItems.RejectedQty))
                    {
                        grinPartsDetails.AcceptedQty = iqcConfirmationItems.AcceptedQty;
                        grinPartsDetails.RejectedQty = iqcConfirmationItems.RejectedQty;
                        _grinPartsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError("Grinpart Quantity should not be lesser than accepted + rejected Quantity .");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Grinpart Quantity should not be lesser than accepted + rejected Quantity ";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    iqcConfirmationItems.IsIqcCompleted = true;
                    await _iQCConfirmationItemsRepository.CreateIqcItem(iqcConfirmationItems);
                    _iQCConfirmationItemsRepository.SaveAsync();

                    //Updating IQC Status in GrinParts

                    grinPartsDetails.IsIqcCompleted = true;
                    await _grinPartsRepository.UpdateGrinQty(grinPartsDetails);
                    _grinPartsRepository.SaveAsync();

                    //Updating IQC Status in Grin

                    var grinPartsIqcStatuscount = await _grinPartsRepository.GetGrinPartsIqcStatusCount(grinPartsDetails.GrinsId);

                    if(grinPartsIqcStatuscount == 0)
                    {
                        var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                        grinDetails.IsIqcCompleted = true;
                        await _grinRepository.UpdateGrin(grinDetails);
                        _grinRepository.SaveAsync();
                    }

                    //Updating IQC Status in IQC

                    var grinIqcStatuscount = await _grinRepository.GetGrinIqcStatusCount(grinNumber);

                    if (grinIqcStatuscount > 0)
                    {
                        var iqcDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                        iqcDetails.IsIqcCompleted = true;
                        await _iQCConfirmationRepository.UpdateIqc(iqcDetails);
                        _iQCConfirmationRepository.SaveAsync();
                    }


                    //Inventory Update Code
                    decimal acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinPartId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.ProjectNumbers)
                    {
                        var grinNo = iqcConfirmation.GrinNumber;
                        var grinPartsIds = projectNo.GrinPartsId;
                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var projectNos = projectNo.ProjectNumber;
                        var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                            "GetInventoryDetailsByGrinNoandGrinId?", "GrinNo=", grinNo, "&GrinPartsId=",
                            grinPartsId, "&ItemNumber=", itemNo, "&ProjectNumber=", projectNos));
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
                            var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                                "UpdateInventory?id=", inventoryObject.id), data);
                        }

                        //InventoryTranction Update Code

                        decimal acceptedQuantity = iqcConfirmationItemsDto.AcceptedQty;
                        var inventoryTranctionObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryTranctionAPI"],
                            "GetInventoryTranctionDetailsByGrinNoandGrinId?", "GrinNo=", grinNo, "&GrinPartsId=",
                            grinPartsId, "&ItemNumber=", itemNo, "&ProjectNumber=", projectNos));
                        var inventoryTranctionObjectString = await inventoryTranctionObjectResult.Content.ReadAsStringAsync();
                        dynamic inventoryTranctionObjectData = JsonConvert.DeserializeObject(inventoryTranctionObjectString);
                        dynamic inventoryTranctionObject = inventoryTranctionObjectData.data;
                        if (inventoryTranctionObject != null)
                        {
                            decimal balanceQty = inventoryTranctionObject.issued_Quantity;

                            if (inventoryTranctionObject.issued_Quantity <= acceptedQuantity && inventoryTranctionObject.issued_Quantity != 0)
                            {
                                inventoryTranctionObject.warehouse = "IQC";
                                inventoryTranctionObject.from_Location = "IQC";
                                inventoryTranctionObject.tO_Location = "IQC";
                                inventoryTranctionObject.referenceIDFrom = "GRIN";
                                acceptedQuantity -= balanceQty;

                            }
                            else if (inventoryTranctionObject.issued_Quantity > acceptedQuantity)
                            {
                                if (acceptedQuantity == 0)
                                {
                                    inventoryTranctionObject.issued_Quantity = acceptedQuantity;
                                    inventoryTranctionObject.warehouse = "IQC";
                                    inventoryTranctionObject.from_Location = "IQC";
                                    inventoryTranctionObject.tO_Location = "IQC";
                                    inventoryTranctionObject.referenceIDFrom = "GRIN";
                                    inventoryTranctionObject.isStockAvailable = false;
                                }
                                else
                                {
                                    inventoryTranctionObject.issued_Quantity = acceptedQuantity;
                                    inventoryTranctionObject.warehouse = "IQC";
                                    inventoryTranctionObject.from_Location = "IQC";
                                    inventoryTranctionObject.tO_Location = "IQC";
                                    inventoryTranctionObject.referenceIDFrom = "GRIN";
                                    acceptedQuantity = 0;
                                }
                            }

                            var json = JsonConvert.SerializeObject(inventoryTranctionObject);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await _httpClient.PutAsync(string.Concat(_config["InventoryTranctionAPI"],
                                "UpdateInventoryTranction?id=", inventoryTranctionObject.id), data);
                        }
                        else
                        {
                            IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                            iqcInventoryTranctionDto.PartNumber = grinPartsDetails.ItemNumber;
                            iqcInventoryTranctionDto.LotNumber = grinPartsDetails.LotNumber;
                            iqcInventoryTranctionDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                            iqcInventoryTranctionDto.Description = grinPartsDetails.ItemDescription;
                            iqcInventoryTranctionDto.ProjectNumber = projectNo.ProjectNumber;
                            iqcInventoryTranctionDto.Issued_Quantity = Convert.ToDecimal(projectNo.ProjectQty);
                            iqcInventoryTranctionDto.UOM = grinPartsDetails.UOM;
                            iqcInventoryTranctionDto.Warehouse = "IQC";
                            iqcInventoryTranctionDto.From_Location = "IQC";
                            iqcInventoryTranctionDto.TO_Location = "IQC";
                            iqcInventoryTranctionDto.GrinNo = grinNo;
                            iqcInventoryTranctionDto.GrinPartId = grinPartsDetails.Id;
                            iqcInventoryTranctionDto.PartType = grinPartsDetails.ItemType;  //We need to check this
                            iqcInventoryTranctionDto.ReferenceID = Convert.ToString(grinPartsDetails.Id);
                            iqcInventoryTranctionDto.ReferenceIDFrom = "GRIN";
                            iqcInventoryTranctionDto.GrinMaterialType = "";
                            iqcInventoryTranctionDto.ShopOrderNo = "";
                            iqcInventoryTranctionDto.IsStockAvailable = true;

                            var json = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            // Include the token in the Authorization header
                            var tokenValues = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                            if (!string.IsNullOrEmpty(tokenValues) && tokenValues.StartsWith("Bearer "))
                            {
                                var token = tokenValues.Substring(7);
                                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            }
                            var response = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), data);

                        }
                    }

                    ////update accepted qty and rejected qty in grin model

                    var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinPartsQty(iqcConfirmationItems.GrinPartId, iqcConfirmationItems.AcceptedQty.ToString(), iqcConfirmationItems.RejectedQty.ToString());

                    var iQCCreates = _mapper.Map<GrinParts>(updatedGrinPartsQty);

                    string result = await _grinPartsRepository.UpdateGrinQty(iQCCreates);

                    _grinPartsRepository.SaveAsync();

                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmationItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
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
                    if (grinPartsDetails.Qty <= (iqcConfirmationItems.AcceptedQty + iqcConfirmationItems.RejectedQty))
                    {
                        grinPartsDetails.AcceptedQty = iqcConfirmationItems.AcceptedQty;
                        grinPartsDetails.RejectedQty = iqcConfirmationItems.RejectedQty; 
                        _grinPartsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError("Grinpart Quantity should not be lesser than accepted + rejected Quantity .");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Grinpart Quantity should not be lesser than accepted + rejected Quantity ";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    iqcConfirmationItems.IsIqcCompleted = true;
                    iqcConfirmation.IQCConfirmationItems = new List<IQCConfirmationItems> { iqcConfirmationItems };
                    await _iQCConfirmationRepository.CreateIqc(iqcConfirmation);
                    _iQCConfirmationRepository.SaveAsync();

                    //Updating IQC Status in GrinParts

                    grinPartsDetails.IsIqcCompleted = true;
                    await _grinPartsRepository.UpdateGrinQty(grinPartsDetails);
                    _grinPartsRepository.SaveAsync();

                    //Inventory Update Code
                    decimal acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinPartId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.ProjectNumbers)
                    {
                        var grinNo = iqcConfirmation.GrinNumber;
                        var grinPartsIds = projectNo.GrinPartsId;
                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var projectNos = projectNo.ProjectNumber;
                        var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                            "GetInventoryDetailsByGrinNoandGrinId?", "GrinNo=", grinNo, "&GrinPartsId=",
                            grinPartsId, "&ItemNumber=", itemNo, "&ProjectNumber=", projectNos));
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
                            var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                                "UpdateInventory?id=", inventoryObject.id), data);
                        }

                        //InventoryTranction Update Code

                        decimal acceptedQuantity = iqcConfirmationItemsDto.AcceptedQty;
                        var inventoryTranctionObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryTranctionAPI"],
                            "GetInventoryTranctionDetailsByGrinNoandGrinId?", "GrinNo=", grinNo, "&GrinPartsId=",
                            grinPartsId, "&ItemNumber=", itemNo, "&ProjectNumber=", projectNos));
                        var inventoryTranctionObjectString = await inventoryTranctionObjectResult.Content.ReadAsStringAsync();
                        dynamic inventoryTranctionObjectData = JsonConvert.DeserializeObject(inventoryTranctionObjectString);
                        dynamic inventoryTranctionObject = inventoryTranctionObjectData.data;
                        if (inventoryTranctionObject != null)
                        {
                            decimal balanceQty = inventoryTranctionObject.issued_Quantity;

                            if (inventoryTranctionObject.issued_Quantity <= acceptedQuantity && inventoryTranctionObject.issued_Quantity != 0)
                            {
                                inventoryTranctionObject.warehouse = "IQC";
                                inventoryTranctionObject.from_Location = "IQC";
                                inventoryTranctionObject.tO_Location = "IQC";
                                inventoryTranctionObject.referenceIDFrom = "GRIN";
                                acceptedQuantity -= balanceQty;

                            }
                            else if (inventoryTranctionObject.issued_Quantity > acceptedQuantity)
                            {
                                if (acceptedQuantity == 0)
                                {
                                    inventoryTranctionObject.issued_Quantity = acceptedQuantity;
                                    inventoryTranctionObject.warehouse = "IQC";
                                    inventoryTranctionObject.from_Location = "IQC";
                                    inventoryTranctionObject.tO_Location = "IQC";
                                    inventoryTranctionObject.referenceIDFrom = "GRIN";
                                    inventoryTranctionObject.isStockAvailable = false;
                                }
                                else
                                {
                                    inventoryTranctionObject.issued_Quantity = acceptedQuantity;
                                    inventoryTranctionObject.warehouse = "IQC";
                                    inventoryTranctionObject.from_Location = "IQC";
                                    inventoryTranctionObject.tO_Location = "IQC";
                                    inventoryTranctionObject.referenceIDFrom = "GRIN";
                                    acceptedQuantity = 0;
                                }
                            }

                            var json = JsonConvert.SerializeObject(inventoryTranctionObject);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await _httpClient.PutAsync(string.Concat(_config["InventoryTranctionAPI"],
                                "UpdateInventoryTranction?id=", inventoryTranctionObject.id), data);
                        }
                        else
                        {
                            IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                            iqcInventoryTranctionDto.PartNumber = grinPartsDetails.ItemNumber;
                            iqcInventoryTranctionDto.LotNumber = grinPartsDetails.LotNumber;
                            iqcInventoryTranctionDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                            iqcInventoryTranctionDto.Description = grinPartsDetails.ItemDescription;
                            iqcInventoryTranctionDto.ProjectNumber = projectNo.ProjectNumber;
                            iqcInventoryTranctionDto.Issued_Quantity = Convert.ToDecimal(projectNo.ProjectQty);
                            iqcInventoryTranctionDto.UOM = grinPartsDetails.UOM;
                            iqcInventoryTranctionDto.Warehouse = "IQC";
                            iqcInventoryTranctionDto.From_Location = "IQC";
                            iqcInventoryTranctionDto.TO_Location = "IQC";
                            iqcInventoryTranctionDto.GrinNo = grinNo;
                            iqcInventoryTranctionDto.GrinPartId = grinPartsDetails.Id;
                            iqcInventoryTranctionDto.PartType = grinPartsDetails.ItemType;  //We need to check this
                            iqcInventoryTranctionDto.ReferenceID = Convert.ToString(grinPartsDetails.Id);
                            iqcInventoryTranctionDto.ReferenceIDFrom = "GRIN";
                            iqcInventoryTranctionDto.GrinMaterialType = "";
                            iqcInventoryTranctionDto.ShopOrderNo = "";
                            iqcInventoryTranctionDto.IsStockAvailable = true;

                            var json = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            // Include the token in the Authorization header
                            var tokenValues = _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                            if (!string.IsNullOrEmpty(tokenValues) && tokenValues.StartsWith("Bearer "))
                            {
                                var token = tokenValues.Substring(7);
                                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            }
                            var response = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), data);

                        }
                    }

                    ////update accepted qty and rejected qty in grin model

                    var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinPartsQty(iqcConfirmationItems.GrinPartId, iqcConfirmationItems.AcceptedQty.ToString(), iqcConfirmationItems.RejectedQty.ToString());

                    var iQCCreates = _mapper.Map<GrinParts>(updatedGrinPartsQty);

                    string result = await _grinPartsRepository.UpdateGrinQty(iQCCreates);

                    _grinPartsRepository.SaveAsync();

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

    }
}


