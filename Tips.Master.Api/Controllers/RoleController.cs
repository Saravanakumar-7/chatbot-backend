using System.Net;
using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public RoleController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
       // public async Task<IActionResult> GetAllRoles([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
            public async Task<IActionResult> GetAllRoles([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<RoleDto>> serviceResponse = new ServiceResponse<IEnumerable<RoleDto>>();
            try
            {

                var roleDetails = await _repository.RoleRepository.GetAllRoles(searchParams);

                //var metadata = new
                //{
                //    roleDetails.TotalCount,
                //    roleDetails.PageSize,
                //    roleDetails.CurrentPage,
                //    roleDetails.HasNext,
                //    roleDetails.HasPreviuos
                //};

                //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all Roles");
                var result = _mapper.Map<IEnumerable<RoleDto>>(roleDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RoleDetails  Successfully";
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
        public async Task<IActionResult> GetAllActiveRoles()
        {
            ServiceResponse<IEnumerable<RoleDto>> serviceResponse = new ServiceResponse<IEnumerable<RoleDto>>();

            try
            {
                var activeRolesDetails = await _repository.RoleRepository.GetAllActiveRoles();
                _logger.LogInfo("Returned all Roles");
                var result = _mapper.Map<IEnumerable<RoleDto>>(activeRolesDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Roles Successfully";
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
        public async Task<IActionResult> GetRoleById(int id)
        {
            ServiceResponse<RoleDto> serviceResponse = new ServiceResponse<RoleDto>();

            try
            {
                var roleById = await _repository.RoleRepository.GetRoleById(id);
                if (roleById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Role hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Role with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned Role with id: {id}");
                    var result = _mapper.Map<RoleDto>(roleById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Role with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRoleById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public IActionResult CreateRole([FromBody] RolePostDto rolePostDto)
        {
            ServiceResponse<RolePostDto> serviceResponse = new ServiceResponse<RolePostDto>();

            try
            {
                if (rolePostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Role object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Role object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Role object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Role object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var roles = _mapper.Map<Role>(rolePostDto);
                _repository.RoleRepository.CreateRole(roles);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Role Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetRoleById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateRole action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleUpdateDto roleUpdateDto)
        {
            ServiceResponse<RoleUpdateDto> serviceResponse = new ServiceResponse<RoleUpdateDto>();

            try
            {
                if (roleUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Role object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Role object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update Role object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Role object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var roleDetailsById = await _repository.RoleRepository.GetRoleById(id);
                if (roleDetailsById is null)
                {
                    _logger.LogError($"Update Role with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Role hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(roleUpdateDto, roleDetailsById);
                string result = await _repository.RoleRepository.UpdateRole(roleDetailsById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Role Updated Successfully";
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
                _logger.LogError($"Something went wrong inside UpdateRole action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            ServiceResponse<RoleDto> serviceResponse = new ServiceResponse<RoleDto>();

            try
            {
                var roleDetailsById = await _repository.RoleRepository.GetRoleById(id);
                if (roleDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Role object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete Role with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.RoleRepository.DeleteRole(roleDetailsById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Role Deleted Successfully";
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
                _logger.LogError($"Something went wrong inside DeleteRole action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
