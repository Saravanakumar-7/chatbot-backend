using System.Net;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Repository;

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private IQuoteRepository _repository;
        private ILoggerManager _logger;
        private IMapper _mapper;
 
        public QuoteController(IQuoteRepository repository,ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
         }

        // GET: api/<QuoteController>
        [HttpGet]
        public async Task<IActionResult> GetAllQuote([FromQuery] PagingParameter pagingParameter)
        {
            ServiceResponse<IEnumerable<QuoteDto>> serviceResponse = new ServiceResponse<IEnumerable<QuoteDto>>();
            try
            {
                var listOfQuote = await _repository.GetAllQuote(pagingParameter);
                var metadata = new
                {
                    listOfQuote.TotalCount,
                    listOfQuote.PageSize,
                    listOfQuote.CurrentPage,
                    listOfQuote.HasNext,
                    listOfQuote.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all Quote");
                var result = _mapper.Map<IEnumerable<QuoteDto>>(listOfQuote);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Quote";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // GET api/<QuoteController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuoteById(int id)
        {
            ServiceResponse<QuoteDto> serviceResponse = new ServiceResponse<QuoteDto>();
            try
            {
                var quoteDetails = await _repository.GetQuoteById(id);

                if (quoteDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Quote  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Quote with id: {id}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned owner with id: {id}");
                    
                    var quoteGeneralList=_mapper.Map<IEnumerable<QuoteGeneralDto>>(quoteDetails.quoteGenerals);
                    var quoteAdditionalChargesList = _mapper.Map<IEnumerable<QuoteAdditionalChargesDto>>(quoteDetails.quoteAdditionalCharges);
                    var quoteOtherTermsList = _mapper.Map<IEnumerable<QuoteOtherTermsDto>>(quoteDetails.quoteOtherTerms);
                    var quoteRFQNotesList = _mapper.Map<IEnumerable<QuoteRFQNotesDto>>(quoteDetails.quoteRFQNotes);
                    var quoteSpecialTermslist = _mapper.Map<IEnumerable<QuoteSpecialTermsDto>>(quoteDetails.quoteSpecialTerms);
                    var quote = _mapper.Map<QuoteDto>(quoteDetails);

                    quote.quoteGeneralDtos = quoteGeneralList.ToList();
                    quote.quoteAdditionalChargesDtos = quoteAdditionalChargesList.ToList();
                    quote.quoteOtherTermsDtos = quoteOtherTermsList.ToList();
                    quote.quoteRFQNotesDtos = quoteRFQNotesList.ToList();
                    quote.quoteSpecialTermsDtos = quoteSpecialTermslist.ToList();

                    serviceResponse.Data = quote;
                    serviceResponse.Message = "Returned Quote";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetQuoteById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // POST api/<QuoteController>
        [HttpPost]
        public async Task<IActionResult> CreateQuote([FromBody] QuoteDtoPost quoteDtoPost)
        {
            ServiceResponse<QuoteDto> serviceResponse = new ServiceResponse<QuoteDto>();
             
            try
            {
                if (quoteDtoPost is null)
                {
                    _logger.LogError("QuoteDetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "QuoteDetails object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid QuoteDetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid QuoteDetails object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var quoteGeneralList = _mapper.Map<IEnumerable<QuoteGeneral>>(quoteDtoPost.quoteGeneralDtoPost);
                var quoteAdditionalChargesList = _mapper.Map<IEnumerable<QuoteAdditionalCharges>>(quoteDtoPost.quoteAdditionalChargesDtoPost);
                var quoteOtherTermsList = _mapper.Map<IEnumerable<QuoteOtherTerms>>(quoteDtoPost.quoteOtherTermsDtoPost);
                var quoteRFQNotesList = _mapper.Map<IEnumerable<QuoteRFQNotes>>(quoteDtoPost.quoteRFQNotesDtoPost);
                var quoteSpecialTermslist = _mapper.Map<IEnumerable<QuoteSpecialTerms>>(quoteDtoPost.quoteSpecialTermsDtoPost);
                var quote = _mapper.Map<Quote>(quoteDtoPost);

                quote.quoteGenerals = quoteGeneralList.ToList();
                quote.quoteAdditionalCharges = quoteAdditionalChargesList.ToList();
                quote.quoteOtherTerms = quoteOtherTermsList.ToList();
                quote.quoteRFQNotes = quoteRFQNotesList.ToList();
                quote.quoteSpecialTerms = quoteSpecialTermslist.ToList();

                _repository.CreateQuote(quote);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Quote Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateQuote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        //change versionnumber

        [HttpPost]
        public async Task<IActionResult> ChangeRevisionNumber([FromBody] QuoteDtoPost quoteDtoPost)
        {
            ServiceResponse<QuoteDto> serviceResponse = new ServiceResponse<QuoteDto>();

            try
            {
                if (quoteDtoPost is null)
                {
                    _logger.LogError("QuoteDetails object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "QuoteDetails object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid QuoteDetails object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid QuoteDetails object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var quoteGeneralList = _mapper.Map<IEnumerable<QuoteGeneral>>(quoteDtoPost.quoteGeneralDtoPost);
                var quoteAdditionalChargesList = _mapper.Map<IEnumerable<QuoteAdditionalCharges>>(quoteDtoPost.quoteAdditionalChargesDtoPost);
                var quoteOtherTermsList = _mapper.Map<IEnumerable<QuoteOtherTerms>>(quoteDtoPost.quoteOtherTermsDtoPost);
                var quoteRFQNotesList = _mapper.Map<IEnumerable<QuoteRFQNotes>>(quoteDtoPost.quoteRFQNotesDtoPost);
                var quoteSpecialTermslist = _mapper.Map<IEnumerable<QuoteSpecialTerms>>(quoteDtoPost.quoteSpecialTermsDtoPost);
                var quote = _mapper.Map<Quote>(quoteDtoPost);

                quote.quoteGenerals = quoteGeneralList.ToList();
                quote.quoteAdditionalCharges = quoteAdditionalChargesList.ToList();
                quote.quoteOtherTerms = quoteOtherTermsList.ToList();
                quote.quoteRFQNotes = quoteRFQNotesList.ToList();
                quote.quoteSpecialTerms = quoteSpecialTermslist.ToList();

                _repository.ChangeQuoteVersion(quote);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Quote Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreateQuote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }




        // PUT api/<QuoteController>/5

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateQuote(int id, [FromBody] QuoteDtoUpdate quoteDtoUpdate)
        //{
            
        //    ServiceResponse<QuoteDtoUpdate> serviceResponse = new ServiceResponse<QuoteDtoUpdate>();

        //    try
        //    {
        //        if (quoteDtoUpdate is null)
        //        {
        //            _logger.LogError("Update Quote object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Update Quote object is null";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid Update Quote object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid Update Quote object sent from client.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        var updateQuote = await _repository.GetQuoteById(id);
        //        if (updateQuote is null)
        //        {
        //            _logger.LogError($"Update Quote with id: {id}, hasn't been found in db.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = $"UpdateQuote hasn't been found in db.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.NotFound;
        //            return NotFound(serviceResponse);
        //        }
        //        var quoteGeneralList = _mapper.Map<IEnumerable<QuoteGeneral>>(quoteDtoUpdate.quoteGeneralDtoUpdate);
        //        var quoteAdditionalChargesList = _mapper.Map<IEnumerable<QuoteAdditionalCharges>>(quoteDtoUpdate.quoteAdditionalChargesDtoUpdate);
        //        var quoteOtherTermsList = _mapper.Map<IEnumerable<QuoteOtherTerms>>(quoteDtoUpdate.quoteOtherTermsDtoUpdate);
        //        var quoteRFQNotesList = _mapper.Map<IEnumerable<QuoteRFQNotes>>(quoteDtoUpdate.quoteRFQNotesDtoUpdate);
        //        var quoteSpecialTermslist = _mapper.Map<IEnumerable<QuoteSpecialTerms>>(quoteDtoUpdate.quoteSpecialTermsDtoUpdate);


        //        var quote = _mapper.Map(quoteDtoUpdate, updateQuote);

        //        quote.quoteGenerals = quoteGeneralList.ToList();
        //        quote.quoteAdditionalCharges = quoteAdditionalChargesList.ToList();
        //        quote.quoteOtherTerms = quoteOtherTermsList.ToList();
        //        quote.quoteRFQNotes = quoteRFQNotesList.ToList();
        //        quote.quoteSpecialTerms = quoteSpecialTermslist.ToList();


        //        var version = Convert.ToDecimal(0.1);

        //        quote.RevisionNumber = quote.RevisionNumber + version;

        //        var quoteDetails = await _repository.GetQuoteById(id);

        //         string result = await _repository.UpdateQuote(quote);
                 
        //         _repository.SaveAsync();

        //        _logger.LogInfo(result);
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Updated Quote Successfully";
        //        serviceResponse.Success = true;
        //        serviceResponse.StatusCode = HttpStatusCode.OK;
        //        return Ok(serviceResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside UpdateQuote action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}


        // DELETE api/<VendorController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            ServiceResponse<QuoteDto> serviceResponse = new ServiceResponse<QuoteDto>();

            try
            {
                var deleteQuote = await _repository.GetQuoteById(id);
                if (deleteQuote == null)
                {
                    _logger.LogError($"Delete Quote with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete Quote hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteQuote(deleteQuote);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Quote Deleted Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
