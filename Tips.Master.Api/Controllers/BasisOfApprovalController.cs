using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BasisOfApprovalController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;


        public BasisOfApprovalController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<BasisOfApprovalController>
        [HttpGet]
        public async Task<IActionResult> GetAllBasisOfApproval([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<BasisOfApprovalDto>> serviceResponse = new ServiceResponse<IEnumerable<BasisOfApprovalDto>>();

            try
            {
                var basisOfApprovals = await _repository.BasisOfApprovalRepository.GetAllBasisOfApproval(searchParams);

                _logger.LogInfo("Returned all BasisOfApproval");
                var result = _mapper.Map<IEnumerable<BasisOfApprovalDto>>(basisOfApprovals);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BasisOfApproval Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllBasisOfApproval API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllBasisOfApproval API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveBasisOfApprovals([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<BasisOfApprovalDto>> serviceResponse = new ServiceResponse<IEnumerable<BasisOfApprovalDto>>();

            try
            {
                var basisOfApprovals = await _repository.BasisOfApprovalRepository.GetAllBasisOfApproval(searchParams);
                _logger.LogInfo("Returned all BasisOfApproval");
                var result = _mapper.Map<IEnumerable<BasisOfApprovalDto>>(basisOfApprovals);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active BasisOfApproval Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveBasisOfApprovals API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveBasisOfApprovals API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<BasisOfApprovalController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasisOfApprovalById(int id)
        {
            ServiceResponse<BasisOfApprovalDto> serviceResponse = new ServiceResponse<BasisOfApprovalDto>();

            try
            {
                var basisOfApproval = await _repository.BasisOfApprovalRepository.GetBasisOfApprovalById(id);
                if (basisOfApproval == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BasisOfApproval with id hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"BasisOfApproval with id: {id}, hasn't been found in db.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"BasisOfApproval owner with id: {id}");
                    var result = _mapper.Map<BasisOfApprovalDto>(basisOfApproval);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned BasisOfApproval with id Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBasisOfApprovalById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBasisOfApprovalById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500,serviceResponse);
            }
        }

        // POST api/<BasisOfApprovalController>
        [HttpPost]
        public IActionResult CreateBasisOfApproval([FromBody] BasisOfApprovalPostDto basisOfApprovalPostDto)
        {
            ServiceResponse<BasisOfApprovalDto> serviceResponse = new ServiceResponse<BasisOfApprovalDto>();

            try
            {
                if (basisOfApprovalPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BasisOfApproval object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("BasisOfApproval object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid BasisOfApproval object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid BasisOfApproval object sent from client.");
                    return BadRequest(serviceResponse);
                }
                //var deliverTermEntity = _mapper.Map<DeliveryTerm>(deliveryTerm);
                //var id = _repository.DeliveryTermRepo.CreateDeliveryTerm(deliverTermEntity);
                //_repository.SaveAsync();

                var basisOfApproval = _mapper.Map<BasisOfApproval>(basisOfApprovalPostDto);
                _repository.BasisOfApprovalRepository.CreateBasisOfApproval(basisOfApproval);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetBasisOfApprovalById", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in CreateBasisOfApproval API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateBasisOfApproval API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<BasisOfApprovalController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBasisOfApproval(int id, [FromBody] BasisOfApprovalUpdateDto basisOfApprovalUpdateDto)
        {
            ServiceResponse<BasisOfApprovalDto> serviceResponse = new ServiceResponse<BasisOfApprovalDto>();

            try
            {
                if (basisOfApprovalUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update BasisOfApproval object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update BasisOfApproval object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update BasisOfApproval object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Invalid BasisOfApproval object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var basisOfApproval = await _repository.BasisOfApprovalRepository.GetBasisOfApprovalById(id);
                if (basisOfApproval is null)
                {
                    _logger.LogError($"Update BasisOfApproval with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update BasisOfApproval with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(basisOfApprovalUpdateDto, basisOfApproval);
                string result = await _repository.BasisOfApprovalRepository.UpdateBasisOfApproval(basisOfApproval);
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
                _logger.LogError($"Error Occured in UpdateBasisOfApproval API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in UpdateBasisOfApproval API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<BasisOfApprovalController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBasisOfApproval(int id)
        {
            ServiceResponse<BasisOfApprovalDto> serviceResponse = new ServiceResponse<BasisOfApprovalDto>();

            try
            {
                var basisOfApproval = await _repository.BasisOfApprovalRepository.GetBasisOfApprovalById(id);
                if (basisOfApproval == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete BasisOfApproval object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete BasisOfApproval with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.BasisOfApprovalRepository.DeleteBasisOfApproval(basisOfApproval);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeleteBasisOfApproval API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteBasisOfApproval API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateBasisOfApproval(int id)
        {
            ServiceResponse<BasisOfApprovalDto> serviceResponse = new ServiceResponse<BasisOfApprovalDto>();

            try
            {
                var BasisOfApproval = await _repository.BasisOfApprovalRepository.GetBasisOfApprovalById(id);
                if (BasisOfApproval is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BasisOfApproval object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"BasisOfApproval with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                BasisOfApproval.IsActive = true;
                string result = await _repository.BasisOfApprovalRepository.UpdateBasisOfApproval(BasisOfApproval);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Activated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in ActivateBasisOfApproval API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ActivateBasisOfApproval API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateBasisOfApproval(int id)
        {
            ServiceResponse<BasisOfApprovalDto> serviceResponse = new ServiceResponse<BasisOfApprovalDto>();

            try
            {
                var BasisOfApproval = await _repository.BasisOfApprovalRepository.GetBasisOfApprovalById(id);
                if (BasisOfApproval is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BasisOfApproval object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"BasisOfApproval with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                BasisOfApproval.IsActive = false;
                string result = await _repository.BasisOfApprovalRepository.UpdateBasisOfApproval(BasisOfApproval);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Message = "Deactivated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in DeactivateBasisOfApproval API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeactivateBasisOfApproval API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
