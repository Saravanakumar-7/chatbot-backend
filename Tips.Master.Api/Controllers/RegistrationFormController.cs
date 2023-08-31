using System.Net;
using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegistrationFormController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public RegistrationFormController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        //public async Task<IActionResult> GetAllRegistrationForm([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
             public async Task<IActionResult> GetAllRegistrationForm([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<RegistrationFormDto>> serviceResponse = new ServiceResponse<IEnumerable<RegistrationFormDto>>();
            try
            {

                var registrationFormDetails = await _repository.RegistrationFormRepository.GetAllRegistrationForm(searchParams);

                //var metadata = new
                //{
                //    registrationFormDetails.TotalCount,
                //    registrationFormDetails.PageSize,
                //    registrationFormDetails.CurrentPage,
                //    registrationFormDetails.HasNext,
                //    registrationFormDetails.HasPreviuos
                //};

                //Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all RegistrationForm");
                var result = _mapper.Map<IEnumerable<RegistrationFormDto>>(registrationFormDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RegistrationFormDetails  Successfully";
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
        public async Task<IActionResult> GetAllActiveRegistrationForm([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<RegistrationFormDto>> serviceResponse = new ServiceResponse<IEnumerable<RegistrationFormDto>>();

            try
            {
                var activeRegistrationFormDetails = await _repository.RegistrationFormRepository.GetAllActiveRegistrationForm(pagingParameter, searchParams);
                _logger.LogInfo("Returned all RegistrationForm");
                var result = _mapper.Map<IEnumerable<RegistrationFormDto>>(activeRegistrationFormDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active RegistrationForm Successfully";
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
        public async Task<IActionResult> GetAllActiveRegistrationFormList()
        {
            ServiceResponse<IEnumerable<RegistrationFormDetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<RegistrationFormDetailsDto>>();

            try
            {
                var activeRegistrationFormList = await _repository.RegistrationFormRepository.GetAllActiveRegistrationFormList();
                _logger.LogInfo("Returned all RegistrationFormList");
                var result = _mapper.Map<IEnumerable<RegistrationFormDetailsDto>>(activeRegistrationFormList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active RegistrationFormList Successfully";
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

        //test

        [HttpGet]
        public async Task<IActionResult> GetRegistrationFormByUserNameandPassword(string username, string password)
        {
            ServiceResponse<RegistrationFormDto> serviceResponse = new ServiceResponse<RegistrationFormDto>();

            try
            {
                var registrationFormDetails = await _repository.RegistrationFormRepository.GetRegistrationFormByUserNameandPassword(username, password);
                if (registrationFormDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RegistrationForm hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"RegistrationForm with id: {registrationFormDetails.Id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned RegistrationForm with id: {registrationFormDetails.Id}");
                    var result = _mapper.Map<RegistrationFormDto>(registrationFormDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned RegistrationForm with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRegistrationFormById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegistrationFormById(int id)
        {
            ServiceResponse<RegistrationFormDto> serviceResponse = new ServiceResponse<RegistrationFormDto>();

            try
            {
                var registrationFormById = await _repository.RegistrationFormRepository.GetRegistrationFormById(id);
                if (registrationFormById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"RegistrationForm hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"RegistrationForm with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned RegistrationForm with id: {id}");
                    var result = _mapper.Map<RegistrationFormDto>(registrationFormById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned RegistrationForm with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRegistrationFormById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public IActionResult CreateRegistrationForm([FromBody] RegistrationFormPostDto registrationFormPostDto)
        {
            ServiceResponse<RegistrationFormPostDto> serviceResponse = new ServiceResponse<RegistrationFormPostDto>();

            try
            {
                if (registrationFormPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RegistrationForm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("RegistrationForm object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid RegistrationForm object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid RegistrationForm object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var roles = _mapper.Map<RegistrationForm>(registrationFormPostDto);
                _repository.RegistrationFormRepository.CreateRegistrationForm(roles);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RegistrationForm Created Successfully";
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
                _logger.LogError($"Something went wrong inside CreateRegistrationForm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegistrationForm(int id, [FromBody] RegistrationFormUpdateDto registrationFormUpdateDto)
        {
            ServiceResponse<RegistrationFormUpdateDto> serviceResponse = new ServiceResponse<RegistrationFormUpdateDto>();

            try
            {
                if (registrationFormUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update RegistrationForm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update RegistrationForm object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update RegistrationForm object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update RegistrationForm object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var registrationFormDetailsById = await _repository.RegistrationFormRepository.GetRegistrationFormById(id);
                if (registrationFormDetailsById is null)
                {
                    _logger.LogError($"Update RegistrationForm with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update RegistrationForm hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(registrationFormUpdateDto, registrationFormDetailsById);
                string result = await _repository.RegistrationFormRepository.UpdateRegistrationForm(registrationFormDetailsById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RegistrationForm Updated Successfully";
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
                _logger.LogError($"Something went wrong inside UpdateRegistrationForm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegistrationForm(int id)
        {
            ServiceResponse<RegistrationFormDto> serviceResponse = new ServiceResponse<RegistrationFormDto>();

            try
            {
                var registrationFormDetailsById = await _repository.RegistrationFormRepository.GetRegistrationFormById(id);
                if (registrationFormDetailsById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete RegistrationForm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete RegistrationForm with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.RegistrationFormRepository.DeleteRegistrationForm(registrationFormDetailsById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "RegistrationForm Deleted Successfully";
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
                _logger.LogError($"Something went wrong inside DeleteRegistrationForm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
