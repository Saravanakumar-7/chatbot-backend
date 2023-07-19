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
    public class PartTypesController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public PartTypesController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<PartTypeController>
        [HttpGet]
        public async Task<IActionResult> GetAllPartTypes([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<PartTypesDto>> serviceResponse = new ServiceResponse<IEnumerable<PartTypesDto>>();
            try
            {

                var PartTypeList = await _repository.partTypesRepository.GetAllPartTypes(searchParams);
                _logger.LogInfo("Returned all PartTypes");
                var result = _mapper.Map<IEnumerable<PartTypesDto>>(PartTypeList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PartTypes Successfully";
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
        public async Task<IActionResult> GetAllActivePartTypes([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<PartTypesDto>> serviceResponse = new ServiceResponse<IEnumerable<PartTypesDto>>();

            try
            {
                var PartTypes = await _repository.partTypesRepository.GetAllActivePartTypes(searchParams);
                _logger.LogInfo("Returned all PartTypes");
                var result = _mapper.Map<IEnumerable<PartTypesDto>>(PartTypes);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active PartTypes Successfully";
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
        // GET api/<PartTypeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPartTypeById(int id)
        {
            ServiceResponse<PartTypesDto> serviceResponse = new ServiceResponse<PartTypesDto>();

            try
            {
                var PartTypes = await _repository.partTypesRepository.GetPartTypesById(id);
                if (PartTypes == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PartTypes with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PartTypes with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned PartType with id: {id}");
                    var result = _mapper.Map<PartTypesDto>(PartTypes);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned PartTypes with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPartTypeById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Servere Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<PartTypeController>
        [HttpPost]
        public IActionResult CreatePartTypes([FromBody] PartTypesDtoPost partTypesDtoPost)
        {
            ServiceResponse<PartTypesDto> serviceResponse = new ServiceResponse<PartTypesDto>();

            try
            {
                if (partTypesDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PartType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PartType object sent from client is null.");
                    //return BadRequest("PartType object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PartType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PartType object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var parttypes = _mapper.Map<PartTypes>(partTypesDtoPost);
                _repository.partTypesRepository.CreatePartTypes(parttypes);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "PartTypes Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetPartTypesById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreatePartType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<PartTypeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePartTypes(int id, [FromBody] PartTypesDtoUpdate partTypesDtoUpdate)
        {
            ServiceResponse<PartTypesDtoUpdate> serviceResponse = new ServiceResponse<PartTypesDtoUpdate>();

            try
            {
                if (partTypesDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update PartType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update PartType object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update PartType object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update PartType object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var parttype = await _repository.partTypesRepository.GetPartTypesById(id);
                if (parttype is null)
                {
                    _logger.LogError($"Update PartTypes with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update PartType with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(partTypesDtoUpdate, parttype);
                string result = await _repository.partTypesRepository.UpdatePartTypes(parttype);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "PartType Updated Successfully";
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
                _logger.LogError($"Something went wrong inside UpdatePartType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<PartTypeController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartTypes(int id)
        {
            ServiceResponse<PartTypesDto> serviceResponse = new ServiceResponse<PartTypesDto>();

            try
            {
                var parttype = await _repository.partTypesRepository.GetPartTypesById(id);
                if (parttype == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete PartType object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete PartType with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.partTypesRepository.DeletePartTypes(parttype);
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
                _logger.LogError($"Something went wrong inside PartType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivatePartTypes(int id)
        {
            ServiceResponse<PartTypesDto> serviceResponse = new ServiceResponse<PartTypesDto>();

            try
            {
                var parttype = await _repository.partTypesRepository.GetPartTypesById(id);
                if (parttype is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Parttype object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Parttype with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                parttype.IsActive = true;
                string result = await _repository.partTypesRepository.UpdatePartTypes(parttype);
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
                _logger.LogError($"Something went wrong inside ActivatedPartType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivatePartType(int id)
        {
            ServiceResponse<PartTypesDto> serviceResponse = new ServiceResponse<PartTypesDto>();

            try
            {
                var parttype = await _repository.partTypesRepository.GetPartTypesById(id);
                if (parttype is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "parttype object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"parttype with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                parttype.IsActive = false;
                string result = await _repository.partTypesRepository.UpdatePartTypes(parttype);
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
                _logger.LogError($"Something went wrong inside DeactivatePartType action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
