using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Net;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TypeSolutionController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public TypeSolutionController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTypeSolutions([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<TypeSolutionDto>> serviceResponse = new ServiceResponse<IEnumerable<TypeSolutionDto>>();
            try
            {

                var getAllTypeSolution = await _repository.TypeSolutionRepository.GetAllTypeSolutions(searchParams);
                _logger.LogInfo("Returned all TypeSolution");
                var result = _mapper.Map<IEnumerable<TypeSolutionDto>>(getAllTypeSolution);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all TypeSolution Successfully";
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
        public async Task<IActionResult> GetTypeSolutionById(int id)
        {
            ServiceResponse<TypeSolutionDto> serviceResponse = new ServiceResponse<TypeSolutionDto>();

            try
            {
                var typeSolutionbyId = await _repository.TypeSolutionRepository.GetTypeSolutionById(id);
                if (typeSolutionbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"typeSolution with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"typeSolution with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned TypeSolution with id: {id}");
                    var result = _mapper.Map<TypeSolutionDto>(typeSolutionbyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned TypeSolution with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetTypeSolutionById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost]
        public IActionResult CreateTypeSolution([FromBody] TypeSolutionPostDto typeSolutionPostDto)
        {
            ServiceResponse<TypeSolutionDto> serviceResponse = new ServiceResponse<TypeSolutionDto>();

            try
            {
                if (typeSolutionPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "typeSolution object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Entered Invalid Value.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid typeSolution object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Entered Invalid Value.");

                    return BadRequest(serviceResponse);
                }
                var typeSolutionCreate = _mapper.Map<TypeSolution>(typeSolutionPostDto);
                _repository.TypeSolutionRepository.CreateTypeSolution(typeSolutionCreate);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "TypeSolution Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetTypeSolutionById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateTypeSolution action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTypeSolution(int id, [FromBody] TypeSolutionUpdateDto typeSolutionUpdateDto)
        {
            ServiceResponse<TypeSolutionDto> serviceResponse = new ServiceResponse<TypeSolutionDto>();

            try
            {
                if (typeSolutionUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update TypeSolution object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Value Cannot be Null , Pass Proper Value.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update TypeSolution object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update TypeSolution object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var typeSolutionDetail = await _repository.TypeSolutionRepository.GetTypeSolutionById(id);
                if (typeSolutionDetail is null)
                {
                    _logger.LogError($"Value NotFound");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update TypeSolution with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(typeSolutionUpdateDto, typeSolutionDetail);
                string result = await _repository.TypeSolutionRepository.UpdateTypeSolution(typeSolutionDetail);
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
                _logger.LogError($"Something went wrong inside UpdateTypeSolution action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeSolution(int id)
        {
            ServiceResponse<TypeSolutionDto> serviceResponse = new ServiceResponse<TypeSolutionDto>();

            try
            {
                var deleteTypeSolution = await _repository.TypeSolutionRepository.GetTypeSolutionById(id);
                if (deleteTypeSolution == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete TypeSolution object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Value NotFound , Please Enter Proper Value.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.TypeSolutionRepository.DeleteTypeSolution(deleteTypeSolution);
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
                _logger.LogError($"Something went wrong inside DeleteTypeSolution action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}

