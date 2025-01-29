using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
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
                _logger.LogError($"Something went wrong inside GetLocationTransferPartNoById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]

        public async Task<IActionResult> CreateLocationTransferPartNo([FromBody] List<LocationTransferPartNoPostDto> locationTransferPartNoPostDto)
        {
            ServiceResponse<LocationTransferPartNoDto> serviceResponse = new ServiceResponse<LocationTransferPartNoDto>();
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
                    await _locationTransferPartNoRepository.CreateLocationTransferPartNo(loca);

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
                    var toUOM = loca.ToUOM;

                    var availstock = loca.AvailableStockInLocation;
                    var transferQty = loca.TransferQty;

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var encodedItemNumber = Uri.EscapeDataString(toPartNumber);
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                            $"GetItemMasterByItemNumberAndPartType?ItemNumber={encodedItemNumber}&PartType={fromPartType}"));
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
                        var inventoryDetails = await _inventoryRepository.GetInventoryDetailsByItemNumberandLotNumber(toPartNumber, toProjectNumber);
                        if (inventoryDetails != null && inventoryDetails.Count() > 0)
                        {
                            foreach (var inventoryItem in inventoryDetails)
                            {
                                var LocaTransId = await _locationTransferPartNoRepository.GetLatestLocationTransferPartNoId();
                                string LocationTransReferId = Convert.ToString(LocaTransId + 1);

                                var balQty = inventoryItem.Balance_Quantity;
                                if (transferQty >= balQty)
                                {
                                    inventoryItem.Balance_Quantity -= Convert.ToDecimal(transferQty);
                                    inventoryItem.IsStockAvailable = false;
                                    inventoryItem.ReferenceID = LocationTransReferId;
                                    inventoryItem.ReferenceIDFrom = "LocationTransfer";
                                    await _inventoryRepository.UpdateInventory(inventoryItem);

                                    InventoryTranction inventoryTranctionPost = new InventoryTranction();
                                    inventoryTranctionPost.PartNumber = inventoryItem.PartNumber;
                                    inventoryTranctionPost.MftrPartNumber = inventoryItem.MftrPartNumber;
                                    inventoryTranctionPost.ProjectNumber = inventoryItem.ProjectNumber;
                                    inventoryTranctionPost.Description = inventoryItem.Description;
                                    inventoryTranctionPost.Issued_Quantity = Convert.ToDecimal(transferQty);
                                    inventoryTranctionPost.UOM = inventoryItem.UOM;
                                    inventoryTranctionPost.GrinMaterialType = "";
                                    inventoryTranctionPost.shopOrderNo = "";
                                    inventoryTranctionPost.Unit = inventoryItem.Unit;
                                    inventoryTranctionPost.GrinNo = "";
                                    inventoryTranctionPost.GrinPartId = inventoryItem.GrinPartId;
                                    inventoryTranctionPost.IsStockAvailable = true;
                                    inventoryTranctionPost.Warehouse = inventoryItem.Warehouse;
                                    inventoryTranctionPost.From_Location = fromLocation;
                                    inventoryTranctionPost.TO_Location = inventoryItem.Location;
                                    inventoryTranctionPost.PartType = inventoryItem.PartType;
                                    inventoryTranctionPost.ReferenceID = inventoryItem.ReferenceID;
                                    inventoryTranctionPost.ReferenceIDFrom = "LocationTransfer";
                                    inventoryTranctionPost.Remarks = "LocationTransfer Done";
                                    await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost);

                                    //_inventoryTranctionRepository.SaveAsync();

                                    inventoryItem.PartNumber = toPartNumber;
                                    inventoryItem.Description = toDescription;
                                    inventoryItem.ProjectNumber = toProjectNumber;
                                    inventoryItem.LotNumber = inventoryItem.LotNumber;
                                    inventoryItem.MftrPartNumber = itemObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                                    inventoryItem.UOM = toUOM;
                                    inventoryItem.Min = inventoryItem.Min;
                                    inventoryItem.Max = inventoryItem.Max;
                                    inventoryItem.Balance_Quantity -= Convert.ToDecimal(transferQty);
                                    inventoryItem.GrinMaterialType = inventoryItem.GrinMaterialType;
                                    inventoryItem.shopOrderNo = inventoryItem.shopOrderNo;
                                    inventoryItem.Unit = inventoryItem.Unit;
                                    inventoryItem.GrinNo = inventoryItem.GrinNo;
                                    inventoryItem.GrinPartId = inventoryItem.GrinPartId;
                                    inventoryItem.Warehouse = fromWarehouse;
                                    inventoryItem.Location = fromLocation;
                                    inventoryItem.PartType = fromPartType;
                                    inventoryItem.ReferenceID = LocationTransReferId;
                                    inventoryItem.ReferenceIDFrom = "LocationTransferPartNo";
                                    await _inventoryRepository.CreateInventory(inventoryItem);

                                    transferQty -= balQty;

                                    InventoryTranction inventoryTranctionPost1 = new InventoryTranction();
                                    inventoryTranctionPost1.PartNumber = inventoryItem.PartNumber;
                                    inventoryTranctionPost1.MftrPartNumber = inventoryItem.MftrPartNumber;
                                    inventoryTranctionPost1.ProjectNumber = inventoryItem.ProjectNumber;
                                    inventoryTranctionPost1.Description = inventoryItem.Description;
                                    inventoryTranctionPost1.Issued_Quantity = inventoryItem.Balance_Quantity;
                                    inventoryTranctionPost1.UOM = inventoryItem.UOM;
                                    inventoryTranctionPost1.GrinMaterialType = inventoryItem.GrinMaterialType;
                                    inventoryTranctionPost1.shopOrderNo = inventoryItem.shopOrderNo;
                                    inventoryTranctionPost1.Unit = inventoryItem.Unit;
                                    inventoryTranctionPost1.GrinNo = inventoryItem.GrinNo;
                                    inventoryTranctionPost1.GrinPartId = inventoryItem.GrinPartId;
                                    inventoryTranctionPost1.IsStockAvailable = true;
                                    inventoryTranctionPost1.Warehouse = inventoryItem.Warehouse;
                                    inventoryTranctionPost1.From_Location = fromLocation;
                                    inventoryTranctionPost1.TO_Location = inventoryItem.Location;
                                    inventoryTranctionPost1.PartType = inventoryItem.PartType;
                                    inventoryTranctionPost1.ReferenceID = inventoryItem.ReferenceID;
                                    inventoryTranctionPost1.ReferenceIDFrom = "LocationTransferPartNo";
                                    await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost1);

                                    //_inventoryTranctionRepository.SaveAsync();

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
                                    inventoryTranctionPost.Issued_Quantity = Convert.ToDecimal(transferQty);
                                    inventoryTranctionPost.UOM = inventoryItem.UOM;
                                    inventoryTranctionPost.GrinMaterialType = inventoryItem.GrinMaterialType;
                                    inventoryTranctionPost.shopOrderNo = inventoryItem.shopOrderNo;
                                    inventoryTranctionPost.Unit = inventoryItem.Unit;
                                    inventoryTranctionPost.GrinNo = inventoryItem.GrinNo;
                                    inventoryTranctionPost.GrinPartId = inventoryItem.GrinPartId;
                                    inventoryTranctionPost.IsStockAvailable = true;
                                    inventoryTranctionPost.Warehouse = inventoryItem.Warehouse;
                                    inventoryTranctionPost.From_Location = fromLocation;
                                    inventoryTranctionPost.TO_Location = inventoryItem.Location;
                                    inventoryTranctionPost.PartType = inventoryItem.PartType;
                                    inventoryTranctionPost.ReferenceID = LocationTransReferId; /*Convert.ToString(loca.Id)*/;
                                    inventoryTranctionPost.ReferenceIDFrom = "LocationTransferPartNo";
                                    inventoryTranctionPost.Remarks = "LocationTransferPartNo Done";
                                    await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost);

                                    //_inventoryTranctionRepository.SaveAsync();

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
                                    inventoryPost.PartType = fromPartType;
                                    inventoryPost.ReferenceID = LocationTransReferId; 
                                    inventoryPost.ReferenceIDFrom = "LocationTransferPartNo";
                                    await _inventoryRepository.CreateInventory(inventoryPost);

                                    InventoryTranction inventoryTranctionPost1 = new InventoryTranction();
                                    inventoryTranctionPost1.PartNumber = inventoryPost.PartNumber;
                                    inventoryTranctionPost1.MftrPartNumber = inventoryPost.MftrPartNumber;
                                    inventoryTranctionPost1.ProjectNumber = inventoryPost.ProjectNumber;
                                    inventoryTranctionPost1.Description = inventoryPost.Description;
                                    inventoryTranctionPost1.Issued_Quantity = Convert.ToDecimal(transferQty);
                                    inventoryTranctionPost1.UOM = inventoryPost.UOM;
                                    inventoryTranctionPost1.GrinMaterialType = inventoryPost.GrinMaterialType;
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
                                    await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost1);



                                    transferQty = 0;
                                }
                                if (transferQty <= 0)
                                {
                                    break;
                                }
                            }
                            _inventoryRepository.SaveAsync();
                            _inventoryTranctionRepository.SaveAsync();
                        }
                        else
                        {
                            var LocaTransId = await _locationTransferPartNoRepository.GetLatestLocationTransferPartNoId();
                            string LocationTransReferId = Convert.ToString(LocaTransId + 1);

                            Inventory inventoryPost = new Inventory();
                            inventoryPost.PartNumber = toPartNumber;
                            inventoryPost.Description = toDescription;
                            inventoryPost.LotNumber = fromLotnumber;
                            inventoryPost.MftrPartNumber = itemObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                            inventoryPost.ProjectNumber = toProjectNumber;
                            inventoryPost.Balance_Quantity = Convert.ToDecimal(transferQty);
                            inventoryPost.IsStockAvailable = true;
                            inventoryPost.UOM = toUOM;
                            inventoryPost.Min = itemObject.min;
                            inventoryPost.Max = itemObject.max;
                            inventoryPost.GrinMaterialType = "";
                            inventoryPost.shopOrderNo = "";
                            inventoryPost.GrinNo = "";
                            inventoryPost.GrinPartId =  0;
                            inventoryPost.Unit = _unitname;
                            inventoryPost.Warehouse = fromWarehouse;
                            inventoryPost.Location = fromLocation;
                            inventoryPost.PartType = fromPartType;
                            inventoryPost.ReferenceID = LocationTransReferId;
                            inventoryPost.ReferenceIDFrom = "LocationTransferPartNo";
                            await _inventoryRepository.CreateInventory(inventoryPost);

                            InventoryTranction inventoryTranctionPost1 = new InventoryTranction();
                            inventoryTranctionPost1.PartNumber = inventoryPost.PartNumber;
                            inventoryTranctionPost1.MftrPartNumber = inventoryPost.MftrPartNumber;
                            inventoryTranctionPost1.ProjectNumber = inventoryPost.ProjectNumber;
                            inventoryTranctionPost1.Description = inventoryPost.Description;
                            inventoryTranctionPost1.Issued_Quantity = Convert.ToDecimal(transferQty);
                            inventoryTranctionPost1.UOM = inventoryPost.UOM;
                            inventoryTranctionPost1.GrinMaterialType = inventoryPost.GrinMaterialType;
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
                            await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost1);

                            _inventoryRepository.SaveAsync();
                            _inventoryTranctionRepository.SaveAsync();
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
                serviceResponse.Message = "locationTransferPartNo Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreatelocationTransferPartNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
