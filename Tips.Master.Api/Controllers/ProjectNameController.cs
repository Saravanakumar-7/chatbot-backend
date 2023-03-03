using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProjectNameController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public ProjectNameController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllProjectName()
        {
            ServiceResponse<IEnumerable<ProjectNameDto>> serviceResponse = new ServiceResponse<IEnumerable<ProjectNameDto>>();
            try
            {

                var projectNamesList = await _repository.ProjectNameRepository.GetAllProjectName();
                _logger.LogInfo("Returned all ProjectNames");
                var result = _mapper.Map<IEnumerable<ProjectNameDto>>(projectNamesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all projectNamesList  Successfully";
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
        public async Task<IActionResult> GetAllActiveProjectName()
        {
            ServiceResponse<IEnumerable<ProjectNameDto>> serviceResponse = new ServiceResponse<IEnumerable<ProjectNameDto>>();

            try
            {
                var projectNamesList = await _repository.ProjectNameRepository.GetAllActiveProjectName();
                _logger.LogInfo("Returned all projectNames");
                var result = _mapper.Map<IEnumerable<ProjectNameDto>>(projectNamesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active projectNames Successfully";
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
        // GET api/<DemoStatusController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectNameById(int id)
        {
            ServiceResponse<ProjectNameDto> serviceResponse = new ServiceResponse<ProjectNameDto>();

            try
            {
                var sourceDetailById = await _repository.ProjectNameRepository.GetProjectNameById(id);
                if (sourceDetailById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"projectName with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"projectName with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned ProjectName with id: {id}");
                    var result = _mapper.Map<ProjectNameDto>(sourceDetailById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned ProjectName with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetProjectNameById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateProjectName([FromBody] ProjectNamePostDto projectNamePostDto)
        {
            ServiceResponse<ProjectNamePostDto> serviceResponse = new ServiceResponse<ProjectNamePostDto>();

            try
            {
                if (projectNamePostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "projectName object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("projectName object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid projectName object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid projectName object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var projectNames = _mapper.Map<ProjectName>(projectNamePostDto);
                _repository.ProjectNameRepository.CreateProjectName(projectNames);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "projectName Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetprojectNameById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateprojectName action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProjectName(int id, [FromBody] ProjectNameUpdateDto projectNameUpdateDto)
        {
            ServiceResponse<ProjectNameUpdateDto> serviceResponse = new ServiceResponse<ProjectNameUpdateDto>();

            try
            {
                if (projectNameUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update projectName object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update projectName object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update projectName object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update projectName object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var projectnameDetail = await _repository.ProjectNameRepository.GetProjectNameById(id);
                if (projectnameDetail is null)
                {
                    _logger.LogError($"Update projectName with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update projectName with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(projectNameUpdateDto, projectnameDetail);
                string result = await _repository.ProjectNameRepository.UpdateProjectName(projectnameDetail);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateProjectName action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectName(int id)
        {
            ServiceResponse<ProjectNameDto> serviceResponse = new ServiceResponse<ProjectNameDto>();

            try
            {
                var projectnameDetail = await _repository.ProjectNameRepository.GetProjectNameById(id);
                if (projectnameDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete projectname object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete projectname with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.ProjectNameRepository.DeleteProjectName(projectnameDetail);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteProjectName action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateProjectName(int id)
        {
            ServiceResponse<ProjectNameDto> serviceResponse = new ServiceResponse<ProjectNameDto>();

            try
            {
                var ProjectName = await _repository.ProjectNameRepository.GetProjectNameById(id);
                if (ProjectName is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "ProjectName object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"ProjectName with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                ProjectName.IsActive = true;
                string result = await _repository.ProjectNameRepository.UpdateProjectName(ProjectName);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside ActivateProjectName action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateProjectName(int id)
        {
            ServiceResponse<ProjectNameDto> serviceResponse = new ServiceResponse<ProjectNameDto>();

            try
            {
                var projectName = await _repository.ProjectNameRepository.GetProjectNameById(id);
                if (projectName is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BHK object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"BHK with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                projectName.IsActive = false;
                string result = await _repository.ProjectNameRepository.UpdateProjectName(projectName);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivatedprojectName action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}

