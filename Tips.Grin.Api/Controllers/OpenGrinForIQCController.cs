using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
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
    public class OpenGrinForIQCController : ControllerBase
    {
        private IOpenGrinForIQCRepository _repository;
        private IOpenGrinForGrinRepository _openGrinForGrinRepository;
        private IOpenGrinForGrinItemRepository _openGrinForGrinItemRepository;
        private IOpenGrinForIQCItemRepository _openGrinForIQCItemRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;

        public OpenGrinForIQCController(IHttpClientFactory clientFactory, IOpenGrinForIQCRepository repository, IHttpContextAccessor httpContextAccessor,
                                                IOpenGrinForGrinItemRepository openGrinForGrinItemRepository, IWebHostEnvironment webHostEnvironment, ILoggerManager logger,
                                                IMapper mapper, HttpClient httpClient, IConfiguration config, IOpenGrinForIQCItemRepository openGrinForIQCItemRepository,
                                                    IOpenGrinForGrinRepository openGrinForGrinRepository)
        {
            _repository = repository;
            _openGrinForGrinRepository = openGrinForGrinRepository;
            _openGrinForGrinItemRepository = openGrinForGrinItemRepository;
            _openGrinForIQCItemRepository = openGrinForIQCItemRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllOpenGrinForIQCDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<OpenGrinForIQCDto>> serviceResponse = new ServiceResponse<IEnumerable<OpenGrinForIQCDto>>();

            try
            {
                var openGrinForIQCDetails = await _repository.GetAllOpenGrinForIQCDetails(pagingParameter, searchParams);

                var metadata = new
                {
                    openGrinForIQCDetails.TotalCount,
                    openGrinForIQCDetails.PageSize,
                    openGrinForIQCDetails.CurrentPage,
                    openGrinForIQCDetails.HasNext,
                    openGrinForIQCDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all OpenGrinForIQC details()s");

                var result = _mapper.Map<IEnumerable<OpenGrinForIQCDto>>(openGrinForIQCDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all OpenGrinForIQC";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpenGrinForIQCDetailsbyId(int id)
        {
            ServiceResponse<OpenGrinForIQCDto> serviceResponse = new ServiceResponse<OpenGrinForIQCDto>();

            try
            {
                var openForGrinIQCDetails = await _repository.GetOpenGrinForIQCDetailsbyId(id);
                if (openForGrinIQCDetails == null)
                {
                    _logger.LogError($"OpenForGrinIQC details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OpenForGrinIQC details with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned IQCConfirmation details with id: {id}");
                    List<OpenGrinForIQCItemsDto> openGrinForIQCItemsList = new List<OpenGrinForIQCItemsDto>();
                    var openGrinNumber = openForGrinIQCDetails.OpenGrinNumber;
                    var openGrinForGrinDetails = await _openGrinForGrinRepository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);

                    var openGrinForIQCDto = _mapper.Map<OpenGrinForIQCDto>(openGrinForGrinDetails);
                    openGrinForIQCDto.Id = id;
                    openGrinForIQCDto.OpenGrinForGrinId = openGrinForIQCDto.Id;
                    var openGrinForgrinItems = openGrinForGrinDetails.OpenGrinForGrinItems.Where(x => x.RejectedQty != 0 || x.AcceptedQty != 0).ToList();
                    if (openGrinForgrinItems.Count() != 0)
                    {
                        foreach (var openGrinForGrinItem in openGrinForgrinItems)
                        {
                            OpenGrinForIQCItemsDto openGrinForIQCItemsDto = _mapper.Map<OpenGrinForIQCItemsDto>(openGrinForGrinItem);
                            openGrinForIQCItemsDto.ReferenceSONumbers = _mapper.Map<List<OpenGrinForGrinProjectNumberDto>>(openGrinForGrinItem.OGNProjectNumber);
                            openGrinForIQCItemsDto.Id = openGrinForIQCItemsDto.Id;
                            openGrinForIQCItemsDto.ReceivedQty = openGrinForGrinItem.Qty;
                            openGrinForIQCItemsDto.OpenGrinForGrinItemId = openGrinForGrinItem.Id;
                            openGrinForIQCItemsList.Add(openGrinForIQCItemsDto);
                        }
                    }
                    openGrinForIQCDto.OpenGrinForIQCItems = openGrinForIQCItemsList;
                    serviceResponse.Data = openGrinForIQCDto;
                    serviceResponse.Message = "OpenGrinForIQCDetailsbyId Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetOpenGrinForIQCDetailsbyId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
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
        public async Task<IActionResult> CreateOpenGrinForIQC([FromBody] OpenGrinForIQCPostDto openGrinForIQCPostDto)
        {
            ServiceResponse<OpenGrinForIQCDto> serviceResponse = new ServiceResponse<OpenGrinForIQCDto>();

            try
            {
                string serverKey = GetServerKey();

                if (openGrinForIQCPostDto == null)
                {
                    _logger.LogError(" OpenGrinForIQC details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " OpenGrinForIQC details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid  OpenGrinForIQC details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var openGrinForIQC = _mapper.Map<OpenGrinForIQC>(openGrinForIQCPostDto);
                var openGrinNumber = openGrinForIQC.OpenGrinNumber;
                var openGrinForIQCItemsDto = openGrinForIQCPostDto.OpenGrinForIQCItems;

                var openGrinForIQCItemNo = openGrinForIQCItemsDto[0].ItemNumber;
                var openGrinForIQCItemList = new List<OpenGrinForIQCItems>();

                HttpStatusCode getItemmResp = HttpStatusCode.OK;
                HttpStatusCode createInvfromGrin = HttpStatusCode.OK;
                HttpStatusCode createInvTransfromGrin = HttpStatusCode.OK;
                HttpStatusCode getInvdetailsGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode updateInvTrans = HttpStatusCode.OK;

                var existingOpenGrinForIQCDetails = await _repository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);

                if (existingOpenGrinForIQCDetails != null)
                {

                    for (int i = 0; i < openGrinForIQCItemsDto.Count; i++)
                    {
                        OpenGrinForIQCItems openGrinForIQCItems = _mapper.Map<OpenGrinForIQCItems>(openGrinForIQCItemsDto[i]);
                        openGrinForIQCItems.OpenGrinForIQCId = existingOpenGrinForIQCDetails.Id;
                        var openGrinForGrinItemId = openGrinForIQCItemsDto[i].OpenGrinForGrinItemId;
                        var openGrinForGrinItemDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);
                        if (openGrinForIQCItemsDto[i].OpenGrinForGrinItemId != openGrinForGrinItemDetails.Id)
                        {
                            _logger.LogError($"Invalid OpenGrinForGrin Part Id {openGrinForGrinItemId}");
                            serviceResponse.Data = null;
                            serviceResponse.Message = $"Invalid OpenGrinForGrin Part Id {openGrinForGrinItemId}";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }
                        if (openGrinForGrinItemDetails.Qty <= (openGrinForIQCItemsDto[i].AcceptedQty + openGrinForIQCItemsDto[i].RejectedQty))
                        {
                            openGrinForGrinItemDetails.AcceptedQty = openGrinForIQCItemsDto[i].AcceptedQty;
                            openGrinForGrinItemDetails.RejectedQty = openGrinForIQCItemsDto[i].RejectedQty;
                        }
                        else
                        {
                            _logger.LogError("OpenGrinForGrinItem Quantity should not be lesser than accepted + rejected Quantity .");
                            serviceResponse.Data = null;
                            serviceResponse.Message = "OpenGrinForGrinItem Quantity should not be lesser than accepted + rejected Quantity ";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }

                        //Updating IQC Status in IqcItem Level

                        openGrinForIQCItems.IsOpenGrinForIqcCompleted = true;
                        await _openGrinForIQCItemRepository.CreateOpenGrinForIQCItems(openGrinForIQCItems);
                        _openGrinForIQCItemRepository.SaveAsync();

                        ////Inventory Update Code

                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = openGrinForIQCItemsDto[i].ItemNumber;
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

                        decimal acceptedQty = openGrinForIQCItemsDto[i].AcceptedQty;
                        decimal rejectedQty = openGrinForIQCItemsDto[i].RejectedQty;
                        foreach (var projectNo in openGrinForGrinItemDetails.OGNProjectNumber)
                        {
                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                            var itemNo = openGrinForIQCItemsDto[i].ItemNumber;
                            var encodedItemNo = Uri.EscapeDataString(itemNo);
                            var openGrinForGrinNo = openGrinForIQC.OpenGrinNumber;
                            var encodedOpenGrinForGrinNo = Uri.EscapeDataString(openGrinForGrinNo);
                            var openGrinForGrinItemIds = openGrinForIQCItemsDto[i].OpenGrinForGrinItemId;
                            var referenceSONumber = projectNo.ReferenceSONumber;
                            var encodedreferenceSONumber = Uri.EscapeDataString(referenceSONumber);

                            var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={openGrinForGrinNo}&GrinPartsId={openGrinForGrinItemIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedreferenceSONumber}"));
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
                                    inventoryObject.warehouse = "OPGIQC";
                                    inventoryObject.location = "OPGIQC";
                                    inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                    inventoryObject.referenceIDFrom = "OPGGRIN";
                                    acceptedQty -= balanceQty;

                                }
                                else if (inventoryObject.balance_Quantity > acceptedQty)
                                {
                                    if (acceptedQty == 0)
                                    {
                                        inventoryObject.balance_Quantity = acceptedQty;
                                        inventoryObject.warehouse = "OPGIQC";
                                        inventoryObject.location = "OPGIQC";
                                        inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                        inventoryObject.referenceIDFrom = "OPGGRIN";
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
                                        inventoryObject.warehouse = "OPGIQC";
                                        inventoryObject.location = "OPGIQC";
                                        inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                        inventoryObject.referenceIDFrom = "OPGGRIN";
                                        acceptedQty = 0;

                                    }
                                }
                                if (inventoryObject.balance_Quantity == 0) { inventoryObject.isStockAvailable = 0; }
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
                                iqcInventoryTranctionDto.Warehouse = "OPGIQC";
                                iqcInventoryTranctionDto.From_Location = "OPGGRIN";
                                iqcInventoryTranctionDto.TO_Location = "OPGIQC";
                                iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; 
                                iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                                iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                                iqcInventoryTranctionDto.ReferenceID = inventoryObject.referenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                iqcInventoryTranctionDto.ReferenceIDFrom = inventoryObject.referenceIDFrom;
                                iqcInventoryTranctionDto.GrinMaterialType = "OPGGRIN";
                                iqcInventoryTranctionDto.ShopOrderNo = "";

                                string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                                //var rfqApiUrls = _config["InventoryTranctionAPI"];
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

                                if (openGrinForIQCItemsDto[i].RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                                {
                                    IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                    grinInventoryDto.PartNumber = openGrinForIQCItems.ItemNumber;
                                    grinInventoryDto.LotNumber = openGrinForGrinItemDetails.LotNumber;
                                    grinInventoryDto.MftrPartNumber = itemMasterObject.mftrItemNumber;
                                    grinInventoryDto.Description = itemMasterObject.itemDescription;
                                    grinInventoryDto.ProjectNumber = referenceSONumber;
                                    //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(openGrinForIQCItems.RejectedQty);
                                    grinInventoryDto.Max = itemMasterObject.max;
                                    grinInventoryDto.Min = itemMasterObject.min;
                                    grinInventoryDto.UOM = openGrinForGrinItemDetails.UOM;
                                    grinInventoryDto.Warehouse = "Reject";
                                    grinInventoryDto.Location = "Reject";
                                    grinInventoryDto.GrinNo = openGrinForIQC.OpenGrinNumber;
                                    grinInventoryDto.GrinPartId = openGrinForIQCItems.OpenGrinForGrinItemId;
                                    grinInventoryDto.PartType = itemMasterObject.itemType;
                                    grinInventoryDto.ReferenceID = openGrinForIQC.OpenGrinForIQCNumber; // Convert.ToString(openGrinForIQCItems.Id) //;
                                    grinInventoryDto.ReferenceIDFrom = "OPGIQC";
                                    grinInventoryDto.GrinMaterialType = "OPGGRIN";
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

                                    var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                    if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInvfromGrin = rfqCustomerIdResponse.StatusCode;

                                    //InventoryTranction Update Code

                                    IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                    iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                    iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                    iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                    iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                    iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                    iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                    iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                    iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                    iqcInventoryTranctionDtos.From_Location = "OPGGRIN";
                                    iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                    iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo; ;
                                    iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                    iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                    iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.ReferenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                    iqcInventoryTranctionDtos.ReferenceIDFrom = "OPGIQC";
                                    iqcInventoryTranctionDtos.GrinMaterialType = "OPGGRIN";
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

                            }
                        }

                        ////update accepted qty and rejected qty in OpenGrinForGrin model
                        //Updating IQC Status in OpenGrinForGrinItem

                        var updatedOpenGrinForGrinItemsQty = await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItemsQty
                                                                        (openGrinForIQCItems.OpenGrinForGrinItemId, openGrinForIQCItems.AcceptedQty.ToString(),
                                                                        openGrinForIQCItems.RejectedQty.ToString());

                        var openGrinForGrinItems = _mapper.Map<OpenGrinForGrinItems>(updatedOpenGrinForGrinItemsQty);
                        openGrinForGrinItems.IsOpenGrinForIqcCompleted = true;
                        string result = await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItems);
                        _openGrinForGrinItemRepository.SaveAsync();

                        //Updating binning Status in OpenGrinForGrinItem
                        if (openGrinForIQCItemsDto[i].AcceptedQty == 0)
                        {
                            var openGrinForGrinItemData = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForIQCItemsDto[i].OpenGrinForGrinItemId);
                            openGrinForGrinItemData.IsOpenGrinForBinningCompleted = true;
                            if (openGrinForGrinItemData.Remarks == null || string.IsNullOrWhiteSpace(openGrinForGrinItemData.Remarks))
                            {
                                openGrinForGrinItemData.Remarks = "Iqc Rejected for all";
                            }
                            else
                            {
                                openGrinForGrinItemData.Remarks = openGrinForGrinItemData.Remarks + "[Iqc Rejected for all]";
                            }
                            await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItemData);
                            _openGrinForGrinItemRepository.SaveAsync();
                        }

                    }

                    //Updating IQC Status in Grin

                    var openGrinForGrinitemsIqcStatuscount = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemsIqcStatusCount(openGrinForIQC.OpenGrinForGrinId);

                    if (openGrinForGrinitemsIqcStatuscount == 0)
                    {
                        var openGrinForGrinDetails = await _openGrinForGrinRepository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);
                        openGrinForGrinDetails.IsOpenGrinForIqcCompleted = true;
                        await _openGrinForGrinRepository.UpdateOpenGrinForGrin(openGrinForGrinDetails);
                        _openGrinForGrinRepository.SaveAsync();

                    }

                    //Updating IQC Status in IQC

                    var openGrinForGrinIqcStatuscount = await _openGrinForGrinRepository.GetOpenGrinForGrinIqcStatusCount(openGrinNumber);

                    if (openGrinForGrinIqcStatuscount > 0)
                    {
                        var openGrinForIQCDetails = await _repository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);
                        openGrinForIQCDetails.IsOpenGrinForIqcCompleted = true;
                        await _repository.UpdateOpenGrinForIQC(openGrinForIQCDetails);

                    }
                    if (getItemmResp == HttpStatusCode.OK && createInvfromGrin == HttpStatusCode.OK && createInvTransfromGrin == HttpStatusCode.OK && getInvdetailsGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK)
                    {
                        _repository.SaveAsync();
                        _openGrinForGrinItemRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create OpenGrinForIQC action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }


                    serviceResponse.Data = null;
                    serviceResponse.Message = "OpenGrinForIQC and OpenGrinForIQCItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (serverKey == "avision")
                    {
                        var openGrinNum = openGrinForIQC.OpenGrinNumber;
                        if (openGrinNum != null)
                        {
                            var openGrinForIqcNum = openGrinNum.Replace("OPGN", "OPGNIQC");
                            openGrinForIQC.OpenGrinForIQCNumber = openGrinForIqcNum;
                        }

                    }

                    for (int i = 0; i < openGrinForIQCItemsDto.Count; i++)
                    {
                        OpenGrinForIQCItems openGrinForIQCItems = _mapper.Map<OpenGrinForIQCItems>(openGrinForIQCItemsDto[i]);
                        var openGrinForGrinItemId = openGrinForIQCItemsDto[i].OpenGrinForGrinItemId;

                        var openGrinForGrinItemDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);
                        if (openGrinForGrinItemDetails == null)
                        {
                            _logger.LogError($"Invalid OpenGrinForGrinItem Id {openGrinForGrinItemDetails}");
                            serviceResponse.Data = null;
                            serviceResponse.Message = $"Invalid OpenGrinForGrinItem  Id {openGrinForGrinItemDetails}";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }
                        if (openGrinForGrinItemDetails.Qty <= (openGrinForIQCItemsDto[i].AcceptedQty + openGrinForIQCItemsDto[i].RejectedQty))
                        {
                            openGrinForGrinItemDetails.AcceptedQty = openGrinForIQCItemsDto[i].AcceptedQty;
                            openGrinForGrinItemDetails.RejectedQty = openGrinForIQCItemsDto[i].RejectedQty;
                        }
                        else
                        {
                            _logger.LogError("OpenGrinForGrinItem Quantity should not be lesser than accepted + rejected Quantity .");
                            serviceResponse.Data = null;
                            serviceResponse.Message = "OpenGrinForGrinItem Quantity should not be lesser than accepted + rejected Quantity ";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                            return BadRequest(serviceResponse);
                        }

                        //Updating IQC Status in IqcItem Level

                        openGrinForIQCItems.IsOpenGrinForIqcCompleted = true;
                        openGrinForIQCItemList.Add(openGrinForIQCItems);

                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = openGrinForIQCItemsDto[i].ItemNumber;
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

                        decimal acceptedQty = openGrinForIQCItemsDto[i].AcceptedQty;
                        decimal rejectedQty = openGrinForIQCItemsDto[i].RejectedQty;
                        foreach (var projectNo in openGrinForGrinItemDetails.OGNProjectNumber)
                        {
                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                            var itemNo = openGrinForIQCItemsDto[i].ItemNumber;
                            var encodedItemNo = Uri.EscapeDataString(itemNo);
                            var openGrinForGrinNo = openGrinForIQC.OpenGrinNumber;
                            var encodedOpenGrinForGrinNo = Uri.EscapeDataString(openGrinForGrinNo);
                            var openGrinForGrinItemIds = openGrinForIQCItemsDto[i].OpenGrinForGrinItemId;
                            var referenceSONumber = projectNo.ReferenceSONumber;
                            var encodedreferenceSONumber = Uri.EscapeDataString(referenceSONumber);

                            var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={openGrinForGrinNo}&GrinPartsId={openGrinForGrinItemIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedreferenceSONumber}"));
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
                                    inventoryObject.warehouse = "OPGIQC";
                                    inventoryObject.location = "OPGIQC";
                                    inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                    inventoryObject.referenceIDFrom = "OPGGRIN";
                                    acceptedQty -= balanceQty;

                                }
                                else if (inventoryObject.balance_Quantity > acceptedQty)
                                {
                                    if (acceptedQty == 0)
                                    {
                                        inventoryObject.balance_Quantity = acceptedQty;
                                        inventoryObject.warehouse = "OPGIQC";
                                        inventoryObject.location = "OPGIQC";
                                        inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                        inventoryObject.referenceIDFrom = "OPGGRIN";
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
                                        inventoryObject.warehouse = "OPGIQC";
                                        inventoryObject.location = "OPGIQC";
                                        inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                        inventoryObject.referenceIDFrom = "OPGGRIN";
                                        acceptedQty = 0;


                                    }
                                }
                                if (inventoryObject.balance_Quantity == 0) { inventoryObject.isStockAvailable = 0; }
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
                                iqcInventoryTranctionDto.Warehouse = "OPGIQC";
                                iqcInventoryTranctionDto.From_Location = "OPGGRIN";
                                iqcInventoryTranctionDto.TO_Location = "OPGIQC";
                                iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; ;
                                iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                                iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                                iqcInventoryTranctionDto.ReferenceID = inventoryObject.referenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                iqcInventoryTranctionDto.ReferenceIDFrom = inventoryObject.referenceIDFrom;
                                iqcInventoryTranctionDto.GrinMaterialType = "OPGGRIN";
                                iqcInventoryTranctionDto.ShopOrderNo = "";

                                string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                                //var rfqApiUrls = _config["InventoryTranctionAPI"];
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

                                if (openGrinForIQCItemsDto[i].RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                                {
                                    IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                    grinInventoryDto.PartNumber = openGrinForIQCItems.ItemNumber;
                                    grinInventoryDto.LotNumber = openGrinForGrinItemDetails.LotNumber;
                                    grinInventoryDto.MftrPartNumber = itemMasterObject.mftrItemNumber;
                                    grinInventoryDto.Description = itemMasterObject.itemDescription;
                                    grinInventoryDto.ProjectNumber = referenceSONumber;
                                    //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(openGrinForIQCItems.RejectedQty);
                                    grinInventoryDto.Max = itemMasterObject.max;
                                    grinInventoryDto.Min = itemMasterObject.min;
                                    grinInventoryDto.UOM = openGrinForGrinItemDetails.UOM;
                                    grinInventoryDto.Warehouse = "Reject";
                                    grinInventoryDto.Location = "Reject";
                                    grinInventoryDto.GrinNo = openGrinForIQC.OpenGrinNumber;
                                    grinInventoryDto.GrinPartId = openGrinForIQCItems.OpenGrinForGrinItemId;
                                    grinInventoryDto.PartType = itemMasterObject.itemType;
                                    grinInventoryDto.ReferenceID = openGrinForIQC.OpenGrinForIQCNumber; // Convert.ToString(openGrinForIQCItems.Id) //;
                                    grinInventoryDto.ReferenceIDFrom = "OPGIQC";
                                    grinInventoryDto.GrinMaterialType = "OPGGRIN";
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

                                    var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                    if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInvfromGrin = rfqCustomerIdResponse.StatusCode;

                                    //InventoryTranction Update Code

                                    IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                    iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                    iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                    iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                    iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                    iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                    iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                    iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                    iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                    iqcInventoryTranctionDtos.From_Location = "OPGGRIN";
                                    iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                    iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo; ;
                                    iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                    iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                    iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.ReferenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                    iqcInventoryTranctionDtos.ReferenceIDFrom = "OPGIQC";
                                    iqcInventoryTranctionDtos.GrinMaterialType = "OPGGRIN";
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
                            }
                        }
                        ////update accepted qty and rejected qty in OpenGrinForGrin model
                        //Updating IQC Status in OpenGrinForGrinItem

                        var updatedOpenGrinForGrinItemsQty = await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItemsQty
                                                                        (openGrinForIQCItems.OpenGrinForGrinItemId, openGrinForIQCItems.AcceptedQty.ToString(),
                                                                        openGrinForIQCItems.RejectedQty.ToString());

                        var openGrinForGrinItems = _mapper.Map<OpenGrinForGrinItems>(updatedOpenGrinForGrinItemsQty);
                        openGrinForGrinItems.IsOpenGrinForIqcCompleted = true;
                        string result = await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItems);

                        //Updating binning Status in OpenGrinForGrinItem
                        if (openGrinForIQCItemsDto[i].AcceptedQty == 0)
                        {
                            var openGrinForGrinItemData = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForIQCItemsDto[i].OpenGrinForGrinItemId);
                            openGrinForGrinItemData.IsOpenGrinForBinningCompleted = true;
                            if (openGrinForGrinItemData.Remarks == null || string.IsNullOrWhiteSpace(openGrinForGrinItemData.Remarks))
                            {
                                openGrinForGrinItemData.Remarks = "Iqc Rejected for all";
                            }
                            else
                            {
                                openGrinForGrinItemData.Remarks = openGrinForGrinItemData.Remarks + "[Iqc Rejected for all]";
                            }
                            await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItemData);
                            _openGrinForGrinItemRepository.SaveAsync();
                        }

                    }

                    //Updating IQC Status in IQC
                    openGrinForIQC.OpenGrinForIQCItems = openGrinForIQCItemList;
                    openGrinForIQC.IsOpenGrinForIqcCompleted = true;
                    await _repository.CreateOpenGrinForIQC(openGrinForIQC);



                    //Updating IQC Status in Grin
                    var openGrinForGrinDetails = await _openGrinForGrinRepository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);
                    openGrinForGrinDetails.IsOpenGrinForIqcCompleted = true;
                    await _openGrinForGrinRepository.UpdateOpenGrinForGrin(openGrinForGrinDetails);

                    if (getItemmResp == HttpStatusCode.OK && createInvfromGrin == HttpStatusCode.OK && createInvTransfromGrin == HttpStatusCode.OK && getInvdetailsGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK)
                    {
                        _repository.SaveAsync();
                        _openGrinForGrinRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create CreateOpenGrinForIQC action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = null;
                    serviceResponse.Message = "CreateOpenGrinForIQC Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);


                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Create CreateOpenGrinForIQC action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);


            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOpenGrinForIQCItems([FromBody] OpenGrinForIQCSaveDto openGrinForIQCSaveDto)
        {
            ServiceResponse<OpenGrinForIQCSaveDto> serviceResponse = new ServiceResponse<OpenGrinForIQCSaveDto>();

            try
            {
                string serverKey = GetServerKey();

                if (openGrinForIQCSaveDto is null)
                {
                    _logger.LogError("Create OpenGrinForIQCItems object sent from the client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Create OpenGrinForIQCItems object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Create OpenGrinForIQCItems object sent from the client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Create OpenGrinForIQCItems object sent from the client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

               // var openGrinForIQC = _mapper.Map<OpenGrinForIQC>(openGrinForIQCSaveDto);
                var openGrinForIQC = new OpenGrinForIQC()
                {
                    OpenGrinForGrinId = openGrinForIQCSaveDto.OpenGrinForGrinId,
                    OpenGrinNumber = openGrinForIQCSaveDto.OpenGrinNumber
                };
                var openGrinForIQCItemsDto = openGrinForIQCSaveDto.OpenGrinForIQCItems;
                var openGrinForIQCItems = _mapper.Map<OpenGrinForIQCItems>(openGrinForIQCItemsDto);
                var openGrinNumber = openGrinForIQC.OpenGrinNumber;

                HttpStatusCode getInvGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode getInvTrancGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInvTranc = HttpStatusCode.OK;
                HttpStatusCode createInv = HttpStatusCode.OK;
                HttpStatusCode createInvTrans = HttpStatusCode.OK;
                HttpStatusCode getItemmResp = HttpStatusCode.OK;

                var existingOpenGrinForIQCDetails = await _repository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);

                if (existingOpenGrinForIQCDetails != null)
                {
                    openGrinForIQCItems.OpenGrinForIQCId = existingOpenGrinForIQCDetails.Id;

                    var openGrinForGrinItemId = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                    var openGrinForGrinItemDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);
                    if (openGrinForIQCItemsDto.OpenGrinForGrinItemId != openGrinForGrinItemDetails.Id)
                    {
                        _logger.LogError($"Invalid OpenGrinForGrin Part Id {openGrinForGrinItemId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid OpenGrinForGrin Part Id {openGrinForGrinItemId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }
                    if (openGrinForGrinItemDetails.Qty <= (openGrinForIQCItems.AcceptedQty + openGrinForIQCItems.RejectedQty))
                    {
                        openGrinForGrinItemDetails.AcceptedQty = openGrinForIQCItems.AcceptedQty;
                        openGrinForGrinItemDetails.RejectedQty = openGrinForIQCItems.RejectedQty;
                        //_grinPartsRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError("OpenGrinForGrinItem Quantity should not be lesser than accepted + rejected Quantity .");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "OpenGrinForGrinItem Quantity should not be lesser than accepted + rejected Quantity ";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    //Updating IQC Status in IqcItem

                    openGrinForIQCItems.IsOpenGrinForIqcCompleted = true;
                    await _openGrinForIQCItemRepository.CreateOpenGrinForIQCItems(openGrinForIQCItems);


                    //Updating IQC Status in GrinParts

                    openGrinForGrinItemDetails.IsOpenGrinForIqcCompleted = true;
                    await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItemDetails);
                    _openGrinForGrinItemRepository.SaveAsync();

                    //Updating binning Status in GrinParts
                    if (openGrinForIQCItems.AcceptedQty == 0)
                    {
                        var openGrinForGrinItemData = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForIQCItemsDto.OpenGrinForGrinItemId);
                        openGrinForGrinItemData.IsOpenGrinForBinningCompleted = true;
                        if (openGrinForGrinItemData.Remarks == null || string.IsNullOrWhiteSpace(openGrinForGrinItemData.Remarks))
                        {
                            openGrinForGrinItemData.Remarks = "Iqc Rejected for all";
                        }
                        else
                        {
                            openGrinForGrinItemData.Remarks = openGrinForGrinItemData.Remarks + "[Iqc Rejected for all]";
                        }
                        await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItemData);
                        _openGrinForGrinItemRepository.SaveAsync();
                    }


                    //Updating IQC Status in Grin

                    var openGrinForGrinitemsIqcStatuscount = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemsIqcStatusCount(openGrinForIQC.OpenGrinForGrinId);
                    if (openGrinForGrinitemsIqcStatuscount == 0)
                    {
                        var openGrinForGrinDetails = await _openGrinForGrinRepository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);
                        openGrinForGrinDetails.IsOpenGrinForIqcCompleted = true;
                        await _openGrinForGrinRepository.UpdateOpenGrinForGrin(openGrinForGrinDetails);
                        _openGrinForGrinRepository.SaveAsync();

                    }

                    //Updating IQC Status in IQC

                    var openGrinForGrinIqcStatuscount = await _openGrinForGrinRepository.GetOpenGrinForGrinIqcStatusCount(openGrinNumber);

                    if (openGrinForGrinIqcStatuscount > 0)
                    {
                        var openGrinForIQCDetails = await _repository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);
                        openGrinForIQCDetails.IsOpenGrinForIqcCompleted = true;
                        await _repository.UpdateOpenGrinForIQC(openGrinForIQCDetails);

                    }

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = openGrinForIQCItemsDto.ItemNumber;
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
                    decimal acceptedQty = openGrinForIQCItemsDto.AcceptedQty;
                    decimal rejectedQty = openGrinForIQCItemsDto.RejectedQty;
                    var grinPartsId = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                    var openGrinForGrinItemsDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);

                    foreach (var projectNo in openGrinForGrinItemsDetails.OGNProjectNumber)
                    {
                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = openGrinForIQCItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var openGrinForGrinNo = openGrinForIQC.OpenGrinNumber;
                        var encodedOpenGrinForGrinNo = Uri.EscapeDataString(openGrinForGrinNo);
                        var openGrinForGrinItemIds = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                        var referenceSONumber = projectNo.ReferenceSONumber;
                        var encodedreferenceSONumber = Uri.EscapeDataString(referenceSONumber);


                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedOpenGrinForGrinNo}&GrinPartsId={openGrinForGrinItemIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedreferenceSONumber}"));
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
                                inventoryObject.warehouse = "OPGIQC";
                                inventoryObject.location = "OPGIQC";
                                inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                inventoryObject.referenceIDFrom = "OPGGRIN";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "OPGIQC";
                                    inventoryObject.location = "OPGIQC";
                                    inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                    inventoryObject.referenceIDFrom = "OPGGRIN";
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
                                    inventoryObject.warehouse = "OPGIQC";
                                    inventoryObject.location = "OPGIQC";
                                    inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                    inventoryObject.referenceIDFrom = "OPGGRIN";
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
                            iqcInventoryTranctionDto.Warehouse = "OPGIQC";
                            iqcInventoryTranctionDto.From_Location = "OPGGRIN";
                            iqcInventoryTranctionDto.TO_Location = "OPGIQC";
                            iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; ;
                            iqcInventoryTranctionDto.GrinPartId = inventoryObject.openGrinForGrinItemId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.referenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                            iqcInventoryTranctionDto.ReferenceIDFrom = inventoryObject.referenceIDFrom;
                            iqcInventoryTranctionDto.GrinMaterialType = "OPGGRIN";
                            iqcInventoryTranctionDto.ShopOrderNo = "";

                            string rfqSourcingPPdetailsJsons = JsonConvert.SerializeObject(iqcInventoryTranctionDto);
                            //var rfqApiUrls = _config["InventoryTranctionAPI"];
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

                            if (openGrinForIQCItemsDto.RejectedQty != 0 && acceptedQty == 0 && (flag1 == 1 || flag2 == 1))
                            {
                                IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                grinInventoryDto.PartNumber = openGrinForIQCItems.ItemNumber;
                                grinInventoryDto.LotNumber = openGrinForGrinItemDetails.LotNumber;
                                grinInventoryDto.MftrPartNumber = itemMasterObject.mftrItemNumber;
                                grinInventoryDto.Description = itemMasterObject.itemDescription;
                                grinInventoryDto.ProjectNumber = referenceSONumber;
                                //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(openGrinForIQCItems.RejectedQty);
                                grinInventoryDto.Max = itemMasterObject.max;
                                grinInventoryDto.Min = itemMasterObject.min;
                                grinInventoryDto.UOM = openGrinForGrinItemDetails.UOM;
                                grinInventoryDto.Warehouse = "Reject";
                                grinInventoryDto.Location = "Reject";
                                grinInventoryDto.GrinNo = openGrinForIQC.OpenGrinNumber;
                                grinInventoryDto.GrinPartId = openGrinForIQCItems.OpenGrinForGrinItemId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;
                                grinInventoryDto.ReferenceID = openGrinForIQC.OpenGrinForIQCNumber; // Convert.ToString(openGrinForIQCItems.Id) //;
                                grinInventoryDto.ReferenceIDFrom = "OPGIQC";
                                grinInventoryDto.GrinMaterialType = "OPGGRIN";
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

                                var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInv = rfqCustomerIdResponse.StatusCode;

                                //InventoryTranction Update Code

                                IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo; ;
                                iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.ReferenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "OPGIQC";
                                iqcInventoryTranctionDtos.GrinMaterialType = "OPGGRIN";
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

                        }
                    }
                    ////update accepted qty and rejected qty in grin model

                    var updatedOpenGrinForGrinItemsQty = await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItemsQty
                                                                       (openGrinForIQCItems.OpenGrinForGrinItemId, openGrinForIQCItems.AcceptedQty.ToString(),
                                                                       openGrinForIQCItems.RejectedQty.ToString());

                    var openGrinForGrinItems = _mapper.Map<OpenGrinForGrinItems>(updatedOpenGrinForGrinItemsQty);
                    openGrinForGrinItems.IsOpenGrinForIqcCompleted = true;
                    string result = await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItems);

                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && getInvTrancGrinId == HttpStatusCode.OK && updateInvTranc == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                    {
                        _openGrinForIQCItemRepository.SaveAsync();
                        _openGrinForGrinItemRepository.SaveAsync();
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
                    serviceResponse.Data = null;
                    serviceResponse.Message = "openGrinForIQCItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (serverKey == "avision")
                    {
                        var openGrinNum = openGrinForIQC.OpenGrinNumber;
                        if (openGrinNum != null)
                        {
                            var openGrinForIqcNum = openGrinNum.Replace("OPGN", "OPGNIQC");
                            openGrinForIQC.OpenGrinForIQCNumber = openGrinForIqcNum;
                        }

                    }

                    var openGrinForGrinItemId = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                    var openGrinForGrinItemDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);
                    if (openGrinForIQCItemsDto.OpenGrinForGrinItemId != openGrinForGrinItemDetails.Id)
                    {
                        _logger.LogError($"Invalid OpenGrinForGrinItem Part Id {openGrinForGrinItemId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid OpenGrinForGrinItem Part Id {openGrinForGrinItemId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }
                    if (openGrinForGrinItemDetails.Qty <= (openGrinForIQCItems.AcceptedQty + openGrinForIQCItems.RejectedQty))
                    {
                        openGrinForGrinItemDetails.AcceptedQty = openGrinForIQCItems.AcceptedQty;
                        openGrinForGrinItemDetails.RejectedQty = openGrinForIQCItems.RejectedQty;
                        _openGrinForGrinItemRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError("OpenGrinForGrinItem Quantity should not be lesser than accepted + rejected Quantity .");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "OpenGrinForGrinItem Quantity should not be lesser than accepted + rejected Quantity ";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    //Updating IQC Status in IqcItem

                    openGrinForIQCItems.IsOpenGrinForIqcCompleted = true;
                    openGrinForIQC.OpenGrinForIQCItems = new List<OpenGrinForIQCItems> { openGrinForIQCItems };
                    await _repository.CreateOpenGrinForIQC(openGrinForIQC);

                    //Updating IQC Status in GrinParts

                    openGrinForGrinItemDetails.IsOpenGrinForIqcCompleted = true;
                    await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItemDetails);
                    _openGrinForGrinItemRepository.SaveAsync();

                    //Updating binning Status in OpenGrinForGrinItem
                    if (openGrinForIQCItemsDto.AcceptedQty == 0)
                    {
                        var openGrinForGrinItemData = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForIQCItemsDto.OpenGrinForGrinItemId);
                        openGrinForGrinItemData.IsOpenGrinForBinningCompleted = true;
                        if (openGrinForGrinItemData.Remarks == null || string.IsNullOrWhiteSpace(openGrinForGrinItemData.Remarks))
                        {
                            openGrinForGrinItemData.Remarks = "Iqc Rejected for all";
                        }
                        else
                        {
                            openGrinForGrinItemData.Remarks = openGrinForGrinItemData.Remarks + "[Iqc Rejected for all]";
                        }
                        await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItemData);
                        _openGrinForGrinItemRepository.SaveAsync();
                    }



                    //Updating IQC Status in Grin

                    var openGrinForGrinitemsIqcStatuscount = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemsIqcStatusCount(openGrinForIQC.OpenGrinForGrinId);

                    if (openGrinForGrinitemsIqcStatuscount == 0)
                    {
                        var openGrinForGrinDetails = await _openGrinForGrinRepository.GetOpenGrinForGrinDetailsByOpenGrinNo(openGrinNumber);
                        openGrinForGrinDetails.IsOpenGrinForIqcCompleted = true;
                        await _openGrinForGrinRepository.UpdateOpenGrinForGrin(openGrinForGrinDetails);
                        _openGrinForGrinRepository.SaveAsync();

                    }

                    //Updating IQC Status in IQC

                    var openGrinForGrinIqcStatuscount = await _openGrinForGrinRepository.GetOpenGrinForGrinIqcStatusCount(openGrinNumber);

                    if (openGrinForGrinIqcStatuscount > 0)
                    {
                        var openGrinForIQCDetails = await _repository.GetOpenGrinForIQCDetailsbyOpenGrinNo(openGrinNumber);
                        openGrinForIQCDetails.IsOpenGrinForIqcCompleted = true;
                        await _repository.UpdateOpenGrinForIQC(openGrinForIQCDetails);

                    }

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = openGrinForIQCItemsDto.ItemNumber;
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
                    decimal acceptedQty = openGrinForIQCItemsDto.AcceptedQty;
                    var openGrinForGrinPartsId = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                    var openGrinForGrinItemsDetails = await _openGrinForGrinItemRepository.GetOpenGrinForGrinItemDetailsbyOpenGrinForGrinItemId(openGrinForGrinItemId);

                    foreach (var projectNo in openGrinForGrinItemsDetails.OGNProjectNumber)
                    {
                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = openGrinForIQCItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var openGrinForGrinNo = openGrinForIQC.OpenGrinNumber;
                        var encodedOpenGrinForGrinNo = Uri.EscapeDataString(openGrinForGrinNo);
                        var openGrinForGrinItemIds = openGrinForIQCItemsDto.OpenGrinForGrinItemId;
                        var referenceSONumber = projectNo.ReferenceSONumber;
                        var encodedreferenceSONumber = Uri.EscapeDataString(referenceSONumber);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetInventoryDetailsByGrinNoandGrinId?GrinNo={encodedOpenGrinForGrinNo}&GrinPartsId={openGrinForGrinItemIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedreferenceSONumber}"));
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
                                inventoryObject.warehouse = "OPGIQC";
                                inventoryObject.location = "OPGIQC";
                                inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                inventoryObject.referenceIDFrom = "OPGGRIN";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "OPGIQC";
                                    inventoryObject.location = "OPGIQC";
                                    inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                    inventoryObject.referenceIDFrom = "OPGGRIN";
                                    inventoryObject.isStockAvailable = false;
                                }
                                else
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "OPGIQC";
                                    inventoryObject.location = "OPGIQC";
                                    inventoryObject.referenceID = openGrinForIQC.OpenGrinForIQCNumber;
                                    inventoryObject.referenceIDFrom = "OPGGRIN";
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
                            iqcInventoryTranctionDto.Warehouse = "OPGIQC";
                            iqcInventoryTranctionDto.From_Location = "OPGGRIN";
                            iqcInventoryTranctionDto.TO_Location = "OPGIQC";
                            iqcInventoryTranctionDto.GrinNo = inventoryObject.grinNo; ;
                            iqcInventoryTranctionDto.GrinPartId = inventoryObject.grinPartId;
                            iqcInventoryTranctionDto.PartType = inventoryObject.partType;
                            iqcInventoryTranctionDto.ReferenceID = inventoryObject.referenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                            iqcInventoryTranctionDto.ReferenceIDFrom = inventoryObject.referenceIDFrom;
                            iqcInventoryTranctionDto.GrinMaterialType = "OPGGRIN";
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

                            if (openGrinForIQCItemsDto.RejectedQty != 0 && acceptedQty == 0)
                            {
                                IQCInventoryDto grinInventoryDto = new IQCInventoryDto();
                                grinInventoryDto.PartNumber = openGrinForIQCItems.ItemNumber;
                                grinInventoryDto.LotNumber = openGrinForGrinItemDetails.LotNumber;
                                grinInventoryDto.MftrPartNumber = itemMasterObject.mftrItemNumber;
                                grinInventoryDto.Description = itemMasterObject.itemDescription;
                                grinInventoryDto.ProjectNumber = referenceSONumber;
                                //grinInventoryDto.Balance_Quantity = Convert.ToDecimal(openGrinForIQCItems.RejectedQty);
                                grinInventoryDto.Max = itemMasterObject.max;
                                grinInventoryDto.Min = itemMasterObject.min;
                                grinInventoryDto.UOM = openGrinForGrinItemDetails.UOM;
                                grinInventoryDto.Warehouse = "Reject";
                                grinInventoryDto.Location = "Reject";
                                grinInventoryDto.GrinNo = openGrinForIQC.OpenGrinNumber;
                                grinInventoryDto.GrinPartId = openGrinForIQCItems.OpenGrinForGrinItemId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;
                                grinInventoryDto.ReferenceID = openGrinForIQC.OpenGrinForIQCNumber; // Convert.ToString(openGrinForIQCItems.Id) //;
                                grinInventoryDto.ReferenceIDFrom = "OPGIQC";
                                grinInventoryDto.GrinMaterialType = "OPGGRIN";
                                grinInventoryDto.ShopOrderNo = "";

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

                                var rfqCustomerIdResponse = await client6.SendAsync(request6);
                                if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInv = rfqCustomerIdResponse.StatusCode;

                                //InventoryTranction Update Code

                                IQCInventoryTranctionDto iqcInventoryTranctionDtos = new IQCInventoryTranctionDto();
                                iqcInventoryTranctionDtos.PartNumber = grinInventoryDto.PartNumber;
                                iqcInventoryTranctionDtos.LotNumber = grinInventoryDto.LotNumber;
                                iqcInventoryTranctionDtos.MftrPartNumber = grinInventoryDto.MftrPartNumber;
                                iqcInventoryTranctionDtos.Description = grinInventoryDto.Description;
                                iqcInventoryTranctionDtos.ProjectNumber = grinInventoryDto.ProjectNumber;
                                iqcInventoryTranctionDtos.Issued_Quantity = grinInventoryDto.Balance_Quantity;
                                iqcInventoryTranctionDtos.UOM = grinInventoryDto.UOM;
                                iqcInventoryTranctionDtos.Warehouse = grinInventoryDto.Warehouse;
                                iqcInventoryTranctionDtos.From_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.TO_Location = grinInventoryDto.Location;
                                iqcInventoryTranctionDtos.GrinNo = grinInventoryDto.GrinNo; ;
                                iqcInventoryTranctionDtos.GrinPartId = grinInventoryDto.GrinPartId;
                                iqcInventoryTranctionDtos.PartType = grinInventoryDto.PartType;
                                iqcInventoryTranctionDtos.ReferenceID = grinInventoryDto.ReferenceID;/* Convert.ToString(openGrinForGrinItemDetails.Id);*/
                                iqcInventoryTranctionDtos.ReferenceIDFrom = "OPGIQC";
                                iqcInventoryTranctionDtos.GrinMaterialType = "OPGGRIN";
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



                        }
                    }

                ////update accepted qty and rejected qty in grin model

                var updatedOpenGrinForGrinItemsQty = await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItemsQty
                                                                    (openGrinForIQCItems.OpenGrinForGrinItemId, openGrinForIQCItems.AcceptedQty.ToString(),
                                                                    openGrinForIQCItems.RejectedQty.ToString());

                var openGrinForGrinItems = _mapper.Map<OpenGrinForGrinItems>(updatedOpenGrinForGrinItemsQty);
                openGrinForGrinItems.IsOpenGrinForIqcCompleted = true;
                string result = await _openGrinForGrinItemRepository.UpdateOpenGrinForGrinItem(openGrinForGrinItems);

                if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && getInvTrancGrinId == HttpStatusCode.OK && updateInvTranc == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK && getItemmResp == HttpStatusCode.OK)
                {
                    _openGrinForGrinItemRepository.SaveAsync();
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

                serviceResponse.Data = null;
                serviceResponse.Message = "openGrinForIQC and openGrinForIQCItems Created Successfully";
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
