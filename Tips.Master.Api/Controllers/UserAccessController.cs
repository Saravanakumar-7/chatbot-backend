using System.Net;
using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO;
using MySqlX.XDevAPI.Common;
using Microsoft.AspNetCore.Authorization;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserAccessController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public UserAccessController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserAccess([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<UserAccessDto>> serviceResponse = new ServiceResponse<IEnumerable<UserAccessDto>>();
            try
            {

                var userAccessDetails = await _repository.UserAccessRepository.GetAllUserAccess(pagingParameter, searchParams);

                var metadata = new
                {
                    userAccessDetails.TotalCount,
                    userAccessDetails.PageSize,
                    userAccessDetails.CurrentPage,
                    userAccessDetails.HasNext,
                    userAccessDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all UserAccess");
                var result = _mapper.Map<IEnumerable<UserAccessDto>>(userAccessDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all UserAccessDetails  Successfully";
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
        public async Task<IActionResult> GetAllActiveUserAccess([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<UserAccessDto>> serviceResponse = new ServiceResponse<IEnumerable<UserAccessDto>>();

            try
            {
                var activeUserAccessDetails = await _repository.UserAccessRepository.GetAllActiveUserAccess(pagingParameter, searchParams);
                _logger.LogInfo("Returned all UserAccess");
                var result = _mapper.Map<IEnumerable<UserAccessDto>>(activeUserAccessDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active UserAccess Successfully";
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAccessById(int id)
        {
            ServiceResponse<UserAccessDto> serviceResponse = new ServiceResponse<UserAccessDto>();

            try
            {
                var userAccessById = await _repository.UserAccessRepository.GetUserAccessById(id);
                if (userAccessById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"UserAccess hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"UserAccess with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned UserAccess with id: {id}");
                    var result = _mapper.Map<UserAccessDto>(userAccessById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned UserAccess with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUserAccessById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        //test
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleAccessByRoleId(int roleId)
        {
            ServiceResponse<IEnumerable<RoleAccessDto>> serviceResponse = new ServiceResponse<IEnumerable<RoleAccessDto>>();

            try
            {
                var roleAccessByRoleId = await _repository.RoleAccessRepository.GetRoleAccessByRoleId(roleId);


                if (roleAccessByRoleId.Count() == 0)
                {
                    var formAccessList = await _repository.FormsAccessRepository.GetAllFormsAccess();
                    var formAccess = _mapper.Map<List<RoleAccessDto>>(formAccessList);
                    serviceResponse.Data = formAccess;
                    serviceResponse.Message = "Returned FormAccessDetials Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned RoleAccess with id: {roleId}");
                    var result = _mapper.Map<List<RoleAccessDto>>(roleAccessByRoleId);

                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned RoleAccess with Roleid successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                } 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRoleAccessByRoleId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserAccessByUserId(int userId)
        {
            ServiceResponse<IEnumerable<UserAccessDto>> serviceResponse = new ServiceResponse<IEnumerable<UserAccessDto>>();

            try
            {
                var userAccessByRoleId = await _repository.UserAccessRepository.GetUserAccessByUserId(userId);

                if (userAccessByRoleId.Count() == 0)
                {
                    var getRoleId = await _repository.RegistrationFormRepository.GetRegistrationUserById(userId);

                    var roleAccessByRoleId = await _repository.RoleAccessRepository.GetRoleAccessByRoleId(getRoleId);


                    if (roleAccessByRoleId.Count() != 0)
                    {
                      var result =  await GetRoleAccessByRoleId(getRoleId);
                      return result;
                    }
                    else
                    {

                        //if (roleAccessByRoleId.Count() != 0)
                        //{
                        //    var result = _mapper.Map<List<RoleAccessDto>>(roleAccessByRoleId);
                        //    serviceResponse.Data = result;
                        //    serviceResponse.Message = "Returned RoleAccessDetials Successfully";
                        //    serviceResponse.Success = true;
                        //    serviceResponse.StatusCode = HttpStatusCode.OK;
                        //    return Ok(serviceResponse);
                        //}
                        var formAccessList = await _repository.FormsAccessRepository.GetAllFormsAccess();
                        var formAccess = _mapper.Map<List<UserAccessDto>>(formAccessList);
                        serviceResponse.Data = formAccess;
                        serviceResponse.Message = "Returned FormAccessDetials Successfully";
                        serviceResponse.Success = true;
                        serviceResponse.StatusCode = HttpStatusCode.OK;
                        return Ok(serviceResponse);
                    }
                   

                }
                else
                {

                    _logger.LogInfo($"Returned UserAccess with id: {userId}");
                    var result = _mapper.Map<List<UserAccessDto>>(userAccessByRoleId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned UserAccess with Userid successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetUserAccessByUserId action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAccess([FromBody] List<UserAccessPostDto> userAccessPostDto)
        {
            ServiceResponse<UserAccessPostDto> serviceResponse = new ServiceResponse<UserAccessPostDto>();

            try
            {
                if (userAccessPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "UserAccess object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("UserAccess object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid UserAccess object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid UserAccess object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var userAccess = _mapper.Map<List<UserAccess>>(userAccessPostDto);
                var userId = userAccess[0].UserId;
                var userAccessDetails = await _repository.UserAccessRepository.GetUserAccessByUserId(userId);
                for (int i = 0; i < userAccess.Count; i++)
                {
                    if (userAccessDetails.Count() != 0)
                    {
                        await UpdateUserAccess(userAccessDetails[i].Id, userAccessPostDto[i]);
                    }
                    else
                    {
                        await _repository.UserAccessRepository.CreateUserAccess(userAccess[i]);
                        _repository.SaveAsync();
                    }
                }
                serviceResponse.Data = null;
                serviceResponse.Message = "UserAccess Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetUserAccessById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateUserAccess action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAccess(int id, [FromBody] UserAccessPostDto userAccessUpdateDto)
        {
            ServiceResponse<UserAccessUpdateDto> serviceResponse = new ServiceResponse<UserAccessUpdateDto>();

            try
            {
                if (userAccessUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update UserAccess object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update UserAccess object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update UserAccess object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update UserAccess object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var userAccessDetailsById = await _repository.UserAccessRepository.GetUserAccessById(id);
                if (userAccessDetailsById is null)
                {
                    _logger.LogError($"Update UserAccess with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update UserAccess hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(userAccessUpdateDto, userAccessDetailsById);
                string result = await _repository.UserAccessRepository.UpdateUserAccess(userAccessDetailsById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "UserAccess Updated Successfully";
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
                _logger.LogError($"Something went wrong inside UpdateUserAccess action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAccess(int id)
        {
            ServiceResponse<UserAccessDto> serviceResponse = new ServiceResponse<UserAccessDto>();

            try
            {
                var userAccessDetailsById = await _repository.UserAccessRepository.GetUserAccessById(id);
                if (userAccessDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete UserAccess object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete UserAccess with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.UserAccessRepository.DeleteUserAccess(userAccessDetailsById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "UserAccess Deleted Successfully";
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
                _logger.LogError($"Something went wrong inside DeleteUserAccess action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
