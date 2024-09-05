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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again";
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
                            _logger.LogError($"Something went wrong inside Create GetQuoteById action: ItemMaster PartType is not avaivable");
                            serviceResponse.Data = null;
                            serviceResponse.Message = "ItemMaster Details is null";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetQuoteSPReport action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailandWhatsAppMessageforQuote([FromBody] QuoteEmailPostDto quoteEmailPostDto)
        {
            ServiceResponse<QuoteEmailMessageSuccessMessage> serviceResponse = new ServiceResponse<QuoteEmailMessageSuccessMessage>();
            try
            {
                if (quoteEmailPostDto.WhatsAppPhoneNos.IsNullOrEmpty())
                {
                    _logger.LogError($"The WhatsApp Numbers is Empty these are required");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
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
                    whatsapptemplate = "advait_quote_automaiton";
                }
                else if (quoteDetails.TypeOfSolution == "Accessories" || quoteDetails.TypeOfSolution == "Lock")
                {
                    emaildetails = "Your Keus Accessories Quotation";
                    FileName = "Quote_Accessories_Book";
                    whatsapptemplate = "quotation_sent";
                }
                else
                {
                    emaildetails = "Your Keus Lights Quotation";
                    FileName = "Quote_Lights_Book";
                    whatsapptemplate = "advait_quote_light";
                }
                if (emaildetails.IsNullOrEmpty())
                {
                    _logger.LogError($"The Subject of the Email is Empty as the Type of Solution has not matched any Type Of Solution");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }
                if (whatsapptemplate.IsNullOrEmpty())
                {
                    _logger.LogError($"The Template for Whatsapp is Empty as the Type of Solution has not matched any Type Of Solution");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
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
                    builder.Attachments.Add(FileName, fileBytes, ContentType.Parse("application/pdf"));
                    base64 = Convert.ToBase64String(fileBytes);
                }

                email.Body = builder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                int port = (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.port).FirstOrDefault() ?? default(int));
                smtp.Connect((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.host).FirstOrDefault()), port, SecureSocketOptions.StartTls);
                smtp.Authenticate((emaildetails1.data.Where(x => x.operation == "From").Select(x => x.emailIds).FirstOrDefault()), (emaildetails1.data.Where(x => x.operation == "From").Select(x => x.password).FirstOrDefault()));

                smtp.Send(email);
                smtp.Disconnect(true);

                var jsonpayload = "{\r\n  \"recipient_type\": \"individual\",\r\n  \"to\": \"\",\r\n  \"type\": \"template\",\r\n  \"template\": {\r\n    \"name\": \"\",\r\n    \"language\": {\r\n      \"policy\": \"deterministic\",\r\n      \"code\": \"en\"\r\n    },\r\n    \"components\": [\r\n      {\r\n        \"type\": \"header\",\r\n        \"parameters\": [\r\n          {\r\n            \"type\": \"document\",\r\n            \"document\": {\r\n              \"filename\": \"\"\r\n            }\r\n          }\r\n        ]\r\n      }\r\n    ]\r\n  },\r\n  \"metadata\": {\r\n    \"messageId\": \"\",\r\n    \"media\": {\r\n      \"mimeType\": \"application/pdf\",\r\n      \"content\": \"\"\r\n    }\r\n  }\r\n}";
                WhatsAppMessagePayload whatsAppMessagePayload = JsonConvert.DeserializeObject<WhatsAppMessagePayload>(jsonpayload);
                whatsAppMessagePayload.Template.Name = whatsapptemplate;
                whatsAppMessagePayload.Template.Components[0].Parameters[0].Document.Filename = FileName;
                whatsAppMessagePayload.Metadata.Media.Content = base64;
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
                    _logger.LogError($"Unable to Generate Token for Whatsapp Message");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
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
                    _logger.LogError($"Unable to OptinNumbers for Whatsapp Message");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Something went wrong ,try again";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return StatusCode(500, serviceResponse);
                }

                foreach (var number in whatsappNumbers)
                {
                    whatsAppMessagePayload.To = number;
                    var whatsappCreate = JsonConvert.SerializeObject(whatsAppMessagePayload);
                    var data4 = new StringContent(whatsappCreate, Encoding.UTF8, "application/json");
                    var request4 = new HttpRequestMessage(HttpMethod.Post, "https://api.aclwhatsapp.com/pull-platform-receiver/v2/wa/messages")
                    {
                        Content = data4
                    };
                    request4.Headers.Add("Authorization", "Bearer " + whatsAppCreateTokenResponse.AccessToken);
                    var response4 = await client.SendAsync(request4);
                    if (!response4.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Unable to Create a Whatsapp Message");
                        serviceResponse.Data = null;
                        serviceResponse.Message = $"Something went wrong ,try again";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }
                }


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
                _logger.LogError($"Something went wrong inside SendEmailforQuote action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong ,try again";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetQuotationSPReportWithParam action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetQuotationSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
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
                headerRow.CreateCell(6).SetCellValue("Type Of Solution");
                headerRow.CreateCell(7).SetCellValue("Product Type");
                headerRow.CreateCell(8).SetCellValue("Material Group");
                headerRow.CreateCell(9).SetCellValue("Quote Created On");
                headerRow.CreateCell(10).SetCellValue("Quote Sent On");
                headerRow.CreateCell(11).SetCellValue("Room Name");
                headerRow.CreateCell(12).SetCellValue("KPN");
                headerRow.CreateCell(13).SetCellValue("KPN Description");
                headerRow.CreateCell(14).SetCellValue("UOC");
                headerRow.CreateCell(15).SetCellValue("UOM");
                headerRow.CreateCell(16).SetCellValue("Price List");
                headerRow.CreateCell(17).SetCellValue("Unit Price");
                headerRow.CreateCell(18).SetCellValue("Basic Amount");
                headerRow.CreateCell(19).SetCellValue("Discount Type");
                headerRow.CreateCell(20).SetCellValue("Total Final Amount");
                headerRow.CreateCell(21).SetCellValue("TotalAdditionalCharges");
                headerRow.CreateCell(22).SetCellValue("OrderQty");


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
                    row.CreateCell(6).SetCellValue(item.TypeOfSolution ?? "");
                    row.CreateCell(7).SetCellValue(item.ProductType ?? "");
                    row.CreateCell(8).SetCellValue(item.MaterialGroup ?? "");
                    row.CreateCell(9).SetCellValue(item.QuoteCreatedOn.HasValue ? item.QuoteCreatedOn.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(10).SetCellValue(item.QuoteSentOn.HasValue ? item.QuoteSentOn.Value.ToString("MM/dd/yyyy") : "");
                    row.CreateCell(11).SetCellValue(item.RoomName ?? "");
                    row.CreateCell(12).SetCellValue(item.KPN ?? "");
                    row.CreateCell(13).SetCellValue(item.KPNDescription ?? "");
                    row.CreateCell(14).SetCellValue(item.UOC ?? "");
                    row.CreateCell(15).SetCellValue(item.Uom ?? "");
                    row.CreateCell(16).SetCellValue(item.PriceList ?? "");
                    row.CreateCell(17).SetCellValue(item.UnitPrice.HasValue ? Convert.ToDouble(item.UnitPrice.Value) : 0);
                    row.CreateCell(18).SetCellValue(item.BasicAmount.HasValue ? Convert.ToDouble(item.BasicAmount.Value) : 0);
                    row.CreateCell(19).SetCellValue(item.DiscountType ?? "");
                    row.CreateCell(20).SetCellValue(item.TotalFinalAmount.HasValue ? Convert.ToDouble(item.TotalFinalAmount.Value) : 0);
                    row.CreateCell(21).SetCellValue(item.TotalAdditionalCharges.HasValue ? Convert.ToDouble(item.TotalAdditionalCharges.Value) : 0);
                    row.CreateCell(22).SetCellValue(item.OrderQty.HasValue ? Convert.ToDouble(item.OrderQty.Value) : 0);
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
                return StatusCode(500, $"An error occurred: {ex.Message}");
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSOSummarySPReportWithParam action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSoSummaryQuotationSPReportWithParam action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSoSummaryQuotationSPReportWithDate action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllQuoteNumberList action";
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
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllQuoteNoList action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        
    }
}

