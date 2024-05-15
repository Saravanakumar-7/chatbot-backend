using AutoMapper;
using Contracts;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class EmailIDsServiceController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryWrapperForMaster _repository;
        private IMapper _mapper;
        public EmailIDsServiceController(ILoggerManager logger, IRepositoryWrapperForMaster repository, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> GetEmailIdDetailsbyOperation(string Operations)
        {
            ServiceResponse<List<EmailIDsDto>> serviceResponse = new ServiceResponse<List<EmailIDsDto>>();
            try
            {
                if (Operations is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Operations object is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Operations object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Operations object.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Operations object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var EmailIds = _repository.EmailIDsRepository.GetEmailIdDetailsbyOperation(Operations);
                var result =_mapper.Map<List<EmailIDsDto>>(EmailIds);
                serviceResponse.Data = result;
                serviceResponse.Message = $"EmailDetails Return for {Operations}";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetEmailIdDetailsbyOperation  action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
