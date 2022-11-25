using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemMasterController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public ItemMasterController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<ItemMasterController>
        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();

            try
            {
                var ItemMasterList = await _repository.ItemMasterRepository.GetAllItems(pagingParameter);
                _logger.LogInfo("Returned all ItemMasters");
                var metadata = new
                {
                    ItemMasterList.TotalCount,
                    ItemMasterList.PageSize,
                    ItemMasterList.CurrentPage,
                    ItemMasterList.HasNext,
                    ItemMasterList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(ItemMasterList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ItemMasters Successfully";
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

        // GET api/<ItemMasterController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var itemMaster = await _repository.ItemMasterRepository.GetItemById(id);
                if (itemMaster == null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ItemMaster with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ItemMaster with id: {id}");
                    var result = _mapper.Map<ItemMasterDto>(itemMaster);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned ItemMaster with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
             }
        }

        // POST api/<ItemMasterController>
        [HttpPost]
        public IActionResult CreateItemMaster([FromBody] ItemMasterDtoPost itemMasterDtoPost)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                if (itemMasterDtoPost is null)
                {
                    _logger.LogError("ItemMaster object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ItemMaster object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var itemMasterEntity = _mapper.Map<ItemMaster>(itemMasterDtoPost);
                var itemMasterAlternate = _mapper.Map<IEnumerable<ItemmasterAlternate>>(itemMasterDtoPost.ItemmasterAlternate);
                var itemMasterApprovedVendor = _mapper.Map<IEnumerable<ItemMasterApprovedVendor>>(itemMasterDtoPost.ItemMasterApprovedVendor);
                var itemMasterFileUpload = _mapper.Map<IEnumerable<ItemMasterFileUpload>>(itemMasterDtoPost.ItemMasterFileUpload);
                var itemMasterRouting=_mapper.Map<IEnumerable<ItemMasterRouting>>(itemMasterDtoPost.ItemMasterRouting);
                var itemMasterWarehouse = _mapper.Map<IEnumerable<ItemMasterWarehouse>>(itemMasterDtoPost.ItemMasterWarehouse);
                _repository.ItemMasterRepository.CreateItem(itemMasterEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetItemById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemMaster(int id, [FromBody] ItemMasterDto itemMasterDtoUpdate)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                if (itemMasterDtoUpdate is null)
                {
                    _logger.LogError("ItemMaster object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Vendor object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ItemMaster object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var itemMasterEntity = await _repository.ItemMasterRepository.GetItemById(id);
                if (itemMasterEntity is null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemMaster hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                 }
                 var itemMasterAlternate = _mapper.Map<IEnumerable<ItemmasterAlternate>>(itemMasterDtoUpdate.ItemmasterAlternate);
                var itemMasterApprovedVendor = _mapper.Map<IEnumerable<ItemMasterApprovedVendor>>(itemMasterDtoUpdate.ItemMasterApprovedVendor);
                var itemMasterFileUpload = _mapper.Map<IEnumerable<ItemMasterFileUpload>>(itemMasterDtoUpdate.ItemMasterFileUpload);
                var itemMasterRouting = _mapper.Map<IEnumerable<ItemMasterRouting>>(itemMasterDtoUpdate.ItemMasterRouting);
                var itemMasterWarehouse = _mapper.Map<IEnumerable<ItemMasterWarehouse>>(itemMasterDtoUpdate.ItemMasterWarehouse);
                var itemMaster = _mapper.Map(itemMasterDtoUpdate, itemMasterEntity);

                itemMaster.ItemmasterAlternate = itemMasterAlternate.ToList();
                itemMaster.ItemMasterApprovedVendor = itemMasterApprovedVendor.ToList();
                itemMaster.ItemMasterFileUpload=itemMasterFileUpload.ToList();
                itemMaster.ItemMasterRouting = itemMasterRouting.ToList();
                itemMaster.ItemMasterWarehouse = itemMasterWarehouse.ToList();

                string result = await _repository.ItemMasterRepository.UpdateItem(itemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateItemMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<ItemMasterController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemMaster(int id)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var itemMaster = await _repository.ItemMasterRepository.GetItemById(id);
                if (itemMaster == null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete vendor hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.ItemMasterRepository.DeleteItem(itemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateItemMaster(int id)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var itemMaster = await _repository.ItemMasterRepository.GetItemById(id);
                if (itemMaster is null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                itemMaster.IsActive = true;
                string result = await _repository.ItemMasterRepository.UpdateItem(itemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Activate Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateItemMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateItemMaster(int id)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var itemMaster = await _repository.ItemMasterRepository.GetItemById(id);
                if (itemMaster is null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                 }
                itemMaster.IsActive = false;
                string result = await _repository.ItemMasterRepository.UpdateItem(itemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "DeActivate Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateItemMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllActiveItemMasterIdNoList()
        {
            ServiceResponse<IEnumerable<ItemMasterIdNoListDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterIdNoListDto>>();
            try
            {
                var listOfItemno = await _repository.ItemMasterRepository.GetAllActiveItemMasterIdNoList();
                //_logger.LogInfo("Returned all CustomerMaster");
                var result = _mapper.Map<IEnumerable<ItemMasterIdNoListDto>>(listOfItemno);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all ItemMasterIdNoList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveItemMasterIdNoList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("ItemNumber")]
        public async Task<IActionResult> GetItemByItemNumber(string ItemNumber)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var Itemmasters = await _repository.ItemMasterRepository.GetItemByItemNumber(ItemNumber);
                if (Itemmasters == null)
                {
                    _logger.LogError($"Itemmasters with id: {ItemNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Itemmasters with id: {ItemNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Itemmasters with id: {ItemNumber}");
                    var result = _mapper.Map<ItemMasterDto>(Itemmasters);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemByItemNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
