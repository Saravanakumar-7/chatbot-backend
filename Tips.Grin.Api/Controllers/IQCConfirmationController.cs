using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Repository;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IQCConfirmationController : ControllerBase
    {
        private IIQCConfirmationRepository _iQCConfirmationRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;

        public IQCConfirmationController(IIQCConfirmationRepository iQCConfirmationRepository,ILoggerManager logger, IMapper mapper) 
        {
            _logger = logger;
            _iQCConfirmationRepository = iQCConfirmationRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllIqcDetails()
        {
                ServiceResponse<IEnumerable<IQCConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationDto>>();

            try
            {
                var IQCList = await _iQCConfirmationRepository.GetAllIqcDetails();
                _logger.LogInfo("Returned all IQCConfirmation details()s");
                var result = _mapper.Map<IEnumerable<IQCConfirmationDto>>(IQCList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all IQCConfirmation";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

            [HttpGet("{grinNumber}")]
            public async Task<IActionResult> GetIqcDetailsbyGrinNo(string grinNumber) 
            {
                ServiceResponse<IEnumerable<IQCConfirmationDto>> serviceResponse = new ServiceResponse<IEnumerable<IQCConfirmationDto>>();

                try
                {
                    var IQCList = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                    if (IQCList == null)
                    {
                        _logger.LogError($"IQCConfirmation Details with GrinNumber: {grinNumber}, hasn't been found in db.");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"IQCConfirmation Details with GrinNumber hasn't been found in db.";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogInfo($"Returned IQCConfirmation Details with id: {grinNumber}");
                        var result = _mapper.Map<IEnumerable<IQCConfirmationDto>>(IQCList);
                        serviceResponse.Data = result;
                        serviceResponse.Message = "Successfully Returned IQCConfirmationbyGrinNo";
                        serviceResponse.Success = true;
                        serviceResponse.StatusCode = HttpStatusCode.OK;
                        return Ok(result);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Something went wrong inside IQCConfirmationByGrinNo action: {ex.Message}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Inter server error";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, "Internal server error");
                }
            }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIqc(int id, [FromBody] IQCConfirmationDto IQCConfirmationUpdateDto)
        {
            ServiceResponse<IQCConfirmationDto> serviceResponse = new ServiceResponse<IQCConfirmationDto>();

            try
            {
                if (IQCConfirmationUpdateDto is null)
                {
                    _logger.LogError("IQCConfirmation details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update IQCConfirmation details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid IQCConfirmation details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var IQCList = await _iQCConfirmationRepository.GetIqcDetailsbyId(id);
                if (IQCList is null)
                {
                    _logger.LogError($"IQCConfirmation details with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }

                var IQCEntity = _mapper.Map(IQCConfirmationUpdateDto, IQCList);

                string result = await _iQCConfirmationRepository.UpdateIqc(IQCEntity);
                _logger.LogInfo(result);
                _iQCConfirmationRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "IQCConfirmation Updated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Update IQCConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult CreateIqc([FromBody] IQCConfirmationPostDto iQCConfirmationPostDto)
        {
            ServiceResponse<IQCConfirmationPostDto> serviceResponse = new ServiceResponse<IQCConfirmationPostDto>();

            try
            {
                if (iQCConfirmationPostDto == null)
                {
                    _logger.LogError("IQCConfirmation details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmation details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid IQCConfirmation details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }


                var IQCList = _mapper.Map<IQCConfirmation>(iQCConfirmationPostDto);
                
                _iQCConfirmationRepository.CreateIqc(IQCList);
                _iQCConfirmationRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "IQCConfirmation Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("IQCConfirmationById", "Successfully Created");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Create IQCConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");


            }
        }

        //bulk save

        [HttpPost]
        public async Task<IActionResult> SaveMultipleIqc([FromBody] List<IQCConfirmationPostDto> iQCConfirmationPostDtos)
        {
            ServiceResponse<IQCConfirmationPostDto> serviceResponse = new ServiceResponse<IQCConfirmationPostDto>();

            try
            {
                if (iQCConfirmationPostDtos == null)
                {
                    _logger.LogError("IQCConfirmation details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "IQCConfirmation details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid IQCConfirmation details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var IqcConfirmationList = _mapper.Map<List<IQCConfirmation>>(iQCConfirmationPostDtos);
                bool isAnyRecordCreated = false;
                foreach (var iqcDetails in IqcConfirmationList)
                {
                    if(iqcDetails.AcceptedQunatity > 0 || iqcDetails.RejectedQunatity > 0)
                    {
                        _iQCConfirmationRepository.Create(iqcDetails);
                        isAnyRecordCreated = true;
                    }                     
                }
                if (isAnyRecordCreated)
                {
                    _iQCConfirmationRepository.SaveAsync();
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Created("IQCConfirmationById", serviceResponse);
                }
                else
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Nothing to save,Because accepted or rejected quantity is not greater than 0 in any rows !";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Created("IQCConfirmationById", serviceResponse);
                }
            
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Create IQCConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");


            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIqcDetailsbyId(int id)
        {
            ServiceResponse<IQCConfirmationDto> serviceResponse = new ServiceResponse<IQCConfirmationDto>();

            try
            {
                var IQCList = await _iQCConfirmationRepository.GetIqcDetailsbyId(id);
                if (IQCList == null)
                {
                    _logger.LogError($"IQCConfirmation details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation details with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned IQCConfirmation details with id: {id}");
                    var result = _mapper.Map<IQCConfirmationDto>(IQCList);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside IQCConfirmationById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIQC(int id)
        {
            ServiceResponse<IQCConfirmationDto> serviceResponse = new ServiceResponse<IQCConfirmationDto>();

            try
            {
                var shopOrders = await _iQCConfirmationRepository.GetIqcDetailsbyId(id);
                if (shopOrders == null)
                {
                    _logger.LogError($"Confirmation with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"IQCConfirmation with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                shopOrders.IsDeleted = true;
                string result = await _iQCConfirmationRepository.UpdateIqc(shopOrders);
                serviceResponse.Message = "IQCConfirmation Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Delete IQCConfirmation action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        /*
        [HttpPost]
        public async Task<IActionResult> CreateAllIqc([FromBody] IQCConfirmationPostDto iQCConfirmationPostDto)
        {
            ServiceResponse<IQCConfirmationPostDto> serviceResponse = new ServiceResponse<IQCConfirmationPostDto>();

            try
            {
                var bank = await _iQCConfirmationRepository.GetBankById(id);
                if (bank is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Bank object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    _logger.LogError($"bank with id: {id}, hasn't been found in db.");
                    return BadRequest(serviceResponse);
                }
                bank.IsActive = false;
                string result = await _iQCConfirmationRepository.UpdateBank(bank);
                _logger.LogInfo(result);
                _iQCConfirmationRepository.SaveAsync();
                serviceResponse.Message = "Deactivated Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError($"Something went wrong inside Deactivate Bank action: {ex.Message}");
                return StatusCode(500, serviceResponse);
            }
        }*/
    }
}
