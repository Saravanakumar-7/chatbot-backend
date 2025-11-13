using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Dml.Diagram;
//using NuGet.Protocol.Core.Types;
using System.Net;
using System.Net.Http;
using System.Web;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LocationTransferController : ControllerBase
    {

        private ILocationTransferRepository _locationTransferRepository;
        private IInventoryRepository _inventoryRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private IMapper _mapper;
        private ILoggerManager _logger;
        private object _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public LocationTransferController(IHttpClientFactory clientFactory, IInventoryTranctionRepository inventoryTranctionRepository, IInventoryRepository inventoryRepository, ILocationTransferRepository locationTransferRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _locationTransferRepository = locationTransferRepository;
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLocationTransfer([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<LocationTransferDto>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferDto>>();

            try
            {
                var getAllLocationTransfers = await _locationTransferRepository.GetAllLocationTransfer(pagingParameter, searchParammes);

                var metadata = new
                {
                    getAllLocationTransfers.TotalCount,
                    getAllLocationTransfers.PageSize,
                    getAllLocationTransfers.CurrentPage,
                    getAllLocationTransfers.HasNext,
                    getAllLocationTransfers.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all LocationTransferdetails");
                var result = _mapper.Map<IEnumerable<LocationTransferDto>>(getAllLocationTransfers);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LocationTransfers Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLocationTransfer API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLocationTransfer API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationTransferById(int id)
        {
            ServiceResponse<LocationTransferDto> serviceResponse = new ServiceResponse<LocationTransferDto>();

            try
            {
                var locationTransFerDetails = await _locationTransferRepository.GetLocationTransferById(id);

                if (locationTransFerDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"locationDetails with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"locationDetails with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogError($"Returned LocationTransfer with id: {id}");
                    var result = _mapper.Map<LocationTransferDto>(locationTransFerDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned LocationTransfer with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetLocationTransferById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetLocationTransferById API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LocationTransferSPReport([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<LocationTransferSPReport>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferSPReport>>();
            try
            {
                var products = await _locationTransferRepository.LocationTransferSPReport(pagingParameter);

                var metadata = new
                {
                    products.TotalCount,
                    products.PageSize,
                    products.CurrentPage,
                    products.HasNext,
                    products.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all LocationTransferSPReport");

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"LocationTransfer hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"LocationTransfer hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned LocationTransfer Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in LocationTransferSPReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in LocationTransferSPReport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchLocationTransferDate([FromQuery] SearchDateParam searchDatesParams)
        {
            ServiceResponse<IEnumerable<LocationTransferDto>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferDto>>();
            try
            {
                var locationTransFerDate = await _locationTransferRepository.SearchLocationTransferDate(searchDatesParams);
                var result = _mapper.Map<IEnumerable<LocationTransferDto>>(locationTransFerDate);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LocationTransferDate";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchLocationTransferDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchLocationTransferDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchLocationTransfer([FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<LocationTransferDto>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferDto>>();
            try
            {
                var locationTransferList = await _locationTransferRepository.SearchLocationTransfer(searchParammes);

                _logger.LogInfo("Returned all LocationTransfer");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<LocationTransferDto, LocationTransfer>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<LocationTransferDto>>(locationTransferList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LocationTransfers";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SearchLocationTransfer API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SearchLocationTransfer API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllLocationTransferWithItems([FromBody] LocationTransferSearchDto locationTransferSearchDto)
        {
            ServiceResponse<IEnumerable<LocationTransferDto>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferDto>>();
            try
            {
                var locationTransferList = await _locationTransferRepository.GetAllLocationTransferWithItems(locationTransferSearchDto);

                _logger.LogInfo("Returned all LocationTransfer");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<LocationTransferDto, LocationTransfer>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<LocationTransferDto>>(locationTransferList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all LocationTransfersWithItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLocationTransferWithItems API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLocationTransferWithItems API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]

        public async Task<IActionResult> CreateLocationTransfer([FromBody] List<LocationTransferPostDto> locationTransferPostDto)
        {
            ServiceResponse<LocationTransferDto> serviceResponse = new ServiceResponse<LocationTransferDto>();
            string warningMessage = "";
            try
            {
                if (locationTransferPostDto is null)
                {
                    _logger.LogError("locationTransfer object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "locationTransfer object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid locationTransfer object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid locationTransfer object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                HttpStatusCode GetItemMas = HttpStatusCode.OK;
                var createLocationTransfer = _mapper.Map<List<LocationTransfer>>(locationTransferPostDto);
                foreach (var loca in createLocationTransfer)
                {
                    await _locationTransferRepository.CreateLocationTransfer(loca);

                    var fromPartNumber = loca.FromPartNumber;
                    var toDescription = loca.ToDescription;
                    var toPartNumber = loca.ToPartNumber;
                    var fromLotnumber = loca.FromLotNumber;
                    var toLotnumber = loca.FromLotNumber;
                    var fromProjectNumber = loca.FromProjectNumber;
                    var toProjectNumber = loca.ToProjectNumber;
                    var fromLocation = loca.FromLocation;
                    var toLocation = loca.ToLocation;
                    var fromWarehouse = loca.FromWarehouse;
                    var toWarehouse = loca.ToWarehouse;
                    var availstock = loca.AvailableStockInLocation;
                    var transferQty = loca.TransferQty;
                    var toPartType = loca.ToPartType;
                    var toUOM = loca.ToUOM;

                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var encodedItemNumber = Uri.EscapeDataString(toPartNumber);
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemDetailFromItemmaster = await client.SendAsync(request);
                    // var itemDetailFromItemmaster = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"], "GetItemMasterByItemNumber?", "&ItemNumber=", HttpUtility.UrlEncode(toPartNumber)));
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
                        var inventoryDetails = await _inventoryRepository.GetInventoryDetailsByItemNumberandLocation(fromPartNumber, fromLocation, fromWarehouse, fromProjectNumber, fromLotnumber);
                        if (inventoryDetails != null && inventoryDetails.Count() > 0)
                        {
                            var availableStockinventoryDetails = inventoryDetails.Where(x => x.Balance_Quantity > 0).ToList();

                            if (availableStockinventoryDetails.Count() > 0)
                            {
                                var inventoryBalQty = availableStockinventoryDetails.Sum(x => x.Balance_Quantity);
                                if (inventoryBalQty >= transferQty)
                                {
                                    foreach (var inventoryItem in availableStockinventoryDetails)
                                    {
                                        var LocaTransId = await _locationTransferRepository.GetLatestLocationTransferId();
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
                                            inventoryTranctionPost.TO_Location = toLocation;
                                            inventoryTranctionPost.PartType = inventoryItem.PartType;
                                            inventoryTranctionPost.ReferenceID = inventoryItem.ReferenceID;
                                            inventoryTranctionPost.ReferenceIDFrom = inventoryItem.ReferenceIDFrom;
                                            inventoryTranctionPost.Remarks = "LocationTransfer Done";
                                            inventoryTranctionPost.TransactionType = InventoryType.Outward;

                                            await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost);


                                            inventoryItem.Balance_Quantity -= Convert.ToDecimal(transferQty);
                                            inventoryItem.IsStockAvailable = false;
                                            inventoryItem.ReferenceID = LocationTransReferId;
                                            inventoryItem.ReferenceIDFrom = "LocationTransfer";

                                            await _inventoryRepository.UpdateInventory(inventoryItem);


                                            Inventory inventoryPost = new Inventory();
                                            inventoryPost.PartNumber = toPartNumber;
                                            inventoryPost.Description = toDescription;
                                            inventoryPost.ProjectNumber = toProjectNumber;
                                            inventoryPost.LotNumber = toLotnumber;
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
                                            inventoryPost.Warehouse = toWarehouse;
                                            inventoryPost.Location = toLocation;
                                            inventoryPost.PartType = toPartType;
                                            inventoryPost.ReferenceID = LocationTransReferId;
                                            inventoryPost.ReferenceIDFrom = "LocationTransfer";

                                            await _inventoryRepository.CreateInventory(inventoryPost);


                                            InventoryTranction inventoryTranctionPost_1 = new InventoryTranction();
                                            inventoryTranctionPost_1.PartNumber = inventoryPost.PartNumber;
                                            inventoryTranctionPost_1.MftrPartNumber = inventoryPost.MftrPartNumber;
                                            inventoryTranctionPost_1.ProjectNumber = inventoryPost.ProjectNumber;
                                            inventoryTranctionPost_1.Description = inventoryPost.Description;
                                            inventoryTranctionPost_1.Issued_Quantity = inventoryPost.Balance_Quantity;
                                            inventoryTranctionPost_1.LotNumber = inventoryPost.LotNumber;
                                            inventoryTranctionPost_1.UOM = inventoryPost.UOM;
                                            inventoryTranctionPost_1.GrinMaterialType = inventoryPost.GrinMaterialType;
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
                                            inventoryTranctionPost_1.ReferenceIDFrom = "LocationTransfer";
                                            inventoryTranctionPost_1.Remarks = "LocationTransfer Done";
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
                                            inventoryTranctionPost.Issued_Quantity = Convert.ToDecimal(transferQty);
                                            //inventoryTranctionPost.Issued_By = inventoryItem.LastModifiedBy;
                                            inventoryTranctionPost.LotNumber = inventoryItem.LotNumber;
                                            inventoryTranctionPost.UOM = inventoryItem.UOM;
                                            inventoryTranctionPost.GrinMaterialType = "";
                                            inventoryTranctionPost.shopOrderNo = inventoryItem.shopOrderNo;
                                            inventoryTranctionPost.Unit = inventoryItem.Unit;
                                            inventoryTranctionPost.GrinNo = inventoryItem.GrinNo;
                                            inventoryTranctionPost.GrinPartId = inventoryItem.GrinPartId;
                                            inventoryTranctionPost.IsStockAvailable = true;
                                            inventoryTranctionPost.Warehouse = inventoryItem.Warehouse;
                                            inventoryTranctionPost.From_Location = inventoryItem.Location;
                                            inventoryTranctionPost.TO_Location = toLocation;
                                            inventoryTranctionPost.PartType = inventoryItem.PartType;
                                            inventoryTranctionPost.ReferenceID = inventoryItem.ReferenceID;
                                            inventoryTranctionPost.ReferenceIDFrom = inventoryItem.ReferenceIDFrom;
                                            inventoryTranctionPost.Remarks = "LocationTransfer Done";
                                            inventoryTranctionPost.TransactionType = InventoryType.Outward;
                                            await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost);


                                            Inventory inventoryPost = new Inventory();
                                            inventoryPost.PartNumber = toPartNumber;
                                            inventoryPost.LotNumber = toLotnumber;
                                            inventoryPost.MftrPartNumber = itemObject.itemmasterAlternate.Where(x => x.isDefault == true).Select(x => x.manufacturerPartNo).FirstOrDefault();
                                            inventoryPost.ProjectNumber = toProjectNumber;
                                            inventoryPost.Description = toDescription;
                                            inventoryPost.Balance_Quantity = Convert.ToDecimal(transferQty);
                                            inventoryPost.UOM = toUOM;
                                            inventoryPost.Min = inventoryItem.Min;
                                            inventoryPost.Max = inventoryItem.Max;
                                            inventoryPost.GrinMaterialType = inventoryItem.GrinMaterialType;
                                            inventoryPost.shopOrderNo = "";
                                            inventoryPost.Unit = inventoryItem.Unit;
                                            inventoryPost.GrinNo = "";
                                            inventoryPost.GrinPartId = 0;
                                            inventoryPost.LotNumber = toLotnumber;
                                            inventoryPost.IsStockAvailable = true;
                                            inventoryPost.Warehouse = toWarehouse;
                                            inventoryPost.Location = toLocation;
                                            inventoryPost.PartType = toPartType;
                                            inventoryPost.ReferenceID = LocationTransReferId; /*Convert.ToString(loca.Id);*/
                                            inventoryPost.ReferenceIDFrom = "LocationTransfer";

                                            await _inventoryRepository.CreateInventory(inventoryPost);


                                            InventoryTranction inventoryTranctionPost1 = new InventoryTranction();
                                            inventoryTranctionPost1.PartNumber = inventoryPost.PartNumber;
                                            inventoryTranctionPost1.MftrPartNumber = inventoryPost.MftrPartNumber;
                                            inventoryTranctionPost1.ProjectNumber = inventoryPost.ProjectNumber;
                                            inventoryTranctionPost1.Description = inventoryPost.Description;
                                            inventoryTranctionPost1.Issued_Quantity = inventoryPost.Balance_Quantity;
                                            inventoryTranctionPost1.LotNumber = inventoryPost.LotNumber;
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
                                            inventoryTranctionPost1.ReferenceIDFrom = "LocationTransfer";
                                            inventoryTranctionPost1.Remarks = "LocationTransfer Done";
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
                                }

                            }
                            else
                            {
                                string partNumbersList = string.Join(", ", fromPartNumber);
                                warningMessage = $"For these PartNumbers: {partNumbersList}, quantity has been fully transferred. No available stock for LocationTransfer.";
                            }

                            //_inventoryRepository.SaveAsync();
                            //_inventoryTranctionRepository.SaveAsync();
                        }
                        else
                        {
                            _logger.LogError($"Something went wrong inside CreateLocationTransfer action. Inventory hasn't been found in db");
                            serviceResponse.Data = null;
                            serviceResponse.Message = " Inventory hasn't been found";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.NotFound;
                            return NotFound(serviceResponse);
                        }

                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside CreateLocationTransfer action. GetItemMaster  action Other Service Calling  failed! ");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Internal server error";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                }

                _locationTransferRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = $"locationTransfer Successfully Created \n{warningMessage}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateLocationTransfer API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateLocationTransfer API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        //public async Task<IActionResult> CreateLocationTransfer([FromBody] LocationTransferPostDto locationTransferPostDto)
        //{
        //    ServiceResponse<LocationTransferDto> serviceResponse = new ServiceResponse<LocationTransferDto>();

        //    try
        //    {
        //        if (locationTransferPostDto is null)
        //        {
        //            _logger.LogError("locationTransfer object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "locationTransfer object sent from client is null.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid locationTransfer object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid locationTransfer object sent from client.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        var createLocationTransfer = _mapper.Map<LocationTransfer>(locationTransferPostDto);

        //        await _locationTransferRepository.CreateLocationTransfer(createLocationTransfer);
        //        _locationTransferRepository.SaveAsync();

        //        var fromPartNumber = locationTransferPostDto.FromPartNumber;
        //        var toPartNumber = locationTransferPostDto.ToPartNumber;
        //        var fromProjectNumber = locationTransferPostDto.FromProjectNumber;
        //        var toProjectNumber = locationTransferPostDto.ToProjectNumber;
        //        var fromLocation = locationTransferPostDto.FromLocation;
        //        var toLocation = locationTransferPostDto.ToLocation;
        //        var fromWarehouse = locationTransferPostDto.FromWarehouse;
        //        var toWarehouse = locationTransferPostDto.ToWarehouse;
        //        var availstock = locationTransferPostDto.AvailableStockInLocation;
        //        var transferQty = locationTransferPostDto.TransferQty;

        //        var itemDetailFromItemmaster = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
        //                            "GetItemMasterByItemNumber?", "&ItemNumber=", toPartNumber));
        //        _logger.LogInfo("getitemmasterdata" + Convert.ToString(itemDetailFromItemmaster));
        //        var itemDetail = await itemDetailFromItemmaster.Content.ReadAsStringAsync();
        //        dynamic itemData = JsonConvert.DeserializeObject(itemDetail);
        //        dynamic itemObject = itemData.data;

        //        //Add Inventory table
        //        var inventoryDetails = await _inventoryRepository.GetInventoryDetailsByItemNumberandLocation(fromPartNumber, fromLocation, fromWarehouse, fromProjectNumber);
        //        if (inventoryDetails != null)
        //        {
        //            foreach (var inventoryItem in inventoryDetails)
        //            {
        //                if (transferQty >= inventoryItem.Balance_Quantity)
        //                {
        //                    inventoryItem.PartNumber = toPartNumber;
        //                    inventoryItem.ProjectNumber = toProjectNumber;
        //                    inventoryItem.MftrPartNumber = toPartNumber;
        //                    inventoryItem.Description = itemObject.description; 
        //                    inventoryItem.UOM = itemObject.uom;
        //                    inventoryItem.Warehouse = toWarehouse;
        //                    inventoryItem.Location = toLocation;
        //                    inventoryItem.PartType = itemObject.itemType;
        //                    inventoryItem.ReferenceID = Convert.ToString(createLocationTransfer.Id);
        //                    inventoryItem.ReferenceIDFrom = "LocationTransfer";
        //                    await _inventoryRepository.UpdateInventory(inventoryItem);
        //                    _inventoryRepository.SaveAsync();
        //                    transferQty -= inventoryItem.Balance_Quantity;
        //                }
        //                else
        //                {

        //                    inventoryItem.Balance_Quantity -= transferQty;
        //                    await _inventoryRepository.UpdateInventory(inventoryItem);


        //                    Inventory inventoryPost = new Inventory();
        //                    inventoryPost.PartNumber = toPartNumber;
        //                    inventoryPost.MftrPartNumber = toPartNumber;
        //                    inventoryPost.ProjectNumber = toProjectNumber;
        //                    inventoryPost.Description = itemObject.description;
        //                    inventoryPost.Balance_Quantity = transferQty;
        //                    inventoryPost.UOM = itemObject?.uom;
        //                    inventoryPost.GrinMaterialType = "";
        //                    inventoryPost.shopOrderNo = "";
        //                    inventoryPost.Unit = itemObject?.unit;
        //                    inventoryPost.GrinNo = "";
        //                    inventoryPost.GrinPartId = 0;
        //                    inventoryPost.IsStockAvailable = true;
        //                    inventoryPost.Warehouse = toWarehouse;
        //                    inventoryPost.Location = toLocation;
        //                    inventoryPost.PartType = itemObject.itemType;
        //                    inventoryPost.ReferenceID = Convert.ToString(createLocationTransfer.Id);
        //                    inventoryPost.ReferenceIDFrom = "LocationTransfer";
        //                    await _inventoryRepository.CreateInventory(inventoryPost);
        //                    transferQty = 0;

        //                    _inventoryRepository.SaveAsync();
        //                }
        //                if (transferQty <= 0)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = " Inventory hasn't been found in db.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }

        //        //Add InventoryTranction table
        //        var inventoryTranctionDetails = await _inventoryTranctionRepository.GetInventoryTranctionDetailsByItemNumberandLocation(fromPartNumber, fromLocation, fromWarehouse, fromProjectNumber);
        //        if (inventoryTranctionDetails != null)
        //        {
        //            foreach (var inventoryTranctionItem in inventoryTranctionDetails)
        //            {
        //                if (transferQty >= inventoryTranctionItem.Issued_Quantity)
        //                {
        //                    inventoryTranctionItem.PartNumber = toPartNumber;
        //                    inventoryTranctionItem.ProjectNumber = toProjectNumber;
        //                    inventoryTranctionItem.MftrPartNumber = toPartNumber;
        //                    inventoryTranctionItem.Description = itemObject.description;
        //                    inventoryTranctionItem.UOM = itemObject.uom;
        //                    inventoryTranctionItem.Warehouse = toWarehouse;
        //                    inventoryTranctionItem.From_Location = fromLocation;
        //                    inventoryTranctionItem.TO_Location = toLocation;
        //                    inventoryTranctionItem.PartType = itemObject.itemType;
        //                    inventoryTranctionItem.ReferenceID = Convert.ToString(createLocationTransfer.Id);
        //                    inventoryTranctionItem.ReferenceIDFrom = "LocationTransfer";
        //                    await _inventoryTranctionRepository.UpdateInventoryTraction(inventoryTranctionItem);
        //                    _inventoryTranctionRepository.SaveAsync();
        //                    transferQty -= inventoryTranctionItem.Issued_Quantity;
        //                }
        //                else
        //                {

        //                    inventoryTranctionItem.Issued_Quantity -= transferQty;
        //                    await _inventoryTranctionRepository.UpdateInventoryTraction(inventoryTranctionItem);


        //                    InventoryTranction inventoryTranctionPost = new InventoryTranction();
        //                    inventoryTranctionPost.PartNumber = toPartNumber;
        //                    inventoryTranctionPost.MftrPartNumber = toPartNumber;
        //                    inventoryTranctionPost.ProjectNumber = toProjectNumber;
        //                    inventoryTranctionPost.Description = itemObject.description;
        //                    inventoryTranctionPost.Issued_Quantity = transferQty;
        //                    inventoryTranctionPost.UOM = itemObject?.uom;
        //                    inventoryTranctionPost.GrinMaterialType = "";
        //                    inventoryTranctionPost.shopOrderNo = "";
        //                    inventoryTranctionPost.Unit = itemObject?.unit;
        //                    inventoryTranctionPost.GrinNo = "";
        //                    inventoryTranctionPost.GrinPartId = 0;
        //                    inventoryTranctionPost.IsStockAvailable = true;
        //                    inventoryTranctionPost.Warehouse = toWarehouse;
        //                    inventoryTranctionPost.From_Location = fromLocation;
        //                    inventoryTranctionPost.TO_Location = toLocation;
        //                    inventoryTranctionPost.PartType = itemObject.itemType;
        //                    inventoryTranctionPost.ReferenceID = Convert.ToString(createLocationTransfer.Id);
        //                    inventoryTranctionPost.ReferenceIDFrom = "LocationTransfer";
        //                    await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranctionPost);
        //                    transferQty = 0;

        //                    _inventoryTranctionRepository.SaveAsync();
        //                }
        //                if (transferQty <= 0)
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = " InventoryTranction hasn't been found in db.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "locationTransfer Successfully Created";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Created("GetLocationTransferId", serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside CreateLocationTransfer action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        [HttpPut]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryDtoUpdate inventoryDtoUpdate)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                if (inventoryDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Inventory object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Inventory object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Inventory object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Inventory object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var getInventoryById = await _inventoryRepository.GetInventoryById(id);
                if (getInventoryById is null)
                {
                    _logger.LogError($"Inventory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Inventory with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateInventory = _mapper.Map(inventoryDtoUpdate, getInventoryById);

                string result = await _inventoryRepository.UpdateInventory(updateInventory);
                _logger.LogInfo(result);
                _inventoryRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateInventory API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in UpdateInventory API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocationTransfer(int id, [FromBody] LocationTransferUpdateDto locationTransferUpdateDto)
        {
            ServiceResponse<LocationTransferDto> serviceResponse = new ServiceResponse<LocationTransferDto>();

            try
            {
                if (locationTransferUpdateDto is null)
                {
                    _logger.LogError("locationTransfer object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update locationTransfer object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid locationTransfer object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update locationTransfer object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getLocationTransferById = await _locationTransferRepository.GetLocationTransferById(id);
                if (getLocationTransferById is null)
                {
                    _logger.LogError($"locationTransfer with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update locationTransfer with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var updateData = _mapper.Map(locationTransferUpdateDto, getLocationTransferById);

                string result = await _locationTransferRepository.UpdateLocationTransfer(updateData);
                _logger.LogError(result);
                _locationTransferRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "LocationTransfers Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateLocationTransfer API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateLocationTransfer API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> LocationTransferSPReportWithParam([FromBody] LocationTransferSPReportWithParamDTO locationTransferSPReport)
        {
            ServiceResponse<IEnumerable<LocationTransferSPReport>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferSPReport>>();
            try
            {
                var products = await _locationTransferRepository.LocationTransferSPReportWithParam(locationTransferSPReport.FromPartNumber, locationTransferSPReport.FromPartType,
                                                                     locationTransferSPReport.FromWarehouse, locationTransferSPReport.FromLocation,
                                                                     locationTransferSPReport.FromProjectNumber, locationTransferSPReport.ToPartNumber,
                                                                     locationTransferSPReport.ToPartType, locationTransferSPReport.ToWarehouse,
                                                                     locationTransferSPReport.ToLocation, locationTransferSPReport.ToProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"LocationTransfer hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"LocationTransfer hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    // var result = _mapper.Map<IEnumerable<LocationTransferSPReportDTO>>(products);

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned LocationTransfer Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in LocationTransferSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in LocationTransferSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> LocationTransferSPReportWithParamForTras([FromBody] LocationTransferSPReportWithParamDTO locationTransferSPReport)
        {
            ServiceResponse<IEnumerable<LocationTransferSpReportForTras>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferSpReportForTras>>();
            try
            {
                var products = await _locationTransferRepository.LocationTransferSPReportWithParamForTras(locationTransferSPReport.FromPartNumber, locationTransferSPReport.FromPartType,
                                                                     locationTransferSPReport.FromWarehouse, locationTransferSPReport.FromLocation,
                                                                     locationTransferSPReport.FromProjectNumber, locationTransferSPReport.ToPartNumber,
                                                                     locationTransferSPReport.ToPartType, locationTransferSPReport.ToWarehouse,
                                                                     locationTransferSPReport.ToLocation, locationTransferSPReport.ToProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"LocationTransfer hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"LocationTransfer hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    // var result = _mapper.Map<IEnumerable<LocationTransferSPReportDTO>>(products);

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned LocationTransfer Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in LocationTransferSPReportWithParamForTras API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in LocationTransferSPReportWithParamForTras API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> LocationTransferSPReportWithParamForAvi([FromBody] LocationTransferSPReportWithParamDTO locationTransferSPReport)
        {
            ServiceResponse<IEnumerable<LocationTransferSpReportForAvi>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferSpReportForAvi>>();
            try
            {
                var products = await _locationTransferRepository.LocationTransferSPReportWithParamForAvi(locationTransferSPReport.FromPartNumber, locationTransferSPReport.FromPartType,
                                                                     locationTransferSPReport.FromWarehouse, locationTransferSPReport.FromLocation,
                                                                     locationTransferSPReport.FromProjectNumber, locationTransferSPReport.ToPartNumber,
                                                                     locationTransferSPReport.ToPartType, locationTransferSPReport.ToWarehouse,
                                                                     locationTransferSPReport.ToLocation, locationTransferSPReport.ToProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"LocationTransfer hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"LocationTransfer hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    // var result = _mapper.Map<IEnumerable<LocationTransferSPReportDTO>>(products);

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned LocationTransfer Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in LocationTransferSPReportWithParamForAvi API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in LocationTransferSPReportWithParamForAvi API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LocationTransferSPReportDates([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<LocationTransferSPReport>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferSPReport>>();
            try
            {
                var products = await _locationTransferRepository.LocationTransferSPReportDates(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"LocationTransfer hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"LocationTransfer hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned LocationTransfer Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in LocationTransferSPReportDates API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in LocationTransferSPReportDates API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LocationTransferSPReportDatesForTras([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<LocationTransferSpReportForTras>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferSpReportForTras>>();
            try
            {
                var products = await _locationTransferRepository.LocationTransferSPReportDatesForTras(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"LocationTransfer hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"LocationTransfer hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned LocationTransfer Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in LocationTransferSPReportDatesForTras API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in LocationTransferSPReportDatesForTras API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LocationTransferSPReportDatesForAvi([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<LocationTransferSpReportForAvi>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferSpReportForAvi>>();
            try
            {
                var products = await _locationTransferRepository.LocationTransferSPReportDatesForAvi(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"LocationTransfer hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"LocationTransfer hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned LocationTransfer Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in LocationTransferSPReportDatesForAvi API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in LocationTransferSPReportDatesForAvi API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocationTransfer(int id)
        {
            ServiceResponse<LocationTransferDto> serviceResponse = new ServiceResponse<LocationTransferDto>();

            try
            {
                var getLocationTransfer = await _locationTransferRepository.GetLocationTransferById(id);
                if (getLocationTransfer == null)
                {
                    _logger.LogError($"Delete LocationTransfer with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete LocationTransfer with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _locationTransferRepository.DeleteLocationTransfer(getLocationTransfer);
                _logger.LogError(result);
                _locationTransferRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "LocationTransfer Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteLocationTransfer API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteLocationTransfer API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectLocWareFromInventoryByItemNo(string itemNumber)
        {
            ServiceResponse<List<LocationTransferFromDto>> serviceResponse = new ServiceResponse<List<LocationTransferFromDto>>();
            try
            {
                var InventoryDetails = await _locationTransferRepository.GetProjectLocWareFromInventoryByItemNo(itemNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Location,Project,Warehouse Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber}, is invalid");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory Location,Project,Warehouse with Itemnumber: {itemNumber}");
                    var result = _mapper.Map<List<LocationTransferFromDto>>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryDetails with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetProjectLocWareFromInventoryByItemNo API for the following itemNumber:{itemNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetProjectLocWareFromInventoryByItemNo API for the following itemNumber:{itemNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllLocationTransferIdNameList()
        {
            ServiceResponse<IEnumerable<LocationTransferIdNameList>> serviceResponse = new ServiceResponse<IEnumerable<LocationTransferIdNameList>>();
            try
            {
                var listOfAllLocationTransferIdNames = await _locationTransferRepository.GetAllLocationTransferIdNameList();
                var result = _mapper.Map<IEnumerable<LocationTransferIdNameList>>(listOfAllLocationTransferIdNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All listOfAllLocationTransferIdNames";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLocationTransferIdNameList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLocationTransferIdNameList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost]
        public async Task<IActionResult> MRNSPReportWithParam([FromBody] MRNSPReportGetDto mRNSPReportGetDto)
        {
            ServiceResponse<IEnumerable<MRNSPReport>> serviceResponse = new ServiceResponse<IEnumerable<MRNSPReport>>();
            try
            {
                var products = await _locationTransferRepository.MRNSPReportWithParam(mRNSPReportGetDto.ProjectNumber, mRNSPReportGetDto.ShopOrderType, mRNSPReportGetDto.ShopOrderNumber, mRNSPReportGetDto.KPN, mRNSPReportGetDto.PartType);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MRN hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MRN hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MRN Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in MRNSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in MRNSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> MRNSPReportWithParamForTrans([FromBody] MRNSPReportGetDtoForTrans mRNSPReportGetDto)
        {
            ServiceResponse<IEnumerable<MRNSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<MRNSPReportForTrans>>();
            try
            {
                var products = await _locationTransferRepository.MRNSPReportWithParamForTrans(mRNSPReportGetDto.ProjectNumber, mRNSPReportGetDto.ShopOrderType, mRNSPReportGetDto.ShopOrderNumber,
                                                                                                mRNSPReportGetDto.PartNumber, mRNSPReportGetDto.PartType);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MRN hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MRN hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MRN Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in MRNSPReportWithParamForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in MRNSPReportWithParamForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> MRNSPReportDates([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<MRNSPReport>> serviceResponse = new ServiceResponse<IEnumerable<MRNSPReport>>();
            try
            {
                var products = await _locationTransferRepository.MRNSPReportDates(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MRN hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MRN hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MRN Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in MRNSPReportDates API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in MRNSPReportDates API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> MRNSPReportDatesForTrans([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<MRNSPReportForTrans>> serviceResponse = new ServiceResponse<IEnumerable<MRNSPReportForTrans>>();
            try
            {
                var products = await _locationTransferRepository.MRNSPReportDatesForTrans(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"MRN hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"MRN hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned MRN Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in MRNSPReportDatesForTrans API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in MRNSPReportDatesForTrans API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}