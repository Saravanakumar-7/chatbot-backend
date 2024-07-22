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
    public class GrinsForServiceItemsController : ControllerBase
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private IGrinsForServiceItemsPartsRepository _grinPartsRepository;
        private IGrinsForServiceItemsRepository _repository;
        private IIQCForServiceItemsRepository _iQCForServiceItemsRepository;
        private IIQCForServiceItems_ItemsRepository _iQCForServiceItems_ItemsRepository;
        private IDocumentUploadRepository _documentUploadRepository;

        public GrinsForServiceItemsController(IMapper mapper, ILoggerManager logger, IGrinsForServiceItemsRepository repository, IDocumentUploadRepository documentUploadRepository, IIQCForServiceItems_ItemsRepository iQCForServiceItems_ItemsRepository, IIQCForServiceItemsRepository iQCForServiceItemsRepository, IHttpClientFactory clientFactory, IConfiguration config, IGrinsForServiceItemsPartsRepository grinPartsRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _config = config;
            _clientFactory = clientFactory;
            _grinPartsRepository = grinPartsRepository;
            _iQCForServiceItemsRepository = iQCForServiceItemsRepository;
            _iQCForServiceItems_ItemsRepository = iQCForServiceItems_ItemsRepository;
            _documentUploadRepository = documentUploadRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGrinsForServiceItems([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)

        {
            ServiceResponse<IEnumerable<GrinsForServiceItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinsForServiceItemsDto>>();

            try
            {
                var GrinsForServiceItemsForServiceItems = await _repository.GrinsForServiceItemsForServiceItems(pagingParameter, searchParams);

                if (GrinsForServiceItemsForServiceItems == null || GrinsForServiceItemsForServiceItems.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GrinsForServiceItems data not found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GrinsForServiceItems data not found in db");
                    return NotFound(serviceResponse);
                }
                var metadata = new
                {
                    GrinsForServiceItemsForServiceItems.TotalCount,
                    GrinsForServiceItemsForServiceItems.PageSize,
                    GrinsForServiceItemsForServiceItems.CurrentPage,
                    GrinsForServiceItemsForServiceItems.HasNext,
                    GrinsForServiceItemsForServiceItems.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all GrinsForServiceItems");
                var result = _mapper.Map<IEnumerable<GrinsForServiceItemsDto>>(GrinsForServiceItemsForServiceItems);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GrinsForServiceItems Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Internal server error {ex.Message}{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGrinsForServiceItemsById(int id)
        {
            ServiceResponse<GrinsForServiceItemsItemMasterEnggDto> serviceResponse = new ServiceResponse<GrinsForServiceItemsItemMasterEnggDto>();
            try
            {
                var GrinDetailsbyId = await _repository.GetGrinsForServiceItemsById(id);

                if (GrinDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GrinsForServiceItems with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GrinsForServiceItems with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned GrinsForServiceItemsDetailsById with id: {id}");
                    GrinsForServiceItemsItemMasterEnggDto grinItemMasterEnggDto = _mapper.Map<GrinsForServiceItemsItemMasterEnggDto>(GrinDetailsbyId);
                    List<GrinsForServiceItemsPartsItemMasterEnggDto> grinPartsItemMasterEnggList = new List<GrinsForServiceItemsPartsItemMasterEnggDto>();
                    foreach (var GrinpartsDetails in GrinDetailsbyId.GrinsForServiceItemsParts)
                    {
                        GrinsForServiceItemsPartsItemMasterEnggDto grinPartsItemMasterEnggDto = _mapper.Map<GrinsForServiceItemsPartsItemMasterEnggDto>(GrinpartsDetails);
                        grinPartsItemMasterEnggDto.GrinsForServiceItemsProjectNumbers = _mapper.Map<List<GrinsForServiceItemsProjectNumbersDto>>(GrinpartsDetails.GrinsForServiceItemsProjectNumbers);
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = grinPartsItemMasterEnggDto.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterDetails = await client.SendAsync(request);
                        var inventoryObjectString = await itemMasterDetails.Content.ReadAsStringAsync();
                        dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                        dynamic inventoryObject = inventoryObjectData.data;
                        grinPartsItemMasterEnggDto.DrawingNo = inventoryObject.drawingNo;
                        grinPartsItemMasterEnggDto.DocRet = inventoryObject.docRet;
                        grinPartsItemMasterEnggDto.RevNo = inventoryObject.revNo;
                        grinPartsItemMasterEnggDto.IsCocRequired = inventoryObject.isCocRequired;
                        grinPartsItemMasterEnggDto.IsRohsItem = inventoryObject.isRohsItem;
                        grinPartsItemMasterEnggDto.IsShelfLife = inventoryObject.isShelfLife;
                        grinPartsItemMasterEnggDto.IsReachItem = inventoryObject.isReachItem;

                        grinPartsItemMasterEnggList.Add(grinPartsItemMasterEnggDto);

                    }

                    grinItemMasterEnggDto.GrinsForServiceItemsParts = grinPartsItemMasterEnggList;
                    serviceResponse.Data = grinItemMasterEnggDto;
                    serviceResponse.Message = $"Returned GrinsForServiceItems with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetGrinsForServiceItemsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGrinsForServiceItems([FromBody] GrinsForServiceItemsPostDto grinsForServiceItemsPostDto)
        {
            ServiceResponse<GrinsForServiceItemsDto> serviceResponse = new ServiceResponse<GrinsForServiceItemsDto>();

            try
            {
                string serverKey = GetServerKey();

                if (grinsForServiceItemsPostDto is null)
                {
                    _logger.LogError("GrinsForServiceItems object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GrinsForServiceItems object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid GrinsForServiceItems object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid GrinsForServiceItems object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var grinsForServiceItems = _mapper.Map<GrinsForServiceItems>(grinsForServiceItemsPostDto);

                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));
                HttpStatusCode invStatusCode = HttpStatusCode.OK;

                if (serverKey == "avision")
                {
                    var grinNum = await _repository.GenerateGrinsForServiceItemsNumberForAvision();
                    grinsForServiceItems.GrinsForServiceItemsNumber = grinNum;
                }
                else
                {
                    var dateFormat = days + months + years;
                    var grinNumber = await _repository.GenerateGrinsForServiceItemsNumber();
                    var grinNo = dateFormat + grinNumber;
                    grinsForServiceItems.GrinsForServiceItemsNumber = grinNo;
                }
                var grinPartsDto = grinsForServiceItemsPostDto.GrinsForServiceItemsParts;
                var grinCal = _mapper.Map<List<GrinsForServiceItemsPartscalculationofAvgcost>>(grinPartsDto);
                List<GrinsForServiceItemsParts> grinPartsList = new List<GrinsForServiceItemsParts>();

                var othercosttotal = grinsForServiceItems.Freight + grinsForServiceItems.Insurance + grinsForServiceItems.LoadingorUnLoading + grinsForServiceItems.Transport;
                decimal? conversionrate = 1;
                if (grinsForServiceItems.CurrencyConversion > 1) conversionrate = grinsForServiceItems.CurrencyConversion;
                foreach (var gPart in grinCal)
                {
                    decimal? EP = gPart.Qty * gPart.UnitPrice;
                    decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
                    gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
                    gPart.EPforSingleQty = gPart.EPwithTax / gPart.Qty;
                }
                decimal? SumofEPwithtax = grinCal.Sum(x => x.EPwithTax);
                foreach (var gPart in grinCal)
                {
                    decimal? distriduteOthercostforitem = (gPart.EPwithTax / SumofEPwithtax) * othercosttotal;
                    decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / gPart.Qty;
                    gPart.AverageCost = distriduteOthercostforitemsSingleQty + gPart.EPforSingleQty;
                    GrinsForServiceItemsParts grinParts = _mapper.Map<GrinsForServiceItemsParts>(gPart);
                    grinPartsList.Add(grinParts);
                }

                grinsForServiceItems.GrinsForServiceItemsParts = grinPartsList;
                grinsForServiceItems.IsGrinsForServiceItemsCompleted = true;

                await _repository.CreateGrinsForServiceItems(grinsForServiceItems);
                _repository.SaveAsync();



                if (grinsForServiceItems.GrinsForServiceItemsParts != null)
                {
                    foreach (var grinPart in grinsForServiceItems.GrinsForServiceItemsParts)
                    {
                        var grinPartsId = await _grinPartsRepository.GetGrinsForServiceItemsPartsById(grinPart.Id);
                        grinPartsId.LotNumber = grinsForServiceItems.GrinsForServiceItemsNumber + grinPartsId.Id;
                        await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsId);

                    }
                }
                // HttpStatusCode createinvforServiceItemsResp = HttpStatusCode.OK;
                HttpStatusCode createinvResp = HttpStatusCode.OK;
                HttpStatusCode createinvTrancResp = HttpStatusCode.OK;
                HttpStatusCode getItemmResp = HttpStatusCode.OK;
                HttpStatusCode getItemForIqcResp = HttpStatusCode.OK;
                HttpStatusCode UpdatePoStatus = HttpStatusCode.OK;
                HttpStatusCode UpdatePoQty = HttpStatusCode.OK;
                HttpStatusCode UpdatePoProjQty = HttpStatusCode.OK;

                foreach (var parts in grinPartsList)
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = parts.ItemNumber;
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

                    if (parts.GrinsForServiceItemsProjectNumbers != null)
                    {
                        foreach (var project in parts.GrinsForServiceItemsProjectNumbers)
                        {
                            GrinInventoryDto grinInventoryDto = new GrinInventoryDto();
                            grinInventoryDto.PartNumber = parts.ItemNumber;
                            grinInventoryDto.LotNumber = parts.LotNumber;
                            grinInventoryDto.MftrPartNumber = parts.MftrItemNumber;
                            grinInventoryDto.Description = parts.ItemDescription;
                            grinInventoryDto.ProjectNumber = project.ProjectNumber;
                            grinInventoryDto.Balance_Quantity = Convert.ToDecimal(project.ProjectQty);
                            grinInventoryDto.Max = itemMasterObject.max;
                            grinInventoryDto.Min = itemMasterObject.min;
                            grinInventoryDto.UOM = parts.UOM;
                            grinInventoryDto.Warehouse = "GRIN";
                            grinInventoryDto.Location = "GRIN";
                            grinInventoryDto.GrinNo = grinsForServiceItems.GrinsForServiceItemsNumber;
                            grinInventoryDto.GrinPartId = parts.Id;
                            grinInventoryDto.PartType = parts.ItemType;
                            grinInventoryDto.ReferenceID = grinsForServiceItems.GrinsForServiceItemsNumber;
                            grinInventoryDto.ReferenceIDFrom = "GrinsForServiceItems";
                            grinInventoryDto.GrinMaterialType = "";
                            grinInventoryDto.ShopOrderNo = "";

                            var json = JsonConvert.SerializeObject(grinInventoryDto);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryForServiceItemsAPI"],
                            "CreateInventoryForServiceItemsFromGrin"))
                            {
                                Content = data
                            };
                            request1.Headers.Add("Authorization", token1);

                            var response = await client1.SendAsync(request1);
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                createinvResp = response.StatusCode;
                            }
                        }
                    }

                    if (parts.GrinsForServiceItemsProjectNumbers != null)
                    {
                        foreach (var project in parts.GrinsForServiceItemsProjectNumbers)
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
                            grinInventoryTranctionDto.GrinNo = grinsForServiceItems.GrinsForServiceItemsNumber;
                            grinInventoryTranctionDto.GrinPartId = parts.Id;
                            grinInventoryTranctionDto.PartType = parts.ItemType;
                            grinInventoryTranctionDto.ReferenceID = grinsForServiceItems.GrinsForServiceItemsNumber;
                            grinInventoryTranctionDto.ReferenceIDFrom = "GrinsForServiceItems";
                            grinInventoryTranctionDto.GrinMaterialType = "";
                            grinInventoryTranctionDto.ShopOrderNo = "";
                            grinInventoryTranctionDto.IsStockAvailable = true;

                            var json = JsonConvert.SerializeObject(grinInventoryTranctionDto);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranctionFromGrin"))
                            {
                                Content = data
                            };
                            request1.Headers.Add("Authorization", token1);

                            var response = await client1.SendAsync(request1);
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                createinvTrancResp = response.StatusCode;
                            }
                        }
                    }
                }

                var grinPartsDetail = _mapper.Map<List<GrinsForServiceItemsUpdateQtyDetailsDto>>(grinPartsDto);
                var jsons = JsonConvert.SerializeObject(grinPartsDetail);
                var data1 = new StringContent(jsons, Encoding.UTF8, "application/json");
                var client2 = _clientFactory.CreateClient();
                var token2 = HttpContext.Request.Headers["Authorization"].ToString();
                var request2 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"],
                "UpdateBalanceQtyDetails"))
                {
                    Content = data1
                };
                request2.Headers.Add("Authorization", token2);

                var responses = await client2.SendAsync(request2);
                if (responses.StatusCode != HttpStatusCode.OK)
                {
                    UpdatePoQty = responses.StatusCode;
                }
                if (grinPartsDto.Count() > 0)
                {
                    foreach (var grinparts in grinPartsDto)
                    {
                        List<GrinsForServiceItemsUpdateProjectBalQtyDetailsDto> projectNameDtos = new List<GrinsForServiceItemsUpdateProjectBalQtyDetailsDto>();
                        foreach (var projectNo in grinparts.GrinsForServiceItemsProjectNumbers)
                        {
                            var grinPartsProjectNoDtoDetail = _mapper.Map<GrinsForServiceItemsUpdateProjectBalQtyDetailsDto>(projectNo);
                            grinPartsProjectNoDtoDetail.ItemNumber = grinparts.ItemNumber;
                            grinPartsProjectNoDtoDetail.PoItemId = grinparts.PoItemId;
                            projectNameDtos.Add(grinPartsProjectNoDtoDetail);
                        }

                        var jsonss = JsonConvert.SerializeObject(projectNameDtos);
                        var data2 = new StringContent(jsonss, Encoding.UTF8, "application/json");
                        var client3 = _clientFactory.CreateClient();
                        var token3 = HttpContext.Request.Headers["Authorization"].ToString();
                        var request3 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"],
                        "UpdatePoProjectNoBalanceQtyDetails"))
                        {
                            Content = data2
                        };
                        request3.Headers.Add("Authorization", token3);

                        var results = await client3.SendAsync(request3);
                        if (results.StatusCode != HttpStatusCode.OK)
                        {
                            UpdatePoProjQty = results.StatusCode;
                        }
                    }
                }

                var grinPartsDetails = _mapper.Map<List<GrinsForServiceItemsQtyPoStatusUpdateDto>>(grinPartsDto);
                var jsonCon = JsonConvert.SerializeObject(grinPartsDetails);
                var data3 = new StringContent(jsonCon, Encoding.UTF8, "application/json");
                var client4 = _clientFactory.CreateClient();
                var token4 = HttpContext.Request.Headers["Authorization"].ToString();
                var request4 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["PurchaseAPI"],
                "UpdatePoStatus"))
                {
                    Content = data3
                };
                request4.Headers.Add("Authorization", token4);

                var result = await client4.SendAsync(request4);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    UpdatePoStatus = result.StatusCode;
                }
                // createinvforServiceItemsResp == HttpStatusCode.OK &&
                if (getItemmResp == HttpStatusCode.OK && UpdatePoStatus == HttpStatusCode.OK && UpdatePoQty == HttpStatusCode.OK
                    && UpdatePoProjQty == HttpStatusCode.OK && createinvTrancResp == HttpStatusCode.OK && createinvResp == HttpStatusCode.OK)
                {
                    _repository.SaveAsync();
                    _grinPartsRepository.SaveAsync();

                    if (grinsForServiceItems.GrinsForServiceItemsParts != null)
                    {
                        List<string> grinPartsItemNoListDtos = new List<string>();
                        foreach (var grinParts in grinsForServiceItems.GrinsForServiceItemsParts)
                        {
                            var grinPartsProjectNoDtoDetail = _mapper.Map<string>(grinParts.ItemNumber);
                            grinPartsItemNoListDtos.Add(grinPartsProjectNoDtoDetail);
                        }

                        var jsonss = JsonConvert.SerializeObject(grinPartsItemNoListDtos);
                        var data4 = new StringContent(jsonss, Encoding.UTF8, "application/json");
                        var client5 = _clientFactory.CreateClient();
                        var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                        var request5 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["ItemMasterEnggAPI"],
                        "GetAllClosedIqcItemMasterItemNoList"))
                        {
                            Content = data4
                        };
                        request5.Headers.Add("Authorization", token5);

                        var itemMasterObjectResult = await client5.SendAsync(request5);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                        {
                            getItemForIqcResp = itemMasterObjectResult.StatusCode;
                        }
                        if (itemMasterObjectResult.StatusCode == HttpStatusCode.OK)
                        {
                            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                            dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                            dynamic itemMasterObject = itemMasterObjectData;

                            if (itemMasterObject != null && itemMasterObject.Count > 0)
                            {
                                for (int i = 0; i < grinsForServiceItems.GrinsForServiceItemsParts.Count; i++)
                                {
                                    var grinPartItemNo = grinsForServiceItems.GrinsForServiceItemsParts[i].ItemNumber;
                                    for (int j = 0; j < itemMasterObject.Count; j++)
                                    {
                                        if (itemMasterObject[j] == grinPartItemNo)
                                        {
                                            var iqcGrinDetails = _mapper.Map<GrinsForServiceItemsIQCForServiceItemsSaveDto>(grinsForServiceItems);
                                            iqcGrinDetails.GrinsForServiceItemsId = grinsForServiceItems.Id;
                                            iqcGrinDetails.GrinsForServiceItemsNumber = grinsForServiceItems.GrinsForServiceItemsNumber;

                                            GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto grinIQCForServiceItemsItemsSaveDto = _mapper.Map<GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto>(grinsForServiceItems.GrinsForServiceItemsParts[i]);
                                            grinIQCForServiceItemsItemsSaveDto.ItemNumber = grinsForServiceItems.GrinsForServiceItemsParts[i].ItemNumber;
                                            grinIQCForServiceItemsItemsSaveDto.GrinsForServiceItemsPartId = grinsForServiceItems.GrinsForServiceItemsParts[i].Id;
                                            grinIQCForServiceItemsItemsSaveDto.ReceivedQty = grinsForServiceItems.GrinsForServiceItemsParts[i].Qty;
                                            grinIQCForServiceItemsItemsSaveDto.AcceptedQty = grinsForServiceItems.GrinsForServiceItemsParts[i].Qty;
                                            grinIQCForServiceItemsItemsSaveDto.RejectedQty = grinsForServiceItems.GrinsForServiceItemsParts[i].RejectedQty;
                                            iqcGrinDetails.GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto = grinIQCForServiceItemsItemsSaveDto;

                                            await CreateIQCGrinsForServiceItems_Items(iqcGrinDetails);

                                        }
                                    }
                                }
                            }

                        }

                    }

                }
                else
                {
                    _logger.LogError($"Something went wrong inside Create CreateGrinsForServiceItems action: Other Service Calling");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Saving Failed";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                serviceResponse.Data = null;
                serviceResponse.Message = "GrinsForServiceItems Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetGrinById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateGrinsForServiceItems action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        private async Task<IActionResult> CreateIQCGrinsForServiceItems_Items([FromBody] GrinsForServiceItemsIQCForServiceItemsSaveDto grinsForServiceItemsIQCForServiceItemsSaveDto)
        {
            ServiceResponse<GrinsForServiceItemsIQCForServiceItemsSaveDto> serviceResponse = new ServiceResponse<GrinsForServiceItemsIQCForServiceItemsSaveDto>();

            try
            {
                string serverKey = GetServerKey();

                if (grinsForServiceItemsIQCForServiceItemsSaveDto is null)
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

                var iqcForServiceItems = _mapper.Map<IQCForServiceItems>(grinsForServiceItemsIQCForServiceItemsSaveDto);
                var iqcConfirmationItemsDto = grinsForServiceItemsIQCForServiceItemsSaveDto.GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto;
                var iqcforserviceitems_items = _mapper.Map<IQCForServiceItems_Items>(iqcConfirmationItemsDto);
                var grinsForServiceItemsNumber = iqcForServiceItems.GrinsForServiceItemsNumber;
                var GrinsForServiceItemsId = iqcForServiceItems.GrinsForServiceItemsId;
                var existingIqcConfirmation = await _iQCForServiceItemsRepository.GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(grinsForServiceItemsNumber);

                if (existingIqcConfirmation != null)
                {
                    iqcforserviceitems_items.IQCForServiceItemsId = existingIqcConfirmation.Id;

                    var grinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                    var grinPartsDetails = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(grinPartId);
                    if (iqcConfirmationItemsDto.GrinsForServiceItemsPartId != grinPartsDetails.Id)
                    {
                        _logger.LogError($"Invalid GrinsForServiceItemsPart Id {grinPartId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid GrinsForServiceItemsPart Id {grinPartId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    iqcforserviceitems_items.IsIqcForServiceItemsCompleted = true;
                    await _iQCForServiceItems_ItemsRepository.CreateIqcForServiceItems_Items(iqcforserviceitems_items);
                    _iQCForServiceItems_ItemsRepository.SaveAsync();
                    grinPartsDetails.IsIqcForServiceItemsCompleted = true;

                    //await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsDetails);
                    //_grinPartsRepository.SaveAsync();

                    var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinsForServiceItemsPartsQty(iqcforserviceitems_items.GrinsForServiceItemsPartsId, iqcforserviceitems_items.AcceptedQty.ToString(), iqcforserviceitems_items.RejectedQty.ToString());
                    var grinParts = _mapper.Map<GrinsForServiceItemsParts>(updatedGrinPartsQty);
                    grinParts.RejectedQty = grinsForServiceItemsIQCForServiceItemsSaveDto.GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto.RejectedQty;
                    grinParts.AcceptedQty = grinsForServiceItemsIQCForServiceItemsSaveDto.GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto.AcceptedQty ?? 0m;
                    grinParts.IsIqcForServiceItemsCompleted = true;
                    await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinParts);
                    _grinPartsRepository.SaveAsync();

                    var grinPartsCount = await _grinPartsRepository.GetGrinsForServiceItemsPartsCount(GrinsForServiceItemsId);
                    var iqcConfomationCount = await _iQCForServiceItems_ItemsRepository.GetIQCForServiceItems_ItemsCount(existingIqcConfirmation.Id);

                    if (grinPartsCount == iqcConfomationCount)
                    {
                        var iqcDetails = await _iQCForServiceItemsRepository.GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(grinsForServiceItemsNumber);
                        iqcDetails.IsIqcForServiceItemsCompleted = true;
                        _iQCForServiceItemsRepository.SaveAsync();

                        var grinDetails = await _repository.GetGrinForServiceItemsByGrinForServiceItemsNo(grinsForServiceItemsNumber);
                        grinDetails.IsIqcForServiceItemsCompleted = true;
                        _repository.SaveAsync();
                    }
                    HttpStatusCode getInvGrinsForServiceItemsId = HttpStatusCode.OK;
                    HttpStatusCode updateInv = HttpStatusCode.OK;
                    HttpStatusCode getInvTrancGrinsForServiceItemsId = HttpStatusCode.OK;
                    HttpStatusCode updateInvTranc = HttpStatusCode.OK;
                    HttpStatusCode createInvTranc = HttpStatusCode.OK;
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = iqcConfirmationItemsDto.ItemNumber;
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;
                    //Inventory Update Code
                    decimal? acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    decimal rejectedQty = iqcConfirmationItemsDto.RejectedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.GrinsForServiceItemsProjectNumbers)
                    {
                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var grinNo = iqcForServiceItems.GrinsForServiceItemsNumber;
                        var encodedgrinNo = Uri.EscapeDataString(grinNo);
                        var grinPartsIds = projectNo.GrinsForServiceItemsPartsId;
                        var projectNos = projectNo.ProjectNumber;
                        var encodedprojectNos = Uri.EscapeDataString(projectNos);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryForServiceItemsAPI"],
                            $"GetInventoryForServiceDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                        request1.Headers.Add("Authorization", token1);

                        var inventoryObjectResult = await client1.SendAsync(request1);
                        if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinsForServiceItemsId = inventoryObjectResult.StatusCode;
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
                                inventoryObject.referenceIDFrom = "GrinsForServiceItems";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceIDFrom = "GrinsForServiceItems";
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
                                    inventoryObject.referenceIDFrom = "GrinsForServiceItems";
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
                                grinInventoryDto.GrinNo = iqcForServiceItems.GrinsForServiceItemsNumber;
                                grinInventoryDto.GrinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;
                                grinInventoryDto.ReferenceID = iqcForServiceItems.GrinsForServiceItemsNumber;
                                grinInventoryDto.ReferenceIDFrom = "GrinsForServiceItems";
                                grinInventoryDto.GrinMaterialType = "GrinsForServiceItems";
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
                                //if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK) createInvfromGrin = rfqCustomerIdResponse.StatusCode;
                            }
                        }
                    }

                    //var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinsForServiceItemsPartsQty(iqcforserviceitems_items.GrinsForServiceItemsPartsId, iqcforserviceitems_items.AcceptedQty.ToString(), iqcforserviceitems_items.RejectedQty.ToString());

                    //var grinParts = _mapper.Map<GrinsForServiceItemsParts>(updatedGrinPartsQty);
                    //string result = await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinParts);
                    if (getInvGrinsForServiceItemsId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && getInvTrancGrinsForServiceItemsId == HttpStatusCode.OK && updateInvTranc == HttpStatusCode.OK && createInvTranc == HttpStatusCode.OK)
                    {
                        _grinPartsRepository.SaveAsync();
                        _iQCForServiceItemsRepository.SaveAsync();
                        _repository.SaveAsync();
                        _iQCForServiceItems_ItemsRepository.SaveAsync();
                    }
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCForServiceItemsItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (serverKey == "avision")
                    {
                        var grinNum = iqcForServiceItems.GrinsForServiceItemsNumber;
                        if (grinNum != null)
                        {
                            var iqcNum = grinNum.Replace("GSI", "IQCSI");
                            iqcForServiceItems.IQCForServiceItemsNumber = iqcNum;
                        }
                    }
                    var grinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                    var grinPartsDetails = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(grinPartId);
                    if (iqcConfirmationItemsDto.GrinsForServiceItemsPartId != grinPartsDetails.Id)
                    {
                        _logger.LogError($"Invalid GrinsForServiceItemsPart Id {grinPartId}");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Invalid GrinsForServiceItemsPart Id {grinPartId}";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    iqcforserviceitems_items.IsIqcForServiceItemsCompleted = true;
                    iqcforserviceitems_items.IQCForServiceItemsId = iqcForServiceItems.Id;
                    iqcForServiceItems.IQCForServiceItems_Items = new List<IQCForServiceItems_Items> { iqcforserviceitems_items };
                    await _iQCForServiceItemsRepository.CreateIQCForServiceItems(iqcForServiceItems);
                    _iQCForServiceItemsRepository.SaveAsync();

                    //Updating IQC Status in GrinParts

                    grinPartsDetails.IsIqcForServiceItemsCompleted = true;
                    await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinPartsDetails);
                    _grinPartsRepository.SaveAsync();

                    //Updating IQC and Grin Main Level Status
                    var grinPartsCount = await _grinPartsRepository.GetGrinsForServiceItemsPartsCount(GrinsForServiceItemsId);
                    var iqcConfomationCount = await _iQCForServiceItems_ItemsRepository.GetIQCForServiceItems_ItemsCount(iqcForServiceItems.Id);

                    if (grinPartsCount == iqcConfomationCount)
                    {
                        var iqcDetails = await _iQCForServiceItemsRepository.GetIqcForServiceItemsDetailsbyGrinForServiceItemsNo(grinsForServiceItemsNumber);
                        iqcDetails.IsIqcForServiceItemsCompleted = true;
                        _iQCForServiceItemsRepository.SaveAsync();

                        var grinDetails = await _repository.GetGrinForServiceItemsByGrinForServiceItemsNo(grinsForServiceItemsNumber);
                        grinDetails.IsIqcForServiceItemsCompleted = true;
                        _repository.SaveAsync();
                    }

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = iqcConfirmationItemsDto.ItemNumber;
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterObject = itemMasterObjectData.data;
                    //Inventory Update Code
                    decimal? acceptedQty = iqcConfirmationItemsDto.AcceptedQty;
                    var grinPartsId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                    var grinPartsDetail = await _grinPartsRepository.GetGrinsForServiceItemsPartsDetailsbyGrinsForServiceItemsPartsId(grinPartsId);
                    foreach (var projectNo in grinPartsDetail.GrinsForServiceItemsProjectNumbers)
                    {
                        var client1 = _clientFactory.CreateClient();
                        var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                        var itemNo = iqcConfirmationItemsDto.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(itemNo);
                        var grinNo = iqcForServiceItems.GrinsForServiceItemsNumber;
                        var encodedgrinNo = Uri.EscapeDataString(grinNo);
                        var grinPartsIds = projectNo.GrinsForServiceItemsPartsId;
                        var projectNos = projectNo.ProjectNumber;
                        var encodedprojectNos = Uri.EscapeDataString(projectNos);

                        var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryForServiceItemsAPI"],
                           $"GetInventoryForServiceDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                        request1.Headers.Add("Authorization", token1);

                        var inventoryObjectResult = await client1.SendAsync(request1);
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
                                inventoryObject.referenceIDFrom = "GrinsForServiceItems";
                                acceptedQty -= balanceQty;

                            }
                            else if (inventoryObject.balance_Quantity > acceptedQty)
                            {
                                if (acceptedQty == 0)
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceIDFrom = "GrinsForServiceItems";
                                    inventoryObject.isStockAvailable = false;
                                }
                                else
                                {
                                    inventoryObject.balance_Quantity = acceptedQty;
                                    inventoryObject.warehouse = "IQC";
                                    inventoryObject.location = "IQC";
                                    inventoryObject.referenceIDFrom = "GrinsForServiceItems";
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
                                grinInventoryDto.GrinNo = iqcForServiceItems.GrinsForServiceItemsNumber;
                                grinInventoryDto.GrinPartId = iqcConfirmationItemsDto.GrinsForServiceItemsPartId;
                                grinInventoryDto.PartType = itemMasterObject.itemType;
                                grinInventoryDto.ReferenceID = iqcForServiceItems.GrinsForServiceItemsNumber;
                                grinInventoryDto.ReferenceIDFrom = "GrinsForServiceItems";
                                grinInventoryDto.GrinMaterialType = "GrinsForServiceItems";
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

                                var responses = await client6.SendAsync(request6);

                            }
                        }
                    }

                    ////update accepted qty and rejected qty in grin model

                    var updatedGrinPartsQty = await _grinPartsRepository.UpdateGrinsForServiceItemsPartsQty(iqcforserviceitems_items.GrinsForServiceItemsPartsId, iqcforserviceitems_items.AcceptedQty.ToString(), iqcforserviceitems_items.RejectedQty.ToString());

                    var grinParts = _mapper.Map<GrinsForServiceItemsParts>(updatedGrinPartsQty);
                    string result = await _grinPartsRepository.UpdateGrinsForServiceItemsParts(grinParts);

                    _grinPartsRepository.SaveAsync();

                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCForServiceItems and IQCForServiceItems_Items Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }



            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateIQCForServiceItemsItems action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateGrinForServiceItemsFileUpload([FromBody] List<DocumentUploadPostDto> fileUploadPostDtos)
        {
            ServiceResponse<List<string>> serviceResponse = new ServiceResponse<List<string>>();
            try
            {
                if (fileUploadPostDtos is null)
                {
                    _logger.LogError("GrinForServiceItemsFile object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GrinForServiceItemsFile object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid GrinForServiceItemsFile object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                List<string>? id_s = new List<string>();
                var FileUploadDetails = fileUploadPostDtos;
                foreach (var FileUploadDetail in FileUploadDetails)
                {
                    Guid guids = Guid.NewGuid();
                    byte[] fileContent = Convert.FromBase64String(FileUploadDetail.FileByte);
                    string fileName = guids.ToString() + "_" + FileUploadDetail.FileName + "." + FileUploadDetail.FileExtension;
                    string FileExt = Path.GetExtension(fileName).ToUpper();
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "GrinDocument", fileName);
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
                            ParentId = "GrinForServiceItemsFile",
                            DocumentFrom = "GrinForServiceItemsFile Document",
                            FileByte = FileUploadDetail.FileByte
                        };
                        await _documentUploadRepository.CreateUploadDocumentGrin(uploadedFile);
                        _documentUploadRepository.SaveAsync();
                        id_s.Add(uploadedFile.Id.ToString());

                    }
                }
                serviceResponse.Data = id_s;
                serviceResponse.Message = " GrinForServiceItemsFile Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GrinForServiceItemsFile action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDownloadUrlDetailsforGrinForServiceItemsFiles(string fileids)
        {
            ServiceResponse<List<DocumentUploadDto>> serviceResponse = new ServiceResponse<List<DocumentUploadDto>>();
            try
            {
                string serverKey = GetServerKey();
                var itemsFiles = await _documentUploadRepository.GetDownloadUrlDetails(fileids);
                if (itemsFiles == null)
                {
                    _logger.LogError($"DownloadDetail with id: {fileids}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DownloadDetail with id: {fileids}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid GrinForServiceItems UploadDocument.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid GrinForServiceItems UploadDocument sent from client.");
                    return BadRequest(serviceResponse);
                }
                List<DocumentUploadDto> fileUploads = new List<DocumentUploadDto>();
                if (itemsFiles != null)
                {
                    foreach (var fileUploadDetails in itemsFiles)
                    {
                        DocumentUploadDto fileUploadDto = _mapper.Map<DocumentUploadDto>(fileUploadDetails);
                        if (serverKey == "avision")
                        {
                            var baseUrl = $"{_config["GrinUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/apigateway/tips/Grin/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        else
                        {
                            var baseUrl = $"{_config["GrinUrl"]}";
                            fileUploadDto.DownloadUrl = $"{baseUrl}/api/Grin/DownloadFile?Filename={fileUploadDto.FileName}";
                        }
                        fileUploads.Add(fileUploadDto);
                    }
                }
                _logger.LogInfo($"Returned DownloadDetail with id: {fileids}");
                serviceResponse.Data = fileUploads;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GrinForServiceItems action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrinsForServiceItems(int id, [FromBody] GrinsForServiceItemsUpdateDto grinDto)
        {
            ServiceResponse<GrinsForServiceItemsDto> serviceResponse = new ServiceResponse<GrinsForServiceItemsDto>();

            try
            {
                if (grinDto is null)
                {
                    _logger.LogError("Update GrinsForServiceItems object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update GrinsForServiceItems object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update GrinsForServiceItems object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update GrinsForServiceItems object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updategrin = await _repository.GetGrinsForServiceItemsById(id);
                if (updategrin is null)
                {
                    _logger.LogError($"Update GrinsForServiceItems with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update GrinsForServiceItems with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var grinparts = _mapper.Map<IEnumerable<GrinsForServiceItemsParts>>(updategrin.GrinsForServiceItemsParts);

                var grinList = _mapper.Map<GrinsForServiceItems>(grinDto);

                var grinPartsDto = updategrin.GrinsForServiceItemsParts;
                var grinCal = _mapper.Map<List<GrinsForServiceItemsPartscalculationofAvgcost>>(grinPartsDto);
                var GrinpartsList = new List<GrinsForServiceItemsParts>();

                var othercosttotal = grinList.Freight + grinList.Insurance + grinList.LoadingorUnLoading + grinList.Transport;
                decimal? conversionrate = 1;
                if (grinList.CurrencyConversion > 1) conversionrate = grinList.CurrencyConversion;
                foreach (var gPart in grinCal)
                {
                    decimal? EP = gPart.Qty * gPart.UnitPrice;
                    decimal? Itemwithtax = gPart.SGST + gPart.IGST + gPart.CGST + gPart.UTGST + gPart.Duties;
                    gPart.EPwithTax = (EP + (EP * (Itemwithtax / 100))) * conversionrate;
                    gPart.EPforSingleQty = gPart.EPwithTax / gPart.Qty;
                }
                decimal? SumofEPwithtax = grinCal.Sum(x => x.EPwithTax);
                foreach (var gPart in grinCal)
                {
                    decimal? distriduteOthercostforitem = (gPart.EPwithTax / SumofEPwithtax) * othercosttotal;
                    decimal? distriduteOthercostforitemsSingleQty = distriduteOthercostforitem / gPart.Qty;
                    gPart.AverageCost = distriduteOthercostforitemsSingleQty + gPart.EPforSingleQty;
                    GrinsForServiceItemsParts grinParts = _mapper.Map<GrinsForServiceItemsParts>(gPart);
                    grinParts.GrinsForServiceItemsProjectNumbers = _mapper.Map<List<GrinsForServiceItemsProjectNumbers>>(gPart.GrinsForServiceItemsProjectNumbers);
                    GrinpartsList.Add(grinParts);
                }
                var data = _mapper.Map(grinDto, updategrin);
                data.GrinsForServiceItemsParts = GrinpartsList;

                string result = await _repository.UpdateGrinsForServiceItems(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "GrinsForServiceItems Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateGrinsForServiceItems action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGrinForServiceItemsNumberForIqcForServiceItems()
        {
            ServiceResponse<IEnumerable<GrinForServiceItemsNoForIqcForServiceItems>> serviceResponse = new ServiceResponse<IEnumerable<GrinForServiceItemsNoForIqcForServiceItems>>();
            try
            {
                var result = await _repository.GetAllGrinForServiceItemsNumberForIqcForServiceItems();
                //var result = _mapper.Map<IEnumerable<GrinForServiceItemsNoForIqcForServiceItems>>(grinNoForIqc);
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
    }
}
