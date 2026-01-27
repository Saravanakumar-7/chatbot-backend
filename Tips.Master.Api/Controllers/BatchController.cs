using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BatchController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public BatchController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<BatchController>
        [HttpGet]
        public async Task<IActionResult> GetAllBatches([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<BatchDto>> serviceResponse = new ServiceResponse<IEnumerable<BatchDto>>();
            try
            {

                var Batchlist = await _repository.BatchRepository.GetAllBatches(searchParams);
                _logger.LogInfo("Returned all Batchlist");
                var result = _mapper.Map<IEnumerable<BatchDto>>(Batchlist);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Batchlist Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllBatches API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllBatches API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]

        public async Task<IActionResult> GetAllActiveBatches([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<BatchDto>> serviceResponse = new ServiceResponse<IEnumerable<BatchDto>>();

            try
            {
                var Batchlist = await _repository.BatchRepository.GetAllActiveBatches(searchParams);
                _logger.LogInfo("Returned all Batchlist");
                var result = _mapper.Map<IEnumerable<BatchDto>>(Batchlist);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active Batchlist Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllActiveBatches API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllActiveBatches API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);

            }
        }

        // GET api/<BatchController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBatchById(int id)
        {
            ServiceResponse<BatchDto> serviceResponse = new ServiceResponse<BatchDto>();

            try
            {
                var Batch = await _repository.BatchRepository.GetBatchById(id);
                if (Batch == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Batch with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Batch with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned Batch with id: {id}");
                    var result = _mapper.Map<BatchDto>(Batch);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned Batch with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetBatchById API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetBatchById API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<BatchController>
        [HttpPost]
        public IActionResult CreateBatch([FromBody] BatchDtoPost batchDtoPost)
        {
            ServiceResponse<BatchDto> serviceResponse = new ServiceResponse<BatchDto>();

            try
            {
                if (batchDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Batch object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Batch object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Batch object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Batch object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var Batch = _mapper.Map<Batch>(batchDtoPost);
                _repository.BatchRepository.CreateBatch(Batch);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Batch Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetBatchById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateBatch API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in CreateBatch API : \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<BatchController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatch(int id, [FromBody] BatchDtoUpdate batchDtoUpdate)
        {
            ServiceResponse<BatchDto> serviceResponse = new ServiceResponse<BatchDto>();

            try
            {
                if (batchDtoUpdate is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update Batch object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update Batch object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update Batch object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update Batch object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var Batch = await _repository.BatchRepository.GetBatchById(id);
                if (Batch is null)
                {
                    _logger.LogError($"Update Batch with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update Batch with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(batchDtoUpdate, Batch);
                string result = await _repository.BatchRepository.UpdateBatch(Batch);
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
                serviceResponse.Message = $"Error Occured in UpdateBatch API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in UpdateBatch API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<BatchController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            ServiceResponse<BatchDto> serviceResponse = new ServiceResponse<BatchDto>();

            try
            {
                var Batch = await _repository.BatchRepository.GetBatchById(id);
                if (Batch == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete Batch object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete Batch with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.BatchRepository.DeleteBatch(Batch);
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
                serviceResponse.Message = $"Error Occured in DeleteBatch API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in DeleteBatch API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateBatch(int id)
        {
            ServiceResponse<BatchDto> serviceResponse = new ServiceResponse<BatchDto>();

            try
            {
                var Batch = await _repository.BatchRepository.GetBatchById(id);
                if (Batch is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Batch object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Batch with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Batch.IsActive = true;
                string result = await _repository.BatchRepository.UpdateBatch(Batch);
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
                serviceResponse.Message = $"Error Occured in ActivateBatch API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Error Occured in ActivateBatch API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateBatch(int id)
        {
            ServiceResponse<BatchDto> serviceResponse = new ServiceResponse<BatchDto>();

            try
            {
                var Batch = await _repository.BatchRepository.GetBatchById(id);
                if (Batch is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Batch object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Batch with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                Batch.IsActive = false;
                string result = await _repository.BatchRepository.UpdateBatch(Batch);
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
                serviceResponse.Message = $"Error Occured in DeactivateBatch API for the following id : {id} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Error Occured in DeactivateBatch API for the following id : {id} \n {ex.Message} \n{ex.InnerException}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
