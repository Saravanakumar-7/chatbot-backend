using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public LanguageController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<LanguageController>
        [HttpGet]
        public async Task<IActionResult> GetAllLanguages()
        {
            ServiceResponse<IEnumerable<LanguageDto>> serviceResponse = new ServiceResponse<IEnumerable<LanguageDto>>();
            try
            {

                var LanguageList = await _repository.LanguageRepository.GetAllLanguages();

                _logger.LogInfo("Returned all Languages");
                var result = _mapper.Map<IEnumerable<LanguageDto>>(LanguageList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Languages Successfully";
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
        public async Task<IActionResult> GetAllActiveLanguages()
        {
            ServiceResponse<IEnumerable<LanguageDto>> serviceResponse = new ServiceResponse<IEnumerable<LanguageDto>>();

            try
            {
                var Languages = await _repository.LanguageRepository.GetAllActiveLanguages();
                _logger.LogInfo("Returned all Languages");
                var result = _mapper.Map<IEnumerable<LanguageDto>>(Languages);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Languages Successfully";
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

        // GET api/<LanguageController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLanguageById(int id)
        {
            ServiceResponse<LanguageDto> serviceResponse = new ServiceResponse<LanguageDto>();

            try
            {
                var Language = await _repository.LanguageRepository.GetLanguageById(id);
                if (Language == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Language with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Language with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned Language with id: {id}");
                    var result = _mapper.Map<LanguageDto>(Language);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Language with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetLanguageById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<LanguageController>
        [HttpPost]
        public IActionResult CreateLanguage([FromBody] LanguageDtoPost languageDtoPost)
        {
            ServiceResponse<LanguageDto> serviceResponse = new ServiceResponse<LanguageDto>();

            try
            {
                if (languageDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Language object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Language object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Language object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Language object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var Language = _mapper.Map<Language>(languageDtoPost);
                _repository.LanguageRepository.CreateLanguage(Language);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetLanguageById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside Language action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<LanguageController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLanguage(int id, [FromBody] LanguageDtoUpdate languageDtoUpdate)
        {
            ServiceResponse<LanguageDto> serviceResponse = new ServiceResponse<LanguageDto>();

            try
            {
                if (languageDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Language object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Update Language object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Language object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Language object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var Language = await _repository.LanguageRepository.GetLanguageById(id);
                if (Language is null)
                {
                    _logger.LogError($"Update Language with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Language with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(languageDtoUpdate, Language);
                string result = await _repository.LanguageRepository.UpdateLanguage(Language);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateLanguage action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        // DELETE api/<LanguageController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            ServiceResponse<LanguageDto> serviceResponse = new ServiceResponse<LanguageDto>();

            try
            {
                var Language = await _repository.LanguageRepository.GetLanguageById(id);
                if (Language == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Language object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete Language with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.LanguageRepository.DeleteLanguage(Language);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteLanguage action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateLanguage(int id)
        {
            ServiceResponse<LanguageDto> serviceResponse = new ServiceResponse<LanguageDto>();

            try
            {
                var Language = await _repository.LanguageRepository.GetLanguageById(id);
                if (Language is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Language object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Language with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Language.IsActive = true;
                string result = await _repository.LanguageRepository.UpdateLanguage(Language);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside ActivateLanguage action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateLanguage(int id)
        {
            ServiceResponse<LanguageDto> serviceResponse = new ServiceResponse<LanguageDto>();

            try
            {
                var Language = await _repository.LanguageRepository.GetLanguageById(id);
                if (Language is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Language object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Language with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Language.IsActive = false;
                string result = await _repository.LanguageRepository.UpdateLanguage(Language);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeactivateLanguage action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }

        }
    }
}
