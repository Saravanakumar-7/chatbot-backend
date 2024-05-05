using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UnitController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public UnitController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUnits([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<UnitDto>> serviceResponse = new ServiceResponse<IEnumerable<UnitDto>>();
            try
            {

                var getAllUnits = await _repository.unitRepository.GetAllUnits(pagingParameter, searchParams);


                var metadata = new
                {
                    getAllUnits.TotalCount,
                    getAllUnits.PageSize,
                    getAllUnits.CurrentPage,
                    getAllUnits.HasNext,
                    getAllUnits.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all Units");
                var result = _mapper.Map<IEnumerable<UnitDto>>(getAllUnits);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Units Successfully";
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

        public async Task<IActionResult> GetAllActiveUnits([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<UnitDto>> serviceResponse = new ServiceResponse<IEnumerable<UnitDto>>();

            try
            {
                var allActiveUnits = await _repository.unitRepository.GetAllActiveUnits(pagingParameter, searchParams);
                _logger.LogInfo("Returned all Units");
                var result = _mapper.Map<IEnumerable<UnitDto>>(allActiveUnits);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Units Successfully";
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
        // GET api/<AuditFrequencyController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUnitById(int id)
        {
            ServiceResponse<UnitDto> serviceResponse = new ServiceResponse<UnitDto>();

            try
            {
                var unitDetail = await _repository.unitRepository.GetUnitById(id);
                if (unitDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Unit with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Unit with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned Unit with id: {id}");
                    var result = _mapper.Map<UnitDto>(unitDetail);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Unit with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUnitById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<AuditFrequencyController>
        [HttpPost]
        public IActionResult CreateUnit([FromBody] UnitPostDto unitPostDto)
        {
            ServiceResponse<UnitDto> serviceResponse = new ServiceResponse<UnitDto>();

            try
            {
                if (unitPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Unit object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Unit object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Unit object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Unit object sent from client.");

                    return BadRequest(serviceResponse);
                }
                var units = _mapper.Map<Unit>(unitPostDto);
                _repository.unitRepository.CreateUnit(units);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Unit Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetUnitById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateAuditFrequency action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<AuditFrequencyController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit(int id, [FromBody] UnitUpdateDto unitUpdateDto)
        {
            ServiceResponse<UnitDto> serviceResponse = new ServiceResponse<UnitDto>();

            try
            {
                if (unitUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Unit object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Unit object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update Unit object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Unit object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var unitUpdate = await _repository.unitRepository.GetUnitById(id);
                if (unitUpdate is null)
                {
                    _logger.LogError($"Update Unit with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Unit with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(unitUpdateDto, unitUpdate);
                string result = await _repository.unitRepository.UpdateUnit(unitUpdate);
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
                _logger.LogError($"Something went wrong inside UpdateUnit action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<AuditFrequencyController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            ServiceResponse<UnitDto> serviceResponse = new ServiceResponse<UnitDto>();

            try
            {
                var UnitDetail = await _repository.unitRepository.GetUnitById(id);
                if (UnitDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Unit object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete Unit with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.unitRepository.DeleteUnit(UnitDetail);
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
                _logger.LogError($"Something went wrong inside DeleteUnit action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateUnit(int id)
        {
            ServiceResponse<UnitDto> serviceResponse = new ServiceResponse<UnitDto>();

            try
            {
                var UnitDetail = await _repository.unitRepository.GetUnitById(id);
                if (UnitDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Unit object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Unit with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                UnitDetail.IsActive = true;
                string result = await _repository.unitRepository.UpdateUnit(UnitDetail);
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
                _logger.LogError($"Something went wrong inside ActivatedUnit action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeActivateUnit(int id)
        {
            ServiceResponse<UnitDto> serviceResponse = new ServiceResponse<UnitDto>();

            try
            {
                var UnitDetail = await _repository.unitRepository.GetUnitById(id);
                if (UnitDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Unit object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Unit with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                UnitDetail.IsActive = true;
                string result = await _repository.unitRepository.UpdateUnit(UnitDetail);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeactivatedUnit action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}