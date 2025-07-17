using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Warehouse.Api.Entities.DTOs;
using Tips.Warehouse.Api.Entities;
using Tips.Warehouse.Api.Contracts;

namespace Tips.Warehouse.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class InventoryTranctionforServiceItemsController : ControllerBase
    {
        private IInventoryTranctionforServiceItemsRepository _inventoryTranctionforServiceItemsRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public InventoryTranctionforServiceItemsController(IInventoryTranctionforServiceItemsRepository inventoryTranctionforServiceItemsRepository,ILoggerManager logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _inventoryTranctionforServiceItemsRepository = inventoryTranctionforServiceItemsRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateInventoryTranctionforServiceItems([FromBody] InventoryTranctionforServiceItemsPostDto inventoryTranctionforServiceItemsPostDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();

            try
            {
                if (inventoryTranctionforServiceItemsPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "InventoryTranctionforServiceItems object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("InventoryTranctionforServiceItems object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid InventoryTranctionforServiceItems object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid InventoryTranctionforServiceItems object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var createInventoryTranctionforServiceItems = _mapper.Map<InventoryTranctionforServiceItems>(inventoryTranctionforServiceItemsPostDto);

                await _inventoryTranctionforServiceItemsRepository.CreateInventoryTranctionforServiceItems(createInventoryTranctionforServiceItems);
                _inventoryTranctionforServiceItemsRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "InventoryTranctionforServiceItems was Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                _logger.LogInfo($"InventoryTranctionforServiceItems was Successfully Created");
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error occured in CreateInventoryTranctionforServiceItems:\n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error occured in CreateInventoryTranctionforServiceItems:\n{ex.Message}\n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
