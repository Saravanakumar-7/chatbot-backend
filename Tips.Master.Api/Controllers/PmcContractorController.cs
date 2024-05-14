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
    public class PmcContractorController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public PmcContractorController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllPmcContractor()
        {
            ServiceResponse<IEnumerable<PmcContractorDto>> serviceResponse = new ServiceResponse<IEnumerable<PmcContractorDto>>();
            try
            {

                var pmcContractors = await _repository.PmcContractorRepository.GetAllPmcContractor();
                _logger.LogInfo("Returned all PmcContractors");
                var result = _mapper.Map<IEnumerable<PmcContractorDto>>(pmcContractors);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all pmcContractors  Successfully";
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
        public async Task<IActionResult> GetAllActivePmcContractor()
        {
            ServiceResponse<IEnumerable<PmcContractorDto>> serviceResponse = new ServiceResponse<IEnumerable<PmcContractorDto>>();

            try
            {
                var citiesList = await _repository.PmcContractorRepository.GetAllActivePmcContractor();
                _logger.LogInfo("Returned all PmcContractor");
                var result = _mapper.Map<IEnumerable<PmcContractorDto>>(citiesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active PmcContractor Successfully";
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
        public async Task<IActionResult> GetPmcContractorById(int id)
        {
            ServiceResponse<PmcContractorDto> serviceResponse = new ServiceResponse<PmcContractorDto>();

            try
            {
                var PmcContractorById = await _repository.PmcContractorRepository.GetPmcContractorById(id);
                if (PmcContractorById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PmcContractor with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PmcContractor with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned PmcContractor with id: {id}");
                    var result = _mapper.Map<PmcContractorDto>(PmcContractorById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned PmcContractor with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPmcContractorById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreatePmcContractor([FromBody] PmcContractorPostDto pmcContractorPostDto)
        {
            ServiceResponse<PmcContractorPostDto> serviceResponse = new ServiceResponse<PmcContractorPostDto>();

            try
            {
                if (pmcContractorPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PmcContractor object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PmcContractor object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PmcContractor object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PmcContractor object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var pmcContractor = _mapper.Map<PmcContractor>(pmcContractorPostDto);
                _repository.PmcContractorRepository.CreatePmcContractor(pmcContractor);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "PmcContractor Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetPmcContractorById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreatePmcContractor action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePmcContractor(int id, [FromBody] PmcContractorUpdateDto pmcContractorUpdateDto)
        {
            ServiceResponse<PmcContractorUpdateDto> serviceResponse = new ServiceResponse<PmcContractorUpdateDto>();

            try
            {
                if (pmcContractorUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update PmcContractor object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update PmcContractor object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update PmcContractor object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update PmcContractor object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var pmcContractorDetail = await _repository.PmcContractorRepository.GetPmcContractorById(id);
                if (pmcContractorDetail is null)
                {
                    _logger.LogError($"Update PmcContractor with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update PmcContractor with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(pmcContractorUpdateDto, pmcContractorDetail);
                string result = await _repository.PmcContractorRepository.UpdatePmcContractor(pmcContractorDetail);
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
                _logger.LogError($"Something went wrong inside UpdatePmcContractor action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePmcContractor(int id)
        {
            ServiceResponse<PmcContractorDto> serviceResponse = new ServiceResponse<PmcContractorDto>();

            try
            {
                var pmcContractorDetail = await _repository.PmcContractorRepository.GetPmcContractorById(id);
                if (pmcContractorDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete PmcContractor object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete PmcContractor with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.PmcContractorRepository.DeletePmcContractor(pmcContractorDetail);
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
                _logger.LogError($"Something went wrong inside DeletePmcContractor action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivatePmcContractor(int id)
        {
            ServiceResponse<PmcContractorDto> serviceResponse = new ServiceResponse<PmcContractorDto>();

            try
            {
                var PmcContractorDetail = await _repository.PmcContractorRepository.GetPmcContractorById(id);
                if (PmcContractorDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PmcContractor object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PmcContractor with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                PmcContractorDetail.IsActive = true;
                string result = await _repository.PmcContractorRepository.UpdatePmcContractor(PmcContractorDetail);
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
                _logger.LogError($"Something went wrong inside ActivatePmcContractor action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivatePmcContractor(int id)
        {
            ServiceResponse<PmcContractorDto> serviceResponse = new ServiceResponse<PmcContractorDto>();

            try
            {
                var PmcContractorDetail = await _repository.PmcContractorRepository.GetPmcContractorById(id);
                if (PmcContractorDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PmcContractor object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"PmcContractor with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                PmcContractorDetail.IsActive = true;
                string result = await _repository.PmcContractorRepository.UpdatePmcContractor(PmcContractorDetail);
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
                _logger.LogError($"Something went wrong inside DeactivatePmcContractor action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}

