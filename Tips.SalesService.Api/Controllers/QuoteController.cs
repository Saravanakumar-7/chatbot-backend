using System.Net;
using System.Net.Http;
using AutoMapper;
using Contracts;
using Entities;
using Entities.DTOs;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;

using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using NuGet.Packaging;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Repository;
using EmailIDsDto = Tips.SalesService.Api.Entities.DTOs.EmailIDsDto;
using EmailTemplateDto = Tips.SalesService.Api.Entities.DTOs.EmailTemplateDto;

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class QuoteController : ControllerBase
    {
        private IQuoteRepository _repository;
        private IQuoteEmailsDetailsRepository _quoteEmailsDetailsRepository;
        private IRfqCustomerSupportItemRepository _rfqCustomerSupportItemRepository;
        private IRfqRepository _rfqRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public QuoteController(IQuoteRepository repository, IQuoteEmailsDetailsRepository quoteEmailsDetailsRepository, IHttpClientFactory clientFactory, HttpClient httpClient, IConfiguration config, IRfqCustomerSupportItemRepository rfqCustomerSupportItemRepository, IRfqRepository rfqRepository, ILoggerManager logger, IMapper mapper)
        {
            _quoteEmailsDetailsRepository=quoteEmailsDetailsRepository;
            _repository = repository;
            _logger = logger;
            _clientFactory = clientFactory;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _rfqRepository = rfqRepository;
            _rfqCustomerSupportItemRepository = rfqCustomerSupportItemRepository;
        }

        // GET: api/<QuoteController>
        [HttpGet]
        public async Task<IActionResult> GetAllQuote([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<QuoteDto>> serviceResponse = new ServiceResponse<IEnumerable<QuoteDto>>();
            try
            {
                var listOfQuote = await _repository.GetAllQuote(pagingParameter, searchParammes);
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
                serviceResponse.Message = "Returned all Quotes Successfully";
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
                string serverKey = GetServerKey();
                var quoteDetails = await _repository.GetQuoteById(id);
                var rfqnumber = quoteDetails.RFQNumber;
                //var customerId = await _rfqRepository.GetCustomerIdByRfqNumber(rfqnumber);

                if (quoteDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Quote  hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Quote with id: {id}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Quote with id: {id}");

                    List<QuoteGeneralDto> quoteGeneralDtos = new List<QuoteGeneralDto>();


                    var quoteGeneralList = _mapper.Map<IEnumerable<QuoteGeneralDto>>(quoteDetails.QuoteGenerals);

                    //foreach (var quoteItems in quoteGeneralList)
                    //{ 
                    //    var priceListNamesResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"], "GetStockDetailsForAllLocationWarehouseByItemNo?", "ItemNumber=", quoteItems.ItemNumber));
                    //    var inventoryObjectString = await priceListNamesResult.Content.ReadAsStringAsync();
                    //    dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                    //    dynamic inventoryObject = inventoryObjectData;
                    //    quoteItems.AvailableStock = inventoryObject.ToDecimal();
                    //}

                    foreach (var quoteItems in quoteGeneralList)
                    {
                        if (serverKey == "keus")
                        {
                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();
                            var encodedItemNumber = Uri.EscapeDataString(quoteItems.ItemNumber);
                            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetStockDetailsForAllLocationWarehouseByItemNo?ItemNumber={encodedItemNumber}"));
                            request.Headers.Add("Authorization", token);

                            var inventoryObjectResult = await client.SendAsync(request);
                            //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"], "GetStockDetailsForAllLocationWarehouseByItemNo?",
                            //                                                                                                                "ItemNumber=", quoteItems.ItemNumber));
                            var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                            dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                            dynamic inventoryObject = inventoryObjectData.data;

                            // Convert double to decimal
                            decimal availableStock = Convert.ToDecimal(inventoryObject);

                            quoteItems.AvailableStock = availableStock;
                        }
                        else
                        {
                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();
                            var encodedItemNumber = Uri.EscapeDataString(quoteItems.ItemNumber);
                            var encodedRFQNumber = Uri.EscapeDataString(rfqnumber);
                            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                            $"GetStockDetailsForAllLocationWarehouseByItemNoAndProjectNo?ItemNumber={encodedItemNumber}&ProjectNo={encodedRFQNumber}"));
                            request.Headers.Add("Authorization", token);

                            var inventoryObjectResult = await client.SendAsync(request);
                            //var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"], "GetStockDetailsForAllLocationWarehouseByItemNoAndProjectNo?",
                            //                                                                                                    "ItemNumber=", quoteItems.ItemNumber, "&ProjectNo=",rfqnumber));
                            var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                            dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                            dynamic inventoryObject = inventoryObjectData;

                            // Convert double to decimal
                            decimal availableStock = Convert.ToDecimal(inventoryObject);

                            quoteItems.AvailableStock = availableStock;
                        }
                    }

                    var quoteAdditionalChargesList = _mapper.Map<IEnumerable<QuoteAdditionalChargesDto>>(quoteDetails.QuoteAdditionalCharges);
                    var quoteOtherTermsList = _mapper.Map<IEnumerable<QuoteOtherTermsDto>>(quoteDetails.QuoteOtherTerms);
                    var quoteRFQNotesList = _mapper.Map<IEnumerable<QuoteRFQNotesDto>>(quoteDetails.QuoteRFQNotes);
                    var quoteSpecialTermslist = _mapper.Map<IEnumerable<QuoteSpecialTermsDto>>(quoteDetails.QuoteSpecialTerms);
                    var quote = _mapper.Map<QuoteDto>(quoteDetails);
                    //quote.CustomerId = customerId.CustomerId;
                    quote.QuoteGeneralDtos = quoteGeneralList.ToList();
                    quote.QuoteAdditionalChargesDtos = quoteAdditionalChargesList.ToList();
                    quote.QuoteOtherTermsDtos = quoteOtherTermsList.ToList();
                    quote.QuoteRFQNotesDtos = quoteRFQNotesList.ToList();
                    quote.QuoteSpecialTermsDtos = quoteSpecialTermslist.ToList();

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


        //getcsitemdetailsbyrfqnumberforquote

        [HttpGet]
        public async Task<IActionResult> GetCsItemDetailsForQuote(string rfqNumber)
        {
            ServiceResponse<IEnumerable<CsItemDetailsForQuoteDto>> serviceResponse = new ServiceResponse<IEnumerable<CsItemDetailsForQuoteDto>>();

            try
            {

                //get latest price list name from master
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                //var priceListNamesResult = await _httpClient.GetAsync(string.Concat(_config["PriceListAPI"],
                //             "GetLatestPriceListName?"));
                var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["PriceListAPI"], "GetLatestPriceListName"));
                request.Headers.Add("Authorization", token);

                var priceListNamesResult = await client.SendAsync(request);
                var priceListNamesString = await priceListNamesResult.Content.ReadAsStringAsync();
                dynamic priceListNamesData = JsonConvert.DeserializeObject(priceListNamesString);
                dynamic priceListNamesObject = priceListNamesData.data;

                List<string> priceListNames = new List<string>();

                foreach (var item in priceListNamesObject)
                {
                    // Assuming "pricelistname" is a property of each item
                    string priceListName = item.pricelist;

                    // Add the priceListName to the list
                    priceListNames.Add(priceListName);
                }

                // Now, you have a list of priceListNames that you can pass to the repository  
                var getItemPriceList = await _repository.GetCsItemDetailsForQuote(rfqNumber, priceListNames);

                if (getItemPriceList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "itemPriceList not found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"itemPriceList with : {rfqNumber}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = getItemPriceList;
                    serviceResponse.Message = "Returned priceList";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside ItemPricelist action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetRfqEnggDetailsByRfqNo(string rfqNumber)
        {
            ServiceResponse<IEnumerable<rfqEnggItemDetailsForQuoteDto>> serviceResponse = new ServiceResponse<IEnumerable<rfqEnggItemDetailsForQuoteDto>>();

            try
            {
                var rfqEnggItemList = await _repository.GetAllRfqEnggDetailsByRfqNo(rfqNumber);

                if (rfqEnggItemList == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "itemPriceList not found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"itemPriceList with : {rfqNumber}, hasn't been found.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = rfqEnggItemList;
                    serviceResponse.Message = "Returned priceList";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRfqEnggDetailsByRfqNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        private string GetServerKey()
        {
            var serverName = Environment.MachineName;
            var serverConfiguration = _config.GetSection("ServerConfiguration");

            if (serverConfiguration.GetValue<bool?>("Server1:EnableKeus") == true)
            {
                return "keus";
            }
            else if (serverConfiguration.GetValue<bool?>("Server1:EnableAvision") == true)
            {
                return "avision";

            }
            else
            {
                return "trasccon";
            }
        }
        // POST api/<QuoteController>
        [HttpPost]
        public async Task<IActionResult> CreateQuote([FromBody] QuotePostDto quotePostDto)
        {
            ServiceResponse<QuoteDto> serviceResponse = new ServiceResponse<QuoteDto>();

            try
            {
                string serverKey = GetServerKey();
                if (quotePostDto is null)
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

                var quoteGeneralList = _mapper.Map<IEnumerable<QuoteGeneral>>(quotePostDto.QuoteGeneralPostDtos);
                var quoteAdditionalChargesList = _mapper.Map<IEnumerable<QuoteAdditionalCharges>>(quotePostDto.QuoteAdditionalChargesPostDtos);
                var quoteOtherTermsList = _mapper.Map<IEnumerable<QuoteOtherTerms>>(quotePostDto.QuoteOtherTermsPostDtos);
                var quoteRFQNotesList = _mapper.Map<IEnumerable<QuoteRFQNotes>>(quotePostDto.QuoteRFQNotesPostDtos);
                var quoteSpecialTermslist = _mapper.Map<IEnumerable<QuoteSpecialTerms>>(quotePostDto.QuoteSpecialTermsPostDtos);
                var quote = _mapper.Map<Quote>(quotePostDto);

                var date = DateTime.Now;
                var days = Convert.ToString(date.Day.ToString("D2"));
                var months = Convert.ToString(date.Month.ToString("D2"));
                var years = Convert.ToString(date.ToString("yy"));


                //var newcount = await _repository.GetQuoteNumberAutoIncrementCount(date);

                //if (newcount > 0)
                //{
                //    var number = newcount + 1;
                //    string e = String.Format("{0:D4}", number);
                //    quote.QuoteNumber = days + months + years + "QT" + (e);
                //}
                //else
                //{
                //    var count = 1;
                //    var e = count.ToString("D4");
                //    quote.QuoteNumber = days + months + years + "QT" + (e);
                //}

                if (serverKey == "trasccon")
                {
                    var dateFormat = days + months + years;
                    var quoteNumber = await _repository.GenerateQuoteNumber();
                    quote.QuoteNumber = dateFormat + quoteNumber;
                }
                else if (serverKey == "keus")
                {
                    var dateFormat = days + months + years;
                    var quoteNumber = await _repository.GenerateQuoteNumber();
                    quote.QuoteNumber = dateFormat + quoteNumber;
                }
                else
                {
                    var quoteNumber = await _repository.GenerateQuoteNumberAvision();
                    quote.QuoteNumber = quoteNumber;
                }
                quote.QuoteGenerals = quoteGeneralList.ToList();
                quote.QuoteAdditionalCharges = quoteAdditionalChargesList.ToList();
                quote.QuoteOtherTerms = quoteOtherTermsList.ToList();
                quote.QuoteRFQNotes = quoteRFQNotesList.ToList();
                quote.QuoteSpecialTerms = quoteSpecialTermslist.ToList();

                _repository.CreateQuote(quote);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Quote Created Successfully";
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
        public async Task<IActionResult> ChangeRevisionNumber([FromBody] QuoteUpdateDto quotePostDto)
        {
            ServiceResponse<QuotePostDto> serviceResponse = new ServiceResponse<QuotePostDto>();

            try
            {
                if (quotePostDto is null)
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

                var quoteGeneralList = _mapper.Map<IEnumerable<QuoteGeneral>>(quotePostDto.QuoteGeneralUpdateDtos);
                var quoteAdditionalChargesList = _mapper.Map<IEnumerable<QuoteAdditionalCharges>>(quotePostDto.QuoteAdditionalChargesUpdateDtos);
                var quoteOtherTermsList = _mapper.Map<IEnumerable<QuoteOtherTerms>>(quotePostDto.QuoteOtherTermsUpdateDtos);
                var quoteRFQNotesList = _mapper.Map<IEnumerable<QuoteRFQNotes>>(quotePostDto.QuoteRFQNotesUpdateDtos);
                var quoteSpecialTermslist = _mapper.Map<IEnumerable<QuoteSpecialTerms>>(quotePostDto.QuoteSpecialTermsUpdateDtos);
                var quote = _mapper.Map<Quote>(quotePostDto);

                quote.QuoteGenerals = quoteGeneralList.ToList();
                quote.QuoteAdditionalCharges = quoteAdditionalChargesList.ToList();
                quote.QuoteOtherTerms = quoteOtherTermsList.ToList();
                quote.QuoteRFQNotes = quoteRFQNotesList.ToList();
                quote.QuoteSpecialTerms = quoteSpecialTermslist.ToList();

                _repository.ChangeQuoteVersion(quote);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Quote Created Successfully";
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
                _logger.LogError($"Something went wrong inside DeleteQuote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        // To allow short closed
        [HttpPost]
        public async Task<IActionResult> QuoteShortClosed([FromBody] ShortClosedDto shortClosedDto)
        {
            ServiceResponse<QuoteDto> serviceResponse = new ServiceResponse<QuoteDto>();
            try
            {
                if (shortClosedDto.QuoteNumber is null)
                {
                    _logger.LogError("QuoteNumber object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "QuoteNumber object sent from client is null.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid QuoteNumber object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid QuoteNumber object sent from client.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                await _repository.CreateShortClosed(shortClosedDto);
                serviceResponse.Data = null;
                serviceResponse.Message = " Quote ShortClosed Successfully"; ;
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside QuoteShortClosed action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetQuoteSPReport()

        {
            ServiceResponse<IEnumerable<QuoteSPReport>> serviceResponse = new ServiceResponse<IEnumerable<QuoteSPReport>>();
            try
            {
                var products = await _repository.GetQuoteSPReport();

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"QuoteSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"QuoteSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned QuoteSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetQuoteSPReport action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailforQuote(string SentTo,string? CusEmail,string jasperfileUrl,int Quoteid)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailAPI"],"GetEmailTemplatebyProcessType?ProcessType=QuoteEmail"));
                request.Headers.Add("Authorization", token);
                var response = await client.SendAsync(request);
                var EmailTempString = await response.Content.ReadAsStringAsync();
                var emaildetails = JsonConvert.DeserializeObject<EmailTemplateDto>(EmailTempString);

                var Operations = "From";
                var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                request1.Headers.Add("Authorization", token);
                var response1 = await client.SendAsync(request1);
                var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                var emaildetails1 = JsonConvert.DeserializeObject<EmailIDsDto>(EmailTempString1);

                var quoteDetails = await _repository.GetQuoteById(Quoteid);

                var httpclientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(httpclientHandler);
                var mails = SentTo.Split(',').ToList();
                if (!CusEmail.IsNullOrEmpty())
                {
                    mails.Add(CusEmail);
                }
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("chethan.v@wyzmindz.com"));
                email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));
                email.Subject = emaildetails.data.subject;
                string body = emaildetails.data.template;
                body = body.Replace("{{Quote Number}}", quoteDetails.QuoteNumber);
                body = body.Replace("{{RFQ Number}}", quoteDetails.RFQNumber);
                body = body.Replace("{{Revision Number}}", quoteDetails.RevisionNumber.ToString());
                body = body.Replace("{{Created On Date}}", quoteDetails.CreatedOn.ToString());
                body = body.Replace("{{Created By}}", quoteDetails.CreatedBy);
                body = body.Replace("{{Total Final Amount Value}}", quoteDetails.TotalFinalAmount.ToString());
                body = body.Replace("{{Customer Name}}", quoteDetails.CustomerName);
                body = body.Replace("{{Sales Person}}", quoteDetails.SalesPerson);
                var builder = new BodyBuilder();
                builder.HtmlBody= body;
                using (HttpClient client1 = new HttpClient())
                {
                    HttpResponseMessage response2 = await client1.GetAsync(jasperfileUrl);
                    response2.EnsureSuccessStatusCode();
                    byte[] fileBytes = await response2.Content.ReadAsByteArrayAsync();
                    Uri uri = new Uri(jasperfileUrl);
                    var filename= Path.GetFileName(uri.LocalPath);
                    builder.Attachments.Add(filename, fileBytes, ContentType.Parse("application/pdf"));
                }
                    //email.Body = new TextPart(TextFormat.Html) { Text = body };
                    email.Body=builder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                smtp.Send(email);
                smtp.Disconnect(true);

                QuoteEmailsDetails quoteEmailsDetails = new QuoteEmailsDetails()
                {
                    QuoteNumber = quoteDetails.QuoteNumber,
                    RevisionNumber= quoteDetails.RevisionNumber,
                    RFQNumber= quoteDetails.RFQNumber,
                    SentTo= SentTo,
                    CustomerEmailId= CusEmail,
                    CustomerId=quoteDetails.CustomerId,
                    CustomerName= quoteDetails.CustomerName,
                    QuoteId=quoteDetails.Id
                };
               await _quoteEmailsDetailsRepository.CreateQuoteEmailsDetails(quoteEmailsDetails);
                _quoteEmailsDetailsRepository.SaveAsync();

                serviceResponse.Data = "Email sent successfully.";
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside SendEmailforQuote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
    }
}

