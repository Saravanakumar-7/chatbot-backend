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

namespace Tips.Grin.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BinningController : ControllerBase
    {
        private IBinningRepository _binningRepository;
        private IBinningItemsRepository _binningItemsRepository;
        private ILoggerManager _logger;
        private IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private IGrinRepository _grinRepository;
        private IGrinPartsRepository _grinPartsRepository;
        private IIQCConfirmationRepository _iQCConfirmationRepository;
        public BinningController(IIQCConfirmationRepository iQCConfirmationRepository,IGrinPartsRepository grinPartsRepository, IGrinRepository grinRepository,IBinningRepository binningRepository, IBinningItemsRepository binningItemsRepository,ILoggerManager logger, IMapper mapper, HttpClient httpClient, IConfiguration config)
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
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBinningDetails([FromQuery] PagingParameter pagingParameter ,[FromQuery] SearchParams searchParams)
        {
            ServiceResponse<IEnumerable<BinningDto>> serviceResponse = new ServiceResponse<IEnumerable<BinningDto>>();

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
                var result = _mapper.Map<IEnumerable<BinningDto>>(getAllBinnings);
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
                serviceResponse.Message = $"Something went wrong,try again";
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
        public async Task<IActionResult> UpdateBinning(int id ,[FromBody] BinningDto binningDto)
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
                
                var binningItemList = new List<BinningItems>();
                if (binningsItemsDto != null)
                {
                    for (int i = 0; i < binningsItemsDto.Count; i++)
                    {

                        BinningItems binningItems = _mapper.Map<BinningItems>(binningsItemsDto[i]);
                        binningItems.binningLocations = _mapper.Map<List<BinningLocation>>(binningsItemsDto[i].BinningLocations);
                        binningItemList.Add(binningItems);


                    }
                }
                binningDetail.BinningItems = binningItemList;
                binningDetail.IsBinningCompleted = true;
                binningDetails = await  _binningRepository.CreateBinning(binningDetail);
                _binningRepository.SaveAsync();

                //Updating Binning Status in Grin

                var grinNumber = binningDetail.GrinNumber;
                var grinDetails = await _grinRepository.GetGrinByGrinNo(grinNumber);
                grinDetails.IsBinningCompleted = true;
                await _grinRepository.UpdateGrin(grinDetails);
                _grinRepository.SaveAsync();

                //Updating Binning Status in IQC
                
                var iqcDetails = await _iQCConfirmationRepository.GetIqcDetailsbyGrinNo(grinNumber);
                iqcDetails.IsBinningCompleted = true;
                await _iQCConfirmationRepository.UpdateIqc(iqcDetails);
                _iQCConfirmationRepository.SaveAsync();

                // Inventory Update Code
                string grinPartId = "";

                if (binningsItemsDto != null)
                {
                    for (int i = 0; i < binningsItemsDto.Count; i++)
                    {
                        var binningLocations = binningsItemsDto[i].BinningLocations;
                        
                        int j = 0;
                        
                        foreach (var location in binningLocations)
                        {
                            if (j == 0)
                            {
                                var inventoryObjectResult = await _httpClient.GetAsync(string.Concat(_config["InventoryAPI"],
                              "GetInventoryDetailsByGrinNo?", "GrinNo=", binningDetail.GrinNumber, "&ItemNumber=",
                               binningsItemsDto[i].ItemNumber, "&ProjectNumber=", location.ProjectNumber));

                                var inventoryObjectString = await inventoryObjectResult.Content.ReadAsStringAsync();
                                dynamic inventoryObjectData = JsonConvert.DeserializeObject(inventoryObjectString);
                                dynamic inventoryObject = inventoryObjectData.data;

                                inventoryObject.balance_Quantity = location.Qty;
                                inventoryObject.warehouse = location.Warehouse;
                                inventoryObject.projectNumbers = location.ProjectNumber;
                                inventoryObject.location = location.Location;
                                inventoryObject.isStockAvailable = true;
                                var json = JsonConvert.SerializeObject(inventoryObject);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                var response = await _httpClient.PutAsync(string.Concat(_config["InventoryAPI"],
                                    "UpdateInventory?id=", inventoryObject.id), data);
                                j++;
                            }
                            else
                            {
                              
                                var grinId = binningsItemsDto[i].GrinPartId;
                                var grinPartsDetails = await _grinPartsRepository.GetGrinPartsDetailsbyGrinPartId(grinId);
                                BinningInventoryDtoPost inventoryObjectNew = new BinningInventoryDtoPost();
                                inventoryObjectNew.PartNumber = binningsItemsDto[i].ItemNumber;
                                inventoryObjectNew.MftrPartNumber = grinPartsDetails.MftrItemNumber;
                                inventoryObjectNew.Description = grinPartsDetails.ItemDescription;
                                inventoryObjectNew.ProjectNumber = location.ProjectNumber;
                                inventoryObjectNew.Balance_Quantity = location.Qty;
                                inventoryObjectNew.UOM = grinPartsDetails.UOM;
                                inventoryObjectNew.Warehouse = location.Warehouse;
                                inventoryObjectNew.Location = location.Location;
                                inventoryObjectNew.GrinNo = binningDetail.GrinNumber;
                                inventoryObjectNew.GrinPartId = grinId;
                                inventoryObjectNew.PartType = "PurchasePart"; // we have to take parttype from grinparts model;
                                inventoryObjectNew.ReferenceID = Convert.ToString(grinId);
                                inventoryObjectNew.ReferenceIDFrom = "GRIN";

                                var json = JsonConvert.SerializeObject(inventoryObjectNew);
                                var data = new StringContent(json, Encoding.UTF8, "application/json");
                                var response = await _httpClient.PostAsync(string.Concat(_config["InventoryAPI"], "CreateInventory"), data);

                            }
                        }
                    }
                }

                serviceResponse.Data = binningDetails;
                serviceResponse.Message = "Successfully Created";
                serviceResponse.Success = true;
                serviceResponse.StatusCode = HttpStatusCode.OK;
                return Created("BinningById",serviceResponse);
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
                    var grinDetailsbyGrinNo = await _grinRepository.GetGrinByGrinNo(binningGrinNo);
                    var binningDetailsDto = _mapper.Map<BinningDto>(grinDetailsbyGrinNo);

                    if (grinDetailsbyGrinNo.GrinParts .Count!=0)
                    {
                        foreach (var grinDetails in grinDetailsbyGrinNo.GrinParts)
                        {
                            BinningItemsDto binningItemDtos = _mapper.Map<BinningItemsDto>(grinDetails);

                            var binningDetails = _mapper.Map<List<BinningLocationDto>>(binningItemDtos.binningLocations);
                            binningItemDtos.binningLocations = binningDetails;
                            binningItemDtos.ReceivedQty = grinDetails.Qty;
                            binningItemDtos.AcceptedQty = grinDetails.AcceptedQty;
                            binningItemDtos.RejectedQty = grinDetails.RejectedQty;
                            binningItemList.Add(binningItemDtos);
                        }
                    }

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

    }
}