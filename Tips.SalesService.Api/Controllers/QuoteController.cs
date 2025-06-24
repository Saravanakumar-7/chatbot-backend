using System.Net;
using System.Net.Http;
using AutoMapper;
using Azure;
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
using NPOI.HPSF;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using NuGet.Configuration;
using NuGet.Packaging;
using Tips.SalesService.Api.Contracts;
using Tips.SalesService.Api.Entities;
using Tips.SalesService.Api.Entities.Dto;
using Tips.SalesService.Api.Entities.DTOs;
using Tips.SalesService.Api.Entities.Enum;
using Tips.SalesService.Api.Repository;

using SalesEmailIDsDto = Tips.SalesService.Api.Entities.DTOs.SalesEmailIDsDto;
using EmailTemplateDto = Tips.SalesService.Api.Entities.DTOs.EmailTemplateDto;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text;
using System.Net.Http.Headers;
using System.Collections;
using Microsoft.AspNetCore.StaticFiles;

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
       // private IGoogleGCPstorageService _googlegcpservice;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public QuoteController(IQuoteRepository repository, IQuoteEmailsDetailsRepository quoteEmailsDetailsRepository, IHttpClientFactory clientFactory, HttpClient httpClient, IConfiguration config, IRfqCustomerSupportItemRepository rfqCustomerSupportItemRepository, IRfqRepository rfqRepository, ILoggerManager logger, IMapper mapper)
        {//, IGoogleGCPstorageService googlegcpservice
            //_googlegcpservice = googlegcpservice;
            _quoteEmailsDetailsRepository = quoteEmailsDetailsRepository;
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
                _logger.LogError($"Error Occured in GetAllQuote API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllQuote API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllQuoteforKeus([FromQuery] PagingParameter pagingParameter, [FromQuery] string? SearchTerm, [FromQuery] int Offset, [FromQuery] int Limit)
        {
            ServiceResponse<List<QuoteforKeusDto>> serviceResponse = new ServiceResponse<List<QuoteforKeusDto>>();
            try
            {
                var result = await _repository.GetAllQuoteforKeus(SearchTerm, Offset, Limit);
                var TotalCount = await _repository.GetAllQuoteCountforKeus(SearchTerm);
                var metadata = new
                {
                    TotalCount,
                    pagingParameter.PageSize,
                    CurrentPage = pagingParameter.PageNumber
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                _logger.LogInfo("Returned all Quote for keus");
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Quotes Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllQuoteforKeus API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllQuoteforKeus API : \n {ex.Message}";
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
                        var client2 = _clientFactory.CreateClient();
                        var token2 = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = quoteItems.ItemNumber;
                        var encodedItemNo = Uri.EscapeDataString(ItemNumber);

                        var request2 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterMainAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNo}"));
                        request2.Headers.Add("Authorization", token2);

                        var itemMasterObjectResult = await client2.SendAsync(request2);
                        if (itemMasterObjectResult.StatusCode == HttpStatusCode.OK)
                        {

                            _logger.LogInfo("getitemmasterdata" + Convert.ToString(itemMasterObjectResult));
                            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                            var itemMatserObjectData = JsonConvert.DeserializeObject<QuoteItemMasterDetails>(itemMasterObjectString);
                            var itemMasterTranctionObject = itemMatserObjectData.data;

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
                                quoteItems.PartType = itemMasterTranctionObject.itemType;
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
                                quoteItems.PartType = itemMasterTranctionObject.itemType;
                            }
                        }
                        else
                        {
                            _logger.LogError($"Something went wrong inside Create GetQuoteById action: ItemMaster Item: {ItemNumber} is not avaivable");
                            serviceResponse.Data = null;
                            serviceResponse.Message = $"ItemMaster Details is null for Item: {ItemNumber}";
                            serviceResponse.Success = false;
                            serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                            return StatusCode(500, serviceResponse);

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
                _logger.LogError($"Error Occured in GetQuoteById API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetQuoteById API for the following id:{id} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetCsItemDetailsForQuote API for the following rfqNumber:{rfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetCsItemDetailsForQuote API for the following rfqNumber:{rfqNumber} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in GetRfqEnggDetailsByRfqNo API for the following rfqNumber:{rfqNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetRfqEnggDetailsByRfqNo API for the following rfqNumber:{rfqNumber} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in CreateQuote API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in CreateQuote API : \n {ex.Message}";
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
                _logger.LogError($"Error Occured in ChangeRevisionNumber API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in ChangeRevisionNumber API : \n {ex.Message}";
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
                _logger.LogError($"Error Occured in DeleteQuote API for the following id:{id} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in DeleteQuote API for the following id:{id} \n {ex.Message}";
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
                _logger.LogError($"Error Occured in QuoteShortClosed API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in QuoteShortClosed API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetQuoteSPReport(QuoteSpReportDto quoteSpReportDto) 

        {
            ServiceResponse<IEnumerable<QuoteSPReport>> serviceResponse = new ServiceResponse<IEnumerable<QuoteSPReport>>();
            try
            {
                var products = await _repository.GetQuoteSPReport(quoteSpReportDto.CustomerName, quoteSpReportDto.CustomerId, quoteSpReportDto.RfqNumber);

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
                _logger.LogError($"Error Occured in GetQuoteSPReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetQuoteSPReport API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> DownloadFile(string Filename)
        {
            ServiceResponse<FileContentResult> serviceResponse = new ServiceResponse<FileContentResult>();
            var filename = Uri.UnescapeDataString(Filename);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "Temp_Email", filename);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var ContentType))
            {
                ContentType = "application/octet-stream";
            }
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var DownloadFilename = filename.Split('_');
            var downloadFilename = string.IsNullOrWhiteSpace(DownloadFilename[1]) ? Path.GetFileName(filePath) : DownloadFilename[1];

            return File(bytes, ContentType, downloadFilename);
        }
        [HttpPost]
        public async Task<IActionResult> SendEmailandWhatsAppMessageforQuote([FromBody] QuoteEmailPostDto quoteEmailPostDto)
        {
            ServiceResponse<QuoteEmailMessageSuccessMessage> serviceResponse = new ServiceResponse<QuoteEmailMessageSuccessMessage>();
            try
            {
                if (quoteEmailPostDto.WhatsAppPhoneNos.IsNullOrEmpty())
                {
                    _logger.LogError($"The WhatsApp Numbers is Empty these are required. For the QuoteId:{quoteEmailPostDto.Quoteid}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"The WhatsApp Numbers is Empty these are required. For the QuoteId:{quoteEmailPostDto.Quoteid}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(400, serviceResponse);
                }
                var whatsappNumbers = quoteEmailPostDto.WhatsAppPhoneNos.Split(',').ToList();

                var quoteDetails = await _repository.GetQuoteById(quoteEmailPostDto.Quoteid);
                string? emaildetails;
                string? whatsapptemplate;
                string? FileName;
                var client = _clientFactory.CreateClient();
                var token = HttpContext.Request.Headers["Authorization"].ToString();
                if (quoteDetails.TypeOfSolution == "Automation" || quoteDetails.TypeOfSolution == "Upsell - Automation")
                {
                    emaildetails = "Your Keus Automation Quotation";
                    FileName = "Quote_Automation_Book";
                    whatsapptemplate = "new_revised_automation_quote";
                   // whatsapptemplate = "advait_quote_automaiton";
                }
                else if (quoteDetails.TypeOfSolution == "Accessories" || quoteDetails.TypeOfSolution == "Lock")
                {
                    emaildetails = "Your Keus Accessories Quotation";
                    FileName = "Quote_Accessories_Book";
                    whatsapptemplate = "new_revised_automation_quote";
                    //whatsapptemplate = "quotation_sent";
                }
                else
                {
                    emaildetails = "Your Keus Lights Quotation";
                    FileName = "Quote_Lights_Book";
                    whatsapptemplate = "new_revised_lights_quote";
                    //whatsapptemplate = "advait_quote_light";
                }
                if (emaildetails.IsNullOrEmpty())
                {
                    _logger.LogError($"The Subject of the Email is Empty as the Type of Solution has not matched any Type Of Solution. For the Quote: {quoteDetails.QuoteNumber} and RevisionNo: {quoteDetails.RevisionNumber}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"The Subject of the Email is Empty as the Type of Solution has not matched any Type Of Solution. For the Quote: {quoteDetails.QuoteNumber} and RevisionNo: {quoteDetails.RevisionNumber}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                if (whatsapptemplate.IsNullOrEmpty())
                {
                    _logger.LogError($"The Template for Whatsapp is Empty as the Type of Solution has not matched any Type Of Solution. For the Quote: {quoteDetails.QuoteNumber} and RevisionNo: {quoteDetails.RevisionNumber}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"The Template for Whatsapp is Empty as the Type of Solution has not matched any Type Of Solution. For the Quote: {quoteDetails.QuoteNumber} and RevisionNo: {quoteDetails.RevisionNumber}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                var Operations = "From";
                var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["EmailIDsAPI"], $"GetEmailIdDetailsbyOperation?Operations={Operations}"));
                request1.Headers.Add("Authorization", token);
                var response1 = await client.SendAsync(request1);
                var EmailTempString1 = await response1.Content.ReadAsStringAsync();
                var emaildetails1 = JsonConvert.DeserializeObject<SalesEmailIDsDto>(EmailTempString1);


                var httpclientHandler = new HttpClientHandler();
                var httpClient = new HttpClient(httpclientHandler);
                var mails = quoteEmailPostDto.SentTo.Split(',').ToList();
                if (!quoteEmailPostDto.CusEmail.IsNullOrEmpty())
                {
                    mails.Add(quoteEmailPostDto.CusEmail);
                }
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()));
                email.To.AddRange(mails.Select(x => MailboxAddress.Parse(x)));
                email.Subject = emaildetails;
                string? body;
                if (quoteDetails.TypeOfSolution == "Automation" || quoteDetails.TypeOfSolution == "Upsell - Automation" || quoteDetails.TypeOfSolution == "Accessories" || quoteDetails.TypeOfSolution == "Lock")
                {
                    string htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "keus-automation-quotation.html");

                    body = System.IO.File.ReadAllText(htmlFilePath);
                    body = body.Replace("{{Quote Number}}", quoteDetails.QuoteNumber);
                    body = body.Replace("{{Customer Name}}", quoteDetails.CustomerName);
                }
                else
                {
                    string htmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "keus-light-quotation.html");

                    body = System.IO.File.ReadAllText(htmlFilePath);
                    body = body.Replace("{{Customer Name}}", quoteDetails.CustomerName);
                }
                string base64;
                var builder = new BodyBuilder();
                builder.HtmlBody = body;
                using (HttpClient client1 = new HttpClient())
                {
                    client1.Timeout = TimeSpan.FromMinutes(5);
                    var request2 = new HttpRequestMessage(HttpMethod.Get, quoteEmailPostDto.jasperfileUrl);

                    request2.Headers.Add("Authorization", "Basic amFzcGVyYWRtaW46Uk11aExncXdkOXBJUGI0");
                    request2.Headers.Add("X-Remote-Domain", "1");
                    var response2 = await client1.SendAsync(request2);
                    response2.EnsureSuccessStatusCode();

                    byte[] fileBytes = await response2.Content.ReadAsByteArrayAsync();
                    //builder.Attachments.Add(FileName, fileBytes, ContentType.Parse("application/pdf"));
                    base64 = Convert.ToBase64String(fileBytes);
                }
                //Guid guids = Guid.NewGuid();
                //byte[] fileContent = Convert.FromBase64String(base64);
                //string fileName = /*guids.ToString() + "_" +*/ FileName + ".pdf";
                //string FileExt = Path.GetExtension(fileName).ToUpper();
                //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "Temp_Email", fileName);
                //using (MemoryStream ms = new MemoryStream(fileContent))
                //{
                //    ms.Position = 0;
                //    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                //    {
                //        ms.WriteTo(fileStream);
                //    }
                //}
                email.Body = builder.ToMessageBody();
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                smtp.Send(email);
                smtp.Disconnect(true);

                var jsonpayload = "{\r\n  \"recipient_type\": \"individual\",\r\n  \"to\": \"\",\r\n  \"type\": \"template\",\r\n  \"template\": {\r\n    \"name\": \"\",\r\n    \"language\": {\r\n      \"policy\": \"deterministic\",\r\n      \"code\": \"en\"\r\n    },\r\n    \"components\": [\r\n      {\r\n        \"type\": \"header\",\r\n        \"parameters\": [\r\n          {\r\n            \"type\": \"document\",\r\n            \"document\": {\r\n              \"filename\": \"\"\r\n            }\r\n          }\r\n        ]\r\n      }\r\n    ]\r\n  },\r\n  \"metadata\": {\r\n    \"messageId\": \"\",\r\n    \"media\": {\r\n      \"mimeType\": \"application/pdf\"}\r\n  }\r\n}";
                WhatsAppMessagePayload whatsAppMessagePayload = JsonConvert.DeserializeObject<WhatsAppMessagePayload>(jsonpayload);
                whatsAppMessagePayload.Template.Name = whatsapptemplate;
                whatsAppMessagePayload.Template.Components[0].Parameters[0].Document.Filename = FileName;
                //string PDFLink = quoteEmailPostDto.jasperfileUrl;
                //PDFLink = PDFLink.Replace("https://", "https://jasperadmin:RMuhLgqwd9pIPb4@");

                //await _googlegcpservice.UploadFileAsync(filePath, fileName, "");
                //var PDFLink = _googlegcpservice.GenerateSignedUrl(fileName, TimeSpan.FromDays(7), "");
          
                //var generatedURL = $"{_config["SalesOrderUrl"]}/api/Quote/DownloadFile?Filename={fileName}";

                //whatsAppMessagePayload.Template.Components[0].Parameters[0].Document.Link = generatedURL;

                Component component = new Component();
                List<Tips.SalesService.Api.Entities.DTOs.Parameter> parameters = new List<Tips.SalesService.Api.Entities.DTOs.Parameter>();

                Entities.DTOs.Parameter parameter1 = new Entities.DTOs.Parameter();
                Entities.DTOs.Parameter parameter2 = new Entities.DTOs.Parameter();
                Entities.DTOs.Parameter parameter3 = new Entities.DTOs.Parameter();
                parameter1.Type = "text";
                parameter2.Type = "text";
                parameter3.Type = "text";
                parameter1.Text = quoteDetails.LeadId; parameter2.Text = quoteDetails.QuoteNumber; parameter3.Text = "Client";

                parameters.Add(parameter1); parameters.Add(parameter2); parameters.Add(parameter3);

                component.Type = "body";
                component.Parameters = parameters;
                whatsAppMessagePayload.Template.Components.Add(component);


                //whatsAppMessagePayload.Metadata.Media.Content = "___BASE64___";
                WhatsAppCreateTokenResponse whatsAppCreateTokenResponse;

                
                var request = new HttpRequestMessage(HttpMethod.Post, "https://auth.aclwhatsapp.com/realms/ipmessaging/protocol/openid-connect/token");
                request.Headers.Add("cache-control", "no-cache");
                var content = new StringContent($"grant_type=password&client_id=ipmessaging-client&username=keuspd&password=keuspd30", Encoding.UTF8, "application/x-www-form-urlencoded");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Content = content;
                var response = await client.SendAsync(request);

                whatsAppCreateTokenResponse = JsonConvert.DeserializeObject<WhatsAppCreateTokenResponse>(await response.Content.ReadAsStringAsync());
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Unable to Generate Token for Whatsapp Message. For the Quote: {quoteDetails.QuoteNumber} and RevisionNo: {quoteDetails.RevisionNumber}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Unable to Generate Token for Whatsapp Message. For the Quote: {quoteDetails.QuoteNumber} and RevisionNo: {quoteDetails.RevisionNumber}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }

                MsisdnListRequest msisdnListRequest = new MsisdnListRequest
                {
                    MsisdnList = whatsappNumbers
                };
                var No_s = JsonConvert.SerializeObject(msisdnListRequest);
                var data = new StringContent(No_s, Encoding.UTF8, "application/json");
                var request3 = new HttpRequestMessage(HttpMethod.Post, "https://optin.aclwhatsapp.com/api/v1/optin/bulk")
                {
                    Content = data
                };
                request3.Headers.Add("Authorization", "Bearer "+whatsAppCreateTokenResponse.AccessToken);
                var response3 = await client.SendAsync(request3);
                if (!response3.IsSuccessStatusCode)
                {
                    _logger.LogError($"Unable to OptinNumbers for Whatsapp Message. For the Quote: {quoteDetails.QuoteNumber} and RevisionNo: {quoteDetails.RevisionNumber}");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Unable to OptinNumbers for Whatsapp Message. For the Quote: {quoteDetails.QuoteNumber} and RevisionNo: {quoteDetails.RevisionNumber}";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }

                //foreach (var number in whatsappNumbers)
                //{
                //    whatsAppMessagePayload.To = number;
                //    var whatsappCreate = JsonConvert.SerializeObject(whatsAppMessagePayload);
                //    //whatsappCreate=whatsappCreate.Replace("___BASE64___", base64);
                //    var data4 = new StringContent(whatsappCreate, Encoding.UTF8, "application/json");
                //    var request4 = new HttpRequestMessage(HttpMethod.Post, "https://api.aclwhatsapp.com/pull-platform-receiver/v2/wa/messages")
                //    {
                //        Content = data4
                //    };
                //    request4.Headers.Add("Authorization", "Bearer " + whatsAppCreateTokenResponse.AccessToken);
                //    var response4 = await client.SendAsync(request4);
                //    if (!response4.IsSuccessStatusCode)
                //    {
                //        _logger.LogError($"Unable to Create a Whatsapp Message");
                //        serviceResponse.Data = null;
                //        serviceResponse.Message = $"Something went wrong ,try again";
                //        serviceResponse.Success = false;
                //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                //        return StatusCode(500, serviceResponse);
                //    }
                //}


                QuoteEmailsDetails quoteEmailsDetails = new QuoteEmailsDetails()
                {
                    QuoteNumber = quoteDetails.QuoteNumber,
                    RevisionNumber = quoteDetails.RevisionNumber,
                    RFQNumber = quoteDetails.RFQNumber,
                    SentTo = quoteEmailPostDto.SentTo,
                    CustomerEmailId = quoteEmailPostDto.CusEmail,
                    CustomerId = quoteDetails.CustomerId,
                    CustomerName = quoteDetails.CustomerName,
                    QuoteValue = quoteDetails.TotalFinalAmount,
                    QuoteId = quoteDetails.Id,
                    TypeOfSolution = quoteDetails.TypeOfSolution,
                    WhatsAppPhoneNos = quoteEmailPostDto.WhatsAppPhoneNos
                };
                await _quoteEmailsDetailsRepository.CreateQuoteEmailsDetails(quoteEmailsDetails);
                _quoteEmailsDetailsRepository.SaveAsync();

                QuoteEmailMessageSuccessMessage emailMessageSuccessMessage = new QuoteEmailMessageSuccessMessage()
                {
                    QuoteNumber = quoteDetails.QuoteNumber,
                    RevisionNumber = quoteDetails.RevisionNumber ?? 0
                };
                serviceResponse.Data = emailMessageSuccessMessage;
                serviceResponse.Message = $"Email sent successfully.";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in SendEmailandWhatsAppMessageforQuote API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in SendEmailandWhatsAppMessageforQuote API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetQuotationSPReportWithParam([FromBody] QuoteSPResportParamDTO quoteSPReportDto)

        {
            ServiceResponse<IEnumerable<QuotationSPReport>> serviceResponse = new ServiceResponse<IEnumerable<QuotationSPReport>>();
            try
            {
                var products = await _repository.GetQuotationSPReportWithParam(quoteSPReportDto.CustomerId, quoteSPReportDto.QuoteNumber, quoteSPReportDto.QuotationVersionNo);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Quotation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Quotation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Quotation Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetQuotationSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetQuotationSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetQuotationSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<QuotationSPReport>> serviceResponse = new ServiceResponse<IEnumerable<QuotationSPReport>>();
            try
            {
                var products = await _repository.GetQuotationSPReportWithDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Quotation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Quotation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Quotation Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetQuotationSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetQuotationSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }
        [HttpGet]
        public async Task<IActionResult> ExportQuotationWithDateSPReportToExcel([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            try
            {
                // Get data from repository using stored procedure
                var quotationSPReportDetails = await _repository.GetQuotationSPReportWithDate(FromDate, ToDate);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("QuotationSPReport");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Quote Number");
                headerRow.CreateCell(1).SetCellValue("Quotation Version No");
                headerRow.CreateCell(2).SetCellValue("RFQ Number");
                headerRow.CreateCell(3).SetCellValue("Customer ID");
                headerRow.CreateCell(4).SetCellValue("Customer Name");
                headerRow.CreateCell(5).SetCellValue("Lead ID");
                headerRow.CreateCell(6).SetCellValue("City");
                headerRow.CreateCell(7).SetCellValue("Address");
                headerRow.CreateCell(8).SetCellValue("Type Of Solution");
                headerRow.CreateCell(9).SetCellValue("Product Type");
                headerRow.CreateCell(10).SetCellValue("Material Group");
                headerRow.CreateCell(11).SetCellValue("Quote Created On");
                headerRow.CreateCell(12).SetCellValue("Quote Sent On");
                headerRow.CreateCell(13).SetCellValue("Room Name");
                headerRow.CreateCell(14).SetCellValue("KPN");
                headerRow.CreateCell(15).SetCellValue("KPN Description");
                headerRow.CreateCell(16).SetCellValue("UOC");
                headerRow.CreateCell(17).SetCellValue("UOM");
                headerRow.CreateCell(18).SetCellValue("Price List");
                headerRow.CreateCell(19).SetCellValue("Unit Price");
                headerRow.CreateCell(20).SetCellValue("Basic Amount");
                headerRow.CreateCell(21).SetCellValue("Discount Type");
                headerRow.CreateCell(22).SetCellValue("Total Final Amount");
                headerRow.CreateCell(23).SetCellValue("TotalAdditionalCharges");
                headerRow.CreateCell(24).SetCellValue("OrderQty");
                headerRow.CreateCell(25).SetCellValue("InstallationCharges");


                // Populate data rows
                int rowIndex = 1;
                foreach (var item in quotationSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.QuoteNumber ?? "");
                    row.CreateCell(1).SetCellValue(item.QuotationVersionNo.HasValue ? Convert.ToDouble(item.QuotationVersionNo.Value) : 0);
                    row.CreateCell(2).SetCellValue(item.RfqNumber ?? "");
                    row.CreateCell(3).SetCellValue(item.CustomerId ?? "");
                    row.CreateCell(4).SetCellValue(item.CustomerName ?? "");
                    row.CreateCell(5).SetCellValue(item.LeadId ?? "");
                    row.CreateCell(6).SetCellValue(item.City ?? "");
                    row.CreateCell(7).SetCellValue(item.Address ?? "");
                    row.CreateCell(8).SetCellValue(item.TypeOfSolution ?? "");
                    row.CreateCell(9).SetCellValue(item.ProductType ?? "");
                    row.CreateCell(10).SetCellValue(item.MaterialGroup ?? "");
                    row.CreateCell(11).SetCellValue(item.QuoteCreatedOn.HasValue ? item.QuoteCreatedOn.Value.ToString("MM-dd-yyyy HH:mm:ss") : "");
                    row.CreateCell(12).SetCellValue(item.QuoteSentOn.HasValue ? item.QuoteSentOn.Value.ToString("MM-dd-yyyy HH:mm:ss") : "");
                    row.CreateCell(13).SetCellValue(item.RoomName ?? "");
                    row.CreateCell(14).SetCellValue(item.KPN ?? "");
                    row.CreateCell(15).SetCellValue(item.KPNDescription ?? "");
                    row.CreateCell(16).SetCellValue(item.UOC ?? "");
                    row.CreateCell(17).SetCellValue(item.Uom ?? "");
                    row.CreateCell(18).SetCellValue(item.PriceList ?? "");
                    row.CreateCell(19).SetCellValue(item.UnitPrice.HasValue ? Convert.ToDouble(item.UnitPrice.Value) : 0);
                    row.CreateCell(20).SetCellValue(item.BasicAmount.HasValue ? Convert.ToDouble(item.BasicAmount.Value) : 0);
                    row.CreateCell(21).SetCellValue(item.DiscountType ?? "");
                    row.CreateCell(22).SetCellValue(item.TotalFinalAmount.HasValue ? Convert.ToDouble(item.TotalFinalAmount.Value) : 0);
                    row.CreateCell(23).SetCellValue(item.TotalAdditionalCharges.HasValue ? Convert.ToDouble(item.TotalAdditionalCharges.Value) : 0);
                    row.CreateCell(24).SetCellValue(item.OrderQty.HasValue ? Convert.ToDouble(item.OrderQty.Value) : 0);
                    row.CreateCell(25).SetCellValue(item.InstallationCharges.HasValue ? Convert.ToDouble(item.InstallationCharges.Value) : 0);

                }


                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "QuotationSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred in ExportQuotationWithDateSPReportToExcel API: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ExportQuotationRFQSPReportToExcel([FromBody] QuoteSPResportParamDTO quoteSPReportDto)
        {
            try
            {
                // Get data from repository using stored procedure
                var quotationSPReportDetails = await _repository.GetQuotationSPReportWithParam(quoteSPReportDto.CustomerId, quoteSPReportDto.QuoteNumber, quoteSPReportDto.QuotationVersionNo);

                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("QuotationSPReport");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Quote Number");
                headerRow.CreateCell(1).SetCellValue("Quotation Version No");
                headerRow.CreateCell(2).SetCellValue("RFQ Number");
                headerRow.CreateCell(3).SetCellValue("Customer ID");
                headerRow.CreateCell(4).SetCellValue("Customer Name");
                headerRow.CreateCell(5).SetCellValue("Lead ID");
                headerRow.CreateCell(6).SetCellValue("City");
                headerRow.CreateCell(7).SetCellValue("Address");
                headerRow.CreateCell(8).SetCellValue("Type Of Solution");
                headerRow.CreateCell(9).SetCellValue("Product Type");
                headerRow.CreateCell(10).SetCellValue("Material Group");
                headerRow.CreateCell(11).SetCellValue("Quote Created On");
                headerRow.CreateCell(12).SetCellValue("Quote Sent On");
                headerRow.CreateCell(13).SetCellValue("Room Name");
                headerRow.CreateCell(14).SetCellValue("KPN");
                headerRow.CreateCell(15).SetCellValue("KPN Description");
                headerRow.CreateCell(16).SetCellValue("UOC");
                headerRow.CreateCell(17).SetCellValue("UOM");
                headerRow.CreateCell(18).SetCellValue("Price List");
                headerRow.CreateCell(19).SetCellValue("Unit Price");
                headerRow.CreateCell(20).SetCellValue("Basic Amount");
                headerRow.CreateCell(21).SetCellValue("Discount Type");
                headerRow.CreateCell(22).SetCellValue("Total Final Amount");
                headerRow.CreateCell(23).SetCellValue("TotalAdditionalCharges");
                headerRow.CreateCell(24).SetCellValue("OrderQty");
                headerRow.CreateCell(25).SetCellValue("InstallationCharges");


                // Populate data rows
                int rowIndex = 1;
                foreach (var item in quotationSPReportDetails)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.QuoteNumber ?? "");
                    row.CreateCell(1).SetCellValue(item.QuotationVersionNo.HasValue ? Convert.ToDouble(item.QuotationVersionNo.Value) : 0);
                    row.CreateCell(2).SetCellValue(item.RfqNumber ?? "");
                    row.CreateCell(3).SetCellValue(item.CustomerId ?? "");
                    row.CreateCell(4).SetCellValue(item.CustomerName ?? "");
                    row.CreateCell(5).SetCellValue(item.LeadId ?? "");
                    row.CreateCell(6).SetCellValue(item.City ?? "");
                    row.CreateCell(7).SetCellValue(item.Address ?? "");
                    row.CreateCell(8).SetCellValue(item.TypeOfSolution ?? "");
                    row.CreateCell(9).SetCellValue(item.ProductType ?? "");
                    row.CreateCell(10).SetCellValue(item.MaterialGroup ?? "");
                    row.CreateCell(11).SetCellValue(item.QuoteCreatedOn.HasValue ? item.QuoteCreatedOn.Value.ToString("MM-dd-yyyy HH:mm:ss") : "");
                    row.CreateCell(12).SetCellValue(item.QuoteSentOn.HasValue ? item.QuoteSentOn.Value.ToString("MM-dd-yyyy HH:mm:ss") : "");
                    row.CreateCell(13).SetCellValue(item.RoomName ?? "");
                    row.CreateCell(14).SetCellValue(item.KPN ?? "");
                    row.CreateCell(15).SetCellValue(item.KPNDescription ?? "");
                    row.CreateCell(16).SetCellValue(item.UOC ?? "");
                    row.CreateCell(17).SetCellValue(item.Uom ?? "");
                    row.CreateCell(18).SetCellValue(item.PriceList ?? "");
                    row.CreateCell(19).SetCellValue(item.UnitPrice.HasValue ? Convert.ToDouble(item.UnitPrice.Value) : 0);
                    row.CreateCell(20).SetCellValue(item.BasicAmount.HasValue ? Convert.ToDouble(item.BasicAmount.Value) : 0);
                    row.CreateCell(21).SetCellValue(item.DiscountType ?? "");
                    row.CreateCell(22).SetCellValue(item.TotalFinalAmount.HasValue ? Convert.ToDouble(item.TotalFinalAmount.Value) : 0);
                    row.CreateCell(23).SetCellValue(item.TotalAdditionalCharges.HasValue ? Convert.ToDouble(item.TotalAdditionalCharges.Value) : 0);
                    row.CreateCell(24).SetCellValue(item.OrderQty.HasValue ? Convert.ToDouble(item.OrderQty.Value) : 0);
                    row.CreateCell(25).SetCellValue(item.InstallationCharges.HasValue ? Convert.ToDouble(item.InstallationCharges.Value) : 0);

                }


                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "QuotationSPReport.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred in ExportQuotationRFQSPReportToExcel in API: {ex.Message}");
            }
        }
        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetSoSummaryQuotationSPReport()

        {
            ServiceResponse<IEnumerable<SoSummaryQuotationDto>> serviceResponse = new ServiceResponse<IEnumerable<SoSummaryQuotationDto>>();
            try
            {
                var products = await _repository.GetSoSummaryQuotationSPReport();

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SoSummaryQuotationSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SoSummaryQuotationSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GetSoSummaryQuotationSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetSoSummaryQuotationSPReport API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetSoSummaryQuotationSPReport API : \n {ex.Message} ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetSoSummaryQuotationSPReportWithParam([FromBody] SoSummaryQuotePostDto quoteSPReportDto)

        {
            ServiceResponse<IEnumerable<SoSummaryQuotationDto>> serviceResponse = new ServiceResponse<IEnumerable<SoSummaryQuotationDto>>();
            try
            {
                var products = await _repository.GetSoSummaryQuotationSPReportWithParam(quoteSPReportDto.FirstQuotenumber, quoteSPReportDto.SOLatestSalesOrderSentNumber,
                                                                                                                    quoteSPReportDto.Leadid,quoteSPReportDto.CustomerName
                                                                                                                    ,quoteSPReportDto.TypeOfSolution,quoteSPReportDto.ProductType);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"SoSummaryQuotationSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"SoSummaryQuotationSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned GetSoSummaryQuotationSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetSoSummaryQuotationSPReportWithParam API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetSoSummaryQuotationSPReportWithParam API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetSoSummaryQuotationSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<SoSummaryQuotationDto>> serviceResponse = new ServiceResponse<IEnumerable<SoSummaryQuotationDto>>();
            try
            {
                var products = await _repository.GetSoSummaryQuotationSPReportWithDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Quotation hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Quotation hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Quotation Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetSoSummaryQuotationSPReportWithDate API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetSoSummaryQuotationSPReportWithDate API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        [HttpPost]
        public async Task<IActionResult> ExportSoSummaryQuotationSPReportToExcel([FromBody] SoSummaryQuotePostDto quoteSPReportDto)
        {
            try
            {
                var soSummaryQuotationDtos = await _repository.GetSoSummaryQuotationSPReportWithParam(quoteSPReportDto.FirstQuotenumber, quoteSPReportDto.SOLatestSalesOrderSentNumber,
                                                                                                                    quoteSPReportDto.Leadid, quoteSPReportDto.CustomerName
                                                                                                                    , quoteSPReportDto.TypeOfSolution, quoteSPReportDto.ProductType);
                // Create a new Excel workbook
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("SoSummaryQuotation");

                // Set header row
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Lead ID");
                headerRow.CreateCell(1).SetCellValue("Customer Name");
                headerRow.CreateCell(2).SetCellValue("City");
                headerRow.CreateCell(3).SetCellValue("Address");
                headerRow.CreateCell(4).SetCellValue("Type Of Solution");
                headerRow.CreateCell(5).SetCellValue("Product Type");
                headerRow.CreateCell(6).SetCellValue("First Quote Number");
                headerRow.CreateCell(7).SetCellValue("First Quote Revision Number");
                headerRow.CreateCell(8).SetCellValue("First Quote Created On");
                headerRow.CreateCell(9).SetCellValue("First Quote Total Final Amount");
                headerRow.CreateCell(10).SetCellValue("First Quote Sent Number");
                headerRow.CreateCell(11).SetCellValue("First Quote Sent Revision Number");
                headerRow.CreateCell(12).SetCellValue("First Quote Sent Created On");
                headerRow.CreateCell(13).SetCellValue("First Quote Email Sent On");
                headerRow.CreateCell(14).SetCellValue("First Quote Sent General Discount");
                headerRow.CreateCell(15).SetCellValue("First Quote Sent General Discount Type");
                headerRow.CreateCell(16).SetCellValue("First Quote Sent Taxed Value");
                headerRow.CreateCell(17).SetCellValue("First Quote Sent Untaxed Value");
                headerRow.CreateCell(18).SetCellValue("Latest Quote Sent Number");
                headerRow.CreateCell(19).SetCellValue("Latest Quote Sent Revision Number");
                headerRow.CreateCell(20).SetCellValue("Latest Quote Sent Created On");
                headerRow.CreateCell(21).SetCellValue("Latest Quote Sent General Discount");
                headerRow.CreateCell(22).SetCellValue("Latest Quote Sent General Discount Type");
                headerRow.CreateCell(23).SetCellValue("Latest Quote Sent Taxed Value");
                headerRow.CreateCell(24).SetCellValue("Latest Quote Sent Untaxed Value");
                headerRow.CreateCell(25).SetCellValue("First Sales Order Sent Number");
                headerRow.CreateCell(26).SetCellValue("First SO Sent Revision Number");
                headerRow.CreateCell(27).SetCellValue("First SO Sent Created On");
                headerRow.CreateCell(28).SetCellValue("First SO Sent Discount");
                headerRow.CreateCell(29).SetCellValue("First SO Sent Discount Type");
                headerRow.CreateCell(30).SetCellValue("First SO Sent Taxed Value");
                headerRow.CreateCell(31).SetCellValue("First SO Sent Untaxed Value");
                headerRow.CreateCell(32).SetCellValue("Latest Sales Order Sent Number");
                headerRow.CreateCell(33).SetCellValue("Latest SO Sent Revision Number");
                headerRow.CreateCell(34).SetCellValue("Latest SO Sent Created On");
                headerRow.CreateCell(35).SetCellValue("Latest SO Sent Discount");
                headerRow.CreateCell(36).SetCellValue("Latest SO Sent Discount Type");
                headerRow.CreateCell(37).SetCellValue("Latest SO Sent Taxed Value");
                headerRow.CreateCell(38).SetCellValue("Latest SO Sent Untaxed Value");


                // Populate data rows
                int rowIndex = 1;
                foreach (var item in soSummaryQuotationDtos)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.LeadId ?? "");
                    row.CreateCell(1).SetCellValue(item.CustomerName ?? "");
                    row.CreateCell(2).SetCellValue(item.City ?? "");
                    row.CreateCell(3).SetCellValue(item.Address ?? "");
                    row.CreateCell(4).SetCellValue(item.TypeOfSolution ?? "");
                    row.CreateCell(5).SetCellValue(item.ProductType ?? "");
                    row.CreateCell(6).SetCellValue(item.FirstQuoteNumber ?? "");
                    row.CreateCell(7).SetCellValue((double?)item.FirstQuoteRevisionNumber ?? 0);
                    row.CreateCell(8).SetCellValue(item.FirstQuoteCreatedOn?.ToString("MM-dd-yyyy HH:mm:ss") ?? "");
                    row.CreateCell(9).SetCellValue((double?)item.FirstQuoteTotalFinalAmount ?? 0);
                    row.CreateCell(10).SetCellValue(item.FirstQuoteSentNumber ?? "");
                    row.CreateCell(11).SetCellValue((double?)item.FirstQuoteSentRevisionNumber ?? 0);
                    row.CreateCell(12).SetCellValue(item.FirstQuoteSentCreatedOn?.ToString("MM-dd-yyyy HH:mm:ss") ?? "");
                    row.CreateCell(13).SetCellValue(item.FirstQuoteEmailSentOn?.ToString("MM-dd-yyyy HH:mm:ss") ?? "");
                    row.CreateCell(14).SetCellValue((double?)item.FirstQuoteSentGeneralDiscount ?? 0);
                    row.CreateCell(15).SetCellValue(item.FirstQuoteSentGeneralDiscountType ?? "");
                    row.CreateCell(16).SetCellValue((double?)item.FirstQuoteSentTaxedValue ?? 0);
                    row.CreateCell(17).SetCellValue((double?)item.FirstQuoteSentUntaxedValue ?? 0);
                    row.CreateCell(18).SetCellValue(item.LatestQuoteSentNumber ?? "");
                    row.CreateCell(19).SetCellValue((double?)item.LatestQuoteSentRevisionNumber ?? 0);
                    row.CreateCell(20).SetCellValue(item.LatestQuoteSentCreatedOn?.ToString("MM-dd-yyyy HH:mm:ss") ?? "");
                    row.CreateCell(21).SetCellValue((double?)item.LatestQuoteSentGeneralDiscount ?? 0);
                    row.CreateCell(22).SetCellValue(item.LatestQuoteSentGeneralDiscountType ?? "");
                    row.CreateCell(23).SetCellValue((double?)item.LatestQuoteSentTaxedValue ?? 0);
                    row.CreateCell(24).SetCellValue((double?)item.LatestQuoteSentUntaxedValue ?? 0);
                    row.CreateCell(25).SetCellValue(item.FirstSalesOrderSentNumber ?? "");
                    row.CreateCell(26).SetCellValue((double?)item.FirstSOSentRevisionNumber ?? 0);
                    row.CreateCell(27).SetCellValue(item.FirstSOSentCreatedOn?.ToString("MM-dd-yyyy HH:mm:ss") ?? "");
                    row.CreateCell(28).SetCellValue((double?)item.FirstSOSentDiscount ?? 0);
                    row.CreateCell(29).SetCellValue(item.FirstSOSentDiscountType ?? "");
                    row.CreateCell(30).SetCellValue((double?)item.FirstSOSentTaxedValue ?? 0);
                    row.CreateCell(31).SetCellValue((double?)item.FirstSOSentUntaxedValue ?? 0);
                    row.CreateCell(32).SetCellValue(item.LatestSalesOrderSentNumber ?? "");
                    row.CreateCell(33).SetCellValue((double?)item.LatestSOSentRevisionNumber ?? 0);
                    row.CreateCell(34).SetCellValue(item.LatestSOSentCreatedOn?.ToString("MM-dd-yyyy HH:mm:ss") ?? "");
                    row.CreateCell(35).SetCellValue((double?)item.LatestSOSentDiscount ?? 0);
                    row.CreateCell(36).SetCellValue(item.LatestSOSentDiscountType ?? "");
                    row.CreateCell(37).SetCellValue((double?)item.LatestSOSentTaxedValue ?? 0);
                    row.CreateCell(38).SetCellValue((double?)item.LatestSOSentUntaxedValue ?? 0);

                }

                // Save Excel workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);
                    var excelBytes = memoryStream.ToArray();

                    // Send Excel file as a response
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SoSummaryQuotation.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"error occurred in ExportSoSummaryQuotationSPReportToExcel API: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllQuoteNumberList()
        {
            ServiceResponse<IEnumerable<QuoteNoDto>> serviceResponse = new ServiceResponse<IEnumerable<QuoteNoDto>>();
            try
            {
                var quoteNoList = await _repository.GetAllQuoteNumberList();
                var result = _mapper.Map<IEnumerable<QuoteNoDto>>(quoteNoList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all QuoteNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllQuoteNumberList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllQuoteNumberList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuoteNoList()
        {
            ServiceResponse<IEnumerable<QuoteNumberDto>> serviceResponse = new ServiceResponse<IEnumerable<QuoteNumberDto>>();
            try
            {
                var quoteNoList = await _repository.GetAllQuoteNoList();
                var result = _mapper.Map<IEnumerable<QuoteNumberDto>>(quoteNoList);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all QuoteNumberList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllQuoteNoList API : \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllQuoteNoList API : \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetClosedQuoteDetailsByQuoteNo(string quoteNumber)
        {
            ServiceResponse<IEnumerable<QuoteNoDto>> serviceResponse = new ServiceResponse<IEnumerable<QuoteNoDto>>();

            try
            {
                var quoteDetails = await _repository.GetClosedQuoteDetailsByQuoteNo(quoteNumber);
                if (quoteDetails.Count() == 0)
                {
                    _logger.LogError($"QuoteDetails with ItemNo: {quoteNumber}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"QuoteDetails with ItemNo: {quoteNumber}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned QuoteDetails with ItemNo: {quoteNumber}");
                    var result = _mapper.Map<List<QuoteNoDto>>(quoteDetails);
                    serviceResponse.Data = result;
                    serviceResponse.Message = "Successfully Returned QuoteDetails";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetClosedQuoteDetailsByQuoteNo API for the following quoteNumber:{quoteNumber} \n {ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetClosedQuoteDetailsByQuoteNo API for the following quoteNumber:{quoteNumber} \n {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetAllQuoteRevisionSPReportWithParam([FromBody] QuoteRevNoSPReportParamDTO quoteRevNoSPResportParamDTO)

        {
            ServiceResponse<IEnumerable<QuoteRevNoSPReportParam>> serviceResponse = new ServiceResponse<IEnumerable<QuoteRevNoSPReportParam>>();
            try
            {
                var products = await _repository.GetAllQuoteRevisionSPReportWithParam(quoteRevNoSPResportParamDTO.LeadId, quoteRevNoSPResportParamDTO.QuoteNumber, quoteRevNoSPResportParamDTO.ItemNumber);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"QuoteRevisionSPReport hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"QuoteRevisionSPReport hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned All QuoteRevisionSPReport Successfully");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned All QuoteRevisionSPReport Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllQuoteRevisionSPReportWithParam API : \n{ex.Message} \n{ex.InnerException}"); ;
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllQuoteRevisionSPReportWithParam API : \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet] // Adjust your route as needed
        public async Task<IActionResult> GetAllQuoteRevisionSPReportWithDate([FromQuery] DateTime? FromDate, [FromQuery] DateTime? ToDate)
        {
            ServiceResponse<IEnumerable<QuoteRevNoSPReportParam>> serviceResponse = new ServiceResponse<IEnumerable<QuoteRevNoSPReportParam>>();
            try
            {
                var products = await _repository.GetAllQuoteRevisionSPReportWithDate(FromDate, ToDate);
                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"QuoteRevisionSPReportWithDate hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"QuoteRevisionSPReportWithDate hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned All QuoteRevisionSPReportWithDate Successfully");
                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned All QuoteRevisionSPReportWithDate Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Occured in GetAllQuoteRevisionSPReportWithDate API : \n{ex.Message} \n{ex.InnerException}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Error Occured in GetAllQuoteRevisionSPReportWithDate API : \n{ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

    }
}

