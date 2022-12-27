
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

        private IFgOqcRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IFgOqcRepository _fgOqcRepository;

        public FgOqcController(IFgOqcRepository repository, IFgOqcRepository fgOqcRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
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
                var listOfFgoqcs = await _fgOqcRepository.GetAllFgOqcs(pagingParameter);
                var metadata = new
                {
                    listOfFgoqcs.TotalCount,
                    listOfFgoqcs.PageSize,
                    listOfFgoqcs.CurrentPage,
                    listOfFgoqcs.HasNext,
                    listOfFgoqcs.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all FgQoc");
                var result = _mapper.Map<IEnumerable<FgOqcDto>>(listOfFgoqcs);
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
                var fgQoc = await _fgOqcRepository.GetFgOqcById(id);

                if (fgQoc == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"fgOqc with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"fgOqc with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<FgOqcDto>(fgQoc);
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
        public async Task<IActionResult> CreateFgOqc([FromBody] FgOqcDtoPost fgOqcDtoPost)
        {
            ServiceResponse<FgOqcDto> serviceResponse = new ServiceResponse<FgOqcDto>();

            try
            {
                if (fgOqcDtoPost is null)
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
                var fgQocs = _mapper.Map<FgOqc>(fgOqcDtoPost);

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                _fgOqcRepository.CreateFgOqc(fgQocs);
                _fgOqcRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
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
        public async Task<IActionResult> UpdateFgQoc(int id, [FromBody] FgOqcDtoUpdate fgOqcDtoUpdate)
        {
            ServiceResponse<FgOqcDto> serviceResponse = new ServiceResponse<FgOqcDto>();

            try
            {
                if (fgOqcDtoUpdate is null)
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
                var fgQocs = await _fgOqcRepository.GetFgOqcById(id);
                if (fgQocs is null)
                {
                    _logger.LogError($"fgQoc with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update fgQoc with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var data = _mapper.Map(fgOqcDtoUpdate, fgQocs);


                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                string result = await _fgOqcRepository.UpdateFgOqc(data);
                _logger.LogInfo(result);
                _fgOqcRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
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
                var fgQoc = await _fgOqcRepository.GetFgOqcById(id);
                if (fgQoc == null)
                {
                    _logger.LogError($"FgOqc with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete FgOqc with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _fgOqcRepository.DeleteFgOqc(fgQoc);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
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
