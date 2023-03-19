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
        public async Task<IActionResult> GetAllRfqSourcings([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<RfqSourcingDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqSourcingDto>>();

            try
            {
                var getAllRfqSourcing = await _repository.GetAllRfqSourcing(pagingParameter, searchParammes);
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
                var rfqSourcings = await _repository.GetRfqSourcingById(id);

                if (rfqSourcings == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"rfqsourcing with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"rfqsourcing with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned rfqsourcing with id: {id}");

                   

                    RfqSourcingDto rfqSourceDto = _mapper.Map<RfqSourcingDto>(rfqSourcings);
                   
                    
                    List<RfqSourcingItemsDto> rfqSourseItemDtos = new List<RfqSourcingItemsDto>();            


                        foreach (var rfqSourceItemDetails in rfqSourcings.RfqSourcingItems)
                        {
                            RfqSourcingItemsDto rfqSourceItemDto = _mapper.Map<RfqSourcingItemsDto>(rfqSourceItemDetails);
                            rfqSourceItemDto.RfqSourcingVendorDtos = _mapper.Map<List<RfqSourcingVendorDto>>(rfqSourceItemDetails.RfqSourcingVendors);
                            rfqSourseItemDtos.Add(rfqSourceItemDto);
                        }
                    

                    rfqSourceDto.RfqSourcingItemsDtos = rfqSourseItemDtos;
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
        public async Task<IActionResult> CreateRfqSourcing([FromBody] RfqSourcingPostDto rfqSourcingPostDto)
        {
            ServiceResponse<RfqSourcingDto> serviceResponse = new ServiceResponse<RfqSourcingDto>();

            try
            {
                if (rfqSourcingPostDto is null)
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



                var createRfqSource = _mapper.Map<RfqSourcing>(rfqSourcingPostDto);

                var rfqSourceData = createRfqSource.RFQNumber;
                
                var rfqIsSourcingUpdate = await _rfqRepository.RfqSourcingByRfqNumbers(rfqSourceData);

                rfqIsSourcingUpdate.IsSourcing = true;

                var rfqSourceDto = rfqSourcingPostDto.RfqSourcingItemsPostDtos;

                var sourceItemList = new List<RfqSourcingItems>();

                    if (rfqSourceDto != null)
                    {
                        for (int i = 0; i < rfqSourceDto.Count; i++)
                        {
                            RfqSourcingItems rfqSourcingItems = _mapper.Map<RfqSourcingItems>(rfqSourceDto[i]);
                            rfqSourcingItems.RfqSourcingVendors = _mapper.Map<List<RfqSourcingVendor>>(rfqSourceDto[i].RfqSourcingVendorPostDtos);
                            sourceItemList.Add(rfqSourcingItems);
                        }
                    }
                    createRfqSource.RfqSourcingItems = sourceItemList;

                    await _repository.CreateRfqSourcing(createRfqSource);
                    _rfqRepository.Update(rfqIsSourcingUpdate);
                    _repository.SaveAsync();
                    serviceResponse.Data = null;
                    serviceResponse.Message = "RfqSourcing Created Successfully";
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
        public async Task<IActionResult> UpdateRfqSourcing(int id, [FromBody] RfqSourcingUpdateDto rfqSourcingUpdateDto)
        {
            ServiceResponse<RfqSourcingDto> serviceResponse = new ServiceResponse<RfqSourcingDto>();

            try
            {
                if (rfqSourcingUpdateDto is null)
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
                var getRfqSourcings = await _repository.GetRfqSourcingById(id);
                if (getRfqSourcings is null)
                {
                    _logger.LogError($"RfqSource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update RfqSource with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                
                var updateRfqSourcing = _mapper.Map<RfqSourcing>(rfqSourcingUpdateDto);

                var sourceItemtemDto = rfqSourcingUpdateDto.RfqSourcingItemsUpdateDtos;

                var rfqSourceItemList = new List<RfqSourcingItems>();

                if (sourceItemtemDto !=null) 
                {
                    for (int i = 0; i < sourceItemtemDto.Count; i++)
                    {
                        RfqSourcingItems sourceItemDetail = _mapper.Map<RfqSourcingItems>(sourceItemtemDto[i]);
                        sourceItemDetail.RfqSourcingVendors = _mapper.Map<List<RfqSourcingVendor>>(sourceItemtemDto[i].RfqSourcingVendorUpdateDtos);

                        rfqSourceItemList.Add(sourceItemDetail);

                    }
                }
                var updateData = _mapper.Map(rfqSourcingUpdateDto, getRfqSourcings);

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
                var deleteRfqSourcing = await _repository.GetRfqSourcingById(id);
                if (deleteRfqSourcing == null)
                {
                    _logger.LogError($"RfqSource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete RfqSource with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteRfqSourcing(deleteRfqSourcing);
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
