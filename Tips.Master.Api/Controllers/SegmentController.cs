using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SegmentController : ControllerBase
    {

        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public SegmentController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<SegmentController>
        [HttpGet]
        public async Task<IActionResult> GetAllSegment([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<SegmentDto>> serviceResponse = new ServiceResponse<IEnumerable<SegmentDto>>();
            try
            {
                var segmentList = await _repository.SegmentRepository.GetAllSegment(searchParams);
                _logger.LogInfo("Returned all Segment");
                var result = _mapper.Map<IEnumerable<SegmentDto>>(segmentList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all Segment";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong inside,Try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<SegmentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSegmentById(int id)
        {
            ServiceResponse<SegmentDto> serviceResponse = new ServiceResponse<SegmentDto>();
            try
            {
                var segment = await _repository.SegmentRepository.GetSegmentById(id);
                if (segment == null)
                {
                    _logger.LogError($"Segment with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Segment hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<SegmentDto>(segment);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Segment Successfully Returned";
                    serviceResponse.Success =true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSegmentById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }


        // POST api/<SegmentController>
        [HttpPost]
        public IActionResult CreateSegment([FromBody] SegmentDtoPost segmentDtoPost)
        {
            ServiceResponse<SegmentDtoPost> serviceResponse = new ServiceResponse<SegmentDtoPost>();
            try
            {
                if (segmentDtoPost is null)
                {
                    _logger.LogError("Segment object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Segment object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Segment object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Segment object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                var segmentEntity = _mapper.Map<Segment>(segmentDtoPost);
                _repository.SegmentRepository.CreateSegment(segmentEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Segment Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<SegmentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSegment(int id, [FromBody] SegmentDtoUpdate segmentDtoUpdate)
        {
            ServiceResponse<SegmentDtoUpdate> serviceResponse = new ServiceResponse<SegmentDtoUpdate>();
            try
            {
                if (segmentDtoUpdate is null)
                {
                    _logger.LogError("Segment object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Segment object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Segment object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Segment object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                var segmentEntity = await _repository.SegmentRepository.GetSegmentById(id);
                if (segmentEntity is null)
                {
                    _logger.LogError($"Segment with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Segment hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                _mapper.Map(segmentDtoUpdate, segmentEntity);
                string result = await _repository.SegmentRepository.UpdateSegment(segmentEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Segment Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateSegment action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<SegmentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSegment(int id)
        {
            ServiceResponse<SegmentDto> serviceResponse = new ServiceResponse<SegmentDto>();
            try
            {
                var segment = await _repository.SegmentRepository.GetSegmentById(id);
                if (segment == null)
                {
                    _logger.LogError($"segment with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Segment hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                string result = await _repository.SegmentRepository.DeleteSegment(segment);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Segment Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateSegment(int id)
        {
            ServiceResponse<SegmentDto> serviceResponse = new ServiceResponse<SegmentDto>();
            try
            {
                var segment = await _repository.SegmentRepository.GetSegmentById(id);
                if (segment is null)
                {
                    _logger.LogError($"Segment with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Segment hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                segment.ActiveStatus = true;
                string result = await _repository.SegmentRepository.UpdateSegment(segment);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned ActivateSegment";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateSegment action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateSegment(int id)
        {
            ServiceResponse<SegmentDto> serviceResponse = new ServiceResponse<SegmentDto>();
            try
            {
                var segment = await _repository.SegmentRepository.GetSegmentById(id);
                if (segment is null)
                {
                    _logger.LogError($"Segment with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Segment hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                segment.ActiveStatus = false;
                string result = await _repository.SegmentRepository.UpdateSegment(segment);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned DeactivateSegment";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateSegment action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
                
            }
        }

    }
}
