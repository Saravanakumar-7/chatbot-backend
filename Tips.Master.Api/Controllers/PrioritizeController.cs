using System.Net;
using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Tips.Master.Api.Controllers
{
      [Route("api/[controller]/[action]")]
      [ApiController]
      [Authorize]
    public class PrioritizeController : ControllerBase
     {
          private IRepositoryWrapperForMaster _repository;
          private ILoggerManager _logger;
          private IMapper _mapper;

          private readonly IPrioritizeRepository prioritizeRepository;

         public PrioritizeController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper, IPrioritizeRepository _prioritizeRepository)
         {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            prioritizeRepository = _prioritizeRepository;
         }

        [HttpGet]
        public async Task<IActionResult> GetAllPrioritize([FromQuery] PagingParameter pagingParameter,[FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<PrioritizeDTO>> serviceResponse = new ServiceResponse<IEnumerable<PrioritizeDTO>>();

            try
            {
                var prioritizeList = await _repository.PrioritizeRepository.GetAllPrioritize(pagingParameter, searchParams);
                _logger.LogInfo("Returned all Prioritize");
                var metadata = new
                {
                    prioritizeList.TotalCount,
                    prioritizeList.PageSize,
                    prioritizeList.CurrentPage,
                    prioritizeList.HasNext,
                    prioritizeList.HasPreviuos
                };
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var result= _mapper.Map<IEnumerable<PrioritizeDTO>>(prioritizeList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Priorirtize Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllPrioritize API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllPrioritize API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrioritizeById(int id)
        {
            ServiceResponse<PrioritizeDTO> serviceResponse = new ServiceResponse<PrioritizeDTO>();

            try
            {
                var Prioritize = await _repository.PrioritizeRepository.GetPrioritizeById(id);
                if (Prioritize == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Prioritize with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Prioritize with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Prioritize with id: {id}");
                    var result = _mapper.Map<PrioritizeDTO>(Prioritize);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Prioritize with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetPrioritizeById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetPrioritizeById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public  IActionResult CreatePrioritize([FromBody] PrioritizeDtoPost prioritizeDTOPost)
        {
            ServiceResponse<PrioritizeDTO> serviceResponse = new ServiceResponse<PrioritizeDTO>();

            try
            {
                if (prioritizeDTOPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Prioritize object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Prioritize object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Prioritize object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Prioritize object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var PrioritizeEntity = _mapper.Map<Prioritize>(prioritizeDTOPost);
                _repository.PrioritizeRepository.CreatePrioritize(PrioritizeEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreatePrioritize API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in CreatePrioritize API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrioritize(int id, [FromBody] PrioritizeDtoUpdate prioritizeDtoUpdate)
        {
            ServiceResponse<PrioritizeDTO> serviceResponse = new ServiceResponse<PrioritizeDTO>();

            try
            {
                if (prioritizeDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Prioritize object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Prioritize object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Prioritize object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Prioritize object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var PrioritizeEntity = await _repository.PrioritizeRepository.GetPrioritizeById(id);
                if (PrioritizeEntity is null)
                {
                    _logger.LogError($"Prioritize with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                _mapper.Map(prioritizeDtoUpdate, PrioritizeEntity);
                string result = await _repository.PrioritizeRepository.UpdatePrioritize(PrioritizeEntity);
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
                serviceResponse.Message = $"Error Occured in UpdatePrioritize API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdatePrioritize API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrioritizeById(int id)
        {
            ServiceResponse<PrioritizeDTO> serviceResponse = new ServiceResponse<PrioritizeDTO>();

            try
            {
                var Prioritize = await _repository.PrioritizeRepository.GetPrioritizeById(id);
                if (Prioritize == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Prioritize  object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Priortize with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.PrioritizeRepository.DeletePrioritize(Prioritize);
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
                serviceResponse.Message = $"Error Occured in DeletePrioritizeById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeletePrioritizeById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

     }
}
