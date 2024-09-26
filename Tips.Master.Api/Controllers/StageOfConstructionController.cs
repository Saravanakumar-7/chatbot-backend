using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class StageOfConstructionController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public StageOfConstructionController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllStageOfConstructions()
        {
            ServiceResponse<IEnumerable<StageOfConstructionDto>> serviceResponse = new ServiceResponse<IEnumerable<StageOfConstructionDto>>();
            try
            {

                var stageOfConstructions = await _repository.StageOfConstructionRepository.GetAllStageOfConstructions();
                _logger.LogInfo("Returned all StageOfConstructions");
                var result = _mapper.Map<IEnumerable<StageOfConstructionDto>>(stageOfConstructions);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all StageOfConstructions  Successfully";
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
        public async Task<IActionResult> GetAllActiveStageOfConstruction()
        {
            ServiceResponse<IEnumerable<StageOfConstructionDto>> serviceResponse = new ServiceResponse<IEnumerable<StageOfConstructionDto>>();

            try
            {
                var stageOfConstructionsList = await _repository.StageOfConstructionRepository.GetAllActiveStageOfConstruction();
                _logger.LogInfo("Returned all StageOfConstructions");
                var result = _mapper.Map<IEnumerable<StageOfConstructionDto>>(stageOfConstructionsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active StageOfConstructions Successfully";
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
        public async Task<IActionResult> GetStageOfConstructionById(int id)
        {
            ServiceResponse<StageOfConstructionDto> serviceResponse = new ServiceResponse<StageOfConstructionDto>();

            try
            {
                var stageOfConstructionById = await _repository.StageOfConstructionRepository.GetStageOfConstructionById(id);
                if (stageOfConstructionById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"StageOfConstructions with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"StageOfConstructions with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned StageOfConstructions with id: {id}");
                    var result = _mapper.Map<StageOfConstructionDto>(stageOfConstructionById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned StageOfConstructions with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetStageOfConstructionById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateStageOfConstruction([FromBody] StageOfConstructionPostDto stageOfConstructionPostDto)
        {
            ServiceResponse<StageOfConstructionPostDto> serviceResponse = new ServiceResponse<StageOfConstructionPostDto>();

            try
            {
                if (stageOfConstructionPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "StageOfConstruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("StageOfConstruction object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid StageOfConstruction object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid StageOfConstruction object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var stageOfConstruction = _mapper.Map<StageOfConstruction>(stageOfConstructionPostDto);
                _repository.StageOfConstructionRepository.CreateStageOfConstruction(stageOfConstruction);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "StageOfConstruction Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetstateById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateStageOfConstruction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStageOfConstruction(int id, [FromBody] StageOfConstructionUpdateDto stageOfConstructionUpdateDto)
        {
            ServiceResponse<StageOfConstructionUpdateDto> serviceResponse = new ServiceResponse<StageOfConstructionUpdateDto>();

            try
            {
                if (stageOfConstructionUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update StageOfConstruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update StageOfConstruction object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update StageOfConstruction object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update StageOfConstruction object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var stageOfConstructionDetail = await _repository.StageOfConstructionRepository.GetStageOfConstructionById(id);
                if (stageOfConstructionDetail is null)
                {
                    _logger.LogError($"Update StageOfConstruction with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update StageOfConstruction with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(stageOfConstructionUpdateDto, stageOfConstructionDetail);
                string result = await _repository.StageOfConstructionRepository.UpdateStageOfConstruction(stageOfConstructionDetail);
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
                _logger.LogError($"Something went wrong inside UpdateStageOfConstruction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStageOfConstruction(int id)
        {
            ServiceResponse<StageOfConstructionDto> serviceResponse = new ServiceResponse<StageOfConstructionDto>();

            try
            {
                var stageOfConstructionDetail = await _repository.StageOfConstructionRepository.GetStageOfConstructionById(id);
                if (stageOfConstructionDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete StageOfConstruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete StageOfConstruction with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.StageOfConstructionRepository.DeleteStageOfConstruction(stageOfConstructionDetail);
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
                _logger.LogError($"Something went wrong inside DeleteStageOfConstruction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateStageOfConstruction(int id)
        {
            ServiceResponse<StageOfConstructionDto> serviceResponse = new ServiceResponse<StageOfConstructionDto>();

            try
            {
                var stageofContructionDetail = await _repository.StageOfConstructionRepository.GetStageOfConstructionById(id);
                if (stageofContructionDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "stateofContruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"stateofContruction with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                stageofContructionDetail.IsActive = true;
                string result = await _repository.StageOfConstructionRepository.UpdateStageOfConstruction(stageofContructionDetail);
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
                _logger.LogError($"Something went wrong inside ActivateStageOfConstruction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateStateofContruction(int id)
        {

            ServiceResponse<StageOfConstructionDto> serviceResponse = new ServiceResponse<StageOfConstructionDto>();

            try
            {
                var stageofContructionDetail = await _repository.StageOfConstructionRepository.GetStageOfConstructionById(id);
                if (stageofContructionDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "stageofContructionDetail object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"stageofContructionDetail with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                stageofContructionDetail.IsActive = true;
                string result = await _repository.StageOfConstructionRepository.UpdateStageOfConstruction(stageofContructionDetail);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "DeActivated Successfully";
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
                _logger.LogError($"Something went wrong inside DeactivateStageofContruction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}