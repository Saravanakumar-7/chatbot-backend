using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Repository;

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class InventoryForServiceItemsController : ControllerBase
    {
        private IInventoryForServiceItemsRepository _inventoryForServiceItemsRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public InventoryForServiceItemsController(IInventoryForServiceItemsRepository inventoryForServiceItemsRepository,IConfiguration config,HttpClient httpClient,IHttpClientFactory clientFactory, ILoggerManager logger, IMapper mapper)
        {
            _inventoryForServiceItemsRepository = inventoryForServiceItemsRepository;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
        }
        [HttpPost]
        public async Task<IActionResult> CreateInventoryForServiceItemsFromGrin([FromBody] InventoryForServiceItemsGrinDtoPost inventoryforServiceItemsDto)
        {
            ServiceResponse<InventoryForServiceItemsDto> serviceResponse = new ServiceResponse<InventoryForServiceItemsDto>();

            try
            {
                if (inventoryforServiceItemsDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "InventoryForServiceItems object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("InventoryForServiceItems object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid InventoryForServiceItems object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid InventoryForServiceItems object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var createInvetory = _mapper.Map<InventoryForServiceItems>(inventoryforServiceItemsDto);
                createInvetory.IsStockAvailable = true;
                await _inventoryForServiceItemsRepository.CreateInventoryForServiceItems(createInvetory);
                _inventoryForServiceItemsRepository.SaveAsync();
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
                _logger.LogError($"Something went wrong inside CreateInventoryForServiceItemsFromGrin action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetInventoryForServiceDetailsByGrinNoandGrinId(string GrinNo, int GrinPartsId, string ItemNumber, string ProjectNumber)

        {
            ServiceResponse<InventoryForServiceItemsDto> serviceResponse = new ServiceResponse<InventoryForServiceItemsDto>();

            try
            {
                var getInventoryDetailsByGrinNoandGrinId = await _inventoryForServiceItemsRepository.GetInventoryForServiceDetailsByGrinNoandGrinId(GrinNo, GrinPartsId, ItemNumber, ProjectNumber);
                if (getInventoryDetailsByGrinNoandGrinId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"InventoryForService with id: {GrinNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"InventoryForService with id: {GrinNo}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned InventoryForService with id: {GrinNo}");
                    var result = _mapper.Map<InventoryForServiceItemsDto>(getInventoryDetailsByGrinNoandGrinId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Inventory with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetInventoryForServiceDetailsByGrinNoandGrinId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateInventoryForServiceDetails(int id, [FromBody] InventoryForServiceItemsDtoUpdate inventoryDtoUpdate)
        {
            ServiceResponse<InventoryForServiceItemsDto> serviceResponse = new ServiceResponse<InventoryForServiceItemsDto>();

            try
            {
                if (inventoryDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "InventoryForService object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("InventoryForService object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid InventoryForService object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid InventoryForService object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var getInventoryById = await _inventoryForServiceItemsRepository.GetInventoryForServiceItemsById(id);
                if (getInventoryById is null)
                {
                    _logger.LogError($"InventoryForService with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update InventoryForService with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateInventory = _mapper.Map(inventoryDtoUpdate, getInventoryById);
                if (updateInventory.Balance_Quantity == 0) updateInventory.IsStockAvailable = false;
                string result = await _inventoryForServiceItemsRepository.UpdateInventoryForServiceItems(updateInventory);
                _logger.LogInfo(result);
                _inventoryForServiceItemsRepository.SaveAsync();
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
                _logger.LogError($"Something went wrong inside UpdateInventoryForServiceDetails action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
