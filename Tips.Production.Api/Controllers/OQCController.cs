using AutoMapper;
using Contracts;
using Entities;
using Entities.Enums;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using System.Drawing;
using System.Net;
using System.Security.Claims;
using System.Text;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class OQCController : ControllerBase
    {
        private IShopOrderConfirmationRepository _shopOrderConfirmationRepository;
        private IOQCRepository _oQCRepository;
        private IItemMasterRepository _itemMasterRepository;
        private ILoggerManager _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private IMapper _mapper;
        private IShopOrderRepository _shopOrderRepo;
        private IOQCBinningRepository _oQCBinningRepository;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public OQCController(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, IShopOrderConfirmationRepository shopOrderConfirmationRepository, IOQCRepository oQCRepository, IOQCBinningRepository oQCBinningRepository, IShopOrderRepository shopOrderRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _oQCRepository = oQCRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _shopOrderRepo = shopOrderRepository;
            _oQCBinningRepository = oQCBinningRepository;
            _shopOrderConfirmationRepository = shopOrderConfirmationRepository;
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOQC([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<OQCDto>> serviceResponse = new ServiceResponse<IEnumerable<OQCDto>>();
            try
            {
                var oQCDetails = await _oQCRepository.GetAllOQC(pagingParameter, searchParamess);
                var metadata = new
                {
                    oQCDetails.TotalCount,
                    oQCDetails.PageSize,
                    oQCDetails.CurrentPage,
                    oQCDetails.HasNext,
                    oQCDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all OQC");
                var result = _mapper.Map<IEnumerable<OQCDto>>(oQCDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OQCs Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOQC API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOQC API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOQCById(int id)
        {
            ServiceResponse<OQCDto> serviceResponse = new ServiceResponse<OQCDto>();

            try
            {
                var oQCDetailsbyId = await _oQCRepository.GetOQCById(id);
                if (oQCDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQC  hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"OQC with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned OQC with id: {id}");
                    var result = _mapper.Map<OQCDto>(oQCDetailsbyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned OQC with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOQCById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOQCById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchOQCDate([FromQuery] SearchDateparames searchDateParam)
        {
            ServiceResponse<IEnumerable<OQCDto>> serviceResponse = new ServiceResponse<IEnumerable<OQCDto>>();
            try
            {
                var oqcDate = await _oQCRepository.SearchOQCDate(searchDateParam);
                var result = _mapper.Map<IEnumerable<OQCDto>>(oqcDate);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OQCDate";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchOQCDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchOQCDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchOQC([FromQuery] SearchParamess searchParames)
        {
            ServiceResponse<IEnumerable<OQCDto>> serviceResponse = new ServiceResponse<IEnumerable<OQCDto>>();
            try
            {
                var oqcList = await _oQCRepository.SearchOQC(searchParames);

                _logger.LogInfo("Returned all OQC");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<OQCDto, OQC>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<OQCDto>>(oqcList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OQC";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchOQC API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchOQC API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllOQCWithItems([FromBody] OQCSearchDto oQCSearchDto)
        {
            ServiceResponse<IEnumerable<OQCDto>> serviceResponse = new ServiceResponse<IEnumerable<OQCDto>>();
            try
            {
                var oqcList = await _oQCRepository.GetAllOQCWithItems(oQCSearchDto);

                _logger.LogInfo("Returned all OQC");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<OQCDto, OQC>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<OQCDto>>(oqcList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OQC";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOQCWithItems API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOQCWithItems API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOQC([FromBody] OQCPostDto oQCPostDto)
        {
            ServiceResponse<OQCDto> serviceResponse = new ServiceResponse<OQCDto>();

            try
            {
                if (oQCPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OQC object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("OQC object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OQC object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;

                    _logger.LogError("Invalid OQC object sent from client.");

                    return BadRequest(serviceResponse);
                }
                var oQCCreate = _mapper.Map<OQC>(oQCPostDto);

                await _oQCRepository.CreateOQC(oQCCreate);

                _oQCRepository.SaveAsync();
                //var shopOrderNumber = oQCCreate.ShopOrderNumber;
                //var shopOrderDetails = await _shopOrderRepo.GetShopOrderDetailsByShopOrderNo(shopOrderNumber);
                //shopOrderDetails.OqcQty = shopOrderDetails.OqcQty + oQCCreate.AcceptedQty;
                //_shopOrderRepo.SaveAsync();  
                HttpStatusCode CreateSAInv = HttpStatusCode.OK;
                HttpStatusCode CreateSAInvTrans = HttpStatusCode.OK;
                HttpStatusCode CreateSARejectInv = HttpStatusCode.OK;
                HttpStatusCode CreateSARejectInvTrans = HttpStatusCode.OK;
                HttpStatusCode CreateFGInv = HttpStatusCode.OK;
                HttpStatusCode CreateFGInvTrans = HttpStatusCode.OK;
                HttpStatusCode CreateFGRejectInv = HttpStatusCode.OK;
                HttpStatusCode CreateFGRejectInvTrans = HttpStatusCode.OK;
                HttpStatusCode GetSAItemMas = HttpStatusCode.OK;
                HttpStatusCode GetFGItemMas = HttpStatusCode.OK;


                var shopOrderDetails = await _shopOrderRepo.GetShopOrderByShopOrderNo(oQCPostDto.ShopOrderNumber);
                shopOrderDetails.OqcQty = shopOrderDetails.OqcQty + oQCPostDto.AcceptedQty;
                if (shopOrderDetails.TotalSOReleaseQty == shopOrderDetails.OqcQty) shopOrderDetails.OQCStatus = ShopOrderConformationStatus.FullyDone;
                if (shopOrderDetails.TotalSOReleaseQty > shopOrderDetails.OqcQty) shopOrderDetails.OQCStatus = ShopOrderConformationStatus.PartiallyDone;
                await _shopOrderRepo.UpdateShopOrder(shopOrderDetails);
                
                var SOConfirmations = await _shopOrderConfirmationRepository.GetAllShopOrderConfirmationByShopOrderNo(shopOrderDetails.ShopOrderNumber);
                var OQCQty = shopOrderDetails.OqcQty;
                foreach (var soCon in SOConfirmations)
                {
                    if (soCon.WipConfirmedQty == OQCQty) { soCon.IsOQCDone = ShopOrderConformationStatus.FullyDone;
                        _shopOrderConfirmationRepository.UpdateShopOrderConfirmation(soCon); 
                        break; }
                    if (soCon.WipConfirmedQty > OQCQty) { soCon.IsOQCDone = ShopOrderConformationStatus.PartiallyDone;
                        _shopOrderConfirmationRepository.UpdateShopOrderConfirmation(soCon); 
                        break; }
                    if (soCon.WipConfirmedQty < OQCQty) { soCon.IsOQCDone = ShopOrderConformationStatus.FullyDone; OQCQty -= soCon.WipConfirmedQty; }

                    _shopOrderConfirmationRepository.UpdateShopOrderConfirmation(soCon);
                }

                var shopOrderItemDetail = shopOrderDetails?.ShopOrderItems?.FirstOrDefault();
                var projectNo = shopOrderItemDetail?.ProjectNumber;
                if (oQCCreate.ItemType == PartType.SA) //sa
                {
                    //var ItemNumber = oQCCreate.ItemNumber;
                    //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
                    //        "GetItemMasterByItemNumber?", "&ItemNumber=", ItemNumber));

                    var client2 = _clientFactory.CreateClient();
                    var token2 = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = oQCCreate.ItemNumber;
                    var encodedItemNo = Uri.EscapeDataString(ItemNumber);

                    var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNo}"));
                    request2.Headers.Add("Authorization", token2);

                    var itemMasterObjectResult = await client2.SendAsync(request2);
                    if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                    {
                        GetSAItemMas = itemMasterObjectResult.StatusCode;
                    }
                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(itemMasterObjectResult));
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    var itemMatserObjectData = JsonConvert.DeserializeObject<OqcItemMasterDetails>(itemMasterObjectString);
                    var itemMasterTranctionObject = itemMatserObjectData.data;

                    //Adding SA in Inventory Table
                    InventoryPostDto inventory = new InventoryPostDto();
                    var ItemNo = itemMasterTranctionObject.itemNumber;
                    var Desc = itemMasterTranctionObject.description;
                    var uom = itemMasterTranctionObject.uom;

                    inventory.PartNumber = itemMasterTranctionObject.itemNumber;
                    inventory.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                    inventory.Description = itemMasterTranctionObject.description;
                    inventory.ProjectNumber = projectNo;
                    inventory.Balance_Quantity = oQCCreate.AcceptedQty;
                    inventory.UOM = itemMasterTranctionObject.uom;
                    inventory.Max = itemMasterTranctionObject.max;
                    inventory.Min = itemMasterTranctionObject.min;
                    inventory.Warehouse = "SA";
                    inventory.Location = "SA";
                    inventory.GrinNo = "";
                    inventory.GrinMaterialType = "";
                    inventory.GrinPartId = oQCCreate.Id;
                    inventory.PartType = oQCCreate.ItemType; // we have to take parttype from grinparts model;
                    inventory.ReferenceID = oQCCreate.Id.ToString();
                    inventory.ReferenceIDFrom = "Final OQC";
                    inventory.ShopOrderNo = oQCCreate.ShopOrderNumber;
                    inventory.LotNumber= oQCCreate.ShopOrderNumber;

                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(inventory));
                    var json = JsonConvert.SerializeObject(inventory);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);

                    var client1 = _clientFactory.CreateClient();
                    var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                    "CreateInventory"))
                    {
                        Content = data
                    };
                    request1.Headers.Add("Authorization", token1);

                    var response = await client1.SendAsync(request1);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        CreateSAInv = response.StatusCode;
                    }
                    //Adding Rejected in Inventory Table
                    InventoryPostDto inventory1 = new InventoryPostDto();

                    inventory1.PartNumber = itemMasterTranctionObject.itemNumber;
                    inventory1.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); ;
                    inventory1.Description = itemMasterTranctionObject.description;
                    inventory1.ProjectNumber = projectNo;
                    inventory1.Balance_Quantity = oQCCreate.RejectedQty;
                    inventory1.UOM = itemMasterTranctionObject.uom;
                    inventory1.Max = itemMasterTranctionObject.max;
                    inventory1.Min = itemMasterTranctionObject.min;
                    inventory1.Warehouse = "Reject";
                    inventory1.Location = "Reject";
                    inventory1.GrinNo = "";
                    inventory1.GrinMaterialType = "";
                    inventory1.GrinPartId = oQCCreate.Id;
                    inventory1.PartType = oQCCreate.ItemType; // we have to take parttype from grinparts model;
                    inventory1.ReferenceID = oQCCreate.Id.ToString() + "-R";
                    inventory1.ReferenceIDFrom = "Final OQC";
                    inventory1.ShopOrderNo = oQCCreate.ShopOrderNumber;
                    inventory1.LotNumber= oQCCreate.ShopOrderNumber;

                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(inventory1));
                    var json1 = JsonConvert.SerializeObject(inventory1);
                    var data1 = new StringContent(json1, Encoding.UTF8, "application/json");
                   // var response1 = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data1);

                    var client3 = _clientFactory.CreateClient();
                    var token3 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request3 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                    "CreateInventory"))
                    {
                        Content = data1
                    };
                    request3.Headers.Add("Authorization", token3);

                    var response1 = await client3.SendAsync(request3);
                    if (response1.StatusCode != HttpStatusCode.OK)
                    {
                        CreateSARejectInv = response1.StatusCode;
                    }
                    //Adding SA in InventoryTranction Table
                    InventoryTranctionDto inventoryTranction = new InventoryTranctionDto();

                    inventoryTranction.PartNumber = ItemNo;
                    inventoryTranction.LotNumber = oQCCreate.ShopOrderNumber;
                    inventoryTranction.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); ;
                    inventoryTranction.Description = Desc;
                    inventoryTranction.ProjectNumber = projectNo;
                    inventoryTranction.PartType = oQCCreate.ItemType;
                    inventoryTranction.Issued_Quantity = oQCCreate.AcceptedQty;
                    inventoryTranction.UOM = uom;
                    inventoryTranction.BOM_Version_No = 0;
                    inventoryTranction.Issued_DateTime = DateTime.Now;
                    inventoryTranction.Issued_By = _createdBy;
                    inventoryTranction.GrinNo = inventory.GrinNo;
                    inventoryTranction.GrinPartId = inventory.GrinPartId;
                    inventoryTranction.shopOrderNo = oQCCreate.ShopOrderNumber;
                    inventoryTranction.ReferenceID = oQCCreate.Id.ToString();
                    inventoryTranction.ReferenceIDFrom = "Final OQC"; ;
                    inventoryTranction.From_Location = "SA";
                    inventoryTranction.TO_Location = "SA";
                    inventoryTranction.Warehouse = "SA";
                    inventoryTranction.Remarks = "OQC Done";
                    
                    var json2 = JsonConvert.SerializeObject(inventoryTranction);
                    var data2 = new StringContent(json2, Encoding.UTF8, "application/json");
                   // var response2 = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), data2);

                    var client4 = _clientFactory.CreateClient();
                    var token4 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request4 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                    "CreateInventoryTranction"))
                    {
                        Content = data2
                    };
                    request4.Headers.Add("Authorization", token4);

                    var response2 = await client4.SendAsync(request4);
                    if (response2.StatusCode != HttpStatusCode.OK)
                    {
                        CreateSAInvTrans = response2.StatusCode;
                    }

                    //Adding Rejected Item in InventoryTranction Table
                    InventoryTranctionDto inventoryTranction1 = new InventoryTranctionDto();

                    inventoryTranction1.PartNumber = ItemNo;
                    inventoryTranction1.LotNumber = oQCCreate.ShopOrderNumber;
                    inventoryTranction1.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); ;
                    inventoryTranction1.Description = Desc;
                    inventoryTranction1.ProjectNumber = projectNo;
                    inventoryTranction1.PartType = oQCCreate.ItemType;
                    inventoryTranction1.Issued_Quantity = oQCCreate.RejectedQty;
                    inventoryTranction1.UOM = uom;
                    inventoryTranction1.BOM_Version_No = 0;
                    inventoryTranction1.Issued_DateTime = DateTime.Now;
                    inventoryTranction1.Issued_By = _createdBy;
                    inventoryTranction1.GrinNo = inventory1.GrinNo;
                    inventoryTranction1.GrinPartId = inventory1.GrinPartId;
                    inventoryTranction1.shopOrderNo = oQCCreate.ShopOrderNumber;
                    inventoryTranction1.ReferenceID = oQCCreate.Id.ToString() + "-R";
                    inventoryTranction1.ReferenceIDFrom = "Final OQC"; ;
                    inventoryTranction1.From_Location = "Reject";
                    inventoryTranction1.TO_Location = "Reject";
                    inventoryTranction1.Warehouse = "Reject";
                    inventoryTranction1.Remarks = "OQC Done";
                   
                    var json4 = JsonConvert.SerializeObject(inventoryTranction1);
                    var data4 = new StringContent(json4, Encoding.UTF8, "application/json");
                    //var response4 = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), data4);

                    var client5 = _clientFactory.CreateClient();
                    var token5 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request5 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                    "CreateInventoryTranction"))
                    {
                        Content = data4
                    };
                    request5.Headers.Add("Authorization", token5);

                    var response4 = await client5.SendAsync(request5);
                    if (response4.StatusCode != HttpStatusCode.OK)
                    {
                        CreateSARejectInvTrans = response4.StatusCode;
                    }

                }
                else if(oQCCreate.ItemType == PartType.Kit)
                {
                    //var ItemNumber = oQCCreate.ItemNumber;
                    //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
                    //        "GetItemMasterByItemNumber?", "&ItemNumber=", ItemNumber));

                    var client2 = _clientFactory.CreateClient();
                    var token2 = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = oQCCreate.ItemNumber;
                    var encodedItemNo = Uri.EscapeDataString(ItemNumber);

                    var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNo}"));
                    request2.Headers.Add("Authorization", token2);

                    var itemMasterObjectResult = await client2.SendAsync(request2);
                    if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                    {
                        GetSAItemMas = itemMasterObjectResult.StatusCode;
                    }
                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(itemMasterObjectResult));
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    var itemMatserObjectData = JsonConvert.DeserializeObject<OqcItemMasterDetails>(itemMasterObjectString);
                    var itemMasterTranctionObject = itemMatserObjectData.data;

                    //Adding KIT in Inventory Table
                    InventoryPostDto inventory = new InventoryPostDto();
                    var ItemNo = itemMasterTranctionObject.itemNumber;
                    var Desc = itemMasterTranctionObject.description;
                    var uom = itemMasterTranctionObject.uom;

                    inventory.PartNumber = itemMasterTranctionObject.itemNumber;
                    inventory.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                    inventory.Description = itemMasterTranctionObject.description;
                    inventory.ProjectNumber = projectNo;
                    inventory.Balance_Quantity = oQCCreate.AcceptedQty;
                    inventory.UOM = itemMasterTranctionObject.uom;
                    inventory.Max = itemMasterTranctionObject.max;
                    inventory.Min = itemMasterTranctionObject.min;
                    inventory.Warehouse = "KIT";
                    inventory.Location = "KIT";
                    inventory.GrinNo = "";
                    inventory.GrinMaterialType = "";
                    inventory.GrinPartId = oQCCreate.Id;
                    inventory.PartType = oQCCreate.ItemType; // we have to take parttype from grinparts model;
                    inventory.ReferenceID = oQCCreate.Id.ToString();
                    inventory.ReferenceIDFrom = "Final OQC";
                    inventory.ShopOrderNo = oQCCreate.ShopOrderNumber;
                    inventory.LotNumber = oQCCreate.ShopOrderNumber;

                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(inventory));
                    var json = JsonConvert.SerializeObject(inventory);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);

                    var client1 = _clientFactory.CreateClient();
                    var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                    "CreateInventory"))
                    {
                        Content = data
                    };
                    request1.Headers.Add("Authorization", token1);

                    var response = await client1.SendAsync(request1);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        CreateSAInv = response.StatusCode;
                    }
                    //Adding Rejected in Inventory Table
                    InventoryPostDto inventory1 = new InventoryPostDto();

                    inventory1.PartNumber = itemMasterTranctionObject.itemNumber;
                    inventory1.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); ;
                    inventory1.Description = itemMasterTranctionObject.description;
                    inventory1.ProjectNumber = projectNo;
                    inventory1.Balance_Quantity = oQCCreate.RejectedQty;
                    inventory1.UOM = itemMasterTranctionObject.uom;
                    inventory1.Max = itemMasterTranctionObject.max;
                    inventory1.Min = itemMasterTranctionObject.min;
                    inventory1.Warehouse = "Reject";
                    inventory1.Location = "Reject";
                    inventory1.GrinNo = "";
                    inventory1.GrinMaterialType = "";
                    inventory1.GrinPartId = oQCCreate.Id;
                    inventory1.PartType = oQCCreate.ItemType; // we have to take parttype from grinparts model;
                    inventory1.ReferenceID = oQCCreate.Id.ToString() + "-R";
                    inventory1.ReferenceIDFrom = "Final OQC";
                    inventory1.ShopOrderNo = oQCCreate.ShopOrderNumber;
                    inventory1.LotNumber = oQCCreate.ShopOrderNumber;

                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(inventory1));
                    var json1 = JsonConvert.SerializeObject(inventory1);
                    var data1 = new StringContent(json1, Encoding.UTF8, "application/json");
                    // var response1 = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data1);

                    var client3 = _clientFactory.CreateClient();
                    var token3 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request3 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                    "CreateInventory"))
                    {
                        Content = data1
                    };
                    request3.Headers.Add("Authorization", token3);

                    var response1 = await client3.SendAsync(request3);
                    if (response1.StatusCode != HttpStatusCode.OK)
                    {
                        CreateSARejectInv = response1.StatusCode;
                    }
                    //Adding KIT in InventoryTranction Table
                    InventoryTranctionDto inventoryTranction = new InventoryTranctionDto();

                    inventoryTranction.PartNumber = ItemNo;
                    inventoryTranction.LotNumber = oQCCreate.ShopOrderNumber;
                    inventoryTranction.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); ;
                    inventoryTranction.Description = Desc;
                    inventoryTranction.ProjectNumber = projectNo;
                    inventoryTranction.PartType = oQCCreate.ItemType;
                    inventoryTranction.Issued_Quantity = oQCCreate.AcceptedQty;
                    inventoryTranction.UOM = uom;
                    inventoryTranction.BOM_Version_No = 0;
                    inventoryTranction.Issued_DateTime = DateTime.Now;
                    inventoryTranction.Issued_By = _createdBy;
                    inventoryTranction.GrinNo = inventory.GrinNo;
                    inventoryTranction.GrinPartId = inventory.GrinPartId;
                    inventoryTranction.shopOrderNo = oQCCreate.ShopOrderNumber;
                    inventoryTranction.ReferenceID = oQCCreate.Id.ToString();
                    inventoryTranction.ReferenceIDFrom = "Final OQC"; ;
                    inventoryTranction.From_Location = "KIT";
                    inventoryTranction.TO_Location = "KIT";
                    inventoryTranction.Warehouse = "KIT";
                    inventoryTranction.Remarks = "OQC Done";

                    var json2 = JsonConvert.SerializeObject(inventoryTranction);
                    var data2 = new StringContent(json2, Encoding.UTF8, "application/json");
                    // var response2 = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), data2);

                    var client4 = _clientFactory.CreateClient();
                    var token4 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request4 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                    "CreateInventoryTranction"))
                    {
                        Content = data2
                    };
                    request4.Headers.Add("Authorization", token4);

                    var response2 = await client4.SendAsync(request4);
                    if (response2.StatusCode != HttpStatusCode.OK)
                    {
                        CreateSAInvTrans = response2.StatusCode;
                    }

                    //Adding Rejected Item in InventoryTranction Table
                    InventoryTranctionDto inventoryTranction1 = new InventoryTranctionDto();

                    inventoryTranction1.PartNumber = ItemNo;
                    inventoryTranction1.LotNumber = oQCCreate.ShopOrderNumber;
                    inventoryTranction1.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); ;
                    inventoryTranction1.Description = Desc;
                    inventoryTranction1.ProjectNumber = projectNo;
                    inventoryTranction1.PartType = oQCCreate.ItemType;
                    inventoryTranction1.Issued_Quantity = oQCCreate.RejectedQty;
                    inventoryTranction1.UOM = uom;
                    inventoryTranction1.BOM_Version_No = 0;
                    inventoryTranction1.Issued_DateTime = DateTime.Now;
                    inventoryTranction1.Issued_By = _createdBy;
                    inventoryTranction1.GrinNo = inventory1.GrinNo;
                    inventoryTranction1.GrinPartId = inventory1.GrinPartId;
                    inventoryTranction1.shopOrderNo = oQCCreate.ShopOrderNumber;
                    inventoryTranction1.ReferenceID = oQCCreate.Id.ToString() + "-R";
                    inventoryTranction1.ReferenceIDFrom = "Final OQC"; ;
                    inventoryTranction1.From_Location = "Reject";
                    inventoryTranction1.TO_Location = "Reject";
                    inventoryTranction1.Warehouse = "Reject";
                    inventoryTranction1.Remarks = "OQC Done";

                    var json4 = JsonConvert.SerializeObject(inventoryTranction1);
                    var data4 = new StringContent(json4, Encoding.UTF8, "application/json");
                    //var response4 = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), data4);

                    var client5 = _clientFactory.CreateClient();
                    var token5 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request5 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                    "CreateInventoryTranction"))
                    {
                        Content = data4
                    };
                    request5.Headers.Add("Authorization", token5);

                    var response4 = await client5.SendAsync(request5);
                    if (response4.StatusCode != HttpStatusCode.OK)
                    {
                        CreateSARejectInvTrans = response4.StatusCode;
                    }

                }
                else
                {
                    //var ItemNumber = oQCCreate.ItemNumber;
                    //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
                    //        "GetItemMasterByItemNumber?", "&ItemNumber=", ItemNumber));

                    var client2 = _clientFactory.CreateClient();
                    var token2 = HttpContext.Request.Headers["Authorization"].ToString();

                    var ItemNumber = oQCCreate.ItemNumber;
                    var encodedItemNo = Uri.EscapeDataString(ItemNumber);

                    var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNo}"));
                    request2.Headers.Add("Authorization", token2);

                    var itemMasterObjectResult = await client2.SendAsync(request2);
                    if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                    {
                        GetFGItemMas = itemMasterObjectResult.StatusCode;
                    }
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    var itemMasterObjectData = JsonConvert.DeserializeObject<OqcItemMasterDetails>(itemMasterObjectString);
                    var itemMasterTranctionObject = itemMasterObjectData.data;

                    InventoryPostDto inventory = new InventoryPostDto();
                    var ItemNo = itemMasterTranctionObject.itemNumber;
                    var Desc = itemMasterTranctionObject.description;
                    var uom = itemMasterTranctionObject.uom;
                    inventory.PartNumber = ItemNo;
                    inventory.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x=>x.isDefault == true).Select(x=>x.manufacturerPartNo).FirstOrDefault();
                    inventory.Description = Desc;
                    inventory.ProjectNumber = projectNo;
                    inventory.Balance_Quantity = oQCCreate.AcceptedQty;
                    inventory.UOM = uom;
                    inventory.Max = itemMasterTranctionObject.max;
                    inventory.Min = itemMasterTranctionObject.min;
                    inventory.Warehouse = "FG";
                    inventory.Location = "FG";
                    inventory.GrinNo = "";
                    inventory.GrinMaterialType = "Issue";
                    inventory.GrinPartId = oQCCreate.Id;
                    inventory.PartType = oQCCreate.ItemType; // we have to take parttype from grinparts model;
                    inventory.ReferenceID = oQCCreate.Id.ToString();
                    inventory.ReferenceIDFrom = "Final OQC";
                    inventory.ShopOrderNo = oQCCreate.ShopOrderNumber;
                    inventory.LotNumber = oQCCreate.ShopOrderNumber;

                    var json = JsonConvert.SerializeObject(inventory);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                   // var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);

                    var client1 = _clientFactory.CreateClient();
                    var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request1 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                    "CreateInventory"))
                    {
                        Content = data
                    };
                    request1.Headers.Add("Authorization", token1);

                    var response = await client1.SendAsync(request1);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        CreateFGInv = response.StatusCode;
                    }

                    // For Rejected Item Store
                    InventoryPostDto inventory1 = new InventoryPostDto();

                    inventory1.PartNumber = ItemNo;
                    inventory1.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); ;
                    inventory1.Description = Desc;
                    inventory1.ProjectNumber = projectNo;
                    inventory1.Balance_Quantity = oQCCreate.RejectedQty;
                    inventory1.UOM = uom;
                    inventory1.Max = itemMasterTranctionObject.max;
                    inventory1.Min = itemMasterTranctionObject.min;
                    inventory1.Warehouse = "Scrap";
                    inventory1.Location = "Reject";
                    inventory1.GrinNo = "";
                    inventory1.GrinMaterialType = "";
                    inventory1.GrinPartId = oQCCreate.Id;
                    inventory1.PartType = oQCCreate.ItemType; // we have to take parttype from grinparts model;
                    inventory1.ReferenceID = oQCCreate.Id.ToString() + "-R";
                    inventory1.ReferenceIDFrom = "Final OQC";
                    inventory1.ShopOrderNo = oQCCreate.ShopOrderNumber;
                    inventory1.LotNumber = oQCCreate.ShopOrderNumber;

                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(inventory1));
                    var json1 = JsonConvert.SerializeObject(inventory1);
                    var data1 = new StringContent(json1, Encoding.UTF8, "application/json");
                    //var response1 = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data1);

                    var client3 = _clientFactory.CreateClient();
                    var token3 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request3 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                    "CreateInventory"))
                    {
                        Content = data1
                    };
                    request3.Headers.Add("Authorization", token3);

                    var response1 = await client3.SendAsync(request3);
                    if (response1.StatusCode != HttpStatusCode.OK)
                    {
                        CreateFGRejectInv = response1.StatusCode;
                    }
                    //Adding FG in InventoryTranction Table
                    InventoryTranctionDto inventoryTranction = new InventoryTranctionDto();

                    inventoryTranction.PartNumber = ItemNo;
                    inventoryTranction.LotNumber = oQCCreate.ShopOrderNumber;
                    inventoryTranction.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); ;
                    inventoryTranction.Description = Desc;
                    inventoryTranction.ProjectNumber = projectNo;
                    inventoryTranction.PartType = oQCCreate.ItemType;
                    inventoryTranction.Issued_Quantity = oQCCreate.AcceptedQty;
                    inventoryTranction.UOM = uom;
                    inventoryTranction.BOM_Version_No = 0;
                    inventoryTranction.Issued_DateTime = DateTime.Now;
                    inventoryTranction.Issued_By = _createdBy;
                    inventoryTranction.shopOrderNo = oQCCreate.ShopOrderNumber;
                    inventoryTranction.ReferenceID = oQCCreate.Id.ToString();
                    inventoryTranction.GrinNo = inventory.GrinNo;
                    inventoryTranction.GrinPartId = oQCCreate.Id;
                    inventoryTranction.GrinMaterialType = "Issue";
                    inventoryTranction.ReferenceIDFrom = "Final OQC"; 
                    inventoryTranction.From_Location = "FG";
                    inventoryTranction.TO_Location = "FG";
                    inventoryTranction.Warehouse = "FG";
                    inventoryTranction.Remarks = "OQC Done";

                    var json2 = JsonConvert.SerializeObject(inventoryTranction);
                    var data2 = new StringContent(json2, Encoding.UTF8, "application/json");
                    //var response2 = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), data2);

                    var client4 = _clientFactory.CreateClient();
                    var token4 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request4 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                    "CreateInventoryTranction"))
                    {
                        Content = data2
                    };
                    request4.Headers.Add("Authorization", token4);

                    var response2 = await client4.SendAsync(request4);
                    if (response2.StatusCode != HttpStatusCode.OK)
                    {
                        CreateFGInvTrans = response2.StatusCode;
                    }

                    //Adding Rejected Item in InventoryTranction Table
                    InventoryTranctionDto inventoryTranction1 = new InventoryTranctionDto();

                    inventoryTranction1.PartNumber = ItemNo;
                    inventoryTranction1.MftrPartNumber = itemMasterTranctionObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault(); ;
                    inventoryTranction1.Description = Desc;
                    inventoryTranction1.ProjectNumber = projectNo;
                    inventoryTranction1.PartType = oQCCreate.ItemType;
                    inventoryTranction1.Issued_Quantity = oQCCreate.RejectedQty;
                    inventoryTranction1.UOM = uom;
                    inventoryTranction1.BOM_Version_No = 0;
                    inventoryTranction1.Issued_DateTime = DateTime.Now;
                    inventoryTranction1.Issued_By = _createdBy;
                    inventoryTranction1.shopOrderNo = oQCCreate.ShopOrderNumber;
                    inventoryTranction1.LotNumber = oQCCreate.ShopOrderNumber;
                    inventoryTranction1.GrinPartId = oQCCreate.Id;
                    inventoryTranction1.GrinNo = inventory1.GrinNo;
                    inventoryTranction1.ReferenceID = oQCCreate.Id.ToString() + "-R";
                    inventoryTranction1.ReferenceIDFrom = "Final OQC"; 
                    inventoryTranction1.From_Location = "Reject";
                    inventoryTranction1.TO_Location = "Reject";
                    inventoryTranction1.Warehouse = "Scrap";
                    inventoryTranction1.Remarks = "OQC Done";

                    var json4 = JsonConvert.SerializeObject(inventoryTranction1);
                    var data4 = new StringContent(json4, Encoding.UTF8, "application/json");
                    //var response4 = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), data4);

                    var client5 = _clientFactory.CreateClient();
                    var token5 = HttpContext.Request.Headers["Authorization"].ToString();

                    var request5 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                    "CreateInventoryTranction"))
                    {
                        Content = data4
                    };
                    request5.Headers.Add("Authorization", token5);

                    var response4 = await client5.SendAsync(request5);
                    if (response4.StatusCode != HttpStatusCode.OK)
                    {
                        CreateFGRejectInvTrans = response4.StatusCode;
                    }
                }
                _logger.LogInfo("aftergettingdata");
                if (CreateSAInv == HttpStatusCode.OK && CreateSARejectInv == HttpStatusCode.OK && CreateSAInvTrans == HttpStatusCode.OK && CreateSARejectInvTrans == HttpStatusCode.OK
                    && CreateFGInv == HttpStatusCode.OK && CreateFGRejectInv == HttpStatusCode.OK && CreateFGInvTrans == HttpStatusCode.OK
                    && CreateFGRejectInvTrans == HttpStatusCode.OK && GetSAItemMas == HttpStatusCode.OK && GetFGItemMas == HttpStatusCode.OK)
                {
                    _shopOrderRepo.SaveAsync();
                    _shopOrderConfirmationRepository.SaveAsync();

                    string serverKey = GetServerKey();
                    if (serverKey == "avision")
                    {
                        _logger.LogInfo($"Avision OQC Email Creation Process");
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();
                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"], "GetEmailTemplatebyProcessType?ProcessType=CreateOQC"));
                        request.Headers.Add("Authorization", token);
                        var response1 = await client.SendAsync(request);
                        _logger.LogInfo($"GetEmailTemplatebyProcessType is doing");
                        if (response1.StatusCode != HttpStatusCode.OK)
                            _logger.LogError($"Something went wrong inside GetEmailTemplatebyProcessType During Email action");
                        var EmailTempString = await response1.Content.ReadAsStringAsync();
                        var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                        var Operations = "From,CreateOQC";
                        var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                        request2.Headers.Add("Authorization", token);
                        var response2 = await client.SendAsync(request2);
                        _logger.LogInfo($"GetEmailIdDetailsbyOperation is doing");
                        if (response2.StatusCode != HttpStatusCode.OK)
                            _logger.LogError($"Something went wrong inside GetEmailIdDetailsbyOperation During Email action");
                        var EmailTempString1 = await response2.Content.ReadAsStringAsync();
                        var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);
                        var httpclientHandler = new HttpClientHandler();
                        var httpClient = new HttpClient(httpclientHandler);
                        var mails = (emaildetails1.data.Where(x => x.operation == "CreateOQC").Select(x => x.emailIds).FirstOrDefault()).Split(',');
                        var email = new MimeMessage();
                        email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));

                        email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));

                        email.Subject = emaildetails.data.subject;
                        string body = emaildetails.data.template;

                        body = body.Replace("{{ShopOrderNo}}", shopOrderDetails.ShopOrderNumber);
                        List<string>? SalesorderList = new List<string>();
                        string? salesorderNos = null;
                        foreach (var item in shopOrderDetails.ShopOrderItems)
                        {
                            if (salesorderNos == null)
                            {
                                salesorderNos = item.SalesOrderNumber;
                                SalesorderList.Add(item.SalesOrderNumber);
                            }
                            else
                            {
                                if (!SalesorderList.Contains(item.SalesOrderNumber))
                                {
                                    salesorderNos = salesorderNos + "," + item.SalesOrderNumber;
                                    SalesorderList.Add(item.SalesOrderNumber);
                                }
                            }
                        }

                        body = body.Replace("{{SalesOrderNo}}", salesorderNos);
                        body = body.Replace("{{ConfirmedBy}}", oQCCreate.CreatedBy);
                        body = body.Replace("{{ProjectNo}}", shopOrderDetails.ShopOrderItems[0].ProjectNumber);
                        body = body.Replace("{{Sl.No}}", "1");
                        body = body.Replace("{{ItemNumbers}}", shopOrderDetails.ItemNumber);
                        body = body.Replace("{{ItemDesc}}", shopOrderDetails.Description);
                        body = body.Replace("{{RevNo}}", shopOrderDetails.BomRevisionNo.ToString());
                        body = body.Replace("{{ProductQty}}",  shopOrderDetails.WipQty.ToString());
                        body = body.Replace("{{WIPQty}}", (shopOrderDetails.TotalSOReleaseQty - shopOrderDetails.WipQty).ToString());
                        body = body.Replace("{{ShopOrderQty}}", shopOrderDetails.TotalSOReleaseQty.ToString());
                        body = body.Replace("{{AcceptedQty}}", oQCCreate.AcceptedQty.ToString());
                        body = body.Replace("{{RejectQty}}", oQCCreate.RejectedQty.ToString());
                        var ItemNumber = shopOrderDetails.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);
                        var request3 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request3.Headers.Add("Authorization", token);
                        var itemMasterObjectResult = await client.SendAsync(request3);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                            _logger.LogError($"Something went wrong inside GetItemMasterByItemNumber During Email action");
                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                        dynamic itemMasterObject = itemMasterObjectData.data;
                        string uom = itemMasterObject.uom;
                        body = body.Replace("{{UOM}}", uom);

                        email.Body = new TextPart(TextFormat.Html) { Text = body };
                        _logger.LogInfo($"SmtpClient is doing");

                        using var smtp = new MailKit.Net.Smtp.SmtpClient();
                        int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                        smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                        smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                        smtp.Send(email);
                        smtp.Disconnect(true);

                    }
                }
                else
                {
                    _logger.LogError($"Something went wrong inside CreateOQC action. Inventory update action Other Service Calling  failed! ");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Internal server error";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }

                serviceResponse.Data = null;
                serviceResponse.Message = "OQC Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateOQC API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in CreateOQC API : \n {ex.Message} \n{ex.InnerException}");
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
        public async Task<IActionResult> UpdateOQC(int id, [FromBody] OQCUpdateDto oQCUpdateDto)
        {
            ServiceResponse<OQCDto> serviceResponse = new ServiceResponse<OQCDto>();

            try
            {
                if (oQCUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update OQC object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update OQC object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update OQC object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update OQC object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var oQCDetailsById = await _oQCRepository.GetOQCById(id);
                if (oQCDetailsById is null)
                {
                    _logger.LogError($"Update OQC with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update OQC hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(oQCUpdateDto, oQCDetailsById);
                string result = await _oQCRepository.UpdateOQC(oQCDetailsById);
                _logger.LogInfo(result);
                _oQCRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "OQC Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateOQC API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateOQC API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOQC(int id)
        {
            ServiceResponse<OQCDto> serviceResponse = new ServiceResponse<OQCDto>();

            try
            {
                var oQCDetailsById = await _oQCRepository.GetOQCById(id);
                if (oQCDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete OQC object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete OQC with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _oQCRepository.DeleteOQC(oQCDetailsById);
                _logger.LogInfo(result);
                _oQCRepository.SaveAsync();
                serviceResponse.Message = "OQC Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteOQC API for the following id : {id} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteOQC API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShopOrderConfirmationItemNoByFGItemType()
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationItemNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationItemNoListDto>>();

            try
            {
                var shopOrderConfirmationItemNoList = await _oQCRepository.GetShopOrderConfirmationItemNoByFGItemType();
                if (shopOrderConfirmationItemNoList == null)
                {
                    _logger.LogError($"FGItemType hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"FGItemType hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ShopOrderConfirmationItemNo By FGItemType");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationItemNoListDto>>(shopOrderConfirmationItemNoList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderConfirmationItemNoList By FGItemType  Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetShopOrderConfirmationItemNoByFGItemType API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetShopOrderConfirmationItemNoByFGItemType API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShopOrderConfirmationItemNoBySAItemType()
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationItemNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationItemNoListDto>>();

            try
            {
                var shopOrderConfirmationItemNoList = await _oQCRepository.GetShopOrderConfirmationItemNoBySAItemType();
                if (shopOrderConfirmationItemNoList == null)
                {
                    _logger.LogError($"SAItemType hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SAItemType hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ShopOrderConfirmationItemNo By SAItemType");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationItemNoListDto>>(shopOrderConfirmationItemNoList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderConfirmationItemNoList By SAItemType  Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetShopOrderConfirmationItemNoBySAItemType API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetShopOrderConfirmationItemNoBySAItemType API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetShopOrderConfirmationFGSAItemTypeDetailsByItemNo(string itemNumber)
        {
            ServiceResponse<IEnumerable<ShopOrderConfirmationDetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<ShopOrderConfirmationDetailsDto>>();

            try
            {
                var shopOrderConfirmationDeails = await _oQCRepository.GetShopOrderConfirmationDetailsByItemNo(itemNumber);
                if (shopOrderConfirmationDeails == null)
                {
                    _logger.LogError($"ShopOrderConfirmationItemNo hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ShopOrderConfirmationItemNo hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ShopOrderConfirmationDetails By ItemNo");
                    var result = _mapper.Map<IEnumerable<ShopOrderConfirmationDetailsDto>>(shopOrderConfirmationDeails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShopOrderConfirmationFGSAItemTypeDetails ByItemNo Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetShopOrderConfirmationFGSAItemTypeDetailsByItemNo API for the following Itemnumber : {itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetShopOrderConfirmationFGSAItemTypeDetailsByItemNo API for the following Itemnumber : {itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOQCIdNameList()
        {
            ServiceResponse<IEnumerable<OQCIdNameList>> serviceResponse = new ServiceResponse<IEnumerable<OQCIdNameList>>();
            try
            {
                var listOfAllOQCIdNames = await _oQCRepository.GetAllOQCIdNameList();
                var result = _mapper.Map<IEnumerable<OQCIdNameList>>(listOfAllOQCIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All listOfAllOQCIdNames";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllOQCIdNameList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllOQCIdNameList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetStockByItemFromOqc(string Itemnumber)
        {
            ServiceResponse<List<OQCStock>> serviceResponse = new ServiceResponse<List<OQCStock>>();
            try
            {
                var OQCAcceptedQty = await _oQCRepository.GetOQCAcceptedQty(Itemnumber);
                if (OQCAcceptedQty != null && OQCAcceptedQty.Count() > 0)
                {
                    var OqcBinningQty = await _oQCBinningRepository.GetOqcBinningShopOrderQty(Itemnumber);
                    List<OQCStock>? result = new List<OQCStock>();
                    if (OqcBinningQty != null && OqcBinningQty.Count() > 0)
                    {
                        foreach (var Pro in OQCAcceptedQty)
                        {
                            var OqcBin = OqcBinningQty.Where(x => x.ShopOrderNumber == Pro.ShopOrderNumber).FirstOrDefault();
                            if (OqcBin != null)
                            {
                                Pro.TotalAcceptedQty -= OqcBin.TotalAcceptedQty;
                                if (Pro.TotalAcceptedQty > 0) result.Add(Pro);
                                else continue;
                            }
                            else result.Add(Pro);
                        }
                    }
                    else result.AddRange(OQCAcceptedQty);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned All GetFGStockByItemFromOqc";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"No ShopOrder Found for itemnumber: {Itemnumber}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetStockByItemFromOqc API for the following Itemnumber : {Itemnumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetStockByItemFromOqc API for the following Itemnumber : {Itemnumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetOQCSPReportWithParam([FromBody] OQCSPReportDto oqcSPReportDto)
        {
            ServiceResponse<IEnumerable<OQCSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OQCSPReport>>();
            try
            {
                var products = await _oQCRepository.GetOQCSPReportWithParam(oqcSPReportDto.ItemNumber,oqcSPReportDto.ShopOrderNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQC hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OQC hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OQC Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOQCSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOQCSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetOQCSPReportWithParamForTrans([FromBody] OQCSPReportForTransDto oqcSPReportDto)
        {
            ServiceResponse<IEnumerable<OQCSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OQCSPReport>>();
            try
            {
                var products = await _oQCRepository.GetOQCSPReportWithParamForTrans(oqcSPReportDto.ItemNumber, oqcSPReportDto.ShopOrderNumber, oqcSPReportDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQC hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OQC hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OQC Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOQCSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOQCSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetOQCSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<OQCSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OQCSPReport>>();
            try
            {
                var products = await _oQCRepository.GetOQCSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQC WithDate hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OQC WithDate hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OQCWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOQCSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOQCSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetOQCPendingSPReportWithParamForTrans([FromBody] OQCSPReportForTransDto oqcSPReportDto)
        {
            ServiceResponse<IEnumerable<OQCPendingForTrans>> serviceResponse = new ServiceResponse<IEnumerable<OQCPendingForTrans>>();
            try
            {
                var products = await _oQCRepository.GetOQCPendingSPReportWithParamForTrans(oqcSPReportDto.ItemNumber, oqcSPReportDto.ShopOrderNumber, oqcSPReportDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQCPending hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OQCPending hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OQCPending Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOQCPendingSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOQCPendingSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        

        [HttpPost]
        public async Task<IActionResult> GetOQCAndOQCBinningSPReportWithParam([FromBody] OQCSPReportDto oqcSPReportDto)
        {
            ServiceResponse<IEnumerable<OQCAndOQCBinningSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OQCAndOQCBinningSPReport>>();
            try
            {
                var products = await _oQCRepository.GetOQCAndOQCBinningSPReportWithParam(oqcSPReportDto.ItemNumber, oqcSPReportDto.ShopOrderNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQC And OQCBinning hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OQC And OQCBinning hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OQC And OQCBinning Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOQCAndOQCBinningSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOQCAndOQCBinningSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetOQCAndOQCBinningSPReportWithParamForTrans([FromBody] OQCSPReportForTransDto oqcSPReportDto)
        {
            ServiceResponse<IEnumerable<OQCAndOQCBinningSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OQCAndOQCBinningSPReport>>();
            try
            {
                var products = await _oQCRepository.GetOQCAndOQCBinningSPReportWithParamForTrans(oqcSPReportDto.ItemNumber, oqcSPReportDto.ShopOrderNumber,
                                                                                                                oqcSPReportDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQC And OQCBinning hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OQC And OQCBinning hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OQC And OQCBinning Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOQCAndOQCBinningSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOQCAndOQCBinningSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetOQCAndOQCBinningSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<OQCAndOQCBinningSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OQCAndOQCBinningSPReport>>();
            try
            {
                var products = await _oQCRepository.GetOQCAndOQCBinningSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQC And OQCBinning WithDate hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OQC And OQCBinning WithDate hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OQCAndOQCBinningWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetOQCAndOQCBinningSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetOQCAndOQCBinningSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
