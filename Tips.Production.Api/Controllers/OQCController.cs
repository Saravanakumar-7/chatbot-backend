using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Entities.Enums;
using Tips.Production.Api.Repository;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
        public OQCController(IShopOrderConfirmationRepository shopOrderConfirmationRepository, IOQCRepository oQCRepository, IOQCBinningRepository oQCBinningRepository, IShopOrderRepository shopOrderRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _oQCRepository = oQCRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _shopOrderRepo = shopOrderRepository;
            _oQCBinningRepository = oQCBinningRepository;
            _shopOrderConfirmationRepository = shopOrderConfirmationRepository;
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                _logger.LogError($"Something went wrong inside GetOQCById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
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
                //var shopOrderNumber = oQCCreate.ShopOrderNumber;
                //var shopOrderDetails = await _shopOrderRepo.GetShopOrderDetailsByShopOrderNo(shopOrderNumber);
                //shopOrderDetails.OqcQty = shopOrderDetails.OqcQty + oQCCreate.AcceptedQty;
                //_shopOrderRepo.SaveAsync();  

                await _oQCRepository.CreateOQC(oQCCreate);
                _oQCRepository.SaveAsync();

                var shopOrderDetails = await _shopOrderRepo.GetShopOrderByShopOrderNo(oQCPostDto.ShopOrderNumber);
                shopOrderDetails.OqcQty = shopOrderDetails.OqcQty + oQCPostDto.AcceptedQty;
                if (shopOrderDetails.TotalSOReleaseQty == shopOrderDetails.OqcQty) shopOrderDetails.OQCStatus = ShopOrderConformationStatus.FullyDone;
                if (shopOrderDetails.TotalSOReleaseQty > shopOrderDetails.OqcQty) shopOrderDetails.OQCStatus = ShopOrderConformationStatus.PartiallyDone;
                await _shopOrderRepo.UpdateShopOrder(shopOrderDetails);
                _shopOrderRepo.SaveAsync();
                var SOConfirmations = await _shopOrderConfirmationRepository.GetAllShopOrderConfirmationByShopOrderNo(shopOrderDetails.ShopOrderNumber);
                var OQCQty = shopOrderDetails.OqcQty;
                foreach (var soCon in SOConfirmations)
                {
                    if (soCon.WipConfirmedQty == OQCQty) { soCon.IsOQCDone = ShopOrderConformationStatus.FullyDone; break; }
                    if (soCon.WipConfirmedQty > OQCQty) { soCon.IsOQCDone = ShopOrderConformationStatus.PartiallyDone; break; }
                    if (soCon.WipConfirmedQty < OQCQty) { soCon.IsOQCDone = ShopOrderConformationStatus.FullyDone; OQCQty -= soCon.WipConfirmedQty; }
                    _shopOrderConfirmationRepository.UpdateShopOrderConfirmation(soCon);
                }
                _shopOrderConfirmationRepository.SaveAsync();
                var shopOrderItemDetail = shopOrderDetails?.ShopOrderItems?.FirstOrDefault();
                var projectNo = shopOrderItemDetail?.ProjectNumber;
                if (oQCCreate.ItemType == PartType.SA) //sa
                {
                    var ItemNumber = oQCCreate.ItemNumber;
                    var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
                            "GetItemMasterByItemNumber?", "&ItemNumber=", ItemNumber));
                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(itemMasterObjectResult));
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMatserObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterTranctionObject = itemMatserObjectData.data;
                    InventoryPostDto inventory = new InventoryPostDto();

                    inventory.PartNumber = itemMasterTranctionObject.itemNumber;
                    inventory.MftrPartNumber = itemMasterTranctionObject.itemNumber;
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



                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(inventory));
                    var json = JsonConvert.SerializeObject(inventory);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);


                    InventoryPostDto inventory1 = new InventoryPostDto();

                    inventory1.PartNumber = itemMasterTranctionObject.itemNumber;
                    inventory1.MftrPartNumber = itemMasterTranctionObject.itemNumber;
                    inventory1.Description = itemMasterTranctionObject.description;
                    inventory1.ProjectNumber = projectNo;
                    inventory1.Balance_Quantity = oQCCreate.RejectedQty;
                    inventory1.UOM = itemMasterTranctionObject.uom;
                    inventory.Max = itemMasterTranctionObject.max;
                    inventory.Min = itemMasterTranctionObject.min;
                    inventory1.Warehouse = "Reject";
                    inventory1.Location = "Reject";
                    inventory1.GrinNo = "";
                    inventory1.GrinMaterialType = "";
                    inventory1.GrinPartId = oQCCreate.Id;
                    inventory1.PartType = oQCCreate.ItemType; // we have to take parttype from grinparts model;
                    inventory1.ReferenceID = oQCCreate.Id.ToString() + "-R";
                    inventory1.ReferenceIDFrom = "Final OQC";
                    inventory1.ShopOrderNo = oQCCreate.ShopOrderNumber;

                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(inventory1));
                    var json1 = JsonConvert.SerializeObject(inventory1);
                    var data1 = new StringContent(json1, Encoding.UTF8, "application/json");
                    var response1 = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data1);


                }
                else
                {
                    var ItemNumber = oQCCreate.ItemNumber;
                    var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
                            "GetItemMasterByItemNumber?", "&ItemNumber=", ItemNumber));

                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterTranctionObject = itemMasterObjectData.data;

                    InventoryPostDto inventory = new InventoryPostDto();
                    var ItemNo = itemMasterTranctionObject.itemNumber;
                    var Desc = itemMasterTranctionObject.description;
                    var uom = itemMasterTranctionObject.uom;
                    inventory.PartNumber = ItemNo;
                    inventory.MftrPartNumber = ItemNo;
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

                    var json = JsonConvert.SerializeObject(inventory);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);


                    //var json = JsonConvert.SerializeObject(inventory);
                    //var data = new StringContent(json, Encoding.UTF8, "application/json");
                    //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);


                    // For Rejected Item Store
                    InventoryPostDto inventory1 = new InventoryPostDto();

                    inventory1.PartNumber = ItemNo;
                    inventory1.MftrPartNumber = ItemNo;
                    inventory1.Description = Desc;
                    inventory1.ProjectNumber = projectNo;
                    inventory1.Balance_Quantity = oQCCreate.RejectedQty;
                    inventory1.UOM = uom;
                    inventory.Max = itemMasterTranctionObject.max;
                    inventory.Min = itemMasterTranctionObject.min;
                    inventory1.Warehouse = "Scrap";
                    inventory1.Location = "Reject";
                    inventory1.GrinNo = "";
                    inventory1.GrinMaterialType = "";
                    inventory1.GrinPartId = oQCCreate.Id;
                    inventory1.PartType = oQCCreate.ItemType; // we have to take parttype from grinparts model;
                    inventory1.ReferenceID = oQCCreate.Id.ToString() + "-R";
                    inventory1.ReferenceIDFrom = "Final OQC";
                    inventory1.ShopOrderNo = oQCCreate.ShopOrderNumber;

                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(inventory1));
                    //var json1 = JsonConvert.SerializeObject(inventory1);
                    //var data1 = new StringContent(json1, Encoding.UTF8, "application/json");
                    //var response1 = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data1);

                    var json1 = JsonConvert.SerializeObject(inventory1);
                    var data1 = new StringContent(json1, Encoding.UTF8, "application/json");
                    var response1 = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data1);


                }
                _logger.LogInfo("aftergettingdata");


                serviceResponse.Data = null;
                serviceResponse.Message = "OQC Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOQC action: {ex.Message} {ex.InnerException}");
                return StatusCode(500, serviceResponse);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateOQC action: {ex.Message}");
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOQC action: {ex.Message}");
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
                _logger.LogError($"Something went wrong inside GetShopOrderConfirmationItemNoByFGItemType action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                _logger.LogError($"Something went wrong inside GetShopOrderConfirmationItemNoBySAItemType action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                _logger.LogError($"Something went wrong inside GetShopOrderConfirmationFGSAItemTypeDetailsByItemNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllOQCIdNameList action: {ex.Message}";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetFGStockByItemFromOqc action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


    }
}
