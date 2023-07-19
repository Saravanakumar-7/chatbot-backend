using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NoOfRoomController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        private readonly INoOfRoomRepository _noOfRoomRepository;

        public NoOfRoomController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper, INoOfRoomRepository noOfRoomRepository)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _noOfRoomRepository = noOfRoomRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllNoOfRoom()
        {
            ServiceResponse<IEnumerable<NoOfRoomDto>> serviceResponse = new ServiceResponse<IEnumerable<NoOfRoomDto>>();

            try
            {
                var NoOfRoomList = await _repository.NoOfRoomRepository.GetAllNoOfRoom();
                _logger.LogInfo("Returned all NoOfRoom");
                var result = _mapper.Map<IEnumerable<NoOfRoomDto>>(NoOfRoomList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all NoOfRoom Successfully";
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
        [HttpGet]
        public async Task<IActionResult> GetAllActiveNoOfRooms()
        {
            ServiceResponse<IEnumerable<NoOfRoomDto>> serviceResponse = new ServiceResponse<IEnumerable<NoOfRoomDto>>();

            try
            {
                var NoOfRoomList = await _repository.NoOfRoomRepository.GetAllActiveNoOfRoom();
                _logger.LogInfo("Returned all TypeOfRoom");
                var result = _mapper.Map<IEnumerable<NoOfRoomDto>>(NoOfRoomList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active NoOfRoom Successfully";
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
        public async Task<IActionResult> GetNoOfRoomById(int id)
        {
            ServiceResponse<NoOfRoomDto> serviceResponse = new ServiceResponse<NoOfRoomDto>();

            try
            {
                var NoOfRoom = await _repository.NoOfRoomRepository.GetNoOfRoomById(id);
                if (NoOfRoom == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"NoOfRoom with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"NoOfRoom with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<NoOfRoomDto>(NoOfRoom);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetNoOfRoomById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public IActionResult CreateNoOfRoom([FromBody] NoOfRoomPostDto noOfRoomPostDto)
        {
            ServiceResponse<NoOfRoomDto> serviceResponse = new ServiceResponse<NoOfRoomDto>();

            try
            {
                if (noOfRoomPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "NoOfRoom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("NoOfRoom object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid NoOfRoom object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid NoOfRoom object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var NoOfRoomEntity = _mapper.Map<NoOfRoom>(noOfRoomPostDto);
                _repository.NoOfRoomRepository.CreateNoOfRoom(NoOfRoomEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetNoOfRoomById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside NoOfRoom action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNoOfRoom(int id, [FromBody] NoOfRoomUpdateDto noOfRoomUpdateDto)
        {
            ServiceResponse<NoOfRoomDto> serviceResponse = new ServiceResponse<NoOfRoomDto>();

            try
            {
                if (noOfRoomUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("NoOfRoom object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Department object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update NoOfRoom object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var NoOfRoomEntity = await _repository.NoOfRoomRepository.GetNoOfRoomById(id);
                if (NoOfRoomEntity is null)
                {
                    _logger.LogError($"NoOfRoom with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(noOfRoomUpdateDto, NoOfRoomEntity);
                string result = await _repository.NoOfRoomRepository.UpdateNoOfRoom(NoOfRoomEntity);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdateNoOfRoom action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNoOfRoom(int id)
        {
            ServiceResponse<NoOfRoomDto> serviceResponse = new ServiceResponse<NoOfRoomDto>();

            try
            {
                var NoOfRoom = await _repository.NoOfRoomRepository.GetNoOfRoomById(id);
                if (NoOfRoom == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "NoOfRoom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"NoOfRoom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.NoOfRoomRepository.DeleteNoOfRoom(NoOfRoom);
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
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateNoOfRoom(int id)
        {
            ServiceResponse<NoOfRoomDto> serviceResponse = new ServiceResponse<NoOfRoomDto>();

            try
            {
                var NoOfRoom = await _repository.NoOfRoomRepository.GetNoOfRoomById(id);
                if (NoOfRoom is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "NoOfRoom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"NoOfRoom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                NoOfRoom.ActiveStatus = true;
                string result = await _repository.NoOfRoomRepository.UpdateNoOfRoom(NoOfRoom);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateNoOfRoom action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateNoOfRoom(int id)
        {
            ServiceResponse<NoOfRoomDto> serviceResponse = new ServiceResponse<NoOfRoomDto>();

            try
            {
                var NoOfRoom = await _repository.NoOfRoomRepository.GetNoOfRoomById(id);
                if (NoOfRoom is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "NoOfRoom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"NoOfRoom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                NoOfRoom.ActiveStatus = false;
                string result = await _repository.NoOfRoomRepository.UpdateNoOfRoom(NoOfRoom);
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
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateNoOfRoom action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}

