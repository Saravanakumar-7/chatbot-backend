using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RoomNameController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public RoomNameController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoomNames([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<RoomNamesDto>> serviceResponse = new ServiceResponse<IEnumerable<RoomNamesDto>>();
            try
            {

                var getAllRoomNames = await _repository.RoomNameRepository.GetAllRoomNames(searchParams);
                _logger.LogInfo("Returned all RoomName");
                var result = _mapper.Map<IEnumerable<RoomNamesDto>>(getAllRoomNames);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RoomName Successfully";
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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomNameById(int id)
        {
            ServiceResponse<RoomNamesDto> serviceResponse = new ServiceResponse<RoomNamesDto>();

            try
            {
                var roomNamebyId = await _repository.RoomNameRepository.GetRoomNameById(id);
                if (roomNamebyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RoomName with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"RoomName with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned RoomName with id: {id}");
                    var result = _mapper.Map<RoomNamesDto>(roomNamebyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned RoomName with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRoomNameById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost]
        public IActionResult CreateRoomName([FromBody] RoomNamePostDto roomNamePostDto)
        {
            ServiceResponse<RoomNamesDto> serviceResponse = new ServiceResponse<RoomNamesDto>();

            try
            {
                if (roomNamePostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RoomName object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Entered Invalid Value.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid RoomName object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Entered Invalid Value.");

                    return BadRequest(serviceResponse);
                }
                var roomNameCreate = _mapper.Map<RoomNames>(roomNamePostDto);
                _repository.RoomNameRepository.CreateRoomName(roomNameCreate);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RoomName Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetRoomNameById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateRoomName action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoomName(int id, [FromBody] RoomNameUpdateDto roomNameUpdateDto)
        {
            ServiceResponse<RoomNamesDto> serviceResponse = new ServiceResponse<RoomNamesDto>();

            try
            {
                if (roomNameUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update roomName object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Value Cannot be Null , Pass Proper Value.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update roomName object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update roomName object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var roomNameDetail = await _repository.RoomNameRepository.GetRoomNameById(id);
                if (roomNameDetail is null)
                {
                    _logger.LogError($"Value NotFound");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update roomName with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(roomNameUpdateDto, roomNameDetail);
                string result = await _repository.RoomNameRepository.UpdateRoomName(roomNameDetail);
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
                _logger.LogError($"Something went wrong inside UpdateRoomName action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomName(int id)
        {
            ServiceResponse<RoomNamesDto> serviceResponse = new ServiceResponse<RoomNamesDto>();

            try
            {
                var roomNameDetail = await _repository.RoomNameRepository.GetRoomNameById(id);
                if (roomNameDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete roomName object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Value NotFound , Please Enter Proper Value.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.RoomNameRepository.DeleteRoomName(roomNameDetail);
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
                _logger.LogError($"Something went wrong inside DeleteRoomName action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}

