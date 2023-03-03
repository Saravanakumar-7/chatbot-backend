using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public CityController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            ServiceResponse<IEnumerable<CityDto>> serviceResponse = new ServiceResponse<IEnumerable<CityDto>>();
            try
            {

                var cities = await _repository.CityRepository.GetAllCities();
                _logger.LogInfo("Returned all cities");
                var result = _mapper.Map<IEnumerable<CityDto>>(cities);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Cities  Successfully";
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
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveCities()
        {
            ServiceResponse<IEnumerable<CityDto>> serviceResponse = new ServiceResponse<IEnumerable<CityDto>>();

            try
            {
                var citiesList = await _repository.CityRepository.GetAllActiveCities();
                _logger.LogInfo("Returned all cities");
                var result = _mapper.Map<IEnumerable<CityDto>>(citiesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active cities Successfully";
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
        // GET api/<DemoStatusController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById(int id)
        {
            ServiceResponse<CityDto> serviceResponse = new ServiceResponse<CityDto>();

            try
            {
                var cityById = await _repository.CityRepository.GetCityById(id);
                if (cityById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"City with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"City with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned City with id: {id}");
                    var result = _mapper.Map<CityDto>(cityById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned City with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCityById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateCity([FromBody] CityPostDto cityPostDto)
        {
            ServiceResponse<CityPostDto> serviceResponse = new ServiceResponse<CityPostDto>();

            try
            {
                if (cityPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "City object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("City object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid City object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid City object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var city = _mapper.Map<City>(cityPostDto);
                _repository.CityRepository.CreateCity(city);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "City Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetCityById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateCity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] CityUpdateDto cityUpdateDto)
        {
            ServiceResponse<CityUpdateDto> serviceResponse = new ServiceResponse<CityUpdateDto>();

            try
            {
                if (cityUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update City object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update City object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update City object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update City object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var cityDetail = await _repository.CityRepository.GetCityById(id);
                if (cityDetail is null)
                {
                    _logger.LogError($"Update City with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update City with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(cityUpdateDto, cityDetail);
                string result = await _repository.CityRepository.UpdateCity(cityDetail);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateCity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            ServiceResponse<CityDto> serviceResponse = new ServiceResponse<CityDto>();

            try
            {
                var cityDetail = await _repository.CityRepository.GetCityById(id);
                if (cityDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete BHK object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete BHK with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.CityRepository.DeleteCity(cityDetail);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteCity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateCity(int id)
        {
            ServiceResponse<CityDto> serviceResponse = new ServiceResponse<CityDto>();

            try
            {
                var CityDetail = await _repository.CityRepository.GetCityById(id);
                if (CityDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "City object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"City with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                CityDetail.IsActive = true;
                string result = await _repository.CityRepository.UpdateCity(CityDetail);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside ActivateCity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateCity(int id)
        {
            ServiceResponse<CityDto> serviceResponse = new ServiceResponse<CityDto>();

            try
            {
                var cityDetail = await _repository.CityRepository.GetCityById(id);
                if (cityDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BHK object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"BHK with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                cityDetail.IsActive = false;
                string result = await _repository.CityRepository.UpdateCity(cityDetail);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivatedCity action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
