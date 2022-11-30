using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using AutoMapper;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Entities.Migrations;
using System.Net;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LeadController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public LeadController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: api/<CustomerInfoController>
        [HttpGet]
        public async Task<IActionResult> GetAllLeads([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<LeadDto>> serviceResponse = new ServiceResponse<IEnumerable<LeadDto>>();

            try
            {
                var listOfLeads = await _repository.LeadRepository.GetAllLeads(pagingParameter);

                var metadata = new
                {
                    listOfLeads.TotalCount,
                    listOfLeads.PageSize,
                    listOfLeads.CurrentPage,
                    listOfLeads.HasNext,
                    listOfLeads.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                _logger.LogInfo("Returned all Leads");
                var result = _mapper.Map<IEnumerable<LeadDto>>(listOfLeads);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Leads Successfully";
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

        // GET api/<CustomerInfoController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeadById(int id)
        {
            ServiceResponse<LeadDto> serviceResponse = new ServiceResponse<LeadDto>();

            try
            {
                var LeadDetails = await _repository.LeadRepository.GetLeadById(id);

                if (LeadDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"LeadDetails with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"LeadDetails with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned LeadDetails with id: {id}");
                    var result = _mapper.Map<LeadDto>(LeadDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = $"Returned LeadDetails with id: {id}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetLeadById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        // POST api/<CustomerInfoController>
        [HttpPost]
        public async Task<IActionResult> CreateLead([FromBody] LeadDtoPost leadDtoPost)
        {
            ServiceResponse<LeadDto> serviceResponse = new ServiceResponse<LeadDto>();

            try
            {
                if (leadDtoPost is null)
                {
                    _logger.LogError("lead object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "lead object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid lead object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid lead object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var address = _mapper.Map<IEnumerable<LeadAddress>>(leadDtoPost.LeadAddresses);
               
                var lead = _mapper.Map<Lead>(leadDtoPost);

                lead.LeadAddress = address.ToList();
               

                _repository.LeadRepository.CreateLead(lead);

                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("GetleadById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Lead action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<CustomerInfoController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLead(int id, [FromBody] LeadDto leadUpdateDto)
        {
            ServiceResponse<LeadDto> serviceResponse = new ServiceResponse<LeadDto>();

            try
            {
                if (leadUpdateDto is null)
                {
                    _logger.LogError("Update Lead object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Lead object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update Lead object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update Lead object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updatelead = await _repository.LeadRepository.GetLeadById(id);
                if (updatelead is null)
                {
                    _logger.LogError($" updatelead with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" updatelead with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }



                var leadaddress = _mapper.Map<IEnumerable<LeadAddress>>(leadUpdateDto.LeadAddresses);

               

                var data = _mapper.Map(leadUpdateDto, updatelead);


                data.LeadAddress = leadaddress.ToList();
                

                string result = await _repository.LeadRepository.UpdateLead(data);
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
                _logger.LogError($"Something went wrong inside updateLead action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<CustomerInfoController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLead(int id)
        {
            ServiceResponse<LeadDto> serviceResponse = new ServiceResponse<LeadDto>();

            try
            {
                var deletelead = await _repository.LeadRepository.GetLeadById(id);
                if (deletelead == null)
                {
                    _logger.LogError($"deletelead  with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $" deletelead with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.LeadRepository.DeleteLead(deletelead);
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
                _logger.LogError($"Something went wrong inside deletelead action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}
