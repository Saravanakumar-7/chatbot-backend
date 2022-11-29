using AutoMapper;
using Tips.SalesService.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Repository;
using Contracts;
using Entities.DTOs;
using Entities;
using Entities.Helper;
using System.Net;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RfqController : ControllerBase
    {
        private IRfqRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public RfqController(IRfqRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }

         [HttpGet]
        public async Task<IActionResult> GetAllRfqs([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<RfqDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqDto>>();

            try
            {
                var listOfRfq = await _repository.GetAllRfq(pagingParameter);
                var metadata = new
                {
                    listOfRfq.TotalCount,
                    listOfRfq.PageSize,
                    listOfRfq.CurrentPage,
                    listOfRfq.HasNext,
                    listOfRfq.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all rfq");
                var result = _mapper.Map<IEnumerable<RfqDto>>(listOfRfq);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Grins Successfully";
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
        public async Task<IActionResult> GetRfqById(int id)
        {
            ServiceResponse<RfqDto> serviceResponse = new ServiceResponse<RfqDto>();

            try
            {
                var rfq = await _repository.GetRfqById(id);

                if (rfq == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Rfq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Rfq with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<RfqDto>(rfq);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned Rfq with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetrfqById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
             
        }

         [HttpPost]
        public async Task<IActionResult> CreateRfq([FromBody] RfqPostDto rfqPostDto)
        {
            ServiceResponse<RfqDto> serviceResponse = new ServiceResponse<RfqDto>();

            try
            {
                if (rfqPostDto is null)
                {
                    _logger.LogError("Rfq object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Rfq object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Rfq object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Rfq object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var customfield = _mapper.Map<IEnumerable<RfqCustomerSupport>>(rfqPostDto.rfqCustomerSupports);
                var rfqs = _mapper.Map<Rfq>(rfqPostDto);

                rfqs.rfqCustomerSupports = customfield.ToList();

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                _repository.CreateRfq(rfqs);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetRfqById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfq(int id, [FromBody] RfqUpdateDto rfqUpdateDto)
        {
            ServiceResponse<RfqDto> serviceResponse = new ServiceResponse<RfqDto>();

            try
            {
                if (rfqUpdateDto is null)
                {
                    _logger.LogError("Rfq object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Rfq object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Rfq object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Rfq object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var rfq = await _repository.GetRfqById(id);
                if (rfq is null)
                {
                    _logger.LogError($"Rfq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Rfq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                //_mapper.Map(rfqUpdateDto, rfq);
                var customfield = _mapper.Map<IEnumerable<RfqCustomerSupport>>(rfq.rfqCustomerSupports);
                var data = _mapper.Map(rfqUpdateDto, rfq);

                data.rfqCustomerSupports = customfield.ToList();

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                string result = await _repository.UpdateRfq(data);
                _logger.LogInfo(result);
                 _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRfq action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

         [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRfq(int id)
        {
            ServiceResponse<RfqDto> serviceResponse = new ServiceResponse<RfqDto>();

            try
            {
                var rfq = await _repository.GetRfqById(id);
                if (rfq == null)
                {
                    _logger.LogError($"Rfq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Rfq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteRfq(rfq);
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
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
