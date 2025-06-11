using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> GetAllSalutations([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<SalutationsDto>> serviceResponse = new ServiceResponse<IEnumerable<SalutationsDto>>();
            try
            {

                var SalutationsList = await _repository.SalutationsRepository.GetAllSalutations(searchParams);
                _logger.LogInfo("Returned all SalutationsList");
                var result = _mapper.Map<IEnumerable<SalutationsDto>>(SalutationsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Salutations Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllSalutations API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllSalutations API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveSalutations([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<SalutationsDto>> serviceResponse = new ServiceResponse<IEnumerable<SalutationsDto>>();

            try
            {
                var Salutations = await _repository.SalutationsRepository.GetAllActiveSalutations(searchParams);
                _logger.LogInfo("Returned all Salutations");
                var result = _mapper.Map<IEnumerable<SalutationsDto>>(Salutations);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Salutations Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveSalutations API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveSalutations API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

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

                    _logger.LogInfo($"Returned Salutations with id: {id}");
                    var result = _mapper.Map<SalutationsDto>(Salutations);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Salutations with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetSalutationsById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetSalutationsById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
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
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetSalutationsById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateSalutations API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateSalutations API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
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
                    serviceResponse.Message = "Update Salutation object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update Salutation object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Salutations object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Salutations object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var Salutations = await _repository.SalutationsRepository.GetSalutationsById(id);
                if (Salutations is null)
                {
                    _logger.LogError($"Update Salutations with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Salutations with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(salutationsDtoUpdate, Salutations);
                string result = await _repository.SalutationsRepository.UpdateSalutations(Salutations);
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
                serviceResponse.Message = $"Error Occured in UpdateSalutations API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateSalutations API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
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
                    serviceResponse.Message = "Delete Salutations object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete Salutations with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.SalutationsRepository.DeleteSalutations(Salutations);
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
                serviceResponse.Message = $"Error Occured in DeleteSalutations API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteSalutations API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
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
                    return BadRequest(serviceResponse);
                }
                Salutations.IsActive = true;
                string result = await _repository.SalutationsRepository.UpdateSalutations(Salutations);
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
                serviceResponse.Message = $"Error Occured in ActivateSalutations API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in ActivateSalutations API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
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
                    return BadRequest(serviceResponse);
                }
                Salutations.IsActive = false;
                string result = await _repository.SalutationsRepository.UpdateSalutations(Salutations);
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
                serviceResponse.Message = $"Error Occured in DeactivateSalutations API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in DeactivateSalutations API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }

        }
    }
}
