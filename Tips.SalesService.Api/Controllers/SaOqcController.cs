
using AutoMapper;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SaOqcController : ControllerBase
    {
        private ISaOqcRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private ISaOqcRepository _saOqcRepository;

        public SaOqcController(ISaOqcRepository repository, ISaOqcRepository saOqcRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _saOqcRepository = saOqcRepository;

        }


        [HttpGet]
        public async Task<IActionResult> GetAllSaOqc([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<SaOqcDto>> serviceResponse = new ServiceResponse<IEnumerable<SaOqcDto>>();

            try
            {
                var listOfsaoqc = await _saOqcRepository.GetAllSaOqc(pagingParameter);
                var metadata = new
                {
                    listOfsaoqc.TotalCount,
                    listOfsaoqc.PageSize,
                    listOfsaoqc.CurrentPage,
                    listOfsaoqc.HasNext,
                    listOfsaoqc.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all FgQoc");
                var result = _mapper.Map<IEnumerable<SaOqcDto>>(listOfsaoqc);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all SaOqc Successfully";
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
        public async Task<IActionResult> GetSaOqcById(int id)
        {
            ServiceResponse<SaOqcDto> serviceResponse = new ServiceResponse<SaOqcDto>();

            try
            {
                var SaQoc = await _saOqcRepository.GetSaOqcById(id);

                if (SaQoc == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SaQoc with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SaQoc with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<SaOqcDto>(SaQoc);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned SaQoc with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetSaOqcById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateSaOqc([FromBody] SaOqcDtoPost saOqcDtoPost)
        {
            ServiceResponse<SaOqcDto> serviceResponse = new ServiceResponse<SaOqcDto>();

            try
            {
                if (saOqcDtoPost is null)
                {
                    _logger.LogError("SaOqc object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "SaOqc object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid SaOqc object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid SaOqc object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var saQocs = _mapper.Map<SaOqc>(saOqcDtoPost);

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                _saOqcRepository.CreateSaOqc(saQocs);
                _saOqcRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetSaOqcId", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateSaOqc action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSaQoc(int id, [FromBody] SaOqcDtoUpdate saOqcDtoUpdate)
        {
            ServiceResponse<SaOqcDto> serviceResponse = new ServiceResponse<SaOqcDto>();

            try
            {
                if (saOqcDtoUpdate is null)
                {
                    _logger.LogError("SaOqc object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update SaOqc object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid SaOqc object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update SaOqc object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var saQocs = await _saOqcRepository.GetSaOqcById(id);
                if (saQocs is null)
                {
                    _logger.LogError($"SaQoc with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Sa" +
                        $"Qoc with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                var data = _mapper.Map(saOqcDtoUpdate, saQocs);


                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                string result = await _saOqcRepository.UpdateSaOqc(data);
                _logger.LogInfo(result);
                _saOqcRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateSaoqc action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaQoc(int id)
        {
            ServiceResponse<SaOqc> serviceResponse = new ServiceResponse<SaOqc>();

            try
            {
                var saQoc = await _saOqcRepository.GetSaOqcById(id);
                if (saQoc == null)
                {
                    _logger.LogError($"SaOqc with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete SaOqc with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _saOqcRepository.DeleteSaOqc(saQoc);
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
                _logger.LogError($"Something went wrong inside DeleteSaOqc action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
 