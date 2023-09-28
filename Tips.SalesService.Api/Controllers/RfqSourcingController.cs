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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tips.SalesService.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RfqSourcingController : ControllerBase
    {
        private IRfqSourcingRepository _repository;
        private IRfqEnggItemRepository _rfqEnggItemRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private IRfqRepository _rfqRepository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public RfqSourcingController(IRfqSourcingRepository repository,IRfqRepository rfqRepository, IRfqEnggItemRepository rfqEnggItemRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _rfqRepository = rfqRepository;
            _rfqEnggItemRepository = rfqEnggItemRepository;
            _httpClient = httpClient;
            _config = config;
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

                // LandedPrice and MoqCost Calculation
                int rfqId = rfqSourcingPostDto.RFQId;
                //Taking the Sourcing PP And is Primary Vendor Details 
                List<RfqSourcingPPdetails> rfqSourcingPPdetailsList = new List<RfqSourcingPPdetails>();
                foreach (var ppinsource in createRfqSource.RfqSourcingItems)
                {
                    RfqSourcingPPdetails rfqSourcingPPdetails = new RfqSourcingPPdetails();
                    rfqSourcingPPdetails.PPItemNumber = ppinsource.ItemNumber;
                    foreach (var ppvendor in ppinsource.RfqSourcingVendors)
                    {
                        if (ppvendor.Primary == true)
                        {
                            rfqSourcingPPdetails.VLandindPrice = ppvendor.LandingPrice;
                            rfqSourcingPPdetails.VMoqcost = ppvendor.MoqCost;
                        }
                    }
                    rfqSourcingPPdetailsList.Add(rfqSourcingPPdetails);
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
                    var rfqCustomerIdResponse = await _httpClient.PostAsync($"{rfqApiUrl}GetEngganditsPP?FGItemNumber={fgitemnumber.ItemNumber}&FGRevno={fgitemnumber.CostingBomVersionNo}", content);
                    var rfqCustomerIdString = await rfqCustomerIdResponse.Content.ReadAsStringAsync();
                    var rfqCustomerIdObjectData = JsonConvert.DeserializeObject<EnggItemsLandedandMoq>(rfqCustomerIdString);
                    var rfqEnggItemsDetails = await _rfqEnggItemRepository.GetRfqEnggItemByItemNumber(rfqCustomerIdObjectData.data.fgItemNumber);
                    rfqEnggItemsDetails.LandedPrice = rfqCustomerIdObjectData.data.finalLandindPrice;
                    rfqEnggItemsDetails.MOQCost = rfqCustomerIdObjectData.data.finalMoqcost;
                    await _rfqEnggItemRepository.UpdateRfqEnggItemLandedandMOQ(rfqEnggItemsDetails);
                }
                _rfqEnggItemRepository.SaveAsync();
                _repository.SaveAsync();
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

        // PUT api/<RfqSourcingController>/5
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

                var sourceItemtemDto = rfqSourcingUpdateDto.RfqSourcingItemsUpdateDtos;

                var rfqSourceItemList = new List<RfqSourcingItems>();

                if (sourceItemtemDto !=null) 
                {
                    for (int i = 0; i < sourceItemtemDto.Count; i++)
                    {
                        RfqSourcingItems sourceItemDetail = _mapper.Map<RfqSourcingItems>(sourceItemtemDto[i]);
                        sourceItemDetail.RfqSourcingVendors = _mapper.Map<List<RfqSourcingVendor>>(sourceItemtemDto[i].RfqSourcingVendorDtos);

                        rfqSourceItemList.Add(sourceItemDetail);

                    }
                }
                var updateData = _mapper.Map(rfqSourcingUpdateDto, getRfqSourcings);

                updateData.RfqSourcingItems = rfqSourceItemList;          

                string result = await _repository.UpdateRfqSourcing(updateData);
                _logger.LogInfo(result);
                _repository.SaveAsync();
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
    }
}
