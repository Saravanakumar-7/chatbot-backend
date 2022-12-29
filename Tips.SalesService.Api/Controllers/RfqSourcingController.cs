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

        public RfqSourcingController(IRfqSourcingRepository repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;

        }
        // GET: api/<RfqSourcingController>
        [HttpGet]
        public async Task<IActionResult> GetAllRfqSourcings([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<RfqSourcingDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqSourcingDto>>();

            try
            {
                var listOfRfqSourcing = await _repository.GetAllRfqSourcing(pagingParameter);
                var metadata = new
                {
                    listOfRfqSourcing.TotalCount,
                    listOfRfqSourcing.PageSize,
                    listOfRfqSourcing.CurrentPage,
                    listOfRfqSourcing.HasNext,
                    listOfRfqSourcing.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all listOfRfqSourcing");
                var result = _mapper.Map<IEnumerable<RfqSourcingDto>>(listOfRfqSourcing);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RfqSourcing Successfully";
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
                var rfqsourcing = await _repository.GetRfqSourcingById(id);

                if (rfqsourcing == null)
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
                    //var result = _mapper.Map<RfqSourcingDto>(rfqsourcing);
                    RfqSourcingDto rfqSourceDto = _mapper.Map<RfqSourcingDto>(rfqsourcing);//Main model mapping

                    //below mapping is child under child  
                    
                    List<RfqSourcingItemsDto> rfqSourseItemDtos = new List<RfqSourcingItemsDto>();

                    foreach (var sourceitemDetails in rfqsourcing.rfqSourcingItems)
                    {
                        RfqSourcingItemsDto rfqSourceItemDto = _mapper.Map<RfqSourcingItemsDto>(sourceitemDetails);
                        rfqSourceItemDto.rfqSourcingVendors = _mapper.Map<List<RfqSourcingVendorDto>>(sourceitemDetails.rfqSourcingVendors);
                        rfqSourseItemDtos.Add(rfqSourceItemDto);
                    }

                    rfqSourceDto.rfqSourcingItems = rfqSourseItemDtos;
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

        // POST api/<RfqSourcingController>
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
                //var sourceitems = _mapper.Map<IEnumerable<RfqSourcingItems>>(rfqSourcingDtoPost.rfqSourcingItems);
                var rfqsource = _mapper.Map<RfqSourcing>(rfqSourcingDtoPost);
                var rfqSourceDto = rfqSourcingDtoPost.rfqSourcingItems;

                var sourceItemList = new List<RfqSourcingItems>();
                for (int i = 0; i < rfqSourceDto.Count; i++)
                {
                    RfqSourcingItems sourceItemListDetail = _mapper.Map<RfqSourcingItems>(rfqSourceDto[i]);
                     sourceItemListDetail.rfqSourcingVendors = _mapper.Map<List<RfqSourcingVendor>>(rfqSourceDto[i].rfqSourcingVendors);                   
                    sourceItemList.Add(sourceItemListDetail);

                }
                rfqsource.rfqSourcingItems = sourceItemList;

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                _repository.CreateRfqSourcing(rfqsource);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
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
                var rfqsource = await _repository.GetRfqSourcingById(id);
                if (rfqsource is null)
                {
                    _logger.LogError($"Rfq with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Rfq with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                //_mapper.Map(rfqUpdateDto, rfq);
                //var rfqsourceing = _mapper.Map<IEnumerable<RfqSourcingItems>>(rfqsource.rfqSourcingItems);
                var rfqsourceList = _mapper.Map<RfqSourcing>(rfqSourcingDtoUpdate);

                var sourceitemtemDto = rfqSourcingDtoUpdate.rfqSourcingItems;

                var sourceItemList = new List<RfqSourcingItems>();
                for (int i = 0; i < sourceitemtemDto.Count; i++)
                {
                    RfqSourcingItems sourceItemDetail = _mapper.Map<RfqSourcingItems>(sourceitemtemDto[i]);
                    sourceItemDetail.rfqSourcingVendors = _mapper.Map<List<RfqSourcingVendor>>(sourceitemtemDto[i].rfqSourcingVendors);

                    sourceItemList.Add(sourceItemDetail);

                }
                var data = _mapper.Map(rfqSourcingDtoUpdate, rfqsource);

                data.rfqSourcingItems = sourceItemList;

                //var notes = _mapper.Map<IEnumerable<RfqNotes>>(rfq.rfqNotes);

                string result = await _repository.UpdateRfqSourcing(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
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
                var rfqsource = await _repository.GetRfqSourcingById(id);
                if (rfqsource == null)
                {
                    _logger.LogError($"RfqSource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete RfqSource with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteRfqSourcing(rfqsource);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
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
