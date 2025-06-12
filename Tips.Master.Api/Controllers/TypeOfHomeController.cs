using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TypeOfHomeController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public TypeOfHomeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllTypeOfHome()
        {
            ServiceResponse<IEnumerable<TypeOfHomeDto>> serviceResponse = new ServiceResponse<IEnumerable<TypeOfHomeDto>>();
            try
            {

                var typeOfHomeList = await _repository.TypeOfHomeRepository.GetAllTypeOfHome();
                _logger.LogInfo("Returned all LeadTypes");
                var result = _mapper.Map<IEnumerable<TypeOfHomeDto>>(typeOfHomeList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all typeOfHomes Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllTypeOfHome API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllTypeOfHome API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]

        public async Task<IActionResult> GetAllActiveTypeOfHome()
        {
            ServiceResponse<IEnumerable<TypeOfHomeDto>> serviceResponse = new ServiceResponse<IEnumerable<TypeOfHomeDto>>();

            try
            {
                var typeOfHomesList = await _repository.TypeOfHomeRepository.GetAllActiveTypeOfHome();
                _logger.LogInfo("Returned all leadStatus");
                var result = _mapper.Map<IEnumerable<TypeOfHomeDto>>(typeOfHomesList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active TypeOfHome Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveTypeOfHome API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveTypeOfHome API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }
        // GET api/<DemoStatusController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeOfHomeById(int id)
        {
            ServiceResponse<TypeOfHomeDto> serviceResponse = new ServiceResponse<TypeOfHomeDto>();

            try
            {
                var typeOfHomeById = await _repository.TypeOfHomeRepository.GetTypeOfHomeById(id);
                if (typeOfHomeById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"typeOfHome with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"typeOfHome with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned TypeOfHome with id: {id}");
                    var result = _mapper.Map<TypeOfHomeDto>(typeOfHomeById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned typeOfHome with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetTypeOfHomeById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetTypeOfHomeById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateTypeOfHome([FromBody] TypeOfHomePostDto typeOfHomePostDto)
        {
            ServiceResponse<TypeOfHomePostDto> serviceResponse = new ServiceResponse<TypeOfHomePostDto>();

            try
            {
                if (typeOfHomePostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "typeOfHome object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("typeOfHome object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid typeOfHome object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid typeOfHome object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var typeOfHome = _mapper.Map<TypeOfHome>(typeOfHomePostDto);
                _repository.TypeOfHomeRepository.CreateTypeOfHome(typeOfHome);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "typeOfHome Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GettypeOfHomeById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateTypeOfHome API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in CreateTypeOfHome API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTypeOfHome(int id, [FromBody] TypeOfHomeUpdateDto typeOfHomeUpdateDto)
        {
            ServiceResponse<TypeOfHomeDto> serviceResponse = new ServiceResponse<TypeOfHomeDto>();

            try
            {
                if (typeOfHomeUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update TypeOfHome object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update TypeOfHome object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update TypeOfHome object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update TypeOfHome object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var typeOfHomeDetail = await _repository.TypeOfHomeRepository.GetTypeOfHomeById(id);
                if (typeOfHomeDetail is null)
                {
                    _logger.LogError($"Update TypeOfHome with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update TypeOfHome with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(typeOfHomeUpdateDto, typeOfHomeDetail);
                string result = await _repository.TypeOfHomeRepository.UpdateTypeOfHome(typeOfHomeDetail);
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
                serviceResponse.Message = $"Error Occured in UpdateTypeOfHome API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateTypeOfHome API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeOfHome(int id)
        {
            ServiceResponse<TypeOfHomeDto> serviceResponse = new ServiceResponse<TypeOfHomeDto>();

            try
            {
                var typeOfHomeDetail = await _repository.TypeOfHomeRepository.GetTypeOfHomeById(id);
                if (typeOfHomeDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete typeOfHome object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete typeOfHome with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.TypeOfHomeRepository.DeleteTypeOfHome(typeOfHomeDetail);
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
                serviceResponse.Message = $"Error Occured in DeleteTypeOfHome API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteTypeOfHome API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateTypeOfHome(int id)
        {
            ServiceResponse<TypeOfHomeDto> serviceResponse = new ServiceResponse<TypeOfHomeDto>();

            try
            {
                var typeHomeDetail = await _repository.TypeOfHomeRepository.GetTypeOfHomeById(id);
                if (typeHomeDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "typeOfHomeDetail object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"typeOfHomeDetail with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                typeHomeDetail.IsActive = true;
                string result = await _repository.TypeOfHomeRepository.UpdateTypeOfHome(typeHomeDetail);
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
                serviceResponse.Message = $"Error Occured in ActivateTypeOfHome API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in ActivateTypeOfHome API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateTypeOfHome(int id)
        {
            ServiceResponse<LeadTypeDto> serviceResponse = new ServiceResponse<LeadTypeDto>();

            try
            {
                var typeOfHome = await _repository.TypeOfHomeRepository.GetTypeOfHomeById(id);
                if (typeOfHome is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "typeOfHome object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"typeOfHome with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                typeOfHome.IsActive = false;
                string result = await _repository.TypeOfHomeRepository.UpdateTypeOfHome(typeOfHome);
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
                serviceResponse.Message = $"Error Occured in DeactivateTypeOfHome API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in DeactivateTypeOfHome API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}

