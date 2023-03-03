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
    public class StateOfConstructionController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public StateOfConstructionController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllStateOfConstructions()
        {
            ServiceResponse<IEnumerable<StateOfConstructionDto>> serviceResponse = new ServiceResponse<IEnumerable<StateOfConstructionDto>>();
            try
            {

                var stateOfConstructions = await _repository.StateOfConstructionRepository.GetAllStateOfConstructions();
                _logger.LogInfo("Returned all stateOfConstructions");
                var result = _mapper.Map<IEnumerable<StateOfConstructionDto>>(stateOfConstructions);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all stateOfConstructions  Successfully";
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
        public async Task<IActionResult> GetAllActiveStateOfConstruction()
        {
            ServiceResponse<IEnumerable<StateOfConstructionDto>> serviceResponse = new ServiceResponse<IEnumerable<StateOfConstructionDto>>();

            try
            {
                var stateOfConstructionsList = await _repository.StateOfConstructionRepository.GetAllActiveStateOfConstruction();
                _logger.LogInfo("Returned all stateOfConstructions");
                var result = _mapper.Map<IEnumerable<StateOfConstructionDto>>(stateOfConstructionsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active stateOfConstructions Successfully";
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
        public async Task<IActionResult> GetStateOfConstructionById(int id)
        {
            ServiceResponse<StateOfConstructionDto> serviceResponse = new ServiceResponse<StateOfConstructionDto>();

            try
            {
                var StateOfConstructionById = await _repository.StateOfConstructionRepository.GetStateOfConstructionById(id);
                if (StateOfConstructionById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"stateOfConstructions with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"stateOfConstructions with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned stateOfConstructions with id: {id}");
                    var result = _mapper.Map<StateOfConstructionDto>(StateOfConstructionById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned stateOfConstructions with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetStateOfConstructionById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateStateOfConstruction([FromBody] StateOfConstructionPostDto stateOfConstructionPostDto)
        {
            ServiceResponse<StateOfConstructionPostDto> serviceResponse = new ServiceResponse<StateOfConstructionPostDto>();

            try
            {
                if (stateOfConstructionPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "StateOfConstruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("StateOfConstruction object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid StateOfConstruction object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid StateOfConstruction object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var StateOfConstruction = _mapper.Map<StateOfConstruction>(stateOfConstructionPostDto);
                _repository.StateOfConstructionRepository.CreateStateOfConstruction(StateOfConstruction);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "StateOfConstruction Created Successfully";
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
                _logger.LogError($"Something went wrong inside CreateStateOfConstruction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStateOfConstruction(int id, [FromBody] StateOfConstructionUpdateDto stateOfConstructionUpdateDto)
        {
            ServiceResponse<StateOfConstructionUpdateDto> serviceResponse = new ServiceResponse<StateOfConstructionUpdateDto>();

            try
            {
                if (stateOfConstructionUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update StateOfConstruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update StateOfConstruction object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update StateOfConstruction object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update StateOfConstruction object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var stateOfConstructionDetail = await _repository.StateOfConstructionRepository.GetStateOfConstructionById(id);
                if (stateOfConstructionDetail is null)
                {
                    _logger.LogError($"Update StateOfConstruction with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update StateOfConstruction with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(stateOfConstructionUpdateDto, stateOfConstructionDetail);
                string result = await _repository.StateOfConstructionRepository.UpdateStateOfConstruction(stateOfConstructionDetail);
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
                _logger.LogError($"Something went wrong inside UpdateStateOfConstruction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStateOfConstruction(int id)
        {
            ServiceResponse<StateOfConstructionDto> serviceResponse = new ServiceResponse<StateOfConstructionDto>();

            try
            {
                var stateOfConstructionDetail = await _repository.StateOfConstructionRepository.GetStateOfConstructionById(id);
                if (stateOfConstructionDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete stateOfConstruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete stateOfConstruction with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.StateOfConstructionRepository.DeleteStateOfConstruction(stateOfConstructionDetail);
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
                _logger.LogError($"Something went wrong inside DeleteStateOfConstruction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateStateOfConstruction(int id)
        {
            ServiceResponse<StateOfConstructionDto> serviceResponse = new ServiceResponse<StateOfConstructionDto>();

            try
            {
                var stateofContructionDetail = await _repository.StateOfConstructionRepository.GetStateOfConstructionById(id);
                if (stateofContructionDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "stateofContruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"stateofContruction with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                stateofContructionDetail.IsActive = true;
                string result = await _repository.StateOfConstructionRepository.UpdateStateOfConstruction(stateofContructionDetail);
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
                _logger.LogError($"Something went wrong inside ActivateStateOfConstruction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateStateofContruction(int id)
        {

            ServiceResponse<StateOfConstructionDto> serviceResponse = new ServiceResponse<StateOfConstructionDto>();

            try
            {
                var stateofContructionDetail = await _repository.StateOfConstructionRepository.GetStateOfConstructionById(id);
                if (stateofContructionDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "stateofContruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"stateofContruction with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                stateofContructionDetail.IsActive = true;
                string result = await _repository.StateOfConstructionRepository.UpdateStateOfConstruction(stateofContructionDetail);
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
                _logger.LogError($"Something went wrong inside DeactivateStateofContruction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}