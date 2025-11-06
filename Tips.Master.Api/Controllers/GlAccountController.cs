using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Net;
using System.Security.Claims;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class GlAccountController : ControllerBase
    {
        private readonly IRepositoryWrapperForMaster _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        public GlAccountController(
            IRepositoryWrapperForMaster repository,
            ILoggerManager logger,
            IMapper mapper,
            IHttpClientFactory clientFactory,
            IConfiguration config)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _clientFactory = clientFactory;
            _config = config;
        }

        // GET: api/<GlAccountController>
        [HttpGet]
        public async Task<IActionResult> GetAllGlAccount([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<GlAccountDto>> serviceResponse = new ServiceResponse<IEnumerable<GlAccountDto>>();

            try
            {
                var GlAccountList = await _repository.GLAccountsRepository.GetAllGLAccounts(searchParams);
                _logger.LogInfo("Returned all GlAccount");
                var result = _mapper.Map<IEnumerable<GlAccountDto>>(GlAccountList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all GlAccount Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllGlAccount API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllGlAccount API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveGlAccounts([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<GlAccountDto>> serviceResponse = new ServiceResponse<IEnumerable<GlAccountDto>>();

            try
            {
                var GlAccountList = await _repository.GLAccountsRepository.GetAllActiveGLAccounts(searchParams);
                _logger.LogInfo("Returned all GlAccount");
                var result = _mapper.Map<IEnumerable<GlAccountDto>>(GlAccountList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active GlAccount Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveGlAccounts API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveGlAccounts API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }
        // GET api/<GlAccountController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGlAccountById(int id)
        {
            ServiceResponse<GlAccountDto> serviceResponse = new ServiceResponse<GlAccountDto>();

            try
            {
                var GlAccount = await _repository.GLAccountsRepository.GetGLAccountById(id);
                if (GlAccount == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"GlAccount with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"GlAccount with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<GlAccountDto>(GlAccount);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetGlAccountById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetGlAccountById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<GlAccountController>
        [HttpPost]
        public IActionResult CreateGlAccount([FromBody] GlAccountPostDto glAccountDtoPost)
        {
            ServiceResponse<GlAccountDto> serviceResponse = new ServiceResponse<GlAccountDto>();

            try
            {
                if (glAccountDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GlAccount object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("GlAccount object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid GlAccount object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid GlAccount object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var GlAccountEntity = _mapper.Map<GlAccounts>(glAccountDtoPost);
                _repository.GLAccountsRepository.CreateGLAccount(GlAccountEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetGlAccountById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateGlAccount API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateGlAccount API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<GlAccountController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGlAccount(int id, [FromBody] GlAccountUpdateDto glAccountUpdateDto)
        {
            ServiceResponse<GlAccountDto> serviceResponse = new ServiceResponse<GlAccountDto>();

            try
            {
                if (glAccountUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("GlAccount object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Department object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update GlAccount object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var GlAccountEntity = await _repository.GLAccountsRepository.GetGLAccountById(id);
                if (GlAccountEntity is null)
                {
                    _logger.LogError($"GlAccount with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(glAccountUpdateDto, GlAccountEntity);
                string result = await _repository.GLAccountsRepository.UpdateGLAccount(GlAccountEntity);
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
                serviceResponse.Message = $"Error Occured in UpdateGlAccount API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateGlAccount API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<GlAccountController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGlAccount(int id)
        {
            ServiceResponse<GlAccountDto> serviceResponse = new ServiceResponse<GlAccountDto>();

            try
            {
                var GlAccount = await _repository.GLAccountsRepository.GetGLAccountById(id);
                if (GlAccount == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "c GlAccount object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"GlAccount GlAccount with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.GLAccountsRepository.DeleteGLAccount(GlAccount);
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
                serviceResponse.Message = $"Error Occured in DeleteGlAccount API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteGlAccount API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateGlAccount(int id)
        {
            ServiceResponse<GlAccountDto> serviceResponse = new ServiceResponse<GlAccountDto>();

            try
            {
                var GlAccount = await _repository.GLAccountsRepository.GetGLAccountById(id);
                if (GlAccount is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GlAccount object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"GlAccount with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                GlAccount.IsActive = true;
                string result = await _repository.GLAccountsRepository.UpdateGLAccount(GlAccount);
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
                serviceResponse.Message = $"Error Occured in ActivateGlAccount API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in ActivateGlAccount API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateGlAccount(int id)
        {
            ServiceResponse<GlAccountDto> serviceResponse = new ServiceResponse<GlAccountDto>();

            try
            {
                var GlAccount = await _repository.GLAccountsRepository.GetGLAccountById(id);
                if (GlAccount is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "GlAccount object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"GlAccount with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                GlAccount.IsActive = false;
                string result = await _repository.GLAccountsRepository.UpdateGLAccount(GlAccount);
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
                serviceResponse.Message = $"Error Occured in DeactivateGlAccount API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in DeactivateGlAccount API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
