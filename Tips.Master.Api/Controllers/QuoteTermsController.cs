using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tips.Master.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class QuoteTermsController : ControllerBase
    {
        private IRepositoryWrapperForMaster _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        public QuoteTermsController(IRepositoryWrapperForMaster repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/<QuoteTermsController>
        [HttpGet]
        public async Task<IActionResult> GetAllQuoteTerms([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<QuoteTermsDto>> serviceResponse = new ServiceResponse<IEnumerable<QuoteTermsDto>>();
            try
            {
                var quoteTermsList = await _repository.QuoteTermsRepository.GetAllQuoteTerms(searchParams);
                _logger.LogInfo("Returned all QuoteTerms");
                var result = _mapper.Map<IEnumerable<QuoteTermsDto>>(quoteTermsList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Successfully Returned all QuoteTerms";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
                
            }
        }

        // GET api/<QuoteTermsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuoteTermsById(int id)
        {
            ServiceResponse<QuoteTermsDto> serviceResponse = new ServiceResponse<QuoteTermsDto>();
            try
            {
                var quoteTerms = await _repository.QuoteTermsRepository.GetQuoteTermsById(id);
                if (quoteTerms == null)
                {
                    _logger.LogError($"QuoteTerms with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "QuoteTerms hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                    
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    var result = _mapper.Map<QuoteTermsDto>(quoteTerms);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "QuoteTerms Successfully Returned";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetQuoteTermsById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<QuoteTermsController>
        [HttpPost]
        public IActionResult CreateQuoteTerms([FromBody] QuoteTermsDtoPost quoteTermsDtoPost)
        {
            ServiceResponse<QuoteTermsDtoPost> serviceResponse = new ServiceResponse<QuoteTermsDtoPost>();
            try
            {
                if (quoteTermsDtoPost is null)
                {
                    _logger.LogError("QuoteTerms object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "QuoteTerms object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid QuoteTerms object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid QuoteTerms object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                  
                }
                var quoteTermsEntity = _mapper.Map<QuoteTerms>(quoteTermsDtoPost);
                _repository.QuoteTermsRepository.CreateQuoteTerms(quoteTermsEntity);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "QuoteTerms Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
               
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
              
            }
        }

        // PUT api/<QuoteTermsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuoteTerms(int id, [FromBody] QuoteTermsDtoUpdate quoteTermsDtoUpdate)
        {
            ServiceResponse<QuoteTermsDtoUpdate> serviceResponse = new ServiceResponse<QuoteTermsDtoUpdate>();
            try
            {
                if (quoteTermsDtoUpdate is null)
                {
                    _logger.LogError("QuoteTerms object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "QuoteTerms object sent from client is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                   
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid QuoteTerms object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid QuoteTerms object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                    
                }
                var quoteTermsEntity = await _repository.QuoteTermsRepository.GetQuoteTermsById(id);
                if (quoteTermsEntity is null)
                {
                    _logger.LogError($"QuoteTerms with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "QuoteTerms hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                    
                }
                _mapper.Map(quoteTermsDtoUpdate, quoteTermsEntity);
                string result = await _repository.QuoteTermsRepository.UpdateQuoteTerms(quoteTermsEntity);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "QuoteTerms Successfully Updated";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
             
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside UpdateQuoteTerms action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
                
            }
        }

        // DELETE api/<QuoteTermsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuoteTerms(int id)
        {
            ServiceResponse<QuoteTermsDto> serviceResponse = new ServiceResponse<QuoteTermsDto>();
            try
            {
                var quoteTerms = await _repository.QuoteTermsRepository.GetQuoteTermsById(id);
                if (quoteTerms == null)
                {
                    _logger.LogError($"QuoteTerms with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "QuoteTerms hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                    
                }
                string result = await _repository.QuoteTermsRepository.DeleteQuoteTerms(quoteTerms);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "QuoteTerms Successfully Deleted";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActivateQuoteTerms(int id)
        {
            ServiceResponse<SegmentDto> serviceResponse = new ServiceResponse<SegmentDto>();
            try
            {
                var quoteTerms = await _repository.QuoteTermsRepository.GetQuoteTermsById(id);
                if (quoteTerms is null)
                {
                    _logger.LogError($"QuoteTerms with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "QuoteTerms hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                quoteTerms.ActiveStatus = true;
                string result = await _repository.QuoteTermsRepository.UpdateQuoteTerms(quoteTerms);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned ActivateQuoteTerms";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ActivateQuoteTerms action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> DeactivateQuoteTerms(int id)
        {
            ServiceResponse<SegmentDto> serviceResponse = new ServiceResponse<SegmentDto>();
            try
            {
                var quoteTerms = await _repository.QuoteTermsRepository.GetQuoteTermsById(id);
                if (quoteTerms is null)
                {
                    _logger.LogError($"QuoteTerms with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "QuoteTerms hasn't been found in db";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                quoteTerms.ActiveStatus = false;
                string result = await _repository.QuoteTermsRepository.UpdateQuoteTerms(quoteTerms);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Returned DeactivateQuoteTerms";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeactivateQuoteTerms action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong,Try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
