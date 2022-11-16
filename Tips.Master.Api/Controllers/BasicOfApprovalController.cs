using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BasicOfApprovalController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public BasicOfApprovalController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<BasicOfApprovalController>
        [HttpGet]
        public async Task<IActionResult> GetAllBasicOfApproval()
        {
            ServiceResponse<IEnumerable<BasicOfApprovalDto>> serviceResponse = new ServiceResponse<IEnumerable<BasicOfApprovalDto>>();

            try
            {
                var basicOfApprovals = await _repository.BasicOfApprovalRepository.GetAlBasicOfApproval();
                _logger.LogInfo("Returned all BasicOfApproval");
                var result = _mapper.Map<IEnumerable<BasicOfApprovalDto>>(basicOfApprovals);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BasicOfApproval Successfully";
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
        public async Task<IActionResult> GetAllActiveBasicOfApprovals()
        {
            ServiceResponse<IEnumerable<BasicOfApprovalDto>> serviceResponse = new ServiceResponse<IEnumerable<BasicOfApprovalDto>>();

            try
            {
                var basicOfApprovals = await _repository.BasicOfApprovalRepository.GetAllActiveBasicOfApproval();
                _logger.LogInfo("Returned all BasicOfApproval");
                var result = _mapper.Map<IEnumerable<BasicOfApprovalDto>>(basicOfApprovals);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active BasicOfApproval Successfully";
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

        // GET api/<BasicOfApprovalController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasicOfApprovalById(int id)
        {
            ServiceResponse<BasicOfApprovalDto> serviceResponse = new ServiceResponse<BasicOfApprovalDto>();

            try
            {
                var basicOfApproval = await _repository.BasicOfApprovalRepository.GetBasicOfApprovalById(id);
                if (basicOfApproval == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BasicOfApproval with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BasicOfApproval with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"BasicOfApproval owner with id: {id}");
                    var result = _mapper.Map<BasicOfApprovalDto>(basicOfApproval);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned BasicOfApproval with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBasicOfApprovalById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<BasicOfApprovalController>
        [HttpPost]
        public IActionResult CreateBasicOfApproval([FromBody] BasicOfApprovalPostDto basicOfApprovalPostDto)
        {
            ServiceResponse<BasicOfApprovalDto> serviceResponse = new ServiceResponse<BasicOfApprovalDto>();

            try
            {
                if (basicOfApprovalPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BasicOfApproval object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("BasicOfApproval object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid BasicOfApproval object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid BasicOfApproval object sent from client.");
                    return BadRequest(serviceResponse);
                }
                //var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                //var id = _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                //_repository.SaveAsync();

                var basicOfApproval = _mapper.Map<BasicOfApproval>(basicOfApprovalPostDto);
                _repository.BasicOfApprovalRepository.CreateBasicOfApproval(basicOfApproval);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfylly Created";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetBasicOfApprovalById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<BasicOfApprovalController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBasicOfApproval(int id, [FromBody] BasicOfApprovalUpdateDto basicOfApprovalUpdateDto)
        {
            ServiceResponse<BasicOfApprovalDto> serviceResponse = new ServiceResponse<BasicOfApprovalDto>();

            try
            {
                if (basicOfApprovalUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update BasicOfApproval object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update BasicOfApproval object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update BasicOfApproval object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Invalid BasicOfApproval object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var basicOfApproval = await _repository.BasicOfApprovalRepository.GetBasicOfApprovalById(id);
                if (basicOfApproval is null)
                {
                    _logger.LogError($"Update BasicOfApproval with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update BasicOfApproval with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(basicOfApprovalUpdateDto, basicOfApproval);
                string result = await _repository.BasicOfApprovalRepository.UpdateBasicOfApproval(basicOfApproval);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside UpdateBasicOfApproval action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<BasicOfApprovalController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasicOfApproval(int id)
        {
            ServiceResponse<BasicOfApprovalDto> serviceResponse = new ServiceResponse<BasicOfApprovalDto>();

            try
            {
                var basicOfApproval = await _repository.BasicOfApprovalRepository.GetBasicOfApprovalById(id);
                if (basicOfApproval == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete BasicOfApproval object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete BasicOfApproval with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.BasicOfApprovalRepository.DeleteBasicOfApproval(basicOfApproval);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateBasicOfApproval(int id)
        {
            ServiceResponse<BasicOfApprovalDto> serviceResponse = new ServiceResponse<BasicOfApprovalDto>();

            try
            {
                var basicOfApproval = await _repository.BasicOfApprovalRepository.GetBasicOfApprovalById(id);
                if (basicOfApproval is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BasicOfApproval object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"BasicOfApproval with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                basicOfApproval.IsActive = true;
                string result = await _repository.BasicOfApprovalRepository.UpdateBasicOfApproval(basicOfApproval);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside ActivateBasicOfApproval action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateBasicOfApproval(int id)
        {
            ServiceResponse<BasicOfApprovalDto> serviceResponse = new ServiceResponse<BasicOfApprovalDto>();

            try
            {
                var basicOfApproval = await _repository.BasicOfApprovalRepository.GetBasicOfApprovalById(id);
                if (basicOfApproval is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BasicOfApproval object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"BasicOfApproval with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                basicOfApproval.IsActive = false;
                string result = await _repository.BasicOfApprovalRepository.UpdateBasicOfApproval(basicOfApproval);
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
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivateBasicOfApproval action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
