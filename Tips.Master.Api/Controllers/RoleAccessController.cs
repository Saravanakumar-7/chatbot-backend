using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoleAccessController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public RoleAccessController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoleAccess([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<RoleAccessDto>> serviceResponse = new ServiceResponse<IEnumerable<RoleAccessDto>>();
            try
            {

                var roleAccessDetails = await _repository.RoleAccessRepository.GetAllRoleAccess(pagingParameter, searchParams);

                var metadata = new
                {
                    roleAccessDetails.TotalCount,
                    roleAccessDetails.PageSize,
                    roleAccessDetails.CurrentPage,
                    roleAccessDetails.HasNext,
                    roleAccessDetails.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all RoleAccess");
                var result = _mapper.Map<IEnumerable<RoleAccessDto>>(roleAccessDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RoleAccessDetails  Successfully";
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
        public async Task<IActionResult> GetAllActiveRoleAccess([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<RoleAccessDto>> serviceResponse = new ServiceResponse<IEnumerable<RoleAccessDto>>();

            try
            {
                var activeRoleAccessDetails = await _repository.RoleAccessRepository.GetAllActiveRoleAccess(pagingParameter, searchParams);
                _logger.LogInfo("Returned all RoleAccess");
                var result = _mapper.Map<IEnumerable<RoleAccessDto>>(activeRoleAccessDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active RoleAccess Successfully";
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
        public async Task<IActionResult> GetRoleAccessById(int id)
        {
            ServiceResponse<RoleAccessDto> serviceResponse = new ServiceResponse<RoleAccessDto>();

            try
            {
                var roleAccessById = await _repository.RoleAccessRepository.GetRoleAccessById(id);
                if (roleAccessById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RoleAccess hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"RoleAccess with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned RoleAccess with id: {id}");
                    var result = _mapper.Map<RoleAccessDto>(roleAccessById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned RoleAccess with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRoleAccessById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleAccessByRoleId(int roleId)
        {
            ServiceResponse<RoleAccessDto> serviceResponse = new ServiceResponse<RoleAccessDto>();

            try
            {
                var roleAccessByRoleId = await _repository.RoleAccessRepository.GetRoleAccessByRoleId(roleId);
                                

                if (roleAccessByRoleId == null)
                {
                    //serviceResponse.Data = null;
                    //serviceResponse.Message = $"RoleAccess hasn't been found in db.";
                    //serviceResponse.Success = false;
                    //serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    //_logger.LogError($"RoleAccess with Roleid: {roleId}, hasn't been found in db.");
                    //return BadRequest(serviceResponse);

                    return null;
                }
                else
                {

                    _logger.LogInfo($"Returned RoleAccess with id: {roleId}");
                    var result = _mapper.Map<RoleAccessDto>(roleAccessByRoleId);
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

        [HttpPost]
        public async Task<IActionResult> CreateRoleAccess([FromBody] List<RoleAccessPostDto> roleAccessPostDto)
        {
            ServiceResponse<RoleAccessPostDto> serviceResponse = new ServiceResponse<RoleAccessPostDto>();

            try
            {
                if (roleAccessPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RoleAccess object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("RoleAccess object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid RoleAccess object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid RoleAccess object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var roleAccess = _mapper.Map<List<RoleAccess>>(roleAccessPostDto);
                var roleId = roleAccess[0].RoleId;
                var roleAccessDetails = await _repository.RoleAccessRepository.GetRoleAccessByRoleId(roleId);
                for (int i = 0; i < roleAccess.Count; i++)
                {                   
                    if (roleAccessDetails.Count() != 0)
                    { 
                             await UpdateRoleAccess(roleAccessDetails[i].Id, roleAccessPostDto[i]); 
                    }
                    else
                    {
                        await _repository.RoleAccessRepository.CreateRoleAccess(roleAccess[i]);
                        _repository.SaveAsync();
                    }
                }
                //_repository.RoleAccessRepository.CreateRoleAccess(roleAccess);
                //_repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RoleAccess Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetRoleAccessById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateRoleAccess action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoleAccess(int id, [FromBody] RoleAccessPostDto roleAccessUpdateDto)
        {
            ServiceResponse<RoleAccessPostDto> serviceResponse = new ServiceResponse<RoleAccessPostDto>();

            try
            {
                if (roleAccessUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update RoleAccess object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update RoleAccess object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update RoleAccess object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update RoleAccess object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var roleAccessDetailsById = await _repository.RoleAccessRepository.GetRoleAccessById(id);
                if (roleAccessDetailsById is null)
                {
                    _logger.LogError($"Update RoleAccess with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update RoleAccess hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(roleAccessUpdateDto, roleAccessDetailsById);
                string result = await _repository.RoleAccessRepository.UpdateRoleAccess(roleAccessDetailsById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RoleAccess Updated Successfully";
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
                _logger.LogError($"Something went wrong inside UpdateRoleAccess action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleAccess(int id)
        {
            ServiceResponse<RoleAccessDto> serviceResponse = new ServiceResponse<RoleAccessDto>();

            try
            {
                var roleAccessDetailsById = await _repository.RoleAccessRepository.GetRoleAccessById(id);
                if (roleAccessDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete RoleAccess object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete RoleAccess with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.RoleAccessRepository.DeleteRoleAccess(roleAccessDetailsById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "RoleAccess Deleted Successfully";
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
                _logger.LogError($"Something went wrong inside DeleteRoleAccess action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
