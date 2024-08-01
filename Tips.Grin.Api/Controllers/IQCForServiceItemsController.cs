using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Repository;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class IQCForServiceItemsController : ControllerBase
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IIQCForServiceItemsRepository _iQCForServiceItemsRepository;
        private IIQCForServiceItems_ItemsRepository _iQCForServiceItems_ItemsRepository;
        private IGrinsForServiceItemsPartsRepository _grinPartsRepository;
        private IGrinsForServiceItemsRepository _grinsForServiceItemsRepository;
        public IQCForServiceItemsController(IHttpClientFactory clientFactory, IGrinsForServiceItemsRepository grinsForServiceItemsRepository, IIQCForServiceItems_ItemsRepository iQCForServiceItems_ItemsRepository, IGrinsForServiceItemsPartsRepository grinPartsRepository, IIQCForServiceItemsRepository iQCForServiceItemsRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _iQCForServiceItemsRepository = iQCForServiceItemsRepository;
            _grinPartsRepository = grinPartsRepository;
            _iQCForServiceItems_ItemsRepository = iQCForServiceItems_ItemsRepository;
            _grinsForServiceItemsRepository = grinsForServiceItemsRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllIQCForServiceItemsDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<IQCForServiceItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCForServiceItemsDto>>();
            try
            {
                var getAllIQCDetails = await _iQCForServiceItemsRepository.GetAllIQCForServiceItemsDetails(pagingParameter, searchParams);
                if (getAllIQCDetails == null || getAllIQCDetails.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCForServiceItems data not found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCForServiceItems data not found in db");
                    return NotFound(serviceResponse);
                }
                var metadata = new
                {
                    getAllIQCDetails.TotalCount,
                    getAllIQCDetails.PageSize,
                    getAllIQCDetails.CurrentPage,
                    getAllIQCDetails.HasNext,
                    getAllIQCDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all IQCForServiceItems details()s");
                var result = _mapper.Map<IEnumerable<IQCForServiceItemsDto>>(getAllIQCDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all IQCForServiceItems";
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
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetIQCForServiceItemsSPReportWithParam([FromBody] IQCForServiceItemsReportWithParamDto iQCForServiceItemsReportWithParamDto)
        {
            ServiceResponse<IEnumerable<IQCForServiceItemsSPReport>> serviceResponse = new ServiceResponse<IEnumerable<IQCForServiceItemsSPReport>>();
            try
            {
                var products = await _iQCForServiceItemsRepository.GetIQCForServiceItemsSPReportWithParam(iQCForServiceItemsReportWithParamDto.GrinsForServiceItemsNumber,
                                                                                                                    iQCForServiceItemsReportWithParamDto.ItemNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCForServiceItems hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCForServiceItems hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned IQCForServiceItems Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetIQCForServiceItemsSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetIQCForServiceItemsSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<IQCForServiceItemsSPReport>> serviceResponse = new ServiceResponse<IEnumerable<IQCForServiceItemsSPReport>>();
            try
            {
                var products = await _iQCForServiceItemsRepository.GetIQCForServiceItemsSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCForServiceItems hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"IQCForServiceItems hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned IQCForServiceItems Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetIQCForServiceItemsSPReportWithDate action";
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
        public async Task<IActionResult> CreateIQCForServiceItems([FromBody] IQCForServiceItemsPostDto iQCForServiceItemsPostDto)
        {
            ServiceResponse<IQCForServiceItemsDto> serviceResponse = new ServiceResponse<IQCForServiceItemsDto>();

            try
            {
                string serverKey = GetServerKey();

                if (iQCForServiceItemsPostDto == null)
                {
                    _logger.LogError("IQCForServiceItems details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCForServiceItems details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid IQCForServiceItems details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var iQCCreate = _mapper.Map<IQCForServiceItems>(iQCForServiceItemsPostDto);
                var grinNumber = iQCCreate.GrinsForServiceItemsNumber;
                var iQCDto = iQCForServiceItemsPostDto.IQCForServiceItems_Items;

                var iQcItemNo = iQCDto[0].ItemNumber;
                var iQCItemList = new List<IQCForServiceItems_Items>();

                HttpStatusCode getItemmResp = HttpStatusCode.OK;
                HttpStatusCode createInvfromGrin = HttpStatusCode.OK;
                HttpStatusCode createInvTransfromGrin = HttpStatusCode.OK;
                HttpStatusCode getInvdetailsGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode updateInvTrans = HttpStatusCode.OK;

                var existingIqcConfirmation = await _iQCForServiceItemsRepository.GetIQCForServiceItemsDetailsbyGrinForServiceItemsNo(grinNumber);

                if (existingIqcConfirmation != null)
                {

                    for (int i = 0; i < iQCDto.Count; i++)
                    {
                        IQCForServiceItems_Items iQCForServiceItems_Items = _mapper.Map<IQCForServiceItems_Items>(iQCDto[i]);
                        iQCForServiceItems_Items.IQCForServiceItemsId = existingIqcConfirmation.Id;
                        var grinPartId = iQCDto[i].GrinsForServiceItemsPartsId;
                        var grinPartsDetails = await _grinPartsRepository.GetGrinsForServiceItemsPartsById(grinPartId);
                        if (iQCDto[i].GrinsForServiceItemsPartsId != grinPartsDetails.Id)
                        {
                            _logger.LogError($"Invalid GrinsForServiceItemsPartsId {grinPartId}");
                            serviceResponse.Data = null;
                            serviceResponse.Message = $"Invalid GrinsForServiceItemsPartsId {grinPartId}";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }
                        if (grinPartsDetails.Qty <= (iQCDto[i].AcceptedQty + iQCDto[i].RejectedQty))
                        {
                            grinPartsDetails.AcceptedQty = iQCDto[i].AcceptedQty;
                            grinPartsDetails.RejectedQty = iQCDto[i].RejectedQty;
                        }
                        else
                        {
                            _logger.LogError("GrinsForServiceItemsPart Quantity should not be lesser than accepted + rejected Quantity .");
                            serviceResponse.Data = null;
                            serviceResponse.Message = "GrinsForServiceItemsParts Quantity should not be lesser than accepted + rejected Quantity ";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }

                        //Updating IQC Status in IqcItem Level

                        iQCForServiceItems_Items.IsIqcForServiceItemsCompleted = true;
                        await _iQCForServiceItems_ItemsRepository.CreateIqcForServiceItems_Items(iQCForServiceItems_Items);
                        _iQCForServiceItems_ItemsRepository.SaveAsync();

                        ////Inventory Update Code

                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = iQCDto[i].ItemNumber;
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

                        decimal acceptedQty = iQCDto[i].AcceptedQty;
                        decimal rejectedQty = iQCDto[i].RejectedQty;
                        foreach (var projectNo in grinPartsDetails.GrinsForServiceItemsProjectNumbers)
                        {
                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                            var itemNo = iQCDto[i].ItemNumber;
                            var encodedItemNo = Uri.EscapeDataString(itemNo);
                            var grinNo = iQCCreate.GrinsForServiceItemsNumber;
                            var encodedgrinNo = Uri.EscapeDataString(grinNo);
                            var grinPartsIds = projectNo.GrinsForServiceItemsPartsId;
                            var projectNos = projectNo.ProjectNumber;
                            var encodedprojectNos = Uri.EscapeDataString(projectNos);

                            var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryForServiceItemsAPI"],
                                $"GetInventoryForServiceDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                            request1.Headers.Add("Authorization", token1);

                            var inventoryObjectResult = await client1.SendAsync(request1);
                            if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvdetailsGrinId = inventoryObjectResult.StatusCode;

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
                                    inventoryObject.referenceID = iQCCreate.GrinsForServiceItemsNumber;
                                    inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                    acceptedQty -= balanceQty;

                                }
                                else if (inventoryObject.balance_Quantity > acceptedQty)
                                {
                                    if (acceptedQty == 0)
                                    {
                                        inventoryObject.balance_Quantity = acceptedQty;
                                        inventoryObject.warehouse = "IQC";
                                        inventoryObject.location = "IQC";
                                        inventoryObject.referenceID = iQCCreate.GrinsForServiceItemsNumber;
                                        inventoryObject.referenceIDFrom = "IQCForServiceItems";
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
                                        inventoryObject.referenceID = iQCCreate.GrinsForServiceItemsNumber;
                                        inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                        acceptedQty = 0;

                                    }
                                }
                                if (inventoryObject.balance_Quantity == 0) { inventoryObject.isStockAvailable = 0; }
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                var client5 = _clientFactory.CreateClient();
                                var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryForServiceItemsAPI"],
                                "UpdateInventoryForServiceDetails?id=", inventoryObject.id))
                                {
                                    Content = data
                                };
                                request5.Headers.Add("Authorization", token5);

                                var response = await client5.SendAsync(request5);

                                if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

                                if (iQCDto[i].RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                                {
                                    IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                    grinInventoryDto.PartNumber = iQCForServiceItems_Items.ItemNumber;
                                    grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                                    grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                    grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                                    grinInventoryDto.ProjectNumber = projectNos;
                                    grinInventoryDto.Max = itemMasterObject.max;
                                    grinInventoryDto.Min = itemMasterObject.min;
                                    grinInventoryDto.UOM = grinPartsDetails.UOM;
                                    grinInventoryDto.Warehouse = "Reject";
                                    grinInventoryDto.Location = "Reject";
                                    grinInventoryDto.GrinNo = iQCCreate.GrinsForServiceItemsNumber;
                                    grinInventoryDto.GrinPartId = iQCForServiceItems_Items.GrinsForServiceItemsPartsId;
                                    grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
                                    grinInventoryDto.ReferenceID = iQCCreate.GrinsForServiceItemsNumber; // Convert.ToString(iQCForServiceItems_Items.Id) //;
                                    grinInventoryDto.ReferenceIDFrom = "IQCForServiceItems";
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


                                        string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                        var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                        var client6 = _clientFactory.CreateClient();
                                        var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                        var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryForServiceItemsAPI"],
                                        "CreateInventoryForServiceItemsFromGrin"))
                                        {
                                            Content = content
                                        };
                                        request6.Headers.Add("Authorization", token6);

                                        var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                        if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInvfromGrin = rfqCustomerIdResponse.StatusCode;

                                        //InventoryTranction Update Code

                                        IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                        iqcInventoryTranctionDtos.PartNumber = iQCForServiceItems_Items.ItemNumber;
                                        iqcInventoryTranctionDtos.LotNumber = grinPartsDetails.LotNumber;
                                        iqcInventoryTranctionDtos.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                        iqcInventoryTranctionDtos.Description = grinPartsDetails.ItemDescription;
                                        iqcInventoryTranctionDtos.ProjectNumber = projectNo.ProjectNumber;
                                        iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                        iqcInventoryTranctionDtos.UOM = grinPartsDetails.UOM;
                                        iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                        iqcInventoryTranctionDtos.From_Location = grinInventoryDto.Location;
                                        iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                        iqcInventoryTranctionDtos.GrinNo = iQCCreate.GrinsForServiceItemsNumber; ;
                                        iqcInventoryTranctionDtos.GrinPartId = iQCForServiceItems_Items.GrinsForServiceItemsPartsId;
                                        iqcInventoryTranctionDtos.PartType = itemMasterObject.itemType;
                                        iqcInventoryTranctionDtos.ReferenceID = iQCCreate.GrinsForServiceItemsNumber;/* Convert.ToString(grinPartsDetails.Id);*/
                                        iqcInventoryTranctionDtos.ReferenceIDFrom = "IQCForServiceItems";
                                        iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                        iqcInventoryTranctionDtos.ShopOrderNo = "";

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

                                        if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTransfromGrin = inventoryTransResponses1.StatusCode;
                                    }


                                    //InventoryTranction Update Code

                                    IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                                    iqcInventoryTranctionDto.PartNumber = iQCForServiceItems_Items.ItemNumber;
                                    iqcInventoryTranctionDto.LotNumber = grinPartsDetails.LotNumber;
                                    iqcInventoryTranctionDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                    iqcInventoryTranctionDto.Description = grinPartsDetails.ItemDescription;
                                    iqcInventoryTranctionDto.ProjectNumber = projectNo.ProjectNumber;
                                    iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                    iqcInventoryTranctionDto.UOM = grinPartsDetails.UOM;
                                    iqcInventoryTranctionDto.Warehouse = "IQC";
                                    iqcInventoryTranctionDto.From_Location = "IQC";
                                    iqcInventoryTranctionDto.TO_Location = "IQC";
                                    iqcInventoryTranctionDto.GrinNo = iQCCreate.GrinsForServiceItemsNumber; ;
                                    iqcInventoryTranctionDto.GrinPartId = iQCForServiceItems_Items.GrinsForServiceItemsPartsId;
                                    iqcInventoryTranctionDto.PartType = itemMasterObject.itemType;
                                    iqcInventoryTranctionDto.ReferenceID = iQCCreate.GrinsForServiceItemsNumber;/* Convert.ToString(grinPartsDetails.Id);*/
                                    iqcInventoryTranctionDto.ReferenceIDFrom = "IQCForServiceItems";
                                    iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                                    iqcInventoryTranctionDto.ShopOrderNo = "";

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

                                    if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTransfromGrin = inventoryTransResponses.StatusCode;
                                }
                            }
                        }
                        var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinsForServiceItemsPartsQty(iQCForServiceItems_Items.GrinsForServiceItemsPartsId, iQCForServiceItems_Items.AcceptedQty.ToString(), iQCForServiceItems_Items.RejectedQty.ToString());

                        var grinParts = _mapper.Map<GrinsForServiceItemsParts>(updatedGrinPartsQty);
                        grinParts.IsIqcForServiceItemsCompleted = true;
                        string result = await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinParts);
                        _grinPartsRepository.SaveAsync();

                        //Updating binning Status in GrinParts
                        if (iQCDto[i].AcceptedQty == 0)
                        {
                            var grinPartsData = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(iQCDto[i].GrinsForServiceItemsPartsId);
                            if (grinPartsData.Remarks == null || string.IsNullOrWhiteSpace(grinPartsData.Remarks))
                            {
                                grinPartsData.Remarks = "IQCForServiceItems Rejected for all";
                            }
                            else
                            {
                                grinPartsData.Remarks = grinPartsData.Remarks + "[IQCForServiceItems Rejected for all]";
                            }
                            await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsData);
                            _grinPartsRepository.SaveAsync();
                        }

                    }
                    var grinPartsIqcStatuscount = await _grinPartsRepository.GetGrinsForServiceItemsPartsCount(iQCCreate.GrinsForServiceItemsId);

                    if (grinPartsIqcStatuscount == 0)
                    {
                        var grinDetails = await _grinsForServiceItemsRepository.GetGrinForServiceItemsByGrinForServiceItemsNo(grinNumber);
                        grinDetails.IsGrinsForServiceItemsCompleted = true;
                        await _grinsForServiceItemsRepository.UpdateGrinsForServiceItems(grinDetails);
                        _grinsForServiceItemsRepository.SaveAsync();
                    }

                    //Updating IQC Status in IQC

                    var grinIqcStatuscount = await _grinsForServiceItemsRepository.GetGrinsForServiceItemsIqcForServiceItemsStatusCount(grinNumber);

                    if (grinIqcStatuscount > 0)
                    {
                        var iqcDetails = await _iQCForServiceItemsRepository.GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(grinNumber);
                        iqcDetails.IsIqcForServiceItemsCompleted = true;
                        await _iQCForServiceItemsRepository.UpdateIQCForServiceItems(iqcDetails);
                    }
                    if (getItemmResp == HttpStatusCode.OK && createInvfromGrin == HttpStatusCode.OK && createInvTransfromGrin == HttpStatusCode.OK && getInvdetailsGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK)
                    {
                        _iQCForServiceItemsRepository.SaveAsync();
                        _grinsForServiceItemsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create IQCForServiceItems action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }


                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCForServiceItems and IQCForServiceItems_Items Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (serverKey == "avision")
                    {
                        var grinNum = iQCCreate.GrinsForServiceItemsNumber;
                        if (grinNum != null)
                        {
                            var iqcNum = grinNum.Replace("GSI", "IQCSI");
                            iQCCreate.IQCForServiceItemsNumber = iqcNum;
                        }

                    }

                    for (int i = 0; i < iQCDto.Count; i++)
                    {
                        IQCForServiceItems_Items iQCConfirmationItems = _mapper.Map<IQCForServiceItems_Items>(iQCDto[i]);
                        var grinPartId = iQCDto[i].GrinsForServiceItemsPartsId;

                        var grinPartsDetails = await _grinPartsRepository.GetGrinsForServiceItemsPartsById(grinPartId);
                        if (grinPartsDetails == null)
                        {
                            _logger.LogError($"Invalid GrinPartForServiceItems Id {grinPartId}");
                            serviceResponse.Data = null;
                            serviceResponse.Message = $"Invalid GrinPartForServiceItems Id {grinPartId}";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }
                        if (grinPartsDetails.Qty <= (iQCConfirmationItems.AcceptedQty + iQCConfirmationItems.RejectedQty))
                        {
                            grinPartsDetails.AcceptedQty = iQCConfirmationItems.AcceptedQty;
                            grinPartsDetails.RejectedQty = iQCConfirmationItems.RejectedQty;

                        }
                        else
                        {
                            _logger.LogError("GrinpartForServiceItems Quantity should not be lesser than accepted + rejected Quantity .");
                            serviceResponse.Data = null;
                            serviceResponse.Message = "GrinpartForServiceItems Quantity should not be lesser than accepted + rejected Quantity ";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }

                        //Updating IQC Status in IqcItem Level

                        iQCConfirmationItems.IsIqcForServiceItemsCompleted = true;
                        iQCItemList.Add(iQCConfirmationItems);

                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = iQCDto[i].ItemNumber;
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

                        decimal acceptedQty = iQCDto[i].AcceptedQty;
                        decimal rejectedQty = iQCDto[i].RejectedQty;
                        foreach (var projectNo in grinPartsDetails.GrinsForServiceItemsProjectNumbers)
                        {
                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                            var itemNo = iQCDto[i].ItemNumber;
                            var encodedItemNo = Uri.EscapeDataString(itemNo);
                            var grinNo = iQCCreate.GrinsForServiceItemsNumber;
                            var encodedgrinNo = Uri.EscapeDataString(grinNo);
                            var grinPartsIds = projectNo.GrinsForServiceItemsPartsId;
                            var projectNos = projectNo.ProjectNumber;
                            var encodedprojectNos = Uri.EscapeDataString(projectNos);


                            var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryForServiceItemsAPI"],
                                $"GetInventoryForServiceDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                            request1.Headers.Add("Authorization", token1);

                            var inventoryObjectResult = await client1.SendAsync(request1);
                            if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvdetailsGrinId = inventoryObjectResult.StatusCode;

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
                                    inventoryObject.referenceID = iQCCreate.IQCForServiceItemsNumber;
                                    inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                    acceptedQty -= balanceQty;

                                }
                                else if (inventoryObject.balance_Quantity > acceptedQty)
                                {
                                    if (acceptedQty == 0)
                                    {
                                        inventoryObject.balance_Quantity = acceptedQty;
                                        inventoryObject.warehouse = "IQC";
                                        inventoryObject.location = "IQC";
                                        inventoryObject.referenceID = iQCCreate.IQCForServiceItemsNumber;
                                        inventoryObject.referenceIDFrom = "IQCForServiceItems";
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
                                        inventoryObject.referenceID = iQCCreate.IQCForServiceItemsNumber;
                                        inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                        acceptedQty = 0;


                                    }
                                }
                                if (inventoryObject.balance_Quantity == 0) { inventoryObject.isStockAvailable = 0; }
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                //var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                                //    "UpdateInventory?id=", inventoryObject.id), data);
                                var client5 = _clientFactory.CreateClient();
                                var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryForServiceItemsAPI"],
                                 "UpdateInventoryForServiceDetails?id=", inventoryObject.id))
                                {
                                    Content = data
                                };
                                request5.Headers.Add("Authorization", token5);

                                var response = await client5.SendAsync(request5);
                                if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

                                if (iQCDto[i].RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                                {
                                    IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                    grinInventoryDto.PartNumber = iQCConfirmationItems.ItemNumber;
                                    grinInventoryDto.LotNumber = grinPartsDetails.LotNumber;
                                    grinInventoryDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                    grinInventoryDto.Description = grinPartsDetails.ItemDescription;
                                    grinInventoryDto.ProjectNumber = projectNos;
                                    //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(iQCForServiceItems_Items.RejectedQty);
                                    grinInventoryDto.Max = itemMasterObject.max;
                                    grinInventoryDto.Min = itemMasterObject.min;
                                    grinInventoryDto.UOM = grinPartsDetails.UOM;
                                    grinInventoryDto.Warehouse = "Reject";
                                    grinInventoryDto.Location = "Reject";
                                    grinInventoryDto.GrinNo = iQCCreate.GrinsForServiceItemsNumber;
                                    grinInventoryDto.GrinPartId = iQCConfirmationItems.GrinsForServiceItemsPartsId;
                                    grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
                                    grinInventoryDto.ReferenceID = iQCCreate.IQCForServiceItemsNumber; // Convert.ToString(iQCForServiceItems_Items.Id) //;
                                    grinInventoryDto.ReferenceIDFrom = "IQCForServiceItems";
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

                                    //var httpClientHandler = new HttpClientHandler();
                                    //httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                    //var httpClient = new HttpClient(httpClientHandler);
                                    string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                    //var rfqApiUrl = _config["InventoryAPI"];
                                    var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                    //var rfqCustomerIdResponse = await _httpClient.PostAsync($"{rfqApiUrl}CreateInventoryFromGrin", content);

                                    var client6 = _clientFactory.CreateClient();
                                    var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryForServiceItemsAPI"],
                                    "CreateInventoryForServiceItemsFromGrin"))
                                    {
                                        Content = content
                                    };
                                    request6.Headers.Add("Authorization", token6);

                                    var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                    if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInvfromGrin = rfqCustomerIdResponse.StatusCode;

                                    //InventoryTranction Update Code

                                    IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                    iqcInventoryTranctionDtos.PartNumber = iQCConfirmationItems.ItemNumber;
                                    iqcInventoryTranctionDtos.LotNumber = grinPartsDetails.LotNumber;
                                    iqcInventoryTranctionDtos.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                    iqcInventoryTranctionDtos.Description = grinPartsDetails.ItemDescription;
                                    iqcInventoryTranctionDtos.ProjectNumber = projectNo.ProjectNumber;
                                    iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                    iqcInventoryTranctionDtos.UOM = grinPartsDetails.UOM;
                                    iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                    iqcInventoryTranctionDtos.From_Location = grinInventoryDto.Location;
                                    iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                    iqcInventoryTranctionDtos.GrinNo = iQCCreate.GrinsForServiceItemsNumber; ;
                                    iqcInventoryTranctionDtos.GrinPartId = iQCConfirmationItems.GrinsForServiceItemsPartsId;
                                    iqcInventoryTranctionDtos.PartType = itemMasterObject.itemType;
                                    iqcInventoryTranctionDtos.ReferenceID = iQCCreate.IQCForServiceItemsNumber;/* Convert.ToString(grinPartsDetails.Id);*/
                                    iqcInventoryTranctionDtos.ReferenceIDFrom = "IQCForServiceItems";
                                    iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                    iqcInventoryTranctionDtos.ShopOrderNo = "";

                                    //var httpClientHandlers = new HttpClientHandler();
                                    //httpClientHandlers.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                    //var httpClients = new HttpClient(httpClientHandlers);
                                    string iqcInventoryTranctionDtoJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDtos);
                                    //var rfqApiUrls = _config["InventoryTranctionAPI"];
                                    var contents1 = new StringContent(iqcInventoryTranctionDtoJsons, Encoding.UTF8, "application/json");
                                    //var inventoryTransResponses = await _httpClient.PostAsync($"{rfqApiUrls}CreateInventoryTranction", contents);
                                    var client8 = _clientFactory.CreateClient();
                                    var token8 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request8 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                    "CreateInventoryTranction"))
                                    {
                                        Content = contents1
                                    };
                                    request8.Headers.Add("Authorization", token8);

                                    var inventoryTransResponses1 = await client8.SendAsync(request8);
                                    if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTransfromGrin = inventoryTransResponses1.StatusCode;
                                }
                                //InventoryTranction Update Code

                                IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDto.PartNumber = iQCConfirmationItems.ItemNumber;
                                iqcInventoryTranctionDto.LotNumber = grinPartsDetails.LotNumber;
                                iqcInventoryTranctionDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                iqcInventoryTranctionDto.Description = grinPartsDetails.ItemDescription;
                                iqcInventoryTranctionDto.ProjectNumber = projectNo.ProjectNumber;
                                iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                iqcInventoryTranctionDto.UOM = grinPartsDetails.UOM;
                                iqcInventoryTranctionDto.Warehouse = "IQC";
                                iqcInventoryTranctionDto.From_Location = "IQC";
                                iqcInventoryTranctionDto.TO_Location = "IQC";
                                iqcInventoryTranctionDto.GrinNo = iQCCreate.GrinsForServiceItemsNumber; ;
                                iqcInventoryTranctionDto.GrinPartId = iQCConfirmationItems.GrinsForServiceItemsPartsId;
                                iqcInventoryTranctionDto.PartType = itemMasterObject.itemType;
                                iqcInventoryTranctionDto.ReferenceID = iQCCreate.IQCForServiceItemsNumber;/* Convert.ToString(grinPartsDetails.Id);*/
                                iqcInventoryTranctionDto.ReferenceIDFrom = "IQCForServiceItems";
                                iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                                iqcInventoryTranctionDto.ShopOrderNo = "";

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

                                if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTransfromGrin = inventoryTransResponses.StatusCode;
                            }
                        }
                        ////update accepted qty and rejected qty in grin model
                        //Updating IQC Status in GrinParts
                        var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinsForServiceItemsPartsQty(iQCConfirmationItems.GrinsForServiceItemsPartsId, iQCConfirmationItems.AcceptedQty.ToString(), iQCConfirmationItems.RejectedQty.ToString());

                        var grinParts = _mapper.Map<GrinsForServiceItemsParts>(updatedGrinPartsQty);

                        grinParts.IsIqcForServiceItemsCompleted = true;
                        string result = await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinParts);

                        //Updating binning Status in GrinParts
                        if (iQCDto[i].AcceptedQty == 0)
                        {
                            var grinPartsData = await _grinPartsRepository.GetGrinsForServiceItemsPartsById(iQCDto[i].GrinsForServiceItemsPartsId);
                            if (grinPartsData.Remarks == null || string.IsNullOrWhiteSpace(grinPartsData.Remarks))
                            {
                                grinPartsData.Remarks = "IQCForServiceItems Rejected for all";
                            }
                            else
                            {
                                grinPartsData.Remarks = grinPartsData.Remarks + "[IQCForServiceItems Rejected for all]";
                            }
                            await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsData);
                            _grinPartsRepository.SaveAsync();
                        }
                    }

                    //Updating IQC Status in IQC
                    iQCCreate.IQCForServiceItems_Items = iQCItemList;
                    iQCCreate.IsIqcForServiceItemsCompleted = true;
                    await _iQCForServiceItemsRepository.CreateIQCForServiceItems(iQCCreate);

                    //Updating IQC Status in Grin
                    var grinDetails = await _grinsForServiceItemsRepository.GetGrinForServiceItemsByGrinForServiceItemsNo(grinNumber);
                    grinDetails.IsIqcForServiceItemsCompleted = true;
                    await _grinsForServiceItemsRepository.UpdateGrinsForServiceItems(grinDetails);

                    if (getItemmResp == HttpStatusCode.OK && createInvfromGrin == HttpStatusCode.OK && createInvTransfromGrin == HttpStatusCode.OK && getInvdetailsGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK)
                    {
                        _iQCForServiceItemsRepository.SaveAsync();
                        _grinsForServiceItemsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create IQCForServiceItems action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCForServiceItems Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);


                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Create IQCForServiceItems action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);


            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateIQCForServiceItems_Items([FromBody] IQCForServiceItemsSaveDto iQCForServiceItemsSaveDto)
        {
            ServiceResponse<IQCForServiceItemsSaveDto> serviceResponse = new ServiceResponse<IQCForServiceItemsSaveDto>();

            try
            {
                string serverKey = GetServerKey();

                if (iQCForServiceItemsSaveDto is null)
                {
                    _logger.LogError("Create IQCForServiceItems object sent from the client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Create IQCForServiceItems object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Create IQCForServiceItems object sent from the client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Create IQCForServiceItems object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                //var iqcConfirmation = _mapper.Map<IQCForServiceItems>(iQCForServiceItemsSaveDto);
                var iqcConfirmation = new IQCForServiceItems()
                {
                    GrinsForServiceItemsId= iQCForServiceItemsSaveDto.GrinsForServiceItemsId,
                    GrinsForServiceItemsNumber= iQCForServiceItemsSaveDto.GrinsForServiceItemsNumber
                };
                var iqcConfirmationItemsDto = iQCForServiceItemsSaveDto.IQCForServiceItems_Items;
                var iqcConfirmationItems = _mapper.Map<IQCForServiceItems_Items>(iqcConfirmationItemsDto);
                var grinNumber = iqcConfirmation.GrinsForServiceItemsNumber;
                HttpStatusCode getInvGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode getInvTrancGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInvTranc = HttpStatusCode.OK;
                HttpStatusCode createInv = HttpStatusCode.OK;
                HttpStatusCode createInvTrans = HttpStatusCode.OK;
                HttpStatusCode getItemmResp = HttpStatusCode.OK;

                var existingIqcConfirmation = await _iQCForServiceItemsRepository.GetIQCForServiceItemsDetailsbyGrinForServiceItemsNo(grinNumber);

                if (existingIqcConfirmation != null)
                {
                    iqcConfirmationItems.IQCForServiceItemsId = existingIqcConfirmation.Id;

                    var grinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartsId;
                    var grinPartsDetails = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(grinPartId);
                    if (iqcConfirmationItemsDto.GrinsForServiceItemsPartsId != grinPartsDetails.Id)
                    {
                        _logger.LogError($"Invalid GrinsForServiceItemsPartsId {grinPartId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid GrinsForServiceItemsPartsId {grinPartId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }
                    if (grinPartsDetails.Qty <= (iqcConfirmationItems.AcceptedQty + iqcConfirmationItems.RejectedQty))
                    {
                        grinPartsDetails.AcceptedQty = iqcConfirmationItems.AcceptedQty;
                        grinPartsDetails.RejectedQty = iqcConfirmationItems.RejectedQty;
                    }
                    else
                    {
                        _logger.LogError("GrinsForServiceItemsParts Quantity should not be lesser than accepted + rejected Quantity .");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "GrinsForServiceItemsParts Quantity should not be lesser than accepted + rejected Quantity ";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    //Updating IQC Status in IqcItem

                    iqcConfirmationItems.IsIqcForServiceItemsCompleted = true;
                    await _iQCForServiceItems_ItemsRepository.CreateIqcForServiceItems_Items(iqcConfirmationItems);


                    //Updating IQC Status in GrinParts

                    grinPartsDetails.IsIqcForServiceItemsCompleted = true;
                    await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsDetails);
                    _grinPartsRepository.SaveAsync();

                    //Updating binning Status in GrinParts
                    if (iqcConfirmationItems.AcceptedQty == 0)
                    {
                        var grinPartsData = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(iqcConfirmationItems.GrinsForServiceItemsPartsId);

                        if (grinPartsData.Remarks == null || string.IsNullOrWhiteSpace(grinPartsData.Remarks))
                        {
                            grinPartsData.Remarks = "IqcForServiceItems Rejected for all";
                        }
                        else
                        {
                            grinPartsData.Remarks = grinPartsData.Remarks + "[IqcForServiceItems Rejected for all]";
                        }
                        await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsData);
                        _grinPartsRepository.SaveAsync();
                    }

                    //Updating IQC Status in Grin

                    var grinPartsIqcStatuscount = await _grinPartsRepository.GetGrinsForServiceItemsPartsCount(grinPartsDetails.GrinsForServiceItemsId);

                    if (grinPartsIqcStatuscount == 0)
                    {
                        var grinDetails = await _grinsForServiceItemsRepository.GetGrinForServiceItemsByGrinForServiceItemsNo(grinNumber);
                        grinDetails.IsIqcForServiceItemsCompleted = true;
                        await _grinsForServiceItemsRepository.UpdateGrinsForServiceItems(grinDetails);
                        _grinsForServiceItemsRepository.SaveAsync();

                    }

                    //Updating IQC Status in IQC

                    var grinIqcStatuscount = await _grinsForServiceItemsRepository.GetGrinsForServiceItemsIqcForServiceItemsStatusCount(grinNumber);

                    if (grinIqcStatuscount > 0)
                    {
                        var iqcDetails = await _iQCForServiceItemsRepository.GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(grinNumber);
                        iqcDetails.IsIqcForServiceItemsCompleted = true;
                        await _iQCForServiceItemsRepository.UpdateIQCForServiceItems(iqcDetails);

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
                    decimal acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    decimal rejectedQty = iqcConfirmationItemsDto.RejectedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinsForServiceItemsPartsId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinsForServiceItemsPartsById(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.GrinsForServiceItemsProjectNumbers)
                    {
                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var grinNo = iqcConfirmation.GrinsForServiceItemsNumber;
                        var encodedgrinNo = Uri.EscapeDataString(grinNo);
                        var grinPartsIds = projectNo.GrinsForServiceItemsPartsId;
                        var projectNos = projectNo.ProjectNumber;
                        var encodedprojectNos = Uri.EscapeDataString(projectNos);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryForServiceItemsAPI"],
                           $"GetInventoryForServiceDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
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
                                inventoryObject.referenceID = iqcConfirmation.GrinsForServiceItemsNumber;
                                inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceID = iqcConfirmation.GrinsForServiceItemsNumber;
                                    inventoryObject.referenceIDFrom = "IQCForServiceItems";
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
                                    inventoryObject.referenceID = iqcConfirmation.GrinsForServiceItemsNumber;
                                    inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                    acceptedQty = 0;
                                }
                            }

                            var json = JsonConvert.SerializeObject(inventoryObject);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var client5 = _clientFactory.CreateClient();
                            var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryForServiceItemsAPI"],
                            "UpdateInventoryForServiceDetails?id=", inventoryObject.id))
                            {
                                Content = data
                            };
                            request5.Headers.Add("Authorization", token5);

                            var response = await client5.SendAsync(request5);
                            if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

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
                                grinInventoryDto.GrinNo = iqcConfirmation.GrinsForServiceItemsNumber;
                                grinInventoryDto.GrinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartsId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
                                grinInventoryDto.ReferenceID = iqcConfirmation.GrinsForServiceItemsNumber; // Convert.ToString(iQCConfirmationItems.Id) //;
                                grinInventoryDto.ReferenceIDFrom = "IQCForServiceItems";
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
                                var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryForServiceItemsAPI"],
                                "CreateInventoryForServiceItemsFromGrin"))
                                {
                                    Content = content
                                };
                                request6.Headers.Add("Authorization", token6);

                                var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInv = rfqCustomerIdResponse.StatusCode;

                                //InventoryTranction Update Code

                                IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDtos.PartNumber = iqcConfirmationItemsDto.ItemNumber;
                                iqcInventoryTranctionDtos.LotNumber = grinPartsDetails.LotNumber;
                                iqcInventoryTranctionDtos.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                iqcInventoryTranctionDtos.Description = grinPartsDetails.ItemDescription;
                                iqcInventoryTranctionDtos.ProjectNumber = projectNo.ProjectNumber;
                                iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                iqcInventoryTranctionDtos.UOM = grinPartsDetails.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinNo = iqcConfirmation.GrinsForServiceItemsNumber; ;
                                iqcInventoryTranctionDtos.GrinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartsId;
                                iqcInventoryTranctionDtos.PartType = itemMasterObject.itemType;
                                iqcInventoryTranctionDtos.ReferenceID = iqcConfirmation.GrinsForServiceItemsNumber;/* Convert.ToString(grinPartsDetails.Id);*/
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "IQCForServiceItems";
                                iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                iqcInventoryTranctionDtos.ShopOrderNo = "";
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

                                if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTrans = inventoryTransResponses1.StatusCode;
                            }

                            //InventoryTranction Update Code

                            IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                            iqcInventoryTranctionDto.PartNumber = iqcConfirmationItemsDto.ItemNumber;
                            iqcInventoryTranctionDto.LotNumber = grinPartsDetails.LotNumber;
                            iqcInventoryTranctionDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                            iqcInventoryTranctionDto.Description = grinPartsDetails.ItemDescription;
                            iqcInventoryTranctionDto.ProjectNumber = projectNo.ProjectNumber;
                            iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                            iqcInventoryTranctionDto.UOM = grinPartsDetails.UOM;
                            iqcInventoryTranctionDto.Warehouse = "IQC";
                            iqcInventoryTranctionDto.From_Location = "IQC";
                            iqcInventoryTranctionDto.TO_Location = "IQC";
                            iqcInventoryTranctionDto.GrinNo = iqcConfirmation.GrinsForServiceItemsNumber; ;
                            iqcInventoryTranctionDto.GrinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartsId;
                            iqcInventoryTranctionDto.PartType = itemMasterObject.itemType;
                            iqcInventoryTranctionDto.ReferenceID = iqcConfirmation.GrinsForServiceItemsNumber;/* Convert.ToString(grinPartsDetails.Id);*/
                            iqcInventoryTranctionDto.ReferenceIDFrom = "IQCForServiceItems";
                            iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                            iqcInventoryTranctionDto.ShopOrderNo = "";

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
                            if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTrans = inventoryTransResponses.StatusCode;
                        }
                    }

                    ////update accepted qty and rejected qty in grin model

                    var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinsForServiceItemsPartsQty(iqcConfirmationItems.GrinsForServiceItemsPartsId, iqcConfirmationItems.AcceptedQty.ToString(), iqcConfirmationItems.RejectedQty.ToString());

                    var grinParts = _mapper.Map<GrinsForServiceItemsParts>(updatedGrinPartsQty);
                    string result = await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinParts);
                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && getInvTrancGrinId == HttpStatusCode.OK && updateInvTranc == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                    {
                        _iQCForServiceItems_ItemsRepository.SaveAsync();
                        _grinPartsRepository.SaveAsync();
                        _iQCForServiceItemsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create IQCForServiceItems_Items action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCForServiceItems_Items Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (serverKey == "avision")
                    {
                        var grinNum = iqcConfirmation.GrinsForServiceItemsNumber;
                        if (grinNum != null)
                        {
                            var iqcNum = grinNum.Replace("GSI", "IQCSI");
                            iqcConfirmation.IQCForServiceItemsNumber = iqcNum;
                        }
                    }

                    var grinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartsId;
                    var grinPartsDetails = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(grinPartId);
                    if (iqcConfirmationItemsDto.GrinsForServiceItemsPartsId != grinPartsDetails.Id)
                    {
                        _logger.LogError($"Invalid GrinsForServiceItemsPartsId {grinPartId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid GrinsForServiceItemsPartsId {grinPartId}";
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
                        _logger.LogError("GrinsForServiceItemsParts Quantity should not be lesser than accepted + rejected Quantity .");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "GrinsForServiceItemsParts Quantity should not be lesser than accepted + rejected Quantity ";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    //Updating IQC Status in IqcItem

                    iqcConfirmationItems.IsIqcForServiceItemsCompleted = true;
                    iqcConfirmation.IQCForServiceItems_Items = new List<IQCForServiceItems_Items> { iqcConfirmationItems };
                    await _iQCForServiceItemsRepository.CreateIQCForServiceItems(iqcConfirmation);

                    //Updating IQC Status in GrinParts

                    grinPartsDetails.IsIqcForServiceItemsCompleted = true;
                    await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsDetails);
                    _grinPartsRepository.SaveAsync();

                    //Updating binning Status in GrinParts
                    if (iqcConfirmationItems.AcceptedQty == 0)
                    {
                        var grinPartsData = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(iqcConfirmationItems.GrinsForServiceItemsPartsId);
                        if (grinPartsData.Remarks == null || string.IsNullOrWhiteSpace(grinPartsData.Remarks))
                        {
                            grinPartsData.Remarks = "IQCForServiceItems Rejected for all";
                        }
                        else
                        {
                            grinPartsData.Remarks = grinPartsData.Remarks + "[IQCForServiceItems Rejected for all]";
                        }
                        await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsData);
                        _grinPartsRepository.SaveAsync();
                    }
                    //Updating IQC Status in Grin

                    var grinPartsIqcStatuscount = await _grinPartsRepository.GetGrinsForServiceItemsPartsCount(grinPartsDetails.GrinsForServiceItemsId);

                    if (grinPartsIqcStatuscount == 0)
                    {
                        var grinDetails = await _grinsForServiceItemsRepository.GetGrinForServiceItemsByGrinForServiceItemsNo(grinNumber);
                        grinDetails.IsIqcForServiceItemsCompleted = true;
                        await _grinsForServiceItemsRepository.UpdateGrinsForServiceItems(grinDetails);
                        _grinsForServiceItemsRepository.SaveAsync();
                    }

                    //Updating IQC Status in IQC

                    var grinIqcStatuscount = await _grinsForServiceItemsRepository.GetGrinsForServiceItemsIqcForServiceItemsStatusCount(grinNumber);

                    if (grinIqcStatuscount > 0)
                    {
                        var iqcDetails = await _iQCForServiceItemsRepository.GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(grinNumber);
                        iqcDetails.IsIqcForServiceItemsCompleted = true;
                        await _iQCForServiceItemsRepository.UpdateIQCForServiceItems(iqcDetails);
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
                    decimal acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinsForServiceItemsPartsId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.GrinsForServiceItemsProjectNumbers)
                    {
                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var grinNo = iqcConfirmation.GrinsForServiceItemsNumber;
                        var encodedgrinNo = Uri.EscapeDataString(grinNo);
                        var grinPartsIds = projectNo.GrinsForServiceItemsPartsId;
                        var projectNos = projectNo.ProjectNumber;
                        var encodedprojectNos = Uri.EscapeDataString(projectNos);
                        
                            var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryForServiceItemsAPI"],
                                $"GetInventoryForServiceDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                            request1.Headers.Add("Authorization", token1);

                            var inventoryObjectResult = await client1.SendAsync(request1);
                            if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinId = inventoryObjectResult.StatusCode;
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
                                    inventoryObject.referenceID = iqcConfirmation.IQCForServiceItemsNumber;
                                    inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                    acceptedQty -= balanceQty;

                                }
                                else if (inventoryObject.balance_Quantity > acceptedQty)
                                {
                                    if (acceptedQty == 0)
                                    {
                                        inventoryObject.balance_Quantity = acceptedQty;
                                        inventoryObject.warehouse = "IQC";
                                        inventoryObject.location = "IQC";
                                        inventoryObject.referenceID = iqcConfirmation.IQCForServiceItemsNumber;
                                        inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                        inventoryObject.isStockAvailable = false;
                                    }
                                    else
                                    {
                                        inventoryObject.balance_Quantity = acceptedQty;
                                        inventoryObject.warehouse = "IQC";
                                        inventoryObject.location = "IQC";
                                        inventoryObject.referenceID = iqcConfirmation.IQCForServiceItemsNumber;
                                        inventoryObject.referenceIDFrom = "IQCForServiceItems";
                                        acceptedQty = 0;
                                    }
                                }

                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                var client5 = _clientFactory.CreateClient();
                                var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryForServiceItemsAPI"],
                                "UpdateInventoryForServiceDetails?id=", inventoryObject.id))
                                {
                                    Content = data
                                };
                                request5.Headers.Add("Authorization", token5);

                                var response = await client5.SendAsync(request5);
                                if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

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
                                    grinInventoryDto.GrinNo = iqcConfirmation.GrinsForServiceItemsNumber;
                                    grinInventoryDto.GrinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartsId;
                                    grinInventoryDto.PartType = itemMasterObject.itemType;  //We need to check this
                                    grinInventoryDto.ReferenceID = iqcConfirmation.IQCForServiceItemsNumber; // Convert.ToString(iQCConfirmationItems.Id) //;
                                    grinInventoryDto.ReferenceIDFrom = "IQCForServiceItems";
                                    grinInventoryDto.GrinMaterialType = "GRIN";
                                    grinInventoryDto.ShopOrderNo = "";

                                    string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(grinInventoryDto);
                                    var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                                    var client6 = _clientFactory.CreateClient();
                                    var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryForServiceItemsAPI"],
                                    "CreateInventoryForServiceItemsFromGrin"))
                                    {
                                        Content = content
                                    };
                                    request6.Headers.Add("Authorization", token6);

                                    var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                    if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInv = rfqCustomerIdResponse.StatusCode;

                                    //InventoryTranction Update Code

                                    IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                    iqcInventoryTranctionDtos.PartNumber = iqcConfirmationItemsDto.ItemNumber;
                                    iqcInventoryTranctionDtos.LotNumber = grinPartsDetails.LotNumber;
                                    iqcInventoryTranctionDtos.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                    iqcInventoryTranctionDtos.Description = grinPartsDetails.ItemDescription;
                                    iqcInventoryTranctionDtos.ProjectNumber = projectNo.ProjectNumber;
                                    iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                    iqcInventoryTranctionDtos.UOM = grinPartsDetails.UOM;
                                    iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                    iqcInventoryTranctionDtos.From_Location = grinInventoryDto.Location;
                                    iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                    iqcInventoryTranctionDtos.GrinNo = iqcConfirmation.GrinsForServiceItemsNumber; ;
                                    iqcInventoryTranctionDtos.GrinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartsId;
                                    iqcInventoryTranctionDtos.PartType = itemMasterObject.itemType;
                                    iqcInventoryTranctionDtos.ReferenceID = iqcConfirmation.IQCForServiceItemsNumber;/* Convert.ToString(grinPartsDetails.Id);*/
                                    iqcInventoryTranctionDtos.ReferenceIDFrom = "IQCForServiceItems";
                                    iqcInventoryTranctionDtos.GrinMaterialType = "GRIN";
                                    iqcInventoryTranctionDtos.ShopOrderNo = "";

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

                                    if (inventoryTransResponses1.StatusCode != HttpStatusCode.OK) createInvTrans = inventoryTransResponses1.StatusCode;
                                }


                                //InventoryTranction Update Code

                                IQCInventoryTranctionDto iqcInventoryTranctionDto = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDto.PartNumber = iqcConfirmationItemsDto.ItemNumber;
                                iqcInventoryTranctionDto.LotNumber = grinPartsDetails.LotNumber;
                                iqcInventoryTranctionDto.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                iqcInventoryTranctionDto.Description = grinPartsDetails.ItemDescription;
                                iqcInventoryTranctionDto.ProjectNumber = projectNo.ProjectNumber;
                                iqcInventoryTranctionDto.Issued_Quantity = inventoryObject.balance_Quantity;
                                iqcInventoryTranctionDto.UOM = grinPartsDetails.UOM;
                                iqcInventoryTranctionDto.Warehouse = "IQC";
                                iqcInventoryTranctionDto.From_Location = "IQC";
                                iqcInventoryTranctionDto.TO_Location = "IQC";
                                iqcInventoryTranctionDto.GrinNo = iqcConfirmation.GrinsForServiceItemsNumber; ;
                                iqcInventoryTranctionDto.GrinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartsId;
                                iqcInventoryTranctionDto.PartType = itemMasterObject.itemType;
                                iqcInventoryTranctionDto.ReferenceID = iqcConfirmation.IQCForServiceItemsNumber;/* Convert.ToString(grinPartsDetails.Id);*/
                                iqcInventoryTranctionDto.ReferenceIDFrom = "IQCForServiceItems";
                                iqcInventoryTranctionDto.GrinMaterialType = "GRIN";
                                iqcInventoryTranctionDto.ShopOrderNo = "";

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
                                if (inventoryTransResponses.StatusCode != HttpStatusCode.OK) createInvTrans = inventoryTransResponses.StatusCode;
                            }
                    }
                    ////update accepted qty and rejected qty in grin model

                    var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinsForServiceItemsPartsQty(iqcConfirmationItems.GrinsForServiceItemsPartsId, iqcConfirmationItems.AcceptedQty.ToString(), iqcConfirmationItems.RejectedQty.ToString());

                    var grinParts = _mapper.Map<GrinsForServiceItemsParts>(updatedGrinPartsQty);

                    string result = await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinParts);

                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && getInvTrancGrinId == HttpStatusCode.OK && updateInvTranc == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                    {
                        _grinPartsRepository.SaveAsync();
                        _iQCForServiceItemsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create IQCForServiceItems_Items action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCForServiceItems_Items and IQCForServiceItems_Items Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateIQCForServiceItems_Items action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIQCForServiceItemsDetailsbyId(int id)
        {
            ServiceResponse<IQCForServiceItemsDto> serviceResponse = new ServiceResponse<IQCForServiceItemsDto>();

            try
            {
                var iQCDetailsbyId = await _iQCForServiceItemsRepository.GetIQCForServiceItemsDetailsbyId(id);
                if (iQCDetailsbyId == null)
                {
                    _logger.LogError($"IQCForServiceItems details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCForServiceItems details with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned IQCForServiceItems details with id: {id}");
                    List<IQCForServiceItems_ItemsDto> iQCConfirmationItemsList = new List<IQCForServiceItems_ItemsDto>();
                    var iQcGrinNo = iQCDetailsbyId.GrinsForServiceItemsNumber;
                    var grinDetailsbyGrinNo = await _grinsForServiceItemsRepository.GetGrinForServiceItemsByGrinForServiceItemsNo(iQcGrinNo);

                    var iQCConformationDetailsDto = _mapper.Map<IQCForServiceItemsDto>(grinDetailsbyGrinNo);
                    iQCConformationDetailsDto.Id = id;
                    iQCConformationDetailsDto.GrinsForServiceItemsId = iQCConformationDetailsDto.Id;
                    var grinParts = grinDetailsbyGrinNo.GrinsForServiceItemsParts.Where(x => x.RejectedQty != 0 || x.AcceptedQty != 0).ToList();
                    if (grinParts.Count() != 0)
                    {
                        foreach (var grinDetails in grinParts)
                        {
                            IQCForServiceItems_ItemsDto iQCConfirmationItemsDtos = _mapper.Map<IQCForServiceItems_ItemsDto>(grinDetails);
                            iQCConfirmationItemsDtos.GrinsForServiceItemsProjectNumbersDto = _mapper.Map<List<GrinsForServiceItemsProjectNumbersDto>>(grinDetails.GrinsForServiceItemsProjectNumbers);
                            iQCConfirmationItemsDtos.Id = iQCConfirmationItemsDtos.Id;
                            iQCConfirmationItemsDtos.ReceivedQty = grinDetails.Qty;
                            iQCConfirmationItemsDtos.GrinsForServiceItemsPartsId = grinDetails.Id;
                            iQCConfirmationItemsDtos.ExpiryDate = grinDetails.ExpiryDate;
                            iQCConfirmationItemsList.Add(iQCConfirmationItemsDtos);
                        }
                    }
                    iQCConformationDetailsDto.IQCForServiceItems_Items = iQCConfirmationItemsList;
                    serviceResponse.Data = iQCConformationDetailsDto;
                    serviceResponse.Message = "IQCForServiceItemsById Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside IQCForServiceItemsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
