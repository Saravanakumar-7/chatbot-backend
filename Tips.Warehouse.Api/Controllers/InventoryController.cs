using System.Data;
using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
 using System.IO;

using System.Configuration;

using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
 
namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private IInventoryRepository _inventoryRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private IMaterialIssueTrackerRepository _materialIssueTrackerRepository ;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public InventoryController(IConfiguration config, IInventoryTranctionRepository inventoryTranctionRepository, HttpClient httpClient, IInventoryRepository inventoryRepository,
            ILoggerManager logger, IMapper mapper, IMaterialIssueTrackerRepository materialIssueTrackerRepository)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _config = config;
            _materialIssueTrackerRepository = materialIssueTrackerRepository;
        }


        // GET: api/<InventoryController>
        [HttpGet]
        public async Task<IActionResult> GetAllInventory([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var getAllInventory = await _inventoryRepository.GetAllInventory(pagingParameter, searchParams);
                var metadata = new
                {
                    getAllInventory.TotalCount,
                    getAllInventory.PageSize,
                    getAllInventory.CurrentPage,
                    getAllInventory.HasNext,
                    getAllInventory.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all Inventory");
                var result = _mapper.Map<IEnumerable<InventoryDto>>(getAllInventory);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Inventory";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItemNoByInventoryStock()
        {
            ServiceResponse<IEnumerable<InventoryItemNoStock>> serviceResponse = new ServiceResponse<IEnumerable<InventoryItemNoStock>>();
            try
            {
                var itemNoInventoryStock = await _inventoryRepository.GetItemNoByInventoryStock();
               
                _logger.LogInfo("Returned all Inventory");
                var result = _mapper.Map<IEnumerable<InventoryItemNoStock>>(itemNoInventoryStock);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned ItemNoByInventoryStock";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //passing project 

        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId, string ItemNumber, string ProjectNumber)

        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryDetailsByGrinNoandGrinId = await _inventoryRepository.GetInventoryDetailsByGrinNoandGrinId(GrinNo, GrinPartsId, ItemNumber, ProjectNumber);
                if (getInventoryDetailsByGrinNoandGrinId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with id: {GrinNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with id: {GrinNo}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with id: {GrinNo}");
                    var result = _mapper.Map<InventoryDto>(getInventoryDetailsByGrinNoandGrinId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //passing itemnumber and location

        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsByItemnumberandLocation(string ItemNumber, string Location, string Warehouse,string projectNumber)

        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryDetailsByItemNoandLoc = await _inventoryRepository.GetInventoryDetailsByItemNumberandLocation(ItemNumber, Location, Warehouse, projectNumber);
                if (getInventoryDetailsByItemNoandLoc == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with id: {ItemNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with id: {ItemNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with id: {ItemNumber}");
                    var result = _mapper.Map<InventoryDto>(getInventoryDetailsByItemNoandLoc);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }





        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsByGrinNo(string GrinNo, string ItemNumber, string ProjectNumber)

        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryDetailsByGrinNo = await _inventoryRepository.GetInventoryDetailsByGrinNo(GrinNo, ItemNumber, ProjectNumber);
                if (getInventoryDetailsByGrinNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with id: {GrinNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with id: {GrinNo}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with id: {GrinNo}");
                    var result = _mapper.Map<InventoryDto>(getInventoryDetailsByGrinNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //passing itemnumber and projectnumber to get inventory details


        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsByItemAndProjectNo(string itemNumber, string projectNumber)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetInventoryDetailsByItemAndProjectNo(itemNumber, projectNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with itemNumber and ProjectNumber: {itemNumber} {projectNumber}, is invalid";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber and ProjectNumber: {itemNumber} {projectNumber}, is invalid");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber and ProjectNumber: {itemNumber} {projectNumber}");
                    var result = _mapper.Map<InventoryDto>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Invalid inventory{ex.Message},{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryStockByItemAndProjectNo(string itemNumber, string projectNumber)
        {
            ServiceResponse<List<InventoryBalanceQtyMaterialIssue>> serviceResponse = new ServiceResponse<List<InventoryBalanceQtyMaterialIssue>>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetInventoryStockByItemAndProjectNo(itemNumber, projectNumber);
                if (InventoryDetails.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with itemNumber and ProjectNumber: {itemNumber} {projectNumber}, is invalid";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber and ProjectNumber: {itemNumber} {projectNumber}, is invalid");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber and ProjectNumber: {itemNumber} {projectNumber}");
                    var result = _mapper.Map<List<InventoryBalanceQtyMaterialIssue>>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Invalid inventory{ex.Message},{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]

        public async Task<IActionResult> GetAvailableStockQtyForSalesOrderItems([FromQuery] string salesOrderNo, [FromQuery] int salesOrderStatus ,[FromBody]List<string> itemNumberList)
        {
            ServiceResponse<Dictionary<string, decimal>> serviceResponse = new ServiceResponse<Dictionary<string, decimal>>();
            try
            {
                Dictionary<string, decimal> ItemNoWithQtyDict = new Dictionary<string, decimal>();

                if (itemNumberList == null)
                {
                    _logger.LogError($"ItemNumber list for for SO NO {salesOrderNo} is null in Inventory Controller - GetAvailableStockQtyForSalesOrderItems");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ItemNumber list for SO NO {salesOrderNo} is null in Inventory Controller - GetAvailableStockQtyForSalesOrderItems";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return Ok(serviceResponse);
                }

                ItemNoWithQtyDict =  await GetItemNumberWiseQuantityDict(salesOrderNo, salesOrderStatus, itemNumberList);

                if (ItemNoWithQtyDict == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"For given item number there is no inventory available");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"For given itemnumber there is no inventory available");
                    //var result = InventoryDetails;
                    //serviceResponse.Data = result;
                    //serviceResponse.Message = "Returned InventoryDetails with id Successfully";
                    //serviceResponse.Success = true;
                    //serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(ItemNoWithQtyDict);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task<Dictionary<string, decimal>> GetItemNumberWiseQuantityDict(string salesOrderNo, int salesOrderStatus, List<string> itemNumberList)
        {
            Dictionary<string, decimal> ItemNoWithQtyDict = new Dictionary<string, decimal>();
            foreach (var itemNo in itemNumberList)
            {
                // Get Work Order (Shop Order) number list by passing Sales OrderNumber
                var shopOrderNumberListResponse = await _httpClient.GetAsync(string.Concat(_config["ProductionAPI"],
                         "GetShopOrderNoListBySalesOrderNo?", "SalesOrderNO=", salesOrderNo, "&itemNumber=", itemNo));

                var shopOrderNumberListString = await shopOrderNumberListResponse.Content.ReadAsStringAsync();
                dynamic shopOrderNumberListData = JsonConvert.DeserializeObject(shopOrderNumberListString);

                List<string> shopOrderNumberList = new List<string>();
                foreach (var item in shopOrderNumberListData)
                {
                    shopOrderNumberList.Add(item.ToString());
                }

                //List<string> shopOrderNumberList = (List<string>)shopOrderNumberListData.data;
                if (salesOrderStatus == 0) // Retail SO
                {
                    decimal itemQty = await _inventoryRepository.GetStockQtyForRetailSalesOrderItem(itemNo);
                    ItemNoWithQtyDict.Add(itemNo, itemQty);
                }
                else if (salesOrderStatus == 1) // Built to Print SO
                {
                    decimal itemQty = await _inventoryRepository.GetStockQtyForBtpSalesOrderItem(itemNo, shopOrderNumberList);
                    ItemNoWithQtyDict.Add(itemNo, itemQty);
                }
            }
            return ItemNoWithQtyDict;
        }

        [HttpGet]
        public async Task<IActionResult> GetStockDetailsForAllLocationWarehouseByItemNo(string itemNumber)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                decimal InventoryDetails = await _inventoryRepository.GetStockDetailsForAllLocationWarehouseByItemNo(itemNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber}, is invalid");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber: {itemNumber}");
                    //var result = InventoryDetails;
                    //serviceResponse.Data = result;
                    //serviceResponse.Message = "Returned InventoryDetails with id Successfully";
                    //serviceResponse.Success = true;
                    //serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(InventoryDetails);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsumptionReport>>> CoverageReport()
        {
            ServiceResponse<IEnumerable<ConsumptionReport>> serviceResponse = new ServiceResponse<IEnumerable<ConsumptionReport>>();

            try
            {
                var consumptionDetails = await _inventoryRepository.ConsumptionReports();
                if (consumptionDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Consumption Report is getting Zero Record";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return Ok(serviceResponse);
                }
                else
                {
                    var result = _mapper.Map<IEnumerable<ConsumptionReport>>(consumptionDetails);

                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Coverage Details with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Coverage Report getting Null: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //[HttpGet]
        //public async Task<IEnumerable<ConsumptionReport>> ExecuteStoredProcedure(string? itemNumber, string? salesOrderNumber)
        //{
        //    try
        //    {
        //        var result = await _inventoryRepository.ExecuteStoredProcedure(itemNumber, salesOrderNumber);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}


        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsByItemNo(string itemNumber, string projectNo)
        {
            ServiceResponse<List<InventoryDto>> serviceResponse = new ServiceResponse<List<InventoryDto>>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetInventoryDetailsByItemNoandProjectNo(itemNumber, projectNo);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber}, is invalid");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber: {itemNumber}");
                    var result = _mapper.Map<List<InventoryDto>>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryDetails with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }        

        [HttpGet]
        public async Task<IActionResult> GetInventoryBySAItemNo(string itemNumber)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetInventoryBySAItemNo(itemNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber}, is invalid");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber: {itemNumber}");
                    var result = _mapper.Map<InventoryDto>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryDetails with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetInventoryByItemNo(string itemNumber)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetInventoryByItemNo(itemNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber}, is invalid");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber: {itemNumber}");
                    var result = _mapper.Map<InventoryDto>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryDetails with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //consumption report by itemnumber
        [HttpGet]
        public async Task<IActionResult> GetConsumptionInventoryByItemNo(string itemNumber)
        {
            ServiceResponse<ConsumptionInventoryDto> serviceResponse = new ServiceResponse<ConsumptionInventoryDto>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetConsumptionInventoryByItemNo(itemNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber}, is invalid");
                    return NotFound(serviceResponse);

                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber: {itemNumber}");
                    var result = _mapper.Map<ConsumptionInventoryDto>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryDetails with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetConsumptionChildItemStockWithWipQty(List<string> itemNumberList)
        {
            ServiceResponse<ConsumptionInventoryDto> serviceResponse = new ServiceResponse<ConsumptionInventoryDto>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetConsumptionChildItemStockWithWipQty(itemNumberList);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"In GetConsumptionChildItemStockWithWipQty ItemNumber List is Empty");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber List in GetConsumptionChildItemStockWithWipQty");
                    var result = _mapper.Map<ConsumptionInventoryDto>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryDetails with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"In GetConsumptionChildItemStockWithWipQty error: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //update inventory on shoporder confirmation 
        [HttpPost]
        public async Task<IActionResult> UpdateInventoryOnShopOrderConfirmation(List<InventoryDtoForShopOrderConfirmation> dtoForShopOrderConfirmation)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                foreach (var item in dtoForShopOrderConfirmation)
                {

                    var inventoryDetails = await _inventoryRepository
                        .GetWIPInventoryDetailsByItemNo(item.PartNumber, item.ShopOrderNumber);

                    if (inventoryDetails == null || inventoryDetails.Count == 0)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Inventory Details hasn't been found";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        _logger.LogError($"Inventory with itemNumber: {item.PartNumber}, is not available");
                        return StatusCode(500, serviceResponse);
                    }

                    decimal producedQty = item.NewConvertedToFgQty; // value get from payload

                    //decimal revisionNo = await _releaseProductBomRepository.GetLatestProductionBomByItemNumber(fgPartNumber);

                    //var bom = await _repository.EnggBomRepository.GetLatestEnggBomVersionDetailByItemNumber(fgPartNumber, revisionNo);

                    //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"], "GetLatestEnggBomVersionDetailByItemNumber?", "&fgPartNumber=", itemNumber));

                    //var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    //dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    //dynamic itemMasterObject = itemMasterObjectData.data;

                    //decimal producedQty = item.WipConfirmedQty * ;
                    for (int i = 0; i < inventoryDetails.Count; i++)
                    {
                        decimal balanceqty = inventoryDetails[i].Balance_Quantity;
                        decimal lotNoWiseProducedQty = 0;
                        if (inventoryDetails[i].Balance_Quantity <= producedQty)
                        {
                            inventoryDetails[i].Balance_Quantity = 0;
                            inventoryDetails[i].IsStockAvailable = false;
                            lotNoWiseProducedQty = balanceqty;
                             producedQty -= balanceqty;
                            balanceqty = 0;
                        }
                        else
                        {
                            inventoryDetails[i].Balance_Quantity -= producedQty;
                            lotNoWiseProducedQty = producedQty;
                             
                            producedQty = 0;
                        }

                        string result = await _inventoryRepository.UpdateInventory(inventoryDetails[i]);

                        /*********************************** Update data to Material Issue Tracker *************************/
                        await UpdateDataToMaterialIssueTracker(item, inventoryDetails[i]);
                        /*********************************** End of Add data to Material Issue Tracker *************************/

                        if (producedQty <= 0)
                        {
                            break;
                        }
                    }
                }

                    _inventoryRepository.SaveAsync();
                    _materialIssueTrackerRepository.SaveAsync();
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Updated Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                

            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private async Task UpdateDataToMaterialIssueTracker(InventoryDtoForShopOrderConfirmation item, Inventory inventoryDetail)
        {
            // Retrieve the existing entry from the repository based on the ShopOrderNumber, PartNumber, and LotNumber

            List<ShopOrderMaterialIssueTracker> materialIssueTrackerList = await _materialIssueTrackerRepository
                                .GetDetailsByShopOrderNOItemNoLotNo(inventoryDetail.PartNumber, inventoryDetail.shopOrderNo,  inventoryDetail.LotNumber);

            if (materialIssueTrackerList != null || materialIssueTrackerList.Count > 0)
            {
                decimal newConvertedToFgQty = item.NewConvertedToFgQty;

                foreach (var materialIssueTrack in materialIssueTrackerList)
                {
                    decimal balanceqtyToConvert = (materialIssueTrack.IssuedQty - materialIssueTrack.ConvertedToFgQty);
                    if (balanceqtyToConvert <= newConvertedToFgQty)
                    {
                        materialIssueTrack.ConvertedToFgQty += balanceqtyToConvert;
                        newConvertedToFgQty -= balanceqtyToConvert;
                        balanceqtyToConvert = 0;
                    }
                    else
                    {
                        materialIssueTrack.ConvertedToFgQty += newConvertedToFgQty;
                        balanceqtyToConvert -= newConvertedToFgQty;
                        newConvertedToFgQty = 0;
                    }

                    await _materialIssueTrackerRepository.UpdateMaterialIssueTracker(materialIssueTrack);
                    if (newConvertedToFgQty <= 0)
                    {
                        break;
                    }
                }
 
            }
        }


            //update Material issue tracker consumed Qty

        [HttpPost]
        public async Task<IActionResult> UpdateInventoryOnMaterialIssue(InventoryDtoForMaterialIssue dtoForMaterialIssue)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                var inventoryDetails = await _inventoryRepository
                        .GetInventoryDetailsByItemNoandProjectNo(dtoForMaterialIssue.PartNumber, dtoForMaterialIssue.ProjectNumber);

                if (inventoryDetails == null || inventoryDetails.Count==0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {dtoForMaterialIssue.PartNumber}, is invalid");
                    return Ok(serviceResponse);
                }

                decimal issuedQty = dtoForMaterialIssue.IssueQty;
                string shopOrderNumber = dtoForMaterialIssue.ShopOrderNumber;
                for (int i = 0; i < inventoryDetails.Count; i++)
                {
                    decimal balanceqty = inventoryDetails[i].Balance_Quantity;
                    decimal lotNoWiseIssuedQty = 0;
                    if (inventoryDetails[i].Balance_Quantity <= issuedQty)
                    {

                        inventoryDetails[i].Warehouse = "WIP";
                        inventoryDetails[i].Location = "WIP";
                        inventoryDetails[i].shopOrderNo = shopOrderNumber;
                        inventoryDetails[i].IsStockAvailable = true;
                        lotNoWiseIssuedQty = balanceqty;
                        /** Dont Change the Position of IssuedQty and BalanceQty Code in this Method .it should be always last ***********************/
                        issuedQty -= balanceqty;
                        balanceqty = 0;
                         

                        InventoryTranction inventoryTransaction = new InventoryTranction();
                        inventoryTransaction.PartNumber = inventoryDetails[i].PartNumber;
                        inventoryTransaction.MftrPartNumber = inventoryDetails[i].MftrPartNumber;
                        inventoryTransaction.LotNumber = inventoryDetails[i].LotNumber;
                        inventoryTransaction.Description = inventoryDetails[i].Description;
                        inventoryTransaction.PartType = inventoryDetails[i].PartType;
                        inventoryTransaction.ProjectNumber = inventoryDetails[i].ProjectNumber;
                        inventoryTransaction.Issued_Quantity = issuedQty;
                        inventoryTransaction.UOM = inventoryDetails[i].UOM;
                        inventoryTransaction.Issued_DateTime = DateTime.Now;
                        inventoryTransaction.ReferenceID = inventoryDetails[i].ReferenceID;
                        inventoryTransaction.ReferenceIDFrom = inventoryDetails[i].ReferenceIDFrom;
                        inventoryTransaction.BOM_Version_No = 0;
                        inventoryTransaction.From_Location = inventoryDetails[i].Location;
                        inventoryTransaction.TO_Location = "WIP";
                        inventoryTransaction.Unit = inventoryDetails[i].Unit;
                        inventoryTransaction.GrinMaterialType = inventoryDetails[i].GrinMaterialType;
                        inventoryTransaction.Remarks = "Issue Material";
                        inventoryTransaction.IsStockAvailable = inventoryDetails[i].IsStockAvailable;
                        inventoryTransaction.Warehouse = inventoryDetails[i].Warehouse;
                        inventoryTransaction.GrinNo = inventoryDetails[i].GrinNo;
                        inventoryTransaction.GrinPartId = inventoryDetails[i].GrinPartId;
                        inventoryTransaction.shopOrderNo = shopOrderNumber;

                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransaction);
                        _inventoryTranctionRepository.SaveAsync();

                    }
                    else
                    {
                         inventoryDetails[i].Balance_Quantity -= issuedQty;

                        lotNoWiseIssuedQty = issuedQty;
                        
                        Inventory wipInventory = InsertWipDetailsInInventoryWhenIssuedQtyIsMore(inventoryDetails[i], issuedQty, shopOrderNumber);

                        await _inventoryRepository.CreateInventory(wipInventory);
                         

                        //Add Data to inventory transaction

                        InventoryTranction inventoryTransaction = new InventoryTranction();
                        inventoryTransaction.PartNumber = inventoryDetails[i].PartNumber;
                        inventoryTransaction.MftrPartNumber = inventoryDetails[i].MftrPartNumber;
                        inventoryTransaction.LotNumber = inventoryDetails[i].LotNumber;
                        inventoryTransaction.Description = inventoryDetails[i].Description;
                        inventoryTransaction.PartType = inventoryDetails[i].PartType;
                        inventoryTransaction.ProjectNumber = inventoryDetails[i].ProjectNumber;
                        inventoryTransaction.Issued_Quantity = lotNoWiseIssuedQty;
                        inventoryTransaction.UOM = inventoryDetails[i].UOM;
                        inventoryTransaction.Issued_DateTime = DateTime.Now; 
                        inventoryTransaction.ReferenceID = inventoryDetails[i].ReferenceID;
                        inventoryTransaction.ReferenceIDFrom = inventoryDetails[i].ReferenceIDFrom;
                        inventoryTransaction.BOM_Version_No = 0;
                        inventoryTransaction.From_Location = inventoryDetails[i].Location;
                        inventoryTransaction.TO_Location = "WIP"; 
                        inventoryTransaction.Unit = inventoryDetails[i].Unit;
                        inventoryTransaction.GrinMaterialType = inventoryDetails[i].GrinMaterialType;
                        inventoryTransaction.Remarks = "Issue Material";
                        inventoryTransaction.IsStockAvailable = inventoryDetails[i].IsStockAvailable;
                        inventoryTransaction.Warehouse = inventoryDetails[i].Warehouse;
                        inventoryTransaction.GrinNo = inventoryDetails[i].GrinNo;
                        inventoryTransaction.GrinPartId = inventoryDetails[i].GrinPartId;
                        inventoryTransaction.shopOrderNo = shopOrderNumber;

                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransaction);
                        _inventoryTranctionRepository.SaveAsync();


                        /******* Dont Change the Position of IssuedQty and BalanceQty 
                         * Code in this Method. it should be always last ***********************/

                        issuedQty = 0;
                    }

                    string result = await _inventoryRepository.UpdateInventory(inventoryDetails[i]);

                    ///*********************************** Add data to Material Issue Tracker *************************/
                    ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker = InsertDataToMaterialIssueTracker(dtoForMaterialIssue, inventoryDetails[i], lotNoWiseIssuedQty);
                    int transactionId = await _materialIssueTrackerRepository.AddDataToMaterialIssueTracker(shopOrderMaterialIssueTracker);

                    /*********************************** End of Add data to Material Issue Tracker *************************/

                    if (issuedQty <= 0)
                    {
                        break;
                    }
                }

                _inventoryRepository.SaveAsync();
                _materialIssueTrackerRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        private static ShopOrderMaterialIssueTracker InsertDataToMaterialIssueTracker(InventoryDtoForMaterialIssue dtoForMaterialIssue, Inventory inventoryDetail, decimal lotNoWiseIssuedQty)
        {
            string shopOrderNumber = dtoForMaterialIssue.ShopOrderNumber;
            ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker = new ShopOrderMaterialIssueTracker
            {
                ShopOrderNumber = shopOrderNumber,
                Bomversion = dtoForMaterialIssue.Bomversion,
                PartNumber = inventoryDetail.PartNumber,
                LotNumber = inventoryDetail.LotNumber,
                MftrPartNumber = inventoryDetail.MftrPartNumber,
                Description = inventoryDetail.Description,
                ProjectNumber = inventoryDetail.ProjectNumber,
                IssuedQty = lotNoWiseIssuedQty,
                ConvertedToFgQty = 0,
                UOM = inventoryDetail.UOM,
                Warehouse = inventoryDetail.Warehouse,
                Location = inventoryDetail.Location,
                Unit = inventoryDetail.Unit,
                PartType = inventoryDetail.PartType,
                DataFrom = "ShopOrder",
                MRNumber = "NULL"
            };
            return shopOrderMaterialIssueTracker;
        }

       


        private Inventory InsertWipDetailsInInventoryWhenIssuedQtyIsMore(Inventory inventoryDetail, decimal issueQty,string shopOrderNumber)
        {
            Inventory wipInventory = _mapper.Map<Inventory>(inventoryDetail);

            wipInventory.Balance_Quantity = issueQty;
            wipInventory.Warehouse = "WIP";
            wipInventory.Description = inventoryDetail.Description;
            wipInventory.ReferenceIDFrom = "Shop Order";
            wipInventory.shopOrderNo = shopOrderNumber;
            return wipInventory;
        }



        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsWithInventoryStock(string itemNumber,string warehouse,string location, string projectNumber)
        {
            ServiceResponse<IEnumerable<InventoryDetailsLocationStock>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDetailsLocationStock>>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetInventoryDetailsWithInventoryStock(itemNumber, warehouse, location, projectNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber}, is invalid");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber: {itemNumber}");
                    var result = _mapper.Map<IEnumerable<InventoryDetailsLocationStock>>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned InventoryDetails With LocationStock Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //update material request issued item

        [HttpPost]
        public async Task<IActionResult> MaterialInventoryBalanceQty([FromBody] List<UpdateInventoryBalanceQty> updateInventoryBalanceQty)
        {             
            foreach (var materialIssueQty in updateInventoryBalanceQty)
            {
                foreach (var Location in materialIssueQty.MRNWarehouseList)
                {
                    decimal issuedQty = Location.Qty;
                    IEnumerable<Inventory> inventories = await _inventoryRepository.GetInventoryDetailsByItemNoandLocationandwarehouse(materialIssueQty.PartNumber, Location.Location, Location.Warehouse);
                    foreach (var invItem in inventories)
                    {
                        decimal stock = invItem.Balance_Quantity;
                        if(stock <= issuedQty)
                        {

                            invItem.Balance_Quantity = 0;
                            invItem.IsStockAvailable = false;
                            issuedQty -= stock;
                            
                        }
                        else
                        {
                            invItem.Balance_Quantity -= issuedQty;
                            issuedQty =0 ;
                        }
                        await _inventoryRepository.UpdateInventory(invItem);
                        if(issuedQty <= 0)
                        {
                            break;
                        }
                    }
                   
                }
            }
            _inventoryRepository.SaveAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> MaterialReturnNoteInventoryBalanceQty([FromBody] List<MRNUpdateInventoryBalanceQty> updateInventoryBalanceQty)
        {
            foreach (var materialReturnQty in updateInventoryBalanceQty) 
            {
                var itemNumber = materialReturnQty.PartNumber;
                var projectNo = materialReturnQty.ProjectNumber;
                var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
                                                                                "GetItemMasterDetailsForMNRByItemNo?", "&ItemNumber=", itemNumber));
                var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                dynamic itemMasterObject = itemMasterObjectData.data;

                foreach (var Location in materialReturnQty.MRNDetails)
                {

                    IEnumerable<Inventory> inventories = await _inventoryRepository.GetInventoryDetailsByItemNoandLocationandwarehouse(materialReturnQty.PartNumber, Location.Location, Location.Warehouse);
                    var inventoryItem = inventories.FirstOrDefault();
                    if (inventoryItem == null)
                    {
                        Inventory inventoryPost = new Inventory();
                        inventoryPost.PartNumber = materialReturnQty.PartNumber;
                        inventoryPost.MftrPartNumber = materialReturnQty.PartNumber;
                        inventoryPost.ProjectNumber = projectNo;
                        inventoryPost.Description = itemMasterObject.Description;
                        inventoryPost.Balance_Quantity = Location.Qty;
                        inventoryPost.UOM = itemMasterObject.Uom;
                        inventoryPost.GrinMaterialType = "";
                        inventoryPost.shopOrderNo = "";
                        inventoryPost.Unit = "";
                        inventoryPost.GrinNo = "";
                        inventoryPost.GrinPartId = 0;
                        inventoryPost.IsStockAvailable = true;
                        inventoryPost.Warehouse = Location.Warehouse;
                        inventoryPost.Location = Location.Location;
                        //inventoryPost.PartType = ;
                        inventoryPost.ReferenceID = "0";
                        inventoryPost.ReferenceIDFrom = "MaterialReturnNote";
                        await _inventoryRepository.CreateInventory(inventoryPost);
                        _inventoryRepository.SaveAsync();
                    }
                    else
                    {
                        inventoryItem.Balance_Quantity = inventoryItem.Balance_Quantity + Location.Qty;
                        await _inventoryRepository.UpdateInventory(inventoryItem);
                    }
                }
            }
            _inventoryRepository.SaveAsync();
            return Ok();
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryById(int id)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryById = await _inventoryRepository.GetInventoryById(id);
                if (getInventoryById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with id: {id}");
                    var result = _mapper.Map<InventoryDto>(getInventoryById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchInventoryDate([FromQuery] SearchsDateParms searchDateParam)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.SearchInventoryDate(searchDateParam);
                var result = _mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails By Date";
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
        public async Task<IActionResult> SearchInventory([FromQuery] SearchParames searchParames)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.SearchInventory(searchParames);

                _logger.LogInfo("Returned all Inventory");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<Inventory, InventoryDto>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails";
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
        public async Task<IActionResult> SearchInventoryDetailsWithSumOfStock([FromQuery] InventoryItemNo inventoryItemNo)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.SearchInventoryDetailsWithSumOfStock(inventoryItemNo);

                _logger.LogInfo("Returned all Inventory");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<Inventory, InventoryDto>().ReverseMap();
                //});
                //var mapper = config.CreateMapper();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Inventory, InventoryDto>()
                        .ForMember(dest => dest.Balance_Quantity, opt => opt.MapFrom(src => src.Balance_Quantity));
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails";
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
        public async Task<IActionResult> GetAllInventoryWithItems([FromBody] InventorySearchDto inventorySearch)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.GetAllInventoryWithItems(inventorySearch);

                _logger.LogInfo("Returned all Inventory");
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                    cfg.CreateMap<Inventory, InventoryDto>().ReverseMap();
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails";
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
        public async Task<IActionResult> GetInventoryDetailsWithSumOfStock([FromBody] InventoryBalQty inventoryBalQty)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.GetInventoryDetailsWithSumOfStock(inventoryBalQty);

                _logger.LogInfo("Returned all Inventory");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<Inventory, InventoryDto>().ReverseMap();
                //});
                //var mapper = config.CreateMapper();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Inventory, InventoryDto>()
                        .ForMember(dest => dest.Balance_Quantity, opt => opt.MapFrom(src => src.Balance_Quantity));
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails";
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
        public async Task<IActionResult> GetInventoryDetailsWithSumOfBalQty([FromBody] InventoryDetailsBalQty inventoryDetailsBalQty)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.GetInventoryDetailsWithSumOfBalQty(inventoryDetailsBalQty);

                _logger.LogInfo("Returned all Inventory");
                //var config = new MapperConfiguration(cfg =>
                //{
                //    cfg.AddProfile<MappingProfile>();
                //    cfg.CreateMap<Inventory, InventoryDto>().ReverseMap();
                //});
                //var mapper = config.CreateMapper();
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Inventory, InventoryDto>()
                        .ForMember(dest => dest.Balance_Quantity, opt => opt.MapFrom(src => src.Balance_Quantity));
                });
                var mapper = config.CreateMapper();

                var result = mapper.Map<IEnumerable<InventoryDto>>(inventoryDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all InventoryDetails";
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
        public async Task<IActionResult> CreateInventory([FromBody] InventoryDtoPost inventoryDtoPost)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                if (inventoryDtoPost is null)
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
                var createInvetory = _mapper.Map<Inventory>(inventoryDtoPost);
                createInvetory.IsStockAvailable = true;
                _inventoryRepository.CreateInventory(createInvetory);
                _inventoryRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Inventory Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Created("GetInventoryById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        //Create Inventory From Grin Data
        [HttpPost]
        public async Task<IActionResult> CreateInventoryFromGrin([FromBody] InventoryGrinDtoPost inventoryDto)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                if (inventoryDto is null)
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
                var createInvetory = _mapper.Map<Inventory>(inventoryDto);
                createInvetory.IsStockAvailable = true;
                _inventoryRepository.CreateInventory(createInvetory);
                _inventoryRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Inventory Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside UpdateCommodity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryById = await _inventoryRepository.GetInventoryById(id);
                if (getInventoryById == null)
                {
                    _logger.LogError($"Delete Inventory with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Inventory with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _inventoryRepository.DeleteInventory(getInventoryById);
                _logger.LogInfo(result);
                _inventoryRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Inventory Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteInventory action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //pass data from MRN using _httpclient Production service to Warehouse Service

        [HttpPost]
        public async Task<IActionResult> UpdateInventoryForMRN([FromBody] InventoryUpdateDtoForMRN inventoryUpdateDtoForMRN)
        {
          
            var projectNumber = inventoryUpdateDtoForMRN.ProjectNumber;
            var unit = inventoryUpdateDtoForMRN.Unit;
            foreach (var item in inventoryUpdateDtoForMRN.MaterialReturnNoteItems)
            {
                foreach (var warehouse in item.MRNWarehouseList)
                {
                    Inventory inventoryDetails = await _inventoryRepository.GetInventoryDetailsByItemNoProjectNoUnitWarehouseAndLocation(item.PartNumber,projectNumber,unit,warehouse.Warehouse,warehouse.Location);
                    if (inventoryDetails != null)
                    {
                        inventoryDetails.Balance_Quantity += item.ReturnQty;
                        inventoryDetails.IsStockAvailable = true;
                        await _inventoryRepository.UpdateInventory(inventoryDetails);
                    }
                    else
                    {
                        var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
                                                                                "GetItemMasterDetailsForMNRByItemNo?", "&ItemNumber=",item.PartNumber));
                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                        dynamic inventoryObject = itemMasterObjectData.data;

                        Inventory inventory = new Inventory();
                        inventory.PartNumber = item.PartNumber;
                        inventory.ProjectNumber = projectNumber;
                        inventory.Unit = unit;
                        inventory.Warehouse = warehouse.Warehouse;
                        inventory.Location = warehouse.Location;
                        inventory.MftrPartNumber = inventoryObject.MftrPartNumber;
                        inventory.Description = inventoryObject.Description;
                        inventory.ReferenceIDFrom = inventoryUpdateDtoForMRN.MRNNumber;
                        inventory.ReferenceID = inventoryUpdateDtoForMRN.MRNNumber;
                        //inventory.PartType = "";
                        inventory.UOM = inventoryObject.Uom;
                    }
                }
            }
            _inventoryRepository.SaveAsync();
            return Ok();
        }

        [HttpGet("{itemNumber}")]
        public async Task<ActionResult<decimal>> GetTotalStockOfItemNumber(string itemNumber)
        {
            var inventoryServiceResponse = new ServiceResponse<decimal>();

            try
            {
                decimal stockAvailable = await _inventoryRepository.GetTotalStockOfItemNumber(itemNumber);

                inventoryServiceResponse.Data = stockAvailable;
                inventoryServiceResponse.Message = "Retrieved stock available quantity";
                inventoryServiceResponse.Success = true;

                return Ok(inventoryServiceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteInventory action: {ex.Message}");
                inventoryServiceResponse.Success = false;
                inventoryServiceResponse.Message = "Error getting stock available";
                return StatusCode(500, inventoryServiceResponse);
            }
        }

    }
}
