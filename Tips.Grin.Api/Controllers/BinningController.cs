using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Tips.Grin.Api.Contracts;
using Tips.Grin.Api.Entities.DTOs;
using Tips.Grin.Api.Entities;
using Entities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Dynamic;
using System.IO;
using Tips.Grin.Api.Repository;
using Entities.DTOs;
using MySqlX.XDevAPI.Common;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Diagnostics.Eventing.Reader;
using System.Web;
using Azure;
using Mysqlx.Crud;
using Microsoft.VisualBasic;

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BinningController : ControllerBase
    {
        private IBinningRepository _binningRepository;
        private IBinningItemsRepository _binningItemsRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private IGrinRepository _grinRepository;
        private IGrinPartsRepository _grinPartsRepository;
        private IIQCConfirmationRepository _iQCConfirmationRepository;
        private IIQCConfirmationItemsRepository _iQCConfirmationItemsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IBinningLocationRepository _binningLocationRepository;
        public BinningController(IBinningLocationRepository binningLocationRepository, IHttpClientFactory clientFactory, IIQCConfirmationItemsRepository iQCConfirmationItemsRepository, IIQCConfirmationRepository iQCConfirmationRepository, IGrinPartsRepository grinPartsRepository, IGrinRepository grinRepository, IBinningRepository binningRepository, IBinningItemsRepository binningItemsRepository, ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _binningRepository = binningRepository;
            _binningItemsRepository = binningItemsRepository;
            _mapper = mapper;
            _httpClient = httpClient;
            _config = config;
            _grinRepository = grinRepository;
            _grinPartsRepository = grinPartsRepository;
            _iQCConfirmationRepository = iQCConfirmationRepository;
            _iQCConfirmationItemsRepository = iQCConfirmationItemsRepository;
            _httpContextAccessor = httpContextAccessor;
            _binningLocationRepository = binningLocationRepository;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBinningDetails([FromQuery] PagingParameter pagingParameter, [FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<GrinAndBinningDetailsDto>> serviceResponse = new ServiceResponse<IEnumerable<GrinAndBinningDetailsDto>>();

            try
            {
                var getAllBinnings = await _binningRepository.GetAllBinningDetails(pagingParameter, searchParams);
                var metadata = new
                {
                    getAllBinnings.TotalCount,
                    getAllBinnings.PageSize,
                    getAllBinnings.CurrentPage,
                    getAllBinnings.HasNext,
                    getAllBinnings.HasPreviuos
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                _logger.LogInfo("Returned all Binnings");
                var result = _mapper.Map<IEnumerable<GrinAndBinningDetailsDto>>(getAllBinnings);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned all Binnings";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllBinningDetails,try again";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet("{grinNo}")]
        public async Task<IActionResult> GetBinningDetailsByGrinNo(string grinNo)
        {
            ServiceResponse<IEnumerable<BinningDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningDto>>();

            try
            {
                var binningDetailsByGrinNo = await _binningRepository.GetBinningDetailsByGrinNo(grinNo);
                if (binningDetailsByGrinNo == null)
                {
                    _logger.LogError($"Binning Details with GrinNumber: {grinNo}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Binning Details with GrinNumber: {grinNo}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }
                else
                {
                    _logger.LogInfo($"Returned Binning Details with id: {grinNo}");

                    var binningDto = _mapper.Map<IEnumerable<BinningDto>>(binningDetailsByGrinNo);
                    var binningDto1 = new List<BinningDto>();
                    foreach (var binningDtos in binningDto)
                    {
                        var grinDetails = await _grinRepository.GetGrinByGrinNo(binningDtos.GrinNumber);
                        binningDtos.VendorName = grinDetails.VendorName;
                        binningDtos.InvoiceNumber = grinDetails.InvoiceNumber;
                        binningDto1.Add(binningDtos);
                    }

                    serviceResponse.Data = binningDto1;
                    serviceResponse.Message = "Success";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(binningDto1);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside BinningByGrinNo action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchBinningDate([FromQuery] SearchDateParames searchDateParam)
        {
            ServiceResponse<IEnumerable<BinningReportDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningReportDto>>();
            try
            {
                var binningList = await _binningRepository.SearchBinningDate(searchDateParam);

                // Get all the unique GrinPartIds from the iQCList
                var grinPartIds = binningList
                    .SelectMany(iqc => iqc.BinningItems.Select(item => item.GrinPartId))
                    .Distinct()
                    .ToList();

                // Fetch all the required GrinPart details in a single query and store them in a dictionary
                var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsByGrinPartIds(grinPartIds);
                var grinPartDetailsLookup = grinPartDetails.ToDictionary(gp => gp.Id, gp => gp);

                // Use the grinPartDetailsLookup for quick lookups while mapping the data to DTO objects
                var binningListDto = binningList.Select(bin => new BinningReportDto
                {
                    // Map Binning properties here (assuming properties with the same name exist in the DTO)
                    Id = bin.Id,
                    GrinNumber = bin.GrinNumber,
                    Unit = bin.Unit,
                    CreatedBy = bin.CreatedBy,
                    CreatedOn = bin.CreatedOn,
                    LastModifiedBy = bin.LastModifiedBy,
                    LastModifiedOn = bin.LastModifiedOn,
                    // ...

                    BinningItems = bin.BinningItems.Select(item => new BinningItemsReportDto
                    {
                        //Map BinningItem properties here (assuming properties with the same name exist in the DTO)
                        Id = item.Id,
                        ItemNumber = item.ItemNumber,
                        GrinNumber = bin.GrinNumber,

                        ReceivedQty = grinPartDetailsLookup[item.GrinPartId].Qty,
                        MftrItemNumber = grinPartDetailsLookup[item.GrinPartId].MftrItemNumber,
                        PONumber = grinPartDetailsLookup[item.GrinPartId].PONumber,
                        POBalancedQty = grinPartDetailsLookup[item.GrinPartId].POBalancedQty,
                        POOrderedQty = grinPartDetailsLookup[item.GrinPartId].POOrderQty,
                        POUnitPrice = grinPartDetailsLookup[item.GrinPartId].POUnitPrice,
                        Description = grinPartDetailsLookup[item.GrinPartId].ItemDescription,
                        UnitPrice = grinPartDetailsLookup[item.GrinPartId].UnitPrice,
                        ManufactureBatchNumber = grinPartDetailsLookup[item.GrinPartId].ManufactureBatchNumber,
                        UOM = grinPartDetailsLookup[item.GrinPartId].UOM,
                        ExpiryDate = grinPartDetailsLookup[item.GrinPartId].ExpiryDate,
                        ManufactureDate = grinPartDetailsLookup[item.GrinPartId].ManufactureDate,
                        AcceptedQty = grinPartDetailsLookup[item.GrinPartId].AcceptedQty,
                        RejectedQty = grinPartDetailsLookup[item.GrinPartId].RejectedQty,

                        binningLocations = item.binningLocations.Select(loc => new BinningLocationReportDto
                        {
                            GrinNumber = bin.GrinNumber,
                            ItemNumber = item.ItemNumber,
                            Warehouse = loc.Warehouse,
                            Location = loc.Location,
                            Qty = loc.Qty,
                            CreatedBy = loc.CreatedBy,
                            CreatedOn = loc.CreatedOn,
                            LastModifiedBy = loc.LastModifiedBy,
                            LastModifiedOn = loc.LastModifiedOn,


                        }).ToList(),

                    }).ToList(),
                }).ToList();


                serviceResponse.Data = binningListDto;
                serviceResponse.Message = "Returned all BinningDates";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchBinning([FromQuery] SearchParames searchParams)
        {
            ServiceResponse<IEnumerable<BinningReportDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningReportDto>>();
            try
            {
                var binningList = await _binningRepository.SearchBinning(searchParams);

                // Get all the unique GrinPartIds from the iQCList
                var grinPartIds = binningList
                    .SelectMany(iqc => iqc.BinningItems.Select(item => item.GrinPartId))
                    .Distinct()
                    .ToList();

                // Fetch all the required GrinPart details in a single query and store them in a dictionary
                var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsByGrinPartIds(grinPartIds);
                var grinPartDetailsLookup = grinPartDetails.ToDictionary(gp => gp.Id, gp => gp);

                // Use the grinPartDetailsLookup for quick lookups while mapping the data to DTO objects
                var binningListDto = binningList.Select(bin => new BinningReportDto
                {
                    // Map Binning properties here (assuming properties with the same name exist in the DTO)
                    Id = bin.Id,
                    GrinNumber = bin.GrinNumber,
                    Unit = bin.Unit,
                    CreatedBy = bin.CreatedBy,
                    CreatedOn = bin.CreatedOn,
                    LastModifiedBy = bin.LastModifiedBy,
                    LastModifiedOn = bin.LastModifiedOn,
                    // ...

                    BinningItems = bin.BinningItems.Select(item => new BinningItemsReportDto
                    {
                        //Map BinningItem properties here (assuming properties with the same name exist in the DTO)
                        Id = item.Id,
                        ItemNumber = item.ItemNumber,
                        GrinNumber = bin.GrinNumber,

                        ReceivedQty = grinPartDetailsLookup[item.GrinPartId].Qty,
                        MftrItemNumber = grinPartDetailsLookup[item.GrinPartId].MftrItemNumber,
                        PONumber = grinPartDetailsLookup[item.GrinPartId].PONumber,
                        POBalancedQty = grinPartDetailsLookup[item.GrinPartId].POBalancedQty,
                        POOrderedQty = grinPartDetailsLookup[item.GrinPartId].POOrderQty,
                        POUnitPrice = grinPartDetailsLookup[item.GrinPartId].POUnitPrice,
                        Description = grinPartDetailsLookup[item.GrinPartId].ItemDescription,
                        UnitPrice = grinPartDetailsLookup[item.GrinPartId].UnitPrice,
                        ManufactureBatchNumber = grinPartDetailsLookup[item.GrinPartId].ManufactureBatchNumber,
                        UOM = grinPartDetailsLookup[item.GrinPartId].UOM,
                        ExpiryDate = grinPartDetailsLookup[item.GrinPartId].ExpiryDate,
                        ManufactureDate = grinPartDetailsLookup[item.GrinPartId].ManufactureDate,
                        AcceptedQty = grinPartDetailsLookup[item.GrinPartId].AcceptedQty,
                        RejectedQty = grinPartDetailsLookup[item.GrinPartId].RejectedQty,

                        binningLocations = item.binningLocations.Select(loc => new BinningLocationReportDto
                        {
                            GrinNumber = bin.GrinNumber,
                            ItemNumber = item.ItemNumber,
                            Warehouse = loc.Warehouse,
                            Location = loc.Location,
                            Qty = loc.Qty,
                            CreatedBy = loc.CreatedBy,
                            CreatedOn = loc.CreatedOn,
                            LastModifiedBy = loc.LastModifiedBy,
                            LastModifiedOn = loc.LastModifiedOn,



                        }).ToList(),
                    }).ToList(),
                }).ToList();

                serviceResponse.Data = binningListDto;
                serviceResponse.Message = "Returned all Binnings";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetAllBinningWithItems([FromBody] BinningSearchDto binningSearchDto)
        {
            ServiceResponse<IEnumerable<BinningReportDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningReportDto>>();
            try
            {
                var binningList = await _binningRepository.GetAllBinningWithItems(binningSearchDto);

                // Get all the unique GrinPartIds from the iQCList
                var grinPartIds = binningList
                    .SelectMany(iqc => iqc.BinningItems.Select(item => item.GrinPartId))
                    .Distinct()
                    .ToList();

                // Fetch all the required GrinPart details in a single query and store them in a dictionary
                var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsByGrinPartIds(grinPartIds);
                var grinPartDetailsLookup = grinPartDetails.ToDictionary(gp => gp.Id, gp => gp);

                // Use the grinPartDetailsLookup for quick lookups while mapping the data to DTO objects
                var binningListDto = binningList.Select(bin => new BinningReportDto
                {
                    // Map Binning properties here (assuming properties with the same name exist in the DTO)
                    Id = bin.Id,
                    GrinNumber = bin.GrinNumber,
                    Unit = bin.Unit,
                    CreatedBy = bin.CreatedBy,
                    CreatedOn = bin.CreatedOn,
                    LastModifiedBy = bin.LastModifiedBy,
                    LastModifiedOn = bin.LastModifiedOn,
                    // ...

                    BinningItems = bin.BinningItems.Select(item => new BinningItemsReportDto
                    {
                        //Map BinningItem properties here (assuming properties with the same name exist in the DTO)
                        Id = item.Id,
                        ItemNumber = item.ItemNumber,
                        GrinNumber = bin.GrinNumber,
                        ReceivedQty = grinPartDetailsLookup[item.GrinPartId].Qty,
                        MftrItemNumber = grinPartDetailsLookup[item.GrinPartId].MftrItemNumber,
                        PONumber = grinPartDetailsLookup[item.GrinPartId].PONumber,
                        POBalancedQty = grinPartDetailsLookup[item.GrinPartId].POBalancedQty,
                        POOrderedQty = grinPartDetailsLookup[item.GrinPartId].POOrderQty,
                        POUnitPrice = grinPartDetailsLookup[item.GrinPartId].POUnitPrice,
                        Description = grinPartDetailsLookup[item.GrinPartId].ItemDescription,
                        UnitPrice = grinPartDetailsLookup[item.GrinPartId].UnitPrice,
                        ManufactureBatchNumber = grinPartDetailsLookup[item.GrinPartId].ManufactureBatchNumber,
                        UOM = grinPartDetailsLookup[item.GrinPartId].UOM,
                        ExpiryDate = grinPartDetailsLookup[item.GrinPartId].ExpiryDate,
                        ManufactureDate = grinPartDetailsLookup[item.GrinPartId].ManufactureDate,
                        AcceptedQty = grinPartDetailsLookup[item.GrinPartId].AcceptedQty,
                        RejectedQty = grinPartDetailsLookup[item.GrinPartId].RejectedQty,

                        binningLocations = item.binningLocations.Select(loc => new BinningLocationReportDto
                        {
                            GrinNumber = bin.GrinNumber,
                            ItemNumber = item.ItemNumber,
                            Warehouse = loc.Warehouse,
                            Location = loc.Location,
                            Qty = loc.Qty,
                            CreatedBy = loc.CreatedBy,
                            CreatedOn = loc.CreatedOn,
                            LastModifiedBy = loc.LastModifiedBy,
                            LastModifiedOn = loc.LastModifiedOn,



                        }).ToList(),

                    }).ToList(),
                }).ToList();

                serviceResponse.Data = binningListDto;
                serviceResponse.Message = "Returned all BinningWithItems";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBinning(int id, [FromBody] BinningDto binningDto)
        {
            ServiceResponse<BinningDto> serviceResponse = new ServiceResponse<BinningDto>();

            try
            {
                if (binningDto is null)
                {
                    _logger.LogError("Binning details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Update Binning details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid BinningUpdate details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var getBinningDetailById = await _binningRepository.GetBinningDetailsbyId(id);
                if (getBinningDetailById is null)
                {
                    _logger.LogError($"Binning details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Update Grin with id hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }


                var binningItem = _mapper.Map<IEnumerable<BinningItems>>(binningDto.BinningItems);

                var BinningDetail = _mapper.Map<Binning>(binningDto);

                var binningItemdto = binningDto.BinningItems;

                var binningItemList = new List<BinningItems>();

                if (binningItemdto != null)
                {
                    for (int i = 0; i < binningItemdto.Count; i++)
                    {
                        BinningItems binningItems = _mapper.Map<BinningItems>(binningItemdto[i]);
                        binningItems.binningLocations = _mapper.Map<List<BinningLocation>>(binningItemdto[i].binningLocations);
                        binningItemList.Add(binningItems);

                    }
                }
                var updateBinning = _mapper.Map(binningDto, getBinningDetailById);

                updateBinning.BinningItems = binningItemList;



                string result = await _binningRepository.UpdateBinning(updateBinning);
                _logger.LogInfo(result);

                _binningRepository.SaveAsync();
                serviceResponse.Data = null;
                serviceResponse.Message = "Update Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Update Update action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBinning([FromBody] BinningPostDto binningPostDto)
        {
            ServiceResponse<Binning> serviceResponse = new ServiceResponse<Binning>();
            Binning binningDetails = null;
            try
            {
                if (binningPostDto == null)
                {
                    _logger.LogError("Binning details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Binning details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Binning details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }

                var binningDetail = _mapper.Map<Binning>(binningPostDto);
                var binningsItemsDto = binningPostDto.BinningItems;
                var grinNumber = binningDetail.GrinNumber;
                var binningItemList = new List<BinningItems>();

                HttpStatusCode getInvGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode createInv = HttpStatusCode.OK;
                HttpStatusCode getItemMas = HttpStatusCode.OK;
                HttpStatusCode createInvTrans = HttpStatusCode.OK;

                var existingBinningDetails = await _binningRepository.GetExistingBinningDetailsByGrinNo(grinNumber);
                if (existingBinningDetails != null)
                {
                    if (binningsItemsDto != null)
                    {
                        for (int i = 0; i < binningsItemsDto.Count; i++)
                        {

                            BinningItems binningItems = _mapper.Map<BinningItems>(binningsItemsDto[i]);
                            binningItems.BinningId = existingBinningDetails.Id;
                            binningItems.binningLocations = _mapper.Map<List<BinningLocation>>(binningsItemsDto[i].binningLocations);

                            //Updating Binning Status in BinningItem

                            binningItems.IsBinningCompleted = true;
                            await _binningItemsRepository.UpdateBinningItems(binningItems);
                            _binningItemsRepository.SaveAsync();

                            //Updating Binning Status in GrinParts

                            var grinPartsId = binningsItemsDto[i].GrinPartId;
                            var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartsId);
                            grinPartDetails.IsBinningCompleted = true;
                            await _grinPartsRepository.UpdateGrinQty(grinPartDetails);
                            _grinPartsRepository.SaveAsync();

                            //Updating Binning Status in IQCItems

                            var iqcItemDetails = await _iQCConfirmationItemsRepository.GetIQCConfirmationItemsDetailsbyGrinPartId(grinPartsId);
                            iqcItemDetails.IsBinningCompleted = true;
                            await _iQCConfirmationItemsRepository.UpdateIqcItems(iqcItemDetails);
                            _iQCConfirmationItemsRepository.SaveAsync();

                            //Updating Binning Status in Grin

                            var grinPartsBinningStatuscount = await _grinPartsRepository.GetGrinPartsBinningStatusCount(grinPartDetails.GrinsId);

                            if (grinPartsBinningStatuscount == 0)
                            {
                                var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                                grinDetails.IsBinningCompleted = true;
                                await _grinRepository.UpdateGrin(grinDetails);
                                _grinRepository.SaveAsync();

                            }

                            //Updating Binning Status in IQC & Binning

                            var grinBinningStatuscount = await _grinRepository.GetGrinbinningStatusCount(grinNumber);

                            if (grinBinningStatuscount > 0)
                            {
                                var iqcDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                                iqcDetails.IsBinningCompleted = true;
                                await _iQCConfirmationRepository.UpdateIqc(iqcDetails);


                                var binningList = await _binningRepository.GetBinningDetailsByGrinNumber(grinNumber);
                                binningList.IsBinningCompleted = true;
                                await _binningRepository.UpdateBinning(binningList);

                            }
                        }
                    }

                    // Inventory Update Code
                    string grinPartId = "";
                    if (binningsItemsDto != null)
                    {
                        foreach (var binningsItem in binningsItemsDto)
                        {

                            var grinId = binningsItem.GrinPartId;

                            //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                            //"GetItemMasterByItemNumber?", "&ItemNumber=", binningsItem.ItemNumber));

                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();

                            var ItemNumber = binningsItem.ItemNumber;
                            var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                                $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                            request.Headers.Add("Authorization", token);

                            var itemMasterObjectResult = await client.SendAsync(request);
                            if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK) getItemMas = itemMasterObjectResult.StatusCode;
                            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                            dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                            dynamic itemMasterObject = itemMasterObjectData.data;

                            var binningLocations = binningsItem.binningLocations;

                            //int j = 0;
                            //int k = 0;
                            foreach (var location in binningLocations)
                            {
                                //if (j == 0)
                                //{
                                var client1 = _clientFactory.CreateClient();
                                var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                                var itemNo = binningsItem.ItemNumber;
                                var encodedItemNo = Uri.EscapeDataString(itemNo);
                                var grinNo = binningDetail.GrinNumber;
                                var encodedgrinNo = Uri.EscapeDataString(grinNo);
                                var grinPartsIds = binningsItem.GrinPartId;
                                var projectNos = location.ProjectNumber;
                                var encodedprojectNos = Uri.EscapeDataString(projectNos);

                                var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                    $"GetIQCInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                                request1.Headers.Add("Authorization", token1);

                                var inventoryObjectResult = await client1.SendAsync(request1);
                                //if (inventoryObjectResult.StatusCode != HttpStatusCode.OK) getInvGrinId = inventoryObjectResult.StatusCode;

                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                dynamic inventoryObject = inventoryObjectData.data;

                                if (inventoryObjectResult.StatusCode == HttpStatusCode.OK)
                                {
                                    inventoryObject.balance_Quantity = location.Qty;
                                    inventoryObject.warehouse = location.Warehouse;
                                    inventoryObject.location = location.Location;
                                    var json = JsonConvert.SerializeObject(inventoryObject);
                                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                                    var client5 = _clientFactory.CreateClient();
                                    var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                                    "UpdateInventory?id=", inventoryObject.id))
                                    {
                                        Content = data
                                    };
                                    request5.Headers.Add("Authorization", token5);

                                    var response = await client5.SendAsync(request5);

                                    if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;
                                    //    j++;
                                }
                                else
                                {


                                    var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinId);
                                    BinningInventoryDtoPost inventoryObjectNew = new BinningInventoryDtoPost();
                                    inventoryObjectNew.PartNumber = binningsItem.ItemNumber;
                                    inventoryObjectNew.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                    inventoryObjectNew.Description = grinPartsDetails.ItemDescription;
                                    inventoryObjectNew.ProjectNumber = location.ProjectNumber;
                                    inventoryObjectNew.Balance_Quantity = location.Qty;
                                    inventoryObjectNew.Max = itemMasterObject.max;
                                    inventoryObjectNew.Min = itemMasterObject.min;
                                    inventoryObjectNew.UOM = grinPartsDetails.UOM;
                                    inventoryObjectNew.Warehouse = location.Warehouse;
                                    inventoryObjectNew.Location = location.Location;
                                    inventoryObjectNew.GrinNo = binningDetail.GrinNumber;
                                    inventoryObjectNew.GrinPartId = grinId;
                                    inventoryObjectNew.PartType = grinPartsDetails.ItemType; // we have to take parttype from grinparts model;
                                    inventoryObjectNew.ReferenceID = binningDetail.GrinNumber;
                                    inventoryObjectNew.ReferenceIDFrom = "GRIN";
                                    inventoryObjectNew.LotNumber = grinPartsDetails.LotNumber;
                                    var json = JsonConvert.SerializeObject(inventoryObjectNew);
                                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                                    var client6 = _clientFactory.CreateClient();
                                    var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                    "CreateInventory"))
                                    {
                                        Content = data
                                    };
                                    request6.Headers.Add("Authorization", token6);

                                    var response = await client6.SendAsync(request6);
                                    if (response.StatusCode != HttpStatusCode.OK) createInv = response.StatusCode;
                                }




                                //InventoryTranction Update Code

                                var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinId);
                                BinningInventoryTranctionDto inventoryTranctionObjectNew = new BinningInventoryTranctionDto();
                                inventoryTranctionObjectNew.PartNumber = binningsItem.ItemNumber;
                                inventoryTranctionObjectNew.MftrPartNumber = grinPartDetails.MftrItemNumber;
                                inventoryTranctionObjectNew.Description = grinPartDetails.ItemDescription;
                                inventoryTranctionObjectNew.ProjectNumber = location.ProjectNumber;
                                inventoryTranctionObjectNew.Issued_Quantity = location.Qty;
                                inventoryTranctionObjectNew.UOM = grinPartDetails.UOM;
                                inventoryTranctionObjectNew.Warehouse = location.Warehouse;
                                inventoryTranctionObjectNew.From_Location = location.Location;
                                inventoryTranctionObjectNew.TO_Location = location.Location;
                                inventoryTranctionObjectNew.GrinNo = binningDetail.GrinNumber;
                                inventoryTranctionObjectNew.GrinPartId = grinId;
                                inventoryTranctionObjectNew.PartType = grinPartDetails.ItemType; // we have to take parttype from grinparts model;
                                inventoryTranctionObjectNew.ReferenceID = binningDetail.GrinNumber;
                                inventoryTranctionObjectNew.ReferenceIDFrom = "Binning";

                                var jsons = JsonConvert.SerializeObject(inventoryTranctionObjectNew);
                                var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                                //var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), datas);
                                var client7 = _clientFactory.CreateClient();
                                var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = datas
                                };
                                request7.Headers.Add("Authorization", token7);

                                var responses = await client7.SendAsync(request7);

                                if (responses.StatusCode != HttpStatusCode.OK) createInvTrans = responses.StatusCode;

                            }
                        }
                    }
                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && createInv == HttpStatusCode.OK && createInvTrans == HttpStatusCode.OK
                                                                                                                                    && getItemMas == HttpStatusCode.OK)
                    {
                        _iQCConfirmationRepository.SaveAsync();
                        _binningRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create Binning action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = binningDetails;
                    serviceResponse.Message = "BinningWithItems Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    if (binningsItemsDto != null)
                    {
                        for (int i = 0; i < binningsItemsDto.Count; i++)
                        {

                            BinningItems binningItems = _mapper.Map<BinningItems>(binningsItemsDto[i]);
                            binningItems.binningLocations = _mapper.Map<List<BinningLocation>>(binningsItemsDto[i].binningLocations);
                            binningItems.IsBinningCompleted = true;
                            binningItemList.Add(binningItems);

                            //Updating Binning Status in GrinParts

                            var grinPartsId = binningsItemsDto[i].GrinPartId;
                            var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartsId);
                            grinPartDetails.IsBinningCompleted = true;
                            await _grinPartsRepository.UpdateGrinQty(grinPartDetails);
                            _grinPartsRepository.SaveAsync();

                            //Updating Binning Status in IQCItems

                            var iqcItemDetails = await _iQCConfirmationItemsRepository.GetIQCConfirmationItemsDetailsbyGrinPartId(grinPartsId);
                            iqcItemDetails.IsBinningCompleted = true;
                            await _iQCConfirmationItemsRepository.UpdateIqcItems(iqcItemDetails);
                            _iQCConfirmationItemsRepository.SaveAsync();
                        }
                    }


                    // Inventory Update Code
                    string grinPartId = "";
                    if (binningsItemsDto != null)
                    {
                        foreach (var binningsItem in binningsItemsDto)
                        {


                            var grinId = binningsItem.GrinPartId;

                            //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                            //"GetItemMasterByItemNumber?", "&ItemNumber=", binningsItem.ItemNumber));
                            var client = _clientFactory.CreateClient();
                            var token = HttpContext.Request.Headers["Authorization"].ToString();

                            var ItemNumber = binningsItem.ItemNumber;
                            var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                            var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                                $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                            request.Headers.Add("Authorization", token);

                            var itemMasterObjectResult = await client.SendAsync(request);

                            if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK) getItemMas = itemMasterObjectResult.StatusCode;
                            var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                            dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                            dynamic itemMasterObject = itemMasterObjectData.data;

                            var binningLocations = binningsItem.binningLocations;

                            //int j = 0;
                            //int k = 0;
                            foreach (var location in binningLocations)
                            {

                                var client1 = _clientFactory.CreateClient();
                                var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                                var itemNo = binningsItem.ItemNumber;
                                var encodedItemNo = Uri.EscapeDataString(itemNo);
                                var grinNo = binningDetail.GrinNumber;
                                var encodedgrinNo = Uri.EscapeDataString(grinNo);
                                var grinPartsIds = binningsItem.GrinPartId;
                                var projectNos = location.ProjectNumber;
                                var encodedprojectNos = Uri.EscapeDataString(projectNos);

                                var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                    $"GetIQCInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                                request1.Headers.Add("Authorization", token1);

                                var inventoryObjectResult = await client1.SendAsync(request1);

                                if (inventoryObjectResult.StatusCode == HttpStatusCode.OK)
                                {

                                    var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                    dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                    dynamic inventoryObject = inventoryObjectData.data;

                                    inventoryObject.balance_Quantity = location.Qty;
                                    inventoryObject.warehouse = location.Warehouse;
                                    inventoryObject.location = location.Location;
                                    var json = JsonConvert.SerializeObject(inventoryObject);
                                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                                    var client5 = _clientFactory.CreateClient();
                                    var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                                    "UpdateInventory?id=", inventoryObject.id))
                                    {
                                        Content = data
                                    };
                                    request5.Headers.Add("Authorization", token5);

                                    var response = await client5.SendAsync(request5);
                                    if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

                                }
                                else
                                {
                                    var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinId);
                                    BinningInventoryDtoPost inventoryObjectNew = new BinningInventoryDtoPost();
                                    inventoryObjectNew.PartNumber = binningsItem.ItemNumber;
                                    inventoryObjectNew.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                    inventoryObjectNew.Description = grinPartsDetails.ItemDescription;
                                    inventoryObjectNew.ProjectNumber = location.ProjectNumber;
                                    inventoryObjectNew.Balance_Quantity = location.Qty;
                                    inventoryObjectNew.Max = itemMasterObject.max;
                                    inventoryObjectNew.Min = itemMasterObject.min;
                                    inventoryObjectNew.UOM = grinPartsDetails.UOM;
                                    inventoryObjectNew.Warehouse = location.Warehouse;
                                    inventoryObjectNew.Location = location.Location;
                                    inventoryObjectNew.GrinNo = binningDetail.GrinNumber;
                                    inventoryObjectNew.GrinPartId = grinId;
                                    inventoryObjectNew.PartType = grinPartsDetails.ItemType; // we have to take parttype from grinparts model;
                                    inventoryObjectNew.ReferenceID = binningDetail.GrinNumber;
                                    inventoryObjectNew.ReferenceIDFrom = "GRIN";
                                    inventoryObjectNew.LotNumber = grinPartsDetails.LotNumber;
                                    var json = JsonConvert.SerializeObject(inventoryObjectNew);
                                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                                    var client6 = _clientFactory.CreateClient();
                                    var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                    var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                    "CreateInventory"))
                                    {
                                        Content = data
                                    };
                                    request6.Headers.Add("Authorization", token6);

                                    var response = await client6.SendAsync(request6);
                                    if (response.StatusCode != HttpStatusCode.OK) createInv = response.StatusCode;
                                }

                                //InventoryTranction Update Code

                                var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinId);
                                BinningInventoryTranctionDto inventoryTranctionObjectNew = new BinningInventoryTranctionDto();
                                inventoryTranctionObjectNew.PartNumber = binningsItem.ItemNumber;
                                inventoryTranctionObjectNew.MftrPartNumber = grinPartDetails.MftrItemNumber;
                                inventoryTranctionObjectNew.Description = grinPartDetails.ItemDescription;
                                inventoryTranctionObjectNew.ProjectNumber = location.ProjectNumber;
                                inventoryTranctionObjectNew.Issued_Quantity = location.Qty;
                                inventoryTranctionObjectNew.UOM = grinPartDetails.UOM;
                                inventoryTranctionObjectNew.Warehouse = location.Warehouse;
                                inventoryTranctionObjectNew.From_Location = location.Location;
                                inventoryTranctionObjectNew.TO_Location = location.Location;
                                inventoryTranctionObjectNew.GrinNo = binningDetail.GrinNumber;
                                inventoryTranctionObjectNew.GrinPartId = grinId;
                                inventoryTranctionObjectNew.PartType = grinPartDetails.ItemType; // we have to take parttype from grinparts model;
                                inventoryTranctionObjectNew.ReferenceID = binningDetail.GrinNumber;
                                inventoryTranctionObjectNew.ReferenceIDFrom = "Binning";

                                var jsons = JsonConvert.SerializeObject(inventoryTranctionObjectNew);
                                var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                                //var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), datas);
                                var client7 = _clientFactory.CreateClient();
                                var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                                "CreateInventoryTranction"))
                                {
                                    Content = datas
                                };
                                request7.Headers.Add("Authorization", token7);

                                var responses = await client7.SendAsync(request7);

                                if (responses.StatusCode != HttpStatusCode.OK) createInvTrans = responses.StatusCode;
                            }
                        }
                    }

                    //Updating Binning Status in Binning Main Level

                    binningDetail.BinningItems = binningItemList;
                    binningDetail.IsBinningCompleted = true;
                    await _binningRepository.CreateBinning(binningDetail);

                    //Updating Binning Status in Grin Main Level

                    var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                    grinDetails.IsBinningCompleted = true;
                    await _grinRepository.UpdateGrin(grinDetails);

                    //Updating Binning Status in Iqc Main Level

                    var iqcDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                    iqcDetails.IsBinningCompleted = true;
                    await _iQCConfirmationRepository.UpdateIqc(iqcDetails);

                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && createInv == HttpStatusCode.OK && getItemMas == HttpStatusCode.OK
                                                                                                                            && createInvTrans == HttpStatusCode.OK)
                    {
                        _grinRepository.SaveAsync();
                        _iQCConfirmationRepository.SaveAsync();
                        _binningRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create Binning action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = binningDetails;
                    serviceResponse.Message = "Binning Successfully Created";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Something went wrong inside Create Binning action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal Server Error");


            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetBinningDetailsbyId(int id)
        {
            ServiceResponse<BinningDto> serviceResponse = new ServiceResponse<BinningDto>();

            try
            {
                var binningsById = await _binningRepository.GetBinningDetailsbyId(id);
                if (binningsById == null)
                {
                    _logger.LogError($"Binning details with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Binning details hasn't been found";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }
                else
                {
                    _logger.LogInfo($"Returned Binnings with id: {id}");
                    List<BinningItemsDto> binningItemList = new List<BinningItemsDto>();
                    var binningGrinNo = binningsById.GrinNumber;
                    var binningItemDetails = binningsById.BinningItems;
                    var grinDetailsbyGrinNo = await _grinRepository.GetGrinByGrinNo(binningGrinNo);
                    var binningDetailsDto = _mapper.Map<BinningDto>(grinDetailsbyGrinNo);

                    if (binningsById.BinningItems != null)
                    {
                        foreach (var binItem in binningsById.BinningItems)
                        {
                            foreach (var grinItem in grinDetailsbyGrinNo.GrinParts)
                            {
                                if (binItem.GrinPartId == grinItem.Id)
                                {
                                    BinningItemsDto binningItemDtos = _mapper.Map<BinningItemsDto>(grinItem);
                                    var binningDetails = _mapper.Map<List<BinningLocationDto>>(binItem.binningLocations);
                                    binningItemDtos.binningLocations = binningDetails;
                                    binningItemDtos.ReceivedQty = grinItem.Qty;
                                    binningItemDtos.AcceptedQty = grinItem.AcceptedQty;
                                    binningItemDtos.RejectedQty = grinItem.RejectedQty;
                                    binningItemList.Add(binningItemDtos);
                                    break;
                                }
                            }
                        }
                    }

                    //if (grinDetailsbyGrinNo.GrinParts.Count != 0)
                    //{
                    //    foreach (var grinDetails in grinDetailsbyGrinNo.GrinParts)
                    //    {
                    //        BinningItemsDto binningItemDtos = _mapper.Map<BinningItemsDto>(grinDetails);
                    //        var binningDetails = _mapper.Map<List<BinningLocationDto>>(binningItemDtos.binningLocations);
                    //        binningItemDtos.binningLocations = binningDetails;
                    //        binningItemDtos.ReceivedQty = grinDetails.Qty;
                    //        binningItemDtos.AcceptedQty = grinDetails.AcceptedQty;
                    //        binningItemDtos.RejectedQty = grinDetails.RejectedQty;
                    //        binningItemList.Add(binningItemDtos);
                    //    }
                    //}
                    //if (grinDetailsbyGrinNo.GrinParts.Count != 0)
                    //{
                    //    foreach (var grinDetails in grinDetailsbyGrinNo.GrinParts)
                    //    {
                    //        BinningItemsDto binningItemDtos = _mapper.Map<BinningItemsDto>(grinDetails);

                    //        // Fetch BinningLocations for this BinningItems
                    //        List<BinningLocationDto> binningLocationDtos = new List<BinningLocationDto>();
                    //        foreach (var binningLocation in binningItemDetails[0].binningLocations)
                    //        {
                    //            BinningLocationDto binningLocationDto = _mapper.Map<BinningLocationDto>(binningLocation);
                    //            binningLocationDtos.Add(binningLocationDto);
                    //        }

                    //        binningItemDtos.binningLocations = binningLocationDtos;

                    //        binningItemDtos.ReceivedQty = grinDetails.Qty;
                    //        binningItemDtos.AcceptedQty = grinDetails.AcceptedQty;
                    //        binningItemDtos.RejectedQty = grinDetails.RejectedQty;

                    //        binningItemList.Add(binningItemDtos);
                    //    }
                    //}

                    binningDetailsDto.BinningItems = binningItemList;
                    serviceResponse.Data = binningDetailsDto;
                    serviceResponse.Message = $"Returned BinningbyId Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside BinningById action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBinning(int id)
        {
            ServiceResponse<BinningDto> serviceResponse = new ServiceResponse<BinningDto>();

            try
            {
                var binningDetailById = await _binningRepository.GetBinningDetailsbyId(id);
                if (binningDetailById == null)
                {
                    _logger.LogError($"deleteBinning with id: {id}, hasn't been found in db.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = $"Binning with id: {id}, hasn't been found in db.";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(serviceResponse);
                }

                binningDetailById.IsDeleted = true;
                string result = await _binningRepository.UpdateBinning(binningDetailById);
                serviceResponse.Data = null;
                serviceResponse.Message = "Delete Successfully";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Delete Binning action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBinningItems()
        {
            ServiceResponse<IEnumerable<BinningItemsDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningItemsDto>>();

            try
            {
                var getAllBinningItems = await _binningItemsRepository.GetAllBinningItems();


                _logger.LogInfo("Returned all BinningItems details");
                var result = _mapper.Map<IEnumerable<BinningItemsDto>>(getAllBinningItems);
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
                serviceResponse.Message = "Inter server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveBinningIdNameList()
        {
            ServiceResponse<IEnumerable<BinningIdNameListDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningIdNameListDto>>();
            try
            {
                var listOfActiveBinningName = await _binningRepository.GetAllActiveBinningNameList();

                var result = _mapper.Map<IEnumerable<BinningIdNameListDto>>(listOfActiveBinningName);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned All ActiveBinningIdNameList";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetAllActiveBinningIdNameList action: {ex.Message}";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateBinningItems([FromBody] BinningSaveDto binningSaveDto)
        {
            ServiceResponse<BinningSaveDto> serviceResponse = new ServiceResponse<BinningSaveDto>();
            try
            {
                if (binningSaveDto == null)
                {
                    _logger.LogError("Binning details object sent from client is null.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Binning details object is null";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid Binning details object sent from client.");
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Invalid model object";
                    serviceResponse.Success = false;
                    serviceResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(serviceResponse);
                }
                var binningDetail = new Binning();

                binningDetail.GrinNumber = binningSaveDto.GrinNumber;
                binningDetail.VendorId = binningSaveDto.VendorId;
                binningDetail.VendorName = binningSaveDto.VendorName;
                binningDetail.VendorNumber = binningSaveDto.VendorNumber;

                List<BinningItems>? binningItemsEntityList = new List<BinningItems>();
                BinningItems binningItemsEntity = new BinningItems();
                binningItemsEntity.ItemNumber = binningSaveDto.BinningItems.ItemNumber;
                binningItemsEntity.GrinPartId = binningSaveDto.BinningItems.GrinPartId;
                binningItemsEntity.binningLocations = _mapper.Map<List<BinningLocation>>(binningSaveDto.BinningItems.binningLocations);
                binningItemsEntityList.Add(binningItemsEntity);
                binningDetail.BinningItems = binningItemsEntityList;

                HttpStatusCode getInvGrinId = HttpStatusCode.OK;
                HttpStatusCode updateInv = HttpStatusCode.OK;
                HttpStatusCode createInv = HttpStatusCode.OK;
                HttpStatusCode getItemMas = HttpStatusCode.OK;
                HttpStatusCode createInvTrans = HttpStatusCode.OK;

                var binningsItemsDto = binningSaveDto.BinningItems;
                var binningItems = binningDetail.BinningItems[0];
                var grinNumber = binningDetail.GrinNumber;
                var existingBinningDetails = await _binningRepository.GetExistingBinningDetailsByGrinNo(grinNumber);

                if (existingBinningDetails != null)
                {
                    //Updating Binning Status in BinningItem

                    binningItems.BinningId = existingBinningDetails.Id;
                    binningItems.IsBinningCompleted = true;
                    await _binningItemsRepository.CreateBinningItems(binningItems);
                    _binningItemsRepository.SaveAsync();

                    //Updating Binning Status in GrinParts

                    var grinPartsId = binningsItemsDto.GrinPartId;
                    var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartsId);
                    grinPartDetails.IsBinningCompleted = true;
                    await _grinPartsRepository.UpdateGrinQty(grinPartDetails);
                    _grinPartsRepository.SaveAsync();

                    //Updating Binning Status in IQCItems

                    var iqcItemDetails = await _iQCConfirmationItemsRepository.GetIQCConfirmationItemsDetailsbyGrinPartId(grinPartsId);
                    iqcItemDetails.IsBinningCompleted = true;
                    await _iQCConfirmationItemsRepository.UpdateIqcItems(iqcItemDetails);
                    _iQCConfirmationItemsRepository.SaveAsync();

                    //Updating Binning Status in Grin

                    var grinPartsBinningStatuscount = await _grinPartsRepository.GetGrinPartsBinningStatusCount(grinPartDetails.GrinsId);

                    if (grinPartsBinningStatuscount == 0)
                    {
                        var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                        grinDetails.IsBinningCompleted = true;
                        await _grinRepository.UpdateGrin(grinDetails);
                        _grinRepository.SaveAsync();
                    }

                    //Updating Binning Status in IQC & Binning

                    var grinBinningStatuscount = await _grinRepository.GetGrinbinningStatusCount(grinNumber);

                    if (grinBinningStatuscount > 0)
                    {
                        var iqcDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                        iqcDetails.IsBinningCompleted = true;
                        await _iQCConfirmationRepository.UpdateIqc(iqcDetails);

                        var binningDetails = await _binningRepository.GetBinningDetailsByGrinNumber(grinNumber);
                        binningDetails.IsBinningCompleted = true;
                        await _binningRepository.UpdateBinning(binningDetails);
                    }


                    // Inventory Update Code
                    string grinPartId = "";

                    if (binningsItemsDto != null)
                    {

                        var grinId = binningsItemsDto.GrinPartId;
                        //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                        //    "GetItemMasterByItemNumber?", "&ItemNumber=", binningsItemsDto.ItemNumber));

                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = binningsItemsDto.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterObjectResult = await client.SendAsync(request);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK) getItemMas = itemMasterObjectResult.StatusCode;
                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                        dynamic itemMasterObject = itemMasterObjectData.data;

                        var binningLocations = binningsItemsDto.binningLocations;
                        //int j = 0;
                        //int k = 0;
                        foreach (var location in binningLocations)
                        {



                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                            var itemNo = binningsItemsDto.ItemNumber;
                            var encodedItemNo = Uri.EscapeDataString(itemNo);
                            var grinNo = binningDetail.GrinNumber;
                            var encodedgrinNo = Uri.EscapeDataString(grinNo);
                            var grinPartsIds = binningsItemsDto.GrinPartId;
                            var projectNos = location.ProjectNumber;
                            var encodedprojectNos = Uri.EscapeDataString(projectNos);

                            var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                $"GetIQCInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                            request1.Headers.Add("Authorization", token1);

                            var inventoryObjectResult = await client1.SendAsync(request1);
                            if (inventoryObjectResult.StatusCode == HttpStatusCode.OK)
                            {
                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                dynamic inventoryObject = inventoryObjectData.data;

                                inventoryObject.balance_Quantity = location.Qty;
                                inventoryObject.warehouse = location.Warehouse;
                                inventoryObject.location = location.Location;
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");

                                var client5 = _clientFactory.CreateClient();
                                var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                                "UpdateInventory?id=", inventoryObject.id))
                                {
                                    Content = data
                                };
                                request5.Headers.Add("Authorization", token5);

                                var response = await client5.SendAsync(request5);
                                if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;

                            }
                            else
                            {

                                BinningInventoryDtoPost inventoryObjectNew = new BinningInventoryDtoPost();
                                inventoryObjectNew.PartNumber = grinPartDetails.ItemNumber;
                                inventoryObjectNew.MftrPartNumber = grinPartDetails.MftrItemNumber;
                                inventoryObjectNew.Description = grinPartDetails.ItemDescription;
                                inventoryObjectNew.ProjectNumber = location.ProjectNumber;
                                inventoryObjectNew.Balance_Quantity = location.Qty;
                                inventoryObjectNew.Max = itemMasterObject.max;
                                inventoryObjectNew.Min = itemMasterObject.min;
                                inventoryObjectNew.UOM = grinPartDetails.UOM;
                                inventoryObjectNew.Warehouse = location.Warehouse;
                                inventoryObjectNew.Location = location.Location;
                                inventoryObjectNew.GrinNo = binningDetail.GrinNumber;
                                inventoryObjectNew.GrinPartId = grinId;
                                inventoryObjectNew.LotNumber = grinPartDetails.LotNumber;
                                inventoryObjectNew.PartType = grinPartDetails.ItemType; // we have to take parttype from grinparts model;
                                inventoryObjectNew.ReferenceID = binningDetail.GrinNumber;
                                inventoryObjectNew.ReferenceIDFrom = "GRIN";

                                var json = JsonConvert.SerializeObject(inventoryObjectNew);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");

                                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);
                                var client6 = _clientFactory.CreateClient();
                                var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                "CreateInventory"))
                                {
                                    Content = data
                                };
                                request6.Headers.Add("Authorization", token6);
                                var response = await client6.SendAsync(request6);
                                if (response.StatusCode != HttpStatusCode.OK) createInv = response.StatusCode;
                            }

                            //InventoryTranction Update Code

                            BinningInventoryTranctionDto inventoryTranctionObjectNew = new BinningInventoryTranctionDto();
                            inventoryTranctionObjectNew.PartNumber = binningsItemsDto.ItemNumber;
                            inventoryTranctionObjectNew.MftrPartNumber = grinPartDetails.MftrItemNumber;
                            inventoryTranctionObjectNew.Description = grinPartDetails.ItemDescription;
                            inventoryTranctionObjectNew.ProjectNumber = location.ProjectNumber;
                            inventoryTranctionObjectNew.Issued_Quantity = location.Qty;
                            inventoryTranctionObjectNew.UOM = grinPartDetails.UOM;
                            inventoryTranctionObjectNew.Warehouse = location.Warehouse;
                            inventoryTranctionObjectNew.From_Location = location.Location;
                            inventoryTranctionObjectNew.TO_Location = location.Location;
                            inventoryTranctionObjectNew.GrinNo = binningDetail.GrinNumber;
                            inventoryTranctionObjectNew.GrinPartId = grinId;
                            inventoryTranctionObjectNew.PartType = grinPartDetails.ItemType; // we have to take parttype from grinparts model;
                            inventoryTranctionObjectNew.ReferenceID = binningDetail.GrinNumber;
                            inventoryTranctionObjectNew.ReferenceIDFrom = "Binning";

                            var jsons = JsonConvert.SerializeObject(inventoryTranctionObjectNew);
                            var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                            //var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), datas);

                            var client7 = _clientFactory.CreateClient();
                            var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranction"))
                            {
                                Content = datas
                            };
                            request7.Headers.Add("Authorization", token7);

                            var responses = await client7.SendAsync(request7);
                            if (responses.StatusCode != HttpStatusCode.OK) createInvTrans = responses.StatusCode;
                        }

                    }
                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && createInv == HttpStatusCode.OK && getItemMas == HttpStatusCode.OK
                                                                                                                                && createInvTrans == HttpStatusCode.OK)
                    {
                        _iQCConfirmationRepository.SaveAsync();
                        _binningRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create BinningWithItems action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = null;
                    serviceResponse.Message = "BinningItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
                else
                {
                    //Updating Binning Status in BinningItem

                    binningItems.IsBinningCompleted = true;
                    binningDetail.BinningItems = new List<BinningItems> { binningItems };
                    await _binningRepository.CreateBinning(binningDetail);
                    _binningRepository.SaveAsync();

                    //Updating Binning Status in GrinParts

                    var grinPartsId = binningsItemsDto.GrinPartId;
                    var grinPartDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinPartsId);
                    grinPartDetails.IsBinningCompleted = true;
                    await _grinPartsRepository.UpdateGrinQty(grinPartDetails);
                    _grinPartsRepository.SaveAsync();

                    //Updating Binning Status in IQCItems

                    var iqcItemDetails = await _iQCConfirmationItemsRepository.GetIQCConfirmationItemsDetailsbyGrinPartId(grinPartsId);
                    iqcItemDetails.IsBinningCompleted = true;
                    await _iQCConfirmationItemsRepository.UpdateIqcItems(iqcItemDetails);
                    _iQCConfirmationItemsRepository.SaveAsync();


                    //Updating Binning Status in Grin

                    var grinPartsBinningStatuscount = await _grinPartsRepository.GetGrinPartsBinningStatusCount(grinPartDetails.GrinsId);

                    if (grinPartsBinningStatuscount == 0)
                    {
                        var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                        grinDetails.IsBinningCompleted = true;
                        await _grinRepository.UpdateGrin(grinDetails);
                        _grinRepository.SaveAsync();
                    }

                    //Updating Binning Status in IQC & Binning

                    var grinBinningStatuscount = await _grinRepository.GetGrinbinningStatusCount(grinNumber);

                    if (grinBinningStatuscount > 0)
                    {
                        var iqcDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                        iqcDetails.IsBinningCompleted = true;
                        await _iQCConfirmationRepository.UpdateIqc(iqcDetails);
                        _iQCConfirmationRepository.SaveAsync();

                        var binningDetails = await _binningRepository.GetBinningDetailsByGrinNumber(grinNumber);
                        binningDetails.IsBinningCompleted = true;
                        await _binningRepository.UpdateBinning(binningDetails);
                        _binningRepository.SaveAsync();
                    }

                    // Inventory Update Code
                    string grinPartId = "";

                    if (binningsItemsDto != null)
                    {

                        var grinId = binningsItemsDto.GrinPartId;

                        //var itemMasterObjectResult = await _httpClient.GetAsync(string.Concat(_config["ItemMasterEnggAPI"],
                        //    "GetItemMasterByItemNumber?", "&ItemNumber=", binningsItemsDto.ItemNumber));

                        var client = _clientFactory.CreateClient();
                        var token = HttpContext.Request.Headers["Authorization"].ToString();

                        var ItemNumber = binningsItemsDto.ItemNumber;
                        var encodedItemNumber = Uri.EscapeDataString(ItemNumber);

                        var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["ItemMasterEnggAPI"],
                            $"GetItemMasterByItemNumber?ItemNumber={encodedItemNumber}"));
                        request.Headers.Add("Authorization", token);

                        var itemMasterObjectResult = await client.SendAsync(request);
                        if (itemMasterObjectResult.StatusCode != HttpStatusCode.OK) getItemMas = itemMasterObjectResult.StatusCode;
                        var itemMasterObjectString = await itemMasterObjectResult.Content.ReadAsStringAsync();
                        dynamic itemMasterObjectData = JsonConvert.DeserializeObject(itemMasterObjectString);
                        dynamic itemMasterObject = itemMasterObjectData.data;

                        var binningLocations = binningsItemsDto.binningLocations;
                        //int j = 0;
                        //int k = 0;
                        foreach (var location in binningLocations)
                        {



                            var client1 = _clientFactory.CreateClient();
                            var token1 = HttpContext.Request.Headers["Authorization"].ToString();

                            var itemNo = binningsItemsDto.ItemNumber;
                            var encodedItemNo = Uri.EscapeDataString(itemNo);
                            var grinNo = binningDetail.GrinNumber;
                            var encodedgrinNo = Uri.EscapeDataString(grinNo);
                            var grinPartsIds = binningsItemsDto.GrinPartId;
                            var projectNos = location.ProjectNumber;
                            var encodedprojectNos = Uri.EscapeDataString(projectNos);

                            var request1 = new HttpRequestMessage(HttpMethod.Get, string.Concat(_config["InventoryAPI"],
                                $"GetIQCInventoryDetailsByGrinNoandGrinId?GrinNo={encodedgrinNo}&GrinPartsId={grinPartsIds}&ItemNumber={encodedItemNo}&ProjectNumber={encodedprojectNos}"));
                            request1.Headers.Add("Authorization", token1);

                            var inventoryObjectResult = await client1.SendAsync(request1);
                            if (inventoryObjectResult.StatusCode == HttpStatusCode.OK)
                            {
                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                dynamic inventoryObject = inventoryObjectData.data;

                                inventoryObject.balance_Quantity = location.Qty;
                                inventoryObject.warehouse = location.Warehouse;
                                inventoryObject.location = location.Location;
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");

                                var client5 = _clientFactory.CreateClient();
                                var token5 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request5 = new HttpRequestMessage(HttpMethod.Put, string.Concat(_config["InventoryAPI"],
                                "UpdateInventory?id=", inventoryObject.id))
                                {
                                    Content = data
                                };
                                request5.Headers.Add("Authorization", token5);

                                var response = await client5.SendAsync(request5);
                                if (response.StatusCode != HttpStatusCode.OK) updateInv = response.StatusCode;
                                
                            }
                            else
                            {
                                BinningInventoryDtoPost inventoryObjectNew = new BinningInventoryDtoPost();
                                inventoryObjectNew.PartNumber = binningsItemsDto.ItemNumber;
                                inventoryObjectNew.MftrPartNumber = grinPartDetails.MftrItemNumber;
                                inventoryObjectNew.Description = grinPartDetails.ItemDescription;
                                inventoryObjectNew.ProjectNumber = location.ProjectNumber;
                                inventoryObjectNew.Balance_Quantity = location.Qty;
                                inventoryObjectNew.Max = itemMasterObject.max;
                                inventoryObjectNew.Min = itemMasterObject.min;
                                inventoryObjectNew.UOM = grinPartDetails.UOM;
                                inventoryObjectNew.Warehouse = location.Warehouse;
                                inventoryObjectNew.Location = location.Location;
                                inventoryObjectNew.GrinNo = binningDetail.GrinNumber;
                                inventoryObjectNew.GrinPartId = grinId;
                                inventoryObjectNew.LotNumber = grinPartDetails.LotNumber;
                                inventoryObjectNew.PartType = grinPartDetails.ItemType; // we have to take parttype from grinparts model;
                                inventoryObjectNew.ReferenceID = binningDetail.GrinNumber;
                                inventoryObjectNew.ReferenceIDFrom = "GRIN";

                                var json = JsonConvert.SerializeObject(inventoryObjectNew);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");

                                //var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);

                                var client6 = _clientFactory.CreateClient();
                                var token6 = HttpContext.Request.Headers["Authorization"].ToString();
                                var request6 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryAPI"],
                                "CreateInventory"))
                                {
                                    Content = data
                                };
                                request6.Headers.Add("Authorization", token6);

                                var response = await client6.SendAsync(request6);
                                if (response.StatusCode != HttpStatusCode.OK) createInv = response.StatusCode;
                            }

                            //InventoryTranction Update Code

                            BinningInventoryTranctionDto inventoryTranctionObjectNew = new BinningInventoryTranctionDto();
                            inventoryTranctionObjectNew.PartNumber = binningsItemsDto.ItemNumber;
                            inventoryTranctionObjectNew.MftrPartNumber = grinPartDetails.MftrItemNumber;
                            inventoryTranctionObjectNew.Description = grinPartDetails.ItemDescription;
                            inventoryTranctionObjectNew.ProjectNumber = location.ProjectNumber;
                            inventoryTranctionObjectNew.Issued_Quantity = location.Qty;
                            inventoryTranctionObjectNew.UOM = grinPartDetails.UOM;
                            inventoryTranctionObjectNew.Warehouse = location.Warehouse;
                            inventoryTranctionObjectNew.From_Location = location.Location;
                            inventoryTranctionObjectNew.TO_Location = location.Location;
                            inventoryTranctionObjectNew.GrinNo = binningDetail.GrinNumber;
                            inventoryTranctionObjectNew.GrinPartId = grinId;
                            inventoryTranctionObjectNew.PartType = grinPartDetails.ItemType; // we have to take parttype from grinparts model;
                            inventoryTranctionObjectNew.ReferenceID = Convert.ToString(grinId);
                            inventoryTranctionObjectNew.ReferenceIDFrom = "Binning";

                            var jsons = JsonConvert.SerializeObject(inventoryTranctionObjectNew);
                            var datas = new StringContent(jsons, Encoding.UTF8, "application/json");
                            //var responses = await _httpClient.PostAsync(string.Concat(_config["InventoryTranctionAPI"], "CreateInventoryTranction"), datas);
                            var client7 = _clientFactory.CreateClient();
                            var token7 = HttpContext.Request.Headers["Authorization"].ToString();
                            var request7 = new HttpRequestMessage(HttpMethod.Post, string.Concat(_config["InventoryTranctionAPI"],
                            "CreateInventoryTranction"))
                            {
                                Content = datas
                            };
                            request7.Headers.Add("Authorization", token7);

                            var responses = await client7.SendAsync(request7);
                            if (responses.StatusCode != HttpStatusCode.OK) createInvTrans = responses.StatusCode;
                        }

                    }
                    if (getInvGrinId == HttpStatusCode.OK && updateInv == HttpStatusCode.OK && createInv == HttpStatusCode.OK && getItemMas == HttpStatusCode.OK
                                                                                                                                 && createInvTrans == HttpStatusCode.OK)
                    {
                        _iQCConfirmationRepository.SaveAsync();
                        _binningRepository.SaveAsync();
                    }
                    else
                    {
                        _logger.LogError($"Something went wrong inside Create BinningWithItems action: Other Service Calling");
                        serviceResponse.Data = null;
                        serviceResponse.Message = "Saving Failed";
                        serviceResponse.Success = false;
                        serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                        return StatusCode(500, serviceResponse);
                    }

                    serviceResponse.Data = null;
                    serviceResponse.Message = "Binning and BinningItems Created Successfully";
                    serviceResponse.Success = true;
                    serviceResponse.StatusCode = HttpStatusCode.OK;
                    return Ok(serviceResponse);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"Something went wrong inside Create Binning action: {ex.Message}");
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal server error";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, "Internal Server Error");


            }
        }

        [HttpPost]
        public async Task<IActionResult> GetListOfBinningQtyByItemNoListByProjectNo(string projectNo, string itemNumber)
        {
            ServiceResponse<IEnumerable<BinningQuantityDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningQuantityDto>>();
            try
            {
                var revNumberDetailsbyPONumber = await _binningLocationRepository.GetListOfBinningQtyByItemNoListByProjectNo(projectNo, itemNumber);

            
                var result = _mapper.Map<IEnumerable<BinningQuantityDto>>(revNumberDetailsbyPONumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned  BinningQty  By ProjectNo and ItemNo List";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetListOfBinningQtyByItemNoListByProjectNo action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetListOfBinningQtyByItemNoListByMultipleProjectNo(coverageBinningByMultipleProjectDto coverageBinningByMultipleProjectDto)
        {
            ServiceResponse<IEnumerable<BinningQuantityDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningQuantityDto>>();
            try
            {
                var revNumberDetailsbyPONumber = await _binningLocationRepository.GetListOfBinningQtyByItemNoListByMultipleProjectNo(coverageBinningByMultipleProjectDto.ItemNumber,
                                                                                                                                        coverageBinningByMultipleProjectDto.projectNo);


                var result = _mapper.Map<IEnumerable<BinningQuantityDto>>(revNumberDetailsbyPONumber);
                serviceResponse.Data = result;
                serviceResponse.Message = "Returned  BinningQty  By ProjectNo and ItemNo List";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Ok(serviceResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                serviceResponse.Data = null;
                serviceResponse.Message = $"Something went wrong inside GetListOfBinningQtyByItemNoListByProjectNo action";
                serviceResponse.Success = false;
                serviceResponse.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(500, serviceResponse);
            }
        }

    }
}