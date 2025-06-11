using AutoMapper;
using Contracts;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Authorization;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class IssuingStockController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public IssuingStockController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>

        [HttpGet]
        public async Task<IActionResult> GetAllIssuingStock([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<IssuingStockDto>> serviceResponse = new ServiceResponse<IEnumerable<IssuingStockDto>>();
            try
            {

                var IssuingStockDetails = await _repository.IssuingStockRepository.GetAllIssuingStock(searchParams);


                _logger.LogInfo("Returned all IssuingStockDetails");
                var result = _mapper.Map<IEnumerable<IssuingStockDto>>(IssuingStockDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all IssuingStockDetails  Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllIssuingStock API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllIssuingStock API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetAllActiveIssuingStock([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<IssuingStockDto>> serviceResponse = new ServiceResponse<IEnumerable<IssuingStockDto>>();

            try
            {
                var IssuingStockDetails = await _repository.IssuingStockRepository.GetAllActiveIssuingStock(searchParams);
                _logger.LogInfo("Returned all IssuingStockDetails");
                var result = _mapper.Map<IEnumerable<IssuingStockDto>>(IssuingStockDetails);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active IssuingStockDetails Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveIssuingStock API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveIssuingStock API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }
        // GET api/<DemoStatusController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIssuingStockById(int id)
        {
            ServiceResponse<IssuingStockDto> serviceResponse = new ServiceResponse<IssuingStockDto>();

            try
            {
                var IssuingStockById = await _repository.IssuingStockRepository.GetIssuingStockById(id);
                if (IssuingStockById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IssuingStock with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"IssuingStock with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned IssuingStock with id: {id}");
                    var result = _mapper.Map<IssuingStockDto>(IssuingStockById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned IssuingStock with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetIssuingStockById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetIssuingStockById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateIssuingStock([FromBody] IssuingStockPostDto IssuingStockPost)
        {
            ServiceResponse<IssuingStockDto> serviceResponse = new ServiceResponse<IssuingStockDto>();

            try
            {
                if (IssuingStockPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IssuingStock object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("IssuingStock object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid IssuingStock object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid IssuingStock object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var IssuingStocks = _mapper.Map<IssuingStock>(IssuingStockPost);
                _repository.IssuingStockRepository.CreateIssuingStock(IssuingStocks);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "IssuingStock Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetIssuingStockeById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateIssuingStock API : \n{ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in CreateIssuingStock API : \n{ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIssuingStock(int id, [FromBody] IssuingStockUpdateDto IssuingStockUpdate)
        {
            ServiceResponse<IssuingStockDto> serviceResponse = new ServiceResponse<IssuingStockDto>();

            try
            {
                if (IssuingStockUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update IssuingStock object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update IssuingStock object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update IssuingStock object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update IssuingStock object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var IssuingStockDetail = await _repository.IssuingStockRepository.GetIssuingStockById(id);
                if (IssuingStockDetail is null)
                {
                    _logger.LogError($"Update IssuingStock with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update IssuingStock with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(IssuingStockUpdate, IssuingStockDetail);
                string result = await _repository.IssuingStockRepository.UpdateIssuingStock(IssuingStockDetail);
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
                serviceResponse.Message = $"Error Occured in UpdateIssuingStock API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateIssuingStock API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssuingStock(int id)
        {
            ServiceResponse<IssuingStockDto> serviceResponse = new ServiceResponse<IssuingStockDto>();

            try
            {
                var IssuingStockDetail = await _repository.IssuingStockRepository.GetIssuingStockById(id);
                if (IssuingStockDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete IssuingStock object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete IssuingStock with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.IssuingStockRepository.DeleteIssuingStock(IssuingStockDetail);
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
                serviceResponse.Message = $"Error Occured in DeleteIssuingStock API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteIssuingStock API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateIssuingStock(int id)
        {
            ServiceResponse<IssuingStockDto> serviceResponse = new ServiceResponse<IssuingStockDto>();

            try
            {
                var IssuingStockDetail = await _repository.IssuingStockRepository.GetIssuingStockById(id);
                if (IssuingStockDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IssuingStock object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"IssuingStock with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                IssuingStockDetail.IsActive = true;
                string result = await _repository.IssuingStockRepository.UpdateIssuingStock(IssuingStockDetail);
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
                serviceResponse.Message = $"Error Occured in ActivateIssuingStock API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in ActivateIssuingStock API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateIssuingStock(int id)
        {
            ServiceResponse<IssuingStockDto> serviceResponse = new ServiceResponse<IssuingStockDto>();

            try
            {
                var IssuingStockDetail = await _repository.IssuingStockRepository.GetIssuingStockById(id);
                if (IssuingStockDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IssuingStock object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"IssuingStock with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                IssuingStockDetail.IsActive = false;
                string result = await _repository.IssuingStockRepository.UpdateIssuingStock(IssuingStockDetail);
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
                serviceResponse.Message = $"Error Occured in DeactivateIssuingStock API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeactivateIssuingStock API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
