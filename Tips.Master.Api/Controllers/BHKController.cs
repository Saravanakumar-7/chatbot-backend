using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BHKController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public BHKController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<DemoStatusController>
        [HttpGet]
        public async Task<IActionResult> GetAllBHK([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<BHKDto>> serviceResponse = new ServiceResponse<IEnumerable<BHKDto>>();
            try
            {

                var bHKList = await _repository.BHKRepository.GetAllBHK(pagingParameter, searchParams);

                var metadata = new
                {
                    bHKList.TotalCount,
                    bHKList.PageSize,
                    bHKList.CurrentPage,
                    bHKList.HasNext,
                    bHKList.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all BHK");
                var result = _mapper.Map<IEnumerable<BHKDto>>(bHKList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all BHK  Successfully";
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
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveBHK([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<BHKDto>> serviceResponse = new ServiceResponse<IEnumerable<BHKDto>>();

            try
            {
                var bHKList = await _repository.BHKRepository.GetAllActiveBHK(pagingParameter,searchParams);
                _logger.LogInfo("Returned all BHK");
                var result = _mapper.Map<IEnumerable<BHKDto>>(bHKList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Active BHK Successfully";
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
        // GET api/<DemoStatusController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBHKById(int id)
        {
            ServiceResponse<BHKDto> serviceResponse = new ServiceResponse<BHKDto>();

            try
            {
                var bHKById = await _repository.BHKRepository.GetBHKById(id);
                if (bHKById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"BHK with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"BHK with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                else
                {

                    _logger.LogInfo($"Returned BHK with id: {id}");
                    var result = _mapper.Map<BHKDto>(bHKById);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned BHK with id successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetBHKById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<DemoStatusController>
        [HttpPost]
        public IActionResult CreateBHK([FromBody] BHKPostDto bHKPostDto)
        {
            ServiceResponse<BHKPostDto> serviceResponse = new ServiceResponse<BHKPostDto>();

            try
            {
                if (bHKPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BHK object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("BHK object sent from client is null.");
                    //return BadRequest("PurchaseGroup object is null");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid BHK object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid BHK object sent from client.");
                    //return BadRequest("Invalid model object");
                    return BadRequest(serviceResponse);
                }
                var typeOBHK = _mapper.Map<BHK>(bHKPostDto);
                _repository.BHKRepository.CreateBHK(typeOBHK);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "BHK Created Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetBHKById", serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside CreateBHK action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<DemoStatusController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBHK(int id, [FromBody] BHKUpdateDto bHKUpdateDto)
        {
            ServiceResponse<BHKDto> serviceResponse = new ServiceResponse<BHKDto>();

            try
            {
                if (bHKUpdateDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "update BHK object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("update BHK object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid update BHK object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid Update BHK object sent from client.");
                    return BadRequest(serviceResponse);
                }
                var bHKDetail = await _repository.BHKRepository.GetBHKById(id);
                if (bHKDetail is null)
                {
                    _logger.LogError($"Update BHK with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = " Update BHK with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                _mapper.Map(bHKUpdateDto, bHKDetail);
                string result = await _repository.BHKRepository.UpdateBHK(bHKDetail);
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
                _logger.LogError($"Something went wrong inside UpdateBHK action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<DemoStatusController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBHK(int id)
        {
            ServiceResponse<BHKDto> serviceResponse = new ServiceResponse<BHKDto>();

            try
            {
                var bHKDetail = await _repository.BHKRepository.GetBHKById(id);
                if (bHKDetail == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Delete BHK object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"Delete BHK with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                string result = await _repository.BHKRepository.DeleteBHK(bHKDetail);
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
                _logger.LogError($"Something went wrong inside DeleteBHK action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateBHK(int id)
        {
            ServiceResponse<BHKDto> serviceResponse = new ServiceResponse<BHKDto>();

            try
            {
                var bHKDetail = await _repository.BHKRepository.GetBHKById(id);
                if (bHKDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "bHKDetail object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"bHK with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                bHKDetail.IsActive = true;
                string result = await _repository.BHKRepository.UpdateBHK(bHKDetail);
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
                _logger.LogError($"Something went wrong inside ActivateBHK action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateBHK(int id)
        {
            ServiceResponse<BHKDto> serviceResponse = new ServiceResponse<BHKDto>();

            try
            {
                var bHKDetail = await _repository.BHKRepository.GetBHKById(id);
                if (bHKDetail is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "BHK object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"BHK with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                bHKDetail.IsActive = false;
                string result = await _repository.BHKRepository.UpdateBHK(bHKDetail);
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
                serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                _logger.LogError($"Something went wrong inside DeactivatedBHK action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }
    }
}

