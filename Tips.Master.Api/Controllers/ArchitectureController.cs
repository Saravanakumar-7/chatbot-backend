using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ArchitectureController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public ArchitectureController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllArchitectures([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ArchitectureDto>> serviceResponse = new ServiceResponse<IEnumerable<ArchitectureDto>>();
            try
            {

                var architectList = await _repository.ArchitectureRepository.GetAllArchitectures(pagingParameter, searchParams);

                var metadata = new
                {
                    architectList.TotalCount,
                    architectList.PageSize,
                    architectList.CurrentPage,
                    architectList.HasNext,
                    architectList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all BHK");
                var result = _mapper.Map<IEnumerable<ArchitectureDto>>(architectList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all architectList  Successfully";
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
        public async Task<IActionResult> GetAllArchitecturesDetails()
        {
            ServiceResponse<IEnumerable<ArchitectureDto>> serviceResponse = new ServiceResponse<IEnumerable<ArchitectureDto>>();
            try
            {

                var architectList = await _repository.ArchitectureRepository.GetAllArchitecturesDetails();


                _logger.LogInfo("Returned all Architectures");
                var result = _mapper.Map<IEnumerable<ArchitectureDto>>(architectList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Architectures  Successfully";
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
        public async Task<IActionResult> GetAllActiveArchitectures([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<ArchitectureDto>> serviceResponse = new ServiceResponse<IEnumerable<ArchitectureDto>>();

            try
            {
                var architectList = await _repository.ArchitectureRepository.GetAllActiveArchitectures(pagingParameter, searchParams);
                _logger.LogInfo("Returned all architectList");
                var result = _mapper.Map<IEnumerable<ArchitectureDto>>(architectList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active architects Successfully";
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
        public async Task<IActionResult> GetArchitectureById(int id)
        {
            ServiceResponse<ArchitectureDto> serviceResponse = new ServiceResponse<ArchitectureDto>();

            try
            {
                var architectById = await _repository.ArchitectureRepository.GetArchitectureById(id);
                if (architectById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Archtecture with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Archtecture with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned Archtecture with id: {id}");
                    var result = _mapper.Map<ArchitectureDto>(architectById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Archtecture with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetArchtectureById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateArchitecture([FromBody] ArchitecturePostDto architecturePostDto)
        {
            ServiceResponse<ArchitecturePostDto> serviceResponse = new ServiceResponse<ArchitecturePostDto>();

            try
            {
                if (architecturePostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Architecture object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Architecture object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Architecture object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Architecture object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var architect = _mapper.Map<Architectures>(architecturePostDto);
                _repository.ArchitectureRepository.CreateArchitecture(architect);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "architecture Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetArchitectureById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateArcitecture action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArchitecture(int id, [FromBody] ArchitectureUpdateDto architectureUpdateDto)
        {
            ServiceResponse<ArchitectureUpdateDto> serviceResponse = new ServiceResponse<ArchitectureUpdateDto>();

            try
            {
                if (architectureUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Architecture object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Architecture object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update Architecture object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Architecture object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var architectDetail = await _repository.ArchitectureRepository.GetArchitectureById(id);
                if (architectDetail is null)
                {
                    _logger.LogError($"Update Architecture with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Architecture with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(architectureUpdateDto, architectDetail);
                string result = await _repository.ArchitectureRepository.UpdateArchitecture(architectDetail);
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
                _logger.LogError($"Something went wrong inside UpdateArchitecture action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArchitecture(int id)
        {
            ServiceResponse<ArchitectureDto> serviceResponse = new ServiceResponse<ArchitectureDto>();

            try
            {
                var architectureDetail = await _repository.ArchitectureRepository.GetArchitectureById(id);
                if (architectureDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Architecture object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete Architecture with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.ArchitectureRepository.DeleteArchitecture(architectureDetail);
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
                _logger.LogError($"Something went wrong inside DeleteArchitecture action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateArchitecture(int id)
        {
            ServiceResponse<ArchitectureDto> serviceResponse = new ServiceResponse<ArchitectureDto>();

            try
            {
                var architectureDetail = await _repository.ArchitectureRepository.GetArchitectureById(id);
                if (architectureDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "architecture object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"architecture with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                architectureDetail.IsActive = true;
                string result = await _repository.ArchitectureRepository.UpdateArchitecture(architectureDetail);
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
                _logger.LogError($"Something went wrong inside ActivateArchitecture action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateArchitecture(int id)
        {
            ServiceResponse<ArchitectureDto> serviceResponse = new ServiceResponse<ArchitectureDto>();

            try
            {
                var architectureDetail = await _repository.ArchitectureRepository.GetArchitectureById(id);
                if (architectureDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Architecture object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Architecture with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                architectureDetail.IsActive = false;
                string result = await _repository.ArchitectureRepository.UpdateArchitecture(architectureDetail);
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
                _logger.LogError($"Something went wrong inside DeactivateArchitecture action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
