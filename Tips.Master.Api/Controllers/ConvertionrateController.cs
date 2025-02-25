using System.Net;
using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Numerics;
using System.Xml;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ConvertionrateController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public ConvertionrateController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLatestConvertionrate([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ConvertionrateDto>> serviceResponse = new ServiceResponse<IEnumerable<ConvertionrateDto>>();
            try
            {
                var convertionrateList = await _repository.ConvertionrateRepository.GetAllLatestConvertionrate(searchParams);
                _logger.LogInfo("Returned all Convertionrate");
                var result = _mapper.Map<IEnumerable<ConvertionrateDto>>(convertionrateList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Convertionrate Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something Error Occured in GetAllLatestConvertionrate: {ex.Message} \n {ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something Error Occured in GetAllLatestConvertionrate: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }      

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConvertionrateById(int id)
        {
            ServiceResponse<ConvertionrateDto> serviceResponse = new ServiceResponse<ConvertionrateDto>();

            try
            {
                var convertionrate = await _repository.ConvertionrateRepository.GetConvertionrateById(id);
                if (convertionrate == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Convertionrate with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Convertionrate with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    var result = _mapper.Map<ConvertionrateDto>(convertionrate);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Convertionrate with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetConvertionrateById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetConvertionrateById action: {ex.Message} \n {ex.InnerException}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateConvertionrate([FromBody] ConvertionratePostDto convertionrateDtoPost)
        {
            ServiceResponse<ConvertionrateDto> serviceResponse = new ServiceResponse<ConvertionrateDto>();
            try
            {
                if (convertionrateDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Convertionrate object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Convertionrate object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Convertionrate object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Convertionrate object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var existingConver =await _repository.ConvertionrateRepository.GetLatestConvertionrateByUOC(convertionrateDtoPost.UOC);
                if (existingConver != null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Convertionrate for UOC:{convertionrateDtoPost.UOC} already exists please edit existing value";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Convertionrate for UOC:{convertionrateDtoPost.UOC} already exists please edit existing value");
                    return BadRequest(serviceResponse);
                }
                var convertionrateEntity = _mapper.Map<Convertionrate>(convertionrateDtoPost);
                convertionrateEntity.Version = 1;
                await _repository.ConvertionrateRepository.CreateConvertionrate(convertionrateEntity);
                _repository.SaveAsync();
                serviceResponse.Message = "Convertionrate Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("CreateConvertionrate", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateConvertionrate action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside CreateConvertionrate action: {ex.Message} \n {ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConvertionrate(int id, [FromBody] ConvertionrateUpdateDto convertionrateDtoUpdate)
        {
            ServiceResponse<ConvertionrateDto> serviceResponse = new ServiceResponse<ConvertionrateDto>();

            try
            {
                if (convertionrateDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Convertionrate object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Convertionrate object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Convertionrate object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Convertionrate object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var convertionrateEntity = await _repository.ConvertionrateRepository.GetConvertionrateById(id);
                if (convertionrateEntity is null)
                {
                    _logger.LogError($"Update Convertionrate with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Convertionrate with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.ConvertionrateRepository.UpdateConvertionrate(convertionrateEntity);
                _repository.SaveAsync();
                var NewConv = _mapper.Map(convertionrateDtoUpdate, convertionrateEntity);
                NewConv.Id = 0;
                NewConv.Version = convertionrateEntity.Version + 1;
                NewConv.LastModifiedBy = null;
                NewConv.LastModifiedOn = null;
                await _repository.ConvertionrateRepository.CreateConvertionrate(NewConv);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Convertionrate Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside UpdateConvertionrate action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateConvertionrate action: {ex.Message} \n {ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestConvertionrateByUOC(string currency)
        {
            ServiceResponse<ConvertionrateDto> serviceResponse = new ServiceResponse<ConvertionrateDto>();
            try
            {
                var currrentrate = await _repository.ConvertionrateRepository.GetLatestConvertionrateByUOC(currency);
                var result = _mapper.Map<ConvertionrateDto>(currrentrate);
                _logger.LogInfo("Returned all Convertionrate");
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Convertionrate Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetLatestConvertionrateByUOC action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside GetLatestConvertionrateByUOC action: {ex.Message} \n {ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
