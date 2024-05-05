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
    public class ShipmentInstructionsController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public ShipmentInstructionsController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }


        // GET: api/<ShipmentInstructionsController>
        [HttpGet]
        public async Task<IActionResult> GetAllShipmentInstructions([FromQuery]SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ShipmentInstructionsDto>> serviceResponse = new ServiceResponse<IEnumerable<ShipmentInstructionsDto>>();
            try
            {
                var shipmentInstructionsList = await _repository.ShipmentInstructionsRepository.GetAllShipmentInstructions(searchParams);
                _logger.LogInfo("Returned all ShipmentInstructions");
                var result = _mapper.Map<IEnumerable<ShipmentInstructionsDto>>(shipmentInstructionsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all ShipmentInstructions";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }
            // GET api/<ShipmentInstructionsController>/5
            [HttpGet("{id}")]
            public async Task<IActionResult> GetShipmentInstructionsById(int id)
            {
            ServiceResponse<ShipmentInstructionsDto> serviceResponse = new ServiceResponse<ShipmentInstructionsDto>();
            try
                {
                    var shipmentInstructions = await _repository.ShipmentInstructionsRepository.GetShipmentInstructionsById(id);
                    if (shipmentInstructions == null)
                    {
                      _logger.LogError($"ShipmentInstructions with id: {id}, hasn't been found in db.");
                       serviceResponse.Data = null;
                       serviceResponse.Message = "ShipmentInstructions hasn't been found in db";
                       serviceResponse.Success = false;
                       serviceResponse.StatusCode = HttpStatusCode.OK;
                       return Ok(serviceResponse);
                }
                    else
                    {
                        _logger.LogInfo($"Returned owner with id: {id}");
                        var result = _mapper.Map<ShipmentInstructionsDto>(shipmentInstructions);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "ShipmentInstructions Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Something went wrong inside GetShipmentInstructionsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
            }

        // POST api/<ShipmentInstructionsController>
        [HttpPost]
        public IActionResult CreateShipmentInstructions([FromBody] ShipmentInstructionsDtoPost shipmentInstructionsDtoPost)
        {
            ServiceResponse<ShipmentInstructionsDtoPost> serviceResponse = new ServiceResponse<ShipmentInstructionsDtoPost>();
            try
            {
                if (shipmentInstructionsDtoPost is null)
                {
                    _logger.LogError("ShipmentInstructions object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShipmentInstructions object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ShipmentInstructions object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ShipmentInstructions object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                var shipmentInstructionsEntity = _mapper.Map<ShipmentInstructions>(shipmentInstructionsDtoPost);
                _repository.ShipmentInstructionsRepository.CreateShipmentInstructions(shipmentInstructionsEntity);
                _repository.SaveAsync();


                serviceResponse.Data = null;
                serviceResponse.Message = "ShipmentInstructions Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<ShipmentInstructionsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipmentInstructions(int id, [FromBody] ShipmentInstructionsDtoUpdate shipmentInstructionsDtoUpdate)
        {
            ServiceResponse<ShipmentInstructionsDtoUpdate> serviceResponse = new ServiceResponse<ShipmentInstructionsDtoUpdate>();
            try
            {
                if (shipmentInstructionsDtoUpdate is null)
                {
                    _logger.LogError("ShipmentInstructions object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShipmentInstructions object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid ShipmentInstructions object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid ShipmentInstructions object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                var shipmentInstructionsEntity = await _repository.ShipmentInstructionsRepository.GetShipmentInstructionsById(id);
                if (shipmentInstructionsEntity is null)
                {
                    _logger.LogError($"ShipmentInstructions with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShipmentInstructions hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                _mapper.Map(shipmentInstructionsDtoUpdate, shipmentInstructionsEntity);
                string result = await _repository.ShipmentInstructionsRepository.UpdateShipmentInstructions(shipmentInstructionsEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ShipmentInstructions Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateShipmentInstructions action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<ShipmentInstructionsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipmentInstructions(int id)
        {
            ServiceResponse<ShipmentInstructionsDto> serviceResponse = new ServiceResponse<ShipmentInstructionsDto>();
            try
            {
                var shipmentInstructions = await _repository.ShipmentInstructionsRepository.GetShipmentInstructionsById(id);
                if (shipmentInstructions == null)
                {
                    _logger.LogError($"ShipmentInstructions with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShipmentInstructions hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                string result = await _repository.ShipmentInstructionsRepository.DeleteShipmentInstructions(shipmentInstructions);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "ShipmentInstructions Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Serever Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateShipmentInstructions(int id)
        {
            ServiceResponse<ShipmentInstructionsDto> serviceResponse = new ServiceResponse<ShipmentInstructionsDto>();
            try
            {
                var shipmentInstructions = await _repository.ShipmentInstructionsRepository.GetShipmentInstructionsById(id);
                if (shipmentInstructions is null)
                {
                    _logger.LogError($"ShipmentInstructions with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShipmentInstructions hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                shipmentInstructions.ActiveStatus = true;
                string result = await _repository.ShipmentInstructionsRepository.UpdateShipmentInstructions(shipmentInstructions);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned ActivateShipmentInstructions";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateShipmentInstructions action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateShipmentInstructions(int id)
        {
            ServiceResponse<ShipmentInstructionsDto> serviceResponse = new ServiceResponse<ShipmentInstructionsDto>();
            try
            {
                var shipmentInstructions = await _repository.ShipmentInstructionsRepository.GetShipmentInstructionsById(id);
                if (shipmentInstructions is null)
                {
                    _logger.LogError($"ShipmentInstructions with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ShipmentInstructions hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                shipmentInstructions.ActiveStatus = false;
                string result = await _repository.ShipmentInstructionsRepository.UpdateShipmentInstructions(shipmentInstructions);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned DeactivateShipmentInstructions";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateShipmentInstructions action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}

