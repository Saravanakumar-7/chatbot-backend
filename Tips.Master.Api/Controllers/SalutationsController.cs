using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalutationsController : ControllerBase
    {  
        private IRepositoryWrapperForMaster _repository;
            private ILoggerManager _logger;
            private IMapper _mapper;
            public SalutationsController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
            {
                _repository = repository;
                _logger = logger;
                _mapper = mapper;
            }
            // GET: api/<SalutationsController>
            [HttpGet]
        public async Task<IActionResult> GetAllSalutations()
        {
            ServiceResponse<IEnumerable<SalutationsDto>> serviceResponse = new ServiceResponse<IEnumerable<SalutationsDto>>();
            try
            {

                var SalutationsList = await _repository.SalutationsRepository.GetAllSalutations();
                _logger.LogInfo("Returned all SalutationsList");
                var result = _mapper.Map<IEnumerable<SalutationsDto>>(SalutationsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllActiveSalutations()
        {
            ServiceResponse<IEnumerable<SalutationsDto>> serviceResponse = new ServiceResponse<IEnumerable<SalutationsDto>>();

            try
            {
                var Salutations = await _repository.SalutationsRepository.GetAllActiveSalutations();
                _logger.LogInfo("Returned all Salutations");
                var result = _mapper.Map<IEnumerable<SalutationsDto>>(Salutations);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                return StatusCode(500, "Internal server error");

            }
        }
        // GET api/<SalutationsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalutationsById(int id)
        {
            ServiceResponse<SalutationsDto> serviceResponse = new ServiceResponse<SalutationsDto>();

            try
            {
                var Salutations = await _repository.SalutationsRepository.GetSalutationsById(id);
                if (Salutations == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Salutations with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Salutations with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<SalutationsDto>(Salutations);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSalutationsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/<SalutationsController>
        [HttpPost]
        public IActionResult CreateSalutations([FromBody] SalutationsDtoPost salutationsDtoPost)
        {
            ServiceResponse<SalutationsDto> serviceResponse = new ServiceResponse<SalutationsDto>();

            try
            {
                if (salutationsDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Salutations object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Salutations object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Salutations object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Salutations object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var Salutations = _mapper.Map<Salutations>(salutationsDtoPost);
                _repository.SalutationsRepository.CreateSalutations(Salutations);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfylly Created";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;


                return Created("GetSalutationsById", "Successfully Created");
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<SalutationsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalutations(int id, [FromBody] SalutationsDtoUpdate salutationsDtoUpdate)
        {
            ServiceResponse<SalutationsDto> serviceResponse = new ServiceResponse<SalutationsDto>();

            try
            {
                if (salutationsDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Salutation object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Salutation object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Salutations object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Salutations object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var Salutations = await _repository.SalutationsRepository.GetSalutationsById(id);
                if (Salutations is null)
                {
                    _logger.LogError($"Salutations with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(salutationsDtoUpdate, Salutations);
                string result = await _repository.SalutationsRepository.UpdateSalutations(Salutations);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside UpdateSalutations action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<SalutationsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalutations(int id)
        {
            ServiceResponse<SalutationsDto> serviceResponse = new ServiceResponse<SalutationsDto>();

            try
            {
                var Salutations = await _repository.SalutationsRepository.GetSalutationsById(id);
                if (Salutations == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Salutations object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Salutations with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                string result = await _repository.SalutationsRepository.DeleteSalutations(Salutations);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateSalutations(int id)
        {
            ServiceResponse<SalutationsDto> serviceResponse = new ServiceResponse<SalutationsDto>();

            try
            {
                var Salutations = await _repository.SalutationsRepository.GetSalutationsById(id);
                if (Salutations is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Salutations object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Salutations with id: {id}, hasn't been found in db.");
                    return BadRequest("Salutations object is null");
                }
                Salutations.IsActive = true;
                string result = await _repository.SalutationsRepository.UpdateSalutations(Salutations);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateSalutations action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateSalutations(int id)
        {
            ServiceResponse<SalutationsDto> serviceResponse = new ServiceResponse<SalutationsDto>();

            try
            {
                var Salutations = await _repository.SalutationsRepository.GetSalutationsById(id);
                if (Salutations is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Salutations object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Salutations with id: {id}, hasn't been found in db.");
                    return BadRequest("Salutations object is null");
                }
                Salutations.IsActive = false;
                string result = await _repository.SalutationsRepository.UpdateSalutations(Salutations);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deactivated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return NoContent();
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateSalutations action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
