using AutoMapper;
using Azure;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Tips.Production.Api.Contracts;
using Tips.Production.Api.Entities;
using Tips.Production.Api.Entities.DTOs;
using Tips.Production.Api.Repository;

namespace Tips.Production.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OQCBinningController : ControllerBase
    {
        private ILoggerManager _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private IMapper _mapper;
        private IOQCBinningRepository _oQCBinningRepository;
        public OQCBinningController(IOQCBinningRepository oQCBinningRepository, ILoggerManager logger, HttpClient httpClient, IConfiguration config, IMapper mapper)
        {
            _logger = logger;
            _httpClient = httpClient;
            _config = config;
            _mapper = mapper;
            _oQCBinningRepository = oQCBinningRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOQCBinning([FromBody] OQCBinningPostDto oQCBinningPostDto)
        {
            ServiceResponse<OQCBinningDto> serviceResponse = new ServiceResponse<OQCBinningDto>();
            try
            {
                if (oQCBinningPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "OQCBinning object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("OQCBinning object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid OQCBinning object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid OQCBinning object sent from client.");
                    return BadRequest(serviceResponse);
                }
                HttpStatusCode GetInv = HttpStatusCode.OK;
                HttpStatusCode GetItemMas = HttpStatusCode.OK;
                HttpStatusCode UpdateInv = HttpStatusCode.OK;
                HttpStatusCode CreateInv = HttpStatusCode.OK;
                HttpStatusCode CreateInvTrans = HttpStatusCode.OK;
                var postoqcbinning = _mapper.Map<OQCBinning>(oQCBinningPostDto);
                List<OQCBinningLocation> oqcBinningLocationList = new List<OQCBinningLocation>();
                foreach (var loc in postoqcbinning.oQCBinningLocations)
                {
                    int flag = 0;
                    foreach (var ex in oqcBinningLocationList)
                    {
                        if (loc.Warehouse == ex.Warehouse && loc.Location == ex.Location)
                        {
                            ex.Quantity = ex.Quantity + loc.Quantity;
                            flag = 1;
                        }
                    }
                    if (flag == 0)
                    {
                        oqcBinningLocationList.Add(loc);
                    }
                }
                postoqcbinning.oQCBinningLocations = oqcBinningLocationList;
                int cretedoqcbin = await _oQCBinningRepository.CreateOQCBinning(postoqcbinning);
                _oQCBinningRepository.SaveAsync();

                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                var httpClient = new HttpClient(httpClientHandler);
                var rfqApiUrl = _config["InventoryAPI"];
                var rfqCustomerIdResponse = await _httpClient.GetAsync($"{rfqApiUrl}GetInventoryStockByItemAndShopOrderNo?itemNumber={oQCBinningPostDto.ItemNumber}&shopordernumber={oQCBinningPostDto.ShopOrderNumber}");
                if (rfqCustomerIdResponse.StatusCode != HttpStatusCode.OK)
                {
                    GetInv = rfqCustomerIdResponse.StatusCode;
                }
                var rfqCustomerIdString = await rfqCustomerIdResponse.Content.ReadAsStringAsync();
                var vendorUOC = JsonConvert.DeserializeObject<OQCBinningInventoryDto>(rfqCustomerIdString);
                var oqcbinninglocations = _mapper.Map<List<OQCBinningLocation>>(postoqcbinning.oQCBinningLocations);
                decimal binningqty = 0;
                foreach (var loc in oqcbinninglocations)
                {
                    binningqty = loc.Quantity + binningqty;
                }
                vendorUOC.data.Balance_Quantity = vendorUOC.data.Balance_Quantity - binningqty;
                var updateInventory = _mapper.Map<OQCBinningInventoryUpdateDto>(vendorUOC.data);
                var httpClientHandler_1 = new HttpClientHandler();
                httpClientHandler_1.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                var httpClient_1 = new HttpClient(httpClientHandler_1);
                string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(updateInventory);
                var rfqApiUrl_1 = _config["InventoryAPI"];
                var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                await _httpClient.PutAsync($"{rfqApiUrl_1}UpdateInventory?Id={vendorUOC.data.Id}", content);
                foreach (var loc in oqcBinningLocationList)
                {
                    var newinv = _mapper.Map<OQCBinningInventoryUpdateDto>(updateInventory);
                    newinv.GrinPartId = null;
                    newinv.Warehouse = loc.Warehouse;
                    newinv.Location = loc.Location;
                    newinv.Balance_Quantity = loc.Quantity;
                    var json = JsonConvert.SerializeObject(newinv);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        CreateInv = response.StatusCode;
                    }

                    var ItemNumber = postoqcbinning.ItemNumber;
                    var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
                            "GetItemMasterByItemNumber?", "&ItemNumber=", ItemNumber));
                    if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK)
                    {
                        GetItemMas = itemMasterObjectResult.StatusCode;
                    }
                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(itemMasterObjectResult));
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMatserObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterTranctionObject = itemMatserObjectData.data;

                    //Adding SA in Inventory Table
                    var ItemNo = itemMasterTranctionObject.itemNumber;
                    var Desc = itemMasterTranctionObject.description;
                    var uom = itemMasterTranctionObject.uom;
                    var ProjectNo = itemMasterTranctionObject.projectNumber;
                    var ItemType = itemMasterTranctionObject.itemType;

                    InventoryTranctionDto inventoryTranction = new InventoryTranctionDto();
                    inventoryTranction.PartNumber = ItemNo;
                    inventoryTranction.MftrPartNumber = ItemNo;
                    inventoryTranction.Description = Desc;
                    inventoryTranction.ProjectNumber = ProjectNo;
                    inventoryTranction.PartType = ItemType;
                    inventoryTranction.Issued_Quantity = loc.Quantity;
                    inventoryTranction.UOM = uom;
                    inventoryTranction.BOM_Version_No = 0;
                    inventoryTranction.Issued_DateTime = DateTime.Now;
                    inventoryTranction.shopOrderNo = "";
                    inventoryTranction.ReferenceID = postoqcbinning.Id.ToString();
                    inventoryTranction.ReferenceIDFrom = "OQCBinning"; ;
                    inventoryTranction.From_Location = loc.Location;
                    inventoryTranction.TO_Location = loc.Location;
                    inventoryTranction.Warehouse = loc.Warehouse; ;

                    var json2 = JsonConvert.SerializeObject(inventoryTranction);
                    var data2 = new StringContent(json2, Encoding.UTF8, "application/json");
                    var response2 = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), data2);
                    if (response2.StatusCode != HttpStatusCode.OK)
                    {
                        CreateInvTrans = response2.StatusCode;
                    }
                    if (GetInv == HttpStatusCode.OK && CreateInv == HttpStatusCode.OK && CreateInvTrans == HttpStatusCode.OK)
                    {
                        _oQCBinningRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside CreateOQCBinning action. Inventory update action Calling Other Service failed! ");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Internal server error";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                }
                _logger.LogInfo("aftergettingdata");
                serviceResponse.Data = null;
                serviceResponse.Message = "OQCBinning Created Successfully";
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
                _logger.LogError($"Something went wrong inside CreateOQCBinning action: {ex.Message} {ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOQCBinningById(int id)
        {
            ServiceResponse<OQCBinningDto> serviceResponse = new ServiceResponse<OQCBinningDto>();
            try
            {
                var oQCDetailsbyId = await _oQCBinningRepository.GetOQCBinningById(id);
                if (oQCDetailsbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQCBinning  hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"OQCBinning with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned OQCBinning with id: {id}");
                    var result = _mapper.Map<OQCBinningDto>(oQCDetailsbyId);
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
        public async Task<IActionResult> GetAllOQCBinning([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParamess searchParamess)
        {
            ServiceResponse<IEnumerable<OQCBinningDto>> serviceResponse = new ServiceResponse<IEnumerable<OQCBinningDto>>();
            try
            {
                var oQCDetails = await _oQCBinningRepository.GetAllOQCBinning(pagingParameter, searchParamess);
                var metadata = new
                {
                    oQCDetails.TotalCount,
                    oQCDetails.PageSize,
                    oQCDetails.CurrentPage,
                    oQCDetails.HasNext,
                    oQCDetails.HasPreviuos
                };
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all OQCBinning");
                var result = _mapper.Map<IEnumerable<OQCBinningDto>>(oQCDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all OQCBinnings Successfully";
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
        [HttpPost]
        public async Task<IActionResult> GetOQCBinningSPReportWithParam([FromBody] OQCBinningSPReportDto oqcBinningSPReportDto)
        {
            ServiceResponse<IEnumerable<OQCBinningSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OQCBinningSPReport>>();
            try
            {
                var products = await _oQCBinningRepository.GetOQCBinningSPReportWithParam(oqcBinningSPReportDto.ItemNumber, oqcBinningSPReportDto.ShopOrderNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQCBinning hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OQCBinning hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OQCBinning Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetOQCBinningSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetOQCBinningSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<OQCBinningSPReport>> serviceResponse = new ServiceResponse<IEnumerable<OQCBinningSPReport>>();
            try
            {
                var products = await _oQCBinningRepository.GetOQCBinningSPReportWithDate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"OQCBinning WithDate hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"OQCBinning WithDate hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned OQCBinningWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetOQCBinningSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
