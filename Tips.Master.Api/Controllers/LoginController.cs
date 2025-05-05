using Accounts;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using NuGet.Protocol;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private IUserRepository _userRepository;
        private IJwtAuth _jwtAuth;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public LoginController(IRepositoryWrapperForMaster repository, IUserRepository userRepository, IJwtAuth jwtAuth, ILoggerManager logger, IMapper mapper)
        {
            _userRepository=userRepository;
            _repository = repository;
            _jwtAuth = jwtAuth;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> LogInValidator([FromBody] LoginDto loginDto)
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
                ResetPW confirm = new ResetPW()
                {
                    UserName= loginDto.UserName,
                    Password=loginDto.Password,
                    UnitName=loginDto.UnitName,
                    NewPW="",
                    ConfirmPW=""
                };
                var (loginResult, userId, userName) = await _userRepository.ConfirmUser(confirm);

                switch (loginResult)
                {
                    case LoginResult.Success:
                        var Valid = await _repository.UserTokenActivitiesRepository.ValidateUser(userId);
                        if (Valid==1)
                        {
                            serviceResponse.Message = "The user is active else where. Would you like to close other sessions and log in here?";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.NotExtended;
                            _repository.SaveAsync();
                            break;
                        }
                        serviceResponse.Message = "User Exists Please Generate Token Now";
                        serviceResponse.Success = true;
                        serviceResponse.StatusCode = HttpStatusCode.OK;
                        break;

                    case LoginResult.UserNotFound:
                        serviceResponse.Message = "User does not exist";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        break;

                    case LoginResult.InvalidPassword:
                        serviceResponse.Message = "Invalid password";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.Unauthorized;
                        break;

                    case LoginResult.InvalidUnit:
                        serviceResponse.Message = "User does not exist in this unit";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        break;
                    case LoginResult.InvalidEntry:
                        serviceResponse.Message = "User has been Deactivated";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NonAuthoritativeInformation;
                        break;

                    default: // LoginResult.InvalidEntry
                        serviceResponse.Message = "Invalid entry";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        break;
                }
                serviceResponse.Data = null;
                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in LogInValidator: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in LogInValidator: {ex.Message} \n  {ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
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
                var (loginResult, token, userId, userName, Validity) = await _jwtAuth.GetToken(loginDto);

                LoginResponseDto loginResponseDto = new LoginResponseDto();
                loginResponseDto.Name = userName;
                loginResponseDto.UnitName = loginDto.UnitName;
                loginResponseDto.UserName = loginDto.UserName;

                switch (loginResult)
                {
                    case LoginResult.Success:
                        await _repository.UserTokenActivitiesRepository.UpdateToken(userId, token,Validity);
                        _repository.SaveAsync();
                        loginResponseDto.Token = token;
                        loginResponseDto.UserId = userId;
                        serviceResponse.Message = "Token Successfully Created";
                        serviceResponse.Success = true;
                        serviceResponse.StatusCode = HttpStatusCode.OK;
                        break;

                    case LoginResult.UserNotFound:
                        serviceResponse.Message = "User does not exist";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        break;

                    case LoginResult.InvalidPassword:
                        serviceResponse.Message = "Invalid password";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.Unauthorized;
                        break;

                    case LoginResult.InvalidUnit:
                        serviceResponse.Message = "User does not exist in this unit";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        break;
                    case LoginResult.InvalidEntry:
                        serviceResponse.Message = "User has been Deactivated";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NonAuthoritativeInformation;
                        break;

                    default: // LoginResult.InvalidEntry
                        serviceResponse.Message = "Invalid entry";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        break;
                }

                serviceResponse.Data = loginResponseDto;
                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GenerateUserToken: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in GenerateUserToken: {ex.Message} \n  {ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPW loginDto)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
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
                if (loginDto.UserName == "admin@mail.com")
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Admin's Password can not be changed";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotAcceptable;
                    _logger.LogError("Invalid User Data sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (loginDto.NewPW != loginDto.ConfirmPW)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "New PassWord is not Matching Confirmed Password";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.Forbidden;
                    _logger.LogError("Invalid User Data sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                var (loginResult, userId, userName) = await _userRepository.ConfirmUser(loginDto);

                switch (loginResult)
                {
                    case LoginResult.Success:
                        string newpw = await _userRepository.ResetPassword(userId, loginDto.NewPW, loginDto.ConfirmPW);
                        _repository.SaveAsync();
                        serviceResponse.Message = "New Password Created";
                        serviceResponse.Success = true;
                        serviceResponse.StatusCode = HttpStatusCode.OK;
                        break;

                    case LoginResult.UserNotFound:
                        serviceResponse.Message = "User does not exist";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        break;

                    case LoginResult.InvalidPassword:
                        serviceResponse.Message = "Invalid password";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.Unauthorized;
                        break;

                    case LoginResult.InvalidUnit:
                        serviceResponse.Message = "User does not exist in this unit";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        break;

                    default: // LoginResult.InvalidEntry
                        serviceResponse.Message = "Invalid entry";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        break;
                }
                serviceResponse.Data = loginDto.NewPW;
                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ResetPassword: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in ResetPassword: {ex.Message} \n  {ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserLogOut(int userId)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                var Userdetails = await _userRepository.FindUserDetailsByuserId(userId);
                if (Userdetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Unable to find the user data for UserID:{userId}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    _logger.LogError($"Unable to find the user data for UserID:{userId}");
                    return NotFound(serviceResponse);
                }
                var UserToken = await _repository.UserTokenActivitiesRepository.GetUserTokenDetailsByUserId(userId);
                if (UserToken == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Unable to find the UserToken data for UserID:{userId}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    _logger.LogError($"Unable to find the UserToken data for UserID:{userId}");
                    return NotFound(serviceResponse);
                }
                await _repository.UserTokenActivitiesRepository.DeactivateUserTokenByUserId(userId);
                _repository.SaveAsync();
                serviceResponse.Data = $"User: {Userdetails.UserName} has been Loged Out";
                serviceResponse.Message = $"User: {Userdetails.UserName} has been Loged Out";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                _logger.LogInfo($"User: {Userdetails.UserName} has been Loged Out");
                return StatusCode(200, serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UserLogOut for UserID:{userId} : {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UserLogOut for UserID:{userId} : {ex.Message} \n  {ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        //[HttpPut]
        //public async Task<IActionResult> ResetPassword(int Id, string NewPW, string ConfirmPW)
        //{
        //    ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
        //    try
        //    {
        //        if (Id == 0 || NewPW is null || ConfirmPW is null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "User Data sent";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("User Data sent from client is null");
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid User Data";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("Invalid User Data sent from client is null.");
        //            return BadRequest(serviceResponse);
        //        }
        //        if (NewPW != ConfirmPW)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "New PassWord is not Matching Confirmed Password";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.Forbidden;
        //            _logger.LogError("Invalid User Data sent from client is null.");
        //            return BadRequest(serviceResponse);
        //        }
        //        string newpw = await _userRepository.ResetPassword(Id, NewPW, ConfirmPW);
        //        _repository.SaveAsync();
        //        serviceResponse.Data = newpw;
        //        serviceResponse.Message = "New Password Created";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Created("GetOpenDeliveryOrderById", serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal Server Error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        _logger.LogError($"Something went wrong inside Conformation action: {ex.Message}");
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> GenerateUserToken([FromBody] LoginDto loginDto)
        //{
        //    ServiceResponse<LoginResponseDto> serviceResponse = new ServiceResponse<LoginResponseDto>();
        //    try
        //    {
        //        if (loginDto is null)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "User Data sent";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("User Data sent from client is null");
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid User Data";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            _logger.LogError("Invalid User Data sent from client is null.");
        //            return BadRequest(serviceResponse);
        //        }
        //        var (token, userId) = await _jwtAuth.GetToken(loginDto.UserName, loginDto.Password);

        //        LoginResponseDto loginResponseDto = new LoginResponseDto();
        //        loginResponseDto.Name = loginDto.UserName;
        //        loginResponseDto.UnitName = loginDto.UnitName;
        //        //loginResponseDto.Token = TokenDetails.Result;
        //        loginResponseDto.UnitName = loginDto.UnitName;
        //        loginResponseDto.Token = token;
        //        loginResponseDto.UserId = userId; 
        //        serviceResponse.Data = loginResponseDto;
        //        serviceResponse.Message = "Token Successfully Created";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal Server Error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        _logger.LogError($"Something went wrong inside Costcenter action: {ex.Message}");
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
    }
}
