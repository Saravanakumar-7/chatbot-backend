using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LocationsController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public LocationsController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<LocationsController>
        [HttpGet]
        public async Task<IActionResult> GetAllLocations([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LocationsDto>> serviceResponse = new ServiceResponse<IEnumerable<LocationsDto>>();

            try
            {
                var LocationsList = await _repository.LocationsRepository.GetAllLocations(searchParams);
           
                _logger.LogInfo("Returned all Locations");
                var result = _mapper.Map<IEnumerable<LocationsDto>>(LocationsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Locations Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllLocations API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllLocations API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllActiveLocations([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<LocationsDto>> serviceResponse = new ServiceResponse<IEnumerable<LocationsDto>>();

            try
            {
                var locations = await _repository.LocationsRepository.GetAllActiveLocations(searchParams);
                _logger.LogInfo("Returned all Warehouse");
                var result = _mapper.Map<IEnumerable<LocationsDto>>(locations);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Locations Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveLocations API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveLocations API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListofLocationsByWarehouse(string Warehouse)
        {
            ServiceResponse<IEnumerable<GetListofLocationsByWarehouseDto>> serviceResponse = new ServiceResponse<IEnumerable<GetListofLocationsByWarehouseDto>>();

            try
            {
                var locationbywh = await _repository.LocationsRepository.GetListofLocationsByWarehouse(Warehouse);
                if (locationbywh == null)
                {
                    _logger.LogError($"ListOfLocations with id: {Warehouse}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"ListOfLocations with id: {Warehouse}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned ListOfLocations with id: {Warehouse}");
                    var result = _mapper.Map<IEnumerable<GetListofLocationsByWarehouseDto>>(locationbywh);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)

            {
                _logger.LogError($"Error Occured in GetListofLocationsByWarehouse API for the following Warehouse : {Warehouse} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetListofLocationsByWarehouse API for the following Warehouse : {Warehouse} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<LocationsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationsById(int id)
        {
            ServiceResponse<LocationsDto> serviceResponse = new ServiceResponse<LocationsDto>();

            try
            {
                

                var Locations = await _repository.LocationsRepository.GetLocationsById(id);
                if (Locations == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Locations with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Locations with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<LocationsDto>(Locations);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetLocationsById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetLocationsById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        // POST api/<LocationsController>
        [HttpPost]
        public IActionResult CreateLocations([FromBody] LocationsDtoPost LocationsDtoPost)
        {
            ServiceResponse<LocationsDto> serviceResponse = new ServiceResponse<LocationsDto>();

            try
            {
                if (LocationsDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Locations object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Locations object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Locations object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Locations object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var LocationsEntity = _mapper.Map<Locations>(LocationsDtoPost);
                _repository.LocationsRepository.CreateLocations(LocationsEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetLocationsById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateLocations API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateLocations API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<LocationsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocations(int id, [FromBody] LocationsDtoUpdate locationsDtoUpdate)
        {
            ServiceResponse<LocationsDto> serviceResponse = new ServiceResponse<LocationsDto>();

            try
            {
                if (locationsDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Locations object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update Locations object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Locations object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update Invalid Locations object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var LocationsEntity = await _repository.LocationsRepository.GetLocationsById(id);
                if (LocationsEntity is null)
                {
                    _logger.LogError($"Update Locations with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Locations with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(locationsDtoUpdate, LocationsEntity);
                string result = await _repository.LocationsRepository.UpdateLocations(LocationsEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateLocations API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateLocations API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<LocationsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocations(int id)
        {
            ServiceResponse<LocationsDto> serviceResponse = new ServiceResponse<LocationsDto>();

            try
            {
                var Locations = await _repository.LocationsRepository.GetLocationsById(id);
                if (Locations == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Locations object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete Locations with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.LocationsRepository.DeleteLocations(Locations);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteLocations API for the following id : {id} \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteLocations API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateLocations(int id)
        {
            ServiceResponse<LocationsDto> serviceResponse = new ServiceResponse<LocationsDto>();

            try
            {
                var Locations = await _repository.LocationsRepository.GetLocationsById(id);
                if (Locations is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Locations object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Locations with id: {id}, hasn't been found in db.");
                    return BadRequest("Locations object is null");
                }
                Locations.ActiveStatus = true;
                string result = await _repository.LocationsRepository.UpdateLocations(Locations);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ActivateLocations API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in ActivateLocations API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateLocations(int id)
        {
            ServiceResponse<LocationsDto> serviceResponse = new ServiceResponse<LocationsDto>();

            try
            {
                var Locations = await _repository.LocationsRepository.GetLocationsById(id);
                if (Locations is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Language object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Locations with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Locations.ActiveStatus = false;
                string result = await _repository.LocationsRepository.UpdateLocations(Locations);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deactivated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeactivateLocations API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeactivateLocations API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}