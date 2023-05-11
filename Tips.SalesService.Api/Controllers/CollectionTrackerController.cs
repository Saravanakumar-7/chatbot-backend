using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CollectionTrackerController : ControllerBase
    {
        private ICollectionTrackerRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public CollectionTrackerController(ICollectionTrackerRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCollectionTracker()
        {
            ServiceResponse<IEnumerable<CollectionTrackerDto>> serviceResponse = new ServiceResponse<IEnumerable<CollectionTrackerDto>>();

            try
            {
                var collectionTrackerDetails = await _repository.GetAllCollectionTrackers();
                _logger.LogInfo("Returned all CollectionTracker");
                var result = _mapper.Map<IEnumerable<CollectionTrackerDto>>(collectionTrackerDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CollectionTracker Successfully";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCollectionTrackerById(int id)
        {
            ServiceResponse<CollectionTrackerDto> serviceResponse = new ServiceResponse<CollectionTrackerDto>();
            try
            {
                var collectionTrackerById = await _repository.GetCollectionTrackerById(id);

                if (collectionTrackerById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CollectionTracker with id hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CollectionTracker with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CollectionTracker with id: {id}");
                    var result = _mapper.Map<CollectionTrackerDto>(collectionTrackerById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CollectionTrackerById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCollectionTrackerById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCollectionTrackerById action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCollectionTracker([FromBody] CollectionTrackerPostDto collectionTrackerPostDto)
        {
            ServiceResponse<CollectionTrackerDto> serviceResponse = new ServiceResponse<CollectionTrackerDto>();
            
            try
            {
                if (collectionTrackerPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CollectionTracker object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("CollectionTracker object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CollectionTracker object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid CollectionTracker object sent from client.");
                    return BadRequest("Invalid model object");
                }


                var collectionTrackerItems = _mapper.Map<IEnumerable<SOBreakDown>>(collectionTrackerPostDto.SOBreakDown);
               
                var collectionTracker = _mapper.Map<CollectionTracker>(collectionTrackerPostDto);

                collectionTracker.SOBreakDown = collectionTrackerItems.ToList();
               
                await _repository.CreateCollectionTracker(collectionTracker);

                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "CollectionTracker Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.Created;
                return Created("GetCollectionTracker", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateCollectionTracker action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateCollectionTracker(int id, [FromBody] CollectionTrackerUpdateDto collectionTrackerUpdateDto)
        {
            ServiceResponse<CollectionTrackerUpdateDto> serviceResponse = new ServiceResponse<CollectionTrackerUpdateDto>();
            try
            {
                if (collectionTrackerUpdateDto is null)
                {
                    _logger.LogError("Update CollectionTracker object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update CollectionTracker object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update CollectionTracker object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update CollectionTracker object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest("Invalid model object");
                }
                var collectionTrackerById = await _repository.GetCollectionTrackerById(id);
                if (collectionTrackerById is null)
                {
                    _logger.LogError($"Update CollectionTracker with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update CollectionTracker with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var collectionTrackerItems = _mapper.Map<IEnumerable<SOBreakDown>>(collectionTrackerUpdateDto.SOBreakDown);

                var collectionTracker = _mapper.Map(collectionTrackerUpdateDto, collectionTrackerById);

                collectionTracker.SOBreakDown = collectionTrackerItems.ToList();

                string result = await _repository.UpdateCollectionTracker(collectionTracker);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CollectionTracker Successfully Updated"; ;
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCollectionTracker action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollectionTracker(int id)
        {
            ServiceResponse<CollectionTrackerDto> serviceResponse = new ServiceResponse<CollectionTrackerDto>();
            try
            {
                var collectionTrackerById = await _repository.GetCollectionTrackerById(id);
                if (collectionTrackerById == null)
                {
                    _logger.LogError($"Delete CollectionTracker with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete CollectionTracker with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteCollectionTracker(collectionTrackerById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CollectionTracker Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteCollectionTracker action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
