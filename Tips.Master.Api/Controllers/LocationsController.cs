using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
        public async Task<IActionResult> GetAllLocations()
        {
            try
            {
                var LocationsList = await _repository.LocationsRepository.GetAllLocations();
                _logger.LogInfo("Returned all Locations");
                var result = _mapper.Map<IEnumerable<LocationsDto>>(LocationsList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<LocationsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationsById(int id)
        {
            try
            {
                var Locations = await _repository.LocationsRepository.GetLocationsById(id);
                if (Locations == null)
                {
                    _logger.LogError($"Locations with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<LocationsDto>(Locations);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetLocationsById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

        // POST api/<LocationsController>
        [HttpPost]
        public IActionResult CreateLocations([FromBody] LocationsDtoPost LocationsDtoPost)
        {
            try
            {
                if (LocationsDtoPost is null)
                {
                    _logger.LogError("Locations object sent from client is null.");
                    return BadRequest("Locations object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Locations object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var LocationsEntity = _mapper.Map<Locations>(LocationsDtoPost);
                _repository.LocationsRepository.CreateLocations(LocationsEntity);
                _repository.SaveAsync();

                return Created("GetLocationsById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<LocationsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocations(int id, [FromBody] LocationsDtoUpdate locationsDtoUpdate)
        {
            try
            {
                if (locationsDtoUpdate is null)
                {
                    _logger.LogError("Locations object sent from client is null.");
                    return BadRequest("Locations object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Locations object sent from client.");
                    return BadRequest("Invalid model object");
                }
                var LocationsEntity = await _repository.LocationsRepository.GetLocationsById(id);
                if (LocationsEntity is null)
                {
                    _logger.LogError($"Locations with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(locationsDtoUpdate, LocationsEntity);
                string result = await _repository.LocationsRepository.UpdateLocations(LocationsEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateLocations action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<LocationsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocations(int id)
        {
            try
            {
                var Locations = await _repository.LocationsRepository.GetLocationsById(id);
                if (Locations == null)
                {
                    _logger.LogError($"Locations with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.LocationsRepository.DeleteLocations(Locations);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateLocations(int id)
        {
            try
            {
                var Locations = await _repository.LocationsRepository.GetLocationsById(id);
                if (Locations is null)
                {
                    _logger.LogError($"Locations with id: {id}, hasn't been found in db.");
                    return BadRequest("Locations object is null");
                }
                Locations.ActiveStatus = true;
                string result = await _repository.LocationsRepository.UpdateLocations(Locations);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateLocations action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateLocations(int id)
        {
            try
            {
                var Locations = await _repository.LocationsRepository.GetLocationsById(id);
                if (Locations is null)
                {
                    _logger.LogError($"Locations with id: {id}, hasn't been found in db.");
                    return BadRequest("Locations object is null");
                }
                Locations.ActiveStatus = false;
                string result = await _repository.LocationsRepository.UpdateLocations(Locations);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateLocations action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}