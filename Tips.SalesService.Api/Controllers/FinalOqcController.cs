
using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FinalOqcController : ControllerBase
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IFinalOqcRepository _finalOqcRepository;
        private readonly IHttpClientFactory _clientFactory;
        public FinalOqcController(IFinalOqcRepository finalOqcRepository, ILoggerManager logger, IHttpClientFactory clientFactory, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _finalOqcRepository = finalOqcRepository;
            _clientFactory = clientFactory;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllFinalOqc([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<FinalOqcDto>> serviceResponse = new ServiceResponse<IEnumerable<FinalOqcDto>>();

            try
            {
                var getAllFinalOqc = await _finalOqcRepository.GetAllFinalOqc(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllFinalOqc.TotalCount,
                    getAllFinalOqc.PageSize,
                    getAllFinalOqc.CurrentPage,
                    getAllFinalOqc.HasNext,
                    getAllFinalOqc.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all FinalQoc");
                var result = _mapper.Map<IEnumerable<FinalOqcDto>>(getAllFinalOqc);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FinalQoc Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllFinalOqc API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllFinalOqc API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFinalOqcById(int id)
        {
            ServiceResponse<FinalOqcDto> serviceResponse = new ServiceResponse<FinalOqcDto>();

            try
            {
                var getFinalOqcDetails = await _finalOqcRepository.GetFinalOqcById(id);

                if (getFinalOqcDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"FinalOqc with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"FinalOqc with id: {id}, hasn't been found .");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<FinalOqcDto>(getFinalOqcDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned FinaloQc with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetFinalOqcById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetFinalOqcById API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateFinalOqc([FromBody] FinalOqcPostDto finalOqcPostDto)
        {
            ServiceResponse<FinalOqcDto> serviceResponse = new ServiceResponse<FinalOqcDto>();

            try
            {
                if (finalOqcPostDto is null)
                {
                    _logger.LogError("FinalOqc object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "FinalOqc object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid FinalOqc object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid FinalOqc object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var createFinalOqc = _mapper.Map<FinalOqc>(finalOqcPostDto);

               await _finalOqcRepository.CreateFinalOqc(createFinalOqc);
                _finalOqcRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "FinalOqc Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetFinalOqcId", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateFinalOqc API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateFinalOqc API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFinalOqc(int id, [FromBody] FinalOqcUpdateDto finalOqcUpdateDto)
        {
            ServiceResponse<FinalOqcDto> serviceResponse = new ServiceResponse<FinalOqcDto>();

            try
            {
                if (finalOqcUpdateDto is null)
                {
                    _logger.LogError("FinalOqc object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update FinalOqc object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid FinalOqc object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update FinalOqc object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getFinalOqcDetails = await _finalOqcRepository.GetFinalOqcById(id);
                if (getFinalOqcDetails is null)
                {
                    _logger.LogError($"FinalOqc with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update FinalOqc with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var updateFinalOqc = _mapper.Map(finalOqcUpdateDto, getFinalOqcDetails);

                string result = await _finalOqcRepository.UpdateFinalOqc(updateFinalOqc);
                _logger.LogInfo(result);
                _finalOqcRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "FinalOqc Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in UpdateFinalOqc API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateFinalOqc API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFinalOqc(int id)
        {
            ServiceResponse<FinalOqc> serviceResponse = new ServiceResponse<FinalOqc>();

            try
            {
                var getFinalOqcById = await _finalOqcRepository.GetFinalOqcById(id);
                if (getFinalOqcById == null)
                {
                    _logger.LogError($"FinalOqc with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete FinalOqc with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _finalOqcRepository.DeleteFinalOqc(getFinalOqcById);
                _logger.LogInfo(result);
                _finalOqcRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "FinalOqc Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteFinalOqc API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteFinalOqc API for the following id:{id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
 