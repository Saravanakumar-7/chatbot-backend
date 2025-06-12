using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TypeOfRoomController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        private readonly ITypeOfRoomRepository _typeOfRoomRepository;

        public TypeOfRoomController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper, ITypeOfRoomRepository typeOfRoomRepository)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _typeOfRoomRepository = typeOfRoomRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTypeOfRoom([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<TypeOfRoomDto>> serviceResponse = new ServiceResponse<IEnumerable<TypeOfRoomDto>>();

            try
            {
                var TypeOfRoomList = await _repository.TypeOfRoomRepository.GetAllTypeOfRoom(searchParams);
                _logger.LogInfo("Returned all TypeOfRoom");
                var result = _mapper.Map<IEnumerable<TypeOfRoomDto>>(TypeOfRoomList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all TypeOfRoom Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllTypeOfRoom API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllTypeOfRoom API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //[HttpGet]
        //public async Task<IActionResult> GetAllTypeOfRoom()
        //{
        //    ServiceResponse<IEnumerable<TypeOfRoomDto>> serviceResponse = new ServiceResponse<IEnumerable<TypeOfRoomDto>>();

        //    try
        //    {
        //        var TypeOfRoomList = await _repository.TypeOfRoomRepository.GetAllTypeOfRoom();
        //        _logger.LogInfo("Returned all TypeOfRoom");
        //        var result = _mapper.Map<IEnumerable<TypeOfRoomDto>>(TypeOfRoomList);
        //        serviceResponse.Data = result;
        //        serviceResponse.Message = "Returned all TypeOfRoom Successfully";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}
        [HttpGet]
        public async Task<IActionResult> GetAllActiveTypeOfRooms([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<TypeOfRoomDto>> serviceResponse = new ServiceResponse<IEnumerable<TypeOfRoomDto>>();

            try
            {
                var TypeOfRoomList = await _repository.TypeOfRoomRepository.GetAllActiveTypeOfRoom(searchParams);
                _logger.LogInfo("Returned all TypeOfRoom");
                var result = _mapper.Map<IEnumerable<TypeOfRoomDto>>(TypeOfRoomList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active TypeOfRoom Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveTypeOfRooms API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveTypeOfRooms API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }
        //[HttpGet]
        //public async Task<IActionResult> GetAllActiveTypeOfRooms()
        //{
        //    ServiceResponse<IEnumerable<TypeOfRoomDto>> serviceResponse = new ServiceResponse<IEnumerable<TypeOfRoomDto>>();

        //    try
        //    {
        //        var TypeOfRoomList = await _repository.TypeOfRoomRepository.GetAllActiveTypeOfRoom();
        //        _logger.LogInfo("Returned all TypeOfRoom");
        //        var result = _mapper.Map<IEnumerable<TypeOfRoomDto>>(TypeOfRoomList);
        //        serviceResponse.Data = result;
        //        serviceResponse.Message = "Returned all Active TypeOfRoom Successfully";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);

        //    }
        //}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeOfRoomById(int id)
        {
            ServiceResponse<TypeOfRoomDto> serviceResponse = new ServiceResponse<TypeOfRoomDto>();

            try
            {
                var TypeOfRoom = await _repository.TypeOfRoomRepository.GetTypeOfRoomById(id);
                if (TypeOfRoom == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"TypeOfRoom with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"TypeOfRoom with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<TypeOfRoomDto>(TypeOfRoom);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned owner with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetTypeOfRoomById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetTypeOfRoomById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public IActionResult CreateTypeOfRoom([FromBody] TypeOfRoomPostDto typeOfRoomPostDto)
        {
            ServiceResponse<TypeOfRoomDto> serviceResponse = new ServiceResponse<TypeOfRoomDto>();

            try
            {
                if (typeOfRoomPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "TypeOfRoom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("TypeOfRoom object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid TypeOfRoom object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid TypeOfRoom object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var TypeOfRoomEntity = _mapper.Map<TypeOfRoom>(typeOfRoomPostDto);
                _repository.TypeOfRoomRepository.CreateTypeOfRoom(TypeOfRoomEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetTypeOfRoomById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateTypeOfRoom API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreateTypeOfRoom API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTypeOfRoom(int id, [FromBody] TypeOfRoomUpdateDto typeOfRoomUpdateDto)
        {
            ServiceResponse<TypeOfRoomDto> serviceResponse = new ServiceResponse<TypeOfRoomDto>();

            try
            {
                if (typeOfRoomUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Department object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("TypeOfRoom object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Department object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update TypeOfRoom object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var TypeOfRoomEntity = await _repository.TypeOfRoomRepository.GetTypeOfRoomById(id);
                if (TypeOfRoomEntity is null)
                {
                    _logger.LogError($"TypeOfRoom with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(typeOfRoomUpdateDto, TypeOfRoomEntity);
                string result = await _repository.TypeOfRoomRepository.UpdateTypeOfRoom(TypeOfRoomEntity);
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
                serviceResponse.Message = $"Error Occured in UpdateTypeOfRoom API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateTypeOfRoom API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeOfRoom(int id)
        {
            ServiceResponse<TypeOfRoomDto> serviceResponse = new ServiceResponse<TypeOfRoomDto>();

            try
            {
                var TypeOfRoom = await _repository.TypeOfRoomRepository.GetTypeOfRoomById(id);
                if (TypeOfRoom == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "TypeOfRoom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"TypeOfRoom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.TypeOfRoomRepository.DeleteTypeOfRoom(TypeOfRoom);
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
                serviceResponse.Message = $"Error Occured in DeleteTypeOfRoom API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteTypeOfRoom API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateTypeOfRoom(int id)
        {
            ServiceResponse<TypeOfRoomDto> serviceResponse = new ServiceResponse<TypeOfRoomDto>();

            try
            {
                var TypeOfRoom = await _repository.TypeOfRoomRepository.GetTypeOfRoomById(id);
                if (TypeOfRoom is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "TypeOfRoom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"TypeOfRoom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                TypeOfRoom.ActiveStatus = true;
                string result = await _repository.TypeOfRoomRepository.UpdateTypeOfRoom(TypeOfRoom);
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
                serviceResponse.Message = $"Error Occured in ActivateTypeOfRoom API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in ActivateTypeOfRoom API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateTypeOfRoom(int id)
        {
            ServiceResponse<TypeOfRoomDto> serviceResponse = new ServiceResponse<TypeOfRoomDto>();

            try
            {
                var TypeOfRoom = await _repository.TypeOfRoomRepository.GetTypeOfRoomById(id);
                if (TypeOfRoom is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "TypeOfRoom object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"TypeOfRoom with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                TypeOfRoom.ActiveStatus = false;
                string result = await _repository.TypeOfRoomRepository.UpdateTypeOfRoom(TypeOfRoom);
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
                serviceResponse.Message = $"Error Occured in DeactivateTypeOfRoom API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in DeactivateTypeOfRoom API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
