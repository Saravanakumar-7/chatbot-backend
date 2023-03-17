using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.Warehouse.Api.Contracts;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InventoryTranctionController : ControllerBase
    {       
            private IInventoryTranctionRepository _inventoryTranctionRepository;
            private ILoggerManager _logger;
            private IMapper _mapper;

            public InventoryTranctionController(IInventoryTranctionRepository inventoryTranctionRepository, ILoggerManager logger, IMapper mapper)
            {
                _inventoryTranctionRepository = inventoryTranctionRepository;
                _logger = logger;
                _mapper = mapper;
            }

            // GET: api/<InventoryTranctionController>
            [HttpGet]
            public async Task<IActionResult> GetAllInventoryTranctions([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
            {
                ServiceResponse<IEnumerable<InventoryTranctionDto>> serviceResponse = new ServiceResponse<IEnumerable<InventoryTranctionDto>>();
                try
                {
                    var getAllInventoryTranctions = await _inventoryTranctionRepository.GetAllInventoryTranction(pagingParameter, searchParams);
                    var metadata = new
                    {
                        getAllInventoryTranctions.TotalCount,
                        getAllInventoryTranctions.PageSize,
                        getAllInventoryTranctions.CurrentPage,
                        getAllInventoryTranctions.HasNext,
                        getAllInventoryTranctions.HasPreviuos
                    };

                    Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                    _logger.LogInfo("Returned all InventoryTrancton");
                    var result = _mapper.Map<IEnumerable<InventoryTranctionDto>>(getAllInventoryTranctions);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned all InventoryTrancton";
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

            // GET api/<InventoryTranctionController>/5
            [HttpGet("{id}")]
            public async Task<IActionResult> GetInventoryTranctionById(int id)
            {
                ServiceResponse<InventoryTranctionDto> serviceResponse = new ServiceResponse<InventoryTranctionDto>();

                try
                {
                    var getInventoryTranctionById = await _inventoryTranctionRepository.GetInventoryTranctionById(id);
                    if (getInventoryTranctionById == null)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"InventoryTranction with id: {id}, hasn't been found in db.";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        _logger.LogError($"InventoryTranction with id: {id}, hasn't been found in db.");
                        return NotFound(serviceResponse);
                    }
                    else
                    {
                        _logger.LogInfo($"Returned InventoryTranction with id: {id}");
                        var result = _mapper.Map<InventoryTranctionDto>(getInventoryTranctionById);
                        serviceResponse.Data = result;
                        serviceResponse.Message = "Returned InventoryTranction with id Successfully";
                        serviceResponse.Success = true;
                        serviceResponse.StatusCode = HttpStatusCode.OK;
                        return Ok(serviceResponse);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Something went wrong inside GetInventoryTranctionById action: {ex.Message}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Something went wrong. Please try again!";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
            }

            // POST api/<InventoryTranctionController>
            [HttpPost]
            public IActionResult CreateInventoryTranction([FromBody] InventoryTranctionDtoPost inventoryTranctionDtoPost)
            {
                ServiceResponse<InventoryTranctionDto> serviceResponse = new ServiceResponse<InventoryTranctionDto>();

                try
                {
                    if (inventoryTranctionDtoPost is null)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = "InventoryTranction object sent from client is null";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        _logger.LogError("InventoryTranction object sent from client is null.");
                        return BadRequest(serviceResponse);
                    }
                    if (!ModelState.IsValid)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Invalid InventoryTranction object sent from client";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        _logger.LogError("Invalid InventoryTranction object sent from client.");
                        return BadRequest(serviceResponse);
                    }
                    var createInventoryTranction = _mapper.Map<InventoryTranction>(inventoryTranctionDtoPost);

                    _inventoryTranctionRepository.CreateInventoryTransaction(createInventoryTranction);
                    _inventoryTranctionRepository.SaveAsync();
                    serviceResponse.Data = null;
                    serviceResponse.Message = "MaterialIssue Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;

                    return Created("GetCommodityById", serviceResponse);
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

            // PUT api/<InventoryTranctionController>/5
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateInventoryTranction(int id, [FromBody] InventoryTranctionDtoUpdate inventoryTranctionDtoUpdate)
            {
                ServiceResponse<InventoryTranctionDto> serviceResponse = new ServiceResponse<InventoryTranctionDto>();

                try
                {
                    if (inventoryTranctionDtoUpdate is null)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = "InventoryTranction object sent from client is null";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        _logger.LogError("InventoryTranction object sent from client is null.");
                        return BadRequest(serviceResponse);
                    }
                    if (!ModelState.IsValid)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Invalid InventoryTranction object sent from client";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        _logger.LogError("Invalid InventoryTranction object sent from client.");
                        return BadRequest(serviceResponse);
                    }
                    var getInventoryTranctionById = await _inventoryTranctionRepository.GetInventoryTranctionById(id);
                    if (getInventoryTranctionById is null)
                    {
                        _logger.LogError($"InventoryTranction with id: {id}, hasn't been found in db.");
                        serviceResponse.Data = null;
                        serviceResponse.Message = " Update InventoryTranction with id: {id}, hasn't been found in db.";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(serviceResponse);
                    }
                    var updateInventoryTranction = _mapper.Map(inventoryTranctionDtoUpdate, getInventoryTranctionById);
                    _mapper.Map(inventoryTranctionDtoUpdate, getInventoryTranctionById);
                    string result = await _inventoryTranctionRepository.UpdateInventoryTraction(updateInventoryTranction);
                    _logger.LogInfo(result);
                    _inventoryTranctionRepository.SaveAsync();
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

            // DELETE api/<InventoryTranctionController>/5
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteInventoryTranction(int id)
            {
                ServiceResponse<InventoryTranctionDto> serviceResponse = new ServiceResponse<InventoryTranctionDto>();

                try
                {
                    var getMaterialIssueById = await _inventoryTranctionRepository.GetInventoryTranctionById(id);
                    if (getMaterialIssueById == null)
                    {
                        _logger.LogError($"Delete InventoryTranction with id: {id}, hasn't been found in db.");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Delete InventoryTranction with id hasn't been found in db.";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound(serviceResponse);
                    }
                    string result = await _inventoryTranctionRepository.DeleteInventoryTranction(getMaterialIssueById);
                    _logger.LogInfo(result);
                    _inventoryTranctionRepository.SaveAsync();
                    serviceResponse.Data = null;
                    serviceResponse.Message = "InventoryTranction Deleted Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Something went wrong inside DeleteInventoryTranction action: {ex.Message}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Internal server error";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
            }
        
    }
}
