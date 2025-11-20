using AutoMapper;
using Contracts;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LocationTransferForPartNoController : ControllerBase
    {
        private ILocationTransferPartNoRepository _locationTransferPartNoRepository;
        private IInventoryRepository _inventoryRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private IMapper _mapper;
        private ILoggerManager _logger;
        private object _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly String _createdBy;
        private readonly String _unitname;
        public LocationTransferForPartNoController(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, IInventoryTranctionRepository inventoryTranctionRepository, IInventoryRepository inventoryRepository, ILocationTransferPartNoRepository locationTransferPartNoRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _locationTransferPartNoRepository = locationTransferPartNoRepository;
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            var jwtClaims = _httpContextAccessor.HttpContext.User.Claims;
            _createdBy = jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name) != null ? jwtClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value : "Admin";
            _unitname = jwtClaims.FirstOrDefault(c => c.Type == "UnitName")?.Value ?? "Hyderabad";
        }
        [HttpGet]
        public async Task<IActionResult> GetAllLocationTransferPartNo([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<LocationTransferPartNoDto>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferPartNoDto>>();

            try
            {
                var locationTransfersDetails = await _locationTransferPartNoRepository.GetAllLocationTransferPartNo(pagingParameter, searchParammes);

                var metadata = new
                {
                    locationTransfersDetails.TotalCount,
                    locationTransfersDetails.PageSize,
                    locationTransfersDetails.CurrentPage,
                    locationTransfersDetails.HasNext,
                    locationTransfersDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogError("Returned all LocationTransferPartNodetails");
                var result = _mapper.Map<IEnumerable<LocationTransferPartNoDto>>(locationTransfersDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LocationTransfersPartNo Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLocationTransferPartNo API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLocationTransferPartNo API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationTransferPartNoById(int id)
        {
            ServiceResponse<LocationTransferPartNoDto> serviceResponse = new ServiceResponse<LocationTransferPartNoDto>();

            try
            {
                var locationTransFerDetails = await _locationTransferPartNoRepository.GetLocationTransferPartNoById(id);

                if (locationTransFerDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"LocationTransferForPartNoDetails with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"LocationTransferForPartNoDetails with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned LocationTransferForPartNo with id: {id}");
                    var result = _mapper.Map<LocationTransferPartNoDto>(locationTransFerDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned LocationTransferForPartNo with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetLocationTransferPartNoById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetLocationTransferPartNoById API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]

        public async Task<IActionResult> CreateLocationTransferPartNo([FromBody] List<LocationTransferPartNoPostDto> locationTransferPartNoPostDto)
        {
            ServiceResponse<LocationTransferPartNoDto> serviceResponse = new ServiceResponse<LocationTransferPartNoDto>();
            string warningMessage = "";
            try
            {
                if (locationTransferPartNoPostDto is null)
                {
                    _logger.LogError("locationTransferPartNo object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "locationTransferPartNo object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid locationTransferPartNo object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid locationTransferPartNo object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                HttpStatusCode GetItemMas = HttpStatusCode.OK;
                var createLocationTransferPartNo = _mapper.Map<List<LocationTransferPartNo>>(locationTransferPartNoPostDto);
                foreach (var loca in createLocationTransferPartNo)
                {
                    var fromPartNumber = loca.FromPartNumber;
                    var toPartNumber = loca.ToPartNumber;
                    var fromDescription = loca.FromDescription;
                    var toDescription = loca.ToDescription;
                    var fromLotnumber = loca.FromLotNumber;
                    var fromProjectNumber = loca.FromProjectNumber;
                    var toProjectNumber = loca.ToProjectNumber;
                    var fromLocation = loca.FromLocation;
                    var fromWarehouse = loca.FromWarehouse;
                    var fromPartType = loca.FromPartType;
                    var toPartType = loca.ToPartType;
                    var toUOM = loca.ToUOM;

                    var availstock = loca.AvailableStockInLocation;
                    var transferQty = loca.TransferQty;

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var encodedItemNumber = Uri.EscapeDataString(toPartNumber);
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                            $"GetItemMasterByItemNumberAndPartType?ItemNumber={encodedItemNumber}&PartType={toPartType}"));
                    request.Headers.Add("Authorization", token);

                    var itemDetailFromItemmaster = await client.SendAsync(request);
                    if (itemDetailFromItemmaster.StatusCode != HttpStatusCode.OK)
                    {
                        GetItemMas = itemDetailFromItemmaster.StatusCode;
                    }
                    if (GetItemMas == HttpStatusCode.OK)
                    {

                        _logger.LogInfo("getitemmasterdata" + Convert.ToString(itemDetailFromItemmaster));
                        var itemDetail = await itemDetailFromItemmaster.Content.ReadAsStringAsync();
                        var itemData = JsonConvert.DeserializeObject<LocationTransItemMasterDetails>(itemDetail);
                        var itemObject = itemData.data;

                        //Add Inventory table
                        var inventoryDetails = await _inventoryRepository.GetInventoryDetailsByItemNumberandLotNumber(fromPartNumber, fromProjectNumber, fromLotnumber);
                        if (inventoryDetails != null && inventoryDetails.Count() > 0)
                        {
                            var inventoryBalQty = inventoryDetails.Sum(x => x.Balance_Quantity);
                            if (inventoryBalQty >= transferQty)
                            {
                                foreach (var inventoryItem in inventoryDetails)
                                {
                                    var LocaTransId = await _locationTransferPartNoRepository.GetLatestLocationTransferPartNoId();
                                    string LocationTransReferId = Convert.ToString(LocaTransId + 1);

                                    var balQty = inventoryItem.Balance_Quantity;
                                    if (transferQty >= balQty)
                                    {
                                        InventoryTranction inventoryTranctionPost = new InventoryTranction();
                                        inventoryTranctionPost.PartNumber = inventoryItem.PartNumber;
                                        inventoryTranctionPost.MftrPartNumber = inventoryItem.MftrPartNumber;
                                        inventoryTranctionPost.ProjectNumber = inventoryItem.ProjectNumber;
                                        inventoryTranctionPost.Description = inventoryItem.Description;
                                        inventoryTranctionPost.LotNumber = inventoryItem.LotNumber;
                                        inventoryTranctionPost.Issued_Quantity = inventoryItem.Balance_Quantity;
                                        inventoryTranctionPost.UOM = inventoryItem.UOM;
                                        inventoryTranctionPost.GrinMaterialType = "";
                                        inventoryTranctionPost.shopOrderNo = inventoryItem.shopOrderNo;
                                        inventoryTranctionPost.Unit = inventoryItem.Unit;
                                        inventoryTranctionPost.GrinNo = inventoryItem.GrinNo;
                                        inventoryTranctionPost.GrinPartId = inventoryItem.GrinPartId;
                                        inventoryTranctionPost.IsStockAvailable = false;
                                        inventoryTranctionPost.Warehouse = inventoryItem.Warehouse;
                                        inventoryTranctionPost.From_Location = inventoryItem.Location;
                                        inventoryTranctionPost.TO_Location = inventoryItem.Location;
                                        inventoryTranctionPost.PartType = inventoryItem.PartType;
                                        inventoryTranctionPost.ReferenceID = inventoryItem.ReferenceID;
                                        inventoryTranctionPost.ReferenceIDFrom = inventoryItem.ReferenceIDFrom;
                                        inventoryTranctionPost.Remarks = "LocationTransferPartNo Done";
                                        inventoryTranctionPost.TransactionType = InventoryType.Outward;

                                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost);


                                        inventoryItem.Balance_Quantity = Convert.ToDecimal(transferQty) - inventoryItem.Balance_Quantity;
                                        if (inventoryItem.Balance_Quantity == 0)
                                        {
                                            inventoryItem.IsStockAvailable = false;
                                        }
                                        else
                                        {
                                            inventoryItem.IsStockAvailable = true;
                                        }
                                        inventoryItem.ReferenceID = LocationTransReferId;
                                        inventoryItem.ReferenceIDFrom = "LocationTransferPartNo";

                                        await _inventoryRepository.UpdateInventory(inventoryItem);


                                        Inventory inventoryPost = new Inventory();
                                        inventoryPost.PartNumber = toPartNumber;
                                        inventoryPost.Description = toDescription;
                                        inventoryPost.ProjectNumber = toProjectNumber;
                                        inventoryPost.LotNumber = inventoryItem.LotNumber;
                                        inventoryPost.MftrPartNumber = itemObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                                        inventoryPost.UOM = toUOM;
                                        inventoryPost.Min = inventoryItem.Min;
                                        inventoryPost.Max = inventoryItem.Max;
                                        inventoryPost.Balance_Quantity = Convert.ToDecimal(transferQty);
                                        inventoryPost.GrinMaterialType = inventoryItem.GrinMaterialType;
                                        inventoryPost.shopOrderNo = inventoryItem.shopOrderNo;
                                        inventoryPost.Unit = inventoryItem.Unit;
                                        inventoryPost.GrinNo = inventoryItem.GrinNo;
                                        inventoryPost.GrinPartId = inventoryItem.GrinPartId;
                                        inventoryPost.Warehouse = fromWarehouse;
                                        inventoryPost.Location = fromLocation;
                                        inventoryPost.PartType = toPartType;
                                        inventoryPost.ReferenceID = LocationTransReferId;
                                        inventoryPost.ReferenceIDFrom = "LocationTransferPartNo";

                                        await _inventoryRepository.CreateInventory(inventoryPost);



                                        InventoryTranction inventoryTranctionPost_1 = new InventoryTranction();
                                        inventoryTranctionPost_1.PartNumber = inventoryPost.PartNumber;
                                        inventoryTranctionPost_1.MftrPartNumber = inventoryPost.MftrPartNumber;
                                        inventoryTranctionPost_1.ProjectNumber = inventoryPost.ProjectNumber;
                                        inventoryTranctionPost_1.Description = inventoryPost.Description;
                                        inventoryTranctionPost_1.Issued_Quantity = inventoryPost.Balance_Quantity;
                                        inventoryTranctionPost_1.LotNumber = inventoryPost.LotNumber;
                                        inventoryTranctionPost_1.UOM = toUOM;
                                        inventoryTranctionPost_1.GrinMaterialType = "";
                                        inventoryTranctionPost_1.shopOrderNo = inventoryPost.shopOrderNo;
                                        inventoryTranctionPost_1.Unit = inventoryPost.Unit;
                                        inventoryTranctionPost_1.GrinNo = inventoryPost.GrinNo;
                                        inventoryTranctionPost_1.GrinPartId = inventoryPost.GrinPartId;
                                        inventoryTranctionPost_1.IsStockAvailable = true;
                                        inventoryTranctionPost_1.Warehouse = inventoryPost.Warehouse;
                                        inventoryTranctionPost_1.From_Location = fromLocation;
                                        inventoryTranctionPost_1.TO_Location = inventoryPost.Location;
                                        inventoryTranctionPost_1.PartType = inventoryPost.PartType;
                                        inventoryTranctionPost_1.ReferenceID = inventoryPost.ReferenceID;
                                        inventoryTranctionPost_1.ReferenceIDFrom = "LocationTransferPartNo";
                                        inventoryTranctionPost_1.Remarks = "LocationTransferPartNo Done";
                                        inventoryTranctionPost_1.TransactionType = InventoryType.Inward;

                                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost_1);

                                        _inventoryTranctionRepository.SaveAsync();

                                        _inventoryRepository.SaveAsync();

                                        transferQty -= balQty;
                                        balQty = 0;
                                    }
                                    else
                                    {
                                        inventoryItem.Balance_Quantity -= Convert.ToDecimal(transferQty);
                                        await _inventoryRepository.UpdateInventory(inventoryItem);

                                        InventoryTranction inventoryTranctionPost = new InventoryTranction();
                                        inventoryTranctionPost.PartNumber = inventoryItem.PartNumber;
                                        inventoryTranctionPost.MftrPartNumber = itemObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                                        inventoryTranctionPost.ProjectNumber = inventoryItem.ProjectNumber;
                                        inventoryTranctionPost.Description = inventoryItem.Description;
                                        inventoryTranctionPost.LotNumber = inventoryItem.LotNumber;
                                        inventoryTranctionPost.Issued_Quantity = Convert.ToDecimal(transferQty);
                                        inventoryTranctionPost.UOM = inventoryItem.UOM;
                                        inventoryTranctionPost.GrinMaterialType = "";
                                        inventoryTranctionPost.shopOrderNo = inventoryItem.shopOrderNo;
                                        inventoryTranctionPost.Unit = inventoryItem.Unit;
                                        inventoryTranctionPost.GrinNo = inventoryItem.GrinNo;
                                        inventoryTranctionPost.GrinPartId = inventoryItem.GrinPartId;
                                        inventoryTranctionPost.IsStockAvailable = true;
                                        inventoryTranctionPost.Warehouse = inventoryItem.Warehouse;
                                        inventoryTranctionPost.From_Location = inventoryItem.Location;
                                        inventoryTranctionPost.TO_Location = inventoryItem.Location;
                                        inventoryTranctionPost.PartType = inventoryItem.PartType;
                                        inventoryTranctionPost.ReferenceID = inventoryItem.ReferenceID;
                                        inventoryTranctionPost.ReferenceIDFrom = inventoryItem.ReferenceIDFrom;
                                        inventoryTranctionPost.Remarks = "LocationTransferPartNo Done";
                                        inventoryTranctionPost.TransactionType = InventoryType.Outward;

                                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost);


                                        Inventory inventoryPost = new Inventory();
                                        inventoryPost.PartNumber = toPartNumber;
                                        inventoryPost.Description = toDescription;
                                        inventoryPost.LotNumber = inventoryItem.LotNumber;
                                        inventoryPost.MftrPartNumber = itemObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                                        inventoryPost.ProjectNumber = toProjectNumber;
                                        inventoryPost.Balance_Quantity = Convert.ToDecimal(transferQty);
                                        inventoryPost.UOM = toUOM;
                                        inventoryPost.Min = inventoryItem.Min;
                                        inventoryPost.Max = inventoryItem.Max;
                                        inventoryPost.GrinMaterialType = inventoryItem.GrinMaterialType;
                                        inventoryPost.shopOrderNo = inventoryItem.shopOrderNo;
                                        inventoryPost.Unit = inventoryItem.Unit;
                                        inventoryPost.GrinNo = inventoryItem.GrinNo;
                                        inventoryPost.GrinPartId = inventoryItem.GrinPartId;
                                        inventoryPost.IsStockAvailable = true;
                                        inventoryPost.Warehouse = fromWarehouse;
                                        inventoryPost.Location = fromLocation;
                                        inventoryPost.PartType = toPartType;
                                        inventoryPost.ReferenceID = LocationTransReferId;
                                        inventoryPost.ReferenceIDFrom = "LocationTransferPartNo";

                                        await _inventoryRepository.CreateInventory(inventoryPost);


                                        InventoryTranction inventoryTranctionPost1 = new InventoryTranction();
                                        inventoryTranctionPost1.PartNumber = inventoryPost.PartNumber;
                                        inventoryTranctionPost1.MftrPartNumber = inventoryPost.MftrPartNumber;
                                        inventoryTranctionPost1.ProjectNumber = inventoryPost.ProjectNumber;
                                        inventoryTranctionPost1.Description = inventoryPost.Description;
                                        inventoryTranctionPost1.Issued_Quantity = inventoryPost.Balance_Quantity;
                                        inventoryTranctionPost1.LotNumber = inventoryPost.LotNumber;
                                        inventoryTranctionPost1.UOM = inventoryPost.UOM;
                                        inventoryTranctionPost1.GrinMaterialType = "";
                                        inventoryTranctionPost1.shopOrderNo = inventoryPost.shopOrderNo;
                                        inventoryTranctionPost1.Unit = inventoryPost.Unit;
                                        inventoryTranctionPost1.GrinNo = inventoryPost.GrinNo;
                                        inventoryTranctionPost1.GrinPartId = inventoryPost.GrinPartId;
                                        inventoryTranctionPost1.IsStockAvailable = true;
                                        inventoryTranctionPost1.Warehouse = inventoryPost.Warehouse;
                                        inventoryTranctionPost1.From_Location = fromLocation;
                                        inventoryTranctionPost1.TO_Location = inventoryPost.Location;
                                        inventoryTranctionPost1.PartType = inventoryPost.PartType;
                                        inventoryTranctionPost1.ReferenceID = inventoryPost.ReferenceID;
                                        inventoryTranctionPost1.ReferenceIDFrom = "LocationTransferPartNo";
                                        inventoryTranctionPost1.Remarks = "LocationTransferPartNo Done";
                                        inventoryTranctionPost1.TransactionType = InventoryType.Inward;

                                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost1);

                                        _inventoryTranctionRepository.SaveAsync();

                                        _inventoryRepository.SaveAsync();

                                        transferQty = 0;
                                    }
                                    if (transferQty <= 0)
                                    {
                                        break;
                                    }
                                }

                                await _locationTransferPartNoRepository.CreateLocationTransferPartNo(loca);
                            }

                            // _inventoryRepository.SaveAsync();
                            //_inventoryTranctionRepository.SaveAsync();
                        }
                        else
                        {
                            string partNumbersList = string.Join(", ", fromPartNumber);
                            warningMessage = $"For these part numbers: {partNumbersList}, quantity has been fully transferred. No available stock for PartTransfer.";
                        }

                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside locationTransferPartNo action. GetItemMaster  action Other Service Calling  failed! ");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Internal server error";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                }

                _locationTransferPartNoRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = $"locationTransferPartNo Successfully Created \n {warningMessage}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateLocationTransferPartNo API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateLocationTransferPartNo API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
