using System.Collections.Generic;
using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository;

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
        public async Task<IActionResult> GetAllItemMasters([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();

            try
            {
                var getAllItemMastersList = await _repository.ItemMasterRepository.GetAllItemMasters(pagingParameter);
                _logger.LogInfo("Returned all ItemMasters");
                var metadata = new
                {
                    getAllItemMastersList.TotalCount,
                    getAllItemMastersList.PageSize,
                    getAllItemMastersList.CurrentPage,
                    getAllItemMastersList.HasNext,
                    getAllItemMastersList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(getAllItemMastersList);
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
        //GET All FG items
        [HttpGet]
        public async Task<IActionResult> GetAllFGItems([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();

            try
            {
                var getAllFGItemsList = await _repository.ItemMasterRepository.GetAllFGItems(pagingParameter);
                _logger.LogInfo("Returned all FGItemMasters");
                var metadata = new
                {
                    getAllFGItemsList.TotalCount,
                    getAllFGItemsList.PageSize,
                    getAllFGItemsList.CurrentPage,
                    getAllFGItemsList.HasNext,
                    getAllFGItemsList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(getAllFGItemsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FGItemMasters Successfully";
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
        //passing items number and get process records

      


        //GET All Sa items
        [HttpGet]
        public async Task<IActionResult> GetAllSAItems([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();

            try
            {
                var getAllSAItemsList = await _repository.ItemMasterRepository.GetAllSAItems(pagingParameter);
                _logger.LogInfo("Returned all SAItemMasters");
                var metadata = new
                {
                    getAllSAItemsList.TotalCount,
                    getAllSAItemsList.PageSize,
                    getAllSAItemsList.CurrentPage,
                    getAllSAItemsList.HasNext,
                    getAllSAItemsList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(getAllSAItemsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SAItemMasters Successfully";
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
        //GET All FG&SAItems
        [HttpGet]
        public async Task<IActionResult> GetAllFGSAItems([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<ItemMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<ItemMasterDto>>();

            try
            {
                var getAllFGSAItemsList = await _repository.ItemMasterRepository.GetAllFgSaItems(pagingParameter);
                _logger.LogInfo("Returned all FGSAItemMasters");
                var metadata = new
                {
                    getAllFGSAItemsList.TotalCount,
                    getAllFGSAItemsList.PageSize,
                    getAllFGSAItemsList.CurrentPage,
                    getAllFGSAItemsList.HasNext,
                    getAllFGSAItemsList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var result = _mapper.Map<IEnumerable<ItemMasterDto>>(getAllFGSAItemsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FGSAItemMasters Successfully";
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
        public async Task<IActionResult> GetItemMasterById(int id)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var getItemMaster = await _repository.ItemMasterRepository.GetItemMasterById(id);
                if (getItemMaster == null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ItemMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ItemMaster with id: {id}");
                    var result = _mapper.Map<ItemMasterDto>(getItemMaster);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned ItemMasterById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemMasterById action: {ex.Message}");
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
                _repository.ItemMasterRepository.CreateItemMaster(itemMasterEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemMaster Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetItemMasterById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateItemMaster action: {ex.Message}");
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
                    serviceResponse.Message = "Update ItemMaster object is null";
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
                var updateItemMasterEntity = await _repository.ItemMasterRepository.GetItemMasterById(id);
                if (updateItemMasterEntity is null)
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
                var itemMaster = _mapper.Map(itemMasterDtoUpdate, updateItemMasterEntity);

                itemMaster.ItemmasterAlternate = itemMasterAlternate.ToList();
                itemMaster.ItemMasterApprovedVendor = itemMasterApprovedVendor.ToList();
                itemMaster.ItemMasterFileUpload=itemMasterFileUpload.ToList();
                itemMaster.ItemMasterRouting = itemMasterRouting.ToList();
                itemMaster.ItemMasterWarehouse = itemMasterWarehouse.ToList();

                string result = await _repository.ItemMasterRepository.UpdateItemMaster(itemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemMaster Updated Successfully";
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
                var deleteItemMaster = await _repository.ItemMasterRepository.GetItemMasterById(id);
                if (deleteItemMaster == null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete ItemMaster hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.ItemMasterRepository.DeleteItemMaster(deleteItemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemMaster Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteItemMaster action: {ex.Message}");
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
                var activateItemMaster = await _repository.ItemMasterRepository.GetItemMasterById(id);
                if (activateItemMaster is null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                activateItemMaster.IsActive = true;
                string result = await _repository.ItemMasterRepository.UpdateItemMaster(activateItemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemMaster Activated Successfully";
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
                var deactivateItemMaster = await _repository.ItemMasterRepository.GetItemMasterById(id);
                if (deactivateItemMaster is null)
                {
                    _logger.LogError($"ItemMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ItemMaster object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                 }
                deactivateItemMaster.IsActive = false;
                string result = await _repository.ItemMasterRepository.UpdateItemMaster(deactivateItemMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ItemMaster Deactivated Successfully";
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
                var getAllActiveItemMasters = await _repository.ItemMasterRepository.GetAllActiveItemMasterIdNoList();
                //_logger.LogInfo("Returned all CustomerMaster");
                var result = _mapper.Map<IEnumerable<ItemMasterIdNoListDto>>(getAllActiveItemMasters);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active ItemMasterIdNoList";
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
        public async Task<IActionResult> GetItemMasterByItemNumber(string ItemNumber)
        {
            ServiceResponse<ItemMasterDto> serviceResponse = new ServiceResponse<ItemMasterDto>();

            try
            {
                var getItemMasterByItemNumber = await _repository.ItemMasterRepository.GetItemMasterByItemNumber(ItemNumber);
                if (getItemMasterByItemNumber == null)
                {
                    _logger.LogError($"Itemmasters with id: {ItemNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Itemmasters with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Itemmasters with id: {ItemNumber}");
                    var result = _mapper.Map<ItemMasterDto>(getItemMasterByItemNumber);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned All ItemMasterByItemNumber";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetItemMasterByItemNumber action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
