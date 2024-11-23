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
using System.Text;
using Microsoft.AspNetCore.Authorization;
using MySqlX.XDevAPI;
using NuGet.Common;
using NPOI.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RfqSourcingController : ControllerBase
    {
        private IRfqSourcingRepository _repository;
        private IRfqEnggItemRepository _rfqEnggItemRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IRfqRepository _rfqRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        public RfqSourcingController(IRfqSourcingRepository repository, IHttpClientFactory clientFactory, IRfqRepository rfqRepository, IRfqEnggItemRepository rfqEnggItemRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _rfqRepository = rfqRepository;
            _rfqEnggItemRepository = rfqEnggItemRepository;
            _httpClient = httpClient;
            _config = config;
            _clientFactory = clientFactory;
        }
        // GET: api/<RfqSourcingController>
        [HttpGet]
        public async Task<IActionResult> GetAllRfqSourcings([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParammes searchParammes)
        {
            ServiceResponse<IEnumerable<RfqSourcingDto>> serviceResponse = new ServiceResponse<IEnumerable<RfqSourcingDto>>();

            try
            {
                var getAllRfqSourcing = await _repository.GetAllRfqSourcing(pagingParameter, searchParammes);
                var metadata = new
                {
                    getAllRfqSourcing.TotalCount,
                    getAllRfqSourcing.PageSize,
                    getAllRfqSourcing.CurrentPage,
                    getAllRfqSourcing.HasNext,
                    getAllRfqSourcing.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all getAllRfqSourcing");
                var result = _mapper.Map<IEnumerable<RfqSourcingDto>>(getAllRfqSourcing);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all RfqSourcings Successfully";
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
                var rfqSourcings = await _repository.GetRfqSourcingById(id);

                if (rfqSourcings == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"rfqsourcing with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"rfqsourcing with id: {id}, hasn't been found.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned rfqsourcing with id: {id}");



                    RfqSourcingDto rfqSourceDto = _mapper.Map<RfqSourcingDto>(rfqSourcings);


                    List<RfqSourcingItemsDto> rfqSourceItemDtos = new List<RfqSourcingItemsDto>();


                    foreach (var rfqSourceItemDetails in rfqSourcings.RfqSourcingItems)
                    {
                        RfqSourcingItemsDto rfqSourceItemDto = _mapper.Map<RfqSourcingItemsDto>(rfqSourceItemDetails);
                        rfqSourceItemDto.RfqSourcingVendorDtos = _mapper.Map<List<RfqSourcingVendorDto>>(rfqSourceItemDetails.RfqSourcingVendors);
                        rfqSourceItemDtos.Add(rfqSourceItemDto);
                    }


                    rfqSourceDto.RfqSourcingItemsDtos = rfqSourceItemDtos;
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

        [HttpGet("{rfqNo}")]
        public async Task<IActionResult> GetRfqSourcingDetailsByRfqNo(string rfqNo)
        {
            ServiceResponse<RfqSourcingDto> serviceResponse = new ServiceResponse<RfqSourcingDto>();

            try
            {
                var rfqSourcingByRfqNo = await _repository.GetRfqSourcingDetailsByRfqNo(rfqNo);

                if (rfqSourcingByRfqNo == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"rfqsourcing with rfqNo: {rfqNo}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    _logger.LogError($"rfqsourcing with rfqNo: {rfqNo}, hasn't been found.");
                    return Ok(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned rfqsourcing with rfqNo: {rfqNo}");



                    RfqSourcingDto rfqSourceDto = _mapper.Map<RfqSourcingDto>(rfqSourcingByRfqNo);


                    List<RfqSourcingItemsDto> rfqSourceItemDtos = new List<RfqSourcingItemsDto>();


                    foreach (var rfqSourceItemDetails in rfqSourcingByRfqNo.RfqSourcingItems)
                    {
                        RfqSourcingItemsDto rfqSourceItemDto = _mapper.Map<RfqSourcingItemsDto>(rfqSourceItemDetails);
                        rfqSourceItemDto.RfqSourcingVendorDtos = _mapper.Map<List<RfqSourcingVendorDto>>(rfqSourceItemDetails.RfqSourcingVendors);
                        rfqSourceItemDtos.Add(rfqSourceItemDto);
                    }


                    rfqSourceDto.RfqSourcingItemsDtos = rfqSourceItemDtos;
                    serviceResponse.Data = rfqSourceDto;
                    serviceResponse.Message = $"Returned RfqsourcingDetails with rfqNo: {rfqNo}";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetrfqsourcingDetailsByRfqNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Something went wrong. Please try again!";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }

        }

        //[HttpPost]
        //public async Task<IActionResult> CreateRfqSourcing([FromBody] RfqSourcingPostDto rfqSourcingPostDto)
        //{
        //    ServiceResponse<RfqSourcingDto> serviceResponse = new ServiceResponse<RfqSourcingDto>();

        //    try
        //    {
        //        if (rfqSourcingPostDto is null)
        //        {
        //            _logger.LogError("RfqSourcing object sent from client is null.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "RfqSourcing object sent from client is null.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogError("Invalid RfqSourcing object sent from client.");
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "Invalid RfqSourcing object sent from client.";
        //            serviceResponse.Success = false;
        //            serviceResponse.StatusCode = HttpStatusCode.BadRequest;
        //            return BadRequest(serviceResponse);
        //        }



        //        var createRfqSource = _mapper.Map<RfqSourcing>(rfqSourcingPostDto);

        //        var rfqSourceData = createRfqSource.RFQNumber;

        //        var rfqIsSourcingUpdate = await _rfqRepository.RfqSourcingByRfqNumbers(rfqSourceData);

        //        rfqIsSourcingUpdate.IsSourcing = true;

        //        var rfqSourceDto = rfqSourcingPostDto.RfqSourcingItemsPostDtos;

        //        var sourceItemList = new List<RfqSourcingItems>();

        //            if (rfqSourceDto != null)
        //            {
        //                for (int i = 0; i < rfqSourceDto.Count; i++)
        //                {
        //                    RfqSourcingItems rfqSourcingItems = _mapper.Map<RfqSourcingItems>(rfqSourceDto[i]);
        //                    rfqSourcingItems.RfqSourcingVendors = _mapper.Map<List<RfqSourcingVendor>>(rfqSourceDto[i].RfqSourcingVendorDtos);
        //                    sourceItemList.Add(rfqSourcingItems);
        //                }
        //            }
        //            createRfqSource.RfqSourcingItems = sourceItemList;

        //            await _repository.CreateRfqSourcing(createRfqSource);
        //            _rfqRepository.Update(rfqIsSourcingUpdate);

        //        // LandedPrice and MoqCost Calculation
        //        int rfqId = rfqSourcingPostDto.RFQId;
        //        //Taking the Sourcing PP And is Primary Vendor Details 
        //        List<RfqSourcingPPdetails> rfqSourcingPPdetailsList = new List<RfqSourcingPPdetails>();
        //        foreach (var ppinsource in createRfqSource.RfqSourcingItems)
        //        {
        //            RfqSourcingPPdetails rfqSourcingPPdetails = new RfqSourcingPPdetails();
        //            rfqSourcingPPdetails.PPItemNumber = ppinsource.ItemNumber;
        //            foreach (var ppvendor in ppinsource.RfqSourcingVendors)
        //            {
        //                if (ppvendor.Primary == true)
        //                {
        //                    rfqSourcingPPdetails.VLandindPrice = ppvendor.LandindPrice;
        //                    rfqSourcingPPdetails.VMoqcost = ppvendor.MoqCost;
        //                }
        //            }
        //            rfqSourcingPPdetailsList.Add(rfqSourcingPPdetails);
        //        }
        //        //Getting the FG's of that RFQ
        //        List<RfqEnggItem> listofFgs = await _rfqEnggItemRepository.GetRfqEnggItemsbyRfqId(rfqId);
        //        foreach (var fgitemnumber in listofFgs)
        //        {
        //            //Calculating for the Fg LandedPrice and MOQCost
        //            var httpClientHandler = new HttpClientHandler();
        //            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        //            var httpClient = new HttpClient(httpClientHandler);
        //            string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(rfqSourcingPPdetailsList);
        //            var rfqApiUrl = _config["EngineeringBomAPI"];
        //            var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
        //            var rfqCustomerIdResponse = await _httpClient.PostAsync($"{rfqApiUrl}GetEngganditsPP?FGItemNumber={fgitemnumber.ItemNumber}&FGRevno={fgitemnumber.CostingBomVersionNo}", content);
        //            var rfqCustomerIdString = await rfqCustomerIdResponse.Content.ReadAsStringAsync();
        //            var rfqCustomerIdObjectData = JsonConvert.DeserializeObject<EnggItemsLandedandMoq>(rfqCustomerIdString);
        //            var rfqEnggItemsDetails = await _rfqEnggItemRepository.GetRfqEnggItemByItemNumber(rfqCustomerIdObjectData.data.fgItemNumber);
        //            rfqEnggItemsDetails.LandedPrice = rfqCustomerIdObjectData.data.finalLandindPrice;
        //            rfqEnggItemsDetails.MOQCost = rfqCustomerIdObjectData.data.finalMoqcost;
        //            await _rfqEnggItemRepository.UpdateRfqEnggItemLandedandMOQ(rfqEnggItemsDetails);
        //        }
        //        _rfqEnggItemRepository.SaveAsync();
        //        _repository.SaveAsync();
        //            serviceResponse.Data = null;
        //            serviceResponse.Message = "RfqSourcing Created Successfully";
        //            serviceResponse.Success = true;
        //            serviceResponse.StatusCode = HttpStatusCode.OK;
        //            return Created("GetRfqSourceById", serviceResponse);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Something went wrong inside CreateRfqSource action: {ex.Message}");
        //        serviceResponse.Data = null;
        //        serviceResponse.Message = "Internal server error";
        //        serviceResponse.Success = false;
        //        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
        //        return StatusCode(500, serviceResponse);
        //    }
        //}

        // PUT api/<RfqSourcingController>/5
        [HttpPost]
        public async Task<IActionResult> CreateRfqSourcing([FromBody] RfqSourcingPostDto rfqSourcingPostDto)
        {
            ServiceResponse<RfqSourcingDto> serviceResponse = new ServiceResponse<RfqSourcingDto>();

            try
            {
                if (rfqSourcingPostDto is null)
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
                var createRfqSource = _mapper.Map<RfqSourcing>(rfqSourcingPostDto);
                var rfqSourceData = createRfqSource.RFQNumber;
                var rfqIsSourcingUpdate = await _rfqRepository.RfqSourcingByRfqNumbers(rfqSourceData);
                rfqIsSourcingUpdate.IsSourcing = true;
                var rfqSourceDto = rfqSourcingPostDto.RfqSourcingItemsPostDtos;
                var sourceItemList = new List<RfqSourcingItems>();
                if (rfqSourceDto != null)
                {
                    for (int i = 0; i < rfqSourceDto.Count; i++)
                    {
                        RfqSourcingItems rfqSourcingItems = _mapper.Map<RfqSourcingItems>(rfqSourceDto[i]);
                        rfqSourcingItems.RfqSourcingVendors = _mapper.Map<List<RfqSourcingVendor>>(rfqSourceDto[i].RfqSourcingVendorDtos);
                        sourceItemList.Add(rfqSourcingItems);
                    }
                }
                createRfqSource.RfqSourcingItems = sourceItemList;
                await _repository.CreateRfqSourcing(createRfqSource);
                _rfqRepository.Update(rfqIsSourcingUpdate);
                _logger.LogInfo("Before Json");
                var sourcingData = _mapper.Map<RfqSourcing>(rfqSourcingPostDto);
                var sourceItemList_1 = new List<RfqSourcingItems>();
                if (rfqSourceDto != null)
                {
                    for (int i = 0; i < rfqSourceDto.Count; i++)
                    {
                        RfqSourcingItems rfqSourcingItems = _mapper.Map<RfqSourcingItems>(rfqSourceDto[i]);
                        rfqSourcingItems.RfqSourcingVendors = _mapper.Map<List<RfqSourcingVendor>>(rfqSourceDto[i].RfqSourcingVendorDtos);
                        sourceItemList_1.Add(rfqSourcingItems);
                    }
                }
                sourcingData.RfqSourcingItems = sourceItemList_1;
                // RfqSourcing sourcingData = createRfqSource;
                //var sourcingData = new RfqSourcing
                //{
                //    Id = createRfqSource.Id,
                //    RFQId = createRfqSource.RFQId,
                //    RFQNumber = createRfqSource.RFQNumber,
                //    CustomerName = createRfqSource.CustomerName,
                //    Unit = createRfqSource.Unit,
                //    CreatedBy = createRfqSource.CreatedBy,
                //    CreatedOn = createRfqSource.CreatedOn,
                //    LastModifiedBy = createRfqSource.LastModifiedBy,
                //    LastModifiedOn = createRfqSource.LastModifiedOn,
                //    RfqSourcingItems = createRfqSource.RfqSourcingItems
                //};
                _logger.LogInfo("After Json");
                // LandedPrice and MoqCost Calculation
                int rfqId = rfqSourcingPostDto.RFQId;
                //Taking the Sourcing PP And is Primary Vendor Details 
                List<RfqSourcingPPdetails> rfqSourcingPPdetailsList = new List<RfqSourcingPPdetails>();
                foreach (var ppinsource in sourcingData.RfqSourcingItems)
                {
                    if (ppinsource.RfqSourcingVendors != null && ppinsource.RfqSourcingVendors.Count != 0)
                    {
                        RfqSourcingPPdetails rfqSourcingPPdetails = new RfqSourcingPPdetails();
                        rfqSourcingPPdetails.PPItemNumber = ppinsource.ItemNumber;
                        foreach (var ppvendor in ppinsource.RfqSourcingVendors)
                        {
                            if (ppvendor.Primary == true)
                            {
                                if (ppvendor.Currency != "INR")
                                {
                                    var httpClientHandler = new HttpClientHandler();
                                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                    var httpClient = new HttpClient(httpClientHandler);
                                    var client = _clientFactory.CreateClient();
                                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                                    var rfqApiUrl = _config["ConvertionrateAPI"];
                                    var encodedCurrency = Uri.EscapeDataString(ppvendor.Currency);
                                    var request = new HttpRequestMessage(HttpMethod.Get, $"{rfqApiUrl}GetLatestConvertionrateByUOC?currency={encodedCurrency}");
                                    request.Headers.Add("Authorization", token);

                                    var rfqCustomerIdResponse = await client.SendAsync(request);
                                    //var rfqCustomerIdResponse = await _httpClient.GetAsync($"{rfqApiUrl}GetLatestConvertionrateByUOC?currency={ppvendor.Currency}");
                                    var rfqCustomerIdString = await rfqCustomerIdResponse.Content.ReadAsStringAsync();
                                    var vendorUOC = JsonConvert.DeserializeObject<RfqSourcingConvertionrateDto>(rfqCustomerIdString);
                                    if (vendorUOC.Data.ConvertionRate == 0)
                                    {
                                        _logger.LogError($"Currency was not present for {ppvendor.Currency} for the Vendor {ppvendor.Vendor} for the ItemNumber{ppinsource.ItemNumber}");
                                        serviceResponse.Data = null;
                                        serviceResponse.Message = $"Currency was not present for {ppvendor.Currency} for the Vendor {ppvendor.Vendor} for the ItemNumber{ppinsource.ItemNumber}";
                                        serviceResponse.Success = false;
                                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                                        return NotFound(serviceResponse);
                                    }
                                    ppvendor.LandingPrice = ppvendor.LandingPrice * vendorUOC.Data.ConvertionRate;
                                    ppvendor.MoqCost = ppvendor.MoqCost * vendorUOC.Data.ConvertionRate;
                                }
                                rfqSourcingPPdetails.VLandindPrice = ppvendor.LandingPrice;
                                rfqSourcingPPdetails.VMoqcost = ppvendor.MoqCost;
                            }
                        }
                        rfqSourcingPPdetailsList.Add(rfqSourcingPPdetails);
                    }
                }
                //Getting the FG's of that RFQ
                List<RfqEnggItem> listofFgs = await _rfqEnggItemRepository.GetRfqEnggItemsbyRfqId(rfqId);
                foreach (var fgitemnumber in listofFgs)
                {
                    //Calculating for the Fg LandedPrice and MOQCost
                    var httpClientHandler = new HttpClientHandler();
                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                    var httpClient = new HttpClient(httpClientHandler);
                    string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(rfqSourcingPPdetailsList);
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var rfqApiUrl = _config["EngineeringBomAPI"];
                    var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                    var encodedItemnumber = Uri.EscapeDataString(fgitemnumber.ItemNumber);
                    var request1 = new HttpRequestMessage(HttpMethod.Post, $"{rfqApiUrl}GetEngganditsPP?FGItemNumber={encodedItemnumber}&FGRevno={fgitemnumber.CostingBomVersionNo}")
                    {
                        Content = content
                    };
                    request1.Headers.Add("Authorization", token);

                    var rfqCustomerIdResponse = await client.SendAsync(request1);
                    // var rfqCustomerIdResponse = await _httpClient.PostAsync($"{rfqApiUrl}GetEngganditsPP?FGItemNumber={fgitemnumber.ItemNumber}&FGRevno={fgitemnumber.CostingBomVersionNo}", content);
                    var rfqCustomerIdString = await rfqCustomerIdResponse.Content.ReadAsStringAsync();
                    var rfqCustomerIdObjectData = JsonConvert.DeserializeObject<EnggItemsLandedandMoq>(rfqCustomerIdString);
                    //var rfqEnggItemsDetails = await _rfqRepository.GetRfqEnggItemByItemNumber(rfqCustomerIdObjectData.data.fgItemNumber);
                    fgitemnumber.LandedPrice = rfqCustomerIdObjectData.data.finalLandindPrice;
                    fgitemnumber.MOQCost = rfqCustomerIdObjectData.data.finalMoqcost;
                    await _rfqEnggItemRepository.UpdateRfqEnggItemLandedandMOQ(fgitemnumber);
                }
                _repository.SaveAsync();
                _rfqEnggItemRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RfqSourcing Created Successfully";
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRfqSourcing(int id, [FromBody] RfqSourcingUpdateDto rfqSourcingUpdateDto)
        {
            ServiceResponse<RfqSourcingDto> serviceResponse = new ServiceResponse<RfqSourcingDto>();

            try
            {
                if (rfqSourcingUpdateDto is null)
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
                var getRfqSourcings = await _repository.GetRfqSourcingById(id);
                if (getRfqSourcings is null)
                {
                    _logger.LogError($"RfqSource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update RfqSource with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                var updateRfqSourcing = _mapper.Map<RfqSourcing>(rfqSourcingUpdateDto);
                var rfqSourceData = updateRfqSourcing.RFQNumber;
                var rfqIsSourcingUpdate = await _rfqRepository.RfqSourcingByRfqNumbers(rfqSourceData);
                rfqIsSourcingUpdate.IsSourcing = true;
                var sourceItemtemDto = rfqSourcingUpdateDto.RfqSourcingItemsUpdateDtos;
                var rfqSourceItemList = new List<RfqSourcingItems>();
                if (sourceItemtemDto != null)
                {
                    for (int i = 0; i < sourceItemtemDto.Count; i++)
                    {
                        RfqSourcingItems sourceItemDetail = _mapper.Map<RfqSourcingItems>(sourceItemtemDto[i]);
                        sourceItemDetail.RfqSourcingVendors = _mapper.Map<List<RfqSourcingVendor>>(sourceItemtemDto[i].RfqSourcingVendorDtos);
                        rfqSourceItemList.Add(sourceItemDetail);
                    }
                }
                getRfqSourcings.RfqSourcingItems = rfqSourceItemList;
                var updateData = _mapper.Map(rfqSourcingUpdateDto, getRfqSourcings);
                //updateData.RfqSourcingItems = rfqSourceItemList;          
                string result = await _repository.UpdateRfqSourcing(updateData);
                _rfqRepository.Update(rfqIsSourcingUpdate);
                _logger.LogInfo("Before Json");
                //var sourcingData = new RfqSourcing
                //{
                //    Id = updateData.Id,
                //    RFQId = updateData.RFQId,
                //    RFQNumber = updateData.RFQNumber,
                //    CustomerName = updateData.CustomerName,
                //    Unit = updateData.Unit,
                //    CreatedBy = updateData.CreatedBy,
                //    CreatedOn = updateData.CreatedOn,
                //    LastModifiedBy = updateData.LastModifiedBy,
                //    LastModifiedOn = updateData.LastModifiedOn,
                //    RfqSourcingItems = updateData.RfqSourcingItems
                //};
                var sourcingData = _mapper.Map<RfqSourcing, RfqSourcing>(updateData);

                _logger.LogInfo("After Json");
                // LandedPrice and MoqCost Calculation
                int rfqId = rfqSourcingUpdateDto.RFQId;
                //Taking the Sourcing PP And is Primary Vendor Details 
                List<RfqSourcingPPdetails> rfqSourcingPPdetailsList = new List<RfqSourcingPPdetails>();
                foreach (var ppinsource in sourcingData.RfqSourcingItems)
                {
                    if (ppinsource.RfqSourcingVendors != null && ppinsource.RfqSourcingVendors.Count != 0)
                    {
                        RfqSourcingPPdetails rfqSourcingPPdetails = new RfqSourcingPPdetails();
                        rfqSourcingPPdetails.PPItemNumber = ppinsource.ItemNumber;
                        foreach (var ppvendor in ppinsource.RfqSourcingVendors)
                        {
                            if (ppvendor.Primary == true)
                            {
                                var LP = ppvendor.LandingPrice;
                                var MC = ppvendor.MoqCost;
                                if (ppvendor.Currency != "INR")
                                {
                                    var httpClientHandler = new HttpClientHandler();
                                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                                    var httpClient = new HttpClient(httpClientHandler);
                                    var client = _clientFactory.CreateClient();
                                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                                    var rfqApiUrl = _config["ConvertionrateAPI"];
                                    var encodedCurrency = Uri.EscapeDataString(ppvendor.Currency);
                                    var request = new HttpRequestMessage(HttpMethod.Get, $"{rfqApiUrl}GetLatestConvertionrateByUOC?currency={encodedCurrency}");
                                    request.Headers.Add("Authorization", token);

                                    var rfqCustomerIdResponse = await client.SendAsync(request);
                                    //var rfqCustomerIdResponse = await _httpClient.GetAsync($"{rfqApiUrl}GetLatestConvertionrateByUOC?currency={ppvendor.Currency}");
                                    var rfqCustomerIdString = await rfqCustomerIdResponse.Content.ReadAsStringAsync();
                                    var vendorUOC = JsonConvert.DeserializeObject<RfqSourcingConvertionrateDto>(rfqCustomerIdString);
                                    if (vendorUOC.Data.ConvertionRate == 0)
                                    {
                                        _logger.LogError($"Currency was not present for {ppvendor.Currency} for the Vendor {ppvendor.Vendor} for the ItemNumber{ppinsource.ItemNumber}");
                                        serviceResponse.Data = null;
                                        serviceResponse.Message = $"Currency was not present for {ppvendor.Currency} for the Vendor {ppvendor.Vendor} for the ItemNumber{ppinsource.ItemNumber}";
                                        serviceResponse.Success = false;
                                        serviceResponse.StatusCode = HttpStatusCode.NotFound;
                                        return NotFound(serviceResponse);
                                    }
                                    LP = LP * vendorUOC.Data.ConvertionRate;
                                    MC = MC * vendorUOC.Data.ConvertionRate;
                                }
                                rfqSourcingPPdetails.VLandindPrice = LP;
                                rfqSourcingPPdetails.VMoqcost = MC;
                            }
                        }
                        rfqSourcingPPdetailsList.Add(rfqSourcingPPdetails);
                    }
                }
                //Getting the FG's of that RFQ
                List<RfqEnggItem> listofFgs = await _rfqEnggItemRepository.GetRfqEnggItemsbyRfqId(rfqId);
                foreach (var fgitemnumber in listofFgs)
                {
                    //Calculating for the Fg LandedPrice and MOQCost
                    var httpClientHandler = new HttpClientHandler();
                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                    var httpClient = new HttpClient(httpClientHandler);
                    string rfqSourcingPPdetailsJson = JsonConvert.SerializeObject(rfqSourcingPPdetailsList);
                    var rfqApiUrl = _config["EngineeringBomAPI"];
                    var content = new StringContent(rfqSourcingPPdetailsJson, Encoding.UTF8, "application/json");
                    var client = _clientFactory.CreateClient();
                    var token = HttpContext.Request.Headers["Authorization"].ToString();
                    var encodedItemnumber = Uri.EscapeDataString(fgitemnumber.ItemNumber);
                    var request1 = new HttpRequestMessage(HttpMethod.Post, $"{rfqApiUrl}GetEngganditsPP?FGItemNumber={encodedItemnumber}&FGRevno={fgitemnumber.CostingBomVersionNo}")
                    {
                        Content = content
                    };
                    request1.Headers.Add("Authorization", token);

                    var rfqCustomerIdResponse = await client.SendAsync(request1);
                    // var rfqCustomerIdResponse = await _httpClient.PostAsync($"{rfqApiUrl}GetEngganditsPP?FGItemNumber={fgitemnumber.ItemNumber}&FGRevno={fgitemnumber.CostingBomVersionNo}", content);
                    var rfqCustomerIdString = await rfqCustomerIdResponse.Content.ReadAsStringAsync();
                    var rfqCustomerIdObjectData = JsonConvert.DeserializeObject<EnggItemsLandedandMoq>(rfqCustomerIdString);
                    //var rfqEnggItemsDetails = await _rfqRepository.GetRfqEnggItemByItemNumber(rfqCustomerIdObjectData.data.fgItemNumber);
                    fgitemnumber.LandedPrice = rfqCustomerIdObjectData.data.finalLandindPrice;
                    fgitemnumber.MOQCost = rfqCustomerIdObjectData.data.finalMoqcost;
                    await _rfqEnggItemRepository.UpdateRfqEnggItemLandedandMOQ(fgitemnumber);
                }
                _logger.LogInfo(result);
                _repository.SaveAsync();
                _rfqEnggItemRepository.SaveAsync();
                _rfqRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RfqSourcing Updated Successfully";
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
                var deleteRfqSourcing = await _repository.GetRfqSourcingById(id);
                if (deleteRfqSourcing == null)
                {
                    _logger.LogError($"RfqSource with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Delete RfqSource with id: {id}, hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                string result = await _repository.DeleteRfqSourcing(deleteRfqSourcing);
                _logger.LogInfo(result);
                _repository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "RfqSourcing Deleted Successfully";
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

        [HttpGet]
        public async Task<IActionResult> GetRfqSourcingVendorDetails(string ProjectNumber, string ItemNumber, string VendorId)
        {
            ServiceResponse<RfqSourcingVendorDetailsDto> serviceResponse = new ServiceResponse<RfqSourcingVendorDetailsDto>();
            try
            {
                var rfqSourcingVendorDetails = await _repository.GetRfqSourcingVendorDetails(ProjectNumber, ItemNumber, VendorId);

                if (rfqSourcingVendorDetails == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"VendorName  hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"VendorName with id: {VendorId}, hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned VendorName with id: {VendorId}");

                    serviceResponse.Data = rfqSourcingVendorDetails;
                    serviceResponse.Message = "Returned RfqSourcingVendorDetail Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside GetRfqSourcingVendorDetails action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong,try again ";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }


        }

        [HttpPost] // Adjust your route as needed
        public async Task<IActionResult> GetSourcingSPReportWithParam([FromBody] SourcingSPReportDto sourcingSPReportDto)

        {
            ServiceResponse<IEnumerable<SourcingSPReport>> serviceResponse = new ServiceResponse<IEnumerable<SourcingSPReport>>();
            try
            {
                var products = await _repository.GetSourcingSPReportWithParam(sourcingSPReportDto.Vendor);

                if (products == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Sourcing hasn't been found.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    _logger.LogError($"Sourcing hasn't been found in db.");
                    return NotFound(serviceResponse);
                }
                else
                {

                    serviceResponse.Data = products;
                    serviceResponse.Message = "Returned Sourcing Details";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetSourcingSPReportWithParam action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}
