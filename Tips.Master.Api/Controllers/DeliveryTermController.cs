using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DeliveryTermController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

 
        public DeliveryTermController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }

        // GET: api/<DeliveryTermController>
        [HttpGet]
        public async Task<IActionResult> GetAllDeliveryTerms([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<DeliveryTermGetDto>> serviceResponse = new ServiceResponse<IEnumerable<DeliveryTermGetDto>>();

            try
            {
                var deliveryTermsList = await _repository.DeliveryTermRepo.GetAllDeliveryTerms(pagingParameter,searchParams);

                var metadata = new
                {
                    deliveryTermsList.TotalCount,
                    deliveryTermsList.PageSize,
                    deliveryTermsList.CurrentPage,
                    deliveryTermsList.HasNext,
                    deliveryTermsList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all DeliveryTerms");
                var result = _mapper.Map<IEnumerable<DeliveryTermGetDto>>(deliveryTermsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all DeliveryTerms Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllActiveDeliveryTerms([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<DeliveryTermGetDto>> serviceResponse = new ServiceResponse<IEnumerable<DeliveryTermGetDto>>();

            try
            {
                var deliveryTerms = await _repository.DeliveryTermRepo.GetAllActiveDeliveryTerms(pagingParameter,searchParams);
                _logger.LogInfo("Returned all DeliveryTerms");
                var result = _mapper.Map<IEnumerable<DeliveryTermGetDto>>(deliveryTerms);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active DeliveryTerms Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }
        // GET api/<CustomerTypeController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveryTermsById(int id)
        {
            ServiceResponse<DeliveryTermGetDto> serviceResponse = new ServiceResponse<DeliveryTermGetDto>();

            try
            {
                var deliveryTerm = await _repository.DeliveryTermRepo.GetDeliveryTermById(id);
                if (deliveryTerm == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"deliveryTerm with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"deliveryTerm with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned deliveryTerm with id: {id}");
                    var result = _mapper.Map<DeliveryTermGetDto>(deliveryTerm);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned deliveryTerm with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside GetDeliveryTermsById action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DeliveryTermController> 
        [HttpPost]
        public IActionResult CreateDeliveryTerm([FromBody] DeliveryTermPostDto deliveryTerm)
        {
            ServiceResponse<DeliveryTermGetDto> serviceResponse = new ServiceResponse<DeliveryTermGetDto>();

            try
            {
                if (deliveryTerm is null)
                {
                    _logger.LogError("DeliverTerm object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "DeliverTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest("DeliverTerm object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid DeliverTerm object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Costcenter object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                //var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                //var id = _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                //_repository.SaveAsync();

                var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetDeliveryTermById", serviceResponse);                 
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeliveryTerm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DeliveryTermController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeliveryTerm(int id, [FromBody] DeliveryTermUpdateDto deliveryTerm)
        {
            ServiceResponse<DeliveryTermGetDto> serviceResponse = new ServiceResponse<DeliveryTermGetDto>();

            try
            {
                if (deliveryTerm is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update DeliveryTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update DeliveryTerm object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update DeliveryTerm object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update DeliveryTerm object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var deliveryTermEntity = await _repository.DeliveryTermRepo.GetDeliveryTermById(id);
                if (deliveryTermEntity is null)
                {
                    _logger.LogError($"Update DeliveryTerm with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update DeliveryTerm with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(deliveryTerm, deliveryTermEntity);
                string result = await _repository.DeliveryTermRepo.UpdateDeliveryTerm(deliveryTermEntity);
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
                _logger.LogError($"Something went wrong inside UpdateDeliveryTerm action: {ex.Message}");
                return StatusCode(500,serviceResponse);
            }
        }

        // DELETE api/<DeliveryTermController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeliveryTerm(int id)
        {
            ServiceResponse<DeliveryTermGetDto> serviceResponse = new ServiceResponse<DeliveryTermGetDto>();

            try
            {
                var deliveryTerm = await _repository.DeliveryTermRepo.GetDeliveryTermById(id);
                if (deliveryTerm == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete DeliveryTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete DeliveryTerm with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.DeliveryTermRepo.DeleteDeliveryTerm(deliveryTerm);
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
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateDeliveryTerm(int id)
        {
            ServiceResponse<DeliveryTermGetDto> serviceResponse = new ServiceResponse<DeliveryTermGetDto>();

            try
            {
                var deliveryTerm = await _repository.DeliveryTermRepo.GetDeliveryTermById(id);
                if (deliveryTerm is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "DeliveryTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"DeliveryTerm with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                deliveryTerm.IsActive = true;
                string result = await _repository.DeliveryTermRepo.UpdateDeliveryTerm(deliveryTerm);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside ActivateDeliveryTerm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateDeliveryTerm(int id)
        {
            ServiceResponse<DeliveryTermGetDto> serviceResponse = new ServiceResponse<DeliveryTermGetDto>();

            try
            {
                var deliveryTerm = await _repository.DeliveryTermRepo.GetDeliveryTermById(id);
                if (deliveryTerm is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "DeliveryTerm object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"DeliveryTerm with id: {id}, hasn't been found in db.");
                    return BadRequest("DeliveryTerm object is null");
                }
                deliveryTerm.IsActive = false;
                string result = await _repository.DeliveryTermRepo.UpdateDeliveryTerm(deliveryTerm);
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
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeactivateDeliveryTerm action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
