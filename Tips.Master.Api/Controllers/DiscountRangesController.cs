using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DiscountRangesController : ControllerBase
    {
        private readonly IRepositoryWrapperForMaster _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;

        public DiscountRangesController(
            IRepositoryWrapperForMaster repository,
            ILoggerManager logger,
            IMapper mapper,
            IHttpClientFactory clientFactory,
            IConfiguration config)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _clientFactory = clientFactory;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDiscountRanges([FromQuery] PagingParameter pagingParameter)
        {
            var serviceResponse = new ServiceResponse<List<DiscountRangesDto>>();
            try
            {
                var allRanges = await _repository.DiscountRangesRepository.GetAllDiscountRanges(pagingParameter);
                var result = _mapper.Map<List<DiscountRangesDto>>(allRanges);

                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all DiscountRanges successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAllDiscountRanges API: {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occurred: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscountRanges([FromBody] List<DiscountRangesPostDto> discountRangesPostDto)
        {
            var serviceResponse = new ServiceResponse<string>();

            try
            {
                if (discountRangesPostDto is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "DiscountRanges object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError(serviceResponse.Message);
                    return BadRequest(serviceResponse);
                }
               

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid DiscountRanges object sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError(serviceResponse.Message);
                    return BadRequest(serviceResponse);
                }
                foreach (var discountRange in discountRangesPostDto)
                {
                    if (discountRange.FromAmount <= 0)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = "FromAmount must be greater than 0";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        _logger.LogError(serviceResponse.Message);
                        return BadRequest(serviceResponse);
                    }

                    if (discountRange.ToAmount.HasValue && discountRange.FromAmount > discountRange.ToAmount)
                    {
                        serviceResponse.Message = "FromAmount must be smaller than ToAmount (unless ToAmount is null)";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(serviceResponse);
                    }

                    var existingRange = await _repository.DiscountRangesRepository.GetDiscountRangesByAmount(discountRange.FromAmount);
                    if (existingRange != null)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = "A range already exists starting from this FromAmount";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.Conflict;
                        _logger.LogError(serviceResponse.Message);
                        return Conflict(serviceResponse);
                    }
                    var entity = _mapper.Map<DiscountRanges>(discountRange);
                    await _repository.DiscountRangesRepository.CreateDiscountRanges(entity);
                }
     
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.Created;

                return Created("CreateDiscountRanges", serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occurred in CreateDiscountRanges API: {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occurred: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscountRangesById(int id)
        {
            var serviceResponse = new ServiceResponse<DiscountRangesDto>();
            try
            {
                var discountRange = await _repository.DiscountRangesRepository.GetDiscountRangesById(id);
                if (discountRange == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DiscountRanges with id {id} not found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError(serviceResponse.Message);
                    return NotFound(serviceResponse);
                }

                var result = _mapper.Map<DiscountRangesDto>(discountRange);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned DiscountRanges successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetDiscountRangesById API: {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occurred: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet("{amount}")]
        public async Task<IActionResult> GetDiscountRangesByAmount(decimal amount)
        {
            var serviceResponse = new ServiceResponse<DiscountRangesDto>();
            try
            {
                var discountRange = await _repository.DiscountRangesRepository.GetDiscountRangesByAmount(amount);
                if (discountRange == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"DiscountRanges for {amount} not found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError(serviceResponse.Message);
                    return NotFound(serviceResponse);
                }

                var result = _mapper.Map<DiscountRangesDto>(discountRange);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned DiscountRanges successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetDiscountRangesById API: {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occurred: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDiscountRanges([FromBody] List<DiscountRangesUpdateDto> discountRangesUpdateDto)
        {
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                if (discountRangesUpdateDto is null || !discountRangesUpdateDto.Any())
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "DiscountRanges list sent from client is null or empty";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError(serviceResponse.Message);
                    return BadRequest(serviceResponse);
                }

                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid DiscountRanges object(s) sent from client";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError(serviceResponse.Message);
                    return BadRequest(serviceResponse);
                }

                foreach (var dto in discountRangesUpdateDto)
                {
                    var existing = await _repository.DiscountRangesRepository.GetDiscountRangesById(dto.Id);
                    if (existing == null)
                    {
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"DiscountRanges with id {dto.Id} not found";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        _logger.LogError(serviceResponse.Message);
                        return NotFound(serviceResponse);
                    }

                    // Set old record's IsActive to false
                    existing.IsActive = false;
                    await _repository.DiscountRangesRepository.UpdateDiscountRanges(existing);

                    // Create new record with incoming DTO data
                    var newEntity = _mapper.Map<DiscountRanges>(dto);
                    newEntity.IsActive = true;
                    await _repository.DiscountRangesRepository.CreateDiscountRanges(newEntity);
                }

                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = "DiscountRanges successfully updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;

                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UpdateDiscountRanges API: {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occurred: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
