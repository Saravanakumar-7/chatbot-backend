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

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompanyMasterController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public CompanyMasterController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<CompanyMasterController>
        [HttpGet]
        public async Task<IActionResult> GetAllCompanyMaster([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<CompanyMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<CompanyMasterDto>>();
            try
            {
                var getallCompanyMasters = await _repository.CompanyMasterRepository.GetAllCompanyMasters(pagingParameter);
                var metadata = new
                {
                    getallCompanyMasters.TotalCount,
                    getallCompanyMasters.PageSize,
                    getallCompanyMasters.CurrentPage,
                    getallCompanyMasters.HasNext,
                    getallCompanyMasters.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all CompanyMasters");
                var result = _mapper.Map<IEnumerable<CompanyMasterDto>>(getallCompanyMasters);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all CompanyMasters Successfully";
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

        // GET api/<CompanyMasterController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyMasterById(int id)
        {
            ServiceResponse<CompanyMasterDto> serviceResponse = new ServiceResponse<CompanyMasterDto>();
            try
            {
                var getCompanyMasterbyId = await _repository.CompanyMasterRepository.GetCompanyMasterById(id);

                if (getCompanyMasterbyId == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CompanyMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CompanyMaster with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned CompanyMaster with id: {id}");
                    var result = _mapper.Map<CompanyMasterDto>(getCompanyMasterbyId);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Returned CompanyMasterById Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCompanyMasterById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<CompanyMasterController>
        [HttpPost]
        public async Task<IActionResult> CreateCompanyMaster([FromBody] CompanyMasterDtoPost companyMasterDtoPost)
        {
            ServiceResponse<CompanyMasterDtoPost> serviceResponse = new ServiceResponse<CompanyMasterDtoPost>();
            try
            {
                if (companyMasterDtoPost is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "CompanyMaster object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("CompanyMaster object sent from client is null.");
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CompanyMaster object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid CompanyMaster object sent from client.");
                    return BadRequest(serviceResponse);
                }

                var CompanyMaster = _mapper.Map<CompanyMaster>(companyMasterDtoPost);
                var Contacts = _mapper.Map<IEnumerable<CompanyContacts>>(companyMasterDtoPost.CompanyContacts);
                var Bankings = _mapper.Map<IEnumerable<CompanyBanking>>(companyMasterDtoPost.CompanyBankings);
                var Addresses = _mapper.Map<IEnumerable<CompanyAddresses>>(companyMasterDtoPost.CompanyAddresses);
                var CompanymasterHeadCount = _mapper.Map<IEnumerable<CompanyMasterHeadCounting>>(companyMasterDtoPost.CompanyMasterHeadCountings);

              

                await _repository.CompanyMasterRepository.CreateCompanyMaster(CompanyMaster);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CompanyMaster Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.Created;
                return Created("GetCompanyMasterById", serviceResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateCompanyMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // PUT api/<CompanyMasterController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompanyMaster(int id, [FromBody] CompanyMasterDto companyMasterDtoUpdate)
        {
            ServiceResponse<CompanyMasterDto> serviceResponse = new ServiceResponse<CompanyMasterDto>();
            try
            {
                if (companyMasterDtoUpdate is null)
                {
                    _logger.LogError("Update CompanyMaster object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update CompanyMaster object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update CompanyMaster object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update CompanyMaster object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var updateCompanyMaster = await _repository.CompanyMasterRepository.GetCompanyMasterById(id);
                if (updateCompanyMaster is null)
                {
                    _logger.LogError($"Update CompanyMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update CompanyMaster with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound; 
                    return NotFound(serviceResponse);
                }


                var Addresses = _mapper.Map<IEnumerable<CompanyAddresses>>(companyMasterDtoUpdate.CompanyAddresses);
                var Contacts = _mapper.Map<IEnumerable<CompanyContacts>>(companyMasterDtoUpdate.CompanyContacts);
                var Bankings = _mapper.Map<IEnumerable<CompanyBanking>>(companyMasterDtoUpdate.CompanyBankings);
                var CompanymasterHeadCounting = _mapper.Map<IEnumerable<CompanyMasterHeadCounting>>(companyMasterDtoUpdate.CompanyMasterHeadCountings);

                var companyMaster = _mapper.Map(companyMasterDtoUpdate, updateCompanyMaster);


                companyMaster.CompanyAddresses = Addresses.ToList();
                companyMaster.CompanyContacts = Contacts.ToList();
                companyMaster.CompanyBankings = Bankings.ToList();
                companyMaster.CompanyMasterHeadCountings = CompanymasterHeadCounting.ToList();

                string result = await _repository.CompanyMasterRepository.UpdateCompanyMaster(companyMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CompanyMaster Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCompanyMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // DELETE api/<CompanyMasterController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyMaster(int id)
        {
            ServiceResponse<CompanyMasterDto> serviceResponse = new ServiceResponse<CompanyMasterDto>();
            try
            {
                var deleteCompanyMaster = await _repository.CompanyMasterRepository.GetCompanyMasterById(id);
                if (deleteCompanyMaster == null)
                {
                    _logger.LogError($"Delete Company with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Company with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.CompanyMasterRepository.DeleteCompanyMaster(deleteCompanyMaster);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " CompanyMaster Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteCompanyMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveCompanyIdNameList()
        {
            ServiceResponse<IEnumerable<CompanyIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<CompanyIdNameListDto>>();
            try
            {
                var getAllActiveCompanymasterIdNameList = await _repository.CompanyMasterRepository.GetAllActiveCompanyMasterIdNameList();
                var result = _mapper.Map<IEnumerable<CompanyIdNameListDto>>(getAllActiveCompanymasterIdNameList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveCompanyIdNameList Successfully";
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

    }
}
