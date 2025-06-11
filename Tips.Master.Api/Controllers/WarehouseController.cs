using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public WarehouseController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<WarehouseController>
        [HttpGet]
        public async Task<IActionResult> GetAllWarehouse([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<WarehouseDto>> serviceResponse = new ServiceResponse<IEnumerable<WarehouseDto>>();
            try
            {
                var warehouseList = await _repository.WarehouseRepository.GetAllWarehouse(searchParams);
                _logger.LogInfo("Returned all Warehouse");
                var result = _mapper.Map<IEnumerable<WarehouseDto>>(warehouseList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all Warehouse";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllWarehouse API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllWarehouse API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveWarehouse([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<WarehouseDto>> serviceResponse = new ServiceResponse<IEnumerable<WarehouseDto>>();

            try
            {
                var locations = await _repository.WarehouseRepository.GetAllActiveWarehouse(searchParams);
                _logger.LogInfo("Returned all Warehouse");
                var result = _mapper.Map<IEnumerable<WarehouseDto>>(locations);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Warehouse Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveWarehouse API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveWarehouse API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");

            }
        }

        // GET api/<WarehouseController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarehouseById(int id)
        {
            ServiceResponse<WarehouseDto> serviceResponse = new ServiceResponse<WarehouseDto>();
            try
            {
                var warehouse = await _repository.WarehouseRepository.GetWarehouseById(id);
                if (warehouse == null)
                {
                    _logger.LogError($"Warehouse with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Warehouse hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<WarehouseDto>(warehouse);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Warehouse Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetWarehouseById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetWarehouseById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<WarehouseController>
        [HttpPost]
        public IActionResult CreateWarehouse([FromBody] WarehouseDtoPost warehouseDtoPost)
        {
            ServiceResponse<WarehouseDtoPost> serviceResponse = new ServiceResponse<WarehouseDtoPost>();
            try
            {
                if (warehouseDtoPost is null)
                {
                    _logger.LogError("Warehouse object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Warehouse object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Warehouse object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Warehouse object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                var warehouseEntity = _mapper.Map<Warehouse>(warehouseDtoPost);
                _repository.WarehouseRepository.CreateWarehouse(warehouseEntity);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "Warehouse Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateWarehouse API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateWarehouse API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<WarehouseController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] WarehouseDtoUpdate warehouseDtoUpdate)
        {
            ServiceResponse<WarehouseDtoUpdate> serviceResponse = new ServiceResponse<WarehouseDtoUpdate>();
            try
            {
                if (warehouseDtoUpdate is null)
                {
                    _logger.LogError("Warehouse object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Warehouse object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Warehouse object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Warehouse object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                var warehouseEntity = await _repository.WarehouseRepository.GetWarehouseById(id);
                if (warehouseEntity is null)
                {
                    _logger.LogError($"Warehouse with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Warehouse hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                _mapper.Map(warehouseDtoUpdate, warehouseEntity);
                string result = await _repository.WarehouseRepository.UpdateWarehouse(warehouseEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Warehouse Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateWarehouse API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateWarehouse API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<WarehouseController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            ServiceResponse<WarehouseDto> serviceResponse = new ServiceResponse<WarehouseDto>();
            try
            {
                var warehouse = await _repository.WarehouseRepository.GetWarehouseById(id);
                if (warehouse == null)
                {
                    _logger.LogError($"Warehouse with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Warehouse hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                string result = await _repository.WarehouseRepository.DeleteWarehouse(warehouse);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Warehouse Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteWarehouse API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteWarehouse API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateWarehouse(int id)
        {
            ServiceResponse<WarehouseDto> serviceResponse = new ServiceResponse<WarehouseDto>();
            try
            {
                var warehouse = await _repository.WarehouseRepository.GetWarehouseById(id);
                if (warehouse is null)
                {
                    _logger.LogError($"Warehouse with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Warehouse hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                warehouse.ActiveStatus = true;
                string result = await _repository.WarehouseRepository.UpdateWarehouse(warehouse);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned ActivateWarehouse";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in ActivateWarehouse API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ActivateWarehouse API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateWarehouse(int id)
        {
            ServiceResponse<WarehouseDto> serviceResponse = new ServiceResponse<WarehouseDto>();
            try
            {
                var warehouse = await _repository.WarehouseRepository.GetWarehouseById(id);
                if (warehouse is null)
                {
                    _logger.LogError($"Warehouse with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Warehouse hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                warehouse.ActiveStatus = false;
                string result = await _repository.WarehouseRepository.UpdateWarehouse(warehouse);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned DeactivateWarehouse";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeactivateWarehouse API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeactivateWarehouse API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
