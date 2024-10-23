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
using Entities.DTOs;
using Mysqlx.Crud;
using Microsoft.AspNetCore.Authorization;
using NPOI.SS.Formula.Functions;
using MySqlX.XDevAPI;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private IInventoryRepository _inventoryRepository;
        private IInventoryTranctionRepository _inventoryTranctionRepository;
        private IMaterialIssueTrackerRepository _materialIssueTrackerRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        public InventoryController(IConfiguration config, IInventoryTranctionRepository inventoryTranctionRepository, HttpClient httpClient, IInventoryRepository inventoryRepository,
           IHttpClientFactory clientFactory, ILoggerManager logger, IMapper mapper, IMaterialIssueTrackerRepository materialIssueTrackerRepository)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _inventoryTranctionRepository = inventoryTranctionRepository;
            _config = config;
            _materialIssueTrackerRepository = materialIssueTrackerRepository;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventorybyItemandProject([FromQuery] string itemNumber, [FromQuery] string projectNumber)
        {
            ServiceResponse<IEnumerable<InventoryQtyforDO>> serviceResponse = new ServiceResponse<IEnumerable<InventoryQtyforDO>>();
            try
            {
                var getAlldetails = await _inventoryRepository.GetInventorybyItemandProject(itemNumber, projectNumber);

                serviceResponse.Data = getAlldetails;
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
        public async Task<IActionResult> GetInventorybyItem([FromQuery] string itemNumber)
        {
            ServiceResponse<IEnumerable<InventoryQtyforDO>> serviceResponse = new ServiceResponse<IEnumerable<InventoryQtyforDO>>();
            try
            {
                var getAlldetails = await _inventoryRepository.GetInventorybyItem(itemNumber);

                serviceResponse.Data = getAlldetails;
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

        //WeightedAvgCost
        [HttpGet]
        public async Task<IActionResult> GetInventoryQtybyItemNo(string itemNumber)
        {
            ServiceResponse<IEnumerable<InventoryQtyForWeightedAvgCostDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryQtyForWeightedAvgCostDto>>();
            try
            {
                var getAlldetails = await _inventoryRepository.GetInventoryQtybyItemNo(itemNumber);

                serviceResponse.Data = getAlldetails;
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
                    serviceResponse.Message = $"Inventory with GRIN: {GrinNo},PartID:{GrinPartsId}, PartNo:{ItemNumber}, ProjNo:{ProjectNumber} hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with GRIN: {GrinNo},PartID:{GrinPartsId}, PartNo:{ItemNumber}, ProjNo:{ProjectNumber} hasn't been found in db.");
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

        [HttpGet]
        public async Task<IActionResult> GetIQCInventoryDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId, string ItemNumber, string ProjectNumber)

        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryDetailsByGrinNoandGrinId = await _inventoryRepository.GetIQCInventoryDetailsByGrinNoandGrinId(GrinNo, GrinPartsId, ItemNumber, ProjectNumber);
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
                _logger.LogError($"Something went wrong inside GetIQCInventoryDetailsByGrinNoandGrinId action: {ex.Message} {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetOPGIQCInventoryDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId, string ItemNumber, string ProjectNumber)

        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();

            try
            {
                var getInventoryDetailsByGrinNoandGrinId = await _inventoryRepository.GetOPGIQCInventoryDetailsByGrinNoandGrinId(GrinNo, GrinPartsId, ItemNumber, ProjectNumber);
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
                _logger.LogError($"Something went wrong inside GetOPGIQCInventoryDetailsByGrinNoandGrinId action: {ex.Message} {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //MaterialRequest
        [HttpGet]
        public async Task<IActionResult> GetInventoryItemNoAndDescriptionListByProjectNo(string projectNumber)

        {
            ServiceResponse<IEnumerable<GetInventoryItemNoAndDescriptionList>> serviceResponse = new ServiceResponse<IEnumerable<GetInventoryItemNoAndDescriptionList>>();

            try
            {
                var inventoryDetailsByProjectNo = await _inventoryRepository.GetInventoryItemNoAndDescriptionByProjectNo(projectNumber);
                if (inventoryDetailsByProjectNo.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with ProjectNumber: {projectNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with ProjectNumber: {projectNumber}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory ItemNo and Description with ProjectNumber: {projectNumber}");
                    var result = _mapper.Map<IEnumerable<GetInventoryItemNoAndDescriptionList>>(inventoryDetailsByProjectNo);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory ItemNo and Description List Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemNoAndDescriptionListByProjectNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //MaterialRequest
        [HttpGet]
        public async Task<IActionResult> GetInventoryItemNoAndDescriptionList()

        {
            ServiceResponse<IEnumerable<GetInventoryItemNoAndDescriptionList>> serviceResponse = new ServiceResponse<IEnumerable<GetInventoryItemNoAndDescriptionList>>();

            try
            {
                var inventoryDetails = await _inventoryRepository.GetInventoryItemNoAndDescriptionList();
                if (inventoryDetails.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory ItemNo and Description hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory ItemNo and Description hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory ItemNo and Description");
                    var result = _mapper.Map<IEnumerable<GetInventoryItemNoAndDescriptionList>>(inventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned All Inventory ItemNo and Description List Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryItemNoAndDescriptionList action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //passing itemnumber and location

        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsByItemnumberandLocation(string ItemNumber, string Location, string Warehouse, string projectNumber)

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
        public async Task<IActionResult> GetInventoryStockByItemAndShopOrderNo([FromQuery] string itemNumber, [FromQuery] string shopordernumber)
        {
            ServiceResponse<List<InventoryDto>> serviceResponse = new ServiceResponse<List<InventoryDto>>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetInventoryStockByItemAndShopOrderNo(itemNumber, shopordernumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with itemNumber and shopordernumber: {itemNumber} {shopordernumber}, is invalid";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber and shopordernumber: {itemNumber} {shopordernumber}, is invalid");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber and shopordernumber: {itemNumber} {shopordernumber}");
                    var result = _mapper.Map<List<InventoryDto>>(InventoryDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid inventory action in GetInventoryStockByItemAndShopOrderNo with Itemnumber and shopordernumber: {itemNumber} {shopordernumber} : {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Invalid inventory{ex.Message},{ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFGInventoryStockByItem([FromQuery] string itemNumber)
        {
            ServiceResponse<List<InventoryDto>> serviceResponse = new ServiceResponse<List<InventoryDto>>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetFGInventoryStockByItem(itemNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with itemNumber: {itemNumber} , is invalid";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber} , is invalid");
                    return NotFound(serviceResponse);
                }
                else
                {
                    List<Inventory> inventory = new List<Inventory>();
                    foreach (var eachinv in InventoryDetails)
                    {
                        if (inventory.Count != 0)
                        {
                            var inv = inventory.Where(x => x.shopOrderNo == eachinv.shopOrderNo).FirstOrDefault();
                            if (inv != null) inv.Balance_Quantity = inv.Balance_Quantity + eachinv.Balance_Quantity;
                            else inventory.Add(eachinv);
                        }
                        else inventory.Add(eachinv);
                    }
                    _logger.LogInfo($"Returned Inventory with Itemnumber : {itemNumber} ");
                    var result = _mapper.Map<List<InventoryDto>>(inventory);
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
        public async Task<IActionResult> GetSAInventoryStockByItem([FromQuery] string itemNumber)
        {
            ServiceResponse<List<InventoryDto>> serviceResponse = new ServiceResponse<List<InventoryDto>>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetSAInventoryStockByItem(itemNumber);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory with itemNumber: {itemNumber} , is invalid";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {itemNumber} , is invalid");
                    return NotFound(serviceResponse);
                }
                else
                {
                    List<Inventory> inventory = new List<Inventory>();
                    foreach (var eachinv in InventoryDetails)
                    {
                        if (inventory.Count != 0)
                        {
                            var inv = inventory.Where(x => x.shopOrderNo == eachinv.shopOrderNo).FirstOrDefault();
                            if (inv != null) inv.Balance_Quantity = inv.Balance_Quantity + eachinv.Balance_Quantity;
                            else inventory.Add(eachinv);
                        }
                        else inventory.Add(eachinv);
                    }
                    _logger.LogInfo($"Returned Inventory with Itemnumber : {itemNumber} ");
                    var result = _mapper.Map<List<InventoryDto>>(inventory);
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

        public async Task<IActionResult> GetAvailableStockQtyForSalesOrderItems([FromQuery] string salesOrderNo, [FromQuery] int salesOrderStatus, [FromBody] List<string> itemNumberList)
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

                ItemNoWithQtyDict = await GetItemNumberWiseQuantityDict(salesOrderNo, salesOrderStatus, itemNumberList);

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



                //List<string> shopOrderNumberList = (List<string>)shopOrderNumberListData.data;
                if (salesOrderStatus == 0) // Retail SO
                {
                    decimal itemQty = await _inventoryRepository.GetStockQtyForRetailSalesOrderItem(itemNo);
                    ItemNoWithQtyDict.Add(itemNo, itemQty);
                }
                else if (salesOrderStatus == 1) // Built to Print SO
                {
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();

                    var encodedSalesOrderNumber = Uri.EscapeDataString(salesOrderNo);
                    var encodedItemNumber = Uri.EscapeDataString(itemNo);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ProductionAPI"],
                                $"GetShopOrderNoListBySalesOrderNo?SalesOrderNO={encodedSalesOrderNumber}&itemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var shopOrderNumberListResponse = await client.SendAsync(request);

                    var shopOrderNumberListString = await shopOrderNumberListResponse.Content.ReadAsStringAsync();
                    var shopOrderNumberList = JsonConvert.DeserializeObject<List<string>>(shopOrderNumberListString);

                    //List<string> shopOrderNumberList = new List<string>();
                    //foreach (var item in shopOrderNumberListData)
                    //{
                    //    shopOrderNumberList.Add(item.ToString());
                    //}
                    decimal itemQty = await _inventoryRepository.GetStockQtyForBtpSalesOrderItem(itemNo, shopOrderNumberList);
                    ItemNoWithQtyDict.Add(itemNo, itemQty);
                }
            }
            return ItemNoWithQtyDict;
        }

        [HttpGet]
        public async Task<IActionResult> GetStockDetailsForAllLocationWarehouseByItemNo(string itemNumber)
        {
            ServiceResponse<decimal?> serviceResponse = new ServiceResponse<decimal?>();
            try
            {
                decimal? InventoryDetails = await _inventoryRepository.GetStockDetailsForAllLocationWarehouseByItemNo(itemNumber);
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
                    var result = InventoryDetails;
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
        public async Task<IActionResult> GetStockDetailsForAllLocationWarehouseByItemNoAndProjectNo(string itemNumber, string projectNo)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                decimal InventoryDetails = await _inventoryRepository.GetStockDetailsForAllLocationWarehouseByItemNoAndProjectNo(itemNumber, projectNo);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber And ProjectNumber: {itemNumber},{projectNo}, is invalid");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber And ProjectNumber: {itemNumber},{projectNo}");
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
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetInventoryByItemNo(itemNumber);
                if (InventoryDetails.Count() == 0)
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
                    var result = _mapper.Map<IEnumerable<InventoryDto>>(InventoryDetails);
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
        //passing list of item number in consumption report
        [HttpPost]
        public async Task<IActionResult> GetConsumptionInventoryByItemNotest1(List<string> itemNumberList)
        {
            ServiceResponse<List<ConsumptionInventoryDto>> serviceResponse = new ServiceResponse<List<ConsumptionInventoryDto>>();

            try
            {
                var InventoryDetails = await _inventoryRepository.GetConsumptionInventoryByItemNotest(itemNumberList);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {itemNumberList}, is invalid");
                    return NotFound(serviceResponse);

                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber: {itemNumberList}");
                    var result = _mapper.Map<List<ConsumptionInventoryDto>>(InventoryDetails);
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

        //passing list of item number in consumption report
        [HttpPost]
        public async Task<IActionResult> GetConsumptionInventoryByItemAndProjectNotest2(List<InventoryItemNoAndProjectNoDto> ItemNoAndProjectNoList)
        {
            ServiceResponse<List<ConsumptionInventoryByProjectNoDto>> serviceResponse = new ServiceResponse<List<ConsumptionInventoryByProjectNoDto>>();

            try
            {
                foreach (var item in ItemNoAndProjectNoList)
                {

                    var InventoryDetails = await _inventoryRepository.GetConsumptionInventoryByItemNoAndProjectNotest1(item.FGItemNumber, item.ProjectNumber);

                    if (InventoryDetails == null || InventoryDetails.Count <= 0)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Inventory Details hasn't been found";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        _logger.LogError($"Inventory with itemNumber and ProjectNumber is invalid");
                        return NotFound(serviceResponse);

                    }
                    else
                    {
                        _logger.LogInfo($"Returned Inventory with Itemnumber and ProjectNumber");
                        var result = _mapper.Map<List<ConsumptionInventoryByProjectNoDto>>(InventoryDetails);
                        serviceResponse.Data = result;
                        serviceResponse.Message = "Returned InventoryDetails with id Successfully";
                        serviceResponse.Success = true;
                        serviceResponse.StatusCode = HttpStatusCode.OK;
                        return Ok(serviceResponse);
                    }
                }
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in inventory action GetConsumptionInventoryByItemAndProjectNotest2: {ex.Message},{ex.InnerException}");
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

        [HttpPost]
        public async Task<IActionResult> GetConsumptionChildItemStockWithWipQty(List<string> itemNumberList)
        {
            ServiceResponse<List<ConsumptionChildItemInventoryDto>> serviceResponse = new ServiceResponse<List<ConsumptionChildItemInventoryDto>>();
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
                    //var result = _mapper.Map<ConsumptionInventoryDto>(InventoryDetails);
                    //var result = _mapper.Map<ConsumptionChildItemInventoryDto>(InventoryDetails);
                    serviceResponse.Data = InventoryDetails;
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

        [HttpPost]
        public async Task<IActionResult> GetConsumptionChildItemStockWithWipQtyByProjectNo(string projectNo, List<string> itemNumberList)
        {
            ServiceResponse<List<ConsumptionChildItemInventoryDto>> serviceResponse = new ServiceResponse<List<ConsumptionChildItemInventoryDto>>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetConsumptionChildItemStockWithWipQtyByProjectNo(projectNo, itemNumberList);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"In GetConsumptionChildItemStockWithWipQtyByProjectNo ItemNumber List is Empty");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber List and projectNo in GetConsumptionChildItemStockWithWipQtyByProjectNo");
                    //var result = _mapper.Map<ConsumptionInventoryDto>(InventoryDetails);
                    //var result = _mapper.Map<ConsumptionChildItemInventoryDto>(InventoryDetails);
                    serviceResponse.Data = InventoryDetails;
                    serviceResponse.Message = "Returned InventoryDetails with Itemnumber List and projectNo  Successfully";
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

        [HttpPost]
        public async Task<IActionResult> GetConsumptionChildItemStockWithWipQtyByMultipleProjectNo(coverageInventoryByMultipleProjectDto coverageInventoryByMultipleProjectDto)
        {
            ServiceResponse<List<ConsumptionChildItemForProjectListInventoryDto>> serviceResponse = new ServiceResponse<List<ConsumptionChildItemForProjectListInventoryDto>>();
            try
            {
                var InventoryDetails = await _inventoryRepository.GetConsumptionChildItemStockWithWipQtyByMultipleProjectNo(coverageInventoryByMultipleProjectDto.itemNumberList,
                                                                                                                                        coverageInventoryByMultipleProjectDto.projectNo);
                if (InventoryDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"In GetConsumptionChildItemStockWithWipQtyByProjectNo ItemNumber List is Empty");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Inventory with Itemnumber List and projectNo in GetConsumptionChildItemStockWithWipQtyByProjectNo");
                    //var result = _mapper.Map<ConsumptionInventoryDto>(InventoryDetails);
                    //var result = _mapper.Map<ConsumptionChildItemInventoryDto>(InventoryDetails);
                    serviceResponse.Data = InventoryDetails;
                    serviceResponse.Message = "Returned InventoryDetails with Itemnumber List and projectNo  Successfully";
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

                    var ItemNumber = item.PartNumber;
                    //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
                    //    "GetItemMasterByItemNumber?", "&ItemNumber=", ItemNumber));
                    var client = _clientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromMinutes(5);
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                    var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                        $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                    request.Headers.Add("Authorization", token);

                    var itemMasterObjectResult = await client.SendAsync(request);
                    _logger.LogInfo("getitemmasterdata" + Convert.ToString(itemMasterObjectResult));
                    var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                    dynamic itemMatserObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                    dynamic itemMasterTranctionObject = itemMatserObjectData.data;

                    var ItemNo = itemMasterTranctionObject.itemNumber;
                    var Desc = itemMasterTranctionObject.description;
                    var uom = itemMasterTranctionObject.uom;
                    var ProjectNo = itemMasterTranctionObject.projectNumber;
                    var ItemType = itemMasterTranctionObject.itemType;

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
                        await UpdateDataToMaterialIssueTracker(item, inventoryDetails[i], lotNoWiseProducedQty);
                        /*********************************** End of Add data to Material Issue Tracker *************************/

                        //Add InventoryTransaction

                        InventoryTranction inventoryTranction = new InventoryTranction();
                        inventoryTranction.PartNumber = ItemNo;
                        inventoryTranction.MftrPartNumber = ItemNo;
                        inventoryTranction.Description = Desc;
                        inventoryTranction.ProjectNumber = ProjectNo;
                        inventoryTranction.PartType = ItemType;
                        inventoryTranction.Issued_Quantity = 0;
                        inventoryTranction.UOM = uom;
                        inventoryTranction.BOM_Version_No = 0;
                        inventoryTranction.Issued_DateTime = DateTime.Now;
                        inventoryTranction.shopOrderNo = inventoryDetails[i].shopOrderNo;
                        inventoryTranction.ReferenceID = inventoryDetails[i].ReferenceID;
                        inventoryTranction.ReferenceIDFrom = inventoryDetails[i].ReferenceIDFrom;
                        inventoryTranction.From_Location = inventoryDetails[i].Location;
                        inventoryTranction.TO_Location = inventoryDetails[i].Location;
                        inventoryTranction.Warehouse = inventoryDetails[i].Warehouse;

                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranction);

                        if (producedQty <= 0)
                        {
                            break;
                        }
                    }
                }

                _inventoryRepository.SaveAsync();
                _inventoryTranctionRepository.SaveAsync();
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

        private async Task UpdateDataToMaterialIssueTracker(InventoryDtoForShopOrderConfirmation item, Inventory inventoryDetail, decimal lotNoWiseProducedQty)
        {
            // Retrieve the existing entry from the repository based on the ShopOrderNumber, PartNumber, and LotNumber

            List<ShopOrderMaterialIssueTracker> materialIssueTrackerList = await _materialIssueTrackerRepository
                                .GetDetailsByShopOrderNOItemNoLotNo(inventoryDetail.PartNumber, inventoryDetail.shopOrderNo, inventoryDetail.LotNumber);

            if (materialIssueTrackerList != null || materialIssueTrackerList.Count > 0)
            {
                decimal newConvertedToFgQty = lotNoWiseProducedQty;

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

                if (inventoryDetails == null || inventoryDetails.Count == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {dtoForMaterialIssue.PartNumber}, is invalid");
                    return NotFound(serviceResponse);
                }

                decimal issuedQty = dtoForMaterialIssue.IssueQty;
                string shopOrderNumber = dtoForMaterialIssue.ShopOrderNumber;
                for (int i = 0; i < inventoryDetails.Count; i++)
                {
                    decimal balanceqty = inventoryDetails[i].Balance_Quantity;
                    decimal lotNoWiseIssuedQty = 0;
                    if (inventoryDetails[i].Balance_Quantity <= issuedQty)
                    {

                        lotNoWiseIssuedQty = balanceqty;


                        ///*********************************** Add data to Material Issue Tracker *************************/
                        ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker = InsertDataToMaterialIssueTracker(dtoForMaterialIssue, inventoryDetails[i], lotNoWiseIssuedQty);
                        int transactionId = await _materialIssueTrackerRepository.AddDataToMaterialIssueTracker(shopOrderMaterialIssueTracker);

                        /*********************************** End of Add data to Material Issue Tracker *************************/

                        inventoryDetails[i].Warehouse = "WIP";
                        inventoryDetails[i].Location = "WIP";
                        inventoryDetails[i].shopOrderNo = shopOrderNumber;
                        inventoryDetails[i].IsStockAvailable = true;
                        inventoryDetails[i].ReferenceIDFrom = "Material Issue";

                        /** Dont Change the Position of IssuedQty and BalanceQty Code in this Method .it should be always last ***********************/
                        issuedQty -= balanceqty;
                        balanceqty = 0;
                    }
                    else
                    {
                        inventoryDetails[i].Balance_Quantity -= issuedQty;

                        lotNoWiseIssuedQty = issuedQty;

                        Inventory wipInventory = InsertWipDetailsInInventoryWhenIssuedQtyIsMore(inventoryDetails[i], issuedQty, shopOrderNumber);

                        await _inventoryRepository.CreateInventory(wipInventory);

                        InventoryTranction inventoryTransaction1 = new InventoryTranction();
                        inventoryTransaction1.PartNumber = wipInventory.PartNumber;
                        inventoryTransaction1.MftrPartNumber = wipInventory.MftrPartNumber;
                        inventoryTransaction1.LotNumber = wipInventory.LotNumber;
                        inventoryTransaction1.Description = wipInventory.Description;
                        inventoryTransaction1.PartType = wipInventory.PartType;
                        inventoryTransaction1.ProjectNumber = wipInventory.ProjectNumber;
                        inventoryTransaction1.Issued_Quantity = wipInventory.Balance_Quantity;
                        inventoryTransaction1.UOM = wipInventory.UOM;
                        inventoryTransaction1.Issued_DateTime = DateTime.Now;
                        // inventoryTransaction1.Issued_By = 
                        inventoryTransaction1.ReferenceID = wipInventory.ReferenceID;
                        inventoryTransaction1.ReferenceIDFrom = wipInventory.ReferenceIDFrom;
                        inventoryTransaction1.BOM_Version_No = 0;
                        inventoryTransaction1.IsStockAvailable = wipInventory.IsStockAvailable;
                        inventoryTransaction1.From_Location = wipInventory.Location;
                        inventoryTransaction1.TO_Location = wipInventory.Location;
                        inventoryTransaction1.Unit = wipInventory.Unit;
                        inventoryTransaction1.GrinMaterialType = wipInventory.GrinMaterialType;
                        inventoryTransaction1.Remarks = "Issue Material";
                        inventoryTransaction1.IsStockAvailable = wipInventory.IsStockAvailable;
                        inventoryTransaction1.Warehouse = wipInventory.Warehouse;
                        inventoryTransaction1.GrinNo = wipInventory.GrinNo;
                        inventoryTransaction1.GrinPartId = wipInventory.GrinPartId;
                        inventoryTransaction1.shopOrderNo = shopOrderNumber;

                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransaction1);


                        ///*********************************** Add data to Material Issue Tracker *************************/
                        ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker = InsertDataToMaterialIssueTracker(dtoForMaterialIssue, inventoryDetails[i], lotNoWiseIssuedQty);
                        int transactionId = await _materialIssueTrackerRepository.AddDataToMaterialIssueTracker(shopOrderMaterialIssueTracker);

                        /*********************************** End of Add data to Material Issue Tracker *************************/

                        /******* Dont Change the Position of IssuedQty and BalanceQty 
                         * Code in this Method. it should be always last ***********************/

                        issuedQty = 0;
                    }

                    string result = await _inventoryRepository.UpdateInventory(inventoryDetails[i]);


                    InventoryTranction inventoryTransaction = new InventoryTranction();
                    inventoryTransaction.PartNumber = inventoryDetails[i].PartNumber;
                    inventoryTransaction.MftrPartNumber = inventoryDetails[i].MftrPartNumber;
                    inventoryTransaction.LotNumber = inventoryDetails[i].LotNumber;
                    inventoryTransaction.Description = inventoryDetails[i].Description;
                    inventoryTransaction.PartType = inventoryDetails[i].PartType;
                    inventoryTransaction.ProjectNumber = inventoryDetails[i].ProjectNumber;
                    inventoryTransaction.Issued_Quantity = inventoryDetails[i].Balance_Quantity;
                    inventoryTransaction.UOM = inventoryDetails[i].UOM;
                    inventoryTransaction.Issued_DateTime = DateTime.Now;
                    inventoryTransaction.ReferenceID = inventoryDetails[i].ReferenceID;
                    inventoryTransaction.ReferenceIDFrom = inventoryDetails[i].ReferenceIDFrom;
                    inventoryTransaction.BOM_Version_No = 0;
                    inventoryTransaction.IsStockAvailable = inventoryDetails[i].IsStockAvailable;
                    inventoryTransaction.From_Location = inventoryDetails[i].Location;
                    inventoryTransaction.TO_Location = inventoryDetails[i].Location;
                    inventoryTransaction.Unit = inventoryDetails[i].Unit;
                    inventoryTransaction.GrinMaterialType = inventoryDetails[i].GrinMaterialType;
                    inventoryTransaction.Remarks = "Issue Material";
                    inventoryTransaction.IsStockAvailable = inventoryDetails[i].IsStockAvailable;
                    inventoryTransaction.Warehouse = inventoryDetails[i].Warehouse;
                    inventoryTransaction.GrinNo = inventoryDetails[i].GrinNo;
                    inventoryTransaction.GrinPartId = inventoryDetails[i].GrinPartId;
                    inventoryTransaction.shopOrderNo = shopOrderNumber;

                    await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransaction);

                    if (issuedQty <= 0)
                    {
                        break;
                    }
                }

                _inventoryRepository.SaveAsync();
                _inventoryTranctionRepository.SaveAsync();
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

        [HttpPost]
        public async Task<IActionResult> UpdateInventoryOnMaterialIssueLocation(InventoryDtoForMaterialIssueLocation dtoForMaterialIssue)
        {
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                var inventoryDetails = await _inventoryRepository
                        .GetInventoryDetailsByItemNoandProjectNoandWarehouseandLocation(dtoForMaterialIssue.PartNumber, dtoForMaterialIssue.ProjectNumber, dtoForMaterialIssue.Warehouse,
                                                                                                        dtoForMaterialIssue.Location, dtoForMaterialIssue.LotNumber);

                if (inventoryDetails.Count() == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Inventory Details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Inventory with itemNumber: {dtoForMaterialIssue.PartNumber}, is invalid");
                    return NotFound(serviceResponse);
                }

                decimal disQty = dtoForMaterialIssue.DistributingQty;
                string shopOrderNumber = dtoForMaterialIssue.ShopOrderNumber;
                for (int i = 0; i < inventoryDetails.Count(); i++)
                {
                    decimal balanceqty = inventoryDetails[i].Balance_Quantity;
                    decimal lotNoWiseIssuedQty = 0;
                    if (inventoryDetails[i].Balance_Quantity <= disQty)
                    {

                        lotNoWiseIssuedQty = balanceqty;


                        ///*********************************** Add data to Material Issue Tracker *************************/
                        ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker = InsertMaterialIssueDataToMaterialIssueTracker(dtoForMaterialIssue, inventoryDetails[i], lotNoWiseIssuedQty);
                        int transactionId = await _materialIssueTrackerRepository.AddDataToMaterialIssueTracker(shopOrderMaterialIssueTracker);

                        /*********************************** End of Add data to Material Issue Tracker *************************/

                        inventoryDetails[i].Warehouse = "WIP";
                        inventoryDetails[i].Location = "WIP";
                        inventoryDetails[i].shopOrderNo = shopOrderNumber;
                        inventoryDetails[i].IsStockAvailable = true;
                        inventoryDetails[i].ReferenceIDFrom = "Material Issue";
                        inventoryDetails[i].ReferenceID = shopOrderNumber;
                        /** Dont Change the Position of IssuedQty and BalanceQty Code in this Method .it should be always last ***********************/
                        disQty -= balanceqty;
                        balanceqty = 0;
                    }
                    else
                    {
                        inventoryDetails[i].ReferenceID = shopOrderNumber;
                        inventoryDetails[i].Balance_Quantity -= disQty;

                        lotNoWiseIssuedQty = disQty;

                        Inventory wipInventory = InsertWipDetailsInInventoryWhenIssuedQtyIsMore(inventoryDetails[i], disQty, shopOrderNumber);

                        await _inventoryRepository.CreateInventory(wipInventory);

                        InventoryTranction inventoryTransaction1 = new InventoryTranction();
                        inventoryTransaction1.PartNumber = wipInventory.PartNumber;
                        inventoryTransaction1.MftrPartNumber = wipInventory.MftrPartNumber;
                        inventoryTransaction1.LotNumber = wipInventory.LotNumber;
                        inventoryTransaction1.Description = wipInventory.Description;
                        inventoryTransaction1.PartType = wipInventory.PartType;
                        inventoryTransaction1.ProjectNumber = wipInventory.ProjectNumber;
                        inventoryTransaction1.Issued_Quantity = wipInventory.Balance_Quantity;
                        inventoryTransaction1.UOM = wipInventory.UOM;
                        inventoryTransaction1.Issued_DateTime = DateTime.Now;
                        // inventoryTransaction1.Issued_By = 
                        inventoryTransaction1.ReferenceID = wipInventory.ReferenceID;
                        inventoryTransaction1.ReferenceIDFrom = wipInventory.ReferenceIDFrom;
                        inventoryTransaction1.BOM_Version_No = 0;
                        inventoryTransaction1.IsStockAvailable = wipInventory.IsStockAvailable;
                        inventoryTransaction1.From_Location = wipInventory.Location;
                        inventoryTransaction1.TO_Location = wipInventory.Location;
                        inventoryTransaction1.Unit = wipInventory.Unit;
                        inventoryTransaction1.GrinMaterialType = wipInventory.GrinMaterialType;
                        inventoryTransaction1.Remarks = "Issue Material";
                        inventoryTransaction1.IsStockAvailable = wipInventory.IsStockAvailable;
                        inventoryTransaction1.Warehouse = wipInventory.Warehouse;
                        inventoryTransaction1.GrinNo = wipInventory.GrinNo;
                        inventoryTransaction1.GrinPartId = wipInventory.GrinPartId;
                        inventoryTransaction1.shopOrderNo = shopOrderNumber;

                        await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransaction1);


                        ///*********************************** Add data to Material Issue Tracker *************************/
                        ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker = InsertMaterialIssueDataToMaterialIssueTracker(dtoForMaterialIssue, inventoryDetails[i], lotNoWiseIssuedQty);
                        int transactionId = await _materialIssueTrackerRepository.AddDataToMaterialIssueTracker(shopOrderMaterialIssueTracker);

                        /*********************************** End of Add data to Material Issue Tracker *************************/

                        /******* Dont Change the Position of IssuedQty and BalanceQty 
                         * Code in this Method. it should be always last ***********************/

                        disQty = 0;
                    }

                    string result = await _inventoryRepository.UpdateInventory(inventoryDetails[i]);


                    InventoryTranction inventoryTransaction = new InventoryTranction();
                    inventoryTransaction.PartNumber = inventoryDetails[i].PartNumber;
                    inventoryTransaction.MftrPartNumber = inventoryDetails[i].MftrPartNumber;
                    inventoryTransaction.LotNumber = inventoryDetails[i].LotNumber;
                    inventoryTransaction.Description = inventoryDetails[i].Description;
                    inventoryTransaction.PartType = inventoryDetails[i].PartType;
                    inventoryTransaction.ProjectNumber = inventoryDetails[i].ProjectNumber;
                    inventoryTransaction.Issued_Quantity = inventoryDetails[i].Balance_Quantity;
                    inventoryTransaction.UOM = inventoryDetails[i].UOM;
                    inventoryTransaction.Issued_DateTime = DateTime.Now;
                    inventoryTransaction.Issued_By = inventoryDetails[i].LastModifiedBy;
                    inventoryTransaction.ReferenceID = inventoryDetails[i].ReferenceID;
                    inventoryTransaction.ReferenceIDFrom = inventoryDetails[i].ReferenceIDFrom;
                    inventoryTransaction.BOM_Version_No = 0;
                    inventoryTransaction.IsStockAvailable = inventoryDetails[i].IsStockAvailable;
                    inventoryTransaction.From_Location = inventoryDetails[i].Location;
                    inventoryTransaction.TO_Location = inventoryDetails[i].Location;
                    inventoryTransaction.Unit = inventoryDetails[i].Unit;
                    inventoryTransaction.GrinMaterialType = inventoryDetails[i].GrinMaterialType;
                    inventoryTransaction.Remarks = "Issue Material";
                    inventoryTransaction.IsStockAvailable = inventoryDetails[i].IsStockAvailable;
                    inventoryTransaction.Warehouse = inventoryDetails[i].Warehouse;
                    inventoryTransaction.GrinNo = inventoryDetails[i].GrinNo;
                    inventoryTransaction.GrinPartId = inventoryDetails[i].GrinPartId;
                    inventoryTransaction.shopOrderNo = shopOrderNumber;

                    await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransaction);

                    if (disQty <= 0)
                    {
                        break;
                    }
                }

                _inventoryRepository.SaveAsync();
                _inventoryTranctionRepository.SaveAsync();
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

        private static ShopOrderMaterialIssueTracker InsertMaterialIssueDataToMaterialIssueTracker(InventoryDtoForMaterialIssueLocation dtoForMaterialIssue, Inventory inventoryDetail, decimal lotNoWiseIssuedQty)
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

        private static ShopOrderMaterialIssueTracker MRInsertDataToMaterialIssueTracker(string mrNumber, string shopOrderNumber, Inventory inventoryDetail, decimal lotNoWiseIssuedQty)
        {
            ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker = new ShopOrderMaterialIssueTracker
            {
                ShopOrderNumber = shopOrderNumber,
                Bomversion = 0,
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
                DataFrom = "Material Request",
                MRNumber = mrNumber
            };
            return shopOrderMaterialIssueTracker;
        }

        private Inventory InsertWipDetailsInInventoryWhenIssuedQtyIsMore(Inventory inventoryDetail, decimal issueQty, string shopOrderNumber)
        {
            //Inventory wipInventory = _mapper.Map<Inventory>(inventoryDetail);

            Inventory wipInventory = new Inventory();

            wipInventory.PartNumber = inventoryDetail.PartNumber;
            wipInventory.LotNumber = inventoryDetail.LotNumber;
            wipInventory.MftrPartNumber = inventoryDetail.MftrPartNumber;
            wipInventory.Description = inventoryDetail.Description;
            wipInventory.ProjectNumber = inventoryDetail.ProjectNumber;
            wipInventory.UOM = inventoryDetail.UOM;
            wipInventory.Unit = inventoryDetail.Unit;
            wipInventory.GrinNo = inventoryDetail.GrinNo;
            wipInventory.GrinPartId = inventoryDetail.GrinPartId;
            wipInventory.PartType = inventoryDetail.PartType;
            wipInventory.GrinMaterialType = inventoryDetail.GrinMaterialType;
            wipInventory.ReferenceIDFrom = "Material Issue";
            wipInventory.ReferenceID = shopOrderNumber;
            wipInventory.CreatedOn = DateTime.Now;
            wipInventory.Balance_Quantity = issueQty;
            wipInventory.Warehouse = "WIP";
            wipInventory.Location = "WIP";
            wipInventory.IsStockAvailable = true;
            wipInventory.shopOrderNo = shopOrderNumber;
            wipInventory.Min = inventoryDetail.Min;
            wipInventory.Max = inventoryDetail.Max;

            return wipInventory;
        }

        //for material request tracker

        private Inventory MRInsertWipDetailsInInventoryWhenIssuedQtyIsMore(Inventory inventoryDetail, decimal issueQty, string? shopOrderNumber)
        {
            //Inventory wipInventory = _mapper.Map<Inventory>(inventoryDetail);

            Inventory wipInventory = new Inventory();

            wipInventory.PartNumber = inventoryDetail.PartNumber;
            wipInventory.LotNumber = inventoryDetail.LotNumber;
            wipInventory.MftrPartNumber = inventoryDetail.MftrPartNumber;
            wipInventory.Description = inventoryDetail.Description;
            wipInventory.ProjectNumber = inventoryDetail.ProjectNumber;
            wipInventory.UOM = inventoryDetail.UOM;
            wipInventory.Unit = inventoryDetail.Unit;
            wipInventory.GrinNo = inventoryDetail.GrinNo;
            wipInventory.GrinPartId = inventoryDetail.GrinPartId;
            wipInventory.PartType = inventoryDetail.PartType;
            wipInventory.GrinMaterialType = inventoryDetail.GrinMaterialType;
            wipInventory.ReferenceIDFrom = "Material Request";
            // wipInventory.ReferenceID = inventoryDetail.ReferenceID;
            wipInventory.CreatedOn = DateTime.Now;
            wipInventory.Balance_Quantity = issueQty;
            wipInventory.Warehouse = "WIP";
            wipInventory.Location = "WIP";
            wipInventory.IsStockAvailable = true;
            wipInventory.shopOrderNo = shopOrderNumber;
            wipInventory.Min = inventoryDetail.Min;
            wipInventory.Max = inventoryDetail.Max;

            return wipInventory;
        }





        [HttpGet]
        public async Task<IActionResult> GetInventoryDetailsWithInventoryStock(string itemNumber, string warehouse, string location, string projectNumber)
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
            ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
            try
            {
                foreach (var materialIssueQty in updateInventoryBalanceQty)
                {
                    var mrNumber = materialIssueQty.MRNumber;
                    foreach (var Location in materialIssueQty.MRNWarehouseList)
                    {
                        if (Location.IsMRIssueDone != true)
                        {
                            decimal issuedQty = Location.Qty;
                            decimal lotNoWiseIssuedQty = 0;
                            IEnumerable<Inventory> inventories = await _inventoryRepository.GetInventoryDetailsByItemNoandLocationandwarehouse(materialIssueQty.PartNumber, Location.Location, Location.Warehouse, materialIssueQty.ProjectNumber);
                            foreach (var invItem in inventories)
                            {
                                invItem.shopOrderNo = updateInventoryBalanceQty[0].ShopOrderNumber;
                                var shopOrderNumber = invItem.shopOrderNo;
                                decimal stock = invItem.Balance_Quantity;
                                if (stock <= issuedQty)
                                {
                                    invItem.Balance_Quantity = 0;
                                    invItem.IsStockAvailable = false;
                                    issuedQty -= stock;
                                    lotNoWiseIssuedQty = stock;
                                    /** Dont Change the Position of IssuedQty and BalanceQty Code in this Method .it should be always last ***********************/


                                    Inventory wipInventory = MRInsertWipDetailsInInventoryWhenIssuedQtyIsMore(invItem, lotNoWiseIssuedQty, shopOrderNumber);
                                    wipInventory.ReferenceID = mrNumber;
                                    await _inventoryRepository.CreateInventory(wipInventory);

                                    //create inventory transaction
                                    InventoryTranction inventoryTransaction1 = new InventoryTranction();
                                    inventoryTransaction1.PartNumber = wipInventory.PartNumber;
                                    inventoryTransaction1.MftrPartNumber = wipInventory.MftrPartNumber;
                                    inventoryTransaction1.LotNumber = wipInventory.LotNumber;
                                    inventoryTransaction1.Description = wipInventory.Description;
                                    inventoryTransaction1.PartType = wipInventory.PartType;
                                    inventoryTransaction1.ProjectNumber = wipInventory.ProjectNumber;
                                    inventoryTransaction1.Issued_Quantity = wipInventory.Balance_Quantity;
                                    inventoryTransaction1.UOM = wipInventory.UOM;
                                    inventoryTransaction1.Issued_DateTime = DateTime.Now;
                                    inventoryTransaction1.ReferenceID = wipInventory.ReferenceID;
                                    inventoryTransaction1.ReferenceIDFrom = wipInventory.ReferenceIDFrom;
                                    inventoryTransaction1.BOM_Version_No = 0;
                                    inventoryTransaction1.From_Location = invItem.Location;
                                    inventoryTransaction1.TO_Location = wipInventory.Location;
                                    inventoryTransaction1.Unit = wipInventory.Unit;
                                    inventoryTransaction1.GrinMaterialType = wipInventory.GrinMaterialType;
                                    inventoryTransaction1.Remarks = "Material Request";
                                    inventoryTransaction1.IsStockAvailable = wipInventory.IsStockAvailable;
                                    inventoryTransaction1.Warehouse = wipInventory.Warehouse;
                                    inventoryTransaction1.GrinNo = wipInventory.GrinNo;
                                    inventoryTransaction1.GrinPartId = wipInventory.GrinPartId;
                                    inventoryTransaction1.shopOrderNo = wipInventory.shopOrderNo;

                                    await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransaction1);

                                }
                                else
                                {

                                    invItem.Balance_Quantity -= issuedQty;
                                    lotNoWiseIssuedQty = issuedQty;

                                    Inventory wipInventory = MRInsertWipDetailsInInventoryWhenIssuedQtyIsMore(invItem, issuedQty, shopOrderNumber);
                                    wipInventory.ReferenceID = mrNumber;
                                    await _inventoryRepository.CreateInventory(wipInventory);

                                    InventoryTranction inventoryTransaction1 = new InventoryTranction();
                                    inventoryTransaction1.PartNumber = wipInventory.PartNumber;
                                    inventoryTransaction1.MftrPartNumber = wipInventory.MftrPartNumber;
                                    inventoryTransaction1.LotNumber = wipInventory.LotNumber;
                                    inventoryTransaction1.Description = wipInventory.Description;
                                    inventoryTransaction1.PartType = wipInventory.PartType;
                                    inventoryTransaction1.ProjectNumber = wipInventory.ProjectNumber;
                                    inventoryTransaction1.Issued_Quantity = wipInventory.Balance_Quantity;
                                    inventoryTransaction1.UOM = wipInventory.UOM;
                                    inventoryTransaction1.Issued_DateTime = DateTime.Now;
                                    inventoryTransaction1.ReferenceID = wipInventory.ReferenceID;
                                    inventoryTransaction1.ReferenceIDFrom = wipInventory.ReferenceIDFrom;
                                    inventoryTransaction1.BOM_Version_No = 0;
                                    inventoryTransaction1.From_Location = invItem.Location;
                                    inventoryTransaction1.TO_Location = wipInventory.Location;
                                    inventoryTransaction1.Unit = wipInventory.Unit;
                                    inventoryTransaction1.GrinMaterialType = wipInventory.GrinMaterialType;
                                    inventoryTransaction1.Remarks = "Material Request";
                                    inventoryTransaction1.IsStockAvailable = wipInventory.IsStockAvailable;
                                    inventoryTransaction1.Warehouse = wipInventory.Warehouse;
                                    inventoryTransaction1.GrinNo = wipInventory.GrinNo;
                                    inventoryTransaction1.GrinPartId = wipInventory.GrinPartId;
                                    inventoryTransaction1.shopOrderNo = wipInventory.shopOrderNo;

                                    await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransaction1);

                                    issuedQty = 0;
                                }
                                await _inventoryRepository.UpdateInventory(invItem);

                                InventoryTranction inventoryTransaction = new InventoryTranction();
                                inventoryTransaction.PartNumber = invItem.PartNumber;
                                inventoryTransaction.MftrPartNumber = invItem.MftrPartNumber;
                                inventoryTransaction.LotNumber = invItem.LotNumber;
                                inventoryTransaction.Description = invItem.Description;
                                inventoryTransaction.PartType = invItem.PartType;
                                inventoryTransaction.ProjectNumber = invItem.ProjectNumber;
                                inventoryTransaction.Issued_Quantity = invItem.Balance_Quantity;
                                inventoryTransaction.UOM = invItem.UOM;
                                inventoryTransaction.Issued_DateTime = DateTime.Now;
                                inventoryTransaction.ReferenceID = invItem.ReferenceID;
                                inventoryTransaction.ReferenceIDFrom = invItem.ReferenceIDFrom;
                                inventoryTransaction.BOM_Version_No = 0;
                                inventoryTransaction.From_Location = invItem.Location;
                                inventoryTransaction.TO_Location = invItem.Location;
                                inventoryTransaction.Unit = invItem.Unit;
                                inventoryTransaction.GrinMaterialType = invItem.GrinMaterialType;
                                inventoryTransaction.Remarks = "Material Request";
                                inventoryTransaction.IsStockAvailable = invItem.IsStockAvailable;
                                inventoryTransaction.Warehouse = invItem.Warehouse;
                                inventoryTransaction.GrinNo = invItem.GrinNo;
                                inventoryTransaction.GrinPartId = invItem.GrinPartId;
                                inventoryTransaction.shopOrderNo = invItem.shopOrderNo;

                                await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransaction);

                                ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker = MRInsertDataToMaterialIssueTracker(mrNumber, shopOrderNumber, invItem, lotNoWiseIssuedQty);
                                int transactionId = await _materialIssueTrackerRepository.AddDataToMaterialIssueTracker(shopOrderMaterialIssueTracker);

                                /*********************************** End of Add data to Material Issue Tracker *************************/



                                if (issuedQty <= 0)
                                {
                                    break;
                                }
                            }
                        }
                    }

                }
                _inventoryRepository.SaveAsync();
                _inventoryTranctionRepository.SaveAsync();
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

        // public async Task<IActionResult> MaterialInventoryBalanceQty([FromBody] List<UpdateInventoryBalanceQty> updateInventoryBalanceQty)
        //{
        //     ServiceResponse<InventoryDto> serviceResponse = new ServiceResponse<InventoryDto>();
        //     try
        //     {
        //         var inventoryDetails = await _inventoryRepository
        //                 .GetInventoryDetailsByItemNoandProjectNo(dtoForMaterialIssue.PartNumber, dtoForMaterialIssue.ProjectNumber);

        //         if (inventoryDetails == null || inventoryDetails.Count == 0)
        //         {
        //             serviceResponse.Data = null;
        //             serviceResponse.Message = $"Inventory Details hasn't been found";
        //             serviceResponse.Success = false;
        //             serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //             _logger.LogError($"Inventory with itemNumber: {dtoForMaterialIssue.PartNumber}, is invalid");
        //             return NotFound(serviceResponse);
        //         }

        //         decimal issuedQty = dtoForMaterialIssue.IssueQty;
        //         string shopOrderNumber = dtoForMaterialIssue.ShopOrderNumber;
        //         for (int i = 0; i < inventoryDetails.Count; i++)
        //         {
        //             decimal balanceqty = inventoryDetails[i].Balance_Quantity;
        //             decimal lotNoWiseIssuedQty = 0;
        //             if (inventoryDetails[i].Balance_Quantity <= issuedQty)
        //             {

        //                 inventoryDetails[i].Warehouse = "WIP";
        //                 inventoryDetails[i].Location = "WIP";
        //                 inventoryDetails[i].shopOrderNo = shopOrderNumber;
        //                 inventoryDetails[i].IsStockAvailable = true;
        //                 lotNoWiseIssuedQty = balanceqty;
        //                 /** Dont Change the Position of IssuedQty and BalanceQty Code in this Method .it should be always last ***********************/
        //                 issuedQty -= balanceqty;
        //                 balanceqty = 0;


        //                 InventoryTranction inventoryTransaction = new InventoryTranction();
        //                 inventoryTransaction.PartNumber = inventoryDetails[i].PartNumber;
        //                 inventoryTransaction.MftrPartNumber = inventoryDetails[i].MftrPartNumber;
        //                 inventoryTransaction.LotNumber = inventoryDetails[i].LotNumber;
        //                 inventoryTransaction.Description = inventoryDetails[i].Description;
        //                 inventoryTransaction.PartType = inventoryDetails[i].PartType;
        //                 inventoryTransaction.ProjectNumber = inventoryDetails[i].ProjectNumber;
        //                 inventoryTransaction.Issued_Quantity = issuedQty;
        //                 inventoryTransaction.UOM = inventoryDetails[i].UOM;
        //                 inventoryTransaction.Issued_DateTime = DateTime.Now;
        //                 inventoryTransaction.ReferenceID = inventoryDetails[i].ReferenceID;
        //                 inventoryTransaction.ReferenceIDFrom = inventoryDetails[i].ReferenceIDFrom;
        //                 inventoryTransaction.BOM_Version_No = 0;
        //                 inventoryTransaction.From_Location = inventoryDetails[i].Location;
        //                 inventoryTransaction.TO_Location = "WIP";
        //                 inventoryTransaction.Unit = inventoryDetails[i].Unit;
        //                 inventoryTransaction.GrinMaterialType = inventoryDetails[i].GrinMaterialType;
        //                 inventoryTransaction.Remarks = "Issue Material";
        //                 inventoryTransaction.IsStockAvailable = inventoryDetails[i].IsStockAvailable;
        //                 inventoryTransaction.Warehouse = "WIP";
        //                 inventoryTransaction.GrinNo = inventoryDetails[i].GrinNo;
        //                 inventoryTransaction.GrinPartId = inventoryDetails[i].GrinPartId;
        //                 inventoryTransaction.shopOrderNo = shopOrderNumber;

        //                 await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransaction);


        //             }
        //             else
        //             {
        //                 inventoryDetails[i].Balance_Quantity -= issuedQty;

        //                 lotNoWiseIssuedQty = issuedQty;

        //                 Inventory wipInventory = InsertWipDetailsInInventoryWhenIssuedQtyIsMore(inventoryDetails[i], issuedQty, shopOrderNumber);

        //                 await _inventoryRepository.CreateInventory(wipInventory);


        //                 //Add Data to inventory transaction

        //                 InventoryTranction inventoryTransaction = new InventoryTranction();
        //                 inventoryTransaction.PartNumber = inventoryDetails[i].PartNumber;
        //                 inventoryTransaction.MftrPartNumber = inventoryDetails[i].MftrPartNumber;
        //                 inventoryTransaction.LotNumber = inventoryDetails[i].LotNumber;
        //                 inventoryTransaction.Description = inventoryDetails[i].Description;
        //                 inventoryTransaction.PartType = inventoryDetails[i].PartType;
        //                 inventoryTransaction.ProjectNumber = inventoryDetails[i].ProjectNumber;
        //                 inventoryTransaction.Issued_Quantity = lotNoWiseIssuedQty;
        //                 inventoryTransaction.UOM = inventoryDetails[i].UOM;
        //                 inventoryTransaction.Issued_DateTime = DateTime.Now;
        //                 inventoryTransaction.ReferenceID = inventoryDetails[i].ReferenceID;
        //                 inventoryTransaction.ReferenceIDFrom = inventoryDetails[i].ReferenceIDFrom;
        //                 inventoryTransaction.BOM_Version_No = 0;
        //                 inventoryTransaction.From_Location = inventoryDetails[i].Location;
        //                 inventoryTransaction.TO_Location = "WIP";
        //                 inventoryTransaction.Unit = inventoryDetails[i].Unit;
        //                 inventoryTransaction.GrinMaterialType = inventoryDetails[i].GrinMaterialType;
        //                 inventoryTransaction.Remarks = "Issue Material";
        //                 inventoryTransaction.IsStockAvailable = inventoryDetails[i].IsStockAvailable;
        //                 inventoryTransaction.Warehouse = "WIP";
        //                 inventoryTransaction.GrinNo = inventoryDetails[i].GrinNo;
        //                 inventoryTransaction.GrinPartId = inventoryDetails[i].GrinPartId;
        //                 inventoryTransaction.shopOrderNo = shopOrderNumber;

        //                 await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTransaction);



        //                 /******* Dont Change the Position of IssuedQty and BalanceQty 
        //                  * Code in this Method. it should be always last ***********************/

        //                 issuedQty = 0;
        //             }

        //             string result = await _inventoryRepository.UpdateInventory(inventoryDetails[i]);

        //             ///*********************************** Add data to Material Issue Tracker *************************/
        //             ShopOrderMaterialIssueTracker shopOrderMaterialIssueTracker = InsertDataToMaterialIssueTracker(dtoForMaterialIssue, inventoryDetails[i], lotNoWiseIssuedQty);
        //             int transactionId = await _materialIssueTrackerRepository.AddDataToMaterialIssueTracker(shopOrderMaterialIssueTracker);

        //             /*********************************** End of Add data to Material Issue Tracker *************************/

        //             if (issuedQty <= 0)
        //             {
        //                 break;
        //             }
        //         }

        //         _inventoryRepository.SaveAsync();
        //         _inventoryTranctionRepository.SaveAsync();
        //         _materialIssueTrackerRepository.SaveAsync();
        //         serviceResponse.Data = null;
        //         serviceResponse.Message = "Updated Successfully";
        //         serviceResponse.Success = true;
        //         serviceResponse.StatusCode = HttpStatusCode.OK;
        //         return Ok(serviceResponse);

        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError($"Invalid inventory action: {ex.Message},{ex.InnerException}");
        //         serviceResponse.Data = null;
        //         serviceResponse.Message = "Internal Server Error";
        //         serviceResponse.Success = false;
        //         serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //         return StatusCode(500, serviceResponse);
        //     }
        // }


        [HttpPost]
        public async Task<IActionResult> MaterialReturnNoteInventoryBalanceQty([FromBody] List<MRNUpdateInventoryBalanceQty> updateInventoryBalanceQty)
        {
            ServiceResponse<MRNIssueDetailsofMIandMR> serviceResponse = new ServiceResponse<MRNIssueDetailsofMIandMR>();
            try
            {
                MRNIssueDetailsofMIandMR mRNIssueDetailsofMIandMR = new MRNIssueDetailsofMIandMR();
                mRNIssueDetailsofMIandMR.ShopOrderNumber = updateInventoryBalanceQty[0].ShopOrderNumber;
                List<MIDetailsfromMRN>? mIDetailsfromMRN = new List<MIDetailsfromMRN>();
                List<MRDetailsfromMRN>? mRDetailsfromMRN1 = new List<MRDetailsfromMRN>();

                foreach (var materialReturnQty in updateInventoryBalanceQty)
                {
                    var itemNumber = materialReturnQty.PartNumber;
                    var projectNumber = materialReturnQty.ProjectNumber;
                    var shopOrderNumber = materialReturnQty.ShopOrderNumber;
                    var MRNNumber= materialReturnQty.MRNNumber;
                    foreach (var Location in materialReturnQty.MRNDetails)
                    {
                        if (Location.IsMRNIssueDone != true)
                        {
                            var materialWithLotNoFromIssueTracker = await _materialIssueTrackerRepository.GetWipQtyFromMaterialIssueTracker(materialReturnQty.ShopOrderNumber, itemNumber, Location.Qty);

                            foreach (var itemWithLotNo in materialWithLotNoFromIssueTracker)
                            {
                                if (itemWithLotNo.MaterialIssueData != null)
                                {
                                    var item = mIDetailsfromMRN?.Where(y => y.PartNumber == itemNumber).FirstOrDefault();
                                    if (item == null)
                                    {
                                        MIDetailsfromMRN? mIDetailsfromMRN3 = new MIDetailsfromMRN();
                                        mIDetailsfromMRN3.PartNumber = itemNumber;
                                        mIDetailsfromMRN3.QtyUsed = itemWithLotNo.MaterialIssueData.QtyUsed;
                                        mIDetailsfromMRN.Add(mIDetailsfromMRN3);
                                    }
                                    else item.QtyUsed += itemWithLotNo.MaterialIssueData.QtyUsed;
                                    var lotNumber = itemWithLotNo.LotNumber;
                                    var wipQtyInIssueTracker = itemWithLotNo.WipQty;
                                    var inventories = await _inventoryRepository.GetWipInventoryDetailsByLotNumber(itemNumber, lotNumber, shopOrderNumber);

                                    if (inventories != null || inventories.Count() > 0)
                                    {
                                        foreach (var inventoryDetail in inventories)
                                        {
                                            var wipQtyInventoryQty = inventoryDetail.Balance_Quantity;
                                            if (wipQtyInventoryQty > wipQtyInIssueTracker)
                                            {
                                                inventoryDetail.Balance_Quantity -= wipQtyInIssueTracker;
                                                await _inventoryRepository.UpdateInventory(inventoryDetail);
                                                _inventoryRepository.SaveAsync();

                                                InventoryTranction inventoryTranction = new InventoryTranction();
                                                inventoryTranction.PartNumber = inventoryDetail.PartNumber;
                                                inventoryTranction.MftrPartNumber = inventoryDetail.MftrPartNumber;
                                                inventoryTranction.LotNumber = inventoryDetail.LotNumber;
                                                inventoryTranction.Description = inventoryDetail.Description;
                                                inventoryTranction.PartType = inventoryDetail.PartType;
                                                inventoryTranction.ProjectNumber = inventoryDetail.ProjectNumber;
                                                inventoryTranction.Issued_Quantity = inventoryDetail.Balance_Quantity;
                                                inventoryTranction.UOM = inventoryDetail.UOM;
                                                inventoryTranction.Issued_DateTime = DateTime.Now;
                                                inventoryTranction.Issued_By = "";
                                                inventoryTranction.ShopOrderId = "";
                                                inventoryTranction.IsStockAvailable = inventoryDetail.IsStockAvailable;
                                                inventoryTranction.shopOrderNo = inventoryDetail.shopOrderNo;
                                                inventoryTranction.ReferenceID = inventoryDetail.ReferenceID;
                                                inventoryTranction.ReferenceIDFrom = inventoryDetail.ReferenceIDFrom;
                                                inventoryTranction.BOM_Version_No = 0;
                                                inventoryTranction.From_Location = inventoryDetail.Location;
                                                inventoryTranction.TO_Location = inventoryDetail.Location;
                                                inventoryTranction.Warehouse = inventoryDetail.Warehouse;
                                                inventoryTranction.Remarks = "Open Material Return Note";
                                                await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranction);

                                                Inventory inventoryPost = new Inventory();
                                                inventoryPost.PartNumber = inventoryDetail.PartNumber;
                                                inventoryPost.MftrPartNumber = inventoryDetail.MftrPartNumber;
                                                inventoryPost.ProjectNumber = inventoryDetail.ProjectNumber;
                                                inventoryPost.Description = inventoryDetail.Description;
                                                inventoryPost.Balance_Quantity = wipQtyInIssueTracker;
                                                inventoryPost.UOM = inventoryDetail.UOM;
                                                inventoryPost.Max = inventoryDetail.Max;
                                                inventoryPost.Min = inventoryDetail.Min;
                                                inventoryPost.GrinMaterialType = inventoryDetail.GrinMaterialType;
                                                inventoryPost.shopOrderNo = inventoryDetail.shopOrderNo;
                                                inventoryPost.Unit = inventoryDetail.Unit;
                                                inventoryPost.GrinNo = inventoryDetail.GrinNo;
                                                inventoryPost.GrinPartId = inventoryDetail.GrinPartId;
                                                inventoryPost.IsStockAvailable = true;
                                                inventoryPost.Warehouse = Location.Warehouse;
                                                inventoryPost.Location = Location.Location;
                                                inventoryPost.PartType = inventoryDetail.PartType;
                                                inventoryPost.ReferenceID = MRNNumber;
                                                inventoryPost.LotNumber = inventoryDetail.LotNumber;
                                                inventoryPost.ReferenceIDFrom = "Material Return Note";
                                                await _inventoryRepository.CreateInventory(inventoryPost);
                                                _inventoryRepository.SaveAsync();


                                                InventoryTranction inventoryTranction1 = new InventoryTranction();
                                                inventoryTranction1.PartNumber = inventoryPost.PartNumber;
                                                inventoryTranction1.MftrPartNumber = inventoryPost.MftrPartNumber;
                                                inventoryTranction1.LotNumber = inventoryPost.LotNumber;
                                                inventoryTranction1.Description = inventoryPost.Description;
                                                inventoryTranction1.PartType = inventoryPost.PartType;
                                                inventoryTranction1.ProjectNumber = inventoryPost.ProjectNumber;
                                                inventoryTranction1.Issued_Quantity = inventoryPost.Balance_Quantity;
                                                inventoryTranction1.UOM = inventoryPost.UOM;
                                                inventoryTranction1.Issued_DateTime = DateTime.Now;
                                                inventoryTranction1.Issued_By = inventoryPost.LastModifiedBy;
                                                inventoryTranction1.ShopOrderId = "";
                                                inventoryTranction1.IsStockAvailable = inventoryPost.IsStockAvailable;
                                                inventoryTranction1.shopOrderNo = inventoryPost.shopOrderNo;
                                                inventoryTranction1.ReferenceID = inventoryPost.ReferenceID;
                                                inventoryTranction1.ReferenceIDFrom = inventoryPost.ReferenceIDFrom;
                                                inventoryTranction1.BOM_Version_No = 0;
                                                inventoryTranction1.From_Location = inventoryPost.Location;
                                                inventoryTranction1.TO_Location = inventoryPost.Location;
                                                inventoryTranction1.Warehouse = inventoryPost.Warehouse;

                                                await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranction1);

                                                wipQtyInIssueTracker = 0;
                                            }
                                            else
                                            {
                                                inventoryDetail.Location = Location.Location;
                                                inventoryDetail.Warehouse = Location.Warehouse;
                                                inventoryDetail.ReferenceIDFrom = "Material Return Note";
                                                await _inventoryRepository.UpdateInventory(inventoryDetail);
                                                _inventoryRepository.SaveAsync();

                                                InventoryTranction inventoryTranction = new InventoryTranction();
                                                inventoryTranction.PartNumber = inventoryDetail.PartNumber;
                                                inventoryTranction.MftrPartNumber = inventoryDetail.MftrPartNumber;
                                                inventoryTranction.LotNumber = inventoryDetail.LotNumber;
                                                inventoryTranction.Description = inventoryDetail.Description;
                                                inventoryTranction.PartType = inventoryDetail.PartType;
                                                inventoryTranction.ProjectNumber = inventoryDetail.ProjectNumber;
                                                inventoryTranction.Issued_Quantity = inventoryDetail.Balance_Quantity;
                                                inventoryTranction.UOM = inventoryDetail.UOM;
                                                inventoryTranction.Issued_DateTime = DateTime.Now;
                                                inventoryTranction.Issued_By = inventoryDetail.LastModifiedBy;
                                                inventoryTranction.ShopOrderId = "";
                                                inventoryTranction.IsStockAvailable = inventoryDetail.IsStockAvailable;
                                                inventoryTranction.shopOrderNo = inventoryDetail.shopOrderNo;
                                                inventoryTranction.ReferenceID = inventoryDetail.ReferenceID;
                                                inventoryTranction.ReferenceIDFrom = inventoryDetail.ReferenceIDFrom;
                                                inventoryTranction.BOM_Version_No = 0;
                                                inventoryTranction.From_Location = inventoryDetail.Location;
                                                inventoryTranction.TO_Location = inventoryDetail.Location;
                                                inventoryTranction.Warehouse = inventoryDetail.Warehouse;

                                                await _inventoryTranctionRepository.Create(inventoryTranction);

                                                wipQtyInIssueTracker -= wipQtyInventoryQty;

                                            }


                                            if (wipQtyInIssueTracker <= 0)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (itemWithLotNo.MaterialRequestData != null)
                                {

                                    var mr = mRDetailsfromMRN1?.Where(x => x.MRNumber == itemWithLotNo.MaterialRequestData.MRNumber).FirstOrDefault();
                                    if (mr != null)
                                    {
                                        var item = mr.items?.Where(y => y.PartNumber == itemNumber).FirstOrDefault();
                                        if (item != null)
                                            item.QtyUsed += itemWithLotNo.MaterialRequestData.QtyUsed;

                                        else
                                        {
                                            MIDetailsfromMRN? mIDetailsfromMRN2 = new MIDetailsfromMRN();
                                            mIDetailsfromMRN2.PartNumber = itemNumber;
                                            mIDetailsfromMRN2.QtyUsed = itemWithLotNo.MaterialRequestData.QtyUsed;
                                            mr.items.Add(mIDetailsfromMRN2);
                                        }

                                    }
                                    else
                                    {
                                        MRDetailsfromMRN mRDetailsfromMRN = new MRDetailsfromMRN();
                                        mRDetailsfromMRN.MRNumber = itemWithLotNo.MaterialRequestData.MRNumber;
                                        mRDetailsfromMRN.items = new List<MIDetailsfromMRN>();
                                        MIDetailsfromMRN? mIDetailsfromMRN2 = new MIDetailsfromMRN();
                                        mIDetailsfromMRN2.PartNumber = itemNumber;
                                        mIDetailsfromMRN2.QtyUsed = itemWithLotNo.MaterialRequestData.QtyUsed;
                                        mRDetailsfromMRN.items.Add(mIDetailsfromMRN2);
                                        mRDetailsfromMRN1.Add(mRDetailsfromMRN);
                                    }

                                    var lotNumber = itemWithLotNo.LotNumber;
                                    var wipQtyInIssueTracker = itemWithLotNo.WipQty;
                                    var inventories = await _inventoryRepository.GetWipInventoryDetailsByLotNumberofMR(itemNumber, lotNumber, shopOrderNumber, itemWithLotNo.MaterialRequestData.MRNumber);

                                    if (inventories != null || inventories.Count() > 0)
                                    {
                                        foreach (var inventoryDetail in inventories)
                                        {
                                            var wipQtyInventoryQty = inventoryDetail.Balance_Quantity;
                                            if (wipQtyInventoryQty > wipQtyInIssueTracker)
                                            {
                                                inventoryDetail.Balance_Quantity -= wipQtyInIssueTracker;
                                                await _inventoryRepository.UpdateInventory(inventoryDetail);
                                                _inventoryRepository.SaveAsync();

                                                InventoryTranction inventoryTranction = new InventoryTranction();
                                                inventoryTranction.PartNumber = inventoryDetail.PartNumber;
                                                inventoryTranction.MftrPartNumber = inventoryDetail.MftrPartNumber;
                                                inventoryTranction.LotNumber = inventoryDetail.LotNumber;
                                                inventoryTranction.Description = inventoryDetail.Description;
                                                inventoryTranction.PartType = inventoryDetail.PartType;
                                                inventoryTranction.ProjectNumber = inventoryDetail.ProjectNumber;
                                                inventoryTranction.Issued_Quantity = inventoryDetail.Balance_Quantity;
                                                inventoryTranction.UOM = inventoryDetail.UOM;
                                                inventoryTranction.Issued_DateTime = DateTime.Now;
                                                inventoryTranction.Issued_By = "";
                                                inventoryTranction.ShopOrderId = "";
                                                inventoryTranction.IsStockAvailable = inventoryDetail.IsStockAvailable;
                                                inventoryTranction.shopOrderNo = inventoryDetail.shopOrderNo;
                                                inventoryTranction.ReferenceID = inventoryDetail.ReferenceID;
                                                inventoryTranction.ReferenceIDFrom = inventoryDetail.ReferenceIDFrom;
                                                inventoryTranction.BOM_Version_No = 0;
                                                inventoryTranction.From_Location = inventoryDetail.Location;
                                                inventoryTranction.TO_Location = inventoryDetail.Location;
                                                inventoryTranction.Warehouse = inventoryDetail.Warehouse;
                                                inventoryTranction.Remarks = "Open Material Return Note";
                                                await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranction);

                                                Inventory inventoryPost = new Inventory();
                                                inventoryPost.PartNumber = inventoryDetail.PartNumber;
                                                inventoryPost.MftrPartNumber = inventoryDetail.MftrPartNumber;
                                                inventoryPost.ProjectNumber = inventoryDetail.ProjectNumber;
                                                inventoryPost.Description = inventoryDetail.Description;
                                                inventoryPost.Balance_Quantity = wipQtyInIssueTracker;
                                                inventoryPost.UOM = inventoryDetail.UOM;
                                                inventoryPost.Max = inventoryDetail.Max;
                                                inventoryPost.Min = inventoryDetail.Min;
                                                inventoryPost.GrinMaterialType = inventoryDetail.GrinMaterialType;
                                                inventoryPost.shopOrderNo = inventoryDetail.shopOrderNo;
                                                inventoryPost.Unit = inventoryDetail.Unit;
                                                inventoryPost.GrinNo = inventoryDetail.GrinNo;
                                                inventoryPost.GrinPartId = inventoryDetail.GrinPartId;
                                                inventoryPost.IsStockAvailable = true;
                                                inventoryPost.Warehouse = Location.Warehouse;
                                                inventoryPost.Location = Location.Location;
                                                inventoryPost.PartType = inventoryDetail.PartType;
                                                inventoryPost.ReferenceID = MRNNumber;
                                                inventoryPost.LotNumber = inventoryDetail.LotNumber;
                                                inventoryPost.ReferenceIDFrom = "Material Return Note";
                                                await _inventoryRepository.CreateInventory(inventoryPost);
                                                _inventoryRepository.SaveAsync();


                                                InventoryTranction inventoryTranction1 = new InventoryTranction();
                                                inventoryTranction1.PartNumber = inventoryPost.PartNumber;
                                                inventoryTranction1.MftrPartNumber = inventoryPost.MftrPartNumber;
                                                inventoryTranction1.LotNumber = inventoryPost.LotNumber;
                                                inventoryTranction1.Description = inventoryPost.Description;
                                                inventoryTranction1.PartType = inventoryPost.PartType;
                                                inventoryTranction1.ProjectNumber = inventoryPost.ProjectNumber;
                                                inventoryTranction1.Issued_Quantity = inventoryPost.Balance_Quantity;
                                                inventoryTranction1.UOM = inventoryPost.UOM;
                                                inventoryTranction1.Issued_DateTime = DateTime.Now;
                                                inventoryTranction1.Issued_By = inventoryPost.LastModifiedBy;
                                                inventoryTranction1.ShopOrderId = "";
                                                inventoryTranction1.IsStockAvailable = inventoryPost.IsStockAvailable;
                                                inventoryTranction1.shopOrderNo = inventoryPost.shopOrderNo;
                                                inventoryTranction1.ReferenceID = inventoryPost.ReferenceID;
                                                inventoryTranction1.ReferenceIDFrom = inventoryPost.ReferenceIDFrom;
                                                inventoryTranction1.BOM_Version_No = 0;
                                                inventoryTranction1.From_Location = inventoryPost.Location;
                                                inventoryTranction1.TO_Location = inventoryPost.Location;
                                                inventoryTranction1.Warehouse = inventoryPost.Warehouse;

                                                await _inventoryTranctionRepository.CreateInventoryTransaction(inventoryTranction1);

                                                wipQtyInIssueTracker = 0;
                                            }
                                            else
                                            {
                                                inventoryDetail.Location = Location.Location;
                                                inventoryDetail.Warehouse = Location.Warehouse;
                                                inventoryDetail.ReferenceIDFrom = "Material Return Note";
                                                await _inventoryRepository.UpdateInventory(inventoryDetail);
                                                _inventoryRepository.SaveAsync();

                                                InventoryTranction inventoryTranction = new InventoryTranction();
                                                inventoryTranction.PartNumber = inventoryDetail.PartNumber;
                                                inventoryTranction.MftrPartNumber = inventoryDetail.MftrPartNumber;
                                                inventoryTranction.LotNumber = inventoryDetail.LotNumber;
                                                inventoryTranction.Description = inventoryDetail.Description;
                                                inventoryTranction.PartType = inventoryDetail.PartType;
                                                inventoryTranction.ProjectNumber = inventoryDetail.ProjectNumber;
                                                inventoryTranction.Issued_Quantity = inventoryDetail.Balance_Quantity;
                                                inventoryTranction.UOM = inventoryDetail.UOM;
                                                inventoryTranction.Issued_DateTime = DateTime.Now;
                                                inventoryTranction.Issued_By = inventoryDetail.LastModifiedBy;
                                                inventoryTranction.ShopOrderId = "";
                                                inventoryTranction.IsStockAvailable = inventoryDetail.IsStockAvailable;
                                                inventoryTranction.shopOrderNo = inventoryDetail.shopOrderNo;
                                                inventoryTranction.ReferenceID = inventoryDetail.ReferenceID;
                                                inventoryTranction.ReferenceIDFrom = inventoryDetail.ReferenceIDFrom;
                                                inventoryTranction.BOM_Version_No = 0;
                                                inventoryTranction.From_Location = inventoryDetail.Location;
                                                inventoryTranction.TO_Location = inventoryDetail.Location;
                                                inventoryTranction.Warehouse = inventoryDetail.Warehouse;

                                                await _inventoryTranctionRepository.Create(inventoryTranction);

                                                wipQtyInIssueTracker -= wipQtyInventoryQty;

                                            }


                                            if (wipQtyInIssueTracker <= 0)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                            }

                        }
                    }
                }
                mRNIssueDetailsofMIandMR.mIDetailsfromMRN = mIDetailsfromMRN;
                mRNIssueDetailsofMIandMR.mRDetailsfromMRN = mRDetailsfromMRN1;
                _inventoryRepository.SaveAsync();
                _inventoryTranctionRepository.SaveAsync();

                serviceResponse.Data = mRNIssueDetailsofMIandMR;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside MaterialReturnNoteInventoryBalanceQty action: {ex.Message},{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

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
        public async Task<IActionResult> GetRandomInventoryItemDetails()
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var inventoryDetails = await _inventoryRepository.GetRandomInventoryItemDetails();

                _logger.LogInfo("Returned all Inventory");

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
                return Ok(serviceResponse);
                //return Created("GetInventoryById", serviceResponse);
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

        [HttpPost]
        public async Task<IActionResult> CreateInventoryForOqcBinning([FromBody] InventoryOqcBinningPostDto inventoryDtoPost)
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
                return Ok(serviceResponse);
                //return Created("GetInventoryById", serviceResponse);
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
                if (updateInventory.Balance_Quantity == 0) updateInventory.IsStockAvailable = false;
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
                    Inventory inventoryDetails = await _inventoryRepository.GetInventoryDetailsByItemNoProjectNoUnitWarehouseAndLocation(item.PartNumber, projectNumber, unit, warehouse.Warehouse, warehouse.Location);
                    if (inventoryDetails != null)
                    {
                        inventoryDetails.Balance_Quantity += item.ReturnQty;
                        inventoryDetails.IsStockAvailable = true;
                        await _inventoryRepository.UpdateInventory(inventoryDetails);
                    }
                    else
                    {
                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();
                        var encodedItemNumber = Uri.EscapeDataString(item.PartNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterObjectResult = await client.SendAsync(request);
                        //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterAPI"],
                        //                                                      "GetItemMasterDetailsForMNRByItemNo?", "&ItemNumber=",item.PartNumber));
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

        [HttpGet]
        public async Task<ActionResult<decimal>> GetTotalStockOfItemNumber(string saItemNumber)
        {
            var inventoryServiceResponse = new ServiceResponse<decimal>();

            try
            {
                decimal stockAvailable = await _inventoryRepository.GetTotalStockOfItemNumber(saItemNumber);

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
        [HttpGet]
        public async Task<ActionResult<decimal>> GetTotalStockOfSAItemNumberAndProjectNo(string saItemNumber, string projectNumber)
        {
            var inventoryServiceResponse = new ServiceResponse<decimal>();

            try
            {
                decimal stockAvailable = await _inventoryRepository.GetTotalStockOfSAItemNumberAndProjectNo(saItemNumber, projectNumber);

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

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetCrossMarginSPReportsWithParam([FromBody] CrossMarginSPReportDto crossMarginSPReportDto)
        {
            ServiceResponse<IEnumerable<CrossMarginSPReport>> serviceResponse = new ServiceResponse<IEnumerable<CrossMarginSPReport>>();
            try
            {
                var products = await _inventoryRepository.GetCrossMarginSPReportsWithParam(crossMarginSPReportDto.CustomerId, crossMarginSPReportDto.CustomerName);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CrossMarginSPReportsWithParam hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"CrossMarginSPReportsWithParam hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned CrossMarginSPReportsWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCrossMarginSPReportsWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetStockMovementSPReports()
        {
            ServiceResponse<IEnumerable<StockMovementSPReport>> serviceResponse = new ServiceResponse<IEnumerable<StockMovementSPReport>>();
            try
            {
                var products = await _inventoryRepository.GetStockMovementSPReports();

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"StockMovementSPReportsWithParam hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"StockMovementSPReportsWithParam hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned StockMovementSPReportsWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetStockMovementSPReports action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetStockMovementLatestSPReports()
        {
            ServiceResponse<IEnumerable<StockMovementLatestSPReport>> serviceResponse = new ServiceResponse<IEnumerable<StockMovementLatestSPReport>>();
            try
            {
                var products = await _inventoryRepository.GetStockMovementLatestSPReports();

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GetStockMovementLatestSPReports hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"GetStockMovementLatestSPReports hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned StockMovementLatestSPReports Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetStockMovementLatestSPReports action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetStockMovementHistorySPReportsWithDate(StockMovementHistorySPReportDto stockMovementHistorySPReportDto)
        {
            ServiceResponse<IEnumerable<StockMovementHistorySPReport>> serviceResponse = new ServiceResponse<IEnumerable<StockMovementHistorySPReport>>();
            try
            {
                var products = await _inventoryRepository.GetStockMovementHistorySPReportsWithDate(stockMovementHistorySPReportDto.FromDate, stockMovementHistorySPReportDto.ToDate,
                                                                                                                    stockMovementHistorySPReportDto.ItemNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"StockMovementHistorySPReportsWithDate hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"StockMovementHistorySPReportsWithDate hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned StockMovementHistorySPReportsWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetStockMovementHistorySPReportsWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetTrascationKPNWSPReportsWithParam([FromBody] TrascationKPNWSPReportsDto trascationKPNWSPReportsDto)
        {
            ServiceResponse<IEnumerable<TrascationKPNWSPReport>> serviceResponse = new ServiceResponse<IEnumerable<TrascationKPNWSPReport>>();
            try
            {
                var products = await _inventoryRepository.GetTrascationKPNWSPReportsWithParam(trascationKPNWSPReportsDto.KPN);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"TrascationKPNWSPReports hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"TrascationKPNWSPReports hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned TrascationKPNWSPReports Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetTrascationKPNWSPReportsWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetInventoryBySumOfFilteringDatesSPReportsWithParam([FromBody] InventoryBySumOfFilteringDatesSPReportDto inventoryBySumOfFilteringDatesSPReportDto)
        {
            ServiceResponse<IEnumerable<InventoryBySumOfFilteringDatesSPReport>> serviceResponse = new ServiceResponse<IEnumerable<InventoryBySumOfFilteringDatesSPReport>>();
            try
            {
                var products = await _inventoryRepository.GetInventoryBySumOfFilteringDatesSPReportsWithParam(inventoryBySumOfFilteringDatesSPReportDto.FromDate, inventoryBySumOfFilteringDatesSPReportDto.ToDate,
                                                                                        inventoryBySumOfFilteringDatesSPReportDto.PartNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryBySumOfFilteringDatesSPReports hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"InventoryBySumOfFilteringDatesSPReports hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned InventoryBySumOfFilteringDatesSPReports Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetInventoryBySumOfFilteringDatesSPReportsWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetInventorySPReportsWithParam([FromBody] InventorySPReportDto inventorySPReportDto)
        {
            ServiceResponse<IEnumerable<InventorySPReport>> serviceResponse = new ServiceResponse<IEnumerable<InventorySPReport>>();
            try
            {
                var products = await _inventoryRepository.GetInventorySPReportsWithParam(inventorySPReportDto.PartNumber, inventorySPReportDto.Description,
                                                                                    inventorySPReportDto.Warehouse, inventorySPReportDto.Location,
                                                                                    inventorySPReportDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventorySPReportsWithParam hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"InventorySPReportsWithParam hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned InventorySPReportsWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetInventorySPReportsWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> InventorySPReportdate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<InventorySPReport>> serviceResponse = new ServiceResponse<IEnumerable<InventorySPReport>>();
            try
            {
                var products = await _inventoryRepository.InventorySPReportdate(FromDate, ToDate);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventorySPReportdate hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"InventorySPReportdate hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned InventorySPReportdate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside InventorySPReportdate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetInventorySPReportForAvision([FromBody] GetInventorySPReportForAviDto getInventorySPReportForAviDto)
        {
            ServiceResponse<IEnumerable<GetInventorySPReportForAvi>> serviceResponse = new ServiceResponse<IEnumerable<GetInventorySPReportForAvi>>();
            try
            {
                var products = await _inventoryRepository.GetInventorySPReportForAvision(getInventorySPReportForAviDto.FromDate, getInventorySPReportForAviDto.ToDate,
                                                                                        getInventorySPReportForAviDto.PartNumber, getInventorySPReportForAviDto.ProjectNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventorySPReportForAvision hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"InventorySPReportForAvision hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned InventorySPReportForAvision Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetInventorySPReportForAvision action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetInventoryForStockSPReportsWithParam([FromBody] InventoryForStockSPReportDto inventoryForStockSPReportDto)
        {
            ServiceResponse<IEnumerable<InventoryForStockSPReport>> serviceResponse = new ServiceResponse<IEnumerable<InventoryForStockSPReport>>();
            try
            {
                var products = await _inventoryRepository.GetInventoryForStockSPReportsWithParam(inventoryForStockSPReportDto.PartNumber,
                                                                                    inventoryForStockSPReportDto.Warehouse, inventoryForStockSPReportDto.Location);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryForStockSPReportsWithParam hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"InventoryForStockSPReportsWithParam hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned InventoryForStockSPReportsWithParam Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetInventoryForStockSPReportsWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetInventoryWarehouseReport([FromBody] InventorySPReportDto inventoryWarehouseReportDto)
        {
            ServiceResponse<IEnumerable<InventorySPReport>> serviceResponse = new ServiceResponse<IEnumerable<InventorySPReport>>();
            try
            {
                var inv = await _inventoryRepository.GetInventoryWarehouseReport(inventoryWarehouseReportDto.PartNumber, inventoryWarehouseReportDto.Description,
                                                                                    inventoryWarehouseReportDto.Warehouse, inventoryWarehouseReportDto.Location,
                                                                                    inventoryWarehouseReportDto.ProjectNumber);
                var products = _mapper.Map<List<InventorySPReport>>(inv);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryWarehouseReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"InventoryWarehouseReport hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned InventoryWarehouseReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetInventoryWarehouseReport action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetInventoryWIPReport([FromBody] InventorySPReportDto inventoryWarehouseReportDto)
        {
            ServiceResponse<IEnumerable<InventorySPReport>> serviceResponse = new ServiceResponse<IEnumerable<InventorySPReport>>();
            try
            {
                var inv = await _inventoryRepository.GetInventoryWIPReport(inventoryWarehouseReportDto.PartNumber, inventoryWarehouseReportDto.Description,
                                                                                    inventoryWarehouseReportDto.ProjectNumber);
                var products = _mapper.Map<List<InventorySPReport>>(inv);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryWarehouseReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"InventoryWarehouseReport hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned InventoryWarehouseReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetInventoryWarehouseReport action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetInventoryGrinAndIqcReport([FromBody] InventorySPReportDto inventoryWarehouseReportDto)
        {
            ServiceResponse<IEnumerable<InventorySPReport>> serviceResponse = new ServiceResponse<IEnumerable<InventorySPReport>>();
            try
            {
                var inv = await _inventoryRepository.GetInventoryGrinAndIqcReport(inventoryWarehouseReportDto.PartNumber, inventoryWarehouseReportDto.Description,
                                                                                    inventoryWarehouseReportDto.ProjectNumber, inventoryWarehouseReportDto.Warehouse,
                                                                                    inventoryWarehouseReportDto.Location);
                var products = _mapper.Map<List<InventorySPReport>>(inv);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryGrinAndIqcReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"InventoryGrinAndIqcReport hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned InventoryGrinAndIqcReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetInventoryGrinAndIqcReport action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetInventoryOpenGrinForGrinAndOpenGrinForIQCReport([FromBody] InventorySPReportDto inventoryWarehouseReportDto)
        {
            ServiceResponse<IEnumerable<InventorySPReport>> serviceResponse = new ServiceResponse<IEnumerable<InventorySPReport>>();
            try
            {
                var inv = await _inventoryRepository.GetInventoryOpenGrinForGrinAndOpenGrinForIQCReport(inventoryWarehouseReportDto.PartNumber, inventoryWarehouseReportDto.Description,
                                                                                    inventoryWarehouseReportDto.ProjectNumber, inventoryWarehouseReportDto.Warehouse,
                                                                                    inventoryWarehouseReportDto.Location);
                var products = _mapper.Map<List<InventorySPReport>>(inv);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryOpenGrinForGrinAndOpenGrinForIQCReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"InventoryOpenGrinForGrinAndOpenGrinForIQCReport hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned InventoryOpenGrinForGrinAndOpenGrinForIQCReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetInventoryOpenGrinForGrinAndOpenGrinForIQCReport action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetInventoryNotUseableReport([FromBody] InventorySPReportDto inventoryWarehouseReportDto)
        {
            ServiceResponse<IEnumerable<InventorySPReport>> serviceResponse = new ServiceResponse<IEnumerable<InventorySPReport>>();
            try
            {
                var inv = await _inventoryRepository.GetInventoryNotUseableReport(inventoryWarehouseReportDto.PartNumber, inventoryWarehouseReportDto.Description,
                                                                                    inventoryWarehouseReportDto.ProjectNumber, inventoryWarehouseReportDto.Warehouse,
                                                                                    inventoryWarehouseReportDto.Location);
                var products = _mapper.Map<List<InventorySPReport>>(inv);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryNotUseableReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"InventoryNotUseableReport hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned InventoryNotUseableReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetInventoryNotUseableReport action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
