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
    public class PurchaseGroupController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public PurchaseGroupController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<PurchaseGroupController>
        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseGroups()
        {
            ServiceResponse<IEnumerable<PurchaseGroupDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseGroupDto>>();
            try
            {

                var PurchaseGroupList = await _repository.PurchaseGroupRepository.GetAllPurchaseGroups();
                _logger.LogInfo("Returned all PurchaseGroups");
                var result = _mapper.Map<IEnumerable<PurchaseGroupDto>>(PurchaseGroupList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all PurchaseGroups Successfully";
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
        public async Task<IActionResult> GetAllActivePurchaseGroups()
        {
            ServiceResponse<IEnumerable<PurchaseGroupDto>> serviceResponse = new ServiceResponse<IEnumerable<PurchaseGroupDto>>();

            try
            {
                var PurchaseGroups = await _repository.PurchaseGroupRepository.GetAllActivePurchaseGroups();
                _logger.LogInfo("Returned all PurchaseGroups");
                var result = _mapper.Map<IEnumerable<PurchaseGroupDto>>(PurchaseGroups);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active  PurchaseGroups Successfully";
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
        // GET api/<PurchaseGroupController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseGroupById(int id)
        {
            ServiceResponse<PurchaseGroupDto> serviceResponse = new ServiceResponse<PurchaseGroupDto>();

            try
            {
                var PurchaseGroup = await _repository.PurchaseGroupRepository.GetPurchaseGroupById(id);
                if (PurchaseGroup == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"PurchaseGroup with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"PurchaseGroup with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned PurchaseGroup with id: {id}");
                    var result = _mapper.Map<PurchaseGroupDto>(PurchaseGroup);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned PurchaseGroup Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetPurchaseGroupById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<PurchaseGroupController>
        [HttpPost]
        public IActionResult CreatePurchaseGroup([FromBody] PurchaseGroupDtoPost purchaseGroupDtoPost)
        {
            ServiceResponse<PurchaseGroupDto> serviceResponse = new ServiceResponse<PurchaseGroupDto>();

            try
            {
                if (purchaseGroupDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "PurchaseGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("PurchaseGroup object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse); 
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid PurchaseGroup object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid PurchaseGroup object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var purchasegroupEntity = _mapper.Map<PurchaseGroup>(purchaseGroupDtoPost);
                _repository.PurchaseGroupRepository.CreatePurchaseGroup(purchasegroupEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfylly Created";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetPurchaseGroupById",serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside PurchaseGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<PurchaseGroupController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseGroup(int id, [FromBody] PurchaseGroupDtoUpdate purchaseGroupDtoUpdate)
        {
            ServiceResponse<PurchaseGroupDto> serviceResponse = new ServiceResponse<PurchaseGroupDto>();

            try
            {
                if (purchaseGroupDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update PurchaseGroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update PurchaseGroup object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update PurchaseGroup object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid update PurchaseGroup object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var PurchaseGroup = await _repository.PurchaseGroupRepository.GetPurchaseGroupById(id);
                if (PurchaseGroup is null)
                {
                    _logger.LogError($"update PurchaseGroup with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update PurchaseGroup with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(purchaseGroupDtoUpdate, PurchaseGroup);
                string result = await _repository.PurchaseGroupRepository.UpdatePurchaseGroup(PurchaseGroup);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside UpdatePurchaseGroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }


        // DELETE api/<PurchaseGroupController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePruchaseGroup(int id)
        {
            ServiceResponse<PurchaseGroupDto> serviceResponse = new ServiceResponse<PurchaseGroupDto>();

            try
            {
                var purchasegroup = await _repository.PurchaseGroupRepository.GetPurchaseGroupById(id);
                if (purchasegroup == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Purchasegroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete PurchaseGroup with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.PurchaseGroupRepository.DeletePurchaseGroup(purchasegroup);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivatePurchaseGroup(int id)
        {
            ServiceResponse<PurchaseGroupDto> serviceResponse = new ServiceResponse<PurchaseGroupDto>();

            try
            {
                var purchasegroup = await _repository.PurchaseGroupRepository.GetPurchaseGroupById(id);
                if (purchasegroup is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "purchasegroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"purchasegroup with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchasegroup.IsActive = true;
                string result = await _repository.PurchaseGroupRepository.UpdatePurchaseGroup(purchasegroup);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside Activatepurchasegroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivatePurchaseGroup(int id)
        {
            ServiceResponse<PurchaseGroupDto> serviceResponse = new ServiceResponse<PurchaseGroupDto>();

            try
            {
                var purchasegroup = await _repository.PurchaseGroupRepository.GetPurchaseGroupById(id);
                if (purchasegroup is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "purchasegroup object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"purchasegroup with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                purchasegroup.IsActive = false;
                string result = await _repository.PurchaseGroupRepository.UpdatePurchaseGroup(purchasegroup);
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
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside Deactivatepurchasegroup action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }

        }
    }
}
