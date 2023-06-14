using Accounts;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Protocol;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private IJwtAuth _jwtAuth;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public LoginController(IRepositoryWrapperForMaster repository, IJwtAuth jwtAuth, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _jwtAuth = jwtAuth;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateUserToken([FromBody] LoginDto loginDto)
        {
            ServiceResponse<LoginResponseDto> serviceResponse = new ServiceResponse<LoginResponseDto>();
            try
            {
                if (loginDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "User Data sent";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("User Data sent from client is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid User Data";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid User Data sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                var (token, userId) = await _jwtAuth.GetToken(loginDto.UserName, loginDto.Password);
                 
                LoginResponseDto loginResponseDto = new LoginResponseDto();
                loginResponseDto.UserName = loginDto.UserName;
                loginResponseDto.UnitName = loginDto.UnitName;
                //loginResponseDto.Token = TokenDetails.Result;
                loginResponseDto.UnitName = loginDto.UnitName;
                loginResponseDto.Token = token;
                loginResponseDto.UserId = userId; 
                serviceResponse.Data = loginResponseDto;
                serviceResponse.Message = "Token Successfully Created";
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
                _logger.LogError($"Something went wrong inside Costcenter action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
