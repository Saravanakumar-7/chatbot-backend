using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;


namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private IInventoryRepository _inventoryRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public InventoryController(IInventoryRepository inventoryRepository, ILoggerManager logger, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
            _mapper = mapper;
        }


        // GET: api/<InventoryController>
        [HttpGet]
        public async Task<IActionResult> GetAllInventory([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<InventoryDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryDto>>();
            try
            {
                var getAllInventory = await _inventoryRepository.GetAllInventory(pagingParameter);
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
        //passing project 
         

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

        [HttpPost]
        public IActionResult CreateInventory([FromBody] InventoryDtoPost inventoryDtoPost)
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
        [HttpPut("{id}")]
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
    
    }
}
