using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PackingInstructionController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public PackingInstructionController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<PackingInstructionController>
        [HttpGet]
        public async Task<IActionResult> GetAllPackingInstructions([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<PackingInstructionDto>> serviceResponse = new ServiceResponse<IEnumerable<PackingInstructionDto>>();
            try
            {

                var packingInstructionList = await _repository.PackingInstructionRepository.GetAllPackingInstruction(searchParams);

                _logger.LogInfo("Returned all PackingInstructions");
                var result = _mapper.Map<IEnumerable<PackingInstructionDto>>(packingInstructionList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PackingInstructions Successfully";
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


        [HttpGet]
        public async Task<IActionResult> GetAllActivePackingInstructions([FromQuery]SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<PackingInstructionDto>> serviceResponse = new ServiceResponse<IEnumerable<PackingInstructionDto>>();

            try
            {
                var PackingInstructionList = await _repository.PackingInstructionRepository.GetAllActivePackingInstruction(searchParams);
                _logger.LogInfo("Returned all PackingInstructions");
                var result = _mapper.Map<IEnumerable<PackingInstructionDto>>(PackingInstructionList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active PackingInstructions Successfully";
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
        // GET api/<PackingInstructionController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackingInstructionById(int id)
        {
            ServiceResponse<PackingInstructionDto> serviceResponse = new ServiceResponse<PackingInstructionDto>();

            try
            {
                var PackingInstructions = await _repository.PackingInstructionRepository.GetPackingInstructionById(id);
                if (PackingInstructions == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PackingInstruction with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PackingInstruction with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<PackingInstructionDto>(PackingInstructions);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPackingInstructionById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<PackingInstructionController>
        [HttpPost]
        public IActionResult CreatePackingInstruction([FromBody] PackingInstructionDtoPost packingInstructionDtoPost)
        {
            ServiceResponse<PackingInstructionDto> serviceResponse = new ServiceResponse<PackingInstructionDto>();

            try
            {
                if (packingInstructionDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PackingInstruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PackingInstruction object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PackingInstruction object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PackingInstruction object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var PackingInstructions = _mapper.Map<PackingInstruction>(packingInstructionDtoPost);
                _repository.PackingInstructionRepository.CreatePackingInstruction(PackingInstructions);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetPackingInstructionById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<PackingInstructionController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePackingInstruction(int id, [FromBody] PackingInstructionDtoUpdate packingInstructionDtoUpdate)
        {
            ServiceResponse<PackingInstructionDto> serviceResponse = new ServiceResponse<PackingInstructionDto>();

            try
            {
                if (packingInstructionDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update PackingInstruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("updare PackingInstruction object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update PackingInstruction object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update PackingInstruction object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var PackingInstruction = await _repository.PackingInstructionRepository.GetPackingInstructionById(id);
                if (PackingInstruction is null)
                {
                    _logger.LogError($"Update PackingInstruction with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update PackingInstruction with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(packingInstructionDtoUpdate, PackingInstruction);
                string result = await _repository.PackingInstructionRepository.UpdatePackingInstruction(PackingInstruction);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdatePackingInstruction action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<PackingInstructionController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackingInstruction(int id)
        {
            ServiceResponse<PackingInstructionDto> serviceResponse = new ServiceResponse<PackingInstructionDto>();

            try
            {
                var PackingInstruction = await _repository.PackingInstructionRepository.GetPackingInstructionById(id);
                if (PackingInstruction == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PackingInstruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PackingInstruction with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.PackingInstructionRepository.DeletePackingInstruction(PackingInstruction);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivatePackingInstruction(int id)
        {
            ServiceResponse<PackingInstructionDto> serviceResponse = new ServiceResponse<PackingInstructionDto>();

            try
            {
                var PackingInstruction = await _repository.PackingInstructionRepository.GetPackingInstructionById(id);
                if (PackingInstruction is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PackingInstruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PackingInstruction with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                PackingInstruction.IsActive = true;
                string result = await _repository.PackingInstructionRepository.UpdatePackingInstruction(PackingInstruction);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside ActivatedauditFrequency action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivatePackingInstruction(int id)
        {
            ServiceResponse<PackingInstructionDto> serviceResponse = new ServiceResponse<PackingInstructionDto>();

            try
            {
                var PackingInstruction = await _repository.PackingInstructionRepository.GetPackingInstructionById(id);
                if (PackingInstruction is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PackingInstruction object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PackingInstruction with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                PackingInstruction.IsActive = false;
                string result = await _repository.PackingInstructionRepository.UpdatePackingInstruction(PackingInstruction);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeactivatedauditFrequency action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
