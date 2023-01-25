
using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FgOqcController : ControllerBase
    {       
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IFgOqcRepository _fgOqcRepository;

        public FgOqcController( IFgOqcRepository fgOqcRepository, ILoggerManager logger, IMapper mapper)
        {
           
            _logger = logger;
            _mapper = mapper;
            _fgOqcRepository = fgOqcRepository;

        }


        [HttpGet]
        public async Task<IActionResult> GetAllFgOqcs([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<FgOqcDto>> serviceResponse = new ServiceResponse<IEnumerable<FgOqcDto>>();

            try
            {
                var getAllFgOqcs = await _fgOqcRepository.GetAllFgOqcs(pagingParameter);
                var metadata = new
                {
                    getAllFgOqcs.TotalCount,
                    getAllFgOqcs.PageSize,
                    getAllFgOqcs.CurrentPage,
                    getAllFgOqcs.HasNext,
                    getAllFgOqcs.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all FgQoc");
                var result = _mapper.Map<IEnumerable<FgOqcDto>>(getAllFgOqcs);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all FgOqc Successfully";
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
        public async Task<IActionResult> GetFgOqcById(int id)
        {
            ServiceResponse<FgOqcDto> serviceResponse = new ServiceResponse<FgOqcDto>();

            try
            {
                var getFgOqcDetail = await _fgOqcRepository.GetFgOqcById(id);

                if (getFgOqcDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"fgOqc with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"fgOqc with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned fgOqc with id: {id}");
                    var result = _mapper.Map<FgOqcDto>(getFgOqcDetail);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned fgOqc with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetFgOqcById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateFgOqc([FromBody] FgOqcPostDto fgOqcPostDto)
        {
            ServiceResponse<FgOqcDto> serviceResponse = new ServiceResponse<FgOqcDto>();

            try
            {
                if (fgOqcPostDto is null)
                {
                    _logger.LogError("fgOqc object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "fgOqc object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid fgOqc object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid fgOqc object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var fgQocs = _mapper.Map<FgOqc>(fgOqcPostDto);
               
                   await _fgOqcRepository.CreateFgOqc(fgQocs);              

                _fgOqcRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "fgOqc Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetFgOqcId", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateFgOqc action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFgQoc(int id, [FromBody] FgOqcUpdateDto fgOqcUpdateDto)
        {
            ServiceResponse<FgOqcDto> serviceResponse = new ServiceResponse<FgOqcDto>();

            try
            {
                if (fgOqcUpdateDto is null)
                {
                    _logger.LogError("FgOqc object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update FgOqc object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid FgOqc object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update FgOqc object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getFgOqcDetail = await _fgOqcRepository.GetFgOqcById(id);
                if (getFgOqcDetail is null)
                {
                    _logger.LogError($"fgQoc with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update fgQoc with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var data = _mapper.Map(fgOqcUpdateDto, getFgOqcDetail);             

                string result = await _fgOqcRepository.UpdateFgOqc(data);
                _logger.LogInfo(result);
                _fgOqcRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "fgQoc Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateFgoqc action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFgQoc(int id)
        {
            ServiceResponse<FgOqc> serviceResponse = new ServiceResponse<FgOqc>();

            try
            {
                var getFgOqcDetails = await _fgOqcRepository.GetFgOqcById(id);
                if (getFgOqcDetails == null)
                {
                    _logger.LogError($"FgOqc with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete FgOqc with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _fgOqcRepository.DeleteFgOqc(getFgOqcDetails);
                _logger.LogInfo(result);
                _fgOqcRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "FgOqc Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteFgOqc action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
