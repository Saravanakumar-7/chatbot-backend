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
    public class RfqSourcingController : ControllerBase
    {
        private IRfqSourcingRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IRfqRepository _rfqRepository;

        public RfqSourcingController(IRfqSourcingRepository repository,IRfqRepository rfqRepository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _rfqRepository = rfqRepository;
        }
        // GET: api/<RfqSourcingController>
        [HttpGet]
        public async Task<IActionResult> GetAllRfqSourcings([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<RfqSourcingDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqSourcingDto>>();

            try
            {
                var getAllRfqSourcing = await _repository.GetAllRfqSourcing(pagingParameter);
                var metadata = new
                {
                    getAllRfqSourcing.TotalCount,
                    getAllRfqSourcing.PageSize,
                    getAllRfqSourcing.CurrentPage,
                    getAllRfqSourcing.HasNext,
                    getAllRfqSourcing.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all getAllRfqSourcing");
                var result = _mapper.Map<IEnumerable<RfqSourcingDto>>(getAllRfqSourcing);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RfqSourcings Successfully";
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
        // GET api/<RfqSourcingController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRfqSourcingById(int id)
        {
            ServiceResponse<RfqSourcingDto> serviceResponse = new ServiceResponse<RfqSourcingDto>();

            try
            {
                var rfqSourcingById = await _repository.GetRfqSourcingById(id);

                if (rfqSourcingById == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"rfqsourcing with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"rfqsourcing with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned rfqsourcing with id: {id}");

                   

                    RfqSourcingDto rfqSourceDto = _mapper.Map<RfqSourcingDto>(rfqSourcingById);
                   
                    
                    List<RfqSourcingItemsDto> rfqSourseItemDtos = new List<RfqSourcingItemsDto>();            


                        foreach (var rfqSourceItemDetails in rfqSourcingById.RfqSourcingItems)
                        {
                            RfqSourcingItemsDto rfqSourceItemDto = _mapper.Map<RfqSourcingItemsDto>(rfqSourceItemDetails);
                            rfqSourceItemDto.RfqSourcingVendors = _mapper.Map<List<RfqSourcingVendorDto>>(rfqSourceItemDetails.RfqSourcingVendors);
                            rfqSourseItemDtos.Add(rfqSourceItemDto);
                        }
                    

                    rfqSourceDto.RfqSourcingItems = rfqSourseItemDtos;
                    serviceResponse.Data = rfqSourceDto;
                    serviceResponse.Message = $"Returned rfqsourcing with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetrfqsourcingById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateRfqSourcing([FromBody] RfqSourcingDtoPost rfqSourcingDtoPost)
        {
            ServiceResponse<RfqSourcingDto> serviceResponse = new ServiceResponse<RfqSourcingDto>();

            try
            {
                if (rfqSourcingDtoPost is null)
                {
                    _logger.LogError("RfqSourcing object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RfqSourcing object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid RfqSourcing object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid RfqSourcing object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }



                var createRfqSource = _mapper.Map<RfqSourcing>(rfqSourcingDtoPost);

                var rfqSourceData = createRfqSource.RFQNumber;
                
                var rfqIsSourcingUpdate = await _rfqRepository.RfqSourcingByRfqNumbers(rfqSourceData);

                rfqIsSourcingUpdate.IsSourcing = true;

                var rfqSourceDto = rfqSourcingDtoPost.RfqSourcingItems;

                var sourceItemList = new List<RfqSourcingItems>();

                    if (rfqSourceDto != null)
                    {
                        for (int i = 0; i < rfqSourceDto.Count; i++)
                        {
                            RfqSourcingItems rfqSourcingItems = _mapper.Map<RfqSourcingItems>(rfqSourceDto[i]);
                            rfqSourcingItems.RfqSourcingVendors = _mapper.Map<List<RfqSourcingVendor>>(rfqSourceDto[i].RfqSourcingVendors);
                            sourceItemList.Add(rfqSourcingItems);
                        }
                    }
                    createRfqSource.RfqSourcingItems = sourceItemList;

                    await _repository.CreateRfqSourcing(createRfqSource);
                    _rfqRepository.Update(rfqIsSourcingUpdate);
                    _repository.SaveAsync();
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RfqSourcing Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Created("GetRfqSourceById", serviceResponse);
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateRfqSource action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<RfqSourcingController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfqSourcing(int id, [FromBody] RfqSourcingDtoUpdate rfqSourcingDtoUpdate)
        {
            ServiceResponse<RfqSourcingDto> serviceResponse = new ServiceResponse<RfqSourcingDto>();

            try
            {
                if (rfqSourcingDtoUpdate is null)
                {
                    _logger.LogError("RfqSourcing object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update RfqSourcing object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid RfqSourcing object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update RfqSourcing object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var getRfqSourcingById = await _repository.GetRfqSourcingById(id);
                if (getRfqSourcingById is null)
                {
                    _logger.LogError($"RfqSource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update RfqSource with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                
                var updateRfqSourcing = _mapper.Map<RfqSourcing>(rfqSourcingDtoUpdate);

                var sourceItemtemDto = rfqSourcingDtoUpdate.RfqSourcingItems;

                var rfqSourceItemList = new List<RfqSourcingItems>();

                if (sourceItemtemDto !=null) 
                {
                    for (int i = 0; i < sourceItemtemDto.Count; i++)
                    {
                        RfqSourcingItems sourceItemDetail = _mapper.Map<RfqSourcingItems>(sourceItemtemDto[i]);
                        sourceItemDetail.RfqSourcingVendors = _mapper.Map<List<RfqSourcingVendor>>(sourceItemtemDto[i].RfqSourcingVendors);

                        rfqSourceItemList.Add(sourceItemDetail);

                    }
                }
                var updateData = _mapper.Map(rfqSourcingDtoUpdate, getRfqSourcingById);

                updateData.RfqSourcingItems = rfqSourceItemList;          

                string result = await _repository.UpdateRfqSourcing(updateData);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RfqSourcing Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateRfqSourcing action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<RfqSourcingController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRfqSourcing(int id)
        {
            ServiceResponse<RfqSourcingDto> serviceResponse = new ServiceResponse<RfqSourcingDto>();

            try
            {
                var getRfqSourcingById = await _repository.GetRfqSourcingById(id);
                if (getRfqSourcingById == null)
                {
                    _logger.LogError($"RfqSource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete RfqSource with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteRfqSourcing(getRfqSourcingById);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RfqSourcing Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteRfqSource action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
