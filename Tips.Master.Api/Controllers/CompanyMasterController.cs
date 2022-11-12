using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAllCompanyMaster()
        {
            ServiceResponse<IEnumerable<CompanyMasterDto>> serviceResponse = new ServiceResponse<IEnumerable<CompanyMasterDto>>();
            try
            {
                var listOfCompanyMaster = await _repository.CompanyMasterRepository.GetAllCompanyMaster();
                _logger.LogInfo("Returned all CompanyMaster");
                var result = _mapper.Map<IEnumerable<CompanyMasterDto>>(listOfCompanyMaster);
                serviceResponse.Data = result;
                serviceResponse.Message = "Success";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllCompanyMaster action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        // GET api/<CompanyMasterController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyMasterById(int id)
        {
            ServiceResponse<CompanyMasterDto> serviceResponse = new ServiceResponse<CompanyMasterDto>();
            try
            {
                var CompanyMasterDetails = await _repository.CompanyMasterRepository.GetCompanyMasterById(id);

                if (CompanyMasterDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"CompanyMaster with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"CompanyMaster with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<CompanyMasterDto>(CompanyMasterDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetCompanyMasterById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetCompanyMasterById action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
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
                    return BadRequest("CompanyMaster object is null");
                }
                if (!ModelState.IsValid)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid CompanyMaster object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError("Invalid CompanyMaster object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var CompanyMaster = _mapper.Map<CompanyMaster>(companyMasterDtoPost);
                var Contacts = _mapper.Map<IEnumerable<CompanyContacts>>(companyMasterDtoPost.CompanyContacts);
                var Bankings = _mapper.Map<IEnumerable<CompanyBanking>>(companyMasterDtoPost.CompanyBankings);
                var Addresses = _mapper.Map<IEnumerable<CompanyAddresses>>(companyMasterDtoPost.CompanyAddresses);
                

                await _repository.CompanyMasterRepository.CreateCompanyMaster(CompanyMaster);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CompanyMaster Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.Created;
                return Created("GetCompanyMasterById", "Successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside CreateOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<CompanyMasterController>/5
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateCompanyMaster(int id, [FromBody] CompanyMasterDtoUpdate companyMasterDtoUpdate)
        {
            ServiceResponse<CompanyMasterDtoUpdate> serviceResponse = new ServiceResponse<CompanyMasterDtoUpdate>();
            try
            {
                if (companyMasterDtoUpdate is null)
                {
                    _logger.LogError("Update CompanyMaster object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update CompanyMaster object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest("Update CompanyMaster object is null");
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Update CompanyMaster object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid Update CompanyMaster object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest("Invalid model object");
                }
                var updateCompanyMaster = await _repository.CompanyMasterRepository.GetCompanyMasterById(id);
                if (updateCompanyMaster is null)
                {
                    _logger.LogError($"Update CompanyMaster with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update CompanyMaster with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var Addresses = _mapper.Map<IEnumerable<CompanyAddresses>>(companyMasterDtoUpdate.CompanyAddresses);
                var Contacts = _mapper.Map<IEnumerable<CompanyContacts>>(companyMasterDtoUpdate.CompanyContacts);
                var Banking = _mapper.Map<IEnumerable<CompanyBanking>>(companyMasterDtoUpdate.CompanyBankings);
                var data = _mapper.Map(companyMasterDtoUpdate, updateCompanyMaster);

                data.CompanyAddresses = Addresses.ToList();
                data.CompanyContacts = Contacts.ToList();
                data.CompanyBankings = Banking.ToList();

                string result = await _repository.CompanyMasterRepository.UpdateCompanyMaster(data);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = " CompanyMaster Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.NoContent;
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateCompanyMaster action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside UpdateCompanyMaster action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<CompanyMasterController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyMaster(int id)
        {
            ServiceResponse<CompanyMasterDto> serviceResponse = new ServiceResponse<CompanyMasterDto>();
            try
            {
                var deleteCompany = await _repository.CompanyMasterRepository.GetCompanyMasterById(id);
                if (deleteCompany == null)
                {
                    _logger.LogError($"Delete Company with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Company with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.CompanyMasterRepository.DeleteCompanyMaster(deleteCompany);
                _logger.LogInfo(result);
                _repository.SaveAsync();

                serviceResponse.Data = null;
                serviceResponse.Message = " CompanyMaster Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.NoContent;
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside DeleteOwner action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
